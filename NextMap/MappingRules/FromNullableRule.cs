using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NextMap.MappingRules
{
	internal class FromNullableRule : IMemberMappingRule
	{
		public string SourceProperty { get; private set; }
		public string DestinationProperty { get; private set; }
		public Type SourceType { get; private set; }

		public FromNullableRule(string sourceProperty, string destinationProperty, Type sourceType)
		{
			SourceProperty = sourceProperty;
			DestinationProperty = destinationProperty;
			SourceType = sourceType;
		}

		public string GenerateCode(string destinationObject, string sourceObject)
		{
			return string.Format("{0}.{1} = {2}.{3} ?? default({4});", destinationObject, DestinationProperty,
				sourceObject, SourceProperty, SourceType.GetGenericArguments()[0].Name);
		}
	}
}
