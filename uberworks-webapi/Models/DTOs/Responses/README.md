# Response DTOs

Objects returned to the client (webapp or mobile) in API responses (e.g. `UserResponse`, `ServiceResponse`).

Let sensitive fields (like `CL_PASSWORD`) stay hidden and shape the response without coupling it to the database schema.
