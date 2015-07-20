using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Mediachase.Ibn.Assignments
{
	/// <summary>
	/// Represents dynamic value collection.
	/// </summary>
	[Serializable]
	public class PropertyValueCollection: List<PropertyValue>
	{
		#region Const
		#endregion

		#region Properties
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicValueCollection"/> class.
		/// </summary>
		public PropertyValueCollection()
		{
		}
		#endregion

		//private void Serialzie()
		//{
		//    PropertyValueCollection col = new PropertyValueCollection();
		//    col.Add(new PropertyValue("aaaa", "aaaa"));
		//    col.Add(new PropertyValue("aaaa", new DynamicValue("aaaa", "bbbb")));
		//    col.Add(new PropertyValue("aaaa", new DynamicValue("eeee", null)));

		//    string str = McXmlSerializer.GetString<PropertyValueCollection>(col);

		//    Trace.WriteLine(str);
		//}

		#region Methods
		/// <summary>
		/// Gets the <see cref="Mediachase.Ibn.Assignments.DynamicValue"/> with the specified name.
		/// </summary>
		/// <value></value>
		public object this[string name]
		{
			get
			{
				foreach (PropertyValue item in this)
				{
					if (item.Name == name)
						return item.Value;
				}

				return null;
			}
			set
			{
				foreach (PropertyValue item in this)
				{
					if (item.Name == name)
					{
						item.Value = value;
						return;
					}
				}
				this.Add(new PropertyValue(name, value));
			}
		}


		/// <summary>
		/// Determines whether [contains] [the specified name].
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>
		/// 	<c>true</c> if [contains] [the specified name]; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(string name)
		{
			foreach (PropertyValue item in this)
			{
				if (item.Name == name)
					return true;
			}

			return false;
		}

		#endregion

		
	}
}
