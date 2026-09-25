using Microsoft.AspNetCore.Mvc;
using UserManagerApi.Data;
using UserManagerApi.Models;

namespace UserManagerApi.Controllers;

[Route("api/[controller]")]
public class AdminLogsController : CrudController<AdminLog>
{
    public AdminLogsController(ApplicationDbContext context) : base(context) { }

    protected override IQueryable<AdminLog> ListQuery(IQueryable<AdminLog> query) =>
        query.OrderByDescending(l => l.CreatedAt);

    protected override void BeforeCreate(AdminLog entity) =>
        entity.CreatedAt = DateTime.UtcNow;
}
