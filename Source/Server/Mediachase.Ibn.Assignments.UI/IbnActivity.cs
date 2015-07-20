using System;
using System.Collections.Generic;
using System.Text;
using System.Workflow.ComponentModel;

namespace Mediachase.Ibn.Assignments.UI
{
	
	public class IbnActivity
	{

		#region prop: Name
		private string _name;
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		} 
		#endregion

		#region prop: Description
		private string _description;
		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				_description = value;
			}
		}
		#endregion

		#region prop: HasChildNodes
		private bool _hasChildNodes;
		/// <summary>
		/// Gets or sets a value indicating whether this instance has child nodes.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has child nodes; otherwise, <c>false</c>.
		/// </value>
		public bool HasChildNodes
		{
			get
			{
				return _hasChildNodes;
			}
			set
			{
				_hasChildNodes = value;
			}
		}
		#endregion

		#region .ctor()
		public IbnActivity()
		{
		}

		public IbnActivity(string Name)
			: this()
		{
			this.Name = Name;
		}

		public IbnActivity(string Name, string Description)
			: this(Name)
		{
			this.Description = Description;
		} 
		#endregion

	}
	
}
