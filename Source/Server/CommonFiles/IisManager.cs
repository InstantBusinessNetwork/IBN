using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;

using Microsoft.Win32;
using Microsoft.Web.Administration;

namespace Mediachase.Ibn.Configuration
{
	#region public interface IIisManager
	public interface IIisManager
	{
		int IisVersion { get; }
		bool Is64Bit();
		bool IsApplicationPoolSupported { get; }

		bool CheckIfApplicationPoolExists(string name);
		void ChangeApplicationPool(long siteId, string poolName);
		void CreateApplicationPool(string name, bool managedCode);
		void DeleteApplicationPool(string name);
		string[] ListApplicationPools();
		void StartApplicationPool(string name);
		void StopApplicationPool(string name, bool wait);

		bool AddIsapiExtension(string name);
		void DeleteIsapiExtension();

		long CreateCompanySite(string name, string ipAddress, int port, string hostName, bool x64, string poolIM, string poolPortal);
		void DeleteSite(long siteId);
		string[] ListSites();
		void StartSite(long siteId);
		void StopSite(long siteId);
		long GetSiteId(string siteName);

		void AddBinding(long siteId, string ipAddress, int port, string hostName);
		void DeleteBinding(long siteId, string hostName);
		bool ChangeBinding(long siteId, string oldHostName, int oldPort, string newHostName, int newPort);

		bool CheckIfHostIsRegistered(string host);

		long CreateSite(string name, string physicalPath, string ipAddress, int port, string hostName, string poolName, bool enableAnonymous, bool enableWindows);
	}
	#endregion

	#region public sealed class WebName
	public sealed class WebName
	{
		//public const string Asp = "asp";
		public const string Download = "Download";
		public const string Images = "Images";
		public const string IMDownload = "IMDownload";
		public const string IMServer = "instmsg";
		public const string IMServer64 = "instmsg64";
		public const string Layouts = "Layouts";
		public const string Scripts = "Scripts";
		public const string Styles = "Styles";
		public const string Themes = "Themes";
		public const string Portal = "Portal";
		public const string Public = "Public";
		//public const string SelectWS = "selectws";
		//public const string UpdateWS = "updatews";

		private WebName()
		{
		}

		public static string GetIMFolder(bool x64)
		{
			return x64 ? IMServer64 : IMServer;
		}

		public static string GetIMPath(string webPhysicalPath, bool x64)
		{
			return GetIMPath(webPhysicalPath, GetIMFolder(x64));
		}

		public static string GetIMPath(string webPhysicalPath, string imFolder)
		{
			return Path.Combine(Path.Combine(webPhysicalPath, imFolder), "ibn_server.dll");
		}

		public static string GetPortalApplicationName(string domainName)
		{
			return domainName.Replace(".", "_");
		}
	}
	#endregion

	#region public sealed class IisManager
	public sealed class IisManager
	{
		private IisManager()
		{
		}

		#region public static IIisManager Create(string webPhysicalPath)
		public static IIisManager Create(string webPhysicalPath)
		{
			if (webPhysicalPath == null)
				throw new ArgumentNullException("webPhysicalPath");

			int iisVersion = 0;

			using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\InetStp"))
			{
				if (key != null)
				{
					iisVersion = (int)key.GetValue("MajorVersion");
					key.Close();
				}
			}

			webPhysicalPath = webPhysicalPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

			switch (iisVersion)
			{
				case 5:
					return new IisManager5(webPhysicalPath);
				case 6:
					return new IisManager6(webPhysicalPath);
				case 7:
					return new IisManager7(webPhysicalPath);
				default:
					throw new IisManagerException(string.Format(CultureInfo.InvariantCulture, "Unsupported IIS version: {0}.", iisVersion));
			}
		}
		#endregion
	}
	#endregion

	#region public class IisManagerException : Exception
	[Serializable]
	public class IisManagerException : Exception
	{
		public IisManagerException()
			: base()
		{
		}

		public IisManagerException(string message)
			: base(message)
		{
		}

		public IisManagerException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected IisManagerException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
	#endregion

	#region class IisManager5 : IisManager6
	class IisManager5 : IisManager6
	{
		public IisManager5(string webPhysicalPath)
			: base(webPhysicalPath)
		{
		}

		public override int IisVersion
		{
			get
			{
				return 5;
			}
		}

		public override bool IsApplicationPoolSupported
		{
			get
			{
				return false;
			}
		}


		public override bool CheckIfApplicationPoolExists(string name)
		{
			return false;
		}
		public override void ChangeApplicationPool(long siteId, string poolName)
		{
		}
		public override void CreateApplicationPool(string name, bool managedCode)
		{
		}
		public override void DeleteApplicationPool(string name)
		{
		}
		public override string[] ListApplicationPools()
		{
			return new string[] { };
		}
		public override void StartApplicationPool(string name)
		{
		}
		public override void StopApplicationPool(string name, bool wait)
		{
		}

		public override bool AddIsapiExtension(string name)
		{
			return false;
		}
		public override void DeleteIsapiExtension()
		{
		}

		#region protected override string ScriptMapGetVerbs()
		protected override string ScriptMapGetVerbs()
		{
			return "GET,HEAD,LOCK,UNLOCK,OPTIONS,PROPFIND,PUT";
		}
		#endregion
	}
	#endregion

