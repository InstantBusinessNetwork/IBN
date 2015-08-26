// OfsDhtmlCtrl.cpp: implementation of the COfsDhtmlEditCtrl class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "OfsDhtmlCtrl.h"
#include <AFXPRIV.H>
#include <afxdisp.h>
#include <TRIEDIID.h>
#include <triedcid.h>
//#include <atlbase.h>

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

COfsDhtmlEditCtrl::COfsDhtmlEditCtrl( /* NULL*/)
{
	m_cRef                = 0UL;
	m_tagCount            = 0;
	m_pIOleIPActiveObject = NULL;
	m_hWndObj             = NULL;
	m_pSite               = NULL;
	dwInfoMessage         = 0L;
	pContextMenu          = NULL;
    pNotifyParent         = NULL;
	MenuItem              = 0;
	m_bsDefaultFontName	  = L"Arial";
	m_lDefaultFontSize	  = 10;	
}

COfsDhtmlEditCtrl::~COfsDhtmlEditCtrl()
{
	if(m_pSite!=NULL)
	{
		m_pSite->Close(FALSE);
		ReleaseInterface(m_pSite);
		m_pSite = NULL;
	}
	delete pContextMenu;
}

IMPLEMENT_DYNCREATE(COfsDhtmlEditCtrl, CWnd)

BEGIN_MESSAGE_MAP(COfsDhtmlEditCtrl, CWnd)
	ON_WM_SIZE()
	ON_WM_SETFOCUS()
//	ON_WM_ERASEBKGND()
	ON_WM_CREATE()
//	ON_WM_PAINT()
END_MESSAGE_MAP()

STDMETHODIMP COfsDhtmlEditCtrl::QueryInterface(REFIID riid, void **ppv)
{
	AFX_MANAGE_STATE(this->m_pModuleState);
    /*
     * We provide IOleInPlaceFrame and IOleCommandTarget
	 *   interfaces here for the ActiveX Document hosting
	 */
    *ppv = NULL;

    if ( IID_IUnknown == riid || IID_IOleInPlaceUIWindow == riid
        || IID_IOleWindow == riid || IID_IOleInPlaceFrame == riid )
	{
        *ppv = (IOleInPlaceFrame *)this;
	}

	if ( IID_IOleCommandTarget == riid )
	{
        *ppv = (IOleCommandTarget *)this;
	}

    if ( NULL != *ppv )
    {
        ((LPUNKNOWN)*ppv)->AddRef();
        return NOERROR;
    }

    return E_NOINTERFACE;//E_NOINTERFACE;
}

STDMETHODIMP_(ULONG) COfsDhtmlEditCtrl::AddRef(void)
{
	AFX_MANAGE_STATE(this->m_pModuleState);
    return ++m_cRef;
}

STDMETHODIMP_(ULONG) COfsDhtmlEditCtrl::Release(void)
{
	AFX_MANAGE_STATE(this->m_pModuleState);
    //Nothing special happening here-- life if user-controlled.
	// Debug check to see we don't fall below 0
	ASSERT( m_cRef != 0 );
    return --m_cRef;
}

//IOleInPlaceFrame implementation
STDMETHODIMP  COfsDhtmlEditCtrl::GetWindow(HWND *phWnd)
{
	phWnd = &(this->m_hWnd);
	return NOERROR;
}

STDMETHODIMP  COfsDhtmlEditCtrl::ContextSensitiveHelp(BOOL fEnterMode)
{
	return NOERROR;
}

STDMETHODIMP  COfsDhtmlEditCtrl::GetBorder(LPRECT prcBorder)
{
    GetClientRect( prcBorder );
    return NOERROR;
}

STDMETHODIMP  COfsDhtmlEditCtrl::RequestBorderSpace(LPCBORDERWIDTHS pBW)
{
    // We have no border space restrictions
    return NOERROR;
}

STDMETHODIMP  COfsDhtmlEditCtrl::SetBorderSpace(LPCBORDERWIDTHS pBW)
{
	return NOERROR;
}

STDMETHODIMP  COfsDhtmlEditCtrl::SetActiveObject(LPOLEINPLACEACTIVEOBJECT pIIPActiveObj, 
											 LPCOLESTR pszObj)
{
    if ( NULL != m_pIOleIPActiveObject )
	{
        m_pIOleIPActiveObject->Release();
	}

    //NULLs m_pIOleIPActiveObject if pIIPActiveObj is NULL
    m_pIOleIPActiveObject = pIIPActiveObj;

    if ( NULL != m_pIOleIPActiveObject )
	{
        m_pIOleIPActiveObject->AddRef();
		m_pIOleIPActiveObject->GetWindow( &m_hWndObj );
	}
    return NOERROR;
}

STDMETHODIMP  COfsDhtmlEditCtrl::InsertMenus(HMENU hMenu, LPOLEMENUGROUPWIDTHS pMGW)
{
	return E_NOTIMPL;
}

STDMETHODIMP  COfsDhtmlEditCtrl::SetMenu(HMENU hMenu, HOLEMENU hOLEMenu, HWND hWndObj)
{
	return E_NOTIMPL;
}

STDMETHODIMP  COfsDhtmlEditCtrl::RemoveMenus(HMENU hMenu)
{
	return E_NOTIMPL;
}

STDMETHODIMP  COfsDhtmlEditCtrl::SetStatusText(LPCOLESTR pszText)
{
	return NOERROR;
}

STDMETHODIMP  COfsDhtmlEditCtrl::EnableModeless(BOOL fEnable)
{
	return NOERROR;
}

STDMETHODIMP  COfsDhtmlEditCtrl::TranslateAccelerator(LPMSG pMSG, WORD wID)
{
	return S_FALSE;
}
	
