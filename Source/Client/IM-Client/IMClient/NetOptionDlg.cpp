// NetOptionDlg.cpp : implementation file
//

#include "stdafx.h"
#include "OfsTv.h"
#include "NetOptionDlg.h"
#include "GlobalFunction.h"


#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CNetOptionDlg dialog


CNetOptionDlg::CNetOptionDlg(CWnd* pParent /*=NULL*/)
	: COFSNcDlg(CNetOptionDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CNetOptionDlg)
	m_ServerName = _T("");
	m_ServerPort = _T("");
	m_BypassCheck = FALSE;
	//}}AFX_DATA_INIT
}


void CNetOptionDlg::DoDataExchange(CDataExchange* pDX)
{
	COFSNcDlg::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CNetOptionDlg)
	DDX_Control(pDX, IDC_SERVERPORT, m_ServerPortEdit);
	DDX_Control(pDX, IDC_SERVERNAME, m_ServerNameEdit);
	DDX_Text(pDX, IDC_SERVERNAME, m_ServerName);
	DDX_Text(pDX, IDC_SERVERPORT, m_ServerPort);
	//DDX_Check(pDX, IDC_BYPASSCHECK, m_BypassCheck);
	DDX_Text(pDX, IDC_FIREWALLPASS, m_FireWallPass);
	DDX_Text(pDX, IDC_FIREWALLUSER, m_FireWallUser);
	DDX_Check(pDX, IDC_USEFIREWALL_CHECK, m_UserFireWall);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CNetOptionDlg, COFSNcDlg)
	//{{AFX_MSG_MAP(CNetOptionDlg)
	ON_BN_CLICKED(IDC_DEFAULT_RADIO, OnDefaultRadio)
	ON_BN_CLICKED(IDC_DIRECT_RADIO, OnDirectRadio)
	ON_BN_CLICKED(IDC_PROXY_RADIO, OnProxyRadio)
	ON_BN_CLICKED(IDC_USEFIREWALL_CHECK, OnUsefirewallCheck)
	ON_EN_CHANGE(IDC_FIREWALLUSER, OnChangeFirewalluser)
	ON_EN_ERRSPACE(IDC_FIREWALLPASS, OnErrspaceFirewallpass)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CNetOptionDlg message handlers

