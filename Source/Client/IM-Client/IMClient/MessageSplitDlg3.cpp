// MessageSplitDlg.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "MainDlg.h"
#include "MessageSplitDlg3.h"
#include "resource.h"
#include "User.h"
#include <triedcid.h>

#include "MemDc.h"
//#include "smileyahoopopup.h"
#include "FileDescriptioDlg.h"
#include "DlgTV.h"

#include "mshtmcid.h"

#include "SmileManager.h"
#include "SelectSmileDlg.h"

extern  CSmileManager CurrentSmileManager;

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

#define WM_EDIT_UPDATE WM_USER + 201
#define WM_AUTOREFRESH         + 202 
/////////////////////////////////////////////////////////////////////////////
// CMessageSplitDlg3 dialog


CMessageSplitDlg3::CMessageSplitDlg3(CMainDlg *pMessenger,CWnd* pParent /*=NULL*/)
	: COFSNcDlg2(CMessageSplitDlg3::IDD, pParent)
{
	EnableAutomation();
	//{{AFX_DATA_INIT(CMessageSplitDlg3)
	//}}AFX_DATA_INIT
	m_bDisableEditOnStart	=	FALSE;
	m_strSkinSettings = _T("/Shell/Chat/skin.xml");
	bBlock  = FALSE;	
	this->pMessenger = pMessenger;
	Handle  = 0L;
	MessageTime = 0L;
	bInitEdit = FALSE;
	SetBoundary(0,0);
	SetBoundaryColor(RGB(0,0,0));
	SetCaption(COLORREF(0), COLORREF(0), 0);
	pFonBmp = NULL;
	bIsKillWinodow	=	FALSE;
	m_bWasCtrlEnter	=	FALSE;
	m_bWasCtrlExit	=	FALSE;
	m_bLoadSkin = FALSE;
	m_dwSessionCookie	=	0;
	m_bEnableIfActiavte	=	FALSE;
	m_bEnableRefresh	=	FALSE;
	m_bSendBody = FALSE;
}

CMessageSplitDlg3::~CMessageSplitDlg3()
{
	if(pFonBmp)
	{
		pFonBmp->DeleteObject();
		delete pFonBmp;
	}
}


void CMessageSplitDlg3::DoDataExchange(CDataExchange* pDX)
{
	COFSNcDlg2::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CMessageSplitDlg3)
	DDX_Control(pDX, IDC_SENDERUSERINFO, m_SenderUserInfo);
	DDX_Control(pDX, IDC_USERINFO, m_UserInfo);
	DDX_Control(pDX, IDC_MCCLOSE, m_btnX);
	DDX_Control(pDX, IDC_MCSEND, m_Send);
	DDX_Control(pDX, IDC_MCOPTIONS, m_Options);
	DDX_Control(pDX, IDC_MCMENU, m_Menu);
	DDX_Control(pDX, IDC_MCMINI, m_btnMin);
	DDX_Control(pDX, IDC_BTN_COLOR, m_btnColor);
	DDX_Control(pDX, IDC_BTN_BOLD, m_btnBold);
	DDX_Control(pDX, IDC_BTN_ITALIC, m_btnItalic);
	DDX_Control(pDX, IDC_BTN_UNDERLINE, m_btnUnderline);
	DDX_Control(pDX, IDC_BTN_SMILES, m_btnSmiles);
	DDX_Control(pDX, IDC_EXPLORER, m_History);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CMessageSplitDlg3, COFSNcDlg2)
//{{AFX_MSG_MAP(CMessageSplitDlg3)
ON_WM_CREATE()
ON_BN_CLICKED(IDOK, OnOk)
ON_MESSAGE(WM_NLT_CONTAINER_EVENT,OnNetEvent)	
//ON_BN_CLICKED(IDC_SEND_BUTTON, OnSendButton)
ON_BN_CLICKED(IDC_COLOR_BUTTON, OnColorButton)
ON_CBN_SELENDOK(IDC_FONT_COMBO, OnSelendokFontCombo)
ON_CBN_SELENDOK(IDC_SIZE_COMBO, OnSelendokSizeCombo)
ON_COMMAND(ID_EDITMENU_COPY, OnEditmenuCopy)
ON_UPDATE_COMMAND_UI(ID_EDITMENU_COPY, OnUpdateEditmenuCopy)
ON_COMMAND(ID_EDITMENU_CUT, OnEditmenuCut)
ON_UPDATE_COMMAND_UI(ID_EDITMENU_CUT, OnUpdateEditmenuCut)
ON_COMMAND(ID_EDITMENU_DELETE, OnEditmenuDelete)
ON_UPDATE_COMMAND_UI(ID_EDITMENU_DELETE, OnUpdateEditmenuDelete)
ON_COMMAND(ID_EDITMENU_PAST, OnEditmenuPast)
ON_UPDATE_COMMAND_UI(ID_EDITMENU_PAST, OnUpdateEditmenuPast)
ON_WM_CAPTURECHANGED()
ON_WM_MOVE()
ON_WM_PAINT()
	ON_WM_SETCURSOR()
	ON_WM_DROPFILES()
	ON_BN_CLICKED(IDC_INSERTSMILE_CHECK, OnInsertsmileCheck)
	ON_WM_ACTIVATE()
ON_MESSAGE(WM_EDIT_UPDATE,OnEditUpdate)
ON_MESSAGE(WM_AUTOREFRESH,OnAutoRefresh)
	ON_WM_NCACTIVATE()
	//}}AFX_MSG_MAP
ON_MESSAGE(WM_SET_RECIPIENT,OnSetRecipient)
ON_MESSAGE(WM_SWM_REFRESH,OnSWMRefreh)
ON_MESSAGE(WM_SWM_SETBODY,OnSWMSetBody)
ON_COMMAND_RANGE(20000,20000+SmileBuffSize,OnSmileItem)
END_MESSAGE_MAP()


BEGIN_INTERFACE_MAP(CMessageSplitDlg3, CCmdTarget)
INTERFACE_PART(CMessageSplitDlg3, __uuidof(_IMpaWebCustomizerEvents), Dispatch)
END_INTERFACE_MAP()

BEGIN_DISPATCH_MAP(CMessageSplitDlg3, CCmdTarget)
//{{AFX_DISPATCH_MAP(CEventContainer)	
DISP_FUNCTION_ID(CMpaWebEvent, "", 2,  OnCmdGetVariable, VT_EMPTY,VTS_BSTR VTS_PBSTR)	
DISP_FUNCTION_ID(CMessageSplitDlg3,"", 18, OnShowContextMenu, VT_EMPTY, VTS_I4 VTS_I4 VTS_I4 VTS_I4 VTS_I4 VTS_PI4)
//}}AFX_DISPATCH_MAP
END_DISPATCH_MAP()


/////////////////////////////////////////////////////////////////////////////
// CMessageSplitDlg3 message handlers
LPARAM CMessageSplitDlg3::OnSetRecipient(WPARAM w, LPARAM l)
{
	SetRecipient(*((CUser*)w));
	return 0;
}

LRESULT CMessageSplitDlg3::OnSWMRefreh(WPARAM w, LPARAM l)
{
	Refresh();
	return 0;
}

void CMessageSplitDlg3::OnOk() 
{
	// TODO: Add your control notification handler code here
}

void CMessageSplitDlg3::OnCancel() 
{
	// TODO: Add extra cleanup here
	if((Handle==0L)||!pMessenger->ConnectEnable())
	{
		//COFSNcDlg2::OnCancel();
		KillWindow();
		return;
	}
	pSession->CancelOperation(Handle);
}

