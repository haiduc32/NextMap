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

			mappingDict[memberMap.MemberName] = memberMap;
		}

		public void Ignore(string memberName)
		{
			MemberMap memberMap = new MemberMap
			{
				MemberName = memberName,
				Ignore = true
			};

			mappingDict[memberMap.MemberName] = memberMap;
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
			foreach (MemberMap map in mappingDict.Values.Where(x => x.MappingRule is IRelatedConfigRule))
			{
				IRelatedConfigRule rule = (IRelatedConfigRule)map.MappingRule;
				if (!Mapper.IsConfigurationDefined(rule.MapSourceType, rule.MapDestinationType))
				{
					throw new MappingException(string.Format("For {0} to {1} configuration mapping to the destination" +
						"member {2} reolies on having a configuration defined from {3} to {4} that was not found.",
						sourceType.GetCSharpName(), destinationType.GetCSharpName(), map.MemberName,
						rule.MapDestinationType.GetCSharpName(), rule.MapDestinationType.GetCSharpName()));	
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
						throw new MappingException(string.Format("No mapping could be defined for member {0} from {1} to {2}.",
							destinationMember, destinationType.GetCSharpName(), sourceType.GetCSharpName()));
					}

					mappingDict[memberMap.MemberName] = memberMap;
				}
			}
		}

		private bool TryConfigureMemberMapping(MemberInfo sourceMember, 
			MemberInfo destinationMember, out MemberMap memberMap)
		{
			memberMap = new MemberMap
			{
				Ignore = false,
				MemberName = destinationMember.Name,
				SourceMemberName = sourceMember.Name
			};

			Type sourceType = GetMemberType(sourceMember);
			Type destinationType = GetMemberType(destinationMember);

			//first check for primitive type
			#region primitives
			if (destinationType.GetNullableType().IsPrimitive)
			{
				//first check if the types are assignable
				//if you update the rules here consider updating them for assignable value types
				if (destinationType.IsAssignableFrom(sourceType))
				{
					memberMap.MappingRule = new SimpleRule(sourceMember.Name, destinationMember.Name);
				}
				else if (destinationType.IsPrimitive && sourceType.IsGenericType &&
					destinationType.IsAssignableFrom(sourceType.GetGenericArguments()[0]))
				{
					memberMap.MappingRule = new FromNullableRule(sourceMember.Name, destinationMember.Name,
						sourceType);
				}
			}
			#endregion Primitives
			//mapping to decimal requires special handling, mapping from decimal is not supported at this moment
			#region mapping to decimal
			else if (destinationType.Equals(typeof(decimal)))
			{
				if (sourceType.IsPrimitive)
				{
					memberMap.MappingRule = new CastRule(sourceMember.Name, destinationMember.Name, destinationType);
				}
			}
			#endregion mapping to decimal
			//mapping for String members
			#region String
			else if (destinationType.Equals(typeof(string)) && sourceType.Equals(typeof(string)))
			{
				memberMap.MappingRule = new SimpleRule(sourceMember.Name, destinationMember.Name);
			}
			#endregion String
			//mapping enums
			#region Enums
			else if (destinationType.IsEnum && sourceType.IsEnum)
			{
				//if enum types are same apply the simple rule
				if (destinationType.Equals(sourceType))
				{
					memberMap.MappingRule = new SimpleRule(sourceMember.Name, destinationMember.Name);
				}
				else
				{
					memberMap.MappingRule = new CastRule(sourceMember.Name, destinationMember.Name, destinationType);
				}
			}
			#endregion Enums
			//assignable value types
			#region assignable value types
			else if (destinationType.GetNullableType().IsValueType && sourceType.GetNullableType().IsValueType &&
				destinationType.GetNullableType().IsAssignableFrom(sourceType.GetNullableType()))
			{
				if (destinationType.IsAssignableFrom(sourceType))
				{
					memberMap.MappingRule = new SimpleRule(sourceMember.Name, destinationMember.Name);
				}

				else if (destinationType.IsValueType && sourceType.IsGenericType &&
					destinationType.IsAssignableFrom(sourceType.GetGenericArguments()[0]))
				{
					memberMap.MappingRule = new FromNullableRule(sourceMember.Name, destinationMember.Name,
						sourceType);
				}
			}
			#endregion same value tyeps
			else if ((destinationType.IsClass || destinationType.IsValueType) && (sourceType.IsClass || sourceType.IsValueType))
			{
				if (destinationType.Equals(sourceType))
				{
					memberMap.MappingRule = new SimpleRule(sourceMember.Name, destinationMember.Name);
				}
				else if (destinationType.GetInterfaces().Contains(typeof(System.Collections.IEnumerable)) &&
					sourceType.GetInterfaces().Contains(typeof(System.Collections.IEnumerable)))
				{
					//if types are not generic we can't map them, so throw an InvalidOperationException
					if (!destinationType.IsGenericType || !sourceType.IsGenericType)
					{
						throw new InvalidOperationException("Can't map classes derived from IEnumerable that are not generic.");
					}

					if (destinationType.GetInterfaces().Contains(typeof(System.Collections.IDictionary)))
					{
						throw new NotImplementedException("Dictionary is not supported yet.");
					}
					else
					{
						memberMap.MappingRule = new EnumerableRule(sourceMember.Name, destinationMember.Name,
							sourceType, destinationType);
					}
				}
				else
				{
					//will setup the Map<> for this types, but unfortunately we can't check them at this point
					memberMap.MappingRule = new MapClassRule(sourceMember.Name, destinationMember.Name,
						sourceType, destinationType);
				}
			}

			//if a mapping could be defined return true
			if (memberMap.MappingRule != null)
			{
				return true;
			}
			else
			{
				//if could not setup the mapping ignore the member and return false
				memberMap.Ignore = true;
				return false;
			}
		}

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
