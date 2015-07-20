using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Core;

using Mediachase.Ibn.Data.Services;

namespace Mediachase.Ibn.Web.UI.MetaUI.ActionControls
{
	public partial class ColumnEditorModalPopup : System.Web.UI.UserControl
	{

		#region prop: SaveToBase
		public bool SaveToBase
		{
			get
			{
				if (ViewState["SaveToBase"] == null)
					ViewState["SaveToBase"] = true;

				return Convert.ToBoolean(ViewState["SaveToBase"]);
			}
			set
			{
				ViewState["SaveToBase"] = value;
			}
		} 
		#endregion

		#region prop: ViewName
		public string ViewName
		{
			get
			{
				if (ViewState["MetaViewName"] == null)
					ViewState["MetaViewName"] = CHelper.GetFromContext("MetaViewName");
				return ViewState["MetaViewName"].ToString();
			}
		}
		#endregion

		#region prop: ViewPreference - principalId
		private McMetaViewPreference viewPreference;

		public McMetaViewPreference ViewPreference
		{
			get
			{
				if (viewPreference == null)
				{
					viewPreference = Mediachase.Ibn.Core.UserMetaViewPreference.Load(ViewName, (int)DataContext.Current.CurrentUserId);
				}

				return viewPreference;
			}
		}
		#endregion

		#region prop: CurrentView
		private MetaView currentView;

		public MetaView CurrentView
		{
			get
			{
				if (currentView == null)
				{
					if (Mediachase.Ibn.Data.DataContext.Current.MetaModel.MetaViews[ViewName] == null)
						throw new ArgumentException(String.Format("Cant find meta view: {0}", ViewName));

					currentView = Mediachase.Ibn.Data.DataContext.Current.MetaModel.MetaViews[ViewName];
				}

				return currentView;
			}
		}
		#endregion

		#region prop: GridId
		private string gridId;

		public string GridId
		{
			get { return gridId; }
			set { gridId = value; }
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			btnSave.Click += new EventHandler(btnSave_Click);
			btnSave.OnClientClick = String.Format("mc_grid_columnEditor_hide();");

			btnCancelPopup.OnClientClick = String.Format("mc_grid_columnEditor_hide(); return false;");

			lbLeftColumn.Attributes.Add("ondblclick", "RemoveHPSection('left');");
			lbRightColumn.Attributes.Add("ondblclick", "RemoveHPSection('right');");

		}

		void btnSave_Click(object sender, EventArgs e)
		{
			if (this.SaveToBase)
			{
				ViewPreference.ShowAllMetaField();

				string hiddenColumns = in_leftcol_hide.Value;
				if (hiddenColumns.Length > 0)
				{
					foreach (string fieldName in hiddenColumns.Split(','))
					{
						ViewPreference.HideMetaField(fieldName);
					}
				}
			}
			else
			{
				ViewPreference.Attributes.Set("tmp_columnsToShow", in_rightcol_hide.Value);
				ViewPreference.Attributes.Set("tmp_columnsToHide", in_leftcol_hide.Value);


				CurrentView.AvailableFields.Clear();
				List<MetaField> allFields = CHelper.GetAllMetaFields(CurrentView);

				foreach (string s in in_rightcol_hide.Value.Split(','))
				{
					foreach (MetaField field in allFields)
					{
						if (field.Name == s)
						{
							CurrentView.AvailableFields.Add(field);
						}
					}
				}
			}

			UserMetaViewPreference.Save((int)DataContext.Current.CurrentUserId, ViewPreference);
			CHelper.UpdateParentPanel(this);
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			BindLists();
		}

		#region BindLists
		void BindLists()
		{
			in_rightcol_hide.Value = string.Empty;
			in_leftcol_hide.Value = string.Empty;
			lbRightColumn.Items.Clear();
			lbLeftColumn.Items.Clear();
			if (this.SaveToBase)
			{
				foreach (MetaField field in CurrentView.AvailableFields)
				{
					//if (!field.IsReferencedField)
					//{
						if (ViewPreference.HidenFields.Contains(field.Name))
						{

							lbLeftColumn.Items.Add(new ListItem(CHelper.GetResFileString(field.FriendlyName), field.Name));
							if (in_leftcol_hide.Value == string.Empty)
								in_leftcol_hide.Value = field.Name;
							else
								in_leftcol_hide.Value += "," + field.Name;
						}
						else
						{
							lbRightColumn.Items.Add(new ListItem(CHelper.GetResFileString(field.FriendlyName), field.Name));
							if (in_rightcol_hide.Value == string.Empty)
								in_rightcol_hide.Value = field.Name;
							else
								in_rightcol_hide.Value += "," + field.Name;
						}
					//}
				}
			}
			else
			{
				foreach (MetaField field in CHelper.GetAllMetaFields(CurrentView))
				{
					if (CurrentView.AvailableFields.Contains(CurrentView.MetaClass.Fields[field.Name]))
					{
						lbRightColumn.Items.Add(new ListItem(CHelper.GetResFileString(field.FriendlyName), field.Name));
						if (in_rightcol_hide.Value == string.Empty)
							in_rightcol_hide.Value = field.Name;
						else
							in_rightcol_hide.Value += "," + field.Name;
					}
					else
					{
						lbLeftColumn.Items.Add(new ListItem(CHelper.GetResFileString(field.FriendlyName), field.Name));
						if (in_leftcol_hide.Value == string.Empty)
							in_leftcol_hide.Value = field.Name;
						else
							in_leftcol_hide.Value += "," + field.Name;
					}
				}
			}
		}
		#endregion

	}
}