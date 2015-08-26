// ScreenShotDlg.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "MainDlg.h"
#include "ScreenShotDlg.h"
#include "resource.h"

#include "LoadSkins.h"
#include "cdib.h"

#include "ColorMenu.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CScreenShotDlg dialog
double	ZoomValues[]	=	{0.25,0.33333,0.5,0.6667,1.0,2.0,3.0,4.0,5.0,6.0};

CScreenShotDlg::CScreenShotDlg(CMainDlg *pMessenger, CScreenShotDlg::DlgMode Mode, CWnd* pParent /*=NULL*/)
	: COFSNcDlg2(CScreenShotDlg::IDD, pParent)
{
	EnableAutomation();
	
	SetBoundary(0,0);
	SetBoundaryColor(RGB(0,0,0));
	SetCaption(COLORREF(0), COLORREF(0), 0);

	m_strSkinSettings = _T("/Shell/ScreenShot/skin.xml");
	
	//{{AFX_DATA_INIT(CScreenShotDlg)
	//}}AFX_DATA_INIT
	bIsKillWinodow	=	FALSE;

	this->pMessenger = pMessenger;
	m_bWasCtrlEnter	=	FALSE;
	m_bWasCtrlExit	=	FALSE;

	m_Mode = Mode;

	m_RecepientUserId = 0;

	m_iZoomValueIndex = 4;
}


void CScreenShotDlg::DoDataExchange(CDataExchange* pDX)
{
	COFSNcDlg2::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CScreenShotDlg)
	//DDX_Control(pDX, IDC_IMAGE_STATIC, m_ImageStatic);
	DDX_Control(pDX, IDC_MCCLOSE, m_btnX);
	DDX_Control(pDX, IDC_MCMINI, m_btnMin);
	DDX_Control(pDX, IDC_MCMAXI, m_btnMax);
	DDX_Control(pDX, IDC_MCMAXIMINI, m_btnRestore);
	DDX_Control(pDX, IDC_MCMENU, m_Menu);
	DDX_Control(pDX, IDC_MCSAVE, m_btnSave);
	DDX_Control(pDX, IDC_MCUNDO, m_btnUndo);
	DDX_Control(pDX, IDC_MCREDO, m_btnRedo);
	DDX_Control(pDX, IDC_MCSCROLL, m_btnScroll);
	DDX_Control(pDX, IDC_MCCROP, m_btnCrop);
	DDX_Control(pDX, IDC_MCZOOMOUT, m_btnZoomOut);
	DDX_Control(pDX, IDC_MCZOOMIN, m_btnZoomIn);
	DDX_Control(pDX, IDC_MCTEXT, m_btnText);
	DDX_Control(pDX, IDC_MCPEN, m_btnPen);
	DDX_Control(pDX, IDC_MCSEND, m_btnSend);
	DDX_Control(pDX, IDC_USER_LIST, m_treectrl);
	DDX_Control(pDX, IDC_MCPENCOLOR,m_btnPenColor);

	DDX_Control(pDX, IDC_MCCHECKSEND,m_checkSend);
	DDX_Control(pDX, IDC_MCCHECKISSUE,m_checkIssue);
	DDX_Control(pDX, IDC_MCCHECKTODO,m_checkToDo);
	DDX_Control(pDX, IDC_MCCHECKPUBLISH,m_checkPublish);
	
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CScreenShotDlg, COFSNcDlg2)
	//{{AFX_MSG_MAP(CScreenShotDlg)
	ON_WM_CAPTURECHANGED()
	ON_MESSAGE(WM_UNDO_UPDATED,OnUndoUpdated)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

BEGIN_EVENTSINK_MAP(CScreenShotDlg, COFSNcDlg2)
//{{AFX_EVENTSINK_MAP(CChatDlg)
ON_EVENT(CScreenShotDlg, IDC_MCCLOSE, -600 , OnClickMcclose, VTS_NONE)
ON_EVENT(CScreenShotDlg, IDC_MCMINI,-600 , OnClickMcmini, VTS_NONE)
ON_EVENT(CScreenShotDlg, IDC_MCMAXI,-600 , OnClickMcmaxi, VTS_NONE)
ON_EVENT(CScreenShotDlg, IDC_MCMAXIMINI,-600 , OnClickMcmaximini, VTS_NONE)
ON_EVENT(CScreenShotDlg, IDC_MCMENU, -600 , OnClickMcmenu, VTS_NONE)

