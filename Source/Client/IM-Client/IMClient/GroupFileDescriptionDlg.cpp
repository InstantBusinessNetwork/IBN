// MessageSplitDlg.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "MainDlg.h"
#include "GroupFileDescriptionDlg.h"
#include "User.h"
#include "LoadSkins.h"
#include "CDib.h"
#include <triedcid.h>

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
// CGroupFileDescriptionDlg dialog
extern CString GetCurrentSkin ();

CGroupFileDescriptionDlg::CGroupFileDescriptionDlg(CMainDlg *pMessenger,CWnd* pParent /*=NULL*/)
	: COFSNcDlg2(CGroupFileDescriptionDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CGroupFileDescriptionDlg)
	m_strFileName = _T("");
	//}}AFX_DATA_INIT
	this->pMessenger = pMessenger;
	m_bInitEdit = FALSE;
	m_bWasCtrlEnter	=	FALSE;
	m_bWasCtrlExit	=	FALSE;
	m_strSkinSettings = _T("/Shell/GroupFile/skin.xml");
//	m_bLoadSkin = FALSE;
}

CGroupFileDescriptionDlg::~CGroupFileDescriptionDlg()
{
}


void CGroupFileDescriptionDlg::DoDataExchange(CDataExchange* pDX)
{
	COFSNcDlg2::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CGroupFileDescriptionDlg)
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
	DDX_Text(pDX, IDC_USERINFO, m_strFileName);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CGroupFileDescriptionDlg, COFSNcDlg2)
	//{{AFX_MSG_MAP(CGroupFileDescriptionDlg)
	ON_BN_CLICKED(IDOK, OnOk)
	ON_CBN_SELENDOK(IDC_FONT_COMBO, OnSelEndOkFontCombo)
	ON_CBN_SELENDOK(IDC_SIZE_COMBO, OnSelEndOkSizeCombo)
	ON_COMMAND(ID_EDITMENU_COPY, OnEditMenuCopy)
	ON_COMMAND(ID_EDITMENU_CUT, OnEditMenuCut)
	ON_COMMAND(ID_EDITMENU_DELETE, OnEditMenuDelete)
	ON_COMMAND(ID_EDITMENU_PAST, OnEditMenuPaste)
	ON_UPDATE_COMMAND_UI(ID_EDITMENU_COPY, OnUpdateEditMenuCopy)
	ON_UPDATE_COMMAND_UI(ID_EDITMENU_CUT, OnUpdateEditMenuCut)
	ON_UPDATE_COMMAND_UI(ID_EDITMENU_DELETE, OnUpdateEditMenuDelete)
	ON_UPDATE_COMMAND_UI(ID_EDITMENU_PAST, OnUpdateEditMenuPaste)
	ON_WM_CAPTURECHANGED()
	ON_WM_MOVE()
	ON_WM_SIZE()
	ON_WM_DROPFILES()
	ON_MESSAGE(WM_EDIT_UPDATE,OnEditUpdate)
	ON_WM_CREATE()
	//}}AFX_MSG_MAP
	ON_MESSAGE(WM_SWM_SETBODY,OnSWMSetBody)
	ON_COMMAND_RANGE(20000,20000+SmileBuffSize,OnSmileItem)
END_MESSAGE_MAP()


/////////////////////////////////////////////////////////////////////////////
// CGroupFileDescriptionDlg message handlers

void CGroupFileDescriptionDlg::OnOk() 
{
}

void CGroupFileDescriptionDlg::OnCancel() 
{
	COFSNcDlg2::OnCancel();
}

BOOL CGroupFileDescriptionDlg::OnInitDialog() 
{
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
	
	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),FALSE);
	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),TRUE);
	
	/// Create Font ...
