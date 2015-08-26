// MessageDlg.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "MessageDlg.h"
#include "GlobalFunction.h"
#include "LoadSkins.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CMessageDlg dialog
extern CString GetCurrentSkin ();

CMessageDlg::CMessageDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CMessageDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CMessageDlg)
	m_MessageText = _T("");
	m_strExitTime = _T("");
	IDResource = IDS_MAIN;
	//}}AFX_DATA_INIT
	//m_ShowNext = GetOptionInt(IDS_MESSAGES,IDResource,0);
	Type      = MB_OK;
	ClickButton = 0;
	m_ExitTime  = -1;
}

CMessageDlg::CMessageDlg(UINT ID,CWnd* pParent/* = NULL*/)
    : CDialog(CMessageDlg::IDD, pParent)
{
	m_MessageText = _T("");
	IDResource = ID;
	//m_ShowNext = GetOptionInt(IDS_MESSAGES,IDResource,0);
	Type      = MB_OK;
	ClickButton = 0;
	m_ExitTime  = -1;
}


void CMessageDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CMessageDlg)
	DDX_Control(pDX, IDC_TIME_STATIC, m_TimeStatic);
	DDX_Control(pDX, IDB1,	m_mb1);
	DDX_Control(pDX, IDB21,	m_mb21);
	DDX_Control(pDX, IDB22,	m_mb22);
	DDX_Control(pDX, IDB_DONTSHOWAGAIN,	m_mbDontShowAgain);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CMessageDlg, CDialog)
	//{{AFX_MSG_MAP(CMessageDlg)
	ON_WM_MOVE()
	ON_WM_TIMER()
	ON_BN_CLICKED(IDB1, OnB1)
	ON_BN_CLICKED(IDB21, OnB21)
	ON_BN_CLICKED(IDB22, OnB22)
	ON_BN_CLICKED(IDB_DONTSHOWAGAIN, OnDontshowAgain)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

BEGIN_EVENTSINK_MAP(CMessageDlg, CDialog)
//{{AFX_EVENTSINK_MAP(CFileManagerDlg)
//}}AFX_EVENTSINK_MAP
END_EVENTSINK_MAP()


/////////////////////////////////////////////////////////////////////////////
// CMessageDlg message handlers

void CMessageDlg::OnMove(int x, int y) 
{
	CDialog::OnMove(x, y);

	if(::IsWindow(GetParent()->GetSafeHwnd()))
		GetParent()->UpdateWindow();
}

int CMessageDlg::Show(LPCTSTR strMessage,UINT nType)
{
	Type = nType;
	ClickButton  = 0;

	m_ShowNext = GetOptionInt(IDS_MESSAGES,IDResource,0);

	if(!m_ShowNext)
	{
		m_MessageText = strMessage;
		
		DoModal();

		WriteOptionInt(IDS_MESSAGES,IDResource,m_ShowNext);
		return ReturnByClick(ClickButton);
	}
	else
	{
		int Vaulue = 0;
		switch(nType)
		{
		default:
		case MB_OK:
			Vaulue = IDOK;
			break;
		case MB_OKCANCEL:
			Vaulue = IDOK;
			break;
		case MB_ABORTRETRYIGNORE:         
			Vaulue = IDABORT;
			break;
		case MB_YESNOCANCEL: 
			Vaulue = IDYES;
			break;
		case MB_YESNO:     
			Vaulue = IDYES;
			break;
		case MB_RETRYCANCEL:              
			Vaulue = IDRETRY;
			break;
		}
		return Vaulue;
	}
}

void CMessageDlg::SetText(LPCTSTR strMessage)
{
	m_MessageText = strMessage;
	SetDlgItemText(IDC_MESSAGE, m_MessageText);
}

int CMessageDlg::Show(UINT nType)
{
	Type = nType;
    ClickButton  = 0;
	
	m_ShowNext = GetOptionInt(IDS_MESSAGES,IDResource,0);

	if(!m_ShowNext)
	{
		DoModal();
		WriteOptionInt(IDS_MESSAGES,IDResource,m_ShowNext);
		return ReturnByClick(ClickButton);
	}
	return IDOK;
}

