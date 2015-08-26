// File.h : Declaration of the CFile

#ifndef __FILE_H_
#define __FILE_H_

#include "resource.h"       // main symbols
#include "users.h"

class CSession;
/////////////////////////////////////////////////////////////////////////////
// CFile
class ATL_NO_VTABLE CFile : 
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<CFile>,//, &CLSID_File>,
	public IDispatchImpl<IFile, &IID_IFile, &LIBID_ATL_NETLIBLib>
{
public:
	CFile()
	{
	}

	HRESULT CFile::FinalConstruct()
	{
	   return CUsers::CreateInstance(&m_pRecipients); 
	}
DECLARE_REGISTRY_RESOURCEID(IDR_FILE)
DECLARE_NOT_AGGREGATABLE(CFile)

DECLARE_PROTECT_FINAL_CONSTRUCT()

BEGIN_COM_MAP(CFile)
	COM_INTERFACE_ENTRY(IFile)
	COM_INTERFACE_ENTRY(IDispatch)
END_COM_MAP()

// IFile
public:
	STDMETHOD(get_Size)(/*[out, retval]*/ long *pVal);
	STDMETHOD(put_hWnd)(/*[in]*/ long newVal);
	STDMETHOD(get_SID)(/*[out, retval]*/ BSTR *pVal);
	STDMETHOD(Receive)(/*[out]*/ long* Handle);
	STDMETHOD(Send)(/*[out]*/ long* Handle);
	STDMETHOD(get_Recipients)(/*[out, retval]*/ IUsers* *pVal);
	STDMETHOD(get_Sender)(/*[out, retval]*/ IUser* *pVal);
	STDMETHOD(get_date_time)(/*[out, retval]*/ long *pVal);
	STDMETHOD(get_Body)(/*[out, retval]*/ BSTR *pVal);
	STDMETHOD(put_Body)(/*[in]*/ BSTR newVal);
	STDMETHOD(get_RealName)(/*[out, retval]*/ BSTR *pVal);
	STDMETHOD(put_RealName)(/*[in]*/ BSTR newVal);
	STDMETHOD(get_FID)(/*[out, retval]*/ BSTR *pVal);
	STDMETHOD(put_FID)(/*[in]*/ BSTR newVal);
	CSession*			m_pSession;
	sFile				m_sFile;
	CComPtr<IUsers>		m_pRecipients;
};

#endif //__FILE_H_
