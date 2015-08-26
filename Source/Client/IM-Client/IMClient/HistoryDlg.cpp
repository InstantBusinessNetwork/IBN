// HistoryDlg.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "HistoryDlg.h"
#include "MainDlg.h"
#include "loadskins.h"
#include "cdib.h"
#include "MessageDlg.h"

#include "mcsettings.h"
#include "PageHistorySync.h"
#include "DlgTV.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

const GUID	CGID_IWebBrowser	=	{0xED016940L,0xBD5B,0x11cf,0xBA,0x4E,0x00,0xC0,0x4F,0xD7,0x08,0x16};

#define HTMLID_FIND			1
#define HTMLID_VIEWSOURCE	2
#define HTMLID_OPTIONS		3

/////////////////////////////////////////////////////////////////////////////
// CHistoryDlg dialog
extern CString GetCurrentSkin();


CHistoryDlg::CHistoryDlg(CWnd* pParent /*=NULL*/)
	: COFSNcDlg2(CHistoryDlg::IDD, pParent)
{
	EnableAutomation();
	//{{AFX_DATA_INIT(CHistoryDlg)
	//}}AFX_DATA_INIT
	m_LastFirst = TRUE;//GetOptionInt(IDS_HISTORY,IDS_SORT,FALSE);
	m_listType = GetOptionInt(IDS_HISTORY,IDS_MODE,1);
	SetBoundary(0,0);
	SetCaption(RGB(0,0,0),RGB(0,0,0),0);
	m_SynchronizeteHandle = 0L;
	pSaverDB			  = NULL;
	m_strSkinSettings = _T("/Shell/History/skin.xml");
	pMessenger = NULL;
}


void CHistoryDlg::DoDataExchange(CDataExchange* pDX)
{
	COFSNcDlg2::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CHistoryDlg)
	DDX_Control(pDX, IDC_USER_STATIC, m_ctrlUser);
	DDX_Check(pDX, IDC_LASTFIRST, m_LastFirst);
	DDX_Control(pDX, IDC_MCCLOSE, m_btnX);
	DDX_Control(pDX, IDC_MCMINI, m_btnMin);
	DDX_Control(pDX, IDC_MCMENU, m_btnMenu);
	DDX_Control(pDX, IDC_MCINCOMING, m_btnIncoming);
	DDX_Control(pDX, IDC_MCOUTGOING, m_btnOutgoing);
	DDX_Control(pDX, IDC_MCDIALOG, m_btnDialog);
	DDX_Control(pDX, IDC_MCMAXI, m_btnMax);
	DDX_Control(pDX, IDC_MCMAXIMINI, m_btnRestore);
	DDX_Control(pDX, IDC_CCOOTREECTRL, m_treebox);
	DDX_Control(pDX,IDC_MCPROGRESSCTRL,m_Progress);
	DDX_Control(pDX, ID_DHTML_CTRL, m_History);
	DDX_Control(pDX, IDC_MCFIND, m_btnFind);
	DDX_Control(pDX,IDC_TIMEFILTERMODE_COMBO,m_TimeFilterMode);
	DDX_DateTimeCtrl(pDX,IDC_FROMTIME_DATETIMEPICKER,m_FromTime);
	DDX_DateTimeCtrl(pDX,IDC_TOTIME_DATETIMEPICKER,m_ToTime);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CHistoryDlg, COFSNcDlg2)
	//{{AFX_MSG_MAP(CHistoryDlg)
	ON_BN_CLICKED(IDC_LASTFIRST, OnLastfirst)
	ON_BN_CLICKED(IDC_DIALOG, OnDialog)
	ON_BN_CLICKED(IDC_INCOMING, OnIncoming)
	ON_WM_CAPTURECHANGED()
	ON_BN_CLICKED(IDC_OUTGOING, OnOutgoing)
	ON_WM_SETCURSOR()
	ON_WM_LBUTTONDOWN()
	ON_COMMAND(ID_HISTORY_SYNHRONIZEHISTORY, OnHistorySynhronizehistory)
	ON_UPDATE_COMMAND_UI(ID_HISTORY_SYNHRONIZEHISTORY, OnUpdateHistorySynhronizehistory)
	ON_COMMAND(ID_HISTORY_PREFERENCES, OnHistoryPreferences)
	ON_WM_PAINT()
	ON_WM_DESTROY()
	ON_COMMAND(ID_HISTORY_FIND, OnHistoryFind)
	ON_CBN_SELENDOK(IDC_TIMEFILTERMODE_COMBO, OnSelendokTimeFilterMode)
	ON_NOTIFY(DTN_DATETIMECHANGE, IDC_FROMTIME_DATETIMEPICKER, OnDateTimeChangeFrom)
	ON_NOTIFY(DTN_DATETIMECHANGE, IDC_TOTIME_DATETIMEPICKER, OnDateTimeChangeTo)
	
	//}}AFX_MSG_MAP
	ON_MESSAGE(WM_NLT_CONTAINER_EVENT,OnNetEvent)
	ON_MESSAGE(WM_SAVE_TO_LOCALHISTORY_COMPLETE,OnSaveToLHComplete)
END_MESSAGE_MAP()

