using System;
using Sandbox;

namespace Advisor.Commands.Attributes
{
	/// <summary>
	/// This is required by s&box as it needs a LibraryAttribute in order to find all classes that derive from IArgumentConverter.
	/// You can also just use LibraryAttribute if you want.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class ArgumentConverterAttribute : LibraryAttribute
	{
	}
}
