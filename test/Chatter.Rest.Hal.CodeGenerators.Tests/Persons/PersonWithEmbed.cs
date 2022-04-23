using System.Text.Json.Serialization;

namespace Chatter.Rest.Hal.CodeGenerators.Tests.Persons;

[HalResponse]
public partial class PersonWithEmbed : BasePerson
{
	[JsonPropertyName("_embedded")] public EmbeddedResourceCollection? Embedded { get; set; }
}