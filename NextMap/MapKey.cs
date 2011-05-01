using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NextMap
{
	public class MapKey
	{
		private Type sourceType;
		private Type targetType;

		public MapKey(Type sourceType, Type targetType)
		{
			this.sourceType = sourceType;
			this.targetType = targetType;
		}

		public override bool Equals(object obj)
		{
			if (obj == null) return false;

			if (!(obj is MapKey)) return false;
			MapKey compared = (MapKey)obj;

			return sourceType.Equals(compared.sourceType) && targetType.Equals(compared.targetType);
		}

		public override int GetHashCode()
		{
			return sourceType.GetHashCode() ^ targetType.GetHashCode();
		}
	}
}
