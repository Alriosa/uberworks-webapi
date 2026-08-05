# Scripts

Standalone SQL/PowerShell scripts that aren't part of the EF Core migration pipeline.

- **`CreateDatabase.sql`** builds the whole `UberworksDb` database from scratch (all 13 tables, columns, constraints, indexes) in one file — useful for sharing the schema with someone who doesn't run `dotnet-ef`, or for quickly restoring a clean database. It's kept in sync with the EF Core migrations; if you add a new migration, update this file to match.
- **`GeneratePasswordHash.ps1`** generates a password hash in the exact format the app uses (PBKDF2-SHA256, matching `Common/Helpers/PasswordHasher.cs`). Run it locally, paste the output into `InsertUser.sql`. The password is typed into a secure prompt and never leaves your machine.
- **`InsertUser.sql`** manually inserts a user into `TBL_USERS` via SQL. This is the intended way to create `MASTER_ADMIN`/`ADMIN` accounts by hand (besides the automatic seed in `Data/Seed/MasterAdminSeeder.cs`), since the public `/api/users/register` endpoint rejects both of those roles.
