using System;
using System.Reflection;
using System.Collections;
using Mediachase.IBN.Database.EMail;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Summary description for IncidentBoxRuleFunction.
	/// </summary>
	public class IncidentBoxRuleFunction
	{
		IncidentBoxRuleFunctionRow _srcRow = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="IncidentBoxRuleFunction"/> class.
		/// </summary>
		/// <param name="row">The row.</param>
		private IncidentBoxRuleFunction(IncidentBoxRuleFunctionRow row)
		{
			_srcRow = row;
		}

		/// <summary>
		/// Lists this instance.
		/// </summary>
		/// <returns></returns>
		public static IncidentBoxRuleFunction[] List()
		{
			ArrayList retVal = new ArrayList();

			foreach(IncidentBoxRuleFunctionRow row in IncidentBoxRuleFunctionRow.List())
			{
				retVal.Add(new IncidentBoxRuleFunction(row));
			}

			return (IncidentBoxRuleFunction[])retVal.ToArray(typeof(IncidentBoxRuleFunction));
		}

		/// <summary>
		/// Loads the specified incident box rule function id.
		/// </summary>
		/// <param name="IncidentBoxRuleFunctionId">The incident box rule function id.</param>
		/// <returns></returns>
		public static IncidentBoxRuleFunction Load(int IncidentBoxRuleFunctionId)
		{
			return new IncidentBoxRuleFunction(new IncidentBoxRuleFunctionRow(IncidentBoxRuleFunctionId));
		}

		/// <summary>
		/// Invokes the specified values.
		/// </summary>
		/// <param name="values">The values.</param>
		/// <returns></returns>
		public bool Invoke(params string[] values)
		{
			string[] typeInformation = this.Type.Split(',');

			if( typeInformation.Length < 2 )
				throw new ArgumentException("Unable to parse Type name.  Use 'type, assembly'");

			Assembly asm = Assembly.Load(typeInformation[1].Trim());
			if(asm == null)
				throw new ArgumentException("Unable to load assembly " + typeInformation[1]);

			Type networkServerType = asm.GetType(typeInformation[0].Trim());
			if( networkServerType == null)
				throw new ArgumentException("Unable to load type " + typeInformation[0]);

			MethodInfo methodInfo = networkServerType.GetMethod(this.MethodName);

			object newObject = Activator.CreateInstance(networkServerType);

			return (bool)methodInfo.Invoke(newObject, values);
		}

		#region Public Properties
		
		/// <summary>
		/// Gets the incident box rule function id.
		/// </summary>
		/// <value>The incident box rule function id.</value>
		public virtual int IncidentBoxRuleFunctionId
	    
		{
			get
			{
				return _srcRow.IncidentBoxRuleFunctionId;
			}
			
		}
		
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public virtual string Name
	    
		{
			get
			{
				return _srcRow.Name;
			}
			set
			{
				_srcRow.Name = value;
			}	
			
		}
		
		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		public virtual string Description
	    
		{
			get
			{
				return _srcRow.Description;
			}
			
			set
			{
				_srcRow.Description = value;
			}	
			
		}
		
		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		public virtual string Type
	    
		{
			get
			{
				return _srcRow.Type;
			}
			
			set
			{
				_srcRow.Type = value;
			}	
			
		}
		
		/// <summary>
		/// Gets or sets the name of the method.
		/// </summary>
		/// <value>The name of the method.</value>
		public virtual string MethodName
	    
		{
			get
			{
				return _srcRow.MethodName;
			}
			
			set
			{
				_srcRow.MethodName = value;
			}	
			
		}

		/// <summary>
		/// Gets the parameters information.
		/// </summary>
		/// <returns></returns>
		public ParameterInfo[] GetParameters()
		{
			string[] typeInformation = this.Type.Split(',');

			if( typeInformation.Length < 2 )
				throw new ArgumentException("Unable to parse Type name.  Use 'type, assembly'");

			Assembly asm = Assembly.Load(typeInformation[1].Trim());
			if(asm == null)
				throw new ArgumentException("Unable to load assembly " + typeInformation[1]);

			Type networkServerType = asm.GetType(typeInformation[0].Trim());
			if( networkServerType == null)
				throw new ArgumentException("Unable to load type " + typeInformation[0]);

			MethodInfo methodInfo = networkServerType.GetMethod(this.MethodName);

			return methodInfo.GetParameters();
		}
		
		#endregion
	}
}
