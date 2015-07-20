<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CalendarControl.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Calendar.Modules.CalendarControl" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register Assembly="Mediachase.AjaxCalendar" Namespace="Mediachase.AjaxCalendar"
	TagPrefix="mc" %>
<%@ Register TagPrefix="ibn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>	
<%@ Register TagPrefix="ibn" TagName="DTCC" src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register Src="~/Apps/MetaUI/Toolbar/MetaToolbar.ascx" TagPrefix="ibn" TagName="MetaToolbar" %>
<link type="text/css" rel="stylesheet" href='<%= ResolveClientUrl("~/Styles/IbnFramework/TimeSheetStyle.css")%>' />
<link type="text/css" rel="stylesheet" href='<%= Mediachase.Ibn.Web.UI.WebControls.McScriptLoader.Current.GetScriptUrl("~/Styles/IbnFramework/grid.css", this.Page)%>' />
<style type="text/css">
	.ibn-propertysheet2{
		color: #ffffff;
	}
	.ibn-propertysheet2 a {
		text-decoration: none; 
		color: #ffffff;
	}
	.ibn-propertysheet2 a:hover {
		text-decoration: underline; 
		color: #dddddd;
	}
</style>
<script type="text/javascript">
    var resizeFlag = false;
    function LayoutResizeHandler(sender, eventArgs)
    {
		//alert('resized fired: '+eventArgs._name+':'+eventArgs._blockHeight+'x'+eventArgs._blockWidth);

		if (eventArgs._name == "spDetailsH")
		{
			var s = new Object();
			s.ViewName = Request.QueryString.Item("viewName").toString();
			
			Mediachase.Ibn.Web.UI.WebServices.ListHandler.LayoutResized(Sys.Serialization.JavaScriptSerializer.serialize(s), "south", eventArgs._blockHeight);
		}
    }
    
    function LayoutEndResizeHadler(sender, eventArgs)
    {
		//resizeFlag = true;
    }
    
    function executeEvalScript(scriptToExecute)
	{
		eval(scriptToExecute);
	}
</script>

<ibn:McDock runat="server" ID="DockTop" DefaultSize="28" Anchor="top" EnableSplitter="false">
	<DockItems>
		<div style="padding-left: 7px; padding-right: 7px; padding-top: 2px;">
		<table border="0" cellpadding="0" cellspacing="0" style="width:100%; " class="text">
			<tr>
				<td style="width:30px;">
					<asp:ImageButton runat="server" ID="imbPrev" Width="29px" Height="17px" ImageAlign="AbsMiddle" ImageUrl="~/Images/TimeTracking/left.gif" BackColor="#7799BB" />
				</td>
				<td style="width:30px;">
					<asp:ImageButton runat="server" ID="imbNext" Width="29px" Height="17px" ImageAlign="AbsMiddle" ImageUrl="~/Images/TimeTracking/right.gif" BackColor="#7799BB" />
				</td>
				<td style="width:250px;">
					<asp:Button runat="server" ID="lbToday" Text="<%$Resources: IbnFramework.Calendar, Today%>" CssClass="text"/>
				</td>
				<td >
					<ibn:DTCC AutoPostBack="true" id="dtcWeek" runat="server" SelectedMode="Week" ShowImageButton="false" DateCssClass="IbnCalendarText" ReadOnly="true" DateFormat="d MMM yyyy"></ibn:DTCC>
				</td>
				
			</tr>
		</table>
		</div>	
	</DockItems>
</ibn:McDock>

