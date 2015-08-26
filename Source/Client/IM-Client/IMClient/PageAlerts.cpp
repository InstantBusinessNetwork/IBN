// PageAlerts.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "PageAlerts.h"
#include "GlobalFunction.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CPageAlerts dialog


CPageAlerts::CPageAlerts(LPCTSTR szTitle)
	: CMcSettingsPage(CPageAlerts::IDD, szTitle)
{
	//{{AFX_DATA_INIT(CPageAlerts)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
}


void CPageAlerts::DoDataExchange(CDataExchange* pDX)
{
	CMcSettingsPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CPageAlerts)
	DDX_Control(pDX, IDC_ALERT_LIST, m_AlertList);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CPageAlerts, CMcSettingsPage)
//{{AFX_MSG_MAP(CPageAlerts)
	ON_NOTIFY(NM_CLICK, IDC_ALERT_LIST, OnClickAlertList)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

static UINT IdItemMap[]	=	
{
	IDS_MAIN,
	IDS_INVALID_LOGIN_OR_PASSWORD,
	IDS_SYNCHRONIZE,
	IDS_USER_OFFLINE,
	IDS_BAD_FILE,
	IDS_SERVICENOTAVAILABLE
};

/////////////////////////////////////////////////////////////////////////////
// CPageAlerts message handlers

BOOL CPageAlerts::OnInitDialog() 
{
	CMcSettingsPage::OnInitDialog();
	
	DWORD dwStyle = m_AlertList.GetExtendedStyle();
	dwStyle |= LVS_EX_FULLROWSELECT|LVS_EX_CHECKBOXES;
	m_AlertList.SetExtendedStyle(dwStyle);
	
	CRect	listRect;
	m_AlertList.GetWindowRect(&listRect);
	m_AlertList.InsertColumn(0,_T(""),LVCFMT_LEFT,listRect.Width()-10);
	
	for(int i=0;i<sizeof(IdItemMap)/sizeof(IdItemMap[0]);i++)
	{
		CString strText;
		strText.Format(GetString(IDS_SHOW_ALERT_MESAGE_FORMAT),GetString(IdItemMap[i]));
		m_AlertList.InsertItem(i,strText);
		m_AlertList.SetCheck(i,!GetOptionInt(IDS_MESSAGES,IdItemMap[i],0));
	}
	
	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

BOOL CPageAlerts::SaveSettings()
{
	for(int i=0;i<sizeof(IdItemMap)/sizeof(IdItemMap[0]);i++)
	{
		WriteOptionInt(IDS_MESSAGES,IdItemMap[i],!m_AlertList.GetCheck(i));
	}

	return CMcSettingsPage::SaveSettings();
}

void CPageAlerts::OnClickAlertList(NMHDR* pNMHDR, LRESULT* pResult) 
{
	*pResult = 0;
	SetModified();
}
