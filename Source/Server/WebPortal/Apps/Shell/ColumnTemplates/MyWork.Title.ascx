<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.UI.Web.Apps.Shell.ColumnTemplates.MyWork.Title" %>
<script language="c#" runat="server">
	protected string GetValue(object assignmentId, object assignmentName, object objectTypeId, object objectId, object objectName, object stateId, object isOverdue, object isNewMessage)
	{
		string retVal = string.Empty;
		if (objectId != DBNull.Value)
		{
			if (assignmentId == DBNull.Value || assignmentId == string.Empty)
			{
				retVal = Mediachase.UI.Web.Util.CommonHelper.GetObjectLinkWithIcon(
					(int)objectTypeId,
					(int)objectId,
					(string)objectName,
					(int)stateId,
					(bool)isOverdue,
					this.Page);
			}
			else
			{
				retVal = Mediachase.Ibn.Web.UI.CHelper.GetAssignmentLinkWithIcon(
					(string)assignmentId,
					(string)assignmentName,
					(int)objectTypeId,
					(int)objectId,
					(string)objectName,
					(int)stateId,
					(bool)isOverdue,
					this.Page);
			}

			if ((bool)isNewMessage)
				retVal = string.Concat("<b>", retVal, "</b>");
		}
		return retVal;
	}
</script>
<%# GetValue(Eval("AssignmentId"), Eval("AssignmentName"), Eval("ObjectTypeId"), Eval("ObjectId"), Eval("ObjectName"), Eval("StateId"), Eval("IsOverdue"), Eval("IsNewMessage")) %>