//	m_font.Attach(GetStockObject(DEFAULT_GUI_FONT));

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
	
	m_bInitEdit = FALSE;
	
	//  [4/29/2002]
	CString strFontName = GetOptionString(IDS_OFSMESSENGER,IDS_FONT,_T("Arial"));
	//int FontId = GetOptionInt(IDS_OFSMESSENGER,IDS_FONT,-1);
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
	
	SetRecipientGroup(m_strRecepientGroupName);
	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

//DEL void CGroupFileDescriptionDlg::OnSendButton() 
//DEL {
//DEL 	UpdateData(TRUE);
//DEL 	
//DEL 	if(!EnableRecepients())
//DEL 		return;
//DEL 
//DEL 	COFSNcDlg2::OnOK();
//DEL }

void CGroupFileDescriptionDlg::SetRecipientGroup(LPCTSTR strName)
{
	m_strRecepientGroupName = strName;

	CString strCaption;
//	GetWindowText(strCaption);
//	strCaption += " with ";
//	strCaption += strName;
//	SetWindowText(strCaption);
	
//	strCaption = "To: ";
//	strCaption += strName;
	m_UserInfo.SetText(m_strFileName);
	
	m_ContactList.Clear();
	
	pMessenger->GetCopyContactList(m_ContactList);
	
	// Step 2. Init Check or Uncheck Mode [2/6/2002]
	if(m_ContactList.InitIteration())
	{
		CUser* pUser=NULL;
		
		if(POSITION pos = m_ContactList.InitIteration())
		{
			for(int i=0; m_ContactList.GetNext(pos,pUser); i++)
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
		}
	}

	BuildTree();
}


void CGroupFileDescriptionDlg::OnCaptureChanged(CWnd *pWnd) 
{
	CRect rWindow;
	GetWindowRect(&rWindow);
	if(!IsIconic())
		WriteOptionString(IDS_OFSMESSENGER,IDS_GROUPSENDFILE,RectToString(rWindow));

	COFSNcDlg2::OnCaptureChanged(pWnd);
}


void CGroupFileDescriptionDlg::OnMove(int x, int y) 
{
	COFSNcDlg2::OnMove(x, y);
	
	if(::IsWindow(GetParent()->GetSafeHwnd()))
		GetParent()->UpdateWindow();
	if(::IsWindow(pMessenger->GetMessageParent()->GetSafeHwnd()))
		pMessenger->GetMessageParent()->UpdateWindow();
}

void CGroupFileDescriptionDlg::SetFon(HBITMAP hFon)
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

	m_ResizeFon.AddAnchor(CRect(0,0,223,100),CSize(0,0),CSize(0,0));
	m_ResizeFon.AddAnchor(CRect(223,0,277,100),CSize(0,0),CSize(100,0),CResizableImage::DUPLICATE);
	m_ResizeFon.AddAnchor(CRect(277,0,299,166),CSize(100,0),CSize(100,0));

	m_ResizeFon.AddAnchor(CRect(0,100,223,165),CSize(0,0),CSize(0,100),CResizableImage::DUPLICATE);
	m_ResizeFon.AddAnchor(CRect(223,100,277,165),CSize(0,0),CSize(100,100),CResizableImage::DUPLICATE);
	m_ResizeFon.AddAnchor(CRect(277,100,299,165),CSize(100,0),CSize(100,100),CResizableImage::DUPLICATE);

	m_ResizeFon.AddAnchor(CRect(0,165,223,199),CSize(0,100),CSize(0,100));
	m_ResizeFon.AddAnchor(CRect(223,165,277,199),CSize(0,100),CSize(100,100),CResizableImage::DUPLICATE);
	m_ResizeFon.AddAnchor(CRect(277,165,299,199),CSize(100,100),CSize(100,100));

	CRect rgnRect;
	GetWindowRect(&rgnRect);
	CRgn	WinRgn;
	WinRgn.CreateRoundRectRgn(0,0,rgnRect.Width(),rgnRect.Height(),20,20);
	SetWindowRgn(WinRgn,TRUE);
