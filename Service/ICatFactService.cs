using CatFact.models;

namespace CatFact;

public interface ICatFactService
{
    Task<CatFactDto?> FetchAndSaveFactAsync(CancellationToken cancellationToken = default);
}

