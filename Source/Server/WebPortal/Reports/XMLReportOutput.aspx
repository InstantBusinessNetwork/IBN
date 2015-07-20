<%@ Reference Control="~/Modules/ReportHeader.ascx" %>
<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Reports.XMLReportOutput" CodeBehind="XMLReportOutput.aspx.cs" %>
<%@ Register TagPrefix="ibn" TagName="up" Src="..\Modules\ReportHeader.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	
	<title><%=sTitle %></title>
</head>
<body>

	<script type="text/javascript">
		//<![CDATA[
			function BeforePrint()
			{
				var coll = document.all;
				if (coll!=null)
				{
					for (var i=0; i<coll.length; i++)
					{
						if (coll[i].Printable == "0")
							coll[i].style.display = "none";
						if (coll[i].Printable == "1")
							coll[i].style.display = "block";
					}
				}
			}

			function AfterPrint()
			{
				var coll = document.all;
				if (coll!=null)
				{
					for (var i=0; i<coll.length; i++)
					{
						if (coll[i].Printable == "0")
							coll[i].style.display = "block";
						if (coll[i].Printable == "1")
							coll[i].style.display = "none";
					}
				}
			}

			if (browseris.ie5up)
			{
				window.onbeforeprint = BeforePrint;
				window.onafterprint = AfterPrint;	
			}

			function collapse_expand(obj)
			{
				if (obj.parentNode.parentNode.cells[1].firstChild.style.display!=null &&
					obj.parentNode.parentNode.cells[1].firstChild.style.display=="none")
				{
					obj.parentNode.parentNode.cells[1].firstChild.style.display="";
					obj.innerHTML = "<img src='../layouts/images/minus.gif' border='0' />";
				}
				else
				{
					obj.parentNode.parentNode.cells[1].firstChild.style.display="none";
					obj.innerHTML = "<img src='../layouts/images/plus.gif' border='0' />";
				}
			}

			function ShowHideList(curObj)
			{
				obj = document.getElementById('divSaveTemplate');
				if (obj.style.display == "none")
				{
					openMenu(curObj);
				}
				else
					closeMenu();
			}

			function closeMenu()
			{
				getMenu('divSaveTemplate').style.display = "none";
			}

			function openMenu(curObj)
			{
				var menu = getMenu('divSaveTemplate');
				off = GetTotalOffset(curObj);
				menu.style.left = (off.Left-30).toString() + "px";
				menu.style.top = (off.Top-45).toString() + "px";
				menu.style.display = "";
				document.forms[0].<%=txtTemplateTitle.ClientID %>.focus();
			}

			function getMenu(s)
			{
				return document.getElementById(s);
			}

			function GetTotalOffset(eSrc)
			{
				this.Top = 0;
				this.Left = 0;
				while (eSrc)
				{
					this.Top += eSrc.offsetTop;
					this.Left += eSrc.offsetLeft;
					eSrc = eSrc.offsetParent;
				}
				return this;
			}
		//]]>
	</script>

	<form id="Form1" method="post" runat="server">
	<ibn:up ID="_header" Title='XML Report' runat="server"></ibn:up>
	<table width="100%" cellspacing="0" cellpadding="0" border="0">
		<tr height="30px" printable="0">
			<td align="right" style="border-bottom: 1px solid #ffd275;">
				<input id="btnSaveVis" runat="server" type="button" class="text" style="width: 130px"
					onclick='javascript:ShowHideList(this);' />&nbsp;&nbsp;
				<input id="btnPrint" type="button" style="width: 110px;" value='<% =LocRM.GetString("Print")%>'
					onclick="javascript:window.print();" />&nbsp;&nbsp;
				<asp:Button ID="btnExcel" runat="server" CssClass="text" Width="110px" OnClick="btnExcel_Click">
				</asp:Button>&nbsp;&nbsp;
				<asp:Button ID="btnXML" runat="server" CssClass="text" Width="110px" OnClick="btnXML_Click">
				</asp:Button>
			</td>
		</tr>
		<tr>
			<td>
				<asp:Label ID="lblXML" runat="server" EnableViewState="false"></asp:Label>
			</td>
		</tr>
	</table>
	<div id="divSaveTemplate" style="position: absolute; top: 30px; left: 100px; width: 225px;
		z-index: 255; padding: 5px; display: none; border: 1px solid #95b7f3;" class="ibn-rtetoolbarmenu ibn-propertysheet ibn-selectedtitle">
		<fieldset style="height: 135px; margin: 0; padding: 2px">
			<legend class="text ibn-legend-default" id="lgdSaveTemplate" runat="server"></legend>
			<table cellpadding="3" cellspacing="0" border="0" width="100%">
				<tr>
					<td class="text">
						<b>
							<%= LocRM.GetString("tTemplateTitle")%>:</b>
					</td>
				</tr>
				<tr>
					<td>
						<asp:TextBox ID="txtTemplateTitle" runat="server" Width="195px" CssClass="text"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td>
						<asp:CheckBox ID="cbSaveResult" Checked="True" runat="server" CssClass="text"></asp:CheckBox>
					</td>
				</tr>
				<tr>
					<td>
						<asp:CheckBox ID="cbOnlyForMe" runat="server" CssClass="text"></asp:CheckBox>
					</td>
				</tr>
				<tr valign="bottom">
					<td style="padding-left: 40px">
						<asp:Button ID="btnSave" runat="server" CssClass="text" Width="50px"></asp:Button>&nbsp;
						<input type="button" class="text" value='<%=LocRM.GetString("tClose")%>' style="width: 50px"
							onclick='javascript:closeMenu();' />
					</td>
				</tr>
			</table>
		</fieldset>
	</div>
	</form>
</body>
</html>
