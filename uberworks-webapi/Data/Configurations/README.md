# EF Core Configurations

Una clase `IEntityTypeConfiguration<T>` por entidad (Fluent API), en vez de meter todo en `OnModelCreating` del `AppDbContext`.

Ej: `UserConfiguration.cs`, `ServiceConfiguration.cs`, etc. Aquí se definen nombres de tabla/columna, longitudes, checks, relaciones y llaves foráneas según el diagrama original.
