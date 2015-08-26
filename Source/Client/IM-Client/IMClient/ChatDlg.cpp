// MessageSplitDlg.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "MainDlg.h"
#include "ChatDlg.h"
#include "resource.h"
#include "User.h"
#include <triedcid.h>

#include "MemDc.h"
//#include "smileyahoopopup.h"
#include "FileDescriptioDlg.h"
#include "DlgTV.h"
#include "LoadSkins.h"
#include "cdib.h"

#include "mshtmcid.h"

//#include "McCreateWebFolders.h"

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
// CChatDlg dialog
extern CString GetCurrentSkin();

CChatDlg::CChatDlg(CMainDlg *pMessenger,CWnd* pParent /*=NULL*/)
: COFSNcDlg2(CChatDlg::IDD, pParent)
{
	EnableAutomation();
	//{{AFX_DATA_INIT(CChatDlg)
	//}}AFX_DATA_INIT
	m_bDisableEditOnStart	=	FALSE;
	m_strSkinSettings = _T("/Shell/Conference/skin.xml");
	bBlock  = FALSE;	
	this->pMessenger = pMessenger;
	Handle  = 0L;
	MessageTime = 0L;
	bInitEdit = FALSE;
	SetBoundary(0,0);
	SetBoundaryColor(RGB(0,0,0));
	SetCaption(COLORREF(0), COLORREF(0), 0);
	bIsKillWinodow	=	FALSE;
	m_bWasCtrlEnter	=	FALSE;
	m_bWasCtrlExit	=	FALSE;
	m_bLoadSkin = FALSE;
	m_dwSessionCookie	=	0;
	m_bEnableIfActiavte	=	FALSE;
	m_bEnableRefresh	=	FALSE;
	
	CurrTID = -1;
}

CChatDlg::~CChatDlg()
{
}


void CChatDlg::DoDataExchange(CDataExchange* pDX)
{
	COFSNcDlg2::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CChatDlg)
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
	DDX_Control(pDX, IDC_USER_LIST, m_treebox);
	DDX_Control(pDX, IDC_BTN_USERS, m_btnUsers);
	DDX_Control(pDX, IDC_BTN_FILES, m_btnFiles);
	//}}AFX_DATA_MAP
	//if(!IsWindow(m_InWindow.GetSafeHwnd()))
	//	m_InWindow.CreateAsChild(this, pMessenger);
	//if(!IsWindow(m_WebFolderView.GetSafeHwnd()))
	//	m_WebFolderView.Create(AfxRegisterWndClass(CS_HREDRAW|CS_VREDRAW|CS_DBLCLKS,::LoadCursor(NULL, IDC_ARROW), (HBRUSH) ::GetStockObject(WHITE_BRUSH), NULL),
	//	NULL,WS_OVERLAPPED|WS_VISIBLE|WS_CHILD|WS_CLIPCHILDREN,CRect(10,10,20,20),this,777);
	
}


BEGIN_MESSAGE_MAP(CChatDlg, COFSNcDlg2)
//{{AFX_MSG_MAP(CChatDlg)
ON_WM_CREATE()
ON_BN_CLICKED(IDOK, OnOk)
ON_MESSAGE(WM_NLT_CONTAINER_EVENT,OnNetEvent)	
//ON_BN_CLICKED(IDC_SEND_BUTTON, OnSendButton)
//ON_BN_CLICKED(IDC_COLOR_BUTTON, OnColorButton)
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
//ON_BN_CLICKED(IDC_INSERTSMILE_CHECK, OnInsertsmileCheck)
ON_WM_ACTIVATE()
ON_MESSAGE(WM_EDIT_UPDATE,OnEditUpdate)
ON_MESSAGE(WM_AUTOREFRESH,OnAutoRefresh)
ON_WM_NCACTIVATE()
//}}AFX_MSG_MAP
//ON_MESSAGE(WM_SET_RECIPIENT,OnSetRecipient)
ON_MESSAGE(WM_SWM_REFRESH,OnSWMRefreh)
ON_MESSAGE(WM_SWM_SETBODY,OnSWMSetBody)
ON_COMMAND_RANGE(20000,20000+SmileBuffSize,OnSmileItem)
END_MESSAGE_MAP()


BEGIN_INTERFACE_MAP(CChatDlg, CCmdTarget)
INTERFACE_PART(CChatDlg, __uuidof(_IMpaWebCustomizerEvents), Dispatch)
END_INTERFACE_MAP()

BEGIN_DISPATCH_MAP(CChatDlg, CCmdTarget)
//{{AFX_DISPATCH_MAP(CEventContainer)	
DISP_FUNCTION_ID(CChatDlg,"", 18, OnShowContextMenu,    VT_EMPTY, VTS_I4 VTS_I4 VTS_I4 VTS_I4 VTS_I4 VTS_PI4)
//}}AFX_DISPATCH_MAP
END_DISPATCH_MAP()

