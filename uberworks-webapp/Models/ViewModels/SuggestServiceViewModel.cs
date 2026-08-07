// =====================================================================================
// FILE SUMMARY
// What it does: Backs the "¿No encontraste el servicio que buscabas?" modal form on
//               AllServices.cshtml. HomeController.SuggestService binds a POST of this
//               shape (multipart/form-data, because of Attachment) and forwards its fields
//               to IContactApiClient.SuggestServiceAsync, which relays them to the API's
//               POST /api/contact/suggest-service.
// Entities connected: None — WebApp has no database entities
// Tables related: None
// =====================================================================================
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace uberworks_webapp.Models.ViewModels;

public class SuggestServiceViewModel
{
    [Required(ErrorMessage = "Please enter your name.")]
    public string Name { get; set; } = string.Empty;

    public bool IsFromCompany { get; set; }

    public string? CompanyName { get; set; }

    [Required(ErrorMessage = "Please enter your email.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please describe the service you'd like to see.")]
    public string Message { get; set; } = string.Empty;

    public IFormFile? Attachment { get; set; }
}
