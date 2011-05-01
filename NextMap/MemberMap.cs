using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NextMap
{
	internal class MemberMap
	{

		public string MemberName { get; set; }

		public string SourceMemberName { get; set; }

		public bool Ignore { get; set; }

		public MemberMapType MapType { get; set; }

		public IMemberMappingRule MappingRule { get; set; }
	}
}
