<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ResourceUtilFilter.ascx.cs" Inherits="Mediachase.UI.Web.Projects.Modules.ResourceUtilFilter" %>
<%@ Reference Control="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="EntityDD" src="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<script type="text/javascript">
	function ChangeOT(obj)
	{
		document.forms[0].<%=cbCalEntries.ClientID%>.checked = obj.checked;
		var objIss = document.forms[0].<%=cbIssues.ClientID%>;
		if(objIss)
			objIss.checked = obj.checked;
		document.forms[0].<%=cbDocs.ClientID%>.checked = obj.checked;
		var objTasks = document.forms[0].<%=cbTasks.ClientID%>;
		if(objTasks)
			objTasks.checked = obj.checked;
		document.forms[0].<%=cbToDo.ClientID%>.checked = obj.checked;
	}
	
	function ChangeModify()
	{
		id=document.forms[0].<%=ddCreated.ClientID %>.value;
		
		objTblFrom = document.getElementById('<%=tableDateFrom.ClientID %>');
		objTblTo = document.getElementById('<%=tableDateTo.ClientID %>');
		
		if(id=="9")
		{
			objTblFrom.style.display = 'block';
			objTblTo.style.display = 'block';
		}
		else
		{
			objTblFrom.style.display = 'none';
			objTblTo.style.display = 'none';
		}
	}
</script>
<table class="ibn-propertysheet" cellspacing="0" cellpadding="5" width="100%" border="0">
	<tr>
		<td width="300px" valign="top">
			<table class="text">
				<tr height="30px" id="trGroup" runat="server">
					<td class="text" width="90px"><%=LocRM.GetString("tGroup")%>:&nbsp;</td>
					<td> 
						<mc:indenteddropdownlist id="ddGroups" AutoPostBack="True" runat="server" Width="200px"></mc:indenteddropdownlist>
					</td>
				</tr>
				<tr height="30px" id="trUser" runat="server">
					<td class="text"><%=LocRM.GetString("tUser")%>:</td>
					<td>
						<asp:DropDownList id="ddUser" runat="server" Width="200px"></asp:DropDownList>
					</td>
				</tr>
				<tr height="30px" id="trClient" runat="server">
					<td class="text"><%=LocRM.GetString("Client")%>:</td>
					<td>
						<ibn:EntityDD ClassName="Project" PlaceName="ProjectView" CommandName="MC_ResUtil_EntityDD" id="ClientControl" ObjectTypes="Contact,Organization" ItemCount="4" runat="server" Width="200px"/>
					</td>
				</tr>
				<tr height="30px" id="trProject" runat="server">
					<td class="text"><%=LocRM.GetString("tProject")%>:</td>
					<td>
						<asp:DropDownList id="ddPrjs" runat="server" Width="200px"></asp:DropDownList>
					</td>
				</tr>
				<tr height="30px" id="trManager" runat="server">
					<td class="text"><%=LocRM.GetString("tManager")%>:</td>
					<td>
						<asp:DropDownList id="ddManager" runat="server" Width="200px"></asp:DropDownList>
					</td>
				</tr>
				<tr runat="server" id="trCategory">
					<td class="text">
						<%=LocRM.GetString("Category")%>:
					</td>
					<td>
						<asp:DropDownList ID="ddCategory" runat="server" Width="200px">
						</asp:DropDownList>
					</td>
				</tr>
			</table>
		</td>
		<td width="180px" valign="top">
			<mc:BlockHeaderLight2 ID="secHeader" runat="server" HeaderCssClass="ibn-toolbar-light" />
			<table class="text ibn-stylebox-light" cellspacing="0" cellpadding="0" width="100%" style="padding:4px 0px 4px 4px;">
				<tr>
					<td><div><%=LocRM.GetString("tShowCompleted")%>:</div>
						<asp:DropDownList id="ddCompleted" runat="server" Width="160px"></asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td><div style="padding-top:7px;"><%=LocRM.GetString("tActive")%>:</div>
						<asp:DropDownList id="ddShowActive" runat="server" Width="160px"></asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td><div style="padding-top:7px;"><%=LocRM.GetString("tShowUpcoming")%>:</div>
						<asp:DropDownList id="ddUpcoming" runat="server" Width="160px"></asp:DropDownList>
					</td>
				</tr>
			</table>
		</td>
		<td valign="top" id="tdObjs" runat="server">
			<mc:BlockHeaderLight2 ID="secHeaderObj" runat="server" HeaderCssClass="ibn-toolbar-light" />
			<table class="text ibn-stylebox-light" cellspacing="0" width="100%" style="padding:4px 0px">
				<tr>
					<td style="padding-left:12px"><asp:CheckBox ID="cbCalEntries" Runat="server"></asp:CheckBox></td>
				</tr>
				<tr>
					<td style="padding-left:12px"><asp:CheckBox ID="cbIssues" Runat="server"></asp:CheckBox></td>
				</tr>
				<tr>
					<td style="padding-left:12px"><asp:CheckBox ID="cbDocs" Runat="server"></asp:CheckBox></td>
				</tr>
				<tr>
					<td style="padding-left:12px"><asp:CheckBox ID="cbTasks" Runat="server"></asp:CheckBox></td>
				</tr>
				<tr>
					<td style="padding-left:12px"><asp:CheckBox ID="cbToDo" Runat="server"></asp:CheckBox></td>
				</tr>
			</table>
		</td>
		<td>&nbsp;</td>
	</tr>
	<tr>
		<td valign="top" colspan="3">
			<table class="text">
				<tr>
					<td style="width:90px;">&nbsp;</td>
					<td>
						<asp:CheckBox runat="server" ID="ChildTodoCheckbox" />
					</td>
				</tr>
				<tr style="height:30px;">
					<td><%=LocRM.GetString("CreatedPeriod")%>:</td>
					<td>
						<asp:DropDownList runat="server" ID="ddCreated"></asp:DropDownList>
					</td>
					<td>
						<table id="tableDateFrom" cellspacing="2" cellpadding="0" runat="server">
							<tr>
								<td>&nbsp;<asp:label id="lblFrom" Font-Bold="True" Runat="server" CssClass="text" Visible="true"></asp:label>&nbsp;</td>
								<td><mc:Picker ID="dtcStartDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" /></td>
							</tr>
						</table>
					</td>
					<td>
						<table id="tableDateTo" cellspacing="2" cellpadding="0" runat="server">
							<tr>
								<td><asp:label id="lblTo" Font-Bold="True" Runat="server" CssClass="text"></asp:label></td>
								<td><mc:Picker ID="dtcEndDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" />
							</tr>
						</table>
					</td>
					<td class="text"><asp:customvalidator id="CustomValidator1" runat="server" Display="Dynamic" ErrorMessage="*"></asp:customvalidator></td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td valign="top" colspan="3" align="center">
			<nobr><asp:Button ID="btnApplyF" Runat="server" CssClass="text" Width="80px"></asp:Button>&nbsp;
			<asp:Button ID="btnResetF" Runat="server" CssClass="text" Width="80px"></asp:Button>&nbsp;
			<asp:Button ID="btnClose" runat="server" CssClass="text" Width="80px" /></nobr>
		</td>
	</tr>
</table>