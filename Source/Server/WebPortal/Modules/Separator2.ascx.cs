namespace Mediachase.UI.Web.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;

	/// <summary>
	///		Summary description for Separator2.
	/// </summary>
	public partial class Separator2 : System.Web.UI.UserControl
	{
		private UserLightPropertyCollection pc =  Security.CurrentUser.Properties;

		#region Title
		public string Title
		{
			set
			{
				lblTitle.Text = value;
			}
		}
		#endregion

		#region ControlledPanel
		Panel controlledPanel = null;
		public Panel ControlledPanel
		{
			get
			{
				return controlledPanel;
			}
			set 
			{
				controlledPanel = value;
			}
		}
		#endregion

		#region IsMinimized
		public bool IsMinimized
		{
			get
			{
				if (ViewState["IsMinimized"]!=null)
					return (bool)ViewState["IsMinimized"];
				else
				{
					ViewState["IsMinimized"] = true;
					return true;
				}
			}

			set
			{
				ViewState["IsMinimized"] = value;			
			}
		}
		#endregion

		#region PCValue
		public string PCValue
		{
			get
			{
				if (ViewState["PCValue"]==null)
					ViewState["PCValue"] = "";
				return ViewState["PCValue"].ToString();
			}
			set
			{
				ViewState["PCValue"] = value;			
			}
		}
		#endregion


		protected void Page_Load(object sender, System.EventArgs e)
		{
			trSeparator.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(btnSubmit, ""));
			if (!IsPostBack)
				BindSavedValues();
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
			this.btnSubmit.Click +=new EventHandler(btnSubmit_Click);
		}
		#endregion

		#region BindSavedValues
		private void BindSavedValues()
		{
			if (PCValue != "" && pc[PCValue] != null) 
				IsMinimized = bool.Parse(pc[PCValue]);
		}
		#endregion

		#region btnSubmit_Click
		private void btnSubmit_Click(object sender, EventArgs e)
		{
			IsMinimized = !IsMinimized;
		}
		#endregion

    #region Page_PreRender
    private void Page_PreRender(object sender, EventArgs e)
		{
			if (IsMinimized)
			{
				if (ControlledPanel !=null ) 
					ControlledPanel.Visible = false;
				imgPlusMinus.ImageUrl = "../layouts/images/plusxp.gif";
			}
			else
			{
				if (ControlledPanel !=null ) 
					ControlledPanel.Visible = true;
				imgPlusMinus.ImageUrl = "../layouts/images/minusxp.gif";
			}

			SaveValues();
		}
		#endregion

		#region SaveValues
		private void SaveValues()
		{
			if (PCValue != "") 
				pc[PCValue] = IsMinimized.ToString();
		}
		#endregion
	}
}
