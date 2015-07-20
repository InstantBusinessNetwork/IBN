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
using System.Xml;

using Mediachase.Database;
using Mediachase.Ibn;
using Mediachase.Ibn.Configuration;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Schema;

namespace IbnServerUpdate
{
	class Program
	{
		const int CommandTimeout = 36000;

		static List<UpdateAction> _actions = new List<UpdateAction>();
		static List<string> _services = new List<string>();
		static List<string> _backupFiles = new List<string>();
		static List<string> _copiedFiles = new List<string>();

		#region static int Main(string[] args)
		static int Main(string[] args)
		{
			bool ok = false;
			Error ret = Error.OK;

			if (!Settings.Init(args))
			{
				ret = Error.InvalidArgs;
			}
			else
			{
				LogFile.Open(Settings.LogFile);
				try
				{
					// Validate current version
					if (Settings.Version != 47)
						LogFile.WriteMessageFormat("This update can be installed only on IBN Server 4.7.47.");
					else
					{
						Update();
						ok = true;
					}
				}
				catch (Exception ex)
				{
					string s = ex.ToString();
					LogFile.WriteLine("");
					LogFile.WriteMessageFormat("Error details:\n{0}", s);
					ret = Error.Unknown;
				}
				finally
				{
					LogFile.WriteLine("");
					LogFile.Close();
					if (!ok)
						LogFile.Show();
				}
			}
			return (int)ret;
		}
		#endregion

