-- =====================================================================================
-- FILE SUMMARY
-- What it does: Standalone T-SQL script that creates the UberworksDb database from
--               scratch, with all 14 tables, columns, types, defaults, CHECK constraints,
--               foreign keys, and indexes — matching the final state produced by EF Core's
--               9 migrations (InitialCreate, AddServiceLocationAndCompletionFields,
--               UpdateUserRoles, AddUsernameToUsers, AddAuditLogging,
--               MakePenaltyTypeAndReasonRequired, MakeMoreFieldsRequired,
--               AddManagerCompanyRolesAndWorkerLink, AddPasswordResetTokens). Useful for
--               handing the schema to someone who doesn't run
--               dotnet-ef (a DBA, the webapp/mobile teams, or just to inspect the schema in
--               one place), or for restoring a clean database quickly.
--               Tables are ordered so each one is created after the tables it has a foreign
--               key to (no forward references). The 3 audit log tables (TBL_ERROR_LOGS,
--               TBL_USER_ACTION_LOGS, TBL_ADMIN_ACTION_LOGS) have no foreign keys at all —
--               they're independent, append-only tables by design (see the FILE SUMMARY on
--               each of those entities in Models/Entities for why).
--               This script also seeds the __EFMigrationsHistory table at the end, so that
--               if you point EF Core at a database created with this script, it correctly
--               thinks all 7 migrations are already applied (and won't try to re-run them
--               or complain about a mismatched schema).
-- Entities related: All 14 (User, Professional, WorkType, Service, ServiceProfessional,
--                   Review, Payment, Chat, Penalty, Reward, ErrorLog, UserActionLog,
--                   AdminActionLog, PasswordResetToken)
-- Tables created: TBL_USERS, TBL_WORKTYPES, TBL_PROFESSIONALS, TBL_SERVICES,
--                 TBL_SERVICE_PROFESSIONALS, TBL_REVIEWS, TBL_PAYMENTS, TBL_CHATS,
--                 TBL_PENALTIES, TBL_REWARDS, TBL_ERROR_LOGS, TBL_USER_ACTION_LOGS,
--                 TBL_ADMIN_ACTION_LOGS, TBL_PASSWORD_RESET_TOKENS, __EFMigrationsHistory
-- =====================================================================================

IF DB_ID(N'UberworksDb') IS NULL
BEGIN
    CREATE DATABASE [UberworksDb];
END;
GO

USE [UberworksDb];
GO

-- -------------------------------------------------------------------------------------
-- TBL_USERS — root entity of the system (Client/Professional/Admin/MasterAdmin are all
-- Users with extra data attached via TBL_PROFESSIONALS).
-- -------------------------------------------------------------------------------------
CREATE TABLE [TBL_USERS] (
    [PK_USER_ID]            INT IDENTITY(1,1) NOT NULL,
    [CL_USERNAME]           NVARCHAR(50)      NOT NULL,
    [CL_FIRST_NAME]         NVARCHAR(100)     NOT NULL,
    [CL_LAST_NAME]          NVARCHAR(100)     NOT NULL,
    [CL_EMAIL]              NVARCHAR(150)     NOT NULL,
    [CL_PHONE]              NVARCHAR(20)      NULL,
    [CL_PASSWORD]           NVARCHAR(255)     NOT NULL,
    [CL_ROLE]               NVARCHAR(50)      NOT NULL,
    [CL_STATUS]             NVARCHAR(20)      NOT NULL CONSTRAINT [DF_USERS_STATUS] DEFAULT (N'ACTIVE'),
    [CL_REGISTRATION_DATE]  DATETIME          NOT NULL CONSTRAINT [DF_USERS_REGISTRATION_DATE] DEFAULT (GETDATE()),
    -- Facebook's own user ID, saved on first Facebook sign-in to link that account to this
    -- row (see UserService.ExternalLoginAsync). Null for anyone who has never used Facebook.
    [CL_FACEBOOK_ID]        NVARCHAR(100)     NULL,
    -- False for accounts auto-created via Google/Facebook sign-in until the person sets a
    -- real password (POST /api/users/set-password) — the WebApp re-shows a "create your
    -- password" modal on every login while this stays 0, even across interrupted attempts.
    [CL_IS_PASSWORD_SET]    BIT               NOT NULL CONSTRAINT [DF_USERS_IS_PASSWORD_SET] DEFAULT (1),
    CONSTRAINT [PK_TBL_USERS] PRIMARY KEY ([PK_USER_ID]),
    CONSTRAINT [CK_USERS_ROLE] CHECK ([CL_ROLE] IN (N'MASTER_ADMIN', N'ADMIN', N'CLIENT', N'PROFESSIONAL', N'MANAGER', N'COMPANY'))
);
GO

