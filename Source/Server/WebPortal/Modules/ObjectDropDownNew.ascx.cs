using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Resources;
using Mediachase.Ibn.Web.UI.WebControls;
using System.Collections.Generic;
using System.Text;
using Mediachase.IBN.Business;

namespace Mediachase.UI.Web.Modules
{
	public partial class ObjectDropDownNew : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentGeneral", typeof(ObjectDropDownNew).Assembly);
		private const string _scriptKey = "D80763DA8C24437a811F43FA276FFE90";

		#region ObjectTypeId
		public int ObjectTypeId
		{
			set
			{
				hidSelType.Value = value.ToString();
			}
			get
			{
				return (hidSelType.Value != "") ? int.Parse(hidSelType.Value) : -1;
			}
		}
		#endregion

		#region ObjectId
		public int ObjectId
		{
			set
			{
				hidSelId.Value = value.ToString();
			}
			get
			{
				int _temp = (hidSelId.Value != "") ? int.Parse(hidSelId.Value) : -1;
				if (_temp > 0)
					Mediachase.IBN.Business.Common.AddUsedHistory(ObjectTypeId, _temp, Util.CommonHelper.GetObjectTitle(ObjectTypeId, _temp));
				return _temp;
			}
		}
		#endregion

		#region ObjectTypes
		public string ObjectTypes
		{
			set
			{
				hidTypes.Value = value;
			}
			get
			{
				return hidTypes.Value;
			}
		}
		#endregion

		#region OnChange
		private string _onChange;
		public string OnChange
		{
			set
			{
				_onChange = value;
			}
			get
			{
				return _onChange;
			}
		}
		#endregion

		#region Width
		public string Width
		{
			set
			{
				tblMain.Width = value;
			}
			get
			{
				return tblMain.Width;
			}
		}
		#endregion

		#region ItemCount
		private int _itemCount = 10;
		public int ItemCount
		{
			set
			{
				_itemCount = value;
			}
			get
			{
				return _itemCount;
			}
		}
		#endregion

		#region ReadOnly
		private bool _readOnly = false;
		public bool ReadOnly
		{
			set
			{
				_readOnly = value;
			}
			get
			{
				return _readOnly;
			}
		}
		#endregion

		#region BackgroundColor
		public string BackgroundColor
		{
			set
			{
				tdValue.Style.Add(HtmlTextWriterStyle.BackgroundColor, value);
			}
		}
		#endregion

		#region ClassName
		private string _className = "Project";
		public string ClassName
		{
			get
			{
				return _className;
			}
			set
			{
				_className = value;
			}
		}
		#endregion

		#region ViewName
		private string _viewName = String.Empty;
		public string ViewName
		{
			get
			{
				return _viewName;
			}
			set
			{
				_viewName = value;
			}
		}
		#endregion

		#region PlaceName
		private string _placeName = "ProjectView";
		public string PlaceName
		{
			get
			{
				return _placeName;
			}
			set
			{
				_placeName = value;
			}
		}
		#endregion

		#region CommandName
		private string _commandName = "MC_PM_ObjectDD";
		public string CommandName
		{
			get
			{
				return _commandName;
			}
			set
			{
				_commandName = value;
			}
		}
		#endregion

