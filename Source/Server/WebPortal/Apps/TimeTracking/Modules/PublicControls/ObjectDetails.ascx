<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ObjectDetails.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.TimeTracking.Modules.PublicControls.ObjectDetails" %>
<%@ Register TagPrefix="ibn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<div style="margin:2px;" id="mainDivDetails" runat="server">
<ibn:XmlFormBuilder ID="xmlStruct" runat="server" />
<div style="text-align:center;padding-top:6px;border:1px solid #95b7f3;width:100%;height:30px;" id="divNoDetails" runat="server"></div>
</div>
<asp:HiddenField ID="ctrlUpdate" runat="server" />