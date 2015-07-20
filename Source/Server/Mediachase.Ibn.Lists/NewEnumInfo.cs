using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Lists
{
	/// <summary>
	/// Represents a new meta enum information for list import.
	/// </summary>
	[Serializable]
	public class NewEnumInfo
	{
		#region Const
		#endregion

		#region Fields

		private string _name;
		private string _fn;
		private bool _editable;
		private bool _isPrivate;
		private bool _multiValue;

		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="NewEnumInfo"/> class.
		/// </summary>
		public NewEnumInfo()
		{
		}

		public NewEnumInfo(string name, string friendlyName, bool multipleValues, bool editable, bool isPrivate)
		{
			this.Name = name;
			this.FriendlyName = friendlyName;
			this.MultipleValues = multipleValues;
			this.Editable = editable;
			this.IsPrivate = isPrivate;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// Gets or sets the name of the friendly.
		/// </summary>
		/// <value>The name of the friendly.</value>
		public string FriendlyName
		{
			get { return _fn; }
			set { _fn = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="NewEnumInfo"/> is editable.
		/// </summary>
		/// <value><c>true</c> if editable; otherwise, <c>false</c>.</value>
		public bool Editable
		{
			get { return _editable; }
			set { _editable = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="NewEnumInfo"/> is private.
		/// </summary>
		/// <value><c>true</c> if private; otherwise, <c>false</c>.</value>
		public bool IsPrivate
		{
			get { return _isPrivate; }
			set { _isPrivate = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether [multi value].
		/// </summary>
		/// <value><c>true</c> if [multi value]; otherwise, <c>false</c>.</value>
		public bool MultipleValues
		{
			get { return _multiValue; }
			set { _multiValue = value; }
		}

		#endregion

		#region Methods
		#endregion


	}
}
