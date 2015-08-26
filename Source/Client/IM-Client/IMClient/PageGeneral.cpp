// PageGeneral.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "PageGeneral.h"
#include "GlobalFunction.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CPageGeneral dialog


CPageGeneral::CPageGeneral(LPCTSTR szTitle)
	: CMcSettingsPage(CPageGeneral::IDD, szTitle)
{
	//{{AFX_DATA_INIT(CPageGeneral)
	m_ShowNew = FALSE;
	m_HideMpa = FALSE;
	m_bKeepTop = FALSE;
	m_bRemoveTaskBar = FALSE;
	m_bShowSendTo = TRUE;
	//}}AFX_DATA_INIT
	MessageMode		= 0;
	m_lDefaultView	= 0;
	m_lIBNActionBrowser = 0;
	m_bOpenInMaximaze = TRUE;
	m_bCreateOfflineFolder = TRUE;
	m_bMinimizeOnClose = FALSE;
}


void CPageGeneral::DoDataExchange(CDataExchange* pDX)
{
	CMcSettingsPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CPageGeneral)
	DDX_Check(pDX, IDC_SHOWNEWMESSAGE_CHECK, m_ShowNew);
	DDX_Check(pDX, IDC_ANIMATION_CHECK,m_AnimationClose);
	DDX_Check(pDX, IDC_HIDEMPAINTRAY_CHECK, m_HideMpa);
	DDX_Check(pDX, IDC_KEEPTOP_CHECK, m_bKeepTop);
	DDX_Check(pDX, IDC_SHOWOFFLINECHECK, m_bShowOffline);
	DDX_Check(pDX, IDC_CREATEOFFLINEFOLDER, m_bCreateOfflineFolder);
	DDX_Check(pDX, IDC_REMOVE_IBN_FROM_TASK_BAR_CHECK, m_bRemoveTaskBar);
	DDX_Check(pDX, IDC_CHECK_SHOWSENDTO, m_bShowSendTo);
	DDX_Check(pDX, IDC_CHECK_MICRO_HAS_MAXIMIZE, m_bOpenInMaximaze);
	DDX_Check(pDX, IDC_MINIMIZE_ONCLOSE_CHECK, m_bMinimizeOnClose);
	DDX_Check(pDX, IDC_GET_DEFAULT_BROWSER_FROM_REGISTRY, m_bGetDefBrowserFromRegistry);
	DDX_Check(pDX, IDC_CONTACTLIST_SORTBYFIRSTNAME, m_bContactListSortByFirstName);
	DDX_Check(pDX, IDC_UPDATE_AUTOCHECK, m_bUpdateAutoCheck);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CPageGeneral, CMcSettingsPage)
	//{{AFX_MSG_MAP(CPageGeneral)
	ON_BN_CLICKED(IDC_SINGLE_RADIO, OnSingleRadio)
	ON_BN_CLICKED(IDC_SPLIT_RADIO, OnSplitRadio)
	ON_BN_CLICKED(IDC_SHOWNEWMESSAGE_CHECK, OnShownewmessageCheck)
	ON_BN_CLICKED(IDC_ANIMATION_CHECK, OnAnimationCheck)
	ON_BN_CLICKED(IDC_HIDEMPAINTRAY_CHECK, OnHidempaintrayCheck)
	ON_BN_CLICKED(IDC_KEEPTOP_CHECK, OnKeeptopCheck)
	ON_BN_CLICKED(IDC_SHOWOFFLINECHECK, OnShowofflinecheck)
	ON_BN_CLICKED(IDC_CREATEOFFLINEFOLDER, OnCreateOfflineFolderCheck)
	ON_BN_CLICKED(IDC_REMOVE_IBN_FROM_TASK_BAR_CHECK, OnRemoveIbnFromTaskBarCheck)
	ON_BN_CLICKED(IDC_CHECK_SHOWSENDTO, OnShowSendTo)
	ON_BN_CLICKED(IDC_RADIO_IBN_ACTIONS, OnIBNActionsView)
	ON_BN_CLICKED(IDC_RADIO_CONTACT_LIST, OnContactListView)
	ON_BN_CLICKED(IDC_RADIO_IBN_MICRO_BROWSER, OnIBNActionsViewInIBNMiniBrowser)
	ON_BN_CLICKED(IDC_RADIO_STANDART_BROWSER, OnIBNActionsViewInExternalBrowser)
	ON_BN_CLICKED(IDC_MINIMIZE_ONCLOSE_CHECK, OnMinimizeOnCloseCheck)
	ON_BN_CLICKED(IDC_GET_DEFAULT_BROWSER_FROM_REGISTRY, OnGetDefBrowserFromRegistry)
	ON_BN_CLICKED(IDC_CONTACTLIST_SORTBYFIRSTNAME, OnContactlistSortByFirstName)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CPageGeneral message handlers

