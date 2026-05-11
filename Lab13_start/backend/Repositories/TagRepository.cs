using Lab12.Data;
using Lab12.Models;

namespace Lab12.Repositories;

public class TagRepository : Repository<Tag>, ITagRepository
{
    public TagRepository(AppDbContext context) : base(context) { }
}
