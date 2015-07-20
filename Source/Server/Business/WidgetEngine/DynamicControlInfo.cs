using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.IBN.Business.WidgetEngine
{
	[Serializable]
	public class DynamicControlInfo
	{
		private string _uid = string.Empty;
		private string _title = string.Empty;
		private string _description;
		private string _iconPath;
		private string _category = string.Empty;

		private string _path;
		private string _type;

		private string _adapterPath;
		private string _adapterType;

		private string _propertyPagePath;
		private string _propertyPageType;

		#region Properties

		#region Control
		/// <summary>
		/// Gets or sets the Uid.
		/// </summary>
		/// <value>The Uid.</value>
		public string Uid
		{
			get
			{
				return _uid;

			}
			set
			{
				_uid = value;
			}
		}

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public string Title
		{
			get
			{
				return _title;
			}
			set
			{
				_title = value;
			}
		}

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

		/// <summary>
		/// Gets or sets the icon file.
		/// </summary>
		/// <value>The icon file.</value>
		public string IconPath
		{
			get
			{
				return _iconPath;
			}

			set
			{
				_iconPath = value;
			}
		}

		/// <summary>
		/// Gets or sets the category.
		/// </summary>
		/// <value>The category.</value>
		public string Category
		{
			get
			{
				return _category;
			}
			set
			{
				_category = value;
			}
		}

		/// <summary>
		/// Gets or sets the path.
		/// </summary>
		/// <value>The path.</value>
		public string Path
		{
			get
			{
				return _path;
			}
			set
			{
				_path = value;
			}
		}

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		public string Type
		{
			get { return _type; }
			set { _type = value; }
		}

		#region SmallThumbnail
		private string _smallThumbnail;
		public string SmallThumbnail
		{
			get { return _smallThumbnail; }
			set { _smallThumbnail = value; }
		}
		#endregion

		#region LargeThumbnail
		private string _largeThumbnail;
		public string LargeThumbnail
		{
			get { return _largeThumbnail; }
			set { _largeThumbnail = value; }
		}
		#endregion

		#endregion

		#region Adapter
		/// <summary>
		/// Gets or sets the adapter path.
		/// </summary>
		/// <value>The adapter path.</value>
		public string AdapterPath
		{
			get
			{
				return _adapterPath;
			}
			set
			{
				_adapterPath = value;
			}
		}

		/// <summary>
		/// Gets or sets the type of the adapter.
		/// </summary>
		/// <value>The type of the adapter.</value>
		public string AdapterType
		{
			get { return _adapterType; }
			set { _adapterType = value; }
		}
		#endregion

		#region Property Page
		/// <summary>
		/// Gets or sets the property page path.
		/// </summary>
		/// <value>The property page path.</value>
		public string PropertyPagePath
		{
			get
			{
				return _propertyPagePath;
			}
			set
			{
				_propertyPagePath = value;
			}
		}

		/// <summary>
		/// Gets or sets the type of the property page.
		/// </summary>
		/// <value>The type of the property page.</value>
		public string PropertyPageType
		{
			get { return _propertyPageType; }
			set { _propertyPageType = value; }
		}
		#endregion

		#endregion
	}
}
