using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Clients;
using Mediachase.Ibn.Lists;
using Bus = Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI
{
	public class CHelper
	{
		protected readonly static int timeOutInterval = 1000;
		public readonly static string NeedToDataBindKey = "NeedToDataBind";
		public readonly static string NeedToBindGridKey = "GridBindFlag";
		public readonly static string MetaClassAdminPage = "~/Apps/MetaDataBase/Pages/Admin/MetaClassView.aspx";
		public readonly static string ListAdminPage = "~/Apps/ListApp/Pages/ListInfoView.aspx";
		public readonly static string PrinterVersionKey = "PrinterVersion";

		#region GetUserName
		public static string GetUserName(int UserID)
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", typeof(CHelper).Assembly);

			string retval = "[" + LocRM.GetString("tUnknownUser") + "]";
			if (UserID > 0 || UserID == -10)
			{
				try
				{
					MetaObject pc = MetaObjectActivator.CreateInstance("Principal", UserID);
					return GetResFileString(pc.Properties[MetaDataWrapper.GetMetaClassByName("Principal").TitleFieldName].Value.ToString());
				}
				catch { }
			}

			return retval;
		}
		#endregion

		#region GetAbsolutePath
		public static string GetAbsolutePath(string xs_Path)
		{
			string UrlScheme = System.Configuration.ConfigurationManager.AppSettings["UrlScheme"];

			StringBuilder builder = new StringBuilder();
			if (UrlScheme != null)
				builder.Append(UrlScheme);
			else
				builder.Append(HttpContext.Current.Request.Url.Scheme);
			builder.Append("://");

			// Oleg Rylin: Fixing the problem with non-default port [6/20/2006]
			builder.Append(HttpContext.Current.Request.Url.Authority);

			string sApp = HttpContext.Current.Request.ApplicationPath;
			if(sApp != "/")
				builder.Append(HttpContext.Current.Request.ApplicationPath);
			builder.Append("/");
			if (xs_Path != string.Empty)
			{
				if (xs_Path[0] == '/')
					xs_Path = xs_Path.Substring(1, xs_Path.Length - 1);
				builder.Append(xs_Path);
			}
			return builder.ToString().Replace("\\\\", "\\");

			//return string.Format(CultureInfo.InvariantCulture,
			//    "{0}{1}{2}",
			//    HttpContext.Current.Request.ApplicationPath,
			//    (xs_Path[0] == '/') ? "" : "/",
			//    xs_Path);
		}
		#endregion

		#region GetFullPath
		public static string GetFullPath(string xs_Path)
		{
			string UrlScheme = System.Configuration.ConfigurationManager.AppSettings["UrlScheme"];

			StringBuilder builder = new StringBuilder();
			if (UrlScheme != null)
				builder.Append(UrlScheme);
			else
				builder.Append(HttpContext.Current.Request.Url.Scheme);
			builder.Append("://");

			// Oleg Rylin: Fixing the problem with non-default port [6/20/2006]
			builder.Append(HttpContext.Current.Request.Url.Authority);

			builder.Append(HttpContext.Current.Request.ApplicationPath);

			if (builder[builder.Length - 1] != '/')
				builder.Append("/");

			if (xs_Path != string.Empty)
			{
				if (xs_Path[0] == '/')
					xs_Path = xs_Path.Substring(1, xs_Path.Length - 1);
				builder.Append(xs_Path);
			}
			return builder.ToString();
		}
		#endregion

		#region GetResFileString
		public static string GetResFileString(string fileAndKey)//for {file:key} values
		{
			return Mediachase.IBN.Business.Common.GetWebResourceString(fileAndKey, CultureInfo.CurrentUICulture);
		}
		#endregion

		#region SafeSelect
		public static void SafeSelect(ListControl ddl, string val)
		{
			ListItem li = ddl.Items.FindByValue(val);
			if (li != null)
			{
				ddl.ClearSelection();
				li.Selected = true;
			}
		}
		#endregion

		#region SafeMultipleSelect
		public static void SafeMultipleSelect(ListControl ddl, string val)
		{
			ListItem li = ddl.Items.FindByValue(val);
			if (li != null)
				li.Selected = true;
		}
		#endregion

		#region GetFullPageTitle
		public static string GetFullPageTitle(string Title)
		{
			string productName = IbnConst.ProductFamily;
			if (Title.Length > 0)
				return String.Format("{0} | {1} {2}", Title, productName, IbnConst.VersionMajorDotMinor);
			else
				return String.Format("{0} {1}", productName, IbnConst.VersionMajorDotMinor); ;
		}
		#endregion

		#region CloseItAndRefresh
		public static void CloseItAndRefresh(HttpResponse response)
		{
			response.Clear();
			response.Write("<script type=\"text/javascript\">");
			response.Write("try {window.opener.document.forms[0].submit();} catch (e) {}");
			response.Write("window.close()");
			response.Write("</script>");
			response.End();
		}

		public static void CloseItAndRefresh(HttpResponse response, string refreshButton)
		{
			response.Clear();
			response.Write("<script language=\"javascript\" type=\"text/javascript\">");
			response.Write(string.Format(CultureInfo.InvariantCulture, "try {{window.opener.{0};}} catch (e) {{}}", refreshButton));
			response.Write(string.Format(CultureInfo.InvariantCulture, "setTimeout('window.close()', {0});", timeOutInterval));
			response.Write("</script>");
			response.End();
		}
		#endregion

		#region CloseIt
		public static void CloseIt(HttpResponse response)
		{
			response.Clear();
			response.Write("<script type=\"text/javascript\">");
			response.Write("window.close();");
			response.Write("</script>");
			response.End();
		}
		#endregion

		#region GetCloseRefreshString
		public static string GetCloseRefreshString(string RefreshButton)
		{
			string retVal = String.Format(CultureInfo.InvariantCulture, "try {{window.opener.{0};}} catch (e) {{}}", RefreshButton);
			retVal += String.Format(CultureInfo.InvariantCulture, "setTimeout('window.close()', {0}); return false;", timeOutInterval);
			return retVal;
		}
		#endregion

		#region AddToContext
		public static void AddToContext(string key, object value)
		{
			if (HttpContext.Current.Items.Contains(key))
				HttpContext.Current.Items[key] = value;
			else
				HttpContext.Current.Items.Add(key, value);
		}
		#endregion

		#region GetFromContext
		public static object GetFromContext(string key)
		{
			if (HttpContext.Current.Items.Contains(key))
				return HttpContext.Current.Items[key];
			else
				return null;
		}
		#endregion

		#region RemoveFromContext
		public static void RemoveFromContext(string key)
		{
			if (HttpContext.Current.Items.Contains(key))
				HttpContext.Current.Items.Remove(key);
		}
		#endregion

		#region GetMetaTypeName
		public static string GetMetaTypeName(MetaField field)
		{
			string typeName = "";
			if (field.IsMultivalueEnum)
				typeName = "EnumMultiValue";
			else if (field.IsEnum)
				typeName = "Enum";
			else if (field.GetMetaType().McDataType == McDataType.MultiReference)
				typeName = "MultiReference";
			else if (field.IsReferencedField) // O.R. [2010-02-24] Fix: Referenced field is Enum
			{
				string refFieldName = field.Attributes[McDataTypeAttribute.ReferencedFieldMetaFieldName].ToString();
				if (field.ReferenceToMetaClass != null && field.ReferenceToMetaClass.Fields[refFieldName] != null)
				{
					MetaField referencedField = field.ReferenceToMetaClass.Fields[refFieldName];
					if (referencedField.IsMultivalueEnum)
						typeName = "EnumMultiValue";
					else if (referencedField.IsEnum)
						typeName = "Enum";
					else
						typeName = field.GetMetaType().Name;
				}
			}
			else
				typeName = field.GetMetaType().Name;
			return typeName;
		}
		#endregion

		#region UpdateParentPanel
		public static void UpdateParentPanel(Control startPoint)
		{
			Control c = startPoint;
			do
			{
				c = c.Parent;
				if (c is UpdatePanel)
				{
					((UpdatePanel)c).Update();
					break;
				}
			} while (c != startPoint.Page);
		}
		#endregion

		#region UpdateAllParentPanels
		public static void UpdateAllParentPanels(Control startPoint)
		{
			Control c = startPoint;
			do
			{
				c = c.Parent;
				if (c is UpdatePanel)
				{
					((UpdatePanel)c).Update();
				}
			} while (c != startPoint.Page);
		}
		#endregion

		#region CheckCardField
		public static bool CheckCardField(MetaClass _class, MetaField cardField)
		{
			string CardPKeyName = string.Format(CultureInfo.InvariantCulture, "{0}Id", cardField.Owner.Name);
			string CardRefKeyName = string.Format(CultureInfo.InvariantCulture, "{0}Id", _class.Name);
			return (cardField.Name != CardRefKeyName &&
					cardField.Name != CardPKeyName &&
					!(cardField.GetOriginalMetaType().McDataType == McDataType.ReferencedField &&
					cardField.Attributes.GetValue<string>(McDataTypeAttribute.ReferencedFieldMetaClassName) == _class.Name)
					);
		}
		#endregion

		#region GetAllMetaFields
		public static List<MetaField> GetAllMetaFields(MetaView View)
		{
			List<MetaField> retVal = new List<MetaField>();
			foreach (MetaField field in View.MetaClass.Fields)
			{
				retVal.Add(field);
			}

			if (View.Card != null)
			{
				MetaClass mcCard = MetaDataWrapper.GetMetaClassByName(View.Card.Name);
				foreach (MetaField field in mcCard.Fields)
				{
					if (CHelper.CheckCardField(View.MetaClass, field))
						retVal.Add(field);
				}
			}

			return retVal;
		}
		#endregion

		#region GetMetaViewPreference
		public static Mediachase.Ibn.Core.McMetaViewPreference GetMetaViewPreference(MetaView currentView)
		{
			Mediachase.Ibn.Core.McMetaViewPreference preference = Mediachase.Ibn.Core.UserMetaViewPreference.Load(currentView, Mediachase.IBN.Business.Security.CurrentUser.UserID);

			if (preference == null || preference.Attributes.Count == 0)
			{
				//CreateDefaultUserPreference(CurrentView);
				McMetaViewPreference.CreateDefaultUserPreference(currentView);
				preference = Mediachase.Ibn.Core.UserMetaViewPreference.Load(currentView, Mediachase.IBN.Business.Security.CurrentUser.UserID);
			}

			return preference;
		}
		#endregion

		#region GetPermissionIconPath
		public static string GetPermissionIconPath(int rightValue)
		{
			return GetPermissionIconPath(rightValue, false);
		}

		public static string GetPermissionIconPath(int rightValue, bool isInhereted)
		{
			string path;
			path = GetAbsolutePath("/Images/IbnFramework/Blank.gif");
			if (rightValue == (int)Mediachase.Ibn.Data.Services.Security.Rights.Allow)
				path = GetAbsolutePath("/Images/IbnFramework/Shield-Green-Tick.png");
			else if (rightValue == (int)Mediachase.Ibn.Data.Services.Security.Rights.Forbid)
				path = GetAbsolutePath("/Images/IbnFramework/Shield-Red-Cross.png");
			return path;
		}
		#endregion

		#region GetPermissionImage
		public static string GetPermissionImage(int rightValue)
		{
			return GetPermissionImage(rightValue, false);
		}

		public static string GetPermissionImage(int rightValue, bool isInhereted)
		{
			string toolTip;
			if (rightValue == (int)Mediachase.Ibn.Data.Services.Security.Rights.Allow)
				toolTip = HttpContext.GetGlobalResourceObject("IbnFramework.Security", "PermissionAllow").ToString();
			else if (rightValue == (int)Mediachase.Ibn.Data.Services.Security.Rights.Forbid)
				toolTip = HttpContext.GetGlobalResourceObject("IbnFramework.Security", "PermissionForbid").ToString();
			else
				toolTip = HttpContext.GetGlobalResourceObject("IbnFramework.Security", "PermissionNone").ToString();

			return String.Format(CultureInfo.InvariantCulture,
				"<img alt='{1}' src='{0}'/>",
				GetPermissionIconPath(rightValue, isInhereted),
				toolTip);
		}
		#endregion

		#region GetEventResourceString
		public static string GetEventResourceString(MetaObject eventObject)
		{
			string retVal = GetResFileString(eventObject.Properties["EventTitle"].Value.ToString());
			//{event:...}
			MatchCollection coll = Regex.Matches(retVal, "{event:(?<EventProp>[^}]*)}");
			foreach (Match match in coll)
			{
				string sArg = match.Groups["EventProp"].Value;
				retVal = retVal.Replace(match.ToString(), GetResFileString(eventObject.Properties[sArg].Value.ToString()));
			}
			//{args:...}
			if (eventObject.Properties["ArgumentType"].Value != null &&
				eventObject.Properties["ArgumentData"].Value != null)
			{
				string argumentType = eventObject.Properties["ArgumentType"].Value.ToString();
				string argumentData = eventObject.Properties["ArgumentData"].Value.ToString();
				MatchCollection argscoll = Regex.Matches(retVal, "{args:(?<EventArg>[^}]*)}");
				if (argscoll.Count > 0)
				{
					Type objType = Mediachase.Ibn.Data.AssemblyUtil.LoadType(argumentType);
					object obj = McXmlSerializer.GetObject(objType, argumentData);
					if (obj != null)
					{
						foreach (Match match in argscoll)
						{
							string p_name = match.Groups["EventArg"].Value;
							PropertyInfo pinfo = objType.GetProperty(p_name);
							if (pinfo != null)
							{
								string sTemp = pinfo.GetValue(obj, null).ToString();

								retVal = retVal.Replace(match.ToString(), GetResFileString(sTemp));
							}
						}
					}
				}
			}
			return retVal;
		}
		#endregion

		#region GetWeekStartByDate
		public static DateTime GetWeekStartByDate(DateTime start)
		{
			start = start.Date;
			int dow = (int)start.DayOfWeek;
			int fdow = Mediachase.IBN.Business.PortalConfig.PortalFirstDayOfWeek;
			fdow = (fdow == 7) ? 0 : fdow;

			int diff = dow - fdow;
			DateTime result;
			if (diff < 0)
				result = start.AddDays(-(7 + diff));
			else
				result = start.AddDays(-diff);

			if (result.Year < start.Year)
				result = new DateTime(start.Year, 1, 1);

			return result;
		}
		#endregion

		#region GetRealWeekStartByDate
		public static DateTime GetRealWeekStartByDate(DateTime start)
		{
			start = start.Date;
			int dow = (int)start.DayOfWeek;
			int fdow = Mediachase.IBN.Business.PortalConfig.PortalFirstDayOfWeek;
			fdow = (fdow == 7) ? 0 : fdow;

			int diff = dow - fdow;
			DateTime result;
			if (diff < 0)
				result = start.AddDays(-(7 + diff));
			else
				result = start.AddDays(-diff);

			return result;
		}
		#endregion

		#region GetWeekEndByDate
		public static DateTime GetWeekEndByDate(DateTime start)
		{
			start = start.Date;
			int dow = (int)start.DayOfWeek;
			int fdow = Mediachase.IBN.Business.PortalConfig.PortalFirstDayOfWeek;
			fdow = (fdow == 7) ? 0 : fdow;

			int diff = dow - fdow;
			DateTime result;
			if (diff < 0)
				result = start.AddDays(-(7 + diff));
			else
				result = start.AddDays(-diff);
			result = result.AddDays(6);
			if (result.Year > start.Year)
				return new DateTime(start.Year, 12, 31);
			else
				return result;
		}
		#endregion

		#region GetRealWeekEndByDate
		public static DateTime GetRealWeekEndByDate(DateTime start)
		{
			start = start.Date;
			int dow = (int)start.DayOfWeek;
			int fdow = Mediachase.IBN.Business.PortalConfig.PortalFirstDayOfWeek;
			fdow = (fdow == 7) ? 0 : fdow;

			int diff = dow - fdow;
			DateTime result;
			if (diff < 0)
				result = start.AddDays(-(7 + diff));
			else
				result = start.AddDays(-diff);
			result = result.AddDays(6);
			return result;
		}
		#endregion

		#region GetObjectLink
		// 2007-08-21: fix by DV for working with IbnNext Pages
		public static string GetObjectLink(int ObjectTypeId, int ObjectId)
		{
			string retval = "";
			switch (ObjectTypeId)
			{
				case (int)Mediachase.IBN.Business.ObjectTypes.User:
					//retval = String.Format("../Directory/UserView.aspx?UserId={0}", ObjectId);
					retval = String.Format(GetAbsolutePath("/Directory/UserView.aspx?UserId={0}"), ObjectId);
					break;
				case (int)Mediachase.IBN.Business.ObjectTypes.Project:
					//retval = String.Format("../Projects/ProjectView.aspx?ProjectId={0}", ObjectId);
					retval = String.Format(GetAbsolutePath("/Projects/ProjectView.aspx?ProjectId={0}"), ObjectId);
					break;
				case (int)Mediachase.IBN.Business.ObjectTypes.Task:
					//retval = String.Format("../Tasks/TaskView.aspx?TaskId={0}", ObjectId);
					retval = String.Format(GetAbsolutePath("/Tasks/TaskView.aspx?TaskId={0}"), ObjectId);
					break;
				case (int)Mediachase.IBN.Business.ObjectTypes.ToDo:
					//retval = String.Format("../ToDo/ToDoView.aspx?ToDoId={0}", ObjectId);
					retval = String.Format(GetAbsolutePath("/ToDo/ToDoView.aspx?ToDoId={0}"), ObjectId);
					break;
				case (int)Mediachase.IBN.Business.ObjectTypes.CalendarEntry:
					//retval = String.Format("../Events/EventView.aspx?EventId={0}", ObjectId);
					retval = String.Format(GetAbsolutePath("/Events/EventView.aspx?EventId={0}"), ObjectId);
					break;
				case (int)Mediachase.IBN.Business.ObjectTypes.Issue:
					//retval = String.Format("../Incidents/IncidentView.aspx?IncidentId={0}", ObjectId);
					retval = String.Format(GetAbsolutePath("/Incidents/IncidentView.aspx?IncidentId={0}"), ObjectId);
					break;
				case (int)Mediachase.IBN.Business.ObjectTypes.Document:
					//retval = String.Format("../Documents/DocumentView.aspx?DocumentId={0}", ObjectId);
					retval = String.Format(GetAbsolutePath("/Documents/DocumentView.aspx?DocumentId={0}"), ObjectId);
					break;
				case (int)Mediachase.IBN.Business.ObjectTypes.List:
					retval = String.Format(GetAbsolutePath("/Apps/MetaUIEntity/Pages/EntityList.aspx?ClassName=List_{0}"), ObjectId);
					break;
				case (int)Mediachase.IBN.Business.ObjectTypes.IssueRequest:
					//retval = String.Format("../Incidents/MailRequestView.aspx?RequestId={0}", ObjectId);
					retval = String.Format(GetAbsolutePath("/Incidents/MailRequestView.aspx?RequestId={0}"), ObjectId);
					break;
				case (int)Mediachase.IBN.Business.ObjectTypes.KnowledgeBase:
					//retval = String.Format("../Incidents/ArticleView.aspx?ArticleId={0}", ObjectId);
					retval = String.Format(GetAbsolutePath("/Incidents/ArticleView.aspx?ArticleId={0}"), ObjectId);
					break;
			}

			return retval;
		}
		#endregion

		#region GetLinkObjectView 
		public static string GetLinkObjectView(string className, string objectId)
		{
			return String.Format(CultureInfo.InvariantCulture, "~/Apps/MetaUI/Pages/Public/ObjectView.aspx?ClassName={0}&ObjectId={1}", className, objectId);
		}
		#endregion

		#region GetLinkObjectEdit
		public static string GetLinkObjectEdit(string className, string objectId)
		{
			return String.Format(CultureInfo.InvariantCulture, "~/Apps/MetaUI/Pages/Public/ObjectEdit.aspx?ClassName={0}&ObjectId={1}", className, objectId);
		}
		#endregion

		#region GetLinkObjectView_Edit
		public static string GetLinkObjectView_Edit(string className, string objectId)
		{
			if (MetaDataWrapper.GetMetaClassByName(className).Attributes.ContainsKey("HasCompositePage"))
				return GetLinkObjectView(className, objectId);
			else
				return GetLinkObjectEdit(className, objectId);
		}
		#endregion

		#region GetLinkEntityView
		public static string GetLinkEntityView(string className, string objectId)
		{
			return String.Format(CultureInfo.InvariantCulture, "~/Apps/MetaUIEntity/Pages/EntityView.aspx?ClassName={0}&ObjectId={1}", className, objectId);
		}
		#endregion

		#region GetLinkEntityEdit
		public static string GetLinkEntityEdit(string className, string objectId)
		{
			return String.Format(CultureInfo.InvariantCulture, "~/Apps/MetaUIEntity/Pages/EntityEdit.aspx?ClassName={0}&ObjectId={1}", className, objectId);
		}
		#endregion

		#region GetLinkEntityView_Edit
		public static string GetLinkEntityView_Edit(string className, string objectId)
		{
			MetaClass mc = MetaDataWrapper.GetMetaClassByName(className);
			string realClassName = (mc.CardOwner != null) ? mc.CardOwner.Name : mc.Name;
			if (Mediachase.Ibn.Core.Layout.FormDocument.Load(className, FormController.GeneralViewFormType) != null)
				return GetLinkEntityView(realClassName, objectId);
			else
			{
				if (MetaDataWrapper.GetMetaClassByName(className).Attributes.ContainsKey("HasCompositePage"))
					return GetLinkEntityView(realClassName, objectId);
				else
					return GetLinkEntityEdit(realClassName, objectId);
			}
		}
		#endregion

		#region GetLinkEntityList
		public static string GetLinkEntityList(string className)
		{
			MetaClass mc = MetaDataWrapper.GetMetaClassByName(className);
			string realClassName = (mc.CardOwner != null) ? mc.CardOwner.Name : mc.Name;
			return String.Format(CultureInfo.InvariantCulture, "~/Apps/MetaUIEntity/Pages/EntityList.aspx?ClassName={0}", realClassName);
		}
		#endregion

		#region UpdatePanelUpdate
		public static bool UpdatePanelUpdate(Page _page, string upName)
		{
			UpdatePanel up = null;
			up = GetUpdatePanelFromCollection(_page.Controls, upName);
			if (up != null)
			{
				up.Update();
				return true;
			}
			return false;
		}

		private static UpdatePanel GetUpdatePanelFromCollection(ControlCollection coll, string upName)
		{
			UpdatePanel retVal = null;
			foreach (Control c in coll)
			{
				if (c is UpdatePanel && c.ID == upName)
				{
					retVal = (UpdatePanel)c;
					break;

				}
				else
				{
					retVal = GetUpdatePanelFromCollection(c.Controls, upName);
					if (retVal != null)
						break;
				}
			}
			return retVal;
		}
		#endregion

		#region LoadExtJsGridScripts
		/// <summary>
		/// Loads the ext js grid scripts.
		/// </summary>
		/// <param name="p">The p.</param>
		public static void LoadExtJsGridScripts(Page page)
		{
			if (page == null)
				throw new ArgumentNullException("page");

			if (ScriptManager.GetCurrent(page) == null)
				throw new NullReferenceException("ScriptManager.GetCurrent(page) returned null.");

			if (McScriptLoader.Current == null)
				throw new NullReferenceException("McScriptLoader.Current returned null.");

			ScriptManager.GetCurrent(page).Scripts.Add(new ScriptReference(McScriptLoader.Current.GetScriptReferenceUrl("~/Scripts/IbnFramework/ext-all.js")));

			page.ClientScript.RegisterClientScriptBlock(page.GetType(), Guid.NewGuid().ToString(),
				String.Format("<link type='text/css' rel='stylesheet' href='{0}' />", page.ResolveClientUrl("~/styles/IbnFramework/ext-all2.css")));

		} 
		#endregion

		#region LoadExtJSGridScriptsToHead
		/// <summary>
		/// Loads the ext js grid scripts and styles into the head element.
		/// </summary>
		/// <param name="page">The page.</param>
		public static void LoadExtJSGridScriptsToHead(Page page)
		{
			UtilHelper.RegisterCssStyleSheet(page, "~/Styles/IbnFramework/ext-all2.css");
			UtilHelper.RegisterScript(page, "~/Scripts/IbnFramework/ext-all.js");
		}
		#endregion

		#region ParseText
		/// <summary>
		/// Parses the text.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns></returns>
		public static string ParseText(string text)
		{
			return ParseText(text, false, false, false);
		}

		/// <summary>
		/// Parses the text.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="preserveWhiteSpace">if set to <c>true</c> [preserve white space].</param>
		/// <param name="preserveLineBreaks">if set to <c>true</c> [preserve line breaks].</param>
		/// <param name="preserveHtmlTags">if set to <c>true</c> [preserve HTML tags].</param>
		/// <returns></returns>
		public static string ParseText(string text, bool preserveWhiteSpace, bool preserveLineBreaks, bool preserveHtmlTags)
		{
			StringBuilder sb = new StringBuilder(text);
			if (preserveWhiteSpace)
				sb.Replace("  ", " &nbsp;");
			if (!preserveHtmlTags)
			{
				sb.Replace("<", "&lt;");
				sb.Replace(">", "&gt;");
				sb.Replace("\"", "&quot;");
			}
			string resultString = sb.ToString();
			if (preserveLineBreaks)
			{
				StringReader sr = new StringReader(resultString);
				StringWriter sw = new StringWriter();
				while (sr.Peek() > -1)
				{
					string temp = sr.ReadLine();
					sw.Write(temp + "<br />");
				}
				resultString = sw.GetStringBuilder().ToString();
			}
			return resultString;
		}
		#endregion

		#region GetParentControl
		public static Control GetParentControl(Control startPoint, Type type, string clientId)
		{
			Control retVal = null;
			Control c = startPoint;
			do
			{
				c = c.Parent;
				if (c.GetType() == type && c.ClientID == clientId)
				{
					retVal = c;
					break;
				}
			} while (c != startPoint.Page);
			return retVal;
		}
		#endregion

		#region UpdateModalPopupContainer
		/// <summary>
		/// Updates the modal popup container.
		/// </summary>
		/// <param name="startPoint">The start point control.</param>
		/// <param name="clientId">The client id of container.</param>
		//public static void UpdateModalPopupContainer(Control startPoint, string clientId)
		//{
		//    Control c = GetParentControl(startPoint, typeof(ModalPopupContainer), clientId);
		//    if (c != null)
		//        ((ModalPopupContainer)c).Update();
		//}
		#endregion 

		#region GetMetaFieldName
		public static string GetMetaFieldName(MetaField field)
		{
			string name = GetResFileString(field.FriendlyName);
			if (field.IsReference)
			{
				name = String.Format(CultureInfo.InvariantCulture,
					"{0} ({1})", name, HttpContext.GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Reference"));
			}
			return name;
		}
		#endregion

		#region GetMcDataTypeName
		public static string GetMcDataTypeName(McDataType mcDataType)
		{
			return GetMcDataTypeName(mcDataType.ToString());
		}

		public static string GetMcDataTypeName(string mcDataType)
		{
			string key = String.Format(CultureInfo.InvariantCulture, "McDataType_{0}", mcDataType);
			return HttpContext.GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", key).ToString();
		}
		#endregion

		#region NeedToDataBind
		public static bool NeedToDataBind()
		{
			bool retval = false;
			object needtodatabind = GetFromContext(NeedToDataBindKey);
			if (needtodatabind != null && needtodatabind.ToString() == "true")
			{
				retval = true;
			}
			return retval;
		}
		#endregion

		#region RequireDataBind
		public static void RequireDataBind()
		{
			AddToContext(NeedToDataBindKey, "true");
		}

		public static void RequireDataBind(bool value)
		{
			if (value)
			{
				AddToContext(NeedToDataBindKey, "true");
			}
			else if (NeedToDataBind())
			{
				RemoveFromContext(NeedToDataBindKey);
			}
		}
		#endregion

		#region NeedToBindGrid
		public static bool NeedToBindGrid()
		{
			bool retval = false;
			object needtobindgrid = GetFromContext(NeedToBindGridKey);
			if (needtobindgrid != null && (needtobindgrid.ToString() == "1" || needtobindgrid.ToString() == "true"))
			{
				retval = true;
			}
			return retval;
		}
		#endregion

		#region RequireBindGrid
		public static void RequireBindGrid()
		{
			AddToContext(NeedToBindGridKey, 1);
		}

		public static void RequireBindGrid(bool value)
		{
			if (value)
			{
				AddToContext(NeedToBindGridKey, 1);
			}
			else if (NeedToBindGrid())
			{
				RemoveFromContext(NeedToBindGridKey);
			}
		}
		#endregion


		#region SaveXML(DataTable dt, string filename)
		public static void ExportXml(string xml, string fileName)
		{
			if (String.IsNullOrEmpty(xml))
				return;

			HttpResponse Response = HttpContext.Current.Response;
			Response.Clear();
			Response.ContentType = "text/xml";
			Response.AddHeader("content-disposition", String.Format("attachment; filename=\"{0}\"", fileName));

			Response.Write(xml);
			Response.End();


		}
		#endregion

		#region GetFormName
		public static string GetFormName(string systemFormName)
		{
			string retval = systemFormName;
			switch (systemFormName)
			{
				case FormController.BaseFormType:
					retval = Resources.IbnFramework.MetaForm.Form_Edit;
					break;
				case FormController.CreateFormType:
					retval = Resources.IbnFramework.MetaForm.Form_Create;
					break;
				case FormController.GeneralViewFormType:
					retval = Resources.IbnFramework.MetaForm.Form_View;
					break;
				case FormController.ShortViewFormType:
					retval = Resources.IbnFramework.MetaForm.Form_ShortInfo;
					break;
				case FormController.PublicEditFormType:
					retval = Resources.IbnFramework.MetaForm.Form_PublicInfo;
					break;
				case FormController.GeneralViewHistoryFormType:
					retval = String.Format(CultureInfo.InvariantCulture, "{0} ({1})",
						Resources.IbnFramework.MetaForm.Form_View, Resources.IbnFramework.ListInfo.History);
					break;
				case FormController.CustomFormType:
					retval = Resources.IbnFramework.MetaForm.CustomForm;
					break;
				default:
					break;
			}

			return retval;
		}
		#endregion

		#region GetDateDiffInSeconds
		/// <summary>
		/// Gets the date diff in seconds.
		/// </summary>
		/// <param name="dt1">The date1.</param>
		/// <param name="dt2">The date2.</param>
		/// <returns></returns>
		public static int GetDateDiffInSeconds(DateTime dt1, DateTime dt2)
		{
			TimeSpan ts;
			if (dt1 > dt2)
				ts = dt1.Subtract(dt2);
			else
				ts = dt2.Subtract(dt1);
			
			return (int)ts.TotalSeconds;
		}
		#endregion

		#region GenerateErrorReport(Exception ex)
		public static string GenerateErrorReport(Exception ex)
		{
			HttpContext httpContext = HttpContext.Current;

			HttpRequest request = null;
			if (httpContext != null)
				request = HttpContext.Current.Request;

			string errorId = Guid.NewGuid().ToString().Substring(0, 6);
			string prefix = string.Empty;
			string postData = string.Empty;
			string referrer = string.Empty;
			string httpMethod = string.Empty;
			string rawUrl = string.Empty;
			string browser = string.Empty;
			string userHostAddress = string.Empty;

			if (request != null)
			{
				httpMethod = request.HttpMethod;
				rawUrl = request.RawUrl;
				prefix = request.Url.Host.Replace(".", "_");
				browser = request.Browser.Browser + " " + request.Browser.Version;
				userHostAddress = request.UserHostAddress;

				if (request.UrlReferrer != null)
					referrer = string.Concat(request.UrlReferrer.Host, request.UrlReferrer.PathAndQuery);

				try
				{
					if (request.Form.Keys.Count > 0)
					{
						postData = "<table border='0' cellpadding='0' cellspacing='0'>";
						foreach (string key in request.Form.Keys)
						{
							postData += "<tr><td>" + HttpUtility.HtmlEncode(key) + "</td>";
							string values = "";
							foreach (string val in request.Form.GetValues(key))
							{
								string _val = "";

								if (!string.IsNullOrEmpty(val))
								{
									if (key == "__VIEWSTATE")
										_val = val.Substring(0, 20) + "...";
									else
										if (val.Length > 100)
											_val = val.Substring(0, 100) + "...";
										else
											_val = val;
								}

								values += HttpUtility.HtmlEncode(_val) + "<br />";
							}

							postData += "<td>" + values + "</td></tr>";
						}
						postData += "</table>";
					}
				}
				catch
				{
				}
			}

			string login = "";
			string displayName = "";

			try
			{
				Mediachase.IBN.Business.UserLight usr = Mediachase.IBN.Business.Security.CurrentUser;
				if (usr != null)
				{
					login = usr.Login;
					displayName = usr.FirstName + " " + usr.LastName;
				}
			}
			catch
			{
			}

			string reportFolder;
			if (httpContext != null)
				reportFolder = httpContext.Server.MapPath("~/Admin/Log/Error");
			else
				reportFolder = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, @"Admin\Log\Error");

			string reportFileName = Path.Combine(reportFolder, prefix + "_" + errorId + ".html");

			using (TextWriter tw = File.CreateText(reportFileName))
			{
				tw.Write(
					"<?xml version='1.0' encoding='utf-8'?>" +
					"<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.1//EN' 'http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd'>" +
					"<html xmlns='http://www.w3.org/1999/xhtml'>" +

					"<head>" +
					"<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />" +
					"<title>Error report</title>" +
					"<style type='text/css'>" +
					"div.errorReport { font-family: Verdana, Arial, Helvetica, Sans-Serif; font-size: 10pt; }" +
					"table.topAlign tr td { vertical-align: top; }" +
					"td.system { color: #0000CC; font-weight: bold; }" +
					"td.query { color: #009900; font-weight: bold; }" +
					"td.error { color: #FF0000; font-weight: bold; }" +
					"</style>" +
					"</head>" +

					"<body>" +
					"<div class='errorReport'>" +
					"<table width='100%' border='0' cellspacing='0' cellpadding='3' class='topAlign'>" +

					// System information
					"<tr>" +
					"<td class='system'>Error ID:</td>" +
					"<td>" + HttpUtility.HtmlEncode(errorId) + "</td>" +
					"</tr>" +
					"<tr>" +
					"<td class='system'>Date/Time:</td>" +
					"<td>" + HttpUtility.HtmlEncode(DateTime.UtcNow.ToString()) + " (UTC)</td>" +
					"</tr>" +
					"<tr>" +
					"<td class='system'>"+ IbnConst.ProductFamilyShort + " Version:</td>" +
					"<td>" + HttpUtility.HtmlEncode(Mediachase.Ibn.IbnConst.FullVersion) + "</td>" +
					"</tr>" +

					"<tr>" +
					"<td class='system'>Database:</td>" +
					"<td>" + HttpUtility.HtmlEncode(Mediachase.IBN.Database.DbContext.Current.PortalDatabase) + "</td>" +
					"</tr>" +

					"<tr>" +
					"<td class='system'>Browser:</td>" +
					"<td>" + HttpUtility.HtmlEncode(browser) + "</td>" +
					"</tr>" +
					"<tr>" +
					"<td class='system'>OS Version:</td>" +
					"<td>" + HttpUtility.HtmlEncode(Environment.OSVersion.VersionString) + "</td>" +
					"</tr>" +
					"<tr>" +
					"<td class='system'>NET Framework Version:</td>" +
					"<td>" + HttpUtility.HtmlEncode(Environment.Version.ToString()) + "</td>" +
					"</tr>" +
					"<tr>" +
					"<td class='system'>User Login:</td>" +
					"<td>" + HttpUtility.HtmlEncode(login) + "</td>" +
					"</tr>" +
					"<tr>" +
					"<td class='system'>User Name:</td>" +
					"<td>" + HttpUtility.HtmlEncode(displayName) + "</td>" +
					"</tr>" +
					"<tr>" +
					"<td class='system'>User IP Address:</td>" +
					"<td>" + HttpUtility.HtmlEncode(userHostAddress) + "</td>" +
					"</tr>" +

					// Query information
					"<tr>" +
					"<td class='query'>Method:</td>" +
					"<td>" + HttpUtility.HtmlEncode(httpMethod) + "</td>" +
					"</tr>" +
					"<tr>" +
					"<td class='query'>Query:</td>" +
					"<td>" + HttpUtility.HtmlEncode(rawUrl) + "</td>" +
					"</tr>" +
					"<tr>" +
					"<td class='query'>Referrer:</td>" +
					"<td>" + HttpUtility.HtmlEncode(referrer) + "</td>" +
					"</tr>" +
					"<tr>" +
					"<td class='query'>Postback:</td>" +
					"<td>" + postData + "</td>" +
					"</tr>" +

					// Error information
					"<tr>" +
					"<td class='error'>Message:</td>" +
					"<td>" + HttpUtility.HtmlEncode(ex.Message) + "</td>" +
					"</tr>" +
					"<tr>" +
					"<td class='error'>Source:</td>" +
					"<td>" + HttpUtility.HtmlEncode(ex.Source) + "</td>" +
					"</tr>" +
					"<tr>" +
					"<td class='error'>Target Site:</td>" +
					"<td>" + HttpUtility.HtmlEncode(ex.TargetSite.ToString()) + "</td>" +
					"</tr>" +
					"<tr>" +
					"<td class='error'>Details:</td>" +
					"<td>" + HttpUtility.HtmlEncode(ex.ToString()).Replace("\r\n", "<br />") + "</td>" +
					"</tr>" +

					"</table></div></body></html>"
					);
			}

			return errorId;
		}
		#endregion

		#region GetEntityTitleHtml
		public static string GetEntityTitleHtml(string className, PrimaryKeyId key)
		{
			string retVal = String.Empty;
			MetaClass mc = MetaDataWrapper.GetMetaClassByName(className);
			EntityObject eo = BusinessManager.Load(className, key);
			if (eo != null && eo.Properties[mc.TitleFieldName] != null && eo[mc.TitleFieldName] != null)
				retVal = eo[mc.TitleFieldName].ToString();
			return String.Format("<div style='overflow:hidden;' title='{0}'>{0}</div>", retVal);
		}

		public static string GetEntityTitle(string className, PrimaryKeyId key)
		{
			string retVal = String.Empty;
			MetaClass mc = MetaDataWrapper.GetMetaClassByName(className);
			EntityObject eo = BusinessManager.Load(className, key);
			if (eo != null && eo.Properties[mc.TitleFieldName] != null && eo[mc.TitleFieldName] != null)
				retVal = eo[mc.TitleFieldName].ToString();
			return retVal;
		}
		#endregion

		#region GetSearchFilterElementByKeyword
		/// <summary>
		/// Gets the search filter element by keyword.
		/// </summary>
		/// <param name="keyword">The keyword.</param>
		/// <param name="className">Name of the class.</param>
		/// <returns></returns>
		public static FilterElement GetSearchFilterElementByKeyword(string keyword, string className)
		{
			keyword = Regex.Replace(keyword, @"(\[|%|_)", "[$0]", RegexOptions.IgnoreCase);

			StringBuilder sbQuery = new StringBuilder(255);

			MetaClass metaClass = Mediachase.Ibn.Core.MetaDataWrapper.GetMetaClassByName(className);

			InnerGetSearchFilterElementByKeyword(sbQuery, metaClass, string.Empty);

			return new FilterElement(sbQuery.ToString(), FilterElementType.Contains, keyword);
		}

		/// <summary>
		/// Inners the get search filter element by keyword.
		/// </summary>
		/// <param name="sbQuery">The sb query.</param>
		/// <param name="metaClass">The meta class.</param>
		/// <param name="metaFieldPrefix">The meta field prefix.</param>
		private static void InnerGetSearchFilterElementByKeyword(StringBuilder sbQuery, MetaClass metaClass, string metaFieldPrefix)
		{
			foreach (MetaField mf in metaClass.Fields)
			{
				if (mf.IsAggregation)
				{
					// TODO: Process
					InnerGetSearchFilterElementByKeyword(sbQuery, mf.AggregationToMetaClass, "aggr" + mf.Name + "_");
					continue;
				}

				if (sbQuery.Length > 0)
					sbQuery.Append(" + N' ' + ");

				sbQuery.AppendFormat("ISNULL(CAST([{0}] AS NVARCHAR(4000)),N'')", metaFieldPrefix + mf.Name);

				if (mf.GetMetaType().McDataType == McDataType.File)
				{
					sbQuery.AppendFormat(" + N' ' + ISNULL(CAST([{0}_FileName] AS NVARCHAR(4000)), N'') + ' ' + ISNULL(CAST([{0}_Length] AS NVARCHAR(4000)),N'')", metaFieldPrefix + mf.Name);
				}
				else if (mf.GetMetaType().McDataType == McDataType.DateTime)
				{
					sbQuery.Append(" + N' ' + ");
					sbQuery.AppendFormat("ISNULL(CONVERT(NVARCHAR(10), [{0}], 101),N'')", metaFieldPrefix + mf.Name);
					sbQuery.Append(" + N' ' + ");
					sbQuery.AppendFormat("ISNULL(CONVERT(NVARCHAR(10), [{0}], 104),N'')", metaFieldPrefix + mf.Name);
				}
			}
		}


		#endregion

		#region GetIconLink
		public static string GetIconLink(string link, string text, string iconLink, string tooltip)
		{
			return String.Format(CultureInfo.InvariantCulture,
				"<a href=\"{0}\" title=\"{1}\"><span><img alt=\"\" src=\"{2}\"/></span> <span>{3}</span>",
				HttpUtility.HtmlAttributeEncode(link),
				HttpUtility.HtmlAttributeEncode(tooltip),
				HttpUtility.HtmlAttributeEncode(iconLink),
				HttpUtility.HtmlEncode(text));
		}
		#endregion

		#region GetIconText
		public static string GetIconText(string text, string iconLink)
		{
			return String.Format(CultureInfo.InvariantCulture,
				"<span><img alt=\"\" src=\"{0}\"/></span> <span>{1}</span>",
				HttpUtility.HtmlAttributeEncode(iconLink),
				HttpUtility.HtmlEncode(text));
		}
		#endregion

		#region GetIcon
		public static string GetIcon(string iconLink)
		{
			return GetIcon(iconLink, string.Empty);
		}
		public static string GetIcon(string iconLink, string title)
		{
			return String.Format(CultureInfo.InvariantCulture,
				"<img alt=\"\" src=\"{0}\" title=\"{1}\"/>",
				HttpUtility.HtmlAttributeEncode(iconLink),
				HttpUtility.HtmlAttributeEncode(title));
		}
		#endregion

		#region GetHistorySystemListViewProfile
		public static string GetHistorySystemListViewProfile(string historyClassName, string placeName)
		{
			ListViewProfile[] mas = ListViewProfile.GetSystemProfiles(historyClassName, placeName);
			if (mas.Length == 0)
			{
				ListViewProfile profile = new ListViewProfile();
				profile.Id = Guid.NewGuid().ToString();
				profile.IsPublic = true;
				profile.IsSystem = true;
				MetaClass mc = Mediachase.Ibn.Core.MetaDataWrapper.GetMetaClassByName(historyClassName);
				profile.Name = CHelper.GetResFileString(mc.PluralName);
				profile.FieldSetName = mc.Name;


				MetaClass mc2 = Mediachase.Ibn.Core.MetaDataWrapper.GetMetaClassByName(HistoryManager.GetOriginalMetaClassName(historyClassName));
				HistoryMetaClassInfo historyInfo = HistoryManager.GetInfo(mc2);
				Collection<string> selectedFields = historyInfo.SelectedFields;

				profile.FieldSet = new List<string>();
				profile.ColumnsUI = new ColumnPropertiesCollection();
				profile.FieldSet.Add(ChangeDetectionService.ModifiedFieldName);
				profile.FieldSet.Add(mc2.TitleFieldName);
				profile.ColumnsUI.Add(new ColumnProperties(ChangeDetectionService.ModifiedFieldName, "200px", String.Empty));
				profile.ColumnsUI.Add(new ColumnProperties(mc2.TitleFieldName, "200px", String.Empty));
				foreach (string fieldName in selectedFields)
				{
					if (fieldName == mc2.TitleFieldName || fieldName == ChangeDetectionService.ModifiedFieldName)
						continue;
					profile.FieldSet.Add(fieldName);
					profile.ColumnsUI.Add(new ColumnProperties(fieldName, "200px", String.Empty));
				}
				profile.GroupByFieldName = String.Empty;
				FilterElementCollection fec = new FilterElementCollection();
				FilterElement fe = new FilterElement();
				fe.ValueIsTemplate = true;
				fe.Source = "ObjectId";
				fe.Value = "{QueryString:ObjectId}";
				fec.Add(fe);
				profile.Filters = fec;
				profile.Sorting = new SortingElementCollection();

				ListViewProfile.SaveSystemProfile(historyClassName, placeName, Mediachase.IBN.Business.Security.CurrentUser.UserID, profile);
				mas = ListViewProfile.GetSystemProfiles(historyClassName, placeName);
				if (mas.Length == 0)
					throw new Exception("ListViewProfiles are not exist!");
			}
			return mas[0].Id;
		} 
		#endregion

		#region CheckMetaClassIsPublic
		public static bool CheckMetaClassIsPublic(MetaClass mc)
		{
			if (mc.Name == OrganizationEntity.GetAssignedMetaClassName()
				|| mc.Name == ContactEntity.GetAssignedMetaClassName())
				return true;
			return false;
		}

		public static bool CheckMetaClassIsPublic(string metaClassName)
		{
			return CheckMetaClassIsPublic(DataContext.Current.GetMetaClass(metaClassName));
		}
		#endregion

		#region GetMetaClassAdminPageLink
		public static string GetMetaClassAdminPageLink(string className, Page page)
		{
			return GetMetaClassAdminPageLink(MetaDataWrapper.GetMetaClassByName(className), page);
		}

		public static string GetMetaClassAdminPageLink(MetaClass mc, Page page)
		{
			string friendlyName = GetResFileString(mc.FriendlyName);
			string retval = friendlyName;
			if (ListManager.MetaClassIsList(mc))	// list
			{
				if (Mediachase.IBN.Business.ListInfoBus.CanAdmin(ListManager.GetListIdByClassName(mc.Name)))
				{
					retval = String.Format(CultureInfo.InvariantCulture,
						"<a href='{0}?class={1}'>{2}</a>",
						page.ResolveClientUrl(ListAdminPage),
						mc.Name,
						friendlyName);
				}
			}
			else // not list
			{
				if (CheckMetaClassIsPublic(mc))
				{
					retval = String.Format(CultureInfo.InvariantCulture,
						"<a href='{0}?class={1}'>{2}</a>",
						page.ResolveClientUrl(MetaClassAdminPage),
						mc.Name,
						friendlyName);
				}
			}
			return retval;
		}
		#endregion

		#region GetLinkObjectViewByOwnerName
		public static string GetLinkObjectViewByOwnerName(string ownerName, string ownerId)
		{
			ownerName = ownerName.ToLowerInvariant();

			string retval = string.Empty;
			switch (ownerName)
			{
				case "ownerdocumentid":
					retval = String.Format(CultureInfo.InvariantCulture, "~/Documents/DocumentView.aspx?DocumentId={0}&Tab=Workflow", ownerId);
					break;
				case "ownertaskid":
					retval = String.Format(CultureInfo.InvariantCulture, "~/Tasks/TaskView.aspx?TaskId={0}&Tab=Workflow", ownerId);
					break;
				case "ownertodoid":
					retval = String.Format(CultureInfo.InvariantCulture, "~/ToDo/ToDoView.aspx?ToDoId={0}&Tab=Workflow", ownerId);
					break;
				case "ownerincidentid":
					retval = String.Format(CultureInfo.InvariantCulture, "~/Incidents/IncidentView.aspx?IncidentId={0}&Tab=Workflow", ownerId);
					break;
				case "ownereventid":
					retval = String.Format(CultureInfo.InvariantCulture, "~/Events/EventView.aspx?EventId={0}&Tab=Workflow", ownerId);
					break;
				case "ownerprojectid":
					retval = String.Format(CultureInfo.InvariantCulture, "~/Projects/ProjectView.aspx?ProjectId={0}&Tab=Workflow", ownerId);
					break;
				default:
					break;
			}
			return retval;
		}
		#endregion

		#region GetAssignmentLink
		public static string GetAssignmentLink(string assignmentId, int ownerTypeId, int ownerId, Page page)
		{
			string retVal = string.Empty;

			switch (ownerTypeId)
			{
				case (int)Bus.ObjectTypes.Document:
					retVal = String.Format(
						CultureInfo.InvariantCulture,
						"{0}?DocumentId={1}&assignmentId={2}&Tab=General",
						page.ResolveClientUrl("~/Documents/DocumentView.aspx"),
						ownerId,
						assignmentId);
					break;
				case (int)Bus.ObjectTypes.Issue:
					retVal = String.Format(
						CultureInfo.InvariantCulture,
						"{0}?IncidentId={1}&assignmentId={2}",
						page.ResolveClientUrl("~/Incidents/IncidentView.aspx"),
						ownerId,
						assignmentId);
					break;
				case (int)Bus.ObjectTypes.Task:
					retVal = String.Format(
						CultureInfo.InvariantCulture,
						"{0}?TaskId={1}&assignmentId={2}",
						page.ResolveClientUrl("~/Tasks/TaskView.aspx"),
						ownerId,
						assignmentId);
					break;
				case (int)Bus.ObjectTypes.ToDo:
					retVal = String.Format(
						CultureInfo.InvariantCulture,
						"{0}?ToDoId={1}&assignmentId={2}",
						page.ResolveClientUrl("~/ToDo/ToDoView.aspx"),
						ownerId,
						assignmentId);
					break;
			}
			return retVal;
		}

		public static string GetAssignmentLink(string assignmentId, int ownerTypeId, int ownerId)
		{
			string retVal = string.Empty;

			switch (ownerTypeId)
			{
				case (int)Bus.ObjectTypes.Document:
					retVal = String.Format(
						CultureInfo.InvariantCulture,
						"~/Documents/DocumentView.aspx?DocumentId={0}&assignmentId={1}&Tab=General",
						ownerId,
						assignmentId);
					break;
				case (int)Bus.ObjectTypes.Issue:
					retVal = String.Format(
						CultureInfo.InvariantCulture,
						"~/Incidents/IncidentView.aspx?IncidentId={0}&assignmentId={1}",
						ownerId,
						assignmentId);
					break;
				case (int)Bus.ObjectTypes.Task:
					retVal = String.Format(
						CultureInfo.InvariantCulture,
						"~/Tasks/TaskView.aspx?TaskId={0}&assignmentId={1}",
						ownerId,
						assignmentId);
					break;
				case (int)Bus.ObjectTypes.ToDo:
					retVal = String.Format(
						CultureInfo.InvariantCulture,
						"~/ToDo/ToDoView.aspx?ToDoId={0}&assignmentId={1}",
						ownerId,
						assignmentId);
					break;
			}
			return retVal;
		} 
		#endregion

		#region GetAssignmentLinkWithIcon
		public static string GetAssignmentLinkWithIcon(string assignmentId, string assignmentName, int ownerTypeId, int ownerId, int stateId, bool isOverdue, Page page)
		{
			string link = GetAssignmentLink(assignmentId, ownerTypeId, ownerId, page);

			string icon = Mediachase.UI.Web.Util.CommonHelper.GetObjectIcon(ownerTypeId, ownerId, stateId, isOverdue, page);

			return string.Format("<a href=\"{0}\">{1} {2}</a>", link, icon, assignmentName);
		}

		public static string GetAssignmentLinkWithIcon(string assignmentId, string assignmentName, int ownerTypeId, int ownerId, string ownerName, int stateId, bool isOverdue, Page page)
		{
			string link = GetAssignmentLink(assignmentId, ownerTypeId, ownerId, page);

			string icon = Mediachase.UI.Web.Util.CommonHelper.GetObjectIcon(ownerTypeId, ownerId, stateId, isOverdue, page);

			return string.Format("<a href=\"{0}\">{1} {2}: {3}</a>", link, icon, ownerName, assignmentName);
		}
		#endregion

		#region GetActiveAssignmentLink
		public static string GetActiveAssignmentLink(string assignmentId, int ownerTypeId, int ownerId, Page page)
		{
			string retVal = string.Empty;

			switch (ownerTypeId)
			{
				case (int)Bus.ObjectTypes.Document:
					retVal = String.Format(
						CultureInfo.InvariantCulture,
						"{0}?DocumentId={1}&activeAssignmentId={2}&Tab=General",
						page.ResolveClientUrl("~/Documents/DocumentView.aspx"),
						ownerId,
						assignmentId);
					break;
				case (int)Bus.ObjectTypes.Issue:
					retVal = String.Format(
						CultureInfo.InvariantCulture,
						"{0}?IncidentId={1}&activeAssignmentId={2}",
						page.ResolveClientUrl("~/Incidents/IncidentView.aspx"),
						ownerId,
						assignmentId);
					break;
				case (int)Bus.ObjectTypes.Task:
					retVal = String.Format(
						CultureInfo.InvariantCulture,
						"{0}?TaskId={1}&activeAssignmentId={2}",
						page.ResolveClientUrl("~/Tasks/TaskView.aspx"),
						ownerId,
						assignmentId);
					break;
				case (int)Bus.ObjectTypes.ToDo:
					retVal = String.Format(
						CultureInfo.InvariantCulture,
						"{0}?ToDoId={1}&activeAssignmentId={2}",
						page.ResolveClientUrl("~/ToDo/ToDoView.aspx"),
						ownerId,
						assignmentId);
					break;
			}
			return retVal;
		}
		#endregion

		#region GetActiveAssignmentLinkWithIcon
		public static string GetActiveAssignmentLinkWithIcon(string assignmentId, string assignmentName, int ownerTypeId, int ownerId, int stateId, bool isOverdue, Page page)
		{
			string link = GetActiveAssignmentLink(assignmentId, ownerTypeId, ownerId, page);

			string icon = Mediachase.UI.Web.Util.CommonHelper.GetObjectIcon(ownerTypeId, ownerId, stateId, isOverdue, page);

			return string.Format("<a href=\"{0}\">{1} {2}</a>", link, icon, assignmentName);
		}

		public static string GetActiveAssignmentLinkWithIcon(string assignmentId, string assignmentName, int ownerTypeId, int ownerId, string ownerName, int stateId, bool isOverdue, Page page)
		{
			string link = GetActiveAssignmentLink(assignmentId, ownerTypeId, ownerId, page);

			string icon = Mediachase.UI.Web.Util.CommonHelper.GetObjectIcon(ownerTypeId, ownerId, stateId, isOverdue, page);

			return string.Format("<a href=\"{0}\">{1} {2}: {3}</a>", link, icon, ownerName, assignmentName);
		}
		#endregion

		#region GetProjectNum
		/// <summary>
		/// Gets the project code or project id.
		/// </summary>
		/// <param name="projectId">The project id.</param>
		/// <param name="projectCode">The project code.</param>
		/// <returns></returns>
		public static string GetProjectNum(int projectId, string projectCode)
		{
			string retval = projectId.ToString();
			if (Bus.PortalConfig.ShowProjectCode && !String.IsNullOrEmpty((string)projectCode))
				retval = (string)projectCode;

			return retval;
		} 
		#endregion

		#region GetProjectNumPostfix
		/// <summary>
		/// Gets the project code or project id as postfix (#xxxx).
		/// </summary>
		/// <param name="projectId">The project id.</param>
		/// <param name="projectCode">The project code.</param>
		/// <returns></returns>
		public static string GetProjectNumPostfix(int projectId, string projectCode)
		{
			return String.Concat(" (#", GetProjectNum(projectId, projectCode), ")");
		}
		#endregion

		#region UpperFirstChar
		public static string UpperFirstChar(string str)
		{
			string retval = str;
			if (str.Length > 1)
				retval = String.Concat(str.Substring(0, 1).ToUpper(), str.Substring(1));
			return retval;
		}
		#endregion

		#region PrintControl
		public static void PrintControl(Control ctrl, string title, Page page)
		{
			System.IO.StringWriter stringWrite = new System.IO.StringWriter();
			System.Web.UI.HtmlTextWriter htmlWrite = new System.Web.UI.HtmlTextWriter(stringWrite);
			ctrl.RenderControl(htmlWrite);
			htmlWrite.Flush();

			title = title.Replace("\"", "\\\"");

			string content =
				"var winprops = 'scrollbars=1,resizable=1,status=1';"
				+ "var w = window.open('blank.html', '_blank', winprops);"
				+ "w.document.write(\"<html><head><title>" + title + "</title><style type='text/css'>body, table {font-family: Verdana; font-size:8pt;} @media print {input {display:none;}}</style></head><body>\");"
				+ "w.document.write(\"<table width='100%' cellpadding='0' cellspacing='0'><tr><td><h3>" + title + "</h3></td><td align='right'><input type='button' value='" + HttpContext.GetGlobalResourceObject("IbnFramework.Global", "_mc_Print") + "' onclick='window.print()'  /></td></tr></table>\");"
				+ "w.document.write(\"" + stringWrite.ToString().Replace("\"", "\\\"").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ") + "\");"
				+ "w.document.write(\"</body></html>\");"
				+ "w.document.close();";

			Mediachase.Ibn.Web.UI.WebControls.ClientScript.RegisterStartupScript(
				page,
				page.GetType(),
				"PrintVersion",
				content,
				true);
		}
		#endregion
	}
}
