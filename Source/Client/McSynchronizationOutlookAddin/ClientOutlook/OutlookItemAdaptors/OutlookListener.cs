using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Mediachase.Sync.Core;
using System.Reflection;

namespace OutlookAddin.OutlookItemAdaptors
{
	public class OutlookListener : Control
	{
		private delegate void VoidFunc();
		private delegate void VoidFunc<T>(T param1);
		private delegate void VoidFunc<T, T1>(T param1, T1 param2);

		AddinModule _addinModule;
		private OutlookItemFactory _factory;
		public OutlookListener(AddinModule addinModule)
		{
			_addinModule = addinModule;
			_factory = new OutlookItemFactory(this);
		}

		/// <summary>
		/// Appointments the delete.
		/// </summary>
		/// <param name="oAppItem">The o app item.</param>
		public void AppointmentDelete(Outlook._AppointmentItem oAppItem)
		{
			if (oAppItem == null)
				throw new ArgumentNullException("oAppItem");
			if (this.InvokeRequired)
			{
				VoidFunc<Outlook._AppointmentItem> func = AppointmentDelete;
				this.Invoke(func, oAppItem);
			}
			else
			{
				oAppItem.Delete();
			}
		}
		/// <summary>
		/// Appointments the save.
		/// </summary>
		/// <param name="oAppItem">The o app item.</param>
		public void AppointmentSave(Outlook._AppointmentItem oAppItem)
		{
			if (oAppItem == null)
				throw new ArgumentNullException("oAppItem");
			if (this.InvokeRequired)
			{
				VoidFunc<Outlook._AppointmentItem> func = AppointmentSave;
				this.Invoke(func, oAppItem);
			}
			else
			{
				oAppItem.Save();
			}
		}

		/// <summary>
		/// Gets the recurrence occurence.
		/// </summary>
		/// <param name="orPattern">The or pattern.</param>
		/// <param name="recurrenceId">The recurrence id.</param>
		/// <returns></returns>
		public OutlookAppointment GetRecurrenceOccurence(Outlook.RecurrencePattern orPattern, DateTime recurrenceId)
		{
			OutlookAppointment retVal = null;
			if (this.InvokeRequired)
			{
				Func<Outlook.RecurrencePattern, DateTime, OutlookAppointment> func = GetRecurrenceOccurence;
				retVal = this.Invoke(func, orPattern, recurrenceId) as OutlookAppointment;
			}
			else
			{
				Outlook._AppointmentItem oOcurrence = orPattern.GetOccurrence(recurrenceId);
				if (oOcurrence != null)
				{
					retVal = _factory.Create<OutlookItem>(oOcurrence) as OutlookAppointment;
				}
			}
			return retVal;

		}
		/// <summary>
		/// Adds the appointment recipient.
		/// </summary>
		/// <param name="oAppItem">The o app item.</param>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public OutlookRecipient AddAppointmentRecipient(Outlook._AppointmentItem oAppItem, string name)
		{
			OutlookRecipient retVal = null;
			if (this.InvokeRequired)
			{
				Func<Outlook._AppointmentItem, string, OutlookRecipient> func = AddAppointmentRecipient;
				retVal = this.Invoke(func, oAppItem, name) as OutlookRecipient;
			}
			else
			{
				Outlook.Recipient oRecipient = oAppItem.Recipients.Add(name);
				retVal = _factory.Create<OutlookItem>(oRecipient) as OutlookRecipient;
			}
			return retVal;
		}

		/// <summary>
		/// Sets the outlook prop val.
		/// </summary>
		/// <param name="oItem">The o item.</param>
		/// <param name="propName">Name of the prop.</param>
		/// <param name="val">The val.</param>
		/// <returns></returns>
		public bool SetOutlookPropVal(object oItem, string propName, object val)
		{
			bool retVal = false;
			if (this.InvokeRequired)
			{
				Func<object, string, object, bool> func = this.SetOutlookPropVal;
				retVal = (bool)this.Invoke(func, oItem, propName, val);
			}
			else
			{
				Type outlookType = GetOutlookTypeForComObject(oItem);
				if (outlookType == null)
					throw new ArgumentException("oItem invalid outlook type");

				PropertyInfo propInfo = outlookType.GetProperty(propName);
				if (propInfo == null)
					throw new NullReferenceException("prop with name " + propName + " not found");

				//Пытаемся преобразовать значение c типом заместителя в outlook  тип 
				OutlookItem outlookItem = val as OutlookItem;
				if (outlookItem != null)
				{
					val = outlookItem.InnerOutlookObject;
				}

				propInfo.SetValue(oItem, val, null);
				retVal = true;
			}
			return retVal;

		}
		/// <summary>
		/// Gets the outlook prop val.
		/// </summary>
		/// <param name="oItem">The o item.</param>
		/// <param name="propName">Name of the prop.</param>
		/// <returns></returns>
		public object GetOutlookPropVal(object oItem, string propName)
		{
			object retVal = null;
			if (oItem == null)
				throw new ArgumentNullException("oItem");

