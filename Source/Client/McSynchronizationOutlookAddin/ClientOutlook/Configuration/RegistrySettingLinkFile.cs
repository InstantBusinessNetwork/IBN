using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Mediachase.Sync.Core;

namespace Mediachase.ClientOutlook.Configuration
{
    internal class RegistrySettingLinkFile<T> : RegistrySettingLinkBase<T> where T : class, new()
    {
        public string FilePath { get; set; }

        public RegistrySettingLinkFile(Guid id, string filePath )
            : base(id)
        {
            FilePath = filePath;
			ReferencedSetting = new T();

			InitializeDefaultValues();
        }

        protected override T LoadSetting()
        {
            if (!File.Exists(FilePath))
            {
                throw new FileNotFoundException(FilePath + "not found");
            }

            DebugAssistant.Log("RegistrySettingLinkFile: Load setting from file " + FilePath);
            T retVal = null;
            string xmlDoc = File.ReadAllText(FilePath);
            XmlSerializer xmlsz = new XmlSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xmlDoc)))
            {
                retVal = (T)xmlsz.Deserialize(ms);
            }

            return retVal;
        }

        public override void SaveLink()
        {

            DebugAssistant.Log("RegistrySettingLinkFile: Save setting in file " + FilePath);
            //XmlSerializer xmlsz = new XmlSerializer(typeof(T));
            //using (Stream writer = new FileStream(FilePath, FileMode.Create))
            //{
            //    xmlsz.Serialize(writer, base.ReferencedSetting);
            //}

            base.SaveLink();

        }
    }
}
