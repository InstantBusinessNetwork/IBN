// IChats.h : Declaration of the CIChats

#ifndef __ICHATS_H_
#define __ICHATS_H_

#include "resource.h"       // main symbols

#include "resource.h"       // main symbols
#include "collections.h"
#include "ichat.h"
#pragma warning(disable: 4530)
typedef CComEnumOnSTL<IEnumVARIANT, &IID_IEnumVARIANT, VARIANT,
                        _CopyVariantFromAdaptItf<IChat>,
                        list< CAdapt< CComPtr<IChat> > > >
        CComEnumVariantOnListOfChats;

typedef ICollectionOnSTLImpl<IDispatchImpl<IChats, &IID_IChats>,
                             list< CAdapt< CComPtr<IChat> > >,
                             IChat*,
                             _CopyItfFromAdaptItf<IChat>,
                             CComEnumVariantOnListOfChats>
        IChatsCollImpl;
#pragma warning(default: 4530)
/////////////////////////////////////////////////////////////////////////////
// CFiles

class ATL_NO_VTABLE CChats : 
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<CChats>,
	public IChatsCollImpl
{
public:
	CChats()
	{
	}

DECLARE_REGISTRY_RESOURCEID(IDR_ICHATS)
DECLARE_NOT_AGGREGATABLE(CChats)

DECLARE_PROTECT_FINAL_CONSTRUCT()

BEGIN_COM_MAP(CChats)
	COM_INTERFACE_ENTRY(IChats)
	COM_INTERFACE_ENTRY(IDispatch)
END_COM_MAP()

	STDMETHODIMP AddChat(IChat** ppChat) 
	{ 
		HRESULT hr = CChat::CreateInstance(ppChat);
		if( SUCCEEDED(hr) ) 
		{
			// Put the document on the list
			CComPtr<IChat>  spChat = *ppChat;
			m_coll.push_back(spChat);
		}
	  return hr;
	}
// IIChats
public:
};

#endif //__ICHATS_H_
