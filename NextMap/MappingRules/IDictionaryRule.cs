using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NextMap.MappingRules
{
	/// <summary>
	/// Interface for mappings rules that represent defined mappings by user.
	/// </summary>
	interface IDictionaryRule
	{
		Type SourceType { get; }
		Type DestinationType { get; }
	}
}
