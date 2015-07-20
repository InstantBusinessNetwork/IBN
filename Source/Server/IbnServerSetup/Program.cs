using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Xml;

using Mediachase.Ibn;
using Mediachase.Ibn.Configuration;

namespace IbnServer
{
	#region Enums
	enum Error
	{
		Unknown = -1,
		OK = 0,
		InvalidArgs = 10001,
		CompanyExists = 10015,
		CompanyError = 10016,
		AddCompanyRecord = 10017,
		AddHostHeader = 10018,
		AddVirtualDir = 10019,
		CreatePortalDB = 10020,
		AddSchedulerRecord = 10021,
		FillTables = 10022
	}

	enum SetupAction
	{
		Services,
		IMServer,
		Companies
	}
	#endregion

	class IbnServer
	{
		static List<SetupAction> _actions = new List<SetupAction>();

		#region static int Main(string[] args)
		[STAThread]
		static int Main(string[] args)
		{
#if DEBUG
			foreach (string arg in args)
				Console.WriteLine(arg);
			Console.ReadLine();
#endif

			bool ok = false;
			Error ret = Error.OK;

			if(!Settings.Init(args))
			{
				ShowUsage();
				ret = Error.InvalidArgs;
			}
			else
			{
				LogFile.Open(Settings.LogFile);
				try
				{
					switch (Settings.Action)
					{
						case "INSTALL":
							Install();
							break;
						case "REMOVE":
							Remove();
							break;
					}
					ok = true;
				}
				catch(Exception ex)
				{
					string s = ex.ToString();
					LogFile.WriteMessage("");
					LogFile.WriteMessageFormat("Error details:\n{0}", s);
					ret = Error.Unknown;

					if (ex is ConfigurationException)
						ret = Error.CompanyError;
				}
				finally
				{
					LogFile.WriteMessage("");
					LogFile.Close();
					if(!ok)
						LogFile.Show();
				}
			}
			return (int)ret;
		}
		#endregion

		#region static void ShowUsage()
		static void ShowUsage()
		{
			Console.WriteLine("Usage:");
			Console.WriteLine("IbnServerSetup.exe </act:install|remove> [/id:InstallDir] [/l:LogFile]");
		}
		#endregion

		#region static void Install()
		static void Install()
		{
			LogFile.WriteMessage("*** Installing " + IbnConst.ProductName + " ***");
			try
			{
				IIisManager iisManager = IisManager.Create(Settings.InstallDirWeb);

				InstallMmc30();
				InstallServices();
				RegisterIMServer(true, Settings.InstallDirWeb, iisManager.Is64Bit());
				StartServices();
				UpdateRegistry();
				RegisterAspNet(iisManager);
				//throw new Exception();

				LogFile.WriteMessage("*** OK ***");
			}
			catch
			{
				LogFile.WriteMessage("*** Failed ***");
				if(_actions.Count > 0)
				{
					LogFile.WriteMessage("");
					LogFile.WriteMessage("Rollback started");
					try
					{
						RollBack();
					}
					catch{}
					LogFile.WriteMessage("Rollback finished");
				}
				throw;
			}
		}
		#endregion

		#region static void Remove()
		static void Remove()
		{
			LogFile.WriteMessage("*** Removing " + IbnConst.ProductName + " ***");
			try
			{
				_actions.Clear();

				// Unregister IM server
				_actions.Add(SetupAction.IMServer);

				// Unregister services
				_actions.Add(SetupAction.Services);

				// Delete company
				if (Settings.DeleteCompany)
					_actions.Add(SetupAction.Companies);

				RollBack();

				LogFile.WriteMessage("*** OK ***");
			}
			catch
			{
				LogFile.WriteMessage("*** Failed ***");
			}
		}
		#endregion

		#region static void RollBack()
		static void RollBack()
		{
			foreach(SetupAction action in _actions)
			{
				try
				{
					switch(action)
					{
						case SetupAction.Services:
							RemoveServices();
							break;
						case SetupAction.IMServer:
							RegisterIMServer(false, Settings.InstallDirWeb, false);
							RegisterIMServer(false, Settings.InstallDirWeb, true);
							break;
						case SetupAction.Companies:
							DeleteCompanies();
							break;
					}
				}
				catch{}
			}
		}
		#endregion

		#region static void InstallMmc30()
		static void InstallMmc30()
		{
			if (Environment.OSVersion.Version.Major < 6)
			{
				LogFile.WriteMessage("Installing MMC 3.0 to GAC...");

				try
				{
					int result = StartProcess("MMCPerf.exe", null);
					if (result != 0)
					{
						LogWriteFailed();
					}
					else
					{
						LogWriteOk();
					}
				}
				catch (Exception ex)
				{
					LogWriteFailed();
					LogFile.WriteMessage(ex.ToString());
				}
			}
		}
		#endregion

		#region static void InstallServices()
		static void InstallServices()
		{
			RegisterComponents(true);
		}
		#endregion
		#region static void RemoveServices()
		static void RemoveServices()
		{
			RegisterComponents(false);
		}
		#endregion

