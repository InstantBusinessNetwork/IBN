<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Clone.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.TimeTracking.Modules.PublicControls.Clone" %>
<%@ Reference Control="~/Apps/Common/Design/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Apps/Common/Design/BlockHeader.ascx" %>
<%@ register TagPrefix="ibn" namespace="Mediachase.UI.Web.Modules" Assembly="Mediachase.UI.Web" %>
<script language="javascript" type="text/javascript">
	function resizeTable()
	{
		var obj = document.getElementById('mainDiv');
		var buttonsRow = document.getElementById('ButtonsRow');

		var intWidth = 0;
		var intHeight = 0;
		if (typeof(window.innerWidth) == "number") 
		{
			intWidth = window.innerWidth;
			intHeight = window.innerHeight;
		} 
		else if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight)) 
		{
			intWidth = document.documentElement.clientWidth;
			intHeight = document.documentElement.clientHeight;
		} 
		else if (document.body && (document.body.clientWidth || document.body.clientHeight)) 
		{
			intWidth = document.body.clientWidth;
			intHeight = document.body.clientHeight;
		}
		var h = intHeight - buttonsRow.offsetHeight - 2;
		if (h > 0)
			obj.style.height = h + "px";
		obj.style.width = intWidth + "px";
	}
	window.onresize=resizeTable;
	window.onload=resizeTable;
	
	function CheckAll(obj)
	{
		aInputs = document.getElementsByTagName("input");
		for (var i=0; i<aInputs.length; i++)
		{
			oInput = aInputs[i];
			if(oInput.type == "checkbox" && oInput.name.indexOf("chkElement") >= 0)
			{
				oInput.checked = obj.checked;
			}
		}
	}
	
	function CheckChildren(obj, btiId)
	{
		aInputs = document.getElementsByTagName("input");
		for (var i=0; i<aInputs.length; i++)
		{
			oInput = aInputs[i];
			if(oInput.type == "checkbox" && oInput.name.indexOf("chkElement") >= 0 && oInput.parentNode && oInput.parentNode.attributes["bti"] && oInput.parentNode.attributes["bti"].value == btiId)
			{
				oInput.checked = obj.checked;
			}
		}
	}
</script>
<table cellspacing="0" cellpadding="0" border="0" width="100%">
	<tr>
		<td class="ibn-navline">
			<div id="mainDiv" style="height:420px;width:640px;overflow:auto;">
				<div style="height:100%">
					<div style="padding:10px" class="ibn-error" runat="server" id="NoItemsDiv">
						<asp:Literal runat="server" ID="Literal1" Text="<%$Resources : IbnFramework.TimeTracking, EntriesNotFound%>"></asp:Literal>
					</div>
					<asp:DataGrid runat="server" ID="MainGrid" AutoGenerateColumns="false" Width="100%" BorderWidth="1" BorderColor="lightgray"
					CellPadding="1" GridLines="Horizontal" AllowPaging="false" AllowSorting="false" EnableViewState="true" >
					<HeaderStyle CssClass="GridHeader" />
					<Columns>
						<asp:BoundColumn DataField="EntryId" Visible="false"></asp:BoundColumn>
						<asp:BoundColumn DataField="ObjectTypeId" Visible="false"></asp:BoundColumn>
						<asp:BoundColumn DataField="BlockTypeInstanceId" Visible="false"></asp:BoundColumn>
						<asp:TemplateColumn HeaderText="<%$Resources : IbnFramework.TimeTracking, Element%>">
							<HeaderTemplate>
								<asp:checkbox runat="server" id="checkAll" onclick="CheckAll(this);" ToolTip="<%$Resources : IbnFramework.Common, SelectUnselectAll%>" Text="<%$Resources : IbnFramework.TimeTracking, Element%>"></asp:checkbox>
							</HeaderTemplate>
							<ItemTemplate>
								<asp:CheckBox runat="server" ID="chkElement" Text='<%# Eval("ObjectName")%>' bti='<%# Eval("BlockTypeInstanceId")%>'/>
							</ItemTemplate>
							<ItemStyle CssClass="ibn-vb2" />
							<HeaderStyle CssClass="ibn-vh2" />
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="<%$Resources : IbnFramework.TimeTracking, ObjectType%>">
							<ItemTemplate>
								<%# Mediachase.UI.Web.Util.CommonHelper.GetObjectTypeName((int)Eval("ObjectTypeId"))%>
							</ItemTemplate>
							<ItemStyle CssClass="ibn-vb2" />
							<HeaderStyle CssClass="ibn-vh2" />
						</asp:TemplateColumn>
					</Columns>
				</asp:DataGrid>
				</div>
			</div>
		</td>
	</tr>
	<tr id="ButtonsRow">
		<td align="center" style="height:40px;">
			<asp:Button runat="server" ID="AddButton" Text="<%$Resources: IbnFramework.Global, _mc_Add %>" OnClick="AddButton_Click" />
			<asp:Button runat="server" ID="CloseButton" Text="<%$Resources: IbnFramework.GlobalMetaInfo, Close %>" OnClick="CloseButton_Click" style="margin-left:20px;" />
		</td>
	</tr>
</table>

