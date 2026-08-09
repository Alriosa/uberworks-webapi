-- =====================================================================================
-- FILE SUMMARY
-- What it does: Idempotent migration for databases created BEFORE the real Chat system
--               existed (see Models/Entities/Chat.cs's SenderRole). TBL_CHATS originally had
--               no way to tell which of the two parties (the client or the professional)
--               actually sent a given message — impossible to render a real two-sided
--               conversation without that. Adds CL_SENDER_ROLE (NOT NULL, defaulted to
--               'CLIENT' only so the ALTER succeeds against a pre-existing empty table — this
--               feature was never wired up before now, so there's no real historical data to
--               get "wrong"). Safe to run multiple times: it only adds the column if it
--               doesn't already exist.
-- Entities connected: Chat.cs
-- Tables related: TBL_CHATS.CL_SENDER_ROLE
-- =====================================================================================
USE [UberworksDb];
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('TBL_CHATS') AND name = 'CL_SENDER_ROLE'
)
BEGIN
    ALTER TABLE [TBL_CHATS] ADD [CL_SENDER_ROLE] NVARCHAR(20) NOT NULL CONSTRAINT [DF_CHATS_SENDER_ROLE] DEFAULT ('CLIENT');
END
GO
