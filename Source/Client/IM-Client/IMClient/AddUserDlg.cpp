// AddUserDlg.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "AddUserDlg.h"
#include "MainDlg.h"
#include "LoadSkins.h"
#include <triedcid.h>
//#include "smileyahoopopup.h"


#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

extern CString GetCurrentSkin();

// New Smile Addon [2007-02-06]
#include "SmileManager.h"
#include "SelectSmileDlg.h"

extern  CSmileManager CurrentSmileManager;
//

/////////////////////////////////////////////////////////////////////////////
// CAddUserDlg dialog
#define WM_EDIT_UPDATE WM_USER + 201

CAddUserDlg::CAddUserDlg(CMainDlg* pParent /*=NULL*/)
: CResizableDialog(CAddUserDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CAddUserDlg)
	//}}AFX_DATA_INIT
	Handle = 0;
	pMessenger = pParent;
	bIsKillWinodow = FALSE;
	bInitEdit = FALSE;
	m_bWasCtrlEnter	=	FALSE;
	m_bWasCtrlExit	=	FALSE;
	bBlock			=	FALSE;
}


void CAddUserDlg::DoDataExchange(CDataExchange* pDX)
{
	CResizableDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CAddUserDlg)
	DDX_Control(pDX, IDC_FONT_COMBO, m_FontCombo);
	DDX_Control(pDX, IDC_SIZE_COMBO, m_SizeCombo);
	DDX_Control(pDX, IDC_FRAME_EDIT, m_frameEdit);
	DDX_Control(pDX, IDC_NICK_EDIT, m_Nick);
	DDX_Control(pDX, IDC_COLOR_BUTTON, m_ColorButton);
	DDX_Control(pDX, IDC_BOLD_CHECK, m_BoldButton);
	DDX_Control(pDX, IDC_ITALIC_CHECK, m_ItalicButton);
	DDX_Control(pDX, IDC_UNDERLINE_CHECK, m_UnderLineButton);
	DDX_Control(pDX, IDCANCEL, m_CancelButton);
	DDX_Control(pDX, IDC_INSERTSMILE_CHECK, m_InsertSmileButton);
	DDX_Control(pDX, IDC_MCOK, m_Ok);
	DDX_Control(pDX, IDC_MCCANCEL, m_Cancel);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CAddUserDlg, CResizableDialog)
	//{{AFX_MSG_MAP(CAddUserDlg)
	ON_BN_CLICKED(ID_AUTHORIZATION, OnAuthorization)
	ON_BN_CLICKED(IDC_COLOR_BUTTON, OnColorButton)
	ON_BN_CLICKED(IDC_BOLD_CHECK, OnBoldCheck)
	ON_BN_CLICKED(IDC_ITALIC_CHECK, OnItalicCheck)
	ON_BN_CLICKED(IDC_UNDERLINE_CHECK, OnUnderlineCheck)
	ON_COMMAND(ID_EDITMENU_COPY, OnEditmenuCopy)
	ON_UPDATE_COMMAND_UI(ID_EDITMENU_COPY, OnUpdateEditmenuCopy)
	ON_COMMAND(ID_EDITMENU_CUT, OnEditmenuCut)
	ON_UPDATE_COMMAND_UI(ID_EDITMENU_CUT, OnUpdateEditmenuCut)
	ON_COMMAND(ID_EDITMENU_DELETE, OnEditmenuDelete)
	ON_UPDATE_COMMAND_UI(ID_EDITMENU_DELETE, OnUpdateEditmenuDelete)
	ON_COMMAND(ID_EDITMENU_PAST, OnEditmenuPast)
	ON_UPDATE_COMMAND_UI(ID_EDITMENU_PAST, OnUpdateEditmenuPast)
	ON_WM_CAPTURECHANGED()
	ON_BN_CLICKED(IDC_INSERTSMILE_CHECK, OnInsertsmileCheck)
	ON_CBN_SELENDOK(IDC_FONT_COMBO, OnSelendokFontCombo)
	ON_CBN_SELENDOK(IDC_SIZE_COMBO, OnSelendokSizeCombo)
	ON_BN_CLICKED(IDC_MCOK, OnClickAuth)
	ON_BN_CLICKED(IDC_MCCANCEL, OnClickCancel)
	//}}AFX_MSG_MAP
	ON_MESSAGE(WM_NLT_CONTAINER_EVENT,OnNetEvent)	
	ON_MESSAGE(WM_EDIT_UPDATE,OnEditUpdate)
	ON_COMMAND_RANGE(20000,20000+SmileBuffSize,OnSmileItem)
