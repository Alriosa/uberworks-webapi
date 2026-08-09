// =====================================================================================
// FILE SUMMARY
// What it does: The landing page after a successful login (see AccountController.Login/
//               GoogleCallback, which redirect here instead of Home/Index). [Authorize]
//               (no specific role) — LandingPage() reads the caller's own name/id/role from the
//               "access_token" JWT claim, and picks a COMPLETELY DIFFERENT view per role
//               (Views/Dashboard/Client.cshtml, Professional.cshtml, Company.cshtml,
//               Manager.cshtml, Admin.cshtml, MasterAdmin.cshtml — one file per role, same
//               reasoning as ProfileController.cs). Each dashboard shows real summary data
//               where it exists — a Company sees its real worker count, a Professional sees
//               its real average rating AND the real list of open Work Posts (GET
//               /api/services/open — not filtered by skill/category yet, since Professional.cs
//               has no WorkTypes relationship in the schema), a Client sees its real request
//               history (GET /api/services/mine) and its real profile data, a MasterAdmin
//               renders the SAME Admin.cshtml view as Role=Admin (see the UserRole.MasterAdmin
//               case below and AdminDashboardViewModel.IsMasterAdmin) — full CRUD parity plus
//               one extra "Finanzas" card — everyone else gets a welcome + quick links to what
//               they can actually do. Everything without a real backing entity yet
//               (Payment/Penalty-based numbers on the Professional dashboard) stays
//               decorative — see each view's own FILE SUMMARY for exactly what's real vs. mock.
//               UpdateProfile backs the Client dashboard's "Editar mi perfil" modal — posts
//               here, which forwards to PUT /api/users/{id} via IUsersApiClient.UpdateAsync.
//               ClientServiceProfessional is a small JSON (not view) endpoint the Client
//               dashboard's job-detail modal calls on demand to find out which professional
//               (if any) was accepted for one of the client's own Services.
//               Finances() is MasterAdmin-only — its own page (Views/Dashboard/Finances.cshtml)
//               for the "Finanzas" card, with still-decorative daily/monthly earnings charts
//               (no Payment system exists yet).
//               The Admin dashboard's 3 CRUD panels (Usuarios/Trabajos/Reportes) are REAL —
//               UpdateUserAdmin/DeleteUserAdmin, UpdateServiceAdmin/DeleteServiceAdmin, and
//               CreateReport/UpdateReportAdmin/CancelReportAdmin each round-trip to the API
//               and redirect back here, using TempData["AdminActionSuccess"/"AdminActionError"]
//               plus TempData["ReopenModal"] (which panel to re-open on error) the same way
//               ProfileController.cs's Editar modal reports back.
// Entities connected: None — this project has no database entities
// Tables related: None — reaches TBL_USERS/TBL_PROFESSIONALS/TBL_SERVICES/TBL_REPORTS only
//                 indirectly, through the API
// =====================================================================================
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using uberworks_webapp.Common.Exceptions;
using uberworks_webapp.Models.ApiContracts;
using uberworks_webapp.Models.ViewModels;
using uberworks_webapp.Services.ApiClient;

