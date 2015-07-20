namespace Mediachase.UI.Web.Modules.EditControls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using MetaDataPlus;
	using MetaDataPlus.Configurator;
	using Mediachase.IBN.Business;
	using Mediachase.Ibn.Web.Interfaces;
	using System.Web.UI;

	/// <summary>
	///		Summary description for DictionarySingleValue.
	/// </summary>
	public partial class DictionarySingleValue : System.Web.UI.UserControl, ICustomField
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", typeof(DictionarySingleValue).Assembly);
		private string itemValue = "0";
		private string itemText = "";

		protected void Page_Load(object sender, System.EventArgs e)
		{

		}

		private int fieldId = -1;
		public void InitControl(int FieldId, bool AddEmptyItem)
		{
			fieldId = FieldId;
			MetaField field = MetaField.Load(FieldId);

			ItemPanel.Controls.Clear();
			if (AddEmptyItem)
			{
				LinkButton lb = new LinkButton();
				lb.Text = LocRM.GetString("EmptyValue");
				lb.CommandArgument = "0";
				lb.CssClass = "ContextMenuItem";
				lb.CausesValidation = false;
				ItemPanel.Controls.Add(lb);
				lb.Click += new EventHandler(lb_Click);
				ScriptManager.GetCurrent(this.Page).RegisterAsyncPostBackControl(lb);

				itemText = LocRM.GetString("EmptyValue");
				itemValue = "0";
			}
			else if (field.Dictionary.Count > 0)
			{
				itemText = field.Dictionary[0].Value;
				itemValue = field.Dictionary[0].Id.ToString();
			}
			foreach (MetaDictionaryItem item in field.Dictionary)
			{
				LinkButton lb = new LinkButton();
				lb.Text = item.Value;
				lb.CommandArgument = item.Id.ToString();
				lb.CssClass = "ContextMenuItem";
				lb.CausesValidation = false;
				ItemPanel.Controls.Add(lb);
				lb.Click += new EventHandler(lb_Click);
				ScriptManager.GetCurrent(this.Page).RegisterAsyncPostBackControl(lb);
			}
			TextLabel.Text = LocRM.GetString("EmptyValue");

			btnEditItems.Visible = false;
			if (Security.IsManager() && field.DataType == MetaDataType.DictionarySingleValue)
			{
				btnEditItems.Visible = true;
				btnEditItems.Attributes.Add("onclick", String.Format("EditItems({0})", fieldId));

				if (!Page.ClientScript.IsClientScriptBlockRegistered("EditItems"))
				{
					String scriptString = "<script language=JavaScript>\r\n";
					scriptString += "function EditItems(FieldId)\r\n";
					scriptString += "{\r\n";
					scriptString += "	var w = 640;\r\n";
					scriptString += "	var h = 350;\r\n";
					scriptString += "	var l = (screen.width - w) / 2;\r\n";
					scriptString += "	var t = (screen.height - h) / 2;\r\n";
					scriptString += "	var winprops = 'resizable=0, height='+h+',width='+w+',top='+t+',left='+l;\r\n";
					scriptString += String.Format("	window.open('{0}?Id=' + FieldId, 'EditItems', winprops);\r\n", ResolveUrl("~/Common/MetaDictionary.aspx"));
					scriptString += "}\r\n";
					scriptString += "</script>\r\n";

					Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "EditItems", scriptString);
				}
			}
		}

		void lb_Click(object sender, EventArgs e)
		{
			TextLabel.Text = ((LinkButton)(sender)).Text;
			SavedValue.Value = ((LinkButton)(sender)).CommandArgument;
			UpdatePanel1.Update();
		}

		#region Implementation of ICustomField
		public object Value
		{
			set
			{
				if (!IsPostBack)
				{
					MetaDictionaryItem item = (MetaDictionaryItem)value;
					itemText = item.Value;
					itemValue = item.Id.ToString();
				}
			}
			get
			{
				object retval = null;
				if (!String.IsNullOrEmpty(SavedValue.Value))
				{
					int itemId = int.Parse(SavedValue.Value);
					if (fieldId > 0 && itemId > 0)
					{
						MetaField field = MetaField.Load(fieldId);
						MetaDictionaryItem item = field.Dictionary.GetItem(itemId);
						retval = item;
					}
				}
				return retval;
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

		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			// Ќадо вытащить значение и проверить, есть ли такое в Dictionary
			// ѕри отсутствии - использовать пустое (или первое)
			if (IsPostBack && fieldId > 0)
			{
				MetaField field = MetaField.Load(fieldId);

				foreach (MetaDictionaryItem item in field.Dictionary)
				{
					if (item.Id.ToString() == SavedValue.Value)
					{
						itemValue = SavedValue.Value;
						itemText = item.Value;
						break;
					}
				}
			}

			SavedValue.Value = itemValue;
			TextLabel.Text = itemText;
		}
	}
}
