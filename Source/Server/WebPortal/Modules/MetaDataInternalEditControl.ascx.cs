using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

using Mediachase.IBN.Business;
using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.WebSaltatoryControl;
using Mediachase.UI.Web.Modules.EditControls;
using Mediachase.Ibn.Web.Interfaces;

namespace Mediachase.UI.Web.Modules
{
	public partial class MetaDataInternalEditControl : System.Web.UI.UserControl
	{
		#region MetaClassName
		public string MetaClassName
		{
			set
			{
				ViewState["MetaClassName"] = value;
			}
			get
			{
				string retval = string.Empty;
				if (ViewState["MetaClassName"] != null)
					retval = ViewState["MetaClassName"].ToString();
				return retval;
			}
		}
		#endregion

		#region ObjectId
		public int ObjectId
		{
			set
			{
				ViewState["ObjectId"] = value;
			}
			get
			{
				int retval = -1;
				if (ViewState["ObjectId"] != null)
					retval = (int)ViewState["ObjectId"];
				return retval;
			}
		}
		#endregion

		private Table mainTable;
		private PageView pageView;
		MetaClass mc;
		MetaObject obj;
		bool wasBound = false;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!wasBound)
				BindData(false);
		}

		#region BindData
		public void BindData()
		{
			BindData(true);
		}

		private void BindData(bool needToBind)
		{
			pageView = new PageView(false, PageViewType.Static, string.Empty, this.Page, MetaClassName);

			mc = MetaClass.Load(MetaClassName);

			if (ObjectId > 0)
				obj = MetaDataWrapper.LoadMetaObject(ObjectId, MetaClassName);
			if (obj == null)
				obj = MetaDataWrapper.NewMetaObject(ObjectId, MetaClassName);

			GenerateMainTable();

			LoadControls(needToBind);

			wasBound = true;
		} 
		#endregion

		#region GenerateMainTable
		private void GenerateMainTable()
		{
			this.Controls.Clear();

			mainTable = new Table();
			mainTable.ID = "MainTable";
			mainTable.Width = Unit.Percentage(100);
			mainTable.CssClass = "text";
			mainTable.CellPadding = 3;
			mainTable.CellSpacing = 0;

			this.Controls.Add(mainTable);
		}
		#endregion

		#region LoadControls
		private void LoadControls(bool needToBind)
		{
			// We can have up to the 4 ControlPlaces: CntrlPlTop, CntrlPlLeft, CntrlPlRight, CntrlPlBottom
			// Each ControlPlace has a collection of blocks (block has a Name and collection of metafields)

			// Top
			ControlData[] topBlocks = ControlManager.Config.SettingsStorage.Load(pageView, "CntrlPlTop");
			if (topBlocks.Length > 0)
			{
				TableRow row = new TableRow();
				mainTable.Rows.Add(row);

				TableCell cell = new TableCell();
				cell.ColumnSpan = 2;
				row.Cells.Add(cell);
				LoadBlocksToCell(cell, topBlocks, "top", needToBind);
			}

			// Left, Right
			ControlData[] leftBlocks = ControlManager.Config.SettingsStorage.Load(pageView, "CntrlPlLeft");
			ControlData[] rightBlocks = ControlManager.Config.SettingsStorage.Load(pageView, "CntrlPlRight");
			if (leftBlocks.Length > 0 || rightBlocks.Length > 0)
			{
				TableRow row = new TableRow();
				mainTable.Rows.Add(row);

				TableCell leftCell = new TableCell();
				leftCell.Width = Unit.Percentage(50);
				leftCell.VerticalAlign = VerticalAlign.Top;
				row.Cells.Add(leftCell);
				LoadBlocksToCell(leftCell, leftBlocks, "left", needToBind);

				TableCell rightCell = new TableCell();
				rightCell.Width = Unit.Percentage(50);
				rightCell.VerticalAlign = VerticalAlign.Top;
				row.Cells.Add(rightCell);
				LoadBlocksToCell(rightCell, rightBlocks, "right", needToBind);
			}

			// Bottom
			ControlData[] bottomBlocks = ControlManager.Config.SettingsStorage.Load(pageView, "CntrlPlBottom");
			if (bottomBlocks.Length > 0)
			{
				TableRow row = new TableRow();
				mainTable.Rows.Add(row);

				TableCell cell = new TableCell();
				cell.ColumnSpan = 2;
				row.Cells.Add(cell);
				LoadBlocksToCell(cell, bottomBlocks, "bottom", needToBind);
			}
		}
		#endregion

		#region LoadBlocksToCell
		private void LoadBlocksToCell(TableCell cell, ControlData[] blocks, string prefix, bool needToBind)
		{
			for (int i = 0; i < blocks.Length; i++)
			{
				if (!String.IsNullOrEmpty(blocks[i].Settings))
				{
					XmlDocument settings = new XmlDocument();
					settings.LoadXml(blocks[i].Settings);
					XmlNode nameNode = settings.SelectSingleNode("MetaDataBlockViewControl/Name");
					XmlNodeList mfNodeList = settings.SelectNodes("MetaDataBlockViewControl/MetaField");

					// BlockHeader
					BlockHeaderLightWithMenu blockHeader = (BlockHeaderLightWithMenu)LoadControl("~/Modules/BlockHeaderLightWithMenu.ascx");
					blockHeader.ID = String.Concat(prefix, i.ToString());
					cell.Controls.Add(blockHeader);

					// Collapsible Table 
					Table collapsibleTable = new Table();
					collapsibleTable.CssClass = "ibn-stylebox-light text";
					collapsibleTable.CellSpacing = 0;
					collapsibleTable.CellPadding = 5;
					collapsibleTable.Width = Unit.Percentage(100);
					collapsibleTable.ID = String.Concat("tbl", prefix, i.ToString());
					cell.Controls.Add(collapsibleTable);

					blockHeader.CollapsibleControlId = collapsibleTable.ID;
					blockHeader.AddText(nameNode.InnerText);

					foreach (XmlNode mfNode in mfNodeList)
					{
						foreach (MetaField field in mc.UserMetaFields)
						{
							if (field.Name == mfNode.InnerText)
							{
								TableRow row = new TableRow();
								collapsibleTable.Rows.Add(row);

								TableCell cellTitle = new TableCell();
								cellTitle.VerticalAlign = VerticalAlign.Middle;
								cellTitle.CssClass = "ibn-label";
								cellTitle.Width = Unit.Pixel(220);
								cellTitle.Text = String.Concat(field.FriendlyName, ":");
								row.Cells.Add(cellTitle);

								TableCell cellValue = new TableCell();
								row.Cells.Add(cellValue);

								object fieldValue = obj[field.Name];
								System.Web.UI.UserControl control = null;

								switch (field.DataType)
								{
									case MetaDataType.Binary:
										cellValue.Text = "[BinaryData]";
										break;
									case MetaDataType.File:
										cellTitle.VerticalAlign = VerticalAlign.Top;
										control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/FileValue.ascx");
										break;
									case MetaDataType.ImageFile:
										cellTitle.VerticalAlign = VerticalAlign.Top;
										control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/ImageFileValue.ascx");
										break;
									case MetaDataType.DateTime:
										Mediachase.UI.Web.Modules.EditControls.DateTimeValue control_datetime = (Mediachase.UI.Web.Modules.EditControls.DateTimeValue)Page.LoadControl("~/Modules/EditControls/DateTimeValue.ascx");
										control_datetime.Path_JS = "../../Scripts/";
										control = (System.Web.UI.UserControl)control_datetime;
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
										Mediachase.UI.Web.Modules.EditControls.DateValue control_date = (Mediachase.UI.Web.Modules.EditControls.DateValue)Page.LoadControl("~/Modules/EditControls/DateValue.ascx");
										control_date.Path_JS = "../../Scripts/";
										control = (System.Web.UI.UserControl)control_date;
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
										cellTitle.VerticalAlign = VerticalAlign.Top;
										control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/LongStringValue.ascx");
										break;
									case MetaDataType.LongHtmlString:
										cellTitle.VerticalAlign = VerticalAlign.Top;
										control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/LongHTMLStringValue.ascx");
										break;
									case MetaDataType.DictionarySingleValue:
									case MetaDataType.EnumSingleValue:
										control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/DictionarySingleValue.ascx");
										((DictionarySingleValue)control).InitControl(field.Id, (field.AllowNulls ? !field.IsRequired : field.AllowNulls));
										break;
									case MetaDataType.DictionaryMultivalue:
									case MetaDataType.EnumMultivalue:
										cellTitle.VerticalAlign = VerticalAlign.Top;
										control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/DictionaryMultivalue.ascx");
										((DictionaryMultivalue)control).InitControl(field.Id);
										break;
									default:
										if (fieldValue != null)
											cellValue.Text = fieldValue.ToString();
										break;
								}

								if (control != null)
								{
									cellValue.Controls.Add(control);

									if (field.DataType == MetaDataType.File)
									{
										((FileValue)control).MetaClassName = MetaClassName;
										((FileValue)control).ObjectId = ObjectId;
									}
									else if (field.DataType == MetaDataType.ImageFile)
									{
										((ImageFileValue)control).MetaClassName = MetaClassName;
										((ImageFileValue)control).ObjectId = ObjectId;
									}

									ICustomField iCustomField = ((ICustomField)control);
									iCustomField.FieldName = field.Name;
									if (fieldValue != null && needToBind)
										iCustomField.Value = fieldValue;
									iCustomField.AllowEmptyValues = !mc.GetFieldIsRequired(field);
								}
							}
						}
					}
				}
			}
		}
		#endregion

		#region Save
		public void Save(int objectId)
		{
			obj = MetaDataWrapper.LoadMetaObject(objectId, MetaClassName);
			if (obj == null)
				obj = MetaDataWrapper.NewMetaObject(objectId, MetaClassName);

			if (mainTable.Controls.Count > 0)
				ProcessCollection(mainTable.Controls, obj);

			MetaDataWrapper.AcceptChanges(obj);
		} 
		#endregion

		#region ProcessCollection
		private void ProcessCollection(ControlCollection coll, MetaObject obj)
		{
			foreach (Control ctrl in coll)
			{
				if (ctrl is ICustomField)
				{
					ICustomField field = (ICustomField)ctrl;
					object FieldValue = field.Value;
					string FieldName = field.FieldName;

					obj[FieldName] = FieldValue;
				}
				else if (ctrl.HasControls())
				{
					ProcessCollection(ctrl.Controls, obj);
				}
			}
		} 
		#endregion
	}
}