using Newtonsoft.Json;

namespace AutoEndpoints.Cosmos.App;

public sealed record CosmosTestModel(
    [property: JsonProperty("id")] string Id,
    [property: JsonProperty("partition")] string Partition,
    int Value);