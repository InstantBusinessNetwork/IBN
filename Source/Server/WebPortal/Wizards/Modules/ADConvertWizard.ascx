<%@ Reference Control="~/Wizards/Modules/WizardTemplate.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Wizards.Modules.ADConvertWizard" CodeBehind="ADConvertWizard.ascx.cs" %>
<%@ Register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="mc" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<style type="text/css">
	.hClass
	{
		font-family: verdana;
		color: #808080;
		text-align: left;
		text-decoration: none;
		font-weight: bold;
		vertical-align: top;
		padding: 3px 0;
	}
	.bClass
	{
		font-family: verdana;
		text-decoration: none;
		font-weight: normal;
		vertical-align: top;
		border-top: 1px solid #9e9e9e;
		padding: 3px 0;
	}
</style>

<script language="javascript">
<!--
	function GroupExistence (sender,args)
	{
		if(((document.forms[0].<%=lbSecurityRoles.ClientID%> != null)&& (document.forms[0].<%=lbSecurityRoles.ClientID%>.selectedIndex>-1)) || ((document.forms[0].<%=lbSelectedGroups.ClientID%> != null)&& (document.forms[0].<%=lbSelectedGroups.ClientID%>.options.length>0)))
		{
			args.IsValid = true;	
			return;
		}
		args.IsValid = false;	
	}
	function SaveGroups()	
	{
		var sControl=document.forms[0].<%=lbSelectedGroups.ClientID%>;
		
		var str="";
		if(sControl != null)
		{
			for(var i=0;i<sControl.options.length;i++)
			{
				str += sControl.options[i].value + ",";
			}
		}
		
		var sControl2=document.forms[0].<%=lbSecurityRoles.ClientID%>;
		
		if(sControl2 != null)
		{
			for(var i=0;i<sControl2.options.length;i++)
			{
				if(sControl2.options[i].selected)
					str += sControl2.options[i].value + ",";
			}
		}
		document.getElementById('<%=iGroups.ClientID%>').value = str;
	}
	function CheckAll(obj)
	{
		aInputs = document.getElementsByTagName("input");
		for (var i=0; i<aInputs.length; i++)
		{
			oInput = aInputs[i];
			if(oInput.type == "checkbox" && oInput.name.indexOf("cbConvert") > 0)
			{
				oInput.checked = obj.checked;
			}
		}
	}
	function CheckAll2(obj)
	{
		aInputs = document.getElementsByTagName("input");
		for (var i=0; i<aInputs.length; i++)
		{
			oInput = aInputs[i];
			if(oInput.type == "checkbox" && oInput.name.indexOf("chkItem") > 0)
			{
				oInput.checked = obj.checked;
			}
		}
	}
	function ChangeAD(obj)
	{
		var inputColl = document.getElementsByTagName("input");
		for(var i=0;i<inputColl.length;i++)
		{
			var objtxt = inputColl[i];
			if(objtxt.id.indexOf("txtADField")>=0)
			{
				var sValue = obj.value;
				if(sValue=="0")
					sValue = "";
				objtxt.value = sValue;
				break;
			}
		}
	}
//-->
</script>

<asp:Panel ID="step0" runat="server" Height="100%">
	<div class="text" style="padding-left: 150px">
		<%=LocRM.GetString("s0TopDivConvert") %></div>
	<br>
	<table height="60%" width="100%">
		<tr>
			<td width="150px" align="center">
				<img height="250" src="../layouts/images/wizard.jpg" width="120" align="absMiddle">
			</td>
			<td valign="top" width="450px">
				<fieldset id="fsSource" style="width: 95%">
					<legend class="text" id="lgdSourceType" runat="server"></legend>
					<table width="99%">
						<tr>
							<td width="50%">
								<asp:RadioButtonList ID="rbSourceObject" runat="server" CssClass="text">
								</asp:RadioButtonList>
							</td>
							<td valign="top" align="middle" width="60">
								<img alt="" src="../layouts/images/quicktip.gif" border="0">
							</td>
							<td class="text" style="padding-right: 15px" valign="top">
								<%=LocRM.GetString("s0ConvertComments") %>
							</td>
						</tr>
					</table>
				</fieldset>
			</td>
		</tr>
	</table>
