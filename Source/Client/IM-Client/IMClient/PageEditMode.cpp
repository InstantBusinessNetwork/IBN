// PageEditMode.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "PageEditMode.h"
#include "GlobalFunction.h"
#include "ChooseFolder.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CPageEditMode dialog


CPageEditMode::CPageEditMode(LPCTSTR UserRole, int UserId, LPCTSTR szTitle)
	: CMcSettingsPage(CPageEditMode::IDD, szTitle)
{
	//{{AFX_DATA_INIT(CPageEditMode)
	m_bClearChatWindow = FALSE;
	m_bShowFriendStatus = FALSE;
	m_strReceivedFilePath = _T("");
	//}}AFX_DATA_INIT
	m_UserRole	=	UserRole;
	m_UserId	=	UserId;
}


void CPageEditMode::DoDataExchange(CDataExchange* pDX)
{
	CMcSettingsPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CPageEditMode)
	DDX_Check(pDX, IDC_CLEAR_CHATWINDOW_CHECK, m_bClearChatWindow);
	DDX_Check(pDX, IDC_SHOWFRIENDSTATUS_CHECK, m_bShowFriendStatus);
	DDX_Text(pDX, IDC_RECEIVEDFILESPATH_EDIT, m_strReceivedFilePath);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CPageEditMode, CMcSettingsPage)
	//{{AFX_MSG_MAP(CPageEditMode)
	ON_BN_CLICKED(IDC_SELECT_BUTTON, OnSelectButton)
	ON_BN_CLICKED(IDC_CLEAR_CHATWINDOW_CHECK, OnClearChatwindowCheck)
	ON_BN_CLICKED(IDC_SHOWFRIENDSTATUS_CHECK, OnShowfriendstatusCheck)
	ON_BN_CLICKED(IDC_ENTER_SEND_RADIO, OnEnterSendRadio)
	ON_BN_CLICKED(IDC_ENTER_NEWLINE_RADIO, OnEnterNewlineRadio)
	ON_BN_CLICKED(IDC_CTRLENTER_NEWLINE_RADIO, OnCtrlenterNewlineRadio)
	ON_BN_CLICKED(IDC_CTRLENTER_SEND_RADIO, OnCtrlenterSendRadio)
	ON_BN_CLICKED(IDC_SHIFTENTER_SEND_RADIO, OnShiftenterSendRadio)
	ON_BN_CLICKED(IDC_SHIFTENTER_NEWLINE_RADIO, OnShiftenterNewlineRadio)
	ON_BN_CLICKED(IDC_ALTS_SEND_RADIO, OnAltsSendRadio)
	ON_BN_CLICKED(IDC_ALTS_NEWLINE_RADIO, OnAltsNewlineRadio)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CPageEditMode message handlers