//IOleCommandTarget
STDMETHODIMP COfsDhtmlEditCtrl::QueryStatus(const GUID *pguidCmdGroup, ULONG cCmds
	, OLECMD prgCmds[], OLECMDTEXT *pCmdText)
{
    if ( pguidCmdGroup != NULL )
	{
		// It's a nonstandard group!!
        return OLECMDERR_E_UNKNOWNGROUP;
	}

    MSOCMD*     pCmd;
    INT         c;
    HRESULT     hr = S_OK;

    // By default command text is NOT SUPPORTED.
    if ( pCmdText && ( pCmdText->cmdtextf != OLECMDTEXTF_NONE ) )
	{
        pCmdText->cwActual = 0;
	}

    // Loop through each command in the ary, setting the status of each.
    for ( pCmd = prgCmds, c = cCmds; --c >= 0; pCmd++ )
    {
        // By default command status is NOT SUPPORTED.
        pCmd->cmdf = 0;

        switch ( pCmd->cmdID )
        {
			case OLECMDID_SETPROGRESSTEXT:
			case OLECMDID_SETTITLE:
				pCmd->cmdf = OLECMDF_SUPPORTED;
				break;
			case OLECMDID_NEW:
			case OLECMDID_OPEN:
			case OLECMDID_SAVE:
				pCmd->cmdf = (MSOCMDF_SUPPORTED | MSOCMDF_ENABLED);
				break;
        }
    }

    return (hr);
}
//-------------------------------------------------------------------------------
// Name: Exec
// Desc: Обрабатывает Стандыртные События для Exec'a...
//-------------------------------------------------------------------------------
STDMETHODIMP COfsDhtmlEditCtrl::Exec(const GUID *pguidCmdGroup, DWORD nCmdID
	, DWORD nCmdexecopt, VARIANTARG *pvaIn, VARIANTARG *pvaOut)
{
    HRESULT hr = S_OK;

    if ( pguidCmdGroup == NULL )
    {
		USES_CONVERSION;

        switch (nCmdID)
        {
			case OLECMDID_SAVE:
				// We don't support any save stuff!
				hr = OLECMDERR_E_NOTSUPPORTED;
				break;

			case OLECMDID_SETPROGRESSTEXT:
				if ( pvaIn && V_VT(pvaIn) == VT_BSTR )
				{
//					CFrameWnd* pFrame = GetTopLevelFrame();
//					if ( pFrame != NULL )
//					{
//						pFrame->SetMessageText( OLE2T(V_BSTR(pvaIn)) );
//					}
				}
				else
				{
					hr = OLECMDERR_E_NOTSUPPORTED;
				}
				break;

			case OLECMDID_UPDATECOMMANDS:
				// MFC updates all other commands in it's idle so we don't bother forcing the update here
				{
					///////////////////////////////////////////// 
					/// Посылать Сообщение предку на Обновление 
					///////////////////////////////////////////// 
					if(dwInfoMessage)
						GetParent()->SendMessage(dwInfoMessage);
					//SetFontState();
				}
				break;

			case OLECMDID_SETTITLE:
				if (pvaIn && V_VT(pvaIn) == VT_BSTR)
				{
				//	CCEditDoc* pDoc = GetDocument();
				//	ASSERT_VALID(pDoc);

				//	pDoc->SetTitle(OLE2T(V_BSTR(pvaIn)));
				}
				else
				{
					hr = OLECMDERR_E_NOTSUPPORTED;
				}
				break;

			default:
				hr = OLECMDERR_E_NOTSUPPORTED;
				break;
        }
    }
    else
    {
        hr = OLECMDERR_E_UNKNOWNGROUP;
    }
    return (hr);
}

//-------------------------------------------------------------------------------
// Name: PreTranslateMessage
// Desc: перехватывает нажатие ESC и перенаправлет его Родителю, это сделано что 
// бы главное окно закрывалось по ESC.
//-------------------------------------------------------------------------------
BOOL COfsDhtmlEditCtrl::PreTranslateMessage(MSG* pMsg) 
{
    if ( NULL != m_pIOleIPActiveObject )
    {
        HRESULT     hr = S_OK;
		if(pMsg->message == WM_KEYDOWN&&pMsg->wParam == VK_ESCAPE)
    	{
			GetParent()->PostMessage(WM_KEYDOWN,VK_ESCAPE,pMsg->lParam);
		}
		hr = m_pIOleIPActiveObject->TranslateAccelerator( pMsg );
		
        //If the object translated the accelerator, we're done
        if ( NOERROR == hr )
		{
            return TRUE;
		}
    }
	return CWnd::PreTranslateMessage(pMsg);
}

//-------------------------------------------------------------------------------
// Name: SetFontState
// Desc: Не используется можно убить.
//-------------------------------------------------------------------------------
void COfsDhtmlEditCtrl::SetFontState()
{

/*	VARIANT va;
	DWORD dwFontNameStatus = 0;
	DWORD dwFontSizeStatus = 0;
//	CComboBox* pFontNameBox = &(GetMainFrame()->m_wndStyleBar.m_fontNameCombo);
//	CComboBox* pFontSizeBox = &(GetMainFrame()->m_wndStyleBar.m_fontSizeCombo);

//	ASSERT(pFontNameBox);
//	ASSERT(pFontSizeBox);

	dwFontNameStatus = GetCommandStatus( IDM_TRIED_FONTNAME );
	dwFontSizeStatus = GetCommandStatus( IDM_TRIED_FONTSIZE );

//	pFontNameBox->EnableWindow(dwFontNameStatus & OLECMDF_ENABLED);
//	pFontSizeBox->EnableWindow(dwFontSizeStatus & OLECMDF_ENABLED);

	if (dwFontNameStatus & OLECMDF_NINCHED)
	{
		//pFontNameBox->SetCurSel(-1);
	}
	else if (dwFontNameStatus & OLECMDF_ENABLED)
	{

		VariantInit(&va);
		CString fontName;

		HrExecCommand(IDM_TRIED_FONTNAME, NULL, &va, FALSE);
		fontName = va.bstrVal;

//		for (int i=0; i < pFontNameBox->GetCount(); ++i)
		{
			CString itemStr;

			//pFontNameBox->GetLBText(i, itemStr);

			if (itemStr && itemStr == fontName)
			{
				//pFontNameBox->SetCurSel(i);
//				break;
			}
		}
	}
	else
	{
		//pFontNameBox->SetCurSel(-1);
	}

	if (dwFontSizeStatus & OLECMDF_NINCHED)
	{
		//pFontSizeBox->SetCurSel(-1);
	}
	else if (dwFontSizeStatus & OLECMDF_ENABLED)
	{

		VariantInit(&va);

		HrExecCommand(IDM_TRIED_FONTSIZE, NULL, &va, FALSE);

		if (va.vt == VT_I4 && va.lVal >= 1)
		{
		//	pFontSizeBox->SetCurSel(va.lVal-1);
		}
	}
	else
	{
		//pFontSizeBox->SetCurSel(-1);
	}
*/
}

//-------------------------------------------------------------------------------
// Name: GetCommandStatus
// Desc: Запросить состояние команды по ее ID
// За дополнительной информацией смотрие MSDN DhtmlEdit.
//-------------------------------------------------------------------------------
DWORD COfsDhtmlEditCtrl::GetCommandStatus( ULONG ucmdID )
{
	DWORD dwReturn = 0;
	if ( m_pSite != NULL )
	{
		LPOLECOMMANDTARGET pCommandTarget = m_pSite->GetCommandTarget();
		if ( pCommandTarget != NULL )
		{
			HRESULT hr = S_OK;
			MSOCMD msocmd;
			msocmd.cmdID = ucmdID;
			msocmd.cmdf  = 0;

			hr = pCommandTarget->QueryStatus(&GUID_TriEditCommandGroup, 1, &msocmd, NULL);
			
			dwReturn = msocmd.cmdf;
		}
	}
	return dwReturn;
}

