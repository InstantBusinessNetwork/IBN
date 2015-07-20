<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.FinanceSpreadSheet" Codebehind="FinanceSpreadSheet.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockControl" src="../../Modules/BlockControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="../../Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>

<link href='<%= ResolveUrl("~/styles/IbnFramework/dhtmlXGrid.css") %>' type=text/css rel=stylesheet>
<script type="text/javascript" src='<%= ResolveUrl("~/Scripts/dhtmlGrid/dhtmlXCommon.js") %>'></script>
<script type="text/javascript" src='<%= Mediachase.Ibn.Web.UI.WebControls.McScriptLoader.Current.GetScriptUrl("~/Scripts/dhtmlGrid/dhtmlXGrid.js", this.Page)%>'></script>
<script type="text/javascript" src='<%= ResolveUrl("~/Scripts/dhtmlGrid/dhtmlXGridCell.js") %>'></script>
<script type="text/javascript" src='<%= ResolveUrl("~/Scripts/dhtmlGrid/dhtmlXTreeGrid.js") %>'></script>
<script type="text/javascript" src='<%= ResolveUrl("~/Scripts/dhtmlGrid/dhtmlXGrid_drag.js") %>'></script>
<script type="text/javascript" src='<%= ResolveUrl("~/Scripts/dhtmlGrid/dhtmlXExpand.js") %>'></script>
<script type="text/javascript" src='<%= ResolveUrl("~/Scripts/dhtmlGrid/dhtmlXMovingSelection.js") %>'></script>

<script type="text/javascript">
//<![CDATA[
//Open popup
function OpenWindow(query,w,h,scroll)
{
	var l = (screen.width - w) / 2;
	var t = (screen.height - h) / 2;
	
	winprops = 'modal=1,resizable=0,height='+h+',width='+w+',top='+t+',left='+l;
	if (scroll) winprops+=',scrollbars=1';
	var f = window.open(query, "_blank", winprops);
}
//Parsing querystring
function get_querystring(name)   
{
   var tmp = ( location.search.substring(1) );
   var i   = tmp.toUpperCase().indexOf(name.toUpperCase()+"=");
   if ( i >= 0 )
      {
            tmp = tmp.substring( name.length+i+1 );
            i = tmp.indexOf("&");
            return tmp.substring( 0, (i>=0) ? i : tmp.length );
            return("");
      }
}

//Generate GUID
function newGuid()
{
    var _guid = "";
    for(var i = 0; i < 32; i++)
    _guid += Math.floor(Math.random() * 0xF).toString(0xF) + (i == 8 || i == 12 || i == 16 || i == 20 ? "-" : "")
    return _guid;
}

//Delete Row + Ajax callback
function dhtmlXGrid_DeleteRow(row_id)
{
	mygrid.deleteRow(row_id);
	var req2 = window.XMLHttpRequest? 
	new XMLHttpRequest() : 
	new ActiveXObject("Microsoft.XMLHTTP");
	req2.onreadystatechange = function() 
	{
			if (req2.readyState != 4 ) return ;
			if (req2.readyState == 4)
			{
				if (req2.status == 200)
				{
					loadResponseXml(req2.responseText.toString());
				}
				else
				{
					alert("There was a problem retrieving the XML data:\n" + req2.statusText);
				}
			}
	}
	var path = '<%= this.ResolveUrl("~/Modules/XmlForTreeView.aspx") %>';
	var qstring = '?action=deleterow&rowid='+row_id+'&ProjectId='+get_querystring("ProjectId")+'&ProjectFinance=1&cache_protection='+newGuid()+qstring2;
	req2.open("GET", path+qstring, true);
	req2.send(null);  
 
}