ON_EVENT(CScreenShotDlg, IDC_MCSAVE, -600 , OnClickMcsave, VTS_NONE)
ON_EVENT(CScreenShotDlg, IDC_MCUNDO, -600 , OnClickMcundo, VTS_NONE)
ON_EVENT(CScreenShotDlg, IDC_MCREDO, -600 , OnClickMcredo, VTS_NONE)
ON_EVENT(CScreenShotDlg, IDC_MCSCROLL, -600 , OnClickMcscroll, VTS_NONE)
ON_EVENT(CScreenShotDlg, IDC_MCCROP, -600 , OnClickMccrop, VTS_NONE)
ON_EVENT(CScreenShotDlg, IDC_MCZOOMOUT, -600 , OnClickMczoomout, VTS_NONE)
ON_EVENT(CScreenShotDlg, IDC_MCZOOMIN, -600 , OnClickMczoomin, VTS_NONE)
ON_EVENT(CScreenShotDlg, IDC_MCTEXT, -600 , OnClickMctext, VTS_NONE)
ON_EVENT(CScreenShotDlg, IDC_MCPEN, -600 , OnClickMcpen, VTS_NONE)
ON_EVENT(CScreenShotDlg, IDC_MCPENCOLOR, -600 , OnClickMcpencolor, VTS_NONE)
ON_EVENT(CScreenShotDlg, IDC_MCSEND, -600 , OnClickMcsend, VTS_NONE)

ON_EVENT(CScreenShotDlg, IDC_MCCHECKSEND, -600 , OnClickCheckSend, VTS_NONE)
ON_EVENT(CScreenShotDlg, IDC_MCCHECKISSUE, -600 , OnClickCheckIssue, VTS_NONE)
ON_EVENT(CScreenShotDlg, IDC_MCCHECKTODO, -600 , OnClickCheckToDo, VTS_NONE)
ON_EVENT(CScreenShotDlg, IDC_MCCHECKPUBLISH, -600 , OnClickCheckPublish, VTS_NONE)

ON_EVENT(CScreenShotDlg, IDC_USER_LIST, 1 /* Menu */, OnMenuCcootreectrl, VTS_I4 VTS_BOOL)
ON_EVENT(CScreenShotDlg, IDC_USER_LIST, 3 /* Action */, OnActionCcootreectrl, VTS_I4 VTS_BOOL)

//}}AFX_EVENTSINK_MAP
END_EVENTSINK_MAP()

/////////////////////////////////////////////////////////////////////////////
// CScreenShotDlg message handlers
BOOL CScreenShotDlg::OnInitDialog() 
{
	COFSNcDlg2::OnInitDialog();
	
	//////////////////////////////////////////////////////////////////////////
	// McToolTipAddon
	m_ToolTip.AddTool(&m_btnX,IDS_TIP_CLOSE);
	m_ToolTip.AddTool(&m_btnMin,IDS_TIP_MINIMIZE);
	m_ToolTip.AddTool(&m_btnMax,IDS_TIP_MAXIMIZE);
	m_ToolTip.AddTool(&m_btnRestore,IDS_TIP_RESTORY);
	m_ToolTip.AddTool(&m_Menu,IDS_TIP_MENU);

	m_ToolTip.AddTool(&m_btnSave,IDS_TIP_SAVE);
	m_ToolTip.AddTool(&m_btnUndo,IDS_TIP_UNDO);
	m_ToolTip.AddTool(&m_btnRedo,IDS_TIP_REDO);
	m_ToolTip.AddTool(&m_btnScroll,IDS_TIP_MOVE);
	m_ToolTip.AddTool(&m_btnCrop,IDS_TIP_CROP);
	m_ToolTip.AddTool(&m_btnZoomOut,IDS_TIP_ZOOMOUT);
	m_ToolTip.AddTool(&m_btnZoomIn,IDS_TIP_ZOOMIN);
	m_ToolTip.AddTool(&m_btnText,IDS_TIP_TEXT);
	m_ToolTip.AddTool(&m_btnPen,IDS_TIP_PEN);
	m_ToolTip.AddTool(&m_btnPenColor,IDS_TIP_PENCOLOR);

	CreateTree();

	SetRecipientGroup(m_strRecepientGroupName);

	CreateCapture();

	CString strRect = GetOptionString(IDS_OFSMESSENGER, IDS_SCREEN_CAPTURE, _T(""));
	if(!strRect.IsEmpty())
	{
		CRect rWindow = StringToRect(strRect);
		FitRectToWindow(rWindow);
		SetWindowPos(NULL,rWindow.left,rWindow.top,rWindow.Width(),rWindow.Height(),SWP_NOZORDER|SWP_NOACTIVATE);
	}
	

	return TRUE;
}

