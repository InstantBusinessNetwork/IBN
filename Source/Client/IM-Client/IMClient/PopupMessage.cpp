// PopupMessage.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "PopupMessage.h"
#include "ExDispid.h"
#include "mshtml.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

template <class T> class CHtmlEventObject : public IDispatch  
{
	
	typedef void (T::*EVENTFUNCTIONCALLBACK)(DISPID id, VARIANT* pVarResult);
	
public:
	CHtmlEventObject() { m_cRef = 0; }    
	~CHtmlEventObject() {}
	
	HRESULT __stdcall QueryInterface(REFIID riid, void** ppvObject)
	{
		*ppvObject = NULL;
		
		if (IsEqualGUID(riid, IID_IUnknown))
			*ppvObject = reinterpret_cast<void**>(this);
		
		if (IsEqualGUID(riid, IID_IDispatch))
			*ppvObject = reinterpret_cast<void**>(this); 
		
		if (*ppvObject)
		{
			((IUnknown*)*ppvObject)->AddRef();
			return S_OK;
		}
		else return E_NOINTERFACE;
	}
	
	DWORD __stdcall AddRef()
	{
		return InterlockedIncrement(&m_cRef);
	}
	
	DWORD __stdcall Release()
	{
		if (InterlockedDecrement(&m_cRef) == 0)
		{
			delete this;
			return 0;
		}
		return m_cRef;
	}
	
	STDMETHOD(GetTypeInfoCount)(unsigned int FAR* pctinfo)
	{ return E_NOTIMPL; }
	
	STDMETHOD(GetTypeInfo)(unsigned int iTInfo, LCID  lcid, ITypeInfo FAR* FAR*  ppTInfo)
	{ return E_NOTIMPL; }
	
	STDMETHOD(GetIDsOfNames)(REFIID riid, OLECHAR FAR* FAR* rgszNames, unsigned int cNames, LCID lcid, DISPID FAR* rgDispId)
	{ return S_OK; }
	
	STDMETHOD(Invoke)(DISPID dispIdMember, REFIID riid, LCID lcid,
		WORD wFlags, DISPPARAMS* pDispParams, VARIANT* pVarResult,
		EXCEPINFO * pExcepInfo, UINT * puArgErr)
	{
		if (DISPID_VALUE == dispIdMember)
			(m_pT->*m_pFunc)(m_id, pVarResult);
		else
			TRACE(_T("Invoke dispid = %d\n"), dispIdMember);
		
		return S_OK;
	}
	
public:
	static LPDISPATCH CreateHandler(T* pT,
		EVENTFUNCTIONCALLBACK pFunc, DISPID id)
	{
		CHtmlEventObject<T>* pFO = new CHtmlEventObject<T>;
		pFO->m_pT = pT;
		pFO->m_pFunc = pFunc;
		pFO->m_id = id;
		return reinterpret_cast<LPDISPATCH>(pFO);
	}
	
protected:
	T* m_pT;
	EVENTFUNCTIONCALLBACK m_pFunc;
	DISPID m_id;
	long m_cRef;
};

/////////////////////////////////////////////////////////////////////////////
// CPopupMessage dialog
#define		TIMER_DEGREE			(50)
#define		WINDOW_OFFSET			(3)
#define		WINDOW_SHOW_CY_OFFSET	(15)

CPopupMessage::CPopupMessage(CWnd* pParent /*=NULL*/)
: COFSNcDlg2(CPopupMessage::IDD, pParent)
{
	//{{AFX_DATA_INIT(CPopupMessage)
	//}}AFX_DATA_INIT
	m_bResizable		=	FALSE;
	SetBoundary(0, 0);
	m_strSkinSettings	=	_T("/Shell/Alert/skin.xml");
	m_dwTimeout = -1;
	pmState =	PMS_NONE;
	
	m_MsgId	=	NULL;
	m_MsgL	=	0;
	m_MsgW	=	0;
}


void CPopupMessage::DoDataExchange(CDataExchange* pDX)
{
	COFSNcDlg2::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CPopupMessage)
	DDX_Control(pDX, IDC_IN_EXPLORER, m_browser);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CPopupMessage, COFSNcDlg2)
