using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.SessionState;

namespace Mediachase.Ibn.WebAsp
{
	/// <summary>
	/// Summary description for Global.
	/// </summary>
	public class Global : System.Web.HttpApplication
	{
		private static string Product;

		public Global()
		{
			InitializeComponent();
		}	
		
		protected void Application_Start(Object sender, EventArgs e)
		{
			AssemblyProductAttribute attribute = ( AssemblyProductAttribute )Attribute.GetCustomAttribute( Assembly.GetAssembly( typeof( PageBase ) ), typeof( AssemblyProductAttribute ), true );
			Product = attribute.Product;

//			if ( ! EventLog.SourceExists( Product ) )
//				EventLog.CreateEventSource( Product, "Application" );

		}
 
		protected void Session_Start(Object sender, EventArgs e)
		{

		}

		protected void Application_BeginRequest(Object sender, EventArgs e)
		{
			// Set preferred culture
			string[] userLanguages = Request.UserLanguages;
			if (userLanguages != null && userLanguages.Length > 0 && userLanguages[0] != null)
			{
				try
				{
					CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture(userLanguages[0]);
					Thread.CurrentThread.CurrentCulture = cultureInfo;
					Thread.CurrentThread.CurrentUICulture = cultureInfo;
				}
				catch (ArgumentException)
				{
				}
			}
		}

		protected void Application_EndRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(Object sender, EventArgs e)
		{

		}


		private void GenerateErrorReport(Exception ex,string errorid)
		{
			TextWriter tw = File.CreateText(Server.MapPath("~/Log/Error/") + errorid + ".aspx");
			
			string postdata = "<table border='0' cellpadding='0' cellspasing='0'>";
			foreach (String key in Request.Form.Keys)
			{
				postdata += "<tr><td width='120px' valign='top'>" + key + "</td>";
				string values = "";
				foreach (string val in Request.Form.GetValues(key))
				{
					string _val ="";
					if (key == "__VIEWSTATE") 
						_val = val.Substring(0,20)+"...";
					else 
						if (val.Length>100) 
						_val = val.Substring(0,100)+"...";
					else 
						_val = val;

					values += _val+"<br>";
				}

				postdata += "<td>" + values + "</td></tr>";
			}
			postdata +="</table>";

			string stacktrace = ex.StackTrace.Replace("\r\n","<br>");

			tw.Write(
				"<html><head><title>Error report</title> <meta http-equiv='Content-Type' content='text/html;'></head>"+
				"</head><body><p><font face='Verdana, Arial, Helvetica, sans-serif'><strong>Error report " + errorid + "</strong></font></p>"+
				"<table width='100%' border='0' cellspacing='0' cellpadding='3'>"+
				"<tr>"+
				"<td width='123' valign='top'><font color='#FF0000' size='-1' face='Verdana, Arial, Helvetica, sans-serif'><strong>Error ID:</strong></font></td>" +
				"<td><font size='-1' face='Verdana, Arial, Helvetica, sans-serif'>"+errorid+"&nbsp;&nbsp;&nbsp;"+DateTime.Now.ToString()+"</td>" +
				"</tr>" +
				"<tr>" +
				"<td width='123' valign='top'><strong><font color='#009900' size='-1' face='Verdana, Arial, Helvetica, sans-serif'>Query:</font></strong></td>" +
				"<td><font size='-1' face='Verdana, Arial, Helvetica, sans-serif'>" + Request.HttpMethod +" " + Request.RawUrl + "</font></td>" +
				"</tr>" +
				"<tr>" +
				"<td width='125' valign='top'><strong><font color='#009900' size='-1' face='Verdana, Arial, Helvetica, sans-serif'>Postback:</font></strong></td>" +
				"<td><font size='-1' face='Verdana, Arial, Helvetica, sans-serif'>"+postdata+"</font></td>" +
				"</tr>" +
				"<tr>" +
				"<tr>" +
				"<td width='123' valign='top'><strong><font color='#0000CC' size='-1' face='Verdana, Arial, Helvetica, sans-serif'>Message:</font></strong></td>"+
				"<td><font size='-1' face='Verdana, Arial, Helvetica, sans-serif'>"+ ex.Message+"</font></td>"+
				"</tr>"+
				"<tr>" +
				"<td width='123' valign='top'><strong><font color='#0000CC' size='-1' face='Verdana, Arial, Helvetica, sans-serif'>Source:</font></strong></td>"+
				"<td><font size='-1' face='Verdana, Arial, Helvetica, sans-serif'>"+ ex.Source+"</font></td>"+
				"</tr>"+
				"<tr>" +
				"<td width='123' valign='top'><strong><font color='#0000CC' size='-1' face='Verdana, Arial, Helvetica, sans-serif'>Stack Trace:</font></strong></td>" +
				"<td><font size='-1' face='Verdana, Arial, Helvetica, sans-serif'>"+ stacktrace +"</font></td>" +
				"</tr>" +
				"<tr>" +
				"<td width='123' valign='top'><strong><font color='#0000CC' size='-1' face='Verdana, Arial, Helvetica, sans-serif'>Inner Exception:</font></strong></td>" +
				"<td><font size='-1' face='Verdana, Arial, Helvetica, sans-serif'>"+ ex.InnerException+"</font></td>" +
				"</tr>" +
				"<tr>" +
				"<td width='123' valign='top'><strong><font color='#0000CC' size='-1' face='Verdana, Arial, Helvetica, sans-serif'>Target Site:</font></strong></td>" +
				"<td><font size='-1' face='Verdana, Arial, Helvetica, sans-serif'>"+ ex.TargetSite+"</font></td>" +
				"</tr></table></body></html>"
				);
			tw.Close();
		}


		protected void Application_Error(Object sender, EventArgs e)
		{

			Exception ex;
			string errorid = Guid.NewGuid().ToString().Substring(0,6);
			if (Server.GetLastError().GetBaseException()!=null)
				ex = Server.GetLastError().GetBaseException();
			else
				ex = Server.GetLastError();

			GenerateErrorReport(ex,errorid);
			
			
			switch (ex.GetType().ToString())
			{
	
				case "System.NullReferenceException":
					Response.Write("<div style='color:red'><b>Warning " + errorid +"<b></div><br>");
					Response.Write("The requested item does not exist.<br>");
					Response.Write("<a href ='../default.aspx'>Jump to Workspace</a>");
					Response.End();
					break;
				default:
					Response.Write("<div style='color:red'><b>Error " + errorid +"<b></div><br>");
					Response.Write("Requested page is not accessible. Please send information about this error to your system administrator.<br>");
					Response.Write("<a href ='../default.aspx'>home</a>");
					Response.End();
					break;
			}
			#if (!DEBUG)
				Server.ClearError();
			#endif
		}

		protected void Session_End(Object sender, EventArgs e)
		{

		}

		protected void Application_End(Object sender, EventArgs e)
		{
			//IBNDatabaseManager.UnInit();
		}

			
		#region Web Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
		}
		#endregion
	}
}

