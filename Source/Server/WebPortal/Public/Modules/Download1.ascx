<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Public.Modules.Download1" Codebehind="Download1.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<table width="100%" border="0" cellspacing="0" cellpadding="0" class="ibn-stylebox2 text">
	<tr>
		<td>
			<ibn:BlockHeader ID="secHeader" runat="server" Title=""></ibn:BlockHeader>
		</td>
	</tr>
	<tr>
		<td>
			<table cellpadding="5" cellspacing="0" border="0" width="100%" class="ibn-propertysheet">
				<asp:Repeater ID="repCategories" runat="server">
					<ItemTemplate>
						<tr>
							<td class="ibn-navline ibn-sectionheader" style="font-size: 10pt">
								<%# DataBinder.Eval(Container.DataItem,"Title") %>
							</td>
						</tr>
						<tr>
							<td class="ibn-navline ibn-alternating text" style="background-color: #F0F8FF">
								<%# DataBinder.Eval(Container.DataItem,"Description") %>
							</td>
						</tr>
						<tr>
							<td class="ibn-navline">
								<asp:DataGrid ID="dgProduct" ShowHeader="true" CellPadding="2" BorderWidth="0px"
									GridLines="None" Width="100%" PagerStyle-HorizontalAlign="Right" PagerStyle-Visible="true"
									CellSpacing="0" AllowSorting="False" AutoGenerateColumns="False" runat="server"
									EnableViewState="False">
									<Columns>
										<asp:TemplateColumn>
											<HeaderTemplate>
												<table border="0" cellpadding="0" cellspacing="0" width="100%" class="ibn-propertysheet">
													<tr>
														<td>
															<table style="table-layout: fixed;" width="100%" border="0" cellpadding="2" cellspacing="0"
																class="ibn-propertysheet">
																<colgroup>
																	<col />
																	<col width="180px" />
																	<col width="100px" />
																	<col width="100px" />
																</colgroup>
																<tr>
																	<td>
																		<%# LocRM.GetString("tDescr")%>
																	</td>
																	<td style="width: 180px;">
																		<%# LocRM.GetString("tVersNum")%>
																	</td>
																	<td style="width: 100px;">
																		<%# LocRM.GetString("tSize")%>
																	</td>
																	<td style="width: 100px;">
																		&nbsp;</td>
																</tr>
															</table>
														</td>
													</tr>
												</table>
											</HeaderTemplate>
											<ItemTemplate>
												<table border="0" cellpadding="0" cellspacing="0" width="100%" class="ibn-propertysheet">
													<tr>
														<td>
															<table style="table-layout: fixed; border-top: 1px solid #ccc;" width="100%" border="0"
																cellpadding="2" cellspacing="0" class="ibn-propertysheet">
																<colgroup>
																	<col />
																	<col width="180px" />
																	<col width="100px" />
																	<col width="100px" />
																</colgroup>
																<tr>
																	<td>
																		<a href='../Download/<%# DataBinder.Eval(Container.DataItem,"FileName") %>'>
																			<%# DataBinder.Eval(Container.DataItem,"ProductTitle") %>
																		</a>
																	</td>
																	<td style="width: 180px;">
																		<%# DataBinder.Eval(Container.DataItem,"Build") %>
																		&nbsp;(<%# ((DateTime)DataBinder.Eval(Container.DataItem,"Date")).ToString("d") %>)
																	</td>
																	<td style="width: 100px;">
																		<%# GetFileSize(DataBinder.Eval(Container.DataItem,"FileName").ToString())%>
																	</td>
																	<td style="width: 100px;">
																		<a href='../Download/<%# DataBinder.Eval(Container.DataItem,"FileName") %>'>
																			<%=LocRM.GetString("tDownloadNow")%>
																		</a>
																	</td>
																</tr>
																<tr>
																	<td colspan="4">
																		<div style="padding-top: 4px; color: #666; font-style: italic">
																			<%# DataBinder.Eval(Container.DataItem,"Description") %>
																		</div>
																	</td>
																</tr>
															</table>
														</td>
													</tr>
												</table>
											</ItemTemplate>
										</asp:TemplateColumn>
									</Columns>
								</asp:DataGrid>
							</td>
						</tr>
					</ItemTemplate>
				</asp:Repeater>
			</table>
		</td>
	</tr>
</table>
