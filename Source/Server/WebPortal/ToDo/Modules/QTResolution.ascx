<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.ToDo.Modules.QTResolution" Codebehind="QTResolution.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeaderLightWithMenu.ascx" %>
<table cellspacing="0" cellpadding="0" border="0" width="100%" style="MARGIN-TOP:5px">
	<tr>
		<td>
			<ibn:blockheader id="tbView" runat="server" title="" />
		</td>
	</tr>
</table>
<table cellpadding="5" cellspacing="0" border="0" width="100%" class="ibn-stylebox-light text">
	<tr>
		<td valign="center">
			<%=LocRM.GetString("QTRText1") %>
		</td>
	</tr>
	<tr>
		<td valign="center">
			<%=LocRM.GetString("QTRText2") %>
		</td>
	</tr>
	<tr>
		<td align="middle" style="PADDING-BOTTOM:10px">
			<table cellpadding="5" cellspacing="0" border="0" width="100%" class="text"> 
				<tr>
					<td valign="top"><b><%=LocRM.GetString("Resolution") %>:</b></td>
					<td width="100%">
						<asp:TextBox id="tbResolution" Runat="server" CssClass="text" TextMode="MultiLine" Width="100%" Rows="5"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td valign="top"><b><%=LocRM.GetString("Workaround") %>:</b></td>
					<td>
						<asp:TextBox id="tbWorkaround" CssClass="text" Runat="server" TextMode="MultiLine" Width="100%" Rows="5"></asp:TextBox>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td align="middle" style="PADDING-BOTTOM:10px">
			<P align="right">
				<button type="button" ID="btnCompleteToDo" Runat="server" Class="text" style="MARGIN-BOTTOM:-2px" onserverclick="btnCompleteToDo_ServerClick">
					<img src="../Layouts/Images/SaveItem.gif" border="0" width="16" height="16" align="absMiddle">&nbsp;<%=LocRM.GetString("Save") %>
				</button>
			</P>
		</td>
	</tr>
</table>
		