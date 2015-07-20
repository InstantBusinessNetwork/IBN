<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ColumnEditorModalPopup.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.MetaUI.ActionControls.ColumnEditorModalPopup" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<script type="text/javascript">
	function mc_grid_columnEditor_show()
	{
		$find('<%= mc_grid_columnEditor_Ext.ClientID %>').show();
		InitColumnEditor();
	}
	function mc_grid_columnEditor_hide()
	{
		$find('<%= mc_grid_columnEditor_Ext.ClientID %>').hide();
	}
	
    function RemoveHPSection(str)
	{
	    //alert('RemoveHPSection');
		var removeCol;
		var removeHide;
		
		if(str=="left")
		{
			removeCol = document.getElementById('<%=lbLeftColumn.ClientID%>');
			removeHide = document.getElementById('<%=in_leftcol_hide.ClientID%>');
		}
		else if(str=="right")
		{
			removeCol = document.getElementById('<%=lbRightColumn.ClientID%>');
			removeHide = document.getElementById('<%=in_rightcol_hide.ClientID%>');
		}
			
		if(removeCol.options.length < 1)
			return;
			
		
		//AddToSel(removeCol, ddobj, toHide);
		//DeleteFromSel(removeCol, removeHide);
		if (str == "left")
		{
		    MoveOne2(removeCol, document.getElementById('<%=lbRightColumn.ClientID%>'), removeHide, document.getElementById('<%=in_rightcol_hide.ClientID%>'));
		}
		if (str == "right")
		{
		    MoveOne2(removeCol, document.getElementById('<%=lbLeftColumn.ClientID%>'), removeHide, document.getElementById('<%=in_leftcol_hide.ClientID%>'));
		}
		//AddHPSection();
	}
function UpdatePopUp(path, viewName)
{
    var qstring = "?Action=EditColumns&ViewName="+viewName+"&hiddenColumns="+document.getElementById('<%= in_leftcol_hide.ClientID %>').value;
    
    var req2 = window.XMLHttpRequest? 
	new XMLHttpRequest() : 
	new ActiveXObject("Microsoft.XMLHTTP");
	
	req2.onreadystatechange = function() 
	{
			if (req2.readyState != 4 ) return ;
			if (req2.readyState == 4)
			{
				if (req2.status != 200)
				{
					alert("There was a problem retrieving the XML data:\n" + req2.statusText + "\n HTTP Code: " + req2.status);
				}
				else
				{
				    //refresh
				}
			}
	}
	req2.open("GET", path + qstring, true);
	req2.send(null);    
}
</script>
<asp:Panel runat="server" ID="mc_grid_columnEditor" Width="700px">
		<div class="MCManagerCss">
			<div style="float:left;">
				<table cellpadding="0" cellspacing="0" class="text">
					<tr>
						<td valign="top">
							<b>All columns:</b><br />
							<asp:ListBox ID="lbLeftColumn" Runat="server" Rows="7" CssClass="text" Width="200px"></asp:ListBox>
							<input type="hidden" id="in_leftcol_hide" runat="server"/>
						</td>
					</tr>
				</table>
			</div>
			<div style="float: left; padding-left: 30px;">
				<input type="button" id="btnMove" value=">>>" runat="server" style=" margin-top: 50px;"   />
			</div>
			<div style="float:left;">
				<table cellpadding="0" cellspacing="0" class="text">
					<tr>
						<td style="vertical-align: middle; text-align: center; width: 60px;">
						</td>
						<td valign="top">
							<b>Visible columns:</b><br />
							<asp:ListBox ID="lbRightColumn" Runat="server" Rows="7" CssClass="text" Width="200px"></asp:ListBox>
							<input type="hidden" id="in_rightcol_hide" runat="server"/>
						</td>
					</tr>
				</table>
			</div>
			<div style="clear: left; margin-right: 80px; padding-top: 7px;" align="right">
				<asp:LinkButton runat="server" ID="btnSave" Text="Save" CausesValidation="false" />
				<asp:LinkButton runat="server" ID="btnCancelPopup" Text="Cancel"  CausesValidation="false" />
			</div>
		</div>
</asp:Panel>


<ajaxToolkit:ModalPopupExtender runat="server" ID="mc_grid_columnEditor_Ext"
	PopupControlID="mc_grid_columnEditor"
	TargetControlID="btnRefresh"
	BackgroundCssClass="modalBackground" 
	DropShadow="true"
	X="300"
	Y="200"	
	>
</ajaxToolkit:ModalPopupExtender>

<asp:Button ID="btnRefresh" runat="server" style="display:none;" />

<script type="text/javascript">
//Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler_ColumnEditor);
//function EndRequestHandler_ColumnEditor(sender, args)
//{
//    InitColumnEditor();
//}

function LeftColumnClick(eventArg)
{
    var obj = document.getElementById('<%=lbRightColumn.ClientID%>');
    for (var i = 0; i < obj.options.length; i++)
    {
        if (obj.options[i].selected)
        {
            obj.options[i].setAttribute('selected', 'false');
            obj.options[i].selected = false;
        }
    }
    
    var btn = document.getElementById('<%= btnMove.ClientID %>');
    btn.value = '>>>';
    btn.onclick = function () { RemoveHPSection('left'); }
    //Sys.UI.DomEvent.addHandler(btn, "click", RemoveHPSection);
}

function RightColumnClick(eventArg)
{
    var obj = document.getElementById('<%=lbLeftColumn.ClientID%>');
    for (var i = 0; i < obj.options.length; i++)
    {
        if (obj.options[i].selected)
        {
            obj.options[i].selected = false;
            obj.options[i].setAttribute('selected', 'false');
        }
    }
    
    var btn = document.getElementById('<%= btnMove.ClientID %>');
    btn.value = '<<<';    
    btn.onclick = function () { RemoveHPSection('right'); }
}

function InitColumnEditor()
{
    Sys.UI.DomEvent.addHandler(document.getElementById('<%=lbLeftColumn.ClientID%>'), "click", LeftColumnClick);
    Sys.UI.DomEvent.addHandler(document.getElementById('<%= lbRightColumn.ClientID %>'), "click", RightColumnClick);
}

</script>