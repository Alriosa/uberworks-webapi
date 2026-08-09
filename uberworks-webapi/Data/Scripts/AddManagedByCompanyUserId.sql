-- =====================================================================================
-- FILE SUMMARY
-- What it does: Idempotent migration for databases created BEFORE Manager↔Company linking
--               existed (see Models/Entities/User.cs's ManagedByCompanyUserId). Adds a
--               nullable, self-referencing FK column onto TBL_USERS itself. Safe to run
--               multiple times: it only adds the column (and its FK) if they don't already
--               exist.
-- Entities connected: User.cs
-- Tables related: TBL_USERS.CL_MANAGED_BY_COMPANY_USER_ID
-- =====================================================================================
USE [UberworksDb];
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('TBL_USERS') AND name = 'CL_MANAGED_BY_COMPANY_USER_ID'
)
BEGIN
    ALTER TABLE [TBL_USERS] ADD [CL_MANAGED_BY_COMPANY_USER_ID] INT NULL;

    ALTER TABLE [TBL_USERS] WITH CHECK ADD CONSTRAINT [FK_TBL_USERS_TBL_USERS_CL_MANAGED_BY_COMPANY_USER_ID]
        FOREIGN KEY ([CL_MANAGED_BY_COMPANY_USER_ID]) REFERENCES [TBL_USERS] ([PK_USER_ID]) ON DELETE NO ACTION;
END
GO
