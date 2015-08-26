// PageLaunchMode.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "PageLaunchMode.h"
#include "GlobalFunction.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CPageLaunchMode dialog


CPageLaunchMode::CPageLaunchMode(LPCTSTR szTitle)
	: CMcSettingsPage(CPageLaunchMode::IDD, szTitle)
{
	//{{AFX_DATA_INIT(CPageLaunchMode)
	m_bLaunchOnStartup = FALSE;
	m_bAutoLogon = GetOptionInt(IDS_OFSMESSENGER,IDS_AUTOLOGON,TRUE);
	m_ServerName = _T("");
	m_ServerPort = _T("");
	m_BypassCheck = FALSE;
	m_FireWallPass = _T("");
	m_FireWallUser = _T("");
	m_UserFireWall = FALSE;
	m_UseSSL		=	FALSE;
	//}}AFX_DATA_INIT
	HKEY  hKey = NULL;
	if(RegOpenKeyEx(HKEY_CURRENT_USER,_T("Software\\Microsoft\\Windows\\CurrentVersion\\Run"),0,KEY_READ,&hKey)==ERROR_SUCCESS)
	{
		DWORD	dwDataType			=	REG_SZ;
		TCHAR	strBuff[MAX_PATH]	=	_T("");
		DWORD   dwBuffSize			=	MAX_PATH;
		if(RegQueryValueEx(hKey,GetString(IDR_MAINFRAME),0,&dwDataType,(LPBYTE)&strBuff,&dwBuffSize)==ERROR_SUCCESS)
		{
			m_bLaunchOnStartup = TRUE;
		}
		RegCloseKey(hKey);
	}
}


void CPageLaunchMode::DoDataExchange(CDataExchange* pDX)
{
	CMcSettingsPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CPageLaunchMode)
	DDX_Check(pDX, IDC_LAUNCHONSTART_CHECK, m_bLaunchOnStartup);
	DDX_Check(pDX, IDC_AUTOCONNECT_CHECK, m_bAutoLogon);
	DDX_Control(pDX, IDC_SERVERPORT, m_ServerPortEdit);
	DDX_Control(pDX, IDC_SERVERNAME, m_ServerNameEdit);
	DDX_Text(pDX, IDC_SERVERNAME, m_ServerName);
	DDX_Text(pDX, IDC_SERVERPORT, m_ServerPort);
	//DDX_Check(pDX, IDC_BYPASSCHECK, m_BypassCheck);
	DDX_Text(pDX, IDC_FIREWALLPASS, m_FireWallPass);
	DDX_Text(pDX, IDC_FIREWALLUSER, m_FireWallUser);
	DDX_Check(pDX, IDC_USEFIREWALL_CHECK, m_UserFireWall);
	DDX_Check(pDX, IDC_USESSL_CHECK, m_UseSSL);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CPageLaunchMode, CMcSettingsPage)
//{{AFX_MSG_MAP(CPageLaunchMode)
ON_BN_CLICKED(IDC_AUTOCONNECT_CHECK, OnAutoconnectCheck)
ON_BN_CLICKED(IDC_LAUNCHONSTART_CHECK, OnLaunchonstartCheck)
ON_BN_CLICKED(IDC_DEFAULT_RADIO, OnDefaultRadio)
ON_BN_CLICKED(IDC_DIRECT_RADIO, OnDirectRadio)
ON_BN_CLICKED(IDC_PROXY_RADIO, OnProxyRadio)
	ON_BN_CLICKED(IDC_USEFIREWALL_CHECK, OnUsefirewallCheck)
	ON_BN_CLICKED(IDS_USESSL, OnUseSSLCheck)
	ON_EN_CHANGE(IDC_FIREWALLUSER, OnChangeFirewalluser)
	ON_EN_ERRSPACE(IDC_FIREWALLPASS, OnErrspaceFirewallpass)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CPageLaunchMode message handlers