BEGIN_EVENTSINK_MAP(CHistoryDlg, COFSNcDlg2)
//{{AFX_EVENTSINK_MAP(CHistoryDlg)
	ON_EVENT(CHistoryDlg, IDC_MCCLOSE, -600 /* Click */, OnClickMcclose, VTS_NONE)
	ON_EVENT(CHistoryDlg, IDC_MCMINI, -600 /* Click */, OnClickMcmini, VTS_NONE)
	ON_EVENT(CHistoryDlg, IDC_MCMAXI, -600 /* Click */, OnClickMcmaxi, VTS_NONE)
	ON_EVENT(CHistoryDlg, IDC_MCMAXIMINI, -600 /* Click */, OnClickMcmaximini, VTS_NONE)
	ON_EVENT(CHistoryDlg, IDC_MCMENU, -600 /* Click */, OnClickMcmenu, VTS_NONE)
	ON_EVENT(CHistoryDlg, IDC_MCINCOMING, -600 /* Click */, OnClickIncoming, VTS_NONE)
	ON_EVENT(CHistoryDlg, IDC_MCOUTGOING, -600 /* Click */, OnClickOutgoing, VTS_NONE)
	ON_EVENT(CHistoryDlg, IDC_MCDIALOG, -600 /* Click */, OnClickDialog, VTS_NONE)
	ON_EVENT(CHistoryDlg, IDC_MCFIND, -600 /* Click */, OnHistoryFind, VTS_NONE)
	ON_EVENT(CHistoryDlg, IDC_CCOOTREECTRL, 1 /* Menu */, OnMenuCcootreectrl, VTS_I4 VTS_BOOL)
	ON_EVENT(CHistoryDlg, IDC_CCOOTREECTRL, 2 /* Select */, OnSelectCcootreectrl, VTS_I4 VTS_BOOL)
	ON_EVENT(CHistoryDlg, IDC_CCOOTREECTRL, 3 /* Action */, OnActionCcootreectrl, VTS_I4 VTS_BOOL)
	ON_EVENT(CHistoryDlg, IDC_CCOOTREECTRL, 4 /* Action */, OnDoDropTreectrl, VTS_I4 VTS_BOOL VTS_UNKNOWN VTS_I4 VTS_I4)
	ON_EVENT(CHistoryDlg, ID_DHTML_CTRL, 250 /* BeforeNavigate2 */, OnBeforeNavigate2, VTS_DISPATCH VTS_PVARIANT VTS_PVARIANT VTS_PVARIANT VTS_PVARIANT VTS_PVARIANT VTS_PBOOL)
	//}}AFX_EVENTSINK_MAP
END_EVENTSINK_MAP()

BEGIN_INTERFACE_MAP(CHistoryDlg, CCmdTarget)
INTERFACE_PART(CHistoryDlg, __uuidof(_IMpaWebCustomizerEvents), Dispatch)
END_INTERFACE_MAP()

BEGIN_DISPATCH_MAP(CHistoryDlg, CCmdTarget)
//{{AFX_DISPATCH_MAP(CHistoryDlg)	
DISP_FUNCTION_ID(CHistoryDlg,"", 18, OnShowContextMenu,    VT_EMPTY, VTS_I4 VTS_I4 VTS_I4 VTS_I4 VTS_I4 VTS_PI4)
//}}AFX_DISPATCH_MAP
END_DISPATCH_MAP()

/////////////////////////////////////////////////////////////////////////////
// CHistoryDlg message handlers

void CHistoryDlg::OnOK() 
{
	//m_History.webbrowser2.Navigate("about:blank",NULL,NULL,NULL,NULL);	
	//COFSNcDlg2::OnOK();
}

void CHistoryDlg::OnCancel() 
{
	m_bEnableNavigate = TRUE;
	m_History.Navigate(_T("about:blank"), NULL, NULL, NULL, NULL);
	
	m_treebox.DeleteTree();
	m_ContactList.Clear();
	
	COFSNcDlg2::OnCancel();
}

void CHistoryDlg::OnLastfirst() 
{
	UpdateData(TRUE);
	WriteOptionInt(IDS_HISTORY,IDS_SORT,m_LastFirst);
    Refresh();
}

void CHistoryDlg::OnDialog() 
{
	m_listType = 1;
	WriteOptionInt(IDS_HISTORY,IDS_MODE,m_listType);
	Refresh();	
}

void CHistoryDlg::OnOutgoing() 
{
	m_listType = 3;
	WriteOptionInt(IDS_HISTORY,IDS_MODE,m_listType);
	Refresh();	
}

void CHistoryDlg::OnIncoming() 
{
	m_listType = 2;
	WriteOptionInt(IDS_HISTORY,IDS_MODE,m_listType);
	Refresh();
}

BOOL CHistoryDlg::OnInitDialog() 
{
	COFSNcDlg2::OnInitDialog();

	//////////////////////////////////////////////////////////////////////////
	// McToolTipAddon
	m_ToolTip.AddTool(&m_btnX,IDS_TIP_CLOSE);
	m_ToolTip.AddTool(&m_btnMin,IDS_TIP_MINIMIZE);
	m_ToolTip.AddTool(&m_btnMax,IDS_TIP_MAXIMIZE);
	m_ToolTip.AddTool(&m_btnRestore,IDS_TIP_RESTORY);
	//m_ToolTip.AddTool(&m_btnMenu,IDS_TIP_MENU);
	//////////////////////////////////////////////////////////////////////////
	
	HRESULT hr = m_pWebCustomizer.CreateInstance(CLSID_MpaWebCustomizer);
	
	LPUNKNOWN pDispatch = m_History.GetControlUnknown();
	m_pWebCustomizer->PutRefWebBrowser((LPDISPATCH)pDispatch);
	
	InitMpaWebEvent();

	pSession = theNet2.GetSession();

	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),FALSE);
	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),TRUE);

//	m_btnMin.SetAutoPressed(TRUE);
//	m_btnMin.SetCanStayPressed(FALSE);
//	m_btnX.SetAutoPressed(TRUE);
//	m_btnX.SetCanStayPressed(FALSE);
//	m_btnMenu.SetAutoPressed(TRUE);
//	m_btnMenu.SetCanStayPressed(FALSE);
//	m_btnMax.SetAutoPressed(TRUE);
//	m_btnMax.SetCanStayPressed(FALSE);
//	m_btnRestore.SetAutoPressed(TRUE);
//	m_btnRestore.SetCanStayPressed(FALSE);
	
//	m_btnIncoming.SetAutoPressed(FALSE);
//	m_btnIncoming.SetCanStayPressed(TRUE);
//	m_btnIncoming.SetPressed(FALSE);
//	m_btnOutgoing.SetAutoPressed(FALSE);
//	m_btnOutgoing.SetCanStayPressed(TRUE);
//	m_btnOutgoing.SetPressed(FALSE);
//	m_btnDialog.SetAutoPressed(FALSE);
//	m_btnDialog.SetCanStayPressed(TRUE);
//	m_btnDialog.SetPressed(FALSE);
	
//	m_btnX.SetWindowPos(NULL,382,13,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//	m_btnMin.SetWindowPos(NULL,360,13,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//	m_btnMenu.SetWindowPos(NULL,13,14,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);

