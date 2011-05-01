using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Benchmark
{
	class TestBench
	{
		Customer customer;

		public TestBench()
		{
			InitTests();
		}

		public double ManualMappingPerformance()
		{
			CustomerDto dtoCustomer;

			PerformanceTimer timer = new PerformanceTimer();
			
			timer.Start();
			
			for (int i = 0; i < 1000000; i++)
			{
				dtoCustomer = customer.ToDto();
			}

			return timer.Stop();
		}

		public double NextMapPerformance()
		{
			CustomerDto dtoCustomer;

			//define the NextMap mapping.
			NextMap.Mapper.CreateMap<Customer, CustomerDto>().ForMember(x => x.Years, y => y.MapFrom(z => z.Age));

			PerformanceTimer timer = new PerformanceTimer();

			timer.Start();

			for (int i = 0; i < 1000000; i++)
			{
				dtoCustomer = NextMap.Mapper.Map<Customer, CustomerDto>(customer);
			}

			return timer.Stop();		
		}

		public double AutoMapperPerformance()
		{
			CustomerDto dtoCustomer;

			//define the NextMap mapping.
			AutoMapper.Mapper.CreateMap<Customer, CustomerDto>().ForMember(x => x.Years, y => y.MapFrom(z => z.Age));

			PerformanceTimer timer = new PerformanceTimer();

			timer.Start();

			for (int i = 0; i < 1000000; i++)
			{
				dtoCustomer = AutoMapper.Mapper.Map<Customer, CustomerDto>(customer);
			}

			return timer.Stop();	
		}

		private void InitTests()
		{
			customer = new Customer
			{
				Age = 21,
				Height = 185,
				LastName = "Doe",
				Name = "Jon",
				Weight = 75.5
			};
		}
	}
}
