using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NextMap.MappingRules
{
	/// <summary>
	/// Simple from right to left field mapping rule.
	/// </summary>
	internal class SimpleRule : IMemberMappingRule
	{
		public string SourceProperty { get; private set; }
		public string DestinationProperty { get; private set; }

		public SimpleRule(string sourceProperty, string destinationProperty)
		{
			SourceProperty = sourceProperty;
			DestinationProperty = destinationProperty;
		}

		public string GenerateCode(string destinationObject, string sourceObject)
		{
			return string.Format("{0}.{1} = {2}.{3};", destinationObject, DestinationProperty, sourceObject, SourceProperty);
		}
	}
}
