<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.TimeControl" Codebehind="TimeControl.ascx.cs" %>
<asp:dropdownlist id="ddlHour" runat="server" Width="40px" onchange='<%# "DDSelect" + ddlHour.ClientID +"(this)"%>'></asp:dropdownlist><asp:Label id="lblPoints" runat="server" > : </asp:Label><asp:dropdownlist id="ddlMinute" runat="server" CssClass="text" Width="40px"></asp:dropdownlist>
-
<asp:dropdownlist id="ddlHour1" runat="server" Width="40px" onchange='<%# "DDSelect" + ddlHour.ClientID +"(this)"%>'></asp:dropdownlist><asp:Label id="Label1" runat="server"> : </asp:Label><asp:dropdownlist id="ddlMinute1" runat="server" CssClass="text" Width="40px"></asp:dropdownlist>
&nbsp;&nbsp;
<input type="button" class="text" value='<%=LocRM.GetString("Clear") %>' onclick='<%= "DDClear" + ddlHour.ClientID +"(this)"%>'/>&nbsp;&nbsp;<asp:CustomValidator id="cvCheckSelection" runat="server" ErrorMessage="*" EnableClientScript="False"></asp:CustomValidator>


<script language="javascript">
	function DDSelect<%=ddlHour.ClientID %>(DDList)
	{
		ddh = document.forms[0].<%=ddlHour.ClientID %>;
		ddh1 = document.forms[0].<%=ddlHour1.ClientID %>;
		ddm = document.forms[0].<%=ddlMinute.ClientID %>;
		ddm1 = document.forms[0].<%=ddlMinute1.ClientID %>;
	
		
		if (DDList.id == ddh.id)
		{
			if (ddm.selectedIndex == 0 && DDList.selectedIndex>0)
				ddm.selectedIndex = 1;
		}
		
		if (DDList.id == ddh1.id)
		{
			if (ddm1.selectedIndex == 0 && DDList.selectedIndex>0)
				ddm1.selectedIndex = 1;
		}
	}
	
	function DDClear<%=ddlHour.ClientID %>(DDList)
	{
		ddh = document.forms[0].<%=ddlHour.ClientID %>.selectedIndex = 0;
		ddh1 = document.forms[0].<%=ddlHour1.ClientID %>.selectedIndex = 0;
		ddm = document.forms[0].<%=ddlMinute.ClientID %>.selectedIndex = 0;
		ddm1 = document.forms[0].<%=ddlMinute1.ClientID %>.selectedIndex = 0;
	}
</script>