//	m_btnIncoming.SetWindowPos(NULL,218,65,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//	m_btnOutgoing.SetWindowPos(NULL,278,65,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//	m_btnDialog.SetWindowPos(NULL,338,65,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
	
	CRect windowRect;
	GetClientRect(&windowRect);

	m_bEnableNavigate = TRUE;
	m_History.Navigate(_T("about:blank"), NULL, NULL, NULL, NULL);

//	m_ctrlUser.SetTextColor(0xffffff);
//	m_ctrlUser.SetTransparent(TRUE);
//	m_ctrlUser.SetFontName(_T("Arial"));
//	m_ctrlUser.SetFontSize(-8);
//	m_ctrlUser.SetWindowPos(NULL,17,68,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);

//	m_treebox.SetWindowPos(NULL,13,91,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
	
//	m_History.SetWindowPos(NULL,218,92,windowRect.Width()-15-218, windowRect.Height()-44-92,SWP_NOZORDER|SWP_NOACTIVATE);
	
//	AddAnchor(m_ctrlUser.GetSafeHwnd(),CSize(0,0),CSize(0,0));
//	AddAnchor(&m_History,CSize(0,0),CSize(100,100));
	//AddAnchor(IDCANCEL,CSize(100,100),CSize(100,100));
	//AddAnchor(IDC_LASTFIRST,CSize(100,0),CSize(100,0));

	switch(m_listType)
	{
	case 1:
		//CheckRadioButton(IDC_INCOMING,IDC_DIALOG,IDC_DIALOG);
		m_btnDialog.SetPressed(TRUE);
		break;
	case 2:
		m_btnIncoming.SetPressed(TRUE);
		//CheckRadioButton(IDC_INCOMING,IDC_DIALOG,IDC_INCOMING);
		break;
	case 3:
		m_btnOutgoing.SetPressed(TRUE);
		//CheckRadioButton(IDC_INCOMING,IDC_DIALOG,IDC_OUTGOING);
		break;
	}

//	LoadSkin();

	// Time Filter Addon [5/20/2004]
	m_TimeFilterMode.AddString(GetString(IDS_TIMEFILTER_ALL));
	m_TimeFilterMode.AddString(GetString(IDS_TIMEFILTER_TODAY));
	m_TimeFilterMode.AddString(GetString(IDS_TIMEFILTER_YESTERDAY));
	m_TimeFilterMode.AddString(GetString(IDS_TIMEFILTER_THISWEEK));
	m_TimeFilterMode.AddString(GetString(IDS_TIMEFILTER_LASTWEEK));
	m_TimeFilterMode.AddString(GetString(IDS_TIMEFILTER_THISMONTH));
	m_TimeFilterMode.AddString(GetString(IDS_TIMEFILTER_LASTMONTH));
	m_TimeFilterMode.AddString(GetString(IDS_TIMEFILTER_THISYEAR));
	m_TimeFilterMode.AddString(GetString(IDS_TIMEFILTER_LASTYEAR));
	m_TimeFilterMode.AddString(GetString(IDS_TIMEFILTER_CUSTOM));

	// Load Options [5/20/2004]
	m_TimeFilterMode.SetCurSel(GetOptionInt(IDS_HISTORY,IDS_TIMEFILTER,5));
	
	OnSelendokTimeFilterMode();

	if(GetOptionInt(IDS_HISTORY,IDS_TIMEFILTER,5)==9)
	{
		m_FromTime = GetOptionInt(IDS_HISTORY,IDS_FROMTIME_FILTER,0);
		m_ToTime = GetOptionInt(IDS_HISTORY,IDS_TOTIME_FILTER,MAXLONG);
	}

	UpdateData(FALSE);
	
	CreateTree();
	
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CHistoryDlg::OnCaptureChanged(CWnd *pWnd) 
{
	CRect rWindow;
	GetWindowRect(&rWindow);
	if(!IsIconic())
		WriteOptionString(IDS_OFSMESSENGER,IDS_HISTORY,RectToString(rWindow));
	
	COFSNcDlg2::OnCaptureChanged(pWnd);
}

void CHistoryDlg::ShowHistory(CUser &m_FriendUser)
{
	this->m_FriendUser = m_FriendUser;
	m_ctrlUser.SetText(this->m_FriendUser.GetShowName());

	m_ContactList.Clear();
	
	pMessenger->GetCopyContactList(m_ContactList,TRUE);
	
	BuildContactList();


	UpdateData(FALSE);
	
	if(Refresh())
	{
		CString strCaption;
		strCaption.Format(GetString(IDS_HOSTORY_TITLE_FORMAT),m_FriendUser.GetShowName());
		SetWindowText(strCaption);
		CString strRect = GetOptionString(IDS_OFSMESSENGER, IDS_HISTORY, _T(""));
		if(GetStyle()&WS_MAXIMIZE)
		{
			ShowWindow(SW_SHOW);
		}
		else
		{
			if(!strRect.IsEmpty())
			{
				CRect rWindow = StringToRect(strRect);
				FitRectToWindow(rWindow);
				SetWindowPos(NULL,rWindow.left,rWindow.top,rWindow.Width(),rWindow.Height(),SWP_NOZORDER|SWP_NOACTIVATE);
			}
			ShowWindow(SW_SHOWNORMAL);
		}
		SetForegroundWindow();
	}
	m_ctrlUser.Invalidate(FALSE);
}


BOOL CHistoryDlg::Refresh()
{
	m_btnDialog.SetPressed(FALSE);
	m_btnIncoming.SetPressed(FALSE);
	m_btnOutgoing.SetPressed(FALSE);

	switch(m_listType)
	{
	case 1:
		//CheckRadioButton(IDC_INCOMING,IDC_DIALOG,IDC_DIALOG);
		m_btnDialog.SetPressed(TRUE);
		break;
	case 2:
		m_btnIncoming.SetPressed(TRUE);
		//CheckRadioButton(IDC_INCOMING,IDC_DIALOG,IDC_INCOMING);
		break;
	case 3:
		m_btnOutgoing.SetPressed(TRUE);
		//CheckRadioButton(IDC_INCOMING,IDC_DIALOG,IDC_OUTGOING);
		break;
	}
	
	if(pMessenger)
	{
		if(pMessenger->LocalDataBaseEnable())
		{
			if(m_TimeFilterMode.GetCurSel()==0)
			{
				pMessenger->GetLocalHistory()->PutToTimeFilter(-1);
				pMessenger->GetLocalHistory()->PutFromTimeFilter(-1);
			}
			else
			{
				UpdateTimerInterval();

				pMessenger->GetLocalHistory()->PutToTimeFilter(m_ToTime.GetTime());
				pMessenger->GetLocalHistory()->PutFromTimeFilter(m_FromTime.GetTime());
			}
		}

		IDispatchPtr pDispatch; 
		pDispatch.Attach(m_History.GetDocument());
		if(pDispatch!=NULL)
			return pMessenger->GetMessages/*BySID*/(bstr_t(m_FriendUser.GetShowName()),m_FriendUser.GetGlobalID(),
			     !m_LastFirst,m_listType,(IUnknown*)pDispatch);
	}

	return FALSE;
}

