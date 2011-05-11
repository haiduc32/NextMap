using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace NextMap
{
	/// <summary>
	/// IMappingExpression interface implementation.
	/// </summary>
	internal class MappingExpression<TSource,TDestination> : IMappingExpression<TSource,TDestination>,
		IMemberConfigurationExpression<TSource>
	{
		#region private fields

		private MappingConfiguration<TSource, TDestination> mappingConfiguration;

		private MemberInfo destinationMemberInfo;

		#endregion private fields

		#region .ctor

		/// <summary>
		/// Creates a new instance of <see cref="MappingExpression"/>.
		/// </summary>
		/// <param name="mappingConfiguration">The <see cref="MappingConfiguration"/> for which to create the
		/// <see cref="MappingExpression"/>.</param>
		internal MappingExpression(MappingConfiguration<TSource, TDestination> mappingConfiguration)
		{
			this.mappingConfiguration = mappingConfiguration;
		}

		#endregion .ctor

		#region IMappingExpression<TSource,TDestination> implementation

		/// <summary>
		/// Expression for selecting a mapping rule for a destination member.
		/// </summary>
		/// <param name="destinationMember">The top level member from the destination type.</param>
		/// <param name="memberOptions">Configuration options for the member.</param>
		/// <returns>A mapping expression that can be used for applying more mapping rules.</returns>
		public IMappingExpression<TSource, TDestination> ForMember(Expression<Func<TDestination, object>> destinationMember,
			Action<IMemberConfigurationExpression<TSource>> memberOptions)
		{
			//TODO: check the destination member to be in the target class, and to be a property or a field 
			//and not to be private??
			try
			{
				destinationMemberInfo = GetTopLevelMember(destinationMember);
			}
			catch (ArgumentException e)
			{
				throw new MappingException("Only top level members from th eclass can be selected as destinationMember", e);
			}

			memberOptions(this);
			return new MappingExpression<TSource, TDestination>(mappingConfiguration);
		}

		/// <summary>
		/// Clears all previous mapping rules for this types. Will not clear them if the configuration was defined
		/// previously.
		/// </summary>
		public IMappingExpression<TSource, TDestination> ResetConfiguration()
		{
			mappingConfiguration.ResetConfiguration();

			return new MappingExpression<TSource, TDestination>(mappingConfiguration);
		}

		#endregion IMappingExpression<TSource,TDestination> implementation

		#region IMemberConfigurationExpression<TSource> implementation

		/// <summary>
		/// Sets the mapping source for a destination member.
		/// </summary>
		/// <param name="sourceMember">The top level member from the source type.</param>
		public void MapFrom<TMember>(Expression<Func<TSource, TMember>> sourceMember)
		{
			//TODO: support all levels of members from source
			//TODO: support methods in sourceMember and expressions

			MemberInfo sourceMemberInfo;
			try
			{
				sourceMemberInfo = GetTopLevelMember(sourceMember);
			}
			catch (ArgumentException e)
			{
				throw new MappingException("Only top level members from th eclass can be selected as sourceMember", e);
			}

			mappingConfiguration.MapFrom(destinationMemberInfo.Name, sourceMemberInfo.Name, sourceMemberInfo);
		}

		/// <summary>
		/// Ignores the destination member from the mapping configuration.
		/// </summary>
		/// <param name="ignoreOnCopy">Is only valid for members that have been configured for mapping by a
		/// previous rule or by convention. Will map the source member to destination in case of true when
		/// the mapping is done as a copy from source to destination.</param>
		public void Ignore(bool ignoreOnMap = true)
		{
			//mappingConfiguration.a = "c";
			mappingConfiguration.Ignore(destinationMemberInfo.Name, ignoreOnMap);
		}

		#endregion IMemberConfigurationExpression<TSource> implementation

		#region private methods

		public MemberInfo GetTopLevelMember(Expression lambdaExpression)
		{
			switch (lambdaExpression.NodeType)
			{
				case ExpressionType.Convert:
					return GetTopLevelMember(((UnaryExpression)lambdaExpression).Operand);
				case ExpressionType.Lambda:
					return GetTopLevelMember(((LambdaExpression)lambdaExpression).Body);
				case ExpressionType.MemberAccess:
					MemberExpression memberExpression = ((MemberExpression)lambdaExpression);

					if (memberExpression.Expression.NodeType != ExpressionType.Parameter &&
						memberExpression.Expression.NodeType != ExpressionType.Convert)
					{
						throw new ArgumentException(string.Format("Expression '{0}' must resolve to top-level member.", 
							lambdaExpression), "lambdaExpression");
					}

					MemberInfo member = memberExpression.Member;

					return member;
				default:
					break;
			}

			return null;
		}

		#endregion private methods
	}
}
