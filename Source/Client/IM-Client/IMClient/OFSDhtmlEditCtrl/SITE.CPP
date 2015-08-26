//=--------------------------------------------------------------------------=
//  (C) Copyright 1997-1998 Microsoft Corporation. All Rights Reserved.
//=--------------------------------------------------------------------------=
// Document Object Site Object

#include "stdafx.h"
#include <afxpriv.h>

#include "OfsDhtmlCtrl.h"
#include "..\Resource.h"
#include "site.h"
#include <urlmon.h>
#include <INITGUID.H>
#include <TRIEDIID.h>
#include <afxole.h>


#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif
/*
 * CSite::CSite
 * CSite::~CSite
 *
 * Constructor Parameters:
 *  hWnd            HWND of the window associated with the site
 *  pFR             PCFrame to the parent structure.
 */
CSite::CSite( HWND hWnd, COfsDhtmlEditCtrl* pFR )
{
    m_cRef = 0;
    m_hWnd = hWnd;
    m_pFR = pFR;

    m_pObj = NULL;
	m_bFiltered = TRUE;//FALSE;
    
    m_pIOleObject = NULL;
    m_pIOleIPObject = NULL;
    m_pIOleDocView = NULL;
	m_pIOleCommandTarget = NULL;

    m_pImpIOleClientSite = NULL;
    m_pImpIAdviseSink = NULL;
    m_pImpIOleIPSite = NULL;
    m_pImpIOleDocumentSite = NULL;
	m_pImpIDocHostUIHandler = NULL;
	m_pImpIDocHostShowUI = NULL;
	m_pImpAmbientIDispatch = NULL;

}


CSite::~CSite(void)
{
    //Object pointers cleaned up in Close.

    //We delete our own interfaces since we control them
    DeleteInterfaceImp( m_pImpIOleDocumentSite );
    DeleteInterfaceImp( m_pImpIOleIPSite );
    DeleteInterfaceImp( m_pImpIAdviseSink );
    DeleteInterfaceImp( m_pImpIOleClientSite );
	DeleteInterfaceImp( m_pImpIDocHostUIHandler );
	DeleteInterfaceImp( m_pImpIDocHostShowUI );
	DeleteInterfaceImp( m_pImpAmbientIDispatch);
}


/*
 * CSite::QueryInterface
 * CSite::AddRef
 * CSite::Release
 *
 * Purpose:
 *  IUnknown members for CSite object.
 */
STDMETHODIMP CSite::QueryInterface( REFIID riid, void **ppv )
{
    *ppv = NULL;

#ifdef _DEBUG
	OLECHAR wszGUID[39];
	StringFromGUID2(riid, wszGUID, 39);
	USES_CONVERSION;
	LPTSTR szGUID = OLE2T(wszGUID);
//	OutputDebugString("CSite::QI(");
//	OutputDebugString(szGUID);
//	OutputDebugString(")\n");
#endif

    if ( IID_IOleClientSite == riid )
	{
        *ppv = m_pImpIOleClientSite;
	}

    if ( IID_IAdviseSink == riid )
	{
        *ppv = m_pImpIAdviseSink;
	}

    if ( IID_IOleWindow == riid || IID_IOleInPlaceSite == riid )
	{
        *ppv = m_pImpIOleIPSite;
	}

    if ( IID_IOleDocumentSite == riid )
	{
        *ppv = m_pImpIOleDocumentSite;
	}

    if ( IID_IDocHostUIHandler == riid )
    {
        *ppv = m_pImpIDocHostUIHandler;
    }

    if ( IID_IDocHostShowUI == riid )
    {
        *ppv = m_pImpIDocHostShowUI;
    }

    if ( IID_IDispatch == riid )
    {
        *ppv = m_pImpAmbientIDispatch;
    }

	if(IID_IDropTarget==riid)
	{
		*ppv = this;
	}

    if ( NULL != *ppv )
    {
        ((LPUNKNOWN)*ppv)->AddRef();
        return NOERROR;
    }
	
	// Try Fix It: [4/3/2003]
	// Unhandled exception at 0x61726153 in dump1.mdmp: 
	//  0xC0000005: Access violation reading location 0x61726153. 
	if(IsBadReadPtr(GetFrame(),sizeof(COfsDhtmlEditCtrl)))
		return E_NOINTERFACE;

	// Try the frame instead
	return GetFrame()->QueryInterface( riid, ppv );

}


