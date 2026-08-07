# Scripts

Standalone SQL/PowerShell scripts. The API uses Dapper, not an ORM with migrations — there
is no `dotnet ef` equivalent — so **`CreateDatabase.sql` is the single source of truth for
the whole schema**. Any schema change (new column, new table, new constraint) has to be made
by hand in two places: an `ALTER TABLE`/`CREATE TABLE` statement run against the real
database, AND the matching change in this file, kept in sync manually.

- **`CreateDatabase.sql`** builds the whole `UberworksDb` database from scratch (all 14 tables, columns, constraints, indexes) in one file — useful for sharing the schema with someone else, or for quickly restoring a clean database.
- **`GeneratePasswordHash.ps1`** generates a password hash in the exact format the app uses (PBKDF2-SHA256, matching `Common/Helpers/PasswordHasher.cs`). Run it locally, paste the output into `InsertUser.sql`. The password is typed into a secure prompt and never leaves your machine.
- **`InsertUser.sql`** manually inserts a user into `TBL_USERS` via SQL. This is the intended way to create `MASTER_ADMIN`/`ADMIN` accounts by hand (besides the automatic seed in `Data/Seed/MasterAdminSeeder.cs`), since the public `/api/users/register` endpoint rejects both of those roles.
