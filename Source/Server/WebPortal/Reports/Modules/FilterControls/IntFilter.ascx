<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Reports.Modules.IntFilter" Codebehind="IntFilter.ascx.cs" %>
<SCRIPT language="javascript">
	function ChangeModifyInt(obj)
	{
		objTd = document.getElementById('<%=tdSecond.ClientID %>');
		id=obj.value;
		if(id=="3")
		{
			objTd.style.display = '';
		}
		else
		{
			objTd.style.display = 'none';
		}
	}
</SCRIPT>
<table cellspacing="0" cellpadding="2" border="0">
	<tr height="3px"><td></td></tr>
	<tr height="35px">
		<td runat=server id="Migrated_tdTitle" align="left" class="text">
			<b><asp:Label ID="lblTitle" Runat="server" CssClass="text"></asp:Label>:</b>&nbsp;&nbsp;&nbsp;
		</td>
		<td width="125px" valign="center" class="text">
			<SELECT class="text" id="ddType" style="WIDTH: 115px" onchange="ChangeModifyInt(this);" name="ddType" runat="server"></SELECT>
		</td>
		<td align="left" width="70px" class="text">
			<asp:TextBox id="txtValue1" runat="server" CssClass="text" Wrap="False" Width="60" MaxLength="9"></asp:TextBox>
			<asp:RangeValidator ID="rvValue1" Runat=server ControlToValidate="txtValue1" 
				MinimumValue="-10000000" MaximumValue="100000000" 
				Type=Currency Display=Dynamic ErrorMessage="*" CssClass="text">
			</asp:RangeValidator>
		</td>
		<td id="tdSecond" runat="server" align="left" width="100px" class="text">
			<table cellpadding="0" cellspacing="0">
				<tr>
					<td align="center" width="20px"><b>-</b></td>
					<td>
						<asp:TextBox ID="txtValue2" Runat=server CssClass="text" Wrap="False" Width="60" MaxLength="9"></asp:TextBox>
						<asp:RangeValidator ID="rvValue2" Runat=server ControlToValidate="txtValue2"
							MinimumValue="-10000000" MaximumValue="100000000"
							Type=Currency Display=Dynamic ErrorMessage="*" CssClass="text">
						</asp:RangeValidator>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>