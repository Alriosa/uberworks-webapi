# =====================================================================================
# FILE SUMMARY
# What it does: Generates a password hash in the EXACT same format
#               Common/Helpers/PasswordHasher.Hash() produces (PBKDF2-SHA256, 100,000
#               iterations, 16-byte salt, 32-byte key, "{iterations}.{saltBase64}.{hashBase64}").
#               Use this to create the CL_PASSWORD value for a manual SQL INSERT (see
#               InsertUser.sql in this same folder) — this is the only way to set a real,
#               working password for a manually-created account (MasterAdmin/Admin), since
#               T-SQL has no built-in PBKDF2 function that matches the app's algorithm.
#               The password is read with Read-Host -AsSecureString so it's never echoed to
#               the screen or saved in your PowerShell history.
# Entities related: User.cs (produces a value for User.PasswordHash)
# Tables related: TBL_USERS.CL_PASSWORD (indirectly — this script only prints the value,
#                 it doesn't touch the database itself)
# =====================================================================================

$securePassword = Read-Host "Enter the password to hash" -AsSecureString
$bstr = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
$password = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($bstr)
[System.Runtime.InteropServices.Marshal]::ZeroFreeBSTR($bstr)

$iterations = 100000
$saltBytes = New-Object byte[] 16
$rng = New-Object System.Security.Cryptography.RNGCryptoServiceProvider
$rng.GetBytes($saltBytes)

$derive = New-Object System.Security.Cryptography.Rfc2898DeriveBytes(
    $password, $saltBytes, $iterations, [System.Security.Cryptography.HashAlgorithmName]::SHA256)
$keyBytes = $derive.GetBytes(32)

$hash = "$iterations.$([Convert]::ToBase64String($saltBytes)).$([Convert]::ToBase64String($keyBytes))"

Write-Host ""
Write-Host "Password hash (paste this as CL_PASSWORD in InsertUser.sql):"
Write-Host $hash
