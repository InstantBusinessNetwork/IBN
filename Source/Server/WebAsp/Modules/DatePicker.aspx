<%@ Page language="c#" Inherits="Mediachase.Ibn.WebAsp.Modules.DatePicker" Codebehind="DatePicker.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
		<title>Choose a Date</title>
</HEAD>
	<body leftmargin="0" topmargin="0">
		<form runat="server" ID="Form1">
			<asp:Calendar id="Calendar1" Width="185" Height="90" runat="server" OnSelectionChanged="Calendar1_SelectionChanged" OnDayRender="Calendar1_DayRender" NextMonthText="<IMG src='../layouts/images/monthright.gif' border='0'>" PrevMonthText="<IMG src='../layouts/images/monthleft.gif' border='0'>" BackColor="White" BorderColor="Black" BorderStyle="Solid">
				<TodayDayStyle BackColor="#FFFFC0"></TodayDayStyle>
				<DayStyle Font-Size="8pt" Font-Names="Arial"></DayStyle>
				<DayHeaderStyle Font-Size="10pt" Font-Underline="True" Font-Names="Arial" BorderStyle="None" BackColor="#E0E0E0"></DayHeaderStyle>
				<SelectedDayStyle Font-Size="8pt" Font-Names="Arial" Font-Bold="True" ForeColor="White" BackColor="Navy"></SelectedDayStyle>
				<TitleStyle Font-Size="10pt" Font-Names="Arial" Font-Bold="True" ForeColor="White" BackColor="Navy"></TitleStyle>
				<OtherMonthDayStyle ForeColor="Gray"></OtherMonthDayStyle>
			</asp:Calendar>
			<asp:Literal id="Literal1" runat="server"></asp:Literal>
		</form>
	</body>
</HTML>