void CHistoryDlg::SetMessenger(CMainDlg *pMessenger)
{
	this->pMessenger = pMessenger;
}

void CHistoryDlg::RefreshIfNowThisUser(long Id)
{
	if((GetStyle()&WS_VISIBLE)&&(m_FriendUser.GetGlobalID()==Id))
	{
		Refresh();
	}
}


BOOL CHistoryDlg::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message) 
{
	if(!(GetStyle()&WS_MAXIMIZE))
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
	}

	return COFSNcDlg2::OnSetCursor(pWnd, nHitTest, message);
}

void CHistoryDlg::OnLButtonDown(UINT nFlags, CPoint point) 
{
	if(!(GetStyle()&WS_MAXIMIZE))
	{
		CPoint inPoint	=	point;
		ClientToScreen(&point);
		
		CRect StatusRect, miniRect;
		GetClientRect(&StatusRect);
		
		miniRect = StatusRect;miniRect.InflateRect(-9,-9,-9,-9);
		
		if(!miniRect.PtInRect(inPoint))
		{
			if(inPoint.x<miniRect.left)
				if(inPoint.y<miniRect.top)
					COFSNcDlg2::OnNcLButtonDown(HTTOPLEFT,point);
				else if(inPoint.y>miniRect.bottom)
					COFSNcDlg2::OnNcLButtonDown(HTBOTTOMLEFT,point);
				else
					COFSNcDlg2::OnNcLButtonDown(HTLEFT,point);
				else if(inPoint.x>miniRect.right)
					if(inPoint.y<miniRect.top)
						COFSNcDlg2::OnNcLButtonDown(HTTOPRIGHT,point);
					else if(inPoint.y>miniRect.bottom)
						COFSNcDlg2::OnNcLButtonDown(HTBOTTOMRIGHT,point);
					else
						COFSNcDlg2::OnNcLButtonDown(HTRIGHT,point);
					else if(inPoint.y<miniRect.top)
						COFSNcDlg2::OnNcLButtonDown(HTTOP,point);
					else
						COFSNcDlg2::OnNcLButtonDown(HTBOTTOM,point);
		}
		else
			COFSNcDlg2::OnNcLButtonDown(HTCAPTION,point);
	}
	else
		COFSNcDlg2::OnLButtonDown(nFlags, point);
}

//DEL void CHistoryDlg::LoadSkin()
//DEL {
//DEL 	LoadSkins	m_LoadSkin;
//DEL 	
//DEL 	IStreamPtr pStream = NULL, pStream2 = NULL;
//DEL 	long       Error   = 0L;
//DEL 	
//DEL 	bstr_t bstrPath = L"IBN_SCHEMA://";
//DEL 	bstrPath += (LPCTSTR)GetCurrentSkin();
//DEL 	
//DEL 	m_LoadSkin.Load(bstr_t(bstrPath+"/Shell/History/fon.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 	{
//DEL 		LPPICTURE	Pic	=	NULL;
//DEL 		HRESULT hr = ::OleLoadPicture(pStream,	0, TRUE, IID_IPicture,(void**)&Pic);
//DEL 		if(SUCCEEDED(hr))
//DEL 		{
//DEL 			m_ResizeFon.Create(Pic);
//DEL 			m_ResizeFon.AddAnchor(CRect(0,0,225,90),CSize(0,0),CSize(0,0));
//DEL 			m_ResizeFon.AddAnchor(CRect(226,0,280,90),CSize(0,0),CSize(100,0),CResizableImage::DUPLICATE);
//DEL 			m_ResizeFon.AddAnchor(CRect(281,0,299,90),CSize(100,0),CSize(100,0));
//DEL 			
//DEL 			m_ResizeFon.AddAnchor(CRect(0,91,225,160),CSize(0,0),CSize(0,100),CResizableImage::DUPLICATE);
//DEL 			m_ResizeFon.AddAnchor(CRect(226,91,280,160),CSize(0,0),CSize(100,100),CResizableImage::DUPLICATE);
//DEL 			m_ResizeFon.AddAnchor(CRect(281,91,299,160),CSize(100,0),CSize(100,100),CResizableImage::DUPLICATE);
//DEL 			
//DEL 			m_ResizeFon.AddAnchor(CRect(0,161,225,199),CSize(0,100),CSize(0,100));
//DEL 			m_ResizeFon.AddAnchor(CRect(226,161,280,199),CSize(0,100),CSize(100,100),CResizableImage::DUPLICATE);
//DEL 			m_ResizeFon.AddAnchor(CRect(281,161,299,199),CSize(100,100),CSize(100,100));
//DEL 		}
//DEL 	}
//DEL 	pStream = NULL;
//DEL 
//DEL 	m_LoadSkin.Load(bstrPath+bstr_t("/Common/btn_x.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_btnX.LoadBitmapFromStream(pStream);
//DEL 	pStream = NULL;
//DEL 
//DEL 	m_LoadSkin.Load(bstrPath+bstr_t("/Common/btn_minimize.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_btnMin.LoadBitmapFromStream(pStream);
//DEL 	pStream = NULL;
//DEL 
//DEL 	m_LoadSkin.Load(bstrPath+bstr_t("/Common/btn_menu.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_btnMenu.LoadBitmapFromStream(pStream);
//DEL 	pStream = NULL;
//DEL 
//DEL 	m_LoadSkin.Load(bstrPath+bstr_t("/Common/btn_maximize.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_btnMax.LoadBitmapFromStream(pStream);
//DEL 	pStream = NULL;
//DEL 
//DEL 	m_LoadSkin.Load(bstrPath+bstr_t("/Common/btn_restore.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_btnRestore.LoadBitmapFromStream(pStream);
//DEL 	pStream = NULL;
//DEL 
//DEL 	m_LoadSkin.Load(bstrPath+bstr_t("/Shell/History/incoming.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_btnIncoming.LoadBitmapFromStream(pStream);
//DEL 	pStream = NULL;
//DEL 
//DEL 	m_LoadSkin.Load(bstrPath+bstr_t("/Shell/History/outgoing.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_btnOutgoing.LoadBitmapFromStream(pStream);
//DEL 	pStream = NULL;
//DEL 
//DEL 	m_LoadSkin.Load(bstrPath+bstr_t("/Shell/History/dialog.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_btnDialog.LoadBitmapFromStream(pStream);
//DEL 	pStream = NULL;
//DEL 	//// Load Progress Image
//DEL 
//DEL     m_LoadSkin.Load(bstr_t(bstrPath+"/Shell/History/progress_empty.bmp"),&pStream,&Error);
//DEL 	m_LoadSkin.Load(bstr_t(bstrPath+"/Shell/History/progress_full.bmp"),&pStream2,&Error);
//DEL 	if((pStream!=NULL)&&(pStream2!=NULL))
//DEL 	{
//DEL 		m_Progress.LoadBitmapsFromStream(pStream,pStream2,0,0,262,20);
//DEL 	}
//DEL 	m_Progress.ShowWindow(SW_HIDE);
//DEL }