BEGIN_EVENTSINK_MAP(CChatDlg, COFSNcDlg2)
//{{AFX_EVENTSINK_MAP(CChatDlg)
ON_EVENT(CChatDlg, IDC_MCCLOSE, -600 , OnClickMcclose, VTS_NONE)
ON_EVENT(CChatDlg, IDC_MCSEND, -600 , OnClickMcsend, VTS_NONE)
ON_EVENT(CChatDlg, IDC_MCOPTIONS, -600 , OnClickMcoptions, VTS_NONE)
ON_EVENT(CChatDlg, IDC_MCMENU, -600 , OnClickMcmenu, VTS_NONE)
ON_EVENT(CChatDlg, IDC_MCMINI, -600 , OnClickMcmini, VTS_NONE)
ON_EVENT(CChatDlg, IDC_BTN_COLOR, -600 , OnClickBtnColor, VTS_NONE)
ON_EVENT(CChatDlg, IDC_BTN_BOLD, -600 , OnClickBtnBold, VTS_NONE)
ON_EVENT(CChatDlg, IDC_BTN_ITALIC, -600 , OnClickBtnItalic, VTS_NONE)
ON_EVENT(CChatDlg, IDC_BTN_UNDERLINE, -600 , OnClickBtnUnderline, VTS_NONE)
ON_EVENT(CChatDlg, IDC_BTN_SMILES, -600 , OnClickBtnSmiles, VTS_NONE)
ON_EVENT(CChatDlg, IDC_EXPLORER, 250 , OnBeforeNavigate2History, VTS_DISPATCH VTS_PVARIANT VTS_PVARIANT VTS_PVARIANT VTS_PVARIANT VTS_PVARIANT VTS_PBOOL)
ON_EVENT(CChatDlg, IDC_EXPLORER, 259 , OnDocumentComplete2History, VTS_DISPATCH VTS_PVARIANT)
ON_EVENT(CChatDlg, IDC_BTN_USERS, -600 , OnClickBtnUsers, VTS_NONE)
ON_EVENT(CChatDlg, IDC_BTN_FILES, -600 , OnClickBtnFiles, VTS_NONE)
ON_EVENT(CChatDlg, IDC_USER_LIST, 1 /* Menu */, OnMenuCcootreectrl, VTS_I4 VTS_BOOL)
ON_EVENT(CChatDlg, IDC_USER_LIST, 3 /* Action */, OnActionCcootreectrl, VTS_I4 VTS_BOOL)
//}}AFX_EVENTSINK_MAP
END_EVENTSINK_MAP()


/////////////////////////////////////////////////////////////////////////////
// CChatDlg message handlers
/*
LPARAM CChatDlg::OnSetRecipient(WPARAM w, LPARAM l)
{
SetRecipient(*((CUser*)w));
return 0;
}
*/
void CChatDlg::OnMenuCcootreectrl(long TID, BOOL bGroupe) 
{
	if(!bGroupe)
	{
		CUser *pUser = FindUserInVisualContactList(TID);
		if(pUser)
		{
			/*CUser User = *pUser;
		
			pMessenger->ShowUserMenu(User.GetGlobalID());*/

			CPoint point;
			GetCursorPos(&point);

			CMenu menu;
			menu.LoadMenu(IDR_MESSENGER_MENU);

			CMenu* popup = menu.GetSubMenu(0);

			pMessenger->UpdateUserMenu(pUser->GetGlobalID(),popup);

			popup->AppendMenu(MF_SEPARATOR);
			popup->AppendMenu(MF_STRING,IDS_CHANGE_COLOR,GetString(IDS_CHANGE_COLOR));

			int iSelection = popup->TrackPopupMenu(TPM_LEFTALIGN | TPM_RIGHTBUTTON | TPM_RETURNCMD,  point.x, point.y, this);

			if(IDS_CHANGE_COLOR==iSelection)
			{
				DWORD dwColor	=	0;
				GetColorFromColorStorage(pMessenger->GetUserRole(),pMessenger->GetUserID(),pUser->GetGlobalID(),dwColor);
				
				CColorDialog	colorDlg(dwColor,CC_ANYCOLOR|CC_FULLOPEN,this);
				if(colorDlg.DoModal()==IDOK)
				{
					SetItemToColorStorage(pMessenger->GetUserRole(),pMessenger->GetUserID(), pUser->GetGlobalID(), pUser->GetShowName(), colorDlg.GetColor());
					m_Chat.RefreshColors(pMessenger->GetUserRole(),pMessenger->GetUserID());
					Refresh();
				}
			}
			else
			{
				// Пересылаем выбранную команду окну броузера
				pMessenger->SendMessage(WM_COMMAND, iSelection, NULL);
			}
		}
	}
}

void CChatDlg::OnActionCcootreectrl(long TID, BOOL bGroupe)
{
	if(!bGroupe)
	{
		CUser *pUser = FindUserInVisualContactList(TID);
		if(pUser&&pUser->GetGlobalID()!=pMessenger->GetUserID())
		{
			pMessenger->SendMessageToUser(pUser);
		}
	}
}

LRESULT CChatDlg::OnSWMRefreh(WPARAM w, LPARAM l)
{
	Refresh();
	return 0;
}

void CChatDlg::OnOk() 
{
	// TODO: Add your control notification handler code here
}

void CChatDlg::OnCancel() 
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