//-------------------------------------------------------------------------------
// Name: HrExecDefault
// Desc: Выполнить команду с параметром OLECMDEXECOPT_DODEFAULT.
//-------------------------------------------------------------------------------
HRESULT COfsDhtmlEditCtrl::HrExecDefault(ULONG ucmdID,VARIANT* pVarIn)
{
	HRESULT hr = S_OK;

	if ( m_pSite != NULL )
	{
		LPOLECOMMANDTARGET pCommandTarget = m_pSite->GetCommandTarget();

		if ( pCommandTarget != NULL )
		{
			hr = pCommandTarget->Exec(&GUID_TriEditCommandGroup,
						ucmdID,
						OLECMDEXECOPT_DODEFAULT,
						pVarIn,
						NULL);
		}
	}

	return hr;
}

//-------------------------------------------------------------------------------
// Name: HrExecCommand
// Desc: Выполнить команду с параметром MSOCMDEXECOPT_PROMPTUSER или MSOCMDEXECOPT_DONTPROMPTUSER.
//-------------------------------------------------------------------------------
HRESULT COfsDhtmlEditCtrl::HrExecCommand( ULONG ucmdID, VARIANT* pVarIn, VARIANT* pVarOut, BOOL bPromptUser )
{
	HRESULT hr = S_OK;

	if ( m_pSite != NULL )
	{
		LPOLECOMMANDTARGET pCommandTarget = m_pSite->GetCommandTarget();

		if ( pCommandTarget != NULL )
		{
			DWORD dwCmdOpt = 0;

			if (bPromptUser)
				dwCmdOpt = MSOCMDEXECOPT_PROMPTUSER;
			else
				dwCmdOpt = MSOCMDEXECOPT_DONTPROMPTUSER;

			hr = pCommandTarget->Exec(&GUID_TriEditCommandGroup,
						ucmdID,
						dwCmdOpt,
						pVarIn,
						pVarOut);
		}
	}

	return hr;
}

//-------------------------------------------------------------------------------
// Name: HrGetBody
// Desc: Получит тело Dockumeta если он создан ...
//-------------------------------------------------------------------------------
HRESULT COfsDhtmlEditCtrl::HrGetBody(IHTMLBodyElement** ppBody)
{
	HRESULT hr = E_FAIL;
	IHTMLDocument2* pDoc = NULL;

	*ppBody = NULL;

	if (SUCCEEDED(HrGetDoc(&pDoc)))
	{
		IHTMLElement* pElement = NULL;

		hr = pDoc->get_body(&pElement);

		if (SUCCEEDED(hr))
		{
			IHTMLBodyElement* pBody = NULL;
			hr = pElement->QueryInterface(IID_IHTMLBodyElement, (void **) &pBody);

			if (SUCCEEDED(hr))
			{
				*ppBody = pBody;

				// don't release body - we are returning it
			}
			
			pElement->Release();
		}

		pDoc->Release();
	}

	return hr;
}

//-------------------------------------------------------------------------------
// Name: HrGetDoc
// Desc: Получить Документ
//-------------------------------------------------------------------------------
HRESULT COfsDhtmlEditCtrl::HrGetDoc(IHTMLDocument2 **ppDoc)
{
	HRESULT         hr = E_FAIL;
	IUnknown* lpUnk = m_pSite->GetObjectUnknown();

	if ( lpUnk != NULL )
	{
		// Request the "document" object from the MSHTML
		hr = lpUnk->QueryInterface(IID_IHTMLDocument2, (void **)ppDoc);
	}

	return hr;
}

//-------------------------------------------------------------------------------
// Name: HrGetElementFromSelection
// Desc: Запросить элемент из выделенного
//-------------------------------------------------------------------------------
HRESULT COfsDhtmlEditCtrl::HrGetElementFromSelection(IHTMLElement **ppElement)
{
    IHTMLTxtRange           *pRange=0;
	IHTMLElement			*pElement=0;
    IDispatch               *pID=0;
    HRESULT                 hr=E_FAIL;

    if (ppElement== NULL)
        return E_INVALIDARG;


	if (SUCCEEDED(HrGetRangeFromSelection(&pRange)))
	{
		hr = pRange->parentElement(ppElement);

		pRange->Release();
	}
    
	return hr;
}

//-------------------------------------------------------------------------------
// Name: HrGetRangeFromSelection
// Desc: Запросит Ранг Выделенного
//-------------------------------------------------------------------------------
HRESULT COfsDhtmlEditCtrl::HrGetRangeFromSelection(IHTMLTxtRange **ppRange)
{
    IHTMLSelectionObject    *pSel=0;
    IHTMLTxtRange           *pTxtRange=0;
    IDispatch               *pID=0;
    HRESULT                 hr=E_FAIL;

    if (ppRange == NULL)
        return E_INVALIDARG;

    *ppRange = NULL;

	IHTMLDocument2* pDoc = NULL;

	if (SUCCEEDED(HrGetDoc(&pDoc)))
	{

		if(pDoc)
		{
			pDoc->get_selection(&pSel);
			if (pSel)
			{
				pSel->createRange(&pID);
				if (pID)
				{
					hr = pID->QueryInterface(IID_IHTMLTxtRange, (LPVOID *)ppRange);
					pID->Release();
				}
				pSel->Release();
			}

			pDoc->Release();
		}

	}
	return hr;
}

//-------------------------------------------------------------------------------
// Name: HrGetTags
// Desc: Запросит Тэги ... Не используется
//-------------------------------------------------------------------------------
HRESULT COfsDhtmlEditCtrl::HrGetTags()
{
    HRESULT	hr = S_OK;
/*    VARIANTARG varRange;
    LONG lLBound, lUBound, lIndex;
    BSTR  bstr=0;
    SAFEARRAY * psa = NULL;
	int i = 0;

	VariantInit(&varRange);

    varRange.vt = VT_ARRAY;
    varRange.parray = NULL;

	hr = HrExecCommand(IDM_TRIED_GETBLOCKFMTS, NULL, &varRange, FALSE);

    if(FAILED(hr))
        goto error;

    psa = V_ARRAY(&varRange);
    SafeArrayGetLBound(psa, 1, &lLBound);
    SafeArrayGetUBound(psa, 1, &lUBound);

    for (i=0, lIndex=lLBound; lIndex<=lUBound; lIndex++, i++)
    {
        SafeArrayGetElement(psa, &lIndex, &bstr);

		CString str(bstr);
		m_tags[i] = str;

        if(bstr)
        {
            SysFreeString(bstr);
            bstr = NULL;
        }
    }
	
	m_tagCount = i;

error:
    if(bstr) 
		SysFreeString(bstr);

	VariantClear(&varRange);

    if(psa)
        SafeArrayDestroy(psa);*/

    return hr;
}

