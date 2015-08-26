// InviteChatDlg.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "InviteChatDlg.h"
#include "GlobalFunction.h"
#include "MainDlg.h"
//#include "MainFrm.h"
#include "LoadSkins.h"
#include "ExDispid.h"
#include "ChatCreateDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CInviteChatDlg dialog
extern CString GetCurrentSkin();

CInviteChatDlg::CInviteChatDlg(CMainDlg* pParent)
: CResizableDialog(CInviteChatDlg::IDD, pParent)
{
	EnableAutomation();
	//{{AFX_DATA_INIT(CInviteChatDlg)
	//}}AFX_DATA_INIT
	Handle = 0;
	pMessenger = pParent;
	bIsKillWinodow = FALSE;
	m_dwSessionCookie	=	0;
}


void CInviteChatDlg::DoDataExchange(CDataExchange* pDX)
{
	CResizableDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CInviteChatDlg)
	//DDX_Control(pDX, IDC_FRAME_EDIT, m_frameEdit);
	DDX_Control(pDX, IDC_NICK_EDIT, m_Nick);
	DDX_Control(pDX, IDC_MCACCEPT, m_Accept);
	DDX_Control(pDX, IDC_MCDENY, m_Deny);
	DDX_Control(pDX, IDC_MCUSERDETAILS, m_Details);
	DDX_Control(pDX, IDC_EDIT_EXPLORER, m_edit);
	DDX_Control(pDX, IDC_MCCHATDETAILS,m_ChatDetails);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CInviteChatDlg, CResizableDialog)
//{{AFX_MSG_MAP(CInviteChatDlg)
//ON_BN_CLICKED(ID_DANY, OnDany)
ON_COMMAND(ID_EDITMENU_COPY, OnEditmenuCopy)
ON_UPDATE_COMMAND_UI(ID_EDITMENU_COPY, OnUpdateEditmenuCopy)
ON_COMMAND(ID_EDITMENU_CUT, OnEditmenuCut)
ON_UPDATE_COMMAND_UI(ID_EDITMENU_CUT, OnUpdateEditmenuCut)
ON_COMMAND(ID_EDITMENU_DELETE, OnEditmenuDelete)
ON_UPDATE_COMMAND_UI(ID_EDITMENU_DELETE, OnUpdateEditmenuDelete)
ON_COMMAND(ID_EDITMENU_PAST, OnEditmenuPast)
ON_UPDATE_COMMAND_UI(ID_EDITMENU_PAST, OnUpdateEditmenuPast)
ON_WM_CAPTURECHANGED()
ON_BN_CLICKED(IDC_MCUSERDETAILS, OnMcUserDetails)
ON_BN_CLICKED(IDC_MCACCEPT, OnMcAccept)
ON_WM_CANCELMODE()
ON_BN_CLICKED(IDC_MCDENY, OnMcDeny)
ON_BN_CLICKED(IDC_MCCHATDETAILS,OnMcChatDetails)
//}}AFX_MSG_MAP
ON_MESSAGE(WM_NLT_CONTAINER_EVENT,OnNetEvent)	
END_MESSAGE_MAP()

BEGIN_EVENTSINK_MAP(CInviteChatDlg, CResizableDialog)
//{{AFX_EVENTSINK_MAP(CInviteChatDlg)
ON_EVENT(CInviteChatDlg, IDC_EDIT_EXPLORER, DISPID_DOCUMENTCOMPLETE, OnWebDocumentCompleted, VTS_DISPATCH VTS_PVARIANT)
//}}AFX_EVENTSINK_MAP
END_EVENTSINK_MAP()

BEGIN_INTERFACE_MAP(CInviteChatDlg, CCmdTarget)
INTERFACE_PART(CInviteChatDlg, __uuidof(_IMpaWebCustomizerEvents), Dispatch)
END_INTERFACE_MAP()

BEGIN_DISPATCH_MAP(CInviteChatDlg, CCmdTarget)
//{{AFX_DISPATCH_MAP(CHistoryDlg)	
DISP_FUNCTION_ID(CInviteChatDlg,"", 18, OnShowContextMenu,    VT_EMPTY, VTS_I4 VTS_I4 VTS_I4 VTS_I4 VTS_I4 VTS_PI4)
//}}AFX_DISPATCH_MAP
END_DISPATCH_MAP()


/////////////////////////////////////////////////////////////////////////////
// CInviteChatDlg message handlers

void CInviteChatDlg::OnOK() 
{
}

