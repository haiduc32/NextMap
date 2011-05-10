using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using NextMap.MappingRules;
using NextMap.Extensions;

namespace NextMap
{
	/// <summary>
	/// class for working with mapping configurations.
	/// </summary>
	/// <typeparam name="TSource">The source type from where the mapping is done.</typeparam>
	/// <typeparam name="TDestination">The destination type to be mapped to.</typeparam>
	internal class MappingConfiguration<TSource, TDestination> : IMappingConfiguration
	{
		#region private fields

		private Dictionary<string, MemberMap> mappingDict = new Dictionary<string, MemberMap>();

		/// <summary>
		/// Dictionary for keeping the reflected information for all destination fields.
		/// </summary>
		private Dictionary<string, MemberInfo> fieldInfoDict = new Dictionary<string, MemberInfo>();

		private Type sourceType;
		private Type destinationType;

		#endregion private fields

		#region public properties

		public Type SourceType
		{
			get { return sourceType; }
		}

		public Type DestinationType
		{
			get { return destinationType; }
		}

		public Dictionary<string, MemberMap> Mappings
		{
			get { return mappingDict; }
		}

		#endregion public properties

		#region .ctor

		internal MappingConfiguration()
		{
			sourceType = typeof(TSource);
			destinationType = typeof(TDestination);

			ReflectDestination();

			MapByConvention();
		}

		#endregion .ctor

		#region public methods

		public void MapFrom(string memberName, string sourceName, MemberInfo sourceInfo)
		{
			MemberMap memberMap;
			if (!TryConfigureMemberMapping(sourceInfo, fieldInfoDict[memberName], out memberMap))
			{
				throw new MappingException(string.Format("No mapping could be defined for member {0} from {1} to {2}.", 
					memberName, GetMemberType(fieldInfoDict[memberName]).GetCSharpName(), 
					GetMemberType(sourceInfo).GetCSharpName()));
			}

			mappingDict[memberMap.DestinationMemberName] = memberMap;
		}

		/// <summary>
		/// Ignores the destination member from the mapping configuration.
		/// </summary>
		/// <param name="memberName">The name of the destination member.</param>
		/// <param name="ignoreOnCopy">Is only valid for members that have been configured for mapping by a
		/// previous rule or by convention. Will map the source member to destination in case of true when
		/// the mapping is done as a copy from source to destination.</param>
		public void Ignore(string memberName, bool ignoreOnCopy = true)
		{
			//this method can only be called after mapping by convention has been configured
			//and so the mappingDict is populated already with MemberMap configurations for members
			//that could be mapped

			MemberMap memberMap;
			if (mappingDict.TryGetValue(memberName, out memberMap))
			{
				memberMap.Ignore = true;
				memberMap.IgnoreOnCopy = ignoreOnCopy;
			}
			else
			{
				memberMap = new MemberMap
				{
					DestinationMemberName = memberName,
					Ignore = true,
					IgnoreOnCopy = true
				};
				mappingDict[memberName] = memberMap;
			}
		}

		public void ResetConfiguration()
		{
			fieldInfoDict.Clear();

			MapByConvention();
		}

		/// <summary>
		/// Verifies that all destination members havea a mapping definition. Will throw an exception if
		/// there is any member with no defined mapping behaviour.
		/// </summary>
		public void VerifyDestinationDefinitions()
		{
			string[] unmappedMembers = GetUnmatchedDestinationMembers().Select(x => x.Name).ToArray();
			if (unmappedMembers.Count() > 0)
			{
				string template = "Not all destination members have been mapped for {0} to {1} configuration." +
					" Add ignore rules or add mapping source for the fields: {2}";
				string members = string.Join(", ", unmappedMembers);
				string text = string.Format(template, sourceType.GetCSharpName(), destinationType.GetCSharpName(), members);
				throw new MappingException(text);
			}
		}

		/// <summary>
		/// Verifies that the mappings on which the configuration relies are defined.
		/// </summary>
		public void VerifyDependencies()
		{
			//now this method is tricky since in the future there might be more way relations are 
			//formed with other configurations and this method will have to be updated accordingly.
			foreach (MemberMap map in mappingDict.Values)
			{
				//IRelatedConfigRule rule = (IRelatedConfigRule)map.MappingRule;
				//TODO: we need to iterate thru the dictionary here!
				foreach (KeyValuePair<Type, Type> typePair in map.MappingRule.GetDependantMappings())
				{
					if (!Mapper.IsConfigurationDefined(typePair.Key, typePair.Value))
					{
						throw new MappingException(string.Format("For {0} to {1} configuration mapping to the destination " +
							"member {2} relies on having a configuration defined from {3} to {4} that was not found.",
							sourceType.GetCSharpName(), destinationType.GetCSharpName(), map.DestinationMemberName,
							typePair.Key.GetCSharpName(), typePair.Value.GetCSharpName()));
					}
				}
			}
		}