</asp:Panel>
<asp:Panel ID="step1" runat="server" Height="100%">
	<div class="text" style="padding-left: 150px">
		<%=LocRM.GetString("s1TopDivConvert") %></div>
	<br>
	<table>
		<tr>
			<td width="150px" align="center">
				<img height="250" src="../layouts/images/wizard.jpg" width="120" align="absMiddle">
			</td>
			<td valign="top" width="450px">
				<fieldset style="height: 240px; margin: 0; padding: 2px">
					<legend class="text" id="lgdConnectInf" runat="server"></legend>
					<table id="tblADConnection" style="margin-top: 10px" runat="server" cellspacing="2" cellpadding="4" width="100%" border="0">
						<tr>
							<td width="15px">
							</td>
							<td class="text" width="100">
								<b>
									<%=LocRM.GetString("tLDAPSettings")%>:</b>
							</td>
							<td class="text" width="240">
								<asp:DropDownList ID="ddLDAPSettings" AutoPostBack="True" runat="server" Width="200px">
								</asp:DropDownList>
							</td>
							<td>
							</td>
						</tr>
						<tr>
							<td width="15px">
							</td>
							<td class="text" width="100">
								<b>
									<%=LocRM.GetString("Domain")%>:</b>
							</td>
							<td class="text" width="240">
								<asp:TextBox ID="txtDomain" runat="server" Width="200px"></asp:TextBox>
								<asp:RequiredFieldValidator ID="rfDomain" ControlToValidate="txtDomain" CssClass="text" Display="Dynamic" ErrorMessage="*" runat="server"></asp:RequiredFieldValidator>
							</td>
							<td>
							</td>
						</tr>
						<tr>
							<td width="15px">
							</td>
							<td class="text" width="100">
								<b>
									<%=LocRM.GetString("UserName")%>:</b>
							</td>
							<td class="text" width="240">
								<asp:TextBox ID="txtUserName" runat="server" Width="200px"></asp:TextBox>
								<asp:RequiredFieldValidator ID="rfUserName" ControlToValidate="txtUserName" CssClass="text" Display="Dynamic" ErrorMessage="*" runat="server"></asp:RequiredFieldValidator>
							</td>
							<td>
							</td>
						</tr>
						<tr>
							<td width="15px">
							</td>
							<td class="text" width="100">
								<b>
									<%=LocRM.GetString("tPassword")%>:</b>
							</td>
							<td class="text" width="240">
								<asp:TextBox TextMode="Password" ID="txtPassword" runat="server" Width="200px"></asp:TextBox>
								<asp:RequiredFieldValidator ID="rfPassword" ControlToValidate="txtPassword" CssClass="text" Display="Dynamic" ErrorMessage="*" runat="server"></asp:RequiredFieldValidator>
							</td>
							<td>
							</td>
						</tr>
						<tr>
							<td>
							</td>
							<td colspan="2">
								<asp:CustomValidator ID="cvLogin" runat="server" Display="Dynamic"></asp:CustomValidator>
								<asp:Label ID="lblConnectError" runat="server" CssClass="ibn-alerttext" Visible="False"></asp:Label>
							</td>
							<td>
							</td>
						</tr>
					</table>
					<table id="tblFileConnection" runat="server" cellspacing="2" cellpadding="4" width="100%" border="0">
						<tr height="10px">
							<td>
							</td>
						</tr>
						<tr>
							<td style="padding-left: 10px">
								<%=LocRM.GetString("tSelectFileType")%>:
							</td>
						</tr>
						<tr>
							<td width="50%">
								<asp:RadioButtonList ID="rbSourceType" runat="server" CssClass="text">
								</asp:RadioButtonList>
							</td>
							<td valign="top" align="middle" width="60">
								<img alt="" src="../layouts/images/quicktip.gif" border="0">
							</td>
							<td class="text" style="padding-right: 15px" valign="top">
								<%=LocRM.GetString("step2Comments")%>
							</td>
						</tr>
						<tr>
							<td colspan="3" style="padding-top: 20px; padding-left: 10px">
								<%=LocRM.GetString("tSelectFile")%>:
							</td>
						</tr>
						<tr>
							<td colspan="3" style="padding-left: 10px">
								<mc:McHtmlInputFile Style="width: 350px" ID="fSourceFile" class="text" runat="server" />

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

							</td>
						</tr>
					</table>
				</fieldset>
			</td>
		</tr>
	</table>
