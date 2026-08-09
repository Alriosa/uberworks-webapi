-- =====================================================================================
-- FILE SUMMARY
-- What it does: Idempotent migration for databases created BEFORE the Support role and the
--               Deleted user status existed (see Common/Enums/UserRole.cs and
--               Common/Enums/UserStatus.cs). CL_STATUS has no CHECK constraint (only a
--               DEFAULT), so 'DELETED' needs no schema change to start being accepted —
--               only CL_ROLE's CHECK constraint needs to be widened to allow 'SUPPORT'.
--               Safe to run multiple times: it only drops/recreates CK_USERS_ROLE if it
--               doesn't already include SUPPORT.
-- Entities connected: User.cs
-- Tables related: TBL_USERS.CL_ROLE
-- =====================================================================================
USE [UberworksDb];
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.check_constraints
    WHERE name = 'CK_USERS_ROLE'
      AND definition LIKE '%SUPPORT%'
)
BEGIN
    ALTER TABLE [TBL_USERS] DROP CONSTRAINT [CK_USERS_ROLE];

    ALTER TABLE [TBL_USERS] WITH CHECK ADD CONSTRAINT [CK_USERS_ROLE]
        CHECK ([CL_ROLE] IN (N'MASTER_ADMIN', N'ADMIN', N'CLIENT', N'PROFESSIONAL', N'MANAGER', N'COMPANY', N'SUPPORT'));
END
GO
