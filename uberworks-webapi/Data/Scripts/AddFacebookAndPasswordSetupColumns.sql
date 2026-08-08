-- =====================================================================================
-- FILE SUMMARY
-- What it does: One-time, idempotent ALTER script for an EXISTING UberworksDb — adds the
--               two new TBL_USERS columns that support Google/Facebook sign-in with a
--               "create your password" follow-up step: CL_FACEBOOK_ID (links a Facebook
--               account to a user) and CL_IS_PASSWORD_SET (false until the person sets a
--               real password). CreateDatabase.sql already has these columns for anyone
--               creating a brand new database — this script is only for a database that
--               already existed before this change. Safe to run more than once: each ALTER
--               is guarded by an IF NOT EXISTS check on sys.columns.
-- Entities related: User.cs
-- Tables affected: TBL_USERS
-- =====================================================================================

USE [UberworksDb];
GO

-- Required for the filtered unique index below (CREATE INDEX ... WHERE ...) — sqlcmd's
-- default session doesn't have this ON the way SSMS does, and filtered indexes refuse to
-- create without it.
SET QUOTED_IDENTIFIER ON;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[TBL_USERS]') AND name = N'CL_FACEBOOK_ID'
)
BEGIN
    ALTER TABLE [TBL_USERS] ADD [CL_FACEBOOK_ID] NVARCHAR(100) NULL;
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'[TBL_USERS]') AND name = N'IX_TBL_USERS_CL_FACEBOOK_ID'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TBL_USERS_CL_FACEBOOK_ID] ON [TBL_USERS] ([CL_FACEBOOK_ID]) WHERE [CL_FACEBOOK_ID] IS NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[TBL_USERS]') AND name = N'CL_IS_PASSWORD_SET'
)
BEGIN
    -- DEFAULT 1 so every existing row (all created through the normal Register/admin-create
    -- flow, which always sets a real password) is correctly marked as already having one.
    ALTER TABLE [TBL_USERS] ADD [CL_IS_PASSWORD_SET] BIT NOT NULL CONSTRAINT [DF_USERS_IS_PASSWORD_SET] DEFAULT (1);
END;
GO
