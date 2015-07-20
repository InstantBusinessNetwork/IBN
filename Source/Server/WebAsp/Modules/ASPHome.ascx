<%@ Control Language="c#" Inherits="Mediachase.Ibn.WebAsp.Modules.ASPHome" CodeBehind="ASPHome.ascx.cs" %>
<table cellspacing="0" cellpadding="0" border="0">
	<tr>
		<td valign="top">
			<img alt="" src="../layouts/images/subwebs.gif" />
		</td>
		<td valign="top" class="ibn-propertysheet">
			<div class="mainMenu-Item">
				<img alt="" src="../Layouts/Images/Rect.gif" />
				<a href="../pages/sites.aspx"><%=LocRM.GetString("ManageSites") %></a>
			</div>
			<div class="mainMenu-Item">
				<img alt="" src="../Layouts/Images/Rect.gif" />
				<a href="../pages/TrialRequests.aspx"><%=LocRM.GetString("TrialReqs") %></a>
			</div>
			<div class="mainMenu-Item">
				<img alt="" src="../Layouts/Images/Rect.gif" />
				<a href="../pages/Settings.aspx"><%=LocRM.GetString("Settings") %></a>
			</div>
			<div class="mainMenu-Item">
				<img alt="" src="../Layouts/Images/Rect.gif" />
				<a href="../pages/Resellers.aspx">Resellers</a>
			</div>
			<div class="mainMenu-Item">
				<img alt="" src="../Layouts/Images/Rect.gif" />
				<a href="../pages/TrialTemplates.aspx">Notification Templates</a>
			</div>
			<div class="mainMenu-Item">
				<img alt="" src="../Layouts/Images/Rect.gif" />
				<a href="../Pages/LoginUser.aspx">Login on Behalf of User</a>
			</div>
		</td>
	</tr>
</table>
