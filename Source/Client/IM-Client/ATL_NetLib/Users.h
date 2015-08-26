// Users.h : Declaration of the CUsers

#ifndef __USERS_H_
#define __USERS_H_

#include "resource.h"       // main symbols
#include "ATL_NETlib.h"
#include "user.h"
#include "collections.h"
#pragma warning(disable: 4530)
typedef CComEnumOnSTL<IEnumVARIANT, &IID_IEnumVARIANT, VARIANT,
                        _CopyVariantFromAdaptItf<IUser>,
                        list< CAdapt< CComPtr<IUser> > > >
        CComEnumVariantOnListOfUsers;

typedef ICollectionOnSTLImpl<IDispatchImpl<IUsers, &IID_IUsers>,
                             list< CAdapt< CComPtr<IUser> > >,
                             IUser*,
                             _CopyItfFromAdaptItf<IUser>,
                             CComEnumVariantOnListOfUsers>
        IUsersCollImpl;
#pragma warning(default: 4530)
/////////////////////////////////////////////////////////////////////////////
// CUsers
class ATL_NO_VTABLE CUsers : 
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<CUsers>,
	public IUsersCollImpl

{
public:
DECLARE_NO_REGISTRY()
DECLARE_NOT_AGGREGATABLE(CUsers)
DECLARE_PROTECT_FINAL_CONSTRUCT()

BEGIN_COM_MAP(CUsers)
	COM_INTERFACE_ENTRY(IUsers)
	COM_INTERFACE_ENTRY(IDispatch)
END_COM_MAP()

// IUsers
public:
	CUsers()
	{
		MCTRACE(3,"USERS LIST CREATED");
	};
	HRESULT CUsers::FinalRelease()
	{
		MCTRACE(3,"USERS LIST DELETED");
		return S_OK;
	};
  STDMETHODIMP AddUser(IUser** ppUser) {
    // Create a document to hand back to the client
    HRESULT hr = CUser::CreateInstance(ppUser);
    if( SUCCEEDED(hr) ) {
      // Put the document on the list
      CComPtr<IUser>  spUser = *ppUser;
      m_coll.push_back(spUser);
    }

	  return hr;
  }
};

#endif //__USERS_H_
