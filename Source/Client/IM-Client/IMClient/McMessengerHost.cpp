#include "stdafx.h"
#include "mcmessengerhost.h"
#include "MainDlg.h"
#include "WebBrowser2.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

static UINT _MC_CF_INETURL			=	RegisterClipboardFormat(CFSTR_INETURL);
static UINT _MC_CF_LINKDESCRIPTOR	=	RegisterClipboardFormat(CFSTR_FILEDESCRIPTOR);

DROPEFFECT CMcMessengerDropTarget::OnDragEnter(	CWnd* pWnd,COleDataObject* pDataObject,DWORD dwKeyState,CPoint point )
{
	m_dropEffectCurrent = DROPEFFECT_NONE;
	
	FORMATETC stFormatHDROP = {CF_HDROP,NULL,DVASPECT_CONTENT,-1,TYMED_HGLOBAL};

	FORMATETC stFormatInetUrl = {_MC_CF_INETURL,NULL,DVASPECT_CONTENT,-1,TYMED_HGLOBAL};

	FORMATETC stFormatText = {CF_TEXT,NULL,DVASPECT_CONTENT,-1,TYMED_HGLOBAL};

#ifdef _DEBUG
	FORMATETC stFormatEnum	=	{0};
	pDataObject->BeginEnumFormats();

	while(pDataObject->GetNextFormat(&stFormatEnum))
	{
		TRACE(_T("\r\n cfFormat = 0x%X, dwAspect = %d, tymed = %d;"),
			stFormatEnum.cfFormat,
			stFormatEnum.dwAspect,
			stFormatEnum.tymed);
	}

#endif

	if(pDataObject->IsDataAvailable(CF_HDROP,&stFormatHDROP))
	{
		
		CComPtr<IHTMLDocument>	pDoc;
		pDoc.Attach((IHTMLDocument*)m_pAttachBrowser->GetDocument());
		
		if(pDoc!=NULL)
		{
			CComPtr<IDispatch> spDispScript = NULL;
			HRESULT hr = pDoc->get_Script(&spDispScript);
			if(spDispScript)
			{
				DISPID dispid = -1;
				OLECHAR FAR* szMember = L"_DropFile";
				hr = spDispScript->GetIDsOfNames(IID_NULL, &szMember, 1, LOCALE_SYSTEM_DEFAULT, &dispid);
				if(hr==S_OK)
				{
					m_dropEffectCurrent =  DROPEFFECT_COPY;
				}
			}
		}
	}
	else if(pDataObject->IsDataAvailable(_MC_CF_INETURL,&stFormatInetUrl))
	{
		CComPtr<IHTMLDocument>	pDoc;
		pDoc.Attach((IHTMLDocument*)m_pAttachBrowser->GetDocument());
		
		if(pDoc!=NULL)
		{
			CComPtr<IDispatch> spDispScript = NULL;
			HRESULT hr = pDoc->get_Script(&spDispScript);
			if(spDispScript)
			{
				DISPID dispid = -1;
				OLECHAR FAR* szMember = L"_DropLink";
				hr = spDispScript->GetIDsOfNames(IID_NULL, &szMember, 1, LOCALE_SYSTEM_DEFAULT, &dispid);
				if(hr==S_OK)
				{
					m_dropEffectCurrent =  DROPEFFECT_LINK;
				}
			}
		}
	}
	else if(pDataObject->IsDataAvailable(CF_TEXT,&stFormatText))
	{
		CComPtr<IHTMLDocument>	pDoc;
		pDoc.Attach((IHTMLDocument*)m_pAttachBrowser->GetDocument());
		
		if(pDoc!=NULL)
		{
			CComPtr<IDispatch> spDispScript = NULL;
			HRESULT hr = pDoc->get_Script(&spDispScript);
			if(spDispScript)
			{
				DISPID dispid = -1;
				OLECHAR FAR* szMember = L"_DropText";
				hr = spDispScript->GetIDsOfNames(IID_NULL, &szMember, 1, LOCALE_SYSTEM_DEFAULT, &dispid);
				if(hr==S_OK)
				{
					m_dropEffectCurrent =  DROPEFFECT_COPY;
				}
			}
		}
	}
	
	return m_dropEffectCurrent;
}

