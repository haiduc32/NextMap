using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NextMap.UT.TestClasses;

namespace NextMap.UT
{
	[TestClass]
	public class ValidationTests
	{
		#region UT setup

		[TestCleanup]
		public void TestCleanup()
		{
			//spring cleaning, huh..
			Mapper.ClearConfiguraitons();
		}

		#endregion UT setup

		#region test methods

		/// <summary>
		/// CustomerDto has membes with name that aren't found in the Customer. 
		/// Expected that validation after auto configuration should fail.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(MappingException))]
		public void Basic_Validation_Test()
		{
			Mapper.CreateMap<Customer, CustomerDto>();
			Mapper.AssertConfigurationIsValid();
		}

		/// <summary>
		/// All fields from the CustomerDto have been configured either by convention or
		/// by manual configuration.
		/// Expected that validating the configuration will throw no exceptions.
		/// </summary>
		[TestMethod]
		public void Basic_Validation_With_Valid_Configuration_Test()
		{
			Mapper.CreateMap<Customer, CustomerDto>()
				.ForMember(x => x.Years, x => x.MapFrom(y => y.Age))
				.ForMember(x => x.Surname, x => x.MapFrom(y => y.LastName));
			Mapper.AssertConfigurationIsValid();
		}

		[TestMethod]
		[ExpectedException(typeof(MappingException))]
		public void Complex_Mapping_With_Missing_Dependant_Configuration_Test()
		{
			Mapper.CreateMap<ShoppingCart, ShoppingCartDto>();

			Mapper.AssertConfigurationIsValid();
		}

		[TestMethod]
		public void Complex_Mapping_With_Dependant_Configuration_Test()
		{
			Mapper.CreateMap<ShoppingCart, ShoppingCartDto>();
			//product is defined second even altho it is required for the ShoppingCart configuration,
			//that way we test that the order is not important.
			Mapper.CreateMap<Product, ProductDto>();

			Mapper.AssertConfigurationIsValid();
		}

		#endregion test methods

		private Customer GenerateCustomer()
		{
			Customer customer = new Customer
			{
				Age = 21,
				Height = 181,
				LastName = "Doe",
				Name = "Jon",
				Weight = 75
			};

			return customer;
		}
	}
}
