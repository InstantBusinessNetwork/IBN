// SIDs.h: Definition of the CSIDs class
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_SIDS_H__AB6571B7_5006_4283_9435_B4471028A8AF__INCLUDED_)
#define AFX_SIDS_H__AB6571B7_5006_4283_9435_B4471028A8AF__INCLUDED_

#include "resource.h"       // main symbols
#include "collections.h"
#include "SID.h"
#pragma warning(disable: 4530)
typedef CComEnumOnSTL<IEnumVARIANT, &IID_IEnumVARIANT, VARIANT,
                        _CopyVariantFromAdaptItf<IlocalSID>,
                        list< CAdapt< CComPtr<IlocalSID> > > >
        CComEnumVariantOnListOflocalSIDs;

typedef ICollectionOnSTLImpl<IDispatchImpl<IlocalSIDs, &IID_IlocalSIDs>,
                             list< CAdapt< CComPtr<IlocalSID> > >,
                             IlocalSID*,
                             _CopyItfFromAdaptItf<IlocalSID>,
                             CComEnumVariantOnListOflocalSIDs>
        IlocalSIDsCollImpl;
#pragma warning(default: 4530)

/////////////////////////////////////////////////////////////////////////////
// CSIDs

class ATL_NO_VTABLE ClocalSIDs : 
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<ClocalSIDs>,
	public IlocalSIDsCollImpl

{
public:
	ClocalSIDs() {}
		HRESULT ClocalSIDs::FinalRelease()
	{
		//MCTRACE(4,"localSIDS LIST DELETED");
		return S_OK;
	};
  
	STDMETHODIMP AddSID(IlocalSID** pplocalSID) 
	{ 
		HRESULT hr = ClocalSID::CreateInstance(pplocalSID);
		if( SUCCEEDED(hr) ) 
		{
			// Put the document on the list
			CComPtr<IlocalSID>  splocalSID = *pplocalSID;
			m_coll.push_back(splocalSID);
		}
	  return hr;
	}

BEGIN_COM_MAP(ClocalSIDs)
	COM_INTERFACE_ENTRY(IDispatch)
	COM_INTERFACE_ENTRY(IlocalSIDs)
END_COM_MAP()

DECLARE_NO_REGISTRY()
DECLARE_NOT_AGGREGATABLE(ClocalSIDs) 
DECLARE_PROTECT_FINAL_CONSTRUCT()
// ISIDs
public:
};

#endif // !defined(AFX_SIDS_H__AB6571B7_5006_4283_9435_B4471028A8AF__INCLUDED_)