BOOL CMessageSplitDlg3::OnInitDialog() 
{
	//  [7/23/2002]
	//CDialog::OnInitDialog();
	
	//  [7/23/2002]
		
	COFSNcDlg2::OnInitDialog();

	//////////////////////////////////////////////////////////////////////////
	// McToolTipAddon
	m_ToolTip.AddTool(&m_btnX,IDS_TIP_CLOSE);
	m_ToolTip.AddTool(&m_btnMin,IDS_TIP_MINIMIZE);
	m_ToolTip.AddTool(&m_btnColor,IDS_TIP_FONT_COLOR);
	m_ToolTip.AddTool(&m_btnBold,IDS_TIP_BOLD);
	m_ToolTip.AddTool(&m_btnItalic,IDS_TIP_ITALIC);
	m_ToolTip.AddTool(&m_btnUnderline,IDS_TIP_UNDERLINE);
	m_ToolTip.AddTool(&m_btnSmiles,IDS_TIP_SMILES);
	m_ToolTip.AddTool(&m_FontCombo,IDS_TIP_FONT);
	m_ToolTip.AddTool(&m_SizeCombo,IDS_TIP_FONT_SIZE);
	//////////////////////////////////////////////////////////////////////////
	
	//////////////////////////////////////////////////////////////////////////
	
	HRESULT hr = m_pWebCustomizer.CreateInstance(CLSID_MpaWebCustomizer);
	
	LPUNKNOWN pDispatch = m_History.GetControlUnknown();
	m_pWebCustomizer->PutRefWebBrowser((LPDISPATCH)pDispatch);

	InitMpaWebEvent();

//	COFSNcDlg2::LoadSkin();
//	ShowSizeGrip(FALSE);

	/// Create Font ...
	m_font.Attach(GetStockObject(DEFAULT_GUI_FONT));

/*
	CBitmap bm;
	
	FontStateList.Create(16,16,ILC_COLOR32|ILC_MASK, 0,4);
	bm.LoadBitmap(IDB_BOLD);
	FontStateList.Add(&bm,RGB(255,0,255));
	bm.DeleteObject ();
	
	bm.LoadBitmap (IDB_ITALIC);
	FontStateList.Add(&bm,RGB(255,0,255));
	bm.DeleteObject ();
	
	bm.LoadBitmap (IDB_UNDERLINE);
	FontStateList.Add(&bm,RGB(255,0,255));
	bm.DeleteObject ();
	
    bm.LoadBitmap (IDB_COLOR);
	FontStateList.Add(&bm,RGB(255,0,255));
	bm.DeleteObject ();

	bm.LoadBitmap (IDB_SMILE);
	FontStateList.Add(&bm,RGB(255,0,255));
	bm.DeleteObject ();
*/
	
//   	m_BoldButton.SetIcon(FontStateList.ExtractIcon(0));
//	m_ItalicButton.SetIcon(FontStateList.ExtractIcon(1));
//	m_UnderLineButton.SetIcon(FontStateList.ExtractIcon(2));
//	m_ColorButton.SetIcon(FontStateList.ExtractIcon(3));
//	m_InsertSmileButton.SetIcon(FontStateList.ExtractIcon(4));

//	m_btnX.SetAutoPressed(TRUE);
//	m_btnX.SetCanStayPressed(FALSE);
//	m_Send.SetAutoPressed(TRUE);
//	m_Send.SetCanStayPressed(FALSE);
//	m_Menu.SetAutoPressed(TRUE);
//	m_Menu.SetCanStayPressed(FALSE);
//	m_btnMin.SetAutoPressed(TRUE);
//	m_btnMin.SetCanStayPressed(FALSE);
	
//	m_UserInfo.SetTransparent(TRUE);
//	m_UserInfo.SetFontName(_T("Arial"));
//	m_UserInfo.SetFontSize(8);
//	m_UserInfo.SetTextColor(m_crStatic.crText);

//	m_SenderUserInfo.SetTransparent(TRUE);
//	m_SenderUserInfo.SetFontName(_T("Arial"));
//	m_SenderUserInfo.SetFontSize(8);
//	m_SenderUserInfo.SetTextColor(m_crStatic.crBG);
	
	/// Create and Init History Brouser
//	CRect m_HistoryRect(17,72,393,205);
//	m_HistoryChild.SetBoundary(0,0);
//	m_HistoryChild.Create(NULL,NULL,WS_VISIBLE|WS_CHILD,m_HistoryRect,this,AFX_IDW_PANE_FIRST);
//	m_HistoryChild.webbrowser2.Navigate("about:blank",NULL,NULL,NULL,NULL);
	m_bEnableNavigateHistory = TRUE;
	m_History.Navigate(_T("about:blank"),NULL,NULL,NULL,NULL);
	//GetDlgItem(IDC_SPLITTER_STATIC)->SetWindowPos(NULL,m_HistoryRect.left-5,m_HistoryRect.bottom+1,m_HistoryRect.Width()+10,3,SWP_NOZORDER|SWP_NOACTIVATE);

    /// Create Font ...
	//m_font.Attach(GetStockObject(DEFAULT_GUI_FONT));

	pSession = theNet2.GetSession();
	ASSERT(pSession!=NULL);
	
//	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),FALSE);
//	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),TRUE);

//	CRect rTmp(16,249,351,304);
//	m_edit.Create(NULL,NULL,WS_VISIBLE,rTmp,this,IDC_DHTML_EDIT);

	m_edit.InitInfoMessage(WM_EDIT_UPDATE);
	m_edit.SetContextMenu(IDR_MESSENGER_MENU,1,this);
	m_edit.SetEditMode();
	
//	m_Send.SetWindowPos(NULL,360,249,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//	m_btnX.SetWindowPos(NULL,382,13,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//	m_btnMin.SetWindowPos(NULL,360,13,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//	m_Options.SetWindowPos(NULL,57,16,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//	m_Menu.SetWindowPos(NULL,20,16,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//	m_UserInfo.SetWindowPos(NULL,20,48,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//	m_SenderUserInfo.SetWindowPos(NULL,135,48,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
	
	
	//	m_ColorButton.SetWindowPos(NULL,17,222,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
	//	m_BoldButton.SetWindowPos(NULL,43,222,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
	//	m_ItalicButton.SetWindowPos(NULL,69,222,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
	//	m_UnderLineButton.SetWindowPos(NULL,94,222,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
	//	m_InsertSmileButton.SetWindowPos(NULL,120,222,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
	
	//static int nFontSizes[] = {8, 10, 12, 14, 18, 24, 36};
	//	CRect rect;
	//	rect.top = 221;
	//	rect.bottom = rect.top + nDropHeight;
	//	rect.left = 150;
	//	rect.right = rect.left + 40;
	
	//	m_SizeCombo.Create(CBS_DROPDOWNLIST|WS_VISIBLE|WS_TABSTOP,
	//		rect,this,IDC_SIZE_COMBO);
	m_SizeCombo.SetFont(&m_font);
    CString str;
	for (int i = 0; i < sizeof(nFontSizes)/sizeof(int); i++)
	{
		str.Format(_T("%d"), nFontSizes[i]);
		m_SizeCombo.AddString(str);
	}
	
	///// Font Combo Create ....
	//	rect.top = 221;
	//	rect.bottom = rect.top + nDropHeight;
	//	rect.left = 195;
	//	rect.right = rect.left+120;
	
	//    m_FontCombo.Create(CBS_DROPDOWNLIST|WS_VSCROLL|CBS_SORT |WS_VISIBLE|WS_TABSTOP,
	//		rect, this, IDC_FONT_COMBO);
	m_FontCombo.SetFont(&m_font);
	::EnumFontFamilies(GetDC()->m_hDC, (LPTSTR) NULL, (FONTENUMPROC)NEnumFontNameProc, (LPARAM)&(m_FontCombo));
	
	bInitEdit = FALSE;
	
	//  [4/29/2002]
	CString strFontName = GetOptionString(IDS_OFSMESSENGER,IDS_FONT,_T("Arial"));
	//int FontId = GetOptionString(IDS_OFSMESSENGER,IDS_FONT,_T("Arial"));
	//if(FontId!=-1)
	//	m_FontCombo.GetLBText(FontId, strFontName);

	CComBSTR	bsFontName = strFontName;
	
	m_edit.SetDefaultFontName(bsFontName);
	m_edit.SetDefaultFontSize(nFontSizes[GetOptionInt(IDS_OFSMESSENGER,IDS_SIZE,1)]);
	//  [4/29/2002]

	

	// OZ 2007-02-01
	m_CoolMenuManager.Install(this);

	try
	{
		// Load Smiles
		int SmileIndex = 0;
		for(CSmileInfoListEnum item = CurrentSmileManager.GetSmiles().begin();item!=CurrentSmileManager.GetSmiles().end();item++, SmileIndex++)
		{
			CBitmap* pPreviewBmp = CurrentSmileManager.GetSmilePreview((*item).GetId());

			if(pPreviewBmp!=NULL)
			{
				TOOLBARDATA Tdt = {1,16,16,1,0};
				WORD		dwItemID[] = {20001+(*item).GetIndex()};

				Tdt.items   = (WORD*)dwItemID;

				m_CoolMenuManager.LoadToolbar(*pPreviewBmp,&Tdt,0x808080);

				pPreviewBmp->DeleteObject();
				delete pPreviewBmp;
			}
		}
		
	}
	catch (...) 
	{
		ASSERT(FALSE);
	}
	//

	m_edit.Clear();
	m_InitialDefaultFontName = m_edit.GetDefaultFontName();
	m_InitialDefaultFontSize = m_edit.GetDefaultFontSizeIndex();


	return (m_bDisableEditOnStart?FALSE:TRUE);  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}


