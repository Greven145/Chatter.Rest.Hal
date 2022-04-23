using FluentAssertions;
using Xunit;

namespace Chatter.Rest.Hal.CodeGenerators.Tests;

public class HalResponseGeneratorTests
{
	[Fact]
	public void GeneratorAddsLinksForFileScopedNamespaces()
	{
		var person = new PersonWithFileNamespace
		{
			Name = "John Doe",
			Age = 42,
			Friends = new[] { "Tom", "Harry" }
		};
		person.Links.Should().BeNull();
		person.Embedded.Should().BeNull();
	}

	[Fact]
	public void GeneratorAddsLinksForScopedNameSpaces()
	{
		var person = new PersonWithScopedNamespace
		{
			Name = "John Doe",
			Age = 42,
			Friends = new[] { "Tom", "Harry" }
		};
		person.Links.Should().BeNull();
		person.Embedded.Should().BeNull();
	}

	[Fact]
	public void GeneratorDoesNotAddLinksIfItIsAlreadyDefined()
	{
		var person = new PersonWithLinks
		{
			Name = "John Doe",
			Age = 42,
			Friends = new[] { "Tom", "Harry" }
		};
		person.Links.Should().BeNull();
		person.Embedded.Should().BeNull();
	}

	[Fact]
	public void GeneratorDoesNotAddEmbedIfItIsAlreadyDefined()
	{
		var person = new PersonWithEmbed
		{
			Name = "John Doe",
			Age = 42,
			Friends = new[] { "Tom", "Harry" }
		};
		person.Links.Should().BeNull();
		person.Embedded.Should().BeNull();
	}
}