<div class="filter" style="padding: 5px;">
<div style=" border: solid 1px #6B92CE" >
		<div style="border-bottom: solid 1px #6B92CE">
		<ibn:MetaToolbar runat="server" ID="ctrlToolbar" PlaceName="Calendar" />
		</div>
	<div style="height: 3px;">&nbsp;</div>
	<mc:MediachaseAjaxCalendar runat="server" ID="CalendarCtrl" DrillDownEnabled="true" >
		<DataSource AutoRegisterWebServiceReference="false" ItemsWebServiceFullName="Mediachase.Ibn.Web.UI.Calendar.WebServices.Default" ItemsWebServicePath="~/Apps/Calendar/WebServices/Default.asmx" />
		<MultiDayView UseDefaultCreateHandler="false" UseDefaultDeleteHandler="false" 
			UseDefaultUpdateHandler="false" UseAdjustedEndDate="true"
			runat="server" DayStartHour="6" DayEndHour="19" TodayBackgroundColor="#FFFFCC" 
			GridRowBorderStyle="Solid" GridRowBorderColor="#DDDDDD"
			GridRowBorderWidth="1" GridRowAlternativeBorderStyle="Dotted"
			GridRowAlternativeBorderColor="#DDDDDD" GridColumnSeparatorBorderColor="#DDDDDD"
			GridColumnSeparatorBorderStyle="Solid" GridColumnSeparatorBorderWidth="1"
			GridColumnSeparatorWidth="1" SelectionColor="#CCCCCC" HeaderTodayBackgroundColor="#88AACC"
			HeaderDateFormat="ddd, dd\/MM" ViewMode="Week" EventBarMaxHeight="100"> 
			<HeaderStyle Font-Names="Arial" Font-Size="11px" Height="15px" BackColor="#C3D9FF"  />
			<TimeColumnStyle Font-Names="Arial" Font-Size="11px" BackColor="#E8EEF7"/>
			<EventBarStyle BackColor="#E8EEF7"  Height="0px"/>
			<GridStyle BackColor="#FFFFFF" Height="650px" />
		</MultiDayView>
		<MonthView UseDefaultCreateHandler="false" UseDefaultDeleteHandler="false" 
			UseDefaultUpdateHandler="false" UseAdjustedEndDate="true"
			runat="server" TodayBackgroundColor="#FFFFCC" GridRowBorderStyle="Solid"
			GridRowBorderColor="#DDDDDD" GridRowBorderWidth="1" 
			GridColumnSeparatorBorderColor="#DDDDDD" GridColumnSeparatorBorderStyle="Solid"
			GridColumnSeparatorBorderWidth="1" GridColumnSeparatorWidth="0"
			SelectionColor="#CCCCCC">
			<HeaderStyle Font-Names="Arial" Font-Size="11px" Height="15px" BackColor="#C3D9FF"  />
			<GridStyle Font-Names="Verdana" BackColor="White" Height="500px" />
		</MonthView>
		<YearView UseDefaultCreateHandler="false" UseDefaultDeleteHandler="false" 
			UseDefaultUpdateHandler="false" UseAdjustedEndDate="true"
			runat="server" TodayBackgroundColor="#FFFFCC" GridRowBorderColor="#DDDDDD" GridRowBorderWidth="1" 
			GridColumnSeparatorBorderColor="#DDDDDD" GridColumnSeparatorBorderStyle="Solid"
			GridColumnSeparatorBorderWidth="1" GridColumnSeparatorWidth="0">
			<HeaderStyle Font-Names="Arial" Font-Size="11px" Height="15px" BackColor="#C3D9FF" />
			<GridStyle Font-Names="Verdana" BackColor="WHITE" />
		</YearView>
	</mc:MediachaseAjaxCalendar>
</div>
</div>

<script type="text/javascript">

function mcCalendar_getLayoutBaseDiv()
{
	var divs = document.getElementsByTagName('DIV');
	
	for (var i = 0; i < divs.length; i++)
	{
		if (divs[i].className.indexOf('LayoutBase') > -1)
		{
			//alert(divs[i].firstChild.innerHTML);
			return divs[i].firstChild;
		}
	}
	
	return null;
}

function mcCalendar_getForm(p)
{
	var obj = p;
	
	while (obj.parentNode != null)
	{
		if (obj.nodeType == 1 && obj.tagName == 'FORM')
			return obj;
			
		obj = obj.parentNode;
	}
	
	return null;
}

function mcCalendar_countTotalHeight(node)
{
	var sum = 0;
	
	for (var i = 0; i < node.childNodes.length; i++)
	{
		if (node.childNodes[i].nodeType == 1)
		{
			if (node.childNodes[i].tagName == 'SPAN' && node.childNodes[i].childNodes.length > 0)
			{
				sum += node.childNodes[i].firstChild.offsetHeight
				continue;
			}
					
			if (!isNaN(node.childNodes[i].offsetHeight))
				sum += node.childNodes[i].offsetHeight;
		}
	}
	
	return sum;
}