void CMessageSplitDlg3::OnSendButton() 
{
	// TODO: Add your control notification handler code here
	UpdateData(TRUE);
	
	int iStatus	=	pMessenger->GetUserCurrentStatus(m_Recipient.GetGlobalID());
	
	if(pMessenger->ConnectEnable() && !Handle)
	{
		UINT DlgId	=	0;

		switch(iStatus)
		{
		case S_OFFLINE:
			DlgId = IDS_USER_OFFLINE;
			break;
/*			case S_NA:
				DlgId = IDS_NA;
				break;
			case S_DND:
				DlgId = IDS_DND;
				break;
			case S_AWAY:
				DlgId = IDS_AWAY;
				break;
			case S_AWAITING:
				DlgId = IDS_AWAITING;
				break;*/
		}

		if(DlgId)
		{
			CString strMessageText;
			strMessageText.Format(GetString(IDS_USER_IS_FORMAT),GetString(DlgId));
			CMessageDlg	UserOffline(DlgId,this);
			if(UserOffline.Show(strMessageText,MB_OKCANCEL)!=IDOK)
				return;
		}
		
		
		CComBSTR bstText;
		m_edit.GetHTML(&bstText);

		// Replace Smile Images
		int StartPos = 0, Length = 0;
		CSmileInfo currentSmile = FindSmileImage(bstText.m_str, StartPos, Length);

		while(currentSmile!=CSmileInfo::Empty)
		{
			USES_CONVERSION;

			WCHAR *pBuffer = new WCHAR[Length+1];
			wcsncpy(pBuffer, ((LPCWSTR)bstText.m_str) + StartPos, Length);
			pBuffer[Length] = 0;

			int LenPrev = bstText.Length();
			Replace(bstText.m_str,pBuffer, T2CW(currentSmile.GetHtmlSmile())); 

			int LenNew = bstText.Length();

			StartPos = 0;
			Length = 0;

			delete pBuffer;

			currentSmile = FindSmileImage(bstText.m_str, StartPos, Length);
		}

		///
		WCHAR	Buff[1000];
		swprintf(Buff,L"<FONT face=\"%s\" size=%d>", m_InitialDefaultFontName, m_InitialDefaultFontSize);

		CComBSTR bstTmpText = Buff;
		bstTmpText += (LPCWSTR)bstText;
		bstTmpText += "</FONT>";

		bstText.Empty();
		bstText = bstTmpText;
		///
		
		CComBSTR bsGoodHtml;
		while(bstText.Length()>0)
		{
			bsGoodHtml = SplitHTML(8192,bstText);

			////////
			RemoveParagraf(bsGoodHtml.m_str);

			//
			CComBSTR bsTmpGoodHtml = (LPCWSTR)bsGoodHtml;
			bsGoodHtml.Empty();
			bsGoodHtml = bsTmpGoodHtml;
			//////// 
			
			if(wcslen(bsGoodHtml) ==0)
				continue;

			
			try
			{
				pMessage = pSession->CreateMessage();
				pMessage->PutMID(GUIDGen());
				pMessage->PutBody((BSTR)bsGoodHtml);
				
				IUsersPtr pResepients = pMessage->GetRecipients();
				IUserPtr  pUser = pResepients->AddUser();
				pUser->PutValue("@id",m_Recipient.GetGlobalID());
				
				//////////////////////////////////////////////////////////////////////////
				// Addon For Light MessagrEdition [9/5/2002]
				pMessenger->SendLightNewMessage(pMessage);
				
				pMessage = NULL;
				
			}
			catch (_com_error&) 
			{
			}

			Sleep(100);
		}

		Refresh();
		
		bInitEdit = FALSE;
		
		m_edit.Clear();
		m_InitialDefaultFontName = m_edit.GetDefaultFontName();
		m_InitialDefaultFontSize = m_edit.GetDefaultFontSizeIndex();


		/*if(bsTXTText.Length()==0||bsTXTText.Length()>=900)
		{
			m_edit.SetFocus();
			return;
		}*/

		//  [3/28/2002]
		/*Replace(bstText.m_str,_MC_GET_BUILD_COMMAND, L"<mcdiv mccmd=\""\
				_MC_GET_BUILD_COMMAND\
				L"\">Test</mcdiv>");
		//  [3/29/2002]
		Replace(bstText.m_str,_M_GET_IP_COMMAND, L"<mcdiv mccmd=\""\
			_MC_GET_IP_COMMAND\
			L"\">Test</mcdiv>");
		//  [3/29/2002]*/
		
	
		m_edit.SetFocus();
		//////////////////////////////////////////////////////////////////////////
	}
}


BOOL CMessageSplitDlg3::PreTranslateMessage(MSG* pMsg) 
{
	// TODO: Add your specialized code here and/or call the base class
	//TRACE("\r\n WM = 0x%X wParam = 0x%X (%d)",pMsg->message,pMsg->wParam, GetKeyState(VK_MENU));
	if(m_bWasCtrlExit&&m_bWasCtrlEnter)
	{
		m_bWasCtrlExit = m_bWasCtrlEnter = FALSE;
		COFSNcDlg2::PreTranslateMessage(pMsg);
		pMsg->message	=	WM_KEYDOWN;
		pMsg->wParam	=	VK_RETURN;
		pMsg->lParam	=	0;
		//m_edit.PostMessage(WM_KEYDOWN,VK_RETURN);
	}
	else
	if(pMsg->message==WM_KEYUP&&
		pMsg->wParam==VK_CONTROL&&
		!bBlock&&
		m_bWasCtrlEnter)
	{
		//m_bWasCtrlEnter = FALSE;
		m_bWasCtrlExit  = TRUE;
	}
	else
	if(pMsg->message==WM_KEYDOWN||pMsg->message==WM_SYSKEYDOWN)
	{
		//TRACE("\r\n WM_KEYDOWN wParam = 0x%X",pMsg->wParam);
		if(pMsg->wParam==VK_RETURN&&(GetKeyState(VK_CONTROL)>>1)&&!bBlock)
		{
			if(GetOptionInt(IDS_OFSMESSENGER,IDS_CTRLENTER,1)==0)
			{
				OnSendButton();
				return TRUE;
			}
			else
			{
				m_bWasCtrlEnter = TRUE;
				//pMsg->message	=	WM_KEYDOWN;
				//pMsg->wParam	=	VK_RETURN;
				//pMsg->lParam	=	0;
				//m_edit.SendMessage(WM_KEYDOWN,VK_RETURN);
			}
		}
		else if(pMsg->wParam==VK_RETURN&&(GetKeyState(VK_SHIFT)>>1)&&!bBlock)
		{
			if(GetOptionInt(IDS_OFSMESSENGER,IDS_SHIFTENTER,0)==0)
			{
				OnSendButton();
				return TRUE;
			}
			else
			{
				pMsg->message	=	WM_KEYDOWN;
				pMsg->wParam	=	VK_RETURN;
			}
		}
		else if(pMsg->wParam==VK_RETURN&&!bBlock)
		{
			if(GetOptionInt(IDS_OFSMESSENGER,IDS_ENTER,0)==0)
			{
				OnSendButton();
				return TRUE;
			}
			else
			{
				pMsg->message	=	WM_KEYDOWN;
				pMsg->wParam	=	VK_RETURN;
			}
		}
		else if(pMsg->wParam==0x53&&(GetKeyState(VK_MENU)>>1)&&!bBlock)
		{
			if(GetOptionInt(IDS_OFSMESSENGER,IDS_ALTS,0)==0)
			{
				OnSendButton();
				return TRUE;
			}
			else
			{
				pMsg->message	=	WM_KEYDOWN;
				pMsg->wParam	=	VK_RETURN;
			}
		}
		//else if(pMsg->wParam==0x4C&&(GetKeyState(VK_CONTROL)>>1)&&!bBlock)
		//{
		//	m_FontCombo.ShowDropDown();
		//}
	}
	return COFSNcDlg2::PreTranslateMessage(pMsg);
}

void CMessageSplitDlg3::Block()
{
	bBlock=TRUE;
	m_Send.SetFocus();
	m_edit.EnableWindow(FALSE);
	m_Send.EnableWindow(FALSE);
}

