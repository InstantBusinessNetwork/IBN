<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.WebTrial.AntiRobotControl" Codebehind="AntiRobotControl.ascx.cs" %>
<nobr>
<asp:TextBox ID="txtKeyword" TabIndex="15" runat="server" Width="200px"></asp:TextBox>
<asp:Image ImageAlign="Top" ID="imgARobot" runat="server" />
<asp:CustomValidator runat="server" ID="cvalKeyword" ControlToValidate="txtKeyword" ErrorMessage="*" Display="Static" OnServerValidate="cvalKeyword_ServerValidate" ></asp:CustomValidator>
<asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="*" ControlToValidate="txtKeyword"></asp:RequiredFieldValidator>
<asp:HiddenField ID="hidValue" runat="server" />
</nobr>