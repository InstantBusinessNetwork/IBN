using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.MetaUIEntity.Modules
{
	public partial class EntitySelect : System.Web.UI.UserControl
	{
		#region _classes
		private List<string> _classes
		{
			get
			{
				List<string> retVal = new List<string>();
				if (Request["Classes"] != null)
				{
					string[] mas = Request["Classes"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
					retVal.AddRange(mas);
				}
				return retVal;
			}
		}
		#endregion

		#region _viewName
		private string _viewName
		{
			get
			{
				if (Request["ViewName"] != null)
					return Request["ViewName"];
				return String.Empty;
			}
		}
		#endregion

		private const string _placeName = "EntitySelect";

		#region _isMultipleSelect
		private bool _isMultipleSelect
		{
			get
			{
				if (Request["MultipleSelect"] != null && Request["MultipleSelect"] == "1")
					return true;
				else
					return false;
			}
		}
		#endregion

		#region _isCanCreate
		private bool _isCanCreate
		{
			get
			{
				if (Request["CanCreate"] != null && Request["CanCreate"] == "1")
					return true;
				else
					return false;
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

		#region ProfileName
		protected string ProfileName
		{
			get
			{
				if (Request.QueryString["ProfileName"] != null)
					return Request.QueryString["ProfileName"];
				return String.Empty;
			}
		}
		#endregion

		#region TreeServiceTargetObjectId
		protected PrimaryKeyId TreeServiceTargetObjectId
		{
			get
			{
				PrimaryKeyId retval = PrimaryKeyId.Empty;
				if (Request.QueryString["TreeServiceTargetObjectId"] != null)
					retval = PrimaryKeyId.Parse(Request.QueryString["TreeServiceTargetObjectId"]);
				return retval;
			}
		}
		#endregion

		private Mediachase.IBN.Business.UserLightPropertyCollection _pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (_classes.Count <= 0)
				throw new Exception("Classes is required!");

			ddFilter.SelectedIndexChanged += new EventHandler(ddFilter_SelectedIndexChanged);

			if (!IsPostBack)
			{	
				BindDropDowns();

				btnNew.Visible = _isCanCreate;
				tblSelect.Visible = true;
				tblNew.Visible = false;
			}

			grdMain.ClassName = ddFilter.SelectedValue;
			grdMain.ViewName = "";
			grdMain.PlaceName = _placeName;
			grdMain.ProfileName = ProfileName;
			grdMain.ShowCheckboxes = _isMultipleSelect;

			FilterElementCollection fec = new FilterElementCollection();
			if (!String.IsNullOrEmpty(Request["filterName"]) && !String.IsNullOrEmpty(Request["filterValue"]))
			{
				FilterElement fe = FilterElement.EqualElement(Request["filterName"], Request["filterValue"]);
				FilterElement fe1 = FilterElement.IsNullElement(Request["filterName"]);
				FilterElement feOr = new OrBlockFilterElement();
				feOr.ChildElements.Add(fe);
				feOr.ChildElements.Add(fe1);
				fec.Add(feOr);
			}
			else if (!String.IsNullOrEmpty(Request["filterName"]) && String.IsNullOrEmpty(Request["filterValue"]))
			{
				FilterElement fe = FilterElement.IsNullElement(Request["filterName"]);
				fec.Add(fe);
			}

			if (TreeServiceTargetObjectId != PrimaryKeyId.Empty)
			{
				FilterElement fe = new FilterElement(Mediachase.Ibn.Data.Services.TreeService.OutlineNumberFieldName, FilterElementType.NotContains, TreeServiceTargetObjectId.ToString().ToLower());
				fec.Add(fe);
			}
			grdMain.AddFilters = fec;
			
			ctrlGridEventUpdater.ClassName = ddFilter.SelectedValue;
			ctrlGridEventUpdater.ViewName = "";
			ctrlGridEventUpdater.PlaceName = _placeName;
			ctrlGridEventUpdater.GridId = grdMain.GridClientContainerId;
			ctrlGridEventUpdater.GridActionMode = Mediachase.UI.Web.Apps.MetaUI.Grid.MetaGridServerEventAction.Mode.ListViewUI;
			
			//ddFilter.Visible = (_classes.Count > 1);

			BindDataGrid(!Page.IsPostBack);

			BindButtons();
		}

		#region BindDropDowns
		/// <summary>
		/// Binds the drop downs.
		/// </summary>
		private void BindDropDowns()
		{
			ddFilter.Items.Clear();
			foreach (string className in _classes)
			{
				MetaClass mc = MetaDataWrapper.GetMetaClassByName(className);
				if (mc != null)
					ddFilter.Items.Add(new ListItem(CHelper.GetResFileString(mc.FriendlyName), className));
			}
			if (_classes.Count > 0)
			{
				string key = String.Format("EntitySelect_Class_{0}", String.Join("_", _classes.ToArray()));
				if (_pc[key] != null)
					CHelper.SafeSelect(ddFilter, _pc[key]);
			}
		}
		#endregion

		#region OnPreRender
		protected override void OnPreRender(EventArgs e)
		{
			btnClear.Visible = !String.IsNullOrEmpty(tbSearchString.Text);

			if (CHelper.NeedToBindGrid())
				BindDataGrid(true);

			base.OnPreRender(e);
		}
		#endregion

		#region BindButtons
		private void BindButtons()
		{
			btnSave.Text = GetGlobalResourceObject("IbnFramework.Common", "btnOK").ToString();
			btnSave.CustomImage = this.Page.ResolveUrl("~/Layouts/images/saveitem.gif");
			btnSave.Attributes.Add("onclick", "CheckSelected();return false;");
			btnCancel.Text = GetGlobalResourceObject("IbnFramework.Common", "btnCancel").ToString();
			btnCancel.CustomImage = this.Page.ResolveUrl("~/Layouts/images/cancel.gif");
			btnCancel.Attributes.Add("onclick", Mediachase.Ibn.Web.UI.WebControls.CommandHandler.GetCloseOpenedFrameScript(this.Page, String.Empty, false, true));
			btnNew.Text = GetGlobalResourceObject("IbnFramework.Common", "tAddEntity").ToString();
		}
		#endregion

		#region BindDataGrid
		private void BindDataGrid(bool dataBind)
		{
			grdMain.SearchKeyword = tbSearchString.Text.Trim();
			
			if (dataBind)
				grdMain.DataBind();
		}
		#endregion

		#region ddFilter_SelectedIndexChanged
		/// <summary>
		/// Handles the SelectedIndexChanged event of the ddFilter control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void ddFilter_SelectedIndexChanged(object sender, EventArgs e)
		{
			grdMain.ClassName = ddFilter.SelectedValue;
			ctrlGridEventUpdater.ClassName = ddFilter.SelectedValue;

			string key = String.Format("EntitySelect_Class_{0}", String.Join("_", _classes.ToArray()));
			_pc[key] = ddFilter.SelectedValue;

			BindDataGrid(true);
			grdMainPanel.Update();
		}
		#endregion

		#region InnerEvents
		protected void btnClear_Click(object sender, ImageClickEventArgs e)
		{
			tbSearchString.Text = String.Empty;
			CommonEventPart();
		}

		protected void btnSearch_Click(object sender, ImageClickEventArgs e)
		{
			CommonEventPart();
		}

		private void CommonEventPart()
		{
			upTop.Update();
			grdMainPanel.Update();
			CHelper.RequireBindGrid();
		}
		#endregion

		#region lbSave_Click
		protected void lbSave_Click(object sender, EventArgs e)
		{
			btnSave.Disabled = true;
			btnCancel.Disabled = true;

			CommandParameters cp = new CommandParameters("MC_MUI_EntityDD");
			
			if (Request["ReturnCommand"] != null)
				cp.CommandName = Request["ReturnCommand"];

			cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
			cp.AddCommandArgument("ClassName", ddFilter.SelectedValue);
			if (Request["ObjectId"] != null)
				cp.AddCommandArgument("ObjectId", Request["ObjectId"]);
			if (Request["GridId"] != null)
				cp.AddCommandArgument("GridId", Request["GridId"]);
			if (TreeServiceTargetObjectId != PrimaryKeyId.Empty)
				cp.AddCommandArgument("TreeServiceTargetObjectId", TreeServiceTargetObjectId.ToString());
			cp.AddCommandArgument("SelectedValue", hdnValue.Value);
			
			if (!String.IsNullOrEmpty(Request["SelectCtrlId"]))
			{
				string[] mas = hdnValue.Value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
				if (mas.Length > 0)
				{
					string uid = MetaViewGroupUtil.GetIdFromUniqueKey(mas[0]);
					if (uid != "null")
					{
						cp.AddCommandArgument("SelectObjectId", uid);
						cp.AddCommandArgument("SelectCtrlId", Request["SelectCtrlId"]);
						cp.AddCommandArgument("SelectObjectType", ddFilter.SelectedValue);
						cp.AddCommandArgument("Html", CHelper.GetEntityTitleHtml(ddFilter.SelectedValue, PrimaryKeyId.Parse(uid)));
					}
				}
			}
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}
		#endregion

		#region ibNew_Click
		protected void btnNew_Click(object sender, EventArgs e)
		{
			DockTop.DefaultSize = 0;
			tblSelect.Visible = false;
			tblNew.Visible = true;
			btnSave.Attributes.Remove("onclick");
			btnCancel.Attributes.Remove("onclick");

			EntityObject eo = BusinessManager.InitializeEntity(ddFilter.SelectedValue);
			docFormView.FormType = Mediachase.Ibn.Web.UI.Controls.Util.FormType.Edit;
			docFormView.FormName = FormController.BaseFormType;
			docFormView.DataItem = eo;
			docFormView.DataBind();
		} 
		#endregion

		#region Save New Object
		protected void btnSave_ServerClick(object sender, EventArgs e)
		{
			this.Page.Validate();
			if (!this.Page.IsValid)
			{
				btnSave.Attributes.Remove("onclick");
				btnCancel.Attributes.Remove("onclick");
				return;
			}

			EntityObject _bindObject = BusinessManager.InitializeEntity(ddFilter.SelectedValue);

			if (_bindObject != null)
			{
				ProcessCollection(this.Page.Controls, (EntityObject)_bindObject);

				// Save container id
				if (!String.IsNullOrEmpty(ContainerFieldName)
					&& ((EntityObject)_bindObject).Properties[ContainerFieldName] != null
					&& ContainerId != PrimaryKeyId.Empty)
				{
					((EntityObject)_bindObject)[ContainerFieldName] = ContainerId;
				}

				PrimaryKeyId objectId = BusinessManager.Create(_bindObject);

				CommandParameters cp = new CommandParameters("MC_MUI_EntityDD");

				if (Request["ReturnCommand"] != null)
					cp.CommandName = Request["ReturnCommand"];

				cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
				cp.AddCommandArgument("ClassName", ddFilter.SelectedValue);
				if (Request["ObjectId"] != null)
					cp.AddCommandArgument("ObjectId", Request["ObjectId"]);
				if (Request["GridId"] != null)
					cp.AddCommandArgument("GridId", Request["GridId"]);
				cp.AddCommandArgument("SelectedValue", objectId.ToString());
				cp.AddCommandArgument("SelectObjectId", objectId.ToString());
				cp.AddCommandArgument("SelectObjectType", ddFilter.SelectedValue);
				cp.AddCommandArgument("Html", CHelper.GetEntityTitleHtml(ddFilter.SelectedValue, objectId));

				if (!String.IsNullOrEmpty(Request["SelectCtrlId"]))
					cp.AddCommandArgument("SelectCtrlId", Request["SelectCtrlId"]);

				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
			}
		}

		private void ProcessCollection(ControlCollection _coll, EntityObject _obj)
		{
			foreach (Control c in _coll)
			{
				ProcessControl(c, _obj);
				if (c.Controls.Count > 0)
					ProcessCollection(c.Controls, _obj);
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
				MetaClass ownClass = MetaDataWrapper.GetMetaClassByName(_obj.MetaClassName);
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
				{
					ownField = ownClass.Fields[ownFieldName];
					if (ownField == null)
						ownField = ownClass.CardOwner.Fields[ownFieldName];
				}
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
							aggrObj = BusinessManager.InitializeEntity(aggrClassName);
						aggrObj[aggrFieldName] = eValue;
					}
				}
			}
		} 
		#endregion

		#region btnCancel_ServerClick
		protected void btnCancel_ServerClick(object sender, EventArgs e)
		{
			DockTop.DefaultSize = 33;
			tblSelect.Visible = true;
			tblNew.Visible = false;
		} 
		#endregion

		#region GetExcludeFilter
		private string GetExcludeFilter(string ids, string primaryKeyName)
		{
			string retVal = String.Empty;
			string[] mas = ids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
			if (mas.Length > 0)
				for (int i = 0; i < mas.Length; i++)
				{
					if (i > 0)
						retVal += " AND ";
					retVal += String.Format("({1} <> {0})", mas[i], primaryKeyName);
				}
			return retVal;
		}
		#endregion
	}
}