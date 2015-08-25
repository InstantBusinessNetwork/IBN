// DlgTV.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "DlgTV.h"
#include "GlobalFunction.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDlgTV dialog


CDlgTV::CDlgTV(CWnd* pParent /*=NULL*/)
	: DLG_TV_PARENT(CDlgTV::IDD, pParent)
{
	//{{AFX_DATA_INIT(CDlgTV)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
	m_strSkinSettings = _T("/Shell/TV/skin.xml");
	SetBoundary(0, 0);
	m_nPlayer = 0;
	m_bIsClosed = FALSE;
}


void CDlgTV::DoDataExchange(CDataExchange* pDX)
{
	DLG_TV_PARENT::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDlgTV)
	DDX_Control(pDX, IDC_PLACEHOLDER, m_placeholder);
	DDX_Control(pDX, IDC_BUTTON_X, m_btnX);
	DDX_Control(pDX, IDC_BUTTON_RESTORE, m_btnRestore);
	DDX_Control(pDX, IDC_BUTTON_MIN, m_btnMin);
	DDX_Control(pDX, IDC_BUTTON_MAX, m_btnMax);
	DDX_Control(pDX, IDC_MEDIAPLAYER1, m_mediaplayer);
	DDX_Control(pDX, IDC_SHOCKWAVEFLASH1, m_flash);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CDlgTV, DLG_TV_PARENT)
	//{{AFX_MSG_MAP(CDlgTV)
	ON_WM_CLOSE()
	ON_WM_CREATE()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CDlgTV message handlers

void CDlgTV::OnOK() 
{
//	DLG_TV_PARENT::OnOK();
	PostMessage(WM_CLOSE);
}

void CDlgTV::OnCancel() 
{
//	DLG_TV_PARENT::OnCancel();
	PostMessage(WM_CLOSE);
}

BEGIN_EVENTSINK_MAP(CDlgTV, DLG_TV_PARENT)
    //{{AFX_EVENTSINK_MAP(CDlgTV)
	ON_EVENT(CDlgTV, IDC_BUTTON_X, -600 /* Click */, OnClickButtonX, VTS_NONE)
	ON_EVENT(CDlgTV, IDC_BUTTON_RESTORE, -600 /* Click */, OnClickButtonRestore, VTS_NONE)
	ON_EVENT(CDlgTV, IDC_BUTTON_MIN, -600 /* Click */, OnClickButtonMin, VTS_NONE)
	ON_EVENT(CDlgTV, IDC_BUTTON_MAX, -600 /* Click */, OnClickButtonMax, VTS_NONE)
	//}}AFX_EVENTSINK_MAP
END_EVENTSINK_MAP()

void CDlgTV::OnClickButtonX() 
{
	OnCancel();
}

void CDlgTV::OnClickButtonRestore() 
{
	ShowWindow(SW_SHOWNORMAL);
}

void CDlgTV::OnClickButtonMin() 
{
	ShowWindow(SW_SHOWMINIMIZED);
}

void CDlgTV::OnClickButtonMax() 
{
	ShowWindow(SW_SHOWMAXIMIZED);
}

