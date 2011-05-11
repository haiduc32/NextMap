using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace NextMap
{
	/// <summary>
	/// Interface for mapping expressions.
	/// </summary>
	public interface IMappingExpression<TSource,TDestination>
	{
		/// <summary>
		/// Expression for selecting a mapping rule for a destination member.
		/// </summary>
		/// <param name="destinationMember">The top level member from the destination type.</param>
		/// <param name="memberOptions">Configuration options for the member.</param>
		/// <returns>A mapping expression that can be used for applying more mapping rules.</returns>
		IMappingExpression<TSource, TDestination> ForMember(Expression<Func<TDestination, object>> destinationMember,
			Action<IMemberConfigurationExpression<TSource>> memberOptions);

		/// <summary>
		/// Clears all previous mapping rules for this types. Will not clear them if the configuration was defined
		/// previously.
		/// </summary>
		IMappingExpression<TSource, TDestination> ResetConfiguration();
	}
}