void CMessageDlg::OnB1() 
{
	ClickButton = 11;
	CDialog::OnOK();
}

void CMessageDlg::OnB21() 
{
	ClickButton = 21;

	CDialog::OnOK();
}

void CMessageDlg::OnB22() 
{
	ClickButton = 22;
	CDialog::OnOK();
}

void CMessageDlg::OnB31() 
{
	ClickButton = 31;
	CDialog::OnOK();
}

void CMessageDlg::OnB32() 
{
	ClickButton = 32;
	CDialog::OnOK();
}

void CMessageDlg::OnB33() 
{
	ClickButton = 33;
	CDialog::OnOK();
}

BOOL CMessageDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();

	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),FALSE);
	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),TRUE);

	SetDlgItemText(IDC_MESSAGE, m_MessageText);

//	m_Close.SetAutoPressed(TRUE);
//	m_Close.SetCanStayPressed(FALSE);
//	m_mb1.SetAutoPressed(TRUE);
//	m_mb1.SetCanStayPressed(FALSE);
//	m_mb21.SetAutoPressed(TRUE);
//	m_mb21.SetCanStayPressed(FALSE);
//	m_mb22.SetAutoPressed(TRUE);
//	m_mb22.SetCanStayPressed(FALSE);

//	m_mbDontShowAgain.SetAutoPressed(FALSE);
//	m_mbDontShowAgain.SetCanStayPressed(TRUE);
//	m_mbDontShowAgain.SetPressed(m_ShowNext);
//	m_mbDontShowAgain.ShowWindow(SW_SHOWNORMAL);
	
//	m_TimeStatic.SetTextColor(0xffffff);
//	m_TimeStatic.SetTransparent(TRUE);

//	LoadSkin();

	if(m_ExitTime>0)
	{
		GetDlgItem(IDC_TIME_STATIC)->ShowWindow(SW_SHOWNORMAL);
		SetTimer(1111,1000,NULL);
		m_strExitTime.Format(GetString(IDS_TIME_SEC_FORMAT),m_ExitTime);
		m_TimeStatic.SetText(m_strExitTime);
		UpdateData(FALSE);
	}
	else
	{
		GetDlgItem(IDC_TIME_STATIC)->ShowWindow(SW_HIDE);
	}


	switch(Type)
	{
	default:
	case MB_OK:
		m_mb1.ShowWindow(TRUE);
		m_mb21.ShowWindow(FALSE);
		m_mb22.ShowWindow(FALSE);
		//GetDlgItem(IDB1)->SetWindowText("Ok");
		break;
    case MB_OKCANCEL:          
		GetDlgItem(IDB1)->ShowWindow(TRUE);
		m_mb1.ShowWindow(FALSE);
		m_mb21.ShowWindow(TRUE);
		m_mb22.ShowWindow(TRUE);
		//GetDlgItem(IDB21)->ShowWindow(TRUE);
		//GetDlgItem(IDB22)->ShowWindow(TRUE);
		//GetDlgItem(IDB21)->SetWindowText("Ok");
		//GetDlgItem(IDB22)->SetWindowText("Cancel");
		break;
    case MB_ABORTRETRYIGNORE:         
		m_mb1.ShowWindow(FALSE);
		m_mb21.ShowWindow(FALSE);
		m_mb22.ShowWindow(FALSE);
		//GetDlgItem(IDB31)->ShowWindow(TRUE);
		//		GetDlgItem(IDB32)->ShowWindow(TRUE);
		//		GetDlgItem(IDB33)->ShowWindow(TRUE);		
		//		GetDlgItem(IDB31)->SetWindowText("Abort");
		//		GetDlgItem(IDB32)->SetWindowText("Retry");
		//		GetDlgItem(IDB33)->SetWindowText("Ignore");		
		
		break;
    case MB_YESNOCANCEL:              
		m_mb1.ShowWindow(FALSE);
		m_mb21.ShowWindow(FALSE);
		m_mb22.ShowWindow(FALSE);
		//		GetDlgItem(IDB31)->ShowWindow(TRUE);
		//		GetDlgItem(IDB32)->ShowWindow(TRUE);
		//		GetDlgItem(IDB33)->ShowWindow(TRUE);		
		//		GetDlgItem(IDB31)->SetWindowText("Yes");
		//		GetDlgItem(IDB32)->SetWindowText("No");
		//		GetDlgItem(IDB33)->SetWindowText("Cancel");		
		
		break;
	case MB_YESNO:                    
		m_mb1.ShowWindow(FALSE);
		m_mb21.ShowWindow(TRUE);
		m_mb22.ShowWindow(TRUE);
		//GetDlgItem(IDB21)->ShowWindow(TRUE);
		//GetDlgItem(IDB22)->ShowWindow(TRUE);
		//GetDlgItem(IDB21)->SetWindowText("Yes");
		//GetDlgItem(IDB22)->SetWindowText("No");
		break;
    case MB_RETRYCANCEL:              
		m_mb1.ShowWindow(FALSE);
		m_mb21.ShowWindow(FALSE);
		m_mb22.ShowWindow(FALSE);
		//		GetDlgItem(IDB21)->ShowWindow(TRUE);
		//		GetDlgItem(IDB22)->ShowWindow(TRUE);
		//		GetDlgItem(IDB21)->SetWindowText("Retry");
		//		GetDlgItem(IDB22)->SetWindowText("Cancel");
		
		break;
	}
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

