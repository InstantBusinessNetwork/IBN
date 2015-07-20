using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Net.WebDavServer;
using Mediachase.Net.Wdom;

namespace Mediachase.IBN.Business.WebDAV.ElementStorageProvider
{
	/// <summary>
	/// Эмулирует коллекцию
	/// </summary>
	public class RootFolderStorageProvider : WebDavElementStorageProvider
	{

		public override Mediachase.Net.Wdom.WebDavElementInfo GetElementInfo(string path)
		{

			WebDavElementInfo retVal = null;
			retVal = new CollectionInfo();
			retVal.Name = "root";
            retVal.Created = DateTime.Now;
            retVal.Modified = retVal.Created;
			retVal.AbsolutePath = path;


			return retVal;
		}

		public override Mediachase.Net.Wdom.PropertyInfoCollection GetProperties(Mediachase.Net.Wdom.WebDavElementInfo element)
		{
			PropertyInfoCollection retVal = new PropertyInfoCollection();
			PropertyInfo prop = null;
			#region CreateDefaultProperties
			foreach (PropertyInfo defaultProp in PropertyInfo.CreateDefaultProperties(element))
			{
				SetProperty(retVal, defaultProp, false);
			}
			#endregion
			//Add <supportedlock> property
			if (((int)(WebDavApplication.Class & WebDavServerClass.Class2)) != 0)
			{
				prop = PropertyInfo.CreateSupportedLockProperty();
				prop.Calculated = false;
				SetProperty(retVal, prop, false);
			}

			//Add <resourcetype> property
			prop = PropertyInfo.CreateResourceTypeProperty(element);
			SetProperty(retVal, prop, false);
	
			retVal.ElementInfo = element;
			return retVal;
		}


	
		private void SetProperty(PropertyInfoCollection collection, PropertyInfo property, bool overwrite)
		{
			if (collection != null)
			{
				PropertyInfo setProperty = collection[property.Name];

				if (setProperty == null)
				{
					collection.Add(property);
				}
				else if (overwrite == true)
				{
					setProperty.Namespace = property.Namespace;
					//Calculated property not set
					if (property.Calculated == false)
						setProperty.Value = property.Value;
				}
			}
		}

        public override Mediachase.Net.Wdom.WebDavElementInfo[] GetChildElements(Mediachase.Net.Wdom.WebDavElementInfo element)
        {
            return new WebDavElementInfo[] { };
        }

		#region Not implement
		public override void CopyTo(Mediachase.Net.Wdom.WebDavElementInfo srcElement, Mediachase.Net.Wdom.WebDavElementInfo destElInfo)
		{
			throw new NotImplementedException();
		}

		public override Mediachase.Net.Wdom.CollectionInfo CreateCollection(string path)
		{
			throw new NotImplementedException();
		}

		public override Mediachase.Net.Wdom.ResourceInfo CreateResource(string path)
		{
			throw new NotImplementedException();
		}

		public override void Delete(Mediachase.Net.Wdom.WebDavElementInfo element)
		{
			throw new NotImplementedException();
		}

		public override IEnumerable<ActiveLockElement> GetActiveLocks(Mediachase.Net.Wdom.WebDavElementInfo element)
		{
			throw new NotImplementedException();
		}

	


		public override OpaqueLockToken Lock(Mediachase.Net.Wdom.DepthValue depth, string timeout, Mediachase.Net.Wdom.LockInfoElement lockInfoEl, Mediachase.Net.Wdom.WebDavElementInfo element)
		{
			throw new NotImplementedException();
		}

		public override void MoveTo(Mediachase.Net.Wdom.WebDavElementInfo srcElement, Mediachase.Net.Wdom.WebDavElementInfo destElInfo)
		{
			throw new NotImplementedException();
		}

		public override System.IO.Stream OpenRead(Mediachase.Net.Wdom.WebDavElementInfo element)
		{
			throw new NotImplementedException();
		}

		public override System.IO.Stream OpenWrite(Mediachase.Net.Wdom.WebDavElementInfo element, long contentLength)
		{
			throw new NotImplementedException();
		}

		public override void RefreshLock(Mediachase.Net.Wdom.WebDavElementInfo element, OpaqueLockToken lockToken, string timeout)
		{
			throw new NotImplementedException();
		}

		public override void RemoveProperty(Mediachase.Net.Wdom.PropertyInfoCollection collection, string propName)
		{
			throw new NotImplementedException();
		}

		public override void SetProperty(Mediachase.Net.Wdom.PropertyInfoCollection collection, Mediachase.Net.Wdom.PropertyInfo property)
		{
			throw new NotImplementedException();
		}

		public override void Unlock(Mediachase.Net.Wdom.WebDavElementInfo element, OpaqueLockToken lockToken)
		{
			throw new NotImplementedException();
		} 
		#endregion
	}
}
