-- =====================================================================================
-- FILE SUMMARY
-- What it does: Idempotent migration that adds TBL_REPORTS for databases created BEFORE
--               the dispute/report system existed (see Models/Entities/Report.cs and
--               Common/Enums/ReportStatus.cs). Safe to run multiple times — it only creates
--               the table (and its indexes) if TBL_REPORTS doesn't already exist. See
--               CreateDatabase.sql for the identical CREATE TABLE used on fresh databases.
-- Entities connected: Report.cs
-- Tables related: TBL_REPORTS
-- =====================================================================================
USE [UberworksDb];
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'TBL_REPORTS')
BEGIN
    CREATE TABLE [TBL_REPORTS] (
        [PK_REPORT_ID]                INT IDENTITY(1,1) NOT NULL,
        [CL_TITLE]                    NVARCHAR(200)     NOT NULL,
        [CL_DESCRIPTION]              NVARCHAR(MAX)     NOT NULL,
        [PK_SERVICE_ID]               INT               NULL,
        [CL_CLIENT_USER_ID]           INT               NULL,
        [CL_PROFESSIONAL_USER_ID]     INT               NULL,
        [CL_CREATED_BY_USER_ID]       INT               NOT NULL,
        [CL_INCIDENT_DATE]            DATETIME          NULL,
        [CL_CREATED_AT]               DATETIME          NOT NULL CONSTRAINT [DF_REPORTS_CREATED_AT] DEFAULT (GETDATE()),
        [CL_STATUS]                   NVARCHAR(20)      NOT NULL CONSTRAINT [DF_REPORTS_STATUS] DEFAULT (N'OPEN'),
        [CL_IMAGES_JSON]              NVARCHAR(MAX)     NULL,
        [CL_RESOLUTION_MESSAGE]       NVARCHAR(MAX)     NULL,
        [CL_PAYMENT_OUTCOME]          NVARCHAR(30)      NULL,
        [CL_CANCELLATION_REASON]      NVARCHAR(MAX)     NULL,
        [CL_RESOLVED_BY_USER_ID]      INT               NULL,
        [CL_RESOLVED_AT]              DATETIME          NULL,
        CONSTRAINT [PK_TBL_REPORTS] PRIMARY KEY ([PK_REPORT_ID]),
        CONSTRAINT [FK_TBL_REPORTS_TBL_SERVICES_PK_SERVICE_ID]
            FOREIGN KEY ([PK_SERVICE_ID]) REFERENCES [TBL_SERVICES] ([PK_SERVICE_ID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TBL_REPORTS_TBL_USERS_CL_CLIENT_USER_ID]
            FOREIGN KEY ([CL_CLIENT_USER_ID]) REFERENCES [TBL_USERS] ([PK_USER_ID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TBL_REPORTS_TBL_USERS_CL_PROFESSIONAL_USER_ID]
            FOREIGN KEY ([CL_PROFESSIONAL_USER_ID]) REFERENCES [TBL_USERS] ([PK_USER_ID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TBL_REPORTS_TBL_USERS_CL_CREATED_BY_USER_ID]
            FOREIGN KEY ([CL_CREATED_BY_USER_ID]) REFERENCES [TBL_USERS] ([PK_USER_ID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TBL_REPORTS_TBL_USERS_CL_RESOLVED_BY_USER_ID]
            FOREIGN KEY ([CL_RESOLVED_BY_USER_ID]) REFERENCES [TBL_USERS] ([PK_USER_ID]) ON DELETE NO ACTION,
        CONSTRAINT [CK_REPORTS_STATUS] CHECK ([CL_STATUS] IN (N'OPEN', N'PENDING', N'RESOLVED', N'CANCELLED'))
    );

    CREATE INDEX [IX_TBL_REPORTS_PK_SERVICE_ID] ON [TBL_REPORTS] ([PK_SERVICE_ID]);
    CREATE INDEX [IX_TBL_REPORTS_CL_STATUS] ON [TBL_REPORTS] ([CL_STATUS]);
END
GO
