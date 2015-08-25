// FileManagerDlg.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "FileManagerDlg.h"
#include "MainDlg.h"
#include "LoadSkins.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CFileManagerDlg dialog
extern CString GetCurrentSkin();

CFileManagerDlg::CFileManagerDlg(CMainDlg *pMessenger,CWnd* pParent /*=NULL*/)
	: COFSNcDlg2(CFileManagerDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CFileManagerDlg)
	//}}AFX_DATA_INIT
	m_pMessenger = pMessenger;
	SetBoundary(0,0);
	SetCaption(RGB(0,0,0),RGB(0,0,0),0);
	m_strSkinSettings = _T("/Shell/FileManager/skin.xml");
	m_crSplitter = CLR_NONE;
}

CFileManagerDlg::~CFileManagerDlg()
{
}


void CFileManagerDlg::DoDataExchange(CDataExchange* pDX)
{
	COFSNcDlg2::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CFileManagerDlg)
	DDX_Control(pDX, IDC_MCCLOSE, m_btnX);
	DDX_Control(pDX, IDC_MCMINI, m_btnMin);
	DDX_Control(pDX, IDC_MCMAXI2, m_btnMax);
	DDX_Control(pDX, IDC_MCMAXIMINI, m_btnRestore);
	DDX_Control(pDX, IDC_MCMENU, m_btnMenu);
	DDX_Control(pDX, IDC_BTN_LIBRARY, m_btnLibrary);
	DDX_Control(pDX, IDC_BTN_RECEIVED, m_btnReceived);
	DDX_Control(pDX, IDC_BTN_SENT, m_btnSent);
	DDX_Control(pDX, IDC_DOWN_SHOWOFFLINE, m_DownShowOffline);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CFileManagerDlg, COFSNcDlg2)
//{{AFX_MSG_MAP(CFileManagerDlg)
	ON_WM_TIMER()
	ON_WM_CLOSE()
	ON_WM_CAPTURECHANGED()
	ON_WM_DESTROY()
	ON_WM_CREATE()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

BEGIN_EVENTSINK_MAP(CFileManagerDlg, COFSNcDlg2)
//{{AFX_EVENTSINK_MAP(CFileManagerDlg)
	ON_EVENT(CFileManagerDlg, IDC_BTN_LIBRARY, -600 /* Click */, OnClickBtnLibrary, VTS_NONE)
	ON_EVENT(CFileManagerDlg, IDC_BTN_RECEIVED, -600 /* Click */, OnClickBtnReceived, VTS_NONE)
	ON_EVENT(CFileManagerDlg, IDC_BTN_SENT, -600 /* Click */, OnClickBtnSent, VTS_NONE)
	ON_EVENT(CFileManagerDlg, IDC_MCCLOSE, -600 /* Click */, OnClickMcclose, VTS_NONE)
	ON_EVENT(CFileManagerDlg, IDC_MCMINI, -600 /* Click */, OnClickMcmini, VTS_NONE)
	ON_EVENT(CFileManagerDlg, IDC_MCMENU, -600 /* Click */, OnClickMcmenu, VTS_NONE)
	ON_EVENT(CFileManagerDlg, IDC_MCMAXI2, -600 /* Click */, OnClickMcmaxi, VTS_NONE)
	ON_EVENT(CFileManagerDlg, IDC_MCMAXIMINI, -600 /* Click */, OnClickMcmaximini, VTS_NONE)
	ON_EVENT(CFileManagerDlg, IDC_DOWN_SHOWOFFLINE, -600 /* Click */, OnClickMcDownShowOffline, VTS_NONE)
	//}}AFX_EVENTSINK_MAP
END_EVENTSINK_MAP()


/////////////////////////////////////////////////////////////////////////////
// CFileManagerDlg message handlers

