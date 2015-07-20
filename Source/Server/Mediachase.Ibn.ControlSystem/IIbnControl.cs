using System;
using System.Collections.Specialized;

namespace Mediachase.Ibn.ControlSystem
{
	/// <summary>
	/// Summary description for IIbnControl.
	/// </summary>
	public interface IIbnControl
	{
		void Init(IIbnContainer owner, IbnControlInfo controlInfo);

		string Name{get;}
		string[] Actions{get;}
		NameValueCollection Parameters{get;}
		IIbnContainer OwnerContainer{get;}

		string[] GetBaseActions(string Action);
		string[] GetDerivedActions(string Action);
	}
}