	#region class IisManager6 : IIisManager
	class IisManager6 : IIisManager
	{
		const string ServicePath = "IIS://localhost/W3SVC";

		private string _webPhysicalPath;

		// public

		#region .ctor
		public IisManager6(string webPhysicalPath)
		{
			_webPhysicalPath = webPhysicalPath;
		}
		#endregion

		#region virtual public int IisVersion
		virtual public int IisVersion
		{
			get
			{
				return 6;
			}
		}
		#endregion
		#region public bool Is64Bit()
		public bool Is64Bit()
		{
			bool ret = false;
			try
			{
				if (IntPtr.Size == 8)
				{
					DirectoryEntry de = new DirectoryEntry(ServicePath + "/AppPools");
					ret = !(bool)de.Properties["Enable32bitAppOnWin64"][0];
				}
			}
			catch (DirectoryNotFoundException)
			{
			}
			return ret;
		}
		#endregion
		#region virtual public bool IsApplicationPoolSupported
		virtual public bool IsApplicationPoolSupported
		{
			get
			{
				return true;
			}
		}
		#endregion

		#region virtual public bool CheckIfApplicationPoolExists(string name)
		virtual public bool CheckIfApplicationPoolExists(string name)
		{
			return (GetApplicationPool(name) != null);
		}
		#endregion
		#region virtual public void ChangeApplicationPool(long siteId, string poolName)
		virtual public void ChangeApplicationPool(long siteId, string poolName)
		{
			const int isolation = 2;
			DirectoryEntry site = GetSite(siteId);
			if (site != null)
				site.Invoke("AppCreate3", isolation, poolName, false);
		}
		#endregion
		#region virtual public void CreateApplicationPool(string name, bool managedCode)
		virtual public void CreateApplicationPool(string name, bool managedCode)
		{
			using (DirectoryEntry pools = new DirectoryEntry(ServicePath + "/AppPools"))
			{
				DirectoryEntry pool = pools.Children.Add(name, "IIsApplicationPool");
				pool.CommitChanges();
			}
		}
		#endregion
		#region virtual public void DeleteApplicationPool(string name)
		virtual public void DeleteApplicationPool(string name)
		{
			DeleteEntry(ServicePath + "/AppPools", name, "IIsApplicationPool");
		}
		#endregion
		#region virtual public string[] ListApplicationPools()
		virtual public string[] ListApplicationPools()
		{
			List<string> list = new List<string>();

			using (DirectoryEntry pools = new DirectoryEntry(ServicePath + "/AppPools"))
			{
				foreach (DirectoryEntry child in pools.Children)
				{
					if (child.SchemaClassName == "IIsApplicationPool")
						list.Add(child.Name);
				}
			}

			return list.ToArray();
		}
		#endregion
		#region virtual public void StartApplicationPool(string name)
		virtual public void StartApplicationPool(string name)
		{
			DirectoryEntry pool = GetApplicationPool(name);
			if (pool != null)
				pool.Invoke("Start");
		}
		#endregion
		#region virtual public void StopApplicationPool(string name, bool wait)
		virtual public void StopApplicationPool(string name, bool wait)
		{
			DirectoryEntry pool = GetApplicationPool(name);
			if (pool != null)
			{
				pool.Invoke("Stop");

				if (wait)
				{
					while ((int)pool.Properties["AppPoolState"][0] != 4) // Stopped
						Thread.Sleep(10);
				}
			}
		}
		#endregion

		#region virtual public bool AddIsapiExtension(string name)
		virtual public bool AddIsapiExtension(string name)
		{
			GetService().Invoke("AddExtensionFile", WebName.GetIMPath(_webPhysicalPath, Is64Bit()), true, "", true, name);
			return true;
		}
		#endregion
		#region virtual public void DeleteIsapiExtension()
		virtual public void DeleteIsapiExtension()
		{
			GetService().Invoke("DeleteExtensionFileRecord", WebName.GetIMPath(_webPhysicalPath, Is64Bit()));
		}
		#endregion

		#region public long CreateSite(string name, string physicalPath, string ipAddress, int port, string hostName, string poolName, bool enableAnonymous, bool enableWindows)
		public long CreateSite(string name, string physicalPath, string ipAddress, int port, string hostName, string poolName, bool enableAnonymous, bool enableWindows)
		{
			long id = GenerateNewSiteIdIncremental();

			DirectoryEntry service = GetService();

			DirectoryEntry site = service.Children.Add(id.ToString(CultureInfo.InvariantCulture), "IIsWebServer");
			site.CommitChanges();

			AddBinding(site, ipAddress, port, hostName, true);

			site.Properties["ServerComment"][0] = name;
			SetDefaultProperties(site, true, false);
			site.CommitChanges();

			DirectoryEntry root = CreateVirtualDir2(site, "ROOT", physicalPath, name, poolName, 2, true, false);
			root.CommitChanges();

			int flags = 0;
			if (enableAnonymous)
				flags = flags | 1;
			if (enableWindows)
				flags = flags | 4;

			root.Properties["AuthFlags"].Value = flags;
			root.CommitChanges();

			return id;
		}
		#endregion

