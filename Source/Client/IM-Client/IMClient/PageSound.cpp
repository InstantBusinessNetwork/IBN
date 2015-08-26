// PageSound.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "PageSound.h"
#include "GlobalFunction.h"
#include <Mmsystem.h>
#include "McSettings.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

extern CMcSettings g_settings;
/////////////////////////////////////////////////////////////////////////////
// CPageSound dialog


CPageSound::CPageSound(LPCTSTR szTitle)
	: CMcSettingsPage(CPageSound::IDD, szTitle)
{
	//{{AFX_DATA_INIT(CPageSound)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
}


void CPageSound::DoDataExchange(CDataExchange* pDX)
{
	CMcSettingsPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CPageSound)
	DDX_Control(pDX, IDC_SOUNDPATH_LIST, m_SoundPathList);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CPageSound, CMcSettingsPage)
//{{AFX_MSG_MAP(CPageSound)
ON_BN_CLICKED(IDC_SELECT_BUTTON, OnSelectButton)
ON_BN_CLICKED(IDC_CLEAR_BUTTON, OnClearButton)
ON_BN_CLICKED(IDC_TEST_BUTTON, OnTestButton)
ON_NOTIFY(LVN_ITEMCHANGED, IDC_SOUNDPATH_LIST, OnItemchangedSoundpathList)
ON_NOTIFY(NM_DBLCLK, IDC_SOUNDPATH_LIST, OnDblclkSoundpathList)
	ON_NOTIFY(HDN_ENDTRACK, 0, OnEndtrackSoundpathList)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

static UINT IdItemMap[]	=	
{
	IDS_INCOMING_MESSAGE_KEY,
	IDS_INCOMING_FILE_KEY,
	IDS_ONLINE_ALERT_KEY,
	IDS_STARTUP_KEY,
	IDS_AUTHORIZATION_REQUEST_KEY,
	IDS_NEW_ALERT_COME_KEY
};

/////////////////////////////////////////////////////////////////////////////
// CPageSound message handlers

void CPageSound::OnSelectButton() 
{
	int iSel = GetSelectedItem();
	
	if(iSel!=-1)
	{
		CString strFileName = m_SoundPathList.GetItemText(iSel,1);
		CFileDialog OpenDlg(TRUE,NULL,strFileName,OFN_CREATEPROMPT|OFN_EXPLORER,GetString(IDS_SOUND_FORMAT),this);
		if(OpenDlg.DoModal()==IDOK)
		{
			m_SoundPathList.SetItemText(iSel,1,OpenDlg.GetPathName());
			SetModified();
		}
	}
	BlockOrUnBlock();
}

void CPageSound::OnClearButton() 
{
	int iSel = GetSelectedItem();
	
	if(iSel!=-1)
	{
		m_SoundPathList.SetItemText(iSel,1,_T(""));
		SetModified();
	}
	BlockOrUnBlock();
}

void CPageSound::OnTestButton() 
{
	int iSel = GetSelectedItem();
	
	if(iSel!=-1)
	{
		CString strFileName = m_SoundPathList.GetItemText(iSel,1);
		PlaySound(strFileName,NULL,SND_FILENAME|SND_ASYNC|SND_NODEFAULT);
	}
	BlockOrUnBlock();
}