BOOL CChatDlg::OnInitDialog() 
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
	
	HRESULT hr = m_pWebCustomizer.CreateInstance(CLSID_MpaWebCustomizer);
	
	LPUNKNOWN pDispatch = m_History.GetControlUnknown();
	m_pWebCustomizer->PutRefWebBrowser((LPDISPATCH)pDispatch);
	
	CreateTree();
	
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
	m_History.Navigate(_T("about:blank"), NULL, NULL, NULL, NULL);
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

	HDC hWinDC = ::GetDC(GetSafeHwnd());
	::EnumFontFamilies(hWinDC, (LPTSTR) NULL, (FONTENUMPROC)NEnumFontNameProc, (LPARAM)&(m_FontCombo));
	::ReleaseDC(GetSafeHwnd(),hWinDC);

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
	
	//m_InWindow.ShowWindow(SW_HIDE);
	//m_WebFolderView.ShowWindow(SW_HIDE);
	//m_InWindow.Navigate(_T("IBN_SCHEMA://default/Common/blank.html"));
	
	ShowUsers(TRUE);
	
	return (m_bDisableEditOnStart?FALSE:TRUE);  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

void CChatDlg::OnSendButton() 
{
	// TODO: Add your control notification handler code here
	UpdateData(TRUE);
	
	if(pMessenger->ConnectEnable() && !Handle)
	{
		CChat Chat;

		if(pMessenger->FindChatByGlobalId(m_Chat.GetId(),Chat)&&Chat.GetStatus()==SC_ACTIVE)
		{
			m_Chat = Chat;

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
			CComBSTR bstTmpText = (LPCWSTR)bstText;
			bstText.Empty();
			bstText = bstTmpText;
			///

			////////
			RemoveParagraf(bstText.m_str);
			////////    


						///
			CComBSTR bstTmpText2 = (LPCWSTR)bstText;
			bstText.Empty();
			bstText = bstTmpText2;
			///

			try
			{
				pMessage = m_Chat->CreateMessage();
				pMessage->PutMID(GUIDGen());
				pMessage->PutBody((BSTR)bstText);

				theNet2.LockTranslator();
				try
				{
					pMessage->Send(&Handle);
					if(Handle)
					{
						Block();
						theNet2.AddToTranslator(Handle,this->m_hWnd);
					}
				}
				catch(...)
				{ASSERT(FALSE);}
				theNet2.UnlockTranslator();
			}
			catch (_com_error&) 
			{
			}

		}
	}
}


BOOL CChatDlg::PreTranslateMessage(MSG* pMsg) 
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

void CChatDlg::Block()
{
	bBlock=TRUE;
	m_Send.SetFocus();
	m_edit.EnableWindow(FALSE);
	m_Send.EnableWindow(FALSE);
}

void CChatDlg::UnBlock()
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

//DEL void CChatDlg::SetSender(CUser &user)
//DEL {
//DEL 	m_Sender = user;
//DEL 	//m_edit.SetFocus();
//DEL 	CString strName;
//DEL 	strName.Format(GetString(IDS_FROM_FORMAT),m_Sender.GetShowName());
//DEL 	m_SenderUserInfo.SetText(strName);
//DEL }

//DEL void CChatDlg::SetRecipient(CUser &user)
//DEL {
//DEL 	m_Recipient = user;	
//DEL 	CString strCaption;
//DEL 
//DEL 	//GetWindowText(strCaption);
//DEL 	strCaption.Format(GetString(IDS_INSTANT_CHAT_TITLE_FORMAT),m_Recipient.GetShowName());
//DEL 
//DEL     SetWindowText(strCaption);
//DEL 	m_UserInfo.SetText(m_Recipient.GetShowName());
//DEL 	Refresh();
//DEL 	//m_edit.SetFocus();
//DEL }

LRESULT CChatDlg::OnNetEvent(WPARAM w,LPARAM l)
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
		//MessageTime = pItem->Long1;
		
		
		/*if(!pMessenger->AddMessageToDataBase(m_Recipient.GetGlobalID(),pMessage->GetMID(),MessageTime,pMessage->GetBody()))
		{
		_SHOW_IBN_ERROR_DLG_OK(IDS_LOCDATABASE_ADDMESSAGE_ERROR);
	}*/
		
		bInitEdit = FALSE;
		m_edit.Clear();
		//Refresh();
		
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

void CChatDlg::KillWindow()
{
	if(GetStyle()&WS_VISIBLE&&GetOptionInt(IDS_OFSMESSENGER,IDS_ANIMATION,FALSE))
		RoundExitAddon(this);
	COFSNcDlg2::OnClose();
	
	if(!bIsKillWinodow)
	{
		CloseMpaWebEvent();
		bIsKillWinodow = TRUE;
		DestroyWindow();
		delete this;
	}
}


void CChatDlg::OnEditmenuCopy() 
{
	m_edit.ClipboardCopy();	
}

void CChatDlg::OnUpdateEditmenuCopy(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(m_edit.EnableClipboardCopy());
}

void CChatDlg::OnEditmenuCut() 
{
	m_edit.ClipboardCut();
}

void CChatDlg::OnUpdateEditmenuCut(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(m_edit.EnableClipboardCut());
}

void CChatDlg::OnEditmenuDelete() 
{
	m_edit.ClipboardDelete();
}

void CChatDlg::OnUpdateEditmenuDelete(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(m_edit.EnableClipboardDelete());
}

