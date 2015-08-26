// ConvertSessionWrapper.h : Declaration of the CConvertSessionWrapper

#pragma once
#include "resource.h"       // main symbols

#include "McConverterSessionServer.h"


#if defined(_WIN32_WCE) && !defined(_CE_DCOM) && !defined(_CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA)
#error "Single-threaded COM objects are not properly supported on Windows CE platform, such as the Windows Mobile platforms that do not include full DCOM support. Define _CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA to force ATL to support creating single-thread COM object's and allow use of it's single-threaded COM object implementations. The threading model in your rgs file was set to 'Free' as that is the only threading model supported in non DCOM Windows CE platforms."
#endif



// CConvertSessionWrapper

class ATL_NO_VTABLE CConvertSessionWrapper :
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<CConvertSessionWrapper, &CLSID_ConvertSessionWrapper>,
	public IDispatchImpl<IConvertSessionWrapper, &IID_IConvertSessionWrapper, &LIBID_McConverterSessionServerLib, /*wMajor =*/ 1, /*wMinor =*/ 0>
{
public:
	CConvertSessionWrapper()
	{
	}

DECLARE_REGISTRY_RESOURCEID(IDR_CONVERTSESSIONWRAPPER)


BEGIN_COM_MAP(CConvertSessionWrapper)
	COM_INTERFACE_ENTRY(IConvertSessionWrapper)
	COM_INTERFACE_ENTRY(IDispatch)
END_COM_MAP()



	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
		return S_OK;
	}

	void FinalRelease()
	{
	}

public:

	STDMETHOD(SaveMapiMessage)(BSTR bstrFileName, VARIANT pUnkMessage, long ulEncType);
	STDMETHOD(SaveMapiMessageTmpFile)(VARIANT pUnkMessage, long ulEncType, BSTR* pTmpFileName);
private:

	HRESULT Initialize();

	LPUNKNOWN GetMessageUnk(LPDISPATCH pDisp);

	CComBSTR GetTmpFileName();

	CComBSTR GetTemporaryPath();
};

OBJECT_ENTRY_AUTO(__uuidof(ConvertSessionWrapper), CConvertSessionWrapper)
