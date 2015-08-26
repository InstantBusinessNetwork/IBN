// MessageSplitDlg.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "MainDlg.h"
#include "GroupMessageSendDlg.h"
#include "resource.h"
#include "User.h"
#include <triedcid.h>
//#include "MainFrm.h"
#include "MemDc.h"
//#include "smileyahoopopup.h"

#include "SmileManager.h"
#include "SelectSmileDlg.h"

extern  CSmileManager CurrentSmileManager;


#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

#define WM_EDIT_UPDATE WM_USER + 201
/////////////////////////////////////////////////////////////////////////////
// CGroupMessageSendDlg dialog


CGroupMessageSendDlg::CGroupMessageSendDlg(CMainDlg *pMessenger,CWnd* pParent /*=NULL*/)
	: COFSNcDlg2(CGroupMessageSendDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CGroupMessageSendDlg)
	//}}AFX_DATA_INIT
	this->pMessenger = pMessenger;
	bBlock  = FALSE;	
	Handle  = 0L;
	MessageTime = 0L;
	bInitEdit = FALSE;
	bIsKillWinodow	=	FALSE;
	m_bWasCtrlEnter	=	FALSE;
	m_bWasCtrlExit	=	FALSE;
	m_strSkinSettings = _T("/Shell/GroupMessage/skin.xml");
	m_bLoadSkin = FALSE;

	m_bAutoSend	=	FALSE;
}

CGroupMessageSendDlg::~CGroupMessageSendDlg()
{
}


void CGroupMessageSendDlg::DoDataExchange(CDataExchange* pDX)
{
	COFSNcDlg2::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CGroupMessageSendDlg)
	DDX_Control(pDX, IDC_FONT_COMBO, m_FontCombo);
	DDX_Control(pDX, IDC_SIZE_COMBO, m_SizeCombo);
	DDX_Control(pDX, IDC_USERINFO, m_UserInfo);
	DDX_Control(pDX, IDC_CCOOTREECTRL, m_treectrl);
	DDX_Control(pDX, IDC_BTN_BOLD, m_btnBold);
	DDX_Control(pDX, IDC_BTN_COLOR, m_btnColor);
	DDX_Control(pDX, IDC_BTN_ITALIC, m_btnItalic);
	DDX_Control(pDX, IDC_BTN_MENU, m_btnMenu);
	DDX_Control(pDX, IDC_BTN_MIN, m_btnMin);
	DDX_Control(pDX, IDC_BTN_OPTIONS, m_btnOptions);
	DDX_Control(pDX, IDC_BTN_SELECT_ALL, m_btnSelectAll);
	DDX_Control(pDX, IDC_BTN_SELECT_NONE, m_btnSelectNone);
	DDX_Control(pDX, IDC_BTN_SEND, m_btnSend);
	DDX_Control(pDX, IDC_BTN_SMILES, m_btnSmiles);
	DDX_Control(pDX, IDC_BTN_UNDERLINE, m_btnUnderline);
	DDX_Control(pDX, IDC_BTN_X, m_btnX);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CGroupMessageSendDlg, COFSNcDlg2)
	//{{AFX_MSG_MAP(CGroupMessageSendDlg)
	ON_BN_CLICKED(IDOK, OnOk)
	ON_MESSAGE(WM_NLT_CONTAINER_EVENT,OnNetEvent)	
	ON_CBN_SELENDOK(IDC_FONT_COMBO, OnSelEndOkFontCombo)
	ON_CBN_SELENDOK(IDC_SIZE_COMBO, OnSelEndOkSizeCombo)
	ON_COMMAND(ID_EDITMENU_COPY, OnEditMenuCopy)
	ON_UPDATE_COMMAND_UI(ID_EDITMENU_COPY, OnUpdateEditMenuCopy)
	ON_COMMAND(ID_EDITMENU_CUT, OnEditMenuCut)
	ON_UPDATE_COMMAND_UI(ID_EDITMENU_CUT, OnUpdateEditMenuCut)
	ON_COMMAND(ID_EDITMENU_DELETE, OnEditMenuDelete)
	ON_UPDATE_COMMAND_UI(ID_EDITMENU_DELETE, OnUpdateEditMenuDelete)
	ON_COMMAND(ID_EDITMENU_PAST, OnEditMenuPaste)
	ON_UPDATE_COMMAND_UI(ID_EDITMENU_PAST, OnUpdateEditMenuPaste)
	ON_WM_CAPTURECHANGED()
	ON_WM_MOVE()
	ON_WM_SIZE()
	ON_WM_DROPFILES()
	ON_MESSAGE(WM_EDIT_UPDATE,OnEditUpdate)
	//}}AFX_MSG_MAP
	ON_MESSAGE(WM_SWM_SETBODY,OnSWMSetBody)
	ON_COMMAND_RANGE(20000,20000+SmileBuffSize,OnSmileItem)
END_MESSAGE_MAP()


/////////////////////////////////////////////////////////////////////////////
// CGroupMessageSendDlg message handlers

void CGroupMessageSendDlg::OnOk() 
{
	// TODO: Add your control notification handler code here
}

void CGroupMessageSendDlg::OnCancel()
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

BOOL CGroupMessageSendDlg::OnInitDialog() 
{
	COFSNcDlg2::OnInitDialog();

	pSession = theNet2.GetSession();
	ASSERT(pSession!=NULL);
	
	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),FALSE);
	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),TRUE);

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

	/// Create Font ...
	m_font.Attach(GetStockObject(DEFAULT_GUI_FONT));
	
	CRect rTmp(16,102,351,184);
	m_edit.Create(NULL,NULL,WS_VISIBLE,rTmp,this,IDC_DHTML_EDIT);
	m_edit.InitInfoMessage(WM_EDIT_UPDATE);
	m_edit.SetContextMenu(IDR_MESSENGER_MENU,1,this);
	m_edit.SetEditMode();

    CString str;
	for (int i = 0; i < sizeof(nFontSizes)/sizeof(int); i++)
	{
		str.Format(_T("%d"), nFontSizes[i]);
		m_SizeCombo.AddString(str);
	}
	
	CreateTree();
	
	::EnumFontFamilies(GetDC()->m_hDC, (LPTSTR) NULL, (FONTENUMPROC)NEnumFontNameProc, (LPARAM)&(m_FontCombo));
	
	bInitEdit = FALSE;
	//  [4/29/2002]
	//CString strFontName = _T("Arial");
	CString strFontName = GetOptionString(IDS_OFSMESSENGER,IDS_FONT,_T("Arial"));
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
	m_edit.SetFocus();
	
	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