		#region static void Update()
		static void Update()
		{
			LogFile.WriteMessageFormat("*** Updating Mediachase {0} ***", IbnConst.ProductName);

			//GlobalContext.Current = new GlobalContext(Settings.InstallDirWeb + @"portal\Apps");
			DBHelper2.Init(new DBHelper(string.Format(CultureInfo.InvariantCulture, "Data source={0};Initial catalog=master;User Id={1};Password={2};Connection Timeout=36000", Settings.SqlServer, Settings.SqlUser, Settings.SqlPassword)));
			DBHelper2.DBHelper.CommandTimeout = CommandTimeout;

			IConfigurator configurator = Configurator.Create();
			IIisManager oldIisManager = IisManager.Create(Settings.InstallDirWeb);
			bool x64 = oldIisManager.Is64Bit();

			// Get copmanies
			CompanyInfo[] oldCompanies = CompanyInfo.ListCompanies(DBHelper2.DBHelper);
			bool isAsp = (oldCompanies.Length > 1);

			List<string> newCompanies = ListNewCompanies(configurator);
			bool firstLaunch = (newCompanies == null);

			List<CompanyInfo> companies = new List<CompanyInfo>();
			if (firstLaunch)
				companies.AddRange(oldCompanies);
			else
			{
				foreach (CompanyInfo oldCompany in oldCompanies)
				{
					if (!newCompanies.Contains(oldCompany.Host))
						companies.Add(oldCompany);
				}
			}

			string newAspPath = Path.Combine(Settings.InstallDir, "Asp");
			string codePath = Path.Combine(Settings.InstallDir, "Code");
			string sourcePath = Path.Combine(codePath, "_Source");
			string newWebPath = Path.Combine(sourcePath, "Web");

			try
			{
				// Stop old web site
				oldIisManager.StopSite(Settings.SiteId);

				// Stop old services
				if (firstLaunch)
				{
					StopService("AlertService" + IbnConst.VersionMajorMinor, false);
					StopService("McOleDBService" + IbnConst.VersionMajorMinor, false);
					StopService("ScheduleService" + IbnConst.VersionMajorMinor, false);

					// Unregister old components
					RegisterComponents(false, GetOldComponents());

					// Unregister old IM Server
					RegisterIMServer(false, Settings.InstallDirWeb, x64);

					// TODO: ? Delete ISAPI extension
					// TODO: ? Delete portal application pool
					// TODO: ? Delete IM application pool

					// Copy old files to new location: InstallPath\Code\_Source
					LogFile.WriteLine("Copying old files to new location...");

					CopyDirectory(Path.Combine(Settings.InstallDir, @"Web\Asp"), newAspPath);
					CopyDirectory(Settings.InstallDir, sourcePath, "Tools");
					CopyDirectory(Settings.InstallDir, sourcePath, @"Web\Download");
					CopyDirectory(Settings.InstallDir, sourcePath, @"Web\instmsg");
					CopyDirectory(Settings.InstallDir, sourcePath, @"Web\instmsg64");
					CopyDirectory(Settings.InstallDir, sourcePath, @"Web\Portal");

					LogWriteOk();

					// Delete error logs
					Directory.Delete(Path.Combine(sourcePath, @"Web\Portal\Admin\Log\Error"), true);

					// Update files
					UpdateFiles();

					// Register new components
					RegisterComponents(true, GetNewComponents());

					// Register new IM Server
					RegisterIMServer(true, newWebPath, x64);

					// Start new services
					StartNewServices();
				}
				firstLaunch = false;

				Pause("Restore clean databases.");

				for (int i = 0; i < companies.Count; i++)
				{
					CompanyInfo company = companies[i];

					LogFile.WriteMessageFormat("* Converting company {0} of {1}: '{2}' *", i + 1, companies.Count, company.Host);

					try
					{
						ConvertCompany(configurator, isAsp, company);

						// Delete binding from old web site
						try
						{
							if (isAsp)
								oldIisManager.DeleteBinding(Settings.SiteId, company.Host);
						}
						catch
						{
						}

						LogFile.WriteLine("* OK *");
					}
					catch(Exception ex)
					{
						LogFile.WriteLine("* Failed *");
						LogFile.WriteLine("");

						if (isAsp)
							LogFile.WriteLine(ex.ToString());
						else
							throw;
					}
				}

				Settings.SaveVersion();

				Pause("Backup databases.");

				LogFile.WriteLine("*** OK ***");
			}
			catch (Exception ex)
			{
				LogFile.WriteLine("*** Failed ***");
				LogFile.WriteLine("");
				LogFile.WriteLine(ex.ToString());
				LogFile.WriteLine("");
				LogFile.WriteLine("Rollback started");

				#region Undo changes

				try
				{
					if (!isAsp || firstLaunch)
					{
						// Unregister new components
						try
						{
							RegisterComponents(false, GetNewComponents());
						}
						catch
						{
						}

						// Unregister new IM server
						try
						{
							RegisterIMServer(false, newWebPath, x64);
						}
						catch
						{
						}

						// Recover files
						try
						{
							RecoverFiles();
						}
						catch
						{
						}

						// Delete [InstallDir]\Code
						try
						{
							Directory.Delete(codePath, true);
						}
						catch
						{
						}

						// Delete [InstallDir]\Asp
						try
						{
							Directory.Delete(newAspPath, true);
						}
						catch
						{
						}

						// Register old components
						try
						{
							RegisterComponents(true, GetOldComponents());
						}
						catch
						{
						}

						// Register old IM Server
						try
						{
							RegisterIMServer(true, Settings.InstallDirWeb, x64);
						}
						catch
						{
						}

						// Start old services
						try
						{
							StartOldServices();
						}
						catch
						{
						}
					}

					// Start old web site
					try
					{
						oldIisManager.StartSite(Settings.SiteId);
					}
					catch
					{
					}
				}
				catch
				{
				}

				#endregion

				LogFile.WriteLine("Rollback finished");
				throw;
			}
			finally
			{
				LogFile.WriteLine("");
				// Delete backup files
				DeleteBackupFiles();
			}
		}
		#endregion