RegisterHandler = function()
{	
	var view = $find('<%=CalendarCtrl.ClientViewId%>');
	if(view!=null && typeof(view.get_ScrollContainerId)=='function')
	{
		var sc = $get(view.get_ScrollContainerId());
		var grid = $get(view.get_CalendarGrid().get_ContainerId());

		if(sc!=null && grid!=null)
		{
			if(document.documentElement)
			{
				var maxGridHeight = parseInt(grid.style.height);
				var minGridHeight = 100;
				if(Sys.Browser.agent == Sys.Browser.InternetExplorer)
				{
					//normal window
					if(document.documentElement.offsetHeight>document.documentElement.scrollHeight)
					{
						var dh = document.documentElement.offsetHeight-document.documentElement.scrollHeight;
						//window.status = '1: ' + dh;
						var curH = parseInt(sc.style.height);
						if(curH+dh<maxGridHeight)
						{
							sc.style.height = curH+dh+"px";
						}
						else
							sc.style.height = maxGridHeight+"px";
					}
					else//scrollable window
					{
					//TODO: Fix. Get height from layout container
						var dh = document.documentElement.scrollHeight - document.documentElement.offsetHeight;
						//window.status = '2: ' + dh;
						var curH = parseInt(sc.style.height);
						if(curH-dh>minGridHeight)
						{
							sc.style.height = curH-dh+"px";
						}
						else
							sc.style.height = minGridHeight+"px";
					}
				}
				if(Sys.Browser.agent == Sys.Browser.Firefox)//Firefox
				{
					//alert('can be rise for: ' + canBeMax);
				
					//normal window
					if(document.documentElement.scrollHeight>document.documentElement.offsetHeight)
					{
						var dh = document.documentElement.scrollHeight - document.documentElement.offsetHeight;
						//window.status = '3: ' + dh + ' | ' + document.documentElement.tagName;
						var curH = parseInt(sc.style.height);
						if(curH+dh<maxGridHeight)
						{
							sc.style.height = curH+dh+"px";
						}
						else
							sc.style.height = maxGridHeight+"px";
					}
					//scrollable window
					if(document.documentElement.scrollHeight>document.documentElement.clientHeight)
					{
						//TODO: Fix. Get height from layout container
						var dh = document.documentElement.scrollHeight - document.documentElement.clientHeight;						
						//window.status = '4: ' + dh + ' | ' + document.documentElement.tagName;
						
						var curH = parseInt(sc.style.height);
						if(curH-dh>minGridHeight)
						{
							sc.style.height = curH-dh+"px";
						}
						else
							sc.style.height = minGridHeight+"px";
					}
					
//					var canBeMax = 0;
//					canBeMax = Math.min(sc.scrollHeight - sc.offsetHeight, mcCalendar_getLayoutBaseDiv().offsetHeight - mcCalendar_countTotalHeight(mcCalendar_getLayoutBaseDiv()));

				}
				
				window.setTimeout(function() 
				{ 
					var canBeMax = 0;
					canBeMax = Math.min(sc.scrollHeight - sc.offsetHeight, mcCalendar_getLayoutBaseDiv().offsetHeight - mcCalendar_countTotalHeight(mcCalendar_getLayoutBaseDiv()));
					//window.status = 'scroll = ' + sc.scrollHeight + '| offset = ' + sc.offsetHeight + '| total = ' + mcCalendar_getLayoutBaseDiv().offsetHeight + '| innerHeight = ' + mcCalendar_countTotalHeight(mcCalendar_getLayoutBaseDiv()) + ' | canBeMax:' + canBeMax; 
					
					if (canBeMax >= 1)
					{
						//window.status = 'old = ' +sc.style.height + ' | canBeMax ' + canBeMax;
						sc.style.height = parseInt(sc.style.height) + canBeMax + 'px';
						canBeMax = Math.min(sc.scrollHeight - sc.offsetHeight, mcCalendar_getLayoutBaseDiv().offsetHeight - mcCalendar_countTotalHeight(mcCalendar_getLayoutBaseDiv()));
					}
				}, 300);	
			}
		}
	}
	else
		setTimeout(RegisterHandler,  300);
	
}
$addHandler(window, "load", RegisterHandler);
$addHandler(window, "resize", RegisterHandler);

function MainScript() {
	var view = $find('<%=CalendarCtrl.ClientViewId%>');
	if (view) 
	{

		view.add_itemCreated(Function.createDelegate(this, OnItemCreated));
		view.add_itemUpdated(Function.createDelegate(this, OnItemUpdated));
		view.add_itemDeleted(Function.createDelegate(this, OnItemDeleted));
		
		var eventbar = $find('<%=CalendarCtrl.ClientEventBarId%>');
		if (eventbar) {
			eventbar.add_itemCreated(Function.createDelegate(this, OnItemCreated));
			eventbar.add_itemUpdated(Function.createDelegate(this, OnItemUpdated));
			eventbar.add_itemDeleted(Function.createDelegate(this, OnItemDeleted));
		}
	}
	else
	{
		setTimeout(MainScript, 500);
		return;
	}
}

