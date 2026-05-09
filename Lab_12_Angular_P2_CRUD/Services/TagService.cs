using Lab12.Models;
using Lab12.Repositories;

namespace Lab12.Services;

public class TagService : ITagService
{
    private readonly IUnitOfWork _unitOfWork;

    public TagService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<Tag>> GetAllAsync(CancellationToken cancellationToken = default)
        => (await _unitOfWork.TagRepository.GetAllAsync(cancellationToken))
            .OrderBy(t => t.Name).ToList();

    public async Task<List<Tag>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        var idSet = ids.ToHashSet();
        var all = await _unitOfWork.TagRepository.GetAllAsync(cancellationToken);
        return all.Where(t => idSet.Contains(t.Id)).ToList();
    }
}
