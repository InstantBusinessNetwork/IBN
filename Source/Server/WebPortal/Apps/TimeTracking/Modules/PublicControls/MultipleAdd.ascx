<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MultipleAdd.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.TimeTracking.Modules.PublicControls.MultipleAdd" %>
<%@ Reference Control="~/Apps/Common/Design/BlockHeader.ascx" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Picker" src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Apps/Common/Design/BlockHeader.ascx" %>
<%@ register TagPrefix="mc" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>

<script language="javascript" type="text/javascript">
	function resizeTable()
	{
		var obj = document.getElementById('mainDiv');
		var filterRow = document.getElementById('FilterRow');
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
		var h = intHeight - filterRow.offsetHeight - buttonsRow.offsetHeight - 2;
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
	<tr id="FilterRow">
		<td class="ibn-navline" style="padding-left:5px; padding-top:5px; padding-bottom:5px;">
			<table class="ibn-propertysheet">
				<tr runat="server" id="WeekRow">
					<td style="width:130px;">
						<asp:Literal ID="Literal2" runat="server" Text="<%$Resources : IbnFramework.TimeTracking, Week%>" />:
					</td>
					<td>
						<ibn:Picker AutoPostBack="true" id="DTCWeek" runat="server" SelectedMode="Week" DateCssClass="IbnCalendarText" ShowImageButton="false" DateFormat="d MMM yyyy"></ibn:Picker>
					</td>
				</tr>
				<tr runat="server" id="ProjectRow">
					<td>
						<asp:Literal ID="Literal1" runat="server" Text="<%$Resources : IbnFramework.TimeTracking, Project%>" />:
					</td>
					<td>
						<asp:Label runat="server" ID="NoItemsLabel" CssClass="ibn-alerttext" Text="<%$Resources : IbnFramework.TimeTracking, ProjectsNotFound%>"></asp:Label>
						<mc:IndentedDropDownList ID="BlockInstanceList" AutoPostBack="true" Width="300px" runat="server" OnSelectedIndexChanged="BlockInstanceList_SelectedIndexChanged" />
					</td>
				</tr>
				<tr runat="server" id="UserRow">
					<td>
						<asp:Literal ID="Literal3" runat="server" Text="<%$Resources : IbnFramework.TimeTracking, User%>" />:
					</td>
					<td>
						<asp:DropDownList ID="UserList" Width="300px" runat="server" AutoPostBack="true" OnSelectedIndexChanged="UserList_SelectedIndexChanged"></asp:DropDownList>
					</td>
				</tr>
				<tr id="trWeekInfo" runat="server">
					<td>
						<asp:Literal ID="Literal4" runat="server" Text="<%$Resources : IbnFramework.TimeTracking, Week%>" />:
					</td>
					<td><asp:Label ID="lblWeek" Font-Bold="true" runat="server" CssClass="text"></asp:Label></td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td class="ibn-navline">
			<div id="mainDiv" style="height:370px;overflow:auto;">
				<div style="height:100%">
				<asp:DataGrid runat="server" ID="MainGrid" AutoGenerateColumns="false" Width="100%" BorderWidth="1" BorderColor="lightgray"
					CellPadding="1" GridLines="Horizontal" AllowPaging="false" AllowSorting="false" EnableViewState="true" >
					<HeaderStyle CssClass="GridHeader" />
					<Columns>
						<asp:BoundColumn DataField="ObjectId" Visible="false"></asp:BoundColumn>
						<asp:BoundColumn DataField="ObjectTypeId" Visible="false"></asp:BoundColumn>
						<asp:BoundColumn DataField="BlockTypeInstanceId" Visible="false"></asp:BoundColumn>
						<asp:TemplateColumn HeaderText="<%$Resources : IbnFramework.TimeTracking, Element%>">
							<HeaderTemplate>
								<asp:checkbox runat="server" id="checkAll" onclick="CheckAll(this);" ToolTip="<%$Resources : IbnFramework.Common, SelectUnselectAll%>"></asp:checkbox>
								<asp:Literal ID="Literal3" runat="server" Text="<%$Resources : IbnFramework.TimeTracking, Element%>" />
							</HeaderTemplate>
							<ItemTemplate>
								<asp:CheckBox runat="server" ID="chkElement" Text='<%# GetText(Eval("ObjectName").ToString(), (int)Eval("ObjectTypeId"), (int)Eval("ObjectId"))%>' bti='<%# Eval("BlockTypeInstanceId")%>'/>
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
			<asp:Button runat="server" ID="CloseButton" Text="<%$Resources: IbnFramework.GlobalMetaInfo, Close %>" OnClick="CloseButton_Click" style="margin-left:20px;" CausesValidation="false" />
		</td>
	</tr>
</table>
