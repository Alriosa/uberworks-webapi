// =====================================================================================
// FILE SUMMARY
// What it does: Backs Views/Dashboard/ContactSupport.cshtml — the Client dashboard's real
//               "Contactar con Soporte" self-service view, per explicit request ("va a tener
//               título, la información como texto, como un cuadro de texto, y va a tener
//               para ingresar imágenes. Va a tener, si está asociado o no un caso y se agrega
//               el número de caso"). MyServices is the client's own request history (from
//               GET /api/services/mine, same data Client.cshtml's "Histórico de Trabajos"
//               modal uses) — populates the "¿Es sobre un trabajo en particular?" dropdown so
//               the client picks the job by its real id instead of typing a raw number blind.
// Entities connected: None — this project has no database entities
// Tables related: None
// =====================================================================================
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Models.ViewModels;

public class ContactSupportViewModel
{
    [Required(ErrorMessage = "El título es obligatorio.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Describe tu problema.")]
    public string Description { get; set; } = string.Empty;

    /// <summary>Optional — the Service/job id this case is about, picked from MyServices.</summary>
    public int? ServiceId { get; set; }

    public IFormFile? Image { get; set; }

    /// <summary>Populated on GET so the view can render the case-picker dropdown.</summary>
    public List<ServiceResponse> MyServices { get; set; } = new();
}