//{{AFX_MSG_MAP(CPopupMessage)
ON_WM_TIMER()
ON_WM_DESTROY()
ON_WM_LBUTTONDOWN()
ON_WM_ACTIVATE()
ON_WM_LBUTTONUP()
ON_WM_SETFOCUS()
//}}AFX_MSG_MAP
END_MESSAGE_MAP()

BEGIN_EVENTSINK_MAP(CPopupMessage, COFSNcDlg2)
//{{AFX_EVENTSINK_MAP(CPopupMessage
ON_EVENT(CPopupMessage, IDC_IN_EXPLORER, DISPID_DOCUMENTCOMPLETE, OnWebDocumentCompleted, VTS_DISPATCH VTS_PVARIANT)
ON_EVENT(CPopupMessage, IDC_IN_EXPLORER, DISPID_NAVIGATECOMPLETE2, OnWebNavigateComplete2, VTS_DISPATCH VTS_PVARIANT)
//}}AFX_EVENTSINK_MAP
END_EVENTSINK_MAP()

/////////////////////////////////////////////////////////////////////////////
// CPopupMessage message handlers

/*void CPopupMessage::OnInfolinkStatic() 
{
	// TODO: Add your control notification handler code here
	// Send Message To Parent;
	if(m_MsgId)
		m_pParentWnd->PostMessage(m_MsgId,m_MsgW,m_MsgL);
	ShowWindow(SW_HIDE);
	KillWindow();
}*/

