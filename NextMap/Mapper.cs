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

		private static List<IMappingConfiguration> outstandingConfigurations = new List<IMappingConfiguration>();

		private static object lockObject = new object();

		#endregion private methods

		#region public methods

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

				//add a key in the mapDictionary with null value to support checks for
				//defined mappings
				MapKey mapKey = (new MapKey(typeof(TSource), typeof(TDestination)));

				if (!overrideIfExist && mapDictionary.ContainsKey(mapKey))
				{
					throw new MappingException("The mapping from " + typeof(TSource).Name + " to " + typeof(TDestination).Name + " has already defined.");
				}
				mapDictionary[mapKey] = null;
			}

			return new MappingExpression<TSource,TDestination>(mappingConfiguration);
		}

		public static TDestination Map<TSource, TDestination>(TSource source)
		{
			//TODO: fist must check the list of outstanding mapping configurations for compiling
			//TODO: optimize, try to bypass the lock when possible
			lock (lockObject)
			{
				if (outstandingConfigurations.Count > 0)
				{
					CompileOutstandingConfigurations();
				}
			}

			IMap mapper;

			if (!mapDictionary.TryGetValue(new MapKey(typeof(TSource), typeof(TDestination)), out mapper))
			{
				throw new InvalidOperationException(string.Format("A mapping has not been defined from {0} type to {1} type.", 
					typeof(TSource).GetCSharpName(), typeof(TDestination).GetCSharpName()));
			}

			object mappedObject = mapper.Map(source);
			return (TDestination)mappedObject;
		}

		public static bool IsConfigurationDefined<TSource, TDestination>()
		{
			return IsConfigurationDefined(typeof(TSource), typeof(TDestination));
		}

		public static bool IsConfigurationDefined(Type sourceType, Type destinationType)
		{
			MapKey key = new MapKey(sourceType, destinationType);
			return mapDictionary.ContainsKey(key);
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