		#region public long CreateCompanySite(string name, string ipAddress, int port, string hostName, bool x64, string poolIM, string poolPortal)
		public long CreateCompanySite(string name, string ipAddress, int port, string hostName, bool x64, string poolIM, string poolPortal)
		{
			long id = GenerateNewSiteIdIncremental();

			DirectoryEntry service = GetService();

			DirectoryEntry site = service.Children.Add(id.ToString(CultureInfo.InvariantCulture), "IIsWebServer");
			site.CommitChanges();

			AddBinding(site, ipAddress, port, hostName, true);

			site.Properties["ServerComment"][0] = name;
			SetDefaultProperties(site, true, false);
			site.CommitChanges();

			DirectoryEntry root = CreateVirtualDir(site, "ROOT", WebName.Portal, name, poolPortal, 2, true, false);
			root.CommitChanges();
			ScriptMapUpdate(root, false, ScriptMapGetValue());
			root.CommitChanges();

			DirectoryEntry imServer = CreateVirtualDir(root, WebName.IMServer, x64 ? WebName.IMServer64 : WebName.IMServer, WebName.IMServer, poolIM, 0, false, true);
			imServer.CommitChanges();
			// Remove * from script map
			ScriptMapUpdate(imServer, true, null);

			DirectoryEntry download = CreateWebVirtualDir(root, WebName.Download, null, null, -1, false, false);
			download.CommitChanges();
			DirectoryEntry imDownload = CreateWebVirtualDir(root, WebName.IMDownload, null, null, -1, false, false);
			imDownload.CommitChanges();

			SetExpiration(root, WebName.Layouts);
			SetExpiration(root, WebName.Scripts);
			SetExpiration(root, WebName.Styles);
			SetExpiration(root, WebName.Themes);

			DirectoryEntry pub = CreateWebDirectory(root, WebName.Public);
			pub.CommitChanges();

			DirectoryEntry winLogin = pub.Children.Add("WinLogin.aspx", "IIsWebFile");
			winLogin.CommitChanges();
			winLogin.Properties["AuthFlags"].Value = 4; // AuthNTLM
			winLogin.CommitChanges();

			return id;
		}
		#endregion
		#region public void DeleteSite(long siteId)
		public void DeleteSite(long siteId)
		{
			DeleteEntry(ServicePath, siteId.ToString(CultureInfo.InvariantCulture), "IIsWebServer");
		}
		#endregion
		#region public string[] ListSites()
		public string[] ListSites()
		{
			List<string> list = new List<string>();

			using (DirectoryEntry service = GetService())
			{
				foreach (DirectoryEntry child in service.Children)
				{
					if (child.SchemaClassName == "IIsWebServer")
					{
						list.Add((string)child.Properties["ServerComment"][0]);
					}
				}
			}

			return list.ToArray();
		}
		#endregion
		#region public void StartSite(long siteId)
		public void StartSite(long siteId)
		{
			DirectoryEntry site = GetSite(siteId);
			site.Invoke("Start");
		}
		#endregion
		#region public void StopSite(long siteId)
		public void StopSite(long siteId)
		{
			DirectoryEntry site = GetSite(siteId);
			site.Invoke("Stop");
		}
		#endregion
		#region public long GetSiteId(string siteName)
		public long GetSiteId(string siteName)
		{
			long result = -1;

			using (DirectoryEntry service = GetService())
			{
				foreach (DirectoryEntry child in service.Children)
				{
					if (child.SchemaClassName == "IIsWebServer" && string.Compare((string)child.Properties["ServerComment"][0], siteName, StringComparison.OrdinalIgnoreCase) == 0)
					{
						result = long.Parse(child.Name, CultureInfo.InvariantCulture);
						break;
					}
				}
			}

			return result;
		}
		#endregion

		#region public void AddBinding(long siteId, string ipAddress, int port, string hostName)
		public void AddBinding(long siteId, string ipAddress, int port, string hostName)
		{
			DeleteBinding(siteId, hostName);

			DirectoryEntry site = GetSite(siteId);
			AddBinding(site, ipAddress, port, hostName, false);

			site.CommitChanges();
		}
		#endregion
		#region public void DeleteBinding(long siteId, string hostName)
		public void DeleteBinding(long siteId, string hostName)
		{
			DirectoryEntry server = GetSite(siteId);
			List<string> list = GetBindings(server, false);
			for (int i = 0; i < list.Count; i++)
			{
				string s = list[i] as string;
				if (s != null && 0 == string.Compare(s.Substring(s.LastIndexOf(":", StringComparison.OrdinalIgnoreCase) + 1), hostName, StringComparison.OrdinalIgnoreCase))
				{
					list.RemoveAt(i);
					i--;
				}
			}
			server.Properties["ServerBindings"].Value = list.ToArray();
			server.CommitChanges();
		}
		#endregion
		#region public bool ChangeBinding(long siteId, string oldHostName, int oldPort, string newHostName, int newPort)
		public bool ChangeBinding(long siteId, string oldHostName, int oldPort, string newHostName, int newPort)
		{
			bool changed = false;

			DirectoryEntry server = GetSite(siteId);
			List<string> list = GetBindings(server, false);

			string oldPortString = oldPort.ToString(CultureInfo.InvariantCulture);
			string newPortString = newPort.ToString(CultureInfo.InvariantCulture);

			for (int i = 0; i < list.Count; i++)
			{
				string binding = list[i]; // IP:Port:HostName
				string[] parts = binding.Split(':');
				if (parts.Length == 3)
				{
					if (string.Compare(parts[2], oldHostName, StringComparison.OrdinalIgnoreCase) == 0
						&& string.Compare(parts[1], oldPortString, StringComparison.OrdinalIgnoreCase) == 0
						)
					{
						list[i] = string.Concat(parts[0], ":", newPortString, ":", newHostName);
						changed = true;
					}
				}
			}

			if (changed)
			{
				server.Properties["ServerBindings"].Value = list.ToArray();
				server.CommitChanges();
			}

			return changed;
		}
		#endregion

