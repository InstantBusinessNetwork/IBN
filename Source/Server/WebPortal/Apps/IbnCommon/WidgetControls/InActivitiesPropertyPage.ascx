<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InActivitiesPropertyPage.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Shell.Modules.InActivitiesPropertyPage" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<script language="javascript" type="text/javascript">
function ShowHideCalendars()
{
	var rowFrom = document.getElementById('<%=rowFrom.ClientID%>');
	var rowTo = document.getElementById('<%=rowTo.ClientID%>');
	var listType = document.getElementById('<%=listType.ClientID%>');
	
	if (listType && rowFrom && rowTo)
	{
		if (listType.value == "0")
		{
			rowFrom.style.visibility = "visible";
			rowTo.style.visibility = "visible";
		}
		else
		{
			rowFrom.style.visibility = "hidden";
			rowTo.style.visibility = "hidden";
		}
	}
}
</script>
<table class="text" cellspacing="6" cellpadding="0" width="100%" border="0" style="margin-top:0">
	<tr>
		<td>
			<%= LocRM.GetString("Show")%>:
		</td>
		<td>
			<asp:RadioButton runat="server" ID="buttonActive" GroupName="Group1"/>
			<asp:RadioButton runat="server" ID="buttonAll" GroupName="Group1" />
		</td>
	</tr>
	<tr>
		<td>
			<%= LocRM.GetString("tPeriod")%>:
		</td>
		<td>
			<asp:DropDownList runat="server" ID="listType"></asp:DropDownList>
		</td>
	</tr>
	<tr runat="server" id="rowFrom">
		<td><%= LocRM.GetString("ShowFrom")%>:</td>
		<td>
			<mc:Picker ID="dateFrom" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" />
		</td>
	</tr>
	<tr runat="server" id="rowTo">
		<td><%= LocRM.GetString("ShowTo")%>:</td>
		<td>
			<mc:Picker ID="dateTo" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" />
		</td>
	</tr>
</table>