void CScreenShotDlg::OnOk() 
{
}

void CScreenShotDlg::OnCancel() 
{
	if(!m_ImageStatic.Cancel())
		KillWindow();
}

void CScreenShotDlg::KillWindow()
{
	WriteOptionInt(IDS_SCREEN_CAPTURE,IDS_TEXTCOLOR,m_ImageStatic.GetTextColor());
	WriteOptionInt(IDS_SCREEN_CAPTURE,IDS_TEXTSIZE,m_ImageStatic.GetTextSize());
	WriteOptionString(IDS_SCREEN_CAPTURE,IDS_FONTNAME,m_ImageStatic.GetFontName());

	if(GetStyle()&WS_VISIBLE&&GetOptionInt(IDS_OFSMESSENGER,IDS_ANIMATION,FALSE))
		RoundExitAddon(this);
	COFSNcDlg2::OnClose();
	
	if(!bIsKillWinodow)
	{
		bIsKillWinodow = TRUE;
		DestroyWindow();
		delete this;
	}
}


void CScreenShotDlg::LoadSkin(IXMLDOMNode *pXmlRoot)
{
	CRect r(1,2,3,202);
	m_ImageStatic.Create(NULL, NULL, WS_TABSTOP|WS_CHILD|WS_VISIBLE, r, this, IDC_IMAGE_STATIC);

	m_btnX.ShowWindow(SW_HIDE);
	m_btnMin.ShowWindow(SW_HIDE);
	m_btnMax.ShowWindow(SW_HIDE);
	m_btnRestore.ShowWindow(SW_HIDE);
	m_Menu.ShowWindow(SW_HIDE);

	m_btnSave.ShowWindow(SW_HIDE);
	m_btnUndo.ShowWindow(SW_HIDE);
	m_btnRedo.ShowWindow(SW_HIDE);
	m_btnScroll.ShowWindow(SW_HIDE);
	m_btnCrop.ShowWindow(SW_HIDE);
	m_btnZoomOut.ShowWindow(SW_HIDE);
	m_btnZoomIn.ShowWindow(SW_HIDE);
	m_btnText.ShowWindow(SW_HIDE);
	m_btnPen.ShowWindow(SW_HIDE);
	m_btnPenColor.ShowWindow(SW_HIDE);

	m_treectrl.ShowWindow(SW_HIDE);
	m_ImageStatic.ShowWindow(SW_HIDE);

	LoadButton(pXmlRoot, _T("X"), &m_btnX, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Minimize"), &m_btnMin, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Maximize"), &m_btnMax, TRUE, FALSE, 1);
	LoadButton(pXmlRoot, _T("Restore"), &m_btnRestore, TRUE, FALSE, 2);
	LoadButton(pXmlRoot, _T("Menu"), &m_Menu, TRUE, FALSE);

	LoadButton(pXmlRoot, _T("Save"), &m_btnSave, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Undo"), &m_btnUndo, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Redo"), &m_btnRedo, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Scroll"), &m_btnScroll, FALSE, TRUE);
	LoadButton(pXmlRoot, _T("Crop"), &m_btnCrop, FALSE, TRUE);
	LoadButton(pXmlRoot, _T("ZoomOut"), &m_btnZoomOut, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("ZoomIn"), &m_btnZoomIn, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Text"), &m_btnText, FALSE, TRUE);
	LoadButton(pXmlRoot, _T("Pen"), &m_btnPen, FALSE, TRUE);
	LoadButton(pXmlRoot, _T("PenColor"), &m_btnPenColor, TRUE, FALSE);

	LoadButton(pXmlRoot, _T("CheckSend"), &m_checkSend, FALSE, TRUE);
	LoadButton(pXmlRoot, _T("CheckIssue"), &m_checkIssue, FALSE, TRUE);
	LoadButton(pXmlRoot, _T("CheckAssign"), &m_checkToDo, FALSE, TRUE);
	LoadButton(pXmlRoot, _T("CheckPublish"), &m_checkPublish, FALSE, TRUE);

	LoadButton(pXmlRoot, _T("Send"), &m_btnSend, TRUE, FALSE);

	LoadRectangle(pXmlRoot, _T("Users"), &m_treectrl, TRUE);

	LoadRectangle2(pXmlRoot,_T("Image"),m_ImageStatic.GetSafeHwnd(),TRUE);

	switch(m_Mode)
	{
	case SendFile:
		m_checkSend.SetPressed(TRUE);
		break;
	case AssignToDo:
		m_checkToDo.SetPressed(TRUE);
		break;
	case CreateIssue:
		m_checkIssue.SetPressed(TRUE);
		break;
	case PublishToIBNLibrary:
		m_checkPublish.SetPressed(TRUE);
		break;
	}

	m_btnUndo.EnableWindow(FALSE);
	m_btnRedo.EnableWindow(FALSE);
	//m_btnText.EnableWindow(FALSE);

	m_ImageStatic.SetPenColor(GetOptionInt(IDS_SCREEN_CAPTURE,IDS_PENCOLOR,m_ImageStatic.GetPenColor()));
	m_ImageStatic.SetTextColor(GetOptionInt(IDS_SCREEN_CAPTURE,IDS_TEXTCOLOR,m_ImageStatic.GetTextColor()));
	m_ImageStatic.SetTextSize(GetOptionInt(IDS_SCREEN_CAPTURE,IDS_TEXTSIZE,m_ImageStatic.GetTextSize()));
	m_ImageStatic.SetFontName(GetOptionString(IDS_SCREEN_CAPTURE,IDS_FONTNAME,m_ImageStatic.GetFontName()));

	UpdateButtons();
}	

