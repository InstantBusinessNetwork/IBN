// AddUserRequest.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "AddUserRequest.h"
#include "GlobalFunction.h"
#include "MainDlg.h"
//#include "MainFrm.h"
#include "LoadSkins.h"
#include "ExDispid.h"


#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CAddUserRequest dialog
extern CString GetCurrentSkin();

CAddUserRequest::CAddUserRequest(CMainDlg* pParent)
: CResizableDialog(CAddUserRequest::IDD, pParent)
{
	EnableAutomation();
	//{{AFX_DATA_INIT(CAddUserRequest)
	//}}AFX_DATA_INIT
	Handle = 0;
	pMessenger = pParent;
	bIsKillWinodow = FALSE;
	m_bAddUserCommand = FALSE;
	m_dwSessionCookie	=	0;
//	SetBoundary(0,0);
//	SetCaption(RGB(0,0,0),RGB(0,0,0),0);
}


void CAddUserRequest::DoDataExchange(CDataExchange* pDX)
{
	CResizableDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CAddUserRequest)
	//DDX_Control(pDX, IDC_FRAME_EDIT, m_frameEdit);
	DDX_Control(pDX, IDC_NICK_EDIT, m_Nick);
	DDX_Control(pDX, IDC_MCACCEPT, m_Accept);
	DDX_Control(pDX, IDC_MCDENY, m_Deny);
	DDX_Control(pDX, IDC_MCUSERDETAILS, m_Details);
	DDX_Control(pDX, IDC_MCADDTOCONTACT, m_AddToContact);
	DDX_Control(pDX, IDC_EDIT_EXPLORER, m_edit);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CAddUserRequest, CResizableDialog)
//{{AFX_MSG_MAP(CAddUserRequest)
//ON_BN_CLICKED(ID_DANY, OnDany)
ON_COMMAND(ID_EDITMENU_COPY, OnEditmenuCopy)
ON_UPDATE_COMMAND_UI(ID_EDITMENU_COPY, OnUpdateEditmenuCopy)
ON_COMMAND(ID_EDITMENU_CUT, OnEditmenuCut)
ON_UPDATE_COMMAND_UI(ID_EDITMENU_CUT, OnUpdateEditmenuCut)
ON_COMMAND(ID_EDITMENU_DELETE, OnEditmenuDelete)
ON_UPDATE_COMMAND_UI(ID_EDITMENU_DELETE, OnUpdateEditmenuDelete)
ON_COMMAND(ID_EDITMENU_PAST, OnEditmenuPast)
ON_UPDATE_COMMAND_UI(ID_EDITMENU_PAST, OnUpdateEditmenuPast)
//ON_BN_CLICKED(ID_AUTHORIZATION_REQUEST, OnAuthorizationRequest)
//ON_BN_CLICKED(ID_GETUSERDETAILS_BUTTON, OnGetuserdetailsButton)
ON_WM_CAPTURECHANGED()
ON_BN_CLICKED(IDC_MCUSERDETAILS, OnMcUserDetails)
ON_BN_CLICKED(IDC_MCADDTOCONTACT, OnMcAddToContact)
ON_BN_CLICKED(IDC_MCACCEPT, OnMcAccept)
ON_WM_CANCELMODE()
ON_BN_CLICKED(IDC_MCDENY, OnMcDeny)
//}}AFX_MSG_MAP
ON_MESSAGE(WM_NLT_CONTAINER_EVENT,OnNetEvent)	
END_MESSAGE_MAP()

BEGIN_EVENTSINK_MAP(CAddUserRequest, CResizableDialog)
//{{AFX_EVENTSINK_MAP(CAddUserRequest)
ON_EVENT(CAddUserRequest, IDC_EDIT_EXPLORER, DISPID_DOCUMENTCOMPLETE, OnWebDocumentCompleted, VTS_DISPATCH VTS_PVARIANT)
//}}AFX_EVENTSINK_MAP
END_EVENTSINK_MAP()

BEGIN_INTERFACE_MAP(CAddUserRequest, CCmdTarget)
INTERFACE_PART(CAddUserRequest, __uuidof(_IMpaWebCustomizerEvents), Dispatch)
END_INTERFACE_MAP()

