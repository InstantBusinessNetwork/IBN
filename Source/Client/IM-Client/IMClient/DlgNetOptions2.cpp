// DlgNetOptions2.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "DlgNetOptions2.h"
#include "LoadSkins.h"
#include "cdib.h"
#include "GlobalFunction.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDlgNetOptions2 dialog


CDlgNetOptions2::CDlgNetOptions2(CWnd* pParent /*=NULL*/)
	: COFSNcDlg2(CDlgNetOptions2::IDD, pParent)
{
	//{{AFX_DATA_INIT(CDlgNetOptions2)
	m_strPass = _T("");
	m_strServ = _T("");
	m_strUser = _T("");
	m_strPort = _T("");
	//}}AFX_DATA_INIT
	m_strSkinSettings = _T("/Shell/NetOptions/skin.xml");
	m_bResizable = FALSE;
}


void CDlgNetOptions2::DoDataExchange(CDataExchange* pDX)
{
	COFSNcDlg2::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDlgNetOptions2)
	DDX_Control(pDX, IDC_USESSLCHECKBOX, m_checkssl);
	DDX_Control(pDX, IDC_EDIT_USER, m_edUser);
	DDX_Control(pDX, IDC_EDIT_SERV, m_edServ);
	DDX_Control(pDX, IDC_EDIT_PORT, m_edPort);
	DDX_Control(pDX, IDC_EDIT_PASS, m_edPass);
	DDX_Control(pDX, IDC_RADIO_1, m_radio1);
	DDX_Control(pDX, IDC_RADIO_2, m_radio2);
	DDX_Control(pDX, IDC_RADIO_3, m_radio3);
	DDX_Control(pDX, IDC_CHECKBOX, m_checkbox);
	DDX_Text(pDX, IDC_EDIT_PASS, m_strPass);
	DDX_Text(pDX, IDC_EDIT_SERV, m_strServ);
	DDX_Text(pDX, IDC_EDIT_USER, m_strUser);
	DDX_Control(pDX, IDC_BUTTON_X, m_btnX);
	DDX_Control(pDX, IDC_BUTTON_OK, m_btnOK);
	DDX_Control(pDX, IDC_BUTTON_CANCEL, m_btnCancel);
	DDX_Text(pDX, IDC_EDIT_PORT, m_strPort);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CDlgNetOptions2, COFSNcDlg2)
//{{AFX_MSG_MAP(CDlgNetOptions2)
	ON_WM_LBUTTONDOWN()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CDlgNetOptions2 message handlers

void CDlgNetOptions2::OnOK() 
{
	UpdateData();
	
	WriteOptionInt(IDS_NETOPTIONS, IDS_ACCESSTYPE, m_dwAccessType);
	WriteOptionString(IDS_NETOPTIONS, IDS_PROXYNAME, m_strServ);
	WriteOptionString(IDS_NETOPTIONS, IDS_PROXYPORT, m_strPort);
	//WriteOptionInt(IDS_LOGIN,IDS_BYPASSENABLE,m_BypassCheck);
	WriteOptionInt(IDS_NETOPTIONS, IDS_USEFIREWALL, m_bUseFirewall);
	WriteOptionString(IDS_NETOPTIONS, IDS_FIREWALLUSER, m_strUser);
	WriteOptionString(IDS_NETOPTIONS, IDS_FIREWALLPASS, m_strPass);
	WriteOptionInt(IDS_NETOPTIONS, IDS_USESSL, m_bUseSSL);

	OnClose();
	COFSNcDlg2::OnOK();
}

void CDlgNetOptions2::OnCancel() 
{
	OnClose();
	COFSNcDlg2::OnCancel();
}

