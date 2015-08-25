// DlgAbout.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "DlgAbout.h"
#include "McVersionInfo.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDlgAbout dialog


CDlgAbout::CDlgAbout(CWnd* pParent /*=NULL*/)
	: COFSNcDlg2(CDlgAbout::IDD, pParent)
{
	//{{AFX_DATA_INIT(CDlgAbout)
	//}}AFX_DATA_INIT
	m_strSkinSettings = _T("/Shell/About/skin.xml");
	m_bResizable = FALSE;
}


void CDlgAbout::DoDataExchange(CDataExchange* pDX)
{
	COFSNcDlg2::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDlgAbout)
	DDX_Control(pDX, IDC_VERSION_STATIC, m_Version);
	DDX_Control(pDX, IDC_SERVER_VERSION_STATIC, m_ServerVersion);
	DDX_Control(pDX, IDC_BTN_X, m_btnX);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CDlgAbout, COFSNcDlg2)
//{{AFX_MSG_MAP(CDlgAbout)
	ON_WM_PAINT()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CDlgAbout message handlers
//DEL CDlgAbout::CDlgAbout() : COFSNcDlg2(CDlgAbout::IDD)
//DEL {
//DEL }

BEGIN_EVENTSINK_MAP(CDlgAbout, COFSNcDlg2)
//{{AFX_EVENTSINK_MAP(CDlgAbout)
	ON_EVENT(CDlgAbout, IDC_BTN_X, -600 /* Click */, OnClickBtnX, VTS_NONE)
	//}}AFX_EVENTSINK_MAP
END_EVENTSINK_MAP()

void CDlgAbout::LoadSkin(IXMLDOMNode *pXmlRoot)
{
	m_btnX.ShowWindow(SW_HIDE);
	LoadButton(pXmlRoot, _T("X"), &m_btnX, TRUE, FALSE);
//	LoadRectangle2(pXmlRoot, _T("Frame"), m_frame.GetSafeHwnd(), FALSE);
	LoadLabel(pXmlRoot, _T("Version"), &m_Version, TRUE);
	LoadLabel(pXmlRoot, _T("ServerVersion"), &m_ServerVersion, TRUE);
}

BOOL CDlgAbout::OnInitDialog() 
{
	COFSNcDlg2::OnInitDialog();
	
	//////////////////////////////////////////////////////////////////////////
	// McToolTipAddon
	m_ToolTip.AddTool(&m_btnX,IDS_TIP_CLOSE);
	//////////////////////////////////////////////////////////////////////////
	
	CMcVersionInfo	VerInfo;

	CString strVersion;
	strVersion.Format(GetString(IDS_VERSION_FORMAT),(VerInfo.GetProductVersionMS()&0xffff0000)>>16,VerInfo.GetProductVersionMS()&0x0000ffff,(VerInfo.GetProductVersionLS()&0xffff0000)>>16);
	m_Version.SetText(strVersion);

	m_ServerVersion.SetText(GetString(IDS_SERVER_VERSION));
	
	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

//DEL BOOL CDlgAbout::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message) 
//DEL {
//DEL 	CRect r;
//DEL 	m_frame.GetWindowRect(&r);
//DEL 	CPoint pt;
//DEL 	::GetCursorPos(&pt);
//DEL 	if(r.PtInRect(pt))
//DEL 	{
//DEL 		SetCursor(AfxGetApp()->LoadCursor(IDC_HANDCURSOR));
//DEL 		return TRUE;
//DEL 	}
//DEL 	
//DEL 	return COFSNcDlg2::OnSetCursor(pWnd, nHitTest, message);
//DEL }

//DEL void CDlgAbout::OnLButtonDown(UINT nFlags, CPoint point) 
//DEL {
//DEL 	CPoint pt = point;
//DEL 	ClientToScreen(&pt);
//DEL 	
//DEL 	CRect r;
//DEL 	m_frame.GetWindowRect(&r);
//DEL 				
//DEL 	if(r.PtInRect(pt))
//DEL 	{
//DEL 		OnCancel();
//DEL 		return;
//DEL 	}
//DEL 	
//DEL 	COFSNcDlg2::OnLButtonDown(nFlags, point);
//DEL }

void CDlgAbout::OnClickBtnX() 
{
	OnCancel();
}

void CDlgAbout::OnPaint() 
{
	COFSNcDlg2::OnPaint();
	m_Version.Invalidate();
}