void CMessageSplitDlg3::UnBlock()
{
	CWnd *pActiveWnd = GetForegroundWindow();
	bBlock=FALSE;	
	
	//m_edit.EnableWindow(TRUE);
	m_Send.EnableWindow(TRUE);

	if(pActiveWnd->GetSafeHwnd()==GetSafeHwnd())
	{
		m_edit.EnableWindow(TRUE);
	}
	else
		m_bEnableIfActiavte  = TRUE;
}

void CMessageSplitDlg3::SetSender(CUser &user)
{
	m_Sender = user;
	//m_edit.SetFocus();
	CString strName;
	strName.Format(GetString(IDS_FROM_FORMAT),m_Sender.GetShowName());
	m_SenderUserInfo.SetText(strName);
}

void CMessageSplitDlg3::SetRecipient(CUser &user)
{
	m_Recipient = user;	
	CString strCaption;

	//GetWindowText(strCaption);
	strCaption.Format(GetString(IDS_INSTANT_CHAT_TITLE_FORMAT),m_Recipient.GetShowName());

    SetWindowText(strCaption);
	m_UserInfo.SetText(m_Recipient.GetShowName()+_T(" [")+ pMessenger->GetUserDomain() + _T("]"));
	Refresh();
	
	if(m_Recipient.IsSystemUser())
	{
		m_Send.EnableWindow(FALSE);
		m_FontCombo.EnableWindow(FALSE);
		m_SizeCombo.EnableWindow(FALSE);
		m_btnColor.EnableWindow(FALSE);
		m_btnBold.EnableWindow(FALSE);
		m_btnItalic.EnableWindow(FALSE);
		m_btnUnderline.EnableWindow(FALSE);
		m_btnSmiles.EnableWindow(FALSE);
		m_edit.EnableWindow(FALSE);
	}
	//m_edit.SetFocus();
}

LRESULT CMessageSplitDlg3::OnNetEvent(WPARAM w,LPARAM l)
{
	NLT_Container *pItem = (NLT_Container *)w;
	ASSERT(pItem!=NULL);
	
	theNet2.LockTranslator();
	theNet2.RemoveFromTranslator(Handle);
	theNet2.UnlockTranslator();

	UnBlock();

	switch(pItem->EventType)
	{
    case NLT_ECommandOK:
		MessageTime = pItem->Long1;

		if(!pMessenger->AddMessageToDataBase(m_Recipient.GetGlobalID(),pMessage->GetMID(),MessageTime,pMessage->GetBody()))
		{
			_SHOW_IBN_ERROR_DLG_OK(IDS_LOCDATABASE_ADDMESSAGE_ERROR);
		}

		bInitEdit = FALSE;
		m_edit.Clear();
		m_InitialDefaultFontName = m_edit.GetDefaultFontName();
		m_InitialDefaultFontSize = m_edit.GetDefaultFontSizeIndex();

		Refresh();
		
		break;
	case NLT_ECommandError:
		if(pItem->Long1==etSERVER)
		{
			switch(pItem->Long2)
			{
			case ERR_UNABLE_CREATE_CONN:
				_SHOW_IBN_ERROR_DLG_OK(IDS_SERVICENOTAVAILABLE);
				break;
			}
		}
		break;
	default:
		break;
	}
	
	delete pItem;
    return 0;
}

void CMessageSplitDlg3::KillWindow()
{
	if(GetStyle()&WS_VISIBLE&&GetOptionInt(IDS_OFSMESSENGER,IDS_ANIMATION,FALSE))
		RoundExitAddon(this);
	COFSNcDlg2::OnClose();

	if(!bIsKillWinodow)
	{
		CloseMpaWebEvent();
		bIsKillWinodow = TRUE;
		pMessenger->SendMessage(WM_KILL_SPLIT_MESSAGE_DLG,(WPARAM)m_Recipient.GetGlobalID(),(LPARAM)this);
		DestroyWindow();
		delete this;
	}
}


void CMessageSplitDlg3::OnEditmenuCopy() 
{
	m_edit.ClipboardCopy();	
}

void CMessageSplitDlg3::OnUpdateEditmenuCopy(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(m_edit.EnableClipboardCopy());
}

void CMessageSplitDlg3::OnEditmenuCut() 
{
	m_edit.ClipboardCut();
}

void CMessageSplitDlg3::OnUpdateEditmenuCut(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(m_edit.EnableClipboardCut());
}

void CMessageSplitDlg3::OnEditmenuDelete() 
{
	m_edit.ClipboardDelete();
}

void CMessageSplitDlg3::OnUpdateEditmenuDelete(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(m_edit.EnableClipboardDelete());
}

void CMessageSplitDlg3::OnEditmenuPast() 
{
	if (OpenClipboard())
	{
		HANDLE hText = GetClipboardData(CF_TEXT);

		if(hText!=NULL)
		{
			CComBSTR strText = (LPCTSTR)GlobalLock(hText);
			m_edit.InsertTEXT(strText);
			GlobalUnlock(hText);
		}
		
		CloseClipboard();
	}
		
	//m_edit.ClipboardPast();
}

void CMessageSplitDlg3::OnUpdateEditmenuPast(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(m_edit.EnableClipboardPast());
}



void CMessageSplitDlg3::OnCaptureChanged(CWnd *pWnd) 
{
	TRACE(_T("\r\n CMessageSplitDlg3::OnCaptureChanged"));

	CRect rWindow;
	GetWindowRect(&rWindow);
	if(!IsIconic())
		WriteOptionString(IDS_OFSMESSENGER,IDS_SPLITRECT,RectToString(rWindow));
	
	pMessenger->RemoveAllMessageById(m_Recipient.GetGlobalID());
	m_edit.SetFocus();

	COFSNcDlg2::OnCaptureChanged(pWnd);
}

BOOL CMessageSplitDlg3::Refresh()
{
	if(pMessenger)
	{
		IDispatchPtr pDispatch;
		pDispatch.Attach(m_History.GetDocument());
		if(pDispatch)
		{
			return pMessenger->GetMessagesBySID(bstr_t(m_Recipient.GetShowName()),
					m_Recipient.GetGlobalID(),
					TRUE/*Message DDialog*/,
					1/*Message DDialog*/,
					(IUnknown*)pDispatch,_T("/Shell/Chat/messages.xslt"));
		}
		else
		{
			m_bEnableRefresh	=	TRUE;
		}
	}
	return FALSE;
}




HRESULT CMessageSplitDlg3::OnAutoRefresh(WPARAM w, LPARAM l)
{
    //Refresh();
	return 0;
}

void CMessageSplitDlg3::OnMove(int x, int y) 
{
	COFSNcDlg2::OnMove(x, y);
	
	if(::IsWindow(GetParent()->GetSafeHwnd()))
		GetParent()->UpdateWindow();
	if(::IsWindow(pMessenger->GetMessageParent()->GetSafeHwnd()))
		pMessenger->GetMessageParent()->UpdateWindow();
}