//DEL void CFileManagerDlg::OnClickMcLibrary()
//DEL {
//DEL //	m_Transfer.SetPressed(FALSE);
//DEL //	m_Library.SetPressed(TRUE);
//DEL 	m_FileUploadDlg.ShowWindow(SW_HIDE);
//DEL 	m_FileExplore.ShowWindow(SW_SHOWNOACTIVATE);
//DEL 	// m_SendDelete.ShowWindow(SW_HIDE);
//DEL 	// m_SendUpload.ShowWindow(SW_HIDE);
//DEL 	// m_SendCancel.ShowWindow(SW_HIDE);
//DEL //	m_ctrlSending.ShowWindow(SW_HIDE);
//DEL 
//DEL 	m_FileDownlaodDlg.ShowWindow(SW_HIDE);
//DEL 	// m_DownOffline.ShowWindow(SW_HIDE);
//DEL 	// m_DownDelete.ShowWindow(SW_HIDE);
//DEL 	// m_DownRemindLater.ShowWindow(SW_HIDE);
//DEL 	// m_DownDownload.ShowWindow(SW_HIDE);
//DEL 	// m_DownCancel.ShowWindow(SW_HIDE);
//DEL 	m_DownShowOffline.ShowWindow(SW_HIDE);
//DEL }

//DEL void CFileManagerDlg::OnClickMcTransfer()
//DEL {
//DEL //	m_Transfer.SetPressed(TRUE);
//DEL //	m_Library.SetPressed(FALSE);
//DEL 	m_FileExplore.ShowWindow(SW_HIDE);
//DEL 	m_FileUploadDlg.ShowWindow(SW_SHOWNOACTIVATE);
//DEL 	// m_SendDelete.ShowWindow(SW_SHOWNORMAL);
//DEL 	// m_SendUpload.ShowWindow(SW_SHOWNORMAL);
//DEL 	// m_SendCancel.ShowWindow(SW_SHOWNORMAL);
//DEL //	m_ctrlSending.ShowWindow(SW_SHOWNOACTIVATE);
//DEL 
//DEL 	m_FileDownlaodDlg.ShowWindow(SW_SHOWNOACTIVATE);
//DEL 	// m_DownOffline.ShowWindow(SW_SHOWNORMAL);
//DEL 	// m_DownDelete.ShowWindow(SW_SHOWNORMAL);
//DEL 	// m_DownRemindLater.ShowWindow(SW_SHOWNORMAL);
//DEL 	// m_DownDownload.ShowWindow(SW_SHOWNORMAL);
//DEL 	// m_DownCancel.ShowWindow(SW_SHOWNORMAL);
//DEL 	m_DownShowOffline.ShowWindow(SW_SHOWNOACTIVATE);
//DEL }

void CFileManagerDlg::OnOK() 
{
}

void CFileManagerDlg::OnCancel() 
{
	COFSNcDlg2::OnCancel();
}

