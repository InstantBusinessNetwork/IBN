function framePrint()
{
	parent.right.focus();
	parent.right.print();
}

function showTheHours(theHour) 
{
	if (Is24Hours || (theHour > 0 && theHour < 13)) {
		return (theHour)
	}
	if (theHour == 0) {
		return (12)
	}
	return (theHour-12) 
}

function showZeroFilled(inValue) {
	if (inValue > 9) {
		return ":" + inValue
	}
	return ":0" + inValue
}

function showAmPm(theHour) {
	if (Is24Hours) {
		return ("")
	}

	if (theHour < 12) {
		return (" AM")
	}
	return (" PM")
}

function showTheTime() {
	now = new Date();			
	cHours = now.getUTCHours();
	cMinutes = now.getUTCMinutes();
	
	MinutesOffset = cHours*60 + cMinutes - TimeOffset;
	Hours = Math.floor(MinutesOffset / 60);
	Minutes = MinutesOffset - Hours * 60;
	
	if (Hours > 23) Hours = Hours - 24;
	if (Hours < 0 ) Hours = 24 + Hours;
	
	var spn = document.getElementById('timeSpan');
	if(spn)
	{
		spn.innerHTML = showTheHours(Hours) + showZeroFilled(Minutes) + showAmPm(Hours);
	}
	setTimeout("showTheTime()", 3000);
}