void CChatDlg::OnEditmenuPast() 
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

void CChatDlg::OnUpdateEditmenuPast(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(m_edit.EnableClipboardPast());
}



void CChatDlg::OnCaptureChanged(CWnd *pWnd) 
{
	TRACE(_T("\r\n CChatDlg::OnCaptureChanged"));
	
	CRect rWindow;
	GetWindowRect(&rWindow);
	if(!IsIconic())
		WriteOptionString(IDS_OFSMESSENGER,IDS_CHATSPLITRECT,RectToString(rWindow));
	
	//pMessenger->RemoveAllMessageById(m_Recipient.GetGlobalID());
	m_edit.SetFocus();
	
	COFSNcDlg2::OnCaptureChanged(pWnd);
}

BOOL CChatDlg::Refresh()
{
	if(pMessenger)
	{
		// Step 1. Reload Contact List [8/16/2002]
		CChat FindChat;
		if(pMessenger->FindChatByGlobalId(m_Chat.GetId(),FindChat))
		{
			m_Chat = FindChat;
			BuildContactList();
		}
		// Step 1. Refresh Messages Arhife [8/16/2002]
		
		IDispatchPtr pDispatch;
		pDispatch.Attach(m_History.GetDocument());
		
		if(pDispatch)
		{
			try
			{
				// Step 1. Load XSLT ...	
				IStreamPtr pStream = NULL;
				LoadSkins m_LoadSkins;
				
				bstr_t xsltPath = bstr_t(IBN_SCHEMA) +(LPCTSTR)GetCurrentSkin() + bstr_t("/Shell/Conference/messages.xslt");
				
				long Error = 0L;
				m_LoadSkins.Load(xsltPath,&pStream,&Error);
				
				if(pStream)
				{
					bstr_t bstrtXSLT;
					
					ULONG pRealyRead	= 0;
					BYTE *pRead			=	NULL;
					
					STATSTG	statStg = {0};
					if(S_OK==pStream->Stat(&statStg,0))
					{
						pRead	=	new BYTE[statStg.cbSize.LowPart+2];

						ZeroMemory(pRead,statStg.cbSize.LowPart+2);

						pStream->Read((LPVOID)pRead,statStg.cbSize.LowPart,&pRealyRead);
						
						if((pRead[0]==0xFF&&pRead[1]==0xFE)||
							(pRead[0]==0xFE&&pRead[1]==0xFF))
						{
							// Remove Lead FE FF, FF FE
							//if((pRead[0]==0xFF&&pRead[1]==0xFE)||
							//	(pRead[0]==0xFE&&pRead[1]==0xFF))
								bstrtXSLT	=	(LPWSTR)(LPBYTE)(pRead+2);
							//else
							//	bstrtXSLT	=	(LPWSTR)(LPBYTE)pRead;
						}
						else
						{
							if(pRead[0]==0xEF&&pRead[1]==0xBB&&pRead[2]==0xBF)
							{
								int WideSize	=	MultiByteToWideChar(CP_UTF8,0,(LPCSTR)(pRead+3),-1,0,0);
								
								LPWSTR	wsBuff	=	new WCHAR[WideSize];
								MultiByteToWideChar(CP_UTF8,0,(LPCSTR)(pRead+3),-1,wsBuff,WideSize);
								
								bstrtXSLT	=	wsBuff;
								
								delete [] wsBuff;
							}
							else
							{
								int WideSize	=	MultiByteToWideChar(0,0,(LPCSTR)(pRead),-1,0,0);
								
								LPWSTR	wsBuff	=	new WCHAR[WideSize];
								MultiByteToWideChar(0,0,(LPCSTR)(pRead),-1,wsBuff,WideSize);
								
								bstrtXSLT	=	wsBuff;
								
								delete []wsBuff;
							}
						}
				
				
						delete [] pRead;
				
						// Step 2. Convert XML + XSLT ->HTML and Load into Document [8/16/2002]
						// bstrtXSLT consist XML
						
						CComPtr<IXSLTemplate>		pIXSLTemplate	=	NULL;
						CComPtr<IXMLDOMDocument2>	pXSLT			=	NULL; 
						CComPtr<IXSLProcessor>		pIXSLProcessor	=	NULL;
						
						HRESULT hr = pIXSLTemplate.CoCreateInstance(CLSID_XSLTemplate40,NULL,CLSCTX_SERVER);
						if(hr==S_OK)
						{
							hr = pXSLT.CoCreateInstance(CLSID_FreeThreadedDOMDocument40,NULL,CLSCTX_SERVER);
							
							pXSLT->put_async(VARIANT_FALSE);
							
							VARIANT_BOOL	vbLoad	=	VARIANT_FALSE;
							TRACE(_T("\r\n $$$ Try pStream Load $$$"));
							
							//pXSLT->load(CComVariant((IUnknown*)pStream),&vbLoad);
							pXSLT->loadXML(bstrtXSLT,&vbLoad);
#ifdef _DEBUG

							pXSLT->save(CComVariant(L"c:\\tmp_mess.xml"));
#endif							
							if(vbLoad==VARIANT_TRUE)
							{
								TRACE(_T("\r\n $$$ pStream Load Completed $$$"));
								hr = pIXSLTemplate->putref_stylesheet(pXSLT);
								hr = pIXSLTemplate->createProcessor(&pIXSLProcessor);
								
								if(pIXSLProcessor)
								{
									hr = pIXSLProcessor->put_input(CComVariant((IDispatch*)m_Chat.GetMessagesInterface()));
									hr = pIXSLProcessor->put_output(CComVariant((IDispatch*)pDispatch));

									VARIANT_BOOL	vbResult	=	VARIANT_FALSE;
									hr = pIXSLProcessor->transform(&vbResult);
								}
								
							}
							
						}
						// End [8/16/2002]
					}
				}
			}
			catch(...)
			{
				ASSERT(FALSE);
			}
			
		}
		else
		{
			m_bEnableRefresh	=	TRUE;
		}
	}
	return FALSE;
}




