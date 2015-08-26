// MessageSplitDlg.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "MainDlg.h"
#include "GroupAssingToDo.h"
#include "User.h"
#include "LoadSkins.h"
#include "CDib.h"
#include <triedcid.h>


#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CGroupAssignToDoDlg dialog
extern CString GetCurrentSkin ();

CGroupAssignToDoDlg::CGroupAssignToDoDlg(CMainDlg *pMessenger,CWnd* pParent /*=NULL*/)
	: COFSNcDlg2(CGroupAssignToDoDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CGroupAssignToDoDlg)
	//}}AFX_DATA_INIT
	this->pMessenger = pMessenger;
	m_bWasCtrlEnter	=	FALSE;
	m_bWasCtrlExit	=	FALSE;
	m_strSkinSettings = _T("/Shell/GroupAssignToDo/skin.xml");
}

CGroupAssignToDoDlg::~CGroupAssignToDoDlg()
{
}


void CGroupAssignToDoDlg::DoDataExchange(CDataExchange* pDX)
{
	COFSNcDlg2::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CGroupAssignToDoDlg)
	DDX_Control(pDX, IDC_USERINFO, m_UserInfo);
	DDX_Control(pDX, IDC_CCOOTREECTRL, m_treectrl);
	DDX_Control(pDX, IDC_BTN_MENU, m_btnMenu);
	DDX_Control(pDX, IDC_BTN_MIN, m_btnMin);
	DDX_Control(pDX, IDC_BTN_OPTIONS, m_btnOptions);
	DDX_Control(pDX, IDC_BTN_SELECT_ALL, m_btnSelectAll);
	DDX_Control(pDX, IDC_BTN_SELECT_NONE, m_btnSelectNone);
	DDX_Control(pDX, IDC_BTN_SEND, m_btnSend);
	DDX_Control(pDX, IDC_BTN_X, m_btnX);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CGroupAssignToDoDlg, COFSNcDlg2)
	//{{AFX_MSG_MAP(CGroupAssignToDoDlg)
	ON_BN_CLICKED(IDOK, OnOk)
	ON_WM_CAPTURECHANGED()
	ON_WM_MOVE()
	ON_WM_SIZE()
	ON_WM_DROPFILES()
	ON_WM_CREATE()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()


/////////////////////////////////////////////////////////////////////////////
// CGroupAssignToDoDlg message handlers

void CGroupAssignToDoDlg::OnOk() 
{
}

void CGroupAssignToDoDlg::OnCancel() 
{
	COFSNcDlg2::OnCancel();
}

BOOL CGroupAssignToDoDlg::OnInitDialog() 
{
	COFSNcDlg2::OnInitDialog();
	
	//////////////////////////////////////////////////////////////////////////
	// McToolTipAddon
	m_ToolTip.AddTool(&m_btnX,IDS_TIP_CLOSE);
	m_ToolTip.AddTool(&m_btnMin,IDS_TIP_MINIMIZE);
	//////////////////////////////////////////////////////////////////////////
	
	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),FALSE);
	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),TRUE);
	
	/// Create Font ...

	CreateTree();
	
	SetRecipientGroup(m_strRecepientGroupName);
	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

