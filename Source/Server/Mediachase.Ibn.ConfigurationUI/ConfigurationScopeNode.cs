using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ManagementConsole;

namespace Mediachase.Ibn.ConfigurationUI
{
	/// <summary>
	/// Represents configuration scope node.
	/// </summary>
	public class ConfigurationScopeNode: ScopeNode
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="ConfigurationScopeNode"/> class.
		/// </summary>
		public ConfigurationScopeNode()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConfigurationScopeNode"/> class.
		/// </summary>
		/// <param name="hideExpandIcon">if set to <c>true</c> [hide expand icon].</param>
		public ConfigurationScopeNode(bool hideExpandIcon): base(hideExpandIcon)
		{
		}

		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the type of the data element.
		/// </summary>
		/// <value>The type of the data element.</value>
		public DataElementType DataElementType { get; set; }
		#endregion

		#region Methods
		#endregion

		
	}
}