void CScreenShotDlg::OnClickMcclose ()
{
	KillWindow();
}

void CScreenShotDlg::OnClickMcmini ()
{
	ShowWindow(SW_MINIMIZE);
}

void CScreenShotDlg::OnClickMcmaxi ()
{
	ShowWindow(SW_MAXIMIZE);
}

void CScreenShotDlg::OnClickMcmaximini ()
{
	ShowWindow(SW_RESTORE);
}

void CScreenShotDlg::OnClickMcmenu ()
{
}

void CScreenShotDlg::OnClickMcpencolor()
{
	CColorMenu		colorMenu;
	
	CRect rc(10,10,100,100);
	CPoint p;
	GetCursorPos(&p);

	UINT nCmd = colorMenu.TrackPopupMenu(TPM_LEFTALIGN|TPM_LEFTBUTTON|TPM_NONOTIFY|TPM_RETURNCMD ,p.x ,p.y, this);

	if(nCmd>=ID_COLOR0&&nCmd<=ID_COLOR15)
	{
		COLORREF PenColor = colorMenu.GetColor(nCmd);

		m_ImageStatic.SetPenColor(PenColor);	
		WriteOptionInt(IDS_SCREEN_CAPTURE,IDS_PENCOLOR,PenColor);
	}
	else if(nCmd==ID_COLOR16)
	{
		CColorDialog dlg(m_ImageStatic.GetPenColor(), CC_ANYCOLOR, this);

		if(dlg.DoModal() == IDOK)
		{
			m_ImageStatic.SetPenColor(dlg.GetColor());	
			WriteOptionInt(IDS_SCREEN_CAPTURE,IDS_PENCOLOR,m_ImageStatic.GetPenColor());
		}
	}
}

void CScreenShotDlg::OnClickMcsave()
{
	m_ImageStatic.ShowSaveFileDialog();
}

void CScreenShotDlg::OnClickMcundo ()
{
	m_ImageStatic.Undo();
	//m_btnUndo.EnableWindow(m_ImageStatic.IsUndoEnable());
	//m_btnRedo.EnableWindow(m_ImageStatic.IsRedoEnable());
}

void CScreenShotDlg::OnClickMcredo ()
{
	m_ImageStatic.Redo();
	//m_btnUndo.EnableWindow(m_ImageStatic.IsUndoEnable());
	//m_btnRedo.EnableWindow(m_ImageStatic.IsRedoEnable());
}

void CScreenShotDlg::OnClickMcscroll ()
{
	m_ImageStatic.SetState(CCxImageCtrl::IS_Move);
	UpdateButtons();
}

void CScreenShotDlg::OnClickMccrop ()
{
	m_ImageStatic.SetState(CCxImageCtrl::IS_Crop);
	UpdateButtons();
}