END_MESSAGE_MAP()

BEGIN_EVENTSINK_MAP(CAddUserDlg, CResizableDialog)
//{{AFX_EVENTSINK_MAP(CAddUserDlg)
ON_EVENT(CAddUserDlg, IDC_MCCLOSE, -600 /* Click */, OnClickMcclose, VTS_NONE)
ON_EVENT(CAddUserDlg, IDC_MCMINI, -600 /* Click */, OnClickMcmini, VTS_NONE)
ON_EVENT(CAddUserDlg, IDC_MCOK, -600 /* Click */, OnClickMcOk, VTS_NONE)
ON_EVENT(CAddUserDlg, IDC_MCCANCEL, -600 /* Click */, OnClickMcCancel, VTS_NONE)
//}}AFX_EVENTSINK_MAP
END_EVENTSINK_MAP()

/////////////////////////////////////////////////////////////////////////////
// CAddUserDlg message handlers

void CAddUserDlg::AddNewContact(long user_id, LPCTSTR nick_name, LPCTSTR first_name, LPCTSTR last_name, LPCTSTR email, long company_id, LPCTSTR company_name, long role_id, LPCTSTR role_name)
{
	m_User.m_strLogin       = nick_name;
	m_User.m_strFirstName	=	first_name;
	m_User.m_strLastName	=	last_name;
	m_User.m_strEMail		=	email;
	
	m_Nick.SetText(m_User.GetShowName());

	m_User.GlobalID       = user_id;
	m_User.m_RoleID       = role_id;
	m_User.m_iStatus      = S_AWAITING;

	CString strFormat;
	CString strTitleFormat;
	strTitleFormat.LoadString(IDS_ADD_FRIEND_TITLE);
	strFormat.Format(strTitleFormat,m_User.GetShowName());
	SetWindowText(strFormat);

	UpdateData(FALSE);
}

void CAddUserDlg::OnOK() 
{
}

void CAddUserDlg::OnCancel() 
{
	if(Handle&&pMessenger->ConnectEnable())
	{
		pSession->CancelOperation(Handle);
	}
	else
		KillWindow();
}


BOOL CAddUserDlg::OnInitDialog() 
{
	CResizableDialog::OnInitDialog();

	m_font.Attach(GetStockObject(DEFAULT_GUI_FONT));

	ShowSizeGrip(FALSE);
//	m_Close.SetAutoPressed(TRUE);
//	m_Close.SetCanStayPressed(FALSE);
//	m_Mini.SetAutoPressed(TRUE);
//	m_Mini.SetCanStayPressed(FALSE);
//	m_Ok.SetAutoPressed(TRUE);
//	m_Ok.SetCanStayPressed(FALSE);
//	m_Cancel.SetAutoPressed(TRUE);
//	m_Cancel.SetCanStayPressed(FALSE);
	
//	LoadSkin();
	

	pSession = theNet2.GetSession();
	ASSERT(pSession!=NULL);
	
	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),FALSE);
	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),TRUE);

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

   	m_BoldButton.SetIcon(FontStateList.ExtractIcon(0));
	m_ItalicButton.SetIcon(FontStateList.ExtractIcon(1));
	m_UnderLineButton.SetIcon(FontStateList.ExtractIcon(2));
	m_ColorButton.SetIcon(FontStateList.ExtractIcon(3));
	m_InsertSmileButton.SetIcon(FontStateList.ExtractIcon(4));

//	m_Nick.SetTextColor(0xffffff);
//	m_Nick.SetTransparent(TRUE);
	
//	CRect rect;
//	rect.top = 67;
//	rect.bottom = rect.top + nDropHeight;
//	rect.left = 150;
//	rect.right = rect.left + 40;
	
//	m_SizeCombo.Create(CBS_DROPDOWNLIST|WS_VISIBLE|WS_TABSTOP, rect,this,IDC_SIZE_COMBO);
//	m_SizeCombo.SetFont(&m_font);
    CString str;
	for (int i = 0; i < sizeof(nFontSizes)/sizeof(int); i++)
	{
		str.Format(_T("%d"), nFontSizes[i]);
		m_SizeCombo.AddString(str);
	}
	
	///// Font Combo Create ....
//	rect.top = 67;
//	rect.bottom = rect.top + nDropHeight;
//	rect.left = 195;
//	rect.right = rect.left+120;
	