STDMETHODIMP_(ULONG) CSite::AddRef(void)
{
    return ++m_cRef;
}

STDMETHODIMP_(ULONG) CSite::Release(void)
{
    if ( 0 != --m_cRef )
	{
        return m_cRef;
	}

    delete this;

    return 0;
}

/*
 * CSite::Create
 *
 * Purpose:
 *  Asks the site to instantiate the MSHTML.DLL object.
 *  
 *
 * Parameters:
 *  pIStorage       IStorage * of the parent storage in which we're
 *                  to create an IStorage for the new object.
 *  pchPath         Path of what to load..
 *
 * Return Value:
 *  BOOL            Result of the creation.
 */
BOOL CSite::Create(LPCTSTR pchPath)
{
    HRESULT   hr;

	// Create the site's interface implementations which MSHTML.DLL will call
    m_pImpIOleClientSite = new CImpIOleClientSite( this, this );
    m_pImpIAdviseSink = new CImpIAdviseSink( this, this );
    m_pImpIOleIPSite = new CImpIOleInPlaceSite( this, this );
    m_pImpIOleDocumentSite = new CImpIOleDocumentSite( this, this );
	m_pImpIDocHostUIHandler = new CImpIDocHostUIHandler( this, this );
	m_pImpIDocHostShowUI = new CImpIDocHostShowUI( this, this );
	m_pImpAmbientIDispatch = new CImpAmbientIDispatch( this, this );

    if ( NULL == m_pImpIOleClientSite || NULL == m_pImpIAdviseSink
        || NULL == m_pImpIOleIPSite || NULL == m_pImpIOleDocumentSite
		|| NULL == m_pImpIDocHostUIHandler )
	{
		ASSERT(FALSE);
        return FALSE;
	}


	hr = CoCreateInstance( CLSID_TriEditDocument, NULL,								
			CLSCTX_INPROC_SERVER, IID_IUnknown, (void **)&m_pObj );
    //If creation didn't work, then clean up.

	//hr = E_FAIL;

    if ( FAILED( hr ) )
    {
		LPVOID lpMsgBuf;
		FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
			NULL, hr, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), // Default language
			(LPTSTR) &lpMsgBuf,	0,NULL 
			);
		// Process any inserts in lpMsgBuf.
		CString	strMessage;
		strMessage.Format(GetString(IDS_DHTML_CREATE_ERROR_MGS), hr,(LPCTSTR)lpMsgBuf);

		if(IDYES ==::MessageBox(NULL,strMessage,GetString(IDS_ERROR_TITLE),MB_YESNO|MB_ICONERROR))
		{
			NavigateNewWindow(NULL, _T("http://www.microsoft.com/downloads/details.aspx?familyid=b769a4b8-48ed-41a1-8095-5a086d1937cb&displaylang=en"));
		}

		ASSERT(FALSE);
        return FALSE;
    }

    // Initialize the object we just created. Set ClientSite, the AdviseSink, etc.
    return ObjectInitialize( pchPath );
}


/*
 * CSite::ObjectInitialize
 * (Protected)
 *
 * Purpose:
 *  Performs operations necessary after creating an object or
 *  reloading one from storage.
 *
 * Parameters:
 *	pchPath			The path that we want to load from
 *
 * Return Value:
 *  BOOL            TRUE if the function succeeded, FALSE otherwise.
 */
