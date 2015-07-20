<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.ExceptionsEditor" Codebehind="ExceptionsEditor.ascx.cs" %>
<%@ Reference Control="~/Admin/Modules/TimeControl.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="DateTimeControl" src="~/Admin/Modules/TimeControl.ascx" %>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server" /></td>
	</tr>
	<tr>
		<td style="PADDING:7px">
			<table border="0" cellpadding="10" cellspacing="0">
				<tr>
					<td>
						<asp:RadioButton GroupName="G1" ID="rbWorking" Runat="server" CssClass="text" Text='<%# LocRM.GetString("WorkingDay") %>' AutoPostBack="True" oncheckedchanged="rb_ChangeWorkDay">
						</asp:RadioButton><br>
						<asp:RadioButton GroupName="G1" ID="Radiobutton1" Runat="server" CssClass="text" Text='<%# LocRM.GetString("NoneWorkingDay") %>' AutoPostBack="True" oncheckedchanged="rb_ChangeWorkDay">
						</asp:RadioButton>
					</td>
				</tr>
			</table>
			<fieldset style="PADDING:10px"><legend class="text"><%=LocRM.GetString("Select") %></legend>
				<table border="0" cellpadding="5" cellspacing="0" align="left">
					<tr>
						<td>
							<table cellpadding="0" cellspacing="0" border="0" width="100%" class="text">
								<tr>
									<td><%=LocRM.GetString("From") %>:&nbsp;</td>
									<td><mc:Picker ID="dc1" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" /></td>
									<td>&nbsp;<%=LocRM.GetString("To") %>:&nbsp;</td>
									<td><mc:Picker ID="dc2" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" /></td>
									<td><asp:CustomValidator CssClass="text" id="cv1" runat="server" ErrorMessage='<%# LocRM.GetString("Warning") %>' EnableClientScript="False"></asp:CustomValidator></td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</fieldset>
			<fieldset style="PADDING:8px"><legend class="text"><%=LocRM.GetString("WorkTimeIntervals") %></legend>
				<table border="0" cellpadding="5" cellspacing="0" align="left">
					<tr>
						<td>
							<ibn:datetimecontrol id="tc1" runat="server"></ibn:datetimecontrol>
						</td>
					</tr>
					<tr>
						<td>
							<ibn:datetimecontrol id="tc2" runat="server"></ibn:datetimecontrol>
						</td>
					</tr>
					<tr>
						<td>
							<ibn:datetimecontrol id="tc3" runat="server"></ibn:datetimecontrol>
						</td>
					</tr>
					<tr>
						<td>
							<ibn:datetimecontrol id="tc4" runat="server"></ibn:datetimecontrol>
						</td>
					</tr>
					<tr>
						<td>
							<ibn:datetimecontrol id="tc5" runat="server"></ibn:datetimecontrol>
						</td>
					</tr>
				</table>
				<asp:CustomValidator id="CustomValidator1" runat="server" ErrorMessage='<%# LocRM.GetString("IncorrectIntervals") %>' CssClass="text">
				</asp:CustomValidator>
			</fieldset>
		</td>
	</tr>
</table>
<button id="btnSave" runat=server type=button style="display:none" onserverclick="Button1_Click"></button>

