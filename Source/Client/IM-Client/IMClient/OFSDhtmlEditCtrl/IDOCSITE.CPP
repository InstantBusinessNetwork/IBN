//=--------------------------------------------------------------------------=
//  (C) Copyright 1997-1998 Microsoft Corporation. All Rights Reserved.
//=--------------------------------------------------------------------------=
// IOleDocumentSite for Document Objects CSite class


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
 * CImpIOleDocumentSite::CImpIOleDocumentSite
 * CImpIOleDocumentSite::~CImpIOleDocumentSite
 *
 * Parameters (Constructor):
 *  pSite           PCSite of the site we're in.
 *  pUnkOuter       LPUNKNOWN to which we delegate.
 */
CImpIOleDocumentSite::CImpIOleDocumentSite( PCSite pSite, LPUNKNOWN pUnkOuter)
{
    m_cRef = 0;
    m_pSite = pSite;
    m_pUnkOuter = pUnkOuter;
}

CImpIOleDocumentSite::~CImpIOleDocumentSite( void )
{
}



/*
 * CImpIOleDocumentSite::QueryInterface
 * CImpIOleDocumentSite::AddRef
 * CImpIOleDocumentSite::Release
 *
 * Purpose:
 *  IUnknown members for CImpIOleDocumentSite object.
 */
STDMETHODIMP CImpIOleDocumentSite::QueryInterface( REFIID riid, void **ppv )
{
    return m_pUnkOuter->QueryInterface( riid, ppv );
}


STDMETHODIMP_(ULONG) CImpIOleDocumentSite::AddRef( void )
{
    ++m_cRef;
    return m_pUnkOuter->AddRef();
}

STDMETHODIMP_(ULONG) CImpIOleDocumentSite::Release( void )
{
    --m_cRef;
    return m_pUnkOuter->Release();
}



/*
 * CImpIOleDocumentsite::ActivateMe
 *
 * Purpose:
 *  Instructs the container to activate the object in this site as
 *  a document object.
 *
 * Parameters:
 *  pView           IOleDocumentView * of the object to activate.
 *
 * Return Value:
 *  HRESULT         NOERROR if successful, error code otherwise.
 */
STDMETHODIMP CImpIOleDocumentSite::ActivateMe( IOleDocumentView *pView )
{
    RECT                rc;
    IOleDocument*       pDoc;
    
    /*
     * If we're passed a NULL view pointer, then try to get one from
     * the document object (the object within us).
     */
    if ( NULL == pView )
    {
        if ( FAILED( m_pSite->GetObjectUnknown()->QueryInterface( 
 								IID_IOleDocument, (void **)&pDoc ) ) )
		{
            return E_FAIL;
		}

        if ( FAILED( pDoc->CreateView( m_pSite->GetIPSite(),
												NULL, 0, &pView ) ) )
		{
            return E_OUTOFMEMORY;
		}

        // Release doc pointer since CreateView is a good com method that addrefs
        pDoc->Release();
    }        
    else
    {
        //Make sure that the view has our client site
        pView->SetInPlaceSite( m_pSite->GetIPSite() );

        //We're holding onto the pointer, so AddRef it.
        pView->AddRef();
    }


    // Activation steps, now that we have a view:

    m_pSite->SetDocView( pView );
    
    //This sets up toolbars and menus first    
    pView->UIActivate( TRUE );

    //Set the window size sensitive to new toolbars
    m_pSite->GetFrame()->GetClientRect( &rc );
    pView->SetRect( &rc );

	//Makes it all active
    pView->Show( TRUE );    
    return NOERROR;
}
