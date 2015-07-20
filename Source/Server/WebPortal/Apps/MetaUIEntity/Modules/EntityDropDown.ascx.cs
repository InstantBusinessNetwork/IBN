using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.UI.Web.Util;

namespace Mediachase.Ibn.Web.UI.MetaUIEntity.Modules
{
	public partial class EntityDropDown : System.Web.UI.UserControl
	{
		private const string _scriptKey = "7EA43DF1DD6747f0AAE00B7874EC4533";
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentGeneral", Assembly.GetExecutingAssembly());

		#region ObjectType
		public string ObjectType
		{
			set
			{
				hidSelType.Value = value.ToString();
			}
			get
			{
				return hidSelType.Value;
			}
		}
		#endregion

		#region ObjectId
		public PrimaryKeyId ObjectId
		{
			set
			{
				hidSelId.Value = value.ToString();
			}
			get
			{
				PrimaryKeyId objectId = !String.IsNullOrEmpty(hidSelId.Value) ? PrimaryKeyId.Parse(hidSelId.Value) : PrimaryKeyId.Empty;
				if (objectId != PrimaryKeyId.Empty)
					Mediachase.IBN.Business.Common.AddUsedEntityHistory(ObjectType, objectId, CommonHelper.GetEntityTitle(ObjectType, objectId));
				return objectId;
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

		#region RegisterScript
		private bool _registerScript = true;
		public bool RegisterScript
		{
			set
			{
				_registerScript = value;
			}
			get
			{
				return _registerScript;
			}
		}
		#endregion

		#region XML Define Params
		private string _className = String.Empty;
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

		private string _placeName = String.Empty;
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

		private string _commandName = "MC_MUI_EntityDD";
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

		#region FilterName
		public string FilterName
		{
			set
			{
				ViewState["_filterName"] = value;
			}
			get
			{
				if (ViewState["_filterName"] != null)
					return ViewState["_filterName"].ToString();
				else
					return String.Empty;
			}
		}
		#endregion

		#region FilterValue
		public string FilterValue
		{
			set
			{
				ViewState["_filterValue"] = value;
			}
			get
			{
				if (ViewState["_filterValue"] != null)
					return ViewState["_filterValue"].ToString();
				else
					return String.Empty;
			}
		}
		#endregion

		#region TabIndex
		public short TabIndex
		{
			set
			{
				lblSelected.TabIndex = value;
				lblChange.TabIndex = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/objectDD.css");
			if (RegisterScript)
				UtilHelper.RegisterScript(Page, "~/Scripts/entityDD.js");

			CommandManager cm = CommandManager.GetCurrent(this.Page);
			if (cm != null)
				cm.AddCommand(ClassName, ViewName, PlaceName, CommandName);
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			AddAttributes();

			BindList();
			BindSelectedValue();

			string registerControl = String.Format("MC_EDD['{0}'] = new MCEntityDD(\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\", \"{6}\", \"{7}\");",
						this.ClientID, tblMain.ClientID,
						divDropDown.ClientID,
						tdChange.ClientID, hidSelType.ClientID,
						hidSelId.ClientID, lblSelected.ClientID, OnChange);

			ClientScript.RegisterStartupScript(this.Page, this.Page.GetType(), Guid.NewGuid().ToString(),
				registerControl, true);

			StringBuilder sb = new StringBuilder();
			sb.Append("function EntityDD_Refresh(params){");
			sb.Append("var obj = Sys.Serialization.JavaScriptSerializer.deserialize(params);");
			sb.Append("if(obj && obj.CommandArguments && obj.CommandArguments.SelectCtrlId && obj.CommandArguments.Html && ");
			sb.Append("obj.CommandArguments.SelectObjectType && obj.CommandArguments.SelectObjectId)");
			sb.Append("MC_EDD[obj.CommandArguments.SelectCtrlId].SelectThisHTML(obj.CommandArguments.Html, obj.CommandArguments.SelectObjectType, obj.CommandArguments.SelectObjectId);");
			sb.Append("}");

			ClientScript.RegisterStartupScript(this.Page, this.Page.GetType(), _scriptKey,
				sb.ToString(), true);
		}

		#region AddAttributes
		private void AddAttributes()
		{
			lblChange.Text = String.Format("<img alt='' class='btndown2' src='{0}' />",
				Page.ResolveUrl("~/Layouts/Images/downbtn.gif"));

			if (!ReadOnly)
			{
				tdChange.Attributes.Add("onclick", String.Format("javascript:MC_EDD['{0}'].ShowHideEntityDD(event);", this.ClientID));
				tdValue.Attributes.Add("onclick", String.Format("javascript:MC_EDD['{0}'].ShowHideEntityDD(event);", this.ClientID));
				tdChange.Style.Add("cursor", "pointer");
			}
		}
		#endregion

		#region BindSelectedValue
		private void BindSelectedValue()
		{
			if (!String.IsNullOrEmpty(hidSelId.Value) && PrimaryKeyId.Parse(hidSelId.Value) != PrimaryKeyId.Empty)
				lblSelected.Text = CHelper.GetEntityTitleHtml(ObjectType, PrimaryKeyId.Parse(hidSelId.Value));
			else
				lblSelected.Text = String.Format("<span><img alt='' src='{0}'/></span> <span>{1}</span>",
					Page.ResolveUrl("~/Layouts/Images/not_set.png"),
					LocRM.GetString("tObjectNotSet"));
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
			tc.Attributes.Add("class", "cellclass IconAndText");
			tc.Attributes.Add("onmouseover", "TdOver(this)");
			tc.Attributes.Add("onmouseout", "TdOut(this)");
			tc.Attributes.Add("onclick",
				String.Format("MC_EDD['{0}'].SelectThis(this, '', '{1}');",
					this.ClientID,
					PrimaryKeyId.Empty.ToString()
				)
			);
			tc.InnerHtml = String.Format("<span><img alt='' src='{0}'/></span> <span>{1}</span>",
				Page.ResolveUrl("~/Layouts/Images/not_set.png"),
				LocRM.GetString("tObjectNotSet"));
			tr.Cells.Add(tc);
			tableDD.Rows.Add(tr);

			/// HistoryId, ClassName, ObjectId, ObjectTitle, Dt, IsView
			DataTable dt = Mediachase.IBN.Business.Common.GetListEntityHistoryFull();
			DataView dv = dt.DefaultView;
			//Make filter
			string[] mas = ObjectTypes.Split(',');
			string filter = String.Empty;
			for (int i = 0; i < mas.Length; i++)
			{
				if (!String.IsNullOrEmpty(mas[i]))
				{
					filter += "ClassName='" + mas[i] + "' OR ";
				}
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
				bool fl = true;
				if (!String.IsNullOrEmpty(FilterName))
				{
					fl = false;
					#region Filter
					string className = drv.Row["ClassName"].ToString();
					PrimaryKeyId objId = PrimaryKeyId.Parse(drv.Row["ObjectId"].ToString());
					EntityObject eo = BusinessManager.Load(className, objId);
					if (!String.IsNullOrEmpty(FilterValue))
					{
						if (eo.Properties[FilterName].Value == null ||
							eo.Properties[FilterName].Value.ToString() == FilterValue)
							fl = true;
					}
					else
					{
						if (eo.Properties[FilterName].Value == null)
							fl = true;
					}
					#endregion
				}
				if (fl)
				{
					count++;
					dr = dtClone.NewRow();
					dr.ItemArray = drv.Row.ItemArray;
					dtClone.Rows.Add(dr);
				}
				if (count >= ItemCount)
					break;
			}

			dv = dtClone.DefaultView;
			dv.Sort = "ObjectTitle";
			foreach (DataRowView drv in dv)
			{
				tr = new HtmlTableRow();
				tc = new HtmlTableCell();
				try
				{
					tc.InnerHtml = CommonHelper.GetEntityTitle(drv.Row["ClassName"].ToString(), PrimaryKeyId.Parse(drv.Row["ObjectId"].ToString()));
					tc.Attributes.Add("class", "cellclass");
					tc.Attributes.Add("onmouseover", "TdOver(this)");
					tc.Attributes.Add("onmouseout", "TdOut(this)");
					tc.Attributes.Add("onclick",
						String.Format("MC_EDD['{0}'].SelectThis(this, '{1}', '{2}');",
							this.ClientID, drv.Row["ClassName"].ToString(),
							drv.Row["ObjectId"].ToString()
						)
					);
					tr.Cells.Add(tc);
					tableDD.Rows.Add(tr);
				}
				catch
				{
				}
			}

			tr = new HtmlTableRow();
			tc = new HtmlTableCell();
			tc.InnerHtml = String.Format("<b>{0}</b>", LocRM.GetString("tRespMore"));
			tc.Attributes.Add("class", "cellclass");
			tc.Attributes.Add("onmouseover", "TdOver(this)");
			tc.Attributes.Add("onmouseout", "TdOut(this)");

			CommandManager cm = CommandManager.GetCurrent(this.Page);
			if (cm != null)
			{
				CommandParameters cp = new CommandParameters(CommandName);
				cp.CommandArguments = new Dictionary<string, string>();
				cp.AddCommandArgument("SelectCtrlId", this.ClientID);
				cp.AddCommandArgument("Classes", this.ObjectTypes);
				cp.AddCommandArgument("FilterName", this.FilterName);
				cp.AddCommandArgument("FilterValue", this.FilterValue);
				string cmd = cm.AddCommand(ClassName, ViewName, PlaceName, cp);
				tc.Attributes.Add("onclick", "javascript:entityCloseAll();" + cmd);
				tr.Cells.Add(tc);
				tableDD.Rows.Add(tr);
			}
		}
		#endregion

		public static void RegisterClientScript(Page pageInstance, string className,
			string viewName, string placeName, string commandName)
		{
			CommandManager cm = CommandManager.GetCurrent(pageInstance);
			if (cm != null)
				cm.AddCommand(className, viewName, placeName, commandName);
		}
	}
}
