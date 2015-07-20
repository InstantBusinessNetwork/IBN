using System;

namespace Mediachase.IBN.Business.Pop3
{
	/// <summary>
	/// Summary description for IPersistPop3MessageHandlerControl.
	/// </summary>
	public interface IPersistPop3MessageHandlerStorage
	{
		void Load(Pop3Box box);
		void Save(Pop3Box box);
	}
}
