<%@ Control Language="c#" Inherits="Mediachase.UI.Web.FileStorage.Modules.FileSearch" Codebehind="FileSearch.ascx.cs" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<script type="text/javascript">
	function ChangeModify()
	{
		id=document.forms[0].<%=ddModified.ClientID %>.value;
		
		objTr = document.getElementById('<%=trToDate.ClientID %>');
		objTbl = document.getElementById('<%=tableDateFrom.ClientID %>');
		objTd = document.getElementById('<%=tdDateTo.ClientID%>');
		
		if(id=="9")
		{
			objTbl.style.display = 'block';
			objTd.style.display = 'block';
			objTr.style.display = 'block';
		}
		else
		{
			objTbl.style.display = 'none';
			objTd.style.display = 'none';
			objTr.style.display = 'none';
		}
	}
	
	function ChangeSize()
	{
		id=document.forms[0].<%=ddSize.ClientID %>.value;
		
		obj1 = document.getElementById('<%=tableSizeFrom.ClientID %>');
		obj2 = document.getElementById('<%=tdSizeTo.ClientID %>');
		obj3=document.forms[0].<%=toSize.ClientID%>;
		obj4=document.forms[0].<%=ddKM2.ClientID%>;
		
		if(id=="0")
		{
			obj1.style.display = "none";
			obj2.style.display = "none";
			obj3.style.display = "none";
			obj4.style.display = "none";
		}
		else
		{
			obj1.style.display = "block";
			obj2.style.display = "block";
			obj3.style.display = "block";
			obj4.style.display = "block";
		}
	}
	
	function ViewFile(CName, CKey, FileId)
	{
		ShowWizard('<%=ResolveUrl("~/FileStorage/FileInfoView.aspx")%>'+ '?FileId=' + FileId + '&ContainerKey='+CKey+'&ContainerName='+CName, 650,310);
	}
	function LoginFocusElement(elId)
	{
		var elem=document.getElementById(elId);
		if(!elem)
			return;
		elem.focus();
	}
