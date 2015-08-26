// PageHistorySync.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "PageHistorySync.h"
#include "GlobalFunction.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CPageHistorySync dialog


CPageHistorySync::CPageHistorySync(LPCTSTR szTitle)
	: CMcSettingsPage(CPageHistorySync::IDD, szTitle)
{
	//{{AFX_DATA_INIT(CPageHistorySync)
	m_FromTime = GetOptionInt(IDS_HISTORY,IDS_FROMTIME,0);
	m_ToTime = GetOptionInt(IDS_HISTORY,IDS_TOTIME,time(NULL));
	//}}AFX_DATA_INIT
	Mode = GetOptionInt(IDS_HISTORY,IDS_SYNCMODE,0);
}


void CPageHistorySync::DoDataExchange(CDataExchange* pDX)
{
	CMcSettingsPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CPageHistorySync)
	DDX_DateTimeCtrl(pDX, IDC_FROM_DATETIMEPICKER, m_FromTime);
	DDX_DateTimeCtrl(pDX, IDC_TO_DATETIMEPICKER, m_ToTime);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CPageHistorySync, CMcSettingsPage)
	//{{AFX_MSG_MAP(CPageHistorySync)
	ON_BN_CLICKED(IDC_MONTH_RADIO, OnMonthRadio)
	ON_BN_CLICKED(IDC_WEEK_RADIO, OnWeekRadio)
	ON_BN_CLICKED(IDC_YEAR_RADIO, OnYearRadio)
	ON_BN_CLICKED(IDC_DAY_RADIO, OnDayRadio)
	ON_BN_CLICKED(IDC_CUSTOM_RADIO, OnCustomRadio)
	ON_NOTIFY(DTN_DATETIMECHANGE, IDC_FROM_DATETIMEPICKER, OnDateTimeChangeFrom)
	ON_NOTIFY(DTN_DATETIMECHANGE, IDC_TO_DATETIMEPICKER, OnDateTimeChangeTo)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CPageHistorySync message handlers

BOOL CPageHistorySync::SaveSettings()
{
	UpdateData();
	
	WriteOptionInt(IDS_HISTORY,IDS_SYNCMODE,Mode);
	WriteOptionInt(IDS_HISTORY,IDS_FROMTIME,m_FromTime.GetTime());
	WriteOptionInt(IDS_HISTORY,IDS_TOTIME, m_ToTime.GetTime());

	return CMcSettingsPage::SaveSettings();
}

BOOL CPageHistorySync::OnInitDialog() 
{
	CMcSettingsPage::OnInitDialog();
	
	CheckRadioButton(IDC_DAY_RADIO,IDC_CUSTOM_RADIO,IDC_DAY_RADIO + Mode);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CPageHistorySync::OnMonthRadio() 
{
	Mode = 2;
	SetModified();
}

void CPageHistorySync::OnWeekRadio() 
{
	Mode = 1;
	SetModified();
}

void CPageHistorySync::OnYearRadio() 
{
	Mode = 3;
	SetModified();
}

void CPageHistorySync::OnDayRadio() 
{
	Mode = 0;
	SetModified();
}

void CPageHistorySync::OnCustomRadio() 
{
	Mode = 4; 
	SetModified();
}

void CPageHistorySync::OnDateTimeChangeFrom(NMHDR* pNMHDR, LRESULT* pResult)
{
	SetModified();
}

void CPageHistorySync::OnDateTimeChangeTo(NMHDR* pNMHDR, LRESULT* pResult)
{
	SetModified();
}
