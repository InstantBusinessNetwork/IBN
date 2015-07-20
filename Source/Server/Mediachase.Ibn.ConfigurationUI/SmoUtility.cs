using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
using Microsoft.Win32;
using System.Diagnostics;

namespace Mediachase.Ibn.ConfigurationUI
{
	/// <summary>
	/// Provides methods for execute SQL Server Management Objects (SMO) methods. 
	/// </summary>
	public static class SmoUtility
	{
		public static void GetServers(ref ComboBox cmbServers)
		{
			throw new NotImplementedException();

/*			cmbServers.Items.Clear();

			// SMO Enum Servers
			try
			{
				DataTable dt = SmoApplication.EnumAvailableSqlServers(false);
				if (dt.Rows.Count > 0)
				{
					// Load server names into combo box
					foreach (DataRow dr in dt.Rows)
					{
						//only add if it doesn't exist
						if (cmbServers.FindStringExact((String)dr["Name"]) == -1)
							cmbServers.Items.Add(dr["Name"]);
					}
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex, "SmoUtility");
			}


			//Registry for local
			try
			{
				RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server");
				String[] instances = (String[])rk.GetValue("InstalledInstances");
				if (instances.Length > 0)
				{
					foreach (String element in instances)
					{
						String name = "";
						//only add if it doesn't exist
						if (element == "MSSQLSERVER")
							name = System.Environment.MachineName;
						else
							name = System.Environment.MachineName + @"\" + element;

						if (cmbServers.FindStringExact(name) == -1)
							cmbServers.Items.Add(name);
					}
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex, "SmoUtility");
			}

			try
			{
				// Registered Servers
				RegisteredServer[] rsvrs = SmoApplication.SqlServerRegistrations.EnumRegisteredServers();

				foreach (RegisteredServer rs in rsvrs)
				{
					String name = "";

					name = rs.ServerInstance.Replace(".", System.Environment.MachineName)
											.Replace("(local)", System.Environment.MachineName)
											.Replace("localhost", System.Environment.MachineName);
					//only add if it doesn't exist
					if (cmbServers.FindStringExact(name) == -1 && name.Length > 0)
						cmbServers.Items.Add(name);
				}

				// Default to default instance on this machine 
				cmbServers.SelectedIndex = cmbServers.FindStringExact(System.Environment.MachineName);

				// If this machine is not a SQL server 
				// then select the first server in the list
				if (cmbServers.SelectedIndex < 0)
				{
					cmbServers.SelectedIndex = 0;
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex, "SmoUtility");
			}
 */
		}

	}
}
