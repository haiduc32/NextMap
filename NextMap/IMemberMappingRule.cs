using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NextMap
{
	interface IMemberMappingRule
	{
		string GenerateCode(string destinationObject, string sourceObject);
	}
}
