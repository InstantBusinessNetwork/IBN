using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.ConfigurationUI
{
	/// <summary>
	/// Represents ScopeNodeIdentifier.
	/// </summary>
	public class DataElement
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="ScopeNodeIdentifier"/> class.
		/// </summary>
		public DataElement()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ScopeNodeIdentifier"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="displayName">The display name.</param>
		public DataElement(DataElementType type, string displayName)
		{
			this.Type = type;
			this.DisplayName = displayName;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ScopeNodeIdentifier"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="id">The id.</param>
		/// <param name="displayName">The display name.</param>
		public DataElement(DataElementType type, string id, string displayName)
		{
			this.Type = type;
			this.Id = id;
			this.DisplayName = displayName;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		public DataElementType Type { get; set; }

		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the display name.
		/// </summary>
		/// <value>The display name.</value>
		public string DisplayName { get; set; }

		#endregion

		#region Methods
		#endregion

		
	}
}
