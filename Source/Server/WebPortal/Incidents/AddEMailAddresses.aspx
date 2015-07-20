<%@ Page Language="C#" AutoEventWireup="true" Inherits="Mediachase.UI.Web.Incidents.AddEMailAddresses" CodeBehind="AddEMailAddresses.aspx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="lst" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
	
	<title><%=LocRM.GetString("tAddEMails")%></title>
</head>
<body>
	<form id="form1" runat="server">
	<ibn:BlockHeader ID="secHeader" Title="ToolBar" runat="server"></ibn:BlockHeader>
	<table class="text" style="height: 100%" height="100%" cellspacing="0" cellpadding="2" width="100%" border="0">
		<tr height="22">
			<td width="320px" height="22" style="padding-left: 5px">
				<span class="boldtext">
					<%=LocRM.GetString("Available") %>:</span>
			</td>
			<td width="4px">
			</td>
			<td class="boldtext">
				<%=LocRM.GetString("Selected") %>:
			</td>
		</tr>
		<tr style="height: 100%">
			<td valign="top" width="320px">
				<!-- Groups & Users -->
				<table class="text" style="margin-top: 5px" cellspacing="0" cellpadding="3" width="100%">
					<tr>
						<td width="9%">
							<%=LocRM.GetString("Group") %>:
						</td>
						<td width="91%">
							<lst:IndentedDropDownList ID="ddGroups" runat="server" CssClass="text" Width="200px" AutoPostBack="True">
							</lst:IndentedDropDownList>
						</td>
					</tr>
					<tr>
						<td width="9%">
							<%=LocRM.GetString("Search") %>:
						</td>
						<td width="91%">
							<asp:TextBox ID="tbSearch" runat="server" CssClass="text" Width="125px"></asp:TextBox>
							<asp:Button ID="btnSearch" runat="server" Width="80px" CssClass="text" CausesValidation="False"></asp:Button>
						</td>
					</tr>
					<tr>
						<td valign="top" height="96">
							<%=LocRM.GetString("Users") %>:
						</td>
						<td valign="top">
							<asp:ListBox ID="lbUsers" runat="server" CssClass="text" Width="200px" SelectionMode="Multiple" Rows="6"></asp:ListBox>
						</td>
					</tr>
					<tr>
						<td width="9%">
							<%=LocRM.GetString("eMail") %>:
						</td>
						<td width="91%">
							<asp:TextBox ID="txtMail" runat="server" CssClass="text" Width="200px"></asp:TextBox>
							<asp:RegularExpressionValidator ID="reMail" runat="server" ErrorMessage="*" ControlToValidate="txtMail" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" Display="Dynamic"></asp:RegularExpressionValidator>
						</td>
					</tr>
					<tr>
						<td valign="top" height="28">
							&nbsp;
						</td>
						<td>
							<button id="btnAdd" runat="server" class="text" style="width: 100px;">
							</button>
						</td>
					</tr>
				</table>
				<!-- End Groups & Users -->
			</td>
			<td width="4px">
			</td>
			<td valign="top" height="100%">
				<!-- Selected Users -->
				<table class="text" style="margin-top: 5px" cellspacing="0" cellpadding="3" width="100%">
					<tr>
						<td valign="top">
							<asp:ListBox ID="lstSelected" runat="server" CssClass="text" Width="200px" SelectionMode="Multiple" Rows="13"></asp:ListBox>
						</td>
					</tr>
					<tr>
						<td>
							<button id="btnDel" runat="server" class="text" style="width: 100px;">
							</button>
						</td>
					</tr>
				</table>
				<!-- End Selected Users -->
			</td>
		</tr>
	</table>
	<asp:LinkButton ID="btnSave" runat="server" Visible="False"></asp:LinkButton>
	<input id="iFields" type="hidden" name="iFields" runat="server" />
	<input id="iGroupFields" type="hidden" name="iGroupFields" runat="server" />

	<script type="text/javascript">
		//<![CDATA[
		function SaveFields()
		{
			var sControl=document.forms[0].<%=lstSelected.ClientID%>;
			var str="";
			if(sControl != null)
			{
				for(var i=0;i<sControl.options.length;i++)
				{
					str += sControl.options[i].value + ",";
				}
			}
			document.getElementById('<%=iFields.ClientID%>').value = str;
		}
		function RemoveFew(FromControl,ToControl)
		{
			if((FromControl!=null)&&(ToControl!=null))
			{
				for(var i=0;i<FromControl.options.length;i++)
					if((FromControl.options[i].selected) && (!CheckExistence(ToControl,FromControl.options[i].text)))
					{
						try 
						{
							var user_id = parseInt(FromControl.options[i].value);
							var str = document.forms[0].<%=iGroupFields.ClientID%>.value;
							var str1 = FromControl.options[i].value + ",";
							if(!isNaN(user_id) && str.indexOf(str1)>=0)
							{
								AddOption(ToControl,FromControl.options[i]);
							}
						}
						catch (e) {}
					}
				for(var i=0;i<FromControl.options.length;i++)
					if(FromControl.options[i].selected)
					{
						FromControl.options[i]=null;
						i=i-1;
					}
				SaveFields();
				return true;
			}
		}
		function MoveFew(FromControl,ToControl)
		{
			if((FromControl!=null)&&(ToControl!=null))
			{
				for(var i=0;i<FromControl.options.length;i++)
					if((FromControl.options[i].selected) && (!CheckExistence(ToControl,FromControl.options[i].text)))
							AddOption(ToControl,FromControl.options[i]);

				for(var i=0;i<FromControl.options.length;i++)
					if(FromControl.options[i].selected)
					{
						FromControl.options[i]=null;
						i=i-1;
					}
				FromControl.selectedIndex = -1;
				obj = document.forms[0].<%=txtMail.ClientID%>;
				var re = /\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*/g;
				if(obj!=null && obj.value!="" && re.test(obj.value))
				{
					var oOption = document.createElement("OPTION");
					oOption.text = obj.value;
					oOption.value = obj.value;
					AddOption(ToControl, oOption);
					obj.value = "";
				}
				return true;
			}
		}
		//]]>
	</script>

	</form>
</body>
</html>
