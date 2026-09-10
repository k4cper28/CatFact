using CatFact.Models;
using CatFact.Service;



var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<CatFactOption>(
    builder.Configuration.GetSection(CatFactOption.SectionName));

builder.Services.AddHttpClient<ICatFactService, CatFactService>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/fact", async (ICatFactService catFactService, CancellationToken ct) =>
{
    try
    {
        var result = await catFactService.FetchAndSaveFactAsync(ct);
        return result is not null
            ? Results.Ok(result)
            : Results.NotFound("Nie udało się pobrać danych.");
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: 500);
    }
})
.WithName("GetCatFact");


app.Run();
