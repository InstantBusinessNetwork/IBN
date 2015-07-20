<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Reports.Modules.DateFilter" Codebehind="DateFilter.ascx.cs" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<SCRIPT language="javascript">
	function ChangeModify(obj)
	{
		objTbl = document.getElementById('<%=tableDate.ClientID %>');
		id=obj.value;
		if(id=="9")
		{
			objTbl.style.display = '';
		}
		else
		{
			objTbl.style.display = 'none';
		}
	}
</SCRIPT>
<table border="0" cellspacing="0" cellpadding="2">
	<tr height="35">
		<td runat=server id="Migrated_tdTitle" valign="top" class="text" style="PADDING-TOP:13px">
			<b><asp:Label ID="lblTitle" Runat="server" CssClass="text"></asp:Label>:</b>&nbsp;&nbsp;&nbsp;
		</td>
		<td vAlign="top" style="PADDING-TOP:10px" class="text">
			<SELECT class="text" id="ddPeriod" style="WIDTH: 115px;" onchange="ChangeModify(this);" name="ddPeriod" runat="server"></SELECT>
		</td>
		<td valign="top" class="text">
			<table id="tableDate" cellspacing="0" cellpadding="2" runat="server">
				<tr>
					<td class="text">&nbsp;<B><%=LocRM.GetString("tFrom")%>:</B>&nbsp;</td>
					<td class="text"><mc:Picker ID="dtcFromDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" /></td>
				</tr>
				<tr>
					<td class="text">&nbsp;<B><%=LocRM.GetString("tTo")%>:</B>&nbsp;</td>
					<td vAlign="top" class="text"><mc:Picker ID="dtcToDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" /></td>
					<td vAlign="top" class="text"></td>
				</tr>
			</table>
		</td>
	</tr>
</table>