BOOL CPageGeneral::SaveSettings()
{
	UpdateData();
	
	WriteOptionInt(IDS_OFSMESSENGER,IDS_MESSSAGEMODE,MessageMode);
	WriteOptionInt(IDS_OFSMESSENGER,IDS_SHOWNEW,m_ShowNew);
	WriteOptionInt(IDS_OFSMESSENGER,IDS_ANIMATION,m_AnimationClose);
	WriteOptionInt(IDS_OFSMESSENGER,IDS_HIDEINTRAY,m_HideMpa);
	WriteOptionInt(IDS_OFSMESSENGER,IDS_REMOVE_FROM_TASK_BAR,m_bRemoveTaskBar);
	WriteOptionInt(IDS_OFSMESSENGER,IDS_KEEPTOP,m_bKeepTop);

	WriteOptionInt(IDS_OFSMESSENGER,IDS_SHOWOFFLINEFILES,m_bShowOffline);
	m_bShowSendTo ? CreateSendToLink() : DeleteSendToLink();
	
	WriteOptionInt(IDS_OFSMESSENGER,IDS_CLMODE,m_bCreateOfflineFolder?2:1);

	WriteOptionInt(IDS_OFSMESSENGER,IDS_VIEWMODE,m_lDefaultView);
	
	WriteOptionInt(IDS_OFSMESSENGER,IDS_IBNACTIONBROWSER,m_lIBNActionBrowser);

	WriteOptionInt(IDS_OFSMESSENGER,IDS_OPENINMAXIMAZE, m_bOpenInMaximaze);

	WriteOptionInt(IDS_OFSMESSENGER,IDS_MINIMIZE_ONCLOSE,m_bMinimizeOnClose);

	WriteOptionInt(IDS_OFSMESSENGER,IDS_GET_DEFAULT_BROWSER_FROM_REGISTRY,m_bGetDefBrowserFromRegistry);

	WriteOptionInt(IDS_OFSMESSENGER,IDS_CONTACTLIST_SORT_BY_FIRSTNAME,m_bContactListSortByFirstName);

	WriteOptionInt(IDS_OFSMESSENGER,IDS_UPDATE_AUTOCHECK,m_bUpdateAutoCheck);
	

	return CMcSettingsPage::SaveSettings();
}

