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
		//TODO: 2. test exceptions when mapping can't be done
		//TODO: 3. test mapping from class to struct when source is null

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
		/// Test mapping 2 simple classes with only primitive properties and fields.
		/// </summary>
		[TestMethod]
		public void Basic_Mapping_Test()
		{
			Mapper.CreateMap<Customer, CustomerDto>()
				.ForMember(x => x.Years, x => x.MapFrom(y => y.Age))
				.ForMember(x => x.Surname, x => x.MapFrom(y => y.LastName));

			Customer customer = GenerateCustomer();

			CustomerDto customerDto = Mapper.Map<Customer, CustomerDto>(customer);

			VerifyCustomerDto(customerDto);
		}

		[TestMethod]
		public void Ignore_Member_Test()
		{
			Mapper.CreateMap<Customer, CustomerDto>().ForMember(x => x.Name, x => x.Ignore());

			Customer customer = GenerateCustomer();

			CustomerDto customerDto = Mapper.Map<Customer, CustomerDto>(customer);

			Assert.AreNotEqual(customer.Name, customerDto.Name);
		}

		/// <summary>
		/// Setup an Ignore for a member with ignoreOnCopy to false.
		/// Expect the copy mapping to not ignore the member.
		/// </summary>
		[TestMethod]
		public void Ignore_Copy_Member_Teset()
		{
			Mapper.CreateMap<Customer, CustomerDto>().ForMember(x => x.Name, x => x.Ignore(false));

			Customer customer = GenerateCustomer();

			CustomerDto customerDto = new CustomerDto { Name = "OldName" };
			Mapper.Map(customer, customerDto);

			CustomerDto newCustomerDto = Mapper.Map<Customer, CustomerDto>(customer);

			Assert.AreEqual(customer.Name, customerDto.Name);
			Assert.AreNotEqual(customer.Name, newCustomerDto.Name);
		}

		/// <summary>
		/// Setup an Ignore for a member with ignoreOnCopy to false.
		/// Expect the copy mapping to not ignore the member.
		/// </summary>
		[TestMethod]
		public void Ignore_Attribute_Copy_Member_Teset()
		{
			Mapper.CreateMap<Customer, CustomerDtoWithAttributes>();

			Customer customer = GenerateCustomer();

			CustomerDtoWithAttributes customerDto = new CustomerDtoWithAttributes { Name = "OldName" };
			Mapper.Map(customer, customerDto);

			CustomerDtoWithAttributes newCustomerDto = Mapper.Map<Customer, CustomerDtoWithAttributes>(customer);

			Assert.AreEqual(customer.Name, customerDto.Name);
			Assert.AreNotEqual(customer.Name, newCustomerDto.Name);
		}

		[TestMethod]
		[ExpectedException(typeof(MappingException))]
		public void Check_First_Level_Member_Test()
		{
			Mapper.CreateMap<Customer, CustomerDto>().ForMember(x => x.Name.Length, x => x.Ignore());
		}

		[TestMethod]
		public void Null_Mapping_Test()
		{
			Mapper.CreateMap<Customer, CustomerDto>();

			Customer customer = null;

			CustomerDto customerDto = Mapper.Map<Customer, CustomerDto>(customer);

			Assert.IsNull(customerDto);
		}

		/// <summary>
		/// Define a mapping configuration for customer excluding the Name and Surname.
		/// Map an instance of Customer to an instance of CustomerDto.
		/// Expect that Name and Surname on the instance of CustomerDto did not change. Expect the rest of the 
		/// fields are updated.
		/// </summary>
		[TestMethod]
		public void Basic_Copy_Map()
		{
			Mapper.CreateMap<Customer, CustomerDto>()
				.ForMember(x => x.Years, x => x.MapFrom(y => y.Age))
				.ForMember(x => x.Surname, x => x.Ignore())
				.ForMember(x => x.Name, x => x.Ignore());

			Customer customer = GenerateCustomer();
			CustomerDto customerDto = GenerateCustomerDto();

			CustomerDto resultCustomer = Mapper.Map(customer, customerDto);

			//the result customer must be the same instance as the one supplied as param to destination.
			Assert.AreSame(customerDto, resultCustomer);

			Assert.AreEqual(customer.Age, customerDto.Years);
			Assert.AreEqual(customer.Height, customerDto.Height);
			Assert.AreNotEqual(customer.Name, customerDto.Name);
			Assert.AreNotEqual(customer.LastName, customerDto.Surname);

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

		private CustomerDto GenerateCustomerDto()
		{
			CustomerDto customer = new CustomerDto
			{
				Years = 23,
				Height = 175,
				Surname = "Snow",
				Name = "Mark",
				Weight = 85
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
