using System;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Web.UI;

namespace Mediachase.UI.Web.Modules
{
	public partial class FormHelper : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Common.Resources.strHelp", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		#region ResKey
		public string ResKey
		{
			set
			{
				lblText.Text = LocRM.GetString(value);
			}
		}
		#endregion

		#region Width
		public string Width
		{
			set
			{
				mainDiv.Style.Remove(HtmlTextWriterStyle.Width);
				mainDiv.Style.Add(HtmlTextWriterStyle.Width, value);
			}
		}
		#endregion

		#region Position
		private string position = "";
		/// <summary>
		/// Gets or sets the position.
		/// Available values are:
		///   TL - Top Left
		///   T - Top
		///   TR - Top Right
		///   L - Left
		///   R - Right
		///   BL - Bottom Left
		///   B - Bottom
		///   BR - Bottom Right
		/// </summary>
		/// <value>The position.</value>
		public string Position
		{
			get
			{
				return position;
			}
			set
			{
				position = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			AddScript();

			mainDiv.Attributes.Add("IsHelp", "1");
			imgClose.Attributes.Add("CloseHelp", "1");
		}

		#region AddScript
		private void AddScript()
		{
			if (!Page.ClientScript.IsClientScriptBlockRegistered("Help"))
			{
				StringBuilder sScript = new StringBuilder();

				sScript.AppendLine("function ShowHelp(obj, layout, e) {");
				sScript.AppendLine("	layout = layout.toUpperCase();");
				sScript.AppendLine("	var divObj = obj.nextSibling;");
				sScript.AppendLine("	if (divObj) {");
				sScript.AppendLine("		var objTop = getObjectTop(obj);");
				sScript.AppendLine("		var objLeft = getObjectLeft(obj);");
				sScript.AppendLine("		var objHeight = getObjectHeight(obj);");
				sScript.AppendLine("		var objWidth = getObjectWidth(obj);");
				sScript.AppendLine("		divObj.style.display = \"block\";");
				sScript.AppendLine("		var divHeight = getObjectHeight(divObj);");
				sScript.AppendLine("		var divWidth = getObjectWidth(divObj);");
				sScript.AppendLine("		switch (layout) {");
				sScript.AppendLine("			case \"TL\":");
				sScript.AppendLine("				divObj.style.top = objTop - divHeight + \"px\";");
				sScript.AppendLine("				divObj.style.left = objLeft - divWidth + \"px\";");
				sScript.AppendLine("				break;");
				sScript.AppendLine("			case \"T\":");
				sScript.AppendLine("				divObj.style.top = objTop - divHeight + \"px\";");
				sScript.AppendLine("				divObj.style.left = objLeft + objWidth / 2 - divWidth / 2 + \"px\";");
				sScript.AppendLine("				break;");
				sScript.AppendLine("			case \"TR\":");
				sScript.AppendLine("				divObj.style.top = objTop - divHeight + \"px\";");
				sScript.AppendLine("				divObj.style.left = objLeft + objWidth + \"px\";");
				sScript.AppendLine("				break;");
				sScript.AppendLine("			case \"L\":");
				sScript.AppendLine("				divObj.style.top = objTop + objHeight / 2 - divHeight / 2 + \"px\";");
				sScript.AppendLine("				divObj.style.left = objLeft - divWidth + \"px\";");
				sScript.AppendLine("				break;");
				sScript.AppendLine("			case \"R\":");
				sScript.AppendLine("				divObj.style.top = objTop + objHeight / 2 - divHeight / 2 + \"px\";");
				sScript.AppendLine("				divObj.style.left = objLeft + objWidth + \"px\";");
				sScript.AppendLine("				break;");
				sScript.AppendLine("			case \"BL\":");
				sScript.AppendLine("				divObj.style.top = objTop + objHeight + \"px\";");
				sScript.AppendLine("				divObj.style.left = objLeft - divWidth + \"px\";");
				sScript.AppendLine("				break;");
				sScript.AppendLine("			case \"B\":");
				sScript.AppendLine("				divObj.style.top = objTop + objHeight + \"px\";");
				sScript.AppendLine("				divObj.style.left = objLeft + objWidth / 2 - divWidth / 2 + \"px\";");
				sScript.AppendLine("				break;");
				sScript.AppendLine("			case \"BR\":");
				sScript.AppendLine("				divObj.style.top = objTop + objHeight + \"px\";");
				sScript.AppendLine("				divObj.style.left = objLeft + objWidth + \"px\";");
				sScript.AppendLine("				break;");
				sScript.AppendLine("		}");
				sScript.AppendLine("		hideSelects(divObj);");
				sScript.AppendLine("	}");
				sScript.AppendLine("}");
				sScript.AppendLine("function CloseHelps() {");
				sScript.AppendLine("	var coll = document.getElementsByTagName(\"div\");");
				sScript.AppendLine("	for (var i=0; i<coll.length; i++) {");
				sScript.AppendLine("		if (coll[i].attributes[\"IsHelp\"])");
				sScript.AppendLine("			coll[i].style.display = \"none\";");
				sScript.AppendLine("	}");
				sScript.AppendLine("	showSelects();");
				sScript.AppendLine("}");
				sScript.AppendLine("function CancelBubble(e) {");
				sScript.AppendLine("	e = (e) ? e : ((event) ? event : null);");
				sScript.AppendLine("	if (e) {");
				sScript.AppendLine("		e.cancelBubble = true;");
				sScript.AppendLine("		if(e.stopPropagation)");
				sScript.AppendLine("			e.stopPropagation();");
				sScript.AppendLine("	}");
				sScript.AppendLine("}");
				sScript.AppendLine("var isNewHandlerSet = false;");
				sScript.AppendLine("var savedHandler = null;");
				sScript.AppendLine("function onAction(obj, layout, e) {");
				sScript.AppendLine("	CancelBubble(e);");
				sScript.AppendLine("	CloseHelps();");
				sScript.AppendLine("	ShowHelp(obj, layout, e)");
				sScript.AppendLine("	if (!isNewHandlerSet) {");
				sScript.AppendLine("		savedHandler = document.onclick;");
				sScript.AppendLine("		document.onclick = offAction;");
				sScript.AppendLine("		isNewHandlerSet = true;");
				sScript.AppendLine("	}");
				sScript.AppendLine("}");
				sScript.AppendLine("function offAction(e) {");
				sScript.AppendLine("	CancelBubble(e);");
				sScript.AppendLine("	e = (e) ? e : ((event) ? event : null);");
				sScript.AppendLine("	if (e) {");
				sScript.AppendLine("		var obj = (e.srcElement) ? e.srcElement : ((e.target) ? e.target : null);");
				sScript.AppendLine("		while (obj) {");
				sScript.AppendLine("			if (obj.attributes[\"IsHelp\"])");
				sScript.AppendLine("				return;");
				sScript.AppendLine("			if (obj.tagName == \"HTML\" || obj.attributes[\"CloseHelp\"])");
				sScript.AppendLine("				break;");
				sScript.AppendLine("			obj = obj.parentNode");
				sScript.AppendLine("		}");
				sScript.AppendLine("	}");
				sScript.AppendLine("	CloseHelps();");
				sScript.AppendLine("	if (isNewHandlerSet) {");
				sScript.AppendLine("		document.onclick = savedHandler;");
				sScript.AppendLine("		savedHandler = null;");
				sScript.AppendLine("		isNewHandlerSet = false;");
				sScript.AppendLine("	}");
				sScript.AppendLine("}");

				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Help", sScript.ToString(), true);
			}
		}
		#endregion
	}
}