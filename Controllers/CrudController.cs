using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagerApi.Data;
using UserManagerApi.Helpers;
using UserManagerApi.Models;

namespace UserManagerApi.Controllers;

// Общий CRUD для простых справочников.
// Изменять и удалять может только админ; чтение и создание — по флагам PublicRead / PublicCreate.
[ApiController]
[Authorize(Roles = Roles.Admin)]
public abstract class CrudController<TEntity> : ControllerBase
    where TEntity : class, IEntity
{
    protected readonly ApplicationDbContext Context;

    protected CrudController(ApplicationDbContext context)
    {
        Context = context;
    }

    protected DbSet<TEntity> Set => Context.Set<TEntity>();

    // Разрешить чтение без авторизации
    protected virtual bool PublicRead => false;

    // Разрешить создание без авторизации
    protected virtual bool PublicCreate => false;

    protected virtual string NotFoundMessage => "Запись не найдена.";

    // Сортировка списка
    protected virtual IQueryable<TEntity> ListQuery(IQueryable<TEntity> query) =>
        query.OrderBy(x => x.Id);

    // Подготовка сущности перед добавлением (даты и т.п.)
    protected virtual void BeforeCreate(TEntity entity) { }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        if (!PublicRead && !User.IsAdmin())
            return Denied();

        return Ok(await ListQuery(Set.AsNoTracking()).ToListAsync());
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> Get(int id)
    {
        if (!PublicRead && !User.IsAdmin())
            return Denied();

        var entity = await Set.FindAsync(id);

        return entity == null ? NotFound(NotFoundMessage) : Ok(entity);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create(TEntity entity)
    {
        if (!PublicCreate && !User.IsAdmin())
            return Denied();

        entity.Id = 0;
        BeforeCreate(entity);

        Set.Add(entity);
        await Context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = entity.Id }, entity);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, TEntity entity)
    {
        if (id != entity.Id)
            return BadRequest("ID не совпадают.");

        if (!await Set.AnyAsync(x => x.Id == id))
            return NotFound(NotFoundMessage);

        Context.Entry(entity).State = EntityState.Modified;

        await Context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await Set.FindAsync(id);

        if (entity == null)
            return NotFound(NotFoundMessage);

        Set.Remove(entity);

        try
        {
            await Context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict("Запись используется в других данных и не может быть удалена.");
        }

        return NoContent();
    }

    private IActionResult Denied() =>
        User.Identity?.IsAuthenticated == true ? Forbid() : Unauthorized();
}
