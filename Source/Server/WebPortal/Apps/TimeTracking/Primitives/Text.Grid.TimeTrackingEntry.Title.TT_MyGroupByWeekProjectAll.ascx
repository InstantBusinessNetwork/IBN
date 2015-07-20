<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.TimeTracking.Primitives.Text_Grid_TimeTrackingEntry_Title_TT_MyGroupByWeekProjectAll" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" %>
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
				if (DataItem.Properties["MetaViewGroupByType"].OriginalValue != null)
				{
                    if (DataItem.Properties["MetaViewGroupByType"].OriginalValue.ToString() == MetaViewGroupByType.Primary.ToString())
                    {
                        DateTime date = Convert.ToDateTime(DataItem.Properties["MetaViewGroupByKey"].OriginalValue);
                        baseText = String.Format("{0}-{1}", date.ToShortDateString(), date.AddDays(6).ToShortDateString());
                    }
                    else 
                    if (DataItem.Properties["MetaViewGroupByType"].OriginalValue.ToString() == MetaViewGroupByType.Secondary.ToString())
					{

                        
						//baseText = String.Format("<a href='{0}'>{1}</a>", sUrl, titleText);

						if (DataItem.Properties["ParentBlockId"].OriginalValue != null)
						{
                            MetaObject ttb = MetaObjectActivator.CreateInstance<BusinessObject>(MetaDataWrapper.ResolveMetaClassByNameOrCardName("TimeTrackingBlock"), Convert.ToInt32(DataItem.Properties["ParentBlockId"].OriginalValue.ToString()));
                            StateMachineService sms = ((BusinessObject)ttb).GetService<StateMachineService>();
                            StateTransition[] transitions = sms.GetNextAvailableTransitions(true);

							bool canWrite = TimeTrackingBlock.CheckUserRight((Mediachase.IbnNext.TimeTracking.TimeTrackingBlock)ttb, Security.RightWrite);
                            bool isInitialState = sms.StateMachine.GetStateIndex(sms.CurrentState.Name) == 0;

							System.Collections.Generic.Dictionary<string, string> paramDic = new System.Collections.Generic.Dictionary<string, string>();
							paramDic.Add("primaryKeyId", ttb.PrimaryKeyId.Value.ToString());
							paramDic.Add("groupType", DataItem.Properties["BlockTypeInstanceId"].OriginalValue.ToString());
							string actionScript = CommandManager.GetCurrent(this.Page).AddCommand(metaClassName, "TT_MyGroupByWeekProject", string.Empty, new CommandParameters("MC_TimeTracking_QuickAdd", paramDic));
							actionScript = actionScript.Replace("\"", "&quot;");

							//string actionScript = String.Format(" raiseMCActionHandler('MC_TimeTracking_QuickAdd', ['{0}', '{1}']);", DataItem.Properties["BlockTypeInstanceId"].OriginalValue, ttb.PrimaryKeyId);
                            
							string actionTransitionScript = string.Empty;

                            baseText += "<span style='position: absolute; right: 0px; top: 0px;'>";
                            
                            if (canWrite && isInitialState)
                            {
								//baseText += String.Format("<a class='ibn-tooltip' onclick=\"{1}\" href='#'><img src='{0}' onclick=\"{1}\" alt='{2}'></a>&nbsp;", CHelper.GetAbsolutePath("/Images/IbnFramework/newitem.gif"), actionScript, CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_Add}"));
                            }

                            if (transitions.Length > 0 && isInitialState)
                            {
                                actionTransitionScript = String.Format("raiseMCActionHandler('MC_TimeTracking_MakeTransition', ['{0}', '-{1}']);", transitions[0].Uid, ttb.PrimaryKeyId);
								baseText += String.Format("<a class='ibn-tooltip' onclick=\"{1}\" href='#'><img src='{0}' border=0  alt='{2}'/></a>", CHelper.GetAbsolutePath("/Images/TimeTracking/send3.gif"), actionTransitionScript, CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_Send}"));
                            }

                            baseText += "</span>";

                            if (ttb.Properties["StateFriendlyName"].Value != null)
                            {
                                baseText += " (" + CHelper.GetResFileString(ttb.Properties["StateFriendlyName"].Value.ToString()) + ")";
                            }

						}

                        paddingValue = 25; // 25;
					}
				}
				
			}
			else
			{
				baseText = titleText;
			}				
			
			if (DataItem.PrimaryKeyId == null)
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

				if (DataItem.Properties["MetaViewGroupByType"].OriginalValue.ToString() != MetaViewGroupByType.Total.ToString())
                    retVal = String.Format("<span style='padding-left:0px'><a href='#' onclick='raiseMCActionHandler(\"MC_TimeSheet_CollapseExpandBlock\", [\"{1}\", \"{4}\"])'><img border='0' src='{0}' /></a>&nbsp;{2}</span> ", imgPath, MetaViewGroupUtil.GetUniqueKey(DataItem), baseText, paddingValue, DataItem.Properties["MetaViewGroupByType"].OriginalValue.ToString());
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
                paddingValue = 45; // 45;			
				retVal = "<span style='padding-left: 0px'>" + baseText + "</span>";
			}
		}
        return String.Format("<div style='padding-left: {1}px;'>{0}</div>", retVal, paddingValue + 5);
    }
</script>
<%# (DataItem == null) ? "null" : GetValue(DataItem, FieldName)%>