</asp:Panel>
<asp:Panel ID="step2" runat="server" Height="100%">
	<table>
		<tr>
			<td width="150px" align="center">
				<img height="250" src="../layouts/images/wizard.jpg" width="120" align="absMiddle">
			</td>
			<td valign="top" width="550px">
				<fieldset style="height: 320px; margin: 0; padding: 2px">
					<legend class="text" id="lgdFields" runat="server"></legend>
					<table id="tblFilter" runat="server" cellspacing="0" cellpadding="10" width="100%">
						<tr>
							<td>
								<b>
									<%=LocRM.GetString("tFilter")%>:</b>
							</td>
							<td>
								<asp:TextBox ID="tbFilter" runat="server" Width="350px"></asp:TextBox>
							</td>
						</tr>
					</table>
					<table style="table-layout: fixed;" width="100%" cellspacing="0" cellpadding="3" border="0">
						<tr>
							<td width="130px" class="hClass">
								<%=LocRM.GetString("IBNField")%>
							</td>
							<td width="320px" class="hClass">
								<%=LocRM.GetString("ADField")%>
							</td>
							<td class="hClass">
							</td>
						</tr>
					</table>
					<div style="width: 529px; overflow-y: auto; overflow: auto; height: 220px; padding-bottom: 20px">
						<asp:DataGrid ID="grdFields" runat="server" AutoGenerateColumns="False" AllowSorting="True" AllowPaging="False" CellPadding="0" GridLines="Horizontal" CellSpacing="0" BorderWidth="0px" Width="501px" ShowHeader="False" Style="table-layout: fixed">
							<Columns>
								<asp:TemplateColumn HeaderText="IBN Field">
									<HeaderStyle CssClass="hClass" Width="130px"></HeaderStyle>
									<ItemStyle CssClass="bClass" Width="130px"></ItemStyle>
									<ItemTemplate>
										<a href="#" id="linkTo" runat="server"></a>
										<%# DataBinder.Eval(Container.DataItem, "IBNFieldDisplay")%>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="AD Field">
									<HeaderStyle CssClass="hClass" Width="320px"></HeaderStyle>
									<ItemStyle CssClass="bClass" Width="320px"></ItemStyle>
									<ItemTemplate>
										<%# DataBinder.Eval(Container.DataItem,"ADField") %>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:DropDownList ID="ddADFields" CssClass="text" Width="160px" runat="server">
										</asp:DropDownList>
										<asp:TextBox ID="txtADField" CssClass="text" Width="150px" runat="server"></asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn>
									<HeaderStyle HorizontalAlign="Right" Width="51px" CssClass="hClass"></HeaderStyle>
									<ItemStyle Width="51px" CssClass="bClass"></ItemStyle>
									<ItemTemplate>
										<asp:ImageButton ID="ibMove" runat="server" BorderWidth="0" title='<%# LocRM.GetString("tChange")%>' ImageUrl="../../layouts/images/edit.gif" CommandName="Edit" CausesValidation="False"></asp:ImageButton>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:ImageButton ID="Imagebutton1" runat="server" BorderWidth="0" title='<%# LocRM.GetString("tSave1")%>' ImageUrl="../../layouts/images/Saveitem.gif" CommandName="Update" CausesValidation="True"></asp:ImageButton>
										&nbsp;
										<asp:ImageButton ID="Imagebutton2" runat="server" BorderWidth="0" ImageUrl="../../layouts/images/cancel.gif" title='<%# LocRM.GetString("tCancel")%>' CommandName="Cancel" CausesValidation="False"></asp:ImageButton>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" DataField="realADField" HeaderText="ID" Visible="False"></asp:BoundColumn>
							</Columns>
						</asp:DataGrid>
					</div>
				</fieldset>
			</td>
		</tr>
	</table>
