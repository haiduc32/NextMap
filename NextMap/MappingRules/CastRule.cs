using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NextMap.Extensions;

namespace NextMap.MappingRules
{
	class CastRule : IMemberMappingRule
	{
		public Type DestinationType { get; private set; }

		protected CastRule(Type destinationType)
		{
			DestinationType = destinationType;
		}

		public string GenerateInlineCode(string sourceVar, string destinationVar)
		{
			return string.Format("{0} = ({1}){2};", destinationVar, DestinationType.GetCSharpName(), sourceVar);
		}

		public static bool TryApplyRule(Type sourceType, Type destinationType, out IMemberMappingRule mappingRule)
		{
			mappingRule = null;

			if ((destinationType.IsEnum && sourceType.IsEnum) ||
				(destinationType.Equals(typeof(decimal)) && sourceType.IsPrimitive))
			{
				mappingRule = new CastRule(destinationType);
				return true;
			}

			return false;
		}
	}
}