OnItemCreated = function(sender, args) {
	if (args.MouseEvent.type == 'mouseup' && sender.get_ActionName() == "") {
		var sd = args.StartDate;
		var ed = args.EndDate;
		if (ed.getMinutes() == 59)
			ed = new Date(ed.getTime() + 1000);
		var s = "<%= sCreate %>";
		s = s.replace("%%START%%", sd.format("yyyy.M.d.H.m.s"));
		s = s.replace("%%END%%", ed.format("yyyy.M.d.H.m.s"));
		eval(s);
	}
}

var globalArgs = null;

OnItemUpdated = function(sender, args) {
	if (args.MouseEvent.target.tagName == "A")
		return false;
	if (args.MouseEvent.type == 'mouseup' && sender.get_ActionName() == "") {
		var s = "<%= sUpdate %>";
		s = s.replace("%%UID%%", args.Uid);
		eval(s);
	}
	if (args.MouseEvent.type == 'mouseup' && sender.get_ActionName() == "move") {
		var im = sender.get_ItemsManager();
		im.UpdateItem(args.Uid, args.Title, args.StartDate, args.EndDate, args.Description, args.IsAllDay, args.Extensions, args.CalendarExtension);
	}
	if (args.MouseEvent.type == 'mouseup' && sender.get_ActionName() == "resize") {
		if (args.Extensions && args.Extensions == "1") {
			var s = "<%= sUpdateConfirm %>";
			s = s.replace("%%UID%%", args.Uid);
			globalArgs = args;
			eval(s);
		}
		else {
			var im = sender.get_ItemsManager();
			im.UpdateItem(args.Uid, args.Title, args.StartDate, args.EndDate, args.Description, args.IsAllDay, args.Extensions, args.CalendarExtension);
		}
	}
}

OnItemDeleted = function(sender, args) {
	var s = "<%= sDelete %>";
	s = s.replace("%%UID%%", args.Uid);
	eval(s);
}

OnItemCreatedPostFactum = function(params) {
	var view = $find('<%=CalendarCtrl.ClientViewId%>');
	var im = view.get_ItemsManager();
	im.LoadItems(im.get_ViewStartDate(), im.get_ViewEndDate(), im.get_CalendarExtension());
}

OnItemUpdateConfirmedPostFactum = function(params) {
	var obj = Sys.Serialization.JavaScriptSerializer.deserialize(params);
	if (obj && obj.CommandArguments && obj.CommandArguments.Uid) {
		var view = $find('<%=CalendarCtrl.ClientViewId%>');
		var im = view.get_ItemsManager();
		if (obj.CommandArguments.Uid == 'null') {
			im.LoadItems(im.get_ViewStartDate(), im.get_ViewEndDate(), im.get_CalendarExtension());
		}
		else {
			var ce = im.get_CalendarExtension();
			if (globalArgs == null)
				return;
			var args = globalArgs;
			globalArgs = null;
			im.UpdateItem(obj.CommandArguments.Uid, args.Title, args.StartDate, args.EndDate, args.Description, args.IsAllDay, "resize", ce);
		}
	}
}

OnItemDeletedPostFactum = function(params) {
	var view = $find('<%=CalendarCtrl.ClientViewId%>');
	var im = view.get_ItemsManager();
	var obj = Sys.Serialization.JavaScriptSerializer.deserialize(params);
	if (obj && obj.CommandArguments && obj.CommandArguments.Uid) {
		im.DeleteItem(obj.CommandArguments.Uid, im.get_CalendarExtension());
	}
}

OnSummaryPostFactum = function(params) {
	var view = $find('<%=CalendarCtrl.ClientViewId%>');
	var im = view.get_ItemsManager();
	var obj = Sys.Serialization.JavaScriptSerializer.deserialize(params);
	if (obj && obj.CommandArguments && obj.CommandArguments.Uid && obj.CommandArguments.action) {
		if (obj.CommandArguments.action == "delete")
			im.DeleteItem(obj.CommandArguments.Uid, im.get_CalendarExtension());
		if (obj.CommandArguments.action == "edit") {
			var s = "<%= sEdit %>";
			s = s.replace("%%UID%%", obj.CommandArguments.Uid);
			setTimeout("eval(\"" + s + "\")", 500);
		}
	}
}

MainScript();
</script>