BOOL CPopupMessage::OnInitDialog() 
{
	COFSNcDlg2::OnInitDialog();

	CRect	winRect;
	GetWindowRect(&winRect);
	
	m_MaxCY = winRect.Height();
	m_MinCY		= 40;
	
	//HRESULT hr = m_pWebCustomizer.CreateInstance(CLSID_MpaWebCustomizer);

	
	
	SetWindowPos(&wndTopMost,-1,-1,-1,-1,SWP_NOMOVE|SWP_NOSIZE|SWP_NOACTIVATE);
	
	SetTimer(141,TIMER_DEGREE,0);
	
	//LPUNKNOWN pDispatch = m_browser.GetControlUnknown();
	//m_pWebCustomizer->PutRefWebBrowser((LPDISPATCH)pDispatch);
	
	
	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

void CPopupMessage::LoadSkin(IXMLDOMNode *pXmlRoot)
{
	CRect r;
	if(LoadRectangle(pXmlRoot, _T("Browser"), r))
	{
		m_browser.SetWindowPos(NULL, r.left, r.top, r.Width(), r.Height(), SWP_NOZORDER);
	}
}

/*
BOOL CPopupMessage::Show(LPCTSTR strMessage, DWORD dwTimeoutSec)
{
m_strText = strMessage;
UpdateData(FALSE);
m_dwTimeout = dwTimeoutSec*1000;
m_CurrentCY	=	40;
pmState = PMS_SHOW;

  SetCY(m_CurrentCY);	
  
	return TRUE;
}*/



void CPopupMessage::OnTimer(UINT nIDEvent) 
{
	if(nIDEvent==141)
	{
		switch(pmState) 
		{
		case PMS_SHOW:
			m_CurrentCY += WINDOW_SHOW_CY_OFFSET;
			if(m_CurrentCY>=m_MaxCY)
			{
				m_CurrentCY =m_MaxCY;
				Wait();
			}
			SetCY(m_CurrentCY);
			break;
		case PMS_HIDE:
			m_CurrentCY -= WINDOW_SHOW_CY_OFFSET;
			SetCY(m_CurrentCY);
			if(m_CurrentCY<=m_MinCY)
			{
				m_CurrentCY = m_MinCY;
				KillWindow();
				return;
			}
			break;
		case PMS_WAIT:
			m_dwTimeout-=TIMER_DEGREE;
			if(m_dwTimeout<0)
			{
				Hide();
			}
			break;
		}
	}
	
	COFSNcDlg2::OnTimer(nIDEvent);
}

void CPopupMessage::OnDestroy() 
{
	COFSNcDlg2::OnDestroy();
}

BOOL CPopupMessage::Hide()
{
	pmState = PMS_HIDE;
	return TRUE;
}

void CPopupMessage::SetCY(long Value)
{
	CRect	winRect;
	
	GetWindowRect(winRect);
	CRect rc;
	SystemParametersInfo(SPI_GETWORKAREA, 0, &rc, 0);
	
	SetWindowPos(NULL,
		rc.Width()- WINDOW_OFFSET- winRect.Width(),rc.Height() - WINDOW_OFFSET - Value,
		winRect.Width(),Value,SWP_SHOWWINDOW|SWP_NOZORDER|SWP_NOACTIVATE);
	UpdateWindow();
}

BOOL CPopupMessage::Stop()
{
	pmState = PMS_NONE;
	return TRUE;
}

BOOL CPopupMessage::Wait()
{
	pmState = PMS_WAIT;
	return TRUE;
}

void CPopupMessage::KillWindow()
{
	ShowWindow(SW_HIDE);
	KillTimer(141);
	DestroyWindow();
	delete this;
}

void CPopupMessage::OnLButtonDown(UINT nFlags, CPoint point) 
{
}

BOOL CPopupMessage::InitClickMsg(UINT Message, WPARAM w, LPARAM l)
{
	m_MsgId = Message;
	m_MsgW = w;
	m_MsgL = l;
	return TRUE;
}

void CPopupMessage::OnActivate(UINT nState, CWnd* pWndOther, BOOL bMinimized) 
{
	COFSNcDlg::OnActivate(nState, pWndOther, bMinimized);
}

void CPopupMessage::OnLButtonUp(UINT nFlags, CPoint point) 
{
	Hide();
}

void CPopupMessage::OnSetFocus( CWnd* pOldWnd )
{
	if(pOldWnd)
	{
		pOldWnd->PostMessage(WM_ACTIVATE,WA_ACTIVE,0);
	}
}


BOOL CPopupMessage::Show(LPCWSTR strXML, LPCWSTR strXSLT,DWORD dwTimeoutSec)
{
	// Step 1. Load XML and XSLT [5/7/2002]
	CComPtr<IXMLDOMDocument>	docXML	=	NULL, docXSLT	=	NULL;
	docXML.CoCreateInstance(CLSID_DOMDocument40);
	docXSLT.CoCreateInstance(CLSID_DOMDocument40);

	VARIANT_BOOL	varLoadXML	=	VARIANT_FALSE,	varLoadXSLT	=	VARIANT_FALSE;
	
	docXML->loadXML(CComBSTR(strXML),&varLoadXML);
	docXSLT->loadXML(CComBSTR(strXSLT),&varLoadXSLT);
	
	if(varLoadXML==VARIANT_TRUE&&varLoadXSLT==VARIANT_TRUE)
	{
		// Step 2. Get HTML [5/7/2002]
		if(docXML->transformNode(docXSLT,&m_bsHTML)==S_OK)
		{
			UpdateData(FALSE);
			m_dwTimeout = dwTimeoutSec*1000;
			m_CurrentCY	=	40;
			pmState = PMS_SHOW;
			
			SetCY(m_CurrentCY);	
			
			m_browser.Navigate(_T("about:blank"),NULL,NULL,NULL,NULL);

			return TRUE;
		}
	}

	return FALSE;

			// Step 3. Try Load HTML to WebBrowser
/*			CComPtr<IPersistStreamInit>		pPersistStream	=	NULL;
			CComPtr<IUnknown>				pUnk			=	NULL;
			
			pUnk.Attach(m_browser.GetDocument());

			
			if(pUnk)
			{
				HRESULT hr = pUnk->QueryInterface(IID_IPersistStreamInit,(void**)&pPersistStream);
				if(SUCCEEDED(hr))
				{
					hr = pPersistStream->InitNew();

					HGLOBAL hMem = NULL;
						
					ULONG StrRealSize = (m_bsHTML.Length()+1)*2;
					hMem = GlobalAlloc(GPTR, StrRealSize);
					LPVOID pBuf = (LPVOID)GlobalLock(hMem);
					memcpy((void*)pBuf,(void*)m_bsHTML.m_str,StrRealSize);
					GlobalUnlock(hMem);
						
					CComPtr<IStream> pDataStream = NULL;
					CreateStreamOnHGlobal(hMem,TRUE,&pDataStream);
						
					hr = pPersistStream->Load(pDataStream);
							
					return TRUE;
				}
			}
		}
	}

	return FALSE;*/

}

/*void CPopupMessage::OnAppStatic() 
{
	OnInfolinkStatic();
}

void CPopupMessage::OnTitleStatic() 
{
	OnInfolinkStatic();
}*/

void CPopupMessage::OnWebDocumentCompleted(IDispatch *pDisp, VARIANT *URL)
{
	///TRACE("\r\n -- CPopupMessage::OnWebDocumentCompleted");
	CComPtr<IHTMLDocument2>	pHtmlDoc;
	pHtmlDoc.Attach((IHTMLDocument2*)m_browser.GetDocument());
	
	// Step 3. Подвязаться к событиям KeyDown
	// Создаем объект-обработчик
	LPDISPATCH dispFO = CHtmlEventObject<CPopupMessage>::CreateHandler(this, &CPopupMessage::OnWebMouseDown, 1);
	VARIANT vIn;                            
	V_VT(&vIn) = VT_DISPATCH; 
	V_DISPATCH(&vIn) = dispFO; 

	// устанавливаем обработчик document.onkeydown
	CComVariant	pOut;
	pHtmlDoc->get_onmousedown(&pOut);
	HRESULT hr = pHtmlDoc->put_onmousedown( vIn );
}

void CPopupMessage::OnWebMouseDown(DISPID id, VARIANT *pVarResult)
{
	V_BOOL(pVarResult) = FALSE;  
	
	if(m_MsgId)
		m_pParentWnd->PostMessage(m_MsgId,m_MsgW,m_MsgL);
	ShowWindow(SW_HIDE);
	KillWindow();
}

void CPopupMessage::OnWebNavigateComplete2(IDispatch *pDisp, VARIANT *URL)
{
	// Step1. Получить HTML Document [5/7/2002]
	CComPtr<IHTMLDocument2>	pHtmlDoc;
	pHtmlDoc.Attach((IHTMLDocument2*)m_browser.GetDocument());
	
	// Step 2. Try Load HTML to WebBrowser
	CComPtr<IPersistStreamInit>		pPersistStream	=	NULL;
	
	if(pHtmlDoc&&m_bsHTML.Length())
	{
		HRESULT hr = pHtmlDoc->QueryInterface(IID_IPersistStreamInit,(void**)&pPersistStream);
		if(SUCCEEDED(hr))
		{
			hr = pPersistStream->InitNew();
			
			HGLOBAL hMem = NULL;
			
			ULONG StrRealSize = (m_bsHTML.Length()+1)*2;
			hMem = GlobalAlloc(GPTR, StrRealSize+2);
			LPBYTE pBuf = (LPBYTE)GlobalLock(hMem);
			//////////////////////////////////////////////////////////////////////////
			// !!! CAUTION [6/18/2002]
			// Обязательно добавлять 0xFF, 0xFE, перед Unicode - данными, 
			// иначе IE не правильно будет определять, не английские Unicode символы. 
			// !!! CAUTION [6/18/2002]
			//////////////////////////////////////////////////////////////////////////
			pBuf[0]=0xFF;
			pBuf[1]=0xFE;
			//////////////////////////////////////////////////////////////////////////
			memcpy((LPVOID)(pBuf+2),(void*)m_bsHTML,StrRealSize);
			//////////////////////////////////////////////////////////////////////////
			GlobalUnlock(hMem);
			
			CComPtr<IStream> pDataStream = NULL;
			CreateStreamOnHGlobal(hMem,TRUE,&pDataStream);
			
			hr = pPersistStream->Load(pDataStream);
			
		}
		m_bsHTML.Empty();
	}
}
