using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Resources;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.IBN.Business;
using System.Reflection;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Common
{
	/// <summary>
	/// Summary description for AddCategory.
	/// </summary>
	public partial class AddCategory : System.Web.UI.Page
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strManageDictionaries", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
	
		#region iDictionaryType
		public int iDictionaryType
		{
			get
			{
				try
				{
					return int.Parse(Request["DictType"]);
				}
				catch
				{
					throw new Exception("Wrong Dictionary Type!");
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");

			lblError.Visible = false;

			BindToolbar();
			if (!IsPostBack)
			{
				if ((DictionaryTypes)iDictionaryType != DictionaryTypes.ProjectPhases)
					dgDic.Columns[2].Visible=false;
				else
					dgDic.Columns[2].Visible=true;
				btnAddNewItem.Text = LocRM.GetString("AddNewItem");
				BindDG();
			}
		}

		#region BindToolbar
		private void BindToolbar()
		{
			switch(iDictionaryType)
			{
//				case (int)DictionaryTypes.Clients:
//					secHeader.Title = LocRM.GetString("Clients");
//					break;
				case (int)DictionaryTypes.Categories:
					secHeader.Title = LocRM.GetString("Categories");
					break;
				case (int)DictionaryTypes.ProjectCategories:
					secHeader.Title = LocRM.GetString("ProjectCategories");
					break;
				case (int)DictionaryTypes.IncidentCategories:
					secHeader.Title = LocRM.GetString("IncidentCategories");
					break;	
				case (int)DictionaryTypes.ProjectTypes:
					secHeader.Title = LocRM.GetString("ProjectTypes");
					break;	
				case (int)DictionaryTypes.ProjectPhases:
					secHeader.Title = LocRM.GetString("ProjectPhases");
					break;	
				case (int)DictionaryTypes.ListTypes:
					secHeader.Title = LocRM.GetString("ListTypes");
					break;	
				case (int)DictionaryTypes.IncidentTypes:
					secHeader.Title = LocRM.GetString("IncidentCategories");
					break;	
				case (int)DictionaryTypes.IncidentSeverities:
					secHeader.Title = LocRM.GetString("IncidentSeverities");
					break;	
			}
			secHeader.AddLink("<img alt='' src='../Layouts/Images/newitem.gif'/> " + LocRM.GetString("AddNewItem"),Page.ClientScript.GetPostBackClientHyperlink(btnAddNewItem,""));
			secHeader.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("Exit"),"javascript:FuncSave();");
		}
		#endregion

		#region BindDG
		private void BindDG()
		{
			dgDic.Columns[1].HeaderText = LocRM.GetString("Name");
			dgDic.Columns[2].HeaderText = LocRM.GetString("tWeight");
			dgDic.Columns[3].HeaderText = LocRM.GetString("Options");

			DictionaryTypes dic = (DictionaryTypes)Enum.Parse(typeof(DictionaryTypes) ,iDictionaryType.ToString());
			
			DataView dv = Dictionaries.GetList(dic).DefaultView;
			if ((DictionaryTypes)iDictionaryType != DictionaryTypes.ProjectPhases)
				dv.Sort = "ItemName";
			else
				dv.Sort = "Weight";
			dgDic.DataSource = dv;
			dgDic.DataBind();

			foreach (DataGridItem dgi in dgDic.Items)
			{
				ImageButton ib=(ImageButton)dgi.FindControl("ibDelete");
				if (ib!=null)
					ib.Attributes.Add("onclick","return confirm('" + LocRM.GetString("Warning") +"')");

				RequiredFieldValidator rf = (RequiredFieldValidator)dgi.FindControl("rfName");
				if (rf!=null)
					rf.ErrorMessage = LocRM.GetString("Required");
			}
		}
		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.dgDic.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_cancel);
			this.dgDic.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_edit);
			this.dgDic.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_update);
			this.dgDic.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_delete);

		}
		#endregion

		#region GetVisibleStatus
		protected bool GetVisibleStatus(object val)
		{
			int status = (int)val;
			if (status == 0) 
				return false;
			else
				return true;
		}
		#endregion

		#region dg_delete
		private void dg_delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int ItemID = int.Parse(e.Item.Cells[0].Text);
			DictionaryTypes dic = (DictionaryTypes)Enum.Parse(typeof(DictionaryTypes) ,iDictionaryType.ToString());
			Dictionaries.DeleteItem(ItemID,dic);
			dgDic.EditItemIndex = -1;
			BindDG();
		}
		#endregion

		#region dg_edit
		private void dg_edit(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			dgDic.EditItemIndex = e.Item.ItemIndex;
			BindDG();			
		}
		#endregion

		#region dg_cancel
		private void dg_cancel(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			dgDic.EditItemIndex = -1;
			BindDG();	
		}
		#endregion

		#region dg_update
		private void dg_update(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int ItemID = int.Parse(e.Item.Cells[0].Text);
			TextBox tb = (TextBox)e.Item.FindControl("tbName");
			int _weight = 0;
			TextBox tbW = (TextBox)e.Item.FindControl("tbWeight");
			if(tbW!=null && tbW.Text.Length>0)
				_weight = int.Parse(tbW.Text);
			if (tb!=null)
			{
				try
				{
					if (ItemID>0)
						Dictionaries.UpdateItem(ItemID,tb.Text, _weight, (DictionaryTypes)iDictionaryType);
					else
						Dictionaries.AddItem(tb.Text, _weight, (DictionaryTypes)iDictionaryType);
				}
				catch (SqlException ex)
				{
					if (ex.Number == 2627)	// Violation of UNIQUE KEY
					{
						lblError.Visible = true;
						lblError.Text = LocRM.GetString("Duplication") + "<br>";
					}
				}
			}

			dgDic.EditItemIndex = -1;
			BindDG();
		}
		#endregion

		#region btnAddNewItem_Click
		protected void btnAddNewItem_Click(object sender, System.EventArgs e)
		{
			DataTable dt = Dictionaries.GetList((DictionaryTypes)iDictionaryType);
			
			DataRow dr = dt.NewRow();
			dr["ItemId"] = -1;
			dr["ItemName"] = "";
			dr["Weight"] = "0";
			int i = -1; dr["CanDelete"] = i;
			dt.Rows.Add(dr);

			dgDic.EditItemIndex = dt.Rows.Count-1;
			dgDic.DataSource = dt.DefaultView;
			dgDic.DataBind();
		}
		#endregion

		#region btnSave_ServerClick
		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			if (!String.IsNullOrEmpty(Request["BtnID"]))
			{
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					"try {window.opener.document.forms[0]." + Request["BtnID"] + ".click();}" +
					"catch (e){} window.close();", true);
			}
			else
			{
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					"try {window.opener.top.frames['right'].document.forms[0].submit();}" +
					"catch (e){} window.close();", true);
			}
		}
		#endregion
	}
}
