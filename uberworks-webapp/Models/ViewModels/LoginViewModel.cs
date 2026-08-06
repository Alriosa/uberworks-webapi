// =====================================================================================
// FILE SUMMARY
// What it does: Backs the Login form (Views/Account/Login.cshtml) with model binding and
//               validation attributes. Controllers/AccountController.cs maps this into an
//               ApiContracts.LoginRequest before calling the API — this class only exists
//               for the Razor form, it's never sent over HTTP as-is.
// Entities connected: None — this project has no database entities
// Tables related: None
// =====================================================================================
using System.ComponentModel.DataAnnotations;

namespace uberworks_webapp.Models.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
