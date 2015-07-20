<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EntityImport.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.MetaUIEntity.Modules.EntityImport" %>
<%@ Register TagPrefix="cc1" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<%@ Register TagPrefix="frm" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<link rel="Stylesheet" type="text/css" href='<%=this.ResolveClientUrl("~/styles/IbnFramework/dialog.css") %>' />
<script type="text/javascript">
	function resizeTable()
	{
	   var obj = document.getElementById('stepDiv');
	   var toolbarRow = document.getElementById('tableHeader');

	   var intHeight = 0;
	   if (typeof(window.innerWidth) == "number")
	   {
		  intHeight = window.innerHeight;
	   }
	   else if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight))
	   {
		  intHeight = document.documentElement.clientHeight;
	   }
	   else if (document.body && (document.body.clientWidth || document.body.clientHeight))
	   {
		  intHeight = document.body.clientHeight;
	   }
	   
		if(obj && toolbarRow)
			obj.style.height = (intHeight - toolbarRow.offsetHeight - 50) + "px";
	} 
	window.onresize=resizeTable; 
	window.onload=resizeTable; 
	
</script>
<style type="text/css">
	table.padTable5 tbody tr td
	{
		padding: 5px ! important;
	}
	table.padTable3 tbody tr td
	{
		padding: 3px ! important;
	}
	table.padTable2 tbody tr td
	{
		padding: 2px ! important;
	}