//DEL void CFileManagerDlg::LoadSkin()
//DEL {
//DEL 	LoadSkins	m_LoadSkin;
//DEL 	
//DEL 	IStreamPtr pStream = NULL;
//DEL 	long       Error   = 0L;
//DEL 	
//DEL 	bstr_t bstrPath = L"IBN_SCHEMAmpa://";
//DEL 	bstrPath += (LPCTSTR)GetCurrentSkin();
//DEL 	
//DEL 	m_LoadSkin.Load(bstr_t(bstrPath+"/Shell/FileM/fon.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 	{
//DEL 		LPPICTURE	Pic	=	NULL;
//DEL 		HRESULT hr = ::OleLoadPicture(pStream,	0, TRUE, IID_IPicture,(void**)&Pic);
//DEL 		if(SUCCEEDED(hr))
//DEL 		{
//DEL 			m_ResizeFon.Create(Pic);
//DEL 			m_ResizeFon.AddAnchor(CRect(0,0,224,82),CSize(0,0),CSize(0,0));
//DEL 			m_ResizeFon.AddAnchor(CRect(225,0,385,82),CSize(0,0),CSize(100,0),CResizableImage::DUPLICATE);
//DEL 			m_ResizeFon.AddAnchor(CRect(386,0,399,82),CSize(100,0),CSize(100,0));
//DEL 
//DEL 			m_ResizeFon.AddAnchor(CRect(0,83,224,100),CSize(0,0),CSize(0,100),CResizableImage::DUPLICATE);
//DEL 			m_ResizeFon.AddAnchor(CRect(225,83,385,100),CSize(0,0),CSize(100,100),CResizableImage::DUPLICATE);
//DEL 			m_ResizeFon.AddAnchor(CRect(386,83,399,100),CSize(100,0),CSize(100,100),CResizableImage::DUPLICATE);
//DEL 
//DEL 			m_ResizeFon.AddAnchor(CRect(0,101,224,150),CSize(0,100),CSize(0,100));
//DEL 			m_ResizeFon.AddAnchor(CRect(225,101,385,150),CSize(0,100),CSize(100,100),CResizableImage::DUPLICATE);
//DEL 			m_ResizeFon.AddAnchor(CRect(386,101,399,150),CSize(100,100),CSize(100,100));
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
//DEL 	/*m_LoadSkin.Load(bstrPath+bstr_t("/Shell/FileM/ClearInactive.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_SendDelete.LoadBitmapFromStream(pStream);
//DEL 	pStream = NULL;
//DEL 
//DEL 	m_LoadSkin.Load(bstrPath+bstr_t("/Shell/FileM/Upload.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_SendUpload.LoadBitmapFromStream(pStream);
//DEL 	pStream = NULL;
//DEL 
//DEL 	m_LoadSkin.Load(bstrPath+bstr_t("/Shell/FileM/CancelTransfer.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_SendCancel.LoadBitmapFromStream(pStream);
//DEL 	pStream = NULL;*/
//DEL 
//DEL 	/*m_LoadSkin.Load(bstrPath+bstr_t("/Shell/FileM/Offline.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_DownOffline.LoadBitmapFromStream(pStream);
//DEL 	pStream = NULL;
//DEL 
//DEL 	m_LoadSkin.Load(bstrPath+bstr_t("/Shell/FileM/ClearInactive.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_DownDelete.LoadBitmapFromStream(pStream);
//DEL 	pStream = NULL;
//DEL 
//DEL 	m_LoadSkin.Load(bstrPath+bstr_t("/Shell/FileM/RemindLater.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_DownRemindLater.LoadBitmapFromStream(pStream);
//DEL 	pStream = NULL;
//DEL 
//DEL 	m_LoadSkin.Load(bstrPath+bstr_t("/Shell/FileM/Resume.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_DownDownload.LoadBitmapFromStream(pStream);
//DEL 	pStream = NULL;
//DEL 
//DEL 	m_LoadSkin.Load(bstrPath+bstr_t("/Shell/FileM/CancelTransfer.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_DownCancel.LoadBitmapFromStream(pStream);
//DEL 	pStream = NULL;*/
//DEL 
//DEL 	m_LoadSkin.Load(bstrPath+bstr_t("/Shell/FileM/show_offline.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_DownShowOffline.LoadBitmapFromStream(pStream);
//DEL 	pStream = NULL;
//DEL 
//DEL 	m_LoadSkin.Load(bstrPath+bstr_t("/Shell/FileM/Library.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_Library.LoadBitmapFromStream(pStream);
//DEL 	pStream = NULL;
//DEL 	
//DEL 	m_LoadSkin.Load(bstrPath+bstr_t("/Shell/FileM/Transfer.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_Transfer.LoadBitmapFromStream(pStream);
//DEL 	pStream = NULL;
//DEL 	
//DEL }

