using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NextMap
{
	/// <summary>
	/// Helper class for name generation.
	/// </summary>
	internal class NameGenerator
	{
		private static int inlineVarIndex;

		public static string GenerateInlineVarName()
		{
			return "var" + GetNextInlineVarIndex();
		}

		private static int GetNextInlineVarIndex()
		{
			return inlineVarIndex++;
		}
	}
}