BOOL CSite::ObjectInitialize(LPCTSTR pchPath)
{
    HRESULT         hr;
    DWORD           dw;

    if ( NULL == m_pObj )
	{
		ASSERT(FALSE);
        return FALSE;
	}


    //We need an IOleObject most of the time, so get one here.
    hr = m_pObj->QueryInterface( IID_IOleObject, (void **)&m_pIOleObject );
    if ( FAILED( hr ) )
	{
		ASSERT(FALSE);
        return FALSE;
	}
    //SetClientSite is critical for DocObjects
    hr = m_pIOleObject->SetClientSite( m_pImpIOleClientSite );
	if(FAILED(hr))
	{
		ASSERT(FALSE);
	}

    hr = m_pIOleObject->Advise( m_pImpIAdviseSink, &dw );
	if(FAILED(hr))
	{
		ASSERT(FALSE);
	}

    if ( FAILED( Load( pchPath ) ) )
	{
		ASSERT(FALSE);
		return FALSE;
	}


	// Put the object in the running state
	hr = OleRun( m_pIOleObject );
	if(FAILED(hr))
	{
		ASSERT(FALSE);
	}
    return TRUE;
}



/*
 * CSite::Close
 *
 * Purpose:
 *  Possibly commits the storage, then releases it, afterwards
 *  frees alls the object pointers.
 *
 * Parameters:
 *  fCommit         BOOL indicating if we're to commit.
 *
 * Return Value:
 *  None
 */
void CSite::Close(BOOL fCommit)
{
    //OnInPlaceDeactivate releases this pointer.
    if ( NULL != m_pIOleIPObject )
	{
        m_pIOleIPObject->InPlaceDeactivate();
	}

    ReleaseInterface( m_pIOleDocView );
	ReleaseInterface( m_pIOleCommandTarget );
    ReleaseInterface( m_pObj );
    if ( NULL != m_pIOleObject )
    {
        m_pIOleObject->Close( fCommit ? OLECLOSE_SAVEIFDIRTY : OLECLOSE_NOSAVE );
        ReleaseInterface( m_pIOleObject );
    }
}



/*
 * CSite::Activate
 *
 * Purpose:
 *  Activates a verb on the object living in the site.
 *
 * Parameters:
 *  iVerb           LONG of the verb to execute.
 *
 * Return Value:
 *  None
 */

void CSite::Activate( LONG iVerb )
{
    CWaitCursor *pWait;
    RECT        rc;
            
    pWait = new CWaitCursor;

    GetClientRect( m_hWnd, &rc );

	if(m_pIOleObject!=NULL)
	{
		HRESULT hr = m_pIOleObject->DoVerb( iVerb, NULL, m_pImpIOleClientSite, 0, m_hWnd, &rc );
		//ASSERT(SUCCEEDED(hr));
	}

    delete pWait;
}




/*
 * CSite::UpdateObjectRects
 *
 * Purpose:
 *  Informs the site that the client area window was resized and
 *  that the site needs to also tell the DocObject of the resize.
 *
 * Parameters:
 *  None
 *
 * Return Value:
 *  None
 */

void CSite::UpdateObjectRects( void )
{
    if ( NULL != m_pIOleDocView )
	{
		RECT    rc;
	    GetClientRect(m_hWnd, &rc);
		m_pIOleDocView->SetRect(&rc);
	}
}


/*
 * CSite::Load
 *
 * Purpose:
 *  Loads the path provided using IPersistMoniker or IPersistFile. If no path
 *	was provided it simply does an InitNew
 *
 * Parameters:
 *  pchPath - The path
 *
 * Return Value:
 */