//New row + Ajax callback
function dhtmlXGrid_NewRow(row_id, count, img, img_del, del_msg)
{
	var _newrowid = newGuid();
	
	var onclick_handler = "\"if (confirm('"+del_msg+"')) dhtmlXGrid_DeleteRow('"+row_id+'-'+_newrowid +"');\"";
	
	mygrid.addRow(row_id+'-'+_newrowid, ['Newrow_'+row_id+count, ' <img src="'+img_del+'" onclick='+onclick_handler+'></img>'], null, row_id, img);
	mygrid.openItem(row_id);
	
	var req2 = window.XMLHttpRequest? 
	new XMLHttpRequest() : 
	new ActiveXObject("Microsoft.XMLHTTP");
	req2.onreadystatechange = function() 
	{
		if (req2.readyState != 4 ) return ;
		if (req2.readyState == 4)
		{
			if (req2.status == 200)
			{
				loadResponseXml(req2.responseText.toString());
			}
			else
			{
				alert("There was a problem retrieving the XML data:\n" + req2.statusText);
			}
		}
	}
	var path = '<%= this.ResolveUrl("~/Modules/XmlForTreeView.aspx") %>';
	var qstring = '?action=newrow&rowid='+row_id+'&newrowid='+_newrowid +'&ProjectId='+get_querystring("ProjectId")+'&ProjectFinance=1&cache_protection='+newGuid()+qstring2 ;
	req2.open("GET", path+qstring, true);
	req2.send(null); 
	//+mygrid.cells(row_id+'-'+_newrowid, 0).getValue()
	OpenWindow('<%= this.ResolveUrl("~/Projects/FinanceSpreadSheetPopUp.aspx") %>' + '?RowId=' + row_id + '-' + _newrowid + '&Value=!EMPTY!' + qstring2 + '&ProjectId=' + get_querystring("ProjectId") + '&Created=1', 350, 60, false);
}

function dhtmlXGrid_MoveRow(blockId, rowIdOld, rowIdNew)
{
	popup_show();
	var req2 = window.XMLHttpRequest? 
	new XMLHttpRequest() : 
	new ActiveXObject("Microsoft.XMLHTTP");
	req2.onreadystatechange = function() 
	{
		if (req2.readyState == 4)
		{
			popup_hide();
		}
	}
	var path = '<%= ResolveUrl("~/Modules/XmlForTreeView.aspx") %>';
	var qstring = '?action=moverow&blockId='+blockId+'&rowIdOld='+rowIdOld+'&rowIdNew='+rowIdNew +'&ProjectId='+get_querystring("ProjectId")+'&ProjectFinance=1&cache_protection='+newGuid()+qstring2 ;
	//alert('ajax request:'+path+qstring);
	req2.open("GET", path+qstring, true);
	req2.send(null); 

}

var xmlDoc;
var _newrowid;
var tempValueBefore; // <SPAN>/<div>... etc
var tempValueCell; // value after /..
var lastRowId, lastColId; //last edited cell
var compareMode = 0; //if == 0, 1 view mode show, if == 1, then 2 views mode show
var qstring2; //additional query string with information about year, spreadsheets. Set in GenerateScript();
var mygrid = null;
var gridScale = null; //control scale up/down for grid
var newColumnLength = null;

function loadResponseXml(xmlString)
{
// code for IE
if (window.ActiveXObject)
  {
  xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
  xmlDoc.async=false;
  xmlDoc.loadXML(xmlString);
  //alert(xmlString);
  get_xmlmessage()
  }
// code for Mozilla, etc.
else if (document.implementation && document.implementation.createDocument)
  {
  var oParser = new DOMParser();
  xmlDoc = oParser.parseFromString(xmlString, "text/xml");
  get_xmlmessage()
  }
}	

//parse response grid xml 
function get_xmlmessage()
{
	if (xmlDoc == null)
	{
		alert('Error loading XML data');
		return;
	}
	var action = xmlDoc.documentElement.getElementsByTagName("userdata");
	var changeList = xmlDoc.documentElement.getElementsByTagName("cell");
	
	if (changeList == null || action == null) {return;}
	
	//Update action
	if (action[0].childNodes[0].nodeValue == "update")
	{
		for (i = 0; i < changeList.length; i++)
		{
				var _tmpValue = mygrid.cells(changeList[i].attributes.getNamedItem("rowid").value, changeList[i].attributes.getNamedItem("columnindex").value).getValue();
					//alert(changeList[i].attributes.getNamedItem("rowid").value+'___'+changeList[i].attributes.getNamedItem("columnindex").value);
				if (_tmpValue.substr(0, 5) != "<div>") //esli cell != read-only 
					mygrid.cells(changeList[i].attributes.getNamedItem("rowid").value, changeList[i].attributes.getNamedItem("columnindex").value).setValue(changeList[i].childNodes[0].nodeValue);
				else
					mygrid.cells(changeList[i].attributes.getNamedItem("rowid").value, changeList[i].attributes.getNamedItem("columnindex").value).setValue("<div></div>"+changeList[i].childNodes[0].nodeValue);
					
				if (changeList[i].attributes.getNamedItem("font") != null)
				{
					mygrid.cells(changeList[i].attributes.getNamedItem("rowid").value, changeList[i].attributes.getNamedItem("columnindex").value).setTextColor(changeList[i].attributes.getNamedItem("font").value);
					
				}
				//mygrid.setCellTextStyle(changeList[i].attributes.getNamedItem("rowid").value,0,"color:red;");
		}
	}
}

