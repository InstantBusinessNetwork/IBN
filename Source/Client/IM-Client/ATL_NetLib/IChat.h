// IChat.h : Declaration of the CIChat

#ifndef __ICHAT_H_
#define __ICHAT_H_

#include "resource.h"       // main symbols

class CSession;
/////////////////////////////////////////////////////////////////////////////
// CIChat
class ATL_NO_VTABLE CChat : 
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<CChat>,
	public IDispatchImpl<IChat, &IID_IChat, &LIBID_ATL_NETLIBLib>
{
public:
	CChat()
	{
	}

DECLARE_REGISTRY_RESOURCEID(IDR_ICHAT)
DECLARE_NOT_AGGREGATABLE(CChat)

DECLARE_PROTECT_FINAL_CONSTRUCT()

BEGIN_COM_MAP(CChat)
	COM_INTERFACE_ENTRY(IChat)
	COM_INTERFACE_ENTRY(IDispatch)
END_COM_MAP()

// IIChat
public:
	STDMETHOD(get_Value)(/*[in]*/ BSTR bsName, /*[out, retval]*/ VARIANT *pVal);
	STDMETHOD(Edit)(/*[in]*/ BSTR Name, /*[in]*/ BSTR Descr, /*[out, retval]*/ long* Handle);
	STDMETHOD(Accept)(/*[in]*/ long Result, /*[out, retval]*/ long* Handle);
	STDMETHOD(Leave)(/*[in]*/ long * Handle);
	STDMETHOD(Invite)(/*[in]*/ BSTR Invitation,/*[out, retval]*/ long * Handle);
	STDMETHOD(AddUser)(/*[IN]*/ long UserID);
	STDMETHOD(CreateMessage)(/*[OUT]*/ IMessage** pMessage);
	STDMETHOD(SetStatus)(/*[IN]*/ long Status, /*[IN]*/ long Param, /*[OUT]*/ long* Handle);

	CSession*			m_pSession;
	sChat				m_sChat;
};

#endif //__ICHAT_H_
