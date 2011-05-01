using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NextMap
{
	internal interface IMappingConfiguration
	{
		Type SourceType { get; }

		Type DestinationType { get; }

		Dictionary<string, MemberMap> Mappings { get; }

		/// <summary>
		/// Verifies that the mappings on which the configuration relies are defined.
		/// </summary>
		void VerifyDependencies();

		/// <summary>
		/// Verifies that all destination members havea a mapping definition. Will throw an exception if
		/// there is any member with no defined mapping behaviour.
		/// </summary>
		void VerifyDestinationDefinitions();
	}
}