</asp:Panel>
<asp:Panel ID="step3" runat="server" Height="100%">
	<div class="text" style="padding-left: 20px">
		<%=LocRM.GetString("s3TopDivConvert") %></div>
	<br>
	<table cellspacing="2" cellpadding="4" width="100%" border="0">
		<tr>
			<td>
				<table style="table-layout: fixed;" width="98%" cellspacing="2" cellpadding="2" border="0">
					<tr>
						<td width="30px">
							<nobr><input type="checkbox" class="text" style="WIDTH:16px;" id="chkAll"
					name="chkAll" onclick="CheckAll(this);" />
				<asp:CustomValidator ID="cvAtLeastOne" ErrorMessage="*" Runat=server CssClass="text"></asp:CustomValidator>
				</nobr>
						</td>
						<td width="150px" class="hClass">
							<%=LocRM.GetString("tLastName")%>
						</td>
						<td width="150px" class="hClass">
							<%=LocRM.GetString("tFirstName")%>
						</td>
						<td width="140px" class="hClass">
							<%=LocRM.GetString("tLogin")%>
						</td>
						<td class="hClass">
							<%=LocRM.GetString("tEMail")%>
						</td>
					</tr>
				</table>
				<div style="width: 100%; overflow-y: auto; overflow: auto; height: 300px; padding-bottom: 20px">
					<asp:DataGrid runat="server" Width="98%" ID="dgUsers" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False" CellSpacing="2" CellPadding="0" GridLines="Horizontal" BorderWidth="0" Style="table-layout: fixed" ShowHeader="False">
						<Columns>
							<asp:TemplateColumn ItemStyle-Width="30px" HeaderStyle-Width="30px">
								<ItemStyle CssClass="bClass"></ItemStyle>
								<ItemTemplate>
									<input type="checkbox" checked='<%# (bool)DataBinder.Eval(Container.DataItem, "Add") %>' class="text" runat="server" id="cbConvert" name="cbConvert" />
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:BoundColumn DataField="LastName" HeaderStyle-Width="150px" ItemStyle-Width="150px" ItemStyle-CssClass="bClass"></asp:BoundColumn>
							<asp:BoundColumn DataField="FirstName" HeaderStyle-Width="150px" ItemStyle-Width="150px" ItemStyle-CssClass="bClass"></asp:BoundColumn>
							<asp:BoundColumn DataField="Login" HeaderStyle-Width="140px" ItemStyle-Width="140px" ItemStyle-CssClass="bClass"></asp:BoundColumn>
							<asp:BoundColumn DataField="Email" ItemStyle-CssClass="bClass"></asp:BoundColumn>
						</Columns>
					</asp:DataGrid>
				</div>
			</td>
		</tr>
	</table>
