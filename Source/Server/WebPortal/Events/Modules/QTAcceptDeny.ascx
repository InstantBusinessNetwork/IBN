<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Events.Modules.QuickTrackingAcceptDeny" Codebehind="QTAcceptDeny.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeader.ascx" %>
<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox" style="MARGIN-TOP:10px">
	<tr>
		<td>
			<ibn:blockheader id="tbView" runat="server" title="" />
		</td>
	</tr>
	<tr>
		<td>
			<table cellpadding="3" cellspacing="0" border="0" width="100%" class="text">
				<tr>
					<td width="80" align="middle" rowspan="2">
						<img src="../Layouts/Images/check.gif" alt="" width="60" height="98" border="0">
					</td>
					<td>
						<%=LocRM.GetString("ADText") %>
					</td>
				</tr>
				<tr>
					<td align="right" valign="bottom" style="PADDING-RIGHT:10px; PADDING-LEFT:10px; PADDING-BOTTOM:10px; PADDING-TOP:10px">
						<btn:ImButton ID="btnAccept" Runat="server" Class="text" style="width:110px;" Text='<%# LocRM.GetString("Accept") %>' onserverclick="btnAccept_ServerClick">
						</btn:ImButton>&nbsp;&nbsp;
						<btn:ImButton ID="btnDecline" Runat="server" Class="text" style="width:110px;" Text='<%# LocRM.GetString("Decline") %>' IsDecline=true onserverclick="btnDecline_ServerClick">
						</btn:ImButton>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
