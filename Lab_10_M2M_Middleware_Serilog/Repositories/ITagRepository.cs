namespace Lab10.Repositories;

using Lab10.Models;

// TODO Lab 10 (Ex. 1): Interfata pentru repository-ul de Tag
// - mosteneste IRepository<Tag> (deja are GetAllAsync, GetByIdAsync, AddAsync, etc.)
// - poate ramane goala daca nu adaugati metode noi
public interface ITagRepository : IRepository<Tag>
{
}
