<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.EMailMCDefaultMapping" Codebehind="EMailMCDefaultMapping.ascx.cs" %>
<%@ Reference Control="~/Modules/ObjectDropDownNew.ascx" %>
<%@ Reference Control="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<%@ Register TagPrefix="mc" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="ObjectDD" src="~/Modules/ObjectDropDownNew.ascx" %>
<%@ Register TagPrefix="ibn" TagName="EntityDD" src="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<table cellpadding="7" cellspacing="0" width="100%" border="0">
	<colgroup>
		<col width="110px"/>
		<col width="310px"/>
		<col />
	</colgroup>
	<tr>
		<td width="100px"><b><%=LocRM.GetString("tTitle")%>:</b></td>
		<td width="310px"><asp:DropDownList ID="ddTitle" Runat="server" Width="300px"></asp:DropDownList></td>
		<td>&nbsp;</td>
	</tr>
	<tr>
		<td><b><%=LocRM.GetString("tDescription")%>:</b></td>
		<td><asp:DropDownList ID="ddDescription" Runat="server" Width="300px"></asp:DropDownList></td>
		<td>&nbsp;</td>
	</tr>	
	<tr>
		<td><b><%=LocRM.GetString("tCreator")%>:</b></td>
		<td><asp:DropDownList ID="ddCreator" Runat="server" Width="300px"></asp:DropDownList></td>
		<td>&nbsp;</td>
	</tr>
	<tr id="trProject" runat="server">
		<td><b><%=LocRM.GetString("tProject")%>:</b></td>
		<td><ibn:ObjectDD ID="ucProject" ObjectTypes="3" runat="server" Width="300px" ItemCount="6" ClassName="Incident" ViewName="" PlaceName="IncidentView" CommandName="MC_HDM_PM_ObjectDD" /></td>
		<td>&nbsp;</td>
	</tr>
	<tr id="trClient" runat="server">
		<td><b><%=LocRM.GetString("tClient")%>:</b></td>
		<td><ibn:EntityDD id="ClientControl" ObjectTypes="Contact,Organization" runat="server" Width="300px" ItemCount="6"  ClassName="Project" ViewName="" PlaceName="ProjectList" CommandName="MC_Client_EntityDD"/></td>
		<td>&nbsp;</td>
	</tr>
	<tr>
		<td><b><%=LocRM.GetString("tIssBox")%>:</b></td>
		<td><asp:DropDownList ID="ddIssBox" Runat="server" Width="300px"></asp:DropDownList></td>
		<td>&nbsp;</td>
	</tr>
	<tr id="trPriority" runat="server">
		<td><b><%=LocRM.GetString("tPriority")%>:</b></td>
		<td><asp:DropDownList ID="ddPriority" Runat="server" Width="300px"></asp:DropDownList></td>
		<td>&nbsp;</td>
	</tr>
	<tr id="trType" runat="server">
		<td><b><%=LocRM.GetString("tType")%>:</b></td>
		<td><asp:DropDownList ID="ddType" Runat="server" Width="300px"></asp:DropDownList></td>
		<td>&nbsp;</td>
	</tr>
	<tr id="trSeverity" runat="server">
		<td><b><%=LocRM.GetString("tSeverity")%>:</b></td>
		<td><asp:DropDownList ID="ddSeverity" Runat="server" Width="300px"></asp:DropDownList></td>
		<td>&nbsp;</td>
	</tr>
	<tr id="trCategories" runat="server">
		<td valign="top"><b><%=LocRM.GetString("tGenCategories")%>:</b></td>
		<td valign="top"><asp:ListBox ID="lbGenCats" Runat="server" SelectionMode="Multiple" Rows="4" Width="300px"></asp:ListBox></td>
		<td>&nbsp;</td>
	</tr>
	<tr id="trIssCategories" runat="server">
		<td valign="top"><b><%=LocRM.GetString("tIssCategories")%>:</b></td>
		<td valign="top"><asp:ListBox ID="lbIssCats" Runat="server" SelectionMode="Multiple" Rows="4" Width="300px"></asp:ListBox></td>
		<td>&nbsp;</td>
	</tr>
	<tr height="50px" valign="bottom">
		<td align="right" colspan="3">
			<mc:ImButton runat="server" class="text" ID="imbSave" style="width:110px"></mc:ImButton>&nbsp;
			<mc:ImButton runat="server" class="text" ID="imbCancel" onclick="window.close();" style="width:110px" CausesValidation="false"></mc:ImButton>
		</td>
	</tr>
</table>