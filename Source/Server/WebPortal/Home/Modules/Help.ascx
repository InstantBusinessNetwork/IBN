<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Home.Modules.Help" CodeBehind="Help.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="..\..\Modules\BlockHeader.ascx" %>
<table class="ibn-stylebox2 text" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td>
			<ibn:BlockHeader ID="secHeader" runat="server"></ibn:BlockHeader>
		</td>
	</tr>
	<tr>
		<td>
			<table cellspacing="0" cellpadding="10" border="0" class="text" width="100%">
				<tr id="trQuickHelp1" runat="server">
					<td class="ibn-sectionheader" style="font-size: 1.2em;">
						<%=_quickHelp %>
					</td>
				</tr>
				<tr id="trDocumentation" runat="server">
					<td class="ibn-sectionheader" style="font-size: 1.2em;">
						<img alt="" src="../layouts/images/documentation1.gif" style="padding-right:10px;vertical-align:middle" /><asp:HyperLink
							Target="_blank" ID="hlDocument" runat="server"><%=LocRM.GetString("HelpTitle1") %></asp:HyperLink>
					</td>
				</tr>
				<tr id="trTutorial" runat="server">
					<td class="ibn-sectionheader" style="font-size: 1.2em;">
						<img alt="" src="../layouts/images/icon_course.gif" style="padding-right: 10px;vertical-align:middle" /><asp:HyperLink
							Target="_blank" ID="hlTextBook" runat="server"><%=LocRM.GetString("tTextBook")%></asp:HyperLink>
					</td>
				</tr>
				<tr id="trForum" runat="server">
					<td class="ibn-sectionheader" style="font-size: 1.2em;">
						<img alt="" src="../layouts/images/icon_forum.gif" style="padding-right: 10px;vertical-align:middle" /><asp:HyperLink
							Target="_blank" ID="hlForums" runat="server"><%=LocRM.GetString("HelpTitle2") %></asp:HyperLink>
					</td>
				</tr>
				<tr id="trSupport" runat="server">
					<td class="ibn-sectionheader" style="font-size: 1.2em;">
						<img alt="" src="../layouts/images/support1.gif" style="padding-right: 10px;vertical-align:middle" /><asp:HyperLink
							ID="hlSupport" runat="server"><%=LocRM.GetString("HelpTitle3") %></asp:HyperLink>
					</td>
				</tr>
				<tr>
					<td class="ibn-alternating text" style="background-color: #F0F8FF;">
						<asp:Label ID="lblForMe" runat="server"></asp:Label>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
