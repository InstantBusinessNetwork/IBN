using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Xml;

using Mediachase.Database;
using Mediachase.Ibn;
using Mediachase.Ibn.Configuration;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Schema;
using Mediachase.Ibn.Data.Sql;

namespace Update
{
	class Program
	{
		const int ConstCommandTimeout = 36000;
		const string ConstCodeSource = @"Code\_Source\";
		const string ConstIisReset = "iisreset.exe";

		static List<UpdateAction> _actions = new List<UpdateAction>();
		static List<string> _services = new List<string>();
		static List<string> _backupFiles = new List<string>();
		static List<string> _copiedFiles = new List<string>();
		static Dictionary<string, string> _configs = new Dictionary<string, string>();

		#region static int Main(string[] args)
		static int Main(string[] args)
		{
			//foreach (string arg in args)
			//    Console.WriteLine(arg);
			//Console.ReadLine();

			bool ok = false;
			Error result = Error.OK;

			try
			{
				Settings.Init(args);
				LogFile.Open(Settings.LogFile);

				// Validate current version
				if (Settings.CommonVersion < 50)
				{
					LogFile.WriteFormatted("This update requires {0}.50 or later.", IbnConst.ProductName);
					result = Error.WrongVersion;
				}
				else
				{
					// Wait while calling process is terminating
					if (Settings.ProcessId > 0)
					{
						try
						{
							Process process = Process.GetProcessById(Settings.ProcessId);
							LogFile.WriteLine("Waiting while calling process is terminating...");
							while (!process.HasExited)
								Thread.Sleep(10);
							LogWriteOk();
						}
						catch (ArgumentException)
						{
						}
					}

					IConfigurator configurator = Configurator.Create();
					IAspInfo asp = configurator.GetAspInfo();

					try
					{
						DateTime updatesExpirationDate = configurator.UpdatesExpirationDate;
						if (updatesExpirationDate < DateTime.UtcNow)
						{
							LogFile.WriteFormatted("Your license for {0 does not allow installing updates after {1}.", IbnConst.ProductName, updatesExpirationDate);
							result = Error.UpdatesExpired;
						}
						else
						{
							switch (Settings.Target)
							{
								case "COMPANY":
									if (string.IsNullOrEmpty(Settings.Id))
									{
										LogFile.WriteLine("Company id was not specified.");
										result = Error.InvalidArgs;
									}
									else
										UpdateCompany(configurator, Settings.Id);
									break;
								case "COMMON":
									UpdateCommon(asp);
									break;
								case "SERVER":
									UpdateServer(configurator, asp);
									break;
								default:
									RegisterUpdate(configurator, asp);
									if (asp == null)
										UpdateServer(configurator, asp);
									break;
							}
							ok = true;
						}
					}
					catch (LicenseExpiredException)
					{
						LogFile.WriteFormatted("Your license for {0} has expired.", IbnConst.ProductName);
						result = Error.LicenseExpired;
					}
				}
			}
			catch (Exception ex)
			{
				LogFile.WriteLine();
				LogFile.WriteLine("Error details:");
				LogFile.WriteLine(ex.ToString());
				result = Error.Unknown;
			}
			finally
			{
				LogFile.WriteLine();
				LogFile.Close();
				if (!ok && Settings.ShowLog)
					LogFile.Show();
			}

			return (int)result;
		}
		#endregion

		#region static void RegisterUpdate(IConfigurator configurator, IAspInfo asp)
		static void RegisterUpdate(IConfigurator configurator, IAspInfo asp)
		{
			LogFile.WriteFormatted("Copying update to {0} installation directory...", IbnConst.ProductName);
			IUpdateInfo[] updates = configurator.GetUpdateInfo(Settings.UpdateDir);

			int maxVersion = 0;
			foreach (IUpdateInfo update in updates)
			{
				if (update.Version > maxVersion)
					maxVersion = update.Version;
			}

			string updatesDir = Path.Combine(Settings.InstallDir, "Updates");
			string updateDir = Path.Combine(updatesDir, maxVersion.ToString(CultureInfo.InvariantCulture));

			if (Directory.Exists(updateDir))
			{
				LogWriteFailed();
				string message = string.Format(CultureInfo.InvariantCulture, "Update {0} already exists.", maxVersion);
				LogFile.WriteLine(message);

				if (asp != null)
					throw new UpdateException(message);
			}
			else
			{
				CopyDirectory(Settings.UpdateDir, updateDir);
				LogWriteOk();
			}
		}
		#endregion

