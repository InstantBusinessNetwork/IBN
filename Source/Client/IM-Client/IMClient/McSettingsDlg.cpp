// McSettingsDlg.cpp : implementation file
//

#include "stdafx.h"
#include "McSettingsDlg.h"


// CMcSettingsDlg dialog

IMPLEMENT_DYNAMIC(CMcSettingsDlg, CDialog)

CMcSettingsDlg::CMcSettingsDlg(CWnd* pParent /*=NULL*/)
	: MC_SETTINGS_DLG_PARENT(CMcSettingsDlg::IDD, pParent)
{
	m_nCurrentPage = -1;
//	m_bResizable = FALSE;
//	m_strSkinSettings = _T("/Shell/Preferences/skin.xml");
}

CMcSettingsDlg::~CMcSettingsDlg()
{
}

void CMcSettingsDlg::DoDataExchange(CDataExchange* pDX)
{
	MC_SETTINGS_DLG_PARENT::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDlgPreferences)
	DDX_Control(pDX, IDC_PLACEHOLDER, m_placeholder);
	DDX_Control(pDX, IDC_LIST, m_list);
	DDX_Control(pDX, IDOK, m_btn2OK);
	DDX_Control(pDX, IDCANCEL, m_btn2Cancel);
	DDX_Control(pDX, IDC_BUTTON_APPLY, m_btn2Apply);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CMcSettingsDlg, MC_SETTINGS_DLG_PARENT)
//{{AFX_MSG_MAP(CPageSound)
	ON_NOTIFY(LVN_ITEMCHANGING, IDC_LIST, OnLvnItemChanging_List)
	ON_WM_DESTROY()
	ON_BN_CLICKED(IDC_BUTTON_APPLY, OnButtonApply)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

BEGIN_EVENTSINK_MAP(CMcSettingsDlg, MC_SETTINGS_DLG_PARENT)
//{{AFX_EVENTSINK_MAP(CMcSettingsDlg)
//}}AFX_EVENTSINK_MAP
END_EVENTSINK_MAP()



// CMcSettingsPage dialog

IMPLEMENT_DYNAMIC(CMcSettingsPage, CDialog)

CMcSettingsPage::CMcSettingsPage(UINT nIDTemplate, LPCTSTR szTitle)//, CWnd* pParent /*=NULL*/)
	: CDialog(nIDTemplate, NULL),//pParent),
	m_strTitle(szTitle),
	m_pParent(NULL),
	m_nTemplateID(nIDTemplate)
{
}

CMcSettingsPage::~CMcSettingsPage()
{
}

void CMcSettingsPage::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}


BEGIN_MESSAGE_MAP(CMcSettingsPage, CDialog)
	ON_WM_CREATE()
	ON_WM_CTLCOLOR()
END_MESSAGE_MAP()


// CMcSettingsPage message handlers

BOOL CMcSettingsPage::IsValidSettings(void)
{
	return TRUE;
}

void CMcSettingsPage::Activate()
{
//	CWnd *pWnd = GetNextDlgTabItem(NULL);
//	if(pWnd)
//		pWnd->SetFocus();
}

BOOL CMcSettingsPage::SaveSettings(void)
{
	return TRUE;
}

void CMcSettingsPage::OnOK(void)
{
}

void CMcSettingsPage::OnCancel(void)
{
}

BOOL CMcSettingsDlg::AddPage(CMcSettingsPage* pPage)
{
	ASSERT(pPage != NULL);

	for(int i=0; i<m_aPages.GetSize(); i++)
	{
		if(m_aPages[i] == pPage)
			return FALSE;
	}

	pPage->m_pParent = this;
	m_aPages.Add(pPage);

	if(::IsWindow(pPage->GetSafeHwnd()))
	{
		CRect r;
		pPage->GetWindowRect(&r);
		if(r.Width() > m_rPlaceHolder.Width())
			m_rPlaceHolder.right = m_rPlaceHolder.left + r.Width();
		if(r.Height() > m_rPlaceHolder.Height())
			m_rPlaceHolder.bottom = m_rPlaceHolder.top + r.Height();
		AdjustWindowForPages();
	}
	
	return TRUE;
}

void CMcSettingsDlg::Clear(void)
{
	SelectPage(-1);
	while(m_aPages.GetSize())
		RemovePage(m_aPages[0]);

	m_rPlaceHolder = m_rPlaceHolderInit;
	AdjustWindowForPages();
}

LPCTSTR CMcSettingsPage::GetTitle(void)
{
	return LPCTSTR(m_strTitle);
}

