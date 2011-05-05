using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;
using System.CodeDom.Compiler;

namespace NextMap
{
	internal class MapCompiler
	{
		private const string FIELD_MAP = "targetObject.{0} = sourceObject.{1};\r\n";

		private static int classCounter = 0;

		public Dictionary<MapKey,IMap> Compile(List<IMappingConfiguration> configurationList)
		{
			List<string> referenceList = new List<string>();

			Dictionary<MapKey, string> classMap = new Dictionary<MapKey,string>();

			string code = @"
using System;
using System.Linq;
using NextMap;
";

			foreach (IMappingConfiguration configuration in configurationList)
			{
				List<string> outputReferenceList;

				string className;
				code += GenerateMapClassCode(configuration, out outputReferenceList, out className) + "\r\n\r\n";
				classMap[new MapKey(configuration.SourceType, configuration.DestinationType)] = className;

				//merge the assemblies in the general list
				foreach (string assemblyFile in outputReferenceList)
				{
					if (!referenceList.Contains(assemblyFile))
					{
						referenceList.Add(assemblyFile);
					}
				}
			}
			
			CodeDomProvider loCompiler = CodeDomProvider.CreateProvider("CSharp");
			CompilerParameters loParameters = new CompilerParameters();
			
			loParameters.ReferencedAssemblies.Add("System.dll");
			loParameters.ReferencedAssemblies.Add("System.Core.dll");
			loParameters.ReferencedAssemblies.Add(typeof(Mapper).Assembly.Location);//"NextMap.dll");
			//add the rest of the referenced assemblies
			referenceList.ForEach(x => loParameters.ReferencedAssemblies.Add(x));

			loParameters.GenerateInMemory = true;

			// *** Now compile the whole thing
			CompilerResults loCompiled = loCompiler.CompileAssemblyFromSource(loParameters, code);
			foreach (CompilerError compilerError in loCompiled.Errors)
			{
				string a = compilerError.ErrorText;
			}

			Assembly loAssembly = loCompiled.CompiledAssembly;

			Dictionary<MapKey, IMap> resultDict = new Dictionary<MapKey, IMap>();

			foreach (KeyValuePair<MapKey, string> pair in classMap)
			{
				IMap loObject = (IMap)loAssembly.CreateInstance(pair.Value);
				resultDict.Add(pair.Key, loObject);
			}

			return resultDict;
		}

		/// <summary>
		/// Creates the source code for a mapping class.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		/// <returns></returns>
		private string GenerateMapClassCode(IMappingConfiguration configuration, out List<string> referenceList, out string fullName)
		{
			referenceList = new List<string>();

			string sourceAssemblyFile = configuration.SourceType.Assembly.Location;
			string targetAssemblyFile = configuration.DestinationType.Assembly.Location;

			referenceList.Add(sourceAssemblyFile);
			referenceList.Add(targetAssemblyFile);

			string sourceTypeName = configuration.SourceType.Name;

			string sourceCast = configuration.SourceType.Name + " sourceObject = (" + configuration.SourceType.Name + ")source;\r\n";
			string targetCreate = configuration.DestinationType.Name + " targetObject = new " + configuration.DestinationType.Name + "();\r\n";

			int classIndex = Interlocked.Increment(ref classCounter);
			fullName = string.Format("DynamicCode{0}.DynamicCodeMap{0}", classIndex);

			string mappingLines = string.Empty;
			foreach (KeyValuePair<string, MemberMap> pair in configuration.Mappings)
			{
				MemberMap memberMap = pair.Value;

				if (memberMap == null || memberMap.Ignore) continue;

				mappingLines += memberMap.GenerateCode("sourceObject", "targetObject") + "\r\n";
				// string.Format(FIELD_MAP, memberMap.MemberName, memberMap.SourceMemberName);
			}

			List<string> usingList = new List<string>();
			usingList.Add(configuration.DestinationType.Namespace);
			usingList.Add(configuration.SourceType.Namespace);

			string usingCode = GenerateUsingCode(usingList);

			string code =
@"namespace DynamicCode" + classIndex + @"
{
" + usingCode + @"

	public class DynamicCodeMap" + classIndex + @" : IMap
	{
		public object Map(object source)
		{
			if (source == null) return null;
			" + sourceCast + @"
			" + targetCreate + @"
			" + mappingLines + @"
			return targetObject;
		} 
	}
}";
			return code;
		}

		private string GenerateUsingCode(List<string> usingList)
		{
			string usingCode = string.Empty;
			foreach (string usingNamespace in usingList.Distinct())
			{
				usingCode += "using " + usingNamespace + "; \r\n";
			}

			return usingCode;
		}
	}
}
