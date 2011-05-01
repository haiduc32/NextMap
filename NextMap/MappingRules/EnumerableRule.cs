using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NextMap.Extensions;

namespace NextMap.MappingRules
{
	class EnumerableRule : IMemberMappingRule, IRelatedConfigRule
	{
		public string SourceProperty { get; private set; }
		public string DestinationProperty { get; private set; }
		public Type SourceType { get; private set; }
		public Type DestinationType { get; private set; }

		public Type MapDestinationType
		{
			get { return DestinationType.GetGenericArguments()[0]; }
		}

		public Type MapSourceType
		{
			get { return SourceType.GetGenericArguments()[0]; }
		}

		public EnumerableRule(string sourceProperty, string destinationProperty, Type sourceType, Type destinationType)
		{
			SourceProperty = sourceProperty;
			DestinationProperty = destinationProperty;
			SourceType = sourceType;
			DestinationType = destinationType;
		}

		public string GenerateCode(string destinationObject, string sourceObject)
		{
			return string.Format("{0}.{1} = new {2}({3}.{4}.Select(x=> Mapper.Map<{5},{6}>(x)));", 
				destinationObject, DestinationProperty, DestinationType.GetCSharpName(),
				sourceObject, SourceProperty, MapSourceType.GetCSharpName(), MapDestinationType.GetCSharpName());
		}


	}
}
