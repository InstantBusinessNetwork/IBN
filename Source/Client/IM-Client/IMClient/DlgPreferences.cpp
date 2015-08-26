// DlgPreferences.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "DlgPreferences.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDlgPreferences dialog


CDlgPreferences::CDlgPreferences(CWnd* pParent /*=NULL*/)
	: PREFERENCES_PARENT(CDlgPreferences::IDD, pParent)
{
	//{{AFX_DATA_INIT(CDlgPreferences)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
	m_strSkinSettings = _T("/Shell/Preferences/skin.xml");
}


void CDlgPreferences::DoDataExchange(CDataExchange* pDX)
{
	PREFERENCES_PARENT::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDlgPreferences)
	DDX_Control(pDX, IDC_PLACEHOLDER, m_placeholder);
	DDX_Control(pDX, IDC_LIST, m_list);
	DDX_Control(pDX, IDC_BUTTON_X, m_btnX);
	DDX_Control(pDX, IDC_BUTTON_OK, m_btnOK);
	DDX_Control(pDX, IDC_BUTTON_CANCEL, m_btnCancel);
	DDX_Control(pDX, IDC_BUTTON_APPLY, m_btnApply);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CDlgPreferences, PREFERENCES_PARENT)
	//{{AFX_MSG_MAP(CDlgPreferences)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CDlgPreferences message handlers

void CDlgPreferences::OnOK() 
{
	// TODO: Add extra validation here
	
	PREFERENCES_PARENT::OnOK();
}

void CDlgPreferences::OnCancel() 
{
	// TODO: Add extra cleanup here
	
	PREFERENCES_PARENT::OnCancel();
}

void CDlgPreferences::LoadSkin(IXMLDOMNode *pXmlRoot)
{
	LoadButtons(pXmlRoot);
	LoadRectangle(pXmlRoot, _T("List"), &m_list, TRUE);
	LoadRectangle(pXmlRoot, _T("Page"), &m_placeholder, FALSE);
}

void CDlgPreferences::LoadButtons(IXMLDOMNode *pXmlRoot)
{
	m_btnX.ShowWindow(SW_HIDE);
	m_btnOK.ShowWindow(SW_HIDE);
	m_btnCancel.ShowWindow(SW_HIDE);
	m_btnApply.ShowWindow(SW_HIDE);
	LoadButton(pXmlRoot, _T("X"), &m_btnX, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("OK"), &m_btnOK, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Cancel"), &m_btnCancel, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Apply"), &m_btnApply, TRUE, FALSE);
}