BOOL CNetOptionDlg::OnInitDialog() 
{
	COFSNcDlg::OnInitDialog();
	
    dwAccessType = GetOptionInt(IDS_NETOPTIONS,IDS_ACCESSTYPE,INTERNET_OPEN_TYPE_PRECONFIG);

	BOOL bShowProxy = TRUE;
	int ItemSelect  = IDC_DEFAULT_RADIO;

	switch(dwAccessType)
	{
	case INTERNET_OPEN_TYPE_PRECONFIG:
		ItemSelect = IDC_DEFAULT_RADIO;
		break;
	case INTERNET_OPEN_TYPE_DIRECT:
		ItemSelect = IDC_DIRECT_RADIO;
		break;
	case INTERNET_OPEN_TYPE_PROXY:
		ItemSelect = IDC_PROXY_RADIO;
		bShowProxy = FALSE;
		break;
	}

	CheckRadioButton(IDC_DEFAULT_RADIO,IDC_PROXY_RADIO,ItemSelect);
	
	m_ServerPort = GetOptionString(IDS_NETOPTIONS, IDS_PROXYPORT, _T(""));
	m_ServerName = GetOptionString(IDS_NETOPTIONS, IDS_PROXYNAME, _T(""));
	//m_BypassCheck = GetOptionInt(IDS_LOGIN,IDS_BYPASSENABLE,FALSE);
	m_UserFireWall = GetOptionInt(IDS_NETOPTIONS, IDS_USEFIREWALL, FALSE);
	m_FireWallUser = GetOptionString(IDS_NETOPTIONS, IDS_FIREWALLUSER, _T(""));
	m_FireWallPass = GetOptionString(IDS_NETOPTIONS, IDS_FIREWALLPASS, _T(""));

	m_ServerPortEdit.SetReadOnly(bShowProxy);
	m_ServerNameEdit.SetReadOnly(bShowProxy);
	GetDlgItem(IDC_USEFIREWALL_CHECK)->EnableWindow(dwAccessType!=INTERNET_OPEN_TYPE_DIRECT);
	GetDlgItem(IDC_FIREWALLUSER)->EnableWindow(dwAccessType!=INTERNET_OPEN_TYPE_DIRECT&&m_UserFireWall);
	GetDlgItem(IDC_FIREWALLPASS)->EnableWindow(dwAccessType!=INTERNET_OPEN_TYPE_DIRECT&&m_UserFireWall);	
	
	UpdateData(FALSE);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CNetOptionDlg::OnOK() 
{
	UpdateData();

	WriteOptionInt(IDS_NETOPTIONS,IDS_ACCESSTYPE,dwAccessType);
	WriteOptionString(IDS_NETOPTIONS,IDS_PROXYNAME,m_ServerName);
	WriteOptionString(IDS_NETOPTIONS,IDS_PROXYPORT,m_ServerPort);
	//WriteOptionInt(IDS_LOGIN,IDS_BYPASSENABLE,m_BypassCheck);
	WriteOptionInt(IDS_NETOPTIONS,IDS_USEFIREWALL,m_UserFireWall);
	WriteOptionString(IDS_NETOPTIONS,IDS_FIREWALLUSER,m_FireWallUser);
	WriteOptionString(IDS_NETOPTIONS,IDS_FIREWALLPASS,m_FireWallPass);
	
	COFSNcDlg::OnOK();
}

void CNetOptionDlg::OnDefaultRadio() 
{
	dwAccessType = INTERNET_OPEN_TYPE_PRECONFIG;
	m_ServerPortEdit.SetReadOnly(TRUE);
	m_ServerNameEdit.SetReadOnly(TRUE);
	GetDlgItem(IDC_USEFIREWALL_CHECK)->EnableWindow(TRUE);
	GetDlgItem(IDC_FIREWALLUSER)->EnableWindow(m_UserFireWall);
	GetDlgItem(IDC_FIREWALLPASS)->EnableWindow(m_UserFireWall);	
}

void CNetOptionDlg::OnDirectRadio() 
{
	dwAccessType = INTERNET_OPEN_TYPE_DIRECT;
	m_ServerPortEdit.SetReadOnly(TRUE);
	m_ServerNameEdit.SetReadOnly(TRUE);
	GetDlgItem(IDC_USEFIREWALL_CHECK)->EnableWindow(FALSE);
	GetDlgItem(IDC_FIREWALLUSER)->EnableWindow(FALSE);
	GetDlgItem(IDC_FIREWALLPASS)->EnableWindow(FALSE);	
}

void CNetOptionDlg::OnProxyRadio() 
{
	dwAccessType = INTERNET_OPEN_TYPE_PROXY;
	m_ServerPortEdit.SetReadOnly(FALSE);
	m_ServerNameEdit.SetReadOnly(FALSE);
	GetDlgItem(IDC_USEFIREWALL_CHECK)->EnableWindow(TRUE);
	GetDlgItem(IDC_FIREWALLUSER)->EnableWindow(m_UserFireWall);
	GetDlgItem(IDC_FIREWALLPASS)->EnableWindow(m_UserFireWall);	
}

void CNetOptionDlg::OnUsefirewallCheck() 
{
	UpdateData(TRUE);
	GetDlgItem(IDC_FIREWALLUSER)->EnableWindow(m_UserFireWall);
	GetDlgItem(IDC_FIREWALLPASS)->EnableWindow(m_UserFireWall);	
}

void CNetOptionDlg::OnChangeFirewalluser() 
{
}

void CNetOptionDlg::OnErrspaceFirewallpass() 
{
}