HRESULT CChatDlg::OnAutoRefresh(WPARAM w, LPARAM l)
{
	//Refresh();
	return 0;
}

void CChatDlg::OnMove(int x, int y) 
{
	COFSNcDlg2::OnMove(x, y);
	
	if(::IsWindow(GetParent()->GetSafeHwnd()))
		GetParent()->UpdateWindow();
	if(::IsWindow(pMessenger->GetMessageParent()->GetSafeHwnd()))
		pMessenger->GetMessageParent()->UpdateWindow();
}

//DEL void CChatDlg::SetFon(HBITMAP hFon)
//DEL {
//DEL 	// 03.04.2002 \-
//DEL 	return;
//DEL 	// 03.04.2002 /-
//DEL 	
//DEL 	/*if(pFonBmp)
//DEL 		pFonBmp->DeleteObject();
//DEL     else
//DEL 		pFonBmp = new CBitmap;
//DEL 	
//DEL 	pFonBmp->Attach(hFon);
//DEL 	
//DEL 	BITMAP hb;
//DEL 	pFonBmp->GetBitmap(&hb);
//DEL 	sFon = CSize(hb.bmWidth ,hb.bmHeight);
//DEL 	
//DEL 	SetMinTrackSize(sFon);
//DEL 	SetWindowPos(NULL,-1,-1,sFon.cx,sFon.cy ,SWP_NOZORDER|SWP_NOMOVE|SWP_NOACTIVATE);
//DEL 
//DEL 	CPictureHolder	tmpImage;
//DEL 	tmpImage.CreateFromBitmap(pFonBmp);
//DEL 	m_ResizeFon.Destroy();
//DEL 	m_ResizeFon.Create(tmpImage.m_pPict);
//DEL 	m_ResizeFon.AddAnchor(CRect(0,0,230,100),CSize(0,0),CSize(0,0));
//DEL 	m_ResizeFon.AddAnchor(CRect(231,0,290,100),CSize(0,0),CSize(100,0),CResizableImage::DUPLICATE);
//DEL 	m_ResizeFon.AddAnchor(CRect(291,0,413,100),CSize(100,0),CSize(100,0));
//DEL 	m_ResizeFon.AddAnchor(CRect(0,101,230,170),CSize(0,0),CSize(0,100),CResizableImage::DUPLICATE);
//DEL 	m_ResizeFon.AddAnchor(CRect(291,101,413,170),CSize(100,0),CSize(100,100),CResizableImage::DUPLICATE);
//DEL 	m_ResizeFon.AddAnchor(CRect(0,171,230,350),CSize(0,100),CSize(0,100));
//DEL 	m_ResizeFon.AddAnchor(CRect(231,171,290,350),CSize(0,100),CSize(100,100),CResizableImage::DUPLICATE);
//DEL 	m_ResizeFon.AddAnchor(CRect(291,171,413,350),CSize(100,100),CSize(100,100));
//DEL 
//DEL 	//AddAnchor(m_btnX.GetSafeHwnd(),CSize(100,0),CSize(100,0));
//DEL 	//AddAnchor(m_btnMin.GetSafeHwnd(),CSize(100,0),CSize(100,0));
//DEL 	AddAnchor(m_edit.GetSafeHwnd(),CSize(0,100),CSize(100,100));
//DEL 	AddAnchor(m_History.GetSafeHwnd(),CSize(0,0),CSize(100,100));
//DEL //	AddAnchor(m_ColorButton.GetSafeHwnd(),CSize(0,100),CSize(0,100));
//DEL //	AddAnchor(m_BoldButton.GetSafeHwnd(),CSize(0,100),CSize(0,100));
//DEL //	AddAnchor(m_ItalicButton.GetSafeHwnd(),CSize(0,100),CSize(0,100));
//DEL //	AddAnchor(m_UnderLineButton.GetSafeHwnd(),CSize(0,100),CSize(0,100));
//DEL //	AddAnchor(m_InsertSmileButton.GetSafeHwnd(),CSize(0,100),CSize(0,100));
//DEL 	AddAnchor(m_SizeCombo.GetSafeHwnd(),CSize(0,100),CSize(0,100));
//DEL 	AddAnchor(m_FontCombo.GetSafeHwnd(),CSize(0,100),CSize(0,100));
//DEL 	
//DEL 	//AddAnchor(m_Send.GetSafeHwnd(),CSize(100,100),CSize(100,100));
//DEL 
//DEL 	CRect rgnRect;
//DEL 	GetWindowRect(&rgnRect);
//DEL 	CRgn	WinRgn;
//DEL 	WinRgn.CreateRoundRectRgn(0,0,rgnRect.Width(),rgnRect.Height(),20,20);
//DEL 	SetWindowRgn(WinRgn,TRUE);
//DEL 	
//DEL 	CString strRect = GetOptionString(IDS_OFSMESSENGER,IDS_SPLITRECT,"");
//DEL 	if(!strRect.IsEmpty())
//DEL 	{
//DEL 		CRect rWindow = StringToRect(strRect);
//DEL 		FitRectToWindow(rWindow);
//DEL 		SetWindowPos(NULL,rWindow.left,rWindow.top,rWindow.Width(),rWindow.Height(),SWP_NOZORDER|SWP_NOACTIVATE);
//DEL 	}
//DEL 	*/
//DEL }

