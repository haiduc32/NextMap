using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NextMap.Extensions
{
	static class TypeExtensions
	{
		/// <summary>
		/// Gets the type fully valid c# name.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string GetCSharpName(this Type type)
		{
			if (!type.IsGenericType)
				return type.Namespace + "." + type.Name;

			//so we have a generic type..
			int nameEnd = type.Name.IndexOf('`');
			string name = type.Namespace + "." + type.Name.Substring(0, nameEnd);
			name = name + "<" +  string.Join(", ", type.GetGenericArguments().Select(x=>x.GetCSharpName())) + ">";
			return name;
		}

		/// <summary>
		/// Will check the type of the generic parameter or if it is not a nullable type just
		/// return the type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Type GetNullableType(this Type type)
		{
			if (type.IsGenericType &&
				type.GetGenericTypeDefinition() == typeof(Nullable<>))
				return type.GetGenericArguments()[0];
			return type;
		}
	}
}
