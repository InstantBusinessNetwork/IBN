<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorkflowCreate.aspx.cs" Inherits="Mediachase.Ibn.AssignmentsUI.Modules.Pages.WorkflowCreate" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<div class="box">
			Workflow name: <asp:TextBox runat="server" ID="tbName" />
			<asp:LinkButton runat="server" ID="lblCreate" Text="Create" />
		</div>
    
    </div>
    </form>
</body>
</html>
