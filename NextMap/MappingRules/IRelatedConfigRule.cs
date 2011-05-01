using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NextMap.MappingRules
{
	internal interface IRelatedConfigRule
	{
		/// <summary>
		/// Gets the type required for mapping for the destination member.
		/// </summary>
		Type MapDestinationType { get; }
		/// <summary>
		/// Gets the type required for mapping for the source member.
		/// </summary>
		Type MapSourceType { get; }
	}
}
