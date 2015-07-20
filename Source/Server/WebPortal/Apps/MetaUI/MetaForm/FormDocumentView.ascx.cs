using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core.Layout;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Core.Business;

namespace Mediachase.Ibn.Web.UI.MetaUI
{
	public partial class FormDocumentView : MCDataBoundControl
	{
		#region FormName
		private string formName = String.Empty;
		public string FormName
		{
			get
			{
				return formName;
			}
			set
			{
				formName = value;
			}
		}
		#endregion

		#region FormType
		private FormType formType = FormType.NotAssigned;
		public FormType FormType
		{
			get
			{
				return formType;
			}
			set
			{
				formType = value;
			}
		}
		#endregion

		#region PlaceName
		private string placeName = String.Empty;
		public string PlaceName
		{
			get
			{
				return placeName;
			}
			set
			{
				placeName = value;
			}
		}
		#endregion

		#region FormExists
		private bool formExists = false;
		public bool FormExists
		{
			get { return formExists; }
		} 
		#endregion

		#region OuterCssClass
		public string OuterCssClass
		{
			get
			{
				return OuterDiv.Attributes["class"];
			}
			set
			{
				OuterDiv.Attributes.Add("class", value);
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
		}

		#region DataBind
		public override void  DataBind()
		{
			string className = "";
			if (DataItem != null)
			{
				MetaObject mo = null;
				EntityObject eo = null;
				if (DataItem is MetaObject)
					mo = (MetaObject)DataItem;
				else if (DataItem is BusinessObjectRequest)
					mo = ((BusinessObjectRequest)DataItem).MetaObject;
				else if (DataItem is EntityObject)
					eo = (EntityObject)DataItem;

				if (mo != null)
				{
					className = mo.GetMetaType().Name;
					if (mo.GetCardMetaType() != null)
						className = mo.GetCardMetaType().Name;
				}
				else if (eo != null)
				{
					className = eo.MetaClassName;
					if (HistoryManager.MetaClassIsHistory(className))
						FormName = FormController.GeneralViewHistoryFormType;
				}
				
				ViewState["ClassName"] = className;

				if (String.IsNullOrEmpty(FormName))
				{
					if (DataItem is MetaObject)
						FormName = FormController.BaseFormType;
					else if (DataItem is BusinessObjectRequest)
						FormName = FormController.CreateFormType;
					else if (DataItem is EntityObject)
						FormName = FormController.BaseFormType;
				}
			}
			else if (ViewState["ClassName"] != null)
				className = ViewState["ClassName"].ToString();

			FormDocument doc = FormController.LoadFormDocument(className, FormName);
			if (doc != null)
			{
				this.formExists = true;
				fRenderer.FormDocumentData = doc;
				fRenderer.FormType = FormType;
				fRenderer.PlaceName = PlaceName;
				if (DataItem != null)
					fRenderer.DataItem = DataItem;
				fRenderer.DataBind();
			}
		}
		#endregion
	}
}