*/
	
	CString strRect = GetOptionString(IDS_OFSMESSENGER, IDS_GROUPSENDFILE, _T(""));
	if(!strRect.IsEmpty())
	{
		CRect rWindow = StringToRect(strRect);
		FitRectToWindow(rWindow);
		SetWindowPos(NULL,rWindow.left,rWindow.top,rWindow.Width(),rWindow.Height(),SWP_NOZORDER|SWP_NOACTIVATE);
	}
	
}

//DEL void CGroupFileDescriptionDlg::OnPaint() 
//DEL {
//DEL 	CRect	winRect, editRect;
//DEL 	GetWindowRect(winRect);
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
//DEL 		m_FileName.Invalidate();
//DEL 	}
//DEL 	// Do not call COFSNcDlg2::OnPaint() for painting messages
//DEL }

//DEL void CGroupFileDescriptionDlg::OnLButtonDown(UINT nFlags, CPoint point) 
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

//DEL BOOL CGroupFileDescriptionDlg::OnEraseBkgnd( CDC* pDC )
//DEL {
//DEL 	return TRUE;
//DEL }

BEGIN_EVENTSINK_MAP(CGroupFileDescriptionDlg, COFSNcDlg2)
    //{{AFX_EVENTSINK_MAP(CGroupFileDescriptionDlg)
	ON_EVENT(CGroupFileDescriptionDlg, IDC_CCOOTREECTRL, 3 /* Action */, OnActionCcootreectrl, VTS_I4 VTS_BOOL)
	ON_EVENT(CGroupFileDescriptionDlg, IDC_CCOOTREECTRL, 4 /* DoDrop */, OnDoDropCcootreectrl, VTS_I4 VTS_BOOL VTS_UNKNOWN VTS_I4 VTS_I4)
	ON_EVENT(CGroupFileDescriptionDlg, IDC_CCOOTREECTRL, 1 /* Menu */, OnMenuCcootreectrl, VTS_I4 VTS_BOOL)
	ON_EVENT(CGroupFileDescriptionDlg, IDC_CCOOTREECTRL, 2 /* Select */, OnSelectCcootreectrl, VTS_I4 VTS_BOOL)
	ON_EVENT(CGroupFileDescriptionDlg, IDC_BTN_X, -600 /* Click */, OnClickBtnX, VTS_NONE)
	ON_EVENT(CGroupFileDescriptionDlg, IDC_BTN_MIN, -600 /* Click */, OnClickBtnMin, VTS_NONE)
	ON_EVENT(CGroupFileDescriptionDlg, IDC_BTN_OPTIONS, -600 /* Click */, OnClickBtnOptions, VTS_NONE)
	ON_EVENT(CGroupFileDescriptionDlg, IDC_BTN_MENU, -600 /* Click */, OnClickBtnMenu, VTS_NONE)
	ON_EVENT(CGroupFileDescriptionDlg, IDC_BTN_SELECT_ALL, -600 /* Click */, OnClickBtnSelectAll, VTS_NONE)
	ON_EVENT(CGroupFileDescriptionDlg, IDC_BTN_SELECT_NONE, -600 /* Click */, OnClickBtnSelectNone, VTS_NONE)
	ON_EVENT(CGroupFileDescriptionDlg, IDC_BTN_COLOR, -600 /* Click */, OnClickBtnColor, VTS_NONE)
	ON_EVENT(CGroupFileDescriptionDlg, IDC_BTN_BOLD, -600 /* Click */, OnClickBtnBold, VTS_NONE)
	ON_EVENT(CGroupFileDescriptionDlg, IDC_BTN_ITALIC, -600 /* Click */, OnClickBtnItalic, VTS_NONE)
	ON_EVENT(CGroupFileDescriptionDlg, IDC_BTN_UNDERLINE, -600 /* Click */, OnClickBtnUnderline, VTS_NONE)
	ON_EVENT(CGroupFileDescriptionDlg, IDC_BTN_SMILES, -600 /* Click */, OnClickBtnSmiles, VTS_NONE)
	ON_EVENT(CGroupFileDescriptionDlg, IDC_BTN_SEND, -600 /* Click */, OnClickBtnSend, VTS_NONE)
	//}}AFX_EVENTSINK_MAP
