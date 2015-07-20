<%@ Control Language="c#" Inherits="Mediachase.UI.Web.FileStorage.Modules.FileStorageControl" Codebehind="FileStorageControl.ascx.cs" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<script type="text/javascript">
	function DeleteChecked()
	{
		var Ids = "";
		
		aInputs = document.getElementsByTagName("input");
		for (var i=0; i<aInputs.length; i++)
		{
			oInput = aInputs[i];
			if(oInput.type == "checkbox" && oInput.name.indexOf("chkItem") >= 0 && oInput.checked)
			{
				var str = oInput.value;
				Ids += str+",";
			}
		}
		if(Ids!="" && confirm('<%=LocRM.GetString("tWarningAll")%>'))
		{
			document.forms[0].<%= hidForDelete.ClientID%>.value = Ids;
			<%= Page.ClientScript.GetPostBackEventReference(lbDeleteChecked, "")%>
		}
	}
	
	function SendChecked(CName, CKey)
	{
		var Ids = "";
		
		aInputs = document.getElementsByTagName("input");
		for (var i=0; i<aInputs.length; i++)
		{
			oInput = aInputs[i];
			if(oInput.type == "checkbox" && oInput.name.indexOf("chkItem") >= 0 && oInput.checked)
			{
				var str = oInput.value;
				var str_Beg = str.substr(0, str.indexOf("_"));
				if(str_Beg=="2")
					Ids += str.substr(str.indexOf("_")+1) + ",";
			  oInput.checked = false;
			}
		}
		if(Ids.length>0)
			ShowResizableWizard('<%=ResolveUrl("~/Incidents/AddEMailMessage.aspx")%>'+ '?FileIds=' + Ids + '&ContainerKey='+CKey+'&ContainerName='+CName, 800,600);
	}
	
	function MoveChecked(CName, CKey)
	{
		var FIds = "";
		var DIds = "";
		aInputs = document.getElementsByTagName("input");
		for (var i=0; i<aInputs.length; i++)
		{
			oInput = aInputs[i];
			if(oInput.type == "checkbox" && oInput.name.indexOf("chkItem") >= 0 && oInput.checked)
			{
				var str = oInput.value;
				var str_Beg = str.substr(0, str.indexOf("_"));
				if(str_Beg=="1")
					DIds += str.substr(str.indexOf("_")+1) + ",";
				if(str_Beg=="2")
					FIds += str.substr(str.indexOf("_")+1) + ",";
			}
		}
		if(DIds.length>0 || FIds.length>0)
			ShowWizard('<%=ResolveUrl("~/FileStorage/MoveObject.aspx")%>'+ '?Folders=' + DIds + '&Files='+ FIds +'&ContainerKey='+CKey+'&ContainerName='+CName, 430,350);
	}
	
	function CopyChecked()
	{
		var Ids = "";
		
		aInputs = document.getElementsByTagName("input");
		for (var i=0; i<aInputs.length; i++)
		{
			oInput = aInputs[i];
			if(oInput.type == "checkbox" && oInput.name.indexOf("chkItem") >= 0 && oInput.checked)
			{
				oInput.checked = false;
				var oDiv_id = "divId"+oInput.name.substr(7);
				var oDiv = document.getElementById(oDiv_id);
				if(oDiv)
					oDiv.className='main_span_style';
				var str = oInput.value;
				var str_Beg = str.substr(0, str.indexOf("_"));
				if(str_Beg=="2")
					Ids += str.substr(str.indexOf("_")+1) + ",";
			}
		}
		
		if(Ids.length>0)
		{
			var req = window.XMLHttpRequest? 
				new XMLHttpRequest() : 
				new ActiveXObject("Microsoft.XMLHTTP");
				
			req.onreadystatechange = function() 
			{
				if (req.readyState != 4 ) return ;
				if (req.readyState == 4)
				{
					var oCurDiv = document.getElementById("divAlert");
					var oCurSpan = document.getElementById("spanAlert");
					if (req.status == 200)
						oCurSpan.innerHTML = req.responseText.toString();
					else
						oCurSpan.innerHTML = req.statusText;
					oCurDiv.style.display = "block";
				}
			}
			var dt = new Date();
			var sID = dt.getMinutes() + "_" + dt.getSeconds() + "_" + dt.getMilliseconds();
			req.open("GET", '../Modules/XmlForTreeView.aspx?AddClip=Files&Files='+Ids+"&sID="+sID, true);
			req.send(null);
		}
	}
	
	function PasteFromClip(CName, CKey, FolderId)
	{
		ShowResizableWizard('<%=ResolveUrl("~/FileStorage/PasteHandler.aspx")%>'+ '?FolderId=' + FolderId + '&ContainerKey='+CKey+'&ContainerName='+CName, 200, 200);
	}
	
	function ViewFile(CName, CKey, FileId)
	{
		ShowWizard('<%=ResolveUrl("~/FileStorage/FileInfoView.aspx")%>'+ '?FileId=' + FileId + '&ContainerKey='+CKey+'&ContainerName='+CName, 650,310);
	}
	
	function CheckAll(obj)
	{
		aInputs = document.getElementsByTagName("input");
		for (var i=0; i<aInputs.length; i++)
		{
			oInput = aInputs[i];
			if(oInput.type == "checkbox" && oInput.name.indexOf("chkItem") >= 0)
			{
				oInput.checked = obj.checked;
			}
		}
	}
	
	function FolderSecurity(CName, CKey, FolderId)
	{
		ShowWizard('<%=ResolveUrl("~/FileStorage/Security.aspx")%>'+ '?FolderId=' + FolderId + '&ContainerKey='+CKey+'&ContainerName='+CName, 650,310);
	}
