using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Benchmark
{
	static class ManualMapper
	{
		public static CustomerDto ToDto(this Customer customer)
		{
			return new CustomerDto
			{
				 Height = customer.Height,
				 LastName = customer.LastName,
				 Name = customer.Name,
				 Weight = customer.Weight,
				 Years = customer.Age
			};
		}
	}
}
