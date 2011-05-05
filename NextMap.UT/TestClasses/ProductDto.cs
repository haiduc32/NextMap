using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NextMap.UT.TestClasses
{
	public class ProductDto
	{
		public string Name { get; set; }
		public decimal Price { get; set; }

		public List<string> Feedback { get; set; }
	}
}
