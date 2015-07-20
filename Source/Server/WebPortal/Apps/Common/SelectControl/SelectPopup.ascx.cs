using System;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.Ibn.Web.UI
{
	public partial class SelectPopup : System.Web.UI.UserControl
	{
		#region ClassName
		public string ClassName
		{
			get { return (ViewState["__className"] == null ? "" : ViewState["__className"].ToString()); }
			set { ViewState["__className"] = value; }
		} 
		#endregion

		#region CardFilter
		public string CardFilter
		{
			get { return (ViewState["__cardFilter"] == null ? "" : ViewState["__cardFilter"].ToString()); }
			set { ViewState["__cardFilter"] = value; }
		}
		#endregion

		#region _ownerControl
		private string _ownerControl
		{
			get { return (ViewState["__ownerControl"] == null ? "" : ViewState["__ownerControl"].ToString()); }
			set { ViewState["__ownerControl"] = value; }
		} 
		#endregion

		protected string TitleFieldName;

		protected void Page_Load(object sender, EventArgs e)
		{
			TitleFieldName = MetaDataWrapper.GetMetaClassByName(ClassName).TitleFieldName;
			hfClassName.ValueChanged += new EventHandler(hfClassName_ValueChanged);
			tbMainExtender.XPath = "tbody tr";
			tbMainExtender.XPathId = "td input[type=image]";
			tbMainExtender.IdAttributeName = "onclick";
			tbMainExtender.ListContainer = GridMain.ClientID;
			tbMainExtender.CssSelected = "TextBoxListSelected";
			tbMain.TextChanged += new EventHandler(tbMain_TextChanged);

/*			if (!IsPostBack)
				BindGrid(string.Empty);
*/
			tbMain.Attributes.Add("autocomplete", "off");

			if (!Page.ClientScript.IsClientScriptBlockRegistered("MC_SelectPopup_js"))
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "MC_SelectPopup_js", 
					String.Format("<script language=\"javascript\" type=\"text/javascript\" src=\"{0}\"></script>",
						CHelper.GetAbsolutePath("/Apps/Common/Scripts/SelectPopupScript.js")));

			divClose.Attributes.Add("onclick", String.Format("javascript:MC_SELECT_POPUPS['{0}'].hideSelectPopup();", this.ClientID));
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			string _registerControl = String.Format("MC_SELECT_POPUPS['{0}'] = new MCSelectPopup(\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\");",
					   this.ClientID, hfClassName.ClientID,
					   divQuickAdd.ClientID, backgroundContainer.ClientID,
					   tbMain.ClientID);

			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), _registerControl, true);
		}

		#region hfClassName_ValueChanged
		protected void hfClassName_ValueChanged(object sender, EventArgs e)
		{
			_ownerControl = hfClassName.Value;
			BindGrid(tbMain.Text);
			upSelectPanel.Update();
		} 
		#endregion

		#region tbMain_TextChanged
		void tbMain_TextChanged(object sender, EventArgs e)
		{
			BindGrid(tbMain.Text);
			upSelectPanel.Update();
		} 
		#endregion

		#region BindGrid
		void BindGrid(string text)
		{
			if (String.IsNullOrEmpty(ClassName))
				return;

			int total = 0;
			GridMain.DataSource = genSource(text, out total);
			GridMain.DataBind();

			foreach (GridViewRow gvr in GridMain.Rows)
			{
				ImageButton btn = (ImageButton)gvr.FindControl("btnSave");
				btn.Attributes.Add("onclick", String.Format("javascript:MC_SELECT_POPUPS['{2}'].selectObject('{0}', '{1}');", _ownerControl, btn.CommandArgument, this.ClientID));
				btn.Style.Add("display", "none");
			}
			lblTotalCount.Text = "<b>Total:</b> " + total + " items.";
		} 
		#endregion

		#region genSource
		private DataTable genSource(string text, out int totalCount)
		{
			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
			dt.Columns.Add(new DataColumn("PrimaryKeyId", typeof(int)));
			dt.Columns.Add(new DataColumn(TitleFieldName, typeof(string)));
			MetaClass mc = MetaDataWrapper.ResolveMetaClassByNameOrCardName(ClassName);

			FilterElementCollection fec = new FilterElementCollection();
			fec.Add(new FilterElement(mc.TitleFieldName, FilterElementType.Contains, text));
			if (!String.IsNullOrEmpty(CardFilter))
			{
				string[] mas = CardFilter.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
				if(mas.Length>0)
				{
					OrBlockFilterElement fe = new OrBlockFilterElement();
					foreach(string sCard in mas)
						fe.ChildElements.Add(FilterElement.EqualElement("Card", sCard.Trim()));
					fec.Add(fe);
				}
			}

			MetaObject[] list = MetaObject.List(mc,
					fec,
					new SortingElementCollection(SortingElement.Ascending(mc.TitleFieldName)));
			int count = 0;
			foreach (MetaObject bo in list)
			{
				DataRow row = dt.NewRow();
				row["PrimaryKeyId"] = bo.PrimaryKeyId;
				row[TitleFieldName] = bo.Properties[mc.TitleFieldName].Value.ToString();
				dt.Rows.Add(row);
				count++;
				if (count > 10)
					break;
			}
			totalCount = list.Length;

			return dt;
		} 
		#endregion
	}
}