using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagerApi.Data;
using UserManagerApi.Models;

namespace UserManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReviewsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Получить все отзывы
    [HttpGet]
    public async Task<IActionResult> GetReviews()
    {
        var reviews = await _context.Reviews
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return Ok(reviews);
    }

    // Получить отзыв по ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetReview(int id)
    {
        var review = await _context.Reviews.FindAsync(id);

        if (review == null)
            return NotFound("Отзыв не найден.");

        return Ok(review);
    }

    // Получить отзывы товара
    [HttpGet("product/{productId}")]
    public async Task<IActionResult> GetProductReviews(int productId)
    {
        var reviews = await _context.Reviews
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return Ok(reviews);
    }

    // Получить отзывы пользователя
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserReviews(int userId)
    {
        var reviews = await _context.Reviews
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return Ok(reviews);
    }

    // Добавить отзыв
    [HttpPost]
    public async Task<IActionResult> CreateReview(Review review)
    {
        review.CreatedAt = DateTime.UtcNow;

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetReview),
            new { id = review.Id }, review);
    }

    // Изменить отзыв
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview(int id, Review review)
    {
        if (id != review.Id)
            return BadRequest();

        _context.Entry(review).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Удалить отзыв
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var review = await _context.Reviews.FindAsync(id);

        if (review == null)
            return NotFound();

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
