using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NextMap.Extensions;

namespace NextMap.MappingRules
{
	internal static class RuleProvider
	{
		public static bool GetApplicableRule(Type sourceType, Type destinationType, out IMemberMappingRule mappingRule)
		{
			mappingRule = null;
			//first check for primitive type
			#region primitives
			if (destinationType.GetNullableType().IsPrimitive)
			{
				//first check if the types are assignable
				//if you update the rules here consider updating them for assignable value types
				if (!SimpleRule.TryApplyRule(sourceType, destinationType, out mappingRule))
					FromNullableRule.TryApplyRule(sourceType, destinationType, out mappingRule);
			}
			#endregion Primitives
			//mapping to decimal requires special handling, mapping from decimal is not supported at this moment
			#region mapping to decimal
			else if (destinationType.Equals(typeof(decimal)))
			{
				CastRule.TryApplyRule(sourceType, destinationType, out mappingRule);
			}
			#endregion mapping to decimal
			//mapping for String members
			#region String
			else if (destinationType.Equals(typeof(string)) && sourceType.Equals(typeof(string)))
			{
				SimpleRule.TryApplyRule(sourceType, destinationType, out mappingRule);
			}
			#endregion String
			//mapping enums
			#region Enums
			else if (destinationType.IsEnum && sourceType.IsEnum)
			{
				//if enum types are same apply the simple rule
				//otherwise cast to destination
				if (!SimpleRule.TryApplyRule(sourceType, destinationType, out mappingRule))
					CastRule.TryApplyRule(sourceType, destinationType, out mappingRule);
			}
			#endregion Enums
			//assignable value types
			#region assignable value types
			else if (destinationType.GetNullableType().IsValueType && sourceType.GetNullableType().IsValueType &&
				destinationType.GetNullableType().IsAssignableFrom(sourceType.GetNullableType()))
			{
				if (!SimpleRule.TryApplyRule(sourceType, destinationType, out mappingRule))
					FromNullableRule.TryApplyRule(sourceType, destinationType, out mappingRule);
			}
			#endregion same value tyeps
			//for source and destination being objects or structures
			#region object/structure
			else if ((destinationType.IsClass || destinationType.IsValueType) && (sourceType.IsClass || sourceType.IsValueType))
			{
				if (!SimpleRule.TryApplyRule(sourceType, destinationType, out mappingRule))
					if (destinationType.GetInterfaces().Contains(typeof(System.Collections.IEnumerable)) &&
						sourceType.GetInterfaces().Contains(typeof(System.Collections.IEnumerable)))
					{
						//GetElementType()

						if (destinationType.GetInterfaces().Contains(typeof(System.Collections.IDictionary)))
						{
							throw new NotImplementedException("Dictionary is not supported yet.");
						}

						EnumerableRule.TryApplyRule(sourceType, destinationType, out mappingRule);
					}
					else
					{
						//will setup the Map<> for this types, but unfortunately we can't check them at this point
						MapClassRule.TryApplyRule(sourceType, destinationType, out mappingRule);
					}
				}
			#endregion object/structure

			if (mappingRule != null) return true;
			return false;
		}
	}
}