</script>
<table class="text" style="MARGIN-TOP: 0px" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td>
			<table class="text" style="BORDER-TOP-STYLE: none; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none" cellspacing="4" cellpadding="1" border="0">
				<tr height="30px">
					<td width="105px"><b><%=LocRM.GetString("tSearchString")%>:</b></td>
					<td width="310px">
						<asp:textbox id="tbSerchStr" Runat="server" CssClass="text" Width="280px"></asp:textbox>
					</td>
					<td align="left" width="30px"><b><%=LocRM.GetString("tType")%>:</b></td>
					<td align="left">
						<asp:dropdownlist id="ddType" Runat="server" CssClass="text" Width="200px"></asp:dropdownlist>&nbsp;
					</td>
					<td></td>
				</tr>
				<tr height="30px">
					<td><b><%=LocRM.GetString("tModified")%>:</b></td>
					<td>
						<table cellspacing="0" cellpadding="0">
							<tr>
								<td valign="center">
									<select class="text" id="ddModified" style="WIDTH: 95px" onchange="ChangeModify();" name="ddModified" runat="server">
									</select>
								</td>
								<td>
									<table id="tableDateFrom" cellspacing="2" cellpadding="0" runat="server">
										<tr>
											<td>&nbsp;<asp:label id="lblFrom" Font-Bold="True" Runat="server" CssClass="text" Visible="true"></asp:label>&nbsp;</td>
											<td><mc:Picker ID="dtcStartDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" /></td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
					<td>
						<table cellspacing="0" cellpadding="0">
							<tr>
								<td id="tdDateTo" align="right" runat="server">
									<asp:label id="lblTo" Font-Bold="True" Runat="server" CssClass="text"></asp:label>
								</td>
							</tr>
						</table>
					</td>
					<td width="210">
						<table cellspacing="1" cellpadding="0">
							<tr runat="server" id="trToDate">
								<td valign="bottom">
									<mc:Picker ID="dtcEndDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" /></td>
							</tr>
						</table>
					</td>
					<td class="text"><asp:customvalidator id="CustomValidator1" runat="server" Display="Dynamic" ErrorMessage="*"></asp:customvalidator></td>
				</tr>
				<tr height="30px">
					<td><b><%=LocRM.GetString("tFileSize")%>:</b></td>
					<td>
						<table cellspacing="0" cellpadding="0">
							<tr>
								<td>
									<select class="text" id="ddSize" style="WIDTH: 95px" onchange="ChangeSize();" name="ddSize" runat="server">
									</select>
								</td>
								<td>
									<table id="tableSizeFrom" cellspacing="2" cellpadding="0" runat="server">
										<tr>
											<td>&nbsp;<asp:label id="lblfromSize" Font-Bold="True" Runat="server" CssClass="text"></asp:label>&nbsp;</td>
											<td vAlign="center"><asp:textbox id="fromSize" Runat="server" CssClass="text" Width="90px"></asp:textbox></td>
											<td vAlign="center"><asp:dropdownlist id="ddKM1" Runat="server" CssClass="text" Width="42px"></asp:dropdownlist></td>
											<td><asp:comparevalidator id="CompareValidator1" runat="server" CssClass="text" ErrorMessage="*" ControlToValidate="fromSize" Type="Integer" Operator="GreaterThanEqual" ValueToCompare="0" Display="Dynamic"></asp:comparevalidator></td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
					<td>
						<table cellspacing="0" cellpadding="0">
							<tr>
								<td id="tdSizeTo" align="left" runat="server">
									<asp:label id="lbltoSize" Font-Bold="True" Runat="server" CssClass="text"></asp:label>
								</td>
							</tr>
						</table>
					</td>
					<td width="210px">
						<table cellspacing="2" cellpadding="0">
							<tr>
								<td valign="middle">
									<asp:textbox id="toSize" Runat="server" CssClass="text" Width="90"></asp:textbox>
								</td>
								<td valign="middle">
									<asp:dropdownlist id="ddKM2" Runat="server" CssClass="text" Width="42px"></asp:dropdownlist>
								</td>
								<td>
									<asp:comparevalidator id="CompareValidator2" runat="server" CssClass="text" ErrorMessage="*" ControlToValidate="toSize" Type="Integer" Operator="GreaterThanEqual" ValueToCompare="0"></asp:comparevalidator>
									<asp:label id="lblValid" Runat="server" CssClass="ibn-alerttext"></asp:label>
								</td>
							</tr>
						</table>
					</td>
					<td>
						<asp:button id="btnSearch" Runat="server" CssClass="text" Width="80px" onclick="btnSearch_Click"></asp:button>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td style="BORDER-TOP: #a0a0a0 1px solid; PADDING-TOP: 5px; background-color:#ffffff;" valign="top">
			<dg:datagridextended id="grdMain" runat="server" width="100%" PageSize="10" allowsorting="True" 
				allowpaging="True" autogeneratecolumns="False" borderwidth="0" gridlines="None" 
				cellpadding="1" enableviewstate="false">
				<columns>
					<asp:boundcolumn visible="false" datafield="Id"></asp:boundcolumn>
					<asp:templatecolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headerstyle-width="21"
						itemstyle-width="21">
						<itemtemplate>
							<img alt="" src='<%# Eval("Icon")%>' width="16" height="16" />
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" sortexpression="sortName">
						<itemtemplate>
							<%# GetLink((int)Eval("Id"), Eval("Name").ToString(), Eval("ContentType").ToString()) %>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="Modifier"
						sortexpression="sortModifier" headerstyle-width="150" itemstyle-width="150"></asp:boundcolumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="ModifiedDate"
						headerstyle-width="95" itemstyle-width="95" sortexpression="sortModified"></asp:boundcolumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="Size"
						sortexpression="sortSize" headerstyle-width="70" itemstyle-width="70"></asp:boundcolumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="ActionView"
						headerstyle-width="25" itemstyle-width="25"></asp:boundcolumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="ActionJump"
						headerstyle-width="25" itemstyle-width="25"></asp:boundcolumn>
				</columns>
			</dg:datagridextended>
			
			<dg:datagridextended id="grdDetails" runat="server" allowsorting="True" allowpaging="True" width="100%"
					autogeneratecolumns="False" borderwidth="0" gridlines="None" cellpadding="1" PageSize="10"
					LayoutFixed="True" EnableViewState="false">
				<columns>
					<asp:boundcolumn visible="false" datafield="Id"></asp:boundcolumn>
					<asp:templatecolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headerstyle-width="21"
						itemstyle-width="21">
						<itemtemplate>
							<img alt="" src='<%# Eval("Icon")%>' width="16" height="16" />
						</itemtemplate>
					</asp:templatecolumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2" />
						<ItemStyle CssClass="ibn-vb2" />
						<HeaderTemplate>
							<table border="0" cellpadding="2" cellspacing="0" width="100%" class="ibn-propertysheet" style="table-layout:fixed;">
								<tr>
									<td><asp:LinkButton Runat="server" CommandName="Sort" CommandArgument="sortName" Text = <%# LocRM.GetString("Title") %>></asp:LinkButton></td>
									<td style="width:170px;"><asp:LinkButton Runat="server" CommandName="Sort" CommandArgument="sortModifier" Text = <%# LocRM.GetString("UpdBy") %>></asp:LinkButton></td>
									<td style="width:95px;"><asp:LinkButton Runat="server" CommandName="Sort" CommandArgument="sortModified" Text = <%# LocRM.GetString("UpdDate") %>></asp:LinkButton></td>
									<td style="width:70px;"><asp:LinkButton Runat="server" CommandName="Sort" CommandArgument="sortSize" Text =<%# LocRM.GetString("Size") %>></asp:LinkButton></td>
								</tr>
								<tr>
									<td colspan="4" style="color:#808080;"><%# LocRM2.GetString("tDescription")%></td>
								</tr>
							</table>
						</HeaderTemplate>
						<ItemTemplate>
							<table border="0" cellpadding="2" cellspacing="0" width="100%" class="ibn-propertysheet" style="table-layout:fixed;">
								<tr>
									<td valign="top"><%# GetLink((int)Eval("Id"), Eval("Name").ToString(), Eval("ContentType").ToString()) %></td>
									<td style="width:170px;" valign="top"><%# Eval("Modifier")%></td>
									<td style="width:95px;" valign="top"><%# Eval("ModifiedDate")%></td>
									<td style="width:70px;" valign="top"><%# Eval("Size")%></td>
								</tr>
								<tr>
									<td colspan="4" style="color: #555555;"><%# Eval("Description")%></td>
								</tr>
							</table>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="ActionView"
						headerstyle-width="25" itemstyle-width="25"></asp:boundcolumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="ActionJump"
						headerstyle-width="25" itemstyle-width="25"></asp:boundcolumn>
				</columns>
			</dg:datagridextended>
		</td>
	</tr>
</table>
<asp:LinkButton ID="lbChangeViewTable" Runat="server" Visible="False"></asp:LinkButton>
<asp:LinkButton ID="lbChangeViewDet" Runat="server" Visible="False"></asp:LinkButton>