		#region NotSetText
		private string _notSetText = String.Empty;
		public string NotSetText
		{
			get
			{
				return _notSetText;
			}
			set
			{
				_notSetText = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/objectDD.css");
			UtilHelper.RegisterScript(Page, "~/Scripts/objectDD.js");

			CommandManager cm = CommandManager.GetCurrent(this.Page);
			CommandParameters cp = new CommandParameters(CommandName);
			cm.AddCommand(ClassName, ViewName, PlaceName, cp);
		}

		#region AddAttributes
		private void AddAttributes()
		{
			lblChange.Text = String.Format("<img alt='' class='btndown2' src='{0}'/>",
				Page.ResolveUrl("~/Layouts/Images/downbtn.gif"));

			if (!ReadOnly)
			{
				tdChange.Attributes.Add("onclick", String.Format("javascript:MC_ODD['{0}'].ShowHideObjectDD(event);", this.ClientID));
				tdValue.Attributes.Add("onclick", String.Format("javascript:MC_ODD['{0}'].ShowHideObjectDD(event);", this.ClientID));
				tdChange.Style.Add("cursor", "pointer");
			}
		}
		#endregion

		protected void Page_PreRender(object sender, EventArgs e)
		{
			AddAttributes();

			BindList();
			BindSelectedValue();

			string registerControl = String.Format("MC_ODD['{0}'] = new MCObjectDD(\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\", \"{6}\", \"{7}\");",
				this.ClientID, tblMain.ClientID,
				divDropDown.ClientID,
				tdChange.ClientID, hidSelType.ClientID,
				hidSelId.ClientID, lblSelected.ClientID, OnChange);

			ClientScript.RegisterStartupScript(this.Page, this.Page.GetType(), Guid.NewGuid().ToString(),
				registerControl, true);

			StringBuilder sb = new StringBuilder();
			sb.Append("function ObjectDD_Refresh(params){");
			sb.Append("var obj = Sys.Serialization.JavaScriptSerializer.deserialize(params);");
			sb.Append("if(obj && obj.CommandArguments && obj.CommandArguments.SelectCtrlId && obj.CommandArguments.Html && ");
			sb.Append("obj.CommandArguments.SelectObjectTypeId && obj.CommandArguments.SelectObjectId)");
			sb.Append("MC_ODD[obj.CommandArguments.SelectCtrlId].SelectThisHTML(obj.CommandArguments.Html, obj.CommandArguments.SelectObjectTypeId, obj.CommandArguments.SelectObjectId);");
			sb.Append("}");

			ClientScript.RegisterStartupScript(this.Page, this.Page.GetType(), _scriptKey,
				sb.ToString(), true);
		}

		#region BindSelectedValue
		private void BindSelectedValue()
		{
			if (hidSelId.Value != "" && int.Parse(hidSelId.Value) > 0)
				lblSelected.Text = Util.CommonHelper.GetObjectHTMLTitle(ObjectTypeId, int.Parse(hidSelId.Value));
			else
			{
				if (String.IsNullOrEmpty(this.NotSetText))
					lblSelected.Text = String.Format("<span><img alt='' src='{0}'/></span> <span>{1}</span>",
						Page.ResolveUrl("~/Layouts/Images/not_set.png"),
						LocRM.GetString("tObjectNotSet"));
				else
					lblSelected.Text = this.NotSetText;
			}
		}
		#endregion

		#region BindList
		private void BindList()
		{
			tableDD.Rows.Clear();
			HtmlTableRow tr = null;
			HtmlTableCell tc = null;

			tr = new HtmlTableRow();
			tc = new HtmlTableCell();
			if (String.IsNullOrEmpty(this.NotSetText))
				tc.InnerHtml = String.Format("<span><img alt='' src='{0}'/></span> <span>{1}</span>",
					Page.ResolveUrl("~/Layouts/Images/not_set.png"),
					LocRM.GetString("tObjectNotSet"));
			else
				tc.InnerHtml = this.NotSetText;

			tc.Attributes.Add("class", "cellclass");
			tc.Attributes.Add("onmouseover", "TdOver(this)");
			tc.Attributes.Add("onmouseout", "TdOut(this)");
			tc.Attributes.Add("onclick",
			  String.Format("MC_ODD['{0}'].SelectThis(this, -1, -1);",
				this.ClientID)
			);
			tr.Cells.Add(tc);
			tableDD.Rows.Add(tr);

			/// HistoryId, ObjectTypeId, ObjectId, ObjectTitle, Dt, IsView
			DataTable dt = Mediachase.IBN.Business.Common.GetListHistoryFull();
			DataView dv = dt.DefaultView;
			//Make filter
			string[] mas = ObjectTypes.Split(',');
			string filter = "";
			for (int i = 0; i < mas.Length; i++)
			{
				if (mas[i] != "")
					filter += "ObjectTypeId=" + mas[i] + " OR ";
			}
			if (filter.EndsWith(" OR "))
				filter = filter.Substring(0, filter.Length - 4);
			//apply filter
			dv.RowFilter = filter;

			//top ItemCount
			DataTable dtClone = dt.Clone();
			DataRow dr;
			int count = 0;
			foreach (DataRowView drv in dv)
			{
				if (ObjectTypes == "3")	// Project
				{
					int projectId = (int)drv.Row["ObjectId"];
					//check access
					if (!Project.CanRead(projectId))
						continue;
					// skip inactive projects
					using (IDataReader reader = Project.GetProject(projectId, false))
					{
						if (reader.Read())
						{
							int statusId = (int)reader["StatusId"];
							if (statusId == (int)Project.ProjectStatus.OnHold
								|| statusId == (int)Project.ProjectStatus.Completed
								|| statusId == (int)Project.ProjectStatus.Cancelled)
								continue;
						}
					}
				}

				count++;
				dr = dtClone.NewRow();
				dr.ItemArray = drv.Row.ItemArray;
				dtClone.Rows.Add(dr);
				if (count >= ItemCount)
					break;
			}

			dv = dtClone.DefaultView;
			dv.Sort = "ObjectTitle";
			foreach (DataRowView drv in dv)
			{
				tr = new HtmlTableRow();
				tc = new HtmlTableCell();
				tc.InnerHtml = Util.CommonHelper.GetObjectHTMLTitle((int)drv.Row["ObjectTypeId"], (int)drv.Row["ObjectId"]);
				tc.Attributes.Add("class", "cellclass");
				tc.Attributes.Add("onmouseover", "TdOver(this)");
				tc.Attributes.Add("onmouseout", "TdOut(this)");
				tc.Attributes.Add("onclick",
				  String.Format("MC_ODD['{0}'].SelectThis(this, {1}, {2});",
					this.ClientID, drv.Row["ObjectTypeId"].ToString(),
					drv.Row["ObjectId"].ToString())
				);
				tr.Cells.Add(tc);
				tableDD.Rows.Add(tr);
			}

			if (ObjectTypes == "3")		// Project
			{
				tr = new HtmlTableRow();
				tc = new HtmlTableCell();
				tc.InnerHtml = String.Format("<b>{0}</b>", LocRM.GetString("tRespMore"));
				tc.Attributes.Add("class", "cellclass");
				tc.Attributes.Add("onmouseover", "TdOver(this)");
				tc.Attributes.Add("onmouseout", "TdOut(this)");
				CommandManager cm = CommandManager.GetCurrent(this.Page);
				CommandParameters cp = new CommandParameters(CommandName);
				cp.CommandArguments = new Dictionary<string, string>();
				cp.AddCommandArgument("SelectCtrlId", this.ClientID);
				string cmd = cm.AddCommand(ClassName, ViewName, PlaceName, cp);
				tc.Attributes.Add("onclick", "javascript:closeAll();" + cmd);
				tr.Cells.Add(tc);
				tableDD.Rows.Add(tr);
			}

			if (ObjectTypes == "1")		// User
			{
				tr = new HtmlTableRow();
				tc = new HtmlTableCell();
				tc.InnerHtml = String.Format("<b>{0}</b>", LocRM.GetString("tRespMore"));
				tc.Attributes.Add("class", "cellclass");
				tc.Attributes.Add("onmouseover", "TdOver(this)");
				tc.Attributes.Add("onmouseout", "TdOut(this)");
				CommandManager cm = CommandManager.GetCurrent(this.Page);
				CommandParameters cp = new CommandParameters(CommandName);
				cp.CommandArguments = new Dictionary<string, string>();
				cp.AddCommandArgument("SelectCtrlId", this.ClientID);
				string cmd = cm.AddCommand(ClassName, ViewName, PlaceName, cp);
				tc.Attributes.Add("onclick", "javascript:closeAll();" + cmd);
				tr.Cells.Add(tc);
				tableDD.Rows.Add(tr);
			}
		}
		#endregion
	}
}
