using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace NextMap
{
	/// <summary>
	/// Interface for member configuration expressions.
	/// </summary>
	public interface IMemberConfigurationExpression<TSource>
	{
		/// <summary>
		/// Sets the mapping source for a destination member.
		/// </summary>
		/// <param name="sourceMember">The top level member from the source type.</param>
		void MapFrom<TMember>(Expression<Func<TSource, TMember>> sourceMember);

		/// <summary>
		/// Ignores the destination member from the mapping configuration.
		/// </summary>
		/// <param name="ignoreOnCopy">Is only valid for members that have been configured for mapping by a
		/// previous rule or by convention. Will map the source member to destination in case of true when
		/// the mapping is done as a copy from source to destination.</param>
		void Ignore(bool ignoreOnCopy = true);
	}
}
