using Chatter.Rest.Hal.CodeGenerators.Tests.Persons;
using FluentAssertions;
using Xunit;

namespace Chatter.Rest.Hal.CodeGenerators.Tests;

public class HalResponseGeneratorTests
{
	public static IEnumerable<object[]> Data =>
		new List<object[]>
		{
			new object[] { typeof(PersonWithFileNamespace) },
			new object[] { typeof(PersonWithScopedNamespace) },
			new object[] { typeof(PersonWithLinks) },
			new object[] { typeof(PersonWithEmbed) },
			new object[] { typeof(PersonWithLinksAndEmbed) },
		};

	[Theory]
	[MemberData(nameof(Data))]
	public void GeneratorAddsLinksForFileScopedNamespaces(Type personType)
	{
		dynamic person = Activator.CreateInstance(personType)!;
		var nullLinks = person.Links;
		var nullEmbedded = person.Embedded;
	}
}