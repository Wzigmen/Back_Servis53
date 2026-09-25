using System.ComponentModel.DataAnnotations;

namespace UserManagerApi.DTO;

public class ReviewCreateDto
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    [StringLength(2000)]
    public string? Comment { get; set; }
}
