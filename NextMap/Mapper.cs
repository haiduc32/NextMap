using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using NextMap.Extensions;

namespace NextMap
{
	public static class Mapper
	{
		#region private fields

		private static Dictionary<MapKey, IMap> mapDictionary = new Dictionary<MapKey, IMap>();

		private static Dictionary<MapKey, IMappingConfiguration> configurationsDict =
			new Dictionary<MapKey, IMappingConfiguration>();

		private static List<IMappingConfiguration> outstandingConfigurations = new List<IMappingConfiguration>();
		
		private static object lockObject = new object();

		#endregion private methods

		#region public methods

		/// <summary>
		/// Creates the mapping configuration from TSource type to TDestination type.
		/// </summary>
		/// <param name="overrideIfExist">If a previous configuration was defined for that types it will be overriden by 
		/// the new mapping configuration. Any special mapping conditions from the previous configuration will be lost.</param>
		public static IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>(bool overrideIfExist = false)
			where TSource : class
			where TDestination : class
		{
			//must check that both types are public before mapping
			VerifyTypePublic<TSource>();
			VerifyTypePublic<TDestination>();

			MappingConfiguration<TSource, TDestination> mappingConfiguration = 
				new MappingConfiguration<TSource, TDestination>();

			lock (lockObject)
			{
				outstandingConfigurations.Add(mappingConfiguration);

				MapKey mapKey = (new MapKey(typeof(TSource), typeof(TDestination)));

				if (!overrideIfExist && mapDictionary.ContainsKey(mapKey))
				{
					throw new MappingException("The mapping from " + typeof(TSource).Name + " to " 
						+ typeof(TDestination).Name + " has already defined.");
				}

				//add a key in the mapDictionary with null value to support checks for
				//defined mappings
				mapDictionary[mapKey] = null;
				configurationsDict[mapKey] = mappingConfiguration;
			}

			return new MappingExpression<TSource,TDestination>(mappingConfiguration);
		}

		/// <summary>
		/// Creates a new instance of TDestination mapping all fields from the source object based on a previously
		/// defined mapping configuration.
		/// </summary>
		public static TDestination Map<TSource, TDestination>(TSource source)
		{
			//TODO: optimize, try to bypass the lock when possible
			lock (lockObject)
			{
				//fist must check the list of outstanding mapping configurations for compiling
				if (outstandingConfigurations.Count > 0)
				{
					CompileOutstandingConfigurations();
				}
			}

			IMap mapper;

			if (!mapDictionary.TryGetValue(new MapKey(typeof(TSource), typeof(TDestination)), out mapper))
			{
				throw new MappingException(string.Format("A mapping configuration has not been defined from {0} to {1}.", 
					typeof(TSource).GetCSharpName(), typeof(TDestination).GetCSharpName()));
			}

			object mappedObject = mapper.Map(source);
			return (TDestination)mappedObject;
		}

		/// <summary>
		/// Verifies all defined configurations to be valid and that all members in destination 
		/// types have mapping rules defined.
		/// </summary>
		public static void AssertConfigurationIsValid()
		{
			foreach (IMappingConfiguration configuration in configurationsDict.Values)
			{
				configuration.VerifyDependencies();
				configuration.VerifyDestinationDefinitions();
			}
		}

		/// <summary>
		/// Checks if a configuration has been defined.
		/// </summary>
		public static bool IsConfigurationDefined<TSource, TDestination>()
		{
			return IsConfigurationDefined(typeof(TSource), typeof(TDestination));
		}

		/// <summary>
		/// Checks if a configuration has been defined.
		/// </summary>
		public static bool IsConfigurationDefined(Type sourceType, Type destinationType)
		{
			MapKey key = new MapKey(sourceType, destinationType);
			return mapDictionary.ContainsKey(key);
		}

		/// <summary>
		/// Gets the configuration for the specified types. Will return null if none is defined.
		/// </summary>
		internal static IMappingConfiguration GetConfiguration(Type sourceType, Type destinationType)
		{
			MapKey key = new MapKey(sourceType, destinationType);
			IMappingConfiguration configuration;
			configurationsDict.TryGetValue(key, out configuration);
			return configuration;
		}

		#endregion public methods

		#region private methods

		private static void CompileOutstandingConfigurations()
		{
			MapCompiler compiler = new MapCompiler();
			Dictionary<MapKey, IMap> mapObjects = compiler.Compile(outstandingConfigurations);

			foreach (KeyValuePair<MapKey, IMap> pair in mapObjects)
			{
				mapDictionary[pair.Key] = pair.Value;
			}

			outstandingConfigurations.Clear();
		}

		private static void VerifyTypePublic<T>()
		{
			if (typeof(T).IsNotPublic)
			{
				throw new MappingException(string.Format("{0} is not declared as Public.", typeof(T).Name));
			}
		}

		#endregion private methods
	}
}