void CInviteChatDlg::OnCancel() 
{
	if(Handle&&pMessenger->ConnectEnable())
	{
		pSession->CancelOperation(Handle);
	}
	else
		KillWindow();
}



BOOL CInviteChatDlg::OnInitDialog() 
{
	CResizableDialog::OnInitDialog();
	
	HRESULT hr = m_pWebCustomizer.CreateInstance(CLSID_MpaWebCustomizer);
	
	LPUNKNOWN pDispatch = m_edit.GetControlUnknown();
	m_pWebCustomizer->PutRefWebBrowser((LPDISPATCH)pDispatch);
	
	InitMpaWebEvent();
	
	pSession = theNet2.GetSession();
	ASSERT(pSession!=NULL);

	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),FALSE);
	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),TRUE);

	CRect winRect;
	GetWindowRect(&winRect);

	CRect rTmp(15,70,winRect.Width()-15,winRect.Height()-50);

	CRect r;

	AddAnchor(m_Nick.GetSafeHwnd(), CSize(0,0), CSize(0,0));
	AddAnchor(m_edit.GetSafeHwnd(), CSize(0,0), CSize(100,100));
	AddAnchor(m_Deny.GetSafeHwnd(), CSize(100,100), CSize(100,100));
	AddAnchor(m_Accept.GetSafeHwnd(), CSize(100,100), CSize(100,100));
	AddAnchor(m_Details.GetSafeHwnd(), CSize(0,100), CSize(0,100));
	AddAnchor(m_ChatDetails.GetSafeHwnd(), CSize(0,100), CSize(0,100));
	
	CString strRect = GetOptionString(IDS_OFSMESSENGER, IDS_INVITE_CHAT, _T(""));
	if(!strRect.IsEmpty())
	{
		CRect rWindow = StringToRect(strRect);
		FitRectToWindow(rWindow);
		SetWindowPos(NULL,rWindow.left,rWindow.top,rWindow.Width(),rWindow.Height(),SWP_NOZORDER|SWP_NOACTIVATE);
	}

	m_Nick.Invalidate(FALSE);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}


void CInviteChatDlg::SetInfo(CChat &Chat, CUser &user, BSTR Data)
{
	USES_CONVERSION;

	m_User	= user;

	m_Chat	=	Chat;
	
	CString strNickText;
	strNickText.Format(GetString(IDS_INVITE_CHAT_NICK_NAME),m_User.GetShowName());
	m_Nick.SetWindowText(strNickText);

	m_bsHTML	=	L"<HTML><HEAD><META http-equiv=Content-Type content='text/html;charset=UTF-16'/><STYLE>P{margin: 0 0 0 0;}</STYLE></HEAD><BODY bottomMargin=2 leftMargin=2 rightMargin=2 topMargin=2>";
	m_bsHTML	+= Data;
	m_bsHTML	+= L"</BODY></HTML>";

	m_edit.Navigate(_T("about:blank"),0,0,0,0);

	CString strWindowsCaption;
	strWindowsCaption.Format(GetString(IDD_INVITE_CHAT_DIALOG_TEXT),W2CT(m_Chat.GetName()));
	
	SetWindowText(strWindowsCaption);
	
	UpdateData(FALSE);
	
	if(m_User.IsBad())
	{
		if(!Handle)
		{
			if(pMessenger->ConnectEnable())
			{
				theNet2.LockTranslator();
				try
				{
					Handle = pSession->UserDetails(m_User.GetGlobalID(),1);
					if(Handle)
					{
						Block();
						theNet2.AddToTranslator(Handle,this->GetSafeHwnd());
					}
				}
				catch (_com_error&) 
				{
					ASSERT(FALSE);
				}
				
				theNet2.UnlockTranslator();
			}
		}
	}
}

void CInviteChatDlg::UnBlock()
{
	m_Accept.EnableWindow(TRUE);
	m_Deny.EnableWindow(TRUE);
}

void CInviteChatDlg::Block()
{
	m_Accept.EnableWindow(FALSE);
	m_Deny.EnableWindow(FALSE);
}