HRESULT CSite::Load(LPCTSTR pchPath)
{
    HRESULT   hr = S_OK;

    if ( pchPath != NULL && *pchPath != 0)
    {
		USES_CONVERSION;
		LPWSTR szwName = T2W(const_cast<LPTSTR>(pchPath));

		// Path has been provided so check should we use IPersistMoniker or IPersistFile?
        if (memcmp(pchPath, _T("file:"), 5 * sizeof(TCHAR)) == 0 ||
            memcmp(pchPath, _T("http:"), 5 * sizeof(TCHAR)) == 0 ||
			memcmp(pchPath, _T("https:"), 5 * sizeof(TCHAR)) == 0)
        {
//		    OLECHAR  szwName[256];
//			MultiByteToWideChar(CP_ACP, 0, pchPath, -1, szwName, 256);    

			// Ask the system for a URL Moniker
			IMoniker* pIMoniker;
			hr = CreateURLMoniker(NULL, (LPWSTR)szwName, &pIMoniker);
		    if ( SUCCEEDED(hr) )
			{
				// Get the IPersistMoniker interface
			    IPersistMoniker* pPMk;
				hr = m_pObj->QueryInterface(
										IID_IPersistMoniker,
										(void **)&pPMk);
			    if ( SUCCEEDED(hr) )
				{

					// note: do not register our BSC when calling IPM::Load directly
					IBindCtx *pBCtx;
					hr = CreateBindCtx(0, &pBCtx);
				    if ( SUCCEEDED(hr) )
					{
						// Call Load on the IPersistMoniker
						hr = pPMk->Load(FALSE, pIMoniker, pBCtx, STGM_READ);

						pBCtx->Release();
					}
				    ReleaseInterface(pPMk);
				}
				ReleaseInterface( pIMoniker );
			}
        }
		else if ( m_pFR->IsFilterIn() )
		{
			IPersistStreamInit* pPStm = NULL;
			LPSTREAM pFilteredStream = NULL;
			DWORD dwFlags = 0;

//			dwFlags = ((CCEditApp*) AfxGetApp())->GetOptions().GetFilterFlags();
//			dwFlags |= dwFilterMultiByteStream; // loading an ANSI Stream

			hr = m_pObj->QueryInterface(
						IID_IPersistStreamInit,
						(void **)&pPStm);

			if ( SUCCEEDED(hr) )
			{
				if (SUCCEEDED(hr = HrFilterIn(pchPath, &pFilteredStream, dwFlags)))
				{
					hr = pPStm->Load(pFilteredStream);

					ReleaseInterface(pFilteredStream);
					ReleaseInterface(pPStm);
				}
			}
		}
		else
        {
//		    OLECHAR  szwName[256];
//			MultiByteToWideChar(CP_ACP, 0, pchPath, -1, szwName, 256);    
		    IPersistFile*  pPFile;
            hr = m_pObj->QueryInterface(
									IID_IPersistFile,
									(void **) &pPFile);
		    if ( SUCCEEDED(hr) )
			{
				// Call Load on the IPersistFile
				hr = pPFile->Load((LPWSTR)szwName, 0);
			    ReleaseInterface(pPFile);
			}
        }
    }
	else
	{
		// No path provided so just do an InitNew on the Stream 
	    IPersistStreamInit* pPStm;
		hr = m_pObj->QueryInterface(
					IID_IPersistStreamInit,
					(void **)&pPStm);
	    if ( SUCCEEDED(hr) )
		{
			hr = pPStm->InitNew();
		    ReleaseInterface(pPStm);
		}
	}

	return hr;
}


HRESULT CSite::Save(LPCTSTR pchPath)
{
    HRESULT   hr = S_OK;

//    OLECHAR  szwName[256];
	USES_CONVERSION;
	LPWSTR szwName = T2W(const_cast<LPTSTR>(pchPath));
	LPOLESTR pszwName = szwName;
	LPOLESTR *ppszwName = &pszwName;

	IPersistFile*  pPFile = NULL;
    hr = m_pObj->QueryInterface(
								IID_IPersistFile,
								(void **) &pPFile);
	if ( SUCCEEDED(hr) && pPFile)
	{
		if ( pchPath != NULL && *pchPath != 0)
		{
//			MultiByteToWideChar(CP_ACP, 0, pchPath, -1, szwName, 256);    
		}
		else 
		{
			hr = pPFile->GetCurFile(ppszwName);
		}
		if ( SUCCEEDED(hr) && ppszwName)
		{
			// Call Save on the IPersistFile
			hr = pPFile->Save(*ppszwName, TRUE);
			if ( SUCCEEDED(hr) )
			{
				hr = pPFile->SaveCompleted(*ppszwName);
			}
	    }
	    ReleaseInterface(pPFile);
    }

	return hr;
}


