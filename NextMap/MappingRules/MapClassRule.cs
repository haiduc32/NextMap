using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NextMap.Extensions;

namespace NextMap.MappingRules
{
	class MapClassRule : IMemberMappingRule, IRelatedConfigRule
	{
		public string SourceProperty { get; private set; }
		public string DestinationProperty { get; private set; }
		public Type SourceType { get; private set; }
		public Type DestinationType { get; private set; }

		public Type MapDestinationType { get { return DestinationType; } }
		public Type MapSourceType { get { return SourceType; } }

		public MapClassRule(string sourceProperty, string destinationProperty, Type sourceType, Type destinationType)
		{
			SourceProperty = sourceProperty;
			DestinationProperty = destinationProperty;
			SourceType = sourceType;
			DestinationType = destinationType;
		}

		public string GenerateCode(string destinationObject, string sourceObject)
		{
			return string.Format("{0}.{1} = Mapper.Map<{2}, {3}>({4}.{5});", destinationObject, DestinationProperty,
				SourceType.GetCSharpName(), DestinationType.GetCSharpName(), sourceObject, SourceProperty);
		}


	}
}
