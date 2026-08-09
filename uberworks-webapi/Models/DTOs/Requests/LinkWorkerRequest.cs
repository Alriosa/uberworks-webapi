// =====================================================================================
// FILE SUMMARY
// What it does: Describes the body of POST /api/professionals/link-existing — a Company (or
// one of its Managers) invites an EXISTING Professional-role account to join, searched by a
// single "teléfono, correo o username" field (see IUserRepository.FindByContactAsync) rather
// than three separate inputs, per explicit request.
// Entities connected: None directly (ProfessionalService.LinkExistingWorkerAsync resolves it)
// Tables related: None directly
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Requests;

public class LinkWorkerRequest
{
    public string Contact { get; set; } = string.Empty;
}
