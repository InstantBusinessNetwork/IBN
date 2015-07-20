<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.ReportHeader" Codebehind="ReportHeader.ascx.cs" %>
<table cellspacing="0" cellpadding="0" width="100%" border="0" id="MainTable" runat="server">
	<tr>
		<td class="ibn-titleareaframe" colSpan="3">
			<div class="ibn-titleareaframe" style="border-bottom: 1px solid #ffd275;">
				<table  cellspacing="0" cellpadding="0" width="100%" border="0">
					<tr>
						<td style="PADDING-BOTTOM: 0px">
							<table style="PADDING-LEFT: 2px; PADDING-TOP: 0px" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td style="padding-top: 4px" valign="middle"><img alt="" src="<%=Mediachase.UI.Web.Util.CommonHelper.GetCompanyLogoUrl(Page)%>" /></td>
									<td width="12"><asp:Image ID="Image1" runat="server" Height="1" Width="7" ImageAlign="AbsMiddle" ImageUrl="~/layouts/images/blank.gif" /></td>
									<td style="PADDING-TOP: 2px" noWrap>
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td class="ibn-pagetitle" noWrap>
													<asp:label id="ReportName" runat="server"></asp:label></td>
											</tr>
											<tr>
												<td id="onetidPageTitle">
													<asp:label id="lblFilter" CssClass="text" runat="server"></asp:label></td>
											</tr>
										</table>
									</td>
									<td style="PADDING-RIGHT: 20px; PADDING-BOTTOM: 3px" vAlign="bottom" align="right">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr height="10">
												<td></td>
											</tr>
											<tr>
												<td noWrap>
													<asp:label id="lblDateReport" cssclass="ibn-descriptiontext" runat="server"></asp:label></td>
											</tr>
											<tr>
												<td id="onetidUserName">
													<asp:label id="lblUser" runat="server" cssclass="ibn-descriptiontext"></asp:label>
													<br>
													<br>
												</td>
											</tr>
											<tr>
												<td align="right" valign="bottom">
													<div id="divPrint" runat="server" Printable="0">
														<table cellpadding="0" cellspacing="0" border="0">
															<tr>
																<td>
																	<input type="button" class="text" style="WIDTH:80px" runat="server" id="btnPrint" onclick="window.print();">
																</td>
															</tr>
														</table>
													</div>
												</td>
											</tr>
										</table>
									</td>
								</tr>
							</table>
							<table cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td colSpan="5" height="2"><asp:Image ID="Image2" runat="server" Height="1" Width="1" ImageAlign="AbsMiddle" ImageUrl="~/layouts/images/blank.gif" /></td>
								</tr>
								<tr>
									<td class="ibn-titlearealine" colSpan="5" height="1"><asp:Image ID="Image3" runat="server" Height="1" Width="1" ImageAlign="AbsMiddle" ImageUrl="~/layouts/images/blank.gif" /></td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</div>
		</td>
	</tr>
</table>
