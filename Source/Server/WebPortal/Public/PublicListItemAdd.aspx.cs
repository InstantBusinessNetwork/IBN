using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Web.UI;
using Mediachase.Ibn.Web.UI.Controls.Util;
using System.Text;
using System.IO;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Public
{
	public partial class PublicListItemAdd : System.Web.UI.Page
	{
		private string _className = "";
		private string _formName = "";

		protected void Page_Load(object sender, EventArgs e)
		{
			GetRequestParams();
			CHelper.AddToContext("ClassName", _className);
			CHelper.AddToContext("FormName", _formName);
			btnSave.Text = GetGlobalResourceObject("IbnFramework.ListInfo", "Save").ToString();
			btnReturn.Text = GetGlobalResourceObject("IbnFramework.ListInfo", "Return").ToString();
			lblResults.Text = GetGlobalResourceObject("IbnFramework.ListInfo", "PublicListItemAddResult").ToString();
			divAdding.Visible = true;
			divResults.Visible = false;

			if (!Page.IsPostBack)
			{
				frmView.FormName = _formName;
				try
				{
					object bindObject = BusinessManager.InitializeEntity(_className);

					frmView.DataItem = bindObject;
					frmView.DataBind();
				}
				catch { }
			}
		}

		#region GetRequestParams
		private void GetRequestParams()
		{
			string ss = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(Request["uid"]));
			ss = HttpUtility.UrlDecode(ss);
			string[] queries = ss.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string query in queries)
			{
				string[] values = query.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
				if (values.Length != 2)
					continue;
				if (values[0] == "ClassName")
					_className = values[1];
				if (values[0] == "FormName")
					_formName = values[1];
			}
		} 
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			btnSave.Visible = frmView.FormExists;

			RegisterScriptTags();
		}
		#endregion

		protected void btnReturn_Click(object sender, EventArgs e)
		{
			Response.Redirect("~/Public/PublicListItemAdd.aspx?uid=" + Request["uid"]);
		}

		#region Save Method
		protected void btnSave_Click(object sender, EventArgs e)
		{
			this.Page.Validate();
			if (!this.Page.IsValid)
				return;

			object bindObject = BusinessManager.InitializeEntity(_className);

			if (bindObject != null)
			{
				ProcessCollection(this.Page.Controls, (EntityObject)bindObject);

				BusinessManager.Create((EntityObject)bindObject);
				
				divAdding.Visible = false;
				divResults.Visible = true;
				//int objectId = ((BusinessObject)bindObject).PrimaryKeyId.Value;

				//Response.Redirect("~/Public/PublicListItemAdd.aspx?uid=" + Request["uid"]);
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

			//BaseServiceEditControl bsc = c as BaseServiceEditControl;
			//if (bsc != null)
			//{
			//    bsc.Save(_obj);
			//}
		}
		#endregion


		#region RegisterScriptTags
		private void RegisterScriptTags()
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/ibn.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/IbnFramework/main.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/IbnFramework/common.js");
		}
		#endregion
	}
}
