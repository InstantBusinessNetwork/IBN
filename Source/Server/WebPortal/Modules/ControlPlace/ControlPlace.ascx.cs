namespace ControlPlaceApplication
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Resources;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.WebSaltatoryControl;
	using Mediachase.UI.Web.Modules;
	using System.Reflection;

	/// <summary>
	///		Summary description for ControlPlace.
	/// </summary>
	public partial class ControlPlace : System.Web.UI.UserControl, IWebControlPlace
	{
		//public new System.Web.UI.ControlCollection Controls;

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		#region CustomizeAllow
		private bool customizeallow = false;
		public bool CustomizeAllow
		{
			set
			{
				customizeallow = value;
			}
			get
			{
				return customizeallow;
			}
		}
		#endregion

		#region ShowActivePlace
		private bool showactiveplace = false;
		public bool ShowActivePlace
		{
			set
			{
				showactiveplace = value;
			}
			get
			{
				return showactiveplace;
			}
		}
		#endregion

		#region PropertyPageUrl
		private string propertypageurl = "popup.aspx";
		public string PropertyPageUrl
		{
			set
			{
				propertypageurl = value;
			}
			get
			{
				return propertypageurl;
			}
		}
		#endregion

		#region Path_Img
		private string path_img = "";
		public string Path_Img
		{
			set
			{
				string ValueToSet = value;
				if (ValueToSet.Length > 0)
				{
					if (ValueToSet.Substring(ValueToSet.Length - 1, 1) != "/") ValueToSet += "/";
				}
				path_img = ValueToSet;
			}
			get
			{
				return path_img;
			}
		}
		#endregion

		#region ClassName
		public string ClassName
		{
			get
			{
				return ControlManager.CurrentView.Id;
			}
		}
		#endregion

		#region GetObjectId()
		private int GetObjectId()
		{
			string ObjectName = "";
			if (this.ClassName.StartsWith("ProjectsEx_"))
				ObjectName = "ProjectId";
			if (this.ClassName.StartsWith("ListsEx_"))
				ObjectName = "ID";
			switch (this.ClassName)
			{
				case "UsersEx":
					ObjectName = "UserId";
					break;
				case "IncidentsEx":
					ObjectName = "IncidentId";
					break;
				case "DocumentsEx":
					ObjectName = "DocumentId";
					break;
				case "TaskEx":
					ObjectName = "TaskId";
					break;
				case "ToDoEx":
					ObjectName = "ToDoId";
					break;
				case "EventsEx":
					ObjectName = "EventId";
					break;
				case "PortfolioEx":
					ObjectName = "ProjectGroupId";
					break;
			}
			if (this.ClassName == "UsersEx" && Request.QueryString[ObjectName] == null)
				return Mediachase.IBN.Business.Security.CurrentUser.UserID;
			else
				return int.Parse(Request.QueryString[ObjectName]);
		}
		#endregion

		#region EditLinkHtml
		private string editlinkhtml = "";
		public string EditLinkHtml
		{
			set
			{
				editlinkhtml = value;
			}
			get
			{
				return editlinkhtml;
			}
		}
		#endregion






		Hashtable htControls;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			Page.ClientScript.RegisterArrayDeclaration("CP_dvMainBMIds", "'" + tbView.MainDivClientID + "'");
		}

		private void Controls_InitHash_Add(Control CurrentCtrl)
		{
			foreach (Control ctrl in CurrentCtrl.Controls)
			{
				htControls.Add(ctrl, "1");
				if (ctrl.HasControls())
					Controls_InitHash_Add(ctrl);
			}
		}

		private void Controls_InitHash()
		{
			htControls = new Hashtable();
			foreach (Control ctrl in this.Controls)
			{
				htControls.Add(ctrl, "1");
				Controls_InitHash_Add(ctrl);
			}
		}

		private void Controls_Clear_Childs(Control CurrentCtrl)
		{
			int all_ctrls = CurrentCtrl.Controls.Count - 1;
			for (int i = all_ctrls; i >= 0; i--)
			{
				if (!htControls.ContainsKey(CurrentCtrl.Controls[i]))
					CurrentCtrl.Controls.RemoveAt(i);
				else
				{
					if (CurrentCtrl.Controls[i].HasControls())
						Controls_Clear_Childs(CurrentCtrl.Controls[i]);
				}
			}
		}

		private void Controls_Clear()
		{
			Controls_Clear_Childs(this);
		}

		private void Bind_Cotrols(Mediachase.WebSaltatoryControl.ControlPlace cp)
		{
			int allcntrls = cp.ControlWrappers.Count;
			HtmlTableRow tr;
			HtmlTableCell td;
			int td_top_pad = 20;
			for (int i = 0; i <= allcntrls; i++)
			{
				if (i == allcntrls && !ControlManager.CurrentView.CustomizeEnabled)
				{
					break;
				}

				tr = new HtmlTableRow();
				tbMain.Rows.Add(tr);
				tbMain.Style.Remove("border");
				if (ControlManager.CurrentView.CustomizeEnabled)
				{
					tbMain.Style.Add("border", "1px solid red");
					td = new HtmlTableCell();
					tr.Cells.Add(td);
					HtmlImage img = new HtmlImage();
					img.Src = "Layouts/Images/insert.gif";
					img.Width = 14;
					img.Height = 21;
					img.Border = 0;
					img.Attributes.Add("CP_ClientID", this.ClientID);
					img.Attributes.Add("CP_Index", i.ToString());
					img.Style.Add("position", "relative");
					img.Style.Add("top", "-" + ((img.Height) / 2).ToString());
					img.Style.Add("z-index", "255");
					img.Style.Add("cursor", "pointer");
					img.Attributes.Add("onclick", "javascript:" +
						"CP_ImgClick('" + txtSourceControl.ClientID + "','" + txtSourceNewControl.ClientID + "','" + txtSourceElement.ClientID + "','" + txtImgSubmit.ClientID + "'," + i.ToString() + ");" +
						Page.ClientScript.GetPostBackEventReference(ImgSubmit, "") + ";");
					img.Style.Add("display", "none");
					td.Controls.Add(img);
					td.VAlign = "top";
					td.Width = "1%";
					td.Style.Add("padding-top", td_top_pad.ToString() + "px");
				}
				td = new HtmlTableCell();
				tr.Cells.Add(td);
				td.VAlign = "top";
				td.Width = "99%";
				if (ControlManager.CurrentView.CustomizeEnabled)
					td.Style.Add("padding-top", td_top_pad.ToString() + "px");
				if (i < allcntrls)
				{
					if (td.ColSpan > 1)
					{
						td.ColSpan = 1;
					}
					if (td.InnerHtml != String.Empty)
					{
						td.InnerHtml = String.Empty;
					}

					Mediachase.UI.Web.Modules.MetaDataBlockViewControl control = (Mediachase.UI.Web.Modules.MetaDataBlockViewControl)cp.ControlWrappers[i].Control;

					CustomizeView CustomizeControl = (CustomizeView)this.LoadControl("CustomizeView.ascx");
					CustomizeControl.Path_Img = path_img;
					CustomizeControl.Title = control.Name;

					if (!ControlManager.CurrentView.CustomizeEnabled)
					{
						if (control.ContainFileds)
						{
							CustomizeControl.Control(cp.ControlWrappers[i].Control);
							//string bmDiv_Html = "<A HREF='../Common/MetaDataEdit.aspx?id=335&class=ProjectsEx_28'><img alt='' src='../Layouts/Images/Edit.gif'/> Edit</A>";
							//CustomizeControl.DropMenuHtml = bmDiv_Html;
							if (editlinkhtml != string.Empty)
							{
								CustomizeControl.DropMenuHtml = string.Format(editlinkhtml, this.GetObjectId().ToString(), this.ID, i);
							}
						}
						else CustomizeControl.Description = control.Description;
					}
					else
					{
						CustomizeControl.Description = control.Description;
						string bmDiv_Id = td.ClientID + "dv";
						string bmDiv_sUrl = "ShowRelDivBMenu('" + this.ClientID + "'," + i.ToString() + ",'" + txtActiveElement.ClientID + "','" + tbView.MainDivClientID + "', '" + bmDiv_Id + "', 0, 100);";
						string bmDiv_Html = "<div style='position:relative;left:0px;top:2px;z-index:255;' onmouseover=\"this.className='selectednavover';\" onmouseout=\"this.className='selectednav';\" class='selectednav' id='" + bmDiv_Id + "' onclick=\"" + bmDiv_sUrl + "\">" + "&nbsp;<img alt='' src='" + path_img + "Layouts/Images/Menu/downbtn.gif' border='0' width='9' height='5' align='absmiddle'/>&nbsp;" + "</div>";
						CustomizeControl.DropMenuHtml = bmDiv_Html;
					}
					td.Controls.Add(CustomizeControl);
				}
				else
				{
					//customizeallow true
					td.InnerHtml = "<br>";
					td.ColSpan = 2;
				}
			}
			if (allcntrls == 0 && !ControlManager.CurrentView.CustomizeEnabled)
			{
				this.Visible = false;
				return;
			}
			else this.Visible = true;
			ArrayList BlockMenuItemList = new ArrayList();
			tbView.ClearBlockMenu();
			BlockMenuItemList.Add(new BlockMenuItem("<img alt='' src='" + path_img + "Layouts/Images/Menu/Icons/move.gif' border='0' width='16' height='16' align='absmiddle' title='" + LocRM.GetString("tMove") + "'/> ", LocRM.GetString("tMove"), "CP_Move('" + txtActiveElement.ClientID + "','" + this.ID + "','" + this.ClientID + "','" + tbView.MainDivClientID + "')", ""));
			BlockMenuItemList.Add(new BlockMenuItem("<img alt='' src='" + path_img + "Layouts/Images/Menu/Icons/info.gif' border='0' width='16' height='16' align='absmiddle' title='" + LocRM.GetString("tProperties") + "'/> ", LocRM.GetString("tProperties"), "CP_Properties('" + propertypageurl + "','" + ControlManager.CurrentView.Id + "','" + this.ID + "','" + txtActiveElement.ClientID + "')", ""));
			BlockMenuItemList.Add(new BlockMenuItem("<img alt='' src='" + path_img + "Layouts/Images/Menu/Icons/hide.gif' border='0' width='16' height='16' align='absmiddle' title='" + LocRM.GetString("tDelete") + "'/> ", LocRM.GetString("tDelete"), "if(confirm('" + LocRM.GetString("tWarningControl") + "')){CP_Hide('" + txtHideSubmit.ClientID + "','" + txtActiveElement.ClientID + "');" + Page.ClientScript.GetPostBackEventReference(HideSubmit, "") + "}", ""));
			tbView.AddBlockMenu(tbView.MainDivClientID, BlockMenuItemList);
			if (!ControlManager.CurrentView.CustomizeEnabled)
				tbView.Visible = false;
			//tbView.AddBlockMenu("&nbsp;<img alt='' src='Layouts/Images/Menu/downbtn.gif' border='0' width=9 height=5 align='absmiddle' />&nbsp;" + "", "_divIncGen01", BlockMenuItemList);

			if (!Page.ClientScript.IsClientScriptBlockRegistered("PlaceControlScript"))
			{
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "PlaceControlScript", "<link href='" + path_img + "Layouts/Styles/menuStyle.css' type='text/css' rel='stylesheet'/><script type='text/javascript' src='" + path_img + "Scripts/ControlPlace.js'></script>");
			}
		}

		protected override void RenderChildren(HtmlTextWriter output)
		{
			if (HasControls())
			{
				for (int i = 0; i <= Controls.Count - 1; i++)
				{
					if (Controls[i].ID != null)
					{
						if (!ControlManager.CurrentView.CustomizeEnabled &&
							Controls[i] is System.Web.UI.HtmlControls.HtmlInputHidden)
							continue;
						Controls[i].RenderControl(output);
					}
				}
			}
		}

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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

		protected void ImgSubmit_Click(object sender, System.EventArgs e)
		{
			//Response.Write("Move from: " + txtSourceControl.Value + "-" + txtSourceElement.Value + "<br>");
			//Response.Write("Move to: " + this.ID + "-" + txtImgSubmit.Value);

			if (txtSourceNewControl.Value != string.Empty)
			{
				ControlManager.CurrentView.AddControl(this.ID, int.Parse(txtImgSubmit.Value),
					ControlManager.CurrentView.AvailableControls[txtSourceNewControl.Value]);
			}
			else
				ControlManager.CurrentView.MoveControl(txtSourceControl.Value, int.Parse(txtSourceElement.Value),
				this.ID, int.Parse(txtImgSubmit.Value));
		}

		protected void HideSubmit_Click(object sender, System.EventArgs e)
		{
			//Response.Write("Hide: " + txtHideSubmit.Value);

			ControlManager.CurrentView.RemoveControl(this.ID, int.Parse(txtHideSubmit.Value));
		}
		#region IWebControlPlace Members

		void IWebControlPlace.Init(Mediachase.WebSaltatoryControl.ControlPlace cp, bool bFirstRun)
		{
			// TODO:  Add ControlPlace.Init implementation
			if (bFirstRun)
				Controls_InitHash();
			else
				Controls_Clear();
			Bind_Cotrols(cp);
		}

		#endregion
	}
}