		#region static void UpdateServer(IConfigurator configurator, IAspInfo asp)
		static void UpdateServer(IConfigurator configurator, IAspInfo asp)
		{
			LogFile.WriteLine("* Updating common components and companies *");

			try
			{
				// Stop services
				StopServices();

				// Stop IIS
				StopIis();
				AddAction(UpdateAction.Iis);

				// Update common components
				UpdateCommon2(asp);

				// Update companies
				ICompanyInfo[] companies = configurator.ListCompanies(false);
				for (int i = 0; i < companies.Length; i++)
					UpdateCompany2(configurator, companies[i].Id, i + 1, companies.Length, false);

				// Start IIS
				try
				{
					StartIis();
				}
				catch { }

				// Start services
				StartServices();
			}
			catch (Exception ex)
			{
				LogFile.WriteLine("* Failed *");
				LogFile.WriteLine();
				LogFile.WriteLine(ex.ToString());
				LogFile.WriteLine();

				#region Undo changes

				// Start IIS
				try
				{
					if (_actions.Contains(UpdateAction.Iis))
						StartIis();
				}
				catch { }

				// Start services
				StartServices();

				#endregion

				throw;
			}
			finally
			{
				LogFile.WriteLine();
			}

			LogFile.WriteLine("* OK *");
		}
		#endregion

		#region static void UpdateCommon(IAspInfo asp)
		static void UpdateCommon(IAspInfo asp)
		{
			try
			{
				// Stop services
				StopServices();

				// Stop IIS
				StopIis();
				AddAction(UpdateAction.Iis);

				UpdateCommon2(asp);

				// Start IIS
				try
				{
					StartIis();
				}
				catch { }

				// Start services
				StartServices();
			}
			catch (Exception ex)
			{
				LogFile.WriteLine("* Failed *");
				LogFile.WriteLine();
				LogFile.WriteLine(ex.ToString());
				LogFile.WriteLine();

				#region Undo changes

				// Start IIS
				try
				{
					if (_actions.Contains(UpdateAction.Iis))
						StartIis();
				}
				catch { }

				// Start services
				StartServices();

				#endregion

				throw;
			}
			finally
			{
				LogFile.WriteLine();
			}
		}
		#endregion

		#region static void UpdateCommon2(IAspInfo asp)
		static void UpdateCommon2(IAspInfo asp)
		{
			LogFile.WriteLine();
			LogFile.WriteLine("* Updating common components *");

			string commonRoot = Settings.InstallDir;

			try
			{
				int maxUpdateVersion = FindMaxUpdateVersion();
				Version version = new Version(IbnConst.FullVersion);
				int newCommonVersion = Math.Max(version.Build, maxUpdateVersion);

				// Update files
				UpdateFiles(Settings.CommonVersion, commonRoot, false);

				// Update configs
				UpdateConfigs(Settings.CommonVersion, null);

				// Update ASP database
				if (asp != null)
				{
					string aspPath = Path.Combine(commonRoot, "Asp");
					string connectionString = GetWebApplicationConnectionString(aspPath);
					using (DataContext dataContext = new DataContext(connectionString))
					using (DataContextSwitcher switcher = new DataContextSwitcher(dataContext))
					using (TransactionScope transaction = dataContext.BeginTransaction())
					{
						UpdateDatabase(ScriptType.SqlAsp, asp.Database, null);

						// Update common version in registry
						Settings.UpdateCommonVersion(newCommonVersion);

						transaction.Commit();
					}
				}

				if (asp == null)
				{
					// Update common version in registry
					Settings.UpdateCommonVersion(newCommonVersion);
				}
			}
			catch (Exception ex)
			{
				LogFile.WriteLine("* Failed *");
				LogFile.WriteLine();
				LogFile.WriteLine(ex.ToString());
				LogFile.WriteLine();
				LogFile.WriteLine("Rollback started");

				#region Undo changes

				// Recover configs
				RecoverConfigs();

				// Recover files
				RecoverFiles(commonRoot);

				#endregion

				LogFile.WriteLine("Rollback finished");
				throw;
			}
			finally
			{
				// Delete backup files
				DeleteBackupFiles();
			}

			LogFile.WriteLine("* OK *");
		}
		#endregion

