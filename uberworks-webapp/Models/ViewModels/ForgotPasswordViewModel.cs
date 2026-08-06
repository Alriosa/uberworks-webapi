// =====================================================================================
// FILE SUMMARY
// What it does: Backs the "Forgot password" form (Views/Account/ForgotPassword.cshtml).
//               Just an email — AccountController.ForgotPassword always shows the same
//               generic success message whether or not that email exists, matching the
//               API's own behavior (see uberworks-webapi's UserService.ForgotPasswordAsync).
// Entities connected: None — this project has no database entities
// Tables related: None
// =====================================================================================
using System.ComponentModel.DataAnnotations;

namespace uberworks_webapp.Models.ViewModels;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;
}