void CChatDlg::OnPaint() 
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

//DEL void CChatDlg::OnLButtonDown(UINT nFlags, CPoint point) 
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

//DEL BOOL CChatDlg::OnEraseBkgnd( CDC* pDC )
//DEL {
//DEL 	return TRUE;
//DEL }

void CChatDlg::OnClickMcclose() 
{
	OnCancel();
}

void CChatDlg::OnClickMcsend() 
{
	OnSendButton();
}

void CChatDlg::OnClickMcmenu() 
{
	LONG ChatTID = -1;
	CChat	SelChat;
	if(pMessenger->FindChatByGlobalId(m_Chat.GetId(),SelChat))
	{
		ChatTID = SelChat.GetTID();
	}
	pMessenger->ShowGeneralMenu(ChatTID);
}

void CChatDlg::OnClickMcmini() 
{
	ShowWindow(SW_MINIMIZE);
}

void CChatDlg::OnClickMcoptions() 
{
	pMessenger->PreferenceDlg(this);
}

BOOL CChatDlg::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message) 
{
	CRect StatusRect, miniRect;
	
	GetClientRect(&StatusRect);
	
	CPoint point, inPoint;
	
	::GetCursorPos(&point);
	inPoint = point;
	ScreenToClient(&inPoint);
	
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

void CChatDlg::OnDropFiles( HDROP hDropInfo )
{
	SetForegroundWindow();
	
	/*UINT FileCount = DragQueryFile(hDropInfo,0xFFFFFFFF,NULL,0);
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
		
		for(i=0;i<FileCount;i++)
		{
			TCHAR FileBuffer[MAX_PATH];
			DragQueryFile(hDropInfo,i,FileBuffer,MAX_PATH);
			//pMessenger->SendFileToUser(&m_Recipient,FileBuffer,strDescription);
		}
	}*/
}

void CChatDlg::OnColorButton() 
{
	CColorDialog m_Color(m_edit.GetColor(), CC_ANYCOLOR,this);
	if(m_Color.DoModal()==IDOK)
	{
		m_edit.SetColor(m_Color.GetColor());
	}
	m_edit.SetFocus();
}

HRESULT CChatDlg::OnEditUpdate(WPARAM w, LPARAM l)
{
	if(!m_strSetBody.IsEmpty())
	{
		CComBSTR	bstSetBody = m_strSetBody;
		m_edit.InsertTEXT(bstSetBody);
		m_strSetBody.Empty();
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

void CChatDlg::OnSelendokSizeCombo() 
{
	WriteOptionInt(IDS_OFSMESSENGER,IDS_SIZE,m_SizeCombo.GetCurSel());
	m_edit.SetTextSize(m_SizeCombo.GetCurSel()+1);
	//  [4/29/2002]
	m_edit.SetDefaultFontSize(nFontSizes[m_SizeCombo.GetCurSel()]);
	//  [4/29/2002]
	m_edit.SetFocus();
}

void CChatDlg::OnSelendokFontCombo() 
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

void CChatDlg::OnInsertsmileCheck() 
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

void CChatDlg::OnSmileItem(UINT nID)
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


void CChatDlg::SetBody(LPCTSTR strBody)
{
	m_strSetBody = strBody;
	CComBSTR	bsText	=	L"";
	m_edit.InsertTEXT(bsText);
	m_edit.SetFocus();
}

LPARAM CChatDlg::OnSWMSetBody(WPARAM w, LPARAM l)
{
	if(!Handle)
	{
		SetBody(LPCTSTR(w));

		if(l!=NULL)
			OnSendButton();
	}
	return 0;
}

void CChatDlg::OnActivate( UINT nState, CWnd* pWndOther, BOOL bMinimized )
{
	TRACE(_T("\r\n -- CChatDlg::OnActivate nState== %d, bMinimized = %d"), nState,bMinimized);
	
	//  [7/23/2002]
	if(m_bEnableIfActiavte)
	{
		m_edit.EnableWindow(TRUE);
		m_bEnableIfActiavte = FALSE;
	}
	
	//if(m_bDisableEditOnStart)
	//{
	//TRACE("\r\n -- CChatDlg::OnActivate m_bEnableIfActiavte	=	TRUE");
	//m_bEnableIfActiavte = m_bDisableEditOnStart;
	//m_bDisableEditOnStart	=	FALSE;
	//}
	
	if((nState==WA_ACTIVE||nState==WA_CLICKACTIVE)&&!bMinimized)
	{
		//pMessenger->RemoveAllMessageById(m_Recipient.GetGlobalID());
		m_edit.SetFocus();
	}
	
	COFSNcDlg2::OnActivate(nState, pWndOther, bMinimized);
}

BOOL CChatDlg::OnNcActivate(BOOL bActive)
{
	TRACE(_T("\r\n -- CChatDlg::OnNcActivate bActive== %d"), bActive);
	
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

void CChatDlg::LoadSkin(IXMLDOMNode *pXmlRoot)
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
	m_btnUsers.ShowWindow(SW_HIDE);
	m_btnFiles.ShowWindow(SW_HIDE);
	m_treebox.ShowWindow(SW_HIDE);
	//m_InWindow.ShowWindow(SW_HIDE);
	//m_WebFolderView.ShowWindow(SW_HIDE);
	
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
	
	LoadButton(pXmlRoot, _T("Users"), &m_btnUsers, FALSE, TRUE);
	LoadButton(pXmlRoot, _T("Files"), &m_btnFiles, FALSE, TRUE);
	//LoadRectangle(pXmlRoot, _T("Users"), &m_InWindow, FALSE);
	//LoadRectangle(pXmlRoot, _T("Users"), &m_WebFolderView, FALSE);
	LoadRectangle(pXmlRoot, _T("Users"), &m_treebox, TRUE);

	///m_btnFiles.EnableWindow(FALSE);
}

int CChatDlg::OnCreate(LPCREATESTRUCT lpCreateStruct) 
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

BOOL CChatDlg::Create(CWnd *pParentWnd, BOOL bDisableEditOnStart)
{
	TRACE(_T("\r\n CChatDlg::Create"));
	m_bDisableEditOnStart	=	bDisableEditOnStart;
	
	if(!COFSNcDlg2::Create(IDD, pParentWnd))
	{
		TRACE0("Warning: failed to create CChatDlg.\n");
		return FALSE;
	}
	
	TRACE(_T("\r\n CChatDlg::Create/COFSNcDlg2::LoadSkin"));
	if(!m_bLoadSkin)
		COFSNcDlg2::LoadSkin();
	TRACE(_T("\r\n CChatDlg::Create/COFSNcDlg2::LoadSkin end"));
	
	//  [7/23/2002]	
	CString strRect = GetOptionString(IDS_OFSMESSENGER, IDS_CHATSPLITRECT, _T(""));
	if(!strRect.IsEmpty())
	{
		CRect rWindow = StringToRect(strRect);
		FitRectToWindow(rWindow);
		SetWindowPos(NULL,rWindow.left,rWindow.top,rWindow.Width(),rWindow.Height(),SWP_NOZORDER|SWP_NOACTIVATE);
	}
	TRACE(_T("\r\n CChatDlg::Create End"));
	return TRUE;
}

void CChatDlg::OnClickBtnColor() 
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

void CChatDlg::OnClickBtnBold() 
{
	m_edit.SetBold();
	m_edit.SetFocus();
	WriteOptionInt(IDS_OFSMESSENGER,IDS_BOLD, m_btnBold.GetPressed());
}

void CChatDlg::OnClickBtnItalic() 
{
	m_edit.SetItalic();
	m_edit.SetFocus();
	WriteOptionInt(IDS_OFSMESSENGER,IDS_ITALIC, m_btnItalic.GetPressed());
}

void CChatDlg::OnClickBtnUnderline() 
{
	m_edit.SetUnderline();
	m_edit.SetFocus();
	WriteOptionInt(IDS_OFSMESSENGER,IDS_UNDERLINE, m_btnUnderline.GetPressed());
}

void CChatDlg::OnClickBtnSmiles() 
{
	OnInsertsmileCheck();
}

void CChatDlg::OnBeforeNavigate2History(LPDISPATCH pDisp, VARIANT FAR* URL, VARIANT FAR* Flags, VARIANT FAR* TargetFrameName, VARIANT FAR* PostData, VARIANT FAR* Headers, BOOL FAR* Cancel) 
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

			strUrl = pMessenger->ChangeUrlForCurrentDomain(strUrl);
			
			m_bEnableNavigateHistory = TRUE;

			if(S_OK != ::NavigateNewWindow(m_History.GetControlUnknown(), strUrl))
				ShellExecute(::GetDesktopWindow(), _T("open"), strUrl, NULL, NULL, SW_SHOWDEFAULT);

			m_bEnableNavigateHistory = FALSE;
		}
	}
}

