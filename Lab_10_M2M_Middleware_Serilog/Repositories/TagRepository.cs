namespace Lab10.Repositories;

using Lab10.Data;
using Lab10.Models;

// TODO Lab 10 (Ex. 1): Implementare TagRepository
// - mosteneste Repository<Tag> (mosteneste implementarile de baza)
// - implementeaza ITagRepository
// - constructor pasare AppDbContext la base
public class TagRepository : Repository<Tag>, ITagRepository
{
    public TagRepository(AppDbContext context) : base(context) { }
}