BOOL CDlgNetOptions2::OnInitDialog() 
{
	COFSNcDlg2::OnInitDialog();

	//////////////////////////////////////////////////////////////////////////
	// McToolTipAddon
	m_ToolTip.AddTool(&m_btnX,IDS_TIP_CLOSE);
	//////////////////////////////////////////////////////////////////////////
	
	
    m_dwAccessType = GetOptionInt(IDS_NETOPTIONS,IDS_ACCESSTYPE,INTERNET_OPEN_TYPE_PRECONFIG);
	
	m_strServ = GetOptionString(IDS_NETOPTIONS, IDS_PROXYNAME, _T(""));
	m_strPort = GetOptionString(IDS_NETOPTIONS, IDS_PROXYPORT, _T(""));
	m_bUseFirewall = GetOptionInt(IDS_NETOPTIONS, IDS_USEFIREWALL, FALSE);
	m_strUser = GetOptionString(IDS_NETOPTIONS, IDS_FIREWALLUSER, _T(""));
	m_strPass = GetOptionString(IDS_NETOPTIONS, IDS_FIREWALLPASS, _T(""));

	m_bUseSSL = GetOptionInt(IDS_NETOPTIONS, IDS_USESSL, FALSE);
	
	UpdateControls();
	UpdateData(FALSE);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

BEGIN_EVENTSINK_MAP(CDlgNetOptions2, COFSNcDlg2)
//{{AFX_EVENTSINK_MAP(CDlgNetOptions2)
ON_EVENT(CDlgNetOptions2, IDC_BUTTON_X, -600 /* Click */, OnClickButtonX, VTS_NONE)
	ON_EVENT(CDlgNetOptions2, IDC_RADIO_1, -600 /* Click */, OnClickRadio1, VTS_NONE)
	ON_EVENT(CDlgNetOptions2, IDC_RADIO_2, -600 /* Click */, OnClickRadio2, VTS_NONE)
	ON_EVENT(CDlgNetOptions2, IDC_RADIO_3, -600 /* Click */, OnClickRadio3, VTS_NONE)
	ON_EVENT(CDlgNetOptions2, IDC_CHECKBOX, -600 /* Click */, OnClickCheckbox, VTS_NONE)
	ON_EVENT(CDlgNetOptions2, IDC_BUTTON_OK, -600 /* Click */, OnClickButtonOk, VTS_NONE)
	ON_EVENT(CDlgNetOptions2, IDC_BUTTON_CANCEL, -600 /* Click */, OnClickButtonCancel, VTS_NONE)
	ON_EVENT(CDlgNetOptions2, IDC_USESSLCHECKBOX, -600 /* Click */, OnClickUseSSL, VTS_NONE)
	//}}AFX_EVENTSINK_MAP
END_EVENTSINK_MAP()

void CDlgNetOptions2::OnClickButtonX() 
{
	OnCancel();
}

void CDlgNetOptions2::OnClickUseSSL()
{
	m_bUseSSL = !m_bUseSSL;
}

void CDlgNetOptions2::OnLButtonDown(UINT nFlags, CPoint point) 
{
    PostMessage(WM_NCLBUTTONDOWN,(WPARAM)HTCAPTION,MAKELPARAM(point.x,point.y));
	COFSNcDlg2::OnLButtonDown(nFlags, point);
}

void CDlgNetOptions2::UpdateControls()
{
	m_radio1.SetPressed(m_dwAccessType == INTERNET_OPEN_TYPE_PRECONFIG);
	m_radio2.SetPressed(m_dwAccessType == INTERNET_OPEN_TYPE_DIRECT);
	m_radio3.SetPressed(m_dwAccessType == INTERNET_OPEN_TYPE_PROXY);

	m_edPort.EnableWindow(m_dwAccessType == INTERNET_OPEN_TYPE_PROXY);
	m_edServ.EnableWindow(m_dwAccessType == INTERNET_OPEN_TYPE_PROXY);

	m_checkbox.SetEnabled(m_dwAccessType != INTERNET_OPEN_TYPE_DIRECT);
	m_checkbox.SetPressed(m_bUseFirewall);
	m_edUser.EnableWindow(m_bUseFirewall && m_dwAccessType != INTERNET_OPEN_TYPE_DIRECT);
	m_edPass.EnableWindow(m_bUseFirewall && m_dwAccessType != INTERNET_OPEN_TYPE_DIRECT);

	m_checkssl.SetPressed(m_bUseSSL);
}

void CDlgNetOptions2::OnClickRadio1() 
{
	m_dwAccessType = INTERNET_OPEN_TYPE_PRECONFIG;
	UpdateControls();
}

void CDlgNetOptions2::OnClickRadio2() 
{
	m_dwAccessType = INTERNET_OPEN_TYPE_DIRECT;
	UpdateControls();
}

void CDlgNetOptions2::OnClickRadio3() 
{
	m_dwAccessType = INTERNET_OPEN_TYPE_PROXY;
	UpdateControls();
}

void CDlgNetOptions2::OnClickCheckbox() 
{
	m_bUseFirewall = !m_bUseFirewall;
	UpdateControls();
}

void CDlgNetOptions2::OnClickButtonOk() 
{
	OnOK();
}

void CDlgNetOptions2::OnClickButtonCancel() 
{
	OnCancel();
}

void CDlgNetOptions2::LoadSkin(IXMLDOMNode *pXmlRoot)
{
	LoadButton(pXmlRoot, _T("X"), &m_btnX, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("OK"), &m_btnOK, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Cancel"), &m_btnCancel, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("ProxyDefault"), &m_radio1, FALSE, TRUE);
	LoadButton(pXmlRoot, _T("ProxyDirect"), &m_radio2, FALSE, TRUE);
	LoadButton(pXmlRoot, _T("ProxyCustom"), &m_radio3, FALSE, TRUE);
	LoadButton(pXmlRoot, _T("UseAuth"), &m_checkbox, TRUE, TRUE);
	LoadButton(pXmlRoot, _T("UseSSL"), &m_checkssl, TRUE, TRUE);

	LoadRectangle(pXmlRoot, _T("EditServer"), &m_edServ, TRUE);
	LoadRectangle(pXmlRoot, _T("EditPort"), &m_edPort, TRUE);
	LoadRectangle(pXmlRoot, _T("EditUser"), &m_edUser, TRUE);
	LoadRectangle(pXmlRoot, _T("EditPassword"), &m_edPass, TRUE);
	
}
