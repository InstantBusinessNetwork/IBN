<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExportImportWizardNew.ascx.cs" Inherits="Mediachase.UI.Web.Projects.Modules.ExportImportWizardNew" %>
<%@ Register TagPrefix="cc1" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<link rel="Stylesheet" type="text/css" href='<%=this.ResolveClientUrl("~/styles/IbnFramework/dialog.css") %>' />
<script type="text/javascript">

function OpenWindow(query,w,h,scroll)
{
	var l = (screen.width - w) / 2;
	var t = (screen.height - h) / 2;
	
	winprops = 'resizable=1, height='+h+',width='+w+',top='+t+',left='+l;
	if (scroll) winprops+=',scrollbars=1';
	var f = window.open(query, "_blank", winprops);
}

function removeDupValue(toRemoveValue, excludeId)
{
  var pattern = "ddProjectTeam";
  var allElem =document.getElementsByTagName("select");
  for(var i = 0; i < allElem.length; i++)
  {
    if(allElem[i].id.indexOf(pattern) > -1 && allElem[i].id != excludeId)
	{
		var options = allElem[i].options;
		var notSetOption;
		var removeOptionId;
		for(y = 0; y < options.length; y++)
		{
			var option = options[y];
			
			if(option.value == -1)
			{
				notSetOption = option;
			}
			
			if(option.value == toRemoveValue)
			{
			  removeOptionId = y;
			}		
		}
	  
		if(removeOptionId)
		{
		  if(options[removeOptionId].selected)
			notSetOption.selected = true;
		}
	}
  }
  
}


function canceldiff()
{
	window.close();
}

</script>
<style type="text/css">
	table.mylist input {
      float: left;
   }
   table.mylist label {
      float: left;
      margin-top:3px;
   }
