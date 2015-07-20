namespace Mediachase.UI.Web.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using ComponentArt.Web.UI;
	using Mediachase.UI.Web.Util;
	using System.Globalization;
	using System.Web.UI;
	using System.Text;

	/// <summary>
	///		Summary description for BlockHeaderLightWithMenu.
	/// </summary>
	public partial class BlockHeaderLightWithMenu : System.Web.UI.UserControl
	{

		private string outLStr = string.Empty;
		private string outRStr = string.Empty;
		private string divStr = "<td class='ibn-separator'>|</td>";

		#region ActionsMenu
		public ComponentArt.Web.UI.Menu ActionsMenu
		{
			get
			{
				return AcMenu;
			}
		}
		#endregion

		#region LeftCornerClientId
		public string LeftCornerClientId
		{
			get
			{
				return tdLeftCorner.ClientID;
			}
		} 
		#endregion

		#region RightCornerClientId
		public string RightCornerClientId
		{
			get
			{
				return tdRightCorner.ClientID;
			}
		} 
		#endregion

		#region Collapsed
		public bool Collapsed
		{
			set
			{
				ViewState["Collapsed"] = value;
			}
			get
			{
				bool retval = false;
				if (ViewState["Collapsed"] != null)
					retval = (bool)ViewState["Collapsed"];
				return retval;
			}
		}
		#endregion

		#region CollapsibleControlId
		public string CollapsibleControlId
		{
			set
			{
				ViewState["CollapsibleControlId"] = value;
			}
			get
			{
				string retval = string.Empty;
				if (ViewState["CollapsibleControlId"] != null)
					retval = (string)ViewState["CollapsibleControlId"];
				return retval;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			CommonHelper.SafeRegisterStyle(Page, "~/Styles/IbnFramework/mcBlockMenu.css");

			CollapsedValue.ServerChange += new EventHandler(CollapsedValue_ServerChange);

			#region CollapseExpand
			if (!Page.ClientScript.IsClientScriptBlockRegistered("CollapseExpand"))
			{
				StringBuilder sb = new StringBuilder(1403);
				sb.AppendFormat("function CollapseExpandBlock(imageId, blockId, leftCornerId, rightCornerId, collapsedValueId) {{\r\n");
				sb.AppendFormat("\tvar re;\r\n");
				sb.AppendFormat("\tvar image = document.getElementById(imageId);\r\n");
				sb.AppendFormat("\tvar block = document.getElementById(blockId);\r\n");
				sb.AppendFormat("\tvar leftCorner = document.getElementById(leftCornerId);\r\n");
				sb.AppendFormat("\tvar rightCorner = document.getElementById(rightCornerId);\r\n");
				sb.AppendFormat("\tvar collapsedValue = document.getElementById(collapsedValueId);\r\n");
				sb.AppendFormat("\r\n");
				sb.AppendFormat("\tif (image && image.src) {{\r\n");
				sb.AppendFormat("\t\tif (image.src.indexOf(\"plusxp.gif\") > 0) {{\r\n");
				sb.AppendFormat("\t\t\tre = /plusxp.gif/g;\r\n");
				sb.AppendFormat("\t\t\timage.src = image.src.replace(re, \"minusxp.gif\");\r\n");
				sb.AppendFormat("\r\n");
				sb.AppendFormat("\t\t\tre = /linehz.gif/g;\r\n");
				sb.AppendFormat("\t\t\tif (leftCorner && leftCorner.src)\r\n");
				sb.AppendFormat("\t\t\t\tleftCorner.src = leftCorner.src.replace(re, \"LeftCorner.gif\");\r\n");
				sb.AppendFormat("\r\n");
				sb.AppendFormat("\t\t\tre = /linehz.gif/g;\r\n");
				sb.AppendFormat("\t\t\tif (rightCorner && rightCorner.src)\r\n");
				sb.AppendFormat("\t\t\t\trightCorner.src = rightCorner.src.replace(re, \"RightCorner.gif\");\r\n");
				sb.AppendFormat("\r\n");
				sb.AppendFormat("\t\t\tif (collapsedValue)\r\n");
				sb.AppendFormat("\t\t\t\tcollapsedValue.value = \"0\"\r\n");
				sb.AppendFormat("\r\n");
				sb.AppendFormat("\t\t\tif (block)\r\n");
				sb.AppendFormat("\t\t\t\tblock.style.display = \"\";\r\n");
				sb.AppendFormat("\t\t}}\r\n");
				sb.AppendFormat("\t\telse {{\r\n");
				sb.AppendFormat("\t\t\tre = /minusxp.gif/g;\r\n");
				sb.AppendFormat("\t\t\timage.src = image.src.replace(re, \"plusxp.gif\");\r\n");
				sb.AppendFormat("\r\n");
				sb.AppendFormat("\t\t\tre = /LeftCorner.gif/g;\r\n");
				sb.AppendFormat("\t\t\tif (leftCorner && leftCorner.src)\r\n");
				sb.AppendFormat("\t\t\t\tleftCorner.src = leftCorner.src.replace(re, \"linehz.gif\");\r\n");
				sb.AppendFormat("\r\n");
				sb.AppendFormat("\t\t\tre = /RightCorner.gif/g;\r\n");
				sb.AppendFormat("\t\t\tif (rightCorner && rightCorner.src)\r\n");
				sb.AppendFormat("\t\t\t\trightCorner.src = rightCorner.src.replace(re, \"linehz.gif\");\r\n");
				sb.AppendFormat("\r\n");
				sb.AppendFormat("\t\t\tif (collapsedValue)\r\n");
				sb.AppendFormat("\t\t\t\tcollapsedValue.value = \"1\"\r\n");
				sb.AppendFormat("\r\n");
				sb.AppendFormat("\t\t\tif (block)\r\n");
				sb.AppendFormat("\t\t\t\tblock.style.display = \"none\";\r\n");
				sb.AppendFormat("\t\t}}\r\n");
				sb.AppendFormat("\t}}\r\n");
				sb.AppendFormat("}}");

				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "CollapseExpand", sb.ToString(), true);
			} 
			#endregion

			AcMenu.ClientScriptLocation = "~/Scripts/componentart_webui_client/";
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
			this.PreRender += new EventHandler(Page_PreRender);
		}
		#endregion

		#region AddText
		public void AddText(string Text)
		{
			if (outLStr.Length > 0)
				outLStr += divStr;

			Control collapsibleControl = null;
			if (!String.IsNullOrEmpty(CollapsibleControlId))
			{
				collapsibleControl = this.Parent.FindControl(CollapsibleControlId);

				string script = String.Format(CultureInfo.InvariantCulture,
					"CollapseExpandBlock(\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\")",
					CollapsingImage.ClientID,
					collapsibleControl.ClientID,
					LeftCornerImage.ClientID,
					RightCornerImage.ClientID,
					CollapsedValue.ClientID);

				outLStr += string.Format(CultureInfo.InvariantCulture,
					"<td nowrap='nowrap' style='padding-right:5; padding-left:5; font-weight:bold; cursor:pointer;' onclick='{0}'>{1}</td>",
					script,
					Text);
			}
			else
			{
				outLStr += "<td nowrap='nowrap' style='padding-right:5; padding-left:5; font-weight:bold'>" + Text + "</td>";
			}
		}
		#endregion

		#region AddLeftLink
		public void AddLeftLink(string Text, string Url)
		{
			if (outLStr.Length > 0)
				outLStr += divStr;

			outLStr += "<td nowrap='nowrap' style='padding-right:5; padding-left:5'><a href=\"" + Url + "\">" + Text + "</a></td>";
		}
		#endregion

		#region AddRightLink
		public void AddRightLink(string Text, string Url)
		{
			if (outRStr.Length > 0)
				outRStr += divStr;

			outRStr += "<td style=\"padding-right: 5; padding-left: 5; white-space: nowrap\"><a href=\"" + Url + "\">" + Text + "</a></td>";
		}
		#endregion

		#region AddRightText
		public void AddRightText(string Text)
		{
			if (outRStr.Length > 0)
				outRStr += divStr;

			outRStr += "<td nowrap='nowrap' style='padding-right:5; padding-left:5'>" + Text + "</td>";
		}
		#endregion

		#region ClearLeftItems()
		public void ClearLeftItems()
		{
			outLStr = string.Empty;
		}
		#endregion

		#region ClearRightItems()
		public void ClearRightItems()
		{
			outRStr = string.Empty;
		}
		#endregion

		#region EnsureRender
		public void EnsureRender()
		{
			if (AcMenu.Items.Count == 0)
				tdMenu.Visible = false;
			else
				tdMenu.Visible = true;

			if (outLStr.Length > 0)
			{
				tdLeftItems.InnerHtml = "<table cellpadding='0' cellspacing='0' border='0' class='ibn-toolbar-light'><tr>" + outLStr + "</tr></table>";
				tdLeftItems.Visible = true;
			}
			else
				tdLeftItems.Visible = false;

			if (outRStr.Length > 0)
			{
				tdRightItems.InnerHtml = "<table cellpadding='0' cellspacing='0' border='0' class='ibn-toolbar-light'><tr>" + outRStr + "</tr></table>";
				tdRightItems.Visible = true;
			}
			else
				tdRightItems.Visible = false;

			CollapsingCell.Visible = !String.IsNullOrEmpty(CollapsibleControlId);

			LeftCornerImage.Src = "~/Images/IbnFramework/LeftCorner.gif";
			RightCornerImage.Src = "~/Images/IbnFramework/RightCorner.gif";

			Control collapsibleControl = null;
			if (!String.IsNullOrEmpty(CollapsibleControlId))
				collapsibleControl = this.Parent.FindControl(CollapsibleControlId);

			if (collapsibleControl != null)
			{
				if (Collapsed)
				{
					LeftCornerImage.Src = "~/Images/IbnFramework/linehz.gif";
					RightCornerImage.Src = "~/Images/IbnFramework/linehz.gif";
					CollapsingImage.Src = "~/Images/IbnFramework/plusxp.gif";

					if (collapsibleControl is HtmlControl)
						((HtmlControl)collapsibleControl).Style.Add(HtmlTextWriterStyle.Display, "none");
					else if (collapsibleControl is System.Web.UI.WebControls.WebControl)
						((System.Web.UI.WebControls.WebControl)collapsibleControl).Style.Add(HtmlTextWriterStyle.Display, "none");

					if (!IsPostBack)
						CollapsedValue.Value = "1";
				}
				else
				{
					CollapsingImage.Src = ResolveClientUrl("~/Images/IbnFramework/minusxp.gif");

					if (collapsibleControl is HtmlControl)
						((HtmlControl)collapsibleControl).Style.Add(HtmlTextWriterStyle.Display, "");
					else if (collapsibleControl is System.Web.UI.WebControls.WebControl)
						((System.Web.UI.WebControls.WebControl)collapsibleControl).Style.Add(HtmlTextWriterStyle.Display, "");
				}

				CollapsingImage.Attributes.Add(
					"onclick",
					String.Format(CultureInfo.InvariantCulture,
						"CollapseExpandBlock('{0}', '{1}', '{2}', '{3}', '{4}')",
						CollapsingImage.ClientID,
						collapsibleControl.ClientID,
						LeftCornerImage.ClientID,
						RightCornerImage.ClientID,
						CollapsedValue.ClientID));
			}
		} 
		#endregion

		#region Page_PreRender
		private void Page_PreRender(object sender, EventArgs e)
		{
			EnsureRender();
		}
		#endregion

		#region CollapsedValue_ServerChange
		void CollapsedValue_ServerChange(object sender, EventArgs e)
		{
			if (CollapsedValue.Value == "0")
				Collapsed = false;
			else
				Collapsed = true;
		} 
		#endregion
	}
}