void CMcSettingsDlg::RemovePage(CMcSettingsPage* pPage)
{
	int nIndex = -1;
	for(int i=0; i<m_aPages.GetSize(); i++)
	{
		if(m_aPages[i] == pPage)
		{
			nIndex = i;
			break;
		}
	}
	if(nIndex >= 0)
	{
		if(m_nCurrentPage == nIndex)
			SelectPage(-1);
		pPage->m_pParent = NULL;
		if(m_list.m_hWnd)
			m_list.DeleteItem(nIndex);
		m_aPages.RemoveAt(nIndex);
	}
}

void CMcSettingsDlg::SetModified(BOOL bModified)
{
	CWnd *p = GetDlgItem(IDC_BUTTON_APPLY);
	if(p)
		p->EnableWindow(bModified);
}

void CMcSettingsDlg::ClearList(void)
{
}

void CMcSettingsDlg::OnLvnItemChanging_List(NMHDR *pNMHDR, LRESULT *pResult)
{
	LPNMLISTVIEW pNMLV = reinterpret_cast<LPNMLISTVIEW>(pNMHDR);

	// If old item
	//if((pNMLV->uOldState & LVIS_SELECTED) && !(pNMLV->uNewState & LVIS_SELECTED))
	//{
	//	if(pNMLV->iItem >= 0)
	//	{
	//		if(!m_aPages[pNMLV->iItem]->IsValidSettings())
	//		{
	//			*pResult = -1;
	//			return;
	//		}
	//	}
	//}

	// If new item
	if((pNMLV->uNewState & LVIS_SELECTED) && !(pNMLV->uOldState & LVIS_SELECTED))
	{
		SelectPage(pNMLV->iItem);
	}
	*pResult = 0;
}

void CMcSettingsDlg::SelectPage(int nIndex)
{
	if(m_nCurrentPage == nIndex)
		return;

	CMcSettingsPage *pPage;
	if(m_nCurrentPage >= 0)
	{
		pPage = m_aPages.GetAt(m_nCurrentPage);
		//if(!pPage->IsValidSettings())
		//	return FALSE;
		pPage->ShowWindow(SW_HIDE);
	}

	if(nIndex >= 0)
	{
		CRect r;
		pPage = m_aPages.GetAt(nIndex);
		m_placeholder.GetWindowRect(&r);
		ScreenToClient(&r);
		pPage->SetWindowPos(&wndTop, r.left, r.top, 0, 0, SWP_NOSIZE|SWP_SHOWWINDOW);
		pPage->Activate();
	}
	m_nCurrentPage = nIndex;
}

BOOL CMcSettingsPage::Create(CMcSettingsDlg* pParent)
{
	return CDialog::Create(m_nTemplateID, pParent);
}

void CMcSettingsDlg::OnDestroy()
{
	MC_SETTINGS_DLG_PARENT::OnDestroy();

	Clear();
}

int CMcSettingsPage::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
	if (CDialog::OnCreate(lpCreateStruct) == -1)
		return -1;

	ModifyStyleEx(0, WS_EX_CONTROLPARENT);

	return 0;
}

HBRUSH CMcSettingsPage::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor) 
{
	HBRUSH hbr = CDialog::OnCtlColor(pDC, pWnd, nCtlColor);
	
	// TODO: Change any attributes of the DC here
	
	// TODO: Return a different brush if the default is not desired
//	if(m_pParent)
//		m_pParent->GetCtlColor(pDC, pWnd, nCtlColor, &hbr);
	
	return hbr;
}


