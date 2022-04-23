using System.Text.Json.Serialization;

namespace Chatter.Rest.Hal.CodeGenerators.Tests.Persons;

[HalResponse]
public partial class PersonWithLinks : BasePerson
{
	[JsonPropertyName("_links")] public LinkCollection? Links { get; set; }
}