void CScreenShotDlg::OnClickMczoomout ()
{
	m_iZoomValueIndex--;
	m_ImageStatic.SetZoomValue(ZoomValues[m_iZoomValueIndex]);
	
	if(m_iZoomValueIndex==0)
		m_btnZoomOut.EnableWindow(FALSE);
	m_btnZoomIn.EnableWindow(TRUE);
}

void CScreenShotDlg::OnClickMczoomin ()
{
	m_iZoomValueIndex++;
	m_ImageStatic.SetZoomValue(ZoomValues[m_iZoomValueIndex]);
	
	if(m_iZoomValueIndex==(sizeof(ZoomValues)/sizeof(double)-1))
		m_btnZoomIn.EnableWindow(FALSE);
	m_btnZoomOut.EnableWindow(TRUE);
}

void CScreenShotDlg::OnClickMctext ()
{
	m_ImageStatic.SetState(CCxImageCtrl::IS_Text);	
	UpdateButtons();
}

void CScreenShotDlg::OnClickMcpen ()
{
	m_ImageStatic.SetState(CCxImageCtrl::IS_Pen);
	UpdateButtons();
}

void CScreenShotDlg::OnClickMcsend ()
{
	Send();
}

void CScreenShotDlg::OnMenuCcootreectrl (long TID, BOOL bGroupe)
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

void CScreenShotDlg::CreateTree()
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

void CScreenShotDlg::SetRecipientGroup(LPCTSTR strName)
{
	m_strRecepientGroupName = strName;
	
	CString strCaption;
	
	//m_UserInfo.SetText(m_strRecepientGroupName);
	
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
				

				if(m_RecepientUserId==pUser->GetGlobalID())
					pUser->m_bHasNewMessages = TRUE;
			}
		}
	}
	
	BuildTree();
}


void CScreenShotDlg::BuildTree()
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
					
					m_treectrl.RootOpen(pUser->TID,TRUE);
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
						
						m_treectrl.RootOpen(pUser->TID,TRUE);
				}
			}
		}
		break;
	}
	
	UpdateGroupCheck();
}


void CScreenShotDlg::OnActionCcootreectrl (long TID, BOOL bGroupe)
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
						if(pUser->m_bHasNewMessages)
							m_UserCheckInGroup.SetAt(GetString(IDS_FRIENDS_NAME),(void*)(int(m_UserCheckInGroup[_T("Friends")])+1));
						else
							m_UserCheckInGroup.SetAt(GetString(IDS_FRIENDS_NAME),(void*)(int(m_UserCheckInGroup[_T("Friends")])-1));
						UpdateGroupID(GetString(IDS_OFFLINE));
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

void CScreenShotDlg::UpdateID(long UserId)
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

CUser* CScreenShotDlg::FindUserInVisualContactList(long TID)
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

BOOL CScreenShotDlg::EnableRecepients()
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

void CScreenShotDlg::UpdateGroupCheck()
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

void CScreenShotDlg::UpdateGroupID(LPCTSTR strName)
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

BOOL CScreenShotDlg::PreTranslateMessage(MSG* pMsg) 
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

CString	CScreenShotDlg::SaveImageToTmpFolder()
{
	CString	strDataDirPath	=	GetAppDataDir();
	strDataDirPath	+=	_T("\\Screen Capture");

	CreateDirectory(strDataDirPath,NULL);

	CString strFileName;

	SYSTEMTIME	st =	{0};
	
	GetSystemTime(&st);

	strFileName.Format(_T("%s\\IBNSC-%d-%02d-%02d-%05X.jpg"), strDataDirPath, st.wYear, st.wMonth, st.wDay, st.wHour*60*60+st.wMinute*60+st.wSecond);

	m_ImageStatic.Save(strFileName,CXIMAGE_FORMAT_JPG);
	
	return strFileName;
}