//Show/Hide loading popup div
function popup_show()
{
	var obj = document.getElementById('waitMessagePopUp');
	if (obj != null)
		obj.style.display = 'block';
}

function popup_hide()
{
	var obj = document.getElementById('waitMessagePopUp');
	if (obj != null)
		obj.style.display = 'none';
	
	var obj = document.getElementById('gridbox');
	if (obj)
	{
		window.setTimeout(function () 
			{ 
				document.getElementById('gridbox').style.height = '399px'; 
				window.setTimeout(function() { document.getElementById('gridbox').style.height = '400px'; }, 100);
			}, 200);
		
	}
}

//GRID HANDLERS

//Grid drag'n'drop handlers
  function drag_f(r1,r2){
	if (mygrid.getParentId(r1) == r2) // dragged to first position
	{
		do {mygrid.moveRowUp(r1);} while (mygrid.rowsCol[mygrid.getRowIndex(r1)].idd != mygrid.rowsCol[mygrid.getRowIndex(r2)+1].idd);
		/*for (var i = 0; i < 15; i++)
		
		{ mygrid.moveRowUp(r1); }*/
		dhtmlXGrid_MoveRow(mygrid.getParentId(r1), r1, r2);
		return false;
	}
	return (mygrid.getParentId(r2) != 'null') &&( mygrid.getParentId(r1) == mygrid.getParentId(r2));
  }
  function drop_f(r1,r2){
	dhtmlXGrid_MoveRow(mygrid.getParentId(r2), r1, r2);
  }
  

//OnGridCellEdit Event handler
function GridCellEdit(status, rowid, index, newvalue, oldvalue) 
{ 
	//alert(status+' '+rowid+' '+index+' '+newvalue+' '+oldvalue);
	if (status == 2) 
	{ 
	if (breakEdit) 
	{ 
		breakEdit = false; 
		mygrid.cells(rowid, index).setValue(breakValue); 
		return breakValue;
	}
	
	var cellValue = mygrid.cells(rowid, index).getValue();
	if (cellValue.substr(0, 5).toUpperCase() == "<div>")
	{
		return false;
	}
	else
	{
		if (isNaN(cellValue)) {cellValue = '';}
	}
	
	if (cellValue.lastIndexOf('/') > -1)
	{
		cellValue = cellValue.substr(0, cellValue.lastIndexOf('/'));
	}
			popup_show();
			var req = window.XMLHttpRequest? 
			new XMLHttpRequest() : 
			new ActiveXObject("Microsoft.XMLHTTP");
			req.onreadystatechange = function() 
			{
				if (req.readyState != 4 ) return; 
				if (req.readyState == 4)
				{
					if (req.status == 200)
					{
						var xmlstr = req.responseText.toString();
						//alert(req.responseText.toString());
						//var path2 = '<%= ResolveUrl("~Modules/XmlForTreeView.aspx") %>';
						loadResponseXml(req.responseText.toString());
						popup_hide();
					}
					else
					{
						alert("There was a problem retrieving the XML data:\n" + req.statusText);
					}
				}
			}

		index=index-2;
		
		//if (index < 0) return;
		var path = '<%= ResolveUrl("~/Modules/XmlForTreeView.aspx") %>';
		//var qstring2 = '&FromYear='+get_querystring("FromYear")+'&ToYear='+get_querystring("ToYear")+'&BasePlanSlotId1='+get_querystring("BasePlanSlotId1")+'&BasePlanSlotId2='+get_querystring("BasePlanSlotId2")
		var qstring = '?action=update&rowid='+rowid+'&cellnumber='+index+'&value='+cellValue+'&ProjectId='+get_querystring("ProjectId")+'&ProjectFinance=1&cache_protection='+newGuid()+qstring2;
		req.open("GET", path + qstring, true);
		req.send(null);
		return cellValue;
	}// status == 2
	if (status == 0)
	{
		var _cellValue = mygrid.cells(rowid, index).getValue();
		tempValueBefore = null;
		//alert('cell edit');
		if (_cellValue.substr(0, 5).toUpperCase() == "<div>")
		{
			//alert('read only');
			return false;
		}
		else if(compareMode == 1)
		{
			if (_cellValue.lastIndexOf("/") > -1)
			{
				tempValueCell = _cellValue.substr(_cellValue.lastIndexOf("/"), _cellValue.length - _cellValue.lastIndexOf("/") + 1);
				mygrid.cells(rowid, index).setValue(_cellValue.substr(0, _cellValue.lastIndexOf("/")));
			}
			else
			{
				tempValueCell = "/0";
			}
			lastColId = index;
			lastRowId = rowid;
		}
		//alert('before true');
		return true;
	}
}
//]]>
</script>

