namespace Lab10.Services;

using Lab10.Models;

// TODO Lab 10 (Ex. 1): Interfata pentru service-ul de Tag
// - GetAllAsync: pentru multi-select dropdown in Create / Edit
// - GetByIdsAsync: pentru atasare la articol pornind de la SelectedTagIds
public interface ITagService
{
    Task<List<Tag>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Tag>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
}
