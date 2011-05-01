using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NextMap
{
	public class MappingException : Exception
	{
		public MappingException(string message) : base(message) { }

		public MappingException() : base() { }

		public MappingException(string message, Exception innerException) : base(message, innerException) { }
	}
}
