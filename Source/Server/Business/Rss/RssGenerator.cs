using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using RssToolkit.Rss;
using Mediachase.Ibn;
using System.Web.UI;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.IBN.Business.Resources;
using Mediachase.Ibn.Core;
using System.Globalization;
using Mediachase.Ibn.Core.Business;
using System.Web.UI.WebControls;
using System.IO;
using Mediachase.Ibn.Web.UI.Controls.Util;
using System.Data;
using Mediachase.IBN.Database;
using System.Security.Principal;
using System.Xml;

namespace Mediachase.IBN.Business.Rss
{
	/// <summary>
	/// Represents RSS generator.
	/// </summary>
	public class RssGenerator
	{
		#region Const
		#endregion

		#region Properties
		public Page Page { get; set; }
		public RssGeneratorParameters Parameters { get; set; }

		protected ListViewProfile CurrentProfile { get; set; }
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="RssGenerator"/> class.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="genParams">The gen params.</param>
		public RssGenerator(Page page, RssGeneratorParameters genParams)
		{
			this.Page = page;
			this.Parameters = genParams;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Creates the RSS link.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="userId">The user id.</param>
		/// <param name="className">Name of the class.</param>
		/// <param name="objectId">The object id.</param>
		/// <param name="currentView">The current view.</param>
		/// <returns></returns>
		public static string CreateRssLink(Page page, string className, int? objectId, string currentView)
		{
			Guid userId = User.GetRssKeyByUserId(Security.CurrentUser.UserID); 

			return page.ResolveUrl(string.Format("~/modules/rssdocument.aspx?u={0}&cn={1}&id={2}&cv={3}",
				userId, 
				className, 
				objectId.HasValue?objectId.Value.ToString():string.Empty,
				currentView == null ? string.Empty : currentView
				));
		}

		/// <summary>
		/// Makes the full link.
		/// </summary>
		/// <param name="relativeLink">The relative link.</param>
		/// <returns></returns>
		private static string MakeFullLink(string relativeLink)
		{
			return string.Format("{0}{1}", Configuration.PortalLink, relativeLink);
		}

		/// <summary>
		/// Generates the specified param.
		/// </summary>
		/// <returns></returns>
		public string Generate()
		{
			ResolveListViewProfile();

			RssDocument rssDocument = new RssDocument();

			rssDocument.Version = "2.0";

			rssDocument.Channel = new RssChannel();
			rssDocument.Channel.Image = new RssImage();

			GenerateChannelInformation(rssDocument.Channel);

			rssDocument.Channel.Copyright = IbnConst.AssemblyCopyright;
			rssDocument.Channel.Generator = IbnConst.AssemblyCompany + "RSS Generator";
			rssDocument.Channel.WebMaster = "";

			rssDocument.Channel.Image.Link = MakeFullLink(this.Page.ResolveUrl("~/"));
			rssDocument.Channel.Image.Title = IbnConst.ProductName;
			rssDocument.Channel.TimeToLive = "30";

			rssDocument.Channel.Items = new List<RssItem>();
			rssDocument.Channel.LastBuildDate = GenerateChannelItems(rssDocument.Channel);

			//dvs[2009-05-28]: fix bug with ie (set encoding to utf-8)
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(rssDocument.ToXml(DocumentType.Rss));
			if (xmlDoc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
			{
				XmlDeclaration xmlDeclaration = (XmlDeclaration)xmlDoc.FirstChild;
				xmlDeclaration.Encoding = "utf-8";
			}

			StringWriter sw = new StringWriter();
			XmlTextWriter xw = new XmlTextWriter(sw);
			xmlDoc.WriteTo(xw);
			return sw.ToString();
		}

		/// <summary>
		/// Resolves the list view profile.
		/// </summary>
		private void ResolveListViewProfile()
		{
			this.CurrentProfile = null;

			if (!string.IsNullOrEmpty(this.Parameters.CurrentView))
			{
				this.CurrentProfile = ListViewProfile.Load(this.Parameters.ClassName, this.Parameters.CurrentView, "EntityList");
			}
		}

		/// <summary>
		/// Generates the channel information.
		/// </summary>
		/// <param name="channel">The channel.</param>
		private void GenerateChannelInformation(RssChannel channel)
		{
			if(this.Parameters.ClassName.StartsWith("List_"))
			{
				string strListId = this.Parameters.ClassName.Replace("List_", string.Empty);
				int listId = int.Parse(strListId);

				if (!ListInfoBus.CanRead(listId))
					throw new Mediachase.Ibn.AccessDeniedException();

				GenerateListChannelInformation(channel);
			}
		}

		/// <summary>
		/// Generates the list channel information.
		/// </summary>
		/// <param name="channel">The channel.</param>
		private void GenerateListChannelInformation(RssChannel channel)
		{

			MetaClass listMetaClass = DataContext.Current.GetMetaClass(this.Parameters.ClassName);

			string strViewName = string.Empty;
			if (this.CurrentProfile != null)
				strViewName = Common.GetWebResourceString(this.CurrentProfile.Name, CultureInfo.CurrentUICulture);

			channel.Title = string.Format(CultureInfo.CurrentUICulture, RssResource.RssChannel_List_Title_Format, listMetaClass.FriendlyName, strViewName, Configuration.Domain);
			channel.Description = string.Format(CultureInfo.CurrentUICulture, RssResource.RssChannel_List_Description_Format, listMetaClass.FriendlyName, strViewName, Configuration.Domain); 
			channel.Link = MakeFullLink(this.Page.ResolveUrl(string.Format(CultureInfo.CurrentUICulture, "~/Apps/MetaUIEntity/Pages/EntityList.aspx?ClassName={0}", listMetaClass.Name)));

			channel.Image.Url = MakeFullLink(this.Page.ResolveUrl("~/layouts/images/lists.gif"));
		}

		/// <summary>
		/// Generates the channel items.
		/// </summary>
		/// <param name="rssChannel">The RSS channel.</param>
		/// <returns></returns>
		private string GenerateChannelItems(RssChannel rssChannel)
		{
			if (this.Parameters.ClassName.StartsWith("List_"))
			{
				return GenerateListChannelItems(rssChannel);
			}

			return RssXmlHelper.ToRfc822(DateTime.Now);
		}

		/// <summary>
		/// Generates the list channel items.
		/// </summary>
		/// <param name="rssChannel">The RSS channel.</param>
		/// <returns></returns>
		private string GenerateListChannelItems(RssChannel rssChannel)
		{
			DateTime lastBuildDate = DateTime.MinValue;

			MetaClass listMetaClass = DataContext.Current.GetMetaClass(this.Parameters.ClassName);

			foreach(EntityObject item in BusinessManager.List(this.Parameters.ClassName,
				this.CurrentProfile.Filters.ToArray(), 
				this.CurrentProfile.Sorting.ToArray()))
			{
				RssItem rssItem = new RssItem();
				rssItem.Guid = new RssGuid();

				rssItem.Title = string.IsNullOrEmpty(listMetaClass.TitleFieldName) ?
					("#" + item.PrimaryKeyId.Value.ToString()) :
					(string)item[listMetaClass.TitleFieldName];

				rssItem.Link = MakeFullLink(this.Page.ResolveUrl(string.Format(CultureInfo.CurrentUICulture, "~/Apps/MetaUIEntity/Pages/EntityList.aspx?ClassName={0}", listMetaClass.Name)));
				rssItem.Guid.IsPermaLink = "false";
				rssItem.Guid.Text = item.PrimaryKeyId.Value.ToString();

				rssItem.Description = RenderListEntityObjectDescription(listMetaClass, item);

				DateTime modified = (DateTime)item["Modified"];

				if (modified > lastBuildDate)
					lastBuildDate = modified;

				rssItem.PubDate = RssXmlHelper.ToRfc822(modified);
				UserLight author = UserLight.Load((int)item["ModifierId"]);
				if(author!=null)
					rssItem.Author = string.Format("{0} <{1}>", author.DisplayName, author.Email);

				rssChannel.Items.Add(rssItem);
			}

			if (lastBuildDate == DateTime.MinValue)
				lastBuildDate = DateTime.Now;

			return RssXmlHelper.ToRfc822(lastBuildDate);
		}

		/// <summary>
		/// Renders the list entity object description.
		/// </summary>
		/// <param name="listMetaClass">The list meta class.</param>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		private string RenderListEntityObjectDescription(MetaClass listMetaClass, EntityObject item)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("<table cellpadding='5'>");
			foreach (string fieldName in this.CurrentProfile.FieldSet)
			{
				if (listMetaClass.Fields.Contains(fieldName))
				{
					BaseEntityType ctrl = (BaseEntityType)GetColumn(this.Page, listMetaClass.Fields[fieldName]);
					ctrl.DataItem = item;
					ctrl.FieldName = fieldName;
					ctrl.DataBind();
					StringBuilder sb2 = new StringBuilder();
					StringWriter tw = new StringWriter(sb2);
					HtmlTextWriter hw = new HtmlTextWriter(tw);
					ctrl.RenderControl(hw);

					if (item.Properties.Contains(fieldName))
						sb.AppendFormat("<tr><td><b>{0}:</b></td><td>{1}</td></tr>", listMetaClass.Fields[fieldName].FriendlyName,
							sb2.ToString());
				}
			}
			sb.Append("</table>");
			//sb.Append("TODO: Description Here");

			return sb.ToString();
		}

		#endregion

		#region GetColumn
		/// <summary>
		/// Gets the column.
		/// </summary>
		/// <param name="PageInstance">The page instance.</param>
		/// <param name="Field">The field.</param>
		/// <returns></returns>
		public Control GetColumn(Page PageInstance, MetaField Field)
		{
			TemplateField retVal = new TemplateField();

			string className = Field.Owner.Name;

			ResolvedPath resPath = ControlPathResolver.Current.Resolve(RssGenerator.GetMetaTypeName(Field), "GridEntity", className, Field.Name, string.Empty);

			return this.Page.LoadControl(resPath.Path);
			//if (resPath != null)
			//    retVal.ItemTemplate = PageInstance.LoadTemplate(resPath.Path);

			//return retVal;
		} 
		#endregion

		#region GetMetaTypeName
		/// <summary>
		/// Gets the name of the meta type.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <returns></returns>
		private static string GetMetaTypeName(MetaField field)
		{
			string typeName = "";
			if (field.IsMultivalueEnum)
				typeName = "EnumMultiValue";
			else if (field.IsEnum)
				typeName = "Enum";
			else if (field.GetMetaType().McDataType == McDataType.MultiReference)
				typeName = "MultiReference";
			else
				typeName = field.GetMetaType().Name;
			return typeName;
		}
		#endregion

		#region LogonUserByRssKey
		public static void LogonUserByRssKey(Guid rssKey)
		{
			using (IDataReader reader = DBUser.GetUserInfoByRssKey(rssKey))
			{
				if (!reader.Read())
					throw new Mediachase.Ibn.AccessDeniedException();

				int userId = (int)reader["UserId"];

				try
				{
					string sUserLight = "userlight";

					// check user's name and password here
					UserLight currentUser = UserLight.Load(userId);
					if (currentUser == null)
						throw new Mediachase.Ibn.AccessDeniedException("Exception in user authentication");

					if (HttpContext.Current.Items.Contains(sUserLight))
						HttpContext.Current.Items.Remove(sUserLight);

					HttpContext.Current.Items.Add(sUserLight, currentUser);
					HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(currentUser.Login), null);
				}
				catch(Exception ex)
				{
					throw new Mediachase.Ibn.AccessDeniedException("Exception in user authentication", ex);
				}
			}
		}
		#endregion
	}
}