LRESULT CInviteChatDlg::OnNetEvent(WPARAM w,LPARAM l)
{
	NLT_Container *pItem = (NLT_Container *)w;
	ASSERT(pItem!=NULL);
	
	UnBlock();
	switch(pItem->EventType)
	{
	case NLT_EDetails:
		{
			IUser    *pUser    = NULL;
			HRESULT hr = AutoUnMarchaling(pItem,(LPUNKNOWN*)&pUser);
			
			if(SUCCEEDED(hr))
			{
				theNet2.LockTranslator();
				theNet2.RemoveFromTranslator(Handle);
				try
				{
					CUser InfoUser(pUser);
					m_User = InfoUser;

					CString strNickText;
					strNickText.Format(GetString(IDS_INVITE_CHAT_NICK_NAME),m_User.GetShowName());
					m_Nick.SetWindowText(strNickText);
					
					UpdateData(FALSE);
				}
				catch(...)
				{
					ASSERT(FALSE);
				}
				theNet2.UnlockTranslator();
				pUser->Release();
			}
		}
		break;
	case NLT_EChatCreate:
		{
			IChat *pChat	=	NULL;
			HRESULT hr = AutoUnMarchaling(pItem,(LPUNKNOWN*)&pChat);
			if(hr==S_OK)
			{
				// Step 1. Add new Chat to ChatsCollections [8/9/2002]
				pMessenger->AddNewChat(pChat,SC_INACTIVE);
				// Step 2. Connect to Chat [9/7/2002]
				CChat Chat(pChat);
				pMessenger->SetChatStatus(Chat.GetId(),SC_ACTIVE,CMainDlg::HCI_OPEN_CHAT_WINDOW);
				// Step 2. Free Interface [8/9/2002]
				pChat->Release();
			}
			// Step 3. Close the Window
			KillWindow();
		}
		break;
	case NLT_ECommandOK:
		theNet2.LockTranslator();
		theNet2.RemoveFromTranslator(Handle);
		theNet2.UnlockTranslator();
		KillWindow();
		break;
	case NLT_ECommandError:
		theNet2.LockTranslator();
		theNet2.RemoveFromTranslator(Handle);
		theNet2.UnlockTranslator();
		if(pItem->Long1==etSERVER)
		{
			switch(pItem->Long2)
			{
			case ERR_UNABLE_CREATE_CONN:
				_SHOW_IBN_ERROR_DLG_OK(IDS_SERVICENOTAVAILABLE);
				break;
			}
		}
		MessageBox(GetString(IDS_INVITECHAT_ERROR),GetString(IDS_ERROR_TITLE),MB_OK|MB_ICONSTOP);
		break;
	}
	
	delete pItem;
    return 0;
}

void CInviteChatDlg::KillWindow()
{
	if(GetStyle()&WS_VISIBLE&&GetOptionInt(IDS_OFSMESSENGER,IDS_ANIMATION,FALSE))
		RoundExitAddon(this);
	CResizableDialog::OnClose();
	if(!bIsKillWinodow)
	{
		CloseMpaWebEvent();
		bIsKillWinodow = TRUE;
		DestroyWindow();
		delete this;
	}
}

void CInviteChatDlg::OnDany() 
{
	if(!Handle)
	{
		if(pMessenger->ConnectEnable())
		{
			theNet2.LockTranslator();
			try
			{
				Handle = m_Chat->Accept(0);
				if(Handle)
				{
					theNet2.AddToTranslator(Handle,this->GetSafeHwnd());
					Block();
				}
			}
			catch(_com_error&)
			{
				ASSERT(FALSE);
			}
			
			theNet2.UnlockTranslator();
		}
	}
}

void CInviteChatDlg::OnEditmenuCopy() 
{
	//pCmdUI->Enable(m_edit.EnableClipboardCopy());
}

void CInviteChatDlg::OnUpdateEditmenuCopy(CCmdUI* pCmdUI) 
{
	//pCmdUI->Enable(m_edit.EnableClipboardCopy());
}

void CInviteChatDlg::OnEditmenuCut() 
{
}

void CInviteChatDlg::OnUpdateEditmenuCut(CCmdUI* pCmdUI) 
{
}

void CInviteChatDlg::OnEditmenuDelete() 
{
}

void CInviteChatDlg::OnUpdateEditmenuDelete(CCmdUI* pCmdUI) 
{
}

void CInviteChatDlg::OnEditmenuPast() 
{
}

void CInviteChatDlg::OnUpdateEditmenuPast(CCmdUI* pCmdUI) 
{
}