//-------------------------------------------------------------------------------
// Name: SetOptions
// Desc: Не используется
//-------------------------------------------------------------------------------
void COfsDhtmlEditCtrl::SetOptions()
{
/*	HRESULT hr = S_OK;
	VARIANT varIn;

	VariantInit(&varIn);
	
	// ui activation options
	V_VT(&varIn) = VT_BOOL;
//	V_BOOL(&varIn) = (((CCEditApp*) AfxGetApp())->GetOptions().GetActivateDTCs()) ? VARIANT_TRUE : VARIANT_FALSE;
	hr = HrExecCommand(IDM_TRIED_ACTIVATEDTCS, &varIn, NULL, FALSE);

	V_VT(&varIn) = VT_BOOL;
//	V_BOOL(&varIn) = (((CCEditApp*) AfxGetApp())->GetOptions().GetActivateAXCtls()) ? VARIANT_TRUE : VARIANT_FALSE;
	hr = HrExecCommand(IDM_TRIED_ACTIVATEACTIVEXCONTROLS, &varIn, NULL, FALSE);

	V_VT(&varIn) = VT_BOOL;
//	V_BOOL(&varIn) = (((CCEditApp*) AfxGetApp())->GetOptions().GetActivateJavaApplets()) ? VARIANT_TRUE : VARIANT_FALSE;
	hr = HrExecCommand(IDM_TRIED_ACTIVATEAPPLETS, &varIn, NULL, FALSE);

	// glyph display options

	VariantInit(&varIn);

	V_VT(&varIn) = VT_BOOL;
//	V_BOOL(&varIn) = (((CCEditApp*) AfxGetApp())->GetOptions().GetShowAllTags()) ? VARIANT_TRUE : VARIANT_FALSE;
	hr = HrExecCommand(IDM_TRIED_SHOWDETAILS, &varIn, NULL, FALSE);

	// misc options
	VariantInit(&varIn);

	V_VT(&varIn) = VT_BOOL;
//	V_BOOL(&varIn) = (((CCEditApp*) AfxGetApp())->GetOptions().GetShowBorders()) ? VARIANT_TRUE : VARIANT_FALSE;
	hr = HrExecCommand(IDM_TRIED_SHOWBORDERS, &varIn, NULL, FALSE);
*/
}

//-------------------------------------------------------------------------------
// Name: OnCreate
// Desc: Создание Объекта
//-------------------------------------------------------------------------------
int COfsDhtmlEditCtrl::OnCreate( LPCREATESTRUCT lpCreateStruct )
{
	if (CWnd::OnCreate(lpCreateStruct) == -1) return -1;

	if ( m_pSite == NULL )
	{
     	m_pSite = new CSite(this->m_hWnd,this);
	    m_pSite->AddRef();

	    if(!m_pSite->Create(_T("")))
			ASSERT(FALSE);

		m_pSite->Activate( OLEIVERB_SHOW );

		m_bFilterEnable = FALSE;
	}

	return 1;
}

//DEL void COfsDhtmlEditCtrl::OnPaint()
//DEL {
//DEL 	CWnd::OnPaint();
//DEL 	// If this is the first time draw get's called then
//DEL 	//  let's create the MSHTML Active Document
//DEL }

//-------------------------------------------------------------------------------
// Name: OnSize
// Desc: 
//-------------------------------------------------------------------------------
void COfsDhtmlEditCtrl::OnSize(UINT nType, int cx, int cy)
{
	CWnd::OnSize(nType, cx, cy);
	
	//Tell the site to tell the object.
	if ( NULL != m_pSite )
	{
    	m_pSite->UpdateObjectRects();
	}
}

//-------------------------------------------------------------------------------
// Name: OnSetFocus
// Desc: 
//-------------------------------------------------------------------------------
void COfsDhtmlEditCtrl::OnSetFocus(CWnd* pOldWnd)
{
	CWnd::OnSetFocus(pOldWnd);
	
	// Give the focus to the ActiveX Document window
    if ( m_hWndObj != NULL )
	{
		::SetFocus(m_hWndObj );
	}
}

//-------------------------------------------------------------------------------
// Name: OnEraseBkgnd
// Desc: 
//-------------------------------------------------------------------------------
BOOL COfsDhtmlEditCtrl::OnEraseBkgnd(CDC* pDC)
{
	return TRUE;
}

//-------------------------------------------------------------------------------
// Name: SetBgColor
// Desc: Установит цвет фона ...
//-------------------------------------------------------------------------------
void COfsDhtmlEditCtrl::SetBgColor(COLORREF color)
{

	IUnknown* lpUnk = m_pSite->GetObjectUnknown();

	if ( lpUnk != NULL )
	{
	    HRESULT  hr = S_OK;
		// Then, request the "document" object from the object
		IHTMLDocument2* pHTMLDocument2;
		hr = lpUnk->QueryInterface(IID_IHTMLDocument2, (void **)&pHTMLDocument2);
		if ( SUCCEEDED( hr ) )
		{
			// translate the hex color into a string without using the CRT
			int idx;
			WCHAR			buff[8]=L"#000000";
			WCHAR           HEX[17]=L"0123456789ABCDEF";

			for (int i=0;i<3;i++)
			{
				idx = color&0xF;
				buff[2*i+2] = HEX[idx];
				color >>= 4;
				idx = color&0xF;
				buff[2*i+1] = HEX[idx];
				color >>= 4;
			}

			BSTR bstrColor = SysAllocString( buff );

			// Now, set the background color of the body to be equal to something
			VARIANT varColor;
			varColor.vt = VT_BSTR;
			varColor.bstrVal = bstrColor;
			hr = pHTMLDocument2->put_bgColor( varColor );
			pHTMLDocument2->Release();
		}
	}
	//SetFocus();
}

//-------------------------------------------------------------------------------
// Name: SetColor
// Desc: Установит Текущий Цвет
//-------------------------------------------------------------------------------
void COfsDhtmlEditCtrl::SetColor(COLORREF color)
{
	VARIANT va;
	VariantInit(&va);
	
	va.vt = VT_I4;
	va.lVal= color;
	
	if(SUCCEEDED(HrExecCommand(IDM_TRIED_FORECOLOR, &va, NULL, FALSE)))
	{
		//CString strColor;
		//strColor.Format(_T("%d,%d,%d"),color&0xFFFF00,(color>>15)&0xFFFF00,(color>>31)&0xFFFF00);
		//RegSaveString(_T("Settings"),_T("Text Color"),strColor);
	}
}

//-------------------------------------------------------------------------------
// Name: SetTextSize
// Desc: Установит Размер Шрифта от 0 до 7.
//-------------------------------------------------------------------------------
void COfsDhtmlEditCtrl::SetTextSize(int FontSize)
{
	VARIANT va;
	VariantInit(&va);

	va.vt = VT_I4;
	va.lVal = FontSize;  //[0,7]

	if(SUCCEEDED(HrExecCommand(IDM_TRIED_FONTSIZE, &va, NULL, FALSE)))
	{
		//BYTE bDefaultScript = 6;
		//RegSaveBin(_T("International\\Scripts"),_T("Default_Script"),&bDefaultScript,sizeof(BYTE));
		//RegSaveBin(_T("International\\Scripts\\6"),_T("IEFontSize"),(const BYTE*)&FontSize,sizeof(int));
	}
	//SetFocus();
}