void CChatDlg::InitMpaWebEvent()
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

void CChatDlg::CloseMpaWebEvent()
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


void CChatDlg::OnShowContextMenu(long dwID, long x, long y, IUnknown *pcmdTarget, IDispatch *pdispReserved, long* pShow)
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

void CChatDlg::OnDocumentComplete2History(LPDISPATCH pDisp, VARIANT FAR* URL)
{
	if(m_bEnableRefresh)
	{
		Refresh();
		m_bEnableRefresh	=	FALSE;
	}
}

void CChatDlg::OnClickBtnUsers()
{
	ShowUsers(TRUE);
}

void CChatDlg::OnClickBtnFiles()
{
	ShowUsers(FALSE);
}

void CChatDlg::ShowUsers(BOOL bShow)
{
	USES_CONVERSION;

	m_btnUsers.SetPressed(bShow);
	m_btnFiles.SetPressed(!bShow);
	m_treebox.ShowWindow(bShow?SW_SHOW:SW_HIDE);
	//m_InWindow.ShowWindow(!bShow?SW_SHOW:SW_HIDE);
	//m_WebFolderView.ShowWindow(!bShow?SW_SHOW:SW_HIDE);

	if(bShow)
	{
		//m_InWindow.Navigate(_T("IBN_SCHEMA://default/Common/blank.html"));
		//m_WebFolderView.Navigate(L"");
	}
	else
	{
		//CString strConferencesUrl = _T("#host#/Intranet/#domain#");

		//strConferencesUrl.Replace("#sid#",pMessenger->GetSID());
		//strConferencesUrl.Replace("#host#",pMessenger->GetServerPath());
		//strConferencesUrl.Replace("#domain#",pMessenger->GetUserDomain());

		//CString strDescription;
		//strDescription.Format(_T("IBN Conferences on %s"),pMessenger->GetUserDomain());

		//m_InWindow.Navigate(strNewsUrl);
		//McAddNetworkPlace(GetSafeHwnd(),strConferencesUrl,strDescription);x

		//strConferencesUrl += _T("/Conferences/");
		//strConferencesUrl += (LPCWSTR)m_Chat.GetId();

		//strConferencesUrl = _T("http://212.44.66.3/Intranet/");

		//LPITEMIDLIST	pPIDL	=	NULL;

		//if(S_OK == McCoCreateWFPIDL(T2CW(strDescription),T2CW(strConferencesUrl),&pPIDL))
		//{
			//m_WebFolderView.Destroy();
			//m_WebFolderView.Navigate2(pPIDL);

		//	CComPtr<IMalloc>	pMalloc	=	NULL;
		//	SHGetMalloc(&pMalloc);
		//	pMalloc->Free(pPIDL);
		//}
	}
	
}