END_EVENTSINK_MAP()

//DEL void CGroupFileDescriptionDlg::OnClickMcclose() 
//DEL {
//DEL 	OnCancel();
//DEL }

//DEL void CGroupFileDescriptionDlg::OnClickMcsend() 
//DEL {
//DEL 	OnSendButton();
//DEL }

void CGroupFileDescriptionDlg::OnSize(UINT nType, int cx, int cy) 
{
	COFSNcDlg2::OnSize(nType, cx, cy);
	
	m_edit.SetFocus();
}

//DEL BOOL CGroupFileDescriptionDlg::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message) 
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

//DEL void CGroupFileDescriptionDlg::OnLButtonUp(UINT nFlags, CPoint point) 
//DEL {
//DEL 	COFSNcDlg2::OnLButtonUp(nFlags, point);
//DEL }

void CGroupFileDescriptionDlg::CreateTree()
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

void CGroupFileDescriptionDlg::BuildTree()
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
				for(int i=0; m_ContactList.GetNext(pos, pUser); i++)
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
				for(int i=0; m_ContactList.GetNext(pos, pUser); i++)
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

void CGroupFileDescriptionDlg::OnActionCcootreectrl(long TID, BOOL bGroupe) 
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
					
					while(m_ContactList.GetNext(pos, pUser))
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

void CGroupFileDescriptionDlg::OnDoDropCcootreectrl(long TID, BOOL bGroupe, LPUNKNOWN pDataObject, long DropEffect, long LastKeyState) 
{
	// TODO: Add your control notification handler code here
	
}

void CGroupFileDescriptionDlg::OnMenuCcootreectrl(long TID, BOOL bGroupe) 
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

void CGroupFileDescriptionDlg::OnSelectCcootreectrl(long TID, BOOL bGroupe) 
{
}

void CGroupFileDescriptionDlg::UpdateID(long UserId)
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

CUser* CGroupFileDescriptionDlg::FindUserInVisualContactList(long TID)
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

//DEL void CGroupFileDescriptionDlg::OnClickMcselectAll()
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

BOOL CGroupFileDescriptionDlg::EnableRecepients()
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

//DEL void CGroupFileDescriptionDlg::LoadSkin()
//DEL {
//DEL 	///////////////////////////////////////////////////////////////////////
//DEL 	LoadSkins m_Load;
//DEL 	IStreamPtr pStream = NULL;
//DEL 	long Error = 0;
//DEL 	bstr_t Path = bstr_t("IBN_SCHEMA://") + (LPCTSTR)GetCurrentSkin();
//DEL 	m_Load.Load(Path+bstr_t("/Shell/GroupFile/fon.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 	{
//DEL 		CDib m_Dib(pStream);
//DEL 		CPaintDC dc(this);
//DEL 		SetFon(m_Dib.GetHBITMAP(dc));
//DEL 	}
//DEL 	pStream = NULL;
//DEL 
//DEL 	m_Load.Load(Path+bstr_t("/Common/btn_x.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_Close.LoadBitmapFromStream(pStream);
//DEL 	pStream = NULL;
//DEL 	
//DEL 	m_Load.Load(Path+bstr_t("/Common/btn_send.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_Send.LoadBitmapFromStream(pStream);
//DEL 	pStream = NULL;
//DEL 	
//DEL 	
//DEL 	m_Load.Load(Path+bstr_t("/Shell/GroupSend/selectall.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_SelectAll.LoadBitmapFromStream(pStream);
//DEL 	pStream = NULL;
//DEL 	///////////////////////////////////////////////////////////////////////
//DEL }

void CGroupFileDescriptionDlg::UpdateGroupCheck()
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

void CGroupFileDescriptionDlg::UpdateGroupID(LPCTSTR strName)
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