		#region public bool CheckIfHostIsRegistered(string host)
		public bool CheckIfHostIsRegistered(string host)
		{
			bool result = false;

			using (DirectoryEntry service = GetService())
			{
				foreach (DirectoryEntry server in service.Children)
				{
					if (server.SchemaClassName == "IIsWebServer")
					{
						if ((int)server.Properties["ServerState"][0] != 4) // not stopped
						{
							foreach (string binding in GetBindings(server, false))
							{
								if (binding != null && string.Compare(binding.Substring(binding.LastIndexOf(":", StringComparison.OrdinalIgnoreCase) + 1), host, StringComparison.OrdinalIgnoreCase) == 0)
								{
									result = true;
									break;
								}
							}
						}
					}

					if (result)
						break;
				}
			}

			return result;
		}
		#endregion


		// private

		#region private static DirectoryEntry GetService()
		private static DirectoryEntry GetService()
		{
			return new DirectoryEntry(ServicePath);
		}
		#endregion
		#region private static DirectoryEntry GetSite(long siteId)
		private static DirectoryEntry GetSite(long siteId)
		{
			return new DirectoryEntry(string.Concat(ServicePath, "/", siteId.ToString(CultureInfo.InvariantCulture)));
		}
		#endregion
		#region private int GenerateNewSiteIdIncremental()
		private static int GenerateNewSiteIdIncremental()
		{
			List<int> list = new List<int>();
			list.Add(0);

			using (DirectoryEntry service = GetService())
			{
				foreach (DirectoryEntry child in service.Children)
				{
					if (child.SchemaClassName == "IIsWebServer")
					{
						int id = int.Parse(child.Name, CultureInfo.InvariantCulture);
						if (id > 0)
							list.Add(id);
					}
				}
			}

			list.Sort();

			int newId = list.Count;

			for (int i = 1; i < list.Count; i++)
			{
				if (list[i] != i)
				{
					newId = i;
					break;
				}
			}

			return newId;
		}
		#endregion

		#region private static void DeleteEntry(string parentPath, string name, string schema)
		private static void DeleteEntry(string parentPath, string name, string schema)
		{
			using (DirectoryEntry parent = new DirectoryEntry(parentPath))
			{
				try
				{
					DirectoryEntry child = parent.Children.Find(name, schema);
					parent.Children.Remove(child);
				}
				catch (DirectoryNotFoundException)
				{
				}
			}
		}
		#endregion
		#region private static void SetDefaultProperties(DirectoryEntry de, bool executeScript, bool executeDll)
		private static void SetDefaultProperties(DirectoryEntry de, bool executeScript, bool executeDll)
		{
			de.Properties["AccessExecute"][0] = executeDll;
			de.Properties["AccessScript"][0] = executeScript;
			de.Properties["AccessRead"][0] = true;
			de.Properties["EnableDefaultDoc"][0] = true;
			de.Properties["DefaultDoc"][0] = "Default.aspx";
		}
		#endregion
		#region private DirectoryEntry CreateVirtualDir(DirectoryEntry parent, string name, string relativePath, string appName, string poolName, int isolation, bool execScript, bool execDll)
		private DirectoryEntry CreateVirtualDir(DirectoryEntry parent, string name, string relativePath, string appName, string poolName, int isolation, bool execScript, bool execDll)
		{
			return CreateVirtualDir2(parent, name, Path.Combine(_webPhysicalPath, relativePath), appName, poolName, isolation, execScript, execDll);
		}
		#endregion
		#region private static DirectoryEntry CreateVirtualDir2(DirectoryEntry parent, string name, string physicalPath, string appName, string poolName, int isolation, bool execScript, bool execDll)
		private static DirectoryEntry CreateVirtualDir2(DirectoryEntry parent, string name, string physicalPath, string appName, string poolName, int isolation, bool execScript, bool execDll)
		{
			DirectoryEntry de = parent.Children.Add(name, "IIsWebVirtualDir");
			CreateDefaultApp(de, physicalPath, appName, poolName, isolation, execScript, execDll);

			return de;
		}
		#endregion
		#region private static DirectoryEntry CreateDefaultApp(DirectoryEntry de, string path, string appName, string poolName, int isolation, bool execScript, bool execDll)
		private static DirectoryEntry CreateDefaultApp(DirectoryEntry de, string path, string appName, string poolName, int isolation, bool execScript, bool execDll)
		{
			de.Properties["Path"][0] = path;
			SetDefaultProperties(de, execScript, execDll);

			if (appName != null)
			{
				de.Properties["AppFriendlyName"][0] = appName;

				if (string.IsNullOrEmpty(poolName))
					de.Invoke("AppCreate2", isolation);
				else
					de.Invoke("AppCreate3", isolation, poolName, false);
			}

			return de;
		}
		#endregion
		#region private DirectoryEntry CreateWebVirtualDir(McIbnAction action, DirectoryEntry parent, string name, string appName, int isolation, bool execScript, bool execDll)
		private DirectoryEntry CreateWebVirtualDir(DirectoryEntry parent, string name, string appName, string poolName, int isolation, bool execScript, bool execDll)
		{
			return CreateVirtualDir(parent, name, name, appName, poolName, isolation, execScript, execDll);
		}
		#endregion
		#region private static DirectoryEntry CreateWebDirectory(DirectoryEntry parent, string name)
		private static DirectoryEntry CreateWebDirectory(DirectoryEntry parent, string name)
		{
			return CreateWebDirectory(parent, name, false, false);
		}
		#endregion
		#region private static DirectoryEntry CreateWebDirectory(DirectoryEntry parent, string name, bool overrideParentAuth, bool allowAnonymousAccess)
		private static DirectoryEntry CreateWebDirectory(DirectoryEntry parent, string name, bool overrideParentAuth, bool allowAnonymousAccess)
		{
			DirectoryEntry dir = parent.Children.Add(name, "IIsWebDirectory");
			if (overrideParentAuth)
				dir.Properties["AuthAnonymous"][0] = allowAnonymousAccess;
			dir.CommitChanges();
			return dir;
		}
		#endregion
		#region private static void SetExpiration(DirectoryEntry parent, string name)
		private static void SetExpiration(DirectoryEntry parent, string name)
		{
			DirectoryEntry de = CreateWebDirectory(parent, name);
			if (de != null)
			{
				de.Properties["HttpExpires"][0] = "D, 0x15180";
				de.CommitChanges();
			}
		}
		#endregion