			if (this.InvokeRequired)
			{
				Func<object, string, object> func = this.GetOutlookPropVal;
				retVal = this.Invoke(func, oItem, propName);
			}
			else
			{
				DebugAssistant.Log(DebugSeverity.Debug, "OutlookListener: get property " + propName);
				Type outlookType = GetOutlookTypeForComObject(oItem);
				if (outlookType == null)
					throw new ArgumentException("oItem invalid outlook type");

				PropertyInfo propInfo = outlookType.GetProperty(propName);
				if (propInfo == null)
					throw new NullReferenceException("prop with name " + propName + " not found");

				retVal = propInfo.GetValue(oItem, null);
				//Пытаемся преобразовать выходной тип outlook в тип заместитель
				if (retVal != null)
				{
					OutlookItem outlookItem = _factory.Create<OutlookItem>(retVal);
					if (outlookItem != null)
					{
						retVal = outlookItem;
					}
				}
			}
			return retVal;
		}

		/// <summary>
		/// Appointments the clear recurrence pattern.
		/// </summary>
		/// <param name="oAppItem">The o app item.</param>
		public void AppointmentClearRecurrencePattern(Outlook._AppointmentItem oAppItem)
		{
			if (oAppItem == null)
				throw new ArgumentNullException("oAppItem");
			if (this.InvokeRequired)
			{
				VoidFunc<Outlook._AppointmentItem> func = AppointmentClearRecurrencePattern;
				this.Invoke(func, oAppItem);
			}
			else
			{
				oAppItem.ClearRecurrencePattern();
			}
		}

		public void AppointmentRemoveRecipient(Outlook._AppointmentItem oAppItem, int index)
		{
			if (oAppItem == null)
				throw new ArgumentNullException("oAppItem");
			if (index >= oAppItem.Recipients.Count)
				throw new ArgumentException("index greather that indexed collection");

			if (this.InvokeRequired)
			{
				VoidFunc<Outlook._AppointmentItem, int> func = AppointmentRemoveRecipient;
				this.Invoke(func, oAppItem, index);
			}
			else
			{
				oAppItem.Recipients.Remove(index);
			}
		}
		/// <summary>
		/// Gets the recurrence pattern.
		/// </summary>
		/// <param name="oAppItem">The o app item.</param>
		/// <returns></returns>
		public OutlookRecurrencePattern GetRecurrencePattern(Outlook._AppointmentItem oAppItem)
		{
			if (oAppItem == null)
				throw new ArgumentNullException("oAppItem");

			OutlookRecurrencePattern retVal = null;
			if (this.InvokeRequired)
			{
				Func<Outlook.AppointmentItem, OutlookRecurrencePattern> func = GetRecurrencePattern;
				retVal = this.Invoke(func, oAppItem) as OutlookRecurrencePattern;
			}
			else
			{
				Outlook.RecurrencePattern oRecPattern = oAppItem.GetRecurrencePattern();
				if (oRecPattern != null)
				{
					retVal = _factory.Create<OutlookItem>(oRecPattern) as OutlookRecurrencePattern;
				}
			}
			return retVal;
		}

		/// <summary>
		/// Gets the recurrence exceptions.
		/// </summary>
		/// <param name="oRPattern">The o R pattern.</param>
		/// <returns></returns>
		public List<OutlookException> GetRecurrenceExceptions(Outlook.RecurrencePattern oRPattern)
		{
			if (oRPattern == null)
				throw new ArgumentNullException("oRPattern");

			List<OutlookException> retVal = new List<OutlookException>();
			if (this.InvokeRequired)
			{
				Func<Outlook.RecurrencePattern, List<OutlookException>> func = this.GetRecurrenceExceptions;
				retVal = (List<OutlookException>)this.Invoke(func, oRPattern);
			}
			else
			{
				for (int i = 1; i <= oRPattern.Exceptions.Count; i++)
				{
					Outlook.Exception oException = oRPattern.Exceptions.Item(i);
					OutlookException exception = _factory.Create<OutlookItem>(oException) as OutlookException;
					if (exception != null)
					{
						retVal.Add(exception);
					}

				}
			}
			return retVal;
		}