void CMessageSplitDlg3::SetFon(HBITMAP hFon)
{
	// 03.04.2002 \-
	return;
	// 03.04.2002 /-
	
	/*if(pFonBmp)
		pFonBmp->DeleteObject();
    else
		pFonBmp = new CBitmap;
	
	pFonBmp->Attach(hFon);
	
	BITMAP hb;
	pFonBmp->GetBitmap(&hb);
	sFon = CSize(hb.bmWidth ,hb.bmHeight);
	
	SetMinTrackSize(sFon);
	SetWindowPos(NULL,-1,-1,sFon.cx,sFon.cy ,SWP_NOZORDER|SWP_NOMOVE|SWP_NOACTIVATE);

	CPictureHolder	tmpImage;
	tmpImage.CreateFromBitmap(pFonBmp);
	m_ResizeFon.Destroy();
	m_ResizeFon.Create(tmpImage.m_pPict);
	m_ResizeFon.AddAnchor(CRect(0,0,230,100),CSize(0,0),CSize(0,0));
	m_ResizeFon.AddAnchor(CRect(231,0,290,100),CSize(0,0),CSize(100,0),CResizableImage::DUPLICATE);
	m_ResizeFon.AddAnchor(CRect(291,0,413,100),CSize(100,0),CSize(100,0));
	m_ResizeFon.AddAnchor(CRect(0,101,230,170),CSize(0,0),CSize(0,100),CResizableImage::DUPLICATE);
	m_ResizeFon.AddAnchor(CRect(291,101,413,170),CSize(100,0),CSize(100,100),CResizableImage::DUPLICATE);
	m_ResizeFon.AddAnchor(CRect(0,171,230,350),CSize(0,100),CSize(0,100));
	m_ResizeFon.AddAnchor(CRect(231,171,290,350),CSize(0,100),CSize(100,100),CResizableImage::DUPLICATE);
	m_ResizeFon.AddAnchor(CRect(291,171,413,350),CSize(100,100),CSize(100,100));

	//AddAnchor(m_btnX.GetSafeHwnd(),CSize(100,0),CSize(100,0));
	//AddAnchor(m_btnMin.GetSafeHwnd(),CSize(100,0),CSize(100,0));
	AddAnchor(m_edit.GetSafeHwnd(),CSize(0,100),CSize(100,100));
	AddAnchor(m_History.GetSafeHwnd(),CSize(0,0),CSize(100,100));
//	AddAnchor(m_ColorButton.GetSafeHwnd(),CSize(0,100),CSize(0,100));
//	AddAnchor(m_BoldButton.GetSafeHwnd(),CSize(0,100),CSize(0,100));
//	AddAnchor(m_ItalicButton.GetSafeHwnd(),CSize(0,100),CSize(0,100));
//	AddAnchor(m_UnderLineButton.GetSafeHwnd(),CSize(0,100),CSize(0,100));
//	AddAnchor(m_InsertSmileButton.GetSafeHwnd(),CSize(0,100),CSize(0,100));
	AddAnchor(m_SizeCombo.GetSafeHwnd(),CSize(0,100),CSize(0,100));
	AddAnchor(m_FontCombo.GetSafeHwnd(),CSize(0,100),CSize(0,100));
	
	//AddAnchor(m_Send.GetSafeHwnd(),CSize(100,100),CSize(100,100));

	CRect rgnRect;
	GetWindowRect(&rgnRect);
	CRgn	WinRgn;
	WinRgn.CreateRoundRectRgn(0,0,rgnRect.Width(),rgnRect.Height(),20,20);
	SetWindowRgn(WinRgn,TRUE);
	
	CString strRect = GetOptionString(IDS_OFSMESSENGER,IDS_SPLITRECT,"");
	if(!strRect.IsEmpty())
	{
		CRect rWindow = StringToRect(strRect);
		FitRectToWindow(rWindow);
		SetWindowPos(NULL,rWindow.left,rWindow.top,rWindow.Width(),rWindow.Height(),SWP_NOZORDER|SWP_NOACTIVATE);
	}
	*/
}

void CMessageSplitDlg3::OnPaint() 
{
/*
CRect	winRect, editRect;
GetWindowRect(winRect);

  m_edit.GetWindowRect(&editRect);
  ScreenToClient(&editRect);
  
	CPaintDC dc(this); // device context for painting
	//CMemDC memdc(&dc);
	if(pFonBmp)
	{
	//CDC dc;
	//dc.CreateCompatibleDC(&memdc);
	//dc.SelectObject(pFonBmp);
	//memdc.BitBlt(0,0,sFon.cx,sFon.cy,&dc,0,0,SRCCOPY);
	//memdc.BitBlt(0,0,sFon.cx,sFon.cy,&dc,0,0,SRCCOPY);
	CRect m_Client;
	GetClientRect(&m_Client);
	CSize	winSize(m_Client.Width(),m_Client.Height());
	m_ResizeFon.Render(dc.GetSafeHdc(),winSize);
	m_UserInfo.Invalidate();
	}
*/
	
//	CPaintDC dc(this); // device context for painting
//	DrawBackground(&dc);
	COFSNcDlg2::OnPaint();
	m_UserInfo.Invalidate();
	m_SenderUserInfo.Invalidate();
}

//DEL void CMessageSplitDlg3::OnLButtonDown(UINT nFlags, CPoint point) 
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
//DEL 				COFSNcDlg2::OnNcLButtonDown(HTTOPLEFT,point);
//DEL 			else if(inPoint.y>miniRect.bottom)
//DEL 				COFSNcDlg2::OnNcLButtonDown(HTBOTTOMLEFT,point);
//DEL 			else
//DEL 				COFSNcDlg2::OnNcLButtonDown(HTLEFT,point);
//DEL 			else if(inPoint.x>miniRect.right)
//DEL 				if(inPoint.y<miniRect.top)
//DEL 					COFSNcDlg2::OnNcLButtonDown(HTTOPRIGHT,point);
//DEL 				else if(inPoint.y>miniRect.bottom)
//DEL 					COFSNcDlg2::OnNcLButtonDown(HTBOTTOMRIGHT,point);
//DEL 				else
//DEL 					COFSNcDlg2::OnNcLButtonDown(HTRIGHT,point);
//DEL 				else if(inPoint.y<miniRect.top)
//DEL 					COFSNcDlg2::OnNcLButtonDown(HTTOP,point);
//DEL 				else
//DEL 					COFSNcDlg2::OnNcLButtonDown(HTBOTTOM,point);
//DEL 	}
//DEL 	else
//DEL 		COFSNcDlg2::OnNcLButtonDown(HTCAPTION,point);
//DEL 
//DEL 	return;
//DEL 
//DEL 	
//DEL 
//DEL 	COFSNcDlg2::OnLButtonDown(nFlags, point);
//DEL }

//DEL BOOL CMessageSplitDlg3::OnEraseBkgnd( CDC* pDC )
//DEL {
//DEL 	return TRUE;
//DEL }

BEGIN_EVENTSINK_MAP(CMessageSplitDlg3, COFSNcDlg2)
    //{{AFX_EVENTSINK_MAP(CMessageSplitDlg3)
	ON_EVENT(CMessageSplitDlg3, IDC_MCCLOSE, -600 /* Click */, OnClickMcclose, VTS_NONE)
	ON_EVENT(CMessageSplitDlg3, IDC_MCSEND, -600 /* Click */, OnClickMcsend, VTS_NONE)
	ON_EVENT(CMessageSplitDlg3, IDC_MCOPTIONS, -600 /* Click */, OnClickMcoptions, VTS_NONE)
	ON_EVENT(CMessageSplitDlg3, IDC_MCMENU, -600 /* Click */, OnClickMcmenu, VTS_NONE)
	ON_EVENT(CMessageSplitDlg3, IDC_MCMINI, -600 /* Click */, OnClickMcmini, VTS_NONE)
	ON_EVENT(CMessageSplitDlg3, IDC_BTN_COLOR, -600 /* Click */, OnClickBtnColor, VTS_NONE)
	ON_EVENT(CMessageSplitDlg3, IDC_BTN_BOLD, -600 /* Click */, OnClickBtnBold, VTS_NONE)
	ON_EVENT(CMessageSplitDlg3, IDC_BTN_ITALIC, -600 /* Click */, OnClickBtnItalic, VTS_NONE)
	ON_EVENT(CMessageSplitDlg3, IDC_BTN_UNDERLINE, -600 /* Click */, OnClickBtnUnderline, VTS_NONE)
	ON_EVENT(CMessageSplitDlg3, IDC_BTN_SMILES, -600 /* Click */, OnClickBtnSmiles, VTS_NONE)
	ON_EVENT(CMessageSplitDlg3, IDC_EXPLORER, 250 /* BeforeNavigate2 */, OnBeforeNavigate2History, VTS_DISPATCH VTS_PVARIANT VTS_PVARIANT VTS_PVARIANT VTS_PVARIANT VTS_PVARIANT VTS_PBOOL)
	ON_EVENT(CMessageSplitDlg3, IDC_EXPLORER, 259 /*DocumentComplete*/, OnDocumentComplete2History, VTS_DISPATCH VTS_PVARIANT)
	//ON_EVENT(CMessageSplitDlg3, IDC_EXPLORER, 104/*DISPID_DOWNLOADCOMPLETE*/,OnDownloadCompleteHistory,VTS_DISPATCH VTS_PVARIANT)
	//}}AFX_EVENTSINK_MAP
END_EVENTSINK_MAP()

void CMessageSplitDlg3::OnClickMcclose() 
{
	OnCancel();
}

void CMessageSplitDlg3::OnClickMcsend() 
{
	OnSendButton();
}

void CMessageSplitDlg3::OnClickMcmenu() 
{
	pMessenger->ShowUserMenu(m_Recipient.GetGlobalID());
}

void CMessageSplitDlg3::OnClickMcmini() 
{
	ShowWindow(SW_MINIMIZE);
}

void CMessageSplitDlg3::OnClickMcoptions() 
{
	pMessenger->PreferenceDlg(this);
}