void CInviteChatDlg::OnAuthorizationRequest() 
{
	DHTMLE_ADDCONTACT_Container *pData = new DHTMLE_ADDCONTACT_Container;
	
	pData->email        = m_User.m_strEMail;
	pData->first_name   = m_User.m_strFirstName;
	pData->last_name    = m_User.m_strLastName;
	pData->nick_name    = m_User.m_strLogin;
	pData->role_id      = m_User.m_RoleID;
	pData->role_name    = m_User.m_strType;
	pData->user_id      = m_User.GlobalID;
	
	pMessenger->SendMessage(WM_DHTML_EVENT,(WPARAM)DHTMLE_ADDCONTACT,(LPARAM)pData);

	KillWindow();
}

void CInviteChatDlg::OnGetuserdetailsButton() 
{
	pMessenger->ShowUserDetails(m_User.GetGlobalID());
}

void CInviteChatDlg::OnCaptureChanged(CWnd *pWnd) 
{
	CRect rWindow;
	GetWindowRect(&rWindow);
	if(!IsIconic())
		WriteOptionString(IDS_OFSMESSENGER,IDS_INVITE_CHAT,RectToString(rWindow));
	
	CResizableDialog::OnCaptureChanged(pWnd);
}


void CInviteChatDlg::OnMcUserDetails() 
{
	OnGetuserdetailsButton();
}

void CInviteChatDlg::OnMcAddToContact() 
{
	OnAuthorizationRequest();
}

void CInviteChatDlg::OnMcAccept() 
{
	if(!Handle)
	{
		if(pMessenger->ConnectEnable())
		{
			theNet2.LockTranslator();
			//Handle = pSession->AddUserReply(m_User.GetGlobalID(),1);
			try
			{
				Handle = m_Chat->Accept(1);
				if(Handle)
				{
					theNet2.AddToTranslator(Handle,this->GetSafeHwnd());
					Block();
				}
			}
			catch(_com_error&)
			{
				ASSERT(FALSE);
			}
			theNet2.UnlockTranslator();
		}
	}
}

void CInviteChatDlg::OnMcDeny() 
{
	OnDany();
}

void CInviteChatDlg::InitMpaWebEvent()
{
	IConnectionPointContainer* pCPContainer = NULL;
	CComPtr<IConnectionPoint>  m_pSessionConnectionPoint;
	HRESULT hr = m_pWebCustomizer->QueryInterface(IID_IConnectionPointContainer,(void**)&pCPContainer);
	
	if (pCPContainer)
	{
		hr = pCPContainer->FindConnectionPoint(__uuidof(_IMpaWebCustomizerEvents),&m_pSessionConnectionPoint);
		if(SUCCEEDED(hr))
		{
			LPUNKNOWN pInterEvent = GetInterface(&IID_IUnknown);
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

void CInviteChatDlg::CloseMpaWebEvent()
{
	IConnectionPointContainer* pCPContainer = NULL;
	CComPtr<IConnectionPoint>  m_pSessionConnectionPoint;
	HRESULT hr = m_pWebCustomizer->QueryInterface(IID_IConnectionPointContainer,(void**)&pCPContainer);
	
	if (pCPContainer)
	{
		hr = pCPContainer->FindConnectionPoint(__uuidof(_IMpaWebCustomizerEvents),&m_pSessionConnectionPoint);
		if(SUCCEEDED(hr))
		{
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


void CInviteChatDlg::OnShowContextMenu(long dwID, long x, long y, IUnknown *pcmdTarget, IDispatch *pdispReserved, long* pShow)
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

void CInviteChatDlg::OnWebDocumentCompleted(IDispatch *pDisp, VARIANT *URL)
{
	// Step1. Получить HTML Document [5/7/2002]
	CComPtr<IHTMLDocument2>	pHtmlDoc;
	pHtmlDoc.Attach((IHTMLDocument2*)m_edit.GetDocument());
	
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
			// Обязательно добавлять, перед Unicode - данными, 
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

void CInviteChatDlg::OnMcChatDetails()
{
	USES_CONVERSION;
	CChatCreateDlg	*pChatDetailsDlg = new CChatCreateDlg(pMessenger);

	pChatDetailsDlg->Create(CChatCreateDlg::CCDM_DETAIL,W2CT(m_Chat.GetName()),W2CT(m_Chat.GetDescription()),NULL,&(m_Chat.GetUsers()),m_Chat.GetId());
	pMessenger->AddToAllCloseWindow(pChatDetailsDlg->GetSafeHwnd());
	// Load UserCollections to  CChatCreateDlg [8/8/2002]
	pChatDetailsDlg->ShowWindow(SW_SHOW);
}