void	CScreenShotDlg::Send()
{
	switch(m_Mode) 
	{
	case CScreenShotDlg::SendFile:
		{
			if(pMessenger->ConnectEnable())
			{
				CString	strRecepientShowList;
				
				CString strUserId	=	_T("");
				if(POSITION pos = m_ContactList.InitIteration())
				{
					CUser	*pUser =	 NULL;
					while(m_ContactList.GetNext(pos,pUser))
					{
						if(pUser->m_bHasNewMessages)
						{
							TCHAR Buff[MAX_PATH];
							_ltot(pUser->GetGlobalID(), Buff, 10);
							strUserId += Buff;
							strUserId += ",";
							
							strRecepientShowList	+=	pUser->GetShowName();
							strRecepientShowList += ",";
						}
					}
					
					if(!strUserId.IsEmpty())
					{
						CString strTmpFileName	=	SaveImageToTmpFolder();
						pMessenger->AddToUpload(strTmpFileName,strRecepientShowList,strUserId,GetString(IDS_SCREEN_CAPTURE_NAME));
						KillWindow();
					}
				}
			}
		}
		break;
	case CScreenShotDlg::AssignToDo:
		{
			CString strTmpFileName	=	SaveImageToTmpFolder();
			CString	strAllIBNUsers	=	_T("");
			
			if(POSITION pos = m_ContactList.InitIteration())
			{
				CUser	*pUser =	 NULL;
				while(m_ContactList.GetNext(pos,pUser))
				{
					if(pUser->m_bHasNewMessages)
					{
						CString strTmp;
						if(strAllIBNUsers.GetLength()>0)
							strAllIBNUsers	+=	",";
						strTmp.Format(_T("%d"), pUser->GetGlobalID());
						strAllIBNUsers	+=	strTmp;
					}
				}
			}
			
			CString strParametrs;
			
			if(strAllIBNUsers.GetLength()>0)
			{
				strParametrs.Format(_T("/CREATETODO /L \"%s\" /P \"%s\" /IBNRESOURCES \"%s\" \"%s\""),pMessenger->m_DlgLog.m_LoginStr, pMessenger->m_DlgLog.m_PasswordStr,strAllIBNUsers,strTmpFileName);
			}
			else
			{
				strParametrs.Format(_T("/CREATETODO /L \"%s\" /P \"%s\" \"%s\""),pMessenger->m_DlgLog.m_LoginStr, pMessenger->m_DlgLog.m_PasswordStr,strTmpFileName);
			}
			
			if(pMessenger->IsSSLMode())
				strParametrs	+= _T(" /USESSL");
			
			HINSTANCE retVal = ShellExecute(::GetDesktopWindow(),NULL,pMessenger->ToolboxPath(),strParametrs,NULL,SW_SHOWNORMAL);
			
			KillWindow();
		}
		break;
	case CScreenShotDlg::CreateIssue:
		{
			CString strTmpFileName	=	SaveImageToTmpFolder();
			CString	strAllIBNUsers	=	_T("");
			
			if(POSITION pos = m_ContactList.InitIteration())
			{
				CUser	*pUser =	 NULL;
				while(m_ContactList.GetNext(pos,pUser))
				{
					if(pUser->m_bHasNewMessages)
					{
						CString strTmp;
						if(strAllIBNUsers.GetLength()>0)
							strAllIBNUsers	+=	",";
						strTmp.Format(_T("%d"), pUser->GetGlobalID());
						strAllIBNUsers	+=	strTmp;
					}
				}
			}
			
			CString strParametrs;
			
			if(strAllIBNUsers.GetLength()>0)
			{
				strParametrs.Format(_T("/CREATEINCIDENT /L \"%s\" /P \"%s\" /IBNRESOURCES \"%s\" \"%s\""),pMessenger->m_DlgLog.m_LoginStr, pMessenger->m_DlgLog.m_PasswordStr,strAllIBNUsers,strTmpFileName);
			}
			else
			{
				strParametrs.Format(_T("/CREATEINCIDENT /L \"%s\" /P \"%s\" \"%s\""),pMessenger->m_DlgLog.m_LoginStr, pMessenger->m_DlgLog.m_PasswordStr,strTmpFileName);
			}

			if(pMessenger->IsSSLMode())
				strParametrs	+= _T(" /USESSL");
			
			HINSTANCE retVal = ShellExecute(::GetDesktopWindow(),NULL,pMessenger->ToolboxPath(),strParametrs,NULL,SW_SHOWNORMAL);
			
			KillWindow();
		}
		break;
	case CScreenShotDlg::PublishToIBNLibrary:
		{
			CString strTmpFileName	=	SaveImageToTmpFolder();

			CString strParametrs;
			
			strParametrs.Format(_T("/UPLOAD /L \"%s\" /P \"%s\" \"%s\""),pMessenger->m_DlgLog.m_LoginStr, pMessenger->m_DlgLog.m_PasswordStr,strTmpFileName);

			if(pMessenger->IsSSLMode())
				strParametrs	+= _T(" /USESSL");
			
			
			HINSTANCE retVal = ShellExecute(::GetDesktopWindow(),NULL,pMessenger->ToolboxPath(),strParametrs,NULL,SW_SHOWNORMAL);
			
			KillWindow();
		}	
		break;
	}
}