		#region static void ConvertCompany(IConfigurator configurator, bool isAsp, CompanyInfo company)
		static void ConvertCompany(IConfigurator configurator, bool isAsp, CompanyInfo company)
		{
			try
			{
				using (DBTransaction tran = DBHelper2.DBHelper.BeginTransaction())
				{
					// Load SMTP settings
					SmtpSettings smtpSettings = SmtpSettings.Load(company.Database);

					// Synchronize IM groups
					// Update portal DB
					// Copy data from main DB to portal DB
					NameValueCollection replace = new NameValueCollection();
					replace["SET XACT_ABORT ON"] = string.Empty;
					replace["{MainDB}"] = Settings.SqlDatabase;
					replace["{PortalDB}"] = company.Database;
					replace["{CompanyId}"] = company.OldId.ToString(CultureInfo.InvariantCulture);
					UpdateDatabase(company.Database, ScriptType.SqlPortal, replace);

					// Copy code to separate folder
					// Update portal web.config
					// Create ISAPI extension
					// Create portal application pool
					// Create IM application pool
					// Create web site
					// Update ibn.config
					int port = (string.IsNullOrEmpty(company.Port) ? 80 : int.Parse(company.Port, CultureInfo.InvariantCulture));
					string applicationPool = null;
					if (isAsp)
						applicationPool = RegistrySettings.ReadString("ApplicationPoolType" + company.CompanyType.ToString(CultureInfo.InvariantCulture));

					company.Id = configurator.CreateCompanyForDatabase(company.Database, company.Created, company.IsActive, company.Host, string.Empty, port, applicationPool, false);

					// Save company settings
					string previousDatabase = DBHelper2.DBHelper.Database;
					try
					{
						DBHelper2.DBHelper.Database = company.Database;

						// Save company images to portal database
						SaveCompanyParameter(DBHelper2.DBHelper, "portal.company_logo", company.CompanyLogo);
						SaveCompanyParameter(DBHelper2.DBHelper, "portal.homepage.image", company.HomePageImage);

						// Save scheme, host and port to database
						SaveCompanyParameter(DBHelper2.DBHelper, "system.scheme", "http");
						SaveCompanyParameter(DBHelper2.DBHelper, "system.host", company.Host);
						SaveCompanyParameter(DBHelper2.DBHelper, "system.port", company.Port);

						// Save SMTP settings
						smtpSettings.Save();

						// Set database state to "Ready"
						DBHelper2.DBHelper.RunText("UPDATE [DatabaseVersion] SET [State]=6");
					}
					finally
					{
						DBHelper2.DBHelper.Database = previousDatabase;
					}

					tran.Commit();
				}
			}
			catch
			{
				#region Undo changes

				// Delete company
				if (!string.IsNullOrEmpty(company.Id))
				{
					try
					{
						configurator.DeleteCompany(company.Id, false);
					}
					catch
					{
					}
				}

				#endregion

				throw;
			}
		}
		#endregion

		#region static List<string> ListNewCompanies(IConfigurator configurator)
		static List<string> ListNewCompanies(IConfigurator configurator)
		{
			List<string> list = null;

			try
			{
				ICompanyInfo[] companies = configurator.ListCompanies(false);
				list = new List<string>();
				foreach (ICompanyInfo company in companies)
				{
					list.Add(company.Host);
				}
			}
			catch (ObjectDisposedException)
			{
			}

			return list;
		}
		#endregion

		#region static void RegisterComponents(bool register, string[] components)
		static void RegisterComponents(bool register, string[] components)
		{
			foreach (string component in components)
			{
				int result = StartProcess(GetInstallUtil(), (register ? "" : "/u ") + "/LogToConsole=true \"" + component + "\"");
				if (register && result != 0)
					throw new IbnUpdateException("Failed to register " + component);
			}
		}
		#endregion
		#region static void RegisterIMServer(bool register, string webPath, bool x64)
		static void RegisterIMServer(bool register, string webPath, bool x64)
		{
			StartProcess("regsvr32.exe", (register ? "" : "/u ") + "/s " + WebName.GetIMPath(webPath, x64));
		}
		#endregion