BOOL CFileManagerDlg::OnInitDialog() 
{
	COFSNcDlg2::OnInitDialog();

	//////////////////////////////////////////////////////////////////////////
	// McToolTipAddon
	m_ToolTip.AddTool(&m_btnX,IDS_TIP_CLOSE);
	m_ToolTip.AddTool(&m_btnMin,IDS_TIP_MINIMIZE);
	//m_ToolTip.AddTool(&m_btnMenu,IDS_TIP_MENU);
	m_ToolTip.AddTool(&m_btnMax,IDS_TIP_MAXIMIZE);
	m_ToolTip.AddTool(&m_btnRestore,IDS_TIP_RESTORY);
	//////////////////////////////////////////////////////////////////////////

	CRect	ClRect;
	GetWindowRect(ClRect);
//	m_FileUploadDlg.Create(CFileUploadDlg::IDD,this);
//	m_FileUploadDlg.SetWindowPos(NULL,15,106,ClRect.Width()-30,ClRect.Height()/2-100,SWP_NOZORDER|SWP_SHOWWINDOW);
//	m_FileUploadDlg.SetMessenger(m_pMessenger);

//	m_FileDownlaodDlg.Create(CFileDownloadDlg::IDD,this);
//	m_FileDownlaodDlg.SetWindowPos(NULL,15,ClRect.Height()/2+30,ClRect.Width()-30,ClRect.Height()/2-70,SWP_NOZORDER|SWP_SHOWWINDOW);
//	m_FileDownlaodDlg.SetMessanger(m_pMessenger);

//	m_FileExplore.Create(CFileExplore::IDD,this);
//	m_FileExplore.SetWindowPos(NULL,15,96,ClRect.Width()-30,ClRect.Height()-135,SWP_NOZORDER|SWP_SHOWWINDOW);
//	m_FileExplore.SetBkColor(0x666666);
	
//	ShowSizeGrip(FALSE);

	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),FALSE);
	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),TRUE);
	
//	LoadSkin();

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
	
	// m_SendDelete.SetAutoPressed(TRUE);
	// m_SendDelete.SetCanStayPressed(FALSE);
	// m_SendUpload.SetAutoPressed(TRUE);
	// m_SendUpload.SetCanStayPressed(FALSE);
	// m_SendCancel.SetAutoPressed(TRUE);
	// m_SendCancel.SetCanStayPressed(FALSE);
	
	// m_DownOffline.SetAutoPressed(TRUE);
	// m_DownOffline.SetCanStayPressed(FALSE);
	// m_DownDelete.SetAutoPressed(TRUE);
	// m_DownDelete.SetCanStayPressed(FALSE);
	// m_DownRemindLater.SetAutoPressed(TRUE);
	// m_DownRemindLater.SetCanStayPressed(FALSE);
	// m_DownDownload.SetAutoPressed(TRUE);
	// m_DownDownload.SetCanStayPressed(FALSE);
	// m_DownCancel.SetAutoPressed(TRUE);
	// m_DownCancel.SetCanStayPressed(FALSE);
	
//	m_DownShowOffline.SetAutoPressed(TRUE);
//	m_DownShowOffline.SetCanStayPressed(TRUE);
//	m_DownShowOffline.SetPressed(FALSE);

//	m_Library.SetAutoPressed(FALSE);
//	m_Library.SetCanStayPressed(TRUE);
//	m_Library.SetPressed(FALSE);

//	m_Transfer.SetAutoPressed(FALSE);
//	m_Transfer.SetCanStayPressed(TRUE);
//	m_Transfer.SetPressed(TRUE);
	
//	m_btnX.SetWindowPos(NULL,382,13,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//	m_btnMin.SetWindowPos(NULL,360,13,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//	m_btnMenu.SetWindowPos(NULL,13,14,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//	m_Transfer.SetWindowPos(NULL,137,68,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//	m_Library.SetWindowPos(NULL,14,68,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);

//	m_ctrlReceiving.SetTextColor(0xffffff);
//	m_ctrlReceiving.SetTransparent(TRUE);
//	m_ctrlReceiving.SetFontName(_T("Arial"));
//	m_ctrlReceiving.SetFontSize(-8);
//	m_ctrlReceiving.SetWindowPos(NULL,20,ClRect.Height()/2+10,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);

//	m_ctrlSending.SetTextColor(0xffffff);
//	m_ctrlSending.SetTransparent(TRUE);
//	m_ctrlSending.SetFontName(_T("Arial"));
//	m_ctrlSending.SetFontSize(-8);
//	m_ctrlSending.SetWindowPos(NULL,20,91,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
	