BOOL CGroupMessageSendDlg::PreTranslateMessage(MSG* pMsg) 
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
				Send();
				return TRUE;
			}
			else
			{
				m_bWasCtrlEnter = TRUE;
				//pMsg->message	=	WM_KEYDOWN;
				//pMsg->wParam	=	VK_RETURN;
				//m_edit.SendMessage(WM_KEYDOWN,VK_RETURN);
			}
		}
		else if(pMsg->wParam==VK_RETURN&&(GetKeyState(VK_SHIFT)>>1)&&!bBlock)
		{
			if(GetOptionInt(IDS_OFSMESSENGER,IDS_SHIFTENTER,0)==0)
			{
				Send();
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
				Send();
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
				Send();
				return TRUE;
			}
			else
			{
				pMsg->message	=	WM_KEYDOWN;
				pMsg->wParam	=	VK_RETURN;
			}
		}
	}
	return COFSNcDlg2::PreTranslateMessage(pMsg);
}

void CGroupMessageSendDlg::Block()
{
	bBlock=TRUE;
	m_edit.EnableWindow(FALSE);
	m_btnSend.EnableWindow(FALSE);
	m_btnSelectAll.EnableWindow(FALSE);
	m_btnSelectNone.EnableWindow(FALSE);
	m_treectrl.EnableWindow(FALSE);
}

void CGroupMessageSendDlg::UnBlock()
{
	bBlock=FALSE;	
	m_edit.EnableWindow(TRUE);
	m_edit.SetFocus();
	m_btnSend.EnableWindow(TRUE);
	m_btnSelectAll.EnableWindow(TRUE);
	m_btnSelectNone.EnableWindow(TRUE);
	m_treectrl.EnableWindow(TRUE);
}

//DEL void CGroupMessageSendDlg::SetSender(CUser &user)
//DEL {
//DEL 	m_Sender = user;
//DEL 	m_edit.SetFocus();
//DEL 	CString strName = _T("From: ")+m_Sender.GetShowName();
//DEL }

void CGroupMessageSendDlg::SetRecipientGroup(LPCTSTR strName)
{
	m_strRecepientGroupName = strName;

//	CString strCaption;
//	GetWindowText(strCaption);
//	strCaption += " with ";
//	strCaption += strName;
//	SetWindowText(strCaption);
	
//	strCaption = strName;
	m_UserInfo.SetText(strName);
	m_edit.SetFocus();
	
	m_ContactList.Clear();
	
	pMessenger->GetCopyContactList(m_ContactList);
	
	// Step 2. Init Check or Uncheck Mode [2/6/2002]
	if(POSITION pos = m_ContactList.InitIteration())
	{
		CUser* pUser=NULL;
		
		//if(m_ContactList.InitIteration())
		//{
			for(int i=0; m_ContactList.GetNext(pos, pUser); i++)
			{
				int CLMode  = GetOptionInt(IDS_OFSMESSENGER,IDS_CLMODE,2);
				
				switch(CLMode) 
				{
				case 1:
					{
						if(GetOptionInt(IDS_OFSMESSENGER,IDS_SHOWOFFLINEFILES,TRUE)||(pUser->m_iStatus!=S_OFFLINE&&pUser->m_iStatus!=S_INVISIBLE))
							pUser->m_bHasNewMessages  = (m_strRecepientGroupName.CompareNoCase(pUser->m_strType)==0); 
					}
					break;
				case 2:
					{
						if(pUser->m_iStatus==S_OFFLINE||pUser->m_iStatus==S_INVISIBLE)
						{
							pUser->m_bHasNewMessages  = (m_strRecepientGroupName.CompareNoCase(GetString(IDS_OFFLINE))==0);
						}
						else
							pUser->m_bHasNewMessages  = (m_strRecepientGroupName.CompareNoCase(pUser->m_strType)==0); 
					}
					break;
				}
			}
		//}
	}

	BuildTree();
}

LRESULT CGroupMessageSendDlg::OnNetEvent(WPARAM w,LPARAM l)
{
	NLT_Container *pItem = (NLT_Container *)w;
	ASSERT(pItem!=NULL);
	
	theNet2.LockTranslator();
	theNet2.RemoveFromTranslator(Handle);
	theNet2.UnlockTranslator();
	
	switch(pItem->EventType)
	{
    case NLT_ECommandOK:
		MessageTime = pItem->Long1;
		
		try
		{
			CUser *pRecipient			=	NULL;
			BOOL	bEnableContactUser	=	FALSE;
			
			if(POSITION pos = m_ContactList.InitIteration())
			{
				while(m_ContactList.GetNext(pos, pRecipient))
				{
					if(pRecipient->m_bHasNewMessages)
					{
						if(!pMessenger->AddMessageToDataBase(pRecipient->GetGlobalID(),pMessage->GetMID(),MessageTime,pMessage->GetBody()))
						{
							_SHOW_IBN_ERROR_DLG_OK(IDS_LOCDATABASE_ADDMESSAGE_ERROR);
						}
					}
				}
			}
		}
		catch(...)
		{
		}
		
		bInitEdit = FALSE;
		KillWindow();
		break;
	case NLT_ECommandError:
		UnBlock();
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
	}

	delete pItem;
    return 0;
}

void CGroupMessageSendDlg::KillWindow()
{
	if(GetStyle()&WS_VISIBLE&&GetOptionInt(IDS_OFSMESSENGER,IDS_ANIMATION,FALSE))
		RoundExitAddon(this);
	COFSNcDlg2::OnClose();
	if(!bIsKillWinodow)
	{
		m_ContactList.Clear();
		m_treectrl.DeleteTree();
		
		bIsKillWinodow = TRUE;
		DestroyWindow();
		delete this;
	}
}


void CGroupMessageSendDlg::OnEditMenuCopy() 
{
	m_edit.ClipboardCopy();	
}

void CGroupMessageSendDlg::OnUpdateEditMenuCopy(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(m_edit.EnableClipboardCopy());
}

void CGroupMessageSendDlg::OnEditMenuCut() 
{
	m_edit.ClipboardCut();
}

void CGroupMessageSendDlg::OnUpdateEditMenuCut(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(m_edit.EnableClipboardCut());
}

void CGroupMessageSendDlg::OnEditMenuDelete() 
{
	m_edit.ClipboardDelete();
}

void CGroupMessageSendDlg::OnUpdateEditMenuDelete(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(m_edit.EnableClipboardDelete());
}

void CGroupMessageSendDlg::OnEditMenuPaste() 
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
}

void CGroupMessageSendDlg::OnUpdateEditMenuPaste(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(m_edit.EnableClipboardPast());
}



void CGroupMessageSendDlg::OnCaptureChanged(CWnd *pWnd) 
{
	CRect rWindow;
	GetWindowRect(&rWindow);
	if(!IsIconic())
		WriteOptionString(IDS_OFSMESSENGER,IDS_GROUPSENDMESSAGE,RectToString(rWindow));

	COFSNcDlg2::OnCaptureChanged(pWnd);
}


