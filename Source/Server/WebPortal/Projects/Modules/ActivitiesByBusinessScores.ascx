<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.ActivitiesByBusinessScores" CodeBehind="ActivitiesByBusinessScores.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="..\..\Modules\BlockHeader.ascx" %>
<%@ Register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>

<script type="text/javascript">
	//<![CDATA[
	var mygrid2 = null;
	var mygrid = null;

	//Open popup
	function OpenWindow(query, w, h, scroll) {
		var l = (screen.width - w) / 2;
		var t = (screen.height - h) / 2;

		winprops = 'modal=1,resizable=0,height=' + h + ',width=' + w + ',top=' + t + ',left=' + l;
		if (scroll) winprops += ',scrollbars=1';
		var f = window.open(query, "_blank", winprops);
	}

	function CheckForCustomFilter(obj) {
		if (obj != null && obj.selectedIndex == obj.options.length - 1) {
			OpenWindow('<%= this.ResolveUrl("~/Projects/ProjectsByBusinessScoresPopUp.aspx") %>', 350, 350, false);
		}
	}

	function EnableApplyButton() {
		var but = document.getElementById('<%=btnApplyFilter.ClientID%>');
		if (but != null) {
			but.style.display = 'block';
		}
		var im = document.getElementById("processImage");
		if (im != null) {
			im.style.display = "none";
		}
	}
	//]]>
</script>

<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top: 0px">
	<tr>
		<td class="ms-toolbar">
			<ibn:BlockHeader ID="secHeader" runat="server" />
		</td>
	</tr>
	<tr>
		<td class="ibn-navline ibn-alternating text" style="padding-top: 8px" valign="top">
			<table cellpadding="5px">
				<tr>
					<td class="text" align="right">
						<b>
							<%=LocRM.GetString("tPrjGroup")%>:</b>
						<asp:DropDownList ID="ddPrjGroup" runat="server" Width="190px" CssClass="text" onchange="javascript:CheckForCustomFilter(this);">
						</asp:DropDownList>
					</td>
					<td width="20px">
					</td>
					<td class="text" align="right">
						<b>
							<%=LocRM.GetString("tScale")%>:</b>
						<asp:DropDownList runat="server" ID="ddFinanceType" Width="190px">
						</asp:DropDownList>
					</td>
					<td width="20px">
					</td>
				</tr>
				<tr>
					<td align="right" class="text">
						<b>
							<%=LocRM.GetString("tGroupBy")%>:</b>
						<asp:DropDownList runat="server" ID="ddGroupBy" Width="190px">
						</asp:DropDownList>
					</td>
					<td width="20px">
					</td>
					<td>
					</td>
					<td width="20px">
					</td>
				</tr>
				<tr>
					<td class="text" align="right">
						<b>
							<%=LocRM.GetString("tFrom")%>:</b>
						<asp:TextBox runat="server" ID="tbFromYear" Width="190px"></asp:TextBox>
					</td>
					<td width="20px">
						<asp:RequiredFieldValidator runat="server" ID="rfvFromYear" Display="Dynamic" ControlToValidate="tbFromYear" ErrorMessage="*"></asp:RequiredFieldValidator>
						<asp:CompareValidator runat="server" ID="cvFromYear" Display="Dynamic" ControlToValidate="tbFromYear" Type="Integer" Operator="GreaterThan" ValueToCompare="1995" ErrorMessage="*"></asp:CompareValidator>
						<asp:CompareValidator runat="server" ID="Comparevalidator1" Display="Dynamic" ControlToValidate="tbFromYear" Type="Integer" Operator="LessThan" ValueToCompare="2050" ErrorMessage="*"></asp:CompareValidator>
						<asp:RegularExpressionValidator runat="server" ID="revFromYear" Display="Dynamic" ControlToValidate="tbFromYear" ErrorMessage="*" ValidationExpression="\d+"></asp:RegularExpressionValidator>
					</td>
					<td class="text" align="right">
						<b>
							<%=LocRM.GetString("tTo")%>:</b>
						<asp:TextBox runat="server" ID="tbToYear" Width="190px"></asp:TextBox>
					</td>
					<td width="20px">
						<asp:RequiredFieldValidator runat="server" ID="rfvToYear" Display="Dynamic" ControlToValidate="tbToYear" ErrorMessage="*"></asp:RequiredFieldValidator>
						<asp:CompareValidator runat="server" ID="cvToYear" Display="Dynamic" ControlToValidate="tbToYear" Type="Integer" Operator="GreaterThanEqual" ControlToCompare="tbFromYear" ErrorMessage="*"></asp:CompareValidator>
						<asp:CompareValidator runat="server" ID="cvToYear1" Display="Dynamic" ControlToValidate="tbToYear" Type="Integer" Operator="LessThan" ValueToCompare="2050" ErrorMessage="*"></asp:CompareValidator>
						<asp:RegularExpressionValidator runat="server" ID="revToYear" Display="Dynamic" ControlToValidate="tbToYear" ErrorMessage="*" ValidationExpression="\d+"></asp:RegularExpressionValidator>
					</td>
				</tr>
				<tr>
					<td class="text" align="right">
						<b>
							<%=LocRM.GetString("tCompare")%>:</b>
						<asp:DropDownList runat="server" ID="ddBasePlan1" Width="190px">
						</asp:DropDownList>
					</td>
					<td class="text" align="left">
						&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;/
					</td>
					<td align="right" class="text">
						<asp:DropDownList runat="server" ID="ddBasePlan2" Width="190px">
						</asp:DropDownList>
					</td>
					<td>
					</td>
				</tr>
				<tr>
					<td class="text" align="right" colspan="3">
						<btn:IMButton style="display: none; width: 110px" class="text" runat="server" ID="btnApplyFilter">
						</btn:IMButton>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td>
			<img alt="Loading..." src='<%=ResolveUrl("~/Images/IbnFramework/loading_rss.gif")%>' style="position: absolute; left: 50%; top: 50%; display: block; z-index: 30;" id="processImage" />
			<div id="gridbox" class="text" style="width: 100%; height: 450px; background-color: white; overflow: hidden">
			</div>
		</td>
	</tr>
</table>
