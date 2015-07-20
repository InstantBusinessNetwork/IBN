<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CreateTaskTodoGrid.ascx.cs" Inherits="Mediachase.UI.Web.Apps.ProjectManagement.Modules.CreateTaskTodoGrid" %>
<%@ Register TagPrefix="ibn" Assembly="Mediachase.Ibn.Web.UI.WebControls" Namespace="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="DTCC" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>

<style type="text/css">
	.radioList
	{
		padding: 5px;
	}
	.radioList label
	{
		vertical-align:middle;
		padding-left:3px;
	}
	.radioList input
	{
		margin-left: 15px;
		vertical-align:middle;
	}
	
	.dataTable
	{
		background-color:#E1ECFC;
	}
	.dataTable td, dataTable th
	{
		padding:5px; 
		margin:1px;
		background-color:White;
	}
	.dataTable td table td
	{
		padding:0px;
	}
</style>
<asp:RadioButtonList runat="server" ID="TypeList" RepeatLayout="Table" RepeatDirection="Horizontal" CssClass="radioList">
</asp:RadioButtonList>
<table width="100%">
	<tr>
		<td style="padding:5px;">
			<table width="100%" border="0" class="dataTable" cellpadding="1" cellspacing="1">
				<tr>
					<th><asp:Literal runat="server" ID="TitleLiteral" Text="<%$ Resources:IbnFramework.Task, Title %>"></asp:Literal></th>
					<th><asp:Literal runat="server" ID="StartDateLiteral" Text="<%$ Resources:IbnFramework.Task, StartDate %>"></asp:Literal></th>
					<th><asp:Literal runat="server" ID="FinishDateLiteral" Text="<%$ Resources:IbnFramework.Task, FinishDate %>"></asp:Literal></th>
					<th><asp:Literal runat="server" ID="PriorityLiteral" Text="<%$ Resources:IbnFramework.Task, Priority %>"></asp:Literal></th>
					<th><asp:Literal runat="server" ID="ResourcesLiteral" Text="<%$ Resources:IbnFramework.Task, Resources %>"></asp:Literal></th>
				</tr>
				<tr>
					<td>
						<asp:TextBox runat="server" ID="Title1" Width="150"></asp:TextBox>
					</td>
					<td>
						<ibn:DTCC ID="StartDate1" runat="server" ShowImageButton="false" DateWidth="70px" ShowTime="true" TimeWidth="40px" />
					</td>
					<td>
						<ibn:DTCC ID="FinishDate1" runat="server" ShowImageButton="false" DateWidth="70px" ShowTime="true" TimeWidth="40px" />
					</td>
					<td>
						<asp:DropDownList runat="server" ID="PriorityList1"></asp:DropDownList>
					</td>
					<td>
						<asp:DropDownList runat="server" ID="ResourcesList1"></asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td>
						<asp:TextBox runat="server" ID="Title2" Width="150"></asp:TextBox>
					</td>
					<td>
						<ibn:DTCC ID="StartDate2" runat="server" ShowImageButton="false" DateWidth="70px" ShowTime="true" TimeWidth="40px" />
					</td>
					<td>
						<ibn:DTCC ID="FinishDate2" runat="server" ShowImageButton="false" DateWidth="70px" ShowTime="true" TimeWidth="40px" />
					</td>
					<td>
						<asp:DropDownList runat="server" ID="PriorityList2"></asp:DropDownList>
					</td>
					<td>
						<asp:DropDownList runat="server" ID="ResourcesList2"></asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td>
						<asp:TextBox runat="server" ID="Title3" Width="150"></asp:TextBox>
					</td>
					<td>
						<ibn:DTCC ID="StartDate3" runat="server" ShowImageButton="false" DateWidth="70px" ShowTime="true" TimeWidth="40px" />
					</td>
					<td>
						<ibn:DTCC ID="FinishDate3" runat="server" ShowImageButton="false" DateWidth="70px" ShowTime="true" TimeWidth="40px" />
					</td>
					<td>
						<asp:DropDownList runat="server" ID="PriorityList3"></asp:DropDownList>
					</td>
					<td>
						<asp:DropDownList runat="server" ID="ResourcesList3"></asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td>
						<asp:TextBox runat="server" ID="Title4" Width="150"></asp:TextBox>
					</td>
					<td>
						<ibn:DTCC ID="StartDate4" runat="server" ShowImageButton="false" DateWidth="70px" ShowTime="true" TimeWidth="40px" />
					</td>
					<td>
						<ibn:DTCC ID="FinishDate4" runat="server" ShowImageButton="false" DateWidth="70px" ShowTime="true" TimeWidth="40px" />
					</td>
					<td>
						<asp:DropDownList runat="server" ID="PriorityList4"></asp:DropDownList>
					</td>
					<td>
						<asp:DropDownList runat="server" ID="ResourcesList4"></asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td>
						<asp:TextBox runat="server" ID="Title5" Width="150"></asp:TextBox>
					</td>
					<td>
						<ibn:DTCC ID="StartDate5" runat="server" ShowImageButton="false" DateWidth="70px" ShowTime="true" TimeWidth="40px" />
					</td>
					<td>
						<ibn:DTCC ID="FinishDate5" runat="server" ShowImageButton="false" DateWidth="70px" ShowTime="true" TimeWidth="40px" />
					</td>
					<td>
						<asp:DropDownList runat="server" ID="PriorityList5"></asp:DropDownList>
					</td>
					<td>
						<asp:DropDownList runat="server" ID="ResourcesList5"></asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td>
						<asp:TextBox runat="server" ID="Title6" Width="150"></asp:TextBox>
					</td>
					<td>
						<ibn:DTCC ID="StartDate6" runat="server" ShowImageButton="false" DateWidth="70px" ShowTime="true" TimeWidth="40px" />
					</td>
					<td>
						<ibn:DTCC ID="FinishDate6" runat="server" ShowImageButton="false" DateWidth="70px" ShowTime="true" TimeWidth="40px" />
					</td>
					<td>
						<asp:DropDownList runat="server" ID="PriorityList6"></asp:DropDownList>
					</td>
					<td>
						<asp:DropDownList runat="server" ID="ResourcesList6"></asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td>
						<asp:TextBox runat="server" ID="Title7" Width="150"></asp:TextBox>
					</td>
					<td>
						<ibn:DTCC ID="StartDate7" runat="server" ShowImageButton="false" DateWidth="70px" ShowTime="true" TimeWidth="40px" />
					</td>
					<td>
						<ibn:DTCC ID="FinishDate7" runat="server" ShowImageButton="false" DateWidth="70px" ShowTime="true" TimeWidth="40px" />
					</td>
					<td>
						<asp:DropDownList runat="server" ID="PriorityList7"></asp:DropDownList>
					</td>
					<td>
						<asp:DropDownList runat="server" ID="ResourcesList7"></asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td>
						<asp:TextBox runat="server" ID="Title8" Width="150"></asp:TextBox>
					</td>
					<td>
						<ibn:DTCC ID="StartDate8" runat="server" ShowImageButton="false" DateWidth="70px" ShowTime="true" TimeWidth="40px" />
					</td>
					<td>
						<ibn:DTCC ID="FinishDate8" runat="server" ShowImageButton="false" DateWidth="70px" ShowTime="true" TimeWidth="40px" />
					</td>
					<td>
						<asp:DropDownList runat="server" ID="PriorityList8"></asp:DropDownList>
					</td>
					<td>
						<asp:DropDownList runat="server" ID="ResourcesList8"></asp:DropDownList>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td style="text-align:right; padding-right:30px;">
			<ibn:IMButton id="btnSave" runat="server" Text="Save" style="width:110px;"></ibn:IMButton>&nbsp;
			<ibn:IMButton id="btnCancel" runat="server" Text="Cancel" style="width:110px"></ibn:IMButton>
		</td>
	</tr>
</table>