//=--------------------------------------------------------------------------=
//  (C) Copyright 1997-1998 Microsoft Corporation. All Rights Reserved.
//=--------------------------------------------------------------------------=
// IAdviseSink for Document Objects CSite class

#include "stdafx.h"
#include <docobj.h>
#include "site.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif


/*
 * CImpIAdviseSink::CImpIAdviseSink
 * CImpIAdviseSink::~CImpIAdviseSink
 *
 * Parameters (Constructor):
 *  pSite           PCSite of the site we're in.
 *  pUnkOuter       LPUNKNOWN to which we delegate.
 */

CImpIAdviseSink::CImpIAdviseSink( PCSite pSite, LPUNKNOWN pUnkOuter )
{
    m_cRef = 0;
    m_pSite = pSite;
    m_pUnkOuter = pUnkOuter;
}

CImpIAdviseSink::~CImpIAdviseSink( void )
{
}


/*
 * CImpIAdviseSink::QueryInterface
 * CImpIAdviseSink::AddRef
 * CImpIAdviseSink::Release
 *
 * Purpose:
 *  IUnknown members for CImpIAdviseSink object.
 */

STDMETHODIMP CImpIAdviseSink::QueryInterface( REFIID riid, void **ppv )
{
    return m_pUnkOuter->QueryInterface( riid, ppv );
}


STDMETHODIMP_(ULONG) CImpIAdviseSink::AddRef( void )
{
    ++m_cRef;
    return m_pUnkOuter->AddRef();
}

STDMETHODIMP_(ULONG) CImpIAdviseSink::Release( void )
{
    --m_cRef;
    return m_pUnkOuter->Release();
}



/*
 * Unused members in CImpIAdviseSink
 *  OnDataChange
 *  OnViewChange
 *  OnRename
 *  OnSave
 */

STDMETHODIMP_(void) CImpIAdviseSink::OnDataChange(LPFORMATETC /*pFEIn*/,
											LPSTGMEDIUM /*pSTM*/)
{

}

STDMETHODIMP_(void) CImpIAdviseSink::OnViewChange(DWORD /*dwAspect*/,
												LONG /*lindex*/)
{    
}

STDMETHODIMP_(void) CImpIAdviseSink::OnRename( LPMONIKER /*pmk*/ )
{
}

STDMETHODIMP_(void) CImpIAdviseSink::OnSave( void )
{
}


STDMETHODIMP_(void) CImpIAdviseSink::OnClose( void )
{
}