BEGIN_DISPATCH_MAP(CAddUserRequest, CCmdTarget)
//{{AFX_DISPATCH_MAP(CHistoryDlg)	
DISP_FUNCTION_ID(CAddUserRequest,"", 18, OnShowContextMenu,    VT_EMPTY, VTS_I4 VTS_I4 VTS_I4 VTS_I4 VTS_I4 VTS_PI4)
//}}AFX_DISPATCH_MAP
END_DISPATCH_MAP()


/////////////////////////////////////////////////////////////////////////////
// CAddUserRequest message handlers

void CAddUserRequest::OnOK() 
{
}

void CAddUserRequest::OnCancel() 
{
	if(Handle&&pMessenger->ConnectEnable())
	{
		pSession->CancelOperation(Handle);
	}
	else
		KillWindow();
}



BOOL CAddUserRequest::OnInitDialog() 
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

//	LoadSkin();
	
	CRect winRect;
	GetWindowRect(&winRect);

	CRect rTmp(15,70,winRect.Width()-15,winRect.Height()-50);

	//m_edit.Create(NULL,NULL,WS_VISIBLE|WS_BORDER,rTmp,this,IDC_DHTML_EDIT);
	//m_edit.SetContextMenu(IDR_MESSENGER_MENU,1,this);
	//m_edit.SetViewMode();

	//  [4/29/2002]
	//m_edit.SetDefaultFontName(L"Arial");
	//m_edit.SetDefaultFontSize(nFontSizes[GetOptionInt(IDS_OFSMESSENGER,IDS_SIZE,1)]);
	//  [4/29/2002]
	

	
	//	m_Nick.SetWindowPos(NULL,18,43,winRect.Width()-36,16,SWP_NOZORDER|SWP_NOACTIVATE);
	//	m_Nick.SetTextColor(0xffffff);
	//	m_Nick.SetTransparent(TRUE);
	
	//	m_Close.SetAutoPressed(TRUE);
	//	m_Close.SetCanStayPressed(FALSE);
	//	m_Mini.SetAutoPressed(TRUE);
	//	m_Mini.SetCanStayPressed(FALSE);
	//	m_Deny.SetAutoPressed(TRUE);
	//	m_Deny.SetCanStayPressed(FALSE);
	//	m_AddToContact.SetAutoPressed(TRUE);
	//	m_AddToContact.SetCanStayPressed(FALSE);
	//	m_Accept.SetAutoPressed(TRUE);
	//	m_Accept.SetCanStayPressed(FALSE);
	//	m_Details.SetAutoPressed(TRUE);
	//	m_Details.SetCanStayPressed(FALSE);
	
	
	//	m_Deny.SetWindowPos(NULL,winRect.Width()-91,winRect.Height()-38,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
	//	m_AddToContact.SetWindowPos(NULL,winRect.Width()-91,winRect.Height()-38,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE|SWP_HIDEWINDOW);
	//	m_Accept.SetWindowPos(NULL,winRect.Width()-170,winRect.Height()-38,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
	//	m_Details.SetWindowPos(NULL,15,winRect.Height()-38,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
	
	CRect r;
	//m_frameEdit.GetWindowRect(&r);
	//ScreenToClient(&r);
	//m_edit.SetWindowPos(NULL, r.left, r.top, r.Width(), r.Height(), SWP_NOZORDER);
	AddAnchor(m_edit.GetSafeHwnd(), CSize(0,0), CSize(100,100));
	AddAnchor(m_Deny.GetSafeHwnd(), CSize(100,100), CSize(100,100));
	AddAnchor(m_AddToContact.GetSafeHwnd(), CSize(100,100), CSize(100,100));
	AddAnchor(m_Accept.GetSafeHwnd(), CSize(100,100), CSize(100,100));
	AddAnchor(m_Details.GetSafeHwnd(), CSize(0,100), CSize(0,100));
	
	CString strRect = GetOptionString(IDS_OFSMESSENGER, IDS_ADD_FRIENDR, _T(""));
	if(!strRect.IsEmpty())
	{
		CRect rWindow = StringToRect(strRect);
		FitRectToWindow(rWindow);
		SetWindowPos(NULL,rWindow.left,rWindow.top,rWindow.Width(),rWindow.Height(),SWP_NOZORDER|SWP_NOACTIVATE);
	}
	

	//m_edit.Clear();
	//m_edit.SetFocus();
	
	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}