void CGroupMessageSendDlg::OnMove(int x, int y) 
{
	COFSNcDlg2::OnMove(x, y);
	
	if(::IsWindow(GetParent()->GetSafeHwnd()))
		GetParent()->UpdateWindow();
	if(::IsWindow(pMessenger->GetMessageParent()->GetSafeHwnd()))
		pMessenger->GetMessageParent()->UpdateWindow();
}

void CGroupMessageSendDlg::SetFon(HBITMAP hFon)
{
/*
	if(pFonBmp)
		pFonBmp->DeleteObject();
    else
		pFonBmp = new CBitmap;
	
	pFonBmp->Attach(hFon);
	
	BITMAP hb;
	pFonBmp->GetBitmap(&hb);
	sFon = CSize(hb.bmWidth ,hb.bmHeight);
	
	//SetMinTrackSize(sFon);
	//SetWindowPos(NULL,-1,-1,sFon.cx,sFon.cy ,SWP_NOZORDER|SWP_NOMOVE);

	CPictureHolder	tmpImage;
	tmpImage.CreateFromBitmap(pFonBmp);
	m_ResizeFon.Destroy();
	m_ResizeFon.Create(tmpImage.m_pPict);

	m_ResizeFon.AddAnchor(CRect(0,0,223,67),CSize(0,0),CSize(0,0));
	m_ResizeFon.AddAnchor(CRect(223,0,277,67),CSize(0,0),CSize(100,0),CResizableImage::DUPLICATE);
	m_ResizeFon.AddAnchor(CRect(277,0,299,67),CSize(100,0),CSize(100,0));

	m_ResizeFon.AddAnchor(CRect(0,67,223,115),CSize(0,0),CSize(0,100),CResizableImage::DUPLICATE);
	m_ResizeFon.AddAnchor(CRect(223,67,277,115),CSize(0,0),CSize(100,100),CResizableImage::DUPLICATE);
	m_ResizeFon.AddAnchor(CRect(277,67,299,115),CSize(100,0),CSize(100,100),CResizableImage::DUPLICATE);

	m_ResizeFon.AddAnchor(CRect(0,115,223,149),CSize(0,100),CSize(0,100));
	m_ResizeFon.AddAnchor(CRect(223,115,277,149),CSize(0,100),CSize(100,100),CResizableImage::DUPLICATE);
	m_ResizeFon.AddAnchor(CRect(277,115,299,149),CSize(100,100),CSize(100,100));

	CRect rgnRect;
	GetWindowRect(&rgnRect);
	CRgn	WinRgn;
	WinRgn.CreateRoundRectRgn(0,0,rgnRect.Width(),rgnRect.Height(),20,20);
	SetWindowRgn(WinRgn,TRUE);
*/
	
	CString strRect = GetOptionString(IDS_OFSMESSENGER, IDS_GROUPSENDMESSAGE, _T(""));
	if(!strRect.IsEmpty())
	{
		CRect rWindow = StringToRect(strRect);
		FitRectToWindow(rWindow);
		SetWindowPos(NULL,rWindow.left,rWindow.top,rWindow.Width(),rWindow.Height(),SWP_NOZORDER|SWP_NOACTIVATE);
	}
	
}

//DEL void CGroupMessageSendDlg::OnPaint() 
//DEL {
//DEL 	CRect	winRect, editRect;
//DEL 	GetWindowRect(winRect);
//DEL 	
//DEL 	m_edit.GetWindowRect(&editRect);
//DEL 	ScreenToClient(&editRect);
//DEL 	
//DEL 	CPaintDC dc(this); // device context for painting
//DEL 	//CMemDC memdc(&dc);
//DEL 	if(pFonBmp)
//DEL 	{
//DEL 		//CDC dc;
//DEL 		//dc.CreateCompatibleDC(&memdc);
//DEL 		//dc.SelectObject(pFonBmp);
//DEL 		//memdc.BitBlt(0,0,sFon.cx,sFon.cy,&dc,0,0,SRCCOPY);
//DEL 		//memdc.BitBlt(0,0,sFon.cx,sFon.cy,&dc,0,0,SRCCOPY);
//DEL 		CRect m_Client;
//DEL 		GetClientRect(&m_Client);
//DEL 		CSize	winSize(m_Client.Width(),m_Client.Height());
//DEL 		m_ResizeFon.Render(dc.GetSafeHdc(),winSize);
//DEL 		m_UserInfo.Invalidate();
//DEL 	}
//DEL 	// Do not call COFSNcDlg2::OnPaint() for painting messages
//DEL }

//DEL void CGroupMessageSendDlg::OnLButtonDown(UINT nFlags, CPoint point) 
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

//DEL BOOL CGroupMessageSendDlg::OnEraseBkgnd( CDC* pDC )
//DEL {
//DEL 	return TRUE;
//DEL }

BEGIN_EVENTSINK_MAP(CGroupMessageSendDlg, COFSNcDlg2)
    //{{AFX_EVENTSINK_MAP(CGroupMessageSendDlg)
	ON_EVENT(CGroupMessageSendDlg, IDC_CCOOTREECTRL, 3 /* Action */, OnActionCcootreectrl, VTS_I4 VTS_BOOL)
	ON_EVENT(CGroupMessageSendDlg, IDC_CCOOTREECTRL, 4 /* DoDrop */, OnDoDropCcootreectrl, VTS_I4 VTS_BOOL VTS_UNKNOWN VTS_I4 VTS_I4)
	ON_EVENT(CGroupMessageSendDlg, IDC_CCOOTREECTRL, 1 /* Menu */, OnMenuCcootreectrl, VTS_I4 VTS_BOOL)
	ON_EVENT(CGroupMessageSendDlg, IDC_CCOOTREECTRL, 2 /* Select */, OnSelectCcootreectrl, VTS_I4 VTS_BOOL)
	ON_EVENT(CGroupMessageSendDlg, IDC_BTN_X, -600 /* Click */, OnClickBtnX, VTS_NONE)
	ON_EVENT(CGroupMessageSendDlg, IDC_BTN_MIN, -600 /* Click */, OnClickBtnMin, VTS_NONE)
	ON_EVENT(CGroupMessageSendDlg, IDC_BTN_OPTIONS, -600 /* Click */, OnClickBtnOptions, VTS_NONE)
	ON_EVENT(CGroupMessageSendDlg, IDC_BTN_MENU, -600 /* Click */, OnClickBtnMenu, VTS_NONE)
	ON_EVENT(CGroupMessageSendDlg, IDC_BTN_SELECT_ALL, -600 /* Click */, OnClickBtnSelectAll, VTS_NONE)
	ON_EVENT(CGroupMessageSendDlg, IDC_BTN_SELECT_NONE, -600 /* Click */, OnClickBtnSelectNone, VTS_NONE)
	ON_EVENT(CGroupMessageSendDlg, IDC_BTN_COLOR, -600 /* Click */, OnClickBtnColor, VTS_NONE)
	ON_EVENT(CGroupMessageSendDlg, IDC_BTN_BOLD, -600 /* Click */, OnClickBtnBold, VTS_NONE)
	ON_EVENT(CGroupMessageSendDlg, IDC_BTN_ITALIC, -600 /* Click */, OnClickBtnItalic, VTS_NONE)
	ON_EVENT(CGroupMessageSendDlg, IDC_BTN_UNDERLINE, -600 /* Click */, OnClickBtnUnderline, VTS_NONE)
	ON_EVENT(CGroupMessageSendDlg, IDC_BTN_SMILES, -600 /* Click */, OnClickBtnSmiles, VTS_NONE)
	ON_EVENT(CGroupMessageSendDlg, IDC_BTN_SEND, -600 /* Click */, OnClickBtnSend, VTS_NONE)
	//}}AFX_EVENTSINK_MAP
