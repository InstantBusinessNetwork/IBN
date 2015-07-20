<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.TimeTracking.Primitives.Text_Grid_TimeTrackingEntry_Title_TT_MyGroupByWeekProject" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" %>
<%@ Import Namespace="Mediachase.Ibn.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Services" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI.Controls.Util" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Import Namespace="Mediachase.IbnNext.TimeTracking" %>
<script language="c#" runat="server">
    protected string GetValue(MetaObject DataItem, string FieldName)
    {
		string retVal = "";
        int paddingValue = 0;
        
        if (DataItem != null && DataItem.Properties[FieldName].Value != null)
		{
			string metaClassName = DataItem.GetMetaType().Name;
			string titleText;
			
			if (DataItem.Properties[FieldName].Value != null)
				titleText = DataItem.Properties[FieldName].Value.ToString();
			else
				titleText = string.Empty;

			int id = DataItem.PrimaryKeyId ?? -1;
			//string sUrl = CHelper.GetAbsolutePath(String.Format("/IbnNext/TestFolder/ObjectView.aspx?ClassName={0}&ObjectId={1}", metaClassName, id));
			//string baseText = String.Format("<a href='{0}'>{1}</a>", sUrl, titleText);
			string baseText = titleText; 
			

			if (DataItem.PrimaryKeyId == null)
			{
				if (DataItem.Properties["MetaViewGroupByType"] != null && DataItem.Properties["MetaViewGroupByType"].OriginalValue != null)
				{
                    if (DataItem.Properties["MetaViewGroupByType"].OriginalValue.ToString() == MetaViewGroupByType.Primary.ToString())
					{
                        paddingValue = 5; // 25;
					}
				}
				
			}
			else
			{
				baseText = titleText;
			}				
			
			if (DataItem.PrimaryKeyId == null && DataItem.Properties["MetaViewGroupByType"] != null)
			{
				if (CHelper.GetFromContext("MetaViewName") == null)
					throw new ArgumentException("MetaViewName @ Context");

				MetaView mv = Mediachase.Ibn.Data.DataContext.Current.MetaModel.MetaViews[(string)CHelper.GetFromContext("MetaViewName")];
				if (mv == null)
					throw new ArgumentNullException(String.Format("MetaViewName: {0}", CHelper.GetFromContext("MetaViewName")));

				McMetaViewPreference mvPref = Mediachase.Ibn.Core.UserMetaViewPreference.Load(mv, Security.CurrentUserId);
				
				string imgPath = string.Empty;
				bool isPlus = false;
				if (DataItem.Properties["MetaViewGroupByType"].OriginalValue.ToString() == MetaViewGroupByType.Primary.ToString())
					isPlus = MetaViewGroupUtil.IsCollapsed(MetaViewGroupByType.Primary, mvPref, MetaViewGroupUtil.GetUniqueKey(DataItem));
				else if (DataItem.Properties["MetaViewGroupByType"].OriginalValue.ToString() == MetaViewGroupByType.Secondary.ToString())
					isPlus = MetaViewGroupUtil.IsCollapsed(MetaViewGroupByType.Secondary, mvPref, MetaViewGroupUtil.GetUniqueKey(DataItem));

				if (isPlus)
				{
					imgPath = CHelper.GetAbsolutePath("/Images/IbnFramework/plus9x9.gif");
				}
				else
				{
					imgPath = CHelper.GetAbsolutePath("/Images/IbnFramework/minus9x9.gif");
				}

				System.Collections.Generic.Dictionary<string, string> dic = new System.Collections.Generic.Dictionary<string, string>();
				dic.Add("primaryKeyId", MetaViewGroupUtil.GetUniqueKey(DataItem));
				dic.Add("groupType", DataItem.Properties["MetaViewGroupByType"].OriginalValue.ToString());
				string actionCollapseScript = CommandManager.GetCurrent(this.Page).AddCommand(metaClassName, "TT_MyGroupByWeekProject", string.Empty, new CommandParameters("MC_TimeSheet_CollapseExpandBlock", dic));
				actionCollapseScript = actionCollapseScript.Replace("\"", "&quot;");
				
				if (DataItem.Properties["MetaViewGroupByType"].OriginalValue.ToString() != MetaViewGroupByType.Total.ToString())
					retVal = String.Format("<span style='padding-left:0px'><a href='#' onclick=\"{4}; \"><img border='0' src='{0}' /></a>&nbsp;{2}</span> ", imgPath, MetaViewGroupUtil.GetUniqueKey(DataItem), baseText, paddingValue, actionCollapseScript);
				else
					retVal = baseText;
			}
			else
			{
				if (DataItem.Properties["ObjectId"].OriginalValue != null && DataItem.Properties["ObjectTypeId"].OriginalValue != null
				&& int.Parse(DataItem.Properties["ObjectId"].OriginalValue.ToString()) != 0 && int.Parse(DataItem.Properties["ObjectTypeId"].OriginalValue.ToString()) != 0)
				{
					baseText = String.Format("<a href='{0}'>{1}</a> ", CHelper.GetObjectLink(Convert.ToInt32(DataItem.Properties["ObjectTypeId"].OriginalValue), Convert.ToInt32(DataItem.Properties["ObjectId"].OriginalValue)), baseText);
				}
                paddingValue = 25; // 45;			
				retVal = "<span style='padding-left: 0px'>" + baseText + "</span>";
			}
		}
        return String.Format("<div style='padding-left: {1}px;'>{0}</div>", retVal, paddingValue + 5);
    }
</script>
<%# (DataItem == null) ? "null" : GetValue(DataItem, FieldName)%>