void	CScreenShotDlg::CreateCapture()
{
	HWND hwnd	=	::GetDesktopWindow();
	// get window size
	CRect r;
	::GetWindowRect(hwnd,&r);
	
	// prepare the DCs
	HDC dstDC = ::GetDC(NULL);
	HDC srcDC = ::GetWindowDC(hwnd); //full window (::GetDC(hwnd); = clientarea)
	HDC memDC = ::CreateCompatibleDC(dstDC);
	
	// copy the screen to the bitmap
	HBITMAP bm =::CreateCompatibleBitmap(dstDC, r.Width(), r.Height());
	HBITMAP oldbm = (HBITMAP)::SelectObject(memDC,bm);
	::BitBlt(memDC, 0, 0, r.Width(), r.Height(), srcDC, 0, 0, SRCCOPY);
	
	// prepare the new document
	bool bRetVal = m_ImageStatic.CreateFromHBITMAP(bm);

	m_ImageStatic.Invalidate();

	// free objects
	DeleteObject(SelectObject(memDC,oldbm));
	DeleteObject(memDC);
	
}

void	CScreenShotDlg::UpdateButtons()
{
	switch(m_ImageStatic.GetState()) 
	{
	case CCxImageCtrl::IS_Move:
		m_btnScroll.SetPressed(TRUE);
		m_btnCrop.SetPressed(FALSE);
		m_btnPen.SetPressed(FALSE);
		m_btnText.SetPressed(FALSE);
		break;
	case CCxImageCtrl::IS_Crop:
		m_btnScroll.SetPressed(FALSE);
		m_btnCrop.SetPressed(TRUE);
		m_btnPen.SetPressed(FALSE);
		m_btnText.SetPressed(FALSE);
		break;
	case CCxImageCtrl::IS_Pen:
		m_btnScroll.SetPressed(FALSE);
		m_btnCrop.SetPressed(FALSE);
		m_btnPen.SetPressed(TRUE);
		m_btnText.SetPressed(FALSE);
		break;
	case CCxImageCtrl::IS_Text:
		m_btnScroll.SetPressed(FALSE);
		m_btnCrop.SetPressed(FALSE);
		m_btnPen.SetPressed(FALSE);
		m_btnText.SetPressed(TRUE);
		break;
	}
}

void CScreenShotDlg::OnCaptureChanged(CWnd *pWnd) 
{
	CRect rWindow;
	GetWindowRect(&rWindow);
	if(!IsIconic())
		WriteOptionString(IDS_OFSMESSENGER,IDS_SCREEN_CAPTURE,RectToString(rWindow));
	
	COFSNcDlg2::OnCaptureChanged(pWnd);
}

LRESULT	CScreenShotDlg::OnUndoUpdated(WPARAM w, LPARAM l)
{
	m_btnUndo.EnableWindow(m_ImageStatic.IsUndoEnable());
	m_btnRedo.EnableWindow(m_ImageStatic.IsRedoEnable());
	
	return 1;
}

void CScreenShotDlg::OnClickCheckSend ()
{
	m_Mode = SendFile;
	m_checkSend.SetPressed(TRUE);
	m_checkIssue.SetPressed(FALSE);
	m_checkToDo.SetPressed(FALSE);
	m_checkPublish.SetPressed(FALSE);
}

void CScreenShotDlg::OnClickCheckIssue ()
{
	m_Mode = CreateIssue;
	m_checkSend.SetPressed(FALSE);
	m_checkIssue.SetPressed(TRUE);
	m_checkToDo.SetPressed(FALSE);
	m_checkPublish.SetPressed(FALSE);
}

void CScreenShotDlg::OnClickCheckToDo()
{
	m_Mode = AssignToDo;
	m_checkSend.SetPressed(FALSE);
	m_checkIssue.SetPressed(FALSE);
	m_checkToDo.SetPressed(TRUE);
	m_checkPublish.SetPressed(FALSE);
}

void CScreenShotDlg::OnClickCheckPublish ()
{
	m_Mode = PublishToIBNLibrary;
	m_checkSend.SetPressed(FALSE);
	m_checkIssue.SetPressed(FALSE);
	m_checkToDo.SetPressed(FALSE);
	m_checkPublish.SetPressed(TRUE);
}