//DEL void CMessageSplitDlg3::OnDestroy() 
//DEL {
//DEL 	COFSNcDlg2::OnDestroy();
//DEL }

//DEL void CMessageSplitDlg3::OnSize(UINT nType, int cx, int cy) 
//DEL {
//DEL 	COFSNcDlg2::OnSize(nType, cx, cy);
//DEL 
//DEL 	// 03.04.2002 \-
//DEL 	return;
//DEL 	// 03.04.2002 /-
//DEL 	
//DEL 	CRect rgnRect;
//DEL 	GetWindowRect(&rgnRect);
//DEL 	CRgn	WinRgn;
//DEL 	WinRgn.CreateRoundRectRgn(0,0,rgnRect.Width(),rgnRect.Height(),20,20);
//DEL 	SetWindowRgn(WinRgn,TRUE);
//DEL 	Invalidate(FALSE);
//DEL 
//DEL 	if(m_Send.GetSafeHwnd())
//DEL 	{
//DEL 		m_Send.SetWindowPos(NULL,cx-53,cy-101,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//DEL 		m_btnX.SetWindowPos(NULL,cx-31,13,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//DEL 		m_btnMin.SetWindowPos(NULL,cx-53,13,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//DEL 		//m_edit.SetFocus();
//DEL 	}
//DEL }

BOOL CMessageSplitDlg3::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message) 
{
	CRect StatusRect, miniRect;
	
	GetClientRect(&StatusRect);
				
	CPoint point, inPoint;
	
	::GetCursorPos(&point);
	inPoint = point;
	ScreenToClient(&inPoint);
	
	//CRect ResizeRect(StatusRect.Width()-20,StatusRect.Height()-20,StatusRect.Width(),StatusRect.Height());
	//if(ResizeRect.PtInRect(inPoint))
	//{
	//	SetCursor(LoadCursor(NULL,IDC_SIZENWSE));
	//	return TRUE;
	//}
	miniRect = StatusRect;miniRect.InflateRect(-9,-9,-9,-9);

	if(!miniRect.PtInRect(inPoint))
	{
		if(inPoint.x<miniRect.left)
			if(inPoint.y<miniRect.top)
				nHitTest = HTTOPLEFT;
			else if(inPoint.y>miniRect.bottom)
				nHitTest = HTBOTTOMLEFT;
			else
				nHitTest = HTLEFT;
		else if(inPoint.x>miniRect.right)
			if(inPoint.y<miniRect.top)
				nHitTest = HTTOPRIGHT;
			else if(inPoint.y>miniRect.bottom)
				nHitTest = HTBOTTOMRIGHT;
			else
				nHitTest = HTRIGHT;
		else if(inPoint.y<miniRect.top)
			nHitTest = HTTOP;
		else
			nHitTest = HTBOTTOM;
	}
	
	return COFSNcDlg2::OnSetCursor(pWnd, nHitTest, message);
}

//DEL void CMessageSplitDlg3::OnLButtonUp(UINT nFlags, CPoint point) 
//DEL {
//DEL 	COFSNcDlg2::OnLButtonUp(nFlags, point);
//DEL }

//DEL void CMessageSplitDlg3::OnMouseMove(UINT nFlags, CPoint point) 
//DEL {
//DEL 	COFSNcDlg2::OnMouseMove(nFlags, point);
//DEL }

//DEL void CMessageSplitDlg3::OnCancelMode() 
//DEL {
//DEL 	COFSNcDlg2::OnCancelMode();
//DEL 	
//DEL }

void CMessageSplitDlg3::OnDropFiles( HDROP hDropInfo )
{
	SetForegroundWindow();
	
	UINT FileCount = DragQueryFile(hDropInfo,0xFFFFFFFF,NULL,0);
	if(FileCount>30)
	{
		CString	strMessage;
		strMessage.LoadString(IDS_FILES_SEND_LIMIT);
		MessageBox(strMessage,GetString(IDS_ERROR_TITLE),MB_OK|MB_ICONERROR);
		return ;
	}
	
	
	CString strDescription;
	CFileDescriptioDlg	DescrDlg(this);
	
	DescrDlg.m_strFileName = _T("");
	for(UINT i=0;i<FileCount;i++)
	{
		CString strPath;
		TCHAR  FileBuffer[MAX_PATH];
		DragQueryFile(hDropInfo,i,FileBuffer,MAX_PATH);
		strPath = FileBuffer;
		strPath = strPath.Mid(strPath.ReverseFind('\\')+1);
		
		DescrDlg.m_strFileName += strPath;
		DescrDlg.m_strFileName += _T("; ");
	}
	
	
	if(DescrDlg.DoModalEditMode()==IDOK)
	{
		strDescription = DescrDlg.GetDescription();	

		for(UINT i=0;i<FileCount;i++)
		{
			TCHAR FileBuffer[MAX_PATH];
			DragQueryFile(hDropInfo,i,FileBuffer,MAX_PATH);
			pMessenger->SendFileToUser(&m_Recipient,FileBuffer,strDescription);
		}
	}
}

void CMessageSplitDlg3::OnColorButton() 
{
	CColorDialog m_Color(m_edit.GetColor(), CC_ANYCOLOR,this);
	if(m_Color.DoModal()==IDOK)
	{
		m_edit.SetColor(m_Color.GetColor());
	}
	m_edit.SetFocus();
}

//DEL void CMessageSplitDlg3::OnBoldCheck() 
//DEL {
//DEL 	// TODO: Add your control notification handler code here
//DEL 	m_edit.SetBold();
//DEL 	m_edit.SetFocus();
//DEL 	WriteOptionInt(IDS_OFSMESSENGER,IDS_BOLD,m_BoldButton.GetCheck());
//DEL }

//DEL void CMessageSplitDlg3::OnItalicCheck() 
//DEL {
//DEL 	// TODO: Add your control notification handler code here
//DEL     m_edit.SetItalic();
//DEL 	m_edit.SetFocus();
//DEL 	WriteOptionInt(IDS_OFSMESSENGER,IDS_ITALIC,m_ItalicButton.GetCheck());
//DEL }

//DEL void CMessageSplitDlg3::OnUnderlineCheck() 
//DEL {
//DEL 	// TODO: Add your control notification handler code here
//DEL 	m_edit.SetUnderline();
//DEL 	m_edit.SetFocus();
//DEL 	WriteOptionInt(IDS_OFSMESSENGER,IDS_UNDERLINE,m_UnderLineButton.GetCheck());
//DEL }

HRESULT CMessageSplitDlg3::OnEditUpdate(WPARAM w, LPARAM l)
{
	if(!m_strSetBody.IsEmpty())
	{
		CComBSTR	bstSetBody = m_strSetBody;
		m_edit.InsertTEXT(bstSetBody);
		m_strSetBody.Empty();

		if(m_bSendBody)
		{
			OnSendButton();
			m_bSendBody = FALSE;
		}
	}
	
	if(!bInitEdit)
	{
		bInitEdit = TRUE;
		
		// Try Fix Font Jamping. [7/22/2002]
		/*
		m_edit.SetTextSize(GetOptionInt(IDS_OFSMESSENGER,IDS_SIZE,1)+1);	
				CString strFontName = _T("Arial");
				int FontId = GetOptionInt(IDS_OFSMESSENGER,IDS_FONT,-1);
				if(FontId!=-1)
					m_FontCombo.GetLBText(FontId, strFontName);
				CComBSTR	bsFontName = strFontName;
				m_edit.SetFontName(bsFontName);*/
		
		
		if(GetOptionInt(IDS_OFSMESSENGER,IDS_UNDERLINE,0))
		{
			//m_UnderLineButton.SetCheck(1);
			m_edit.SetUnderline();
		}
		if(GetOptionInt(IDS_OFSMESSENGER,IDS_ITALIC,0))
		{
			//m_ItalicButton.SetCheck(1);
			m_edit.SetItalic();
		}
		if(GetOptionInt(IDS_OFSMESSENGER,IDS_BOLD,0))
		{
			//m_BoldButton.SetCheck(1);
			m_edit.SetBold();
		}

	}
	
	CCmdUI m_Command;
	
/*
	m_Command.m_nID = m_BoldButton.GetDlgCtrlID();
	m_Command.m_pOther = (CWnd*)&m_BoldButton;
	m_edit.UpdateCmdControl(&m_Command,IDM_TRIED_BOLD,TRUE);

	m_Command.m_nID = m_ItalicButton.GetDlgCtrlID();
	m_Command.m_pOther = (CWnd*)&m_ItalicButton;
	m_edit.UpdateCmdControl(&m_Command,IDM_TRIED_ITALIC,TRUE);

	m_Command.m_nID = m_UnderLineButton.GetDlgCtrlID();
	m_Command.m_pOther = (CWnd*)&m_UnderLineButton;
	m_edit.UpdateCmdControl(&m_Command,IDM_TRIED_UNDERLINE,TRUE);
*/
	m_edit.UpdateCmdControl(&m_btnBold, IDM_TRIED_BOLD, TRUE);
	m_edit.UpdateCmdControl(&m_btnItalic, IDM_TRIED_ITALIC, TRUE);
	m_edit.UpdateCmdControl(&m_btnUnderline, IDM_TRIED_UNDERLINE, TRUE);
	
	int TextSize = m_edit.GetTextSize();
	//  [4/29/2002]
	if(TextSize>=1)
		m_SizeCombo.SetCurSel(m_edit.GetTextSize()-1);
	else
	{
		m_edit.SetTextSize(m_SizeCombo.GetCurSel()+1);
	}
	//  [4/29/2002]
	
	CString strFontName = m_edit.GetFontName();
	
	for (int i=0; i < m_FontCombo.GetCount(); ++i)
	{
		CString itemStr;
		
		m_FontCombo.GetLBText(i, itemStr);
		
		if ( itemStr == strFontName)
		{
			m_FontCombo.SetCurSel(i);
			break;
		}
	}
	
	
	return 0;
}

