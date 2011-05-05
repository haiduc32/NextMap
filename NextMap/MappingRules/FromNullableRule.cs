using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NextMap.MappingRules
{
	internal class FromNullableRule : IMemberMappingRule
	{
		public Type SourceType { get; private set; }

		public FromNullableRule(Type sourceType)
		{
			SourceType = sourceType;
		}

		public string GenerateInlineCode(string sourceVar, string destinationVar)
		{
			return destinationVar + " = " + sourceVar + " ?? default(" + SourceType.GetGenericArguments()[0].Name + ");\r\n";
		}

		public static bool TryApplyRule(Type sourceType, Type destinationType, out IMemberMappingRule mappingRule)
		{
			mappingRule = null;

			//for primitive types
			if (destinationType.IsPrimitive && sourceType.IsGenericType &&
				destinationType.IsAssignableFrom(sourceType.GetGenericArguments()[0]))
			{
				mappingRule = new FromNullableRule(sourceType);
				return true;
			}

			//for value types
			if (destinationType.IsValueType && sourceType.IsGenericType &&
				destinationType.IsAssignableFrom(sourceType.GetGenericArguments()[0]))
			{
				mappingRule = new FromNullableRule(sourceType);
				return true;
			}

			return false;
		}
	}
}