</script>
<script type="text/javascript">
		function _select(obj, divId)
		{
			var objDiv = document.getElementById(divId);
			if(obj && objDiv)
			{
				if(obj.checked)
					objDiv.className='main_span_style_selected';
				else
					objDiv.className='main_span_style';
			}
		}
		
		function _hover(objDiv, cbId)
		{
			var objCb = document.getElementById(cbId);
			if(objCb)
			{
				if(!objCb.checked)
					objDiv.className='main_span_style_hover';
			}
			else
			{
				objDiv.className='main_span_style_hover';
			}
		}
		
		function _out(objDiv, cbId)
		{
			var objCb = document.getElementById(cbId);
			if(objCb)
			{
				if(!objCb.checked)
					objDiv.className='main_span_style';
			}
			else
				objDiv.className='main_span_style';
		}
	</script>
	<style type="text/css">
		.main_span_style{width:230px;height:140px;float:left;margin:5px;background-color:#F3F3F3;}
		.main_span_style_hover{width:230px;height:140px;float:left;margin:5px;background-color:#EAEAEA;}
		.main_span_style_selected{width:230px;height:140px;float:left;margin:5px;background-color:#E5E5FF;}
		.main_style_img{width:90px; height:90px;border:1px solid #bbbbbb; text-align:center; vertical-align:middle;}
	</style>
<table cellspacing="0" cellpadding="0" border="0" width="100%">
	<tr>
		<td class="ibn-navline text" style="padding: 5px 5px 5px 10px;">
			<div id="divAlert" align="center" style="PADDING: 5px; DISPLAY: none; ">
				<span id="spanAlert" class="ibn-alerttext" style="padding: 3px; border:1px solid red"></span>
			</div>
			<asp:label id="lblPathTitle" CssClass="text" runat="server"></asp:label>:&nbsp;&nbsp;
			<asp:label id="lblPath" runat="server" cssclass="ibn-nav"></asp:label>
		</td>
	</tr>
	<tr>
		<td style="padding: 10px 5px 5px 10px;">
			<dg:datagridextended id="grdMain" runat="server" allowsorting="True" allowpaging="True" width="100%"
					autogeneratecolumns="False" borderwidth="0" gridlines="None" cellpadding="1"	
					PageSize="10" LayoutFixed="True" enableviewstate="false">
				<columns>
					<asp:boundcolumn visible="false" datafield="Id"></asp:boundcolumn>
					<asp:boundcolumn visible="false" datafield="Weight"></asp:boundcolumn>
					<asp:templatecolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headerstyle-width="25"
						itemstyle-width="25px">
						<HeaderTemplate>
							<asp:checkbox runat="server" id="chkAll" onclick="CheckAll(this);" Visible='<%# !Mediachase.IBN.Business.Security.CurrentUser.IsExternal%>'></asp:checkbox>
						</HeaderTemplate>
						<itemtemplate>
							<input value='<%#Eval("Weight").ToString()+"_"+Eval("Id").ToString()%>' type="checkbox" runat="server" id="chkItem" Visible='<%# !((int)Eval("Weight")==0) && !Mediachase.IBN.Business.Security.CurrentUser.IsExternal%>' />
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headerstyle-width="21"
						itemstyle-width="21px">
						<itemtemplate>
							<img alt="" src='<%# Eval("Icon")%>' width="16" height="16" />
						</itemtemplate>
					</asp:templatecolumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="Name"
						sortexpression="sortName"></asp:boundcolumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="Modifier"
						headerstyle-width="150px" itemstyle-width="150px" sortexpression="sortModifier"></asp:boundcolumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="ModifiedDate"
						headerstyle-width="90px" itemstyle-width="90px" sortexpression="sortModified"></asp:boundcolumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="Size"
						sortexpression="sortSize" headerstyle-width="70px" itemstyle-width="70px"></asp:boundcolumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="CopyLink"
						headerstyle-width="25px" itemstyle-width="25px"></asp:boundcolumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="ActionVS"
						headerstyle-width="25px" itemstyle-width="25px"></asp:boundcolumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="ActionEdit"
						headerstyle-width="25px" itemstyle-width="25px"></asp:boundcolumn>
					<asp:templatecolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headerstyle-width="25px"
						itemstyle-width="25px">
						<itemtemplate>
							<asp:ImageButton Visible='<%# (bool)Eval("CanDelete")%>' ID="ibDelete" Runat=server CommandName="Delete" ImageUrl="~/layouts/Images/Delete.gif"></asp:ImageButton>
						</itemtemplate>
					</asp:templatecolumn>
				</columns>
			</dg:datagridextended>
			
			<dg:datagridextended id="grdDetails" runat="server" allowsorting="True" allowpaging="True" width="100%"
					autogeneratecolumns="False" borderwidth="0" gridlines="None" cellpadding="1"	
					PageSize="10" LayoutFixed="True" enableviewstate="false">
				<columns>
					<asp:boundcolumn visible="false" datafield="Id"></asp:boundcolumn>
					<asp:boundcolumn visible="false" datafield="Weight"></asp:boundcolumn>
					<asp:templatecolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headerstyle-width="25"
						itemstyle-width="25">
						<HeaderTemplate>
							<asp:checkbox runat="server" id="chkAll2" onclick="CheckAll(this);" Visible='<%# !Mediachase.IBN.Business.Security.CurrentUser.IsExternal%>'></asp:checkbox>
						</HeaderTemplate>
						<itemtemplate>
							<input value='<%#Eval("Weight").ToString()+"_"+Eval("Id").ToString()%>' type="checkbox" runat="server" id="chkItem2" Visible='<%# !((int)Eval("Weight")==0) && !Mediachase.IBN.Business.Security.CurrentUser.IsExternal%>' />
						</itemtemplate>
					</asp:templatecolumn>
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
							<table border="0" cellpadding="2" cellspacing="0" width="100%" class="ibn-propertysheet">
								<tr>
									<td><asp:LinkButton Runat="server" CommandName="Sort" CommandArgument="sortName" Text = <%# LocRM.GetString("tName") %>></asp:LinkButton></td>
									<td width="200px"><asp:LinkButton Runat="server" CommandName="Sort" CommandArgument="sortModifier" Text = <%# LocRM.GetString("UpdatedBy") %>></asp:LinkButton></td>
									<td width="90px"><asp:LinkButton Runat="server" CommandName="Sort" CommandArgument="sortModified" Text = <%# LocRM.GetString("UpdatedDate") %>></asp:LinkButton></td>
									<td width="70px"><asp:LinkButton Runat="server" CommandName="Sort" CommandArgument="sortSize" Text =<%# LocRM.GetString("Size") %>></asp:LinkButton></td>
								</tr>
								<tr>
									<td colspan="4" style="color:#808080;"><%# LocRM.GetString("tDescription")%></td>
								</tr>
							</table>
						</HeaderTemplate>
						<ItemTemplate>
							<table border="0" cellpadding="2" cellspacing="0" width="100%" class="ibn-propertysheet" style="table-layout:fixed;">
								<tr>
									<td valign="top"><%# Eval("Name")%></td>
									<td width="200px" valign="top"><%# Eval("Modifier")%></td>
									<td width="90px" valign="top"><%# Eval("ModifiedDate")%></td>
									<td width="70px" valign="top"><%# Eval("Size")%></td>
								</tr>
								<tr>
									<td colspan="4" style="color: #555555;"><%# Eval("Description")%></td>
								</tr>
							</table>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="CopyLink"
						headerstyle-width="25" itemstyle-width="25"></asp:boundcolumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="ActionVS"
						headerstyle-width="25" itemstyle-width="25"></asp:boundcolumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="ActionEdit"
						headerstyle-width="25" itemstyle-width="25"></asp:boundcolumn>
					<asp:templatecolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headerstyle-width="25"
						itemstyle-width="25">
						<itemtemplate>
							<asp:ImageButton Visible='<%# (bool)Eval("CanDelete")%>' ID="ibDelete2" Runat=server CommandName="Delete" ImageUrl="~/layouts/Images/Delete.gif"></asp:ImageButton>
						</itemtemplate>
					</asp:templatecolumn>
				</columns>
			</dg:datagridextended>
			
			<asp:Repeater ID="repThumbs" Runat="server">
				<HeaderTemplate>
					<table width="100%" cellpadding="0" cellspacing="0" border="0"><tr><td>
				</HeaderTemplate>
				<ItemTemplate>
					<div id='<%#"divId"+Eval("Weight").ToString()+Eval("Id").ToString()%>' 
						class="main_span_style" 
						onmouseover="javascript:_hover(this, '<%#"chkItem"+Eval("Weight").ToString()+Eval("Id").ToString()%>');" 
						onmouseout="javascript:_out(this, '<%#"chkItem"+Eval("Weight").ToString()+Eval("Id").ToString()%>');" >
						<table border="0" cellpadding="0" cellspacing="0" style="margin:5px;" class="text">
							<tr height="25px">
								<td width="25px">
									<input align="absmiddle" type="checkbox"
										name='<%#"chkItem"+Eval("Weight").ToString()+Eval("Id").ToString()%>'
										id='<%#"chkItem_"+Eval("Weight").ToString()+"_"+Eval("Id").ToString()%>'
										value='<%#Eval("Weight").ToString()+"_"+Eval("Id").ToString()%>'
										style='display:<%# ((int)Eval("Weight")==0)?"none":""%>'
										onclick="javascript:_select(this, '<%#"divId"+Eval("Weight").ToString()+Eval("Id").ToString()%>');" 
									/>
								</td>
								<td colspan="2"><%#Eval("Name")%></td>
							</tr>
							<tr>
								<td width="25px" valign="bottom">
									<%#Eval("CopyLink")%>
									<%#Eval("ActionVS")%>
									<%#Eval("ActionEdit")%>
									<asp:ImageButton Visible='<%# (bool)Eval("CanDelete")%>' 
										CommandArgument='<%# Eval("Weight").ToString()+"_"+Eval("Id").ToString()%>'
										ID="ibDelete1" Runat="server" CommandName="Delete" ImageUrl="~/layouts/Images/Delete.gif">
									</asp:ImageButton>
								</td>
								<td class="main_style_img">
									<%# Eval("BigIcon")%>
								</td>
								<td width="105px" valign="top">
									<span style="margin-left:5px;"><%#((int)Eval("Weight")!=0)?"<b>"+LocRM.GetString("UpdatedDate")+":</b>":""%></span><br>
									<span style="margin-left:5px;"><nobr><%#Eval("ModifiedDate")%></nobr></span><br><br>
									<span style="margin-left:5px;"><%#((int)Eval("Weight")==2)?"<b>"+LocRM.GetString("Size")+":</b>":""%></span><br>
									<span style="margin-left:5px;"><%#Eval("Size")%></span>
								</td>
							</tr>
							<tr><td colspan="3">&nbsp;</td></tr>
						</table>
					</div>
				</ItemTemplate>
				<FooterTemplate>
					</td></tr></table>
				</FooterTemplate>
			</asp:Repeater>
		</td>
	</tr>
</table>
<asp:LinkButton ID="lbDeleteChecked" Runat="server" Visible="False"></asp:LinkButton>
<asp:LinkButton ID="lbChangeViewTable" Runat="server" Visible="False"></asp:LinkButton>
<asp:LinkButton ID="lbChangeViewThumb" Runat="server" Visible="False"></asp:LinkButton>
<asp:LinkButton ID="lbChangeViewDet" Runat="server" Visible="False"></asp:LinkButton>
<input type="hidden" id="hidForDelete" runat="server" />