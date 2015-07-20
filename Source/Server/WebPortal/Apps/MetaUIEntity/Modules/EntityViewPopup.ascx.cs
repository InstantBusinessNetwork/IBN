using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Web.UI.MetaUIEntity.Modules
{
	public partial class EntityViewPopup : System.Web.UI.UserControl
	{
		#region FormName
		protected string FormName
		{
			get
			{
				string retval = "[MC_BaseForm]";
				if (Request.QueryString["formName"] != null)
					retval = Request.QueryString["formName"];
				return retval;
			}
		}
		#endregion

		#region ClassName
		protected string ClassName
		{
			get
			{
				string retval = String.Empty;
				if (Request.QueryString["className"] != null)
					retval = Request.QueryString["className"];
				return retval;
			}
		}
		#endregion

		#region ObjectId
		protected PrimaryKeyId ObjectId
		{
			get
			{
				PrimaryKeyId retval = PrimaryKeyId.Empty;
				if (Request.QueryString["ObjectId"] != null)
					retval = PrimaryKeyId.Parse(Request.QueryString["ObjectId"]);
				return retval;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				BindData();
			}
		}

		#region BindData
		private void BindData()
		{
			EntityObject entity;
			if (ObjectId != PrimaryKeyId.Empty)
				entity = BusinessManager.Load(ClassName, ObjectId);
			else
				entity = BusinessManager.InitializeEntity(ClassName);

			EditForm.FormName = FormName;
			EditForm.DataItem = entity;
			EditForm.DataBind();
		}
		#endregion
	}
}