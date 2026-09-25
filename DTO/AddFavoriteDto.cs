using System.ComponentModel.DataAnnotations;

namespace UserManagerApi.DTO;

// Пользователь берётся из токена, а не из тела запроса
public class AddFavoriteDto
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }
}
