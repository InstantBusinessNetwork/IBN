<%@ Control Language="c#" Inherits="Mediachase.UI.Web.UserReports.GlobalModules.ReportHeader" CodeBehind="ReportHeader.ascx.cs" %>
<table cellspacing="0" cellpadding="0" width="100%" border="0" id="MainTable" runat="server">
	<tr>
		<td class="ibn-titleareaframe" colspan="3">
			<div class="ibn-titleareaframe" style="border-bottom: #ffd275 1px solid">
				<table cellspacing="0" cellpadding="0" width="100%" border="0">
					<tr>
						<td style="padding-bottom: 0px">
							<table style="padding-left: 2px; padding-top: 0px" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td style="padding-top: 4px; white-space:nowrap" align="center" width="132px" height="50px">
										<img alt="" height="50" src="../Common/CompanyLogo.aspx" width="100" />
									</td>
									<td width="12">
										<img alt="" src="../layouts/images/blank.gif" width="7" height="1" />
									</td>
									<td style="padding-top: 2px; white-space:nowrap">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td class="ibn-pagetitle" style="white-space:nowrap">
													<asp:Label ID="ReportName" runat="server"></asp:Label>
												</td>
											</tr>
											<tr>
												<td class="ibn-titlearea" id="onetidPageTitle">
													<asp:Label ID="lblFilter" CssClass="text" runat="server"></asp:Label>
												</td>
											</tr>
										</table>
									</td>
									<td style="padding-right: 5px; padding-bottom: 3px" valign="bottom" align="right">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr style="height: 10px">
												<td>
												</td>
											</tr>
											<tr>
												<td style="white-space:nowrap">
													<asp:Label ID="lblDateReport" CssClass="ibn-descriptiontext" runat="server"></asp:Label>
												</td>
											</tr>
											<tr>
												<td id="onetidUserName">
													<asp:Label ID="lblUser" runat="server" CssClass="ibn-descriptiontext"></asp:Label>
													<br/>
													<br/>
												</td>
											</tr>
											<tr>
												<td align="right" valign="bottom">
													<div id="divPrint" runat="server" printable="0">
														<table cellpadding="0" cellspacing="0" border="0">
															<tr>
																<td>
																	<input type="button" class="text" style="width: 80px" runat="server" id="btnPrint" onclick="window.print();" />
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
									<td colspan="5" height="2px">
										<img alt="" src="../layouts/images/blank.gif" width="1" height="1" />
									</td>
								</tr>
								<tr>
									<td class="ibn-titlearealine" colspan="5" height="1px">
										<img alt="" src="../layouts/images/blank.gif" width="1" height="1" />
									</td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</div>
		</td>
	</tr>
</table>