<asp:Panel Runat="server" ID="divGrid" Visible="False">
<table cellpadding="0" width="100%" cellspacing="0">
	<tr align="center">
		<td align="center">
			<!-- <ibn:blockheader id="secHeader" runat="server"/> -->
			<table cellpadding="0" cellspacing="0" width="100%" class="ibn-propertysheet">
				<tr>
					<td valign="top" align="center">
					<table runat="server" align="center" id="FilterTable" class="ibn-alternating ibn-navline" cellspacing="0" cellpadding="7" width="100%" border="0">
						<tr class="text">
							<td style="width: 300px"><asp:DropDownList Runat="server" ID="ddPrimarySheet" width="300px"/></td>
							<td >/ <asp:DropDownList Runat="server" ID="ddSecondarySheet" width="300px"/></td>
						</tr>
						<tr>
							<td style="width: 320px">
								<table cellpadding="0" cellspacing="0" width="100%" class="text">
									<tr>
										<td width="50px"><b><%= LocRM.GetString("From") %>: </b></td>
										<td width="90px"><asp:TextBox CssClass="text" Width="85px" onfocus="javascript:GridFocused = false;" Runat="server" ID="tbFrom"></asp:TextBox></td>
										<td width="20px"><asp:RegularExpressionValidator Runat="server" ErrorMessage="*" ControlToValidate="tbFrom" ValidationExpression="\d+" ID="Regularexpressionvalidator1"/></td>

										<td width="50px" align="right" style="padding-right:5px;"><b><%= LocRM.GetString("To") %>: </b></td>
										<td width="90px"><asp:TextBox CssClass="text" onfocus="javascript:GridFocused = false;" Runat="server" ID="tbTo" width="85px" ></asp:TextBox></td>
										<td width="20px"><asp:RegularExpressionValidator Runat="server" ErrorMessage="*" ControlToValidate="tbTo" ValidationExpression="\d+" ID="Regularexpressionvalidator2"/><asp:CompareValidator Runat="server" Operator="GreaterThanEqual" ErrorMessage="*" ControlToValidate="tbTo" ControlToCompare="tbFrom" Type="Integer" ID="Comparevalidator3" /><asp:CompareValidator Runat="server" Operator="LessThanEqual" ErrorMessage="*" ControlToValidate="tbFrom" ControlToCompare="tbTo" Type="Integer" ID="Comparevalidator1" /></td>
									</tr>
									<tr>
										<td width="160px" colspan="3"><asp:CompareValidator Runat="server" Operator="GreaterThan" ValueToCompare="1995" ControlToValidate="tbFrom" Type="Integer" ID="cvMinYear" /> </td>
										<td width="160px" colspan="3"><asp:CompareValidator Runat="server" Operator="LessThan" ValueToCompare="2015" ControlToValidate="tbTo" Type="Integer" ID="cvMaxYear" /></td>
									</tr>
								</table>
							</td>
							
						</tr>

						<tr class="text">
							<td align="left"><!-- <asp:Button Runat="server" ID="btnDeactivate" Text="Deactivate Finanses"></asp:Button> --></td>
							<td align="right">
							<btn:imbutton class="text" runat="server" id="btnDeactivateFinance" Visible="false" />
							<btn:imbutton class=text runat="server" id="btnApplyFilter"></btn:imbutton>
							</td>
						</tr>
					</table>
					</td>
				</tr>
				<tr>
					<td valign="top" onclick="javascript:ProcessMouseClick();" ondblclick="javascript:ProcessMouseDblClick();"> <!-- -->
						<div style="position: relative; margin-right: 5px;margin-top: 5px;">
							<div runat="server" id="panelScale" style="background-color:#E1ECFC; position: absolute; top: 0px; left: 0px; width: 100%; height: 29px; border-Bottom: solid 1px #808080;">&nbsp;</div> <!--filter: alpha(opacity=50);filter: progid:DXImageTransform.Microsoft.Alpha(opacity=50);-moz-opacity: 0.50;opacity: 0.5 E1ECFC;-->
								<div id="waitMessagePopUp" style="background-color:#EEEEEE; position: absolute; top: 100px; left: 175px; width: 180px; height: 50px; border: solid 1px black; display: none;" align="center">
									<!-- <%= LocRM.GetString("tLoadingMsg") %> -->
									<table cellspacing="0" cellpadding="0" height="100%" width="100%"> 
									<tr align="center"><td valign="center">
									<img align="bottom" border="0" src='<%= ResolveUrl("~/Layouts/Images/cl-loading.gif") %>'>
									</td></tr>
									</table>
								</div>
								<div style="padding-top: 15px;">&nbsp;</div>
							<div id="gridbox" width="100%" height="400px" style="background-color:white;overflow:hidden;"></div>
						</div>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
