using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;

namespace Mediachase.Ibn.Web.UI.MetaDataBase.Primitives
{
	public partial class BackReference_Manage : System.Web.UI.UserControl, IManageControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region IManageControl Members
		public string GetDefaultValue(bool AllowNulls)
		{
			return null;
		}

		public Mediachase.Ibn.Data.Meta.Management.AttributeCollection FieldAttributes
		{
			get
			{
				string[] str = ddlClass.SelectedValue.Split(',');
				Mediachase.Ibn.Data.Meta.Management.AttributeCollection Attr = new Mediachase.Ibn.Data.Meta.Management.AttributeCollection();
				Attr.Add(McDataTypeAttribute.BackReferenceMetaClassName, str[0]);
				Attr.Add(McDataTypeAttribute.BackReferenceMetaFieldName, str[1]);
				return Attr;
			}
		}

		public void BindData(MetaClass mc, string FieldType)
		{
			if(mc != null)
				foreach (MetaField field in mc.FindReferencesWithoutBack())
				{
					string RefClassName = field.Owner.Name;
					string RefFieldName = field.Name;

					ddlClass.Items.Add(
						new ListItem(
							String.Format("{0} ({1})", RefClassName, RefFieldName),
							String.Format("{0},{1}", RefClassName, RefFieldName))
						);
				}
		}

		public void BindData(MetaField mf)
		{
			string RefClassName = mf.Attributes[McDataTypeAttribute.BackReferenceMetaClassName].ToString();
			string RefFieldName = mf.Attributes[McDataTypeAttribute.BackReferenceMetaFieldName].ToString();

			ddlClass.Items.Add(
			  new ListItem(
				String.Format("{0} ({1})", RefClassName, RefFieldName),
				String.Format("{0},{1}", RefClassName, RefFieldName))
			  );
			ddlClass.Enabled = false;
		}
		#endregion
	}
}