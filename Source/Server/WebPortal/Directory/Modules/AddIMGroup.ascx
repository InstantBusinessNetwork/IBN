<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.AddIMGroup" Codebehind="AddIMGroup.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<script type="text/javascript"> 
		function ChangeColor()
		{
			try
			{
				<%=colorBox.ClientID%>.style.backgroundColor = document.forms[0].<%=tbColor.ClientID%>.value;
			}
			catch (e) {}
		}
</script>
<table class="ibn-stylebox text" cellspacing="0" cellpadding="0" border="0" width="100%" style="MARGIN-TOP: 0px;margin-left:2px">
	<tr>
		<td>
			<ibn:blockheader id="secHeader" runat="server" title="Department 1" />
		</td>
	</tr>
	<tr>
		<td style="PADDING-RIGHT:15px; PADDING-LEFT:15px; PADDING-BOTTOM:15px; PADDING-TOP:15px">
			<table id="Table1" cellspacing="0" cellpadding="3" width="100%" border="0" class="text">
				<tr>
					<td vAlign="top" width="200" class="ibn-label"><%=LocRM.GetString("GroupTitle") %>:</td>
					<td class="ibn-value">
						<asp:TextBox id="tbGroupTitle" Runat="server" CssClass="text" Width="227"></asp:TextBox>
						<asp:RequiredFieldValidator id="rfGroupTitle" runat="server" CssClass="Text" ErrorMessage=" * " ControlToValidate="tbGroupTitle"></asp:RequiredFieldValidator></td>
				</tr>
				<tr>
					<td vAlign="top" class="ibn-label"><%=LocRM.GetString("GroupLogo") %>:</td>
					<td class="ibn-value">
						<span class="color" id="colorBox" style="BACKGROUND-COLOR:#2b6087" runat="server">
							<div style="OVERFLOW: hidden; width: 227px; height: 36px">
								<img id="imgLogo" name="imgLogo" runat="server">
							</div>
						</span>
						<br>
						<cc1:McHtmlInputFile id="fLogo" runat="server" CssClass="text" Width="227px" size="25"></cc1:McHtmlInputFile>
						<asp:CustomValidator id="cvImage" runat="server" ErrorMessage="*" 
							Display="Static" onservervalidate="cvImage_ServerValidate"></asp:CustomValidator></td>
				</tr>
				<tr>
					<td vAlign="top" class="ibn-label"><%=LocRM.GetString("BackgroundColor") %>:</td>
					<td class="ibn-value">
						<asp:TextBox id="tbColor" runat="server" CssClass="text" Width="227px" onpropertychange="ChangeColor()" onkeyup="ChangeColor()" maxlength="6" style="TEXT-TRANSFORM: uppercase; FONT-FAMILY: monospace"></asp:TextBox></td>
				</tr>
				<tr>
					<td colspan="2" align="left">
						<table cellspacing="0" cellpadding="3" border="0" class="text">
							<tr>
								<td vAlign="top" class="ibn-label" style="padding-left:0"><%=LocRM.GetString("VisibleGroups") %></td>
								<td vAlign="top" class="ibn-label"><%=LocRM.GetString("GroupsCU") %></td>
							</tr>
							<tr>
								<td vAlign="top" style="padding-left:0"><asp:ListBox id="lbVisible" runat="server" CssClass="text" Rows="10" Width="300px" SelectionMode="Multiple"></asp:ListBox></td>
								<td vAlign="top"><asp:ListBox id="lbCU" runat="server" CssClass="text" Width="300px" Rows="10" SelectionMode="Multiple"></asp:ListBox></td>
							</tr>
							<tr>
								<td colspan="2" align="center" class="ibn-value"><%=LocRM.GetString("UseCtrl") %></td>
							</tr>
							<tr>
								<td align="right" style="padding-left:0"><btn:ImButton ID="btnSave" Runat="server" Class="text" style="width:110px;" onserverclick="btnSave_Click"></btn:ImButton></td>
								<td align="left">
									
								<btn:ImButton ID="btnCancel" CausesValidation="false" Runat="server" Class="text" style="width:110px;" IsDecline="true" onserverclick="btnCancel_Click"></btn:ImButton>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
