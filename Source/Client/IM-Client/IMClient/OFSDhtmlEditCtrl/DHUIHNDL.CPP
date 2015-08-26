//=--------------------------------------------------------------------------=
//  (C) Copyright 1997-1998 Microsoft Corporation. All Rights Reserved.
//=--------------------------------------------------------------------------=
// IDocHostUIHandler for Document Objects CSite class


#include "stdafx.h"
#include <docobj.h>
#include "site.h"
#include "OfsDhtmlCtrl.h"
//#include "CEdit.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/*
 * CImpIDocHostUIHandler::CImpIDocHostUIHandler
 * CImpIDocHostUIHandler::~CImpIDocHostUIHandler
 *
 * Parameters (Constructor):
 *  pSite           PCSite of the site we're in.
 *  pUnkOuter       LPUNKNOWN to which we delegate.
 */
CImpIDocHostUIHandler::CImpIDocHostUIHandler( PCSite pSite, LPUNKNOWN pUnkOuter)
{
    m_cRef = 0;
    m_pSite = pSite;
    m_pUnkOuter = pUnkOuter;
}

CImpIDocHostUIHandler::~CImpIDocHostUIHandler( void )
{
}



/*
 * CImpIDocHostUIHandler::QueryInterface
 * CImpIDocHostUIHandler::AddRef
 * CImpIDocHostUIHandler::Release
 *
 * Purpose:
 *  IUnknown members for CImpIOleDocumentSite object.
 */
STDMETHODIMP CImpIDocHostUIHandler::QueryInterface( REFIID riid, void **ppv )
{
    return m_pUnkOuter->QueryInterface( riid, ppv );
}


STDMETHODIMP_(ULONG) CImpIDocHostUIHandler::AddRef( void )
{
    ++m_cRef;
    return m_pUnkOuter->AddRef();
}

STDMETHODIMP_(ULONG) CImpIDocHostUIHandler::Release( void )
{
    --m_cRef;
    return m_pUnkOuter->Release();
}



// * CImpIDocHostUIHandler::GetHostInfo
// *
// * Purpose: Called at initialization
// *
STDMETHODIMP CImpIDocHostUIHandler::GetHostInfo( DOCHOSTUIINFO* pInfo )
{
    pInfo->dwFlags = DOCHOSTUIFLAG_NO3DBORDER;
    pInfo->dwDoubleClick = DOCHOSTUIDBLCLK_DEFAULT;

    return S_OK;
}

// * CImpIDocHostUIHandler::ShowUI
STDMETHODIMP CImpIDocHostUIHandler::ShowUI(
				DWORD dwID, 
				IOleInPlaceActiveObject * /*pActiveObject*/,
				IOleCommandTarget * pCommandTarget,
				IOleInPlaceFrame * /*pFrame*/,
				IOleInPlaceUIWindow * /*pDoc*/)
{

	// If we havent already got the CommandTraget interface then store it away now.
	if ( m_pSite->GetCommandTarget() == NULL )
	{
		if ( pCommandTarget != NULL )
		{
			pCommandTarget->AddRef();
			m_pSite->SetCommandTarget( pCommandTarget );
			m_pSite->GetFrame()->SetOptions();
		}
	}

	// We've already got our own UI in place so just return S_OK
    return S_OK;
}

// * CImpIDocHostUIHandler::HideUI
STDMETHODIMP CImpIDocHostUIHandler::HideUI(void)
{
    return S_OK;
}

// * CImpIDocHostUIHandler::UpdateUI
STDMETHODIMP CImpIDocHostUIHandler::UpdateUI(void)
{
	// MFC is pretty good about updating it's UI in it's Idle loop so don't do anything here
	return S_OK;
}

// * CImpIDocHostUIHandler::EnableModeless
// *
// * Purpose: Called from IOleInPlaceActiveObject::EnableModeless
// *
STDMETHODIMP CImpIDocHostUIHandler::EnableModeless(BOOL /*fEnable*/)
{
    return E_NOTIMPL;
}

// * CImpIDocHostUIHandler::OnDocWindowActivate
// *
// * Purpose: Called from IOleInPlaceActiveObject::OnDocWindowActivate
// *
STDMETHODIMP CImpIDocHostUIHandler::OnDocWindowActivate(BOOL /*fActivate*/)
{
    return E_NOTIMPL;
}