void CAddUserRequest::SetSender(CUser &user, BSTR Data)
{
	m_User = user;

	m_Nick.SetText(m_User.GetShowName());

	m_bsHTML	=	L"<HTML><HEAD><META http-equiv=Content-Type content='text/html;charset=UTF-16'/><STYLE>P{margin: 0 0 0 0;}</STYLE></HEAD><BODY bottomMargin=2 leftMargin=2 rightMargin=2 topMargin=2>";
	m_bsHTML	+= Data;
	m_bsHTML	+= L"</BODY></HTML>";

	m_edit.Navigate(_T("about:blank"),0,0,0,0);
	
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
				catch(_com_error&)
				{
					ASSERT(FALSE);
				}
				
				
				theNet2.UnlockTranslator();
			}
		}
	}
}

void CAddUserRequest::UnBlock()
{
	//GetDlgItem(IDOK)->EnableWindow(TRUE);
	//GetDlgItem(ID_DANY)->EnableWindow(TRUE);
	m_Accept.EnableWindow(TRUE);
	m_Deny.EnableWindow(TRUE);
}

void CAddUserRequest::Block()
{
	m_Accept.EnableWindow(FALSE);
	m_Deny.EnableWindow(FALSE);
	//GetDlgItem(IDOK)->EnableWindow(FALSE);
	//GetDlgItem(ID_DANY)->EnableWindow(FALSE);
}

LRESULT CAddUserRequest::OnNetEvent(WPARAM w,LPARAM l)
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
					m_Nick.SetText(m_User.GetShowName());

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
	case NLT_ECommandOK:
		theNet2.LockTranslator();
		theNet2.RemoveFromTranslator(Handle);
		theNet2.UnlockTranslator();
		if(m_bAddUserCommand&&!pMessenger->CheckUserInContactList(m_User))
		{
			//GetDlgItem(IDOK)->ShowWindow(SW_HIDE);
			//GetDlgItem(ID_DANY)->ShowWindow(SW_HIDE);
			m_Accept.ShowWindow(SW_HIDE);
			m_Deny.ShowWindow(SW_HIDE);
			m_AddToContact.ShowWindow(SW_SHOWNORMAL);
			//GetDlgItem(ID_AUTHORIZATION_REQUEST)->ShowWindow(SW_SHOWNORMAL);
		}
		else
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
		MessageBox(GetString(IDS_ADD_USER_ERROR),GetString(IDS_ERROR_TITLE),MB_OK|MB_ICONSTOP);
		break;
	}
	
	delete pItem;
    return 0;
}

void CAddUserRequest::KillWindow()
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

void CAddUserRequest::OnDany() 
{
	if(!Handle)
	{
		if(pMessenger->ConnectEnable())
		{
			theNet2.LockTranslator();
			try
			{
				Handle = pSession->AddUserReply(m_User.GetGlobalID(),2);
				if(Handle)
				{
					m_bAddUserCommand = FALSE;
					Block();
					theNet2.AddToTranslator(Handle,this->GetSafeHwnd());
				}
			}
			catch(_com_error&)
			{
				ASSERT(FALSE);
			}
			
			theNet2.UnlockTranslator();
		}
		//else
		//{
		//	MessageBox("Error: No Connection","Can't add user",MB_OK|MB_ICONSTOP);
		//}
	}
}

void CAddUserRequest::OnEditmenuCopy() 
{
	//m_edit.ClipboardCopy();	
}

void CAddUserRequest::OnUpdateEditmenuCopy(CCmdUI* pCmdUI) 
{
	//pCmdUI->Enable(m_edit.EnableClipboardCopy());
}

void CAddUserRequest::OnEditmenuCut() 
{
	//m_edit.ClipboardCut();
}

void CAddUserRequest::OnUpdateEditmenuCut(CCmdUI* pCmdUI) 
{
	//pCmdUI->Enable(m_edit.EnableClipboardCut());
}

void CAddUserRequest::OnEditmenuDelete() 
{
	//m_edit.ClipboardDelete();
}

