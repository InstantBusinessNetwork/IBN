using System;
using System.Xml;
using System.Reflection;
using System.Collections;
using System.Configuration; 
using System.IO;
using System.Diagnostics;

namespace Mediachase.IBN.Business.ControlSystem
{
	/// <summary>
	/// Summary description for AssemblyHelper.
	/// </summary>
	public class AssemblyHelper
	{
		public static string GetAssembliesPath()
		{
			Assembly asm = null;
			asm = Assembly.GetExecutingAssembly();

			if(asm.GlobalAssemblyCache)
				asm = Assembly.GetCallingAssembly();

			string result = asm.CodeBase.Substring(0, asm.CodeBase.LastIndexOf("/") + 1);
			string prefix = "///";
			return result.Substring(result.IndexOf(prefix) + prefix.Length);
		}


		public static object LoadObject(string Type, Type InterfaceType) 
		{
			return LoadObject(Type,InterfaceType.FullName);
		}

		public static object LoadObject(string Type, string InterfaceName) 
		{
			string[] typeInformation = Type.Split(',');

			if( typeInformation.Length < 2 )
				throw new ArgumentException("Unable to parse type name.  Use 'type,assembly'");

			Assembly asm = Assembly.Load(typeInformation[1].Trim());
			if(asm == null)
				throw new ArgumentException("Unable to load assembly " + typeInformation[1]);

			Type networkServerType = asm.GetType(typeInformation[0].Trim());
			if( networkServerType == null)
				throw new ArgumentException("Unable to load type " + typeInformation[0]);

			Type ifaceType = networkServerType.GetInterface(InterfaceName);
			if( ! networkServerType.IsClass || ifaceType == null)
				throw new ArgumentException(typeInformation[0] + " must be a valid class implenting " + InterfaceName);

			return Activator.CreateInstance(networkServerType);
		}

		public static object[] LoadRecursive(string path, Type InterfaceType)
		{
			ArrayList	channelList	=	new ArrayList();

			ArrayList files = new ArrayList();

			files.AddRange(Directory.GetFiles(path,"*.exe"));
			files.AddRange(Directory.GetFiles(path,"*.dll"));

			foreach(string asmName in files)
			{
				try
				{
					Assembly asm = Assembly.LoadFrom(Path.Combine(path,asmName));

					if(asm != null)
					{
						Type[] types = asm.GetTypes();

						foreach(Type type in types)
						{
							if(type.IsSubclassOf(InterfaceType))
							{
								channelList.Add(AssemblyHelper.LoadObject(string.Format("{0}, {1}",asm.FullName,type.FullName), InterfaceType));
							}
						}
					}
				}
				catch(Exception ex)
				{
					Trace.WriteLine(ex);
				}
			}

			return channelList.ToArray();
		}

		public static object[] LoadRecursive(string path, Type InterfaceType, bool deep)
		{
			ArrayList	channelList	=	new ArrayList();

			channelList.AddRange(LoadRecursive(path,InterfaceType));

			if(deep)
			{
				foreach(string DirItem in Directory.GetDirectories(path))
				{
					channelList.AddRange(LoadRecursive(Path.Combine(path,DirItem),InterfaceType));
				}
			}

			return channelList.ToArray();
		}

	}
}
