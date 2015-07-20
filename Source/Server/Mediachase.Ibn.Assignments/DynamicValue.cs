using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Assignments
{
	/// <summary>
	/// Represents entity property value.
	/// </summary>
	[Serializable]
	public sealed class DynamicValue
	{
		#region Const
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityPropertyValue"/> class.
		/// </summary>
		public DynamicValue()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicValue"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="param">The param.</param>
		public DynamicValue(string type, string param)
		{
			this.Type = type;
			this.Params = param;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		[XmlAttribute(AttributeName="Type")]
		public string Type { get; set; }

		/// <summary>
		/// Gets or sets the params.
		/// </summary>
		/// <value>The params.</value>
		[XmlAttribute(AttributeName = "Params")]
		public string Params { get; set; }
		#endregion

		#region Methods
		public object EvaluateValue(object thisItem)
		{
			object retVal = null;

			retVal = ValueEvaluator.Eval(Type, thisItem, this.Params);

			return retVal;
		}
		#endregion
	}
}
