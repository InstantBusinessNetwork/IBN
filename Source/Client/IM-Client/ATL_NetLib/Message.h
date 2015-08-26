// Message.h : Declaration of the CMessage

#ifndef __MESSAGE_H_
#define __MESSAGE_H_

#include "resource.h"       // main symbols
#include "Users.h"       // main symbols
class CSession;
/////////////////////////////////////////////////////////////////////////////
// CMessage
class ATL_NO_VTABLE CMessage : 
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<CMessage>,
	public IDispatchImpl<IMessage, &IID_IMessage, &LIBID_ATL_NETLIBLib>
{
public:
DECLARE_REGISTRY_RESOURCEID(IDR_MESSAGE)
DECLARE_PROTECT_FINAL_CONSTRUCT()

	CMessage()
	{

	};

	HRESULT CMessage::FinalConstruct()
	{
	   return CUsers::CreateInstance(&m_pRecipients); 
	}
	
BEGIN_COM_MAP(CMessage)
	COM_INTERFACE_ENTRY(IMessage)
	COM_INTERFACE_ENTRY(IDispatch)
END_COM_MAP()

// IMessage
public:
	STDMETHOD(get_ID)(/*[out, retval]*/ long *pVal);
	STDMETHOD(get_SID)(/*[out, retval]*/ BSTR *pVal);
	STDMETHOD(Send)(/*[out]*/ long *Handle);
	STDMETHOD(get_MID)(/*[out, retval]*/ BSTR *pVal);
	STDMETHOD(put_MID)(/*[in]*/ BSTR newVal);
	STDMETHOD(get_Recipients)(/*[out, retval]*/ IUsers* *pVal);
	STDMETHOD(get_Sender)(/*[out, retval]*/ IUser* *pVal);
	STDMETHOD(get_date_time)(/*[out, retval]*/ long *pVal);
	STDMETHOD(get_Body)(/*[out, retval]*/ BSTR *pVal);
	STDMETHOD(put_Body)(/*[in]*/ BSTR newVal);
	CSession*		m_pSession;
	sMessage		m_sMessage;
	CComBSTR		m_bsCID;
	CComPtr<IUsers>	m_pRecipients;
};

#endif //__MESSAGE_H_