</asp:Panel>
<asp:Panel ID="step4" runat="server" Height="100%">
	<div class="text" style="padding-left: 20px">
		<%=LocRM.GetString("s4TopDivConvert") %></div>
	<table class="ibn-stylesheet">
		<tr>
			<td width="130px">
				<fieldset style="height: 345px; margin: 0; padding: 2px">
					<legend class="text" id="lgdUserList" runat="server"></legend>
					<div style="padding-top: 10px">
					</div>
					<div style="width: 110px; overflow: auto; overflow-y: auto; overflow-x: auto; height: 300px; padding-bottom: 20px">
						<asp:DataList runat="server" ID="dlUsers" CellPadding="2" CellSpacing="2" ShowHeader="False" ShowFooter="False">
							<ItemStyle CssClass="ibn-propertysheet" Font-Size="10"></ItemStyle>
							<SelectedItemStyle CssClass="UserCellSelected ibn-menuimagecell" Font-Size="10"></SelectedItemStyle>
							<ItemTemplate>
								<asp:LinkButton ID="lbUser" CommandName='<%# DataBinder.Eval(Container.DataItem, "Login") %>' runat="server" CausesValidation="False">
						<%# (bool)DataBinder.Eval(Container.DataItem, "IsBad") ? 
							 "<img src='../layouts/images/status/status_offline.gif' border=0 align='absmiddle'>&nbsp;" + (string)DataBinder.Eval(Container.DataItem, "Login") :
							 "<img src='../layouts/images/status/status_online.gif' border=0 align='absmiddle'>&nbsp;" + (string)DataBinder.Eval(Container.DataItem, "Login") %>
								</asp:LinkButton>
							</ItemTemplate>
						</asp:DataList>
					</div>
				</fieldset>
			</td>
			<td valign="top" width="540px">
				<fieldset style="height: 345px; margin: 0; padding: 2px">
					<legend class="text" id="lgdUserInfo" runat="server"></legend>
					<table cellspacing="0" cellpadding="3" width="100%" border="0">
						<tr>
							<td class="text" width="90px">
								<b>
									<%=LocRM.GetString("tLogin")%>:</b>
							</td>
							<td class="text" width="130px">
								<asp:TextBox ID="txtLogin" runat="server" Width="115px" TabIndex="1"></asp:TextBox>
								<asp:RequiredFieldValidator ID="rfLogin" ControlToValidate="txtLogin" CssClass="text" Display="Dynamic" ErrorMessage="*" runat="server"></asp:RequiredFieldValidator>
								<asp:RegularExpressionValidator ID="txtRELoginValidator" runat="server" ErrorMessage="*" ControlToValidate="txtLogin" ValidationExpression="[\-0-9A-Za-z_.]+" Display="Dynamic"></asp:RegularExpressionValidator>
							</td>
							<td class="text">
								<b>
									<%=LocRM.GetString("tWorkPhone")%>:</b>
							</td>
							<td class="text">
								<asp:TextBox ID="txtPhone" runat="server" Width="120px" TabIndex="7"></asp:TextBox>
							</td>
						</tr>
						<tr>
							<td class="text">
								<b>
									<%=LocRM.GetString("tWindowsLogin")%>:</b>
							</td>
							<td class="text">
								<asp:TextBox ID="tbWindowsLogin" runat="server" Width="115px" TabIndex="2"></asp:TextBox>
							</td>
							<td class="text">
								<b>
									<%=LocRM.GetString("tFax")%>:</b>
							</td>
							<td class="text">
								<asp:TextBox ID="txtFax" runat="server" Width="120px" TabIndex="8"></asp:TextBox>
							</td>
						</tr>
						<tr>
							<td class="text">
								<b>
									<%=LocRM.GetString("tFirstName")%>:</b>
							</td>
							<td class="text">
								<asp:TextBox ID="txtFirstName" runat="server" Width="115px" TabIndex="3"></asp:TextBox>
								<asp:RequiredFieldValidator ID="rfFirstName" ControlToValidate="txtFirstName" CssClass="text" Display="Dynamic" ErrorMessage="*" runat="server"></asp:RequiredFieldValidator>
							</td>
							<td class="text">
								<b>
									<%=LocRM.GetString("tLocation")%>:</b>
							</td>
							<td class="text">
								<asp:TextBox ID="txtLocation" runat="server" Width="120px" TabIndex="9"></asp:TextBox>
							</td>
						</tr>
						<tr>
							<td class="text">
								<b>
									<%=LocRM.GetString("tLastName")%>:</b>
							</td>
							<td class="text">
								<asp:TextBox ID="txtLastName" runat="server" Width="115px" TabIndex="4"></asp:TextBox>
								<asp:RequiredFieldValidator ID="rfLastName" ControlToValidate="txtLastName" CssClass="text" Display="Dynamic" ErrorMessage="*" runat="server"></asp:RequiredFieldValidator>
							</td>
							<td class="text" width="95px">
								<b>
									<%=LocRM.GetString("tCompany")%>:</b>
							</td>
							<td class="text" width="125px">
								<asp:TextBox ID="txtCompany" runat="server" Width="120px" TabIndex="10"></asp:TextBox>
							</td>
						</tr>
						<tr>
							<td class="text">
								<b>
									<%=LocRM.GetString("tEMail")%>:</b>
							</td>
							<td class="text">
								<asp:TextBox ID="txtEmail" runat="server" Width="115px" TabIndex="5"></asp:TextBox>
								<asp:RequiredFieldValidator ID="rfEmail" ControlToValidate="txtEmail" CssClass="text" Display="Dynamic" ErrorMessage="*" runat="server"></asp:RequiredFieldValidator>
								<asp:RegularExpressionValidator ID="revEmail" runat="server" ErrorMessage="*" ControlToValidate="txtEmail" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" Display="Dynamic"></asp:RegularExpressionValidator>
							</td>
							<td class="text">
								<b>
									<%=LocRM.GetString("tDepartment")%>:</b>
							</td>
							<td class="text">
								<asp:TextBox ID="txtDepartment" runat="server" Width="120px" TabIndex="11"></asp:TextBox>
							</td>
						</tr>
						<tr>
							<td class="text">
								<b>
									<%=LocRM.GetString("tMobilePhone")%>:</b>
							</td>
							<td class="text">
								<asp:TextBox ID="txtMobile" runat="server" Width="115px" TabIndex="6"></asp:TextBox>
							</td>
							<td class="text">
								<b>
									<%=LocRM.GetString("tPosition")%>:</b>
							</td>
							<td class="text">
								<asp:TextBox ID="txtJobTitle" runat="server" Width="120px" TabIndex="12"></asp:TextBox>
							</td>
						</tr>
						<tr height="80px">
							<td colspan="4">
								<asp:CustomValidator ID="cvUserLogin" runat="server" Display="Dynamic" CssClass="text"></asp:CustomValidator><br>
								<asp:CustomValidator ID="cvEmail" runat="server" CssClass="text" Display="Dynamic"></asp:CustomValidator><br>
								<asp:CustomValidator ID="cvWindowsLogin" runat="server" Display="Dynamic" CssClass="text"></asp:CustomValidator>
							</td>
						</tr>
						<tr>
							<td valign="bottom" align="right" colspan="4">
								<btn:IMButton class="text" ID="btnSave" style="width: 110px" TabIndex="13" runat="server" Text="" OnServerClick="btnSave_ServerClick">
								</btn:IMButton>
							</td>
						</tr>
					</table>
				</fieldset>
			</td>
		</tr>
	</table>
