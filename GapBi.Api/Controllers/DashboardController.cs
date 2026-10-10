using GapBi.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GapBi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _dashboardService;
    private readonly SyncService _syncService;

    public DashboardController(DashboardService dashboardService, SyncService syncService)
    {
        _dashboardService = dashboardService;
        _syncService = syncService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard()
    {
        var result = await _dashboardService.GetDashboardAsync();
        return Ok(result);
    }

    [HttpPost("sync")]
    public async Task<IActionResult> Sync()
    {
        try
        {
            await _syncService.SyncAsync();
            return Ok(new { message = "Sincronização concluída com sucesso!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Falha na sincronização", details = ex.Message });
        }
    }
}