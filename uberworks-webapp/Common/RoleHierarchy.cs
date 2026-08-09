// =====================================================================================
// FILE SUMMARY
// What it does: WebApp-side mirror of UserService.CreateByAdminAsync's CreatableRolesByActor
//               table in uberworks-webapi (see UserRole.cs there for the full
//               account-creation pyramid). This is UI-ONLY — it decides which options
//               AdminController.CreateUser shows in the Role dropdown so a Manager isn't
//               even offered "Admin" as a choice. The API enforces the real rule
//               independently and would reject the call either way; this just avoids
//               showing options that would fail, and a confusing error, to begin with.
// Entities connected: None
// Tables related: None
// =====================================================================================
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Common;

public static class RoleHierarchy
{
    private static readonly Dictionary<UserRole, UserRole[]> CreatableRolesByActor = new()
    {
        [UserRole.MasterAdmin] = [UserRole.Admin, UserRole.Manager, UserRole.Company, UserRole.Support, UserRole.Professional, UserRole.Client],
        [UserRole.Admin] = [UserRole.Manager, UserRole.Company, UserRole.Support, UserRole.Professional, UserRole.Client],
        [UserRole.Manager] = [UserRole.Company, UserRole.Professional, UserRole.Client]
    };

    public static UserRole[] GetCreatableRoles(UserRole actorRole) =>
        CreatableRolesByActor.TryGetValue(actorRole, out var roles) ? roles : [];

    public static string GetDisplayLabel(UserRole role) => role switch
    {
        UserRole.Admin => "Admin",
        UserRole.Manager => "Manager",
        UserRole.Company => "Company — requests and manages its own workers",
        UserRole.Client => "Client — needs services done",
        UserRole.Professional => "Professional — offers services",
        UserRole.Support => "Support — handles disputes and reports",
        _ => role.ToString()
    };
}
