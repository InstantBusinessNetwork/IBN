<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.UI.Web.Incidents.Modules.ArticleListMain" Codebehind="ArticleListMain.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Reference Control="~/Modules/TagCloud.ascx"  %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeaderLight" src="../../Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<%@ Register TagPrefix="ibn" TagName="TagCloud" Src="~/Modules/TagCloud.ascx" %>

<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox2">
	<tr>
		<td>
			<ibn:BlockHeader id="secHeader" runat="server" />
		</td>
	</tr>
	<tr>
		<td class="ibn-navline ibn-alternating text" style="padding:14px;">
			<asp:TextBox ID="txtSearch" runat="server" Width="150px" CssClass="text"></asp:TextBox>
			<asp:Button CssClass="text" ID="btnSearch" runat="server" OnClick="btnSearch_Click" />
		</td>
	</tr>
	<tr>
		<td valign="top">
			<table cellpadding="0" cellspacing="0" width="100%" border="0">
				<tr>
					<td style="padding: 2 0 7 7;" width="60%" valign=top>
						<ibn:BlockHeaderLight id="hdrList" runat="server" />
						<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="0" width="100%" border="0"><tr><td>
							<dg:datagridextended runat="server" ID="grdMain" AutoGenerateColumns="false" Width="100%" 
								CellPadding="0" CellSpacing="0" GridLines="None" AllowPaging="true" AllowSorting="true" 
								PageSize="10" PagerStyle-CssClass="ibn-vb2" ShowHeader="true" 
								OnPageSizeChanged="grdMain_PageSizeChanged" OnPageIndexChanged="grdMain_PageIndexChanged" 
								OnSortCommand="grdMain_SortCommand" OnDeleteCommand="grdMain_DeleteCommand">
								<Columns>
									<asp:boundcolumn visible="false" datafield="ArticleId"></asp:boundcolumn>
									<asp:TemplateColumn sortexpression="Question">
										<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
										<ItemStyle CssClass="ibn-vb2"></ItemStyle>
										<ItemTemplate>
											<a href="../Incidents/ArticleView.aspx?ArticleId=<%# Eval("ArticleId") %>"><img src="../Layouts/images/kb.gif" width="16px" height="16px" border="0" align="absmiddle" /> <%# Eval("Question") %></a>
										</ItemTemplate>
									</asp:TemplateColumn>
									<ASP:BOUNDCOLUMN DataField="Created" DataFormatString="{0:d}" sortexpression="Created">
										<HEADERSTYLE CssClass="ibn-vh2" Width="90px"></HEADERSTYLE>
										<ITEMSTYLE CssClass="ibn-vb2" Width="90px"></ITEMSTYLE>
									</ASP:BOUNDCOLUMN>
									<ASP:TEMPLATECOLUMN>
										<HEADERSTYLE CssClass="ibn-vh-right" Width="60px" HorizontalAlign="Right"></HEADERSTYLE>
										<ITEMSTYLE CssClass="ibn-vb2" Width="60px" HorizontalAlign="Right"></ITEMSTYLE>
										<ITEMTEMPLATE>
											<table cellspacing="0" cellpadding="0" width="100%" border="0">
												<tr>
													<td width="50%">
														<asp:HyperLink id="Hyperlink1" Runat="server" NavigateUrl='<%# "~/Incidents/ArticleEdit.aspx?ArticleId=" + DataBinder.Eval(Container.DataItem, "ArticleId").ToString() + "&back=list" %>' ImageUrl="../../layouts/images/edit.GIF" ToolTip='<%#LocRM2.GetString("tEdit")%>'>
														</asp:HyperLink></td>
													<td>
														<asp:imagebutton id="ibDelete" width="16" borderwidth="0" runat="server" causesvalidation="False" commandname="Delete" imageurl="../../layouts/images/DELETE.GIF"  height="16" title='<%#LocRM2.GetString("tDelete")%>'></asp:imagebutton>
													</td>
												</tr>
											</table>
										</ITEMTEMPLATE>
									</ASP:TEMPLATECOLUMN>
								</Columns>
							</dg:datagridextended>
							</td></tr></table>
					</td>
					<td style="padding: 2 7 7 7;" width="40%" valign=top>
						<ibn:BlockHeaderLight id="hdrTags" runat="server" />
						<table class="ibn-stylebox-light" cellspacing="0" width="100%" border="0"><tr><td style="padding: 0 7 7 7;">
						<ibn:TagCloud runat="server" id="ctrlTagCloud" OnTagClick="ctrlTagCloud_TagClick" ObjectType="20" TagCount="30" TagSizeCount="13"></ibn:TagCloud>
						</td></tr></table>
					</td>
				</tr>
			</table>
		</td>
  </tr>
</table>
<asp:Button runat="server" ID="btnReset" OnClick="btnReset_Click" style="display:none" />