// * CImpIDocHostUIHandler::OnFrameWindowActivate
// *
// * Purpose: Called from IOleInPlaceActiveObject::OnFrameWindowActivate
// *
STDMETHODIMP CImpIDocHostUIHandler::OnFrameWindowActivate(BOOL /*fActivate*/)
{
    return E_NOTIMPL;
}

// * CImpIDocHostUIHandler::ResizeBorder
// *
// * Purpose: Called from IOleInPlaceActiveObject::ResizeBorder
// *
STDMETHODIMP CImpIDocHostUIHandler::ResizeBorder(
				LPCRECT /*prcBorder*/, 
				IOleInPlaceUIWindow* /*pUIWindow*/,
				BOOL /*fRameWindow*/)
{
    return E_NOTIMPL;
}

// * CImpIDocHostUIHandler::ShowContextMenu
// *
// * Purpose: Called when it would normally display its context menu
// *
STDMETHODIMP CImpIDocHostUIHandler::ShowContextMenu(
				DWORD dwID, 
				POINT* pptPosition,
				IUnknown* pCommandTarget,
				IDispatch* pDispatchObjectHit)
{
	if(::IsWindow(m_pSite->GetFrame()->GetSafeHwnd()))
		return m_pSite->GetFrame()->ShowContextMenu(dwID,pptPosition,pCommandTarget,pDispatchObjectHit);
    return E_NOTIMPL;
}

// * CImpIDocHostUIHandler::TranslateAccelerator
// *
// * Purpose: Called from TranslateAccelerator routines
// *
STDMETHODIMP CImpIDocHostUIHandler::TranslateAccelerator(LPMSG lpMsg,
            /* [in] */ const GUID __RPC_FAR *pguidCmdGroup,
            /* [in] */ DWORD nCmdID)
{
	if(m_pSite)
		return m_pSite->TranslateAccelerator(lpMsg,pguidCmdGroup,nCmdID);
	
    return S_FALSE;
}

// * CImpIDocHostUIHandler::GetOptionKeyPath
// *
// * Purpose: Called to find where the host wishes to store 
// *	its options in the registry
// *
STDMETHODIMP CImpIDocHostUIHandler::GetOptionKeyPath(BSTR* pbstrKey, DWORD)
{
	//CString strPath;
	//strPath.Format(_T("Software\\%s\\%s\\McDHTMLEdit"),AfxGetApp()->m_pszRegistryKey,AfxGetApp()->m_pszProfileName);

	HRESULT hr	=	E_INVALIDARG;

	*pbstrKey	=	NULL;

    /*if (pbstrKey)
    {
        BSTR szMyKey = strPath.AllocSysString();
        *pbstrKey = (LPOLESTR)CoTaskMemAlloc((wcslen(szMyKey) + 1) * sizeof(WCHAR));
        if (*pbstrKey)
            wcscpy(*pbstrKey, szMyKey);
        hr = (*pbstrKey) ? S_OK : E_OUTOFMEMORY;
        SysFreeString(szMyKey);
    }*/
	
    return hr;
}

STDMETHODIMP CImpIDocHostUIHandler::GetDropTarget( 
            /* [in] */ IDropTarget __RPC_FAR *pDropTarget,
            /* [out] */ IDropTarget __RPC_FAR *__RPC_FAR *ppDropTarget)
{
    return m_pSite->QueryInterface(IID_IDropTarget,(void**)ppDropTarget);
}

STDMETHODIMP CImpIDocHostUIHandler::GetExternal( 
    /* [out] */ IDispatch __RPC_FAR *__RPC_FAR *ppDispatch)
{
    return E_NOTIMPL;
}

STDMETHODIMP CImpIDocHostUIHandler::TranslateUrl( 
    /* [in] */ DWORD dwTranslate,
    /* [in] */ OLECHAR __RPC_FAR *pchURLIn,
    /* [out] */ OLECHAR __RPC_FAR *__RPC_FAR *ppchURLOut)
{
    return E_NOTIMPL;
}

STDMETHODIMP CImpIDocHostUIHandler::FilterDataObject( 
    /* [in] */ IDataObject __RPC_FAR *pDO,
    /* [out] */ IDataObject __RPC_FAR *__RPC_FAR *ppDORet)
{
    return S_FALSE;
}
