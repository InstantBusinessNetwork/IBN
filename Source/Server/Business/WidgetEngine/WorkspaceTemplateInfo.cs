using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Mediachase.IBN.Business.WidgetEngine
{
	[Serializable]
	public class WorkspaceTemplateInfo
	{
		private string _uid = string.Empty;
		private string _title = string.Empty;
		private string _description;		
		private string _columnInfo;
		private string _imageUrl;

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
		/// Gets or sets the path.
		/// </summary>
		/// <value>The path.</value>
		public string ColumnInfo
		{
			get
			{
				return _columnInfo;
			}
			set
			{
				_columnInfo = value;
			}
		}

		/// <summary>
		/// Gets or sets the image URL.
		/// </summary>
		/// <value>The image URL.</value>
		public string ImageUrl
		{
			get
			{
				return _imageUrl;
			}
			set
			{
				_imageUrl = value;
			}
		}
		#endregion

		#endregion
	}
}