DROPEFFECT CMcMessengerDropTarget::OnDragOver(CWnd* pWnd,COleDataObject* pDataObject,DWORD dwKeyState,CPoint point )
{
	return m_dropEffectCurrent;
}

BOOL CMcMessengerDropTarget::OnDrop(CWnd* pWnd,COleDataObject* pDataObject,DROPEFFECT dropEffect,CPoint point)
{
	USES_CONVERSION;

	FORMATETC stFormatHDROP = {CF_HDROP,NULL,DVASPECT_CONTENT,-1,TYMED_HGLOBAL};

	FORMATETC stFormatInetUrl = {_MC_CF_INETURL,NULL,DVASPECT_CONTENT,-1,TYMED_HGLOBAL};

	FORMATETC stFormatText = {CF_TEXT,NULL,DVASPECT_CONTENT,-1,TYMED_HGLOBAL};
	
	if(pDataObject->IsDataAvailable(CF_HDROP,&stFormatHDROP))
	{
		CComPtr<IHTMLDocument>	pDoc;
		pDoc.Attach((IHTMLDocument*)m_pAttachBrowser->GetDocument());
		
		if(pDoc!=NULL)
		{
			CComPtr<IDispatch> spDispScript = NULL;
			HRESULT hr = pDoc->get_Script(&spDispScript);
			if(spDispScript)
			{
				DISPID dispid = -1;
				OLECHAR FAR* szMember = L"_DropFile";
				hr = spDispScript->GetIDsOfNames(IID_NULL, &szMember, 1, LOCALE_SYSTEM_DEFAULT, &dispid);
				if(SUCCEEDED(hr))
				{
					HGLOBAL	hMem	=	pDataObject->GetGlobalData(CF_HDROP);
					HDROP   hDrop	=	(HDROP)GlobalLock(hMem);
					
					UINT nFileCount	=	DragQueryFile(hDrop,0xFFFFFFFF,0,0);

					if(nFileCount>30)
					{
						CString	strMessage;
						strMessage.LoadString(IDS_FILES_SEND_LIMIT);
						MessageBox(GetDesktopWindow(),strMessage,GetString(IDS_ERROR_TITLE),MB_OK|MB_ICONERROR);
						return TRUE;
					}
					
					if(nFileCount>0)
					{
						CComVariant	varFileMas;
						
						varFileMas.vt	=	VT_ARRAY|VT_VARIANT;
						
						SAFEARRAYBOUND rgsabound[1];
						rgsabound[0].cElements	=	2*nFileCount;
						rgsabound[0].lLbound	=	0;
						
						varFileMas.parray	= SafeArrayCreate(VT_VARIANT,1,(SAFEARRAYBOUND*)&rgsabound);
						
						for(UINT i=0;i<nFileCount;i++)
						{
							TCHAR  FileBuffer[MAX_PATH] = _T("");
							DragQueryFile(hDrop,i,FileBuffer,MAX_PATH);
							
							CComVariant varResult;
							
							LPWSTR pStrMimeType = NULL;
							hr = FindMimeFromData(NULL, T2W(FileBuffer), NULL, 0, NULL, 0, &pStrMimeType, 0);
							
							VARIANT	varFileName,	varFileMime;

							VariantInit(&varFileName);
							varFileName.vt		=	VT_BSTR;
							varFileName.bstrVal	=	T2BSTR(FileBuffer);

							VariantInit(&varFileMime);
							varFileMime.vt		=	VT_BSTR;
							varFileMime.bstrVal	=	W2BSTR(pStrMimeType);

							long	ix[1];
							ix[0]	=	2*i;
							SafeArrayPutElement(varFileMas.parray,ix,(LPVOID)&varFileName);
							ix[0]	=	2*i+1;
							SafeArrayPutElement(varFileMas.parray,ix,(LPVOID)&varFileMime);
						}

						CComVariant* pvars = new CComVariant[1];

						pvars[0] = varFileMas;
						
						CComVariant varResult;
						
						DISPPARAMS params = { pvars, NULL, 1, 0 };
						hr = spDispScript->Invoke(dispid, IID_NULL, LOCALE_USER_DEFAULT, DISPATCH_METHOD, &params, &varResult, NULL, NULL);
						if(SUCCEEDED(hr))
						{
							hr = varResult.ChangeType(VT_BSTR);
							if(SUCCEEDED(hr))
							{
								CComBSTR	bsUrl	=	varResult.bstrVal;
								if(bsUrl.Length())
									m_pMessenger->ShowWebWindow(W2CT(bsUrl),&varFileMas);
							}
						}
						
						delete []pvars;
					}
					GlobalUnlock(hMem);
				}
			}
		}
	}
	else if(pDataObject->IsDataAvailable(_MC_CF_INETURL,&stFormatInetUrl))
	{
		// Load _DropLink (URL) [4/13/2002]
		CComPtr<IHTMLDocument>	pDoc;
		pDoc.Attach((IHTMLDocument*)m_pAttachBrowser->GetDocument());

		if(pDoc!=NULL)
		{
			CComPtr<IDispatch> spDispScript = NULL;
			HRESULT hr = pDoc->get_Script(&spDispScript);
			if(spDispScript)
			{
				DISPID dispid = -1;
				OLECHAR FAR* szMember = L"_DropLink";
				hr = spDispScript->GetIDsOfNames(IID_NULL, &szMember, 1, LOCALE_SYSTEM_DEFAULT, &dispid);
				if(SUCCEEDED(hr))
				{
					CString strLinkDscr;
					// Get link Description [4/17/2002]
					HGLOBAL	hMemLinkDscr	=	pDataObject->GetGlobalData(_MC_CF_LINKDESCRIPTOR);

					if(hMemLinkDscr)
					{
						LPVOID  pData	=	GlobalLock(hMemLinkDscr);

						if(!IsBadReadPtr(pData,sizeof(FILEGROUPDESCRIPTOR)))
						{
							FILEGROUPDESCRIPTOR   *pLinkDescriptor	=	(FILEGROUPDESCRIPTOR*)pData;

							if(!IsBadReadPtr(pLinkDescriptor->fgd,sizeof(FILEDESCRIPTOR)))
							{
								strLinkDscr =  pLinkDescriptor->fgd[0].cFileName;
								if(!strLinkDscr.IsEmpty())
									strLinkDscr = strLinkDscr.Left(strLinkDscr.GetLength()-4);
							}
						}
						GlobalUnlock(hMemLinkDscr);
					}

					// Get link URL [4/17/2002]

					HGLOBAL	hMem	=	pDataObject->GetGlobalData(_MC_CF_INETURL);
					long UrlSize	=	GlobalSize(hMem);
					LPVOID   hUrl	=	GlobalLock(hMem);
					
					if(hUrl)
					{
						CString	strUrl	=	(LPTSTR)hUrl;
						
						CComVariant varResult;
						
						CComVariant* pvars = new CComVariant[2];
						
						pvars[0] = T2W(const_cast<LPTSTR>((LPCTSTR)strLinkDscr));
						pvars[1] = T2W(const_cast<LPTSTR>((LPCTSTR)strUrl));
						
						DISPPARAMS params = { pvars, NULL, 2, 0 };
						hr = spDispScript->Invoke(dispid, IID_NULL, LOCALE_USER_DEFAULT, DISPATCH_METHOD, &params, &varResult, NULL, NULL);
						if(SUCCEEDED(hr))
						{
							hr = varResult.ChangeType(VT_BSTR);
							if(SUCCEEDED(hr))
							{
								CComBSTR	bsUrl	=	varResult.bstrVal;
								if(bsUrl.Length())
									m_pMessenger->ShowWebWindow(W2CT(bsUrl));
							}
						}
						
						delete []pvars;
					}
					GlobalUnlock(hMem);
				}
			}
		}
	}
	else if(pDataObject->IsDataAvailable(CF_TEXT,&stFormatText))
	{
		CComPtr<IHTMLDocument>	pDoc;
		pDoc.Attach((IHTMLDocument*)m_pAttachBrowser->GetDocument());
		
		if(pDoc!=NULL)
		{
			CComPtr<IDispatch> spDispScript = NULL;
			HRESULT hr = pDoc->get_Script(&spDispScript);
			if(spDispScript)
			{
				DISPID dispid = -1;
				OLECHAR FAR* szMember = L"_DropText";
				hr = spDispScript->GetIDsOfNames(IID_NULL, &szMember, 1, LOCALE_SYSTEM_DEFAULT, &dispid);
				if(SUCCEEDED(hr))
				{
					HGLOBAL	hMem	=	pDataObject->GetGlobalData(CF_HDROP);
					LPCTSTR   hText	=	(LPCTSTR)GlobalLock(hMem);

					if(hText)
					{
						CComVariant* pvars = new CComVariant[1];
						
						pvars[0] = hText;
						
						CComVariant varResult;
						
						DISPPARAMS params = { pvars, NULL, 1, 0 };
						hr = spDispScript->Invoke(dispid, IID_NULL, LOCALE_USER_DEFAULT, DISPATCH_METHOD, &params, &varResult, NULL, NULL);
						if(SUCCEEDED(hr))
						{
							hr = varResult.ChangeType(VT_BSTR);
							if(SUCCEEDED(hr))
							{
								CComBSTR	bsUrl	=	varResult.bstrVal;
								if(bsUrl.Length())
									m_pMessenger->ShowWebWindow(W2CT(bsUrl));
							}
						}
						
						delete []pvars;
					}
					
					GlobalUnlock(hMem);
				}
			}
		}
	}
	
	
	return TRUE;
}

