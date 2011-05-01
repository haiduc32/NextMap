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
	}
}
