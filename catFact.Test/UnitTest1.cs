using CatFact.Models;
using CatFact.Service;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using Xunit;

namespace CatFact.Tests;

public class CatFactServiceTests : IDisposable
{
    private readonly string _testFilePath;

    public CatFactServiceTests()
    {
        _testFilePath = $"test_facts_{Guid.NewGuid():N}.txt";
    }

    public void Dispose()
    {
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }

    [Fact]
    public async Task FetchAndSaveFactAsync_ShouldReturnFactAndAppendToFile_WhenApiReturnsSuccess()
    {
        string fakeJson = "{\"fact\":\"Cats sleep 70% of their lives.\",\"length\":32}";
        var handler = new MockHttpMessageHandler(fakeJson, HttpStatusCode.OK);
        var httpClient = new HttpClient(handler);

        var options = Options.Create(new CatFactOption
        {
            ApiUrl = "https://catfact.ninja/fact",
            FilePath = _testFilePath
        });

        var loggerMock = new Mock<ILogger<CatFactService>>();
        var service = new CatFactService(httpClient, options, loggerMock.Object);

        var result = await service.FetchAndSaveFactAsync();

        Assert.NotNull(result);
        Assert.Equal("Cats sleep 70% of their lives.", result.Fact);
        Assert.Equal(32, result.Lenght);

     
        Assert.True(File.Exists(_testFilePath));
        string fileContent = await File.ReadAllTextAsync(_testFilePath);
        Assert.Contains("Cats sleep 70% of their lives.", fileContent);
        Assert.Contains("Length: 32", fileContent);

    }


    [Fact]
    public async Task FetchAndSaveFactAsync_ShouldThrowException_WhenApiFails()
    {
       
        var handler = new MockHttpMessageHandler("Internal Server Error", HttpStatusCode.InternalServerError);
        var httpClient = new HttpClient(handler);

        var options = Options.Create(new CatFactOption
        {
            ApiUrl = "https://catfact.ninja/fact",
            FilePath = _testFilePath
        });

        var loggerMock = new Mock<ILogger<CatFactService>>();
        var service = new CatFactService(httpClient, options, loggerMock.Object);

        
        await Assert.ThrowsAnyAsync<Exception>(() => service.FetchAndSaveFactAsync());

     
        Assert.False(File.Exists(_testFilePath));
    }

}