</asp:Panel>

<asp:Panel Runat="server" ID="divActivate" Visible="False">
<table class="ibn-propertysheet" width="100%" align="center">
	<tr id='ibn-finance-activate' align="center">
		<td style="padding: 10px; margin: 10px;" >
		<table cellspacing="0" cellpadding="0" class="text">
			<tr >
				<td ><%= LocRM.GetString("ActivateMsg2") %></td>
				
			</tr>
		</table>
		<table cellspacing="0" cellpadding="0" class="text" style="padding-top: 15px;">
			<tr >
				<td onclick="document.getElementById('ibn-finance-settings').style.display='block'; document.getElementById('ibn-finance-activate').style.display='none';" onmouseover="this.style.color='Blue'; this.style.cursor='pointer';" onmouseout="this.style.color='Red';"><input type="button" value='<%= LocRM.GetString("Activate") %>' onclick="return false;" /></td> <!--&nbsp;<b><u><%= LocRM.GetString("ActivateHere") %></u></b>.-->
			</tr>
		</table>
		<%--		<table width="100%" align="center" class="text">
			<tr style="color: Red;" align="center"><td ><%= LocRM.GetString("ActivateMsg2") %></td></tr>	 
		</table>--%>
		</td>
	</tr>
	<tr id='ibn-finance-settings' style="display: none" align="center">
		<td align="center">
			<!--<ibn:blockheader id="activateSettings" runat="server"/>-->
			<table cellpadding="5" cellspacing="0" width="100%" class="ibn-propertysheet" align="center">
			<tr class="ibn-propertysheet" align="center">
				<td align="right" > <b><%= LocRM.GetString("SpreadSheetType") %>:</b> </td>
				<td align="left" ><asp:DropDownList Runat="server" ID="ddDocType"></asp:DropDownList></td>
			</tr>
			<tr class="ibn-propertysheet" align="center">
				<td align="right" > <b><%= LocRM.GetString("Template") %>:</b> </td>
				<td align="left"><asp:DropDownList Runat="server" ID="ddTemplate" ></asp:DropDownList></td>
			</tr>
			<tr class="ibn-propertysheet" align="center">
				<td align="right"><btn:imbutton class=text runat="server" id="btnActivate"></btn:imbutton></td>
				<td align="left"><btn:imbutton class=text runat="server" id="btnCancel" onclick="document.getElementById('ibn-finance-settings').style.display = 'none'; document.getElementById('ibn-finance-activate').style.display = 'block';" ></btn:imbutton></td>
			</tr>
			</table>
		</td>
	</tr>
</table>
	
</asp:Panel>