BOOL CPageLaunchMode::OnInitDialog() 
{
	CMcSettingsPage::OnInitDialog();
	
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

	m_UseSSL	   = GetOptionInt(IDS_NETOPTIONS, IDS_USESSL, FALSE);

//	m_ServerPortEdit.SetReadOnly(bShowProxy);
//	m_ServerNameEdit.SetReadOnly(bShowProxy);
	GetDlgItem(IDC_SERVERNAME)->EnableWindow(!bShowProxy);
	GetDlgItem(IDC_SERVERPORT)->EnableWindow(!bShowProxy);
	GetDlgItem(IDC_USEFIREWALL_CHECK)->EnableWindow(dwAccessType!=INTERNET_OPEN_TYPE_DIRECT);
	GetDlgItem(IDC_FIREWALLUSER)->EnableWindow(dwAccessType!=INTERNET_OPEN_TYPE_DIRECT&&m_UserFireWall);
	GetDlgItem(IDC_FIREWALLPASS)->EnableWindow(dwAccessType!=INTERNET_OPEN_TYPE_DIRECT&&m_UserFireWall);	
	
	UpdateData(FALSE);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CPageLaunchMode::OnAutoconnectCheck() 
{
	SetModified();
}

void CPageLaunchMode::OnLaunchonstartCheck() 
{
	SetModified();
}

BOOL CPageLaunchMode::SaveSettings()
{
	UpdateData();
	
	WriteOptionInt(IDS_OFSMESSENGER,IDS_AUTOLOGON,m_bAutoLogon);

	HKEY  hKey = NULL;
	LONG lVal = 0;
	if(RegOpenKeyEx(HKEY_CURRENT_USER,_T("Software\\Microsoft\\Windows\\CurrentVersion\\Run"),0,KEY_ALL_ACCESS,&hKey)==ERROR_SUCCESS)
	{
		DWORD	dwDataType			=	REG_SZ;
		TCHAR	strBuff[MAX_PATH]	=	_T("");
		DWORD   dwBuffSize			=	MAX_PATH;

		if(m_bLaunchOnStartup)
		{
			CString strPath;
			LPTSTR pstrFN = strPath.GetBuffer (MAX_PATH);
			GetModuleFileName(0,pstrFN,MAX_PATH);
			strPath.ReleaseBuffer();

			strPath += _T(" -silent");
			
			RegSetValueEx(hKey,GetString(IDR_MAINFRAME),0,REG_SZ,(const LPBYTE)(LPCTSTR)strPath,strPath.GetLength());
		}
		else
		{
			RegDeleteValue(hKey,GetString(IDR_MAINFRAME));
		}
		
		RegCloseKey(hKey);
	}

	WriteOptionInt(IDS_NETOPTIONS,IDS_ACCESSTYPE,dwAccessType);
	WriteOptionString(IDS_NETOPTIONS,IDS_PROXYNAME,m_ServerName);
	WriteOptionString(IDS_NETOPTIONS,IDS_PROXYPORT,m_ServerPort);
	//WriteOptionInt(IDS_LOGIN,IDS_BYPASSENABLE,m_BypassCheck);
	WriteOptionInt(IDS_NETOPTIONS,IDS_USEFIREWALL,m_UserFireWall);
	WriteOptionString(IDS_NETOPTIONS,IDS_FIREWALLUSER,m_FireWallUser);
	WriteOptionString(IDS_NETOPTIONS,IDS_FIREWALLPASS,m_FireWallPass);
	WriteOptionInt(IDS_NETOPTIONS, IDS_USESSL, m_UseSSL);

	
	return CMcSettingsPage::SaveSettings();
}

void CPageLaunchMode::OnDefaultRadio() 
{
	SetModified();
	dwAccessType = INTERNET_OPEN_TYPE_PRECONFIG;
//	m_ServerPortEdit.SetReadOnly(TRUE);
//	m_ServerNameEdit.SetReadOnly(TRUE);
	GetDlgItem(IDC_SERVERNAME)->EnableWindow(FALSE);
	GetDlgItem(IDC_SERVERPORT)->EnableWindow(FALSE);
	GetDlgItem(IDC_USEFIREWALL_CHECK)->EnableWindow(TRUE);
	GetDlgItem(IDC_FIREWALLUSER)->EnableWindow(m_UserFireWall);
	GetDlgItem(IDC_FIREWALLPASS)->EnableWindow(m_UserFireWall);	
}

void CPageLaunchMode::OnDirectRadio() 
{
	SetModified();
	dwAccessType = INTERNET_OPEN_TYPE_DIRECT;
//	m_ServerPortEdit.SetReadOnly(TRUE);
//	m_ServerNameEdit.SetReadOnly(TRUE);
	GetDlgItem(IDC_SERVERNAME)->EnableWindow(FALSE);
	GetDlgItem(IDC_SERVERPORT)->EnableWindow(FALSE);
	
	GetDlgItem(IDC_USEFIREWALL_CHECK)->EnableWindow(FALSE);
	GetDlgItem(IDC_FIREWALLUSER)->EnableWindow(FALSE);
	GetDlgItem(IDC_FIREWALLPASS)->EnableWindow(FALSE);	
}

void CPageLaunchMode::OnProxyRadio() 
{
	SetModified();
	dwAccessType = INTERNET_OPEN_TYPE_PROXY;
//	m_ServerPortEdit.SetReadOnly(FALSE);
//	m_ServerNameEdit.SetReadOnly(FALSE);
	GetDlgItem(IDC_SERVERNAME)->EnableWindow(TRUE);
	GetDlgItem(IDC_SERVERPORT)->EnableWindow(TRUE);
	GetDlgItem(IDC_USEFIREWALL_CHECK)->EnableWindow(TRUE);
	GetDlgItem(IDC_FIREWALLUSER)->EnableWindow(m_UserFireWall);
	GetDlgItem(IDC_FIREWALLPASS)->EnableWindow(m_UserFireWall);	
}

void CPageLaunchMode::OnUsefirewallCheck() 
{
	SetModified();
	UpdateData(TRUE);
	GetDlgItem(IDC_FIREWALLUSER)->EnableWindow(m_UserFireWall);
	GetDlgItem(IDC_FIREWALLPASS)->EnableWindow(m_UserFireWall);	
}

void CPageLaunchMode::OnChangeFirewalluser() 
{
	SetModified();	
}

void CPageLaunchMode::OnErrspaceFirewallpass() 
{
	SetModified();
}

void CPageLaunchMode::OnUseSSLCheck()
{
	SetModified();
}
