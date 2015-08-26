// SID.h : Declaration of the CSID

#ifndef __SID_H_
#define __SID_H_

#include "resource.h"       // main symbols

/////////////////////////////////////////////////////////////////////////////
// CSID
class ATL_NO_VTABLE ClocalSID : 
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<ClocalSID>,
	public IDispatchImpl<IlocalSID, &IID_IlocalSID, &LIBID_ATL_NETLIBLib>
{
public:
	ClocalSID()
	{
	}

DECLARE_REGISTRY_RESOURCEID(IDR_SID)
DECLARE_NOT_AGGREGATABLE(ClocalSID)

DECLARE_PROTECT_FINAL_CONSTRUCT()

BEGIN_COM_MAP(ClocalSID)
	COM_INTERFACE_ENTRY(IlocalSID)
	COM_INTERFACE_ENTRY(IDispatch)
END_COM_MAP()

// ISID
public:
	STDMETHOD(get_SID)(/*[out, retval]*/ BSTR *pVal);
	STDMETHOD(get_Count)(/*[out, retval]*/ long *pVal);
	slocalSID m_slocalSID;
};

#endif //__SID_H_