//DEL void CHistoryDlg::OnPaint() 
//DEL {
//DEL 	CPaintDC dc(this); // device context for painting
//DEL 	
//DEL 	CRect m_Client;
//DEL 	GetClientRect(&m_Client);
//DEL 	m_ResizeFon.Render(dc.GetSafeHdc(),m_Client.Size());
//DEL 	//ctrlUser.UpdateWindow();
//DEL }

//DEL BOOL CHistoryDlg::OnEraseBkgnd( CDC* pDC )
//DEL {
//DEL 	return TRUE;
//DEL }


//DEL void CHistoryDlg::OnSize(UINT nType, int cx, int cy) 
//DEL {
//DEL 	COFSNcDlg2::OnSize(nType, cx, cy);
//DEL 	
//DEL 	CRect rgnRect;
//DEL 	GetWindowRect(&rgnRect);
//DEL 	CRgn	WinRgn;
//DEL 	WinRgn.CreateRoundRectRgn(0,0,rgnRect.Width(),rgnRect.Height(),20,20);
//DEL 	SetWindowRgn(WinRgn,TRUE);
//DEL 	Invalidate(FALSE);
//DEL 	
//DEL 	if(m_btnX.GetSafeHwnd())
//DEL 	{
//DEL 		m_btnX.SetWindowPos(NULL,cx-31,13,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//DEL 		m_btnMax.SetWindowPos(NULL,cx-53,13,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//DEL 		m_btnRestore.SetWindowPos(NULL,cx-53,13,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//DEL 
//DEL 		m_btnMax.ShowWindow(nType==SIZE_MAXIMIZED?SW_HIDE:SW_SHOWNORMAL);
//DEL 		m_btnRestore.ShowWindow(nType==SIZE_MAXIMIZED?SW_SHOWNORMAL:SW_HIDE);
//DEL 		
//DEL 		m_btnMin.SetWindowPos(NULL,cx-75,13,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//DEL 		
//DEL 		m_treebox.SetWindowPos(NULL,0,0,196,cy-43-92,SWP_NOZORDER|SWP_NOMOVE|SWP_NOACTIVATE);
//DEL 		m_ctrlUser.Invalidate(FALSE);
//DEL 		m_ctrlUser.UpdateWindow();
//DEL 		
//DEL 		m_Progress.SetWindowPos(NULL,cx-277,cy-37,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//DEL 	}
//DEL }

void CHistoryDlg::OnClickMcclose() 
{
	OnCancel();
}


void CHistoryDlg::OnClickMcmini() 
{
	ShowWindow(SW_MINIMIZE);
}

void CHistoryDlg::OnClickMcmenu() 
{
	CMenu m_btnMenu, *pSubMenu = NULL;
	m_btnMenu.LoadMenu(IDR_MESSENGER_MENU);
	pSubMenu = m_btnMenu.GetSubMenu(3);
	CPoint point;
	GetCursorPos(&point);
	UpdateMenu(this, pSubMenu);
	
	pSubMenu->TrackPopupMenu(TPM_LEFTALIGN|TPM_RIGHTBUTTON, point.x, point.y, this);
}

void CHistoryDlg::OnClickIncoming()
{
	OnIncoming();
}

void CHistoryDlg::OnClickOutgoing()
{
	OnOutgoing();
}

void CHistoryDlg::OnClickDialog()
{
	OnDialog();
}

void CHistoryDlg::BuildContactList()
{
	CUser *pUser = NULL;

	m_treebox.DeleteTree();
	
	if(POSITION pos = m_ContactList.InitIteration())
	{
		for(int i=0; m_ContactList.GetNext(pos, pUser); i++)
		{
			pUser->m_nIcon = -1;
			if(pUser->IsSystemUser())
				pUser->TID = m_treebox.AddItem(0,pUser->GetShowName(),m_ShablonId[10]);
			else
				pUser->TID = m_treebox.AddItem(0,pUser->GetShowName(),m_ShablonId[9]);
		}
	}
}

