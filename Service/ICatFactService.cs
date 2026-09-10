using CatFact.Models;

namespace CatFact.Service;

public interface ICatFactService
{
    Task<CatFactDto?> FetchAndSaveFactAsync(CancellationToken cancellationToken = default);
}

