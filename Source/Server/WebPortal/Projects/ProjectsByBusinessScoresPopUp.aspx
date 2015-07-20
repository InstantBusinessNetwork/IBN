<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Projects.ProjectsByBusinessScoresPopUp" CodeBehind="ProjectsByBusinessScoresPopUp.aspx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="..\Modules\BlockHeader.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	
	<title><%=LocRM.GetString("tSelectProjectsToShow")%></title>
</head>
<body>

	<script type="text/javascript">
		//<![CDATA[
		function CheckProjects(obj) {

			var boxes = document.getElementsByTagName('input');
			if (boxes != null && boxes.length > 0) {
				for (i = 0; i < boxes.length; i++) {
					if (boxes[i].type == "checkbox" && boxes[i].id != obj.id)
						boxes[i].checked = obj.checked;
				}
			}
		}
		//]]>
	</script>

	<form id="Form1" method="post" runat="server">
	<table cellspacing="0" cellpadding="0" class="ibn-stylebox" style="width:100%; height:100%">
		<tr>
			<td class="ms-toolbar">
				<ibn:BlockHeader ID="secHeader" runat="server" />
			</td>
		</tr>
		<tr>
			<td class="ibn-navline ibn-alternating text" style="padding-top: 8px" valign="top">
				<asp:CheckBox runat="server" ID="cbAllProjects" Font-Bold="True" CssClass="text" onclick="javascript:CheckProjects(this);"></asp:CheckBox>
			</td>
		</tr>
		<tr class="text">
			<td>
				<div style="width: 100%; height: 300px; overflow-y: auto; width: 350px;">
					<asp:DataGrid runat="server" ID="dgProjects" AllowPaging="False" AllowSorting="False" DataKeyField="ProjectId" GridLines="None" AutoGenerateColumns="False" ShowHeader="False">
						<Columns>
							<asp:TemplateColumn>
								<ItemTemplate>
									<asp:CheckBox runat="server" ID="cbProject" Text='<%# DataBinder.Eval(Container.DataItem,"Title")%>' CssClass="text"></asp:CheckBox>
								</ItemTemplate>
							</asp:TemplateColumn>
						</Columns>
					</asp:DataGrid>
				</div>
			</td>
		</tr>
	</table>
	<asp:LinkButton runat="server" ID="lbApply" Visible="False"></asp:LinkButton>
	</form>
</body>
</html>