void CGroupAssignToDoDlg::SetRecipientGroup(LPCTSTR strName)
{
	m_strRecepientGroupName = strName;

	CString strCaption;
	
	m_UserInfo.SetText(m_strRecepientGroupName);
	
	m_ContactList.Clear();
	
	pMessenger->GetCopyContactList(m_ContactList);
	
	// Step 2. Init Check or Uncheck Mode [2/6/2002]
	if(m_ContactList.InitIteration())
	{
		CUser* pUser=NULL;
		
		if(POSITION pos = m_ContactList.InitIteration())
		{
			int CLMode  = GetOptionInt(IDS_OFSMESSENGER,IDS_CLMODE,2);

			for(int i=0; m_ContactList.GetNext(pos,pUser); i++)
			{
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


void CGroupAssignToDoDlg::OnCaptureChanged(CWnd *pWnd) 
{
	CRect rWindow;
	GetWindowRect(&rWindow);
	if(!IsIconic())
		WriteOptionString(IDS_OFSMESSENGER,IDS_GROUPASSIGNTODO,RectToString(rWindow));

	COFSNcDlg2::OnCaptureChanged(pWnd);
}


void CGroupAssignToDoDlg::OnMove(int x, int y) 
{
	COFSNcDlg2::OnMove(x, y);
	
	if(::IsWindow(GetParent()->GetSafeHwnd()))
		GetParent()->UpdateWindow();
	if(::IsWindow(pMessenger->GetMessageParent()->GetSafeHwnd()))
		pMessenger->GetMessageParent()->UpdateWindow();
}

void CGroupAssignToDoDlg::SetFon(HBITMAP hFon)
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
	
	CString strRect = GetOptionString(IDS_OFSMESSENGER, IDS_GROUPASSIGNTODO, _T(""));
	if(!strRect.IsEmpty())
	{
		CRect rWindow = StringToRect(strRect);
		FitRectToWindow(rWindow);
		SetWindowPos(NULL,rWindow.left,rWindow.top,rWindow.Width(),rWindow.Height(),SWP_NOZORDER|SWP_NOACTIVATE);
	}
	
}

//DEL void CGroupAssignToDoDlg::OnPaint() 
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

//DEL void CGroupAssignToDoDlg::OnLButtonDown(UINT nFlags, CPoint point) 
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

//DEL BOOL CGroupAssignToDoDlg::OnEraseBkgnd( CDC* pDC )
//DEL {
//DEL 	return TRUE;
//DEL }

BEGIN_EVENTSINK_MAP(CGroupAssignToDoDlg, COFSNcDlg2)
    //{{AFX_EVENTSINK_MAP(CGroupAssignToDoDlg)
	ON_EVENT(CGroupAssignToDoDlg, IDC_CCOOTREECTRL, 3 /* Action */, OnActionCcootreectrl, VTS_I4 VTS_BOOL)
	ON_EVENT(CGroupAssignToDoDlg, IDC_CCOOTREECTRL, 4 /* DoDrop */, OnDoDropCcootreectrl, VTS_I4 VTS_BOOL VTS_UNKNOWN VTS_I4 VTS_I4)
	ON_EVENT(CGroupAssignToDoDlg, IDC_CCOOTREECTRL, 1 /* Menu */, OnMenuCcootreectrl, VTS_I4 VTS_BOOL)
	ON_EVENT(CGroupAssignToDoDlg, IDC_CCOOTREECTRL, 2 /* Select */, OnSelectCcootreectrl, VTS_I4 VTS_BOOL)
	ON_EVENT(CGroupAssignToDoDlg, IDC_BTN_X, -600 /* Click */, OnClickBtnX, VTS_NONE)
	ON_EVENT(CGroupAssignToDoDlg, IDC_BTN_MIN, -600 /* Click */, OnClickBtnMin, VTS_NONE)
	ON_EVENT(CGroupAssignToDoDlg, IDC_BTN_OPTIONS, -600 /* Click */, OnClickBtnOptions, VTS_NONE)
	ON_EVENT(CGroupAssignToDoDlg, IDC_BTN_MENU, -600 /* Click */, OnClickBtnMenu, VTS_NONE)
	ON_EVENT(CGroupAssignToDoDlg, IDC_BTN_SELECT_ALL, -600 /* Click */, OnClickBtnSelectAll, VTS_NONE)
	ON_EVENT(CGroupAssignToDoDlg, IDC_BTN_SELECT_NONE, -600 /* Click */, OnClickBtnSelectNone, VTS_NONE)
	ON_EVENT(CGroupAssignToDoDlg, IDC_BTN_SEND, -600 /* Click */, OnClickBtnSend, VTS_NONE)
	//}}AFX_EVENTSINK_MAP
END_EVENTSINK_MAP()

//DEL void CGroupAssignToDoDlg::OnClickMcclose() 
//DEL {
//DEL 	OnCancel();
//DEL }

//DEL void CGroupAssignToDoDlg::OnClickMcsend() 
//DEL {
//DEL 	OnSendButton();
//DEL }

void CGroupAssignToDoDlg::OnSize(UINT nType, int cx, int cy) 
{
	COFSNcDlg2::OnSize(nType, cx, cy);
}

//DEL BOOL CGroupAssignToDoDlg::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message) 
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

//DEL void CGroupAssignToDoDlg::OnLButtonUp(UINT nFlags, CPoint point) 
//DEL {
//DEL 	COFSNcDlg2::OnLButtonUp(nFlags, point);
//DEL }

void CGroupAssignToDoDlg::CreateTree()
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

void CGroupAssignToDoDlg::BuildTree()
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

void CGroupAssignToDoDlg::OnActionCcootreectrl(long TID, BOOL bGroupe) 
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
					
					int CLMode  = GetOptionInt(IDS_OFSMESSENGER,IDS_CLMODE,2);

					while(m_ContactList.GetNext(pos, pUser))
					{
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

void CGroupAssignToDoDlg::OnDoDropCcootreectrl(long TID, BOOL bGroupe, LPUNKNOWN pDataObject, long DropEffect, long LastKeyState) 
{
	// TODO: Add your control notification handler code here
	
}

void CGroupAssignToDoDlg::OnMenuCcootreectrl(long TID, BOOL bGroupe) 
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

void CGroupAssignToDoDlg::OnSelectCcootreectrl(long TID, BOOL bGroupe) 
{
}

void CGroupAssignToDoDlg::UpdateID(long UserId)
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

CUser* CGroupAssignToDoDlg::FindUserInVisualContactList(long TID)
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

//DEL void CGroupAssignToDoDlg::OnClickMcselectAll()
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

BOOL CGroupAssignToDoDlg::EnableRecepients()
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

//DEL void CGroupAssignToDoDlg::LoadSkin()
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

void CGroupAssignToDoDlg::UpdateGroupCheck()
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

void CGroupAssignToDoDlg::UpdateGroupID(LPCTSTR strName)
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

//DEL void CGroupAssignToDoDlg::OnClickSend() 
//DEL {
//DEL 	OnSendButton();
//DEL }

//DEL void CGroupAssignToDoDlg::OnClickSelectAll() 
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

//DEL void CGroupAssignToDoDlg::OnMcunselectall() 
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

void CGroupAssignToDoDlg::LoadSkin(IXMLDOMNode *pXmlRoot)
{
	m_btnMenu.ShowWindow(SW_HIDE);
	m_btnOptions.ShowWindow(SW_HIDE);
	m_btnMin.ShowWindow(SW_HIDE);
	m_btnX.ShowWindow(SW_HIDE);
	
	m_btnSelectAll.ShowWindow(SW_HIDE);
	m_btnSelectNone.ShowWindow(SW_HIDE);
	
	m_btnSend.ShowWindow(SW_HIDE);
	
	LoadButton(pXmlRoot, _T("Menu"), &m_btnMenu, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Options"), &m_btnOptions, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Minimize"), &m_btnMin, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("X"), &m_btnX, TRUE, FALSE);
	
	LoadButton(pXmlRoot, _T("SelectAll"), &m_btnSelectAll, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("SelectNone"), &m_btnSelectNone, TRUE, FALSE);
	
	LoadButton(pXmlRoot, _T("Send"), &m_btnSend, TRUE, FALSE);

	LoadLabel(pXmlRoot, _T("Recipient"), &m_UserInfo, TRUE);

	LoadRectangle(pXmlRoot, _T("Users"), &m_treectrl, TRUE);

}




void CGroupAssignToDoDlg::OnClickBtnX() 
{
	OnCancel();
}

void CGroupAssignToDoDlg::OnClickBtnMin() 
{
	ShowWindow(SW_MINIMIZE);
}

void CGroupAssignToDoDlg::OnClickBtnOptions() 
{
	pMessenger->PreferenceDlg(this);
}

void CGroupAssignToDoDlg::OnClickBtnMenu() 
{
}

void CGroupAssignToDoDlg::OnClickBtnSelectAll() 
{
	SelectAll(TRUE);
}

void CGroupAssignToDoDlg::OnClickBtnSelectNone() 
{
	SelectAll(FALSE);
}

void CGroupAssignToDoDlg::OnClickBtnSend() 
{
	Send();
}

void CGroupAssignToDoDlg::SelectAll(BOOL bSelect)
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

void CGroupAssignToDoDlg::Send()
{
	UpdateData(TRUE);
	
	if(!EnableRecepients())
		return;
	
	COFSNcDlg2::OnOK();
}

int CGroupAssignToDoDlg::OnCreate(LPCREATESTRUCT lpCreateStruct) 
{
	if (COFSNcDlg2::OnCreate(lpCreateStruct) == -1)
		return -1;
	
	return 0;
}

BOOL CGroupAssignToDoDlg::PreTranslateMessage(MSG* pMsg) 
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