void CAddUserRequest::OnUpdateEditmenuDelete(CCmdUI* pCmdUI) 
{
	//pCmdUI->Enable(m_edit.EnableClipboardDelete());
}

void CAddUserRequest::OnEditmenuPast() 
{
	//if (OpenClipboard())
	//	{
	//		HANDLE hText = GetClipboardData(CF_TEXT);
	//		
	//		if(hText!=NULL)
	//		{
	//			CComBSTR strText = (LPCTSTR)GlobalLock(hText);
	//			m_edit.InsertTEXT(strText);
	//			GlobalUnlock(hText);
	//		}
	//		
	//		CloseClipboard();
	//	}
	
}

void CAddUserRequest::OnUpdateEditmenuPast(CCmdUI* pCmdUI) 
{
	//pCmdUI->Enable(m_edit.EnableClipboardPast());
}


void CAddUserRequest::OnAuthorizationRequest() 
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

void CAddUserRequest::OnGetuserdetailsButton() 
{
	pMessenger->ShowUserDetails(m_User.GetGlobalID());
}

//DEL void CAddUserRequest::LoadSkin()
//DEL {
//DEL 	LoadSkins	m_LoadSkin;
//DEL 	
//DEL 	IStreamPtr pStream = NULL;
//DEL 	long       Error   = 0L;
//DEL 	
//DEL 	bstr_t bstrPath =IBN_SCHEMA;
//DEL 	bstrPath += (LPCTSTR)GetCurrentSkin();
//DEL 	
//DEL 	m_LoadSkin.Load(bstr_t(bstrPath+"/Shell/AddR/fon.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 	{
//DEL 		LPPICTURE	Pic	=	NULL;
//DEL 		HRESULT hr = ::OleLoadPicture(pStream,	0, TRUE, IID_IPicture,(void**)&Pic);
//DEL 		if(SUCCEEDED(hr))
//DEL 		{
//DEL 			m_ResizeFon.Create(Pic);
//DEL 			m_ResizeFon.AddAnchor(CRect(0,0,130,70),CSize(0,0),CSize(0,0));
//DEL 			m_ResizeFon.AddAnchor(CRect(130,0,205,70),CSize(0,0),CSize(100,0),CResizableImage::DUPLICATE);
//DEL 			m_ResizeFon.AddAnchor(CRect(205,0,299,70),CSize(100,0),CSize(100,0));
//DEL 			
//DEL 			m_ResizeFon.AddAnchor(CRect(0,70,130,179),CSize(0,0),CSize(0,100),CResizableImage::DUPLICATE);
//DEL 			m_ResizeFon.AddAnchor(CRect(130,70,205,179),CSize(0,0),CSize(100,100),CResizableImage::DUPLICATE);
//DEL 			m_ResizeFon.AddAnchor(CRect(205,70,299,179),CSize(100,0),CSize(100,100),CResizableImage::DUPLICATE);
//DEL 			
//DEL 			m_ResizeFon.AddAnchor(CRect(0,179,130,227),CSize(0,100),CSize(0,100));
//DEL 			m_ResizeFon.AddAnchor(CRect(139,179,205,227),CSize(0,100),CSize(100,100),CResizableImage::DUPLICATE);
//DEL 			m_ResizeFon.AddAnchor(CRect(205,179,299,227),CSize(100,100),CSize(100,100));
//DEL 		}
//DEL 	}
//DEL 	pStream = NULL;
//DEL 
//DEL 	m_LoadSkin.Load(bstrPath+bstr_t("/Common/btn_x.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_Close.LoadBitmapFromStream(pStream);
//DEL 	pStream = NULL;
//DEL 
//DEL 	m_LoadSkin.Load(bstrPath+bstr_t("/Common/btn_minimize.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_Mini.LoadBitmapFromStream(pStream);
//DEL 	pStream = NULL;
//DEL 
//DEL 	m_LoadSkin.Load(bstrPath+bstr_t("/Shell/AddR/accept.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_Accept.LoadBitmapFromStream(pStream);
//DEL 	pStream = NULL;
//DEL 
//DEL 	m_LoadSkin.Load(bstrPath+bstr_t("/Shell/AddR/deny.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_Deny.LoadBitmapFromStream(pStream);
//DEL 	pStream = NULL;
//DEL 
//DEL 	m_LoadSkin.Load(bstrPath+bstr_t("/Shell/AddR/user_details.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_Details.LoadBitmapFromStream(pStream);
//DEL 	pStream = NULL;
//DEL 
//DEL 	m_LoadSkin.Load(bstrPath+bstr_t("/Shell/AddR/add.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_AddToContact.LoadBitmapFromStream(pStream);
//DEL }