</style>
<asp:Wizard runat="server" ID="EIWizard" Width="100%" Height="100%" 
	CssClass="text" DisplaySideBar="false" StartNextButtonText='<%$Resources:IbnFramework.ImportProjectWizard, tNext %>'
	StepNextButtonText='<%$Resources:IbnFramework.ImportProjectWizard, tNext %>'
	StepPreviousButtonText='<%$Resources:IbnFramework.ImportProjectWizard, tBack %>'
	FinishCompleteButtonText='<%$Resources:IbnFramework.ImportProjectWizard, tFinish %>'
	FinishPreviousButtonText='<%$Resources:IbnFramework.ImportProjectWizard, tBack %>' ActiveStepIndex="0" >
	<HeaderStyle VerticalAlign="Top" />
	<CancelButtonStyle Width="90px" />
	<NavigationButtonStyle Width="90px" />
	<FinishCompleteButtonStyle Width="90px" />
	<StepStyle CssClass="wizardStep" VerticalAlign="Top" />
	<NavigationStyle CssClass="wizardFooter" />
	<HeaderTemplate>
		<table width="100%" class="borderBottom">
			<tr>
				<td class="topHeader headerPadding" valign="top">
					<%=MainHeaderText%>
				</td>
			</tr>
			<tr>
				<td valign="top" class="subHeaderPadding">
					<table cellspacing="0" cellpadding="0" width="100%" border="0">
						<tr>
							<td class="topSubHeader">
								<%=SubHeaderText%>
							<td class="step" align="right">
								<%=StepHeaderText%>
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</HeaderTemplate>
	
	<WizardSteps>
		<asp:WizardStep runat="server" ID="step1">
			<div style="text-align:center;">
			<asp:RadioButtonList runat="server" ID="rblFirstStep" CssClass="mylist" Width="420px" >
			</asp:RadioButtonList>
			</div>
			<div runat="server" id="divStep1NotSyncronized" style="padding:10px;">
				<asp:Label runat="server" ID="lbNSImport" Text="<%$ Resources:IbnFramework.ImportProjectWizard,tNSImportDecsription %>"></asp:Label>
				<asp:Label runat="server" ID="lbNSExport" Text="<%$ Resources:IbnFramework.ImportProjectWizard,tNSExportDecsription %>"></asp:Label>
			</div>
			<div runat="server" id="divStep1Syncronized" style="padding:10px;">
				<asp:Label runat="server" ID="lbSUpInIBN" Text="<%$ Resources:IbnFramework.ImportProjectWizard,tUpInIBNDescription %>"></asp:Label>
				<asp:Label runat="server" ID="lbSUpInMS" Text="<%$ Resources:IbnFramework.ImportProjectWizard,tUpInMSDescription %>"></asp:Label>
			</div>
		</asp:WizardStep>
		<asp:WizardStep runat="server" ID="step2">
			<div style="padding-left: 10px; padding-right: 10px;" runat="server" id="divStep2Import">
				<br />
				<asp:Literal runat="server" ID="Literal1" Text="<%$ Resources:IbnFramework.ImportProjectWizard,ChooseXmlFile %>"></asp:Literal>:
				<br /><br />
				<cc1:McHtmlInputFile id="mcImportFile" runat="server" MaxLength="-1" Size="-1" Width="450"></cc1:McHtmlInputFile>
				<br />
				<asp:RequiredFieldValidator id="rfCheckEmpty" runat="server" ControlToValidate="mcImportFile" 
					ErrorMessage='<%$ Resources:IbnFramework.ImportProjectWizard,tFileRequired %>'></asp:RequiredFieldValidator>
				<asp:CustomValidator id="cvFileError" runat="server" ErrorMessage="*" Display="Dynamic"></asp:CustomValidator>
			</div>
		</asp:WizardStep>
		<asp:WizardStep runat="server" ID="step3" StepType="Finish">
			<div style="text-align:center;" runat="server" id="divStep3Import">
				<fieldset style="width:500px;">
					<div style="overflow-y: auto; HEIGHT: 270px; ">
						<asp:DataGrid id="dgMembersImport" Runat="server" borderwidth="0px" 
								gridlines="None" cellpadding="2" 
								AutoGenerateColumns="False" Width="100%">
							<ItemStyle CssClass="text"></ItemStyle>
							<HeaderStyle CssClass="text" Font-Bold="true"></HeaderStyle>
							<Columns>
								<asp:BoundColumn DataField="ResourceId" Visible="False"></asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Name">
									<ItemTemplate>
										<%# DataBinder.Eval(Container.DataItem, "ResourceName")%>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText='Code'>
									<ItemTemplate>
										<asp:DropDownList ID="ddProjectTeam" Runat="server" CssClass="text"></asp:DropDownList>
									</ItemTemplate>
									<ItemStyle Width="130px" />
								</asp:TemplateColumn>
								<asp:BoundColumn DataField="PrincipalId" Visible="False"></asp:BoundColumn>
							</Columns>
						</asp:DataGrid>
					</div>
				</fieldset>
			</div>
			<div style="text-align:center;" runat="server" id="divStep3Synchronization">
				<div style="padding:5px;">
					<b><asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:IbnFramework.ImportProjectWizard, tComplicationType %>" />:</b>
					&nbsp;&nbsp;
					<asp:DropDownList ID="ddCType" runat="server" Width="230px" CssClass="text"></asp:DropDownList>
				</div>
				<fieldset style="width:500px;">
					<legend>
						<asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:IbnFramework.ImportProjectWizard, ColHere %>" />
					</legend>
					<div style="overflow-y: auto; height: 200px">
						<asp:DataGrid ID="dgMembers" runat="server" BorderWidth="0px" GridLines="None"
							CellPadding="2" AutoGenerateColumns="False"
							Width="100%">
							<ItemStyle CssClass="text"></ItemStyle>
							<HeaderStyle CssClass="text"></HeaderStyle>
							<Columns>
								<asp:BoundColumn DataField="ResourceId" Visible="False"></asp:BoundColumn>
								<asp:TemplateColumn HeaderText='Name'>
									<ItemTemplate>
										<%# DataBinder.Eval(Container.DataItem, "ResourceName")%>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText='Code'>
									<ItemTemplate>
										<asp:DropDownList ID="ddProjectTeam" runat="server" CssClass="text">
										</asp:DropDownList>
									</ItemTemplate>
									<ItemStyle Width="130px" />
								</asp:TemplateColumn>
								<asp:BoundColumn DataField="PrincipalId" Visible="False"></asp:BoundColumn>
							</Columns>
						</asp:DataGrid>
					</div>
				</fieldset>
			</div>
			<div style="text-align:center;" runat="server" id="divStep3Export" class="ibn-propertysheet">
				<asp:Literal runat="server" ID="l1" Text="<%$ Resources:IbnFramework.ImportProjectWizard, tGetIBNXMLFile1 %>"></asp:Literal>
				
				<%--<a href='<%= "../Projects/GetIBNProjectXML.aspx?ProjectId=" + (ProjectId.ToString()+(IsProjectMSSynchronized ? "&Synchronized=true" : ""))%>'><asp:Literal runat="server" Text="<%$ Resources:IbnFramework.ImportProjectWizard, tHere %>"></asp:Literal></a>--%>
				<asp:Literal runat="server" ID="Literal4" Text="<%$ Resources:IbnFramework.ImportProjectWizard, tGetIBNXMLFile2 %>"></asp:Literal>
			</div>
		</asp:WizardStep>
		<asp:WizardStep runat="server" StepType="Complete" ID="step4" >
			<table width="100%" height="100%">
				<tr>
					<td style="width:40px"></td>
					<td style="width:80px"><IMG src="../layouts/images/check.gif"></td>
					<td>
						<div runat="server" id="divStepCompleteSuccess">
							<div runat="server" id="divImportSuccess">
								<asp:Literal runat="server" ID="ImportSuccessLiteral" Text="<%$ Resources:IbnFramework.ImportProjectWizard, s2Title %>"></asp:Literal>
								<br />
								<br />
								<div style="text-align:right;">
								<asp:Button ID="btnImportSuccessClose" runat="server" Text="<%$ Resources:IbnFramework.ImportProjectWizard, tClose %>" />
								</div>
							</div>
							<div runat="server" id="divExportSuccess" class="ibn-propertysheet">
								<asp:Literal runat="server" ID="Literal6" Text="<%$ Resources:IbnFramework.ImportProjectWizard, tGetIBNXMLFile1 %>"></asp:Literal>
								<a href='<%= "../Projects/GetIBNProjectXML.aspx?ProjectId=" + ProjectId.ToString()+(IsProjectMSSynchronized ? "&Synchronized=true" : "")%>'><asp:Literal ID="Literal7" runat="server" Text="<%$ Resources:IbnFramework.ImportProjectWizard, tHere %>"></asp:Literal></a><asp:Literal runat="server" ID="Literal8" Text="<%$ Resources:IbnFramework.ImportProjectWizard, tGetIBNXMLFile2 %>"></asp:Literal>
								<br />
								<br />
								<div style="text-align:right;">
								<asp:Button ID="Button1" runat="server" Text="<%$ Resources:IbnFramework.ImportProjectWizard, tClose %>" OnClientClick="window.close(); return false;" />
								</div>
							</div>
						</div>
						<div runat="server" id="divStepCompleteFailed" visible="False">
							<asp:Label ID="Label1" runat="server" Text="<%$ Resources:IbnFramework.ImportProjectWizard, tSyncErrorMessage %>" ForeColor="Red"></asp:Label>
							<br />
							<asp:CheckBox runat="server" ID="cbImortAnyway" Text="<%$ Resources:IbnFramework.ImportProjectWizard, tImportAnyway %>"/>
							<br />
							<div style="text-align:right">
								<asp:Button runat="server" ID="btnImportAnyway" Text="<%$ Resources:IbnFramework.ImportProjectWizard, tClose %>" />
							</div>
						</div>
					</td>
					<td style="width:40px"></td>
				</tr>
			</table>
		</asp:WizardStep>
	</WizardSteps>
