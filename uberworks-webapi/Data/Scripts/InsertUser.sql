-- =====================================================================================
-- FILE SUMMARY
-- What it does: Manually inserts a user directly into TBL_USERS via SQL, bypassing the API
--               entirely. This is the ONLY supported way to create a MasterAdmin or Admin
--               account by hand (besides the automatic seed that MasterAdminSeeder.cs runs
--               once at API startup) — POST /api/users/register explicitly rejects both of
--               those roles (see Services/UserService.cs -> RegisterAsync). For CLIENT or
--               PROFESSIONAL accounts, just use the normal /api/users/register endpoint
--               instead; this script is really meant for MASTER_ADMIN/ADMIN.
--               CL_PASSWORD can't be plain text: it must be a pre-computed PBKDF2 hash in
--               the app's exact format. Run GeneratePasswordHash.ps1 (same folder) locally
--               first to get that value -- your real password is typed only into that
--               PowerShell prompt, it is never sent anywhere else.
-- Entities related: User.cs
-- Tables related: TBL_USERS
-- =====================================================================================

USE [UberworksDb];
GO

DECLARE @Username     NVARCHAR(50)  = 'REPLACE_ME';                    -- must be unique
DECLARE @FirstName    NVARCHAR(100) = 'REPLACE_ME';
DECLARE @LastName     NVARCHAR(100) = 'REPLACE_ME';
DECLARE @Email        NVARCHAR(150) = 'REPLACE_ME@example.com';        -- must be unique
DECLARE @Phone        NVARCHAR(20)  = NULL;
DECLARE @PasswordHash NVARCHAR(255) = 'PASTE_OUTPUT_OF_GeneratePasswordHash.ps1_HERE';
DECLARE @Role         NVARCHAR(50)  = 'ADMIN';                         -- MASTER_ADMIN | ADMIN | CLIENT | PROFESSIONAL

-- Basic sanity checks before inserting -- fails loudly instead of silently violating a
-- constraint with a confusing SQL Server error.
IF EXISTS (SELECT 1 FROM [TBL_USERS] WHERE [CL_EMAIL] = @Email)
BEGIN
    RAISERROR('A user with this email already exists.', 16, 1);
    RETURN;
END

IF EXISTS (SELECT 1 FROM [TBL_USERS] WHERE [CL_USERNAME] = @Username)
BEGIN
    RAISERROR('A user with this username already exists.', 16, 1);
    RETURN;
END

IF @Role NOT IN (N'MASTER_ADMIN', N'ADMIN', N'CLIENT', N'PROFESSIONAL')
BEGIN
    RAISERROR('Invalid role. Must be MASTER_ADMIN, ADMIN, CLIENT, or PROFESSIONAL.', 16, 1);
    RETURN;
END

INSERT INTO [TBL_USERS] (
    [CL_USERNAME], [CL_FIRST_NAME], [CL_LAST_NAME], [CL_EMAIL], [CL_PHONE],
    [CL_PASSWORD], [CL_ROLE], [CL_STATUS], [CL_REGISTRATION_DATE]
)
VALUES (
    @Username, @FirstName, @LastName, @Email, @Phone,
    @PasswordHash, @Role, N'ACTIVE', GETDATE()
);

SELECT SCOPE_IDENTITY() AS NewUserId;
GO
