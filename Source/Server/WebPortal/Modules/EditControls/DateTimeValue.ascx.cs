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
	///		Summary description for DateTimeValue.
	/// </summary>
	public partial class DateTimeValue : System.Web.UI.UserControl, ICustomField
	{

		#region Path_File_JS
		public string Path_JS
		{
			set
			{
				string ValueToSet = value;
				if( ValueToSet.Length > 0 )
				{
					if(ValueToSet.Substring(ValueToSet.Length-1,1) != "/") ValueToSet += "/";
				}
				//dtcValue.Path_JS = ValueToSet;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			//dtcValue.ID = fieldName + "_" + dtcValue.ID;
		}

		#region Implementation of ICustomField
		public object Value
		{
			set
			{
				dtcValue.SelectedDate = (DateTime)value;
			}
			get
			{
				if (AllowEmptyValues && dtcValue.SelectedDate == DateTime.MinValue)
					return null;
				else
					return dtcValue.SelectedDate;
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
				dtcValue.DateIsRequired = !allowEmptyValues;
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
