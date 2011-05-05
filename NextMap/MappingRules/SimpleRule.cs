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
		protected SimpleRule()
		{
		}

		public string GenerateInlineCode(string sourceVar, string destinationVar)
		{
			return string.Format("{0} = {1};\r\n", destinationVar, sourceVar);
		}

		public static bool TryApplyRule(Type sourceType, Type destinationType, out IMemberMappingRule rule)
		{
				
			rule = null;
			if (sourceType.Equals(destinationType) ||
				IsAssignableFrom(sourceType, destinationType) ||
				destinationType.IsAssignableFrom(sourceType)) //TODO: decide if this line is valid or not
			{
				rule = new SimpleRule();
				return true;
			}

			return false;
		}

		private static bool IsAssignableFrom(Type sourceType, Type destinationType)
		{
			long i = 10;
			decimal a = i;
			
			List<Type> naturalTypes = new List<Type> { typeof(byte), typeof(short), typeof(int), typeof(long) };
			List<Type> floatingTypes = new List<Type> { typeof(float), typeof(double) };

			if (naturalTypes.Contains(sourceType) && naturalTypes.Contains(destinationType))
			{
				return naturalTypes.IndexOf(destinationType) >= naturalTypes.IndexOf(sourceType);
			}
			else if (floatingTypes.Contains(destinationType) && floatingTypes.Contains(sourceType))
			{
				return floatingTypes.IndexOf(destinationType) >= floatingTypes.IndexOf(sourceType);
			}
			else if (floatingTypes.Contains(destinationType) && naturalTypes.Contains(sourceType))
			{
				return true;
			}

			return false;
		}
	}
}