//DEL void CGroupFileDescriptionDlg::OnClickSend() 
//DEL {
//DEL 	OnSendButton();
//DEL }

//DEL void CGroupFileDescriptionDlg::OnClickSelectAll() 
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

//DEL void CGroupFileDescriptionDlg::OnMcunselectall() 
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

void CGroupFileDescriptionDlg::LoadSkin(IXMLDOMNode *pXmlRoot)
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

void CGroupFileDescriptionDlg::OnEditMenuCopy() 
{
	m_edit.ClipboardCopy();	
}

void CGroupFileDescriptionDlg::OnEditMenuCut() 
{
	m_edit.ClipboardCut();
}

void CGroupFileDescriptionDlg::OnEditMenuDelete() 
{
	m_edit.ClipboardDelete();
}

void CGroupFileDescriptionDlg::OnEditMenuPaste() 
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

void CGroupFileDescriptionDlg::OnUpdateEditMenuCopy(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(m_edit.EnableClipboardCopy());
}

void CGroupFileDescriptionDlg::OnUpdateEditMenuCut(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(m_edit.EnableClipboardCut());
}

void CGroupFileDescriptionDlg::OnUpdateEditMenuDelete(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(m_edit.EnableClipboardDelete());
}

void CGroupFileDescriptionDlg::OnUpdateEditMenuPaste(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(m_edit.EnableClipboardPast());
}

void CGroupFileDescriptionDlg::SetBody(LPCTSTR strBody)
{
	m_strSetBody = strBody;
	m_edit.InsertTEXT(CComBSTR(_T("")));
	m_edit.SetFocus();
}

LPARAM CGroupFileDescriptionDlg::OnSWMSetBody(WPARAM w, LPARAM l)
{
	//	if(!Handle)
	{
		SetBody(LPCTSTR(w));
	}
	return 0;
}

CString CGroupFileDescriptionDlg::GetFileDescription()
{
	CComBSTR bsText;
	CString Result;
	
	m_edit.GetTXT(&bsText);
	if(bsText.Length() != 0)
	{
		//RemoveParagraf(bsText.m_str);
		Result = CString(bsText.m_str).Left(400);
	}
	return Result;
}

void CGroupFileDescriptionDlg::OnSelEndOkSizeCombo() 
{
	SetFontSize();
}

void CGroupFileDescriptionDlg::OnSelEndOkFontCombo() 
{
	SetFontFace();
}

void CGroupFileDescriptionDlg::OnSmileItem(UINT nID)
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

void CGroupFileDescriptionDlg::SelectColor()
{
	CColorDialog m_Color(m_edit.GetColor(), CC_ANYCOLOR,this);
	if(m_Color.DoModal()==IDOK)
	{
		m_edit.SetColor(m_Color.GetColor());
	}
	m_edit.SetFocus();
}

void CGroupFileDescriptionDlg::InsertSmile()
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

void CGroupFileDescriptionDlg::SetFontBold()
{
	m_edit.SetBold();
	m_edit.SetFocus();
	WriteOptionInt(IDS_OFSMESSENGER,IDS_BOLD, m_btnBold.GetPressed());
}

void CGroupFileDescriptionDlg::SetFontItalic()
{
    m_edit.SetItalic();
	m_edit.SetFocus();
	WriteOptionInt(IDS_OFSMESSENGER,IDS_ITALIC,m_btnItalic.GetPressed());
}

void CGroupFileDescriptionDlg::SetFontUnderline()
{
	m_edit.SetUnderline();
	m_edit.SetFocus();
	WriteOptionInt(IDS_OFSMESSENGER,IDS_UNDERLINE,m_btnUnderline.GetPressed());
}

void CGroupFileDescriptionDlg::SetFontSize()
{
	WriteOptionInt(IDS_OFSMESSENGER,IDS_SIZE,m_SizeCombo.GetCurSel());
	m_edit.SetTextSize(m_SizeCombo.GetCurSel()+1);
	//  [4/29/2002]
	m_edit.SetDefaultFontSize(nFontSizes[m_SizeCombo.GetCurSel()]);
	//  [4/29/2002]
	m_edit.SetFocus();
}

