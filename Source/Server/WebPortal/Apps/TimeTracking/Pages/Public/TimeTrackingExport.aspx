<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TimeTrackingExport.aspx.cs" Inherits="Mediachase.Ibn.Web.UI.TimeTracking.Pages.Public.TimeTrackingExport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<title>Export</title>
</head>
<body>
	<form id="form1" runat="server">
		<asp:DataGrid runat="server" ID="dgMain" AutoGenerateColumns="false" CssClass="SectionTable">			
			<Columns>
				<asp:BoundColumn DataField="Title"></asp:BoundColumn>
				<asp:TemplateColumn>
					<ItemStyle CssClass="TdTextClass" />
					<ItemTemplate>
						<%#  String.Format("{0:D2}:{1:D2}", Convert.ToInt32(Eval("Day1")) / 60, Convert.ToInt32(Eval("Day1")) % 60)%>
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:TemplateColumn>
					<ItemStyle CssClass="TdTextClass" />
					<ItemTemplate>                    
						<%#  String.Format("{0:D2}:{1:D2}", Convert.ToInt32(Eval("Day2")) / 60, Convert.ToInt32(Eval("Day2")) % 60)%>
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:TemplateColumn>
					<ItemStyle CssClass="TdTextClass" />
					<ItemTemplate>
						<%#  String.Format("{0:D2}:{1:D2}", Convert.ToInt32(Eval("Day3")) / 60, Convert.ToInt32(Eval("Day3")) % 60)%>
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:TemplateColumn>
					<ItemStyle CssClass="TdTextClass" />
					<ItemTemplate>
						<%#  String.Format("{0:D2}:{1:D2}", Convert.ToInt32(Eval("Day4")) / 60, Convert.ToInt32(Eval("Day4")) % 60)%>
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:TemplateColumn>
					<ItemStyle CssClass="TdTextClass" />
					<ItemTemplate>						
						<%#  String.Format("{0:D2}:{1:D2}", Convert.ToInt32(Eval("Day5")) / 60, Convert.ToInt32(Eval("Day5")) % 60)%>
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:TemplateColumn>
					<ItemStyle CssClass="TdTextClass" />
					<ItemTemplate>
						<%#  String.Format("{0:D2}:{1:D2}", Convert.ToInt32(Eval("Day6")) / 60, Convert.ToInt32(Eval("Day6")) % 60)%>
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:TemplateColumn>
					<ItemStyle CssClass="TdTextClass" />
					<ItemTemplate>
						<%#  String.Format("{0:D2}:{1:D2}", Convert.ToInt32(Eval("Day7")) / 60, Convert.ToInt32(Eval("Day7")) % 60)%>
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:TemplateColumn>
					<ItemStyle CssClass="TdTextClass" />
					<ItemTemplate>
						<%#  String.Format("{0:D2}:{1:D2}", Convert.ToInt32(Convert.ToInt32(Eval("DayT")) / 60), Convert.ToInt32(Convert.ToInt32(Eval("DayT")) % 60))%>
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:BoundColumn DataField="StateFriendlyName"></asp:BoundColumn>
			</Columns>
		</asp:DataGrid>
		<asp:GridView runat="server" ID="MainGrid" AutoGenerateColumns="false"></asp:GridView>
	</form>
</body>
</html>
