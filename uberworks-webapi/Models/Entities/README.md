# Entities

POCO classes that map 1:1 to the tables in the diagram (`TBL_USERS`, `TBL_PROFESSIONALS`, `TBL_WORKTYPES`, `TBL_SERVICES`, `TBL_SERVICE_PROFESSIONALS`, `TBL_REVIEWS`, `TBL_PAYMENTS`, `TBL_CHATS`, `TBL_PENALTIES`, `TBL_REWARDS`).

Used directly by EF Core (DbContext) and should NEVER be exposed in API responses — that's what the DTOs in `Models/DTOs` are for.
