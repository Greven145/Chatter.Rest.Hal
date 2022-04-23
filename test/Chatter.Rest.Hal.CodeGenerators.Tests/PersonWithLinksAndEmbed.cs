using System.Text.Json.Serialization;

namespace Chatter.Rest.Hal.CodeGenerators.Tests;

[HalResponse]
public partial class PersonWithLinksAndEmbed : BasePerson
{
	[JsonPropertyName("_embedded")] public EmbeddedResourceCollection? Embedded { get; set; }

	[JsonPropertyName("_links")] public LinkCollection? Links { get; set; }
}