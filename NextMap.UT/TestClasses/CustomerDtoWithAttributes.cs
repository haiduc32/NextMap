using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NextMap.UT.TestClasses
{
	public class CustomerDtoWithAttributes
	{
		[IgnoreMap(ignoreOnCopy: false)]
		public string Name { get; set; }

		public int Years { get; set; }

		public string Surname { get; set; }

		public double Weight { get; set; }

		public double? Height { get; set; }
	}
}
