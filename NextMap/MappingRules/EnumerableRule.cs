using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NextMap.Extensions;

namespace NextMap.MappingRules
{
	//TODO: when generating code some collection types might require referencing a different assembly or usings
	internal class EnumerableRule : IMemberMappingRule, IRelatedConfigRule
	{
		private IMemberMappingRule innerRule;

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

		#region .ctor

		protected EnumerableRule(Type sourceType, Type destinationType, IMemberMappingRule innerRule)
		{
			SourceType = sourceType;
			DestinationType = destinationType;
			this.innerRule = innerRule;
		}

		#endregion .ctor

		public string GenerateInlineCode(string sourceVar, string destinationVar)
		{
			string genericDestinationTypeName = MapDestinationType.GetCSharpName();
			string genericSourceTypeName = MapSourceType.GetCSharpName();
			string destinationTypename = DestinationType.GetCSharpName();

			string intermediaryVarName = NameGenerator.GenerateInlineVarName();
			string iteratorName = NameGenerator.GenerateInlineVarName();
			string inlineVarName = NameGenerator.GenerateInlineVarName();


			//TODO: for lists there is no need for intermediary list, optimize in the future
			string code = "System.Collections.Generic.List<" + genericDestinationTypeName + "> " + intermediaryVarName + " = new System.Collections.Generic.List<" + genericDestinationTypeName + ">();\r\n" +
				"foreach (" + genericSourceTypeName + " " + iteratorName + " in " + sourceVar + ")\r\n" +
				"{\r\n" +
				"\t" + genericDestinationTypeName + " " + inlineVarName + ";\r\n" +
				//here the inner rule should be called
				innerRule.GenerateInlineCode(iteratorName, inlineVarName) +
				//d"\t" + inlineVarName + " = Mapper.Map<" + genericSourceTypeName + "," + genericDestinationTypeName + ">(" + iteratorName + ");\r\n" +
				//here the inner rule stops
				"\t" + intermediaryVarName + ".Add(" + inlineVarName + ");\r\n" +
				"}\r\n" +
				destinationVar + " = new " + destinationTypename + "(" + intermediaryVarName + ");\r\n";

			return code;
		}

		public static bool TryApplyRule(Type sourceType, Type destinationType, out IMemberMappingRule rule)
		{
			rule = null;
			//check that it is a class and not an array as arrays should be handled by other rule
			if (!destinationType.IsClass || destinationType.IsArray || !sourceType.IsClass || sourceType.IsArray)
				return false;

			//it source and the destinations must implement IEnumerable
			if (!destinationType.GetInterfaces().Contains(typeof(System.Collections.IEnumerable)) ||
				!sourceType.GetInterfaces().Contains(typeof(System.Collections.IEnumerable)))
				return false;
			
			//and must have one generic type parameter (that will exclude dictionaries and othe special 
			//IEnumerable implementations)
			if (destinationType.GetGenericArguments().Count() != 1 || sourceType.GetGenericArguments().Count() != 1)
				return false;

			Type genericSourceType = sourceType.GetGenericArguments().Single();
			Type genericDestinationType = destinationType.GetGenericArguments().Single();

			IMemberMappingRule innerRule;
			bool canMapGenerics = RuleProvider.GetApplicableRule(genericSourceType, genericDestinationType, out innerRule);
			if (canMapGenerics)
			{
				rule = new EnumerableRule(sourceType, destinationType, innerRule);
			}

			return canMapGenerics;
		}
	}
}