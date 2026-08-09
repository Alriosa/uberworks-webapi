-- =====================================================================================
-- FILE SUMMARY
-- What it does: One-time, idempotent ALTER script for an EXISTING UberworksDb — adds
--               CL_PHOTO_URL to TBL_PROFESSIONALS, backing the "change my profile photo"
--               feature (POST /api/professionals/{id}/photo). CreateDatabase.sql already has
--               this column for anyone creating a brand new database — this script is only
--               for a database that already existed before this change. Safe to run more
--               than once: guarded by an IF NOT EXISTS check on sys.columns.
-- Entities related: Professional.cs
-- Tables affected: TBL_PROFESSIONALS
-- =====================================================================================

USE [UberworksDb];
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[TBL_PROFESSIONALS]') AND name = N'CL_PHOTO_URL'
)
BEGIN
    ALTER TABLE [TBL_PROFESSIONALS] ADD [CL_PHOTO_URL] NVARCHAR(255) NULL;
END;
GO
