using Lab11.Data;
using Lab11.Models;

namespace Lab11.Repositories;

public class TagRepository : Repository<Tag>, ITagRepository
{
    public TagRepository(AppDbContext context) : base(context) { }
}
