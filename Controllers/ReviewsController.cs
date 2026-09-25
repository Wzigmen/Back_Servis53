using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagerApi.Data;
using UserManagerApi.DTO;
using UserManagerApi.Helpers;
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

    [HttpGet]
    public async Task<IActionResult> GetReviews()
    {
        var reviews = await _context.Reviews
            .AsNoTracking()
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return Ok(reviews);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetReview(int id)
    {
        var review = await _context.Reviews.FindAsync(id);

        return review == null ? NotFound("Отзыв не найден.") : Ok(review);
    }

    [HttpGet("product/{productId:int}")]
    public async Task<IActionResult> GetProductReviews(int productId)
    {
        var reviews = await _context.Reviews
            .AsNoTracking()
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return Ok(reviews);
    }

    // Отзывы текущего пользователя
    [HttpGet("my")]
    [Authorize]
    public async Task<IActionResult> GetMyReviews()
    {
        var userId = User.GetUserId();

        var reviews = await _context.Reviews
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return Ok(reviews);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateReview(ReviewCreateDto dto)
    {
        if (!await _context.Products.AnyAsync(p => p.Id == dto.ProductId))
            return NotFound("Товар не найден.");

        var review = new Review
        {
            UserId = User.GetUserId(),
            ProductId = dto.ProductId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            CreatedAt = DateTime.UtcNow
        };

        _context.Reviews.Add(review);

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
    }

    // Изменить отзыв может автор или админ
    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateReview(int id, ReviewCreateDto dto)
    {
        var review = await _context.Reviews.FindAsync(id);

        if (review == null || !CanEdit(review))
            return NotFound("Отзыв не найден.");

        review.Rating = dto.Rating;
        review.Comment = dto.Comment;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var review = await _context.Reviews.FindAsync(id);

        if (review == null || !CanEdit(review))
            return NotFound("Отзыв не найден.");

        _context.Reviews.Remove(review);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CanEdit(Review review) =>
        User.IsAdmin() || review.UserId == User.GetUserId();
}
