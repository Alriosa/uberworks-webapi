# Service Interfaces

Contratos de la lógica de negocio (ej. `IServiceRequestService`, `IAuthService`) que los Controllers consumen vía inyección de dependencias.

Los Controllers nunca deberían hablar directo con los Repositories — siempre pasan por un Service.