		#region virtual protected string ScriptMapGetVerbs()
		virtual protected string ScriptMapGetVerbs()
		{
			return null;
		}
		#endregion
		#region private string ScriptMapGetValue()
		private string ScriptMapGetValue()
		{
			string frameworkInstallRoot;
			using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\.NETFramework"))
			{
				frameworkInstallRoot = key.GetValue("InstallRoot") as string;
				key.Close();
			}

			string retVal = string.Concat("*,", frameworkInstallRoot, "v2.0.50727", Path.DirectorySeparatorChar, "aspnet_isapi.dll,1");
			string verbs = ScriptMapGetVerbs();
			if (!string.IsNullOrEmpty(verbs))
				retVal = string.Concat(retVal, ",", verbs);

			return retVal;
		}
		#endregion
		#region private static void ScriptMapUpdate(DirectoryEntry de, bool del, string value)
		private static void ScriptMapUpdate(DirectoryEntry de, bool del, string value)
		{
			PropertyValueCollection props = de.Properties["ScriptMaps"];

			bool add = !del;
			for (int i = 0; i < props.Count; i++)
			{
				string val = (string)props[i];
				if (val != null && val.StartsWith("*", StringComparison.OrdinalIgnoreCase))
				{
					if (del)
						props.RemoveAt(i);
					else
					{
						props[i] = value;
						add = false;
					}
					break;
				}
			}

			if (add)
				props.Add(value);

			de.CommitChanges();
		}
		#endregion

		#region private static List<string> GetBindings(DirectoryEntry server, bool secure)
		private static List<string> GetBindings(DirectoryEntry server, bool secure)
		{
			List<string> retVal = new List<string>();

			string propertyName = secure ? "SecureBindings" : "ServerBindings";
			object[] objects = server.Invoke("Get", (new object[1] { propertyName })) as object[];
			if (objects != null)
			{
				foreach (object obj in objects)
				{
					string str = obj as string;
					if (str != null)
						retVal.Add(str);
				}
			}

			return retVal;
		}
		#endregion
		#region private static void AddBinding(DirectoryEntry site, string ipAddress, int port, string hostName, bool clearBindings)
		private static void AddBinding(DirectoryEntry site, string ipAddress, int port, string hostName, bool clearBindings)
		{
			List<string> list = GetBindings(site, false);

			if (clearBindings)
				list.Clear();

			list.Add(string.Format(CultureInfo.InvariantCulture, "{0}:{1}:{2}", ipAddress, port, hostName));
			site.Properties["ServerBindings"].Value = list.ToArray();
		}
		#endregion

		#region private static DirectoryEntry GetApplicationPool(string name)
		private static DirectoryEntry GetApplicationPool(string name)
		{
			DirectoryEntry result = null;

			using (DirectoryEntry pools = new DirectoryEntry(ServicePath + "/AppPools"))
			{
				foreach (DirectoryEntry child in pools.Children)
				{
					if (child.SchemaClassName == "IIsApplicationPool" && string.Compare(child.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						result = child;
						break;
					}
				}
			}

			return result;
		}
		#endregion
	}
	#endregion

	#region class IisManager7 : IIisManager
	class IisManager7 : IIisManager
	{
		private string _webPhysicalPath;