void CGroupFileDescriptionDlg::SetFontFace()
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

HRESULT CGroupFileDescriptionDlg::OnEditUpdate(WPARAM w, LPARAM l)
{
	if(!m_strSetBody.IsEmpty())
	{
		CComBSTR	bsSetBody = m_strSetBody;
		m_edit.InsertHTML(bsSetBody);
		m_strSetBody.Empty();
	}
	
	if(!m_bInitEdit)
	{
		m_bInitEdit = TRUE;
		
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
			m_edit.SetUnderline();
		}
		if(GetOptionInt(IDS_OFSMESSENGER,IDS_ITALIC,0))
		{
			m_edit.SetItalic();
		}
		if(GetOptionInt(IDS_OFSMESSENGER,IDS_BOLD,0))
		{
			m_edit.SetBold();
		}
		
	}
	
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

void CGroupFileDescriptionDlg::OnClickBtnX() 
{
	OnCancel();
}

void CGroupFileDescriptionDlg::OnClickBtnMin() 
{
	ShowWindow(SW_MINIMIZE);
}

void CGroupFileDescriptionDlg::OnClickBtnOptions() 
{
	pMessenger->PreferenceDlg(this);
}

void CGroupFileDescriptionDlg::OnClickBtnMenu() 
{
}

void CGroupFileDescriptionDlg::OnClickBtnSelectAll() 
{
	SelectAll(TRUE);
}

void CGroupFileDescriptionDlg::OnClickBtnSelectNone() 
{
	SelectAll(FALSE);
}

void CGroupFileDescriptionDlg::OnClickBtnColor() 
{
	SelectColor();
}

void CGroupFileDescriptionDlg::OnClickBtnBold() 
{
	SetFontBold();
}

void CGroupFileDescriptionDlg::OnClickBtnItalic() 
{
	SetFontItalic();
}

void CGroupFileDescriptionDlg::OnClickBtnUnderline() 
{
	SetFontUnderline();
}

void CGroupFileDescriptionDlg::OnClickBtnSmiles() 
{
	InsertSmile();
}

void CGroupFileDescriptionDlg::OnClickBtnSend() 
{
	Send();
}

void CGroupFileDescriptionDlg::SelectAll(BOOL bSelect)
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

void CGroupFileDescriptionDlg::Send()
{
	UpdateData(TRUE);
	
	if(!EnableRecepients())
		return;
	
	COFSNcDlg2::OnOK();
}

int CGroupFileDescriptionDlg::OnCreate(LPCREATESTRUCT lpCreateStruct) 
{
	if (COFSNcDlg2::OnCreate(lpCreateStruct) == -1)
		return -1;
	
	CRect r(1,2,3,4);
	m_edit.Create(NULL, NULL, WS_VISIBLE, r, this, IDC_DHTML_EDIT);
	
	return 0;
}

BOOL CGroupFileDescriptionDlg::PreTranslateMessage(MSG* pMsg) 
{
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
			m_bWasCtrlEnter)
		{
			//m_bWasCtrlEnter = FALSE;
			m_bWasCtrlExit  = TRUE;
		}
		else
			if(pMsg->message==WM_KEYDOWN||pMsg->message==WM_SYSKEYDOWN)
			{
				//TRACE("\r\n WM_KEYDOWN wParam = 0x%X",pMsg->wParam);
				if(pMsg->wParam==VK_RETURN&&(GetKeyState(VK_CONTROL)>>1))
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
				else if(pMsg->wParam==VK_RETURN&&(GetKeyState(VK_SHIFT)>>1))
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
				else if(pMsg->wParam==VK_RETURN)
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
				else if(pMsg->wParam==0x53&&(GetKeyState(VK_MENU)>>1))
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
