<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActivitiesByBusinessScores2.ascx.cs" Inherits="Mediachase.UI.Web.Projects.Modules.ActivitiesByBusinessScores2" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeader.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<style type="text/css">
.grid
{
	border: solid 1px;
	font-family:verdana;
	font-size:12px;
	font-weight:normal;
	border-color : white Gray Gray white;
}

.gridheader
{
	background-Color:#E1ECFC;
}
</style>
<script type="text/javascript">
function OpenWindow(query,w,h,scroll)
{
	var l = (screen.width - w) / 2;
	var t = (screen.height - h) / 2;
	
	winprops = 'modal=1,resizable=0,height='+h+',width='+w+',top='+t+',left='+l;
	if (scroll) winprops+=',scrollbars=1';
	var f = window.open(query, "_blank", winprops);
}

function CheckForCustomFilter(obj)
{
	if(obj!=null && obj.selectedIndex == obj.options.length-1)
	{
		OpenWindow('../Projects/ProjectsByBusinessScoresPopUp.aspx',350,350,false);
	}
}

function EnableApplyButton()
{
	var but = document.getElementById('<%=btnApplyFilter.ClientID%>');
	if(but!=null)
	{
		but.style.display = 'block';
	}
	
}
</script>

<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="MARGIN-TOP:0px">
	<tr>
		<td class="ms-toolbar">
			<ibn:blockheader id="secHeader" runat="server" />
		</td>
	</tr>
	<tr>
		<td class="ibn-navline ibn-alternating text" style="padding-top:8px" valign="top">
			<table cellpadding="5px">
				<tr>
					<td class="text" align="right" >
						<b><%=LocRM.GetString("tPrjGroup")%>:</b>
						<asp:DropDownList ID="ddPrjGroup" Runat=server Width="190px" CssClass="text" onchange="javascript:CheckForCustomFilter(this);">
						</asp:DropDownList>
					</td>
					<td width="20px"></td>
					<td class="text" align="right">
						<b><%=LocRM.GetString("tScale")%>:</b>
						<asp:DropDownList Runat="server" ID="ddFinanceType" Width="190px">
						</asp:DropDownList>
					</td>
					<td width="20px"></td>
				</tr>
				<tr>
					<td align="right" class="text">
						<b><%=LocRM.GetString("tGroupBy")%>:</b>
						<asp:DropDownList Runat="server" ID="ddGroupBy" Width="190px">
						</asp:DropDownList>
					</td>
					<td width="20px"></td>
					<td></td>
					<td width="20px"></td>
				</tr>
				<tr>
					<td class="text" align="right">
						<b><%=LocRM.GetString("tFrom")%>:</b>
						<asp:TextBox Runat="server" ID="tbFromYear" Width="190px"></asp:TextBox>
						
					</td>
					<td width="20px">
						<asp:RequiredFieldValidator Runat="server" ID="rfvFromYear" Display="Dynamic" ControlToValidate="tbFromYear" ErrorMessage="*"></asp:RequiredFieldValidator>
						<asp:CompareValidator Runat="server" ID="cvFromYear" Display="Dynamic" ControlToValidate="tbFromYear" Type="Integer" Operator="GreaterThan" ValueToCompare="1995" ErrorMessage="*"></asp:CompareValidator> 
						<asp:CompareValidator Runat="server" ID="Comparevalidator1" Display="Dynamic" ControlToValidate="tbFromYear" Type="Integer" Operator="LessThan" ValueToCompare="2050" ErrorMessage="*"></asp:CompareValidator> 
						<asp:RegularExpressionValidator Runat="server" ID="revFromYear" Display="Dynamic" ControlToValidate="tbFromYear" ErrorMessage="*" ValidationExpression="\d+"></asp:RegularExpressionValidator>
					</td>
					<td class="text" align="right">
						<b><%=LocRM.GetString("tTo")%>:</b>
						<asp:TextBox Runat="server" ID="tbToYear" Width="190px"></asp:TextBox>
						
					</td>
					<td width="20px">
						<asp:RequiredFieldValidator Runat="server" ID="rfvToYear" Display="Dynamic" ControlToValidate="tbToYear" ErrorMessage="*"></asp:RequiredFieldValidator>
						<asp:CompareValidator Runat="server" ID="cvToYear" Display="Dynamic" ControlToValidate="tbToYear" Type="Integer" Operator="GreaterThanEqual" ControlToCompare="tbFromYear" ErrorMessage="*"></asp:CompareValidator> 
						<asp:CompareValidator Runat="server" ID="cvToYear1" Display="Dynamic" ControlToValidate="tbToYear" Type="Integer" Operator="LessThan" ValueToCompare="2050" ErrorMessage="*"></asp:CompareValidator>
						<asp:RegularExpressionValidator Runat="server" ID="revToYear" Display="Dynamic" ControlToValidate="tbToYear" ErrorMessage="*" ValidationExpression="\d+"></asp:RegularExpressionValidator>
					</td>
				</tr>
				<tr>
					<td class="text" align="right">
						<b><%=LocRM.GetString("tCompare")%>:</b>
						<asp:DropDownList Runat="server" ID="ddBasePlan1" Width="190px"></asp:DropDownList>
						
					</td>
					<td class="text" align="left">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;/</td>
					<td align="right" class="text">
						<asp:DropDownList Runat="server" ID="ddBasePlan2" Width="190px"></asp:DropDownList>
					</td>
					<td></td>
				</tr>
				<tr >
					<td  class="text" align="right" colspan="3">
						<btn:imbutton style="width:110px" class="text" runat="server" id="btnApplyFilter"></btn:imbutton>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td>
			<table runat="server" id="tGrid" border="1" cellpadding="3" cellspacing="0" style="border-collapse:collapse" class="grid">
			
			</table>
		</td>
	</tr>
</table>