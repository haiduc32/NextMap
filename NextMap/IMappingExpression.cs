using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace NextMap
{
	public interface IMappingExpression<TSource,TDestination>
	{
		IMappingExpression<TSource, TDestination> ForMember(Expression<Func<TDestination, object>> destinationMember,
			Action<IMemberConfigurationExpression<TSource>> memberOptions);

		IMappingExpression<TSource, TDestination> ResetConfiguration();
	}
}
