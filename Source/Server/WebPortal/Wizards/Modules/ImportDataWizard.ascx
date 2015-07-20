<%@ Reference Control="~/Wizards/Modules/WizardTemplate.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Wizards.Modules.ImportDataWizard" Codebehind="ImportDataWizard.ascx.cs" %>
<%@ Register TagPrefix="mc" Namespace="Mediachase.FileUploader.Web.UI" Assembly = "Mediachase.FileUploader" %>
<style type="text/css">
	.hClass { 
		font-family: verdana; 
		color: #808080;
		text-align: left; 
		text-decoration: none; 
		font-weight: bold; 
		vertical-align: top;
		padding:3px 0;}
	.bClass { 
		font-family: verdana; 
		text-decoration: none; 
		font-weight: normal; 
		vertical-align: top;
		border-top: 1px solid #9e9e9e;
		padding:3px 0;}
</style>
<asp:panel id="step2" Runat="server" Height="100%">
	<BR>
	<table height="60%" width="100%">
		<tr>
			<td vAlign="top" align="middle" width="50%">
				<FIELDSET id="fsSource" style="WIDTH: 70%">
					<LEGEND class="text" id="lgdSourceType" runat="server"></LEGEND>
					<table width="99%">
						<tr>
							<td width="50%" valign=top>
								<asp:RadioButtonList id="rbSourceType" Runat="server" CssClass="text"></asp:RadioButtonList>
							</td>
							<td vAlign="top" align="middle" width="60"><IMG alt="" src="../layouts/images/quicktip.gif" border="0">
							</td>
							<td class="text" style="PADDING-RIGHT: 15px" vAlign="top"><%=LocRM.GetString("imStep2Comments")%></td>
						</tr>
					</table>
				</FIELDSET>
				<FIELDSET id="fsFile" style="WIDTH: 70%">
					<LEGEND class="text" id="lgdFile" runat="server"></LEGEND>
					<table width="99%" height="50px">
						<tr>
							<td width="50%">
								<mc:mchtmlinputfile style="width:350px" id="fSourceFile" class="text" runat="server" />
							</td>
						</tr>
					</table>
				</fieldset>
			</td>
		</tr>
	</table>
</asp:Panel>
<asp:panel id="step4" Runat="server" Height="100%">
<table>
	<tr>
		<td valign=top width="550px">
			<FIELDSET style="HEIGHT: 250px;margin:0;padding:2px">
				<LEGEND class="text ibn-legend-default" id="lgdFields" runat="server"></LEGEND>
				<br>
				<table style="TABLE-LAYOUT:fixed;" Width="440px" CellSpacing=0 cellpadding="3" border="0">
					<tr>
						<td width="21px"></td>
						<td Width="210px" class="hClass"><%=LocRM.GetString("imStep4SourceField")%></td>
						<td runat=server id="tdSecondField" Width="240px" class="hClass"><%=LocRM.GetString("imStep4IBNField")%></td>
						<td class="hClass"></td>
					</tr>
				</table>
				<div style="WIDTH: 540px; OVERFLOW-Y: auto;OVERFLOW: auto; HEIGHT: 230px; PADDING-BOTTOM:20px">
				<asp:datagrid id="grdFields" Runat="server" AutoGenerateColumns="False" AllowSorting="True" 
					AllowPaging=False cellpadding="0" gridlines="Horizontal" CellSpacing="0" 
					borderwidth="0px" Width="501px" ShowHeader="False" style="table-layout:fixed">
					<Columns>
						<asp:TemplateColumn>
							<HeaderStyle Width="21px"></HeaderStyle>
							<ItemStyle Width="21px"></ItemStyle>
							<ItemTemplate>&nbsp;</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn>
							<HeaderStyle CssClass="hClass" Width="210px"></HeaderStyle>
							<ItemStyle CssClass="bClass" Width="210px"></ItemStyle>
							<ItemTemplate>
								<%# DataBinder.Eval(Container.DataItem,"SourceField") %>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:DropDownList ID="ddSFields" CssClass="text" Width="90%" Runat="server">
								</asp:DropDownList>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn>
							<HeaderStyle CssClass="hClass" Width="240px"></HeaderStyle>
							<ItemStyle CssClass="bClass" Width="240px"></ItemStyle>
							<ItemTemplate>
								<a href="#" id="linkTo" runat=server></a>
								<%# DataBinder.Eval(Container.DataItem,"IBNField") %>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn>
							<HeaderStyle HorizontalAlign="Right" Width="51px" CssClass="hClass"></HeaderStyle>
							<ItemStyle Width="51px" CssClass="bClass"></ItemStyle>
							<ItemTemplate>
								<asp:imagebutton id="ibMove" runat="server" borderwidth="0" title='<%# LocRM.GetString("tChange")%>' imageurl="../../layouts/images/edit.gif" commandname="Edit" causesvalidation="False">
								</asp:imagebutton>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:imagebutton id="Imagebutton1" runat="server" borderwidth="0" title='<%# LocRM.GetString("tSave1")%>' imageurl="../../layouts/images/Saveitem.gif" commandname="Update" causesvalidation="True">
								</asp:imagebutton>
								&nbsp;
								<asp:imagebutton id="Imagebutton2" runat="server" borderwidth="0" imageurl="../../layouts/images/cancel.gif" title='<%# LocRM.GetString("tCancel")%>' commandname="Cancel" causesvalidation="False">
								</asp:imagebutton>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:boundcolumn datafield="SourceField" Visible="False"></asp:boundcolumn>
					</Columns>
				</asp:DataGrid>
				</div>
			</fieldset>
		</td>
	</tr>
</table>
</asp:Panel>
<asp:panel id="step5" Runat="server" Height="100%">
<table>
	<tr>
		<td valign=top width="450px">
			<asp:Label ID="lblFirstResult" Runat=server></asp:Label>
		</td>
	</tr>
</table>
</asp:Panel>
<asp:Panel ID="step6" Runat=server Height="100%">
	<asp:Label ID="lblFinalResult" Runat=server></asp:Label>
</asp:Panel>

<script language="javascript">
	function ShowProgress()
	{
		if(document.forms[0].<%=fSourceFile.ClientID %>.value!="")
		{
			var w = 300;
			var h = 140;
			var l = (screen.width - w) / 2;
			var t = (screen.height - h) / 2;
			winprops = 'resizable=0, height='+h+',width='+w+',top='+t+',left='+l;
			var f = window.open('../External/Progress.aspx?ID='+document.forms[0].__MEDIACHASE_FORM_UNIQUEID.value, "_blank", winprops);
		}		
	}
</script>

<input id="pastStep" type="hidden" value="0" runat="server" />
<input id="wwwPath" type="hidden" value="" runat="server" />

