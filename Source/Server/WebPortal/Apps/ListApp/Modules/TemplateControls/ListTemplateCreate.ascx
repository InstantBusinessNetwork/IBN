<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListTemplateCreate.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.ListApp.Modules.TemplateControls.ListTemplateCreate" %>
<link rel="Stylesheet" type="text/css" href='<%=this.ResolveClientUrl("~/styles/IbnFramework/dialog.css") %>' />
<asp:Wizard ID="ucWizard" runat="server" Width="100%" Height="100%" CssClass="text">
	<HeaderStyle VerticalAlign="Top" />
	<CancelButtonStyle Width="90px" />
	<NavigationButtonStyle Width="90px" />
	<FinishCompleteButtonStyle Width="90px" />
	<StepStyle CssClass="wizardStep" VerticalAlign="top" />
	<NavigationStyle CssClass="wizardFooter" />
	<HeaderTemplate>
		<table width="100%" class="borderBottom">
			<tr>
				<td class="topHeader headerPadding" valign="top">
					<%=HeaderText %>
				</td>
			</tr>
			<tr>
				<td valign="top" class="subHeaderPadding">
					<table cellspacing="0" cellpadding="0" width="100%" border="0">
						<tr>
							<td class="topSubHeader">
								<%=SubHeaderText %></td>
							<td class="step" align="right">
								<%=StepText %></td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</HeaderTemplate>
	<WizardSteps>
		<asp:WizardStep ID="step1" runat="server" StepType="Auto">
			<table cellpadding="7" cellspacing="0" width="100%">
				<tr>
					<td><b><asp:Label ID="lblTitle" runat="server"></asp:Label></b></td>
					<td>
						<asp:TextBox ID="tbName" runat="server" CssClass="text" Width="250px"></asp:TextBox>
						<asp:RequiredFieldValidator ID="rfName" ControlToValidate="tbName" Display="Static" ErrorMessage="*" runat="server"></asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr>
					<td></td>
					<td><asp:CheckBox ID="cbWithData" runat="server" CssClass="text" /></td>
				</tr>
			</table>
		</asp:WizardStep> 
		<asp:WizardStep ID="step2" StepType="Finish" runat="server">
			<table width="100%" cellpadding="7" cellspacing="0">
				<tr>
					<td style="width:120px">
						<img alt="" src='<%=this.ResolveClientUrl("~/layouts/images/check.gif") %>' />
					</td>
					<td><asp:Label CssClass="stepFinish" ID="lblText" runat="server"></asp:Label></td>
				</tr>
			</table>
		</asp:WizardStep>
	</WizardSteps>
</asp:Wizard>