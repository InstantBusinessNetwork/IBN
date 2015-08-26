// WebWindow.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "WebWindow.h"

#include "MainDlg.h"
#include "VariablesList.h"
#include "LoadSkins.h"
#include "cdib.h"
#include <ExDispID.h>
#include "DlgTV.h"
#include "McSettings.h"
#include "ScreenShotDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif


extern CString GetCurrentSkin();
CVariablesList g_ViewVariablesList;
CList <_bstr_t,_bstr_t> g_NewPromoList;

// XML Functions

HRESULT GetNodeAttribute(IXMLDOMNode *pNode, BSTR bsAttrName, CComBSTR &strAttrValue)
{
	HRESULT hr = S_OK;
	IXMLDOMElement *pEle = NULL;
	
	if(pNode == NULL)
		return E_INVALIDARG;
	
	hr = pNode->QueryInterface(IID_IXMLDOMElement, (void**)&pEle);
	if(pEle)
	{
		CComVariant var;
		hr = pEle->getAttribute(bsAttrName, &var);
		strAttrValue = var.bstrVal;
		pEle->Release();
	}
	return hr;
}

HRESULT GetNodeAttributeAsLong(IXMLDOMNode *pNode, BSTR bsAttrName, long *pAttrValue, int nBase)
{
	HRESULT hr;
	IXMLDOMElement *pEle = NULL;
	WCHAR *szNULL = L"\0x00";
	CComBSTR bs;
	
	ASSERT(pAttrValue != NULL);
	
	if(pNode == NULL)
		return E_INVALIDARG;

	hr = GetNodeAttribute(pNode, bsAttrName, bs);
	if(bs.m_str != NULL)
		*pAttrValue = wcstol(bs.m_str, &szNULL, nBase);

	return hr;
}

HRESULT SelectChildNode(IXMLDOMNode *pNodeParent, BSTR bsSelect, IXMLDOMNode **ppNodeChild, BSTR *pbsNodeText)
{
	HRESULT hr;
	IXMLDOMNodeList *pNodeList = NULL;
	IXMLDOMNode *pNodeChild = NULL;
	
	if(pNodeParent == NULL || (ppNodeChild == NULL && pbsNodeText == NULL))
		return E_INVALIDARG;
	
	if(ppNodeChild != NULL)
		*ppNodeChild = NULL;
	
	DOMNodeType nt;
	hr = pNodeParent->get_nodeType(&nt);
	hr = pNodeParent->selectNodes(bsSelect, &pNodeList);
	if(pNodeList)
	{
		hr = pNodeList->get_item(0, &pNodeChild);
		if(pNodeChild != NULL)
		{
			if(pbsNodeText != NULL)
			{
				BSTR bs;
				hr = pNodeChild->get_text(&bs);
				*pbsNodeText = bs;
			}
			
			if(ppNodeChild == NULL)
			{
				pNodeChild->Release();
				pNodeChild = NULL;
			}
			else
				*ppNodeChild = pNodeChild;
		}
		pNodeList->Release();
		pNodeList = NULL;
	}
	return hr;
}

/////////////////////////////////////////////////////////////////////////////
// CWebWindow dialog


CWebWindow::CWebWindow(CWnd* pParent /*=NULL*/)
: WEB_WINDOW_PARENT(CWebWindow::IDD, pParent)
{
	//{{AFX_DATA_INIT(CWebWindow)
	//}}AFX_DATA_INIT
	m_bChild = FALSE;
	m_bAutoKill = FALSE;
	m_bResizable = TRUE;
	m_pMessageParent = NULL;
	m_pWebCustomizer = NULL;
	m_dwSessionCookie = 0;
	m_SendStatus = 0;
	m_Handle = 0;
	m_pXMLDoc = NULL;
	SetCaption(RGB(0,0,0),RGB(0,0,0),0);
	m_bQueryAttachXML = FALSE;
	m_rBrowser.SetRectEmpty();
	m_rWindow.SetRectEmpty();
	m_rTarget.SetRectEmpty();
	m_nIEVersion = 0;
	m_bBrowserRect = FALSE;
	m_bIEBack = m_bIEForward = m_bIEStop = m_bIERefresh = FALSE;
	m_InWindowDropTarget	=	NULL;
	m_bCatchNavigate	=	TRUE;
	m_bCatchWindowOpen	=	TRUE;
	m_bFileDownload = FALSE;
	m_nFileDownload = -1;
	m_bNavigateStarted = FALSE;
}


void CWebWindow::DoDataExchange(CDataExchange* pDX)
{
	WEB_WINDOW_PARENT::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CWebWindow)
	DDX_Control(pDX, IDC_TITLE, m_title);
	DDX_Control(pDX, IDC_BUTTON_MIN, m_btnMin);
	DDX_Control(pDX, IDC_BUTTON_X, m_btnX);
	DDX_Control(pDX, IDC_BUTTON_MAX, m_btnMax);
	DDX_Control(pDX, IDC_BUTTON_BACK, m_btnBack);
	DDX_Control(pDX, IDC_BUTTON_FORWARD, m_btnForward);
	DDX_Control(pDX, IDC_BUTTON_REFRESH, m_btnRefresh);
	DDX_Control(pDX, IDC_BUTTON_STOP, m_btnStop);
	DDX_Control(pDX, IDC_BUTTON_RESTORE, m_btnRestore);
	DDX_Control(pDX, ID_DHTML_CTRL, m_browser);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CWebWindow, WEB_WINDOW_PARENT)
	//{{AFX_MSG_MAP(CWebWindow)
	ON_WM_CREATE()
	ON_WM_CLOSE()
	ON_WM_DESTROY()
	ON_WM_SIZE()
	ON_MESSAGE(WM_XMLLOADCOPLETED,OnXMLLoadCopleted)
	ON_WM_PAINT()
	//}}AFX_MSG_MAP
	//ON_NOTIFY_EX_RANGE(TTN_NEEDTEXTW, 0, 0xFFFF, OnToolTipNotify)
	//ON_NOTIFY_EX_RANGE(TTN_NEEDTEXTA, 0, 0xFFFF, OnToolTipNotify)
END_MESSAGE_MAP()