void CMessageSplitDlg3::OnSelendokSizeCombo() 
{
	WriteOptionInt(IDS_OFSMESSENGER,IDS_SIZE,m_SizeCombo.GetCurSel());
	m_edit.SetTextSize(m_SizeCombo.GetCurSel()+1);
	//  [4/29/2002]
	m_edit.SetDefaultFontSize(nFontSizes[m_SizeCombo.GetCurSel()]);
	//  [4/29/2002]
	m_edit.SetFocus();
}

void CMessageSplitDlg3::OnSelendokFontCombo() 
{
	int nIndex = m_FontCombo.GetCurSel();
	CString strFontName;
	m_FontCombo.GetLBText(nIndex, strFontName);

	WriteOptionString(IDS_OFSMESSENGER,IDS_FONT,strFontName);

	CComBSTR	bsFontName = strFontName;
	m_edit.SetFontName(bsFontName);
	//  [4/29/2002]
	m_edit.SetDefaultFontName(bsFontName);
	//  [4/29/2002]
	m_edit.SetFocus();
}

void CMessageSplitDlg3::OnInsertsmileCheck() 
{
	// New Smile Addon [2007-02-06]
	CMenu smileMenu;
	smileMenu.CreatePopupMenu();

	int index = 0;
	for(CSmileInfoListEnum item = CurrentSmileManager.GetSmiles().begin();item!=CurrentSmileManager.GetSmiles().end() && index<(3*8-1);item++,index++)
	{
		CString str;
		str.Format("%s\t%s", (*item).GetText(), (*item).GetSmile());
		
		if(index!=0 && (index%8)==0)
			VERIFY(smileMenu.AppendMenu(MF_STRING|MF_ENABLED|MF_MENUBREAK, 20001 + (*item).GetIndex(), str));
		else
			VERIFY(smileMenu.AppendMenu(MF_STRING|MF_ENABLED, 20001 + (*item).GetIndex(), str));

	}

	smileMenu.AppendMenu(MF_STRING|MF_ENABLED, 20000, GetString(IDS_SMILES_MORE));

	CPoint	curPoint;
	GetCursorPos(&curPoint);
	smileMenu.TrackPopupMenu(TPM_LEFTBUTTON,curPoint.x,curPoint.y,this);
	//
}

void CMessageSplitDlg3::OnSmileItem(UINT nID)
{
	// New Smile Addon [2007-02-06]
	int SmileId = -1;

	if(nID==20000)
	{
		// Show Smile Select Popup Window.
		CSelectSmileDlg dlgSelectSmile(this);

		if(dlgSelectSmile.DoModal()==IDOK)
		{
			SmileId = dlgSelectSmile.GetSelectedSmileIndex();
		}
	}
	else
		SmileId =  nID - 20001;

	if(SmileId>=0)
	{

		CSmileInfo smileInfo = CurrentSmileManager.GetSmile(SmileId);
		
		if(smileInfo!=CSmileInfo::Empty)
		{
			CurrentSmileManager.IncHitCount(smileInfo.GetId());

			BOOL bShift = GetKeyState(VK_SHIFT)>>1;

			if(bShift)
			{
				CComBSTR strSmileHtml = smileInfo.GetHtmlSmile();
				m_edit.InsertHTML(strSmileHtml);
			}
			else
			{
				CComBSTR strSmileHtml = L"<img id=\"";
				strSmileHtml += smileInfo.GetId();
				strSmileHtml += "\" title=\"";
				strSmileHtml += smileInfo.GetText();
				strSmileHtml += "\" src=\"";
				strSmileHtml += IBN_SCHEMA;
				strSmileHtml += (LPCTSTR)GetProductLanguage();
				strSmileHtml += L"/Shell/Smiles/";
				strSmileHtml += smileInfo.GetId();
				strSmileHtml += L".gif\">";

				m_edit.InsertHTML(strSmileHtml);
			}

			m_edit.SetFocus();
		}
		else
		{
			ASSERT(FALSE);
		}
	}
	//

}


void CMessageSplitDlg3::SetBody(LPCTSTR strBody)
{
	m_strSetBody = strBody;
	CComBSTR	bsText	=	L"";
	m_edit.InsertTEXT(bsText);
	m_edit.SetFocus();
}


LPARAM CMessageSplitDlg3::OnSWMSetBody(WPARAM w, LPARAM l)
{
	if(!Handle)
	{
		if(l!=NULL)
		{
			m_bSendBody = TRUE;
		}

		SetBody(LPCTSTR(w));
	}
	return 0;
}

void CMessageSplitDlg3::OnActivate( UINT nState, CWnd* pWndOther, BOOL bMinimized )
{
	TRACE(_T("\r\n -- CMessageSplitDlg3::OnActivate nState== %d, bMinimized = %d"), nState, bMinimized);

	//  [7/23/2002]
	if(m_bEnableIfActiavte)
	{
		m_edit.EnableWindow(TRUE);
		m_bEnableIfActiavte = FALSE;
	}

	//if(m_bDisableEditOnStart)
	//{
		//TRACE("\r\n -- CMessageSplitDlg3::OnActivate m_bEnableIfActiavte	=	TRUE");
		//m_bEnableIfActiavte = m_bDisableEditOnStart;
		//m_bDisableEditOnStart	=	FALSE;
	//}
	
	if((nState==WA_ACTIVE||nState==WA_CLICKACTIVE)&&!bMinimized)
	{
		pMessenger->RemoveAllMessageById(m_Recipient.GetGlobalID());
		m_edit.SetFocus();
	}

	COFSNcDlg2::OnActivate(nState, pWndOther, bMinimized);
}

BOOL CMessageSplitDlg3::OnNcActivate(BOOL bActive)
{
	TRACE(_T("\r\n -- CMessageSplitDlg3::OnNcActivate bActive== %d"), bActive);

	if(m_bEnableIfActiavte)
	{
		m_edit.EnableWindow(TRUE);
		m_bEnableIfActiavte = FALSE;
	}

	if(bActive)
	{
		//pMessenger->RemoveAllMessageById(m_Recipient.GetGlobalID());
		//m_edit.SetFocus();
	}

	return COFSNcDlg2::OnNcActivate(bActive);
}	

