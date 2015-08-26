// User.h : Declaration of the CUser

#ifndef __USER_H_
#define __USER_H_

//#include "session.h"
//#include "ATL_NetLib.h"
//#include "Structs.h"
#include "resource.h"       // main symbols
/////////////////////////////////////////////////////////////////////////////
// CUser
class ATL_NO_VTABLE CUser : 
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<CUser>,
	public IDispatchImpl<IUser, &IID_IUser, &LIBID_ATL_NETLIBLib>
{
public:
	CUser()
	{
/*
		WasSetEMail		= FALSE;
		WasSetLastName	= FALSE;
		WasSetFirstName	= FALSE;
		WasSetUserName	= FALSE;
		WasSetStatus	= FALSE;
		WasSetID		= FALSE;*/

	}
	HRESULT CUser::FinalRelease()
	{
		try
		{
		//MCTRACE(1,m_sUser.m_UserName);
		}
		catch(...)
		{
			ATLASSERT(FALSE);
		}
		return S_OK;
	}

DECLARE_REGISTRY_RESOURCEID(IDR_USER)

DECLARE_PROTECT_FINAL_CONSTRUCT()

BEGIN_COM_MAP(CUser)
	COM_INTERFACE_ENTRY(IUser)
	COM_INTERFACE_ENTRY(IDispatch)
END_COM_MAP()

// IUser
public:
	STDMETHOD(get_Value)(/*[in]*/ BSTR bsName, /*[out, retval]*/ VARIANT *pVal);
	STDMETHOD(put_Value)(/*[in]*/ BSTR bsName, /*[in]*/ VARIANT newVal);
		sUser m_sUser;
private:

/*
	bool WasSetEMail;
	bool WasSetLastName;
	bool WasSetFirstName;
	bool WasSetUserName;
	bool WasSetStatus;
	bool WasSetID;*/

};

#endif //__USER_H_
