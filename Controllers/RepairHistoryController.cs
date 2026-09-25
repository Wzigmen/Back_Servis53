using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagerApi.Data;
using UserManagerApi.Models;

namespace UserManagerApi.Controllers;

[Route("api/[controller]")]
public class RepairHistoryController : CrudController<RepairHistory>
{
    public RepairHistoryController(ApplicationDbContext context) : base(context) { }

    protected override IQueryable<RepairHistory> ListQuery(IQueryable<RepairHistory> query) =>
        query.OrderByDescending(h => h.ChangedAt);

    protected override void BeforeCreate(RepairHistory entity) =>
        entity.ChangedAt = DateTime.UtcNow;

    // История конкретного ремонта
    [HttpGet("repair/{repairId:int}")]
    public async Task<IActionResult> GetRepairHistory(int repairId)
    {
        var history = await Set
            .AsNoTracking()
            .Where(h => h.RepairId == repairId)
            .OrderBy(h => h.ChangedAt)
            .ToListAsync();

        return Ok(history);
    }
}