//	AddAnchor(m_ctrlSending.GetSafeHwnd(),CSize(0,0),CSize(0,0));
//	AddAnchor(m_ctrlReceiving.GetSafeHwnd(),CSize(0,50),CSize(100,50));

//	AddAnchor(m_FileUploadDlg.GetSafeHwnd(),CSize(0,0),CSize(100,50));
//	AddAnchor(m_FileDownlaodDlg.GetSafeHwnd(),CSize(0,50),CSize(100,100));
//	AddAnchor(m_FileExplore.GetSafeHwnd(),CSize(0,0),CSize(100,100));

	OnClickBtnLibrary();
	
	CString strRect = GetOptionString(IDS_OFSMESSENGER, IDS_FILEMANAGER, _T(""));
	if(!strRect.IsEmpty())
	{
		CRect rWindow = StringToRect(strRect);
		FitRectToWindow(rWindow);
		SetWindowPos(NULL,rWindow.left,rWindow.top,rWindow.Width(),rWindow.Height(),SWP_NOZORDER|SWP_NOACTIVATE);
	}

	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

//DEL void CFileManagerDlg::OnPaint() 
//DEL {
//DEL 	CPaintDC dc(this); // device context for painting
//DEL 	
//DEL 	CRect m_Client;
//DEL 	GetClientRect(&m_Client);
//DEL 	m_ResizeFon.Render(dc.GetSafeHdc(),m_Client.Size());
//DEL }

//DEL BOOL CFileManagerDlg::OnEraseBkgnd( CDC* pDC )
//DEL {
//DEL 	return TRUE;
//DEL }

//DEL BOOL CFileManagerDlg::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message) 
//DEL {
//DEL 	if(!(GetStyle()&WS_MAXIMIZE))
//DEL 	{
//DEL 		CRect StatusRect, miniRect;
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
//DEL 	}	
//DEL 	return COFSNcDlg2::OnSetCursor(pWnd, nHitTest, message);
//DEL }

//DEL void CFileManagerDlg::OnLButtonDown(UINT nFlags, CPoint point) 
//DEL {
//DEL 	if(!(GetStyle()&WS_MAXIMIZE))
//DEL 	{
//DEL 		
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
//DEL 	}
//DEL 	else
//DEL 		COFSNcDlg2::OnLButtonDown(nFlags, point);
//DEL 	
//DEL }

//DEL void CFileManagerDlg::OnSize(UINT nType, int cx, int cy) 
//DEL {
//DEL 	COFSNcDlg2::OnSize(nType, cx, cy);
//DEL 	
//DEL //	CRect rgnRect;
//DEL //	GetWindowRect(&rgnRect);
//DEL //	CRgn	WinRgn;
//DEL //	WinRgn.CreateRoundRectRgn(0,0,rgnRect.Width(),rgnRect.Height(),20,20);
//DEL //	SetWindowRgn(WinRgn,TRUE);
//DEL //	Invalidate(FALSE);
//DEL 	
//DEL 	if(m_btnX.GetSafeHwnd())
//DEL 	{
//DEL //		m_btnX.SetWindowPos(NULL,cx-31,13,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//DEL //		m_btnX.SetWindowPos(NULL,cx-31,13,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//DEL //		m_btnMax.SetWindowPos(NULL,cx-53,13,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//DEL //		m_btnRestore.SetWindowPos(NULL,cx-53,13,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//DEL 		
//DEL //		m_btnMax.ShowWindow(nType==SIZE_MAXIMIZED?SW_HIDE:SW_SHOWNORMAL);
//DEL //		m_btnRestore.ShowWindow(nType==SIZE_MAXIMIZED?SW_SHOWNORMAL:SW_HIDE);
//DEL 		
//DEL //		m_btnMin.SetWindowPos(NULL,cx-75,13,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//DEL 
//DEL 		// m_SendCancel.SetWindowPos(NULL,15,cy/2-15,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//DEL 		// m_SendUpload.SetWindowPos(NULL,114,cy/2-15,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//DEL 		// m_SendDelete.SetWindowPos(NULL,213,cy/2-15,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//DEL 		
//DEL 		// m_DownCancel.SetWindowPos(NULL,15,cy-64,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//DEL 		// m_DownDownload.SetWindowPos(NULL,114,cy-64,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//DEL 		// m_DownRemindLater.SetWindowPos(NULL,213,cy-64,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//DEL 		// m_DownOffline.SetWindowPos(NULL,312,cy-64,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//DEL 		// m_DownDelete.SetWindowPos(NULL,411,cy-64,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//DEL 		
//DEL 
//DEL //		m_DownShowOffline.SetWindowPos(NULL,cx-126,cy/2+10,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//DEL 	}
//DEL }