void CHistoryDlg::CreateTree()
{
	LoadSkins m_LoadSkin;
	
	IStreamPtr pStream = NULL;
	long Error=0;
	m_LoadSkin.Load(bstr_t(IBN_SCHEMA)+ bstr_t((LPCTSTR)GetProductLanguage())+bstr_t("/Shell/Main/status.bmp"),&pStream,&Error);
	if(pStream)
	{	
		CDib dib(pStream);
		CPaintDC dc(this);
		HBITMAP hBmp = dib.GetHBITMAP((HDC)dc);
		m_treebox.SetImageList((long)hBmp);
		if(hBmp)
			DeleteObject(hBmp);
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

void CHistoryDlg::OnMenuCcootreectrl(long TID, BOOL bGroupe) 
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

void CHistoryDlg::OnSelectCcootreectrl(long TID, BOOL bGroupe) 
{
	if(!bGroupe)
	{
		CUser *pUser = FindUserInVisualContactList(TID);
		if(pUser)
		{
			m_FriendUser = *pUser;
			m_ctrlUser.SetText(m_FriendUser.GetShowName());
			
			if(Refresh())
			{
				CString strCaption;
				strCaption.Format(GetString(IDS_HOSTORY_TITLE_FORMAT),m_FriendUser.GetShowName());
				SetWindowText(strCaption);
			}
			m_ctrlUser.Invalidate(FALSE);
		}
	}
}

void CHistoryDlg::OnActionCcootreectrl(long TID, BOOL bGroupe) 
{
	OnSelectCcootreectrl(TID, bGroupe) ;
}

CUser* CHistoryDlg::FindUserInVisualContactList(long TID)
{
	if(POSITION pos = m_ContactList.InitIteration())
	{
		CUser *pUser=NULL;
		
		while(m_ContactList.GetNext(pos, pUser))
		{
			if(pUser->TID == TID )
				return pUser;
		}
	}
	
	return NULL;
}

void CHistoryDlg::ShowHistory()
{
	pMessenger->GetCopyContactList(m_ContactList);
	if(POSITION pos = m_ContactList.InitIteration())
	{
		CUser *pUser;
		m_ContactList.GetNext(pos,pUser);
		if(pUser)
			ShowHistory(*pUser);
	}
}

void CHistoryDlg::OnDoDropTreectrl(long TID, BOOL bGroupe, LPUNKNOWN pDataObject, long DropEffect, long LastKeyState)
{
	SetForegroundWindow();
	
	if(!bGroupe)
	{
		CUser *pUser = FindUserInVisualContactList(TID);
			
		if(pUser)
		{
			COleDataObject	pData;
			pData.Attach((LPDATAOBJECT)pDataObject,FALSE);
			
			FORMATETC stFormatTEXT = {CF_TEXT,NULL,DVASPECT_CONTENT,-1,TYMED_HGLOBAL};
			FORMATETC stFormatHDROP = {CF_HDROP,NULL,DVASPECT_CONTENT,-1,TYMED_HGLOBAL};
			
			STGMEDIUM outData =	{0};
			
			if(pData.GetData(CF_TEXT,&outData,&stFormatTEXT))
			{
				LPCTSTR strText = (LPCTSTR)GlobalLock(outData.hGlobal);
				pMessenger->SendMessageToUser(pUser,FALSE,strText);
				GlobalUnlock(outData.hGlobal);
			}
			else if(pData.GetData(CF_HDROP,&outData,&stFormatHDROP))
			{
				
				SIZE_T Size = GlobalSize(outData.hGlobal);
				HGLOBAL hGlobal = GlobalAlloc(GMEM_ZEROINIT,Size);

				memcpy(GlobalLock(hGlobal),GlobalLock(outData.hGlobal),Size);

				GlobalUnlock(hGlobal);
				GlobalUnlock(outData.hGlobal);

				pMessenger->PostMessage(WM_SEND_FILE,pUser->GetGlobalID(),(LPARAM)hGlobal);
			}
		}
	}
}

void CHistoryDlg::OnHistorySynhronizehistory() 
{
	CMcSettingsDlg dlg(this);
	CPageHistorySync		page1(GetString(IDS_HISTORY_SYNC_NAME));

	dlg.AddPage(&page1);

	if(dlg.DoModal()==IDOK)
	{
		int Mode       = GetOptionInt(IDS_HISTORY,IDS_SYNCMODE,0);
		long m_FromTime, m_ToTime;
		
		switch(Mode)
		{
		case 0:
			m_ToTime   = time(NULL);
			m_FromTime = m_ToTime - 60*60*24*1; // Секунды Минуты Часы Дни
			break;
		case 1:
			m_ToTime   = time(NULL);
			m_FromTime = m_ToTime - 60*60*24*7; // Секунды Минуты Часы Дни
			break;
		case 2:
			m_ToTime   = time(NULL);
			m_FromTime = m_ToTime - 60*60*24*31; // Секунды Минуты Часы Дни
			break;
		case 3:
			m_ToTime   = time(NULL);
			m_FromTime = m_ToTime - 60*60*24*365; // Секунды Минуты Часы Дни
			break;
		case 4:
			m_FromTime = GetOptionInt(IDS_HISTORY,IDS_FROMTIME,0);
			m_ToTime   = GetOptionInt(IDS_HISTORY,IDS_TOTIME,time(NULL));
			break;
		}
		
		SinchronizateHistory(m_FromTime,m_ToTime);
	}
}

void CHistoryDlg::OnUpdateHistorySynhronizehistory(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(pMessenger->ConnectEnable(FALSE)&&!pSaverDB);
}

void CHistoryDlg::SinchronizateHistory(long From, long To)
{
	theNet2.LockTranslator();
	try
	{
		m_SynchronizeteHandle = pSession->LoadSIDs(From, To);
		if(m_SynchronizeteHandle)
		{
			m_Progress.ShowWindow(SW_NORMAL);
			m_Progress.SetRange(0,100);
			m_Progress.SetPos(0);
			theNet2.AddToTranslator(m_SynchronizeteHandle,GetSafeHwnd());
		}
	}
	catch(...)
	{
		ASSERT(FALSE);
	}
	theNet2.UnlockTranslator();
}

LRESULT CHistoryDlg::OnNetEvent(WPARAM w,LPARAM l)
{
	NLT_Container *pItem = (NLT_Container *)w;
	ASSERT(pItem!=NULL);
	
	IlocalSIDs	*pSIDs			= NULL;
	IMessages	*pMessagesList	= NULL;

	theNet2.LockTranslator();
	theNet2.RemoveFromTranslator(m_SynchronizeteHandle);
	theNet2.UnlockTranslator();
	
	switch(pItem->EventType)
	{
	case NLT_ESessionsList:
		if(SUCCEEDED(AutoUnMarchaling(pItem,(LPUNKNOWN*)&pSIDs)))
		{
			try
			{
				LocalSIDUpdate(pSIDs);
			}
			catch(...)
			{
				ASSERT(FALSE);
			}
			pSIDs->Release();
		}
		else
		{
			m_Progress.ShowWindow(SW_HIDE);
		}
		break;
	case NLT_EMessagesList:
		if(SUCCEEDED(AutoUnMarchaling(pItem,(LPUNKNOWN*)&pMessagesList)))
		{
			try
			{
				SaveMessagesToDataBase(pItem->Handel,pMessagesList);
				pMessagesList->Release();
			}
			catch(...)
			{
				ASSERT(FALSE);
			}
		}
		else
		{
			m_Progress.ShowWindow(SW_HIDE);
			if(pSaverDB)
			{
				delete pSaverDB;
				pSaverDB = NULL;
			}
		}
		break;
	case NLT_ECommandError:
		try
		{
			MessageBox(GetString(IDS_SYNC_HISTORY_ERROR_NAME),GetString(IDS_ERROR_TITLE));
			m_Progress.ShowWindow(SW_HIDE);
			if(pSaverDB)
			{
				delete pSaverDB;
				pSaverDB = NULL;
			}
		}
		catch(...)
		{
		}
		break;
	}
	
	delete pItem;	
	return 0;
}
	
void CHistoryDlg::LocalSIDUpdate(IlocalSIDs *pSid)
{
	long Size = pSid->GetCount();

	
	for(int i=1;i<=Size; i++)
	{
		IlocalSIDPtr pItem = pSid->GetItem(i);
		
		if(pItem==NULL) continue;
		
		bstr_t SID =  pItem->GetSID();
		long  Count = pItem->GetCount();
		long CountInHistory = 0;
		
		//////////////////////////////////////////////////////////////////////////
		IComHistIntPtr  m_LocalHistory = pMessenger->GetLocalHistory();
		try
		{
			m_LocalHistory->GetCountForSID(SID,&CountInHistory);
		}
		catch(...)
		{
			ASSERT(FALSE);
			continue;
		}
		//////////////////////////////////////////////////////////////////////////
				
		if(CountInHistory!=Count)
		{
			m_LoadedSid.Add(SID);
		}
	}
	
	m_Progress.SetRange(0,m_LoadedSid.GetSize());
	CheckProgress();
	LoadNextSID();

}

void CHistoryDlg::CheckProgress()
{
	if(m_LoadedSid.GetSize())
	{
		m_Progress.StepIt();
	}
	else
	{
		m_Progress.ShowWindow(SW_HIDE);
		CMessageDlg m_Dlg(IDS_SYNCHRONIZE);
		m_Dlg.Show(GetString(IDS_SYNC_HISTORY_COMPLETED_NAME));
	}
}

BOOL CHistoryDlg::LoadNextSID()
{
	if(m_LoadedSid.GetSize())
	{
		theNet2.LockTranslator();
		try
		{
			m_SynchronizeteHandle = pSession->LoadMessages(m_LoadedSid[0]);
			if(m_SynchronizeteHandle)
			{
				theNet2.AddToTranslator(m_SynchronizeteHandle,GetSafeHwnd());
			}
		}
		catch(...)
		{
			ASSERT(FALSE);
		}
		theNet2.UnlockTranslator();
		
		return TRUE;
	}
	return FALSE;
}

void CHistoryDlg::SaveMessagesToDataBase(LONG Handle,IMessages *pMessagesList)
{
	IMessagesPtr pMessagesListPtr = pMessagesList;
	pSaverDB = new CSaveDataBase( m_LoadedSid.GetAt(0),	pMessenger->GetLocalHistory(),pMessagesListPtr,WM_SAVE_TO_LOCALHISTORY_COMPLETE,GetSafeHwnd());
	
}

HRESULT CHistoryDlg::OnSaveToLHComplete(WPARAM w, LPARAM l)
{
	if(pSaverDB!=NULL)
	{
		delete pSaverDB;
		pSaverDB = NULL;
	}
	m_LoadedSid.RemoveAt(0);
	CheckProgress();
	LoadNextSID();
	
	return 0;
}


void CHistoryDlg::OnHistoryPreferences() 
{
	pMessenger->PreferenceDlg(this);
}

void CHistoryDlg::OnClickMcmaxi()
{
	ShowWindow(SW_MAXIMIZE);
}

void CHistoryDlg::OnClickMcmaximini()
{
	ShowWindow(SW_RESTORE);
}

void CHistoryDlg::LoadSkin(IXMLDOMNode *pXmlRoot)
{
	LoadButton(pXmlRoot, _T("X"), &m_btnX, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Minimize"), &m_btnMin, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Maximize"), &m_btnMax, TRUE, FALSE, 1);
	LoadButton(pXmlRoot, _T("Restore"), &m_btnRestore, TRUE, FALSE, 2);
	
	LoadButton(pXmlRoot, _T("Menu"), &m_btnMenu, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Incoming"), &m_btnIncoming, TRUE, TRUE);
	LoadButton(pXmlRoot, _T("Outgoing"), &m_btnOutgoing, TRUE, TRUE);
	LoadButton(pXmlRoot, _T("Dialog"), &m_btnDialog, TRUE, TRUE);
	LoadButton(pXmlRoot, _T("Find"), &m_btnFind,TRUE,FALSE);
	
	LoadRectangle(pXmlRoot, _T("Users"), &m_treebox, TRUE);
	LoadRectangle2(pXmlRoot, _T("History"), m_History.GetSafeHwnd(), TRUE, TRUE);
	LoadProgress(pXmlRoot, _T("Synchronize"), &m_Progress, FALSE);
	LoadLabel(pXmlRoot, _T("User"), &m_ctrlUser, TRUE);

	// Time Filter Addon [5/20/2004]
	LoadRectangle(pXmlRoot,_T("TimeFilterMode"),&m_TimeFilterMode,TRUE);
	LoadRectangle2(pXmlRoot,_T("FromTimeFilter"),GetDlgItem(IDC_FROMTIME_DATETIMEPICKER)->GetSafeHwnd() ,TRUE);
	LoadRectangle2(pXmlRoot,_T("ToTimeFilter"),GetDlgItem(IDC_TOTIME_DATETIMEPICKER)->GetSafeHwnd(),TRUE);
}

void CHistoryDlg::OnPaint() 
{
	COFSNcDlg2::OnPaint();
	m_ctrlUser.Invalidate();
}

void CHistoryDlg::OnBeforeNavigate2(LPDISPATCH pDisp, VARIANT FAR* URL, VARIANT FAR* Flags, VARIANT FAR* TargetFrameName, VARIANT FAR* PostData, VARIANT FAR* Headers, BOOL FAR* Cancel) 
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
		if(m_bEnableNavigate)
		{
			m_bEnableNavigate = FALSE;
			return;
		}
		else
		{
			*Cancel = TRUE;
			CString	strUrl(URL->bstrVal);
			
			strUrl = pMessenger->ChangeUrlForCurrentDomain(strUrl);

			m_bEnableNavigate = TRUE;

			if(S_OK != ::NavigateNewWindow(m_History.GetControlUnknown(), strUrl))
				ShellExecute(::GetDesktopWindow(), _T("open"), strUrl, NULL, NULL, SW_SHOWDEFAULT);
			
			m_bEnableNavigate = FALSE;
		}
	}
}