BOOL CMcSettingsDlg::OnInitDialog()
{
	MC_SETTINGS_DLG_PARENT::OnInitDialog();
	
	CRect	listRect;
	m_list.GetClientRect(listRect);
	m_list.InsertColumn(0,_T(""),LVCFMT_LEFT,listRect.Width());
	ClearList();
	
	for(int i=0; i<m_aPages.GetSize(); i++)
	{
		m_aPages[i]->Create(this);
		m_list.InsertItem(m_list.GetItemCount(), m_aPages[i]->GetTitle());
	}
	if(m_aPages.GetSize())
		m_list.SetItemState(0, LVIS_SELECTED|LVIS_FOCUSED, LVIS_SELECTED|LVIS_FOCUSED);
	m_list.SetFocus();
	
	DWORD dwStyle = m_list.GetExtendedStyle();
	dwStyle |= LVS_EX_FULLROWSELECT;
	m_list.SetExtendedStyle(dwStyle);

	SetModified(FALSE);
//	SetWindowPos(NULL, 0, 0, 575, 368, SWP_NOZORDER|SWP_NOMOVE);

	return FALSE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

void CMcSettingsPage::SetModified(void)
{
	if(m_pParent)
		m_pParent->SetModified(TRUE);
}

void CMcSettingsDlg::ApplyChanges(bool bCloseDialog)
{
	CMcSettingsPage *pPage;
	if(m_nCurrentPage >= 0)
	{
		pPage = m_aPages[m_nCurrentPage];
		if(!pPage->IsValidSettings())
			return;
	}

	// Check all other pages
	for(int i=0; i<m_aPages.GetSize(); i++)
	{
		pPage = m_aPages[i];
		if(pPage->IsValidSettings())
		{
			if(!pPage->SaveSettings())
			{
				::MessageBox(m_hWnd, GetString(IDS_CANT_SAVE_SETTINGS_NAME), GetString(IDS_ERROR_TITLE), MB_OK|MB_ICONEXCLAMATION);
				return;
			}
		}
	}
	SetModified(FALSE);
	if(bCloseDialog)
		MC_SETTINGS_DLG_PARENT::OnOK();
}

//DEL void CMcSettingsDlg::LoadSkin(IXMLDOMNode *pXmlRoot)
//DEL {
//DEL 	GetWindowRect(&m_rWindowInit);
//DEL 	ScreenToClient(&m_rWindowInit);
//DEL 
//DEL 	LoadButtons(pXmlRoot);
//DEL 	LoadRectangle(pXmlRoot, _T("List"), &m_list, TRUE);
//DEL 	LoadRectangle(pXmlRoot, _T("Page"), &m_placeholder, FALSE);
//DEL 
//DEL 	m_placeholder.GetWindowRect(&m_rPlaceHolderInit);
//DEL 	ScreenToClient(&m_rPlaceHolderInit);
//DEL 	m_rPlaceHolder = m_rPlaceHolderInit;
//DEL 
//DEL 	if(m_crList.IsValidColor())
//DEL 	{
//DEL 		m_list.SetBkColor(m_crList.crBG);
//DEL 		m_list.SetTextBkColor(m_crList.crBG);
//DEL 		m_list.SetTextColor(m_crList.crText);
//DEL 	}
//DEL }

//DEL void CMcSettingsDlg::LoadButtons(IXMLDOMNode *pXmlRoot)
//DEL {
//DEL 	m_btnX.ShowWindow(SW_HIDE);
//DEL 	m_btnOK.ShowWindow(SW_HIDE);
//DEL 	m_btnCancel.ShowWindow(SW_HIDE);
//DEL 	m_btnApply.ShowWindow(SW_HIDE);
//DEL 	LoadButton(pXmlRoot, _T("X"), &m_btnX, TRUE, FALSE);
//DEL 	LoadButton(pXmlRoot, _T("OK"), &m_btnOK, TRUE, FALSE);
//DEL 	LoadButton(pXmlRoot, _T("Cancel"), &m_btnCancel, TRUE, FALSE);
//DEL 	LoadButton(pXmlRoot, _T("Apply"), &m_btnApply, TRUE, FALSE);
//DEL }
//DEL void CMcSettingsDlg::OnClickButtonOK() 
//DEL {
//DEL 	ApplyChanges(TRUE);
//DEL }

//DEL void CMcSettingsDlg::OnClickButtonApply() 
//DEL {
//DEL 	ApplyChanges(FALSE);
//DEL }

//DEL void CMcSettingsDlg::OnClickButtonX() 
//DEL {
//DEL 	MC_SETTINGS_DLG_PARENT::OnCancel();
//DEL }

//DEL void CMcSettingsDlg::OnClickButtonCancel() 
//DEL {
//DEL 	MC_SETTINGS_DLG_PARENT::OnCancel();
//DEL }

void CMcSettingsDlg::AdjustWindowForPages()
{
	CRect r;
	GetWindowRect(&r);
	r.right += m_rPlaceHolder.Width() - m_rPlaceHolderInit.Width();
	r.bottom += m_rPlaceHolder.Height() - m_rPlaceHolderInit.Height();
	//	AdjustRect(r);
	SetWindowPos(NULL, r.left, r.top, r.Width(), r.Height(), SWP_NOZORDER);
}

void CMcSettingsDlg::OnButtonApply() 
{
	ApplyChanges(FALSE);
}

void CMcSettingsDlg::OnOK() 
{
	ApplyChanges(TRUE);
}
