using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Diagnostics;
using System.Globalization;


namespace Mediachase.IBN.Business
{
	public class JsDebugHandler : IHttpModule
	{
		private bool isEnabled = false;

		#region IHttpModule Members

		public void Dispose()
		{
			
		}

		public void Init(HttpApplication context)
		{
			context.BeginRequest += new EventHandler(context_BeginRequest);
			context.EndRequest += new EventHandler(context_EndRequest);

			isEnabled = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogSriptLoading"], CultureInfo.InvariantCulture);
		}

		#endregion

		#region context_BeginRequest
		/// <summary>
		/// Handles the BeginRequest event of the context control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void context_BeginRequest(object sender, EventArgs e)
		{
			if (isEnabled)
			{
				HttpApplication application = (HttpApplication)sender;
				HttpContext context = application.Context;

				if ((context.Request.Url.AbsolutePath.Contains(".js") || context.Request.Url.AbsolutePath.Contains(".axd")) && !context.Request.Url.AbsolutePath.Contains(".aspx"))
				{
					context.Response.Write(String.Format("\r\n if (typeof(JsScriptDebugger) != 'undefined') {{ JsScriptDebugger('start {0}', '{1}'); }}", context.Request.FilePath, GetAbsolutePath("/Modules/XScriptDebug.aspx")));
				}
			}
		} 
		#endregion

		#region context_EndRequest
		/// <summary>
		/// Handles the EndRequest event of the context control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void context_EndRequest(object sender, EventArgs e)
		{
			if (isEnabled)
			{
				HttpApplication application = (HttpApplication)sender;
				HttpContext context = application.Context;

				if ((context.Request.Url.AbsolutePath.Contains(".js") || context.Request.Url.AbsolutePath.Contains(".axd")) && !context.Request.Url.AbsolutePath.Contains(".aspx"))
				{
					//context.Response.Write(String.Format("if (typeof(JsScriptDebugger) != 'undefined') {{ JsScriptDebugger('finish {0}', '{1}'); }}\r\n", context.Request.FilePath, GetAbsolutePath("/Modules/XScriptDebug.aspx")));
					//context.Response.Write("window.status = 'end';");
				}
			}
		} 
		#endregion

		public string GetAbsolutePath(string xs_Path)
		{
			string UrlScheme = System.Configuration.ConfigurationManager.AppSettings["UrlScheme"];

			StringBuilder builder = new StringBuilder();
			if (UrlScheme != null)
				builder.Append(UrlScheme);
			else
				builder.Append(HttpContext.Current.Request.Url.Scheme);
			builder.Append("://");

			// Oleg Rylin: Fixing the problem with non-default port [6/20/2006]
			builder.Append(HttpContext.Current.Request.Url.Authority);

			builder.Append(HttpContext.Current.Request.ApplicationPath);
			builder.Append("/");
			if (xs_Path != string.Empty)
			{
				if (xs_Path[0] == '/')
					xs_Path = xs_Path.Substring(1, xs_Path.Length - 1);
				builder.Append(xs_Path);
			}
			return builder.ToString();
		}

	}
}
