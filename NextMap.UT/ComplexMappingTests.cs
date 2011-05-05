using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NextMap.UT.TestClasses;

namespace NextMap.UT
{
	[TestClass]
	public class ComplexMappingTests
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

		[TestMethod]
		public void Complex_Class_Auto_Mapping_Test()
		{
			//the mappings are declared in reverse order to test that the order does not influence the mappings
			//as long as the <ShoppingCartDto, ShoppingCart> compilation batch is not done at a later time
			Mapper.CreateMap<ShoppingCart, ShoppingCartDto>();
			Mapper.CreateMap<Product, ProductDto>();

			ShoppingCart shoppingCart = GenerateShoppingCart();

			ShoppingCartDto shoppingCartDto = Mapper.Map<ShoppingCart, ShoppingCartDto>(shoppingCart);

			Assert.IsNotNull(shoppingCartDto);
			Assert.IsNotNull(shoppingCartDto.Products);
			Assert.AreEqual(2, shoppingCartDto.Products.Count);
			Assert.AreEqual("First Product", shoppingCartDto.Products[0].Name);
			Assert.AreEqual(13m, shoppingCartDto.Products[0].Price);
			Assert.AreEqual("Second Product", shoppingCartDto.Products[1].Name);
			Assert.AreEqual(12.5m, shoppingCartDto.Products[1].Price);
			Assert.AreEqual(2, shoppingCart.Products[0].Feedback.Count);
		}

		/// <summary>
		/// ClassWithArray.ArrayToArray is of type short[] and the destination is of type int[].
		/// Expected that they will auto map.
		/// IGNORED: this is future functionality, the test is in place for testing the expected behaviour
		/// </summary>
		//[Ignore]
		[TestMethod]
		public void Class_With_Primitive_Arrays_Test()
		{
			ClassWithArray classWithArray = new ClassWithArray 
			{ 
				//ArrayToArray = new short[] { 1, 2, 3 },
				//ArrayToList = new List<short> { 1, 2, 3 },
				//ListToArray = new short[] { 1, 2, 3 },
				ListToList = new List<short> { 1, 2, 3 },
			};
			ClassWithArrayDto dto;

			Mapper.CreateMap<ClassWithArray, ClassWithArrayDto>();

			dto = Mapper.Map<ClassWithArray, ClassWithArrayDto>(classWithArray);

			Assert.IsNotNull(dto);
			//Assert.AreEqual(3, dto.ArrayToArray.Count());
			//Assert.AreEqual(3, dto.ArrayToList.Count);
			//Assert.AreEqual(3, dto.ListToArray.Count());
			Assert.AreEqual(3, dto.ListToList.Count);
		}

		#endregion test methods

		#region helper methods

		private ShoppingCart GenerateShoppingCart()
		{
			ShoppingCart shoppingCart = new ShoppingCart
			{
				Products = new List<Product>
				{
					new Product("First Product")
					{
						Price = 13.0,
						SecretCode = "11",
						Feedback = new List<string> { "good", "fair good" }
					},
					new Product("Second Product")
					{
						Price = 12.5,
						SecretCode = "31"
					}
				}
			};

			return shoppingCart;
		}

		#endregion helper methods
	}
}
