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

using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Web.UI.Controls.Util;


namespace Mediachase.Ibn.Web.UI.MetaUI.Primitives
{
	public partial class BackReference_Edit : System.Web.UI.UserControl, IEditControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		#region IEditControl Members

		#region Value
		public object Value
		{
			set
			{
				if (value != null)
				{
					string sReferencedClass = ViewState["ReferencedClass"].ToString();
					MetaClass mc = DataContext.Current.MetaModel.MetaClasses[sReferencedClass];

					lblValue.Text = String.Empty;
					MetaObject[] objList = (MetaObject[])value;

					if ((bool)mc.Attributes[MetaClassAttribute.IsBridge])
					{
						MetaField refField = null;
						foreach (MetaField field in mc.Fields)
						{
							if (field.GetMetaType().McDataType == McDataType.Reference
								&& (bool)field.Attributes[MetaClassAttribute.IsSystem]
								//&& field.Attributes[MetaClassAttribute.IsSystem].ToString() == "1"
								&& field.Name != ViewState["ReferencedField"].ToString()
								)
							{
								refField = field;
								break;
							}
						}

						if (refField == null)
							throw new Exception("Referenced field is not found");

						string refClassName = refField.Attributes[McDataTypeAttribute.ReferencedFieldMetaClassName].ToString();


						foreach (MetaObject obj in objList)
						{
							mc = DataContext.Current.MetaModel.MetaClasses[refClassName];
							MetaObject refObj = new MetaObject(mc, obj.PrimaryKeyId.Value);
							lblValue.Text += refObj.Properties[mc.TitleFieldName].Value.ToString();
						}
					}
					else  // Info class
					{
						foreach (MetaObject obj in objList)
						{
							if (lblValue.Text != String.Empty)
								lblValue.Text += "<br />";

							lblValue.Text += obj.Properties[mc.TitleFieldName].Value.ToString();
						}
					}
				}
			}
			get
			{
				return null;
			}
		}
		#endregion

		#region ShowLabel
		public bool ShowLabel
		{
			set { }
			get { return true; }
		}
		#endregion

		#region Label
		public string Label
		{
			set { }
			get { return ""; }
		}
		#endregion

		#region AllowNulls
		public bool AllowNulls
		{
			set
			{
			}
			get
			{
				return true;
			}
		}
		#endregion

		#region RowCount
		public int RowCount
		{
			set
			{
				int iHeight = 20 * value + 10 * (value - 1);
				divBlock.Style.Add(HtmlTextWriterStyle.Height, iHeight.ToString() + "px");
			}
			get
			{
				Unit h = Unit.Parse(divBlock.Style[HtmlTextWriterStyle.Height]);
				return (int)((h.Value + 10) / 30);
			}
		}
		#endregion

		#region ReadOnly
		public bool ReadOnly
		{
			set
			{
			}
			get
			{
				return true;
			}
		}
		#endregion

		#region LabelWidth
		public string LabelWidth
		{
			set { }
			get { return ""; }
		}
		#endregion

		#region BindData
		public void BindData(MetaField field)
		{
			ViewState["FieldType"] = field.TypeName;

			string sReferencedClass = field.Attributes[McDataTypeAttribute.BackReferenceMetaClassName].ToString();
			ViewState["ReferencedClass"] = sReferencedClass;

			string sReferencedField = field.Attributes[McDataTypeAttribute.BackReferenceMetaFieldName].ToString();
			ViewState["ReferencedField"] = sReferencedField;
		}
		#endregion

		#region FieldName
		public string FieldName
		{
			get
			{
				if (ViewState["FieldName"] != null)
					return ViewState["FieldName"].ToString();
				else
					return "";
			}
			set
			{
				ViewState["FieldName"] = value;
			}
		} 
		#endregion

		#region TabIndex
		public short TabIndex
		{
			set
			{
				lblValue.TabIndex = value;
			}
		}
		#endregion

		#endregion
	}
}