namespace Mediachase.UI.Web.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.MetaDataPlus;
	using Mediachase.MetaDataPlus.Configurator;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Modules.EditControls;
	using Mediachase.Ibn.Web.Interfaces;

	/// <summary>
	///		Summary description for MetaDataEditControl.
	/// </summary>
	public partial class MetaDataEditControl : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", typeof(MetaDataEditControl).Assembly);

		#region ObjectId
		private int _objectId = -1;
		public int ObjectId
		{
			set
			{
				_objectId = value;
			}
			get
			{
				return _objectId;
			}
		}
		#endregion

		#region MetaClassName
		private string _metaClassName = "";
		public string MetaClassName
		{
			set
			{
				_metaClassName = value;
			}
			get
			{
				return _metaClassName;
			}
		}
		#endregion

		#region ListId
		private int _listId = -1;
		public int ListId
		{
			set
			{
				_listId = value;
			}
			get
			{
				return _listId;
			}
		}
		#endregion

		#region ShowButtons
		public bool ShowButtons
		{
			set
			{
				trButtons.Visible = value;
			}
			get
			{
				return trButtons.Visible;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			LoadRequestVariables();
			BindCustomFields();
			divError.Style.Add("display", "none");
			lblErrorText.Text = LocRM.GetString("tSummaryValidation");

			btnSave.Attributes.Add("onclick", "CheckValidate(this);");
			btnCancel.Attributes.Add("onclick", "ForCancel(this);");
			btnSave.Text = LocRM.GetString("Save");
			btnCancel.Text = LocRM.GetString("Cancel");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
		}

		#region LoadRequestVariables
		private void LoadRequestVariables()
		{
			if (Request.QueryString["id"] != null)
			{
				try
				{
					ObjectId = int.Parse(Request.QueryString["id"]);
				}
				catch { }
			}

			if (Request.QueryString["class"] != null)
				MetaClassName = Request.QueryString["class"];

			if (Request.QueryString["ListId"] != null)
			{
				try
				{
					ListId = int.Parse(Request.QueryString["ListId"]);
				}
				catch { }
			}
		}
		#endregion

		#region BindCustomFields
		private void BindCustomFields()
		{
			MetaObject obj = null;
			if (ObjectId > 0)
				obj = MetaDataWrapper.LoadMetaObject(ObjectId, MetaClassName);
			if (obj == null)
				obj = MetaDataWrapper.NewMetaObject(ObjectId, MetaClassName);

			foreach (MetaField field in obj.MetaClass.UserMetaFields)
			{
				HtmlTableRow row = new HtmlTableRow();
				HtmlTableCell cellTitle = new HtmlTableCell();
				HtmlTableCell cellValue = new HtmlTableCell();

				cellTitle.Attributes.Add("class", "ibn-label");
				cellTitle.Width = "110px";
				cellTitle.VAlign = "middle";
				cellTitle.InnerHtml = String.Format("{0}:", field.FriendlyName);
				object fieldValue = obj[field.Name];
				System.Web.UI.UserControl control = null;

				switch (field.DataType)
				{
					case MetaDataType.Binary:
						cellValue.InnerText = "[BinaryData]";
						break;
					case MetaDataType.File:
						cellTitle.VAlign = "top";
						control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/FileValue.ascx");
						break;
					case MetaDataType.ImageFile:
						cellTitle.VAlign = "top";
						control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/ImageFileValue.ascx");
						break;
					case MetaDataType.DateTime:
						control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/DateTimeValue.ascx");
						break;
					case MetaDataType.Money:
						control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/MoneyValue.ascx");
						break;
					case MetaDataType.Float:
						control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/FloatValue.ascx");
						break;
					case MetaDataType.Integer:
						control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/IntValue.ascx");
						break;
					case MetaDataType.Boolean:
						control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/BooleanValue.ascx");
						break;
					case MetaDataType.Date:
						control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/DateValue.ascx");
						break;
					case MetaDataType.Email:
						control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/EmailValue.ascx");
						break;
					case MetaDataType.Url:
						control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/URLValue.ascx");
						break;
					case MetaDataType.ShortString:
						control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/ShortStringValue.ascx");
						break;
					case MetaDataType.LongString:
						cellTitle.VAlign = "top";
						control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/LongStringValue.ascx");
						break;
					case MetaDataType.LongHtmlString:
						cellTitle.VAlign = "top";
						control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/LongHTMLStringValue.ascx");
						break;
					case MetaDataType.DictionarySingleValue:
					case MetaDataType.EnumSingleValue:
						control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/DictionarySingleValue.ascx");
						((DictionarySingleValue)control).InitControl(field.Id, (field.AllowNulls ? !field.IsRequired : field.AllowNulls));
						break;
					case MetaDataType.DictionaryMultivalue:
					case MetaDataType.EnumMultivalue:
						cellTitle.VAlign = "top";
						control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/DictionaryMultivalue.ascx");
						((DictionaryMultivalue)control).InitControl(field.Id);
						break;
					default:
						if (fieldValue != null)
							cellValue.InnerText = fieldValue.ToString();
						break;
				}

				if (control != null)
				{
					cellValue.Controls.Add(control);

				}

				row.Cells.Add(cellTitle);
				row.Cells.Add(cellValue);

				tblCustomFields.Rows.Add(row);

				if (control != null)
				{
					ICustomField iCustomField = ((ICustomField)control);
					iCustomField.FieldName = field.Name;
					if (fieldValue != null)
						iCustomField.Value = fieldValue;
					if (field.AllowNulls)
						iCustomField.AllowEmptyValues = !field.IsRequired;
					else
						iCustomField.AllowEmptyValues = false;
				}
			}
		}
		#endregion

		#region btnCancel_ServerClick
		protected void btnCancel_ServerClick(object sender, System.EventArgs e)
		{
			Redirect();
		}
		#endregion

		#region btnSave_ServerClick
		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
			{
				divError.Style.Add("display", "");
				return;
			}

			Save();
			Redirect();
		}
		#endregion

		#region Redirect
		private void Redirect()
		{
			string RedirectUrl = "../";
			MetaClassName = MetaClassName.ToLower();

			switch (MetaClassName)
			{
				case "usersex":
					RedirectUrl += String.Format("Directory/UserView.aspx?UserID={0}&Tab=2", ObjectId);
					break;
				case "incidentsex":
					RedirectUrl += String.Format("Incidents/IncidentView.aspx?IncidentId={0}&Tab=Customization", ObjectId);
					break;
				case "documentsex":
					RedirectUrl += String.Format("Documents/DocumentView.aspx?DocumentId={0}&Tab=Customization", ObjectId);
					break;
				case "todoex":
					RedirectUrl += String.Format("ToDo/ToDoView.aspx?ToDoID={0}&Tab=Customization", ObjectId);
					break;
				case "taskex":
					RedirectUrl += String.Format("Tasks/TaskView.aspx?TaskID={0}&Tab=Customization", ObjectId);
					break;
				case "eventsex":
					RedirectUrl += String.Format("Events/EventView.aspx?EventID={0}&Tab=Customization", ObjectId);
					break;
				case "portfolioex":
					RedirectUrl += String.Format("Projects/ProjectGroupView.aspx?ProjectGroupId={0}", ObjectId);
					break;
				default:
					if (MetaClassName.ToLower().StartsWith("projectsex_"))
						RedirectUrl += String.Format("Projects/ProjectView.aspx?ProjectId={0}&Tab=Customization", ObjectId);
					break;
			}

			Response.Redirect(RedirectUrl);
		}
		#endregion

		#region Save
		public void Save()
		{
			MetaObject obj = null;
			if (ObjectId > 0)
				obj = MetaDataWrapper.LoadMetaObject(ObjectId, MetaClassName, Security.CurrentUser.UserID, DateTime.UtcNow);
			if (obj == null)
				obj = MetaDataWrapper.NewMetaObject(ObjectId, MetaClassName);

			foreach (HtmlTableRow row in tblCustomFields.Rows)
			{
				if (row.Cells.Count > 1)
				{
					HtmlTableCell cell = row.Cells[1];
					if (cell.Controls.Count > 0)
					{
						ICustomField ctrl = (ICustomField)cell.Controls[0];
						object FieldValue = ctrl.Value;
						string FieldName = ctrl.FieldName;

						obj[FieldName] = FieldValue;
					}
				}
			}

			ObjectId = MetaDataWrapper.AcceptChanges(obj);
		}
		#endregion
	}
}