int CMessageDlg::ReturnByClick(int Button)
{
	switch(Type)
	{
	default:
	case MB_OK:
		return IDOK;
		break;
    case MB_OKCANCEL:                 
		if(Button==21) return IDOK;
		else return IDCANCEL;
		break;
    case MB_ABORTRETRYIGNORE:         
		if(Button==31) return IDABORT;
		else
		if(Button==32) return IDRETRY;
		else return IDCANCEL;
		break;
    case MB_YESNOCANCEL:              
		if(Button==31) return IDYES;
		else
		if(Button==32) return IDNO;
		else return IDCANCEL;
		break;
	case MB_YESNO:                    
		if(Button==21) return IDYES;
		else
		return IDNO;
		break;
    case MB_RETRYCANCEL:              
		if(Button==21) return IDRETRY;
		else
		return IDCANCEL;
		break;
	}
	
	return IDCANCEL;
}


void CMessageDlg::OnTimer(UINT nIDEvent) 
{
	if(nIDEvent==1111&&m_ExitTime>0)
	{
		m_ExitTime--;
		m_strExitTime.Format(_T("(%d second)"),m_ExitTime);
		m_TimeStatic.SetText(m_strExitTime);
		UpdateData(FALSE);
		if(m_ExitTime==0)
		{
			switch(Type)
			{
			default:
			case MB_OK:
				OnB1();
				break;
			case MB_YESNO:                    
			case MB_RETRYCANCEL:              
			case MB_OKCANCEL:                 
				OnB21();
				break;
			case MB_ABORTRETRYIGNORE:         
			case MB_YESNOCANCEL:              
				OnB31();
				break;
			}
			return;
		}
	}
	
	CDialog::OnTimer(nIDEvent);
}

void CMessageDlg::SetAutoCloseTime(long SecTime)
{
	m_ExitTime = SecTime;
}

