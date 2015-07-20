//===============================================================================
// Microsoft Asynchronous Invocation Application Block for .NET
// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnpag/html/PAIBlock.asp
//
// AsyncResources.cs
// This class is used to access the constants defined in the
// resource file, AsyncResourcesText.resx.
//
//===============================================================================
// Copyright (C) 2003 Microsoft Corporation
// All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================


using System;
using System.Reflection;
using System.Resources;
using System.Text;

namespace Mediachase.MetaDataPlus.Common
{
	/// <summary>
	///	This class is used to access the constants defined in the resource 
	///	file, AsyncResourcesText.resx.	
	/// </summary>
	internal sealed class AsyncResources
	{
		#region Private member variables

		private static ResourceManager asyncResource 
			= InitializeResourceManager();

		#endregion

		#region Private Constructor

		/// <summary>
		///	Private Constructor prevents class from getting created.
		/// </summary>
		private AsyncResources ()
		{}			

		#endregion
		
		#region Public Methods

		/// <summary>
		///	This method returns the value of constant from the resources file 
		///	for the constant variable passed as input to the method.
		/// </summary>
		/// <remarks>
		///	string constantValue = GetConstantValue(constantName);
		/// </remarks>
		/// <param name="constantName">
		///	Constant name for which the value has to be retrieved from the 
		///	Resource file
		/// </param>
		public static string GetConstantValue(string constantName)
		{

			#region Throwing Input Argument Exception		
		
			if(Object.Equals(constantName, null))
			{
				throw(new ArgumentNullException( "constantName",
					asyncResource.GetString(
					"CONSTANT_NAME_NULL_ERR_MSG")) );
			}
			if(constantName.Length == 0)
			{
				throw(new ArgumentOutOfRangeException( "constantName",
					asyncResource.GetString(
					"CONSTANT_NAME_RANGE_ERR_MSG")) );
			}
		
			#endregion

			return asyncResource.GetString(constantName);
		}
		
		#endregion
	
		#region Private Methods

		/// <summary>
		///	This method is used to initialize the resource manager instance 
		///	variable.
		/// </summary>
		/// <remarks>
		///	ResourceManager resManager = InitializeResourceManager();
		/// </remarks>
		/// <returns>
		///	Instance of ResourceManager class
		/// </returns>
		private static ResourceManager InitializeResourceManager()
		{
			try
			{
				#region Constants

				const char DELIMITER = ',';

				#endregion

				#region Declaring and Initializing Local Variables

				ResourceManager asyncResource;
				StringBuilder resourceNameSpace = new StringBuilder();
				char [] separator = new char [] {DELIMITER};
				string assemblyFullName;
				string assemblyName;

				#endregion

				#region Getting Root Assembly Name

				assemblyFullName = 
					typeof(AsyncResources).Assembly.FullName;
				assemblyName = assemblyFullName.Split
					(separator).GetValue(0).ToString();

				resourceNameSpace.Append(assemblyName);
				resourceNameSpace.Append(".AsyncResourcesText");

				#endregion

				#region Creating the Resource Manager

				asyncResource = new ResourceManager
					( resourceNameSpace.ToString(), 
					Assembly.GetAssembly(typeof(AsyncResources)) );	

				#endregion
				
				return asyncResource;
			}
			catch(Exception genException)
			{
				System.Diagnostics.Trace.WriteLine(genException,"Mediachase.MetaDataPlus.Common.AsyncResources");
				throw;
			}
		}		
		#endregion
	}
}