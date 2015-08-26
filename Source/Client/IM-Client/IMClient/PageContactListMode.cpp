// PageContactListMode.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "PageContactListMode.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CPageContactListMode dialog


CPageContactListMode::CPageContactListMode(LPCTSTR szTitle)
: CMcSettingsPage(CPageContactListMode::IDD, szTitle)
{
	//{{AFX_DATA_INIT(CPageContactListMode)
	m_bShowOffline = TRUE;
	//}}AFX_DATA_INIT
	CLMode = 1;
}


void CPageContactListMode::DoDataExchange(CDataExchange* pDX)
{
	CMcSettingsPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CPageContactListMode)
	DDX_Check(pDX, IDC_SHOWOFFLINECHECK, m_bShowOffline);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CPageContactListMode, CMcSettingsPage)
	//{{AFX_MSG_MAP(CPageContactListMode)
	ON_BN_CLICKED(IDC_CLMODE1_RADIO, OnClmode1Radio)
	ON_BN_CLICKED(IDC_CLMODE2_RADIO, OnClmode2Radio)
	ON_BN_CLICKED(IDC_SHOWOFFLINECHECK, OnShowofflinecheck)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CPageContactListMode message handlers

BOOL CPageContactListMode::OnInitDialog() 
{
	CMcSettingsPage::OnInitDialog();
	
	CLMode			= GetOptionInt(IDS_OFSMESSENGER,IDS_CLMODE,2);
	m_bShowOffline  =  GetOptionInt(IDS_OFSMESSENGER,IDS_SHOWOFFLINEFILES,TRUE);
	CheckRadioButton(IDC_CLMODE1_RADIO,IDC_CLMODE2_RADIO,IDC_CLMODE1_RADIO+CLMode-1);

	UpdateData(FALSE);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}


BOOL CPageContactListMode::SaveSettings()
{
	UpdateData();
	
	WriteOptionInt(IDS_OFSMESSENGER,IDS_CLMODE,CLMode);
	WriteOptionInt(IDS_OFSMESSENGER,IDS_SHOWOFFLINEFILES,m_bShowOffline);
	
	return CMcSettingsPage::SaveSettings();
}

void CPageContactListMode::OnClmode1Radio() 
{
	CLMode	=	1;
	SetModified();
}

void CPageContactListMode::OnClmode2Radio() 
{
	CLMode	=	2;
	SetModified();
}

void CPageContactListMode::OnShowofflinecheck() 
{
	SetModified();
}