BOOL CPageSound::OnInitDialog() 
{
	CMcSettingsPage::OnInitDialog();
	
	CString strSection = GetString(IDS_SOUND);
	CString strEntry = GetString(IDS_COLUMN_WIDTH);
	CString str;
	
	DWORD dwStyle = m_SoundPathList.GetExtendedStyle();
	dwStyle |= LVS_EX_FULLROWSELECT;
	m_SoundPathList.SetExtendedStyle(dwStyle);
	
	
	m_SoundPathList.InsertColumn(0,GetString(IDS_SOUND_EVENT),LVCFMT_LEFT,AfxGetApp()->GetProfileInt(strSection, strEntry+_T("1"), 120));
	m_SoundPathList.InsertColumn(1,GetString(IDS_SOUND_FILE_NAME),LVCFMT_LEFT,AfxGetApp()->GetProfileInt(strSection, strEntry+_T("2"), 150),1);
	
	for(int i=0;i<sizeof(IdItemMap)/sizeof(IdItemMap[0]);i++)
	{
		int iSubIndex = m_SoundPathList.InsertItem(i,GetString(IdItemMap[i]-2000));
//		m_SoundPathList.SetItemText(iSubIndex,1,GetOptionString(IDS_SOUND, IdItemMap[i]));
		g_settings.GetString(IDS_SOUND, IdItemMap[i], str, _T(""));
		m_SoundPathList.SetItemText(iSubIndex, 1, str);
	}
	
	BlockOrUnBlock();
	
	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

BOOL CPageSound::SaveSettings()
{
	for(int i=0;i<sizeof(IdItemMap)/sizeof(IdItemMap[0]);i++)
	{
//		WriteOptionString(IDS_SOUND,IdItemMap[i],m_SoundPathList.GetItemText(i,1));
		g_settings.SetString(IDS_SOUND, IdItemMap[i], m_SoundPathList.GetItemText(i,1));
	}

	return CMcSettingsPage::SaveSettings();
}

void CPageSound::BlockOrUnBlock()
{
	int iSel = GetSelectedItem();

	BOOL	bBlock1	=	FALSE;
	BOOL	bBlock2	=	FALSE;

	if(iSel != -1)
	{
		bBlock1 = TRUE;
		bBlock2 = !m_SoundPathList.GetItemText(iSel,1).IsEmpty();
	}

	GetDlgItem(IDC_SELECT_BUTTON)->EnableWindow(bBlock1);
	GetDlgItem(IDC_CLEAR_BUTTON)->EnableWindow(bBlock1&&bBlock2);
	GetDlgItem(IDC_TEST_BUTTON)->EnableWindow(bBlock2);
}

void CPageSound::OnItemchangedSoundpathList(NMHDR* pNMHDR, LRESULT* pResult) 
{
	NM_LISTVIEW* pNMListView = (NM_LISTVIEW*)pNMHDR;
	
	BlockOrUnBlock();
	
	*pResult = 0;
}

//DEL void CPageSound::OnTrackSoundpathList(NMHDR* pNMHDR, LRESULT* pResult) 
//DEL {
//DEL 	HD_NOTIFY *phdn = (HD_NOTIFY *) pNMHDR;
//DEL 	// TODO: Add your control notification handler code here
//DEL 	
//DEL 	Beep(100,100);
//DEL 	
//DEL 	*pResult = 0;
//DEL }

int CPageSound::GetSelectedItem()
{
	POSITION pos = m_SoundPathList.GetFirstSelectedItemPosition();
	return m_SoundPathList.GetNextSelectedItem(pos);
}

//DEL BOOL CPageSound::DestroyWindow() 
//DEL {
//DEL 	return CMcSettingsPage::DestroyWindow();
//DEL }

void CPageSound::OnDblclkSoundpathList(NMHDR* pNMHDR, LRESULT* pResult) 
{
	OnSelectButton();
	
	*pResult = 0;
}

//DEL void CPageSound::OnClose() 
//DEL {
//DEL 	CMcSettingsPage::OnClose();
//DEL }

void CPageSound::OnEndtrackSoundpathList(NMHDR* pNMHDR, LRESULT* pResult) 
{
	HD_NOTIFY *phdn = (HD_NOTIFY *) pNMHDR;
	
	CString strSection = GetString(IDS_SOUND);
	CString strEntry = GetString(IDS_COLUMN_WIDTH);
	
	AfxGetApp()->WriteProfileInt(strSection, strEntry+_T("1"), m_SoundPathList.GetColumnWidth(0));
	AfxGetApp()->WriteProfileInt(strSection, strEntry+_T("2"), m_SoundPathList.GetColumnWidth(1));
	
	*pResult = 0;
}
