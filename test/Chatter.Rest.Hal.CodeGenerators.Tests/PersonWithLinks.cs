using System.Text.Json.Serialization;

namespace Chatter.Rest.Hal.CodeGenerators.Tests;

[HalResponse]
public partial class PersonWithLinks : BasePerson
{
	[JsonPropertyName("_links")] public LinkCollection? Links { get; set; }
}