using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace NextMap
{
	public interface IMemberConfigurationExpression<TSource>
	{
		void MapFrom<TMember>(Expression<Func<TSource, TMember>> sourceMember);

		void Ignore();
	}
}
