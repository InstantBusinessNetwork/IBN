// FileDescriptioDlg.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "FileDescriptioDlg.h"
#include "LoadSkins.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CFileDescriptioDlg dialog
extern CString GetCurrentSkin();

CFileDescriptioDlg::CFileDescriptioDlg(CWnd* pParent /*=NULL*/)
	: CResizableDialog(CFileDescriptioDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CFileDescriptioDlg)
	m_strFileDescription = _T("");
	m_strFileName = _T("");
	//}}AFX_DATA_INIT
	m_bReadOnlyMode	=	TRUE;
//	SetBoundary(0,0);
//	SetCaption(RGB(0,0,0),RGB(0,0,0),0);
}


void CFileDescriptioDlg::DoDataExchange(CDataExchange* pDX)
{
	CResizableDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CFileDescriptioDlg)
	DDX_Control(pDX, IDOK, m_btnSend);
	DDX_Control(pDX, IDC_FILENAME_STATIC, m_FileName);
	DDX_Control(pDX, IDC_FILEDESCRIPTION_EDIT, m_ctrlDescription);
	DDX_Text(pDX, IDC_FILEDESCRIPTION_EDIT, m_strFileDescription);
	DDV_MaxChars(pDX, m_strFileDescription, 400);
	DDX_Text(pDX, IDC_FILENAME_STATIC, m_strFileName);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CFileDescriptioDlg, CResizableDialog)
	//{{AFX_MSG_MAP(CFileDescriptioDlg)
	ON_WM_CAPTURECHANGED()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

BEGIN_EVENTSINK_MAP(CFileDescriptioDlg, CResizableDialog)
//{{AFX_EVENTSINK_MAP(CFileDescriptioDlg)
	//}}AFX_EVENTSINK_MAP
END_EVENTSINK_MAP()


/////////////////////////////////////////////////////////////////////////////
// CFileDescriptioDlg message handlers

