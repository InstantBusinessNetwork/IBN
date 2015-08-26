// Promo.h : Declaration of the CPromo

#ifndef __PROMO_H_
#define __PROMO_H_

#include "resource.h"       // main symbols
#include "Users.h"

class CSession;
/////////////////////////////////////////////////////////////////////////////
// CPromo
class ATL_NO_VTABLE CPromo : 
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<CPromo>,
	public IDispatchImpl<IPromo, &IID_IPromo, &LIBID_ATL_NETLIBLib>
{
public:
	CPromo()
	{
	}

	HRESULT CPromo::FinalConstruct()
	{
	   return CUsers::CreateInstance(&m_pRecipients); 
	}

DECLARE_REGISTRY_RESOURCEID(IDR_PROMO)

DECLARE_PROTECT_FINAL_CONSTRUCT()

BEGIN_COM_MAP(CPromo)
	COM_INTERFACE_ENTRY(IPromo)
	COM_INTERFACE_ENTRY(IDispatch)
END_COM_MAP()

// IPromo
public:
	STDMETHOD(Send)(/*[out, retval]*/ long* Handle);
	STDMETHOD(get_ProductName)(/*[out, retval]*/ BSTR *pVal);
	STDMETHOD(put_ProductName)(/*[in]*/ BSTR newVal);
	STDMETHOD(get_Sender)(/*[out, retval]*/ IUser* *pVal);
	STDMETHOD(get_Recipients)(/*[out, retval]*/ IUsers* *pVal);
	STDMETHOD(get_Body)(/*[out, retval]*/ BSTR *pVal);
	STDMETHOD(put_Body)(/*[in]*/ BSTR newVal);
	STDMETHOD(get_Product_ID)(/*[out, retval]*/ long *pVal);
	STDMETHOD(put_Product_ID)(/*[in]*/ long newVal);
	STDMETHOD(get_Subject)(/*[out, retval]*/ BSTR *pVal);
	STDMETHOD(put_Subject)(/*[in]*/ BSTR newVal);
	STDMETHOD(get_SID)(/*[out, retval]*/ BSTR *pVal);
	STDMETHOD(get_date_time)(/*[out, retval]*/ long *pVal);
	STDMETHOD(get_PID)(/*[out, retval]*/ BSTR *pVal);
	STDMETHOD(put_PID)(/*[in]*/ BSTR newVal);

	sPromo				m_sPromo;
	CComPtr<IUsers>		m_pRecipients;
	CSession*			m_pSession;
};

#endif //__PROMO_H_
