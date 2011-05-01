using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NextMap.Extensions;

namespace NextMap.MappingRules
{
	class EnumerableRule : IMemberMappingRule
	{
		public string SourceProperty { get; private set; }
		public string DestinationProperty { get; private set; }
		public Type SourceType { get; private set; }
		public Type DestinationType { get; private set; }

		public EnumerableRule(string sourceProperty, string destinationProperty, Type sourceType, Type destinationType)
		{
			SourceProperty = sourceProperty;
			DestinationProperty = destinationProperty;
			SourceType = sourceType;
			DestinationType = destinationType;
		}

		public string GenerateCode(string destinationObject, string sourceObject)
		{
			Type genericSourceType = SourceType.GetGenericArguments()[0];
			Type genericDestinatinoType = DestinationType.GetGenericArguments()[0];

			return string.Format("{0}.{1} = new {2}({3}.{4}.Select(x=> Mapper.Map<{5},{6}>(x)));", 
				destinationObject, DestinationProperty, DestinationType.GetCSharpName(), 
				sourceObject, SourceProperty, genericSourceType.GetCSharpName(), genericDestinatinoType.GetCSharpName());
		}
	}
}