//-------------------------------------------------------------------------------
// Name: SetEditMode
// Desc: перевести в режим редактирования
//-------------------------------------------------------------------------------
void COfsDhtmlEditCtrl::SetEditMode(BOOL bEditMode)
{
	if(bEditMode)
	{
		//LPTSTR Text = GetHTML();
		HrExecDefault(IDM_TRIED_EDITMODE, NULL);
		//SetHTML(Text);
		//delete [] Text;
		//Sleep(10);
	}
	else
	{
		//LPTSTR Text = GetHTML();
		HrExecDefault(IDM_TRIED_BROWSEMODE, NULL);
		//SetHTML(Text);
		//delete [] Text;
		//Sleep(10);
	}
	
	//SetFocus();
}

//-------------------------------------------------------------------------------
// Name: SetViewMode
// Desc: перевести в режим просмотра
//-------------------------------------------------------------------------------
void COfsDhtmlEditCtrl::SetViewMode(BOOL bViewMode)
{
	SetEditMode(!bViewMode);
}

//-------------------------------------------------------------------------------
// Name: GetHTML
// Desc: Запросить HTML
//-------------------------------------------------------------------------------
HRESULT COfsDhtmlEditCtrl::GetHTML(BSTR* pStr)
{
	CComPtr<IHTMLDocument2>			pHTMLDocument2	=	NULL;
	CComPtr<IUnknown>				pUnk			=	m_pSite->GetObjectUnknown();

	HRESULT hr = pUnk->QueryInterface(IID_IHTMLDocument2,(void**)&pHTMLDocument2);

	if(SUCCEEDED(hr))
	{
		CComPtr<IHTMLElement> pBody;

		HRESULT hr = pHTMLDocument2->get_body(&pBody);
		if(SUCCEEDED(hr))
		{
			CComBSTR bsOutput;

			hr = pBody->get_innerHTML(&bsOutput);

			/*if(SUCCEEDED(hr))
			{
				USES_CONVERSION;

				if(m_bsDefaultFontName!=L"Arial"||
					m_lDefaultFontSize!=10)
				{
					WCHAR *Buffer	= new WCHAR[bsOutput.Length()+256];
					
					swprintf(Buffer,L"<FONT face=\"%s\" size=\"%d\">%s</FONT>",m_bsDefaultFontName,m_lDefaultFontSize,bsOutput);
					bsOutput = Buffer;

					delete[] Buffer;
				}
			}*/

			hr = bsOutput.CopyTo(pStr);
		}
	}

	return hr; 
}

//-------------------------------------------------------------------------------
// Name: SetHTML
// Desc: Установит HTML
//-------------------------------------------------------------------------------
HRESULT COfsDhtmlEditCtrl::SetHTML(BSTR str)
{
	CComPtr<IPersistStreamInit>		pPersistStream	=	NULL;

	CComPtr<IUnknown>				pUnk			=	m_pSite->GetObjectUnknown();

	if(pUnk==NULL)
		return E_FAIL;

	HRESULT hr = pUnk->QueryInterface(IID_IPersistStreamInit,(void**)&pPersistStream);
	if(SUCCEEDED(hr))
	{
		hr = pPersistStream->InitNew();

		HGLOBAL hMem = NULL;

		ULONG StrRealSize = (SysStringLen(str)+1)*2;
		hMem = GlobalAlloc(GPTR, StrRealSize+2);
		LPBYTE pBuf = (LPBYTE)GlobalLock(hMem);
		//////////////////////////////////////////////////////////////////////////
		// !!! CAUTION [6/18/2002]
		// Обязательно добавлять, перед Unicode - данными, 
		// иначе IE не правильно будет определять, не английские Unicode символы. 
		// !!! CAUTION [6/18/2002]
		//////////////////////////////////////////////////////////////////////////
		pBuf[0]=0xFF;
		pBuf[1]=0xFE;
		//////////////////////////////////////////////////////////////////////////
		memcpy((LPVOID)(pBuf+2),(void*)str,StrRealSize);
		//////////////////////////////////////////////////////////////////////////
		GlobalUnlock(hMem);
			
		CComPtr<IStream> pDataStream = NULL;
		CreateStreamOnHGlobal(hMem,TRUE,&pDataStream);
		hr = pPersistStream->Load(pDataStream);
	}
	
	return hr;
}

//-------------------------------------------------------------------------------
// Name: SetBold
// Desc: 
//-------------------------------------------------------------------------------
void COfsDhtmlEditCtrl::SetBold()
{
	HrExecDefault(IDM_TRIED_BOLD,NULL);
	//SetFocus();
}

//-------------------------------------------------------------------------------
// Name: SetItalic
// Desc: 
//-------------------------------------------------------------------------------
void COfsDhtmlEditCtrl::SetItalic()
{
	HrExecDefault(IDM_TRIED_ITALIC ,NULL);
	//SetFocus();
}

//-------------------------------------------------------------------------------
// Name: SetUnderline
// Desc: 
//-------------------------------------------------------------------------------
void COfsDhtmlEditCtrl::SetUnderline()
{
	HrExecDefault(IDM_TRIED_UNDERLINE,NULL);
	//SetFocus();
}

CComBSTR COfsDhtmlEditCtrl::GetAbsoluteFontSizeName(int value)
{
	if(value<=1)
		return CComBSTR(L"xx-small");
	else if(value==2)
		return CComBSTR(L"x-small");
	else if(value==3)
		return CComBSTR(L"small");
	else if(value==4)
		return CComBSTR(L"medium");
	else if(value==5)
		return CComBSTR(L"large");
	else if(value==6)
		return CComBSTR(L"x-large");

	return CComBSTR(L"xx-large");
}

//-------------------------------------------------------------------------------
// Name: Clear
// Desc: 
//-------------------------------------------------------------------------------
void COfsDhtmlEditCtrl::Clear()
{
	int FontValue	=	GetDefaultFontSizeIndex();
	
	WCHAR	Buff[1000];
	swprintf(Buff,L"<HEAD><STYLE>P{margin: 0 0 0 0;font-family:%s;font-size:%s;} BODY{word-wrap:break-word;word-break: normal;font-family:%s;font-size:%s;}</STYLE>"
		/*L"<SCRIPT language=\"JavaScript\">"
		L"function document.onclick(){if(window.event.srcElement.tagName==\"A\") "
		L"window.open(window.event.srcElement.href);}"
		L"</SCRIPT>"*/
		L"</HEAD><BODY  bottomMargin=2 leftMargin=2 rightMargin=2 topMargin=2>"
		//L"<P><FONT face=\"%s\" size=\"%d\">&nbsp;</FONT></P></BODY>",
		L"</BODY>",
		m_bsDefaultFontName,GetAbsoluteFontSizeName(FontValue),
		m_bsDefaultFontName,GetAbsoluteFontSizeName(FontValue),
		m_bsDefaultFontName,FontValue);

	CComBSTR	bsCleatText	=	Buff;

	SetHTML(bsCleatText);
}