void CChatDlg::SetChat(const CChat &Chat)
{
	m_Chat = Chat;

	CString strText;

	strText.Format(GetString(IDS_CONFERENCE_CAPTION_TEXT),m_Chat.GetShowName());

	SetWindowText(strText);
	
	m_UserInfo.SetWindowText(m_Chat.GetShowName());
}

void CChatDlg::CreateTree()
{
	LoadSkins m_LoadSkin;
	
	IStreamPtr pStream = NULL;
	long Error=0;
	m_LoadSkin.Load(bstr_t(IBN_SCHEMA) + bstr_t((LPCTSTR)GetProductLanguage()) + bstr_t("/Shell/Main/status.bmp"),&pStream,&Error);
	if(pStream)
	{	
		CDib dib(pStream);
		CPaintDC dc(this);
		HBITMAP lhBmp = dib.GetHBITMAP((HDC)dc);
		m_treebox.SetImageList((long)lhBmp);
		if(lhBmp)
			DeleteObject(lhBmp);
	}
	
	short PriorityIndex[10];
	for(int i=0;i<10;i++)
		PriorityIndex[i] = -1;
	PriorityIndex[0] = 1;
	PriorityIndex[1] = 0;
	
	m_treebox.SetPriority(PriorityIndex);
	
	for(int i = 0 ;i<MaxValueID;i++)
	{
		m_treebox.AddEffect(m_ShablonId[i],m_ShablonIcon[i],m_ShablonRGBTextEnable[i],
			m_ShablonRGBTextSelect[i],m_ShablonRGBFonEnable[i],m_ShablonRGBFonSelect[i]);
	}
	
	m_treebox.SetEventMode(1);
}

void CChatDlg::BuildContactList()
{
//	int m_numIco;
	
	//////////////////////////////////////////////////////////////////////////
	CMapStringToPtr		GroupIsOpen;
	// Сохраним позиции открыта группа или нет [2/25/2002]
	// Step 2. Считать текущее [3/14/2002]
	
	m_treebox.DeleteTree();
	CurrTID = -1;
	
	if(POSITION pos = m_Chat.GetUsers().InitIteration())
	{
		CUser *pUser = NULL;
		while(m_Chat.GetUsers().GetNext(pos,pUser))
		{
			// Step 2. Если нет, то создать группу .
			if(m_Chat.GetStatus()==SC_ACTIVE)
				pUser->TID = m_treebox.AddItem(0,pUser->GetShowName(),m_ShablonId[pUser->GetStatus()==SC_ACTIVE?1:9]);
			else
				pUser->TID = m_treebox.AddItem(0,pUser->GetShowName(),m_ShablonId[9]);
		}
	}
	
	//////////////////////////////////////////////
	//TRACE("\r\n  CMainDlg::BuildContactList End ...");
}

CUser* CChatDlg::FindUserInVisualContactList(long TID)
{
	if(POSITION pos = m_Chat.GetUsers().InitIteration())
	{
		CUser *pUser=NULL;
		
		while(m_Chat.GetUsers().GetNext(pos,pUser))
		{
			if(pUser->TID == TID )
				return pUser;
		}
	}
	
	return NULL;
}
