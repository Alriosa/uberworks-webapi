// =====================================================================================
// FILE SUMMARY
// What it does: Backs the "Create worker" form (Views/Company/CreateWorker.cshtml) with
//               model binding and validation attributes. Controllers/CompanyController.cs
//               maps this into an ApiContracts.CompanyCreateWorkerRequest before calling the
//               API. There's no Role/CompanyUserId field: the worker is always Role=Professional,
//               and it's always linked to whichever Company is currently logged in — never
//               something the form itself can choose. No Password/ConfirmPassword either —
//               the new worker gets a "set your password" email instead (see
//               ProfessionalService.CreateByCompanyAsync on the API side).
// Entities connected: None — this project has no database entities
// Tables related: None
// =====================================================================================
using System.ComponentModel.DataAnnotations;

namespace uberworks_webapp.Models.ViewModels;

public class CreateWorkerViewModel
{
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Enter a valid phone number.")]
    public string? Phone { get; set; }

    public string? Description { get; set; }
    public string? Experience { get; set; }
    public string? Availability { get; set; }
    public string? Location { get; set; }
}