//
//CMcImpIDocHostUIHandler::CMcImpIDocHostUIHandler(CMcMessengerDropTarget* pTarget)
//{
//	m_pTarget = pTarget;
//	//m_pTarget->ExternalAddRef();
//    m_cRef = 0;
//}
//
//CMcImpIDocHostUIHandler::~CMcImpIDocHostUIHandler( void )
//{
//	//m_pTarget->ExternalRelease();
//}
//
//STDMETHODIMP CMcImpIDocHostUIHandler::QueryInterface( REFIID riid, void **ppvObject)
//{
//	*ppvObject = NULL;
//	
//	if (IsEqualGUID(riid, IID_IUnknown))
//		*ppvObject = static_cast<IDocHostUIHandler*>(this);
//	
//	if (IsEqualGUID(riid, IID_IDocHostUIHandler))
//		*ppvObject = static_cast<IDocHostUIHandler*>(this);
//	
//	if (*ppvObject)
//	{
//		((IUnknown*)*ppvObject)->AddRef();
//		return S_OK;
//	}
//	else return E_NOINTERFACE;
//}
//
//STDMETHODIMP_(ULONG) CMcImpIDocHostUIHandler::AddRef( void )
//{
//    return InterlockedIncrement(&m_cRef);
//}
//
//STDMETHODIMP_(ULONG) CMcImpIDocHostUIHandler::Release( void )
//{
//	if (InterlockedDecrement(&m_cRef) == 0)
//	{
//		delete this;
//		return 0;
//	}
//	return m_cRef;
//}
//
//
//
//// * CMcImpIDocHostUIHandler::GetHostInfo
//// *
//// * Purpose: Called at initialization
//// *
//STDMETHODIMP CMcImpIDocHostUIHandler::GetHostInfo( DOCHOSTUIINFO* pInfo )
//{
//    pInfo->dwFlags = DOCHOSTUIFLAG_NO3DBORDER;
//    pInfo->dwDoubleClick = DOCHOSTUIDBLCLK_DEFAULT;
//    return S_OK;
//}
//
//// * CMcImpIDocHostUIHandler::ShowUI
//STDMETHODIMP CMcImpIDocHostUIHandler::ShowUI(
//				DWORD dwID, 
//				IOleInPlaceActiveObject * /*pActiveObject*/,
//				IOleCommandTarget * pCommandTarget,
//				IOleInPlaceFrame * /*pFrame*/,
//				IOleInPlaceUIWindow * /*pDoc*/)
//{
//    return E_NOTIMPL;
//}
//
//// * CMcImpIDocHostUIHandler::HideUI
//STDMETHODIMP CMcImpIDocHostUIHandler::HideUI(void)
//{
//    return E_NOTIMPL;
//}
//
//// * CMcImpIDocHostUIHandler::UpdateUI
//STDMETHODIMP CMcImpIDocHostUIHandler::UpdateUI(void)
//{
//	return E_NOTIMPL;
//}
//
//STDMETHODIMP CMcImpIDocHostUIHandler::EnableModeless(BOOL /*fEnable*/)
//{
//    return E_NOTIMPL;
//}
//
//// * CMcImpIDocHostUIHandler::OnDocWindowActivate
//// *
//// * Purpose: Called from IOleInPlaceActiveObject::OnDocWindowActivate
//// *
//STDMETHODIMP CMcImpIDocHostUIHandler::OnDocWindowActivate(BOOL /*fActivate*/)
//{
//    return E_NOTIMPL;
//}
//
//// * CMcImpIDocHostUIHandler::OnFrameWindowActivate
//// *
//// * Purpose: Called from IOleInPlaceActiveObject::OnFrameWindowActivate
//// *
//STDMETHODIMP CMcImpIDocHostUIHandler::OnFrameWindowActivate(BOOL /*fActivate*/)
//{
//    return E_NOTIMPL;
//}
//
//// * CMcImpIDocHostUIHandler::ResizeBorder
//// *
//// * Purpose: Called from IOleInPlaceActiveObject::ResizeBorder
//// *
//STDMETHODIMP CMcImpIDocHostUIHandler::ResizeBorder(
//				LPCRECT /*prcBorder*/, 
//				IOleInPlaceUIWindow* /*pUIWindow*/,
//				BOOL /*fRameWindow*/)
//{
//    return E_NOTIMPL;
//}
//
//// * CMcImpIDocHostUIHandler::ShowContextMenu
//// *
//// * Purpose: Called when it would normally display its context menu
//// *
//STDMETHODIMP CMcImpIDocHostUIHandler::ShowContextMenu(
//													  DWORD dwID, 
//													  POINT* pptPosition,
//													  IUnknown* pCommandTarget,
//													  IDispatch* pDispatchObjectHit)
//{
//    return E_NOTIMPL;
//}
//
//// * CMcImpIDocHostUIHandler::TranslateAccelerator
//// *
//// * Purpose: Called from TranslateAccelerator routines
//// *
//STDMETHODIMP CMcImpIDocHostUIHandler::TranslateAccelerator(LPMSG lpMsg,
//														   /* [in] */ const GUID __RPC_FAR *pguidCmdGroup,
//														   /* [in] */ DWORD nCmdID)
//{
//    return E_NOTIMPL;
//}
//
//// * CMcImpIDocHostUIHandler::GetOptionKeyPath
//// *
//// * Purpose: Called to find where the host wishes to store 
//// *	its options in the registry
//// *
//STDMETHODIMP CMcImpIDocHostUIHandler::GetOptionKeyPath(BSTR* pbstrKey, DWORD)
//{
//	return E_NOTIMPL;
//}
//
//STDMETHODIMP CMcImpIDocHostUIHandler::GetDropTarget( 
//            /* [in] */ IDropTarget __RPC_FAR *pDropTarget,
//            /* [out] */ IDropTarget __RPC_FAR *__RPC_FAR *ppDropTarget)
//{
//	*ppDropTarget = (IDropTarget*)m_pTarget->GetInterface(&IID_IDropTarget);
//	if(*ppDropTarget)
//	{
//		m_pTarget->ExternalAddRef();
//	}
//	//CString strAdress;
//	//strAdress.Format(_T("%d"),*ppDropTarget);
//	//MessageBox(NULL,_T("CMcImpIDocHostUIHandler::GetDropTarget"),strAdress,0);
//    return S_OK;
//}
//
//STDMETHODIMP CMcImpIDocHostUIHandler::GetExternal( 
//    /* [out] */ IDispatch __RPC_FAR *__RPC_FAR *ppDispatch)
//{
//    return E_NOTIMPL;
//}
//
//STDMETHODIMP CMcImpIDocHostUIHandler::TranslateUrl( 
//    /* [in] */ DWORD dwTranslate,
//    /* [in] */ OLECHAR __RPC_FAR *pchURLIn,
//    /* [out] */ OLECHAR __RPC_FAR *__RPC_FAR *ppchURLOut)
//{
//    return E_NOTIMPL;
//}
//
//STDMETHODIMP CMcImpIDocHostUIHandler::FilterDataObject( 
//    /* [in] */ IDataObject __RPC_FAR *pDO,
//    /* [out] */ IDataObject __RPC_FAR *__RPC_FAR *ppDORet)
//{
//    return E_NOTIMPL;
//}