void CDlgTV::LoadSkin(IXMLDOMNode *pXmlRoot)
{
	m_btnX.ShowWindow(SW_HIDE);
	m_btnMax.ShowWindow(SW_HIDE);
	m_btnRestore.ShowWindow(SW_HIDE);
	m_btnMin.ShowWindow(SW_HIDE);
	LoadButton(pXmlRoot, _T("X"), &m_btnX, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Maximize"), &m_btnMax, TRUE, FALSE, 1);
	LoadButton(pXmlRoot, _T("Restore"), &m_btnRestore, TRUE, FALSE, 2);
	LoadButton(pXmlRoot, _T("Minimize"), &m_btnMin, TRUE, FALSE);

	ShowPlayer(0);
	LoadRectangle(pXmlRoot, _T("Player"), &m_mediaplayer, FALSE);
	LoadRectangle(pXmlRoot, _T("Player"), &m_flash, FALSE);
}

BOOL CDlgTV::CreateAutoKiller(BSTR URL, CWnd *pParentWnd, long ScreenCX, long ScreenCY)
{
	m_bAutoKill = TRUE;
	
	if(!Create(IDD, pParentWnd))
	{
		TRACE0("Warning: failed to create CDlgTV.\n");
		return FALSE;
	}
	
	SetFrameSize(ScreenCX, ScreenCY);
	MMOpen(URL);
	ShowWindow(SW_SHOWNORMAL);
	SetForegroundWindow();
	return TRUE;
}

int CDlgTV::OnCreate(LPCREATESTRUCT lpCreateStruct) 
{
	if (DLG_TV_PARENT::OnCreate(lpCreateStruct) == -1)
		return -1;
	
	AddWindowToClose(this);
	
	return 0;
}

void CDlgTV::OnClose() 
{
	if(m_bIsClosed)
		return;

	m_bIsClosed = TRUE;
	RemoveWindowToClose(this);

	MMStop();
	if(m_bAutoKill)
	{
		if(GetStyle() & WS_VISIBLE && GetOptionInt(IDS_OFSMESSENGER, IDS_ANIMATION, FALSE))
			RoundExitAddon(this);
		DLG_TV_PARENT::OnClose();
		DestroyWindow();
		delete this;
	}
	else
		DLG_TV_PARENT::OnClose();
}

int CDlgTV::GetPlayer(BSTR URL)
{
	// Return value:
	// 0 = Unknown
	// 1 = Windows Media Player
	// 2 = Flash

	// Determine MIME type
	int nResult = 0;
	HRESULT hr;
	LPWSTR pStrMimeType = NULL;
	
	hr = FindMimeFromData(NULL, URL, NULL, 0, NULL, 0, &pStrMimeType, 0);
	if(hr == NOERROR)
	{
		if(0 == wcsicmp(pStrMimeType, L"application/x-shockwave-flash"))
			return 2;
		else
			return 1;
	}
	
	return nResult;
}

void CDlgTV::ShowPlayer(int nPlayer)
{
	m_flash.ShowWindow(SW_HIDE);
	m_mediaplayer.ShowWindow(SW_HIDE);

	if(nPlayer == 1)
	{
		m_mediaplayer.ShowWindow(SW_SHOW);
	}
	if(nPlayer == 2)
	{
		m_flash.ShowWindow(SW_SHOW);
	}
}

void CDlgTV::MMStop()
{
	m_flash.Stop();
	m_mediaplayer.Stop();
}

void CDlgTV::MMOpen(BSTR URL)
{
	int nPrevPlayer = m_nPlayer;
	if(nPrevPlayer > 0)
		MMStop();

	m_nPlayer = GetPlayer(URL);
	ShowPlayer(m_nPlayer);
	
	if(m_nPlayer == 1)
		m_mediaplayer.Open(CString(URL));
	else if(m_nPlayer == 2)
	{
		m_flash.SetMovie(_T(""));
		m_flash.SetMovie(CString(URL));
	}
	if(nPrevPlayer > 0)
		MMPlay();
}

void CDlgTV::MMPlay()
{
	if(m_nPlayer == 1)
		m_mediaplayer.Play();
	else if(m_nPlayer == 2)
		m_flash.Play();
}

void CDlgTV::SetFrameSize(long cx, long cy)
{
	if(cx <= 0 || cy <= 0)
		return;

	CRect rw, rp;
	GetWindowRect(&rw);
	m_mediaplayer.GetWindowRect(&rp);
	rw.InflateRect(0, 0, cx - rp.Width(), cy - rp.Height());
	AdjustRect(rw);
	SetWindowPos(NULL, rw.left, rw.top, rw.Width(), rw.Height(), SWP_NOZORDER);
}
