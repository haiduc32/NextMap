using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NextMap.Extensions;

namespace NextMap.MappingRules
{
	class CastRule : IMemberMappingRule
	{
		public string SourceProperty { get; private set; }
		public string DestinationProperty { get; private set; }
		public Type DestinationType { get; private set; }

		public CastRule(string sourceProperty, string destinationProperty, Type destinationType)
		{
			SourceProperty = sourceProperty;
			DestinationProperty = destinationProperty;
			DestinationType = destinationType;
		}

		public string GenerateCode(string destinationObject, string sourceObject)
		{
			return string.Format("{0}.{1} = ({2}){3}.{4};", destinationObject, DestinationProperty,
				DestinationType.GetCSharpName(), sourceObject, SourceProperty);
		}
	}
}
