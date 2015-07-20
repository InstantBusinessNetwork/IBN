<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.UsersReports" Codebehind="UsersReports.ascx.cs" %>
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
<div style="PADDING-RIGHT:10px;PADDING-LEFT:10px;PADDING-BOTTOM:10px;WIDTH:100%;PADDING-TOP:0px">
	<table cellspacing="0" cellpadding="0" border="0">
		<tr>
			<td height="9"><img height="9" alt="" src="../layouts/images/blank.gif" width="1"></td>
		</tr>
		<tr>
			<td class="ibn-sectionheader" colspan="3"><%=LocRM.GetString("UserReports")%>
			</td>
		</tr>
		<tr>
			<td height="2"><img height="2" alt="" src="../layouts/images/blank.gif" width="1"></td>
		</tr>
		<tr>
			<td class="ibn-sectionline" colspan="3" height="1"><img height="1" alt="" src="../layouts/images/blank.gif" width="1"></td>
		</tr>
		<tr>
			<td style="padding-top: 6px" valign="top"><img alt="" src="../layouts/images/listset.gif" /></td>
			<td width="3"><img height="1" alt="" src="../layouts/images/blank.gif" width="3"></td>
			<td width="100%">
				<table cellspacing="0" cellpadding="0" border="0">
					<tr>
						<td class="ibn-descriptiontext" style="PADDING-TOP: 7px" valign="top">
							<%=LocRM.GetString("Help")%>
						</td>
					</tr>
					<tr>
						<td class="ibn-propertysheet" style="padding-left: 1px">
							<table cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td class="ibn-descriptiontext" style="padding-top: 7px">
										<img alt="" src="../layouts/images/rect.gif" />
										<a href="~/Reports/DirectoryStatistics.aspx" runat="server">
											<%=LocRM.GetString("UserStatistics")%>
										</a>
									</td>
								</tr>
								<tr>
									<td class="ibn-descriptiontext" style="padding-top: 7px">
										<img alt="" src="../layouts/images/rect.gif" />
										<a runat="server" href="javascript:OpenWindow('../Reports/XMLReport.aspx?Type=Usr',750,466,true)">
											<%=LocRM.GetString("XMLReport")%>
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
</div>
