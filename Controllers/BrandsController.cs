using Microsoft.AspNetCore.Mvc;
using UserManagerApi.Data;
using UserManagerApi.Models;

namespace UserManagerApi.Controllers;

[Route("api/[controller]")]
public class BrandsController : CrudController<Brand>
{
    public BrandsController(ApplicationDbContext context) : base(context) { }

    protected override bool PublicRead => true;

    protected override string NotFoundMessage => "Бренд не найден.";

    protected override IQueryable<Brand> ListQuery(IQueryable<Brand> query) =>
        query.OrderBy(x => x.Name);
}
