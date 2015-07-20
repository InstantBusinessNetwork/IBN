<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Incidents.Modules.QuickTracking" Codebehind="QuickTracking.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="../../Modules/BlockHeaderLightWithMenu.ascx" %>
<table cellspacing="0" cellpadding="0" width="100%" border="0" id="trCanwrite" runat=server style="margin-top:5px;margin-right:5px">
	<tr>
		<td><ibn:blockheader id="Migrated_tbQuickTracking" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td>
			<table cellpadding="3" cellspacing="3" border="0" width="100%" class="text ibn-stylebox-light">
				<tr>
					<td valign="top" class="ibn-label"><%=LocRM.GetString("Status")%>:</td>
					<td valign="top" width="100%"><asp:DropDownList id="ddlStatus" runat="server" Width="168px"></asp:DropDownList></td>
				</tr>
				<tr>
					<td valign="top" class="ibn-label"><%=LocRM.GetString("Resolution")%>:</td>
					<td valign="top">
						<asp:TextBox id="txtResolution" runat="server" Height="80px" Width="100%" TextMode="MultiLine" CssClass="text"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td valign="top" class="ibn-label"><%=LocRM.GetString("Workaround")%>:</td>
					<td valign="top"><asp:TextBox id="txtWorkaround" runat="server" Height="80px" Width="100%" TextMode="MultiLine" CssClass="text"></asp:TextBox></td>
				</tr>
				<tr>
					<td colspan="2" align="right"><btn:ImButton ID="btnSave" Runat="server" Class="text" style="width:110px;" onserverclick="btnSave_Click"></btn:ImButton></td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<table cellspacing="0" cellpadding="0" width="100%" border="0" id="trCanread" runat=server style="margin-top:5px;margin-right:5px">
	<tr>
		<td><ibn:blockheader id="Migrated_tbQuickTracking1" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td>
			<table cellpadding="3" cellspacing="3" border="0" width="100%" class="text ibn-stylebox-light">
				<tr>
					<td vAlign="top"><b><%=LocRM.GetString("Status")%>:</b></td>
					<td vAlign="top">
						<asp:Label ID=lblStatus Runat=server></asp:Label>
					</td>
				</tr>
				<tr>
					<td vAlign="top"><b><%=LocRM.GetString("Resolution")%>:</b>
					</td>
					<td vAlign="top" width="100%">
						<asp:Label ID="lblResolution" Runat=server></asp:Label>
					</td>
				</tr>
				<tr>
					<td vAlign="top"><b><%=LocRM.GetString("Workaround")%>:</b></td>
					<td vAlign="top">
						<asp:Label ID="lblWorkaround" Runat=server></asp:Label>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<table cellpadding="0" cellspacing="0" width="99%" border="0"><tr><td valign="top">
<table cellspacing="0" cellpadding="0" width="100%" border="0" id="trAD" runat=server style="margin-top:5px;">
	<tr>
		<td>
			<ibn:blockheader id="Migrated_tbQuickTracking2" runat="server"></ibn:blockheader>
		</td>
	</tr>
	<tr>
		<td>
			<table cellpadding="0" cellspacing="0" border="0" width="100%" class="text ibn-stylebox-light">
				<tr>
					<td align="middle" width="80" rowSpan="2"><IMG height="98" alt="" src="../Layouts/Images/check.gif" width="60" border="0">
					</td>
					<td vAlign="top">
						<table cellspacing="0" cellpadding="7" border="0" class="text" width="100%">
							<tr>
								<td>
									<%=LocRM.GetString("ADText") %>
									<br>
									<br>
									<div style="WIDTH:100%" align="center">
										<btn:ImButton class=text id="btnAccept" Runat="server" style="width:110px;" onserverclick="btnAccept_ServerClick">
										</btn:ImButton>&nbsp;&nbsp;
										<btn:ImButton class=text id="btnDecline" Runat="server" isdecline="true" style="width:110px;" onserverclick="btnDecline_ServerClick">
										</btn:ImButton>
									</div>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table></td></tr></table>