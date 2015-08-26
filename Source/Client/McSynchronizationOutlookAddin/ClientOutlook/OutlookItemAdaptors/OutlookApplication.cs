using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Sync.Core;


namespace OutlookAddin.OutlookItemAdaptors
{
	public class OutlookApplication : OutlookItem
	{

		private OutlookApplication(OutlookListener listener)
			: base(listener)
		{
		}

		public static OutlookApplication CreateInstance(OutlookListener listener)
		{
			if (listener == null)
				throw new ArgumentNullException("listener");

			return new OutlookApplication(listener);
		}

		public OutlookFolder GetOutlookFolderFromPath(string path)
		{
			return base._outlookListener.GetOutlookFolderFromPath(path);
		}

		/// <summary>
		/// Picks the outlook folder path.
		/// </summary>
		/// <param name="oApp">The o app.</param>
		/// <param name="oItemType">Type of the o item.</param>
		/// <returns></returns>
		public static string PickOutlookFolderPath(Outlook._Application oApp, Outlook.OlItemType oItemType)
		{
			string retVal = null;
			bool correctFolderSelected = false;
			try
			{
				while (!correctFolderSelected)
				{

					Outlook.NameSpace oNameSpace = oApp.GetNamespace("MAPI");
					Outlook.MAPIFolder oMapiFolder = oNameSpace.PickFolder();
					if (oMapiFolder.DefaultItemType != oItemType)
					{
						DebugAssistant.Log(DebugSeverity.Error | DebugSeverity.MessageBox, 
										   Resources.ERR_OUTLOOK_BAD_FOLDER_TYPE, oItemType);
						continue;
					}

					correctFolderSelected = true;
					retVal = oMapiFolder.Name;
					while (oMapiFolder.Parent is Outlook.MAPIFolder)
					{
						oMapiFolder = (Outlook.MAPIFolder)oMapiFolder.Parent;
						retVal = string.Format("{0}/", oMapiFolder.Name) + retVal;
					}
					retVal = "//" + retVal;
				}

			}
			catch (Exception e)
			{

				DebugAssistant.Log(DebugSeverity.Debug, e.Message);
				DebugAssistant.Log(DebugSeverity.Debug | DebugSeverity.MessageBox, Resources.DBG_OUTLOOK_FOLDER_NOT_SELECTED);
			}

			return retVal;
		}


		#region Overrides
		public override object InnerOutlookObject
		{
			get { throw new NotImplementedException(); }
		}
		#endregion
	}
}