END_EVENTSINK_MAP()

//void CGroupMessageSendDlg::OnClickMcclose() 
//{
//	OnCancel();
//}

//DEL void CGroupMessageSendDlg::OnClickMcsend() 
//DEL {
//DEL 	OnSendButton();
//DEL }

void CGroupMessageSendDlg::OnClickMcmenu() 
{
	//??pMessenger->ShowUserMenu(m_Recipient.GetGlobalID());
}

//void CGroupMessageSendDlg::OnClickMcmini() 
//{
//	ShowWindow(SW_MINIMIZE);
//}

//DEL void CGroupMessageSendDlg::OnClickMcoptions() 
//DEL {
//DEL 	pMessenger->PreferenceDlg(this);
//DEL }

//DEL void CGroupMessageSendDlg::OnDestroy() 
//DEL {
//DEL 	COFSNcDlg2::OnDestroy();
//DEL }

void CGroupMessageSendDlg::OnSize(UINT nType, int cx, int cy) 
{
	COFSNcDlg2::OnSize(nType, cx, cy);

	m_edit.SetFocus();
}

//DEL BOOL CGroupMessageSendDlg::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message) 
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
//DEL 	//CRect ResizeRect(StatusRect.Width()-20,StatusRect.Height()-20,StatusRect.Width(),StatusRect.Height());
//DEL 	//if(ResizeRect.PtInRect(inPoint))
//DEL 	//{
//DEL 	//	SetCursor(LoadCursor(NULL,IDC_SIZENWSE));
//DEL 	//	return TRUE;
//DEL 	//}
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
//DEL 		else if(inPoint.x>miniRect.right)
//DEL 			if(inPoint.y<miniRect.top)
//DEL 				nHitTest = HTTOPRIGHT;
//DEL 			else if(inPoint.y>miniRect.bottom)
//DEL 				nHitTest = HTBOTTOMRIGHT;
//DEL 			else
//DEL 				nHitTest = HTRIGHT;
//DEL 		else if(inPoint.y<miniRect.top)
//DEL 			nHitTest = HTTOP;
//DEL 		else
//DEL 			nHitTest = HTBOTTOM;
//DEL 	}
//DEL 	
//DEL 	return COFSNcDlg2::OnSetCursor(pWnd, nHitTest, message);
//DEL }

//DEL void CGroupMessageSendDlg::OnLButtonUp(UINT nFlags, CPoint point) 
//DEL {
//DEL 	COFSNcDlg2::OnLButtonUp(nFlags, point);
//DEL }

//DEL void CGroupMessageSendDlg::OnMouseMove(UINT nFlags, CPoint point) 
//DEL {
//DEL 	COFSNcDlg2::OnMouseMove(nFlags, point);
//DEL }

//DEL void CGroupMessageSendDlg::OnCancelMode() 
//DEL {
//DEL 	COFSNcDlg2::OnCancelMode();
//DEL 	
//DEL }

void CGroupMessageSendDlg::OnDropFiles( HDROP hDropInfo )
{
//	UINT FileCount = DragQueryFile(hDropInfo,0xFFFFFFFF,NULL,0);
	//	
	//	CString strDescription;
	//	CFileDescriptioDlg	DescrDlg(this);
	//	
	//	DescrDlg.m_strFileName = _T("");
	//	for(UINT i=0;i<FileCount;i++)
	//	{
	//		CString strPath;
	//		TCHAR  FileBuffer[MAX_PATH];
	//		DragQueryFile(hDropInfo,i,FileBuffer,MAX_PATH);
	//		strPath = FileBuffer;
	//		strPath = strPath.Mid(strPath.ReverseFind('\\')+1);
	//
	//		DescrDlg.m_strFileName += strPath;
	//		DescrDlg.m_strFileName += _T("; ");
	//	}
	//	
	//
	//	if(DescrDlg.DoModalEditMode()==IDOK)
	//	{
	//		strDescription = DescrDlg.GetDescription();	
	//	}
	//	else
	//		strDescription = _T("");
	//	
	//	for(i=0;i<FileCount;i++)
	//	{
	//		TCHAR FileBuffer[MAX_PATH];
	//		DragQueryFile(hDropInfo,i,FileBuffer,MAX_PATH);
	//		pMessenger->SendFileToUser(&m_Recipient,FileBuffer,strDescription);
	//	}
	
}

//DEL void CGroupMessageSendDlg::OnColorButton() 
//DEL {
//DEL 	CColorDialog m_Color(m_edit.GetColor(), CC_ANYCOLOR,this);
//DEL 	if(m_Color.DoModal()==IDOK)
//DEL 	{
//DEL 		m_edit.SetColor(m_Color.GetColor());
//DEL 	}
//DEL 	m_edit.SetFocus();
//DEL }

//DEL void CGroupMessageSendDlg::OnBoldCheck() 
//DEL {
//DEL 	// TODO: Add your control notification handler code here
//DEL 	m_edit.SetBold();
//DEL 	m_edit.SetFocus();
//DEL 	WriteOptionInt(IDS_OFSMESSENGER,IDS_BOLD,m_BoldButton.GetCheck());
//DEL }

//DEL void CGroupMessageSendDlg::OnItalicCheck() 
//DEL {
//DEL 	// TODO: Add your control notification handler code here
//DEL     m_edit.SetItalic();
//DEL 	m_edit.SetFocus();
//DEL 	WriteOptionInt(IDS_OFSMESSENGER,IDS_ITALIC,m_ItalicButton.GetCheck());
//DEL }