//	m_FontCombo.Create(CBS_DROPDOWNLIST|WS_VSCROLL|CBS_SORT |WS_VISIBLE|WS_TABSTOP, rect, this, IDC_FONT_COMBO);
//	m_FontCombo.SetFont(&m_font);
	::EnumFontFamilies(GetDC()->m_hDC, (LPTSTR) NULL, (FONTENUMPROC)NEnumFontNameProc, (LPARAM)&(m_FontCombo));

	CRect windowRect;
	GetWindowRect(&windowRect);
	
	CRect rTmp(15,94,windowRect.Width()-15,windowRect.Height()-50);
    m_edit.Create(NULL,NULL,WS_VISIBLE|WS_BORDER,rTmp,this,IDC_DHTML_EDIT);
	m_edit.InitInfoMessage(WM_EDIT_UPDATE);
	m_edit.SetContextMenu(IDR_MESSENGER_MENU,1,this);
	m_edit.SetEditMode();
	
//	m_Nick.SetWindowPos(NULL,18,43,windowRect.Width()-36,16,SWP_NOZORDER|SWP_NOACTIVATE);
	
//	m_ColorButton.SetWindowPos(NULL,17,68,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//	m_BoldButton.SetWindowPos(NULL,43,68,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//	m_ItalicButton.SetWindowPos(NULL,69,68,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//	m_UnderLineButton.SetWindowPos(NULL,94,68,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//	m_InsertSmileButton.SetWindowPos(NULL,120,68,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
	
//	m_Cancel.SetWindowPos(NULL,windowRect.Width()-91,windowRect.Height()-38,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//	m_Ok.SetWindowPos(NULL,windowRect.Width()-170,windowRect.Height()-38,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
	
//	AddAnchor(&m_Nick,CSize(0,0),CSize(100,0));
//	AddAnchor(&m_edit,CSize(0,0),CSize(100,100));
//	AddAnchor(&m_Ok,CSize(100,100),CSize(100,100));
//	AddAnchor(&m_Cancel,CSize(100,100),CSize(100,100));
	
	AddAnchor(m_Nick.GetSafeHwnd(), CSize(0, 0), CSize(100, 0));
	CRect r;
	m_frameEdit.GetWindowRect(&r);
	ScreenToClient(&r);
	m_edit.SetWindowPos(NULL, r.left, r.top, r.Width(), r.Height(), SWP_NOZORDER);
	AddAnchor(&m_edit, CSize(0, 0), CSize(100, 100));
	AddAnchor(m_SizeCombo.GetSafeHwnd(), CSize(0, 0), CSize(0, 0));
	AddAnchor(m_FontCombo.GetSafeHwnd(), CSize(0, 0), CSize(100, 0));
	AddAnchor(m_Ok.GetSafeHwnd(), CSize(100, 100), CSize(100, 100));
	AddAnchor(m_Cancel.GetSafeHwnd(), CSize(100, 100), CSize(100, 100));
	
	CString strRect = GetOptionString(IDS_OFSMESSENGER, IDS_ADD_FRIEND, _T(""));
	if(!strRect.IsEmpty())
	{
		CRect rWindow = StringToRect(strRect);
		FitRectToWindow(rWindow);
		SetWindowPos(NULL,rWindow.left,rWindow.top,rWindow.Width(),rWindow.Height(),SWP_NOZORDER|SWP_NOACTIVATE);
	}
	
	CString strFontName = GetOptionString(IDS_OFSMESSENGER,IDS_FONT,_T("Arial"));
	//int FontId = GetOptionInt(IDS_OFSMESSENGER,IDS_FONT,-1);
	//if(FontId!=-1)
	//	m_FontCombo.GetLBText(FontId, strFontName);
	CComBSTR	bsFontName = strFontName;
	
	//  [4/29/2002]
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


CComBSTR CAddUserDlg::GetMessageText()
{
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

	RemoveParagraf(bstText.m_str);

		///
	CComBSTR bstTmpText2 = (LPCWSTR)bstText;
	bstText.Empty();
	bstText = bstTmpText2;
	///

	return bstText;
}

void CAddUserDlg::OnAuthorization() 
{
	//CResizableDialog::OnOK();
	if(pMessenger->ConnectEnable()&&!bBlock)
	{
		theNet2.LockTranslator();
		Handle = pSession->AddUser(m_User.GetGlobalID(),_bstr_t(GetMessageText()),1);
		if(Handle)
		{
			Block();
			theNet2.AddToTranslator(Handle,this->GetSafeHwnd());
			pMessenger->AddUserToContactList(m_User);
		}
		theNet2.UnlockTranslator();
			
	}
	///else
	//{
	//	MessageBox("Error: No Connection","Can't add user",MB_OK|MB_ICONSTOP);
	//}
}