//DEL void CMessageDlg::LoadSkin()
//DEL {
//DEL 	LoadSkins	m_LoadSkin;
//DEL 	
//DEL 	IStreamPtr pStream = NULL;
//DEL 	long       Error   = 0L;
//DEL 	
//DEL 	bstr_t bstrPath = L"IBN_SCHEMA://";
//DEL 	bstrPath += (LPCTSTR)GetCurrentSkin();
//DEL 	
//DEL 	m_LoadSkin.Load(bstr_t(bstrPath+"/Shell/Message/fon.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 	{
//DEL 		LPPICTURE	Pic	=	NULL;
//DEL 		HRESULT hr = ::OleLoadPicture(pStream,	0, TRUE, IID_IPicture,(void**)&Pic);
//DEL 		if(SUCCEEDED(hr))
//DEL 		{
//DEL 			m_ResizeFon.Destroy();
//DEL 			m_ResizeFon.Create(Pic);
//DEL 			m_ResizeFon.AddAnchor(CRect(0,0,100,120),CSize(0,0),CSize(0,0));
//DEL 			m_ResizeFon.AddAnchor(CRect(100,0,250,120),CSize(0,0),CSize(100,0),CResizableImage::DUPLICATE);
//DEL 			m_ResizeFon.AddAnchor(CRect(250,0,286,120),CSize(100,0),CSize(100,0));
//DEL 			
//DEL 			m_ResizeFon.AddAnchor(CRect(0,120,100,150),CSize(0,0),CSize(0,100),CResizableImage::DUPLICATE);
//DEL 			m_ResizeFon.AddAnchor(CRect(100,120,250,150),CSize(0,0),CSize(100,100),CResizableImage::DUPLICATE);
//DEL 			m_ResizeFon.AddAnchor(CRect(250,120,286,150),CSize(100,0),CSize(100,100),CResizableImage::DUPLICATE);
//DEL 			
//DEL 			m_ResizeFon.AddAnchor(CRect(0,150,100,168),CSize(0,100),CSize(0,100));
//DEL 			m_ResizeFon.AddAnchor(CRect(100,150,250,168),CSize(0,100),CSize(100,100),CResizableImage::DUPLICATE);
//DEL 			m_ResizeFon.AddAnchor(CRect(250,150,286,168),CSize(100,100),CSize(100,100));
//DEL 		}
//DEL 	}
//DEL 	
//DEL 	m_LoadSkin.Load(bstrPath+bstr_t("/Common/btn_x.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_Close.LoadBitmapFromStream(pStream);
//DEL 
//DEL 	m_LoadSkin.Load(bstrPath+bstr_t("/Shell/Message/dontshow.bmp"),&pStream,&Error);
//DEL 	if(pStream)
//DEL 		m_mbDontShowAgain.LoadBitmapFromStream(pStream);
//DEL 
//DEL 	switch(Type) 
//DEL 	{
//DEL 	case MB_OK:
//DEL 		{
//DEL 			m_LoadSkin.Load(bstrPath+bstr_t("/Common/btn_ok.bmp"),&pStream,&Error);
//DEL 			if(pStream)
//DEL 				m_mb1.LoadBitmapFromStream(pStream);
//DEL 		}
//DEL 		break;
//DEL 	case MB_YESNO:                    
//DEL 	case MB_RETRYCANCEL:              
//DEL 	case MB_OKCANCEL:                 
//DEL 		{
//DEL 			m_LoadSkin.Load(bstrPath+bstr_t("/Common/btn_ok.bmp"),&pStream,&Error);
//DEL 			if(pStream)
//DEL 				m_mb21.LoadBitmapFromStream(pStream);
//DEL 			
//DEL 			m_LoadSkin.Load(bstrPath+bstr_t("/Common/btn_cancel.bmp"),&pStream,&Error);
//DEL 			if(pStream)
//DEL 				m_mb22.LoadBitmapFromStream(pStream);
//DEL 		}
//DEL 		break;
//DEL 	case MB_ABORTRETRYIGNORE:         
//DEL 	case MB_YESNOCANCEL:              
//DEL 		break;
//DEL 	}
//DEL 	
//DEL 	
//DEL }

//DEL BOOL CMessageDlg::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message) 
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
//DEL 	return CDialog::OnSetCursor(pWnd, nHitTest, message);
//DEL }