		#region static void UpdateCompany(IConfigurator configurator, string companyId)
		static void UpdateCompany(IConfigurator configurator, string companyId)
		{
			UpdateCompany2(configurator, companyId, 1, 1, true);
		}
		#endregion
		#region static void UpdateCompany(IConfigurator configurator, string companyId, int companyIndex, int companiesCount, bool processServices)
		static void UpdateCompany2(IConfigurator configurator, string companyId, int companyIndex, int companiesCount, bool processServices)
		{
			string counter = string.Empty;
			if (companiesCount > 1)
				counter = string.Format(CultureInfo.InvariantCulture, " {0} of {1}", companyIndex, companiesCount);

			LogFile.WriteLine();
			LogFile.WriteLine("* Updating Company", counter, " *");
			LogFile.WriteFormatted("* Id: {0} *", companyId);

			// Load company info
			ICompanyInfo company = configurator.GetCompanyInfo(companyId);

			if (company == null)
				LogFile.WriteLine("* Company not found *");
			else
			{
				LogFile.WriteFormatted("* Host: {0} *", company.Host);

				string companyRoot = Path.Combine(configurator.InstallPath, company.CodePath);
				string webPath = Path.Combine(companyRoot, "Web");
				string portalPath = Path.Combine(webPath, "Portal");
				IIisManager iisManager = IisManager.Create(webPath);

				try
				{
					ManageDemoDatabase(configurator, false, company.Host, company.Database);

					string connectionString = GetWebApplicationConnectionString(portalPath);
					using (DataContext dataContext = new DataContext(connectionString))
					using (DataContextSwitcher switcher = new DataContextSwitcher(dataContext))
					using (TransactionScope transaction = dataContext.BeginTransaction())
					{
						if (processServices)
						{
							// Disable ScheduleService for company
							if (company.IsScheduleServiceEnabled)
								DisableScheduleService(configurator, companyId);

							if (iisManager.IisVersion > 5)
							{
								// IIS 6+: Stop web site
								StopWebSite(iisManager, company.SiteId);

								// IIS 6+: Stop portal pool
								StopApplicationPool(iisManager, company.PortalPool);

								// IIS 6+: Stop IM pool
								StopApplicationPool(iisManager, company.IMPool);
							}
							else
							{
								// IIS 5: Stop IIS
								StopIis();
							}
						}

						// Update files
						UpdateFiles(company.CodeVersion, companyRoot, true);

						// Update configs
						UpdateConfigs(company.CodeVersion, companyRoot);

						if (company.DatabaseState == 6) // Ready
							UpdateDatabase(ScriptType.SqlPortal, company.Database, companyRoot);
						else
							LogFile.WriteLine("* Company database is not ready *");

						// Update company version in ibn.config
						configurator.UpdateCompanyVersion(companyId);

						if (processServices)
						{
							if (iisManager.IisVersion > 5)
							{
								// IIS 6+: Start IM pool
								StartApplicationPool(iisManager, company.IMPool);
								// IIS 6+: Start portal pool
								StartApplicationPool(iisManager, company.PortalPool);
								// Start web site
								StartWebSite(iisManager, company.SiteId);
							}
							else
							{
								// IIS 5: Start IIS
								StartIis();
							}

							// Enable ScheduleService for company
							if (company.IsScheduleServiceEnabled)
								EnableScheduleService(configurator, companyId);
						}

						transaction.Commit();
					}
				}
				catch (Exception ex)
				{
					LogFile.WriteLine("* Failed *");
					LogFile.WriteLine();
					LogFile.WriteLine(ex.ToString());
					LogFile.WriteLine();
					LogFile.WriteLine("Rollback started");

					#region Undo changes

					// Recover configs
					RecoverConfigs();

					// Recover files
					RecoverFiles(companyRoot);

					if (processServices)
					{
						if (iisManager.IisVersion > 5)
						{
							// IIS 6+: Start IM pool
							try
							{
								StartApplicationPool(iisManager, company.IMPool);
							}
							catch { }

							// IIS 6+: Start portal pool
							try
							{
								StartApplicationPool(iisManager, company.PortalPool);
							}
							catch { }

							// Start web site
							try
							{
								StartWebSite(iisManager, company.SiteId);
							}
							catch { }
						}
						else
						{
							// IIS 5: Start IIS
							try
							{
								StartIis();
							}
							catch { }
						}

						// Enable ScheduleService for company
						if (company.IsScheduleServiceEnabled)
							EnableScheduleService(configurator, companyId);
					}

					#endregion

					LogFile.WriteLine("Rollback finished");
					throw;
				}
				finally
				{
					// Delete backup files
					DeleteBackupFiles();
				}

				try
				{
					ManageDemoDatabase(configurator, true, company.Host, company.Database);
				}
				catch (Exception ex)
				{
					LogWriteFailed();
					LogFile.WriteLine(ex.ToString());
				}

				LogFile.WriteLine("* OK *");
			}
		}
		#endregion


		#region static int StartProcess(string fileName, string arguments)
		static int StartProcess(string fileName, string arguments)
		{
			using (Process p = new Process())
			{
				p.StartInfo.UseShellExecute = false;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.FileName = fileName;
				p.StartInfo.Arguments = arguments;

				p.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
				p.Start();
				p.BeginOutputReadLine();
				p.WaitForExit();

				return p.ExitCode;
			}
		}
		#endregion
		#region static void OutputHandler(object sendingProcess, DataReceivedEventArgs e)
		static void OutputHandler(object sendingProcess, DataReceivedEventArgs e)
		{
			LogFile.WriteLine(e.Data);
		}
		#endregion