void CAddUserRequest::OnCaptureChanged(CWnd *pWnd) 
{
	CRect rWindow;
	GetWindowRect(&rWindow);
	if(!IsIconic())
		WriteOptionString(IDS_OFSMESSENGER,IDS_ADD_FRIEND,RectToString(rWindow));
	
	CResizableDialog::OnCaptureChanged(pWnd);
}

//DEL void CAddUserRequest::OnPaint() 
//DEL {
//DEL 	CPaintDC dc(this); // device context for painting
//DEL 	
//DEL 	CRect m_Client;
//DEL 	GetClientRect(&m_Client);
//DEL 	m_ResizeFon.Render(dc.GetSafeHdc(),m_Client.Size());
//DEL }

//DEL void CAddUserRequest::OnLButtonDown(UINT nFlags, CPoint point) 
//DEL {
//DEL 	CPoint inPoint	=	point;
//DEL 	ClientToScreen(&point);
//DEL 	
//DEL 	CRect StatusRect, miniRect;
//DEL 	GetClientRect(&StatusRect);
//DEL 	
//DEL 	miniRect = StatusRect;miniRect.InflateRect(-9,-9,-9,-9);
//DEL 	
//DEL 	if(!miniRect.PtInRect(inPoint))
//DEL 	{
//DEL 		if(inPoint.x<miniRect.left)
//DEL 			if(inPoint.y<miniRect.top)
//DEL 				CResizableDialog::OnNcLButtonDown(HTTOPLEFT,point);
//DEL 			else if(inPoint.y>miniRect.bottom)
//DEL 				CResizableDialog::OnNcLButtonDown(HTBOTTOMLEFT,point);
//DEL 			else
//DEL 				CResizableDialog::OnNcLButtonDown(HTLEFT,point);
//DEL 			else if(inPoint.x>miniRect.right)
//DEL 				if(inPoint.y<miniRect.top)
//DEL 					CResizableDialog::OnNcLButtonDown(HTTOPRIGHT,point);
//DEL 				else if(inPoint.y>miniRect.bottom)
//DEL 					CResizableDialog::OnNcLButtonDown(HTBOTTOMRIGHT,point);
//DEL 				else
//DEL 					CResizableDialog::OnNcLButtonDown(HTRIGHT,point);
//DEL 				else if(inPoint.y<miniRect.top)
//DEL 					CResizableDialog::OnNcLButtonDown(HTTOP,point);
//DEL 				else
//DEL 					CResizableDialog::OnNcLButtonDown(HTBOTTOM,point);
//DEL 	}
//DEL 	else
//DEL 		CResizableDialog::OnNcLButtonDown(HTCAPTION,point);
//DEL }

//DEL BOOL CAddUserRequest::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message) 
//DEL {
//DEL 	CRect StatusRect, miniRect;
//DEL 	
//DEL 	GetClientRect(&StatusRect);
//DEL 				
//DEL 	CPoint point, inPoint;
//DEL 	
//DEL 	::GetCursorPos(&point);
//DEL 	inPoint = point;
//DEL 	ScreenToClient(&inPoint);
//DEL 	
//DEL 	miniRect = StatusRect;miniRect.InflateRect(-9,-9,-9,-9);
//DEL 	
//DEL 	if(!miniRect.PtInRect(inPoint))
//DEL 	{
//DEL 		if(inPoint.x<miniRect.left)
//DEL 			if(inPoint.y<miniRect.top)
//DEL 				nHitTest = HTTOPLEFT;
//DEL 			else if(inPoint.y>miniRect.bottom)
//DEL 				nHitTest = HTBOTTOMLEFT;
//DEL 			else
//DEL 				nHitTest = HTLEFT;
//DEL 			else if(inPoint.x>miniRect.right)
//DEL 				if(inPoint.y<miniRect.top)
//DEL 					nHitTest = HTTOPRIGHT;
//DEL 				else if(inPoint.y>miniRect.bottom)
//DEL 					nHitTest = HTBOTTOMRIGHT;
//DEL 				else
//DEL 					nHitTest = HTRIGHT;
//DEL 				else if(inPoint.y<miniRect.top)
//DEL 					nHitTest = HTTOP;
//DEL 				else
//DEL 					nHitTest = HTBOTTOM;
//DEL 	}
//DEL 	
//DEL 	return CResizableDialog::OnSetCursor(pWnd, nHitTest, message);
//DEL }