		#region static string[] GetOldComponents()
		static string[] GetOldComponents()
		{
			string servicesPath = Path.Combine(Settings.InstallDir, "services");
			string portalBinPath = Path.Combine(Settings.InstallDirWeb, @"portal\bin");

			List<string> list = new List<string>();

			list.Add(Path.Combine(servicesPath, "AlertService.exe"));
			list.Add(Path.Combine(servicesPath, "OleDBService.exe"));
			list.Add(Path.Combine(portalBinPath, "ScheduleService.exe"));
			list.Add(Path.Combine(portalBinPath, "Mediachase.IBN.Business.dll"));

			return list.ToArray();
		}
		#endregion
		#region static string[] GetNewComponents()
		static string[] GetNewComponents()
		{
			string servicesPath = Settings.InstallDir;
			string portalBinPath = Path.Combine(Settings.InstallDir, @"Code\_Source\Web\Portal\bin");

			List<string> list = new List<string>();

			list.Add(Path.Combine(servicesPath, "Mediachase.Ibn.Configuration.dll"));
			list.Add(Path.Combine(servicesPath, "OleDBService.exe"));
			list.Add(Path.Combine(servicesPath, "ScheduleService.exe"));
			list.Add(Path.Combine(portalBinPath, "Mediachase.IBN.Business.dll"));

			return list.ToArray();
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

		#region static string GetInstallUtil()
		static string GetInstallUtil()
		{
			return Environment.ExpandEnvironmentVariables(@"%SystemRoot%\Microsoft.NET\Framework\v2.0.50727\InstallUtil.exe");
		}
		#endregion

		#region static void CopyDirectory(string sourceParentPath, string targetParentPath, string relativePath)
		static void CopyDirectory(string sourceParentPath, string targetParentPath, string relativePath)
		{
			string sourcePath = Path.Combine(sourceParentPath, relativePath);
			string targetPath = Path.Combine(targetParentPath, relativePath);
			CopyDirectory(sourcePath, targetPath);
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

		#region static void UpdateFiles()
		static void UpdateFiles()
		{
			List<IbnVersionInfo> files = new List<IbnVersionInfo>();

			GetDeletedFiles(files);
			GetFiles(files);

			files.Sort();

			BackupFiles(files);
			UpdateFiles(files);
		}
		#endregion

		#region static void UpdateFiles(List<IbnVersionInfo> files)
		static void UpdateFiles(List<IbnVersionInfo> files)
		{
			LogFile.WriteLine("Updating files...");
			try
			{
				_copiedFiles.Clear();
				ProcessFiles(false, null, Settings.InstallDir, files);
				LogWriteOk();
			}
			catch
			{
				LogWriteFailed();
				throw;
			}
		}
		#endregion

		#region static void GetDeletedFiles(List<IbnVersionInfo> files)
		static void GetDeletedFiles(List<IbnVersionInfo> files)
		{
			List<IbnVersionInfo> scripts = GetScripts(ScriptType.Files);
			foreach (IbnVersionInfo vi in scripts)
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(vi.Data);

				foreach (XmlNode node in doc.SelectNodes("/files/delete/file"))
				{
					string relativePath = node.InnerText;
					files.Add(new IbnVersionInfo(vi.Version, 1, relativePath));
				}
			}
		}
		#endregion

		#region static List<IbnVersionInfo> GetScripts(ScriptType type)
		static List<IbnVersionInfo> GetScripts(ScriptType type)
		{
			return GetScripts(type, Settings.Version);
		}
		#endregion
		#region static List<IbnVersionInfo> GetScripts(ScriptType type, int currentVersion)
		static List<IbnVersionInfo> GetScripts(ScriptType type, int currentVersion)
		{
			List<IbnVersionInfo> list = new List<IbnVersionInfo>();

			string path = Settings.UpdatesDir;
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
								case ScriptType.SqlPortal:
									names = new string[] { "1before.sql", "2scheme.sql", "metamodelCommands.xml", "3after.sql" };
									break;
								case ScriptType.Files:
									names = new string[] { "files.xml" };
									break;
								default:
									throw new IbnUpdateException("Unsupported script type.");
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

		#region static void GetFiles(List<IbnVersionInfo> files)
		static void GetFiles(List<IbnVersionInfo> files)
		{
			string path = Settings.UpdatesDir;

			foreach (string dir in Directory.GetDirectories(path))
			{
				int i = dir.LastIndexOf("_", StringComparison.OrdinalIgnoreCase);
				if (i != -1)
				{
					int version = int.Parse(dir.Substring(i + 1), CultureInfo.InvariantCulture);
					if (version > Settings.Version)
					{
						string f = dir + @"\files\";
						GetFiles(f, f.Length, version, files);
					}
				}
			}
		}
		#endregion
		#region static void GetFiles(string dir, int rootPathLen, int version, List<IbnVersionInfo> files)
		static void GetFiles(string dir, int rootPathLen, int version, List<IbnVersionInfo> files)
		{
			if (Directory.Exists(dir))
			{
				string relName;
				foreach (string file in Directory.GetFiles(dir))
				{
					relName = file.Substring(rootPathLen);
					files.Add(new IbnVersionInfo(version, 1, relName, file));
				}

				foreach (string child in Directory.GetDirectories(dir))
					GetFiles(child, rootPathLen, version, files);
			}
		}
		#endregion

		#region static void BackupFiles(List<IbnVersionInfo> files)
		static void BackupFiles(List<IbnVersionInfo> files)
		{
			LogFile.WriteLine("Saving backup files...");
			try
			{
				ProcessFiles(true, Settings.InstallDir, Settings.BackupDir, files);
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
					sourceFile = sourceRoot + relativePath;
				else
					sourceFile = vi.Data2;

				string targetFile = targetRoot + relativePath;

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
					throw new IbnUpdateException(string.Format(CultureInfo.InvariantCulture, "Cannot copy file from '{0}' to '{1}'.", sourceFile, targetFile), ex);
				}
			}
		}
		#endregion

		#region static void AddAction(UpdateAction action)
		static void AddAction(UpdateAction action)
		{
			if (!_actions.Contains(action))
				_actions.Insert(0, action);
		}
		#endregion

		#region static void AddCopiedFile(string relativePath)
		static void AddCopiedFile(string relativePath)
		{
			if (!_copiedFiles.Contains(relativePath))
				_copiedFiles.Add(relativePath);
		}
		#endregion

		#region static void LogWriteOk()
		static void LogWriteOk()
		{
			LogFile.WriteLine("OK");
		}
		#endregion
		#region static void LogWriteFailed()
		static void LogWriteFailed()
		{
			LogFile.WriteLine("Failed");
		}
		#endregion

		#region static void Pause(string message)
		static void Pause(string message)
		{
			if (Settings.PauseUpdate)
			{
				Console.WriteLine(message);
				Console.WriteLine("Press any key to continue...");
				Console.ReadKey(true);
			}
		}
		#endregion

		#region static void UpdateDatabase(string databaseName, ScriptType type, NameValueCollection replace)
		static void UpdateDatabase(string databaseName, ScriptType type, NameValueCollection replace)
		{
			string previousDatabase = DBHelper2.DBHelper.Database;
			try
			{
				Version version = new Version(IbnConst.FullVersion);

				DBHelper2.DBHelper.Database = databaseName;

				int currentVersion;
				#region Get database version
				{
					bool databaseHasVersion = false;
					int major = 0;
					int minor = 0;
					int build = 0;

					try
					{
						using (IDataReader reader = DBHelper2.DBHelper.RunTextDataReader("SELECT * FROM [DatabaseVersion]"))
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
						if (major != version.Major || minor != version.Minor || build != 47)
							throw new IbnUpdateException(string.Format(CultureInfo.InvariantCulture, "Unsupported database version: {0}.{1}.{2}. Required version is 4.7.47.", major, minor, build));

						currentVersion = build;
					}
					else
						throw new IbnUpdateException("Unknown database version. Required version is 4.7.47.");
				}
				#endregion

				List<IbnVersionInfo> scriptList = GetScripts(type, currentVersion);

				int maxScriptVersion = currentVersion;

				foreach (IbnVersionInfo vi in scriptList)
				{
					if (vi.Data.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
					{
						SqlExecuteScript(vi.Data, replace);

						if (maxScriptVersion < vi.Version)
							maxScriptVersion = vi.Version;
					}
					else if (vi.Data.EndsWith("metamodelCommands.xml", StringComparison.OrdinalIgnoreCase))
					{
						UpdateMetaModel(vi.Data);

						if (maxScriptVersion < vi.Version)
							maxScriptVersion = vi.Version;
					}
				}

				// Update database version
				NameValueCollection versionReplace = new NameValueCollection();
				versionReplace["{MajorVersion}"] = version.Major.ToString(CultureInfo.InvariantCulture);
				versionReplace["{MinorVersion}"] = version.Minor.ToString(CultureInfo.InvariantCulture);
				int maxBuild = Math.Max(version.Build, maxScriptVersion);
				versionReplace["{BuildNumber}"] = maxBuild.ToString(CultureInfo.InvariantCulture);
				SqlExecuteScript(Path.Combine(Settings.InstallDir, @"Code\_Source\Tools\version.sql"), versionReplace);

				Settings.SetMaxVersion(version.Build);
			}
			catch
			{
				DBHelper2.DBHelper.Database = previousDatabase;
				throw;
			}
		}
		#endregion

		#region static void SqlExecuteScript(string filePath, NameValueCollection replace)
		static void SqlExecuteScript(string filePath, NameValueCollection replace)
		{
			StreamReader r = null;
			LogFile.WriteMessageFormat("Executing SQL script '{0}'...", filePath);
			try
			{
				r = File.OpenText(filePath);
				DBHelper2.DBHelper.RunScript(r, replace, false);
				LogWriteOk();
			}
			catch
			{
				LogWriteFailed();
				throw;
			}
			finally
			{
				if (r != null)
					r.Close();
			}
		}
		#endregion

		#region static void UpdateMetaModel(string commandsFilePath)
		static void UpdateMetaModel(string commandsFilePath)
		{
			LogFile.WriteLine("Updating metamodel...");

			SchemaDocument schema = new SchemaDocument();
			schema.Load(Path.Combine(Settings.InstallDir, @"Code\_Source\Tools\metamodelSchema.xml"));

			SyncCommand[] commands = McXmlSerializer.GetObjectFromFile<SyncCommand[]>(commandsFilePath);

			using (DataContext dataContext = new DataContext(string.Empty))
			{
				DataContext.Current = dataContext;
				dataContext.SqlContext.CommandTimeout = CommandTimeout;
				SqlTransaction previousTransaction = DataContext.Current.SqlContext.Transaction;
				using (DBTransaction tran = DBHelper2.DBHelper.BeginTransaction())
				{
					DataContext.Current.SqlContext.Transaction = tran.SqlTran;
					try
					{
						MetaModelSync.Execute(schema, commands);
						tran.Commit();
						LogWriteOk();
					}
					finally
					{
						DataContext.Current.SqlContext.Transaction = previousTransaction;
					}
				}
			}
		}
		#endregion

		#region static void SaveCompanyParameter(DBHelper dbHelper, string name, string value)
		static void SaveCompanyParameter(DBHelper dbHelper, string name, string value)
		{
			if (value == null)
			{
				dbHelper.RunText("DELETE FROM [PortalConfig] WHERE [Key]=@Key"
					, DBHelper.MP("@Key", SqlDbType.NVarChar, 100, name)
					);
			}
			else
			{
				dbHelper.RunText("IF EXISTS (SELECT 1 FROM [PortalConfig] WHERE [Key]=@Key) UPDATE [PortalConfig] SET [Value]=@Value WHERE [Key]=@Key ELSE INSERT INTO [PortalConfig] ([Key],[Value]) VALUES (@Key,@Value)"
					, DBHelper.MP("@Key", SqlDbType.NVarChar, 100, name)
					, DBHelper.MP("@Value", SqlDbType.NText, value)
					);
			}
		}
		#endregion

		#region static void StartNewServices()
		static void StartNewServices()
		{
			StartService("McOleDBService" + IbnConst.VersionMajorMinor);
			StartService("ScheduleService" + IbnConst.VersionMajorMinor);
		}
		#endregion

		#region static void StartOldServices()
		static void StartOldServices()
		{
			foreach (string name in _services)
				StartService(name);
		}
		#endregion

		#region static void StartService(string name)
		static void StartService(string name)
		{
			LogFile.WriteMessageFormat("Starting service '{0}'...", name);
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

		#region static void StopService(string name, bool mustExist)
		static void StopService(string name, bool mustExist)
		{
			LogFile.WriteMessageFormat("Stopping service '{0}'...", name);
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

		#region static void RecoverFiles()
		static void RecoverFiles()
		{
			LogFile.WriteLine("Rolling back files...");
			try
			{
				string sourceRoot = Settings.BackupDir;
				string targetRoot = Settings.InstallDir;

				foreach (string relFile in _copiedFiles)
				{
					string sourceFile = sourceRoot + relFile;
					string targetFile = targetRoot + relFile;

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
	}
}
