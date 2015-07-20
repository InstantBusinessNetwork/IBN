using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.MetaUIEntity.Modules
{
	public partial class EntityListLight : MCDataBoundControl
	{
		#region ClassName
		public string ClassName
		{
			get
			{
				string retval = String.Empty;
				if (ViewState["ClassName"] != null)
					retval = (string)ViewState["ClassName"];
				return retval;
			}
			set
			{
				ViewState["ClassName"] = value;
			}
		}
		#endregion

		#region ViewName
		public string ViewName
		{
			get
			{
				string retval = String.Empty;
				if (ViewState["ViewName"] != null)
					retval = (string)ViewState["ViewName"];
				return retval;
			}
			set
			{
				ViewState["ViewName"] = value;
			}
		}
		#endregion

		#region PlaceName
		public string PlaceName
		{
			get
			{
				string retval = String.Empty;
				if (ViewState["PlaceName"] != null)
					retval = (string)ViewState["PlaceName"];
				return retval;
			}
			set
			{
				ViewState["PlaceName"] = value;
			}
		}
		#endregion

		#region ProfileName
		public string ProfileName
		{
			get
			{
				string retval = String.Empty;
				if (ViewState["ProfileName"] != null)
					retval = (string)ViewState["ProfileName"];
				return retval;
			}
			set
			{
				ViewState["ProfileName"] = value;
			}
		}
		#endregion

		#region ShowToolbar
		public bool ShowToolbar
		{
			get
			{
				return ToolbarRow.Visible;
			}
			set
			{
				ToolbarRow.Visible = value;
			}
		}
		#endregion

		#region DoPadding
		public bool DoPadding
		{
			get
			{
				if (ViewState["__doPadding"] == null)
					ViewState["__doPadding"] = true;
				return (bool)ViewState["__doPadding"];
			}
			set
			{
				ViewState["__doPadding"] = value;
			}
		}
		#endregion

		#region ShowCheckBoxes
		public bool ShowCheckBoxes
		{
			get
			{
				if (ViewState["__showCheckBoxes"] == null)
					ViewState["__showCheckBoxes"] = true;
				return (bool)ViewState["__showCheckBoxes"];
			}
			set
			{
				ViewState["__showCheckBoxes"] = value;
			}
		}
		#endregion

		#region FilterFieldName
		public string FilterFieldName
		{
			get
			{
				string retval = String.Empty;
				if (ViewState["FilterFieldName"] != null)
					retval = (string)ViewState["FilterFieldName"];
				return retval;
			}
			set
			{
				ViewState["FilterFieldName"] = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.ClientScript.IsClientScriptBlockRegistered("grid.css"))
			{
				string cssLink = String.Format(CultureInfo.InvariantCulture,
					"<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\" />",
					Mediachase.Ibn.Web.UI.WebControls.McScriptLoader.Current.GetScriptUrl("~/Styles/IbnFramework/grid.css", this.Page));
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "grid.css", cssLink);
			}

			StringBuilder sb = new StringBuilder();
			sb.Append("function SelectItems_Refresh(params){");
			sb.Append("var obj = Sys.Serialization.JavaScriptSerializer.deserialize(params);");
			sb.Append("if(obj && obj.CommandArguments && obj.CommandArguments.SelectedValue)");
			sb.AppendFormat("__doPostBack('{0}', obj.CommandArguments.SelectedValue);", lbAddItems.UniqueID);
			sb.Append("}");
			ClientScript.RegisterStartupScript(this.Page, this.Page.GetType(), Guid.NewGuid().ToString("N"),
				sb.ToString(), true);
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (DoPadding)
				mainDiv.Style.Add(HtmlTextWriterStyle.Padding, "5px");
			base.OnPreRender(e);
		}

		#region DataBind
		public override void DataBind()
		{
			grdMain.ClassName = ClassName;
			grdMain.ViewName = ViewName;
			grdMain.PlaceName = PlaceName;

			if (!String.IsNullOrEmpty(ProfileName))
				grdMain.ProfileName = ProfileName;

			grdMain.ShowCheckboxes = ShowCheckBoxes;
			grdMain.PageSize = -1;
			grdMain.DataBind();

			ctrlGridEventUpdater.ClassName = ClassName;
			ctrlGridEventUpdater.ViewName = ViewName;
			ctrlGridEventUpdater.PlaceName = PlaceName;
			ctrlGridEventUpdater.GridId = grdMain.GridClientContainerId;

			MainMetaToolbar.ClassName = ClassName;
			MainMetaToolbar.ViewName = ViewName;
			MainMetaToolbar.PlaceName = PlaceName;
			MainMetaToolbar.DataBind();

		}
		#endregion

		#region lbAddItems_Click
		protected void lbAddItems_Click(object sender, EventArgs e)
		{
			string s = Request["__EVENTARGUMENT"];
			if (!String.IsNullOrEmpty(s))
			{
				string[] mas = s.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
				PrimaryKeyId objId = PrimaryKeyId.Parse(Request["ObjectId"]);
				foreach (string item in mas)
				{
					PrimaryKeyId id = PrimaryKeyId.Parse(MetaViewGroupUtil.GetIdFromUniqueKey(item));
					string className = MetaViewGroupUtil.GetMetaTypeFromUniqueKey(item);

					if (!String.IsNullOrEmpty(className) && className != MetaViewGroupUtil.keyValueNotDefined)
					{
						EntityObject eo = BusinessManager.Load(className, id);
						if (eo != null && !String.IsNullOrEmpty(FilterFieldName) &&
							eo.Properties[FilterFieldName] != null)
						{
							eo[FilterFieldName] = objId;
							BusinessManager.Update(eo);
						}
					}
				}
				CHelper.RequireDataBind();
			}
		}
		#endregion
	}
}