BOOL CFileDescriptioDlg::OnInitDialog() 
{
	CResizableDialog::OnInitDialog();
	
	m_ctrlDescription.SetReadOnly(m_bReadOnlyMode);
	
	if(m_bReadOnlyMode)
	{
		m_btnSend.SetWindowText(GetString(IDS_CLOSE_BTN_NAME));
	}

	ShowSizeGrip(TRUE);
	
//	m_Close.SetAutoPressed(TRUE);
//	m_Close.SetCanStayPressed(FALSE);
	
//	m_Ok.SetAutoPressed(TRUE);
//	m_Ok.SetCanStayPressed(FALSE);
	
//	m_FileName.SetTextColor(0xffffff);
//	m_FileName.SetTransparent(TRUE);
	
//	CRect winRect;
//	GetClientRect(&winRect);
	
//	m_FileName.SetWindowPos(NULL,18,43,winRect.Width()-36,16,SWP_NOZORDER|SWP_NOACTIVATE);
	
//	LoadSkin();
	AddAnchor(m_FileName.GetSafeHwnd(), CSize(0, 0), CSize(100, 0));
	AddAnchor(m_ctrlDescription.GetSafeHwnd(), CSize(0, 0), CSize(100, 100));
	AddAnchor(m_btnSend.GetSafeHwnd(), CSize(100, 100), CSize(100, 100));
	
	CString strRect = GetOptionString(IDS_OFSMESSENGER, IDS_FILE_DESCRIPTION, _T(""));
	if(!strRect.IsEmpty())
	{
		CRect rWindow = StringToRect(strRect);
		FitRectToWindow(rWindow);
		SetWindowPos(NULL,rWindow.left,rWindow.top,rWindow.Width(),rWindow.Height(),SWP_NOZORDER|SWP_NOACTIVATE);
	}
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

LPCTSTR CFileDescriptioDlg::GetDescription()
{
	return m_strFileDescription;
}

int CFileDescriptioDlg::DoModalEditMode(LPCTSTR Text)
{
	m_bReadOnlyMode			=	FALSE;
	m_strFileDescription	=	Text;
	return DoModal();
}

int CFileDescriptioDlg::DoModalReadMode(LPCTSTR Text)
{
	m_bReadOnlyMode			=	TRUE;
	m_strFileDescription	=	Text;
	return DoModal();
}

//DEL void CFileDescriptioDlg::LoadSkin()
//DEL {
//DEL 	LoadSkins	m_LoadSkin;
//DEL 	
//DEL 	IStreamPtr pStream = NULL;
//DEL 	long       Error   = 0L;
//DEL 	
//DEL 	bstr_t bstrPath = L"IBN_SCHEMA://";
//DEL 	bstrPath += (LPCTSTR)GetCurrentSkin();
//DEL 	
//DEL 	m_LoadSkin.Load(bstr_t(bstrPath+"/Shell/FileD/fon.bmp"),&pStream,&Error);
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
//DEL 	m_LoadSkin.Load(bstrPath+bstr_t("/Common/btn_x.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_Close.LoadBitmapFromStream(pStream);
//DEL 	pStream = NULL;
//DEL 	m_LoadSkin.Load(bstrPath+bstr_t("/Common/btn_ok.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_Ok.LoadBitmapFromStream(pStream);
//DEL }

//DEL void CFileDescriptioDlg::OnPaint() 
//DEL {
//DEL 	CPaintDC dc(this); // device context for painting
//DEL 	
//DEL 	CRect m_Client;
//DEL 	GetClientRect(&m_Client);
//DEL 	m_ResizeFon.Render(dc.GetSafeHdc(),m_Client.Size());
//DEL }

//DEL void CFileDescriptioDlg::OnLButtonDown(UINT nFlags, CPoint point) 
//DEL {
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

//DEL BOOL CFileDescriptioDlg::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message) 
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

//DEL void CFileDescriptioDlg::OnSize(UINT nType, int cx, int cy) 
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
//DEL 		m_Ok.SetWindowPos(NULL,cx-94,cy-38,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//DEL 		m_ctrlDescription.SetWindowPos(NULL,14,71,cx-28,cy-120,SWP_NOZORDER|SWP_NOACTIVATE);
//DEL 		m_FileName.SetWindowPos(NULL,0,0,cx-36,16,SWP_NOZORDER|SWP_NOACTIVATE|SWP_NOMOVE);
//DEL 		m_FileName.Invalidate(FALSE);
//DEL 	}
//DEL 	
//DEL }

//DEL BOOL CFileDescriptioDlg::OnEraseBkgnd( CDC* pDC )
//DEL {
//DEL 	return TRUE;
//DEL }


void CFileDescriptioDlg::OnCaptureChanged(CWnd *pWnd) 
{
	CRect rWindow;
	GetWindowRect(&rWindow);
	if(!IsIconic())
		WriteOptionString(IDS_OFSMESSENGER,IDS_FILE_DESCRIPTION,RectToString(rWindow));
	
	CResizableDialog::OnCaptureChanged(pWnd);
}

BOOL CFileDescriptioDlg::PreTranslateMessage(MSG* pMsg) 
{
	if(pMsg->message==WM_KEYDOWN||pMsg->message==WM_SYSKEYDOWN)
	{
		//TRACE("\r\n WM_KEYDOWN wParam = 0x%X",pMsg->wParam);
		if(pMsg->wParam==VK_RETURN&&(GetKeyState(VK_CONTROL)>>1))
		{
			if(GetOptionInt(IDS_OFSMESSENGER,IDS_CTRLENTER,1)==0)
			{
				OnOK();
				return TRUE;
			}
			else
			{
				pMsg->message	=	WM_KEYDOWN;
				pMsg->wParam	=	VK_RETURN;
			}
		}
		else if(pMsg->wParam==VK_RETURN&&(GetKeyState(VK_SHIFT)>>1))
		{
			if(GetOptionInt(IDS_OFSMESSENGER,IDS_SHIFTENTER,0)==0)
			{
				OnOK();
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
				OnOK();
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
				OnOK();
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