//DEL void CMessageDlg::OnLButtonDown(UINT nFlags, CPoint point) 
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
//DEL 				CDialog::OnNcLButtonDown(HTTOPLEFT,point);
//DEL 			else if(inPoint.y>miniRect.bottom)
//DEL 				CDialog::OnNcLButtonDown(HTBOTTOMLEFT,point);
//DEL 			else
//DEL 				CDialog::OnNcLButtonDown(HTLEFT,point);
//DEL 			else if(inPoint.x>miniRect.right)
//DEL 				if(inPoint.y<miniRect.top)
//DEL 					CDialog::OnNcLButtonDown(HTTOPRIGHT,point);
//DEL 				else if(inPoint.y>miniRect.bottom)
//DEL 					CDialog::OnNcLButtonDown(HTBOTTOMRIGHT,point);
//DEL 				else
//DEL 					CDialog::OnNcLButtonDown(HTRIGHT,point);
//DEL 				else if(inPoint.y<miniRect.top)
//DEL 					CDialog::OnNcLButtonDown(HTTOP,point);
//DEL 				else
//DEL 					CDialog::OnNcLButtonDown(HTBOTTOM,point);
//DEL 	}
//DEL 	else
//DEL 		CDialog::OnNcLButtonDown(HTCAPTION,point);
//DEL }

//DEL void CMessageDlg::OnSize(UINT nType, int cx, int cy) 
//DEL {
//DEL 	CDialog::OnSize(nType, cx, cy);
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
//DEL 		m_mb1.SetWindowPos(NULL,cx-91,cy-40,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//DEL 		m_mb21.SetWindowPos(NULL,cx-170,cy-40,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//DEL 		m_mb22.SetWindowPos(NULL,cx-91,cy-40,0,0,SWP_NOZORDER|SWP_NOSIZE|SWP_NOACTIVATE);
//DEL 		m_TimeStatic.SetWindowPos(NULL,cx-91,cy-20,0,0,SWP_NOACTIVATE|SWP_NOZORDER|SWP_NOSIZE);
//DEL 		m_mbDontShowAgain.SetWindowPos(NULL,15,cy-40,0,0,SWP_NOACTIVATE|SWP_NOZORDER|SWP_NOSIZE);
//DEL 	}
//DEL 
//DEL }

//DEL void CMessageDlg::OnPaint() 
//DEL {
//DEL 	CPaintDC dc(this); // device context for painting
//DEL 	
//DEL 	CRect m_Client;
//DEL 	GetClientRect(&m_Client);
//DEL 	m_ResizeFon.Render(dc.GetSafeHdc(),m_Client.Size());
//DEL 	
//DEL 	m_TimeStatic.Invalidate(FALSE);
//DEL 	m_TimeStatic.UpdateWindow();
//DEL 
//DEL 	CRect	rtFrame;
//DEL 	GetClientRect(rtFrame);
//DEL 	rtFrame.left = 90;
//DEL 	rtFrame.right -= 15;
//DEL 	rtFrame.top = 40;
//DEL 	rtFrame.bottom -= 50;
//DEL 
//DEL 	dc.SetBkMode(TRANSPARENT);
//DEL 	dc.SetTextColor(0xffffff);
//DEL 	CFont *pFont = GetFont();
//DEL 	dc.SelectObject(pFont);
//DEL 	dc.DrawText(m_MessageText,rtFrame,DT_LEFT|DT_TOP|DT_WORDBREAK|DT_WORD_ELLIPSIS);
//DEL }

//DEL BOOL CMessageDlg::OnEraseBkgnd( CDC* pDC )
//DEL {
//DEL 	return TRUE;
//DEL }

//DEL void CMessageDlg::OnClickMcclose() 
//DEL {
//DEL 	CDialog::OnCancel();
//DEL }

//DEL void CMessageDlg::OnClickMcB1()
//DEL {
//DEL 	OnB1();
//DEL }

//DEL void CMessageDlg::OnClickMcB21()
//DEL {
//DEL 	OnB21();
//DEL }

//DEL void CMessageDlg::OnClickMcB22()
//DEL {
//DEL 	OnB22();
//DEL }

//DEL void CMessageDlg::OnClickDontShowAgain()
//DEL {
//DEL 	OnDontshowAgain();
//DEL 	m_mbDontShowAgain.SetPressed(m_ShowNext);
//DEL }

void CMessageDlg::OnDontshowAgain() 
{
	m_ShowNext  = !m_ShowNext;
}
