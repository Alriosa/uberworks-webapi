-- =====================================================================================
-- FILE SUMMARY
-- What it does: Idempotent migration that adds TBL_EVENTS and TBL_EVENT_INVITATIONS for
--               databases created BEFORE the Company Event/invitation system existed (see
--               Models/Entities/Event.cs and EventInvitation.cs). Safe to run multiple
--               times — each table is only created if it doesn't already exist. See
--               CreateDatabase.sql for the identical CREATE TABLE statements used on fresh
--               databases.
-- Entities connected: Event.cs, EventInvitation.cs
-- Tables related: TBL_EVENTS, TBL_EVENT_INVITATIONS
-- =====================================================================================
USE [UberworksDb];
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'TBL_EVENTS')
BEGIN
    CREATE TABLE [TBL_EVENTS] (
        [PK_EVENT_ID]                     INT IDENTITY(1,1) NOT NULL,
        [CL_COMPANY_USER_ID]              INT               NOT NULL,
        [CL_TITLE]                        NVARCHAR(200)     NOT NULL,
        [CL_DESCRIPTION]                  NVARCHAR(MAX)     NOT NULL,
        [CL_NOT_INCLUDED]                 NVARCHAR(MAX)     NULL,
        [CL_EVENT_DATE]                   DATETIME          NOT NULL,
        [CL_LOCATION]                     NVARCHAR(300)     NOT NULL,
        [CL_PROFESSIONALS_NEEDED_COUNT]   INT               NOT NULL,
        [CL_CREATED_AT]                   DATETIME          NOT NULL CONSTRAINT [DF_EVENTS_CREATED_AT] DEFAULT (GETDATE()),
        CONSTRAINT [PK_TBL_EVENTS] PRIMARY KEY ([PK_EVENT_ID]),
        CONSTRAINT [FK_TBL_EVENTS_TBL_USERS_CL_COMPANY_USER_ID]
            FOREIGN KEY ([CL_COMPANY_USER_ID]) REFERENCES [TBL_USERS] ([PK_USER_ID]) ON DELETE NO ACTION
    );

    CREATE INDEX [IX_TBL_EVENTS_CL_COMPANY_USER_ID] ON [TBL_EVENTS] ([CL_COMPANY_USER_ID]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'TBL_EVENT_INVITATIONS')
BEGIN
    CREATE TABLE [TBL_EVENT_INVITATIONS] (
        [PK_EVENT_INVITATION_ID]    INT IDENTITY(1,1) NOT NULL,
        [PK_EVENT_ID]                INT              NOT NULL,
        [CL_PROFESSIONAL_USER_ID]    INT              NOT NULL,
        [CL_STATUS]                  NVARCHAR(20)     NOT NULL CONSTRAINT [DF_EVENT_INVITATIONS_STATUS] DEFAULT (N'PENDING'),
        [CL_CREATED_AT]              DATETIME         NOT NULL CONSTRAINT [DF_EVENT_INVITATIONS_CREATED_AT] DEFAULT (GETDATE()),
        [CL_RESPONDED_AT]            DATETIME         NULL,
        CONSTRAINT [PK_TBL_EVENT_INVITATIONS] PRIMARY KEY ([PK_EVENT_INVITATION_ID]),
        CONSTRAINT [FK_TBL_EVENT_INVITATIONS_TBL_EVENTS_PK_EVENT_ID]
            FOREIGN KEY ([PK_EVENT_ID]) REFERENCES [TBL_EVENTS] ([PK_EVENT_ID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TBL_EVENT_INVITATIONS_TBL_USERS_CL_PROFESSIONAL_USER_ID]
            FOREIGN KEY ([CL_PROFESSIONAL_USER_ID]) REFERENCES [TBL_USERS] ([PK_USER_ID]) ON DELETE NO ACTION
    );

    CREATE INDEX [IX_TBL_EVENT_INVITATIONS_PK_EVENT_ID] ON [TBL_EVENT_INVITATIONS] ([PK_EVENT_ID]);
    CREATE INDEX [IX_TBL_EVENT_INVITATIONS_CL_PROFESSIONAL_USER_ID] ON [TBL_EVENT_INVITATIONS] ([CL_PROFESSIONAL_USER_ID]);
END
GO