		#region static void UpdateFiles(int currentVersion, string targetRoot, bool targetIsCompany)
		static void UpdateFiles(int currentVersion, string targetRoot, bool targetIsCompany)
		{
			List<IbnVersionInfo> files = new List<IbnVersionInfo>();

			GetDeletedFiles(files, currentVersion, targetIsCompany);
			GetFiles(files, currentVersion, targetIsCompany);

			files.Sort();

			BackupFiles(files, targetRoot);
			UpdateFiles(files, targetRoot);
		}
		#endregion
		#region static void UpdateFiles(List<IbnVersionInfo> files, string targetRoot)
		static void UpdateFiles(List<IbnVersionInfo> files, string targetRoot)
		{
			LogFile.WriteLine("Updating files...");
			try
			{
				_copiedFiles.Clear();
				ProcessFiles(false, null, targetRoot, files);
				LogWriteOk();
			}
			catch
			{
				LogWriteFailed();
				throw;
			}
		}
		#endregion
		#region static void BackupFiles(List<IbnVersionInfo> files, string sourceRoot)
		static void BackupFiles(List<IbnVersionInfo> files, string sourceRoot)
		{
			LogFile.WriteLine("Saving backup files...");
			try
			{
				ProcessFiles(true, sourceRoot, Settings.BackupDir, files);
				LogWriteOk();
			}
			catch
			{
				LogWriteFailed();
				throw;
			}
		}
		#endregion
		#region static void ProcessFiles(bool backupFiles, string sourceRoot, string targetRoot, List<IbnVersionInfo> files)
		static void ProcessFiles(bool backupFiles, string sourceRoot, string targetRoot, List<IbnVersionInfo> files)
		{
			List<string> dirs = new List<string>();

			foreach (IbnVersionInfo vi in files)
			{
				string relativePath = vi.Data;
				string sourceFile;
				if (sourceRoot != null)
					sourceFile = Path.Combine(sourceRoot, relativePath);
				else
					sourceFile = vi.Data2;

				string targetFile = Path.Combine(targetRoot, relativePath);

				try
				{
					string targetDir = targetFile.Substring(0, targetFile.LastIndexOf(Path.DirectorySeparatorChar) + 1);

					if (!dirs.Contains(targetDir))
					{
						Directory.CreateDirectory(targetDir);
						dirs.Add(targetDir);
					}

					if (File.Exists(targetFile))
						File.SetAttributes(targetFile, FileAttributes.Normal);

					if (backupFiles)
					{
						if (File.Exists(sourceFile))
						{
							File.Copy(sourceFile, targetFile, true);
							AddAction(UpdateAction.BackupFiles);
							_backupFiles.Add(targetFile);
						}
					}
					else // copy and delete files
					{
						if (string.IsNullOrEmpty(sourceFile))
							File.Delete(targetFile);
						else
							File.Copy(sourceFile, targetFile, true);

						AddAction(UpdateAction.UpdateFiles);
						AddCopiedFile(relativePath);
					}

					if (File.Exists(targetFile))
						File.SetAttributes(targetFile, FileAttributes.Normal);
				}
				catch (Exception ex)
				{
					throw new UpdateException(string.Format(CultureInfo.InvariantCulture, "Cannot copy file from '{0}' to '{1}'.", sourceFile, targetFile), ex);
				}
			}
		}
		#endregion
		#region static void AddCopiedFile(string relativePath)
		static void AddCopiedFile(string relativePath)
		{
			if (!_copiedFiles.Contains(relativePath))
				_copiedFiles.Add(relativePath);
		}
		#endregion

		#region static void GetFiles(List<IbnVersionInfo> files, int currentVersion, bool targetIsCompany)
		static void GetFiles(List<IbnVersionInfo> files, int currentVersion, bool targetIsCompany)
		{
			string path = Settings.UpdateDir;

			foreach (string updateDir in Directory.GetDirectories(path))
			{
				int i = updateDir.LastIndexOf("_", StringComparison.OrdinalIgnoreCase);
				if (i != -1)
				{
					int version = int.Parse(updateDir.Substring(i + 1), CultureInfo.InvariantCulture);
					if (version > currentVersion)
					{
						string filesDir = Path.Combine(updateDir, "files");
						GetFiles(filesDir, filesDir.Length, version, files, targetIsCompany);
					}
				}
			}
		}
		#endregion
		#region static void GetFiles(string dir, int rootPathLen, int version, List<IbnVersionInfo> files, bool targetIsCompany)
		static void GetFiles(string dir, int rootPathLen, int version, List<IbnVersionInfo> files, bool targetIsCompany)
		{
			if (Directory.Exists(dir))
			{
				string relativePath;
				foreach (string file in Directory.GetFiles(dir))
				{
					relativePath = file.Substring(rootPathLen + 1);

					bool add = true;
					if (targetIsCompany)
					{
						if (relativePath.StartsWith(ConstCodeSource, StringComparison.OrdinalIgnoreCase)
							&& string.Compare(relativePath, @"Code\_Source\Web\Portal\Web.config", StringComparison.OrdinalIgnoreCase) != 0
							)
							relativePath = relativePath.Substring(ConstCodeSource.Length);
						else
							add = false;
					}

					if (add)
						files.Add(new IbnVersionInfo(version, 1, relativePath, file));
				}

				foreach (string child in Directory.GetDirectories(dir))
					GetFiles(child, rootPathLen, version, files, targetIsCompany);
			}
		}
		#endregion
		#region static void GetDeletedFiles(List<IbnVersionInfo> files, int currentVersion, bool targetIsCompany)
		static void GetDeletedFiles(List<IbnVersionInfo> files, int currentVersion, bool targetIsCompany)
		{
			List<IbnVersionInfo> scripts = GetScripts(ScriptType.Files, currentVersion);
			foreach (IbnVersionInfo vi in scripts)
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(vi.Data);

				foreach (XmlNode node in doc.SelectNodes("/files/delete/file"))
				{
					string relativePath = node.InnerText;

					bool add = true;
					if (targetIsCompany)
					{
						if (relativePath.StartsWith(ConstCodeSource, StringComparison.OrdinalIgnoreCase))
							relativePath = relativePath.Substring(ConstCodeSource.Length);
						else
							add = false;
					}

					if (add)
						files.Add(new IbnVersionInfo(vi.Version, 1, relativePath));
				}
			}
		}
		#endregion
		#region static List<IbnVersionInfo> GetScripts(ScriptType type, int currentVersion)
		static List<IbnVersionInfo> GetScripts(ScriptType type, int currentVersion)
		{
			List<IbnVersionInfo> list = new List<IbnVersionInfo>();

			string path = Settings.UpdateDir;
			foreach (string dir in Directory.GetDirectories(path))
			{
				try
				{
					int i = dir.LastIndexOf("_", StringComparison.OrdinalIgnoreCase);
					if (i != -1)
					{
						int version = int.Parse(dir.Substring(i + 1), CultureInfo.InvariantCulture);
						if (version > currentVersion)
						{
							string dirScript = dir + "\\scripts\\";
							string[] names = null;

							switch (type)
							{
								case ScriptType.Config:
									names = new string[] { "config.xml" };
									break;
								case ScriptType.Files:
									names = new string[] { "files.xml" };
									break;
								case ScriptType.SqlAsp:
									names = new string[] { "asp.sql" };
									break;
								case ScriptType.SqlPortal:
									names = new string[] { "1before.sql", "2scheme.sql", "metamodelCommands.xml", "3after.sql" };
									break;
								default:
									throw new UpdateException("Unsupported script type.");
							}

							if (names != null)
							{
								for (int nameIndex = 0; nameIndex < names.Length; nameIndex++)
								{
									string name = dirScript + names[nameIndex];
									if (File.Exists(name))
									{
										list.Add(new IbnVersionInfo(version, nameIndex + 1, name));
									}
								}
							}
						}
					}
				}
				catch
				{
				}
			}
			list.Sort();

			return list;
		}
		#endregion