void CAddUserDlg::OnColorButton() 
{
	CColorDialog m_Color(m_edit.GetBgColor(),CC_ANYCOLOR,this);
	if(m_Color.DoModal()==IDOK)
	{
		m_edit.SetColor(m_Color.GetColor());
		m_edit.SetFocus();
	}
}

void CAddUserDlg::OnBoldCheck() 
{
	m_edit.SetBold();
	m_edit.SetFocus();
}

void CAddUserDlg::OnItalicCheck() 
{
    m_edit.SetItalic();
	m_edit.SetFocus();
}

void CAddUserDlg::OnUnderlineCheck() 
{
	m_edit.SetUnderline();
	m_edit.SetFocus();
}

void CAddUserDlg::UnBlock()
{
	bBlock = FALSE;
	m_Ok.EnableWindow(TRUE);
	m_edit.EnableWindow(TRUE);
}

void CAddUserDlg::Block()
{
	bBlock = TRUE;
	m_Ok.EnableWindow(FALSE);
	m_edit.EnableWindow(FALSE);
}

LRESULT CAddUserDlg::OnNetEvent(WPARAM w,LPARAM l)
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
		KillWindow();
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
		pMessenger->DeleteFromContact(m_User.GetGlobalID());
		MessageBox(GetString(IDS_ADD_USER_ERROR),GetString(IDS_ERROR_TITLE),MB_OK|MB_ICONSTOP);
		break;
	}
	
	
	delete pItem;
    return 0;
}

void CAddUserDlg::KillWindow()
{
	if(GetStyle()&WS_VISIBLE&&GetOptionInt(IDS_OFSMESSENGER,IDS_ANIMATION,FALSE))
		RoundExitAddon(this);
	CResizableDialog::OnClose();
	if(!bIsKillWinodow)
	{
		bIsKillWinodow = TRUE;
		pMessenger->SendMessage(WM_KILL_ADDUSER_MESSAGE_DLG,(WPARAM)m_User.GetGlobalID(),(LPARAM)this);
		DestroyWindow();
		delete this;
	}
	
}

