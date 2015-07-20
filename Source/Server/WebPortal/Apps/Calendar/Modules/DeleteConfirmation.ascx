<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DeleteConfirmation.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Calendar.Modules.DeleteConfirmation" %>
<div style="text-align:center;padding:10px;">
<asp:Label ID="lblWarning" runat="server"></asp:Label>
</div>
<div style="text-align:center;padding:5px;">
<asp:Button ID="btnOnlyThis" runat="server" />&nbsp;
<asp:Button ID="btnAllSeries" runat="server" />&nbsp;
<asp:Button ID="btnCancel" runat="server" />
</div>