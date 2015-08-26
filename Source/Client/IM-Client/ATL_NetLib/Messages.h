// Messages.h: Definition of the CMessages class
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_MESSAGES_H__99028114_5B92_47DF_9E3F_0929E7384010__INCLUDED_)
#define AFX_MESSAGES_H__99028114_5B92_47DF_9E3F_0929E7384010__INCLUDED_

#include "resource.h"       // main symbols
#include "collections.h"
#include "message.h"
#pragma warning(disable: 4530)
typedef CComEnumOnSTL<IEnumVARIANT, &IID_IEnumVARIANT, VARIANT,
                        _CopyVariantFromAdaptItf<IMessage>,
                        list< CAdapt< CComPtr<IMessage> > > >
        CComEnumVariantOnListOfMessages;

typedef ICollectionOnSTLImpl<IDispatchImpl<IMessages, &IID_IMessages>,
                             list< CAdapt< CComPtr<IMessage> > >,
                             IMessage*,
                             _CopyItfFromAdaptItf<IMessage>,
                             CComEnumVariantOnListOfMessages>
        IMessagesCollImpl;
#pragma warning(default: 4530)

/////////////////////////////////////////////////////////////////////////////
// CMessages

class ATL_NO_VTABLE CMessages : 
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<CMessages>,
	public IMessagesCollImpl
{

public:
	CMessages() {}
	HRESULT CMessages::FinalRelease()
	{
		MCTRACE(4,"MessageS LIST DELETED");
		return S_OK;
	};
  
	STDMETHODIMP AddMessage(IMessage** ppMessage) 
	{ 
		HRESULT hr = CMessage::CreateInstance(ppMessage);
		if( SUCCEEDED(hr) ) 
		{
			// Put the document on the list
			CComPtr<IMessage>  spMessage = *ppMessage;
			m_coll.push_back(spMessage);
		}
	  return hr;
	}
BEGIN_COM_MAP(CMessages)
	COM_INTERFACE_ENTRY(IMessages)
	COM_INTERFACE_ENTRY(IDispatch)
END_COM_MAP()

DECLARE_NO_REGISTRY()
DECLARE_NOT_AGGREGATABLE(CMessages)
DECLARE_PROTECT_FINAL_CONSTRUCT()


// IMessages
public:
};

#endif // !defined(AFX_MESSAGES_H__99028114_5B92_47DF_9E3F_0929E7384010__INCLUDED_)