BEGIN_EVENTSINK_MAP(CWebWindow, WEB_WINDOW_PARENT)
	//{{AFX_EVENTSINK_MAP(CWebWindow)
	ON_EVENT(CWebWindow, IDC_BUTTON_X, -600 /* Click */, OnClickButtonX, VTS_NONE)
	ON_EVENT(CWebWindow, IDC_BUTTON_MIN, -600 /* Click */, OnClickButtonMin, VTS_NONE)
	ON_EVENT(CWebWindow, IDC_BUTTON_MAX, -600 /* Click */, OnClickButtonMax, VTS_NONE)
	ON_EVENT(CWebWindow, IDC_BUTTON_BACK, -600 /* Click */, OnClickButtonBack, VTS_NONE)
	ON_EVENT(CWebWindow, IDC_BUTTON_FORWARD, -600 /* Click */, OnClickButtonForward, VTS_NONE)
	ON_EVENT(CWebWindow, IDC_BUTTON_STOP, -600 /* Click */, OnClickButtonStop, VTS_NONE)
	ON_EVENT(CWebWindow, IDC_BUTTON_REFRESH, -600 /* Click */, OnClickButtonRefresh, VTS_NONE)
	ON_EVENT(CWebWindow, IDC_BUTTON_RESTORE, -600 /* Click */, OnClickButtonRestore, VTS_NONE)
	ON_EVENT(CWebWindow, ID_DHTML_CTRL, 255 /* OnToolBar */, OnWebToolBar, VTS_BOOL)
	//}}AFX_EVENTSINK_MAP
	ON_EVENT(CWebWindow, ID_DHTML_CTRL, DISPID_PROGRESSCHANGE /* ProgressChange */, OnWebProgressChange, VTS_I4 VTS_I4)
	ON_EVENT(CWebWindow, ID_DHTML_CTRL, DISPID_BEFORENAVIGATE2 /* BeforeNavigate2 */, OnWebBeforeNavigate2, VTS_DISPATCH VTS_PVARIANT VTS_PVARIANT VTS_PVARIANT VTS_PVARIANT VTS_PVARIANT VTS_PBOOL)
	ON_EVENT(CWebWindow, ID_DHTML_CTRL, DISPID_NEWWINDOW2 /* NewWindow2 */, OnWebNewWindow2, VTS_PDISPATCH VTS_PBOOL)
	ON_EVENT(CWebWindow, ID_DHTML_CTRL, 113 /* TitleChange */, OnWebTitleChange, VTS_BSTR)
	ON_EVENT(CWebWindow, ID_DHTML_CTRL, 252 /* NavigateComplete2 */, OnWebNavigateComplete2, VTS_DISPATCH VTS_PVARIANT)
	ON_EVENT(CWebWindow, ID_DHTML_CTRL, 262 /* WindowSetResizable */, OnWebWindowSetResizable, VTS_BOOL)
	ON_EVENT(CWebWindow, ID_DHTML_CTRL, 263 /* WindowClosing */, OnWebWindowClosing, VTS_BOOL VTS_PBOOL)
	ON_EVENT(CWebWindow, ID_DHTML_CTRL, 264 /* WindowSetLeft */, OnWebWindowSetLeft, VTS_I4)
	ON_EVENT(CWebWindow, ID_DHTML_CTRL, 265 /* WindowSetTop */, OnWebWindowSetTop, VTS_I4)
	ON_EVENT(CWebWindow, ID_DHTML_CTRL, 266 /* WindowSetWidth */, OnWebWindowSetWidth, VTS_I4)
	ON_EVENT(CWebWindow, ID_DHTML_CTRL, 267 /* WindowSetHeight */, OnWebWindowSetHeight, VTS_I4)
	ON_EVENT(CWebWindow, ID_DHTML_CTRL, 270 /* FileDownload */, OnWebFileDownload, VTS_BOOL VTS_PBOOL)
END_EVENTSINK_MAP()

/////////////////////////////////////////////////////////////////////////////
// CWebWindow message handlers

void CWebWindow::OnOK() 
{
//	PostMessage(WM_CLOSE);
	
	///WEB_WINDOW_PARENT::OnOK();
}

void CWebWindow::OnCancel()
{
	PostMessage(WM_CLOSE);
	
	//WEB_WINDOW_PARENT::OnCancel();
}

void CWebWindow::LoadXML(LPCTSTR URL)
{
	if(m_pXMLDoc)
		return;
	m_pXMLDoc = new CXmlDoc;
	m_pXMLDoc->Init(GetSafeHwnd(),WM_XMLLOADCOPLETED);
	m_pXMLDoc->Load(URL);
}

