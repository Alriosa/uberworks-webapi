// =====================================================================================
// FILE SUMMARY
// What it does: Backs the "create your password" modal (_SetPasswordModal.cshtml), shown to
//               anyone whose account was created via Google/Facebook and hasn't set a real
//               password yet (see AppClaimsFactory.cs's "requires_password_setup" claim).
//               Posts to AccountController.SetPassword, which is [Authorize]'d — the user's
//               own id comes from their JWT, never from this form.
// Entities connected: None — this project has no database entities
// Tables related: None
// =====================================================================================
using System.ComponentModel.DataAnnotations;

namespace uberworks_webapp.Models.ViewModels;

public class SetPasswordViewModel
{
    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please confirm your password.")]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
