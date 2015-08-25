// PageStatusMode.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "PageStatusMode.h"
#include "GlobalFunction.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CPageStatusMode dialog


CPageStatusMode::CPageStatusMode(LPCTSTR szTitle)
	: CMcSettingsPage(CPageStatusMode::IDD, szTitle)
{
	//{{AFX_DATA_INIT(CPageStatusMode)
	m_SetAway = FALSE;
	m_SetNA = FALSE;
	m_SetNAMinute = 0;
	m_SetAwayMinute = 0;
	//}}AFX_DATA_INIT
}


void CPageStatusMode::DoDataExchange(CDataExchange* pDX)
{
	CMcSettingsPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CPageStatusMode)
	DDX_Control(pDX, IDC_SETNA_SPIN, m_SetNASpin);
	DDX_Control(pDX, IDC_SETAWAY_SPIN, m_SetAwaySpin);
	DDX_Check(pDX, IDC_SETAWAY_CHECK, m_SetAway);
	DDX_Check(pDX, IDC_SETNA_CHECK, m_SetNA);
	DDX_Text(pDX, IDC_SETNAMINUTES_EDIT, m_SetNAMinute);
	DDV_MinMaxLong(pDX, m_SetNAMinute, 1, 60);
	DDX_Text(pDX, IDC_SETAWAYMINUTES_EDIT, m_SetAwayMinute);
	DDV_MinMaxLong(pDX, m_SetAwayMinute, 1, 60);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CPageStatusMode, CMcSettingsPage)
	//{{AFX_MSG_MAP(CPageStatusMode)
	ON_BN_CLICKED(IDC_SETAWAY_CHECK, OnSetawayCheck)
	ON_BN_CLICKED(IDC_SETNA_CHECK, OnSetnaCheck)
	ON_EN_CHANGE(IDC_SETAWAYMINUTES_EDIT, OnChangeSetawayminutesEdit)
	ON_EN_CHANGE(IDC_SETNAMINUTES_EDIT, OnChangeSetnaminutesEdit)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CPageStatusMode message handlers

BOOL CPageStatusMode::OnInitDialog() 
{
	CMcSettingsPage::OnInitDialog();
	
	m_SetAway	=	GetOptionInt(IDS_OFSMESSENGER,IDS_SETAWAY_ENABLE,TRUE);
	m_SetAwayMinute = GetOptionInt(IDS_OFSMESSENGER,IDS_SETAWAY,5);
	m_SetAwaySpin.SetRange(1,60);
	m_SetAwaySpin.SetBuddy(GetDlgItem(IDC_SETAWAYMINUTES_EDIT));
	
	m_SetNA	=	GetOptionInt(IDS_OFSMESSENGER,IDS_SETNA_ENABLE,FALSE);
	m_SetNAMinute = GetOptionInt(IDS_OFSMESSENGER,IDS_SETNA,10);
	m_SetNASpin.SetRange(1,60);
	m_SetNASpin.SetBuddy(GetDlgItem(IDC_SETNAMINUTES_EDIT));

	UpdateData(FALSE);

	Refresh();

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

BOOL CPageStatusMode::SaveSettings()
{
	UpdateData();

	WriteOptionInt(IDS_OFSMESSENGER,IDS_SETAWAY_ENABLE,m_SetAway);
	WriteOptionInt(IDS_OFSMESSENGER,IDS_SETAWAY,m_SetAwayMinute);
	
	WriteOptionInt(IDS_OFSMESSENGER,IDS_SETNA_ENABLE,m_SetNA);
	WriteOptionInt(IDS_OFSMESSENGER,IDS_SETNA,m_SetNAMinute);
	
	return CMcSettingsPage::SaveSettings();
}

void CPageStatusMode::OnSetawayCheck() 
{
	SetModified();
	UpdateData();
	Refresh();
}

void CPageStatusMode::OnSetnaCheck() 
{
	SetModified();
	UpdateData();
	Refresh();
}

void CPageStatusMode::Refresh()
{
	GetDlgItem(IDC_SETAWAYMINUTES_EDIT)->EnableWindow(m_SetAway);
	GetDlgItem(IDC_SETAWAY_SPIN)->EnableWindow(m_SetAway);
	///GetDlgItem(IDC_SETAWAY_STATIC)->EnableWindow(m_SetAway);

	GetDlgItem(IDC_SETNA_CHECK)->EnableWindow(m_SetAway);
	GetDlgItem(IDC_SETNAMINUTES_EDIT)->EnableWindow(m_SetAway&&m_SetNA);
	GetDlgItem(IDC_SETNA_SPIN)->EnableWindow(m_SetAway&&m_SetNA);
	//GetDlgItem(IDC_SETNA_STATIC)->EnableWindow(m_SetAway&&m_SetNA);
}

void CPageStatusMode::OnChangeSetawayminutesEdit() 
{
	SetModified();
}

void CPageStatusMode::OnChangeSetnaminutesEdit() 
{
	SetModified();
}
