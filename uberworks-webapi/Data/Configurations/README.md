# EF Core Configurations

One `IEntityTypeConfiguration<T>` class per entity (Fluent API), instead of cramming everything into `AppDbContext`'s `OnModelCreating`.

E.g. `UserConfiguration.cs`, `ServiceConfiguration.cs`, etc. This is where table/column names, lengths, checks, relationships, and foreign keys are defined, matching the original diagram.
