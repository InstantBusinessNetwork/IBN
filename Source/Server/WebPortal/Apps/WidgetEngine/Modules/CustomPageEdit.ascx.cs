using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Business.Customization;
using Mediachase.IBN.Business.WidgetEngine;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI.WebControls;
using System.Collections;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.Apps.WidgetEngine.Modules
{
	public partial class CustomPageEdit : System.Web.UI.UserControl
	{
		#region CustomPageId
		protected PrimaryKeyId CustomPageId
		{
			get
			{
				PrimaryKeyId retval = PrimaryKeyId.Empty;
				if (!String.IsNullOrEmpty(Request["Id"]))
					retval = PrimaryKeyId.Parse(Request["Id"]);

				return retval;
			}
		}
		#endregion

		#region ProfileId
		protected int? ProfileId
		{
			get
			{
				int? retval = null;

				if (String.Compare(Request["ClassName"], CustomizationProfileEntity.ClassName, true) == 0
					&& !String.IsNullOrEmpty(Request["ObjectId"]))
					retval = int.Parse(Request["ObjectId"]);
				return retval;
			}
		}
		#endregion

		#region UserId
		protected int? UserId
		{
			get
			{
				int? retval = null;

				if (String.Compare(Request["ClassName"], "Principal", true) == 0
					&& !String.IsNullOrEmpty(Request["ObjectId"]))
					retval = int.Parse(Request["ObjectId"]);
				return retval;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				BindInfo();
				BindTemplates();
				BindData();
			}
		}

		#region BindInfo
		private void BindInfo()
		{
			if (CustomPageId == PrimaryKeyId.Empty)
				MainHeader.Title = GetGlobalResourceObject("IbnFramework.Profile", "NewPage").ToString();
			else
				MainHeader.Title = GetGlobalResourceObject("IbnFramework.Profile", "PageEditing").ToString();

			string text = CHelper.GetIconText(GetGlobalResourceObject("IbnFramework.Common", "Back").ToString(), ResolveClientUrl("~/Images/IbnFramework/cancel.GIF"));
			string link = ResolveClientUrl(GetLinkToList());
			MainHeader.AddLink(text, link);

			SaveButton.CustomImage = ResolveUrl("~/layouts/images/saveitem.gif");
			SaveButton.Text = GetGlobalResourceObject("IbnFramework.Global", "_mc_Save").ToString();
			CancelButton.Text = GetGlobalResourceObject("IbnFramework.Global", "_mc_Cancel").ToString();
		} 
		#endregion

		#region BindTemplates
		private void BindTemplates()
		{
			List<ListItem> listItems = new List<ListItem>();
			foreach (WorkspaceTemplateInfo wti in WorkspaceTemplateFactory.GetTemplateInfos())
			{
				listItems.Add(new ListItem(CHelper.GetResFileString(wti.Title), wti.Uid.ToLowerInvariant()));
			}
			
			listItems.Sort(delegate(ListItem x, ListItem y) { return x.Text.CompareTo(y.Text); });
			TemplateList.Items.AddRange(listItems.ToArray());
		} 
		#endregion

		#region BindData
		private void BindData()
		{
			if (CustomPageId != PrimaryKeyId.Empty)
			{
				CustomPageEntity page = (CustomPageEntity)BusinessManager.Load(CustomPageEntity.ClassName, CustomPageId);
				ctrlTitleText.Text = page.Title;
				DescriptionText.Text = page.Description;
				CHelper.SafeSelect(TemplateList, page.TemplateId.ToString().ToLowerInvariant());
			}
		} 
		#endregion

		#region SaveButton_ServerClick
		protected void SaveButton_ServerClick(object sender, EventArgs e)
		{
			string className = string.Empty;
			string objectIdString = string.Empty;
			int objectId = 0;
			if (int.TryParse(Request["ObjectId"], out objectId))	// for portal level we can get "{QueryString:ObjectId}"
			{
				objectIdString = objectId.ToString();
				className = Request["ClassName"];
			}

			PrimaryKeyId pageId = PrimaryKeyId.Empty;
			if (CustomPageId == PrimaryKeyId.Empty)	// new
			{
				Guid pageUid = Guid.NewGuid();

				CustomPageEntity page = BusinessManager.InitializeEntity<CustomPageEntity>(CustomPageEntity.ClassName);
				page.Title = ctrlTitleText.Text; //TitleText.Text;
				page.Description = DescriptionText.Text;
				page.TemplateId = new Guid(TemplateList.SelectedValue);
				page.ProfileId = ProfileId;
				page.UserId = UserId;
				page.Uid = pageUid;

				// Json
				List<CpInfo> list = new List<CpInfo>();
				WorkspaceTemplateInfo wti = WorkspaceTemplateFactory.GetTemplateInfo(TemplateList.SelectedValue);
				List<ColumnInfo> columnInfoList = UtilHelper.JsonDeserialize<List<ColumnInfo>>(wti.ColumnInfo);
				foreach (ColumnInfo columnInfo in columnInfoList)
				{
					CpInfo cpInfo = new CpInfo();
					cpInfo.Id = columnInfo.Id;
					cpInfo.Items = new List<CpInfoItem>();
					list.Add(cpInfo);
				}
				page.JsonData = UtilHelper.JsonSerialize(list);
				//

				pageId = BusinessManager.Create(page);

				Response.Redirect(
					String.Format(CultureInfo.InvariantCulture,
						"~/Apps/WidgetEngine/Pages/CustomPageDesign.aspx?id={0}&ClassName={1}&ObjectId={2}&PageUid={3}",
						pageId,
						className,
						objectIdString,
						pageUid)
					);
			}
			else // edit
			{
				CustomPageEntity page = (CustomPageEntity)BusinessManager.Load(CustomPageEntity.ClassName, CustomPageId);
				Guid pageUid = page.Uid;
				Guid newTemplateId = new Guid(TemplateList.SelectedValue);

				// if we edit the page from the current layer
				if ((page.UserId.HasValue && UserId.HasValue && (int)page.UserId.Value == UserId.Value)
					||
					(page.ProfileId.HasValue && ProfileId.HasValue && (int)page.ProfileId.Value == ProfileId.Value)
					||
					(!page.UserId.HasValue && !page.ProfileId.HasValue && !UserId.HasValue && !ProfileId.HasValue && !page.Properties.Contains(LocalDiskEntityObjectPlugin.IsLoadDiskEntityPropertyName)))
				{
					page.Title = ctrlTitleText.Text;// TitleText.Text;
					page.Description = DescriptionText.Text;

					if (page.TemplateId != newTemplateId)
					{
						page.TemplateId = newTemplateId;
						page.JsonData = GetNewJson(page.JsonData, newTemplateId);
					}

					BusinessManager.Update(page);
				}
				else // if we edit the page from the above layer
				{
					CustomPageEntity newPage = BusinessManager.InitializeEntity<CustomPageEntity>(CustomPageEntity.ClassName);
					newPage.Title = ctrlTitleText.Text;// TitleText.Text;
					newPage.Description = DescriptionText.Text;
					newPage.ProfileId = ProfileId;
					newPage.UserId = UserId;
					newPage.Uid = pageUid;
					newPage.TemplateId = newTemplateId;
					if (page.TemplateId != newTemplateId)
					{
						page.JsonData = GetNewJson(page.JsonData, newTemplateId);
					}
					else
					{
						newPage.JsonData = page.JsonData;
					}

					BusinessManager.Create(newPage);
				}

				Response.Redirect(GetLinkToList());
			}
		}
		#endregion

		#region GetNewJson
		private string GetNewJson(string jsonData, Guid newTemplateId)
		{
			List<CpInfo> list = UtilHelper.JsonDeserialize<List<CpInfo>>(jsonData);

			WorkspaceTemplateInfo wti = WorkspaceTemplateFactory.GetTemplateInfo(newTemplateId.ToString());

			List<ColumnInfo> templateColumn = UtilHelper.JsonDeserialize<List<ColumnInfo>>(wti.ColumnInfo);

			if (templateColumn.Count == 0)
				throw new ArgumentException("Template must contain minumum 1 column");

			if (list.Count == 0)
				throw new ArgumentException("Invalid workspace personalize data");

			int counter = 0;
			ArrayList deletedItems = new ArrayList();
			foreach (CpInfo info in list)
			{
				if (!templateColumn.Contains(new ColumnInfo(info.Id)))
				{
					info.Id = templateColumn[0].Id;

					if (counter > 0)
					{
						foreach (CpInfoItem item in info.Items)
						{
							list[0].Items.Add(item);
						}

						deletedItems.Add(info);
					}
				}

				counter++;
			}

			if (deletedItems.Count > 0)
			{
				foreach (CpInfo info in deletedItems)
				{
					list.Remove(info);
				}
			}

			return UtilHelper.JsonSerialize(list);
		} 
		#endregion 

		#region CancelButton_ServerClick
		protected void CancelButton_ServerClick(object sender, EventArgs e)
		{
			Response.Redirect(GetLinkToList());
		} 
		#endregion

		#region GetLinkToList
		private string GetLinkToList()
		{
			string url = "~/Apps/Administration/Pages/PortalCustomization.aspx?Tab=PageCustomization";
			if (ProfileId.HasValue)
			{
				url = String.Format(CultureInfo.InvariantCulture,
					"~/Apps/MetaUIEntity/Pages/EntityView.aspx?ClassName={0}&ObjectId={1}&Tab=PageCustomization",
					CustomizationProfileEntity.ClassName,
					ProfileId.Value);
			}
			else if (UserId.HasValue)
			{
				url = String.Format(CultureInfo.InvariantCulture,
					"~/Apps/MetaUIEntity/Pages/EntityView.aspx?ClassName={0}&ObjectId={1}&Tab=PageCustomization",
					"Principal",
					UserId.Value);
			}

			return url;
		} 
		#endregion
	}
}