void CFileManagerDlg::OnClickMcclose() 
{
	COFSNcDlg2::OnCancel();
}


void CFileManagerDlg::OnClickMcmini() 
{
	ShowWindow(SW_MINIMIZE);
}

void CFileManagerDlg::OnClickMcmenu() 
{
	m_pMessenger->ShowGeneralMenu();
}

void CFileManagerDlg::AddToDownload(CUser &Sender, IFile *pFile)
{
	pFrgWindow = GetForegroundWindow();
	OnClickBtnReceived();
	m_FileDownlaodDlg.AddToDownload(Sender,pFile);
	ShowDialog(TRUE);
}

void CFileManagerDlg::AddToUpLoad(CString FileName, CString Login, long RecepientID, LPCTSTR strDescription)
{
	OnClickBtnSent();
	m_FileUploadDlg.AddToUpLoad(FileName, Login, RecepientID, strDescription);
	ShowDialog();
}

void CFileManagerDlg::RefreshSenderDetails(CUser &User)
{
	m_FileDownlaodDlg.RefreshSenderDetails(User);
}

void CFileManagerDlg::OnTimer(UINT nIDEvent) 
{
	if(nIDEvent==101)
	{
		if(IsWindow(m_FileUploadDlg.GetSafeHwnd()))
		{
			// m_SendDelete.EnableWindow(m_FileUploadDlg.GetDlgItem(IDC_DELETEFILE_BUTTON)->IsWindowEnabled());
			// m_SendUpload.EnableWindow(m_FileUploadDlg.GetDlgItem(IDC_UPLOAD_BUTTON)->IsWindowEnabled());
			// m_SendCancel.EnableWindow(m_FileUploadDlg.GetDlgItem(IDC_CANCEL_BUTTON)->IsWindowEnabled());
		}
		if(IsWindow(m_FileDownlaodDlg.GetSafeHwnd()))
		{
			// m_DownOffline.EnableWindow(m_FileDownlaodDlg.GetDlgItem(IDC_OFFLINE_BUTTON)->IsWindowEnabled());
			// m_DownDelete.EnableWindow(m_FileDownlaodDlg.GetDlgItem(IDC_DELETE_BUTTON)->IsWindowEnabled());
			// m_DownRemindLater.EnableWindow(m_FileDownlaodDlg.GetDlgItem(IDC_REMEMBERLATER_BUTTON)->IsWindowEnabled());
			// m_DownDownload.EnableWindow(m_FileDownlaodDlg.GetDlgItem(IDC_DOWNLOAD_BUTTON)->IsWindowEnabled());
			// m_DownCancel.EnableWindow(m_FileDownlaodDlg.GetDlgItem(IDC_CANCEL_BUTTON)->IsWindowEnabled());
		}
	}
	
	COFSNcDlg2::OnTimer(nIDEvent);
}

void CFileManagerDlg::ShowDialog(BOOL bAfterDownload)
{
	if(!bAfterDownload)
	{
		if(GetStyle()&WS_MAXIMIZE)
		{
			ShowWindow(SW_SHOW);
		}
		else
		{
			ShowWindow(SW_SHOWNORMAL);
		}
		SetForegroundWindow();
		SetFocus();
	}
	else
	{
		if(!(GetStyle()&WS_VISIBLE))
		{
			ShowWindow(SW_SHOWMINNOACTIVE);
			if(pFrgWindow)
				pFrgWindow->PostMessage(WM_ACTIVATE,WA_ACTIVE,0);
		}
		
		/*::FlashWindow(GetSafeHwnd(),TRUE);*/

		FLASHWINFO flashInfo = {0};
		flashInfo.cbSize = sizeof(FLASHWINFO);
		flashInfo.hwnd = GetSafeHwnd();
		flashInfo.dwFlags = FLASHW_ALL;
		flashInfo.uCount  = 5;
		flashInfo.dwTimeout   = 500;

		::FlashWindowEx(&flashInfo);

	}
	//SetTimer(101,100,NULL);
}

