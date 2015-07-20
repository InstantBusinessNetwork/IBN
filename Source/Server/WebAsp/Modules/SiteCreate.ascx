<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.Ibn.WebAsp.Modules.SiteCreate" Codebehind="SiteCreate.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\Modules\BlockHeader.ascx" %>
<script type="text/javascript"  src="../Scripts/FormScripts.js"></script>
<script type="text/javascript" src="../Scripts/cal.js"></script>
<input id="TimeOffset" type="hidden" value="0" name="TimeOffset" runat="server" />
<style type="text/css">
.data
{
	padding-left:10px;
	padding-top:1px;
}
</style>
<script type="text/javascript">
<!--
var dt = new Date();
document.forms[0].<%=TimeOffset.ClientID%>.value = dt.getTimezoneOffset() * 60;
function HideEndDate()
{
	var table = document.getElementById("BigTable");
	var rows = table.rows;
	for(var i=0;i<rows.length;i++)
	{
		if(rows[i].getAttribute("name") == "EndDatePicker")
			rows[i].style.display = "none";
	}
}
function ShowEndDate()
{
	var table = document.getElementById("BigTable");
	var rows = table.rows;
	for(var i=0;i<rows.length;i++)
	{
		if(rows[i].getAttribute("name") == "EndDatePicker")
			rows[i].style.display = "";
	}
}

function DemoClick()
{
	var obj = FindServerControl("input","Demo");
	if(obj && obj.checked)
	{
		ShowEndDate();
	}		
	else if(obj)
	{
		HideEndDate();
	}
}

function FindServerControl(tagName, originalId)
	{
		var obj = null;
		var objs = document.getElementsByTagName(tagName);
		for (var i=0; i<objs.length; i++)
		{
			if(tagName=="../label") var idName = objs[i].htmlFor;
			else var idName = objs[i].id;
			if (idName.lastIndexOf(originalId) >= 0 && idName.lastIndexOf(originalId) == idName.length - originalId.length)
			{
				obj = objs[i];
				break;
			}
		}
		return obj;
	}
