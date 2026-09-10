using System.Text.Json.Serialization;

namespace CatFact.Models;

public class CatFactDto
 {
    [JsonPropertyName("fact")]
    public string Fact { get; set; } = string.Empty;


    [JsonPropertyName("lenght")]
    public int Lenght { get; set; }

 }