void CFileManagerDlg::OnClose() 
{
	KillTimer(101);
	
	COFSNcDlg2::OnClose();
}

void CFileManagerDlg::OnClickMcSendDelete()
{
	m_FileUploadDlg.OnDeletefileButton();
}

void CFileManagerDlg::OnClickMcSendUpload()
{
	m_FileUploadDlg.OnUploadButton();
}

void CFileManagerDlg::OnClickMcSendCancel()
{
	m_FileUploadDlg.OnCancelButton();
}

void CFileManagerDlg::OnClickMcDownOffline()
{
	m_FileDownlaodDlg.OnOfflineButton();
}

void CFileManagerDlg::OnClickMcDownDelete()
{
	m_FileDownlaodDlg.OnDeleteButton();
}

void CFileManagerDlg::OnClickMcDownRemlater()
{
	m_FileDownlaodDlg.OnRememberlaterButton();
}

void CFileManagerDlg::OnClickMcDownDownload()
{
	m_FileDownlaodDlg.OnDownloadButton();
}

void CFileManagerDlg::OnClickMcDownCancel()
{
	m_FileDownlaodDlg.OnCancelButton();
}

void CFileManagerDlg::OnClickMcDownShowOffline()
{
	m_FileDownlaodDlg.m_bShowOfflineFiles =  !m_FileDownlaodDlg.m_bShowOfflineFiles;
	m_FileDownlaodDlg.OnShowofflinefilesCheck();
}

void CFileManagerDlg::OnCaptureChanged(CWnd *pWnd) 
{
	CRect rWindow;
	GetWindowRect(&rWindow);
	if(!IsIconic())
		WriteOptionString(IDS_OFSMESSENGER,IDS_FILEMANAGER,RectToString(rWindow));
	
	COFSNcDlg2::OnCaptureChanged(pWnd);
}



void CFileManagerDlg::OnDestroy() 
{
	COFSNcDlg2::OnDestroy();
	// TODO: Add your message handler code here
	m_FileDownlaodDlg.SendMessage(WM_CLOSE);
	m_FileDownlaodDlg.DestroyWindow();
	m_FileUploadDlg.SendMessage(WM_CLOSE);
	m_FileUploadDlg.DestroyWindow();
	m_FileExplore.SendMessage(WM_CLOSE);
	m_FileExplore.DestroyWindow();
	
}

void CFileManagerDlg::OnClickMcmaxi()
{
	ShowWindow(SW_MAXIMIZE);
}

void CFileManagerDlg::OnClickMcmaximini()
{
	ShowWindow(SW_RESTORE);
}

void CFileManagerDlg::AddToUpload2(CString FileName, CString Login, CString RecepientID, LPCTSTR strDescription)
{
	OnClickBtnSent();
	m_FileUploadDlg.AddToUpload2(FileName, Login, RecepientID, strDescription);
	ShowDialog();
}

void CFileManagerDlg::DeleteAllItem()
{
	m_DownShowOffline.SetPressed(FALSE);
	m_FileUploadDlg.DeleteAllItem();
	m_FileDownlaodDlg.DeleteAllItem();
}

void CFileManagerDlg::SetUserDocumetFolder(LPCTSTR strPath)
{
	m_FileUploadDlg.LoadFilesHistory();
	m_FileDownlaodDlg.LoadFilesHistory();
	m_FileExplore.SetStartFolder(strPath);
}