namespace uberworks_webapp.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IProfessionalsApiClient _professionalsApiClient;
    private readonly IUsersApiClient _usersApiClient;
    private readonly IServicesApiClient _servicesApiClient;
    private readonly IReportsApiClient _reportsApiClient;
    private readonly IEventsApiClient _eventsApiClient;
    private readonly IServiceProfessionalsApiClient _serviceProfessionalsApiClient;
    private readonly IPenaltiesApiClient _penaltiesApiClient;
    private readonly IChatsApiClient _chatsApiClient;
    private readonly IConfiguration _configuration;

    public DashboardController(
        IProfessionalsApiClient professionalsApiClient,
        IUsersApiClient usersApiClient,
        IServicesApiClient servicesApiClient,
        IReportsApiClient reportsApiClient,
        IEventsApiClient eventsApiClient,
        IServiceProfessionalsApiClient serviceProfessionalsApiClient,
        IPenaltiesApiClient penaltiesApiClient,
        IChatsApiClient chatsApiClient,
        IConfiguration configuration)
    {
        _reportsApiClient = reportsApiClient;
        _professionalsApiClient = professionalsApiClient;
        _usersApiClient = usersApiClient;
        _servicesApiClient = servicesApiClient;
        _configuration = configuration;
        _eventsApiClient = eventsApiClient;
        _serviceProfessionalsApiClient = serviceProfessionalsApiClient;
        _penaltiesApiClient = penaltiesApiClient;
        _chatsApiClient = chatsApiClient;
    }

    public async Task<IActionResult> LandingPage()
    {
        var accessToken = User.FindFirst("access_token")!.Value;
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var role = Enum.Parse<UserRole>(User.FindFirstValue(ClaimTypes.Role)!);
        // AppClaimsFactory.cs sets ClaimTypes.Name to the Username (not FirstName) — that's
        // what's actually available on the claims principal without an extra API call.
        var displayName = User.Identity?.Name ?? "there";

        switch (role)
        {
            case UserRole.Professional:
                var professional = await _professionalsApiClient.GetByUserIdAsync(userId);
                var openJobOffers = await _servicesApiClient.GetOpenAsync();
                var myPenalties = await _penaltiesApiClient.GetMineAsync(accessToken);
                var myCompletedJobs = await _servicesApiClient.GetMineAsProfessionalAsync(accessToken);
                return View("Professional", new ProfessionalDashboardViewModel
                {
                    DisplayName = displayName,
                    AverageRating = professional.AverageRating,
                    OpenJobOffers = openJobOffers,
                    Penalties = myPenalties,
                    CompletedJobs = myCompletedJobs
                });

            case UserRole.Company:
                var companyWorkers = await _professionalsApiClient.GetMyWorkersAsync(accessToken);
                var companyEvents = await _eventsApiClient.GetMyEventsAsync(accessToken);
                var companyUser = await _usersApiClient.GetByIdAsync(accessToken, userId);
                return View("Company", new CompanyDashboardViewModel
                {
                    DisplayName = displayName,
                    CompanyName = $"{companyUser.FirstName} {companyUser.LastName}",
                    Workers = companyWorkers,
                    Events = companyEvents
                });

            case UserRole.Manager:
                var managerWorkers = await _professionalsApiClient.GetMyWorkersAsync(accessToken);
                var managerEvents = await _eventsApiClient.GetMyEventsAsync(accessToken);
                var managerCompany = await _usersApiClient.GetMyCompanyAsync(accessToken);
                return View("Manager", new ManagerDashboardViewModel
                {
                    DisplayName = displayName,
                    CompanyName = $"{managerCompany.FirstName} {managerCompany.LastName}",
                    Workers = managerWorkers,
                    Events = managerEvents
                });

            case UserRole.Admin:
                var adminUsers = await _usersApiClient.GetAllUsersAsync(accessToken);
                var adminServices = await _servicesApiClient.GetAllForAdminAsync(accessToken);
                var adminReports = await _reportsApiClient.GetAllAsync(accessToken);
                return View("Admin", new AdminDashboardViewModel
                {
                    DisplayName = displayName,
                    Users = adminUsers,
                    Services = adminServices,
                    Reports = adminReports
                });

            case UserRole.Support:
                var supportReports = await _reportsApiClient.GetAllAsync(accessToken);
                return View("Support", new SupportDashboardViewModel { DisplayName = displayName, Reports = supportReports });

            case UserRole.MasterAdmin:
                // MasterAdmin gets the exact same real-CRUD dashboard as Admin (see
                // AdminDashboardViewModel.IsMasterAdmin) plus the one extra "Finanzas" card —
                // full parity was an explicit requirement ("el master admin tiene que ser
                // capaz de hacer todo lo que los demás pueden hacer").
                var masterAdminUsers = await _usersApiClient.GetAllUsersAsync(accessToken);
                var masterAdminServices = await _servicesApiClient.GetAllForAdminAsync(accessToken);
                var masterAdminReports = await _reportsApiClient.GetAllAsync(accessToken);
                return View("Admin", new AdminDashboardViewModel
                {
                    DisplayName = displayName,
                    Users = masterAdminUsers,
                    Services = masterAdminServices,
                    Reports = masterAdminReports,
                    IsMasterAdmin = true
                });

            default:
                var myServices = await _servicesApiClient.GetMineAsync(accessToken);
                var currentUser = await _usersApiClient.GetByIdAsync(accessToken, userId);
                return View("Client", new ClientDashboardViewModel
                {
                    DisplayName = displayName,
                    MyServices = myServices,
                    CurrentUser = currentUser,
                    EditProfile = new EditProfileViewModel
                    {
                        FirstName = currentUser.FirstName,
                        LastName = currentUser.LastName,
                        Phone = currentUser.Phone
                    }
                });
        }
    }

    /// <summary>
    /// Backs the Client dashboard's "Editar mi perfil" modal (Cancelar/Guardar). Only
    /// FirstName/LastName/Phone are editable — same restriction PUT /api/users/{id} enforces
    /// on the API side (Username/Email/Password/Role need separate flows).
    /// The parameter MUST be named "editProfile" (case-insensitive), not e.g. "model" — the
    /// form's asp-for="EditProfile.FirstName" posts fields literally named
    /// "EditProfile.FirstName", and MVC's default model binder only strips that prefix when
    /// it matches the action parameter's name. A mismatched name silently leaves every field
    /// empty (no binding error, no exception) — it just fails [Required] validation instead,
    /// which is exactly the bug this comment is here to stop from happening again.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(EditProfileViewModel editProfile)
    {
        if (!ModelState.IsValid)
        {
            TempData["EditProfileError"] = "Please fill in your first and last name.";
            return RedirectToAction(nameof(LandingPage));
        }

        var accessToken = User.FindFirst("access_token")!.Value;
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        try
        {
            await _usersApiClient.UpdateAsync(accessToken, userId, new UpdateUserRequest
            {
                FirstName = editProfile.FirstName,
                LastName = editProfile.LastName,
                Phone = editProfile.Phone
            });

            TempData["EditProfileSuccess"] = "Your profile was updated.";
        }
        catch (ApiException ex)
        {
            TempData["EditProfileError"] = ex.Message;
        }

        return RedirectToAction(nameof(LandingPage));
    }

    /// <summary>
    /// Backs the Client dashboard's "Histórico de Trabajos" detail modal — "quién lo hizo"
    /// (which professional was accepted, if any) isn't on ServiceResponse itself (a Service
    /// can have many competing proposals; only one gets Accepted), so this looks it up
    /// on-demand via GET /api/services/{serviceId}/proposals, which is Client-only and
    /// already checks that the caller owns the Service. Returns JSON instead of a view since
    /// it's called from JS (fetch) only when the client opens a specific job's detail, not
    /// eagerly for every row in the history table.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> ClientServiceProfessional(int serviceId)
    {
        var accessToken = User.FindFirst("access_token")!.Value;

        try
        {
            var proposals = await _serviceProfessionalsApiClient.GetProposalsAsync(accessToken, serviceId);
            var accepted = proposals.FirstOrDefault(p => p.Status == ServiceProfessionalStatus.Accepted);

            if (accepted is null)
            {
                return Json(new { found = false });
            }

            return Json(new
            {
                found = true,
                name = $"{accepted.ProfessionalFirstName} {accepted.ProfessionalLastName}",
                rating = accepted.ProfessionalAverageRating,
                negotiatedPrice = accepted.NegotiatedPrice
            });
        }
        catch (ApiException)
        {
            // A Service the client doesn't own, or one with no proposals table entry yet —
            // either way, the detail modal just shows "sin profesional asignado".
            return Json(new { found = false });
        }
    }

    /// <summary>
    /// The Client dashboard's real "Contactar con Soporte" view (own page, not a modal — the
    /// same reasoning as EventInvitations/Finances) — per explicit request. GET loads the
    /// client's own service history so the case-picker dropdown shows real job ids, not a
    /// blind text field.
    /// </summary>
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> ContactSupport()
    {
        var accessToken = User.FindFirst("access_token")!.Value;
        var myServices = await _servicesApiClient.GetMineAsync(accessToken);
        return View(new ContactSupportViewModel { MyServices = myServices });
    }

    [HttpPost]
    [Authorize(Roles = "Client")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ContactSupport(ContactSupportViewModel model)
    {
        var accessToken = User.FindFirst("access_token")!.Value;

        if (!ModelState.IsValid)
        {
            model.MyServices = await _servicesApiClient.GetMineAsync(accessToken);
            return View(model);
        }

        try
        {
            await _reportsApiClient.ContactSupportAsync(
                accessToken,
                model.Title,
                model.Description,
                model.ServiceId,
                model.Image is null ? null : new List<IFormFile> { model.Image });

            TempData["EditProfileSuccess"] = "Tu caso fue enviado a Soporte. Te contactaremos pronto.";
            return RedirectToAction(nameof(LandingPage));
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            model.MyServices = await _servicesApiClient.GetMineAsync(accessToken);
            return View(model);
        }
    }

    /// <summary>
    /// The real conversation view between a Client and the Professional accepted on their
    /// Service — "iniciar chat con la persona cliente, para hablar sobre el trabajo", per
    /// explicit request. Reachable from both the Professional dashboard's "Trabajos
    /// Realizados" detail and the Client dashboard's "Histórico de Trabajos" detail — either
    /// party can open it once a proposal has been accepted (see
    /// IChatService.ResolvePartiesAsync on the API side, which is what actually enforces this
    /// — the [Authorize(Roles=...)] here only checks the caller is A Client or Professional,
    /// not that they're one of THESE TWO specific people).
    /// </summary>
    [Authorize(Roles = "Client,Professional")]
    public async Task<IActionResult> Chat(int serviceId)
    {
        var accessToken = User.FindFirst("access_token")!.Value;
        var role = Enum.Parse<UserRole>(User.FindFirstValue(ClaimTypes.Role)!);

        try
        {
            var messages = await _chatsApiClient.GetConversationAsync(accessToken, serviceId);
            return View(new ChatViewModel { ServiceId = serviceId, Messages = messages, ViewerRole = role });
        }
        catch (ApiException ex)
        {
            TempData["EditProfileError"] = ex.Message;
            return RedirectToAction(nameof(LandingPage));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Client,Professional")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendChatMessage(int serviceId, string message)
    {
        var accessToken = User.FindFirst("access_token")!.Value;

        if (!string.IsNullOrWhiteSpace(message))
        {
            try
            {
                await _chatsApiClient.SendMessageAsync(accessToken, serviceId, message);
            }
            catch (ApiException ex)
            {
                TempData["EditProfileError"] = ex.Message;
            }
        }

        return RedirectToAction(nameof(Chat), new { serviceId });
    }

    // ===== Admin dashboard's "Ver Todos los Usuarios" CRUD panel =====

    /// <summary>
    /// Every field is editable here (Username/Email/Role/Status included) — per explicit
    /// request ("editarlo absolutamente todo"). Calls the dedicated admin-update endpoint,
    /// not the self-service one UpdateProfile above uses.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "MasterAdmin,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateUserAdmin(int id, string username, string firstName, string lastName, string email, string? phone, UserRole role, UserStatus status)
    {
        var accessToken = User.FindFirst("access_token")!.Value;

        try
        {
            await _usersApiClient.AdminUpdateAsync(accessToken, id, new AdminUpdateUserRequest
            {
                Username = username,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone,
                Role = role,
                Status = status
            });

            TempData["AdminActionSuccess"] = "The user was updated.";
        }
        catch (ApiException ex)
        {
            TempData["AdminActionError"] = ex.Message;
            TempData["ReopenModal"] = "usersModal";
        }

        return RedirectToAction(nameof(LandingPage));
    }

    /// <summary>Soft delete (Status=Deleted) — see IUsersApiClient.DeleteAsync.</summary>
    [HttpPost]
    [Authorize(Roles = "MasterAdmin,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUserAdmin(int id)
    {
        var accessToken = User.FindFirst("access_token")!.Value;

        try
        {
            await _usersApiClient.DeleteAsync(accessToken, id);
            TempData["AdminActionSuccess"] = "The user was deleted.";
        }
        catch (ApiException ex)
        {
            TempData["AdminActionError"] = ex.Message;
            TempData["ReopenModal"] = "usersModal";
        }

        return RedirectToAction(nameof(LandingPage));
    }

    // ===== Admin dashboard's "Ver Todos los Trabajos" CRUD panel =====
    // Create is intentionally NOT here: a "Work Post" is always client-authored (WorkTypeId +
    // exact GPS location + address), and an Admin manufacturing one on a client's behalf
    // would need to impersonate that client — Read/Update/Delete(Cancel) is the honest scope.

    [HttpPost]
    [Authorize(Roles = "MasterAdmin,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateServiceAdmin(int id, string? description, decimal? proposedPrice, ServiceStatus status, string zone)
    {
        var accessToken = User.FindFirst("access_token")!.Value;

        try
        {
            await _servicesApiClient.UpdateForAdminAsync(accessToken, id, new UpdateServiceAdminRequest
            {
                Description = description,
                ProposedPrice = proposedPrice,
                Status = status,
                Zone = zone
            });

            TempData["AdminActionSuccess"] = "The job was updated.";
        }
        catch (ApiException ex)
        {
            TempData["AdminActionError"] = ex.Message;
            TempData["ReopenModal"] = "jobsModal";
        }

        return RedirectToAction(nameof(LandingPage));
    }

    /// <summary>Soft delete (Status=Cancelled) — see IServicesApiClient.DeleteForAdminAsync.</summary>
    [HttpPost]
    [Authorize(Roles = "MasterAdmin,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteServiceAdmin(int id)
    {
        var accessToken = User.FindFirst("access_token")!.Value;

        try
        {
            await _servicesApiClient.DeleteForAdminAsync(accessToken, id);
            TempData["AdminActionSuccess"] = "The job was cancelled.";
        }
        catch (ApiException ex)
        {
            TempData["AdminActionError"] = ex.Message;
            TempData["ReopenModal"] = "jobsModal";
        }

        return RedirectToAction(nameof(LandingPage));
    }

    // ===== Admin dashboard's "Ver Todos los Reportes" CRUD panel =====

    [HttpPost]
    [Authorize(Roles = "MasterAdmin,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateReport(
        string title,
        string description,
        int? serviceId,
        int? clientUserId,
        int? professionalUserId,
        DateTime? incidentDate,
        List<IFormFile>? images)
    {
        var accessToken = User.FindFirst("access_token")!.Value;

        try
        {
            await _reportsApiClient.CreateAsync(accessToken, title, description, serviceId, clientUserId, professionalUserId, incidentDate, images);
            TempData["AdminActionSuccess"] = "The report was filed.";
        }
        catch (ApiException ex)
        {
            TempData["AdminActionError"] = ex.Message;
            TempData["ReopenModal"] = "reportsModal";
        }

        return RedirectToAction(nameof(LandingPage));
    }

    [HttpPost]
    [Authorize(Roles = "MasterAdmin,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateReportAdmin(
        int id,
        string title,
        string description,
        int? serviceId,
        int? clientUserId,
        int? professionalUserId,
        DateTime? incidentDate)
    {
        var accessToken = User.FindFirst("access_token")!.Value;

        try
        {
            await _reportsApiClient.UpdateAsync(accessToken, id, new UpdateReportRequest
            {
                Title = title,
                Description = description,
                ServiceId = serviceId,
                ClientUserId = clientUserId,
                ProfessionalUserId = professionalUserId,
                IncidentDate = incidentDate
            });

            TempData["AdminActionSuccess"] = "The report was updated.";
        }
        catch (ApiException ex)
        {
            TempData["AdminActionError"] = ex.Message;
            TempData["ReopenModal"] = "reportsModal";
        }

        return RedirectToAction(nameof(LandingPage));
    }

    /// <summary>Always requires a reason, per explicit request — see IReportsApiClient.CancelAsync.</summary>
    [HttpPost]
    [Authorize(Roles = "MasterAdmin,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelReportAdmin(int id, string reason)
    {
        var accessToken = User.FindFirst("access_token")!.Value;

        try
        {
            await _reportsApiClient.CancelAsync(accessToken, id, new CancelReportRequest { Reason = reason });
            TempData["AdminActionSuccess"] = "The report was cancelled.";
        }
        catch (ApiException ex)
        {
            TempData["AdminActionError"] = ex.Message;
            TempData["ReopenModal"] = "reportsModal";
        }

        return RedirectToAction(nameof(LandingPage));
    }

    // ===== Company dashboard's "Crear Evento" button (Company-only, never Manager) =====

    [HttpPost]
    [Authorize(Roles = "Company")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEvent(string title, string description, string? notIncluded, DateTime eventDate, string location, int professionalsNeededCount)
    {
        var accessToken = User.FindFirst("access_token")!.Value;

        try
        {
            await _eventsApiClient.CreateAsync(accessToken, new CreateEventRequest
            {
                Title = title,
                Description = description,
                NotIncluded = notIncluded,
                EventDate = eventDate,
                Location = location,
                ProfessionalsNeededCount = professionalsNeededCount
            });

            TempData["CompanyActionSuccess"] = "The event was created and your professionals were notified.";
        }
        catch (ApiException ex)
        {
            TempData["CompanyActionError"] = ex.Message;
            TempData["ReopenModal"] = "createEventModal";
        }

        return RedirectToAction(nameof(LandingPage));
    }

    // ===== Company/Manager dashboard's "Ver Todos mis Profesionales" panel =====

    [HttpPost]
    [Authorize(Roles = "Company,Manager")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LinkWorker(string contact)
    {
        var accessToken = User.FindFirst("access_token")!.Value;

        try
        {
            await _professionalsApiClient.LinkExistingAsync(accessToken, contact);
            TempData["CompanyActionSuccess"] = "The professional was added to your team.";
        }
        catch (ApiException ex)
        {
            TempData["CompanyActionError"] = ex.Message;
            TempData["ReopenModal"] = "workersModal";
        }

        return RedirectToAction(nameof(LandingPage));
    }

    [HttpPost]
    [Authorize(Roles = "Company,Manager")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UnlinkWorker(int professionalId)
    {
        var accessToken = User.FindFirst("access_token")!.Value;

        try
        {
            await _professionalsApiClient.UnlinkAsync(accessToken, professionalId);
            TempData["CompanyActionSuccess"] = "The professional was removed from your team.";
        }
        catch (ApiException ex)
        {
            TempData["CompanyActionError"] = ex.Message;
            TempData["ReopenModal"] = "workersModal";
        }

        return RedirectToAction(nameof(LandingPage));
    }

    // ===== Company dashboard's "Crear Manager" button (Company-only — a Manager must never
    // be able to create another Manager, per explicit request) =====

    [HttpPost]
    [Authorize(Roles = "Company")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateManager(string username, string firstName, string lastName, string email, string? phone)
    {
        var accessToken = User.FindFirst("access_token")!.Value;

        try
        {
            await _usersApiClient.CreateManagerAsync(accessToken, new CompanyCreateManagerRequest
            {
                Username = username,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone
            });

            TempData["CompanyActionSuccess"] = $"Manager '{username}' was created. We've emailed them a link to set their password.";
        }
        catch (ApiException ex)
        {
            TempData["CompanyActionError"] = ex.Message;
            TempData["ReopenModal"] = "createManagerModal";
        }

        return RedirectToAction(nameof(LandingPage));
    }

    // ===== Professional-facing side of Company Events — "Genera una vista de todo esa
    // parte", per explicit request: a dedicated page (not a modal) since it's reached from
    // its own link on the Professional dashboard, not from inside another panel. =====

    [Authorize(Roles = "Professional")]
    public async Task<IActionResult> EventInvitations()
    {
        var accessToken = User.FindFirst("access_token")!.Value;
        var invitations = await _eventsApiClient.GetMyInvitationsAsync(accessToken);
        return View(invitations);
    }

    [HttpPost]
    [Authorize(Roles = "Professional")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RespondToInvitation(int id, bool accept)
    {
        var accessToken = User.FindFirst("access_token")!.Value;

        try
        {
            await _eventsApiClient.RespondToInvitationAsync(accessToken, id, accept);
            TempData["EditProfileSuccess"] = accept ? "You accepted the invitation." : "You declined the invitation.";
        }
        catch (ApiException ex)
        {
            TempData["EditProfileError"] = ex.Message;
        }

        return RedirectToAction(nameof(EventInvitations));
    }

    /// <summary>
    /// Backs the Professional dashboard's job-offer detail modal — "un cuadro especial donde
    /// el profesional dictará el monto del por el cual haría ese trabajo", per explicit
    /// request. Calls the ALREADY-EXISTING POST /api/services/{serviceId}/proposals (no new
    /// API work was needed for this — see ServiceProfessionalsController.cs on the API side).
    /// negotiatedPrice is optional (a professional can propose to negotiate arrival first and
    /// name a price later), but estimatedArrivalMinutes is always required by the API.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Professional")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProposal(int serviceId, decimal? negotiatedPrice, int estimatedArrivalMinutes)
    {
        var accessToken = User.FindFirst("access_token")!.Value;

        try
        {
            await _serviceProfessionalsApiClient.CreateProposalAsync(accessToken, serviceId, new CreateServiceProfessionalRequest
            {
                NegotiatedPrice = negotiatedPrice,
                EstimatedArrivalMinutes = estimatedArrivalMinutes
            });
            TempData["EditProfileSuccess"] = "Your proposal was sent to the client.";
        }
        catch (ApiException ex)
        {
            TempData["EditProfileError"] = ex.Message;
        }

        return RedirectToAction(nameof(LandingPage));
    }

    // ===== Support dashboard =====

    /// <summary>
    /// The Support dashboard's "own view" for a single report — reached by clicking a report
    /// in any of the 3 status buckets, per explicit request ("cuando haces click en uno de
    /// ellos te abre una vista que te carga todo el reporte").
    /// </summary>
    [Authorize(Roles = "MasterAdmin,Admin,Support")]
    public async Task<IActionResult> SupportReportDetail(int id)
    {
        var accessToken = User.FindFirst("access_token")!.Value;
        var report = await _reportsApiClient.GetByIdAsync(accessToken, id);

        // Report.Images are relative paths on the API's own host (see ReportsController.Create
        // on the API side) — the view needs this to actually load them, same reasoning as
        // ProfessionalProfileViewModel.ApiBaseUrl.
        ViewData["ApiBaseUrl"] = _configuration["Api:BaseUrl"] ?? string.Empty;

        return View(report);
    }

    /// <summary>
    /// "Resolver": records the resolution message and the held-payment decision. See
    /// IReportService.ResolveAsync on the API side for the honest note on what this does and
    /// does not do yet (no real Chat/Payment system exists to actually deliver/move anything).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "MasterAdmin,Admin,Support")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResolveReport(int id, string message, ReportPaymentOutcome paymentOutcome)
    {
        var accessToken = User.FindFirst("access_token")!.Value;

        try
        {
            await _reportsApiClient.ResolveAsync(accessToken, id, new ResolveReportRequest { Message = message, PaymentOutcome = paymentOutcome });
            TempData["EditProfileSuccess"] = "The report was resolved.";
        }
        catch (ApiException ex)
        {
            TempData["EditProfileError"] = ex.Message;
        }

        return RedirectToAction(nameof(SupportReportDetail), new { id });
    }

    /// <summary>"Fallo a favor de nadie": no payment side is taken, the hold is simply lifted.</summary>
    [HttpPost]
    [Authorize(Roles = "MasterAdmin,Admin,Support")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NoFaultReport(int id)
    {
        var accessToken = User.FindFirst("access_token")!.Value;

        try
        {
            await _reportsApiClient.NoFaultAsync(accessToken, id);
            TempData["EditProfileSuccess"] = "The report was closed with no fault assigned.";
        }
        catch (ApiException ex)
        {
            TempData["EditProfileError"] = ex.Message;
        }

        return RedirectToAction(nameof(SupportReportDetail), new { id });
    }

    /// <summary>"Cancelar reporte": always requires a reason, per explicit request.</summary>
    [HttpPost]
    [Authorize(Roles = "MasterAdmin,Admin,Support")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelReportSupport(int id, string reason)
    {
        var accessToken = User.FindFirst("access_token")!.Value;

        try
        {
            await _reportsApiClient.CancelAsync(accessToken, id, new CancelReportRequest { Reason = reason });
            TempData["EditProfileSuccess"] = "The report was cancelled.";
        }
        catch (ApiException ex)
        {
            TempData["EditProfileError"] = ex.Message;
        }

        return RedirectToAction(nameof(SupportReportDetail), new { id });
    }

    /// <summary>
    /// The MasterAdmin dashboard's "Finanzas" card links here — its own page (not a modal),
    /// since a chart-heavy view deserves a real URL and its own back button. Chart data is
    /// still decorative/mock (see Views/Dashboard/Finances.cshtml) — there's no real
    /// Payment/earnings system in the app yet.
    /// </summary>
    [Authorize(Roles = "MasterAdmin")]
    public IActionResult Finances()
    {
        return View();
    }
}
