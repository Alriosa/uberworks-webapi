# Repository Interfaces

Contratos de acceso a datos (ej. `IUserRepository`, `IServiceRepository`) que las clases en `Repositories/` implementan usando el `AppDbContext`.

Los `Services/` dependen de estas interfaces, no de las implementaciones concretas (para poder mockear en tests).