		// public

		#region .ctor
		public IisManager7(string webPhysicalPath)
		{
			_webPhysicalPath = webPhysicalPath;
		}
		#endregion

		#region public int IisVersion
		public int IisVersion
		{
			get
			{
				return 7;
			}
		}
		#endregion
		#region public bool Is64Bit()
		public bool Is64Bit()
		{
			return IntPtr.Size == 8;
		}
		#endregion
		#region public bool IsApplicationPoolSupported
		public bool IsApplicationPoolSupported
		{
			get
			{
				return true;
			}
		}
		#endregion

		#region public bool CheckIfApplicationPoolExists(string name)
		public bool CheckIfApplicationPoolExists(string name)
		{
			using (ServerManager serverManager = new ServerManager())
			{
				return null != GetApplicationPool(serverManager, name);
			}
		}
		#endregion
		#region public void ChangeApplicationPool(long siteId, string poolName)
		public void ChangeApplicationPool(long siteId, string poolName)
		{
			using (ServerManager serverManager = new ServerManager())
			{
				Site site = GetSite(serverManager, siteId);
				foreach (Application application in site.Applications)
				{
					if (string.Compare(application.Path, "/", StringComparison.OrdinalIgnoreCase) == 0)
					{
						application.ApplicationPoolName = poolName;
						serverManager.CommitChanges();
						break;
					}
				}
			}
		}
		#endregion
		#region public void CreateApplicationPool(string name, bool managedCode)
		public void CreateApplicationPool(string name, bool managedCode)
		{
			using (ServerManager serverManager = new ServerManager())
			{
				ApplicationPool pool = serverManager.ApplicationPools.Add(name);
				pool.Enable32BitAppOnWin64 = false;
				pool.ManagedPipelineMode = ManagedPipelineMode.Integrated;
				pool.ManagedRuntimeVersion = managedCode ? "v2.0" : string.Empty;

				serverManager.CommitChanges();
			}
		}
		#endregion
		#region public void DeleteApplicationPool(string name)
		public void DeleteApplicationPool(string name)
		{
			using (ServerManager serverManager = new ServerManager())
			{
				ApplicationPool pool = GetApplicationPool(serverManager, name);
				if (pool != null)
					serverManager.ApplicationPools.Remove(pool);

				serverManager.CommitChanges();
			}
		}
		#endregion
		#region public string[] ListApplicationPools()
		public string[] ListApplicationPools()
		{
			List<string> list = new List<string>();

			using (ServerManager serverManager = new ServerManager())
			{
				foreach (ApplicationPool pool in serverManager.ApplicationPools)
				{
					list.Add(pool.Name);
				}
			}

			return list.ToArray();
		}
		#endregion
		#region public void StartApplicationPool(string name)
		public void StartApplicationPool(string name)
		{
			using (ServerManager serverManager = new ServerManager())
			{
				ApplicationPool pool = GetApplicationPool(serverManager, name);
				if (pool != null)
					pool.Start();
			}
		}
		#endregion
		#region public void StopApplicationPool(string name, bool wait)
		public void StopApplicationPool(string name, bool wait)
		{
			using (ServerManager serverManager = new ServerManager())
			{
				ApplicationPool pool = GetApplicationPool(serverManager, name);
				if (pool != null)
				{
					pool.Stop();

					if (wait)
					{
						while (pool.State != ObjectState.Stopped)
							Thread.Sleep(10);
					}
				}
			}
		}
		#endregion

		#region public bool AddIsapiExtension(string name)
		public bool AddIsapiExtension(string name)
		{
			using (ServerManager serverManager = new ServerManager())
			{
				string path = WebName.GetIMPath(_webPhysicalPath, Is64Bit());

				Microsoft.Web.Administration.Configuration configuration = serverManager.GetApplicationHostConfiguration();
				ConfigurationSection section = configuration.GetSection("system.webServer/security/isapiCgiRestriction");
				ConfigurationElementCollection collection = section.GetCollection();
				ConfigurationElement element = collection.CreateElement();
				element.SetAttributeValue("description", name);
				element.SetAttributeValue("path", path);
				element.SetAttributeValue("allowed", "true");
				collection.Add(element);

				serverManager.CommitChanges();

				return true;
			}
		}
		#endregion
		#region public void DeleteIsapiExtension()
		public void DeleteIsapiExtension()
		{
			using (ServerManager serverManager = new ServerManager())
			{
				Microsoft.Web.Administration.Configuration configuration = serverManager.GetApplicationHostConfiguration();
				ConfigurationSection section = configuration.GetSection("system.webServer/security/isapiCgiRestriction");
				ConfigurationElementCollection collection = section.GetCollection();

				string imPath = WebName.GetIMPath(_webPhysicalPath, false);
				string imPath64 = WebName.GetIMPath(_webPhysicalPath, true);

				List<ConfigurationElement> elementsToDelete = new List<ConfigurationElement>();
				foreach (ConfigurationElement element in collection)
				{
					string path = (string)element.GetAttributeValue("path");
					if (string.Compare(path, imPath, StringComparison.OrdinalIgnoreCase) == 0
						|| string.Compare(path, imPath64, StringComparison.OrdinalIgnoreCase) == 0)
					{
						elementsToDelete.Add(element);
					}
				}

				foreach (ConfigurationElement element in elementsToDelete)
					collection.Remove(element);

				serverManager.CommitChanges();
			}
		}
		#endregion