//DEL void CAddUserRequest::OnSize(UINT nType, int cx, int cy) 
//DEL {
//DEL 	CResizableDialog::OnSize(nType, cx, cy);
//DEL 	
//DEL 	CRect rgnRect;
//DEL 	GetWindowRect(&rgnRect);
//DEL 	CRgn	WinRgn;
//DEL 	WinRgn.CreateRoundRectRgn(0,0,rgnRect.Width(),rgnRect.Height(),20,20);
//DEL 	SetWindowRgn(WinRgn,TRUE);
//DEL 	Invalidate(FALSE);
//DEL 	
//DEL 	if(m_Close.GetSafeHwnd())
//DEL 	{
//DEL 		m_Close.SetWindowPos(NULL,cx-31,13,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//DEL 		m_Mini.SetWindowPos(NULL,cx-53,13,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//DEL 	}
//DEL }

void CAddUserRequest::OnClickMcclose()
{
	OnCancel();
}

void CAddUserRequest::OnClickMcmini()
{
	ShowWindow(SW_MINIMIZE);
}

void CAddUserRequest::OnClickMcaccept()
{
	if(!Handle)
	{
		if(pMessenger->ConnectEnable())
		{
			theNet2.LockTranslator();
			try
			{
				Handle = pSession->AddUserReply(m_User.GetGlobalID(),1);
				if(Handle)
				{
					m_bAddUserCommand =  TRUE;
					Block();
					theNet2.AddToTranslator(Handle,this->GetSafeHwnd());
				}
			}
			catch(_com_error&)
			{
				ASSERT(FALSE);
			}
			
			theNet2.UnlockTranslator();
		}
		///else
		//{
		//	MessageBox("Error: No Connection","Can't add user",MB_OK|MB_ICONSTOP);
		//}
	}
}

void CAddUserRequest::OnClickMcdeny()
{
	OnDany();
}

void CAddUserRequest::OnClickMcadd()
{
	OnAuthorizationRequest();
}

void CAddUserRequest::OnClickMcdetails()
{
	OnGetuserdetailsButton();
}

//DEL BOOL CAddUserRequest::OnEraseBkgnd(CDC* pDc)
//DEL {
//DEL 	return TRUE;
//DEL }

void CAddUserRequest::OnMcUserDetails() 
{
	OnGetuserdetailsButton();
}

void CAddUserRequest::OnMcAddToContact() 
{
	OnAuthorizationRequest();
}

void CAddUserRequest::OnMcAccept() 
{
	if(!Handle)
	{
		if(pMessenger->ConnectEnable())
		{
			theNet2.LockTranslator();
			try
			{
				Handle = pSession->AddUserReply(m_User.GetGlobalID(),1);
				if(Handle)
				{
					m_bAddUserCommand =  TRUE;
					Block();
					theNet2.AddToTranslator(Handle,this->GetSafeHwnd());
				}
			}
			catch(_com_error&)
			{
				ASSERT(FALSE);
			}
			
			theNet2.UnlockTranslator();
		}
		///else
		//{
		//	MessageBox("Error: No Connection","Can't add user",MB_OK|MB_ICONSTOP);
		//}
	}
}

void CAddUserRequest::OnMcDeny() 
{
	OnDany();
}

void CAddUserRequest::InitMpaWebEvent()
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

void CAddUserRequest::CloseMpaWebEvent()
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


void CAddUserRequest::OnShowContextMenu(long dwID, long x, long y, IUnknown *pcmdTarget, IDispatch *pdispReserved, long* pShow)
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

void CAddUserRequest::OnWebDocumentCompleted(IDispatch *pDisp, VARIANT *URL)
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