CREATE UNIQUE INDEX [IX_TBL_USERS_CL_EMAIL] ON [TBL_USERS] ([CL_EMAIL]);
CREATE UNIQUE INDEX [IX_TBL_USERS_CL_FACEBOOK_ID] ON [TBL_USERS] ([CL_FACEBOOK_ID]) WHERE [CL_FACEBOOK_ID] IS NOT NULL;
CREATE UNIQUE INDEX [IX_TBL_USERS_CL_USERNAME] ON [TBL_USERS] ([CL_USERNAME]);
GO

-- -------------------------------------------------------------------------------------
-- TBL_WORKTYPES — admin-managed catalog of work categories (e.g. "Plumbing").
-- -------------------------------------------------------------------------------------
CREATE TABLE [TBL_WORKTYPES] (
    [PK_WORK_TYPE_ID]  INT IDENTITY(1,1) NOT NULL,
    [CL_NAME]          NVARCHAR(100)     NOT NULL,
    [CL_DESCRIPTION]   NVARCHAR(MAX)     NULL,
    [CL_INCLUDES]      NVARCHAR(MAX)     NULL,
    [CL_NOT_INCLUDES]  NVARCHAR(MAX)     NULL,
    CONSTRAINT [PK_TBL_WORKTYPES] PRIMARY KEY ([PK_WORK_TYPE_ID])
);
GO

