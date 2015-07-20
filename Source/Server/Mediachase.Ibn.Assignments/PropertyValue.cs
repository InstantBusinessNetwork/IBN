using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Mediachase.Ibn.Assignments
{
	/// <summary>
	/// Represents property value.
	/// </summary>
	[XmlInclude(typeof(DynamicValue))]
	[Serializable]
	public class PropertyValue
	{
		#region Const
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyValue"/> class.
		/// </summary>
		public PropertyValue()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyValue"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		public PropertyValue(string name, object value)
		{
			this.Name = name;
			this.Value = value;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		[XmlAttribute(AttributeName="Name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public object Value { get; set; }

		/// <summary>
		/// Gets a value indicating whether this instance is dynamic value.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is dynamic value; otherwise, <c>false</c>.
		/// </value>
		[XmlIgnore]
		public bool IsDynamicValue
		{
			get
			{
				return this.Value is DynamicValue;
			}
		}
		#endregion
	}
}