		/// <summary>
		/// Adds the folder item.
		/// </summary>
		/// <param name="oFolder">The o folder.</param>
		/// <param name="oItemType">Type of the o item.</param>
		/// <returns></returns>
		public OutlookItem AddFolderItem(Outlook.MAPIFolder oFolder, Outlook.OlItemType oItemType)
		{
			OutlookItem retVal = null;
			if (oFolder == null)
				throw new ArgumentNullException("oFolder");
			if (this.InvokeRequired)
			{
				Func<Outlook.MAPIFolder, Outlook.OlItemType, OutlookItem> func = AddFolderItem;
				retVal = this.Invoke(func, oFolder, oItemType) as OutlookItem;
			}
			else
			{
				object newItem = oFolder.Items.Add(oItemType);
				if (newItem != null)
				{
					retVal = _factory.Create<OutlookItem>(newItem) as OutlookItem;
				}
			}
			return retVal;
		}
		/// <summary>
		/// Gets the folder items.
		/// </summary>
		/// <param name="oFolder">The o folder.</param>
		/// <returns></returns>
		public List<OutlookItem> GetFolderItems(Outlook.MAPIFolder oFolder)
		{
			List<OutlookItem> retVal = new List<OutlookItem>();
			if (this.InvokeRequired)
			{
				Func<Outlook.MAPIFolder, List<OutlookItem>> func = this.GetFolderItems;
				return (List<OutlookItem>)this.Invoke(func, oFolder);
			}
			else
			{

				for (int i = 1; i <= oFolder.Items.Count; i++)
				{
					OutlookItem item = _factory.Create<OutlookItem>(oFolder.Items.Item(i));
					if (item != null)
					{
						retVal.Add(item);
					}
				}
			}

			return retVal;
		}

		/// <summary>
		/// Gets the appointment recipients.
		/// </summary>
		/// <param name="oAppItem">The o app item.</param>
		/// <returns></returns>
		public List<OutlookRecipient> GetAppointmentRecipients(Outlook._AppointmentItem oAppItem)
		{
			List<OutlookRecipient> retVal = new List<OutlookRecipient>();
			if (this.InvokeRequired)
			{
				Func<Outlook._AppointmentItem, List<OutlookRecipient>> func = this.GetAppointmentRecipients;
				retVal = (List<OutlookRecipient>)this.Invoke(func, oAppItem);
			}
			else
			{
				for (int i = 1; i <= oAppItem.Recipients.Count; i++)
				{
					OutlookRecipient recipient = _factory.Create<OutlookItem>(oAppItem.Recipients.Item(i)) as OutlookRecipient;
					if (recipient != null)
					{
						retVal.Add(recipient);
					}
				}
			}

			return retVal;
		}

		/// <summary>
		/// Gets the outlook folder from path.
		/// </summary>
		/// <param name="outlookNS">The outlook NS.</param>
		/// <param name="path">The path.</param>
		/// <returns></returns>
		public OutlookFolder GetOutlookFolderFromPath(string path)
		{
			if (string.IsNullOrEmpty(path))
				throw new ArgumentNullException("path");

			OutlookFolder retVal = null;

			if (this.InvokeRequired)
			{
				Func<string, OutlookFolder> func = this.GetOutlookFolderFromPath;
				retVal = (OutlookFolder)this.Invoke(func, path);
			}
			else
			{
				Outlook.MAPIFolder oMapiFolder = null;
				Outlook.NameSpace oNs = null;
				try
				{
					oNs = this._addinModule.OutlookApp.GetNamespace("MAPI");

					string[] folders = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
					if (folders.Length != 0)
					{
						oMapiFolder = oNs.Folders.Item(folders[0]);
						for (int i = 1; i < folders.Length; i++)
						{
							Outlook.MAPIFolder oTmpMapiFolder = oMapiFolder.Folders.Item(folders[i]);
							if (oTmpMapiFolder != null)
							{
								Marshal.ReleaseComObject(oMapiFolder);
								oMapiFolder = oTmpMapiFolder;
							}
						}
					}
				}
				finally
				{
					if (oNs != null)
						Marshal.ReleaseComObject(oNs);
				}

				if (oMapiFolder != null)
				{
					retVal = _factory.Create<OutlookItem>(oMapiFolder) as OutlookFolder;
				}
			}

			return retVal;
		}



		/// <summary>
		/// Gets the outlook type for COM object.
		/// </summary>
		/// <param name="outlookComObject">The outlook COM object.</param>
		/// <returns></returns>
		public static Type GetOutlookTypeForComObject(object outlookComObject)
		{
			Type retVal = null;
			// get the com object and fetch its IUnknown
			IntPtr iunkwn = System.Runtime.InteropServices.Marshal.GetIUnknownForObject(outlookComObject);

			// enum all the types defined in the interop assembly
			System.Reflection.Assembly outlookAssembly =
			System.Reflection.Assembly.GetAssembly(typeof(Outlook._Application));
			Type[] outlookTypes = outlookAssembly.GetTypes();

			// find the first implemented interop type
			foreach (Type currType in outlookTypes)
			{
				// get the iid of the current type
				Guid iid = currType.GUID;
				if (!currType.IsInterface || iid == Guid.Empty)
				{
					// com interop type must be an interface with valid iid
					continue;
				}

				// query supportability of current interface on object
				IntPtr ipointer = IntPtr.Zero;
				System.Runtime.InteropServices.Marshal.QueryInterface(iunkwn, ref iid, out ipointer);

				if (ipointer != IntPtr.Zero)
				{
					// yeah, that’s the one we’re after
					retVal = currType;
					break;
				}
			}

			return retVal;
		}



	}
}
