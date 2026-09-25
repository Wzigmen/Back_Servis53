using Microsoft.AspNetCore.Mvc;
using UserManagerApi.Data;
using UserManagerApi.Models;

namespace UserManagerApi.Controllers;

[Route("api/[controller]")]
public class RolesController : CrudController<Role>
{
    public RolesController(ApplicationDbContext context) : base(context) { }

    protected override string NotFoundMessage => "Роль не найдена.";
}
