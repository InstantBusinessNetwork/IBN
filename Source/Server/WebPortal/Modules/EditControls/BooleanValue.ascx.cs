namespace Mediachase.UI.Web.Modules.EditControls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.Ibn.Web.Interfaces;

	/// <summary>
	///		Summary description for BooleanValue.
	/// </summary>
	public partial class BooleanValue : System.Web.UI.UserControl, ICustomField
	{


		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);

	        ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", typeof(BooleanValue).Assembly);
			rbValue.Items.Add(new ListItem(LocRM.GetString("BooleanYes"),"True"));
			rbValue.Items.Add(new ListItem(LocRM.GetString("BooleanNo"),"False"));
			rbValue.SelectedIndex = 0;
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

		#region Implementation of ICustomField
		public object Value
		{
			set
			{
				if ((bool)value)
					rbValue.SelectedIndex=0;
				else
					rbValue.SelectedIndex=1;
			}
			get
			{
				if (AllowEmptyValues && rbValue.SelectedIndex==-1)
					return null;
				else
					return bool.Parse(rbValue.SelectedValue);
			}
		}
		
		private string fieldName = "";
		public string FieldName
		{
			set
			{
				fieldName = value;
			}
			get
			{
				return fieldName;
			}
		}

		private bool allowEmptyValues = false;
		public bool AllowEmptyValues
		{
			set
			{
				allowEmptyValues = value;
				boolValueRFValidator.Visible = !allowEmptyValues;
			}
			get
			{
				return allowEmptyValues;
			}
		}
		#endregion
	}
}