BOOL CPageGeneral::OnInitDialog() 
{
	CMcSettingsPage::OnInitDialog();

	// HIDE Properties
	GetDlgItem(IDC_RADIO_IBN_ACTIONS)->EnableWindow(FALSE);
	GetDlgItem(IDC_RADIO_IBN_MICRO_BROWSER)->EnableWindow(FALSE);
	GetDlgItem(IDC_RADIO_CONTACT_LIST)->EnableWindow(FALSE);
	GetDlgItem(IDC_RADIO_STANDART_BROWSER)->EnableWindow(FALSE);
	GetDlgItem(IDC_CHECK_MICRO_HAS_MAXIMIZE)->EnableWindow(FALSE);
	//GetDlgItem(IDC_STATIC)->ShowWindow(SW_HIDE);


	m_ShowNew        = GetOptionInt(IDS_OFSMESSENGER,IDS_SHOWNEW,1);
	m_AnimationClose = GetOptionInt(IDS_OFSMESSENGER,IDS_ANIMATION,FALSE);

	m_bKeepTop	=	GetOptionInt(IDS_OFSMESSENGER,IDS_KEEPTOP,FALSE);
	/// 0 - Single
	/// 1 - Split
	MessageMode = GetOptionInt(IDS_OFSMESSENGER,IDS_MESSSAGEMODE,1);
	m_HideMpa   = GetOptionInt(IDS_OFSMESSENGER,IDS_HIDEINTRAY,FALSE);
	m_bShowOffline  =  GetOptionInt(IDS_OFSMESSENGER,IDS_SHOWOFFLINEFILES,TRUE);
	m_bCreateOfflineFolder = (GetOptionInt(IDS_OFSMESSENGER,IDS_CLMODE,2)==2);
	m_bRemoveTaskBar	=	GetOptionInt(IDS_OFSMESSENGER,IDS_REMOVE_FROM_TASK_BAR,TRUE);
	m_bShowSendTo = IsSendToLinkCreated();
	
	CheckRadioButton(IDC_SINGLE_RADIO,IDC_SPLIT_RADIO,IDC_SINGLE_RADIO + MessageMode);

	//  [12/15/2003]
	m_lDefaultView = 1;//GetOptionInt(IDS_OFSMESSENGER,IDS_VIEWMODE,1);

	CheckRadioButton(IDC_RADIO_IBN_ACTIONS,IDC_RADIO_CONTACT_LIST,IDC_RADIO_IBN_ACTIONS + m_lDefaultView);

	//  [12/16/2003]
	m_lIBNActionBrowser = 1;//GetOptionInt(IDS_OFSMESSENGER,IDS_IBNACTIONBROWSER,1);

	if(m_lIBNActionBrowser)
		GetDlgItem(IDC_CHECK_MICRO_HAS_MAXIMIZE)->EnableWindow(FALSE);
	
	CheckRadioButton(IDC_RADIO_IBN_MICRO_BROWSER,IDC_RADIO_STANDART_BROWSER,IDC_RADIO_IBN_MICRO_BROWSER + m_lIBNActionBrowser);

	m_bOpenInMaximaze = GetOptionInt(IDS_OFSMESSENGER,IDS_OPENINMAXIMAZE, TRUE);

	m_bMinimizeOnClose = GetOptionInt(IDS_OFSMESSENGER,IDS_MINIMIZE_ONCLOSE,TRUE);

	m_bGetDefBrowserFromRegistry = GetOptionInt(IDS_OFSMESSENGER,IDS_GET_DEFAULT_BROWSER_FROM_REGISTRY,TRUE);

	m_bContactListSortByFirstName = GetOptionInt(IDS_OFSMESSENGER,IDS_CONTACTLIST_SORT_BY_FIRSTNAME, GetProductLanguage()==CString(_T("1049"))?FALSE:TRUE);

	m_bUpdateAutoCheck =  GetOptionInt(IDS_OFSMESSENGER,IDS_UPDATE_AUTOCHECK, TRUE);

	UpdateData(FALSE);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CPageGeneral::OnGetDefBrowserFromRegistry()
{
	SetModified();
}

void CPageGeneral::OnContactlistSortByFirstName()
{
	SetModified();
}

void CPageGeneral::OnMinimizeOnCloseCheck()
{
	SetModified();
}

void CPageGeneral::OnSingleRadio() 
{
	MessageMode = 0;
	SetModified();
}

void CPageGeneral::OnSplitRadio() 
{
	MessageMode = 1;
	SetModified();
}

void CPageGeneral::OnShownewmessageCheck() 
{
	SetModified();
}

void CPageGeneral::OnAnimationCheck() 
{
	SetModified();
}

void CPageGeneral::OnHidempaintrayCheck() 
{
	SetModified();
}

void CPageGeneral::OnKeeptopCheck() 
{
	SetModified();
}

void CPageGeneral::OnShowofflinecheck() 
{
	SetModified();
}

void CPageGeneral::OnCreateOfflineFolderCheck() 
{
	SetModified();
}

void CPageGeneral::OnRemoveIbnFromTaskBarCheck() 
{
	SetModified();
}

void CPageGeneral::OnShowSendTo() 
{
	SetModified();
}

void CPageGeneral::OnIBNActionsView()
{
	m_lDefaultView = 0;
	SetModified();
}

void CPageGeneral::OnContactListView()
{
	m_lDefaultView = 1;
	SetModified();
}


void CPageGeneral::OnIBNActionsViewInIBNMiniBrowser()
{
	m_lIBNActionBrowser = 0;
	GetDlgItem(IDC_CHECK_MICRO_HAS_MAXIMIZE)->EnableWindow(TRUE);
	SetModified();
}

void CPageGeneral::OnIBNActionsViewInExternalBrowser()
{
	m_lIBNActionBrowser = 1;
	GetDlgItem(IDC_CHECK_MICRO_HAS_MAXIMIZE)->EnableWindow(FALSE);
	SetModified();
}
