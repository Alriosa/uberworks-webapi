# Repository Interfaces

Data-access contracts (e.g. `IUserRepository`, `IServiceRepository`) that the classes in `Repositories/` implement using Dapper against a plain `IDbConnection` (see `Data/IDbConnectionFactory.cs`) — hand-written SQL, no ORM/migrations.

`Services/` depend on these interfaces, not on the concrete implementations (so they can be mocked in tests).
