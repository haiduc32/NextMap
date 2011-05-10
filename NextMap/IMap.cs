using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NextMap
{
	public interface IMap
	{
		object Map(object source);
		object Map(object source, object destination);
	}
}
