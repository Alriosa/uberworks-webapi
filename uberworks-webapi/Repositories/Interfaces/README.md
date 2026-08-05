# Repository Interfaces

Data-access contracts (e.g. `IUserRepository`, `IServiceRepository`) that the classes in `Repositories/` implement using `AppDbContext`.

`Services/` depend on these interfaces, not on the concrete implementations (so they can be mocked in tests).