//-------------------------------------------------------------------------------
// Name: SetRightJustify
// Desc: 
//-------------------------------------------------------------------------------
void COfsDhtmlEditCtrl::SetRightJustify()
{
	HrExecDefault(IDM_TRIED_JUSTIFYRIGHT ,NULL);
	//SetFocus();
}

//-------------------------------------------------------------------------------
// Name: SetCenterJustify
// Desc: 
//-------------------------------------------------------------------------------
void COfsDhtmlEditCtrl::SetCenterJustify()
{
	HrExecDefault(IDM_TRIED_JUSTIFYCENTER,NULL);
	//SetFocus();
}

//-------------------------------------------------------------------------------
// Name: SetLeftJustify
// Desc: 
//-------------------------------------------------------------------------------
void COfsDhtmlEditCtrl::SetLeftJustify()
{
	HrExecDefault(IDM_TRIED_JUSTIFYLEFT,NULL);
	//SetFocus();
}

//-------------------------------------------------------------------------------
// Name: ShowDetails
// Desc: 
//-------------------------------------------------------------------------------
void COfsDhtmlEditCtrl::ShowDetails(BOOL bShow)
{
	VARIANTARG va;
	VariantInit(&va);
	va.vt = VT_BOOL;
	va.lVal= bShow;
    HrExecDefault(IDM_TRIED_SHOWDETAILS,&va); 
	//SetFocus();
}

//-------------------------------------------------------------------------------
// Name: SetUnLink
// Desc: 
//-------------------------------------------------------------------------------
void COfsDhtmlEditCtrl::SetUnLink()
{
    HrExecDefault(IDM_TRIED_UNLINK,NULL);
	//SetFocus();
}

//-------------------------------------------------------------------------------
// Name: InitInfoMessage
// Desc: Инициализирует CallBack Сообщение 
//-------------------------------------------------------------------------------
void COfsDhtmlEditCtrl::InitInfoMessage(DWORD dwInfoMessage)
{
	this->dwInfoMessage = dwInfoMessage;
}

//-------------------------------------------------------------------------------
// Name: UpdateCmdControl
// Desc: Подходит для меню и ToolBar'ов.
//-------------------------------------------------------------------------------
void COfsDhtmlEditCtrl::UpdateCmdControl(CCmdUI *pCmdUI, ULONG ucmdID, BOOL bCheck)
{
    /// look triedcid.h for more help info 	
	DWORD dwStatus = GetCommandStatus(ucmdID);
	pCmdUI->Enable((dwStatus & OLECMDF_ENABLED) == OLECMDF_ENABLED );
	if(bCheck)
		pCmdUI->SetCheck(((dwStatus & OLECMDF_LATCHED) == OLECMDF_LATCHED));
}

//-------------------------------------------------------------------------------
// Name: GetBold
// Desc: Для текщего выделения
//-------------------------------------------------------------------------------
BOOL COfsDhtmlEditCtrl::GetBold()
{
	DWORD dwStatus = GetCommandStatus(IDM_TRIED_BOLD);
	return ((dwStatus & OLECMDF_LATCHED) == OLECMDF_LATCHED); 
}

//-------------------------------------------------------------------------------
// Name: GetItalick
// Desc: 
//-------------------------------------------------------------------------------
BOOL COfsDhtmlEditCtrl::GetItalick()
{
	DWORD dwStatus = GetCommandStatus(IDM_TRIED_ITALIC);
	return ((dwStatus & OLECMDF_LATCHED) == OLECMDF_LATCHED); 
}

//-------------------------------------------------------------------------------
// Name: GetUnderline
// Desc: 
//-------------------------------------------------------------------------------
BOOL COfsDhtmlEditCtrl::GetUnderline()
{
	DWORD dwStatus = GetCommandStatus(IDM_TRIED_UNDERLINE);
	return ((dwStatus & OLECMDF_LATCHED) == OLECMDF_LATCHED); 
}

//-------------------------------------------------------------------------------
// Name: GetTextSize
// Desc: 
//-------------------------------------------------------------------------------
int COfsDhtmlEditCtrl::GetTextSize()
{
	DWORD dwFontSizeStatus = GetCommandStatus( IDM_TRIED_FONTSIZE );
	if (dwFontSizeStatus & OLECMDF_NINCHED)
		return -1;

	VARIANT va;
	VariantInit(&va);
	HrExecCommand(IDM_TRIED_FONTSIZE, NULL, &va, FALSE);

	///TRACE("\r\n -- GetTextSize() -- %d",va.lVal);

	return va.lVal;
}

//-------------------------------------------------------------------------------
// Name: ShowContextMenu
// Desc: Отображает Контекстное меню...
//-------------------------------------------------------------------------------
HRESULT COfsDhtmlEditCtrl::ShowContextMenu(DWORD dwID, POINT *pptPosition, IUnknown *pCommandTarget, IDispatch *pDispatchObjectHit)
{
	if(!pContextMenu) return E_NOTIMPL;

	CMenu *pSubMenu = NULL;
	pSubMenu = pContextMenu->GetSubMenu(MenuItem);
	UpdateMenu(pNotifyParent,pSubMenu);
	pSubMenu->TrackPopupMenu(TPM_LEFTALIGN|TPM_RIGHTBUTTON, pptPosition->x, pptPosition->y, pNotifyParent);

	return S_OK;
}

//-------------------------------------------------------------------------------
// Name: UpdateMenu
// Desc: 
//-------------------------------------------------------------------------------
void COfsDhtmlEditCtrl::UpdateMenu(CWnd *pWnd, CMenu *pMenu)
{
	CCmdUI cmdUI;
	cmdUI.m_pMenu = pMenu;
	cmdUI.m_nIndexMax = pMenu->GetMenuItemCount();
	for (cmdUI.m_nIndex = 0; cmdUI.m_nIndex < cmdUI.m_nIndexMax; ++cmdUI.m_nIndex)
	{
		CMenu* pSubMenu = pMenu->GetSubMenu(cmdUI.m_nIndex);
		if(pSubMenu==NULL)
		{
			cmdUI.m_nID = pMenu->GetMenuItemID(cmdUI.m_nIndex);
			cmdUI.DoUpdate(pWnd, FALSE);
		}
		else
		{
			UpdateMenu(pWnd, pSubMenu);
		}
	}
}

