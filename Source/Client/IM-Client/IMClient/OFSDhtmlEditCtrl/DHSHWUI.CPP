//=--------------------------------------------------------------------------=
//  (C) Copyright 1997-1998 Microsoft Corporation. All Rights Reserved.
//=--------------------------------------------------------------------------=
// IDocHostShowUI for Document Objects CSite class


#include "stdafx.h"
#include <docobj.h>
#include "site.h"
#include "OfsDhtmlCtrl.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/*
 * CImpIDocHostShowUI::CImpIDocHostShowUI
 * CImpIDocHostShowUI::~CImpIDocHostShowUI
 *
 * Parameters (Constructor):
 *  pSite           PCSite of the site we're in.
 *  pUnkOuter       LPUNKNOWN to which we delegate.
 */
CImpIDocHostShowUI::CImpIDocHostShowUI( PCSite pSite, LPUNKNOWN pUnkOuter)
{
    m_cRef = 0;
    m_pSite = pSite;
    m_pUnkOuter = pUnkOuter;
}

CImpIDocHostShowUI::~CImpIDocHostShowUI( void )
{
}



/*
 * CImpIDocHostShowUI::QueryInterface
 * CImpIDocHostShowUI::AddRef
 * CImpIDocHostShowUI::Release
 *
 * Purpose:
 *  IUnknown members for CImpIDocHostShowUI object.
 */
STDMETHODIMP CImpIDocHostShowUI::QueryInterface( REFIID riid, void **ppv )
{
    return m_pUnkOuter->QueryInterface( riid, ppv );
}


STDMETHODIMP_(ULONG) CImpIDocHostShowUI::AddRef( void )
{
    ++m_cRef;
    return m_pUnkOuter->AddRef();
}

STDMETHODIMP_(ULONG) CImpIDocHostShowUI::Release( void )
{
    --m_cRef;
    return m_pUnkOuter->Release();
}


/*
 * CImpIDocHostShowUI::ShowMessage
 *
 */
STDMETHODIMP CImpIDocHostShowUI::ShowMessage(
            HWND /*hwnd*/,
            LPOLESTR /*lpstrText*/,
            LPOLESTR /*lpstrCaption*/, 
            DWORD /*dwType*/,
            LPOLESTR /*lpstrHelpFile*/,
            DWORD /*dwHelpContext*/,
            LRESULT* /*plResult*/)
{
	return S_FALSE;
}

/*
 * CImpIDocHostShowUI::ShowHelp
 */
STDMETHODIMP CImpIDocHostShowUI::ShowHelp(
            HWND /*hwnd*/,
            LPOLESTR /*pszHelpFile*/,
            UINT /*uCommand*/,
            DWORD /*dwData*/,
            POINT /*ptMouse*/,
            IDispatch* /*pDispatchObjectHit*/)
{
	return S_FALSE;
}