		public long CreateSite(string name, string physicalPath, string ipAddress, int port, string hostName, string poolName, bool enableAnonymous, bool enableWindows)
		{
			using (ServerManager serverManager = new ServerManager())
			{
				Site site = serverManager.Sites.Add(name, physicalPath, port);

				AddBinding(site, ipAddress, port, hostName, true);

				Application root = site.Applications[0];
				if (!string.IsNullOrEmpty(poolName))
					root.ApplicationPoolName = poolName;

				SetAuthentication(serverManager, site.Name, enableAnonymous, enableWindows);

				serverManager.CommitChanges();

				return site.Id;
			}
		}

		#region public long CreateCompanySite(string name, string ipAddress, int port, string hostName, bool x64, string poolIM, string poolPortal)
		public long CreateCompanySite(string name, string ipAddress, int port, string hostName, bool x64, string poolIM, string poolPortal)
		{
			using (ServerManager serverManager = new ServerManager())
			{
				Site site = serverManager.Sites.Add(name, Path.Combine(_webPhysicalPath, WebName.Portal), port);

				AddBinding(site, ipAddress, port, hostName, true);

				Application root = site.Applications[0];
				if (!string.IsNullOrEmpty(poolPortal))
					root.ApplicationPoolName = poolPortal;

				string imFolder = WebName.GetIMFolder(x64);
				Application imApplication = CreateApplication(site, WebName.IMServer, imFolder, poolIM);

				CreateVirtualDirectory(root, WebName.Download);
				CreateVirtualDirectory(root, WebName.IMDownload);

				SetAuthentication(serverManager, GetWinLoginPath(site.Name), false, true);

				serverManager.CommitChanges();

				// Update script processor path for IM server.
				{
					Microsoft.Web.Administration.Configuration configuration = imApplication.GetWebConfiguration();
					ConfigurationSection section = configuration.GetSection("system.webServer/handlers");
					ConfigurationElementCollection collection = section.GetCollection();
					foreach (ConfigurationElement element in collection)
					{
						if ((string)element.GetAttributeValue("name") == "IBN 4.7 IM")
						{
							string imPath = WebName.GetIMPath(_webPhysicalPath, imFolder);
							element.SetAttributeValue("scriptProcessor", imPath);
							serverManager.CommitChanges();
							break;
						}
					}
				}

				return site.Id;
			}
		}
		#endregion
		#region public void DeleteSite(long siteId)
		public void DeleteSite(long siteId)
		{
			using (ServerManager serverManager = new ServerManager())
			{
				Site site = GetSite(serverManager, siteId);
				if (site != null)
				{
					serverManager.Sites.Remove(site);
				}

				serverManager.CommitChanges();
			}
		}
		#endregion
		#region public string[] ListSites()
		public string[] ListSites()
		{
			List<string> list = new List<string>();

			using (ServerManager serverManager = new ServerManager())
			{
				foreach (Site item in serverManager.Sites)
				{
					list.Add(item.Name);
				}
			}

			return list.ToArray();
		}
		#endregion
		#region public void StartSite(long siteId)
		public void StartSite(long siteId)
		{
			using (ServerManager serverManager = new ServerManager())
			{
				Site site = GetSite(serverManager, siteId);
				if (site != null && site.State == ObjectState.Stopped)
					site.Start();

				serverManager.CommitChanges();
			}
		}
		#endregion
		#region public void StopSite(long siteId)
		public void StopSite(long siteId)
		{
			using (ServerManager serverManager = new ServerManager())
			{
				Site site = GetSite(serverManager, siteId);
				if (site != null && site.State == ObjectState.Started)
					site.Stop();

				serverManager.CommitChanges();
			}
		}
		#endregion
		#region public long GetSiteId(string siteName)
		public long GetSiteId(string siteName)
		{
			long retVal = -1;

			using (ServerManager serverManager = new ServerManager())
			{
				Site site = GetSite(serverManager, siteName);
				if (site != null)
					retVal = site.Id;
			}

			return retVal;
		}
		#endregion

