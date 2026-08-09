// =====================================================================================
// FILE SUMMARY
// What it does: Backs the general "Contáctanos" page (Views/Home/Contact.cshtml), linked
//               from the site footer (_LandingFooter.cshtml). HomeController.Contact binds a
//               POST of this shape (multipart/form-data, because of Image) and forwards its
//               fields to IContactApiClient.SendMessageAsync, which relays them to the API's
//               POST /api/contact/message. Distinct from SuggestServiceViewModel.cs (that one
//               is specifically "didn't find the service you needed?" on AllServices.cshtml).
// Entities connected: None — WebApp has no database entities
// Tables related: None
// =====================================================================================
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace uberworks_webapp.Models.ViewModels;

public class ContactViewModel
{
    [Required(ErrorMessage = "Please enter a title for your message.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please write your message.")]
    public string Message { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your name.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your email.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    public bool IsFromCompany { get; set; }

    public string? CompanyName { get; set; }

    public IFormFile? Image { get; set; }
}