void CMessageSplitDlg3::LoadSkin(IXMLDOMNode *pXmlRoot)
{
	
	//  [7/23/2002]
	m_btnX.ShowWindow(SW_HIDE);
	m_btnMin.ShowWindow(SW_HIDE);
	m_Send.ShowWindow(SW_HIDE);
	m_Menu.ShowWindow(SW_HIDE);
	m_Options.ShowWindow(SW_HIDE);
//	m_UserInfo.ShowWindow(SW_HIDE);
	m_SenderUserInfo.ShowWindow(SW_HIDE);
	m_btnColor.ShowWindow(SW_HIDE);
	m_btnBold.ShowWindow(SW_HIDE);
	m_btnItalic.ShowWindow(SW_HIDE);
	m_btnUnderline.ShowWindow(SW_HIDE);
	m_btnSmiles.ShowWindow(SW_HIDE);
	
	LoadButton(pXmlRoot, _T("X"), &m_btnX, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Minimize"), &m_btnMin, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Send"), &m_Send, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Menu"), &m_Menu, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Options"), &m_Options, TRUE, FALSE);

	LoadButton(pXmlRoot, _T("Color"), &m_btnColor, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Bold"), &m_btnBold, TRUE, TRUE);
	LoadButton(pXmlRoot, _T("Italic"), &m_btnItalic, TRUE, TRUE);
	LoadButton(pXmlRoot, _T("Underline"), &m_btnUnderline, TRUE, TRUE);
	LoadButton(pXmlRoot, _T("Smiles"), &m_btnSmiles, TRUE, FALSE);
	
	LoadLabel(pXmlRoot, _T("Recipient"), &m_UserInfo, TRUE);
	LoadLabel(pXmlRoot, _T("Sender"), &m_SenderUserInfo, TRUE);
	
	LoadRectangle2(pXmlRoot, _T("History"), m_History.GetSafeHwnd(), TRUE, TRUE);
	LoadRectangle2(pXmlRoot, _T("Edit"), m_edit.GetSafeHwnd(), TRUE);
	LoadRectangle2(pXmlRoot, _T("ComboFont"), m_FontCombo.GetSafeHwnd(), TRUE);
	LoadRectangle2(pXmlRoot, _T("ComboSize"), m_SizeCombo.GetSafeHwnd(), TRUE);
}

int CMessageSplitDlg3::OnCreate(LPCREATESTRUCT lpCreateStruct) 
{
	if (COFSNcDlg2::OnCreate(lpCreateStruct) == -1)
		return -1;

	// TODO: Add your specialized creation code here
	//  [7/23/2002]
	CRect r(1,2,3,202);

	m_SizeCombo.Create(CBS_DROPDOWNLIST|WS_VISIBLE|WS_TABSTOP|WS_CHILD, r, this, IDC_SIZE_COMBO);
    m_FontCombo.Create(CBS_DROPDOWNLIST|WS_VSCROLL|CBS_SORT |WS_VISIBLE|WS_TABSTOP|WS_CHILD, r, this, IDC_FONT_COMBO);

	m_edit.Create(NULL, NULL, WS_TABSTOP|WS_CHILD|WS_VISIBLE|WS_DISABLED/*|(m_bDisableEditOnStart?WS_DISABLED:0)*/, r, this, IDC_DHTML_EDIT);

	m_edit.EnableWindow();
	
	return 0;
}

BOOL CMessageSplitDlg3::Create(CWnd *pParentWnd, BOOL bDisableEditOnStart)
{
	TRACE(_T("\r\n CMessageSplitDlg3::Create"));
	m_bDisableEditOnStart	=	bDisableEditOnStart;

	if(!COFSNcDlg2::Create(IDD, pParentWnd))
	{
		TRACE0("Warning: failed to create CMessageSplitDlg3.\n");
		return FALSE;
	}

	TRACE(_T("\r\n CMessageSplitDlg3::Create/COFSNcDlg2::LoadSkin"));
	if(!m_bLoadSkin)
		COFSNcDlg2::LoadSkin();
	TRACE(_T("\r\n CMessageSplitDlg3::Create/COFSNcDlg2::LoadSkin end"));

//  [7/23/2002]	
	CString strRect = GetOptionString(IDS_OFSMESSENGER, IDS_SPLITRECT, _T(""));
	if(!strRect.IsEmpty())
	{
		CRect rWindow = StringToRect(strRect);
		FitRectToWindow(rWindow);
		SetWindowPos(NULL,rWindow.left,rWindow.top,rWindow.Width(),rWindow.Height(),SWP_NOZORDER|SWP_NOACTIVATE);
	}
	TRACE(_T("\r\n CMessageSplitDlg3::Create End"));
	return TRUE;
}

void CMessageSplitDlg3::OnClickBtnColor() 
{
	CColorDialog dlg(m_edit.GetColor(), CC_ANYCOLOR, this);
	m_bIgnoreActivate = TRUE;
	if(dlg.DoModal() == IDOK)
	{
		m_edit.SetColor(dlg.GetColor());
	}
	m_bIgnoreActivate = FALSE;
	m_edit.SetFocus();
}

void CMessageSplitDlg3::OnClickBtnBold() 
{
	m_edit.SetBold();
	m_edit.SetFocus();
	WriteOptionInt(IDS_OFSMESSENGER,IDS_BOLD, m_btnBold.GetPressed());
}

void CMessageSplitDlg3::OnClickBtnItalic() 
{
    m_edit.SetItalic();
	m_edit.SetFocus();
	WriteOptionInt(IDS_OFSMESSENGER,IDS_ITALIC, m_btnItalic.GetPressed());
}

void CMessageSplitDlg3::OnClickBtnUnderline() 
{
	m_edit.SetUnderline();
	m_edit.SetFocus();
	WriteOptionInt(IDS_OFSMESSENGER,IDS_UNDERLINE, m_btnUnderline.GetPressed());
}

void CMessageSplitDlg3::OnClickBtnSmiles() 
{
/*	CSmileYahooPopup SmilePopup;
	SmilePopup.Create(18, 18, 5, IDB_SMILES, 0x808080);
	CPoint pt;
	GetCursorPos(&pt);
	SmilePopup.TrackPopupMenu(TPM_LEFTBUTTON, pt.x, pt.y, this);*/

	OnInsertsmileCheck();
}

void CMessageSplitDlg3::OnBeforeNavigate2History(LPDISPATCH pDisp, VARIANT FAR* URL, VARIANT FAR* Flags, VARIANT FAR* TargetFrameName, VARIANT FAR* PostData, VARIANT FAR* Headers, BOOL FAR* Cancel) 
{
	/*if(IsTVFileType(URL->bstrVal))
	{
		long cx = 0, cy = 0, tearoff = 0, fullscreen = 0;
		GetTargetRect(pDisp, cx, cy, tearoff, fullscreen);
		
		CDlgTV *pDlg = new CDlgTV;
		if(pDlg)
		{
			pDlg->CreateAutoKiller(URL->bstrVal, GetDesktopWindow(), cx, cy);
			*Cancel = TRUE;
		}
	}
	else*/
	{
		if(m_bEnableNavigateHistory)
		{
			m_bEnableNavigateHistory = FALSE;
			return;
		}
		else
		{
			*Cancel = TRUE;
			CString	strUrl(URL->bstrVal);

			if(strUrl==_T("about:blank#userdetails.show"))
			{
				if(!m_Recipient.IsSystemUser())
				{
					pMessenger->ShowUserDetails(m_Recipient.GetGlobalID());
				}
			}
			else
			{
				strUrl = pMessenger->ChangeUrlForCurrentDomain(strUrl);
				
				m_bEnableNavigateHistory = TRUE;

				if(S_OK != ::NavigateNewWindow(m_History.GetControlUnknown(), strUrl))
					ShellExecute(::GetDesktopWindow(), _T("open"), strUrl, NULL, NULL, SW_SHOWDEFAULT);

				m_bEnableNavigateHistory = FALSE;
			}
		}
	}
}

void CMessageSplitDlg3::InitMpaWebEvent()
{
	IConnectionPointContainer* pCPContainer = NULL;
	IConnectionPointPtr  m_pSessionConnectionPoint;
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

void CMessageSplitDlg3::CloseMpaWebEvent()
{
	IConnectionPointContainer* pCPContainer = NULL;
	IConnectionPointPtr  m_pSessionConnectionPoint;
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

void CMessageSplitDlg3::OnCmdGetVariable(LPCTSTR bstrVarName, BSTR* bstrVarValue)
{
	if(_tcscmp(bstrVarName, _T("ServerPath"))==0)
	{
		bstr_t strReturn = (LPCTSTR)pMessenger->GetServerPath();
		(*bstrVarValue) = strReturn.copy();
	}
	else if(_tcscmp(bstrVarName, _T("WebHost"))==0)
	{
		bstr_t strReturn = (LPCTSTR)pMessenger->GetWebHOST();
		(*bstrVarValue) = strReturn.copy();
	}
}

void CMessageSplitDlg3::OnShowContextMenu(long dwID, long x, long y, IUnknown *pcmdTarget, IDispatch *pdispReserved, long* pShow)
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

void CMessageSplitDlg3::OnDocumentComplete2History(LPDISPATCH pDisp, VARIANT FAR* URL)
{
	if(m_bEnableRefresh)
	{
		Refresh();
		m_bEnableRefresh	=	FALSE;
	}

	
}

