using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NextMap.Extensions;

namespace NextMap.MappingRules
{
	/// <summary>
	/// Map class rule will try to map the source type to destination type using a defined mapping. Will throw
	/// an exception at runtime when mapping if the mapping was not defined. In a rule chain check should be used
	/// as the last resort.
	/// </summary>
	class MapClassRule : IMemberMappingRule
	{
		public Type SourceType { get; private set; }
		public Type DestinationType { get; private set; }

		public MapClassRule(Type sourceType, Type destinationType)
		{
			SourceType = sourceType;
			DestinationType = destinationType;
		}

		public string GenerateInlineCode(string sourceVar, string destinationvar)
		{
			return destinationvar + " = Mapper.Map<" + SourceType.GetCSharpName() + ", " + DestinationType.GetCSharpName() + ">(" + sourceVar + ");\r\n";
		}

		public static bool TryApplyRule(Type sourceType, Type destinationType, out IMemberMappingRule mappingRule)
		{
			mappingRule = null;
			if ((destinationType.IsClass || destinationType.IsValueType) && 
				(sourceType.IsClass || sourceType.IsValueType))
			{
				mappingRule = new MapClassRule(sourceType, destinationType);
				return true;
			}

			return false;
		}
	}
}