HRESULT	CSite::SaveWithFilter(LPCTSTR pchPath)
{
    HRESULT   hr = S_OK;

    if ( pchPath != NULL && *pchPath != 0)
    {
	    IPersistStreamInit* pPStm = NULL;
		LPSTREAM pStream = NULL;
		DWORD dwFlags = 0;

//		dwFlags = ((CCEditApp*) AfxGetApp())->GetOptions().GetFilterFlags();
//		dwFlags |= dwFilterMultiByteStream;

		hr = m_pObj->QueryInterface(
					IID_IPersistStreamInit,
					(void **)&pPStm);

	    if ( SUCCEEDED(hr) )
		{
			hr = CreateStreamOnHGlobal(NULL, TRUE, &pStream);

			ASSERT(SUCCEEDED(hr));
			hr = pPStm->Save(pStream, TRUE);

			ASSERT(SUCCEEDED(hr));

			hr = HrFilterOut(pStream, pchPath, dwFlags);

			ASSERT(SUCCEEDED(hr));

			ReleaseInterface(pStream);
			ReleaseInterface(pPStm);

		}

    }

	return hr;
}


STDMETHODIMP CSite::DragEnter(IDataObject * pDataObject, WORD grfKeyState, POINTL pt,DWORD * pdwEffect)
{
	if(!pDataObject||!pdwEffect)
		return E_INVALIDARG;

	COleDataObject	pData; 
	pData.Attach(pDataObject,FALSE);
	
	FORMATETC stFormatTEXT = {CF_TEXT,NULL,DVASPECT_CONTENT,-1,TYMED_HGLOBAL};
	
	if(pData.IsDataAvailable(CF_HDROP))
	{
		
		*pdwEffect = DROPEFFECT_NONE;
		return E_INVALIDARG;
	}

	*pdwEffect = DROPEFFECT_COPY;
	
	return S_OK;
}

STDMETHODIMP CSite::DragOver(DWORD grfKeyState, POINTL pt,	DWORD * pdwEffect)
{
	if(!pdwEffect)
		return E_INVALIDARG;

	return S_OK;
}

STDMETHODIMP CSite::DragLeave(void)
{
	return S_OK;
}

STDMETHODIMP CSite::Drop(IDataObject * pDataObject,DWORD grfKeyState, POINTL pt,DWORD * pdwEffect)
{
	COleDataObject	pData;
	pData.Attach(pDataObject,FALSE);
	
	FORMATETC stFormatTEXT = {CF_TEXT,NULL,DVASPECT_CONTENT,-1,TYMED_HGLOBAL};
	
	STGMEDIUM outData =	{0};
	
	if(pData.GetData(CF_TEXT,&outData,&stFormatTEXT))
	{
		_bstr_t strText = (LPCTSTR)GlobalLock(outData.hGlobal);

		m_pFR->InsertTEXT(strText);
		
		GlobalUnlock(outData.hGlobal);
	}
				
	return S_OK;
}


HRESULT CSite::TranslateAccelerator(LPMSG lpMsg, const GUID *pguidCmdGroup, DWORD nCmdID)
{
	if(nCmdID==0x1A)
	{
		if (OpenClipboard(GetWindow()))
		{
			HANDLE hText = GetClipboardData(CF_TEXT);
			
			if(hText!=NULL&&m_pFR)
			{
				_bstr_t strText = (LPCTSTR)GlobalLock(hText);
				m_pFR->InsertTEXT(strText);
				GlobalUnlock(hText);
			}
			
			CloseClipboard();
		}
		
		return S_OK;
	}
	return S_FALSE;
}