</asp:Wizard>
<div id='ibn_divWithLoadingRss' style="position: absolute; left: 0px; top: 0px; height: 100%; width: 100%; background-color: White; z-index: 10000;display:none;">
	<div style="left: 40%; top: 40%; height: 30px; width: 200px; position: absolute; z-index: 10001">
		<div style="position: relative;  z-index: 10002">
			<img style="position: absolute; left: 40%; top: 40%; z-index: 10003" src='<%= ResolveClientUrl("~/Images/IbnFramework/loading_rss.gif") %>' border='0' />
		</div>
	</div>
</div>	    
<script type="text/javascript">
	var rbs = null;
	SelectionChanged = function()
	{
		if(rbs!=null && rbs.length>0)
		{
			var syncDiv = document.getElementById('<%=divStep1Syncronized.ClientID%>');
		
			if(!syncDiv)	//update mode
			{
				var lbImp = document.getElementById('<%=lbNSImport.ClientID%>');
				var lbExp = document.getElementById('<%=lbNSExport.ClientID%>');
				for(var i=0; i<2; i++)
				{
					if(rbs[i].checked)
					{
						if(rbs[i].value == "Import")
						{
							if(lbImp!=null) lbImp.style.display = "";
							if(lbExp!=null) lbExp.style.display = "none";
						}
						if(rbs[i].value == "Export")
						{
							if(lbImp!=null) lbImp.style.display = "none";
							if(lbExp!=null) lbExp.style.display = "";
						}
					}
				}
			}
			else//sync mode
			{
				var lbImp = document.getElementById('<%=lbSUpInIBN.ClientID%>');
				var lbExp = document.getElementById('<%=lbSUpInMS.ClientID%>');
				for(var i=0; i<2; i++)
				{
					if(rbs[i].checked)
					{
						if(rbs[i].value == "UpdateInIBN")
						{
							if(lbImp!=null) lbImp.style.display = "";
							if(lbExp!=null) lbExp.style.display = "none";
						}
						if(rbs[i].value == "UpdateInMS")
						{
							if(lbImp!=null) lbImp.style.display = "none";
							if(lbExp!=null) lbExp.style.display = "";
						}
					}
				}
			}
		}
	}
	
	FindButtons = function()
	{
		var inputs = document.getElementsByTagName("input");
		if(inputs!=null && inputs.length>0)
		{
			for(var i=0; i<inputs.length; i++)
			{
				if(inputs[i].type == 'radio') 
				{
					if(rbs == null) rbs = new Array();
					rbs.push(inputs[i]);
					inputs[i].onclick = SelectionChanged;
				}	
			}
		}
	}
	
	window.onload = FindButtons;
	
	MakeHide = function()
	{
		var obj = document.getElementById('ibn_divWithLoadingRss');
		if (obj)
		{
			obj.style.display = '';
		}
	}
</script>