</style>
<asp:Wizard runat="server" ID="ucWizard" Width="100%" Height="100%" CssClass="text">
	<HeaderStyle VerticalAlign="Top" />
	<CancelButtonStyle Width="90px" />
	<NavigationButtonStyle Width="90px" />
	<FinishCompleteButtonStyle Width="90px" />
	<StepStyle CssClass="wizardStep" VerticalAlign="top" />
	<NavigationStyle CssClass="wizardFooter" />
	<HeaderTemplate>
		<table width="100%" class="borderBottom" id="tableHeader">
			<tr>
				<td class="topHeader headerPadding" valign="top">
					<%=HeaderText%>
				</td>
			</tr>
			<tr>
				<td valign="top" class="subHeaderPadding">
					<table cellspacing="0" cellpadding="0" width="100%" border="0">
						<tr>
							<td class="topSubHeader">
								<%=SubHeaderText%>
							</td>
							<td class="step" align="right">
								<%=StepText%>
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</HeaderTemplate>
	<WizardSteps>
		<asp:WizardStep ID="step1" runat="server" StepType="Start">
			<div id="stepDiv">
			<table width="100%" class="text">
				<tr>
					<td valign="top" align="center" width="50%">
						<div style="width:70%;">
						<frm:BlockHeaderLight ID="lgdSourceType" runat="server" HeaderCssClass="ibn-toolbar-light" />
						<table width="100%" class="ibn-stylebox-light padTable5">
							<tr>
								<td width="50%" valign="top">
									<asp:RadioButtonList id="rbSourceType" Runat="server" CssClass="padTable2"></asp:RadioButtonList>
								</td>
								<td valign="top" align="center" width="60">
									<img alt="" src='<% =ResolveClientUrl("~/layouts/images/quicktip.gif") %>' border="0" />
								</td>
								<td class="text" style="PADDING-RIGHT: 15px" valign="top"><asp:Literal ID="Literal1" runat="server" Text="<%$Resources: IbnFramework.Common, tSelectSource%>" /></td>
							</tr>
						</table>
						<br />
						<frm:BlockHeaderLight ID="lgdFile" runat="server" HeaderCssClass="ibn-toolbar-light" />
						<table width="100%" class="ibn-stylebox-light padTable5" height="50px" cellpadding="5">
							<tr>
								<td width="50%">
									<cc1:McHtmlInputFile id="fSourceFile" runat="server" style="width:350px;" Size="40" CssClass="text"></cc1:McHtmlInputFile>
									<asp:RequiredFieldValidator ID="rfFile" runat="server" ControlToValidate="fSourceFile" Display="Static" ErrorMessage="*" CssClass="text"></asp:RequiredFieldValidator>
								</td>
							</tr>
						</table>
						</div>
					</td>
				</tr>
			</table>
			</div>
		</asp:WizardStep>
		<asp:WizardStep ID="step3" runat="server" StepType="Finish">
			<div id="stepDiv">
			<div class="ibn-propertysheet" style="padding-bottom:10px;" id="divCSV" runat="server">
			<frm:BlockHeaderLight ID="bhCSV" runat="server" HeaderCssClass="ibn-toolbar-light" />
			<table width="100%" class="ibn-stylebox-light padTable5">
				<tr>
					<td><asp:Literal ID="Literal2" runat="server" Text="<%$Resources: IbnFramework.Common, tDelimeter%>" />:&nbsp;&nbsp;<asp:DropDownList ID="ddDelimeter" runat="server" Width="80px" AutoPostBack="true"></asp:DropDownList></td>
					<td><asp:Literal ID="Literal3" runat="server" Text="<%$Resources: IbnFramework.Common, tTextQualifier%>" />:&nbsp;&nbsp;<asp:DropDownList ID="ddTextQualifier" runat="server" Width="55px" AutoPostBack="true"></asp:DropDownList></td>
					<td><asp:Literal ID="Literal4" runat="server" Text="<%$Resources: IbnFramework.Common, tEncoding%>" />:&nbsp;&nbsp;<asp:DropDownList ID="ddEncoding" runat="server" Width="125px" AutoPostBack="true"></asp:DropDownList></td>
					<td></td>
				</tr>	
			</table>
			</div>
			<frm:BlockHeaderLight ID="bhMapping" runat="server" HeaderCssClass="ibn-toolbar-light" />
			<table width="100%" class="ibn-stylebox-light" cellpadding="5">
				<tr>
					<td>
						<div style="WIDTH: 100%; OVERFLOW-Y: auto;OVERFLOW: auto; HEIGHT:<%= (divCSV.Visible)? "230px" : "310px"%> ; PADDING-BOTTOM:20px;">
						<asp:DataGrid ID="dgMapping" runat="server" AllowPaging="false" AllowSorting="false"
							AutoGenerateColumns="false" GridLines="None" Width="97%">
							<Columns>
								<asp:BoundColumn DataField="metaFieldName" Visible="false"></asp:BoundColumn>
								<asp:BoundColumn DataField="metaField" 
									HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2"
									HeaderText="<%$Resources: IbnFramework.Common, tMetaField%>">
								</asp:BoundColumn>
								<asp:TemplateColumn HeaderText="<%$Resources: IbnFramework.Common, tAvailable%>">
									<HeaderStyle CssClass="ibn-vh2" />
									<ItemStyle CssClass="ibn-vb2" Width="190px" />
									<ItemTemplate>
										<asp:UpdatePanel ID="upColumns" runat="server" UpdateMode="Conditional">
											<ContentTemplate>
												<asp:DropDownList ID="ddColumns" AutoPostBack="true" OnSelectedIndexChanged="ddi_SelectedIndexChanged" CssClass="text" runat="server" Width="170px"></asp:DropDownList>	
											</ContentTemplate>
										</asp:UpdatePanel>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn>
									<HeaderStyle CssClass="ibn-vh2" />
									<ItemStyle CssClass="ibn-vb2" Width="170px" />
									<ItemTemplate>
										<asp:UpdatePanel ID="upValues" runat="server" UpdateMode="Conditional">
											<ContentTemplate>
												<asp:TextBox ID="tbColumn" runat="server" CssClass="text" Width="150px" Visible="false"></asp:TextBox>
												<asp:DropDownList ID="ddColumn" runat="server" CssClass="text" Width="150px" Visible="false"></asp:DropDownList>
											</ContentTemplate>
										</asp:UpdatePanel>
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
						</asp:DataGrid>
						</div>
					</td>
				</tr>
			</table>
			</div>
		</asp:WizardStep>
		<asp:WizardStep ID="step4" runat="server" StepType="Complete">
			<table width="100%" cellpadding="0" cellspacing="0" border="0" id="tableHeader"
				style="border-collapse:collapse; width:100%;">
				<tr>
					<td class="topHeader headerPadding" valign="top">
						<%=HeaderText%>
					</td>
				</tr>
				<tr>
					<td valign="top" class="subHeaderPadding borderBottom">
						<table cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td class="topSubHeader">
									<%=SubHeaderText%>
								</td>
								<td class="step" align="right">
									<%=StepText%>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
			<div id="stepDiv" class="wizardStep" style="vertical-align: top;">
				<table style="padding:50px;" cellpadding="5" cellspacing="15" class="ibn-propertysheet">
					<tr>
						<td><img alt="" border="0" src='<%=this.Page.ResolveClientUrl("~/layouts/images/check.gif") %>' /></td>
						<td valign="top">
							<asp:Label ID="lblResult" CssClass="text" runat="server"></asp:Label>
						</td>
					</tr>
				</table>
			</div>
			<table width="100%" cellpadding="0" cellspacing="0" border="0"
				style="border-collapse:collapse; width:100%;">
				<tr>
					<td class="wizardCompleteStepFooter" align="right">
						<asp:Button ID="btnClose" runat="server" Text="Close" Width="90px" OnClick="btnClose_Click" />
					</td>
				</tr>
			</table>
		</asp:WizardStep>
	</WizardSteps>
</asp:Wizard>
<asp:HiddenField ID="hdnFilePath" Value="0" runat="server" />
<asp:LinkButton ID="lbErrorLog" runat="server" Visible="false"></asp:LinkButton>