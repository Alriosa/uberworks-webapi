// =====================================================================================
// FILE SUMMARY
// What it does: Backs the "Register users" admin form (Views/Admin/CreateUser.cshtml) with
//               model binding and validation attributes. Controllers/AdminController.cs maps
//               this into an ApiContracts.AdminCreateUserRequest before calling the API.
//               Unlike RegisterViewModel.cs, Role here also offers Admin — MasterAdmin still
//               never appears (there's only ever one, seeded on API startup). No
//               Password/ConfirmPassword fields — nobody but the new account's real owner
//               should ever know their own password, so the API creates the account and
//               emails them a "set your password" link instead (see
//               AdminController.CreateUser's success message).
// Entities connected: None — this project has no database entities
// Tables related: None
// =====================================================================================
using System.ComponentModel.DataAnnotations;
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Models.ViewModels;

public class AdminCreateUserViewModel
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

    [Required(ErrorMessage = "Select an account type.")]
    public UserRole Role { get; set; } = UserRole.Client;
}