//DEL void CGroupMessageSendDlg::OnUnderlineCheck() 
//DEL {
//DEL 	// TODO: Add your control notification handler code here
//DEL 	m_edit.SetUnderline();
//DEL 	m_edit.SetFocus();
//DEL 	WriteOptionInt(IDS_OFSMESSENGER,IDS_UNDERLINE,m_UnderLineButton.GetCheck());
//DEL }

HRESULT CGroupMessageSendDlg::OnEditUpdate(WPARAM w, LPARAM l)
{
	if(!m_strSetBody.IsEmpty())
	{
		CComBSTR	bsSetBody = m_strSetBody;
		m_edit.InsertHTML(bsSetBody);
		m_strSetBody.Empty();

		if(m_bAutoSend)
		{
			m_bAutoSend	=	FALSE;
			Send();
		}
	}
	
	if(!bInitEdit)
	{
		bInitEdit = TRUE;
		
		/*
		m_edit.SetTextSize(GetOptionInt(IDS_OFSMESSENGER,IDS_SIZE,1)+1);	
				CString strFontName = _T("Arial");
				int FontId = GetOptionInt(IDS_OFSMESSENGER,IDS_FONT,-1);
				if(FontId!=-1)
					m_FontCombo.GetLBText(FontId, strFontName);
				CComBSTR	bsFontName	=	strFontName;
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
	
	/*
	CCmdUI m_Command;
	
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

void CGroupMessageSendDlg::OnSelEndOkSizeCombo() 
{
	SetFontSize();
}

void CGroupMessageSendDlg::OnSelEndOkFontCombo() 
{
	SetFontFace();
}

void CGroupMessageSendDlg::OnSmileItem(UINT nID)
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


void CGroupMessageSendDlg::SetBody(LPCTSTR strBody, BOOL bAutoSend)
{
	m_bAutoSend = bAutoSend;
	m_strSetBody = strBody;
	m_edit.InsertTEXT(CComBSTR(_T("")));
	m_edit.SetFocus();
}

LPARAM CGroupMessageSendDlg::OnSWMSetBody(WPARAM w, LPARAM l)
{
	if(!Handle)
	{
		SetBody(LPCTSTR(w),l!=NULL);
	}
	return 0;
}

void CGroupMessageSendDlg::CreateTree()
{

	CBitmap			hbmpCheckImage;	
	hbmpCheckImage.LoadBitmap(IDB_TREECHECK);
	m_treectrl.SetImageList((long)hbmpCheckImage.Detach());
	
	short PriorityIndex[10];
	for(int i=0;i<10;i++)
		PriorityIndex[i] = -1;
	
	m_treectrl.SetPriority(PriorityIndex);
	
	long  m_ShablonId[4][10]	=	{
		{0L,0L,0L,0L,0L,0L,0L,0L,0L,0L}, /// Группа ...
		{1L,0L,0L,0L,0L,0L,0L,0L,0L,0L}, /// Группа ...
		{2L,0L,0L,0L,0L,0L,0L,0L,0L,0L},  /// User
		{3L,0L,0L,0L,0L,0L,0L,0L,0L,0L}  /// User
	};
	
	short m_ShablonIcon[4][10]	=	
	{
		{2,-1,-1,-1,-1,-1,-1,-1,-1,-1}, /// Группа UnCheck
		{3,-1,-1,-1,-1,-1,-1,-1,-1,-1},  /// Группа Check
		{2,-1,-1,-1,-1,-1,-1,-1,-1,-1},  /// User UnCheck
		{3,-1,-1,-1,-1,-1,-1,-1,-1,-1}  /// User Check
	};
	
	DWORD m_ShablonRGBTextEnable [4] = 
	{
		RGB(0,0,100), /// Группа ...
			RGB(0,0,100), /// Группа ...
			RGB(0,0,0), /// User
			RGB(0,0,0) /// User
	};
	
	DWORD m_ShablonRGBTextSelect[4] = 
	{
		RGB(0,0,200), /// Группа ...
			RGB(0,0,200), /// Группа ...
			RGB(0,0,0), /// User
			RGB(0,0,0) /// User
	};
	
	for(int i = 0 ;i<4;i++)
	{
		m_treectrl.AddEffect(m_ShablonId[i],m_ShablonIcon[i],m_ShablonRGBTextEnable[i],
			m_ShablonRGBTextSelect[i],RGB(255,255,255),RGB(200,200,200));
	}
	
	m_treectrl.SetEventMode(1);
}

void CGroupMessageSendDlg::BuildTree()
{
	m_treectrl.DeleteTree();

	m_UserCheckInGroup.RemoveAll();
	m_GroupTIDMap.RemoveAll();
	
	int CLMode  = GetOptionInt(IDS_OFSMESSENGER,IDS_CLMODE,2);

	CUser* pUser=NULL;

	long  m_ShablonId[4][10]	=	{
		{0L,0L,0L,0L,0L,0L,0L,0L,0L,0L}, /// Группа ...
		{1L,0L,0L,0L,0L,0L,0L,0L,0L,0L}, /// Группа ...
		{2L,0L,0L,0L,0L,0L,0L,0L,0L,0L},  /// User
		{3L,0L,0L,0L,0L,0L,0L,0L,0L,0L}  /// User
	};
	
	
	switch(CLMode) 
	{
	case 1:
		{
			if(POSITION pos = m_ContactList.InitIteration())
			{
				for(int i=0; m_ContactList.GetNext(pos,pUser); i++)
				{
					// Step 1. Проверить создавали ли мы группу???
					long	GroupTID	= 0;
					CString	GroupName =	pUser->m_strType;
					
					BOOL isCheck = FALSE;//(m_strRecepientGroupName.CompareNoCase(pUser->m_strType)==0);
					
					if(!m_GroupTIDMap.Lookup(pUser->m_strType,(void*&)GroupTID))
					{
						// Step 2. Если нет, то создать группу .
						GroupTID = m_treectrl.AddItem(0,pUser->m_strType,m_ShablonId[0+isCheck]);
						m_GroupTIDMap.SetAt(pUser->m_strType,(void*)GroupTID);
						m_UserCheckInGroup.SetAt(GroupName,(void*)0);
					}
					// Step 3. добавить пользователя [1/28/2002]
					pUser->TID = m_treectrl.AddItem(GroupTID,pUser->GetShowName(),m_ShablonId[pUser->m_bHasNewMessages+2]);								
					
					if(pUser->m_bHasNewMessages)
						m_UserCheckInGroup.SetAt(GroupName,(void*)(int(m_UserCheckInGroup[GroupName])+1));
					
					m_treectrl.RootOpen(GroupTID,pUser->m_bHasNewMessages);
				}
			}
		}
		break;
	case 2:
		{
			if(POSITION pos = m_ContactList.InitIteration())
			{
				for(int i=0; m_ContactList.GetNext(pos,pUser); i++)
				{
					// Step 1. Проверить создавали ли мы группу???
					long	GroupTID	= 0;
					CString	GroupName =	pUser->m_strType;
					
					BOOL isCheck = FALSE;//(m_strRecepientGroupName.CompareNoCase(pUser->m_strType)==0);
					
					if(pUser->m_iStatus==S_OFFLINE||pUser->m_iStatus==S_INVISIBLE)
					{
						GroupName=  GetString(IDS_OFFLINE);

						if(!m_GroupTIDMap.Lookup(GetString(IDS_OFFLINE),(void*&)GroupTID))
						{
							long ShablonId[10] = {0L,1L,0L,0L,0L,0L,0L,0L,0L,0L};
							// Step 2. Если нет, то создать группу .
							GroupTID = m_treectrl.AddItem(0,GetString(IDS_OFFLINE),ShablonId);
							m_GroupTIDMap.SetAt(GetString(IDS_OFFLINE),(void*)GroupTID);
							m_UserCheckInGroup.SetAt(GroupName,(void*)0);
						}
					}
					else
					if(!m_GroupTIDMap.Lookup(pUser->m_strType,(void*&)GroupTID))
					{
						// Step 2. Если нет, то создать группу .
						GroupTID = m_treectrl.AddItem(0,pUser->m_strType,m_ShablonId[0+isCheck]);
						m_GroupTIDMap.SetAt(pUser->m_strType,(void*)GroupTID);
						m_UserCheckInGroup.SetAt(GroupName,(void*)0);
					}
					// Step 3. добавить пользователя [1/28/2002]
					pUser->TID = m_treectrl.AddItem(GroupTID,pUser->GetShowName(),m_ShablonId[pUser->m_bHasNewMessages+2]);								

					if(pUser->m_bHasNewMessages)
						m_UserCheckInGroup.SetAt(GroupName,(void*)(int(m_UserCheckInGroup[GroupName])+1));
					
					m_treectrl.RootOpen(GroupTID,pUser->m_bHasNewMessages);
				}
			}
		}
		break;
	}
	
	UpdateGroupCheck();
}

void CGroupMessageSendDlg::OnActionCcootreectrl(long TID, BOOL bGroupe) 
{
	if(TID!= -1)
		if(!bGroupe)
		{
			CUser *pUser=FindUserInVisualContactList(TID);
			if(pUser)
			{
				pUser->m_bHasNewMessages = !pUser->m_bHasNewMessages;
				// Change User Check in group [2/21/2002]
				int CLMode  = GetOptionInt(IDS_OFSMESSENGER,IDS_CLMODE,2);
				switch(CLMode) 
				{
				case 1:
					{
						CString strGroupName;
						strGroupName = pUser->m_strType;
						
						if(pUser->m_bHasNewMessages)
							m_UserCheckInGroup.SetAt(strGroupName,(void*)(int(m_UserCheckInGroup[strGroupName])+1));
						else
							m_UserCheckInGroup.SetAt(strGroupName,(void*)(int(m_UserCheckInGroup[strGroupName])-1));
						
						UpdateGroupID(strGroupName);
					}
					break;
				case 2:
					{
						CString strGroupName;
						if(pUser->m_iStatus==S_OFFLINE||pUser->m_iStatus==S_INVISIBLE)
							strGroupName = GetString(IDS_OFFLINE);
						else
							strGroupName = pUser->m_strType;
						
						if(pUser->m_bHasNewMessages)
							m_UserCheckInGroup.SetAt(strGroupName,(void*)(int(m_UserCheckInGroup[strGroupName])+1));
						else
							m_UserCheckInGroup.SetAt(strGroupName,(void*)(int(m_UserCheckInGroup[strGroupName])-1));
						
						UpdateGroupID(strGroupName);
					}
					break;
				};
				// [2/21/2002]
				UpdateID(pUser->GetGlobalID());
			}
		}	
		else
		{
			CString strGrouName;
			
			POSITION pos =  m_GroupTIDMap.GetStartPosition();
			while(pos)
			{
				CString strKey;
				long	Data;
				m_GroupTIDMap.GetNextAssoc(pos,strKey,(void*&)Data);
				if(Data==TID)
				{
					strGrouName = strKey;
					break;
				}
			}

			if(!strGrouName.IsEmpty())
			{
				BOOL bCheck = !((BOOL)m_UserCheckInGroup[strGrouName]);
				if(POSITION pos = m_ContactList.InitIteration())
				{
					CUser *pUser=NULL;

					m_UserCheckInGroup.SetAt(strGrouName,(void*)0);
					
					while(m_ContactList.GetNext(pos,pUser))
					{
								
						int CLMode  = GetOptionInt(IDS_OFSMESSENGER,IDS_CLMODE,2);
						switch(CLMode) 
						{
						case 1:
							{
								if(strGrouName.CompareNoCase(pUser->m_strType)==0)
								{
									pUser->m_bHasNewMessages = bCheck;
									UpdateID(pUser->GetGlobalID());
									if(pUser->m_bHasNewMessages)
										m_UserCheckInGroup.SetAt(strGrouName,(void*)(int(m_UserCheckInGroup[strGrouName])+1));
								}
							}
							break;
						case 2:
							{
								//  [2/21/2002]
								if(strGrouName.CompareNoCase(GetString(IDS_OFFLINE))==0&&
									(pUser->m_iStatus==S_OFFLINE||pUser->m_iStatus==S_INVISIBLE))
								{
									pUser->m_bHasNewMessages = bCheck;
									UpdateID(pUser->GetGlobalID());
									if(pUser->m_bHasNewMessages)
										m_UserCheckInGroup.SetAt(strGrouName,(void*)(int(m_UserCheckInGroup[strGrouName])+1));
								}
								else if(strGrouName.CompareNoCase(pUser->m_strType)==0&&
									!(pUser->m_iStatus==S_OFFLINE||pUser->m_iStatus==S_INVISIBLE))
								{
									pUser->m_bHasNewMessages = bCheck;
									UpdateID(pUser->GetGlobalID());
									if(pUser->m_bHasNewMessages)
										m_UserCheckInGroup.SetAt(strGrouName,(void*)(int(m_UserCheckInGroup[strGrouName])+1));
								}
								//  [2/21/2002]
							}
							break;
						}
					}
				}
				UpdateGroupID(strGrouName);
			}
		}
}

void CGroupMessageSendDlg::OnDoDropCcootreectrl(long TID, BOOL bGroupe, LPUNKNOWN pDataObject, long DropEffect, long LastKeyState) 
{
	// TODO: Add your control notification handler code here
	
}

void CGroupMessageSendDlg::OnMenuCcootreectrl(long TID, BOOL bGroupe) 
{
	if(!bGroupe)
	{
		CUser *pUser = FindUserInVisualContactList(TID);
		if(pUser)
		{
			CUser User = *pUser;
			pMessenger->ShowUserMenu(User.GetGlobalID());
		}
	}
}

void CGroupMessageSendDlg::OnSelectCcootreectrl(long TID, BOOL bGroupe) 
{
	//OnActionCcootreectrl(TID, bGroupe);
}

void CGroupMessageSendDlg::UpdateID(long UserId)
{
	CUser *pUser = m_ContactList.GetAt(UserId);
	if(pUser)
	{	
		long  m_ShablonId[4][10]	=	{
			{0L,0L,0L,0L,0L,0L,0L,0L,0L,0L}, /// Группа ...
			{1L,0L,0L,0L,0L,0L,0L,0L,0L,0L}, /// Группа ...
			{2L,0L,0L,0L,0L,0L,0L,0L,0L,0L},  /// User
			{3L,0L,0L,0L,0L,0L,0L,0L,0L,0L}  /// User
		};
		
		m_treectrl.SetItemId(pUser->TID,m_ShablonId[pUser->m_bHasNewMessages+2]);
	}
}

CUser* CGroupMessageSendDlg::FindUserInVisualContactList(long TID)
{
	if(POSITION pos = m_ContactList.InitIteration())
	{
		CUser *pUser=NULL;
		
		while(m_ContactList.GetNext(pos,pUser))
		{
			if(pUser->TID == TID )
				return pUser;
		}
	}
	
	return NULL;
}

//DEL void CGroupMessageSendDlg::OnClickMcselectAll()
//DEL {
//DEL 	if(m_ContactList.InitIteration())
//DEL 	{
//DEL 		CUser *pUser=NULL;
//DEL 		
//DEL 		while(m_ContactList.GetNext(pUser))
//DEL 		{
//DEL 			pUser->m_bHasNewMessages = TRUE;
//DEL 		}
//DEL 	}
//DEL 	BuildTree();
//DEL }

BOOL CGroupMessageSendDlg::EnableRecepients()
{
	CUser *pRecipient			=	NULL;
	BOOL	bEnableContactUser	=	FALSE;

	if(POSITION pos = m_ContactList.InitIteration())
	{
		while(m_ContactList.GetNext(pos,pRecipient))
		{
			if(pRecipient->m_bHasNewMessages)
				return  TRUE;
		}
	}
	return FALSE;
}

void CGroupMessageSendDlg::UpdateGroupCheck()
{
	POSITION pos =  m_UserCheckInGroup.GetStartPosition();
	while(pos)
	{
		CString strKey;
		int		Data;
		m_UserCheckInGroup.GetNextAssoc(pos,strKey,(void*&)Data);
		UpdateGroupID(strKey);
	}
}

void CGroupMessageSendDlg::UpdateGroupID(LPCTSTR strName)
{
	long  m_ShablonId[4][10]	=	{
		{0L,0L,0L,0L,0L,0L,0L,0L,0L,0L}, /// Группа ...
		{1L,0L,0L,0L,0L,0L,0L,0L,0L,0L}, /// Группа ...
		{2L,0L,0L,0L,0L,0L,0L,0L,0L,0L},  /// User
		{3L,0L,0L,0L,0L,0L,0L,0L,0L,0L}  /// User
	};
	
	if(m_UserCheckInGroup[strName])
		m_treectrl.SetItemId((long)m_GroupTIDMap[strName],m_ShablonId[1]);
	else
		m_treectrl.SetItemId((long)m_GroupTIDMap[strName],m_ShablonId[0]);
}

//DEL void CGroupMessageSendDlg::OnClickSelectAll() 
//DEL {
//DEL }

//DEL void CGroupMessageSendDlg::OnClickSend() 
//DEL {
//DEL 	OnSendButton();
//DEL }

//DEL void CGroupMessageSendDlg::OnMcunselectall() 
//DEL {
//DEL 	if(m_ContactList.InitIteration())
//DEL 	{
//DEL 		CUser *pUser=NULL;
//DEL 		
//DEL 		while(m_ContactList.GetNext(pUser))
//DEL 		{
//DEL 			pUser->m_bHasNewMessages = FALSE;
//DEL 		}
//DEL 	}
//DEL 	BuildTree();
//DEL }

void CGroupMessageSendDlg::LoadSkin(IXMLDOMNode *pXmlRoot)
{
	m_btnMenu.ShowWindow(SW_HIDE);
	m_btnOptions.ShowWindow(SW_HIDE);
	m_btnMin.ShowWindow(SW_HIDE);
	m_btnX.ShowWindow(SW_HIDE);

	m_btnSelectAll.ShowWindow(SW_HIDE);
	m_btnSelectNone.ShowWindow(SW_HIDE);
	
	m_btnColor.ShowWindow(SW_HIDE);
	m_btnBold.ShowWindow(SW_HIDE);
	m_btnItalic.ShowWindow(SW_HIDE);
	m_btnUnderline.ShowWindow(SW_HIDE);
	m_btnSmiles.ShowWindow(SW_HIDE);
	m_btnSend.ShowWindow(SW_HIDE);
	
	LoadButton(pXmlRoot, _T("Menu"), &m_btnMenu, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Options"), &m_btnOptions, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Minimize"), &m_btnMin, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("X"), &m_btnX, TRUE, FALSE);
	
	LoadButton(pXmlRoot, _T("SelectAll"), &m_btnSelectAll, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("SelectNone"), &m_btnSelectNone, TRUE, FALSE);
	
	LoadButton(pXmlRoot, _T("Color"), &m_btnColor, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Bold"), &m_btnBold, TRUE, TRUE);
	LoadButton(pXmlRoot, _T("Italic"), &m_btnItalic, TRUE, TRUE);
	LoadButton(pXmlRoot, _T("Underline"), &m_btnUnderline, TRUE, TRUE);
	LoadButton(pXmlRoot, _T("Smiles"), &m_btnSmiles, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Send"), &m_btnSend, TRUE, FALSE);
	
	LoadLabel(pXmlRoot, _T("Recipient"), &m_UserInfo, TRUE);
	
	LoadRectangle(pXmlRoot, _T("Users"), &m_treectrl, TRUE);
	LoadRectangle2(pXmlRoot, _T("Edit"), m_edit.GetSafeHwnd(), TRUE);
	
	LoadRectangle2(pXmlRoot, _T("ComboFont"), m_FontCombo.GetSafeHwnd(), TRUE);
	LoadRectangle2(pXmlRoot, _T("ComboSize"), m_SizeCombo.GetSafeHwnd(), TRUE);
}

void CGroupMessageSendDlg::SelectAll(BOOL bSelect)
{
	if(POSITION pos = m_ContactList.InitIteration())
	{
		CUser *pUser=NULL;
		
		while(m_ContactList.GetNext(pos,pUser))
		{
			pUser->m_bHasNewMessages = bSelect;
		}
	}
	BuildTree();
}

//DEL void CGroupMessageSendDlg::SelectNone()
//DEL {
//DEL 	if(m_ContactList.InitIteration())
//DEL 	{
//DEL 		CUser *pUser=NULL;
//DEL 		
//DEL 		while(m_ContactList.GetNext(pUser))
//DEL 		{
//DEL 			pUser->m_bHasNewMessages = FALSE;
//DEL 		}
//DEL 	}
//DEL 	BuildTree();
//DEL }

void CGroupMessageSendDlg::Send()
{
	UpdateData(TRUE);
	
	if(pMessenger->ConnectEnable() && !Handle)
	{
		// Testirovanie na ne pustoi Send List  [2/20/2002]
		
		if(!EnableRecepients())
			return;
		//////////////////////////////////////////////////////////////////////////


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
				
				if(POSITION pos = m_ContactList.InitIteration())
				{
					CUser *pRecipient	=	NULL;
					while(m_ContactList.GetNext(pos,pRecipient))
					{
						if(pRecipient->m_bHasNewMessages)
						{
							IUserPtr  pUser = pResepients->AddUser();
							pUser->PutValue("@id",pRecipient->GetGlobalID());
						}
					}
					
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
			}
			catch(_com_error&)
			{
				m_edit.SetFocus();
			}
		}

	}
}

void CGroupMessageSendDlg::SelectColor()
{
	CColorDialog m_Color(m_edit.GetColor(), CC_ANYCOLOR,this);
	if(m_Color.DoModal()==IDOK)
	{
		m_edit.SetColor(m_Color.GetColor());
	}
	m_edit.SetFocus();
}

void CGroupMessageSendDlg::InsertSmile()
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

void CGroupMessageSendDlg::SetFontBold()
{
	m_edit.SetBold();
	m_edit.SetFocus();
	WriteOptionInt(IDS_OFSMESSENGER,IDS_BOLD, m_btnBold.GetPressed());
}

void CGroupMessageSendDlg::SetFontItalic()
{
    m_edit.SetItalic();
	m_edit.SetFocus();
	WriteOptionInt(IDS_OFSMESSENGER,IDS_ITALIC,m_btnItalic.GetPressed());
}

void CGroupMessageSendDlg::SetFontUnderline()
{
	m_edit.SetUnderline();
	m_edit.SetFocus();
	WriteOptionInt(IDS_OFSMESSENGER,IDS_UNDERLINE,m_btnUnderline.GetPressed());
}

void CGroupMessageSendDlg::SetFontSize()
{
	WriteOptionInt(IDS_OFSMESSENGER,IDS_SIZE,m_SizeCombo.GetCurSel());
	m_edit.SetTextSize(m_SizeCombo.GetCurSel()+1);
	//  [4/29/2002]
	m_edit.SetDefaultFontSize(nFontSizes[m_SizeCombo.GetCurSel()]);
	//  [4/29/2002]
	m_edit.SetFocus();
}

void CGroupMessageSendDlg::SetFontFace()
{
	int nIndex = m_FontCombo.GetCurSel();
	
	CString strFontName;
	m_FontCombo.GetLBText(nIndex, strFontName);

	WriteOptionString(IDS_OFSMESSENGER,IDS_FONT,strFontName);

	CComBSTR	bsFontName	=	strFontName;
	m_edit.SetFontName(bsFontName);
	//  [4/29/2002]
	m_edit.SetDefaultFontName(bsFontName);
	//  [4/29/2002]
	m_edit.SetFocus();
}

void CGroupMessageSendDlg::OnClickBtnX() 
{
	OnCancel();
}

BOOL CGroupMessageSendDlg::Create(CWnd *pParentWnd)
{
	if(!COFSNcDlg2::Create(IDD, pParentWnd))
	{
		TRACE0("Warning: failed to create CMessageSplitDlg3.\n");
		return FALSE;
	}
	if(!m_bLoadSkin)
		COFSNcDlg2::LoadSkin();
	
	CString strRect = GetOptionString(IDS_OFSMESSENGER, IDS_GROUPSENDMESSAGE, _T(""));
	if(!strRect.IsEmpty())
	{
		CRect rWindow = StringToRect(strRect);
		FitRectToWindow(rWindow);
		SetWindowPos(NULL, rWindow.left, rWindow.top, rWindow.Width(), rWindow.Height(), SWP_NOZORDER|SWP_NOACTIVATE);
	}
	
	return TRUE;
}

void CGroupMessageSendDlg::OnClickBtnMin() 
{
	ShowWindow(SW_MINIMIZE);
}

void CGroupMessageSendDlg::OnClickBtnOptions() 
{
	pMessenger->PreferenceDlg(this);
}

void CGroupMessageSendDlg::OnClickBtnMenu() 
{
}

void CGroupMessageSendDlg::OnClickBtnSelectAll() 
{
	SelectAll(TRUE);
}

void CGroupMessageSendDlg::OnClickBtnSelectNone() 
{
	SelectAll(FALSE);
}

void CGroupMessageSendDlg::OnClickBtnColor() 
{
	m_bIgnoreActivate = TRUE;
	SelectColor();
	m_bIgnoreActivate = FALSE;
}

void CGroupMessageSendDlg::OnClickBtnBold() 
{
	SetFontBold();
}

void CGroupMessageSendDlg::OnClickBtnItalic() 
{
	SetFontItalic();
}

void CGroupMessageSendDlg::OnClickBtnUnderline() 
{
	SetFontUnderline();
}

void CGroupMessageSendDlg::OnClickBtnSmiles() 
{
	InsertSmile();
}

void CGroupMessageSendDlg::OnClickBtnSend() 
{
	Send();
}

void CGroupMessageSendDlg::SetRecipientGroup(LPCTSTR strName, CUserCollection *pUsers)
{
	m_strRecepientGroupName = strName;
	
	m_UserInfo.SetText(strName);
	
	m_ContactList.Clear();
	
	try
	{
		CUser* pUser=NULL;
		
		if(POSITION pos = pUsers->InitIteration())
		{
			for(int i=0; pUsers->GetNext(pos,pUser); i++)
			{
				m_ContactList.SetAt(*pUser);
			}
		}
	}
	catch(...)
	{
		ASSERT(FALSE);
	}
	
	BuildTree();
}
