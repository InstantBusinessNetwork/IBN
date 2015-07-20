<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Reports.Modules.Reports" Codebehind="Reports.ascx.cs" %>
<script language=javascript>
function OpenWindow(query,w,h,scroll)
	{
		var l = (screen.width - w) / 2;
		var t = (screen.height - h) / 2;
		
		winprops = 'resizable=1, height='+h+',width='+w+',top='+t+',left='+l;
		if (scroll) winprops+=',scrollbars=1';
		var f = window.open(query, "_blank", winprops);
	}
</script>
<table border="0" cellspacing="0" cellpadding="0" width="100%">
	<tr>
		<td height="8"><img height="8" alt="" src="../layouts/images/blank.gif" width="1"></td>
	</tr>
	<tr>
		<td>
			<table cellspacing="0" cellpadding="0" border="0" id="tblProjects" runat=server Visible="false">
				<tr>
					<td height="9"><img height="9" alt="" src="../layouts/images/blank.gif" width="1"></td>
				</tr>
				<tr>
					<td class="ibn-sectionheader" colspan="3"><%=LocRM.GetString("tPrjRep")%>
					</td>
				</tr>
				<tr>
					<td height="2"><img height="2" alt="" src="../layouts/images/blank.gif" width="1"></td>
				</tr>
				<tr>
					<td class="ibn-sectionline" colspan="3" height="1"><img height="1" alt="" src="../layouts/images/blank.gif" width="1"></td>
				</tr>
				<tr>
					<td style="PADDING-TOP: 6px" valign="top"><img alt="" src="../layouts/images/listset.gif" /></td>
					<td width="3"><img height="1" alt="" src="../layouts/images/blank.gif" width="3"></td>
					<td width="100%">
						<table cellspacing="0" cellpadding="0" border="0" >
							<tr>
								<td class="ibn-descriptiontext" style="PADDING-TOP: 7px" valign="top">
									<%=LocRM.GetString("tUse1")%>
								</td>
							</tr>
							<tr>
								<td class="ibn-propertysheet" style="PADDING-LEFT: 1px">
									<table cellspacing="0" cellpadding="0" width="100%" border="0">
										<tr id="Tr5" runat="server">
											<td class="ibn-descriptiontext" style="PADDING-TOP: 7px">
												<img alt="" src="../layouts/images/rect.gif" />
												<a href="javascript:OpenWindow('XMLReport.aspx',750,466,true)">
													<%=LocRM.GetString("tXMLReport")%>
												</a>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
			<table cellspacing="0" cellpadding="0" border="0" id="tblAdminReports" runat="server" Visible="false">
				<tr>
					<td height="9"><img height="9" alt="" src="../layouts/images/blank.gif" width="1"></td>
				</tr>
				<tr>
					<td class="ibn-sectionheader" colspan="3"><%=LocRM.GetString("tAdmPortalRep")%></td>
				</tr>
				<tr>
					<td height="2"><img height="2" alt="" src="../layouts/images/blank.gif" width="1"></td>
				</tr>
				<tr>
					<td class="ibn-sectionline" colspan="3" height="1"><img height="1" alt="" src="../layouts/images/blank.gif" width="1"></td>
				</tr>
				<tr>
					<td style="PADDING-TOP: 6px" valign="top"><img alt="" src="../layouts/images/listset.gif" /></td>
					<td width="3"><img height="1" alt="" src="../layouts/images/blank.gif" width="3"></td>
					<td width="100%" valign="top">
						<table cellspacing="0" cellpadding="0" border="0" runat="server" ID="Table1">
							<tr>
								<td class="ibn-descriptiontext" style="PADDING-TOP: 7px" valign="top">
									<%=LocRM.GetString("tUse2")%>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
			<p>&nbsp;</p>
		</td>
	</tr>
</table>
