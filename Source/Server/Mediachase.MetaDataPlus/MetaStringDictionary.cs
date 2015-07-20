using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Runtime.Serialization;

namespace Mediachase.MetaDataPlus
{
	/// <summary>
	/// Summary description for MetaStringDictionary.
	/// </summary>
	[Serializable]
	public class MetaStringDictionary : Dictionary<string, string>
	{
		public MetaStringDictionary()
			: base(new CaseInsensitiveEqualityComparer())
		{
		}

		protected MetaStringDictionary(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		internal void LoadDictionary(IDataReader reader)
		{
			do
			{
				this.Add((string)reader["Key"], (string)reader["Value"]);
			}
			while (reader.Read());
		}
	}

	internal class CaseInsensitiveEqualityComparer : IEqualityComparer<string>
	{
		#region IEqualityComparer<string> Members

		public bool Equals(string x, string y)
		{
			return (0 == string.Compare(x, y, true, CultureInfo.InvariantCulture));
		}

		public int GetHashCode(string obj)
		{
			return obj.ToLowerInvariant().GetHashCode();
		}

		#endregion
	}
}
