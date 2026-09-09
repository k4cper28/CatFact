using System.Text.Json.Serialization;

namespace CatFact.models;

public class CatFactDto
 {
    [JsonPropertyName("fact")]
    public string CatId { get; set; } = string.Empty;


    [JsonPropertyName("lenght")]
    public int Lenght { get; set; }

 }