//-------------------------------------------------------------------------------
// Name: SetContextMenu
// Desc: 
//-------------------------------------------------------------------------------
BOOL COfsDhtmlEditCtrl::SetContextMenu(UINT resMenuId, int Item,CWnd *pNotifyParent)
{
	if(!pContextMenu)
		pContextMenu = new CMenu;

	if(!pNotifyParent) return FALSE;

	this->pNotifyParent = pNotifyParent;
	MenuItem = Item;

	return pContextMenu->LoadMenu(resMenuId);;
}

//-------------------------------------------------------------------------------
// Name: ClipboardCopy
// Desc: 
//-------------------------------------------------------------------------------
void COfsDhtmlEditCtrl::ClipboardCopy()
{
	HrExecDefault(IDM_TRIED_COPY,NULL);
}

//-------------------------------------------------------------------------------
// Name: ClipboardPast
// Desc: 
//-------------------------------------------------------------------------------
void COfsDhtmlEditCtrl::ClipboardPast()
{
	HrExecDefault(IDM_TRIED_PASTE,NULL);
}

//-------------------------------------------------------------------------------
// Name: ClipboardDelete
// Desc: 
//-------------------------------------------------------------------------------
void COfsDhtmlEditCtrl::ClipboardDelete()
{
	HrExecDefault(IDM_TRIED_DELETE,NULL);
}

//-------------------------------------------------------------------------------
// Name: ClipboardCut
// Desc: 
//-------------------------------------------------------------------------------
void COfsDhtmlEditCtrl::ClipboardCut()
{
	HrExecDefault(IDM_TRIED_CUT,NULL);
}

//-------------------------------------------------------------------------------
// Name: EnableClipboardCopy
// Desc: 
//-------------------------------------------------------------------------------
BOOL COfsDhtmlEditCtrl::EnableClipboardCopy()
{
	DWORD dwStatus = GetCommandStatus(IDM_TRIED_COPY);
	return ((dwStatus & OLECMDF_ENABLED) == OLECMDF_ENABLED); 
}

//-------------------------------------------------------------------------------
// Name: EnableClipboardPast
// Desc: 
//-------------------------------------------------------------------------------
BOOL COfsDhtmlEditCtrl::EnableClipboardPast()
{
	DWORD dwStatus = GetCommandStatus(IDM_TRIED_PASTE);
	return ((dwStatus & OLECMDF_ENABLED) == OLECMDF_ENABLED); 
}

//-------------------------------------------------------------------------------
// Name: EnableClipboardDelete
// Desc: 
//-------------------------------------------------------------------------------
BOOL COfsDhtmlEditCtrl::EnableClipboardDelete()
{
	DWORD dwStatus = GetCommandStatus(IDM_TRIED_DELETE);
	return ((dwStatus & OLECMDF_ENABLED) == OLECMDF_ENABLED); 
}

//-------------------------------------------------------------------------------
// Name: EnableClipboardCut
// Desc: 
//-------------------------------------------------------------------------------
BOOL COfsDhtmlEditCtrl::EnableClipboardCut()
{
	DWORD dwStatus = GetCommandStatus(IDM_TRIED_CUT);
	return ((dwStatus & OLECMDF_ENABLED) == OLECMDF_ENABLED); 
}

//-------------------------------------------------------------------------------
// Name: IsEditMode
// Desc: 
//-------------------------------------------------------------------------------
BOOL COfsDhtmlEditCtrl::IsEditMode()
{
	DWORD dwStatus = GetCommandStatus(IDM_TRIED_EDITMODE);
	return ((dwStatus & OLECMDF_LATCHED) == OLECMDF_LATCHED); 
}

//-------------------------------------------------------------------------------
// Name: IsViewMode
// Desc: 
//-------------------------------------------------------------------------------
BOOL COfsDhtmlEditCtrl::IsViewMode()
{
	DWORD dwStatus = GetCommandStatus(IDM_TRIED_EDITMODE);
	return ((dwStatus & OLECMDF_LATCHED) == OLECMDF_LATCHED); 
}

//-------------------------------------------------------------------------------
// Name: InsertHTML
// Desc: 
//-------------------------------------------------------------------------------
BOOL COfsDhtmlEditCtrl::InsertHTML(BSTR strHTML)
{
	IHTMLTxtRange *pRange= NULL;
	if (SUCCEEDED(HrGetRangeFromSelection(&pRange)))
	{
		bstr_t bstrContent = strHTML;
		pRange->pasteHTML( bstrContent );
		//pRange->put_text( bstrContent );
		pRange->Release();
		return TRUE;
	}
	return FALSE;
}

//-------------------------------------------------------------------------------
// Name: SetBodyMargin
// Desc: 
//-------------------------------------------------------------------------------
BOOL COfsDhtmlEditCtrl::SetBodyMargin(int top, int right, int down, int left)
{
	HRESULT hr = S_OK;
	//hr = CoInitialize(NULL);
	IHTMLBodyElement* pBody = NULL;
	hr = HrGetBody(&pBody);
	if(SUCCEEDED(hr))
	{
		variant_t Variant;
		char buffer[20];
		if(top>=0)
		{
			Variant = itoa(top,buffer,10);
			pBody->put_topMargin(Variant);
		}
		if(right>=0)
		{
			Variant = itoa(right,buffer,10);
			pBody->put_rightMargin(Variant);
		}
		if(down>=0)
		{
			Variant = itoa(down,buffer,10);
			pBody->put_bottomMargin(Variant);
		}
		if(left>=0)
		{
			Variant = itoa(left,buffer,10);
			pBody->put_leftMargin(Variant);
		}
		pBody->Release();
		return TRUE;
	}
	
	return FALSE;
}

//-------------------------------------------------------------------------------
// Name: SetFontName
// Desc: 
//-------------------------------------------------------------------------------
void COfsDhtmlEditCtrl::SetFontName(BSTR strFontName)
{
	VARIANT va;
	
	bstr_t bstrName =  strFontName;
	
	VariantInit(&va);
	va.vt = VT_BSTR;
	va.bstrVal = bstrName;
	
	if(SUCCEEDED(HrExecCommand(IDM_TRIED_FONTNAME, &va, NULL, FALSE)))
	{
		//BYTE bDefaultScript = 6;
		//RegSaveBin(_T("International\\Scripts"),_T("Default_Script"),&bDefaultScript,sizeof(BYTE));
		//RegSaveString(_T("International\\Scripts\\6"),_T("IEFixedFontName"),bstrName);
		//RegSaveString(_T("International\\Scripts\\6"),_T("IEPropFontName"),bstrName);
	}
}