void CWebWindow::CreateAutoKiller(LPCTSTR szSettingsURL, CWnd *pMessageParent, CWnd *pParent, long x, long y, long cx, long cy, LPCTSTR title, LPCTSTR url, BOOL bModal, BOOL bTopMost, BOOL bResizable, UINT DialogTypeId, BOOL bBrowserRect, BOOL bShowToolbar, BOOL bMaximazed)
{
	m_bChild = FALSE;
	m_bAutoKill = TRUE;
	m_pMessageParent = pMessageParent;
	m_bResizable = bResizable;
	m_strURL = url;
	m_strSkinSettings = szSettingsURL;
	m_bBrowserRect = bBrowserRect;
	m_bShowToolbar = bShowToolbar;
	
	CRect r = CRect(CPoint(x,y), CSize(cx,cy));
	AdjustRect(r);
	m_InitialRect = r;

	if(!Create(IDD, pParent))
	{
		TRACE0("Warning: failed to create CWebWindow.\n");
		return ;
	}
	m_strTitle = title;
	SetTitle(m_strTitle);	// Set default title
	
	if(url != NULL)
	{
		CComVariant	varUrl	=	url;
		m_browser.Navigate2(&varUrl, NULL, NULL, NULL, NULL);
	}
		
	
	if(DialogTypeId)
	{
		EnableSaveRestore(GetString(IDS_OFSMESSENGER),GetString(DialogTypeId));
	}
	
	if(bTopMost)	
		this->SetWindowPos(&wndTopMost, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
	
	if(url != NULL)
	{
		if(bMaximazed)
			ShowWindow(SW_SHOWMAXIMIZED);
		else
			ShowWindow(SW_SHOWNORMAL);
		SetForegroundWindow();
	}
	else
	{
		m_rTarget.SetRectEmpty();
		m_bPositionSet = FALSE;
		m_bSizeSet = FALSE;
	}
}

//DEL void CWebWindow::CreateWebWindow(CWnd *pParentWnd, CRect rc, LPCTSTR szCaption, LPCTSTR szSettingsURL, LPCTSTR szURL)
//DEL {
//DEL }

int CWebWindow::OnCreate(LPCREATESTRUCT lpCreateStruct) 
{
	if (WEB_WINDOW_PARENT::OnCreate(lpCreateStruct) == -1)
		return -1;
	
	// Get IE version
	CString str;
	McRegGetString(HKEY_LOCAL_MACHINE, _T("SOFTWARE\\Microsoft\\Internet Explorer"), _T("Version"), str, NULL);
	long nMajor=0, nMinor=0;
	if(!str.IsEmpty())
		_stscanf((LPCTSTR)str, _T("%d.%d"), &nMajor, &nMinor);
	m_nIEVersion = nMajor * 100 + nMinor;
	
	if(!m_bChild)
		AddWindowToClose(this);
	
//	m_browser.Create(NULL, WS_VISIBLE|WS_CHILD, CRect(0,0,0,0), this, ID_DHTML_CTRL);
	
	HRESULT hr = m_pWebCustomizer.CreateInstance(CLSID_MpaWebCustomizer);
	if(FAILED(hr))
		return -1;
	
//	LPUNKNOWN pDispatch = m_browser.GetControlUnknown();
//	m_pWebCustomizer->PutRefWebBrowser((LPDISPATCH)pDispatch);

	m_MpaWebEvent.m_pParent = this;
	InitMpaWebEvent();
	
	return 0;
}

BOOL CWebWindow::OnInitDialog() 
{
	WEB_WINDOW_PARENT::OnInitDialog();

	if(m_bChild)
	{
		m_title.ShowWindow(SW_HIDE);
		m_btnMin.ShowWindow(SW_HIDE);
		m_btnMax.ShowWindow(SW_HIDE);
		m_btnX.ShowWindow(SW_HIDE);
		m_btnBack.ShowWindow(SW_HIDE);
		m_btnForward.ShowWindow(SW_HIDE);
		m_btnRefresh.ShowWindow(SW_HIDE);
		m_title.ShowWindow(SW_HIDE);
		m_btnStop.ShowWindow(SW_HIDE);
		m_btnRestore.ShowWindow(SW_HIDE);
	}
	
	m_InWindowDropTarget = new CMcMessengerDropTarget((CMainDlg*)GetMessageParent(),&m_browser);
	m_browser.SetRegisterAsDropTarget(FALSE);
	m_InWindowDropTarget->Register(&m_browser);
	
	LPUNKNOWN pDispatch = m_browser.GetControlUnknown();
	m_pWebCustomizer->PutRefWebBrowser((LPDISPATCH)pDispatch);

	if(m_bChild)
	{
		ShowToolbar(FALSE);
	}
	else
	{
		//EnableToolTips(TRUE);
		m_ToolTip.AddTool(&m_btnX,IDS_TIP_CLOSE);
		m_ToolTip.AddTool(&m_btnMax,IDS_TIP_MAXIMIZE);
		m_ToolTip.AddTool(&m_btnRestore,IDS_TIP_RESTORY);
		m_ToolTip.AddTool(&m_btnMin,IDS_TIP_MINIMIZE);
		m_ToolTip.AddTool(&m_btnBack,IDS_TIP_BACK);
		m_ToolTip.AddTool(&m_btnForward,IDS_TIP_FORWARD);
		m_ToolTip.AddTool(&m_btnStop,IDS_TIP_STOP);
		m_ToolTip.AddTool(&m_btnRefresh,IDS_TIP_REFRESH);
			
		if(!m_InitialRect.IsRectEmpty())
		{
			if(m_bBrowserRect)
				SetBrowserRect(m_InitialRect);
			else
				SetWindowPos(NULL, m_InitialRect.left, m_InitialRect.top, m_InitialRect.Width(), m_InitialRect.Height(), SWP_NOZORDER);
			Invalidate();
		}
		
		ShowToolbar(m_bShowToolbar);
		m_btnMax.EnableWindow(m_bResizable);
		m_btnRestore.EnableWindow(m_bResizable);
	}
	
	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

void CWebWindow::LoadSkin(IXMLDOMNode *pXmlRoot)
{
	LoadButtons(pXmlRoot);
	if(m_bChild)
	{
	}
	else
	{
		LoadLabel(pXmlRoot, _T("Title"), &m_title, TRUE);
		CRect r;
		if(LoadRectangle(pXmlRoot, _T("Browser"), r))
		{
			m_browser.SetWindowPos(NULL, r.left, r.top, r.Width(), r.Height(), SWP_NOZORDER);
			m_browser.GetWindowRect(&m_rBrowser);
			GetWindowRect(&m_rWindow);
		}
	}
}

void CWebWindow::LoadButtons(IXMLDOMNode *pXmlRoot)
{
	m_btnX.ShowWindow(SW_HIDE);
	m_btnMax.ShowWindow(SW_HIDE);
	m_btnRestore.ShowWindow(SW_HIDE);
	m_btnMin.ShowWindow(SW_HIDE);
	m_btnBack.ShowWindow(SW_HIDE);
	m_btnForward.ShowWindow(SW_HIDE);
	m_btnStop.ShowWindow(SW_HIDE);
	m_btnRefresh.ShowWindow(SW_HIDE);

	if(!m_bChild)
	{
		LoadButton(pXmlRoot, _T("X"), &m_btnX, TRUE, FALSE);
		LoadButton(pXmlRoot, _T("Maximize"), &m_btnMax, TRUE, FALSE, 1);
		LoadButton(pXmlRoot, _T("Restore"), &m_btnRestore, TRUE, FALSE, 2);
		LoadButton(pXmlRoot, _T("Minimize"), &m_btnMin, TRUE, FALSE);
		m_bIEBack = LoadButton(pXmlRoot, _T("IEBack"), &m_btnBack, TRUE, FALSE);
		m_bIEForward = LoadButton(pXmlRoot, _T("IEForward"), &m_btnForward, TRUE, FALSE);
		m_bIEStop = LoadButton(pXmlRoot, _T("IEStop"), &m_btnStop, TRUE, FALSE);
		m_bIERefresh = LoadButton(pXmlRoot, _T("IERefresh"), &m_btnRefresh, TRUE, FALSE);
	}
}

void CWebWindow::OnClose() 
{
	if(m_pXMLDoc)
		return; 

	if(!m_bChild)
	{
		RemoveWindowToClose(this);
		if(m_bAutoKill)
		{
			if(GetStyle()&WS_VISIBLE && GetOptionInt(IDS_OFSMESSENGER, IDS_ANIMATION, FALSE))
				RoundExitAddon(this);
			WEB_WINDOW_PARENT::OnClose();
			DestroyWindow();
			delete this;
		}
		else
			WEB_WINDOW_PARENT::OnClose();
	}
}

void CWebWindow::OnDestroy() 
{
	Unadvise();
	m_MpaWebEvent.m_pParent = NULL;

	//////////////////////////////////////////////////////////////////////////
	// Rem: [4/22/2002] Обязательно очищать перед  WEB_WINDOW_PARENT::OnDestroy();
	// иначе не будет удалятьмя объект m_InWindowDropTarget
	//////////////////////////////////////////////////////////////////////////
	if(m_InWindowDropTarget)
	{
		m_InWindowDropTarget->Revoke();
		m_InWindowDropTarget->ExternalRelease();
	}
	//////////////////////////////////////////////////////////////////////////

	WEB_WINDOW_PARENT::OnDestroy();
}

void CWebWindow::InitMpaWebEvent()
{
	IConnectionPointContainer* pCPContainer = NULL;
	IConnectionPointPtr  m_pSessionConnectionPoint;
	HRESULT hr = m_pWebCustomizer->QueryInterface(IID_IConnectionPointContainer,(void**)&pCPContainer);
	
	if (pCPContainer)
	{
		hr = pCPContainer->FindConnectionPoint(__uuidof(_IMpaWebCustomizerEvents),&m_pSessionConnectionPoint);
		if(SUCCEEDED(hr))
		{
			LPUNKNOWN pInterEvent = m_MpaWebEvent.GetInterface(&IID_IUnknown);
			hr = m_pSessionConnectionPoint->Advise(pInterEvent ,&m_dwSessionCookie);
			pCPContainer->Release();
		}
		else
		{
			ASSERT(FALSE);
		}
	}
	else
	{
		ASSERT(FALSE);
	}
}

///////////////////////////////////////////////////
// Web event handlers
///////////////////////////////////////////////////
void CWebWindow::OnCmdAddCompany(long company_id, LPCTSTR company_name, long role_id, LPCTSTR role_name)
{
	CString str;
	str.Format(_T("CmdAddCompany(%ld, %s, %ld, %s)"), company_id, company_name, role_id, role_name);
	AfxMessageBox(str);
}

void CWebWindow::OnCmdAddProduct(long product_id, LPCTSTR product_name)
{
	CString str;
	str.Format(_T("CmdAddProduct(%ld, %s)"), product_id, product_name);
	AfxMessageBox(str);
}

void CWebWindow::OnCmdAddContact(long user_id, LPCTSTR nick_name, LPCTSTR first_name, LPCTSTR last_name, LPCTSTR email, long company_id, LPCTSTR company_name, long role_id, LPCTSTR role_name)
{
	
//	CString str;
//	str.Format(_T("CmdAddContact(%ld, %s, %s, %s, %s, %ld, %s, %ld, %s)"), user_id, nick_name, first_name, last_name, email, company_id, company_name, role_id, role_name);
//	AfxMessageBox(str);
	DHTMLE_ADDCONTACT_Container *pData = new DHTMLE_ADDCONTACT_Container;

	pData->email        = email;
	pData->first_name   = first_name;
	pData->last_name    = last_name;
	pData->nick_name    = nick_name;
	pData->role_id      = role_id;
	pData->role_name    = role_name;
	pData->user_id      = user_id;

	((CMainDlg*)GetMessageParent())->SendMessage(WM_DHTML_EVENT,(WPARAM)DHTMLE_ADDCONTACT,(LPARAM)pData);
}

void CWebWindow::OnCmdNewWindow(long x, long y, long cx, long cy, LPCTSTR title, LPCTSTR url, BOOL bModal, BOOL bTopMost,  BOOL bResizable )
{
	CString strUrl	=	url;
	strUrl.MakeLower();

	int iIncident	=	strUrl.Find(_T("incidents%2fincidentedit.aspx"));
	
	int iToDo	=	strUrl.Find(_T("todo%2ftodoedit.aspx"));
	int iCalendar	=	strUrl.Find(_T("events%2feventedit.aspx"));
	int iFileUpload	=	strUrl.Find(_T("filelibrary%2fdefault.aspx"));
	int iFileUpload2	=	strUrl.Find(_T("filelibrary%2fdefault.aspx%3f"));
	int iProject	=	strUrl.Find(_T("projects%2fprojectedit.aspx"));
	int iList		=	strUrl.Find(_T("lists%2flistedit.aspx"));

	int iIncidentCapture	=	strUrl.Find(_T("incidents%2fincidentscreencaptureedit.aspx"));

	if(((CMainDlg*)GetMessageParent())->IsToolboxIstalled() &&
		(iIncident>=0||
		iToDo>=0 ||
		iCalendar>=0 ||
		(iFileUpload>=0 && iFileUpload!=iFileUpload2)||
		iProject>=0 || 
		iIncidentCapture>=0 ||
		iList>=0)
		)
	{
		// Run Toolbox [3/26/2004]
		CString strToolboxPath	=	((CMainDlg*)GetMessageParent())->ToolboxPath();

		CString strParametrs;

		if(iIncident>=0)
		{
			strParametrs.Format(_T("/CREATEINCIDENT /L \"%s\" /P \"%s\""),((CMainDlg*)GetMessageParent())->m_DlgLog.m_LoginStr, ((CMainDlg*)GetMessageParent())->m_DlgLog.m_PasswordStr);
		}
		else if(iToDo>=0)
		{
			strParametrs.Format(_T("/CREATETODO /L \"%s\" /P \"%s\""),((CMainDlg*)GetMessageParent())->m_DlgLog.m_LoginStr, ((CMainDlg*)GetMessageParent())->m_DlgLog.m_PasswordStr);
		}
		else if(iCalendar>=0)
		{
			strParametrs.Format(_T("/CREATEEVENT /L \"%s\" /P \"%s\""),((CMainDlg*)GetMessageParent())->m_DlgLog.m_LoginStr, ((CMainDlg*)GetMessageParent())->m_DlgLog.m_PasswordStr);
		}
		else if(iFileUpload>=0)
		{
			strParametrs.Format(_T("/UPLOAD /L \"%s\" /P \"%s\""),((CMainDlg*)GetMessageParent())->m_DlgLog.m_LoginStr, ((CMainDlg*)GetMessageParent())->m_DlgLog.m_PasswordStr);
		}
		else if(iProject>=0)
		{
			strParametrs.Format(_T("/CREATEPROJECT /L \"%s\" /P \"%s\""),((CMainDlg*)GetMessageParent())->m_DlgLog.m_LoginStr, ((CMainDlg*)GetMessageParent())->m_DlgLog.m_PasswordStr);
		}
		else if(iIncidentCapture>=0)
		{
			CScreenShotDlg	*pScreenShotDlg	=	new CScreenShotDlg(((CMainDlg*)GetMessageParent()),CScreenShotDlg::CreateIssue);
			pScreenShotDlg->Create(CScreenShotDlg::IDD,GetDesktopWindow());
			pScreenShotDlg->ShowWindow(SW_NORMAL);
			AddWindowToClose(pScreenShotDlg);
			return;
		}
		else if(iList>=0)
		{
			strParametrs.Format(_T("/CREATELIST /L \"%s\" /P \"%s\""),((CMainDlg*)GetMessageParent())->m_DlgLog.m_LoginStr, ((CMainDlg*)GetMessageParent())->m_DlgLog.m_PasswordStr);
		}
		
		if(((CMainDlg*)GetMessageParent())->IsSSLMode())
			strParametrs	+= _T(" /USESSL");
		
		ShellExecute(::GetDesktopWindow(),NULL,strToolboxPath,strParametrs,NULL,SW_SHOWNORMAL);
	}
	else
	{
		int m_lIBNActionBrowser = 1;//GetOptionInt(IDS_OFSMESSENGER,IDS_IBNACTIONBROWSER,1);
		
		if(m_lIBNActionBrowser==0)
		{
			if(cx==0&&cy==0)
			{
				CRect rd;
				SystemParametersInfo(SPI_GETWORKAREA, 0, &rd, 0);
				
				cx = rd.Width()-256;
				cy = rd.Height()-128;
			}
			
			CWebWindow *pNewWindow = new CWebWindow;
			pNewWindow->CreateAutoKiller(_T("/Browser/Common/skin.xml"), GetMessageParent(), GetDesktopWindow(), x, y, cx, cy, title, url, bModal, bTopMost, bResizable);
			
			if(IsBadReadPtr(NULL,sizeof(COfsDhtmlEditCtrl)))
			{
				ASSERT(FALSE);
			}
			
			if(GetOptionInt(IDS_OFSMESSENGER,IDS_OPENINMAXIMAZE, TRUE))
			{
				pNewWindow->ShowWindow(SW_MAXIMIZE);
			}
		}
		else
		{
			if(S_OK!=this->NavigateNewWindow(url))
				ShellExecute(NULL,_T("open"),url,NULL,NULL,SW_SHOWNORMAL);
		}
	}
}

void CWebWindow::OnCmdBeginPromo(long product_id, LPCTSTR product_name)
{
	CString str;
	str.Format(_T("CmdBeginPromo(%ld, %s)"), product_id, product_name);
	AfxMessageBox(str);
}

void CWebWindow::OnCmdProductDetails(long product_id, LPCTSTR product_name, long role_id, LPCTSTR role_name)
{
	CString str;
	str.Format(_T("CmdProductDetails(%ld, %s, %ld)"), product_id, product_name,role_id);
	AfxMessageBox(str);
}

void CWebWindow::OnCmdCompanyDetails(long company_id, LPCTSTR company_name, long role_id, LPCTSTR role_name)
{
	CString str;
	str.Format(_T("CmdCompanyDetails(%ld, %s, %ld, %s)"), company_id, company_name,role_id, role_name);
	AfxMessageBox(str);
}

void CWebWindow::OnCmdBeginDialogue(long user_id, LPCTSTR NickName, LPCTSTR FirstName, LPCTSTR LastName, LPCTSTR Email, 
		long company_id, LPCTSTR company_name, long role_id, LPCTSTR role_name)
{
	CString str;
	str.Format(_T("CmdBeginDialogue(%ld, %s, %s, %s, %s, %ld, %s %ld, %s)"), user_id, NickName, FirstName, LastName,
		Email, company_id, company_name, role_id, role_name);
	AfxMessageBox(str);
}

void CWebWindow::OnCmdUserDetails(long user_id, LPCTSTR NickName, LPCTSTR FirstName, LPCTSTR LastName, 
	LPCTSTR Email, long company_id, LPCTSTR company_name, long role_id, LPCTSTR role_name)
{
	CString str;
	str.Format(_T("CmdUserDetails(%ld, %s, %s, %s, %s, %ld, %s %ld, %s)"), user_id, NickName, FirstName, LastName,
		Email, company_id, company_name, role_id, role_name);
	AfxMessageBox(str);
//	CUserDetailsDlg *m_pNewDlg;
}

void CWebWindow::OnCmdLoadPromos(BSTR* pPromoList)
{
//	CString str;
//	str.Format(_T("CmdLoadPromos"));
//	AfxMessageBox(str);

	//(*pPromoList) = ((CMainDlg*)GetMessageParent())->GetPromoList().copy();
}

void CWebWindow::OnCmdGetUserID(long* id)
{
//	CString str;
//	str.Format(_T("CmdGetUserID"));
//	AfxMessageBox(str);
	(*id) = ((CMainDlg*)GetMessageParent())->GetUserID();
}

void CWebWindow::OnCmdGetUserRole(long* id)
{
	(*id) = ((CMainDlg*)GetMessageParent())->GetRoleID();
}

void CWebWindow::OnCmdSendPromo(long longProductID, LPCTSTR bstrSubject, LPCTSTR bstrMessage, LPCTSTR bstrRecipients,long *pHandle)
{
//    ::CoInitialize(NULL);
	m_SendStatus = DS_NONE;
///	if(::IsWindow(pMessageParent->GetSafeHwnd()))
//		if(!((CMainDlg*)pMessageParent)->m_MessengerBar.ConnectEnable())
//		{
//			/// WriteMessage Dialog About you OffLine...
//			pMessageParent->SendMessage(WM_SHOWMESSAGEBOX,(WPARAM)IDS_YOUOFFLINE);
//			return;
//		}
	try
	{	
		ISessionPtr pSession = NULL;
		pSession = theNet2.GetSession();
		IPromoPtr pPromo = pSession->CreatePromo();
		pPromo->PutPID(_bstr_t(GUIDGen()));
		pPromo->PutBody(_bstr_t(bstrMessage));
		pPromo->PutProduct_ID(longProductID);
		pPromo->PutSubject(_bstr_t(bstrSubject)) ;
		IUsersPtr pUsers = pPromo->GetRecipients();
		CString str = bstrRecipients;
		while(!str.IsEmpty())
		{
			IUserPtr pUser = pUsers->AddUser();
			long UserId = _tstol((LPCTSTR)str);
			pUser->PutValue("@id",UserId);
			int FindPos = str.Find(',');
			if(FindPos==-1) break;
			str = str.Mid(FindPos + 1);
		}
		
		theNet2.LockTranslator();
		try
		{
			m_Handle = pPromo->Send();
			if(m_Handle)
				theNet2.AddToTranslator(m_Handle,this->GetSafeHwnd());
			m_SendStatus = DS_SEND;
			(*pHandle) = m_Handle;
		}
		catch(...)
		{
		}
		theNet2.UnlockTranslator();
	}
	catch(...)
	{
	}
	
//	::CoUninitialize();
}

void CWebWindow::OnCmdGetProgramSettings(long longSettingsID, BSTR* bstrSettingsXml)
{
	CString str;
	str.Format(_T("CmdGetProgramSettings ( %ld)"),longSettingsID);
	AfxMessageBox(str);
	bstrSettingsXml = NULL;
}

void CWebWindow::OnCmdSendMessage(long londUserID, LPCTSTR bstrNickName, LPCTSTR bstrRole)
{
//	CString str;
//	str.Format(_T("CmdSendMessage ( %ld, %s, %s)"),londUserID, bstrNickName, bstrRole);
//	AfxMessageBox(str);
	DHTMLE_SENDMESSAGE_Container *pData = new DHTMLE_SENDMESSAGE_Container;
	pData->longUserID  = londUserID;
	pData->strNickName = bstrNickName;
	pData->strRole     = bstrRole;
	((CMainDlg*)GetMessageParent())->SendMessage(WM_DHTML_EVENT,(WPARAM)DHTMLE_SENDMESSAGE,(LPARAM)pData);
}

void CWebWindow::OnCmdSetProgramSettings(long longSettingsID, LPCTSTR bstrSettingsXml)
{
	CString str;
	str.Format(_T("CmdSetProgramSettings ( %ld, %s)"),longSettingsID, bstrSettingsXml);
	AfxMessageBox(str);
}

void CWebWindow::OnCmdSendFile(long longUserID, LPCTSTR bstrNickName, LPCTSTR bstrRole)
{
	DHTMLE_SENDMESSAGE_Container *pData = new DHTMLE_SENDMESSAGE_Container;
	pData->longUserID  = longUserID;
	pData->strNickName = bstrNickName;
	pData->strRole     = bstrRole;
	((CMainDlg*)GetMessageParent())->SendMessage(WM_DHTML_EVENT,(WPARAM)DHTMLE_SENDFILE,(LPARAM)pData);
}

void CWebWindow::OnCmdSetVariable(LPCTSTR bstrVarName, LPCTSTR bstrVarValue)
{
	USES_CONVERSION;
	
	if(CString("Invoke_StartAutoUpdate").CompareNoCase(bstrVarName)==0)
	{
		_bstr_t VarValue = bstrVarValue;
		//((CMainDlg*)GetMessageParent())->Invoke_StartAutoUpdate(bstrVarValue);
		GetMessageParent()->PostMessage(WM_INVOKE_STARTAUTO_UPDATE,(WPARAM)VarValue.copy());
		PostMessage(WM_CLOSE);
	}
	else if(CString("LoadXML").CompareNoCase(bstrVarName)==0)
	{
		m_bQueryAttachXML =TRUE;
		LoadXML(bstrVarValue);
	}
	else if(CString(_T("_LoadDropFiles")).CompareNoCase(bstrVarName)==0)
	{
		// Load Script Function bstrVarValue, and SendFiles Array [4/22/2002]
		CComPtr<IHTMLDocument>	pDoc;
		pDoc.Attach((IHTMLDocument*)m_browser.GetDocument());
		if(pDoc!=NULL)
		{
			CComPtr<IDispatch> spDispScript = NULL;
			HRESULT hr = pDoc->get_Script(&spDispScript);
			if(spDispScript)
			{
				DISPID dispid = -1;
				OLECHAR FAR* szMember = T2OLE(const_cast<LPTSTR>(bstrVarValue));
				hr = spDispScript->GetIDsOfNames(IID_NULL, &szMember, 1, LOCALE_SYSTEM_DEFAULT, &dispid);
				if(SUCCEEDED(hr))
				{
					CComVariant varResult;
					
					DISPPARAMS params = { &varFilesData, NULL, 1, 0 };
					hr = spDispScript->Invoke(dispid, IID_NULL, LOCALE_USER_DEFAULT, DISPATCH_METHOD, &params, &varResult, NULL, NULL);
				}
			}
		}
	}	
	else if(CString("Invoke_CreateChat").CompareNoCase(bstrVarName)==0)
	{
		// <строка вида> = "Name#Description#Message#UserId,UserId,UserId" [11/5/2002]
		_bstr_t VarValue = bstrVarValue;
		GetMessageParent()->PostMessage(WM_INVOKE_CREATECHAT,(WPARAM)VarValue.copy());
	}
	else if(CString("Invoke_SendMessage").CompareNoCase(bstrVarName)==0)
	{
		// <строка вида> = "Message#UserId,UserId,UserId" [11/5/2002]
		_bstr_t VarValue = bstrVarValue;
		GetMessageParent()->PostMessage(WM_INVOKE_SENDMESSAGE,(WPARAM)VarValue.copy());
	}
	else
		g_ViewVariablesList.SetVariable(bstrVarName,bstrVarValue);
}

void CWebWindow::OnCmdGetVariable(LPCTSTR bstrVarName, BSTR* bstrVarValue)
{
	if(_tcscmp(bstrVarName, _T("CurrentSkin"))==0)
	{
		bstr_t strReturn = (LPCTSTR)GetCurrentSkin();
		(*bstrVarValue) = strReturn.copy();
	}
	else if(lstrcmp(bstrVarName, _T("Hash")) == 0)
	{
		TCHAR buf[1024];
		buf[0] = 0;
		GetEnvironmentVariable(_T("MpaKey"), buf, 1024);
		bstr_t strReturn = buf;
		(*bstrVarValue) = strReturn.copy();
	}
	else if(_tcscmp(bstrVarName, _T("bandWidthID"))==0)
	{
		int bandwidthID = GetOptionInt(IDS_NETOPTIONS,IDS_BANDWIDTH,2);
		CString strTmp;	strTmp.Format(_T("%d"), bandwidthID);
		bstr_t strReturn = (LPCTSTR)strTmp;
		(*bstrVarValue) = strReturn.copy();
	}
	else if(_tcscmp(bstrVarName, _T("SID"))==0)
	{
		bstr_t strReturn = (LPCTSTR)(((CMainDlg*)GetMessageParent())->GetSID());
		(*bstrVarValue) = strReturn.copy();
	}
	else if(_tcscmp(bstrVarName, _T("HOST"))==0)
	{
		bstr_t strReturn = (LPCTSTR)(((CMainDlg*)GetMessageParent())->GetServerPath());
		(*bstrVarValue) = strReturn.copy();
	}
	else if(_tcscmp(bstrVarName, _T("AttachXML"))==0)
	{
		(*bstrVarValue) = m_AttachLoadXML.Copy();
		m_bQueryAttachXML = TRUE;
	}
	else 
		g_ViewVariablesList.GetVariable(bstrVarName,bstrVarValue);
}

void CWebWindow::OnCmdCancelOperation(long Handle)
{
	if(Handle == this->m_Handle)
	{
		ISessionPtr pSession = NULL;
		pSession = theNet2.GetSession();
		try
		{
			pSession->CancelOperation(Handle);
		}
		catch(...)
		{
		}
	}
}

void CWebWindow::OnCmdCheckStatus(long Handle, long *Result)
{
	if(Handle == this->m_Handle)
		(*Result) = m_SendStatus;
	else
		(*Result) = DS_NONE;
	
	if(m_SendStatus == DS_COMPLETE)
	{
		theNet2.LockTranslator();
		theNet2.RemoveFromTranslator(Handle);
		theNet2.UnlockTranslator();
		m_SendStatus = DS_NONE;
	}
}

void CWebWindow::OnCmdMainWindowNavigate(LPCTSTR URL, long bClose)
{
	bstr_t strUrl = URL;
	GetMessageParent()->PostMessage(WM_MAINFRAME_NAVIGATE,(WPARAM)(strUrl.copy()));
	if(bClose)
		PostMessage(WM_CLOSE);
}

void CWebWindow::OnCmdShowContextMenu(long dwID, long x, long y, IUnknown* pcmdTarget, IDispatch* pdispReserved, long* pShow)
{
	*pShow	=	0;
	
    HRESULT		hr	=	S_OK;
    HINSTANCE	hinstSHDOCLC;
    HWND		hwnd;
    HMENU		hMenu;
	
    CComPtr<IOleCommandTarget> spCT;
    CComPtr<IOleWindow> spWnd;
    
    hr = pcmdTarget->QueryInterface(IID_IOleCommandTarget, (void**)&spCT);
	if(SUCCEEDED(hr))
	{
		hr = pcmdTarget->QueryInterface(IID_IOleWindow, (void**)&spWnd);
		if(SUCCEEDED(hr))
		{
			hr = spWnd->GetWindow(&hwnd);
			
			if(SUCCEEDED(hr))
			{
				hinstSHDOCLC = LoadLibrary(TEXT("SHDOCLC.DLL"));
				
				hMenu = LoadMenu(hinstSHDOCLC,    MAKEINTRESOURCE(24641));
				
				hMenu = GetSubMenu(hMenu, 4);
				
				int iSelection = ::TrackPopupMenu(hMenu,
					TPM_LEFTALIGN | TPM_RIGHTBUTTON | TPM_RETURNCMD,  x, y, 0, hwnd, (RECT*)NULL);
				
				// Пересылаем выбранную команду окну броузера
				LRESULT lr = ::SendMessage(hwnd, WM_COMMAND, iSelection, NULL);
				
				FreeLibrary(hinstSHDOCLC);
			}
		}
	}
}

void CWebWindow::OnCmdDoAction(LPCTSTR ActionName, LPCTSTR Params, BSTR* Result)
{
	if(ActionName == NULL)
		return;
	if(0 == _tcsicmp(ActionName, _T("Window.Close")))
	{	
		if(m_nIEVersion >= 550)	// This function is only for compatibility with IE 5.0
			return;

		PostMessage(WM_CLOSE);
		return;
	}
	if(0 == _tcsicmp(ActionName, _T("Window.SetSize")))
	{
		// Params = string: Width,Height,IsResizable

		//if(m_nIEVersion >= 550)	// This function is only for compatibility with IE 5.0
		//	return;

		CRect r;
		long cx=-1, cy=-1, resizable=-1;
		_stscanf(Params, _T("%d,%d,%d"), &cx, &cy, &resizable);
		if(cx < 0 || cy < 0 || resizable < 0)
			return;
		m_bResizable = resizable != 0;
		m_browser.GetWindowRect(&r);
		r.right = r.left + cx;
		r.bottom = r.top + cy;
		CenterRect(r);
		SetBrowserRect(r);
		return;
	}
	if(0 == _tcsicmp(ActionName, _T("FileUpload")))
	{
		// Params = XML with upload parameters
		CWnd* pWnd = GetMessageParent();
		if(pWnd)
			pWnd->SendMessage(WM_UPLOAD_APP_FILE, reinterpret_cast<WPARAM>(Params));
	}
}
///////////////////////////////////////////////////////////////////////
// End of web handlers
///////////////////////////////////////////////////////////////////////

CWnd* CWebWindow::GetMessageParent() const
{
	if(m_bAutoKill)
		return m_pMessageParent;
	else
		return GetParent();
}


HRESULT CWebWindow::OnXMLLoadCopleted(WPARAM w,LPARAM l)
{
	m_AttachLoadXML.Empty();

	if(w==0)
	{
		m_pXMLDoc->GetXML(&m_AttachLoadXML);

		TryLoadXML();							
	}
	
	delete m_pXMLDoc;
	m_pXMLDoc = NULL;
	
	return 0;
}

void CWebWindow::Unadvise()
{
	IConnectionPointContainer* pCPContainer = NULL;
	IConnectionPointPtr  m_pSessionConnectionPoint;
	HRESULT hr = m_pWebCustomizer->QueryInterface(IID_IConnectionPointContainer,(void**)&pCPContainer);
	
	if (pCPContainer)
	{
		hr = pCPContainer->FindConnectionPoint(__uuidof(_IMpaWebCustomizerEvents),&m_pSessionConnectionPoint);
		if(SUCCEEDED(hr))
		{
			//			LPUNKNOWN pInterEvent = m_MpaWebEvent.GetInterface(&IID_IUnknown);
			//			hr = m_pSessionConnectionPoint->Advise(pInterEvent ,&m_dwSessionCookie);
			hr = m_pSessionConnectionPoint->Unadvise(m_dwSessionCookie);
			pCPContainer->Release();
		}
		else
		{
			ASSERT(FALSE);
		}
	}
	else
	{
		ASSERT(FALSE);
	}
}

void CWebWindow::SetVariable(LPCTSTR Name, LPCTSTR Value)
{
	g_ViewVariablesList.SetVariable(Name,Value);
}

void CWebWindow::OnClickButtonX() 
{
	OnCancel();
}

void CWebWindow::OnClickButtonMax() 
{
	ShowWindow(SW_SHOWMAXIMIZED);
}

void CWebWindow::OnClickButtonRestore() 
{
	ShowWindow(SW_SHOWNORMAL);
}

void CWebWindow::OnClickButtonMin() 
{
	ShowWindow(SW_SHOWMINIMIZED);
}

void CWebWindow::OnClickButtonBack() 
{
	m_browser.GoBack();
}

void CWebWindow::OnClickButtonForward() 
{
	m_browser.GoForward();
}

void CWebWindow::OnClickButtonStop() 
{
	m_browser.Stop();
}

void CWebWindow::OnClickButtonRefresh() 
{
	m_browser.Refresh();
}

/*BOOL CWebWindow::OnToolTipNotify(UINT id, NMHDR *pNMHDR, LRESULT *pResult)
{
	TOOLTIPTEXTA* pTTTA = (TOOLTIPTEXTA*)pNMHDR;
	TOOLTIPTEXTW* pTTTW = (TOOLTIPTEXTW*)pNMHDR;
	
	CString strTipText;

	HWND hWnd = reinterpret_cast<HWND>(pNMHDR->idFrom);
	
	if(hWnd != 0) // will be zero on a separator
	{
		//		strTipText.Format("Control ID = %d", nID);
		//strTipText.LoadString(nID);
		if(hWnd == m_btnX.m_hWnd)
			strTipText = GetString(IDS_TIP_CLOSE);
		if(hWnd == m_btnMax.m_hWnd)
			strTipText = GetString(IDS_TIP_MAXIMIZE);
		if(hWnd == m_btnRestore.m_hWnd)
			strTipText = GetString(IDS_TIP_RESTORY);
		if(hWnd == m_btnMin.m_hWnd)
			strTipText = GetString(IDS_TIP_MINIMIZE);
		if(hWnd == m_btnBack.m_hWnd)
			strTipText = GetString(IDS_TIP_BACK);
		if(hWnd == m_btnForward.m_hWnd)
			strTipText = GetString(IDS_TIP_FORWARD);
		if(hWnd == m_btnStop.m_hWnd)
			strTipText = GetString(IDS_TIP_STOP);
		if(hWnd == m_btnRefresh.m_hWnd)
			strTipText = GetString(IDS_TIP_REFRESH);
	}
	
	if(pNMHDR->code == TTN_NEEDTEXTA)
		lstrcpyn(pTTTA->szText, strTipText, sizeof(pTTTA->szText));
	else
		_mbstowcsz(pTTTW->szText, strTipText, sizeof(pTTTW->szText));
	*pResult = 0;
	
	return TRUE;    // message was handled
}*/

void CWebWindow::OnWebProgressChange(long lProgress, long lProgressMax)
{
	//if(lProgress==0)
	//{
		///m_bDocumentComplete = TRUE;
		//TryLoadXML();
	//}
}

void CWebWindow::TryLoadXML()
{
	if(m_bQueryAttachXML&&m_AttachLoadXML.Length())
	{
		HRESULT hr = S_OK;
		IUnknownPtr spUnk = m_browser.GetControlUnknown();
		/////////
		// Run Script
		////////
		if(spUnk)
		{
			CComQIPtr<IWebBrowser2, &IID_IWebBrowser2> spWB2(spUnk);
			if(spWB2)
			{
				IDispatchPtr spDispDoc = NULL;
				hr = spWB2->get_Document(&spDispDoc);
				if(spDispDoc)
				{
					CComQIPtr<IHTMLDocument, &IID_IHTMLDocument> spDoc(spDispDoc);
					if(spDoc)
					{
						IDispatchPtr spDispScript = NULL;
						hr = spDoc->get_Script(&spDispScript);
						if(spDispScript)
						{
							DISPID dispid = -1;
							OLECHAR FAR* szMember = L"LoadXML";
							hr = spDispScript->GetIDsOfNames(IID_NULL, &szMember, 1, LOCALE_SYSTEM_DEFAULT, &dispid);
							if(SUCCEEDED(hr))
							{
								
								CComVariant* pvars = new CComVariant[1];
								pvars[0] = CComVariant((BSTR)m_AttachLoadXML);
								
								DISPPARAMS params = { pvars, NULL, 1, 0 };
								hr = spDispScript->Invoke(dispid, IID_NULL, LOCALE_USER_DEFAULT, DISPATCH_METHOD, &params, NULL, NULL, NULL);
								delete []pvars;
							}
						}
					}
				}
			}
		}
	}
}

void CWebWindow::OnWebBeforeNavigate2(LPDISPATCH pDisp, VARIANT FAR* URL, VARIANT FAR* Flags, VARIANT FAR* TargetFrameName, VARIANT FAR* PostData, VARIANT FAR* Headers, BOOL FAR* Cancel)
{
	m_bNavigateStarted = TRUE;
	if(!m_bChild && m_bFileDownload)
	{
		PostMessage(WM_CLOSE);
		return;
	}
	if(!m_bCatchNavigate)
	{
		m_bCatchNavigate = TRUE;
		return;
	}
	
	long cx = 0, cy = 0, tearoff = -1, fullscreen = 0;
	GetTargetRect(pDisp, cx, cy, tearoff, fullscreen);
	
	/*if(IsTVFileType(URL->bstrVal))
	{
		CDlgTV *pDlg = new CDlgTV;
		if(pDlg)
		{
			pDlg->CreateAutoKiller(URL->bstrVal, GetDesktopWindow(), cx, cy);
			*Cancel = VARIANT_TRUE;
		}

	}
	else*/ if(tearoff == 0)
	{
		m_bCatchWindowOpen = FALSE;
		*Cancel = TRUE;
		CString str(URL->bstrVal);
		if(S_OK != ::NavigateNewWindow(pDisp, str))
			ShellExecute(::GetDesktopWindow(), _T("open"), str, NULL, NULL, SW_SHOWDEFAULT);
	}
	else if(tearoff == 1)
	{
		CWebWindow *pWnd = new CWebWindow;

		CRect winRect;
		GetWindowRect(&winRect);
			
		pWnd->CreateAutoKiller(_T("/Browser/Common/skin.xml"), GetMessageParent(), GetDesktopWindow(), winRect.left-100, winRect.top+20, cx, cy, NULL, CString(URL->bstrVal), FALSE, FALSE, TRUE, 0, FALSE, TRUE, fullscreen!=0);
		*Cancel = VARIANT_TRUE;
	}
	else if(!m_bChild && m_strURL.IsEmpty()) // if called window.open()
	{
		CRect r = m_rTarget;
		if(!m_bSizeSet)
		{
			r.right = m_rBrowser.Width();
			r.bottom = m_rBrowser.Height();
		}
		if(!m_bPositionSet)
			CenterRect(r);
		SetBrowserRect(r);
		ShowWindow(SW_SHOW);
		SetForegroundWindow();
		SetActiveWindow();
		m_strURL = CString(URL->bstrVal);
	}
}

void CWebWindow::OnWebNewWindow2(LPDISPATCH FAR* ppDisp, BOOL FAR* Cancel)
{
	//int m_lIBNActionBrowser = GetOptionInt(IDS_OFSMESSENGER,IDS_IBNACTIONBROWSER,1);

	//if(m_bChild && m_lIBNActionBrowser==1)
	//	return;

	if(m_bChild)
		return;
	
	CComBSTR bs;
	g_ViewVariablesList.GetVariable(_T("UseGeneralWindow"), &bs);
	if(!m_bCatchWindowOpen || (bs.m_str != NULL && 0 == wcscmp(bs.m_str, L"1")))
	{
		m_bCatchWindowOpen = TRUE;
		return;
	}
	
	LPDISPATCH pDispatch = NULL;
	IUnknown *pUnk = NULL;
	CWebWindow *pNewWindow = new CWebWindow;
	//	pNewWindow->CreateWebWindow(this, _T("/Browser/Common/skin.xml"));
	pNewWindow->CreateAutoKiller(_T("/Browser/Common/skin.xml"), GetMessageParent(), GetDesktopWindow(), 0, 0, 0, 0, NULL, NULL, FALSE, FALSE, TRUE);
	pUnk = pNewWindow->m_browser.GetControlUnknown();
	if(pUnk)
	{
		pUnk->QueryInterface(__uuidof(IDispatch), (void**)&pDispatch);
		if(pDispatch)
			*ppDisp = pDispatch;
		pUnk->Release();
	}
}

void CWebWindow::OnWebWindowClosing(BOOL IsChildWindow, BOOL FAR* Cancel)
{
	PostMessage(WM_CLOSE);
}

void CWebWindow::OnWebWindowSetResizable(BOOL Resizable)
{
	m_bResizable = Resizable;
	m_btnMax.EnableWindow(Resizable);
	m_btnRestore.EnableWindow(Resizable);
}

void CWebWindow::OnWebWindowSetHeight(long Height)
{
	m_rTarget.bottom = m_rTarget.top + Height;
	m_bSizeSet = TRUE;
}

void CWebWindow::OnWebWindowSetWidth(long Width)
{
	m_rTarget.right = m_rTarget.left + Width;
	m_bSizeSet = TRUE;
}

void CWebWindow::OnWebWindowSetTop(long Top)
{
	m_rTarget.OffsetRect(0, Top - m_rTarget.top);
	m_bPositionSet = TRUE;
}

void CWebWindow::OnWebWindowSetLeft(long Left)
{
	m_rTarget.OffsetRect(Left - m_rTarget.left, 0);
	m_bPositionSet = TRUE;
}

void CWebWindow::OnWebTitleChange(LPCTSTR Text)
{
	if(!m_bChild)
		SetTitle(Text);
}

void CWebWindow::OnWebNavigateComplete2(LPDISPATCH pDisp, VARIANT FAR* URL) 
{
	m_bNavigateStarted = FALSE;
	m_bFileDownload = FALSE;
}

void CWebWindow::OnWebToolBar(BOOL ToolBar) 
{
	m_bShowToolbar = ToolBar;
	ShowToolbar(m_bShowToolbar);
}

void CWebWindow::OnSize(UINT nType, int cx, int cy) 
{
	WEB_WINDOW_PARENT::OnSize(nType, cx, cy);
	
	if(m_bChild)
	{
		if(IsWindow(m_browser.GetSafeHwnd()))
			m_browser.SetWindowPos(NULL, 0, 0, cx, cy, SWP_NOZORDER);
	}
	else
	{
		if(m_rBrowser.IsRectEmpty() || m_rWindow.IsRectEmpty())
			return;
		
		if(nType == SIZE_MAXIMIZED || nType == SIZE_RESTORED)
		{
			long x, y, cx, cy;
			CRect rw;
			GetWindowRect(&rw);
			x = m_rBrowser.left - m_rWindow.left;
			y = m_rBrowser.top - m_rWindow.top;
			cx = m_rBrowser.Width() + rw.Width() - m_rWindow.Width();
			cy = m_rBrowser.Height() + rw.Height() - m_rWindow.Height();
			ASSERT(x != 986);
			m_browser.SetWindowPos(NULL, x, y, cx, cy, SWP_NOZORDER);
		}
	}
}

void CWebWindow::SetBrowserRect(CRect r)
{
	if(!m_bChild)
	{
		r.OffsetRect(m_rWindow.left - m_rBrowser.left, m_rWindow.top - m_rBrowser.top);
		r.InflateRect(0, 0, m_rWindow.Width() - m_rBrowser.Width(), m_rWindow.Height() - m_rBrowser.Height());
		AdjustRect(r);
		SetWindowPos(NULL, r.left, r.top, r.Width(), r.Height(), SWP_NOZORDER);
	}
}

void CWebWindow::ShowToolbar(BOOL bShow)
{
	int nCmdShow = m_bShowToolbar ? SW_SHOW : SW_HIDE;
	if(m_bIEBack)
		m_btnBack.ShowWindow(nCmdShow);
	if(m_bIEForward)
		m_btnForward.ShowWindow(nCmdShow);
	if(m_bIEStop)
		m_btnStop.ShowWindow(nCmdShow);
	if(m_bIERefresh)
		m_btnRefresh.ShowWindow(nCmdShow);
}

void CWebWindow::OnPaint() 
{
	if(!m_bChild)
	{
		WEB_WINDOW_PARENT::OnPaint();
		m_title.Invalidate();
	}
	else
	{
		CPaintDC	dc(this);
	}
	
}

void CWebWindow::SetTitle(LPCTSTR szTitle)
{
	CString str;
	if(!m_strTitle.IsEmpty())
		str = m_strTitle;
	else if(szTitle == NULL || _tcslen(szTitle) == 0)
		str = GetString(IDR_MAINFRAME);
	else
	{
		str.Format(GetString(IDS_WEBBROWSER_TITLE_FORMAT),szTitle);
	}
	m_title.SetText(str);
	SetWindowText(str);
}

BOOL CWebWindow::CreateAsChild(CWnd *pParent, CWnd *pMessageParent)
{
	m_bChild = TRUE;
	m_pMessageParent = pMessageParent;
	m_bLoadSkin = FALSE;
	
	ASSERT(pParent != NULL);
	
	if(!Create(IDD_WEB_WINDOW_CHILD, pParent))
	{
		TRACE0("Warning: failed to create CWebWindow.\n");
		return FALSE;
	}
	return TRUE;
}

void CWebWindow::OnCmdGetDropTarget(IDropTarget* pDropTarget, IDropTarget** ppDropTarget)
{
	ASSERT(m_InWindowDropTarget!=NULL);
	if(m_InWindowDropTarget)
	{
		*ppDropTarget = (IDropTarget*)m_InWindowDropTarget->GetInterface(&IID_IDropTarget);
		if(*ppDropTarget)
		{
			m_InWindowDropTarget->ExternalAddRef();
		}
	}
}

BOOL CWebWindow::Navigate(LPCTSTR Url)
{
	m_bCatchNavigate	=	FALSE;
	m_browser.Navigate(/*_T("about:blank")*/Url,0,0,0,0);
	return TRUE;
}


HRESULT CWebWindow::NavigateNewWindow(LPCTSTR strUrl, BOOL bSimpleMode)
{
	HRESULT	hr	=	E_FAIL;

	if(bSimpleMode)
		m_bCatchWindowOpen	=	FALSE;

	hr = ::NavigateNewWindow(m_browser.GetControlUnknown(),strUrl);

	if(FAILED(hr)&&bSimpleMode)
		m_bCatchWindowOpen	=	TRUE;

	return hr;		
}

void CWebWindow::OnWebFileDownload(BOOL b, BOOL FAR* Cancel)
{
	TRACE(_T("b = %d, n = %d\n"), b, m_nFileDownload);
	if(!b && 0 == m_nFileDownload && !m_bNavigateStarted)
	{
		PostMessage(WM_CLOSE);
	}
	else
	{
		m_nFileDownload = (b ? 1 : 0);
	}
}
