# Enums

C# enums representing the `CHECK` constraints and default values from the diagram:

- `UserRole`: MasterAdmin, Admin, Client, Professional
- `UserStatus`: Active, Suspended, Penalized
- `ServiceStatus`: Pending, Accepted, Cancelled, Completed
- `ServiceProfessionalStatus`: UnderNegotiation, Accepted, Rejected, InProgress, Completed
- `PaymentMethod`: CreditCard, PayPal, Zelle
- `PaymentStatus`: Pending, Held, Released
- `PenaltyType`: Temporary, Permanent
- `LogSource`: Direct, WebApp, MobileApp (which client made the call, for audit logging)