//-------------------------------------------------------------------------------
// Name: GetFontName
// Desc: 
//-------------------------------------------------------------------------------
CString COfsDhtmlEditCtrl::GetFontName()
{
	CString retVal;
	DWORD dwFontNameStatus = GetCommandStatus( IDM_TRIED_FONTNAME );

	if (((dwFontNameStatus & OLECMDF_ENABLED)!=OLECMDF_ENABLED)	||((dwFontNameStatus & OLECMDF_NINCHED)== OLECMDF_NINCHED))
		retVal.Empty();
	else
	{
		VARIANT va;
		VariantInit(&va);
		
		HrExecCommand(IDM_TRIED_FONTNAME, NULL, &va, FALSE);
		
		if(va.vt==VT_BSTR)
			retVal = va.bstrVal;
		
		VariantClear(&va);
	}

	//TRACE("\r\n -- GetFontName() -- %s",retVal);
	
	return retVal;
}

HWND COfsDhtmlEditCtrl::GetWindow()
{
	return m_hWndObj;
}

COLORREF COfsDhtmlEditCtrl::GetColor()
{
	DWORD dwFontSizeStatus = GetCommandStatus(IDM_TRIED_FORECOLOR);
	if (dwFontSizeStatus & OLECMDF_NINCHED)
		return 0;
	
	VARIANT va;
	VariantInit(&va);
	HrExecCommand(IDM_TRIED_FORECOLOR, NULL, &va, FALSE);
	
	return va.lVal;	
}

COLORREF COfsDhtmlEditCtrl::GetBgColor()
{
	COLORREF Ret = 0xFFFFFF;

	IUnknown* lpUnk = m_pSite->GetObjectUnknown();
	
	if ( lpUnk != NULL )
	{
		HRESULT  hr = S_OK;
		VARIANT varColor;
		// Then, request the "document" object from the object
		IHTMLDocument2* pHTMLDocument2;
		hr = lpUnk->QueryInterface(IID_IHTMLDocument2, (void **)&pHTMLDocument2);
		if ( SUCCEEDED( hr ) )
		{
			// Now, set the background color of the body to be equal to something
			hr = pHTMLDocument2->get_bgColor(&varColor );
			pHTMLDocument2->Release();
			if(SUCCEEDED(hr))
			{
				WCHAR Buff[16] = L"", *pEnd = NULL;
				swprintf(Buff,L"0x%s",&(varColor.bstrVal[1]));
				VariantClear(&varColor);
				Ret =  wcstol(Buff,&pEnd,16);
			}
		}
	}
	return Ret;	
}

BOOL COfsDhtmlEditCtrl::InsertTEXT(BSTR strText)
{
	IHTMLTxtRange *pRange= NULL;
	if (SUCCEEDED(HrGetRangeFromSelection(&pRange)))
	{
		bstr_t bstrContent = strText;
		pRange->put_text( bstrContent );
		pRange->Release();
		return TRUE;
	}
	return FALSE;
}

HRESULT COfsDhtmlEditCtrl::RegSaveString(LPCTSTR strPath, LPCTSTR strKey, LPCTSTR strData)
{
	CString strFullPath;
	strFullPath.Format(_T("Software\\%s\\%s\\McDHTMLEdit\\%s"),AfxGetApp()->m_pszRegistryKey,AfxGetApp()->m_pszProfileName,strPath);
	
	HKEY hKey = NULL;
	DWORD dwDisposition = NULL;
	
	HRESULT hr = RegCreateKeyEx(HKEY_CURRENT_USER, strFullPath,NULL, NULL, REG_OPTION_NON_VOLATILE, KEY_WRITE,NULL, &hKey, &dwDisposition);
	if(hr==S_OK)
	{
		hr = RegSetValueEx(hKey, strKey, NULL, REG_SZ,(CONST BYTE*)strData, _tcsclen(strData));
		RegCloseKey(hKey);
	}
	return hr;
}

HRESULT COfsDhtmlEditCtrl::RegSaveBin(LPCTSTR strPath, LPCTSTR strKey, const BYTE* Data, LONG DataSize)
{
	CString strFullPath;
	strFullPath.Format(_T("Software\\%s\\%s\\McDHTMLEdit\\%s"),AfxGetApp()->m_pszRegistryKey,AfxGetApp()->m_pszProfileName,strPath);
	
	HKEY hKey = NULL;
	DWORD dwDisposition = NULL;
	
	HRESULT hr = RegCreateKeyEx(HKEY_CURRENT_USER, strFullPath,NULL, NULL, REG_OPTION_NON_VOLATILE, KEY_WRITE,NULL, &hKey, &dwDisposition);
	if(hr==S_OK)
	{
		hr = RegSetValueEx(hKey, strKey, NULL, REG_BINARY,(CONST BYTE*)Data, DataSize);
		RegCloseKey(hKey);
	}
	return hr;
}

HRESULT COfsDhtmlEditCtrl::GetTXT(BSTR *pStr)
{
	CComPtr<IHTMLDocument2>			pHTMLDocument2	=	NULL;
	CComPtr<IUnknown>				pUnk			=	m_pSite->GetObjectUnknown();
	
	HRESULT hr = pUnk->QueryInterface(IID_IHTMLDocument2,(void**)&pHTMLDocument2);
	
	if(SUCCEEDED(hr))
	{
		CComPtr<IHTMLElement> pBody;
		
		HRESULT hr = pHTMLDocument2->get_body(&pBody);
		if(SUCCEEDED(hr))
		{
			pBody->get_innerText(pStr);
		}
	}
	
	return S_OK; 
}

void COfsDhtmlEditCtrl::UpdateCmdControl(CMcButton *pBtn, ULONG ucmdID, BOOL bCheck)
{
    /// look triedcid.h for more help info 	
	DWORD dwStatus = GetCommandStatus(ucmdID);
	pBtn->EnableWindow((dwStatus & OLECMDF_ENABLED) == OLECMDF_ENABLED);
	if(bCheck)
		pBtn->SetPressed(((dwStatus & OLECMDF_LATCHED) == OLECMDF_LATCHED));
}

BOOL COfsDhtmlEditCtrl::SetDefaultFontName(BSTR bsFontName)
{
	m_bsDefaultFontName	=	bsFontName;
	return TRUE;
}

BOOL COfsDhtmlEditCtrl::SetDefaultFontSize(long FontSize)
{
	m_lDefaultFontSize	=	FontSize;
	return TRUE;
}

long COfsDhtmlEditCtrl::GetDefaultFontSize()
{
	return m_lDefaultFontSize;
}


long COfsDhtmlEditCtrl::GetDefaultFontSizeIndex()
{
	int FontValue	=	0;
	int nFontSizes[] = {8, 10, 12, 14, 18, 24, 36};
	//find otnosil param
	for (int i = 0; i < sizeof(nFontSizes)/sizeof(int); i++)
	{
		if(nFontSizes[i]==m_lDefaultFontSize)
		{
			FontValue	=	i+1;	
			break;
		}
	}

	return FontValue;
}

BSTR COfsDhtmlEditCtrl::GetDefaultFontName()
{
	return m_bsDefaultFontName;
}

