using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Web.UI.WebControls;
using System.Globalization;
using System.Collections;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.FileLibrary.CommandHandlers
{
	public class CopySelectedHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;

				string[] checkedElems = MCGrid.GetCheckedCollection(((CommandManager)Sender).Page, cp.CommandArguments["GridId"]);
				ArrayList alMas = new ArrayList();
				foreach (string elem in checkedElems)
				{
					string[] elemMas = elem.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
					//type, id, _containerName, _containerKey
					if (elemMas.Length != 4)
						continue;
					if (elemMas[0] == "2")
					{
						int id = Convert.ToInt32(elemMas[1], CultureInfo.InvariantCulture);
						alMas.Add(id);
					}

					UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
					System.Resources.ResourceManager LocRM = new System.Resources.ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", typeof(CopySelectedHandler).Assembly);
					int iCount = 10;
					if (pc["ClipboardItemsCount"] != null)
						iCount = int.Parse(pc["ClipboardItemsCount"].ToString());
					string ss = String.Empty;
					try
					{
						foreach (int iFileId in alMas)
						{
							string sNewFileClip = "";
							if (pc["ClipboardFiles"] != null)
								sNewFileClip = pc["ClipboardFiles"].ToString();
							sNewFileClip = WorkWithClipboard(iCount, iFileId.ToString() + "|" + sNewFileClip);
							pc["ClipboardFiles"] = sNewFileClip;
						}
						ss = String.Format(LocRM.GetString("tFilesAdded"), alMas.Count.ToString());
					}
					catch
					{
						ss = LocRM.GetString("tFilesNotAdded");
					}
					CommandManager cm = Sender as CommandManager;
					if (cm != null && !String.IsNullOrEmpty(ss))
						cm.InfoMessage = ss;

				}
			}
		}

		#endregion

		#region WorkWithClipboard
		private string WorkWithClipboard(int iCount, string sClip)
		{
			int pCount = 0;
			string sCheck = sClip;
			int[] iArray = new int[iCount + 1];
			ArrayList aList = new ArrayList();
			while (sCheck.Length > 0)
			{
				if (sCheck.IndexOf("|") >= 0)
				{
					int iObj = int.Parse(sCheck.Substring(0, sCheck.IndexOf("|")));
					if (!aList.Contains(iObj))
					{
						aList.Add(iObj);
						iArray[pCount++] = iObj;
					}
					sCheck = sCheck.Substring(sCheck.IndexOf("|") + 1);
				}
				if (pCount >= iCount)
					break;
			}
			sClip = "";
			for (int i = 0; i < pCount; i++)
				sClip += iArray[i].ToString() + "|";
			return sClip;
		}
		#endregion
	}
}