		#region static void RegisterComponents(bool register)
		static void RegisterComponents(bool register)
		{
			bool systemIs64Bit = (IntPtr.Size == 8);
			ComponentInfo[] components = GetComponents();

			foreach (ComponentInfo component in components)
			{
				LogFile.WriteMessage((register ? "Registering " : "Unregistering ") + component + "...");

				int result = StartProcess(GetInstallUtil(systemIs64Bit && component.Supports64Bit), (register ? "" : "/u ") + "/LogToConsole=true \"" + component.FilePath + "\"");
				if (result != 0)
				{
					LogWriteFailed();
					if (register)
						throw new IbnSetupException("Failed to register " + component);
				}
				else
				{
					if (register)
						ActionAdd(SetupAction.Services);
					LogWriteOk();
				}
			}
		}
		#endregion
		#region static void RegisterIMServer(bool register, string webPath, bool x64)
		static void RegisterIMServer(bool register, string webPath, bool x64)
		{
			int result = StartProcess("regsvr32.exe", (register ? "" : "/u ") + "/s \"" + WebName.GetIMPath(webPath, x64) + "\"");
			if (register && result == 0)
				ActionAdd(SetupAction.IMServer);
		}
		#endregion

		#region static void StartServices()
		static void StartServices()
		{
			StartService("McOleDBService" + IbnConst.VersionMajorMinor);
			StartService("ScheduleService" + IbnConst.VersionMajorMinor);
		}
		#endregion
		#region static void StartService(string name)
		static void StartService(string name)
		{
			LogFile.WriteMessageFormat("Starting service '{0}'...", name);
			try
			{
				ServiceController sc = new ServiceController(name);
				if(sc.Status == ServiceControllerStatus.Stopped)
				{
					sc.Start();
					sc.WaitForStatus(ServiceControllerStatus.Running);
				}
				LogWriteOk();
			}
			catch
			{
				LogWriteFailed();
				throw;
			}
		}
		#endregion

		#region static void UpdateRegistry()
		static void UpdateRegistry()
		{
			if(Settings.UpdateRegistry)
				RegistrySettings.WriteString("INSTALLDIR", Settings.InstallDir + @"\");
		}
		#endregion

		#region static void ActionAdd(McIbnAction action)
		static void ActionAdd(SetupAction action)
		{
			_actions.Insert(0, action);
		}
		#endregion

		#region static void LogWriteOk()
		static void LogWriteOk()
		{
			LogFile.WriteMessage("OK.");
		}
		#endregion
		#region static void LogWriteFailed()
		static void LogWriteFailed()
		{
			LogFile.WriteMessage("Failed.");
		}
		#endregion

		#region static int StartProcess(string fileName, string arguments)
		static int StartProcess(string fileName, string arguments)
		{
			using (Process process = new Process())
			{
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.FileName = fileName;
				process.StartInfo.Arguments = arguments;

				process.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
				process.Start();
				process.BeginOutputReadLine();
				process.WaitForExit();

				return process.ExitCode;
			}
		}
		#endregion
		#region static void OutputHandler(object sendingProcess, DataReceivedEventArgs e)
		static void OutputHandler(object sendingProcess, DataReceivedEventArgs e)
		{
			LogFile.WriteMessage(e.Data);
		}
		#endregion

		#region static string GetInstallUtil(bool x64)
		static string GetInstallUtil(bool x64)
		{
			return GetFrameworkToolPath(x64, "InstallUtil.exe");
		}
		#endregion
		#region static string GetAspNetRegIis(bool x64)
		static string GetAspNetRegIis(bool x64)
		{
			return GetFrameworkToolPath(x64, "aspnet_regiis.exe");
		}
		#endregion
		#region static string GetFrameworkToolPath(bool x64, string toolName)
		static string GetFrameworkToolPath(bool x64, string toolName)
		{
			return System.Environment.ExpandEnvironmentVariables(@"%SystemRoot%\Microsoft.NET\Framework" + (x64 ? "64" : "") + @"\v2.0.50727\" + toolName);
		}
		#endregion

		#region static ComponentInfo[] GetComponents()
		static ComponentInfo[] GetComponents()
		{
			string servicesPath = Settings.InstallDir;
			string portalBinPath = Path.Combine(Settings.InstallDirWeb, @"Portal\bin");

			List<ComponentInfo> list = new List<ComponentInfo>();

			list.Add(new ComponentInfo(Path.Combine(servicesPath, "Mediachase.Ibn.ConfigurationUI.dll"), true));
			list.Add(new ComponentInfo(Path.Combine(servicesPath, "Mediachase.Ibn.Configuration.dll"), true));
			list.Add(new ComponentInfo(Path.Combine(servicesPath, "OleDBService.exe"), false));
			list.Add(new ComponentInfo(Path.Combine(servicesPath, "ScheduleService.exe"), true));
			list.Add(new ComponentInfo(Path.Combine(portalBinPath, "Mediachase.IBN.Business.dll"), true));

			return list.ToArray();
		}
		#endregion

		#region static void RegisterAspNet(IIisManager iisManager)
		static void RegisterAspNet(IIisManager iisManager)
		{
			if (iisManager.IisVersion < 7)
			{
				if (Settings.RegisterAspNet)
				{
					StartProcess(GetAspNetRegIis(iisManager.Is64Bit()), "-ir");
					//StartProcess(regiis, string.Format("-s w3svc/{0}/", Settings.SiteId));
				}
				//StartProcess(regiis, "-c");
			}
		}
		#endregion

		#region private static void DeleteCompanies()
		private static void DeleteCompanies()
		{
			IConfigurator configurator = Configurator.Create();
			foreach (ICompanyInfo company in configurator.ListCompanies(false))
			{
				configurator.DeleteCompany(company.Id, false);
			}
			configurator.DeleteAspSite(false);
		}
		#endregion
	}
}
