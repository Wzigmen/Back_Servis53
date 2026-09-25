using Microsoft.AspNetCore.Mvc;
using UserManagerApi.Data;
using UserManagerApi.Models;

namespace UserManagerApi.Controllers;

[Route("api/[controller]")]
public class CategoriesController : CrudController<Category>
{
    public CategoriesController(ApplicationDbContext context) : base(context) { }

    protected override bool PublicRead => true;

    protected override string NotFoundMessage => "Категория не найдена.";

    protected override IQueryable<Category> ListQuery(IQueryable<Category> query) =>
        query.OrderBy(x => x.Name);
}