		#region public void AddBinding(long siteId, string ipAddress, int port, string hostName)
		public void AddBinding(long siteId, string ipAddress, int port, string hostName)
		{
			DeleteBinding(siteId, hostName);

			using (ServerManager serverManager = new ServerManager())
			{
				Site site = GetSite(serverManager, siteId);

				AddBinding(site, ipAddress, port, hostName, false);

				serverManager.CommitChanges();
			}
		}
		#endregion
		#region public void DeleteBinding(long siteId, string hostName)
		public void DeleteBinding(long siteId, string hostName)
		{
			using (ServerManager serverManager = new ServerManager())
			{
				Site site = GetSite(serverManager, siteId);

				List<Binding> bindingsToDelete = new List<Binding>();
				foreach (Binding binding in site.Bindings)
				{
					if (string.Compare(binding.Host, hostName, StringComparison.OrdinalIgnoreCase) == 0)
					{
						bindingsToDelete.Add(binding);
						break;
					}
				}

				foreach (Binding binding in bindingsToDelete)
					site.Bindings.Remove(binding);

				serverManager.CommitChanges();
			}
		}
		#endregion
		#region public bool ChangeBinding(long siteId, string oldHostName, int oldPort, string newHostName, int newPort)
		public bool ChangeBinding(long siteId, string oldHostName, int oldPort, string newHostName, int newPort)
		{
			bool changed = false;

			using (ServerManager serverManager = new ServerManager())
			{
				Site site = GetSite(serverManager, siteId);

				string oldPortString = oldPort.ToString(CultureInfo.InvariantCulture);
				string newPortString = newPort.ToString(CultureInfo.InvariantCulture);

				foreach (Binding bindingItem in site.Bindings)
				{
					string binding = bindingItem.BindingInformation; // IP:Port:HostName
					string[] parts = binding.Split(':');
					if (parts.Length == 3)
					{
						if (string.Compare(parts[2], oldHostName, StringComparison.OrdinalIgnoreCase) == 0
							&& string.Compare(parts[1], oldPortString, StringComparison.OrdinalIgnoreCase) == 0
							)
						{
							bindingItem.BindingInformation = string.Concat(parts[0], ":", newPortString, ":", newHostName);
							changed = true;
						}
					}
				}

				if (changed)
					serverManager.CommitChanges();
			}

			return changed;
		}
		#endregion

		#region public bool CheckIfHostNameIsRegistered(string host)
		public bool CheckIfHostIsRegistered(string host)
		{
			bool result = false;

			using (ServerManager serverManager = new ServerManager())
			{
				foreach (Site site in serverManager.Sites)
				{
					if (site.State != ObjectState.Stopped)
					{
						foreach (Binding binding in site.Bindings)
						{
							if (string.Compare(binding.Host, host, StringComparison.OrdinalIgnoreCase) == 0)
							{
								result = true;
								break;
							}
						}
					}

					if (result)
						break;
				}
			}

			return result;
		}
		#endregion


		// private

		#region private static Site GetSite(ServerManager serverManager, long siteId)
		private static Site GetSite(ServerManager serverManager, long siteId)
		{
			Site retVal = null;

			foreach (Site site in serverManager.Sites)
			{
				if (site.Id == siteId)
				{
					retVal = site;
					break;
				}
			}

			return retVal;
		}
		#endregion
		#region private static Site GetSite(ServerManager serverManager, string siteName)
		private static Site GetSite(ServerManager serverManager, string siteName)
		{
			Site retVal = null;

			foreach (Site site in serverManager.Sites)
			{
				if (string.Compare(site.Name, siteName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					retVal = site;
					break;
				}
			}

			return retVal;
		}
		#endregion

		#region private static string GetWinLoginPath(string siteName)
		private static string GetWinLoginPath(string siteName)
		{
			return string.Concat(siteName, "/public/winlogin.aspx");
		}
		#endregion

		#region private static Site GetApplicationPool(ServerManager serverManager, string name)
		private static ApplicationPool GetApplicationPool(ServerManager serverManager, string name)
		{
			ApplicationPool retVal = null;

			foreach (ApplicationPool pool in serverManager.ApplicationPools)
			{
				if (string.Compare(pool.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					retVal = pool;
					break;
				}
			}

			return retVal;
		}
		#endregion

		#region private Application CreateApplication(Site site, string name, string physicalName)
		private Application CreateApplication(Site site, string name, string physicalName, string poolName)
		{
			Application application = site.Applications.Add(string.Concat("/", name), Path.Combine(_webPhysicalPath, physicalName));
			if (!string.IsNullOrEmpty(poolName))
				application.ApplicationPoolName = poolName;

			return application;
		}
		#endregion

		#region private VirtualDirectory CreateVirtualDirectory(Application application, string name)
		private VirtualDirectory CreateVirtualDirectory(Application application, string name)
		{
			return application.VirtualDirectories.Add(string.Concat("/", name), Path.Combine(_webPhysicalPath, name));
		}
		#endregion

		#region private static void AddBinding(Site site, string ipAddress, int port, string hostName, bool clearBindings)
		private static void AddBinding(Site site, string ipAddress, int port, string hostName, bool clearBindings)
		{
			if (string.IsNullOrEmpty(ipAddress))
				ipAddress = "*";

			if (clearBindings)
				site.Bindings.Clear();

			site.Bindings.Add(string.Format(CultureInfo.InvariantCulture, "{0}:{1}:{2}", ipAddress, port, hostName), "http");
		}
		#endregion

		#region private static void SetAuthentication(ServerManager serverManager, string locationPath, bool enableAnonymous, bool enableWindows)
		private static void SetAuthentication(ServerManager serverManager, string locationPath, bool enableAnonymous, bool enableWindows)
		{
			const string authenticationSection = "system.webServer/security/authentication/";

			Microsoft.Web.Administration.Configuration configuration = serverManager.GetApplicationHostConfiguration();
			configuration.GetSection(authenticationSection + "anonymousAuthentication", locationPath).SetAttributeValue("enabled", enableAnonymous);
			configuration.GetSection(authenticationSection + "windowsAuthentication", locationPath).SetAttributeValue("enabled", enableWindows);
		}
		#endregion
	}
	#endregion
}