</asp:Panel>
<asp:Panel ID="step5" runat="server" Height="100%">
	<div class="text" style="padding-left: 20px">
		<%=LocRM.GetString("s5TopDivConvert") %></div>
	<table class="ibn-stylesheet">
		<tr>
			<td width="195px">
				<fieldset style="height: 345px; margin: 0; padding: 2px">
					<legend class="text" id="lgdUserList2" runat="server">
						<input type="checkbox" class="text" style="width: 16px;" id="chkAll2" name="chkAll" onclick="CheckAll2(this);" />
						<label class="text">
							<%=LocRM.GetString("tUserList") %></label>
					</legend>
					<div style="padding-top: 10px">
					</div>
					<div style="width: 190px; overflow: auto; overflow-y: auto; overflow-x: auto; height: 300px; padding-bottom: 20px">
						<asp:DataList runat="server" ID="dlUserGroups" CellPadding="2" CellSpacing="2" ShowHeader="False" ShowFooter="False">
							<ItemStyle CssClass="ibn-propertysheet" Font-Size="10"></ItemStyle>
							<SelectedItemStyle CssClass="UserCellSelected ibn-menuimagecell" Font-Size="10"></SelectedItemStyle>
							<ItemTemplate>
								<nobr><asp:CheckBox ID="chkItem" Runat="server"></asp:CheckBox>
					<asp:linkbutton id="lbUser2" commandname='<%# DataBinder.Eval(Container.DataItem, "Login") %>' runat="server" causesvalidation="False">
						<%# (bool)DataBinder.Eval(Container.DataItem, "IsBadGroup") ? 
							 "<img src='../layouts/images/status/status_offline.gif' border=0 align='absmiddle'>&nbsp;" + (string)DataBinder.Eval(Container.DataItem, "LastName") + "&nbsp;" + (string)DataBinder.Eval(Container.DataItem, "FirstName") :
							 "<img src='../layouts/images/status/status_online.gif' border=0 align='absmiddle'>&nbsp;" + (string)DataBinder.Eval(Container.DataItem, "LastName") + "&nbsp;" + (string)DataBinder.Eval(Container.DataItem, "FirstName") %>
					</asp:linkbutton></nobr>
							</ItemTemplate>
						</asp:DataList>
					</div>
				</fieldset>
			</td>
			<td valign="top" width="475px">
				<fieldset style="height: 345px; margin: 0; padding: 2px">
					<legend class="text" id="lgdRolesGroups" runat="server"></legend>
					<table cellspacing="0" cellpadding="0" width="100%" border="0">
						<tr>
							<td>
								<table class="text" cellspacing="0" cellpadding="0" border="0" width="100%">
									<tr>
										<td valign="top" width="205px" style="padding: 0,6,1,0;">
											<b>
												<%=LocRM.GetString("tAvailable")%>:</b><br>
											<asp:ListBox ID="lbAvailableGroups" runat="server" Width="200px" CssClass="text" Height="100px"></asp:ListBox>
										</td>
										<td style="padding: 0,6,1,6;">
											<p align="center">
												<asp:Button ID="btnAddOneGr" Style="margin: 1px" runat="server" Width="30px" CssClass="text" CausesValidation="False" Text=">"></asp:Button><br>
												<asp:Button ID="btnAddAllGr" Style="margin: 1px" runat="server" Width="30px" CssClass="text" CausesValidation="False" Text=">>"></asp:Button><br>
												<br>
												<asp:Button ID="btnRemoveOneGr" Style="margin: 1px" runat="server" Width="30px" CssClass="text" CausesValidation="False" Text="<"></asp:Button><br>
												<asp:Button ID="btnRemoveAllGr" Style="margin: 1px" runat="server" Width="30px" CssClass="text" CausesValidation="False" Text="<<"></asp:Button>
											</p>
										</td>
										<td valign="top" width="205px" style="padding: 0,6,1,6;">
											<b>
												<%=LocRM.GetString("tSelected")%>:</b><br>
											<asp:ListBox ID="lbSelectedGroups" runat="server" Width="200px" CssClass="text" Height="100px"></asp:ListBox>
										</td>
									</tr>
								</table>
							</td>
						</tr>
						<tr>
							<td>
								<table class="text" cellspacing="0" cellpadding="0" border="0" width="100%">
									<tr>
										<td valign="top" width="205px" style="padding-right: 6px;">
											<b>
												<%=LocRM.GetString("tRoles")%>:</b><br>
											<asp:ListBox ID="lbSecurityRoles" SelectionMode="Multiple" runat="server" CssClass="text" Width="200px" Rows="5"></asp:ListBox>
										</td>
										<td>
											&nbsp;
										</td>
										<td width="205px" valign="top">
											<div align="left" valign="top" runat="server" id="divIM">
												<b>
													<%=LocRM.GetString("tCommGroup")%>:</b><br>
												<asp:DropDownList ID="ddIMGroups" runat="server" CssClass="text" Width="200px">
												</asp:DropDownList>
											</div>
											<div align="right">
											</div>
										</td>
									</tr>
									<tr style="padding-top: 10px" height="50px">
										<td colspan="3">
											<asp:CustomValidator ID="GroupValidator" ClientValidationFunction="GroupExistence" CssClass="text" Display="Dynamic" Style="vertical-align: top" runat="server"></asp:CustomValidator>
											<input id="iGroups" type="hidden" runat="server" name="iGroups">
										</td>
									</tr>
									<tr valign="bottom">
										<td colspan="3" align="right" style="padding-right: 10px">
											<btn:IMButton class="text" ID="btnSave2" style="width: 110px" runat="server" Text="" OnServerClick="btnSave2_ServerClick">
											</btn:IMButton>
										</td>
									</tr>
									<tr valign="bottom" height="40px">
										<td colspan="3">
											<b>
												<%=LocRM.GetString("tCommonPassword")%>:</b>&nbsp;&nbsp;<asp:TextBox ID="txtCommonPassword" runat="server" Width="205px" CssClass="text"></asp:TextBox>
										</td>
									</tr>
								</table>
							</td>
						</tr>
					</table>
				</fieldset>
			</td>
		</tr>
	</table>
</asp:Panel>
<asp:Panel ID="step6" runat="server" Height="100%">
	<table height="100%" width="100%">
		<tr>
			<td width="100%">
				<table width="100%">
					<tr>
						<td width="40px">
						</td>
						<td valign="top" width="80px">
							<img src="../layouts/images/check.gif" align="absMiddle">
						</td>
						<td class="text" valign="top">
							<%=LocRM.GetString("s5TopText") %><br>
							<br>
							<%=LocRM.GetString("UsersWereCreated") %>:
							<br />
							<br />
							<div style="width: 400px; overflow-y: auto; overflow: auto; height: 170px; padding-bottom: 20px;">
								<asp:Label ID="lblError" runat="server"></asp:Label>
							</div>
						</td>
						<td width="40px">
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
</asp:Panel>
<input id="wwwPath" type="hidden" value="" runat="server" />