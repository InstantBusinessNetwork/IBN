<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Wizards.Modules.NewIssueWizard" Codebehind="NewIssueWizard.ascx.cs" %>
<asp:panel id="step1" Runat="server">
	<div class="text"><%=LocRM.GetString("s1TopDiv") %></div>
	<BR>
	<table cellspacing="0" cellpadding="0" width="100%" border="0">
		<tr>
			<td vAlign="top" width="50%">
				<div class="subHeader" style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 10px; PADDING-TOP: 3px"><%=LocRM.GetString("s1IssueTitle") %></div>
				<asp:TextBox id="txtTitle" Runat="server" Width="260"></asp:TextBox>
				<asp:RequiredFieldValidator id="s1rfTitle" Runat="server" ControlToValidate="txtTitle" ErrorMessage="*" Display="Dynamic"></asp:RequiredFieldValidator>
				<div class="subHeader" style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 10px; PADDING-TOP: 10px"><%=LocRM.GetString("s1Priority") %></div>
				<asp:DropDownList id="ddPriority" Runat="server" Width="260"></asp:DropDownList>
				<div class="subHeader" style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 10px; PADDING-TOP: 10px"><%=LocRM.GetString("s1Severity") %></div>
				<asp:DropDownList id="ddSeverity" Runat="server" Width="260"></asp:DropDownList>
				<div class="subHeader" style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 10px; PADDING-TOP: 10px"><%=LocRM.GetString("s1IssueType") %></div>
				<asp:DropDownList id="ddType" Runat="server" Width="260"></asp:DropDownList></td>
			<td vAlign="top" align="middle" width="60"><IMG alt="" src="../layouts/images/quicktip.gif" border="0">
			</td>
			<td class="text" style="PADDING-RIGHT: 15px" vAlign="top" height="100%"><%=LocRM.GetString("s1QuickTip") %><BR>
				<BR>
				<asp:TextBox id="txtDescr" Runat="server" Width="98%" Height="75%" TextMode="MultiLine"></asp:TextBox></td>
		</tr>
	</table>
</asp:panel>
<asp:panel id="step2" Runat="server">
	<div class="text"></div>
	<BR>
	<table cellspacing="0" cellpadding="0" width="100%" border="0">
		<tr>
			<td vAlign="top" width="50%">
				<div class="subHeader" style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 10px; PADDING-TOP: 3px"><%=LocRM.GetString("s2Project") %></div>
				<asp:DropDownList id="ddProject" Runat="server" Width="260"></asp:DropDownList>
				<div class="subHeader" style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 10px; PADDING-TOP: 3px"><%=LocRM.GetString("s2IssueCategory") %></div>
				<asp:ListBox id="lbIssueCategories" Width="260px" SelectionMode="Multiple" Rows="7" runat="server"></asp:ListBox></td>
			<td vAlign="top" align="middle" width="60"><IMG alt="" src="../layouts/images/quicktip.gif" border="0">
			</td>
			<td class="text" style="PADDING-RIGHT: 15px" vAlign="top"><%=LocRM.GetString("s2QuickTip") %></td>
		</tr>
	</table>
</asp:panel>
<asp:panel id="step3" Runat="server">
	<div class="text"><%=LocRM.GetString("s3TopDiv") %></div>
	<BR>
	<table cellspacing="0" cellpadding="0" width="100%" border="0">
		<tr>
			<td vAlign="top" width="50%">
				<table width="100%" cellpadding=0 cellspacing=10>
					<tr>
						<td><B><%=LocRM.GetString("s3Title") %>:</B></td>
						<td><asp:Label id="lblIssueTitle" Runat="server"></asp:Label></td></tr>
					<tr>
						<td><B><%=LocRM.GetString("s3ProjectTitle") %>:</B></td>
						<td><asp:Label id="lblProjectTitle" Runat="server"></asp:Label></td></tr>
					<tr>
						<td><B><%=LocRM.GetString("s3Priority") %>:</B></td>
						<td><asp:Label id="lblPriority" Runat="server"></asp:Label></td></tr>
					<tr>
						<td><B><%=LocRM.GetString("s3Severity") %>:</B></td>
						<td><asp:Label id="lblSeverity" Runat="server"></asp:Label></td></tr>
					<tr>
						<td><B><%=LocRM.GetString("s3Type") %>:</B></td>
						<td><asp:Label id="lblIssueType" Runat="server"></asp:Label></td></tr>
				</table>
			</td>
			<td vAlign="top" align="middle" width="60"><IMG alt="" src="../layouts/images/quicktip.gif" border="0">
			</td>
			<td class="text" style="PADDING-RIGHT: 15px" vAlign="top" height="100%"><%=LocRM.GetString("s3QuickTip") %>
			</td>
		</tr>
	</table>
</asp:panel>
<asp:panel id="step4" Runat="server">
	<div class="text">
		<IMG height="20" src="../layouts/images/help.gif" width="20" align="absMiddle">&nbsp;<%=LocRM.GetString("s4TopDiv") %>
	</div>
	<BR>
	<div class="SubHeader"><B><%=LocRM.GetString("s4TextHeader") %></B><BR>
		<BR>
	</div>
	<div class="SubHeader" style="PADDING-LEFT: 20px"><%=LocRM.GetString("s4Text") %></div>
</asp:panel>
