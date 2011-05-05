using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NextMap.MappingRules;

namespace NextMap
{
	internal class MemberMap
	{

		public string DestinationMemberName { get; set; }

		public string SourceMemberName { get; set; }

		public bool Ignore { get; set; }

		public MemberMapType MapType { get; set; }

		public IMemberMappingRule MappingRule { get; set; }

		internal string GenerateCode(string sourceObject, string destinationObject)
		{
			return MappingRule.GenerateInlineCode(sourceObject + "." + SourceMemberName,
				destinationObject + "." + DestinationMemberName);
		}
	}
}
