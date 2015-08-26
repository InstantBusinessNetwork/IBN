// PageDialogMode.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "PageDialogMode.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CPageDialogMode dialog


CPageDialogMode::CPageDialogMode(LPCTSTR szTitle)
	: CMcSettingsPage(CPageDialogMode::IDD, szTitle)
{
	//{{AFX_DATA_INIT(CPageDialogMode)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
}


void CPageDialogMode::DoDataExchange(CDataExchange* pDX)
{
	CMcSettingsPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CPageDialogMode)
		// NOTE: the ClassWizard will add DDX and DDV calls here
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CPageDialogMode, CMcSettingsPage)
	//{{AFX_MSG_MAP(CPageDialogMode)
	ON_BN_CLICKED(IDC_ICQ_RADIO, OnIcqRadio)
	ON_BN_CLICKED(IDC_YAHOO_RADIO, OnYahooRadio)
	ON_BN_CLICKED(IDC_MCMESSENGER_RADIO, OnMcmessengerRadio)
	ON_BN_CLICKED(IDC_MCMESSENGER_SUNDANCEEDITION_RADIO, OnMcmessengerSundanceeditionRadio)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CPageDialogMode message handlers

BOOL CPageDialogMode::SaveSettings() 
{
	WriteOptionInt(IDS_OFSMESSENGER,IDS_DIALOGMODE,DialogMode);
	
	return CMcSettingsPage::SaveSettings();
}

BOOL CPageDialogMode::OnInitDialog() 
{
	CMcSettingsPage::OnInitDialog();
	
	DialogMode = GetOptionInt(IDS_OFSMESSENGER,IDS_DIALOGMODE,DM_McMessengerSundanceEdition);

	CheckRadioButton(IDC_MCMESSENGER_SUNDANCEEDITION_RADIO,IDC_ICQ_RADIO,IDC_MCMESSENGER_SUNDANCEEDITION_RADIO + DialogMode);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CPageDialogMode::OnIcqRadio() 
{
	DialogMode  = DM_ICQ;
	SetModified();
	
}

void CPageDialogMode::OnYahooRadio() 
{
	DialogMode  = DM_Yahoo;
	SetModified();
}

void CPageDialogMode::OnMcmessengerRadio() 
{
	DialogMode  = DM_McMessenger;
	SetModified();
}

void CPageDialogMode::OnMcmessengerSundanceeditionRadio() 
{
	DialogMode  = DM_McMessengerSundanceEdition;
	SetModified();
}