void CFileManagerDlg::AddToUpload3(LPCTSTR XML)
{
	OnClickBtnSent();
	m_FileUploadDlg.AddToUpload3(XML);
	ShowDialog();
}

void CFileManagerDlg::LoadSkin(IXMLDOMNode *pXmlRoot)
{
	LoadButton(pXmlRoot, _T("X"), &m_btnX, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Minimize"), &m_btnMin, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Maximize"), &m_btnMax, TRUE, FALSE, 1);
	LoadButton(pXmlRoot, _T("Restore"), &m_btnRestore, TRUE, FALSE, 2);
	
	LoadButton(pXmlRoot, _T("Menu"), &m_btnMenu, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Library"), &m_btnLibrary, TRUE, TRUE);
	LoadButton(pXmlRoot, _T("Received"), &m_btnReceived, TRUE, TRUE);
	LoadButton(pXmlRoot, _T("Sent"), &m_btnSent, TRUE, TRUE);
	LoadButton(pXmlRoot, _T("ShowAll"), &m_DownShowOffline, TRUE, TRUE);

	LoadRectangle2(pXmlRoot, _T("Files"), m_FileExplore.GetSafeHwnd(), TRUE);
	LoadRectangle2(pXmlRoot, _T("Files"), m_FileDownlaodDlg.GetSafeHwnd(), FALSE);
	LoadRectangle2(pXmlRoot, _T("Files"), m_FileUploadDlg.GetSafeHwnd(), FALSE);

	CComBSTR bs;
	SelectChildNode(pXmlRoot, CComBSTR(L"Color[@Name='Splitter']"), NULL, &bs);
	if(bs.m_str != NULL)
	{
		long cr;
		int n = swscanf(bs.m_str, L"0x%06x", &cr);
		if(n == 1)
		{
			m_crSplitter = cr;
			m_FileExplore.SetBkColor(m_crSplitter);
		}
	}
}

void CFileManagerDlg::OnClickBtnLibrary() 
{
	m_btnLibrary.SetPressed(TRUE);
	m_btnReceived.SetPressed(FALSE);
	m_btnSent.SetPressed(FALSE);

	m_FileExplore.ShowWindow(SW_SHOWNOACTIVATE);
	m_FileDownlaodDlg.ShowWindow(SW_HIDE);
	m_FileUploadDlg.ShowWindow(SW_HIDE);

	m_DownShowOffline.ShowWindow(SW_HIDE);
}

void CFileManagerDlg::OnClickBtnReceived() 
{
	m_btnLibrary.SetPressed(FALSE);
	m_btnReceived.SetPressed(TRUE);
	m_btnSent.SetPressed(FALSE);

	m_FileExplore.ShowWindow(SW_HIDE);
	m_FileDownlaodDlg.ShowWindow(SW_SHOWNOACTIVATE);
	m_FileUploadDlg.ShowWindow(SW_HIDE);

	m_DownShowOffline.ShowWindow(SW_SHOWNOACTIVATE);
}

void CFileManagerDlg::OnClickBtnSent() 
{
	m_btnLibrary.SetPressed(FALSE);
	m_btnReceived.SetPressed(FALSE);
	m_btnSent.SetPressed(TRUE);
	
	m_FileExplore.ShowWindow(SW_HIDE);
	m_FileDownlaodDlg.ShowWindow(SW_HIDE);
	m_FileUploadDlg.ShowWindow(SW_SHOWNOACTIVATE);

	m_DownShowOffline.ShowWindow(SW_HIDE);
}

int CFileManagerDlg::OnCreate(LPCREATESTRUCT lpCreateStruct) 
{
	if (COFSNcDlg2::OnCreate(lpCreateStruct) == -1)
		return -1;
	
	m_FileUploadDlg.Create(CFileUploadDlg::IDD,this);
	m_FileUploadDlg.SetMessenger(m_pMessenger);
	
	m_FileDownlaodDlg.Create(CFileDownloadDlg::IDD,this);
	m_FileDownlaodDlg.SetMessanger(m_pMessenger);
	
	m_FileExplore.Create(CFileExplore::IDD,this);
	
	return 0;
}

