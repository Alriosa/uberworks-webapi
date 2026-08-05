# Service Interfaces

Business logic contracts (e.g. `IServiceRequestService`, `IAuthService`) that Controllers consume via dependency injection.

Controllers should never talk directly to Repositories — they always go through a Service.
