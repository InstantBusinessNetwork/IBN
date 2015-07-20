using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Workflow.ComponentModel.Serialization;
using System.IO;

namespace Mediachase.Ibn.Assignments
{
    /// <summary>
    /// Represents an workflow serializer.
    /// </summary>
	public static class McWorkflowSerializer
	{
        #region GetString
        /// <summary>
        /// Gets workflow markup string from the object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static string GetString(object obj)
        {
            StringBuilder sbOutput = new StringBuilder(1024);

            using (XmlWriter writer = XmlWriter.Create(sbOutput))
            {
                WorkflowMarkupSerializer serializer = new WorkflowMarkupSerializer();
                serializer.Serialize(writer, obj);
            }

            return sbOutput.ToString();
        }

        /// <summary>
        /// Gets workflow markup string from the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static string GetString<T>(T obj)
        {
            StringBuilder sbOutput = new StringBuilder(1024);

            using (XmlWriter writer = XmlWriter.Create(sbOutput))
            {
                WorkflowMarkupSerializer serializer = new WorkflowMarkupSerializer();
                serializer.Serialize(writer, obj);
            }

            return sbOutput.ToString();
        } 
        #endregion

        #region GetObject
        /// <summary>
        /// Gets object from the workflow markup string.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns></returns>
        public static object GetObject(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            using (TextReader stringReader = new StringReader(str))
            {
                using (XmlReader reader = XmlReader.Create(stringReader))
                {
                    WorkflowMarkupSerializer serializer = new WorkflowMarkupSerializer();
                    return serializer.Deserialize(reader);
                }
            }
        }

        /// <summary>
        /// Gets object from the workflow markup string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str">The STR.</param>
        /// <returns></returns>
        public static T GetObject<T>(string str)
        {
            if (string.IsNullOrEmpty(str))
                return default(T);

            using (TextReader stringReader = new StringReader(str))
            {
                using (XmlReader reader = XmlReader.Create(stringReader))
                {
                    WorkflowMarkupSerializer serializer = new WorkflowMarkupSerializer();
                    return (T)serializer.Deserialize(reader);
                }
            }
        } 
        #endregion
	}
}
