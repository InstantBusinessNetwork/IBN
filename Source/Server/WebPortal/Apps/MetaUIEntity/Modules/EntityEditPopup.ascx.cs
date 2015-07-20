using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Core;

namespace Mediachase.Ibn.Web.UI.MetaUIEntity.Modules
{
	public partial class EntityEditPopup : System.Web.UI.UserControl
	{
		#region FormName
		protected string FormName
		{
			get
			{
				string retval = "[MC_BaseForm]";
				if (Request.QueryString["formName"] != null)
					retval = Request.QueryString["formName"];
				return retval;
			}
		}
		#endregion

		#region ClassName
		protected string ClassName
		{
			get
			{
				string retval = String.Empty;
				if (Request.QueryString["className"] != null)
					retval = Request.QueryString["className"];
				return retval;
			}
		}
		#endregion

		#region ObjectId
		protected PrimaryKeyId ObjectId
		{
			get
			{
				PrimaryKeyId retval = PrimaryKeyId.Empty;
				if (Request.QueryString["ObjectId"] != null)
					retval = PrimaryKeyId.Parse(Request.QueryString["ObjectId"]);
				return retval;
			}
		}
		#endregion

		#region ContainerFieldName
		protected string ContainerFieldName
		{
			get
			{
				string retval = String.Empty;
				if (Request.QueryString["ContainerFieldName"] != null)
					retval = Request.QueryString["ContainerFieldName"];
				return retval;
			}
		}
		#endregion

		#region ContainerId
		protected PrimaryKeyId ContainerId
		{
			get
			{
				PrimaryKeyId retval = PrimaryKeyId.Empty;
				if (Request.QueryString["ContainerId"] != null)
					retval = PrimaryKeyId.Parse(Request.QueryString["ContainerId"]);
				return retval;
			}
		}
		#endregion

		#region CommandName
		protected string CommandName
		{
			get
			{
				string retval = String.Empty;
				if (Request.QueryString["commandName"] != null)
					retval = Request.QueryString["commandName"];
				return retval;
			}
		}
		#endregion

		#region ShowViewLink
		protected bool ShowViewLink
		{
			get
			{
				if (Request.QueryString["ShowViewLink"] != null && Request.QueryString["ShowViewLink"] == "1")
					return true;
				return false;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				BindData();

				if (Request["closeFramePopup"] != null)
				{
					CancelButton.OnClientClick = String.Format(CultureInfo.InvariantCulture, 
						"javascript:try{{window.parent.{0}();}}catch(ex){{}}", 
						Request["closeFramePopup"]); ;
				}
			}
			cbGoToView.Text = "&nbsp;" + GetGlobalResourceObject("IbnFramework.Global", "GoToViewLink").ToString();
			trGoToView.Visible = this.ShowViewLink;
		}

		#region BindData
		private void BindData()
		{
			EntityObject entity;
			if (ObjectId != PrimaryKeyId.Empty)
				entity = BusinessManager.Load(ClassName, ObjectId);
			else
				entity = BusinessManager.InitializeEntity(ClassName);

			EditForm.FormName = FormName;
			EditForm.DataItem = entity;
			EditForm.DataBind();
		}
		#endregion

		#region SaveButton_Click
		/// <summary>
		/// Handles the Click event of the SaveButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void SaveButton_Click(object sender, EventArgs e)
		{
			this.Page.Validate();
			if (!this.Page.IsValid)
				return;

			EntityObject entity;
			if (ObjectId != PrimaryKeyId.Empty)
				entity = BusinessManager.Load(ClassName, ObjectId);
			else
				entity = BusinessManager.InitializeEntity(ClassName);
			PrimaryKeyId objectId = ObjectId;
			if (entity != null)
			{
				ProcessCollection(this.EditForm.Controls, entity);

				// Save container id
				if (!String.IsNullOrEmpty(ContainerFieldName)
					&& entity.Properties[ContainerFieldName] != null
					&& ContainerId != PrimaryKeyId.Empty)
				{
					entity[ContainerFieldName] = ContainerId;
				}

				// Update/Create
				if (ObjectId != PrimaryKeyId.Empty)
					BusinessManager.Update(entity);
				else
					objectId = BusinessManager.Create(entity);
			}
			
			string command = String.Empty;
			if (!String.IsNullOrEmpty(CommandName))
			{
				CommandParameters cp = new CommandParameters(CommandName);
				command = cp.ToString();
			}
			if (cbGoToView.Checked)
			{
				this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), Guid.NewGuid().ToString(),
					String.Format("window.parent.location.href='{0}?ClassName={1}&ObjectId={2}';", 
						CHelper.GetAbsolutePath("/Apps/MetaUIEntity/Pages/EntityView.aspx"),
						ClassName, objectId.ToString()), true);
			}
			else
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, command);
		}
		#endregion

		#region ProcessCollection
		private void ProcessCollection(ControlCollection _coll, EntityObject mo)
		{
			foreach (Control c in _coll)
			{
				ProcessControl(c, mo);
				if (c.Controls.Count > 0)
					ProcessCollection(c.Controls, mo);
			}
		}

		private void ProcessControl(Control c, EntityObject _obj)
		{
			IEditControl editControl = c as IEditControl;
			if (editControl != null)
			{
				string fieldName = editControl.FieldName;

				#region MyRegion
				string ownFieldName = fieldName;
				string aggrFieldName = String.Empty;
				string aggrClassName = String.Empty;
				MetaField ownField = null;
				MetaField aggrField = null;
				MetaClass ownClass = MetaDataWrapper.GetMetaClassByName(ClassName);
				if (ownFieldName.Contains("."))
				{
					string[] mas = ownFieldName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
					if (mas.Length > 1)
					{
						ownFieldName = mas[0];
						aggrFieldName = mas[1];

						ownField = MetaDataWrapper.GetMetaFieldByName(ownClass, ownFieldName);
						aggrClassName = ownField.Attributes.GetValue<string>(McDataTypeAttribute.AggregationMetaClassName);
						aggrField = MetaDataWrapper.GetMetaFieldByName(aggrClassName, aggrFieldName);
					}
				}
				if (ownField == null)
					ownField = ownClass.Fields[ownFieldName];
				#endregion

				object eValue = editControl.Value;

				bool makeChange = true;

				MetaField field = (aggrField == null) ? ownField : aggrField;
				if (!field.IsNullable && eValue == null)
					makeChange = false;

				if (makeChange)
				{
					if (aggrField == null)
						_obj[ownFieldName] = eValue;
					else
					{
						EntityObject aggrObj = null;
						if (_obj[ownFieldName] != null)
							aggrObj = (EntityObject)_obj[ownFieldName];
						else
							aggrObj = BusinessManager.InitializeEntity(ClassName);
						aggrObj[aggrFieldName] = eValue;
					}
				}
			}
		}
		#endregion
	}
}