HRESULT CAddUserDlg::OnEditUpdate(WPARAM w, LPARAM l)
{
	if(!bInitEdit)
	{
		bInitEdit = TRUE;

		//  [7/22/2002]
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

void CAddUserDlg::OnEditmenuCopy() 
{
	m_edit.ClipboardCopy();	
}

void CAddUserDlg::OnUpdateEditmenuCopy(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(m_edit.EnableClipboardCopy());
}

void CAddUserDlg::OnEditmenuCut() 
{
	m_edit.ClipboardCut();
}

void CAddUserDlg::OnUpdateEditmenuCut(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(m_edit.EnableClipboardCut());
}

void CAddUserDlg::OnEditmenuDelete() 
{
	m_edit.ClipboardDelete();
}

void CAddUserDlg::OnUpdateEditmenuDelete(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(m_edit.EnableClipboardDelete());
}

void CAddUserDlg::OnEditmenuPast() 
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

void CAddUserDlg::OnUpdateEditmenuPast(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(m_edit.EnableClipboardPast());
}



//DEL void CAddUserDlg::OnPaint() 
//DEL {
//DEL 	CPaintDC dc(this); // device context for painting
//DEL 	
//DEL 	CRect m_Client;
//DEL 	GetClientRect(&m_Client);
//DEL 	m_ResizeFon.Render(dc.GetSafeHdc(),m_Client.Size());
//DEL }

//DEL void CAddUserDlg::OnLButtonDown(UINT nFlags, CPoint point) 
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

//DEL BOOL CAddUserDlg::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message) 
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

void CAddUserDlg::OnCaptureChanged(CWnd *pWnd) 
{
	CRect rWindow;
	GetWindowRect(&rWindow);
	if(!IsIconic())
		WriteOptionString(IDS_OFSMESSENGER,IDS_ADD_FRIEND,RectToString(rWindow));
	
	
	CResizableDialog::OnCaptureChanged(pWnd);
}

//DEL void CAddUserDlg::OnSize(UINT nType, int cx, int cy) 
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

void CAddUserDlg::OnClickMcclose()
{
	OnCancel();
}

//DEL void CAddUserDlg::LoadSkin()
//DEL {
//DEL 	LoadSkins	m_LoadSkin;
//DEL 	
//DEL 	IStreamPtr pStream = NULL;
//DEL 	long       Error   = 0L;
//DEL 	
//DEL 	bstr_t bstrPath = IBN_SCHEMA;
//DEL 	bstrPath += (LPCTSTR)GetCurrentSkin();
//DEL 	
//DEL 	m_LoadSkin.Load(bstr_t(bstrPath+"/Shell/Add/fon.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 	{
//DEL 		LPPICTURE	Pic	=	NULL;
//DEL 		HRESULT hr = ::OleLoadPicture(pStream,	0, TRUE, IID_IPicture,(void**)&Pic);
//DEL 		if(SUCCEEDED(hr))
//DEL 		{
//DEL 			m_ResizeFon.Create(Pic);
//DEL 			m_ResizeFon.AddAnchor(CRect(0,0,100,70),CSize(0,0),CSize(0,0));
//DEL 			m_ResizeFon.AddAnchor(CRect(100,0,205,70),CSize(0,0),CSize(100,0),CResizableImage::DUPLICATE);
//DEL 			m_ResizeFon.AddAnchor(CRect(205,0,299,70),CSize(100,0),CSize(100,0));
//DEL 			
//DEL 			m_ResizeFon.AddAnchor(CRect(0,70,100,179),CSize(0,0),CSize(0,100),CResizableImage::DUPLICATE);
//DEL 			m_ResizeFon.AddAnchor(CRect(100,70,205,179),CSize(0,0),CSize(100,100),CResizableImage::DUPLICATE);
//DEL 			m_ResizeFon.AddAnchor(CRect(205,70,299,179),CSize(100,0),CSize(100,100),CResizableImage::DUPLICATE);
//DEL 			
//DEL 			m_ResizeFon.AddAnchor(CRect(0,179,100,227),CSize(0,100),CSize(0,100));
//DEL 			m_ResizeFon.AddAnchor(CRect(100,179,205,227),CSize(0,100),CSize(100,100),CResizableImage::DUPLICATE);
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
//DEL 	m_LoadSkin.Load(bstrPath+bstr_t("/Common/btn_ok.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_Ok.LoadBitmapFromStream(pStream);
//DEL 	pStream = NULL;
//DEL 
//DEL 	m_LoadSkin.Load(bstrPath+bstr_t("/Common/btn_cancel.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_Cancel.LoadBitmapFromStream(pStream);
//DEL }

void CAddUserDlg::OnClickMcmini()
{
	ShowWindow(SW_MINIMIZE);
}

void CAddUserDlg::OnClickMcOk()
{
	OnAuthorization();
}

void CAddUserDlg::OnClickMcCancel()
{
	OnCancel();
}


void CAddUserDlg::OnInsertsmileCheck() 
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

void CAddUserDlg::OnSmileItem(UINT nID)
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

//DEL BOOL CAddUserDlg::OnEraseBkgnd(CDC* pDC)
//DEL {
//DEL 	return TRUE;
//DEL }

void CAddUserDlg::OnSelendokSizeCombo() 
{
	WriteOptionInt(IDS_OFSMESSENGER,IDS_SIZE,m_SizeCombo.GetCurSel());
	m_edit.SetTextSize(m_SizeCombo.GetCurSel()+1);
	//  [4/29/2002]
	m_edit.SetDefaultFontSize(nFontSizes[m_SizeCombo.GetCurSel()]);
	//  [4/29/2002]
	m_edit.SetFocus();
}

void CAddUserDlg::OnSelendokFontCombo() 
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

BOOL CAddUserDlg::PreTranslateMessage(MSG* pMsg) 
{
	if(m_bWasCtrlExit&&m_bWasCtrlEnter)
	{
		m_bWasCtrlExit = m_bWasCtrlEnter = FALSE;
		CResizableDialog::PreTranslateMessage(pMsg);
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
				OnAuthorization();
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
				OnAuthorization();
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
				OnAuthorization();
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
				OnAuthorization();
				return TRUE;
			}
			else
			{
				pMsg->message	=	WM_KEYDOWN;
				pMsg->wParam	=	VK_RETURN;
			}
		}
	}
	return CResizableDialog::PreTranslateMessage(pMsg);
}

void CAddUserDlg::OnClickAuth() 
{
	OnAuthorization();
}

void CAddUserDlg::OnClickCancel() 
{
	OnCancel();
}
