document.forms(0).attachEvent('onsubmit', IBN_StartTimer);
function IBN_StartTimer()
{
	window.setTimeout(IBN_DisableButtons, 1);
}

function IBN_DisableButtons()
{
	var curInput;
	var allInputs = document.all.tags('input');
	for (var i = 0; i < allInputs.length; i++) 
	{
		curInput = allInputs[i];
		if (curInput.getAttribute('DisableOnPost') == 'true') 
		{
			curInput.disabled = true;
		}
	}
}