		#region static void AddAction(UpdateAction action)
		static void AddAction(UpdateAction action)
		{
			if (!_actions.Contains(action))
				_actions.Insert(0, action);
		}
		#endregion

		#region internal static void LogWriteOk()
		internal static void LogWriteOk()
		{
			LogFile.WriteLine("OK");
		}
		#endregion
		#region internal static void LogWriteFailed()
		internal static void LogWriteFailed()
		{
			LogFile.WriteLine("Failed");
		}
		#endregion

		#region static void UpdateDatabase(ScriptType type, string databaseName, string companyRoot)
		static void UpdateDatabase(ScriptType type, string databaseName, string companyRoot)
		{
			LogFile.WriteFormatted("Updating database '{0}'", databaseName);

			SqlContext.Current.CommandTimeout = ConstCommandTimeout;

			Version version = new Version(IbnConst.FullVersion);

			int currentVersion;
			#region Get database version
			{
				bool databaseHasVersion = false;
				int major = 0;
				int minor = 0;
				int build = 0;

				try
				{
					using (IDataReader reader = SqlHelper.ExecuteReader(SqlContext.Current, CommandType.Text, "SELECT * FROM [DatabaseVersion]"))
					{
						if (reader.Read())
						{
							databaseHasVersion = true;

							major = (int)reader["Major"];
							minor = (int)reader["Minor"];
							build = (int)reader["Build"];
						}
					}
				}
				catch
				{
				}

				if (databaseHasVersion)
				{
					if (major != version.Major || minor != version.Minor || build < 50)
						throw new UpdateException(string.Format(CultureInfo.InvariantCulture, "Unsupported database version: {0}.{1}.{2}. Required version is 4.7.50 or later.", major, minor, build));

					currentVersion = build;
				}
				else
					throw new UpdateException("Unknown database version. Required version is 4.7.50 or later.");
			}
			#endregion

			List<IbnVersionInfo> scriptList = GetScripts(type, currentVersion);

			int maxScriptVersion = currentVersion;

			NameValueCollection replace = new NameValueCollection();
			replace["SET XACT_ABORT ON"] = string.Empty;

			foreach (IbnVersionInfo vi in scriptList)
			{
				if (vi.Data.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
				{
					ExecuteSqlScript(vi.Data, replace);

					if (maxScriptVersion < vi.Version)
						maxScriptVersion = vi.Version;
				}
				else if (vi.Data.EndsWith("metamodelCommands.xml", StringComparison.OrdinalIgnoreCase))
				{
					UpdateMetaModel(companyRoot, vi.Data);

					if (maxScriptVersion < vi.Version)
						maxScriptVersion = vi.Version;
				}
			}

			if (type == ScriptType.SqlPortal)
				TimeZoneManager.FillTimeConversionTables();

			// Update database version
			int maxBuild = Math.Max(version.Build, maxScriptVersion);
			NameValueCollection versionReplace = new NameValueCollection();
			versionReplace["{MajorVersion}"] = version.Major.ToString(CultureInfo.InvariantCulture);
			versionReplace["{MinorVersion}"] = version.Minor.ToString(CultureInfo.InvariantCulture);
			versionReplace["{BuildNumber}"] = maxBuild.ToString(CultureInfo.InvariantCulture);
			ExecuteSqlScript(Path.Combine(Settings.InstallDir, @"Code\_Source\Tools\version.sql"), versionReplace);

			// Update database state
			SqlHelper.ExecuteNonQuery(SqlContext.Current, CommandType.Text, "UPDATE [DatabaseVersion] SET [State]=6");
		}
		#endregion

		#region static void ExecuteSqlScript(string scriptFilePath, NameValueCollection replace)
		static void ExecuteSqlScript(string scriptFilePath, NameValueCollection replace)
		{
			LogFile.WriteFormatted("Executing SQL script '{0}'...", scriptFilePath);
			try
			{
				StringBuilder builder = new StringBuilder(File.ReadAllText(scriptFilePath));
				if (replace != null)
				{
					foreach (string oldValue in replace.AllKeys)
						builder = builder.Replace(oldValue, replace[oldValue]);
				}

				SqlHelper.ExecuteScript(SqlContext.Current, builder.ToString());

				LogWriteOk();
			}
			catch
			{
				LogWriteFailed();
				throw;
			}
		}
		#endregion

		#region static void UpdateMetaModel(string companyRoot, string commandsFilePath)
		static void UpdateMetaModel(string companyRoot, string commandsFilePath)
		{
			LogFile.WriteLine("Updating metamodel...");

			SchemaDocument schema = new SchemaDocument();
			schema.Load(Path.Combine(companyRoot, @"Tools\metamodelSchema.xml"));

			SyncCommand[] commands = McXmlSerializer.GetObjectFromFile<SyncCommand[]>(commandsFilePath);

			MetaModelSync.Execute(schema, commands);

			LogWriteOk();
		}
		#endregion

		#region static void StopServices()
		static void StopServices()
		{
			StopService("ScheduleService" + IbnConst.VersionMajorMinor, false);
			StopService("McOleDBService" + IbnConst.VersionMajorMinor, false);
		}
		#endregion
		#region static void StartServices()
		static void StartServices()
		{
			foreach (string name in _services)
				StartService(name);
		}
		#endregion
		#region static void StopService(string name, bool mustExist)
		static void StopService(string name, bool mustExist)
		{
			LogFile.WriteFormatted("Stopping service '{0}'...", name);
			try
			{
				ServiceController service = new ServiceController(name);

				List<string> dependentServices = new List<string>();
				GetDependentServices(service, dependentServices);

				if (service.Status == ServiceControllerStatus.Running)
				{
					service.Stop();
					service.WaitForStatus(ServiceControllerStatus.Stopped);

					foreach (string dependentService in dependentServices)
						_services.Insert(0, dependentService);
					_services.Insert(0, name);

					AddAction(UpdateAction.Services);
				}
				LogWriteOk();
			}
			catch
			{
				LogWriteFailed();
				if (mustExist)
					throw;
			}
		}
		#endregion
		#region static void StartService(string name)
		static void StartService(string name)
		{
			LogFile.WriteFormatted("Starting service '{0}'...", name);
			try
			{
				ServiceController sc = new ServiceController(name);
				if (sc.Status == ServiceControllerStatus.Stopped)
				{
					sc.Start();
					sc.WaitForStatus(ServiceControllerStatus.Running);
				}
				LogWriteOk();
			}
			catch
			{
				LogWriteFailed();
			}
		}
		#endregion
		#region static void GetDependentServices(ServiceController service, List<string> names)
		static void GetDependentServices(ServiceController service, List<string> names)
		{
			foreach (ServiceController sc in service.DependentServices)
			{
				if (sc.Status == ServiceControllerStatus.Running || sc.Status == ServiceControllerStatus.Paused)
				{
					GetDependentServices(sc, names);
					if (!names.Contains(sc.ServiceName))
						names.Add(sc.ServiceName);
				}
			}
		}
		#endregion

		#region static void RecoverFiles(string targetRoot)
		static void RecoverFiles(string targetRoot)
		{
			LogFile.WriteLine("Rolling back files...");
			try
			{
				foreach (string relFile in _copiedFiles)
				{
					string sourceFile = Path.Combine(Settings.BackupDir, relFile);
					string targetFile = Path.Combine(targetRoot, relFile);

					if (File.Exists(sourceFile))
					{
						if (File.Exists(targetFile))
							File.SetAttributes(targetFile, FileAttributes.Normal);

						File.Copy(sourceFile, targetFile, true);
					}
					else
					{
						File.Delete(targetFile);
					}
				}
				LogWriteOk();
			}
			catch
			{
				LogWriteFailed();
			}
		}
		#endregion

		#region static void DeleteBackupFiles()
		static void DeleteBackupFiles()
		{
			if (_backupFiles.Count > 0)
			{
				LogFile.WriteLine("Deleting backup files...");
				foreach (string fileName in _backupFiles)
				{
					try
					{
						File.Delete(fileName);
					}
					catch
					{
					}
				}
				LogWriteOk();
			}
		}
		#endregion

		#region private static void UpdateConfigs(int currentVersion, string companyRoot)
		private static void UpdateConfigs(int currentVersion, string companyRoot)
		{
			LogFile.WriteLine("Updating configuration files...");
			try
			{
				_configs.Clear();
				List<IbnVersionInfo> scripts = GetScripts(ScriptType.Config, currentVersion);

				XmlDocument configurationDoc = new XmlDocument();
				foreach (IbnVersionInfo vi in scripts)
				{
					configurationDoc.Load(vi.Data);
					foreach (XmlNode config in configurationDoc.SelectNodes("/configs/config"))
					{
						string path = config.Attributes["path"].Value;
						// If updating company
						// or if updating common components and config is not in Code\_Source\Web folder
						// then do update
						if (!string.IsNullOrEmpty(companyRoot) || !path.Contains(@"Code\_Source\Web\"))
						{
							path = ExpandVars(path, companyRoot);
							if (File.Exists(path))
							{
								XmlDocument doc = new XmlDocument();
								doc.Load(path);
								string originalDoc = doc.OuterXml;

								ProcessConfig(doc, config, companyRoot);
								doc.Save(path);

								// Save config for rollback.
								if (!_configs.ContainsKey(path))
									_configs.Add(path, originalDoc);
							}
						}
					}
				}
				LogWriteOk();
			}
			catch
			{
				LogWriteFailed();
				throw;
			}
			finally
			{
				AddAction(UpdateAction.Config);
			}
		}
		#endregion
		#region static void ProcessConfig(XmlDocument doc, XmlNode config, string companyRoot)
		static void ProcessConfig(XmlDocument doc, XmlNode config, string companyRoot)
		{
			foreach (XmlNode action in config.ChildNodes)
			{
				if (action.NodeType == XmlNodeType.Element)
				{
					string parentPath = action.Attributes["parent"].Value;
					XmlNode parent = doc.SelectSingleNode(parentPath);

					if (action.Name == "ifexists" && parent != null)
					{
						ProcessConfig(doc, action, companyRoot);
					}
					else if (action.Name == "add")
					{
						string type = action.Attributes["type"].Value;
						if (type == "element")
						{
							XmlNode refChild = parent.LastChild;
							XmlAttribute position = action.Attributes["position"];
							if (position != null && position.Value == "first")
								refChild = null;

							foreach (XmlNode child in action.ChildNodes)
							{
								refChild = parent.InsertAfter(doc.ImportNode(child, true), refChild);
							}
						}
						else if (type == "attr")
						{
							string name = action.Attributes["name"].Value;
							string val = ExpandVars(action.Attributes["value"].Value, companyRoot);
							parent.Attributes.Append(doc.CreateAttribute(name)).Value = val;
						}
					}
					else if (action.Name == "set")
					{
						string val;

						XmlAttribute v = action.Attributes["value"];
						if (v != null)
							val = ExpandVars(v.Value, companyRoot);
						else
							val = doc.SelectSingleNode(action.Attributes["valueXml"].Value).Value;

						parent.Value = val;
					}
					else if (action.Name == "del")
					{
						foreach (XmlNode item in action.SelectNodes("item"))
						{
							foreach (XmlNode child in parent.SelectNodes(item.Attributes["path"].Value))
								parent.RemoveChild(child);
						}
					}
				}
			}
		}
		#endregion
		#region private static void RecoverConfigs()
		private static void RecoverConfigs()
		{
			XmlDocument doc = new XmlDocument();
			foreach (string path in _configs.Keys)
			{
				doc.LoadXml(_configs[path]);
				doc.Save(path);
			}
		}
		#endregion
		#region static string ExpandVars(string val, string companyRoot)
		static string ExpandVars(string val, string companyRoot)
		{
			StringBuilder builder = new StringBuilder(val);

			if(string.IsNullOrEmpty(companyRoot))
				builder.Replace("{InstallDir}", Settings.InstallDir);
			else
				builder.Replace(@"{InstallDir}Code\_Source", companyRoot);

			return builder.ToString();
		}
		#endregion

		#region private static string GetWebApplicationConnectionString(string webApplicationPath)
		private static string GetWebApplicationConnectionString(string webApplicationPath)
		{
			string webConfigPath = Path.Combine(webApplicationPath, @"Web.config");

			XmlDocument webConfig = new XmlDocument();
			webConfig.Load(webConfigPath);

			XmlNode connectionStringNode = webConfig.SelectSingleNode("configuration/appSettings/add[@key='ConnectionString']/@value");

			if (connectionStringNode == null)
				throw new UpdateException("Cannot find ConnectionString.");

			return connectionStringNode.Value + ";Connection Timeout=" + ConstCommandTimeout.ToString(CultureInfo.InvariantCulture);
		}
		#endregion

		#region static int FindMaxUpdateVersion()
		static int FindMaxUpdateVersion()
		{
			int maxVersion = 0;

			foreach (string dir in Directory.GetDirectories(Settings.UpdateDir))
			{
				try
				{
					int i = dir.LastIndexOf("_", StringComparison.OrdinalIgnoreCase);
					if (i != -1)
					{
						int version = int.Parse(dir.Substring(i + 1), CultureInfo.InvariantCulture);
						if (version > maxVersion)
							maxVersion = version;
					}
				}
				catch
				{
				}
			}

			return maxVersion;
		}
		#endregion

		#region static void CopyDirectory(string sourcePath, string targetPath)
		static void CopyDirectory(string sourcePath, string targetPath)
		{
			Directory.CreateDirectory(targetPath);

			foreach (string sourceFilePath in Directory.GetFiles(sourcePath))
			{
				string fileName = Path.GetFileName(sourceFilePath);
				string targetFilePath = Path.Combine(targetPath, fileName);
				File.Copy(sourceFilePath, targetFilePath);
			}

			foreach (string sourceDirectoryPath in Directory.GetDirectories(sourcePath))
			{
				string directoryName = Path.GetFileName(sourceDirectoryPath);
				string targetDirectoryPath = Path.Combine(targetPath, directoryName);
				CopyDirectory(sourceDirectoryPath, targetDirectoryPath);
			}
		}
		#endregion

		#region static void StartIis()
		static void StartIis()
		{
			LogFile.WriteLine("Starting IIS...");
			StartProcess(ConstIisReset, "/start");
			LogWriteOk();
		}
		#endregion 
		#region static void StopIis()
		static void StopIis()
		{
			LogFile.WriteLine("Stopping IIS...");
			StartProcess(ConstIisReset, "/stop");
			LogWriteOk();
		}
		#endregion

		#region static void StartWebSite(IIisManager iisManager, long siteId)
		static void StartWebSite(IIisManager iisManager, long siteId)
		{
			LogFile.WriteFormatted("Starting web site '{0}", siteId);
			iisManager.StartSite(siteId);
			LogWriteOk();
		}
		#endregion
		#region static void StopWebSite(IIisManager iisManager, long siteId)
		static void StopWebSite(IIisManager iisManager, long siteId)
		{
			LogFile.WriteFormatted("Stopping web site '{0}", siteId);
			iisManager.StopSite(siteId);
			LogWriteOk();
		}
		#endregion
		#region static void StartApplicationPool(IIisManager iisManager, string name)
		static void StartApplicationPool(IIisManager iisManager, string name)
		{
			LogFile.WriteFormatted("Starting IIS application pool'{0}", name);
			iisManager.StartApplicationPool(name);
			LogWriteOk();
		}
		#endregion
		#region static void StopApplicationPool(IIisManager iisManager, string name)
		static void StopApplicationPool(IIisManager iisManager, string name)
		{
			LogFile.WriteFormatted("Stopping IIS application pool'{0}", name);
			iisManager.StopApplicationPool(name, true);
			LogWriteOk();
		}
		#endregion

		#region static void EnableScheduleService(IConfigurator configurator, string companyId)
		static void EnableScheduleService(IConfigurator configurator, string companyId)
		{
			LogFile.WriteFormatted("Enabling schedule service for company");
			configurator.EnableScheduleService(companyId, true);
			LogWriteOk();
		}
		#endregion
		#region static void DisableScheduleService(IConfigurator configurator, string companyId)
		static void DisableScheduleService(IConfigurator configurator, string companyId)
		{
			LogFile.WriteFormatted("Disabling schedule service for company");
			configurator.EnableScheduleService(companyId, false);
			LogWriteOk();
		}
		#endregion

		#region static void ManageDemoDatabase(IConfigurator configurator, bool backup, string host, string database)
		static void ManageDemoDatabase(IConfigurator configurator, bool backup, string host, string database)
		{
			if (!string.IsNullOrEmpty(Settings.DemoHosts) && !string.IsNullOrEmpty(Settings.DemoBackupDir))
			{
				bool process = false;

				string[] demoHosts = Settings.DemoHosts.Split(',');
				foreach (string demoHost in demoHosts)
				{
					if (string.Compare(demoHost, host, StringComparison.OrdinalIgnoreCase) == 0)
					{
						process = true;
						break;
					}
				}

				if (process)
				{
					string message;
					string queryTemplate;
					if (backup)
					{
						message = "Backing up database...";
						queryTemplate = "BACKUP DATABASE [{0}] TO DISK='{1}' WITH INIT";
					}
					else
					{
						message = "Restoring database...";
						queryTemplate = "RESTORE DATABASE [{0}] FROM DISK='{1}' WITH REPLACE";
					}

					string backupFile = Path.Combine(Settings.DemoBackupDir, database + ".bak");
					string query = string.Format(CultureInfo.InvariantCulture, queryTemplate, database, backupFile);

					LogFile.WriteLine(message);

					using (DataContext dataContext = new DataContext(configurator.SqlSettings.AdminConnectionString))
					using (DataContextSwitcher switcher = new DataContextSwitcher(dataContext))
					{
						SqlHelper.ExecuteReader(SqlContext.Current, CommandType.Text, query);
						LogWriteOk();
					}
				}
			}
		}
		#endregion
	}
}
