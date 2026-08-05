# Entities

Clases POCO que mapean 1:1 a las tablas del diagrama (`TBL_USERS`, `TBL_PROFESSIONALS`, `TBL_WORKTYPES`, `TBL_SERVICES`, `TBL_SERVICE_PROFESSIONALS`, `TBL_REVIEWS`, `TBL_PAYMENTS`, `TBL_CHATS`, `TBL_PENALTIES`, `TBL_REWARDS`).

Se usan directamente por EF Core (DbContext) y NO deben exponerse en las respuestas de la API — para eso están los DTOs en `Models/DTOs`.
