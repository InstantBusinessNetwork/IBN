using System;
using Mediachase.MetaDataPlus.Configurator;


namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Summary description for MetaFieldFix.
	/// </summary>
	public class MetaFieldFix
	{
		private MetaFieldFix()
		{
		}

		public static string GetUniqueName(string name)
		{
			switch(name.ToLower())
			{
				case "id":
				case "objectid":
				case "modifierid":	
				case "modified":
				case "creatorid":
				case "created":
					name += "0";
					break;
			}

			return MetaField.GetUniqueName(name);
		}
	}
}
