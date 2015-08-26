// Promos.h : Declaration of the CPromos

#ifndef __PROMOS_H_
#define __PROMOS_H_

#include "resource.h"       // main symbols
#include "collections.h"       
#include "promo.h"       
#pragma warning(disable: 4530)
typedef CComEnumOnSTL<IEnumVARIANT, &IID_IEnumVARIANT, VARIANT,
                        _CopyVariantFromAdaptItf<IPromo>,
                        list< CAdapt< CComPtr<IPromo> > > >
        CComEnumVariantOnListOfPromos;

typedef ICollectionOnSTLImpl<IDispatchImpl<IPromos, &IID_IPromos>,
                             list< CAdapt< CComPtr<IPromo> > >,
                             IPromo*,
                             _CopyItfFromAdaptItf<IPromo>,
                             CComEnumVariantOnListOfPromos>
        IPromosCollImpl;
#pragma warning(default: 4530)
/////////////////////////////////////////////////////////////////////////////
// CPromos
class ATL_NO_VTABLE CPromos : 
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<CPromos>,
	public IPromosCollImpl
{
public:
	CPromos()
	{
	}
	
	HRESULT CPromos::FinalRelease()
	{
		MCTRACE(4,"PROMOS LIST DELETED");
		return S_OK;
	};
  
	STDMETHODIMP AddPromo(IPromo** ppPromo) 
	{ 
		HRESULT hr = CPromo::CreateInstance(ppPromo);
		if( SUCCEEDED(hr) ) 
		{
			// Put the document on the list
			CComPtr<IPromo>  spPromo = *ppPromo;
			m_coll.push_back(spPromo);
		}
	  return hr;
	}
DECLARE_NO_REGISTRY()
DECLARE_NOT_AGGREGATABLE(CPromos)
DECLARE_PROTECT_FINAL_CONSTRUCT()

BEGIN_COM_MAP(CPromos)
	COM_INTERFACE_ENTRY(IPromos)
	COM_INTERFACE_ENTRY(IDispatch)
END_COM_MAP()

// IPromos
public:
};

#endif //__PROMOS_H_