-- -------------------------------------------------------------------------------------
-- TBL_PROFESSIONALS — 1:1 extension of TBL_USERS for users with Role = PROFESSIONAL.
-- -------------------------------------------------------------------------------------
CREATE TABLE [TBL_PROFESSIONALS] (
    [PK_PROFESSIONAL_ID]  INT IDENTITY(1,1) NOT NULL,
    [PK_USER_ID]          INT               NOT NULL,
    [CL_DESCRIPTION]      NVARCHAR(MAX)     NOT NULL,
    [CL_EXPERIENCE]       NVARCHAR(MAX)     NOT NULL,
    [CL_AVAILABILITY]     NVARCHAR(100)     NOT NULL,
    [CL_LOCATION]         NVARCHAR(200)     NOT NULL,
    [CL_AVERAGE_RATING]   DECIMAL(3,2)      NOT NULL CONSTRAINT [DF_PROFESSIONALS_AVERAGE_RATING] DEFAULT (0),
    [CL_COMPANY_USER_ID]  INT               NULL,
    CONSTRAINT [PK_TBL_PROFESSIONALS] PRIMARY KEY ([PK_PROFESSIONAL_ID]),
    CONSTRAINT [FK_TBL_PROFESSIONALS_TBL_USERS_PK_USER_ID]
        FOREIGN KEY ([PK_USER_ID]) REFERENCES [TBL_USERS] ([PK_USER_ID]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TBL_PROFESSIONALS_TBL_USERS_CL_COMPANY_USER_ID]
        FOREIGN KEY ([CL_COMPANY_USER_ID]) REFERENCES [TBL_USERS] ([PK_USER_ID]) ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX [IX_TBL_PROFESSIONALS_PK_USER_ID] ON [TBL_PROFESSIONALS] ([PK_USER_ID]);
CREATE INDEX [IX_TBL_PROFESSIONALS_CL_COMPANY_USER_ID] ON [TBL_PROFESSIONALS] ([CL_COMPANY_USER_ID]);
GO

-- -------------------------------------------------------------------------------------
-- TBL_SERVICES — the "Work Post" a client creates. ExactAddress/Latitude/Longitude are
-- private (only exposed to the owner or the accepted professional); Zone is public.
-- -------------------------------------------------------------------------------------
CREATE TABLE [TBL_SERVICES] (
    [PK_SERVICE_ID]                   INT IDENTITY(1,1) NOT NULL,
    [PK_WORK_TYPE_ID]                 INT               NOT NULL,
    [CL_CLIENT_ID]                    INT               NOT NULL,
    [CL_DESCRIPTION]                  NVARCHAR(MAX)     NULL,
    [CL_IMAGE_URL]                    NVARCHAR(255)     NULL,
    [CL_PROPOSED_PRICE]               DECIMAL(10,2)     NULL,
    [CL_STATUS]                       NVARCHAR(50)      NOT NULL CONSTRAINT [DF_SERVICES_STATUS] DEFAULT (N'PENDING'),
    [CL_REQUEST_DATE]                 DATETIME          NOT NULL CONSTRAINT [DF_SERVICES_REQUEST_DATE] DEFAULT (GETDATE()),
    [CL_LATITUDE]                     DECIMAL(9,6)      NOT NULL,
    [CL_LONGITUDE]                    DECIMAL(9,6)      NOT NULL,
    [CL_EXACT_ADDRESS]                NVARCHAR(500)     NOT NULL,
    [CL_ZONE]                         NVARCHAR(100)     NOT NULL,
    [CL_COMPLETION_PHOTO_URL]         NVARCHAR(255)     NULL,
    [CL_CLIENT_CONFIRMED_AT]          DATETIME          NULL,
    [CL_PROFESSIONAL_CONFIRMED_AT]    DATETIME          NULL,
    CONSTRAINT [PK_TBL_SERVICES] PRIMARY KEY ([PK_SERVICE_ID]),
    CONSTRAINT [FK_TBL_SERVICES_TBL_WORKTYPES_PK_WORK_TYPE_ID]
        FOREIGN KEY ([PK_WORK_TYPE_ID]) REFERENCES [TBL_WORKTYPES] ([PK_WORK_TYPE_ID]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TBL_SERVICES_TBL_USERS_CL_CLIENT_ID]
        FOREIGN KEY ([CL_CLIENT_ID]) REFERENCES [TBL_USERS] ([PK_USER_ID]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_TBL_SERVICES_PK_WORK_TYPE_ID] ON [TBL_SERVICES] ([PK_WORK_TYPE_ID]);
CREATE INDEX [IX_TBL_SERVICES_CL_CLIENT_ID] ON [TBL_SERVICES] ([CL_CLIENT_ID]);
GO

-- -------------------------------------------------------------------------------------
-- TBL_SERVICE_PROFESSIONALS — a professional's proposal/bid on a Service. Also tracks
-- the server-timestamped arrival check-in ("I'm on site" button).
-- -------------------------------------------------------------------------------------
CREATE TABLE [TBL_SERVICE_PROFESSIONALS] (
    [PK_SERVICE_PROFESSIONAL_ID]    INT IDENTITY(1,1) NOT NULL,
    [PK_PROFESSIONAL_ID]            INT               NOT NULL,
    [PK_SERVICE_ID]                 INT               NOT NULL,
    [CL_NEGOTIATED_PRICE]           DECIMAL(10,2)     NULL,
    [CL_ESTIMATED_ARRIVAL_MINUTES]  INT               NOT NULL,
    [CL_ARRIVAL_CONFIRMED_AT]       DATETIME          NULL,
    [CL_STATUS]                     NVARCHAR(50)      NOT NULL CONSTRAINT [DF_SERVICE_PROFESSIONALS_STATUS] DEFAULT (N'UNDER NEGOTIATION'),
    CONSTRAINT [PK_TBL_SERVICE_PROFESSIONALS] PRIMARY KEY ([PK_SERVICE_PROFESSIONAL_ID]),
    CONSTRAINT [FK_TBL_SERVICE_PROFESSIONALS_TBL_PROFESSIONALS_PK_PROFESSIONAL_ID]
        FOREIGN KEY ([PK_PROFESSIONAL_ID]) REFERENCES [TBL_PROFESSIONALS] ([PK_PROFESSIONAL_ID]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TBL_SERVICE_PROFESSIONALS_TBL_SERVICES_PK_SERVICE_ID]
        FOREIGN KEY ([PK_SERVICE_ID]) REFERENCES [TBL_SERVICES] ([PK_SERVICE_ID]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_TBL_SERVICE_PROFESSIONALS_PK_PROFESSIONAL_ID] ON [TBL_SERVICE_PROFESSIONALS] ([PK_PROFESSIONAL_ID]);
CREATE INDEX [IX_TBL_SERVICE_PROFESSIONALS_PK_SERVICE_ID] ON [TBL_SERVICE_PROFESSIONALS] ([PK_SERVICE_ID]);
GO

-- -------------------------------------------------------------------------------------
-- TBL_REVIEWS — mutual rating (client <-> professional) after a Service closes.
-- -------------------------------------------------------------------------------------
CREATE TABLE [TBL_REVIEWS] (
    [PK_REVIEW_ID]              INT IDENTITY(1,1) NOT NULL,
    [PK_PROFESSIONAL_ID]        INT               NOT NULL,
    [PK_SERVICE_ID]             INT               NOT NULL,
    [CL_CLIENT_ID]              INT               NOT NULL,
    [CL_CLIENT_RATING]          TINYINT           NOT NULL,
    [CL_PROFESSIONAL_RATING]    TINYINT           NOT NULL,
    [CL_COMMENT]                NVARCHAR(MAX)     NOT NULL,
    [CL_REVIEW_DATE]            DATETIME          NOT NULL CONSTRAINT [DF_REVIEWS_REVIEW_DATE] DEFAULT (GETDATE()),
    CONSTRAINT [PK_TBL_REVIEWS] PRIMARY KEY ([PK_REVIEW_ID]),
    CONSTRAINT [CK_REVIEWS_RATINGS] CHECK (
        [CL_CLIENT_RATING] BETWEEN 1 AND 5 AND [CL_PROFESSIONAL_RATING] BETWEEN 1 AND 5
    ),
    CONSTRAINT [FK_TBL_REVIEWS_TBL_PROFESSIONALS_PK_PROFESSIONAL_ID]
        FOREIGN KEY ([PK_PROFESSIONAL_ID]) REFERENCES [TBL_PROFESSIONALS] ([PK_PROFESSIONAL_ID]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TBL_REVIEWS_TBL_SERVICES_PK_SERVICE_ID]
        FOREIGN KEY ([PK_SERVICE_ID]) REFERENCES [TBL_SERVICES] ([PK_SERVICE_ID]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TBL_REVIEWS_TBL_USERS_CL_CLIENT_ID]
        FOREIGN KEY ([CL_CLIENT_ID]) REFERENCES [TBL_USERS] ([PK_USER_ID]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_TBL_REVIEWS_PK_PROFESSIONAL_ID] ON [TBL_REVIEWS] ([PK_PROFESSIONAL_ID]);
CREATE INDEX [IX_TBL_REVIEWS_PK_SERVICE_ID] ON [TBL_REVIEWS] ([PK_SERVICE_ID]);
CREATE INDEX [IX_TBL_REVIEWS_CL_CLIENT_ID] ON [TBL_REVIEWS] ([CL_CLIENT_ID]);
GO

-- -------------------------------------------------------------------------------------
-- TBL_PAYMENTS — charge associated with a Service.
-- -------------------------------------------------------------------------------------
CREATE TABLE [TBL_PAYMENTS] (
    [PK_PAYMENT_ID]     INT IDENTITY(1,1) NOT NULL,
    [PK_SERVICE_ID]     INT               NOT NULL,
    [CL_METHOD]         NVARCHAR(50)      NOT NULL,
    [CL_AMOUNT]         DECIMAL(10,2)     NOT NULL,
    [CL_STATUS]         NVARCHAR(50)      NOT NULL CONSTRAINT [DF_PAYMENTS_STATUS] DEFAULT (N'PENDING'),
    [CL_PAYMENT_DATE]   DATETIME          NOT NULL CONSTRAINT [DF_PAYMENTS_PAYMENT_DATE] DEFAULT (GETDATE()),
    CONSTRAINT [PK_TBL_PAYMENTS] PRIMARY KEY ([PK_PAYMENT_ID]),
    CONSTRAINT [CK_PAYMENTS_METHOD] CHECK ([CL_METHOD] IN (N'CREDITCARD', N'PAYPAL', N'ZELLE')),
    CONSTRAINT [FK_TBL_PAYMENTS_TBL_SERVICES_PK_SERVICE_ID]
        FOREIGN KEY ([PK_SERVICE_ID]) REFERENCES [TBL_SERVICES] ([PK_SERVICE_ID]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_TBL_PAYMENTS_PK_SERVICE_ID] ON [TBL_PAYMENTS] ([PK_SERVICE_ID]);
GO

-- -------------------------------------------------------------------------------------
-- TBL_CHATS — messages between a client and a professional.
-- -------------------------------------------------------------------------------------
CREATE TABLE [TBL_CHATS] (
    [PK_CHAT_ID]            INT IDENTITY(1,1) NOT NULL,
    [PK_PROFESSIONAL_ID]    INT               NOT NULL,
    [CL_CLIENT_ID]          INT               NOT NULL,
    [CL_MESSAGE]            NVARCHAR(MAX)     NOT NULL,
    [CL_MESSAGE_DATE]       DATETIME          NOT NULL CONSTRAINT [DF_CHATS_MESSAGE_DATE] DEFAULT (GETDATE()),
    CONSTRAINT [PK_TBL_CHATS] PRIMARY KEY ([PK_CHAT_ID]),
    CONSTRAINT [FK_TBL_CHATS_TBL_PROFESSIONALS_PK_PROFESSIONAL_ID]
        FOREIGN KEY ([PK_PROFESSIONAL_ID]) REFERENCES [TBL_PROFESSIONALS] ([PK_PROFESSIONAL_ID]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TBL_CHATS_TBL_USERS_CL_CLIENT_ID]
        FOREIGN KEY ([CL_CLIENT_ID]) REFERENCES [TBL_USERS] ([PK_USER_ID]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_TBL_CHATS_PK_PROFESSIONAL_ID] ON [TBL_CHATS] ([PK_PROFESSIONAL_ID]);
CREATE INDEX [IX_TBL_CHATS_CL_CLIENT_ID] ON [TBL_CHATS] ([CL_CLIENT_ID]);
GO

-- -------------------------------------------------------------------------------------
-- TBL_PENALTIES — sanctions applied to a user (temporary or permanent).
-- -------------------------------------------------------------------------------------
CREATE TABLE [TBL_PENALTIES] (
    [PK_PENALTY_ID]  INT IDENTITY(1,1) NOT NULL,
    [PK_USER_ID]     INT               NOT NULL,
    [CL_TYPE]        NVARCHAR(50)      NOT NULL,
    [CL_REASON]      NVARCHAR(MAX)     NOT NULL,
    [CL_START_DATE]  DATETIME          NOT NULL CONSTRAINT [DF_PENALTIES_START_DATE] DEFAULT (GETDATE()),
    [CL_END_DATE]    DATETIME          NOT NULL CONSTRAINT [DF_PENALTIES_END_DATE] DEFAULT (GETDATE()),
    CONSTRAINT [PK_TBL_PENALTIES] PRIMARY KEY ([PK_PENALTY_ID]),
    CONSTRAINT [FK_TBL_PENALTIES_TBL_USERS_PK_USER_ID]
        FOREIGN KEY ([PK_USER_ID]) REFERENCES [TBL_USERS] ([PK_USER_ID]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_TBL_PENALTIES_PK_USER_ID] ON [TBL_PENALTIES] ([PK_USER_ID]);
GO

-- -------------------------------------------------------------------------------------
-- TBL_REWARDS — a user's reward points balance (1 row per user, updated over time).
-- -------------------------------------------------------------------------------------
CREATE TABLE [TBL_REWARDS] (
    [PK_REWARD_ID]          INT IDENTITY(1,1) NOT NULL,
    [PK_USER_ID]            INT               NOT NULL,
    [CL_POINTS]             INT               NOT NULL CONSTRAINT [DF_REWARDS_POINTS] DEFAULT (0),
    [CL_LAST_UPDATE_DATE]   DATETIME          NOT NULL CONSTRAINT [DF_REWARDS_LAST_UPDATE_DATE] DEFAULT (GETDATE()),
    CONSTRAINT [PK_TBL_REWARDS] PRIMARY KEY ([PK_REWARD_ID]),
    CONSTRAINT [FK_TBL_REWARDS_TBL_USERS_PK_USER_ID]
        FOREIGN KEY ([PK_USER_ID]) REFERENCES [TBL_USERS] ([PK_USER_ID]) ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX [IX_TBL_REWARDS_PK_USER_ID] ON [TBL_REWARDS] ([PK_USER_ID]);
GO

-- -------------------------------------------------------------------------------------
-- TBL_ERROR_LOGS — one row per unhandled/unexpected error (HTTP 500) anywhere in the API,
-- written automatically by the global exception middleware. No FKs on purpose (see
-- Models/Entities/ErrorLog.cs).
-- -------------------------------------------------------------------------------------
CREATE TABLE [TBL_ERROR_LOGS] (
    [PK_ERROR_LOG_ID]    INT IDENTITY(1,1) NOT NULL,
    [CL_OCCURRED_AT]     DATETIME          NOT NULL,
    [CL_SOURCE]          NVARCHAR(20)      NOT NULL,
    [CL_USER_ID]         INT               NULL,
    [CL_USERNAME]        NVARCHAR(50)      NOT NULL,
    [CL_REQUEST_METHOD]  NVARCHAR(10)      NOT NULL,
    [CL_REQUEST_PATH]    NVARCHAR(500)     NOT NULL,
    [CL_STATUS_CODE]     INT               NOT NULL,
    [CL_EXCEPTION_TYPE]  NVARCHAR(200)     NOT NULL,
    [CL_MESSAGE]         NVARCHAR(MAX)     NOT NULL,
    [CL_STACK_TRACE]     NVARCHAR(MAX)     NULL,
    [CL_IP_ADDRESS]      NVARCHAR(45)      NULL,
    CONSTRAINT [PK_TBL_ERROR_LOGS] PRIMARY KEY ([PK_ERROR_LOG_ID])
);
GO

CREATE INDEX [IX_TBL_ERROR_LOGS_CL_OCCURRED_AT] ON [TBL_ERROR_LOGS] ([CL_OCCURRED_AT]);
GO

-- -------------------------------------------------------------------------------------
-- TBL_USER_ACTION_LOGS — one row per action a user takes on THEIR OWN account (register,
-- login success/failed, self-update). No FKs on purpose (see Models/Entities/UserActionLog.cs).
-- -------------------------------------------------------------------------------------
CREATE TABLE [TBL_USER_ACTION_LOGS] (
    [PK_USER_ACTION_LOG_ID]  INT IDENTITY(1,1) NOT NULL,
    [CL_OCCURRED_AT]         DATETIME          NOT NULL,
    [CL_SOURCE]              NVARCHAR(20)      NOT NULL,
    [CL_ACTOR_USER_ID]       INT               NULL,
    [CL_ACTOR_USERNAME]      NVARCHAR(50)      NULL,
    [CL_ACTION]              NVARCHAR(100)     NOT NULL,
    [CL_TARGET_ENTITY_TYPE]  NVARCHAR(100)     NULL,
    [CL_TARGET_ENTITY_ID]    INT               NULL,
    [CL_DETAILS]             NVARCHAR(MAX)     NULL,
    [CL_IP_ADDRESS]          NVARCHAR(45)      NULL,
    CONSTRAINT [PK_TBL_USER_ACTION_LOGS] PRIMARY KEY ([PK_USER_ACTION_LOG_ID])
);
GO

CREATE INDEX [IX_TBL_USER_ACTION_LOGS_CL_OCCURRED_AT] ON [TBL_USER_ACTION_LOGS] ([CL_OCCURRED_AT]);
CREATE INDEX [IX_TBL_USER_ACTION_LOGS_CL_ACTOR_USER_ID] ON [TBL_USER_ACTION_LOGS] ([CL_ACTOR_USER_ID]);
GO

-- -------------------------------------------------------------------------------------
-- TBL_ADMIN_ACTION_LOGS — one row per action an Admin/MasterAdmin takes on SOMEONE ELSE's
-- account or data. No FKs on purpose (see Models/Entities/AdminActionLog.cs).
-- -------------------------------------------------------------------------------------
CREATE TABLE [TBL_ADMIN_ACTION_LOGS] (
    [PK_ADMIN_ACTION_LOG_ID]  INT IDENTITY(1,1) NOT NULL,
    [CL_OCCURRED_AT]          DATETIME          NOT NULL,
    [CL_SOURCE]               NVARCHAR(20)      NOT NULL,
    [CL_ACTOR_USER_ID]        INT               NOT NULL,
    [CL_ACTOR_USERNAME]       NVARCHAR(50)      NOT NULL,
    [CL_ACTOR_ROLE]           NVARCHAR(20)      NOT NULL,
    [CL_ACTION]               NVARCHAR(100)     NOT NULL,
    [CL_TARGET_ENTITY_TYPE]   NVARCHAR(100)     NULL,
    [CL_TARGET_ENTITY_ID]     INT               NULL,
    [CL_DETAILS]              NVARCHAR(MAX)     NULL,
    [CL_IP_ADDRESS]           NVARCHAR(45)      NULL,
    CONSTRAINT [PK_TBL_ADMIN_ACTION_LOGS] PRIMARY KEY ([PK_ADMIN_ACTION_LOG_ID])
);
GO

CREATE INDEX [IX_TBL_ADMIN_ACTION_LOGS_CL_OCCURRED_AT] ON [TBL_ADMIN_ACTION_LOGS] ([CL_OCCURRED_AT]);
CREATE INDEX [IX_TBL_ADMIN_ACTION_LOGS_CL_ACTOR_USER_ID] ON [TBL_ADMIN_ACTION_LOGS] ([CL_ACTOR_USER_ID]);
GO

-- -------------------------------------------------------------------------------------
-- TBL_PASSWORD_RESET_TOKENS — one row per "forgot password" request. Only the SHA256 hash
-- of the token is stored (CL_TOKEN_HASH), never the raw value that goes in the email link.
-- -------------------------------------------------------------------------------------
CREATE TABLE [TBL_PASSWORD_RESET_TOKENS] (
    [PK_TOKEN_ID]     INT IDENTITY(1,1) NOT NULL,
    [PK_USER_ID]      INT               NOT NULL,
    [CL_TOKEN_HASH]   NVARCHAR(100)     NOT NULL,
    [CL_EXPIRES_AT]   DATETIME          NOT NULL,
    [CL_USED]         BIT               NOT NULL CONSTRAINT [DF_PASSWORD_RESET_TOKENS_USED] DEFAULT (0),
    [CL_CREATED_AT]   DATETIME          NOT NULL CONSTRAINT [DF_PASSWORD_RESET_TOKENS_CREATED_AT] DEFAULT (GETDATE()),
    CONSTRAINT [PK_TBL_PASSWORD_RESET_TOKENS] PRIMARY KEY ([PK_TOKEN_ID]),
    CONSTRAINT [FK_TBL_PASSWORD_RESET_TOKENS_TBL_USERS_PK_USER_ID]
        FOREIGN KEY ([PK_USER_ID]) REFERENCES [TBL_USERS] ([PK_USER_ID]) ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX [IX_TBL_PASSWORD_RESET_TOKENS_CL_TOKEN_HASH] ON [TBL_PASSWORD_RESET_TOKENS] ([CL_TOKEN_HASH]);
CREATE INDEX [IX_TBL_PASSWORD_RESET_TOKENS_PK_USER_ID] ON [TBL_PASSWORD_RESET_TOKENS] ([PK_USER_ID]);
GO

-- -------------------------------------------------------------------------------------
-- __EFMigrationsHistory — bookkeeping table EF Core uses to know which migrations have
-- already run. Seeded here so that if you later point the API (EF Core) at a database
-- created with this script, it recognizes the schema as already up to date instead of
-- trying to re-apply migrations 1-3 on top of it.
-- -------------------------------------------------------------------------------------
CREATE TABLE [__EFMigrationsHistory] (
    [MigrationId]     NVARCHAR(150) NOT NULL,
    [ProductVersion]  NVARCHAR(32)  NOT NULL,
    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES
    (N'20260805171850_InitialCreate', N'10.0.10'),
    (N'20260805180233_AddServiceLocationAndCompletionFields', N'10.0.10'),
    (N'20260805181252_UpdateUserRoles', N'10.0.10'),
    (N'20260805192805_AddUsernameToUsers', N'10.0.10'),
    (N'20260805204333_AddAuditLogging', N'10.0.10'),
    (N'20260805210751_MakePenaltyTypeAndReasonRequired', N'10.0.10'),
    (N'20260806153730_MakeMoreFieldsRequired', N'10.0.10'),
    (N'20260806223658_AddManagerCompanyRolesAndWorkerLink', N'10.0.10'),
    (N'20260806230708_AddPasswordResetTokens', N'10.0.10');
GO