		#endregion public methods

		#region private methods

		/// <summary>
		/// Will fill the fieldInfoDict.
		/// </summary>
		private void ReflectDestination()
		{
			Type destinationType = typeof(TDestination);

			MemberInfo[] destinationMembers = GetMembers(destinationType);

			//add all target properties to the fieldInfoDict
			destinationMembers.ToList().ForEach(x => fieldInfoDict.Add(x.Name, x));
		}

		/// <summary>
		/// Will map fields from source type by name and type convention.
		/// </summary>
		private void MapByConvention()
		{
			MemberInfo[] destinationMembers = GetMembers(destinationType);
			MemberInfo[] sourceMembers = GetMembers(sourceType);

			//TODO: after case sensitive matching has been done, and there are unmatched destination members
			//do a non sensitive comparison
			IEnumerable<MemberInfo> matchedProperties = sourceMembers
				.Where(x => destinationMembers.Any(y => x.Name == y.Name));
			foreach (MemberInfo sourceMember in sourceMembers)
			{
				foreach (MemberInfo destinationMember in destinationMembers)
				{
					if (sourceMember.Name != destinationMember.Name)
					{
						continue;
					}

					MemberMap memberMap;
					if (!TryConfigureMemberMapping(sourceMember, destinationMember, out memberMap))
					{
						//probably exceptions should be thrown by AssignConfigurationIsValid() method instead
						//throw new MappingException(string.Format("No mapping could be defined for member {0} from {1} to {2}.",
						//    destinationMember, destinationType.GetCSharpName(), sourceType.GetCSharpName()));
					}

					mappingDict[memberMap.DestinationMemberName] = memberMap;
				}
			}
		}

		private bool TryConfigureMemberMapping(MemberInfo sourceMember, 
			MemberInfo destinationMember, out MemberMap memberMap)
		{
			memberMap = new MemberMap
			{
				Ignore = false,
				IgnoreOnCopy = false,
				DestinationMemberName = destinationMember.Name,
				SourceMemberName = sourceMember.Name
			};

			Type sourceType = GetMemberType(sourceMember);
			Type destinationType = GetMemberType(destinationMember);

			//check if there is an ignore attribute set on the destionation member
			object[] destinationAttributes = destinationMember.GetCustomAttributes(true);
			IgnoreMapAttribute ignoreAttribute = destinationAttributes
				.SingleOrDefault(x => x is IgnoreMapAttribute) as IgnoreMapAttribute;
			if (ignoreAttribute != null)
			{
				memberMap.Ignore = true;

				//if IgnoreOnCopy is set to true, there is no reason for continuing to rule finding
				if (memberMap.IgnoreOnCopy = ignoreAttribute.IgnoreOnCopy)
				{
					return false;
				}
			}

			//if member is not ignored at least for copy find a mapping rule
			IMemberMappingRule mappingRule;

			//if a mapping could be defined return true
			if (RuleProvider.GetApplicableRule(sourceType, destinationType, out mappingRule))
			{
				memberMap.MappingRule = mappingRule;
				return true;
			}
			else
			{
				//if could not setup the mapping ignore the member and return false
				//TODO: throw exception? wait till case insensitive mapping has been tried and then throw?
				memberMap.Ignore = true;
				return false;
			}
		}

		/// <summary>
		/// Gets the destination members with no configuration defined for them.
		/// </summary>
		private IEnumerable<MemberInfo> GetUnmatchedDestinationMembers()
		{
			return fieldInfoDict.Where(x => !mappingDict.ContainsKey(x.Key)).Select(x => x.Value);
		}

		/// <summary>
		/// Gets an array of properties and public fields from the targetType.
		/// </summary>
		private static MemberInfo[] GetMembers(Type targetType)
		{
			MemberInfo[] foundMembers = targetType
						 .GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foundMembers = foundMembers.Union(targetType
				.GetFields(BindingFlags.Public | BindingFlags.Instance)).ToArray();
			return foundMembers;
		}

		/// <summary>
		/// Gets the type of the member.
		/// </summary>
		private Type GetMemberType(MemberInfo member)
		{
			if (member.MemberType == MemberTypes.Property)
			{
				return (member as PropertyInfo).PropertyType;
			}
			else if (member.MemberType == MemberTypes.Field)
			{
				return (member as FieldInfo).FieldType;
			}
			throw new ArgumentException("Argument must be a Property or a Field", "member");
		}

		#endregion private methods
	}
}
