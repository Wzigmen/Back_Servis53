using Microsoft.AspNetCore.Mvc;
using UserManagerApi.Data;
using UserManagerApi.Models;

namespace UserManagerApi.Controllers;

// Сообщения с сайта: отправить может любой, читать — только админ
[Route("api/[controller]")]
public class MessagesController : CrudController<Message>
{
    public MessagesController(ApplicationDbContext context) : base(context) { }

    protected override bool PublicCreate => true;

    protected override string NotFoundMessage => "Сообщение не найдено.";

    protected override IQueryable<Message> ListQuery(IQueryable<Message> query) =>
        query.OrderByDescending(m => m.CreatedAt);

    protected override void BeforeCreate(Message entity) =>
        entity.CreatedAt = DateTime.UtcNow;
}
