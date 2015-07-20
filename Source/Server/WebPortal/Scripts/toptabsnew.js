var startPos = 350;
var toolbarHeight = 25;
var bannerHeight = 60;

function HideFrames2(e)
{
	try
	{
		var isToolbar = 0;
		e = (e) ? e : ((window.event) ? event : null);
		if (e)
		{
			var src = (e.target) ? e.target : ((e.srcElement) ? e.srcElement : null);
			if (src)
			{
				if(src.className && (src.className=="cellclass" || src.className=="hovercellclass" || src.className=="btndown" || src.className=="btndown2")) return;
				
				var obj = src;
				while(obj.parentNode)
				{
					obj = obj.parentNode;
					if (obj.className && obj.className.indexOf("x-toolbar") >= 0)
					{
						isToolbar = 1;
						break;
					}	
				}
			}
		}
		
		if (isToolbar == 0)
		{
			if (this.HideMenu)
				this.HideMenu();
			if (parent && parent.HideMenu)
				parent.HideMenu();
		
			var oCurFrame = document.getElementById("frmList");
			if(!oCurFrame)
				oCurFrame = parent.document.getElementById("frmList");
				
			if (oCurFrame && oCurFrame.style.display == "block")
			{
				oCurFrame.style.display = "none";

				showSelects();
			}
		}
	}
	catch (e) {}
}

function ShowFrame2(url)
{
	try
	{
		if (window.event)
			window.event.cancelBubble = true;
		oCurFrame = document.getElementById("frmList");
		if (!oCurFrame)	
			return;

		var reload = true;
		if (oCurFrame.src 
			&& oCurFrame.src.indexOf("reload=false") > 0
			&& oCurFrame.src.length > 50 
			&& url.length > 50 
			&& oCurFrame.src.substr(oCurFrame.src.length - 50) == url.substr(url.length - 50))
		{
			reload = false;
		}

		if (reload)
		{
			oCurFrame.src = "";
			oCurFrame.src = url;
		}
		if (oCurFrame.style.display == "none")
		{
			var bannerTable = document.getElementById("TopPlaceDiv");
			if (bannerTable && bannerTable.style && bannerTable.style.display == "none")
			{
				oCurFrame.style.top = toolbarHeight + "px";
			}
			else
			{
				oCurFrame.style.top = toolbarHeight + bannerHeight + "px";
			}
			
			iCurPos = startPos;
			oCurFrame.style.height = iCurPos + "px";
			oCurFrame.style.display = "block";
			
//			iTimerID = window.setInterval(Enhance, 1);
		}
	}
	catch(e) {}
}
