using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NextMap.MappingRules
{
	interface IMemberMappingRule
	{
		/// <summary>
		/// Generates code for the rule. Can be also used inline in other rules.
		/// </summary>
		/// <param name="sourceVar">The name of the source variable.</param>
		/// <param name="destinationVar">The name of the destination variable.</param>
		/// <returns>The code to be used inline.</returns>
		string GenerateInlineCode(string sourceVar, string destinationVar);

		/// <summary>
		/// Gets a collection of mappings on which this mapping depends.
		/// </summary>
		/// <returns>Dictionary with key value representign the source type and the destination type.</returns>
		Dictionary<Type, Type> GetDependantMappings();
	}
}