void CHistoryDlg::InitMpaWebEvent()
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

void CHistoryDlg::CloseMpaWebEvent()
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


void CHistoryDlg::OnShowContextMenu(long dwID, long x, long y, IUnknown *pcmdTarget, IDispatch *pdispReserved, long* pShow)
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

void CHistoryDlg::OnDestroy() 
{
	CloseMpaWebEvent();

	COFSNcDlg2::OnDestroy();
}


void CHistoryDlg::OnHistoryFind() 
{
	CComPtr<IDispatch>			lpDispatch			=	NULL;
	CComPtr<IOleCommandTarget>	lpOleCommandTarget	=	NULL;
	
	// Get the IDispatch of the document.
	//
	lpDispatch.Attach(m_History.GetDocument());
	if (lpDispatch)
	{
		// Get a pointer for the IOleCommandTarget interface.
		//
		lpDispatch->QueryInterface(IID_IOleCommandTarget,  (void**)&lpOleCommandTarget);
		
		if(lpOleCommandTarget)
		{
			// Invoke the given command id 
			// for the WebBrowser control.
			HRESULT hr = lpOleCommandTarget->Exec(&CGID_IWebBrowser, HTMLID_FIND, 0, NULL, NULL);
		}
	}
}

int CHistoryDlg::UpdateTimerInterval()
{
	CTime	currBeginTime = CTime::GetCurrentTime();
	CTime	currEndTime = currBeginTime;
	int dow = currBeginTime.GetDayOfWeek() - 2;
	if(dow == 0)
		dow = 6; // Sunday

	int filter = m_TimeFilterMode.GetCurSel();
	switch(filter) 
	{
	case 0: //IDS_TIMEFILTER_ALL
		m_FromTime = CTime((time_t)0);
		m_ToTime = CTime((time_t)MAXLONG);
		break;
	case 1: //IDS_TIMEFILTER_TODAY
		break;
	case 2: //IDS_TIMEFILTER_YESTERDAY
		currBeginTime -= CTimeSpan(1, 0, 0, 0);
		currEndTime = currBeginTime;
		break;
	case 3: //IDS_TIMEFILTER_THISWEEK
		currBeginTime -= CTimeSpan(7, 0, 0, 0);
		break;
	case 4: //IDS_TIMEFILTER_LASTWEEK
		currBeginTime -= CTimeSpan(7*2, 0, 0, 0);
		currEndTime -= CTimeSpan(7, 0, 0, 0);
		break;
	case 5: //IDS_TIMEFILTER_THISMONTH
		currBeginTime -= CTimeSpan(31, 0, 0, 0);
		break;
	case 6: //IDS_TIMEFILTER_LASTMONTH
		currBeginTime -= CTimeSpan(31*2, 0, 0, 0);
		currEndTime -= CTimeSpan(31, 0, 0, 0);
		break;
	case 7: //IDS_TIMEFILTER_THISYEAR
		currBeginTime -= CTimeSpan(365, 0, 0, 0);
		break;
	case 8: //IDS_TIMEFILTER_LASTYEAR
		currBeginTime -= CTimeSpan(365*2, 0, 0, 0);
		currEndTime -= CTimeSpan(365, 0, 0, 0);
		break;
	case 9: //IDS_TIMEFILTER_CUSTOM
		GetDlgItem(IDC_FROMTIME_DATETIMEPICKER)->EnableWindow(TRUE);
		GetDlgItem(IDC_TOTIME_DATETIMEPICKER)->EnableWindow(TRUE);
		//currBeginTime = currBeginTime - CTimeSpan(7,0,0,0);
		break;
	}

	if(filter > 0 && filter<9)
	{
		m_FromTime = CTime(currBeginTime.GetYear(), currBeginTime.GetMonth(), currBeginTime.GetDay(), 0, 0, 0);
		m_ToTime = CTime(currEndTime.GetYear(), currEndTime.GetMonth(), currEndTime.GetDay(), 23, 59, 59);
	}

	UpdateData(FALSE);

	return filter;

}

