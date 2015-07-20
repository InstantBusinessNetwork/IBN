<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.UI.Web.Workspace.Modules.FirstInviteAdmin" Codebehind="FirstInviteAdmin.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="../../Modules/BlockHeader.ascx"%>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<table class="ibn-stylebox" style="MARGIN-TOP:0px" cellspacing="0" cellpadding="0" width="100%"
	border="0">
	<tr>
		<td>
			<ibn:blockheader id="secHeader" runat="server"></ibn:blockheader>
		</td>
	</tr>
	<tr>
	  <td style="padding:10px;">
	    <div class="text" style="padding-left:12px;padding-top:5px;padding-bottom:5px;"><b><asp:Label CssClass="ibn-sectionheader" ID="lblAddUsers" runat="server"></asp:Label></b></div>
	    <div style="padding-left:7px;margin:5px;padding-bottom:3px">
        <asp:Label ID="lblMessage" runat="server" CssClass="text"></asp:Label>
      </div>
	    <table cellspacing="0" cellpadding="7" border="0" class="text" style="margin-left:45px;">
	      <tr style="color:#666;">
	        <td></td>
	        <td align="center"><b><%=LocRM.GetString("tFirstName")%>:</b></td>
	        <td align="center"><b><%=LocRM.GetString("tLastName")%>:</b></td>
	        <td align="center"><b><%=LocRM.GetString("teMail")%>:</b></td>
	        <td align="center"><asp:Label ID="lblMakeAdmin" runat="server"></asp:Label></td>
	      </tr>
		    <tr>
		      <td>1.</td>
		      <td style="width:110px;">
			      <asp:TextBox ID="txtFirstName1" CssClass="text" Runat="server" Width="110px"></asp:TextBox>
		      </td>
		      <td style="width:110px;">
			      <asp:TextBox ID="txtLastName1" CssClass="text" Runat="server" Width="110px"></asp:TextBox>
		      </td>
		      <td style="width:110px;">
			      <asp:TextBox ID="txtEMail1" CssClass="text" Runat="server" Width="95px"></asp:TextBox>
			      <asp:RegularExpressionValidator CssClass="text" id="revEMail1" runat="server" ControlToValidate="txtEMail1" Display="Dynamic" ErrorMessage="*" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
		      </td>
		      <td style="width:120px;" align="center">
			      <asp:CheckBox ID="cbAdmin1" CssClass="text" runat="server" Font-Bold="true" />
		      </td>
	      </tr>
	      <tr>
	        <td>2.</td>
		      <td>
			      <asp:TextBox ID="txtFirstName2" CssClass="text" Runat="server" Width="110px"></asp:TextBox>
		      </td>
		      <td>
			      <asp:TextBox ID="txtLastName2" CssClass="text" Runat="server" Width="110px"></asp:TextBox>
		      </td>
		      <td>
			      <asp:TextBox ID="txtEMail2" CssClass="text" Runat="server" Width="95px"></asp:TextBox>
			      <asp:RegularExpressionValidator CssClass="text" id="revEMail2" runat="server" ControlToValidate="txtEMail2" Display="Dynamic" ErrorMessage="*" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
		      </td>
		      <td align="center">
			      <asp:CheckBox ID="cbAdmin2" CssClass="text" runat="server" Font-Bold="true" />
		      </td>
	      </tr>
	      <tr>
	        <td>3.</td>
		      <td>
			      <asp:TextBox ID="txtFirstName3" CssClass="text" Runat="server" Width="110px"></asp:TextBox>
		      </td>
		      <td>
			      <asp:TextBox ID="txtLastName3" CssClass="text" Runat="server" Width="110px"></asp:TextBox>
		      </td>
		      <td>
			      <asp:TextBox ID="txtEMail3" CssClass="text" Runat="server" Width="95px"></asp:TextBox>
			      <asp:RegularExpressionValidator CssClass="text" id="revEMail3" runat="server" ControlToValidate="txtEMail3" Display="Dynamic" ErrorMessage="*" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
		      </td>
		      <td align="center">
			      <asp:CheckBox ID="cbAdmin3" CssClass="text" runat="server" Font-Bold="true" />
		      </td>
	      </tr>
	      <tr>
	        <td>4.</td>
		      <td>
			      <asp:TextBox ID="txtFirstName4" CssClass="text" Runat="server" Width="110px"></asp:TextBox>
		      </td>
		      <td>
			      <asp:TextBox ID="txtLastName4" CssClass="text" Runat="server" Width="110px"></asp:TextBox>
		      </td>
		      <td>
			      <asp:TextBox ID="txtEMail4" CssClass="text" Runat="server" Width="95px"></asp:TextBox>
			      <asp:RegularExpressionValidator CssClass="text" id="revEMail4" runat="server" ControlToValidate="txtEMail4" Display="Dynamic" ErrorMessage="*" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
		      </td>
		      <td align="center">
			      <asp:CheckBox ID="cbAdmin4" CssClass="text" runat="server" Font-Bold="true" />
		      </td>
	      </tr>
      </table>
      <div class="text" style="padding-left:12px;padding-top:25px;padding-bottom:5px;"><b><asp:Label ID="lblElements" CssClass="ibn-sectionheader" runat="server"></asp:Label></b></div>
      <div style="padding-left:7px;margin:5px;padding-bottom:3px">
        <asp:Label ID="lblMessage1" runat="server" CssClass="text"></asp:Label>
      </div>
      <div class="text" style="font-size:0.8em;color:#222;padding-bottom:10px;padding-top:10px;padding-left:45px;"><asp:CheckBox Font-Bold="true" ID="cbAdd" runat="server" onclick="grayText(this);" /></div>
      <table id="tblDemo" cellspacing="0" cellpadding="7" border="0" class="text" style="margin-left:45px;">
		    <tr>
		      <td><asp:Label ID="lblDemo1" runat="server"></asp:Label></td>
		    </tr>
		    <tr>
		      <td><asp:Label ID="lblDemo2" runat="server"></asp:Label></td>
		    </tr>
		    <tr>
		      <td><asp:Label ID="lblDemo3" runat="server"></asp:Label></td>
		    </tr>
		    <tr>
		      <td><asp:Label ID="lblDemo4" runat="server"></asp:Label></td>
		    </tr>
		    <tr>
		      <td><asp:Label ID="lblDemo5" runat="server"></asp:Label></td>
		    </tr>
		    <tr>
		      <td><asp:Label ID="lblDemo6" runat="server"></asp:Label></td>
		    </tr>
		    <tr>
		      <td><asp:Label ID="lblDemo7" runat="server"></asp:Label></td>
		    </tr>
		  </table>
		  <div style="text-align:left;padding: 24 12 12 12;">
		    <asp:Button Width="250px" ID="btnSend" runat="server" CssClass="text" Font-Size="0.9em" OnClick="btnSend_Click" />
		  </div>
	  </td>
	</tr>
</table>
<style>
  .graytext{
    color:#aaa;
  }
</style>
<script>
  var cbObj = document.getElementById('<%=cbAdd.ClientID %>');
  if(cbObj)
    grayText(cbObj);
  function grayText(obj)
  {
    var tbl = document.getElementById("tblDemo");
    if(tbl && obj.checked)
      tbl.className = "text";
    else if(tbl && !obj.checked)
      tbl.className = "text graytext";
  }
</script>