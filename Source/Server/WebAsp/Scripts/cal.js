
function ShowCal(controlName, relativeName)
{

	var eSrc = document.getElementById(relativeName);
	var oTotalOffset = GetTotalOffset(eSrc);
	var left = 300; //oTotalOffset.Left + 2;
	var top = 295; //oTotalOffset.Top + eSrc.offsetHeight; 
	
	calendar_window=window.open('../Modules/DatePicker.aspx?formname=' + controlName + '','calendar_window','width=185,height=155,left=' + left + ',top='+ top);
	calendar_window.focus();
}

function GetTotalOffset(eSrc)
{
	this.Top = 0;
	this.Left = 0;
	while (eSrc)
	{
		this.Top += eSrc.offsetTop;
		this.Left += eSrc.offsetLeft;
		eSrc = eSrc.offsetParent;
	}
	return this;
} 
