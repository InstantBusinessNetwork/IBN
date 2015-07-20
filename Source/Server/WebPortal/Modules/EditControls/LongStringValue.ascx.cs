namespace Mediachase.UI.Web.Modules.EditControls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.Ibn.Web.Interfaces;

	/// <summary>
	///		Summary description for LongStringValue.
	/// </summary>
	public partial class LongStringValue : System.Web.UI.UserControl, ICustomField
	{


		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
		}

		#region Implementation of ICustomField
		public object Value
		{
			set
			{
				txtValue.Text = value.ToString();
			}
			get
			{
				if (AllowEmptyValues && txtValue.Text == String.Empty)
					return null;
				else
					return txtValue.Text;
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
				txtValueRFValidator.Visible = !allowEmptyValues;
			}
			get
			{
				return allowEmptyValues;
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}
}