BOOL CPageEditMode::OnInitDialog() 
{
	CMcSettingsPage::OnInitDialog();
	
	CheckRadioButton(IDC_ENTER_SEND_RADIO,IDC_ENTER_NEWLINE_RADIO,IDC_ENTER_SEND_RADIO+GetOptionInt(IDS_OFSMESSENGER,IDS_ENTER,0));
	CheckRadioButton(IDC_CTRLENTER_SEND_RADIO,IDC_CTRLENTER_NEWLINE_RADIO,IDC_CTRLENTER_SEND_RADIO+GetOptionInt(IDS_OFSMESSENGER,IDS_CTRLENTER,1));
	//CheckRadioButton(IDC_ALTENTER_SEND_RADIO,IDC_ALTENTER_NEWLINE_RADIO,IDC_ALTENTER_SEND_RADIO+GetOptionInt(IDS_OFSMESSENGER,IDS_ALTENTER,0));
	CheckRadioButton(IDC_SHIFTENTER_SEND_RADIO,IDC_SHIFTENTER_NEWLINE_RADIO,IDC_SHIFTENTER_SEND_RADIO+GetOptionInt(IDS_OFSMESSENGER,IDS_SHIFTENTER,0));
	CheckRadioButton(IDC_ALTS_SEND_RADIO,IDC_ALTS_NEWLINE_RADIO,IDC_ALTS_SEND_RADIO+GetOptionInt(IDS_OFSMESSENGER,IDS_ALTS,0));
	//CheckRadioButton(IDC_CTRLS_SEND_RADIO,IDC_CTRLS_NEWLINE_RADIO,IDC_CTRLS_SEND_RADIO+GetOptionInt(IDS_OFSMESSENGER,IDS_CTRLS,0));

	if(m_UserRole.IsEmpty())
	{
		GetDlgItem(IDC_RECEIVEDFILESPATH_EDIT)->EnableWindow(FALSE);
		GetDlgItem(IDC_SELECT_BUTTON)->EnableWindow(FALSE);
	}
	
	m_bClearChatWindow = GetOptionInt(IDS_OFSMESSENGER,IDS_CLEARCHATWINDOW,TRUE);
	m_bShowFriendStatus = GetOptionInt(IDS_OFSMESSENGER,IDS_SHOWFRIENDSSTATUS,TRUE);
	m_strReceivedFilePath = GetMyDocumetPath(m_UserRole,m_UserId);
	
	UpdateData(FALSE);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

BOOL CPageEditMode::SaveSettings() 
{
	UpdateData(TRUE);

	WriteOptionInt(IDS_OFSMESSENGER,IDS_ENTER,GetCheckedRadioButton(IDC_ENTER_SEND_RADIO,IDC_ENTER_NEWLINE_RADIO)-IDC_ENTER_SEND_RADIO);
	WriteOptionInt(IDS_OFSMESSENGER,IDS_CTRLENTER,GetCheckedRadioButton(IDC_CTRLENTER_SEND_RADIO,IDC_CTRLENTER_NEWLINE_RADIO)-IDC_CTRLENTER_SEND_RADIO);
	//WriteOptionInt(IDS_OFSMESSENGER,IDS_ALTENTER,GetCheckedRadioButton(IDC_ALTENTER_SEND_RADIO,IDC_ALTENTER_NEWLINE_RADIO)-IDC_ALTENTER_SEND_RADIO);
	WriteOptionInt(IDS_OFSMESSENGER,IDS_SHIFTENTER,GetCheckedRadioButton(IDC_SHIFTENTER_SEND_RADIO,IDC_SHIFTENTER_NEWLINE_RADIO)-IDC_SHIFTENTER_SEND_RADIO);
	WriteOptionInt(IDS_OFSMESSENGER,IDS_ALTS,GetCheckedRadioButton(IDC_ALTS_SEND_RADIO,IDC_ALTS_NEWLINE_RADIO)-IDC_ALTS_SEND_RADIO);
	//WriteOptionInt(IDS_OFSMESSENGER,IDS_CTRLS,GetCheckedRadioButton(IDC_CTRLS_SEND_RADIO,IDC_CTRLS_NEWLINE_RADIO)-IDC_CTRLS_SEND_RADIO);
	
	WriteOptionInt(IDS_OFSMESSENGER,IDS_CLEARCHATWINDOW,m_bClearChatWindow);
	WriteOptionInt(IDS_OFSMESSENGER,IDS_SHOWFRIENDSSTATUS,m_bShowFriendStatus);
	
	SetMyDocumetPath(m_UserRole,m_UserId,m_strReceivedFilePath);
	
	return CMcSettingsPage::SaveSettings();
}

void CPageEditMode::OnSelectButton() 
{
	UpdateData();

	CChooseFolder	chooseFolder;
			
	CString strDirPath	=	m_strReceivedFilePath;
				
	if(chooseFolder.DoModal(GetString(IDS_SELECT_FOLDER_NAME),strDirPath,GetSafeHwnd())==IDOK)
	{
		m_strReceivedFilePath = strDirPath;
		SetModified();
	}
	UpdateData(FALSE);
}

void CPageEditMode::OnClearChatwindowCheck() 
{
	SetModified();
}

void CPageEditMode::OnShowfriendstatusCheck() 
{
	SetModified();
}

void CPageEditMode::OnEnterSendRadio() 
{
	SetModified();
}

void CPageEditMode::OnEnterNewlineRadio() 
{
	SetModified();
}

void CPageEditMode::OnCtrlenterNewlineRadio() 
{
	SetModified();
}

void CPageEditMode::OnCtrlenterSendRadio() 
{
	SetModified();
}

void CPageEditMode::OnShiftenterSendRadio() 
{
	SetModified();
}

void CPageEditMode::OnShiftenterNewlineRadio() 
{
	SetModified();
}

void CPageEditMode::OnAltsSendRadio() 
{
	SetModified();
}

void CPageEditMode::OnAltsNewlineRadio() 
{
	SetModified();
}