//-->
</script>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:0px;">
	<tr>
		<td><ibn:blockheader id="secH" title="" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td>
			<table class="ibn-propertysheet" style="padding-left: 15px; PADDING-TOP: 10px" cellspacing="0" cellpadding="0" width="100%" border="0">
				<!-- Page description-->
				<tr style="padding-bottom:8px;">
					<td class="text" id="DescriptionText"><%=LocRM.GetString("TextTop")%></td>
				</tr>
				<tr>
					<td class="ibn-sectionline" height="1"><img height="1" alt="" src="../layouts/images/blank.gif" width="1" /></td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td>
			<table style="padding-left: 15px" cellspacing="0" cellpadding="0" width="100%" border="0">
				<!-- General Section -->
				<tr>
					<td class="ibn-descriptiontext" valign="top" >
						<table cellspacing="0" cellpadding="1" border="0">
							<tr>
								<td class="ibn-sectionheader" valign="top" height="28">Company Information</td>
							</tr>
							<tr>
								<td class="text" id="Title_Desc">
									
								</td>
							</tr>
						</table>
					</td>
					<td class="ibn-authoringcontrols" valign="top">
						<table id="BigTable" class="ibn-authoringcontrols" cellspacing="7" cellpadding="0" width="100%" border="0">
							<tr>
								<td class="data"><b><asp:checkbox id="Demo" runat="server" Text="" CssClass="text"></asp:checkbox></b></td>
							</tr>
							<tr>
								<td>
									<b><%=LocRM.GetString("Company")%>:</b>
									<div class="data">
										<asp:textbox id="txtCompanyName" Runat="server" Width="300" MaxLength="100" cssclass="text"></asp:textbox><asp:requiredfieldvalidator id="Requiredfieldvalidator1" runat="server" ErrorMessage="*" ControlToValidate="txtCompanyName"></asp:requiredfieldvalidator>
									</div>
								</td>
							</tr>
							<tr>
								<td>
									<b>Allowed Users:</b>
									<div class="data">
										<asp:textbox id="txtMaxUsers" Runat="server" Width="300" MaxLength="100" cssclass="text"></asp:textbox><asp:requiredfieldvalidator id="Requiredfieldvalidator3" runat="server" ErrorMessage="*" ControlToValidate="txtMaxUsers" Display=Dynamic></asp:requiredfieldvalidator><asp:rangevalidator id="RangeValidator1" runat="server" ErrorMessage="Wrong value" ControlToValidate="txtMaxUsers" Type="Integer" MaximumValue="1000" MinimumValue="-1" Display=Dynamic></asp:rangevalidator>
										<br />
										Use '-1' value for maximum users count allowed by license<BR>
									</div>
								</td>
							</tr>
							<tr>
								<td>
									<b>Allowed External Users:</b>
									<div class="data">
										<asp:textbox id="tbExternal" Runat="server" Width="300" MaxLength="100" cssclass="text"></asp:textbox><asp:requiredfieldvalidator id="Requiredfieldvalidator6" runat="server" ErrorMessage="*" ControlToValidate="tbExternal" Display=Dynamic></asp:requiredfieldvalidator><asp:rangevalidator id="Rangevalidator4" runat="server" ErrorMessage="Wrong value" ControlToValidate="tbExternal" Type="Integer" MaximumValue="1000" MinimumValue="-1" Display=Dynamic></asp:rangevalidator>
										<br />
										Use '-1' value for maximum external users count allowed by license<BR>
									</div>
								</td>
							</tr>
							<tr>
								<td>
									<b><%=LocRM.GetString("MaxDiskSpace")%>:</b>
									<div class="data">
										<asp:textbox id="txtDiskMax" Runat="server" Width="300" MaxLength="100" cssclass="text"></asp:textbox><asp:requiredfieldvalidator id="Requiredfieldvalidator5" runat="server" ErrorMessage="*" ControlToValidate="txtDiskMax"></asp:requiredfieldvalidator><asp:rangevalidator id="Rangevalidator3" runat="server" ErrorMessage="Wrong value" ControlToValidate="txtDiskMax" Type="Integer" MaximumValue="10000" MinimumValue="-1"></asp:rangevalidator>
									</div>
								</td>
							</tr>
							<tr style="display: none" name="EndDatePicker">
								<td>
									<b><%=LocRM.GetString("StartDate")%>:</b>
									<div class="data">
										<asp:textbox id="txtStartDate" runat="server" CssClass="text" width="97"></asp:textbox><button 
										id=btnStartDate style="BORDER-RIGHT: 0px; BORDER-TOP: 0px; BORDER-LEFT: 0px; WIDTH: 39px; PADDING-TOP: 0px; BORDER-BOTTOM: 0px; POSITION: relative; TOP: 0px; HEIGHT: 24px; BACKGROUND-COLOR: transparent" 
										onclick="ShowCal('<%=txtStartDate.ClientID %>','btnStartDate');" 
										type=button><img height="21" src="../layouts/images/calendar.gif" width="34" border="0"></button>
										<asp:customvalidator id="cvCompareDate" runat="server" ErrorMessage="Date Error"></asp:customvalidator>
									</div>
								</td>
							</tr>
							<tr style="display: none" name="EndDatePicker">
								<td>
									<b><%=LocRM.GetString("TrialEndDate")%>:</b>
									<div class="data">
										<asp:textbox id="txtDateTo" runat="server" CssClass="text" width="97"></asp:textbox><button 
										id=btnEndDate 
										style="BORDER-RIGHT: 0px; BORDER-TOP: 0px; BORDER-LEFT: 0px; WIDTH: 39px; PADDING-TOP: 0px; BORDER-BOTTOM: 0px; POSITION: relative; TOP: 0px; HEIGHT: 24px; BACKGROUND-COLOR: transparent" 
										onclick="ShowCal('<%=txtDateTo.ClientID %>','btnEndDate');" 
										type=button><img height="21" src="../layouts/images/calendar.gif" width="34" border="0"></button>
									</div>
								</td>
							</tr>
							<tr>
								<td>
									<b><%=LocRM.GetString("BasicLanguage")%>:</b>
									<div class="data">
										<asp:DropDownList id="ddLanguage" Runat="server" Width="300" cssclass="text"></asp:DropDownList>
									</div>
								</td>
							</tr>
							<tr>
								<td class="data">
									<asp:checkbox id="IsActive" runat="server" Text="" Checked="True" Font-Bold="true"></asp:checkbox>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td class="ibn-sectionline" colspan="2" height="2"><img height="1" alt="" src="../layouts/images/blank.gif" width="1" /></td>
				</tr>
				<!--Contact Info Section -->
				<tr>
					<td class="ibn-descriptiontext" valign="top">
						<table cellspacing="0" cellpadding="1" border="0">
							<tr>
								<td class="ibn-sectionheader" valign="top" height="28"><%=LocRM.GetString("text6")%></td>
							</tr>
							<tr>
								<td class="text"><%=LocRM.GetString("text5")%></td>
							</tr>
						</table>
					</td>
					<td class="ibn-authoringcontrols" valign="top">
						<table cellspacing="7" cellpadding="0" width="100%" border="0" class="ibn-authoringcontrols" >
							<tr>
								<td>
									<b><%=LocRM.GetString("ContactName")%>:</b>
									<div class="data">
										<asp:textbox id="txtContactName" Runat="server" Width="300" MaxLength="100" cssClass="text"></asp:textbox>
									</div>
								</td>
							</tr>
							<tr>
								<td>
									<b><%=LocRM.GetString("ContactPhone")%>:</b>
									<div class="data">
										<asp:textbox id="txtContactPhone" Runat="server" Width="300" MaxLength="100" cssclass="text"></asp:textbox>
									</div>
								</td>
							</tr>
							<tr>
								<td>
									<b><%=LocRM.GetString("ContactEmail")%>:</b>
									<div class="data">
										<asp:textbox id="txtContactEmail" Runat="server" Width="300" MaxLength="100" cssclass="text"></asp:textbox>
										<asp:RegularExpressionValidator id="RegularExpressionValidator1" runat="server" ControlToValidate="txtContactEmail" Display="Dynamic" ErrorMessage="*" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
									</div>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td class="ibn-sectionline" colspan="2" height="2"><img height="1" alt="" src="../layouts/images/blank.gif" width="1" /></td>
				</tr>
				<!-- Domain Section -->
				<tr>
					<td class="ibn-descriptiontext" valign="top">
						<table cellspacing="0" cellpadding="1" border="0">
							<tr>
								<td class="ibn-sectionheader" valign="top" height="28"><%=LocRM.GetString("text3")%></td>
							</tr>
							<tr>
								<td class="text"><%=LocRM.GetString("text4")%></td>
							</tr>
						</table>
						<br />
					</td>
					<td class="ibn-authoringcontrols" valign="top">
						<table cellspacing="7" cellpadding="0" width="100%" border="0" class="ibn-authoringcontrols" >
							<tr>
								<td>
									<b><%=LocRM.GetString("Domain")%>:</b>
									<div class="data">
										<asp:textbox id="txtDomain" Runat="server" Width="300" MaxLength="50" cssClass="text"></asp:textbox><asp:requiredfieldvalidator id="Requiredfieldvalidator2" runat="server" ErrorMessage="*" ControlToValidate="txtDomain"></asp:requiredfieldvalidator>
									</div>
								</td>
							</tr>
							<tr>
								<td>
									<b><%=LocRM.GetString("ApplicationPool")%>:</b>
									<div class="data">
										<asp:DropDownList runat="server" ID="ApplicationPoolList" Width="300"></asp:DropDownList>
									</div>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td class="ibn-sectionline" colspan="2" height="2"><img height="1" alt="" src="../layouts/images/blank.gif" width="1" /></td>
				</tr>
				<!--Admin Info Section -->
				<tr>
					<td class="ibn-descriptiontext" valign="top">
						<table cellspacing="0" cellpadding="1" border="0">
							<tr>
								<td class="ibn-sectionheader" valign="top" height="28"><%=LocRM.GetString("text7")%></td>
							</tr>
							<tr>
								<td class="text"><%=LocRM.GetString("text8")%></td>
							</tr>
						</table>
					</td>
					<td class="ibn-authoringcontrols" valign="top">
						<table cellspacing="7" cellpadding="0" width="100%" border="0" class="ibn-authoringcontrols" >
							<tr>
								<td>
									<b><%=LocRM.GetString("AdminFirstName")%>:</b>
									<div class="data">
										<asp:textbox id="txtAdminFirstName" Runat="server" Width="300" MaxLength="100" cssClass="text"></asp:textbox>
									</div>
								</td>
							</tr>
							<tr>
								<td>
									<b><%=LocRM.GetString("AdminLastName")%>:</b>
									<div class="data">
										<asp:textbox id="txtAdminLastName" Runat="server" Width="300" MaxLength="100" cssclass="text"></asp:textbox>
									</div>
								</td>
							</tr>
							<tr>
								<td>
									<b>Email:</b>
									<div class="data">
										<asp:textbox id="txtAdminEMail" Runat="server" Width="300" MaxLength="100" cssclass="text"></asp:textbox>
										<asp:requiredfieldvalidator id="ReqValAdminEMail" runat="server" ErrorMessage="*" ControlToValidate="txtAdminEMail"></asp:requiredfieldvalidator>
										<asp:RegularExpressionValidator id="revAdminEMail" runat="server" ControlToValidate="txtAdminEMail" Display="Dynamic" ErrorMessage="*" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
									</div>
								</td>
							</tr>
							<tr>
								<td>
									<b>Login:</b>
									<div class="data">
										<asp:textbox id="txtAdminLogin" Runat="server" Width="300" MaxLength="100" cssClass="text" Text="admin"></asp:textbox>
										<asp:regularexpressionvalidator id="txtRELoginValidator" runat="server" ErrorMessage="*" ControlToValidate="txtAdminLogin" ValidationExpression="[\-0-9A-Za-z_.]+" Display="Dynamic" Font-Bold="true"></asp:regularexpressionvalidator>
									</div>
								</td>
							</tr>
							<tr>
								<td>
									<b>Password:</b>
									<div class="data">
										<asp:textbox id="txtAdminPassword" Runat="server" Width="300" MaxLength="100" cssclass="text" TextMode="Password"></asp:textbox>
										<asp:RequiredFieldValidator runat="server" ID="txtAdminPasswordValidator" Display="Dynamic" ErrorMessage="*" ControlToValidate="txtAdminPassword" Font-Bold="true"></asp:RequiredFieldValidator>
									</div>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<!-- Begin OK/Cancel -->
				<tr>
					<td class="ibn-sectionline" colspan="2" height="4"><img height="1" alt="" src="../layouts/images/blank.gif" width="1" /></td>
				</tr>
				<tr>
					<td class="ibn-descriptiontext" colspan="2" height="10"><img SRC="../layouts/images/blank.gif" width="1" height="10" alt="" /></td>
				</tr>
				<tr>
					<td colspan="2" style="padding-right:10px;">
						<table align="right" border="0" width="100%" cellspacing="0" cellpadding="0">
							<tr>
								<td width="100%" align="center"><img SRC="../layouts/images/blank.gif" width="6" height="1" alt="">
									<asp:CustomValidator id=cvErrorCreation runat="server" CssClass="text" ErrorMessage="*"></asp:CustomValidator></td>
								<td align="right" ID="diidSubmitsection">
									<asp:Button Runat="server" Width="80px" ID="Submit" Text="Create" CssClass="text" onclick="Submit_Click"></asp:Button>
								</td>
								<td width="6" id="idSpace"><img SRC="../layouts/images/blank.gif" width="6" height="1" alt=""></td>
								<td align="right">
									<asp:Button CssClass="text" Width="80px" ID="btnCancel" Runat=server Text="Cancel" CausesValidation=False onclick="Cancel_Click"></asp:Button>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td height="9"></td>
					<!-- End OK/Cancel -->
				</tr>
			</table>
		</td>
	</tr>
</table>