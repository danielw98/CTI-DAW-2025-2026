namespace Lab10.Services;

using Lab10.Models;
using Lab10.Repositories;

public class TagService : ITagService
{
    private readonly IUnitOfWork _unitOfWork;

    public TagService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // TODO Lab 10 (Ex. 1): Returnati toate tag-urile, sortate dupa Name
    // - apelati _unitOfWork.TagRepository.GetAllAsync(cancellationToken)
    // - sortati cu .OrderBy(t => t.Name).ToList()
    public Task<List<Tag>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    // TODO Lab 10 (Ex. 1): Returnati doar tag-urile cu Id in lista data
    // - util pentru transformare SelectedTagIds (din form) -> List<Tag> in controller
    // - var idSet = ids.ToHashSet(); var all = await ...GetAllAsync(...); return all.Where(t => idSet.Contains(t.Id)).ToList();
    public Task<List<Tag>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
