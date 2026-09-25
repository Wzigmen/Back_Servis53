using Microsoft.AspNetCore.Mvc;
using UserManagerApi.Data;
using UserManagerApi.Models;

namespace UserManagerApi.Controllers;

[Route("api/[controller]")]
public class NewsController : CrudController<News>
{
    public NewsController(ApplicationDbContext context) : base(context) { }

    protected override bool PublicRead => true;

    protected override string NotFoundMessage => "Новость не найдена.";

    protected override IQueryable<News> ListQuery(IQueryable<News> query) =>
        query.OrderByDescending(n => n.CreatedAt);

    protected override void BeforeCreate(News entity) =>
        entity.CreatedAt = DateTime.UtcNow;
}
