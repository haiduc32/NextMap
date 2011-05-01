using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NextMap.UT.TestClasses
{
	public class Product
	{
		public Product(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }
		public double Price { get; set; }

		public string SecretCode { set; private get; }
	}
}
