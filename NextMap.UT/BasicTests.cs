using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NextMap.UT.TestClasses;
using NextMap;

namespace NextMap.UT
{
	[TestClass]
	public class BasicTests
	{
		//TODO: 1. test mapping simple structures like DateTime

		#region test methods

		/// <summary>
		/// Test mapping 2 simple classes with only primitive properties and fields.
		/// </summary>
		[TestMethod]
		public void Simple_Mapping_Test()
		{
			CountryEnum b = CountryEnum.Australia;
			CountryEnumDto a = (CountryEnumDto)b;

			Mapper.CreateMap<Customer, CustomerDto>(overrideIfExist: true)
				.ForMember(x => x.Years, x => x.MapFrom(y => y.Age))
				.ForMember(x => x.Surname, x => x.MapFrom(y => y.LastName));

			Customer customer = GenerateCustomer();

			CustomerDto customerDto = Mapper.Map<Customer, CustomerDto>(customer);

			VerifyCustomerDto(customerDto);

			
		}

		[TestMethod]
		public void Ignore_Member_Test()
		{
			Mapper.CreateMap<Customer, CustomerDto>(overrideIfExist: true).ForMember(x => x.Name, x => x.Ignore());

			Customer customer = GenerateCustomer();

			CustomerDto customerDto = Mapper.Map<Customer, CustomerDto>(customer);

			Assert.AreNotEqual(customer.Name, customerDto.Name);
		}

		[TestMethod]
		[ExpectedException(typeof(MappingException))]
		public void Check_First_Level_Member_Test()
		{
			Mapper.CreateMap<Customer, CustomerDto>(overrideIfExist: true).ForMember(x => x.Name.Length, x => x.Ignore());
		}

		#endregion test methods

		#region helper methods

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

		private void VerifyCustomerDto(CustomerDto customerDto)
		{
			Assert.AreEqual(21, customerDto.Years);
		    Assert.AreEqual(181, customerDto.Height);
			Assert.AreEqual("Doe", customerDto.Surname);
			Assert.AreEqual("Jon", customerDto.Name);
			Assert.AreEqual(75, customerDto.Weight);
		}

		#endregion helper methods
	}
}
