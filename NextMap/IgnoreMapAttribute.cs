using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NextMap
{
	/// <summary>
	/// Attribute will inform the NextMap to ignore the member when creating the mapping configuration. Attribute is
	/// considered when the member is part of the destionation object.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class IgnoreMapAttribute : Attribute
	{
		/// <summary>
		/// Gets the IgnoreOnCopy status. If set to true, destination members with that attribute will be ignored 
		/// for mappings where the destination object is instantiated already. If is true, only for created objects
		/// the destination member will be ignroed.
		/// </summary>
		public bool IgnoreOnCopy { get; private set; }

		/// <summary>
		/// Creates a new instance of IgnoreMapAttribute.
		/// </summary>
		/// <param name="ignoreOnCopy">If set to true, destination members with that attribute will be ignored 
		/// for mappings where the destination object is instantiated already. If is true, only for created objects
		/// the destination member will be ignroed.</param>
		public IgnoreMapAttribute(bool ignoreOnCopy = true)
		{
			IgnoreOnCopy = ignoreOnCopy;
		}
	}
}