void CHistoryDlg::OnSelendokTimeFilterMode()
{
	
	GetDlgItem(IDC_FROMTIME_DATETIMEPICKER)->EnableWindow(FALSE);
	GetDlgItem(IDC_TOTIME_DATETIMEPICKER)->EnableWindow(FALSE);

	int filter = UpdateTimerInterval();
	
	UpdateData(FALSE);

	if(pMessenger!=NULL)
	{
		WriteOptionInt(IDS_HISTORY,IDS_FROMTIME_FILTER, m_FromTime.GetTime());
		WriteOptionInt(IDS_HISTORY,IDS_TOTIME_FILTER, m_ToTime.GetTime());
		WriteOptionInt(IDS_HISTORY,IDS_TIMEFILTER, filter == -1 ? 0 : filter);
		
		Refresh();
	}
}

void CHistoryDlg::OnDateTimeChangeFrom (NMHDR* pNMHDR, LRESULT* pResult)
{
	UpdateData();

	WriteOptionInt(IDS_HISTORY,IDS_FROMTIME_FILTER,m_FromTime.GetTime());
	
	if(pMessenger!=NULL)
		Refresh();
}

void CHistoryDlg::OnDateTimeChangeTo(NMHDR* pNMHDR, LRESULT* pResult)
{
	UpdateData();

	WriteOptionInt(IDS_HISTORY,IDS_TOTIME_FILTER,m_ToTime.GetTime());

	if(pMessenger!=NULL)
		Refresh();
	
}


