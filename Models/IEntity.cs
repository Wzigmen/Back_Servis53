namespace UserManagerApi.Models;

// Сущность с целочисленным Id — нужна для общего CrudController
public interface IEntity
{
    int Id { get; set; }
}
