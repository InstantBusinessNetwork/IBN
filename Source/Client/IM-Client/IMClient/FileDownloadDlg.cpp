// FileDownloadDlg.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "FileDownloadDlg.h"
#include "resource.h"
#include "MainDlg.h"
#include "FileDescriptioDlg.h"
#include "ChooseFolder.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CFileDownloadDlg dialog
///TCHAR strDUStatus[3][20]	=	{"New","OffLine","Downloaded"};
DWORD strDUStatus[3]	=	{IDS_FILE_NEW_NAME, IDS_FILE_OFFLINE_NAME, IDS_FILE_DOWNLOADED_NAME};
//TCHAR strDUProgress[7][20]	=	{"","Wait","Wait","0%","","Cancel","Error"};
DWORD strDUProgress[7]={IDS_FILE_EMPTY,IDS_FILE_WAIT_NAME ,IDS_FILE_WAIT_NAME ,IDS_FILE_START_NAME,IDS_FILE_EMPTY,IDS_FILE_CANCEL_NAME,IDS_FILE_ERROR_NAME};

CFileDownloadDlg::CFileDownloadDlg(CWnd* pParent /*=NULL*/)
	: CResizableDialog(CFileDownloadDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CFileDownloadDlg)
	m_bShowOfflineFiles = FALSE;
	//}}AFX_DATA_INIT
//    CoInitialize(NULL);	
	bWasSetComplet			= FALSE;
	pMessenger				= NULL;
	LoadOfflineFileHandle	=	0;
	m_lastUserId			=	0;
}

void CFileDownloadDlg::SetMessanger(CMainDlg *pMessenger)
{
	this->pMessenger = pMessenger;
}

CFileDownloadDlg::~CFileDownloadDlg()
{
	m_LockList.Lock();
	try
	{
		for(int i = 0;i < ListArray.GetSize ();i++)
		{
			CDUFileInfo *pDUFileInfo = ListArray.GetAt(i);
			delete pDUFileInfo;
		}
		ListArray.RemoveAll ();
	}
	catch(...)
	{}
	m_LockList.Unlock ();
}


void CFileDownloadDlg::DoDataExchange(CDataExchange* pDX)
{
	CResizableDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CFileDownloadDlg)
	DDX_Control(pDX, IDC_REMEMBERLATER_BUTTON, m_btnRememberLater);
	DDX_Control(pDX, IDC_OFFLINE_BUTTON, m_btnOffLine);
	DDX_Control(pDX, IDC_CANCEL_BUTTON, m_btnCancel);
	DDX_Control(pDX, IDC_DOWNLOAD_BUTTON, m_btnDownload);
	DDX_Control(pDX, IDC_DELETE_BUTTON, m_btnDelete);
	DDX_Control(pDX, IDC_FILE_DOWNLOAD_LIST, m_FileDownLoadList);
	DDX_Check(pDX, IDC_SHOWOFFLINEFILES_CHECK, m_bShowOfflineFiles);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CFileDownloadDlg, CResizableDialog)
	//{{AFX_MSG_MAP(CFileDownloadDlg)
	ON_BN_CLICKED(IDOK, OnOk)
	ON_WM_TIMER()
	ON_WM_CLOSE()
	ON_NOTIFY(LVN_ITEMCHANGED, IDC_FILE_DOWNLOAD_LIST, OnItemchangedFileDownloadList)
	ON_NOTIFY(HDN_ITEMCLICK, 0, OnItemclickFileDownloadList)
	ON_NOTIFY(NM_DBLCLK, IDC_FILE_DOWNLOAD_LIST, OnDblclkFileDownloadList)
	ON_NOTIFY(NM_RCLICK, IDC_FILE_DOWNLOAD_LIST, OnRclickFileDownloadList)
	ON_COMMAND(ID_FILEDOWNLOAD_CANCELTRANSFORM, OnFiledownloadCanceltransform)
	ON_UPDATE_COMMAND_UI(ID_FILEDOWNLOAD_CANCELTRANSFORM, OnUpdateFiledownloadCanceltransform)
	ON_COMMAND(ID_FILEDOWNLOAD_CLEARRECORD, OnFiledownloadClearrecord)
	ON_UPDATE_COMMAND_UI(ID_FILEDOWNLOAD_CLEARRECORD, OnUpdateFiledownloadClearrecord)
	ON_COMMAND(ID_FILEDOWNLOAD_DOWNLOAD, OnFiledownloadDownload)
	ON_UPDATE_COMMAND_UI(ID_FILEDOWNLOAD_DOWNLOAD, OnUpdateFiledownloadDownload)
	ON_COMMAND(ID_FILEDOWNLOAD_INFORMATION, OnFiledownloadInformation)
	ON_UPDATE_COMMAND_UI(ID_FILEDOWNLOAD_INFORMATION, OnUpdateFiledownloadInformation)
	ON_COMMAND(ID_FILEDOWNLOAD_MOVETOOFFLINE, OnFiledownloadMovetooffline)
	ON_UPDATE_COMMAND_UI(ID_FILEDOWNLOAD_MOVETOOFFLINE, OnUpdateFiledownloadMovetooffline)
	ON_COMMAND(ID_FILEDOWNLOAD_OPEN, OnFiledownloadOpen)
	ON_UPDATE_COMMAND_UI(ID_FILEDOWNLOAD_OPEN, OnUpdateFiledownloadOpen)
	ON_BN_CLICKED(IDC_OFFLINE_BUTTON, OnOfflineButton)
	ON_BN_CLICKED(IDC_DELETE_BUTTON, OnDeleteButton)
	ON_BN_CLICKED(IDC_REMEMBERLATER_BUTTON, OnRememberlaterButton)
	ON_BN_CLICKED(IDC_DOWNLOAD_BUTTON, OnDownloadButton)
	ON_BN_CLICKED(IDC_CANCEL_BUTTON, OnCancelButton)
	ON_BN_CLICKED(IDC_SHOWOFFLINEFILES_CHECK, OnShowofflinefilesCheck)
	ON_MESSAGE(WM_NLT_CONTAINER_EVENT,OnNetEvent)
	ON_MESSAGE(WM_DOWNLOAD_BEGIN,OnDownloadBegin)
	ON_MESSAGE(WM_DOWNLOAD_STEP,OnDownloadStep)
	ON_COMMAND(ID_FILEDOWNLOAD_GOTODIR, OnFiledownloadGotodir)
	ON_UPDATE_COMMAND_UI(ID_FILEDOWNLOAD_GOTODIR, OnUpdateFiledownloadGotodir)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CFileDownloadDlg message handlers

void CFileDownloadDlg::OnOk() 
{
	// TODO: Add your control notification handler code here
	
}

void CFileDownloadDlg::OnCancel() 
{
	// TODO: Add extra cleanup here
	///CResizableDialog::OnCancel();
}

void CFileDownloadDlg::OnOfflineButton() 
{
	POSITION	pos = m_FileDownLoadList.GetFirstSelectedItemPosition();
	
	if(pos == NULL) return;
	
	if(pMessenger->ConnectEnable ())
	{
		while(pos)
		{
			int iSel	=	m_FileDownLoadList.GetNextSelectedItem(pos);
			
			CDUFileInfo *pDUFileInfo=(CDUFileInfo *)m_FileDownLoadList.GetItemData (iSel);
			theNet2.LockTranslator();
			try
			{
				BSTR FileID = pDUFileInfo->strFileID.AllocSysString();
				if(pDUFileInfo->Handle = pSession->ConfirmFile(FileID,fcDoOffline))
				{
					ChangeStatus(DS_OFFLINE,iSel);
					ChangeProgress(DP_WAIT,iSel);
					theNet2.AddToTranslator(pDUFileInfo->Handle,this->m_hWnd);
				}
				::SysFreeString(FileID);
			}
			catch(...)
			{
			}
			theNet2.UnlockTranslator();
		}
	}
	BlockOrUnBlock();
}

void CFileDownloadDlg::OnDeleteButton() 
{
	POSITION	pos = m_FileDownLoadList.GetFirstSelectedItemPosition();
	
	if(pos == NULL) return;
	
	if(pMessenger->ConnectEnable ())
	{
		while(pos)
		{
			int iSel = m_FileDownLoadList.GetNextSelectedItem(pos);

			CDUFileInfo *pDUFileInfo=(CDUFileInfo *)m_FileDownLoadList.GetItemData (iSel);
			
			theNet2.LockTranslator();
			try
			{
				BSTR FileID = pDUFileInfo->strFileID.AllocSysString();
				if(pDUFileInfo->Handle = pSession->ConfirmFile(FileID,fcDelete))
				{
					ChangeProgress(DP_WAIT,iSel);
					theNet2.AddToTranslator(pDUFileInfo->Handle,this->m_hWnd);
				}
				::SysFreeString(FileID);
			}
			catch(...)
			{
			}
			
			theNet2.UnlockTranslator();
		}
	}
	BlockOrUnBlock();
}

void CFileDownloadDlg::OnRememberlaterButton() 
{
	POSITION	pos = m_FileDownLoadList.GetFirstSelectedItemPosition();
	
	if(pos == NULL) return;
	
	while(pos)
	{
		int iSel = m_FileDownLoadList.GetNextSelectedItem(pos);
		ChangeProgress(DP_COMPLET,iSel);
		bWasSetComplet = TRUE;
	}
}

void CFileDownloadDlg::OnDownloadButton() 
{
	POSITION	pos = m_FileDownLoadList.GetFirstSelectedItemPosition();
	
	if(pos == NULL) return;
	
	if(pMessenger->ConnectEnable ())
	{
		if(m_FileDownLoadList.GetSelectedCount()==1)
		{
			while(pos)
			{
				int iSel = m_FileDownLoadList.GetNextSelectedItem(pos);
				
				CDUFileInfo *pDUFileInfo=(CDUFileInfo *)m_FileDownLoadList.GetItemData (iSel);
				CString strFullPath;
				strFullPath.Format(_T("%s\\%s"),GetMyDocumetPath(pMessenger->GetUserRole(),pMessenger->GetUserID()),pDUFileInfo->strFileName);
				CFileDialog SaveDlg(FALSE,NULL,/*pDUFileInfo->strFileName*/strFullPath,OFN_EXPLORER|OFN_OVERWRITEPROMPT,GetString(IDS_ALL_FILES_FORMAT),this);
				
				if(SaveDlg.DoModal ()==IDOK)
				{
					theNet2.LockTranslator();
					try
					{
						pDUFileInfo->strLocalPath = SaveDlg.GetPathName ();
						BSTR Path = SaveDlg.GetPathName ().AllocSysString();
						pDUFileInfo->pFile->PuthWnd(long(this->GetSafeHwnd()));
						pDUFileInfo->pFile->PutRealName(Path);
						pDUFileInfo->pFile->Receive(&(pDUFileInfo->Handle));
						if(pDUFileInfo->Handle!=0)
						{
							theNet2.AddToTranslator(pDUFileInfo->Handle,this->m_hWnd);
							ChangeProgress(DP_WAIT,iSel);
						}
						::SysFreeString(Path);
					}
					catch(...)
					{
					}
					theNet2.UnlockTranslator();
				}
			}
		}
		else
		{
			CChooseFolder	chooseFolder;
			
			CString strDirPath	=	GetMyDocumetPath(pMessenger->GetUserRole(),pMessenger->GetUserID());

			if(chooseFolder.DoModal(GetString(IDS_SELECT_FOLDER_NAME),strDirPath,this->GetSafeHwnd())==IDOK)
			{
				while(pos)
				{
					int iSel = m_FileDownLoadList.GetNextSelectedItem(pos);
					
					CDUFileInfo *pDUFileInfo=(CDUFileInfo *)m_FileDownLoadList.GetItemData (iSel);
					
					theNet2.LockTranslator();
					try
					{
						CString strTmp = strDirPath + _T('\\')+pDUFileInfo->strFileName;
						BSTR Path = strTmp.AllocSysString();
						
						pDUFileInfo->pFile->PuthWnd(long(this->GetSafeHwnd()));
						pDUFileInfo->pFile->PutRealName(Path);
						pDUFileInfo->pFile->Receive(&(pDUFileInfo->Handle));
						pDUFileInfo->strLocalPath = Path;

						if(pDUFileInfo->Handle!=0)
						{
							theNet2.AddToTranslator(pDUFileInfo->Handle,this->m_hWnd);
							ChangeProgress(DP_WAIT,iSel);
						}
						::SysFreeString(Path);
					}
					catch(...)
					{
					}
					theNet2.UnlockTranslator();
				}
			}
		}
	}
	BlockOrUnBlock();
}

BOOL CFileDownloadDlg::OnInitDialog() 
{
	CResizableDialog::OnInitDialog();

	//m_FileStatusImageList.Create(IDB_DOWNLOAD_FILESTATUS,16,1,0xff00ff);
	m_SortHeader.SubclassWindow(m_FileDownLoadList.GetHeaderCtrl()->GetSafeHwnd());
	
	// TODO: Add extra initialization here
	AddAnchor(IDC_OFFLINE_BUTTON,CSize(100,100),CSize(100,100));
	AddAnchor(IDC_DELETE_BUTTON,CSize(100,100),CSize(100,100));
	AddAnchor(IDC_REMEMBERLATER_BUTTON,CSize(100,100),CSize(100,100));
	AddAnchor(IDC_DOWNLOAD_BUTTON,CSize(100,100),CSize(100,100));
	AddAnchor(IDC_CANCEL_BUTTON,CSize(100,100),CSize(100,100));
	AddAnchor(IDC_FILE_DOWNLOAD_LIST,CSize(0,0),CSize(100,100));

	pSession = theNet2.GetSession();
	ASSERT(pSession!=NULL);

	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),FALSE);	
	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),TRUE);
	
	// TODO: Add extra initialization here
	DWORD ExStyle=m_FileDownLoadList.GetExtendedStyle ();
	ExStyle|=LVS_EX_FULLROWSELECT;
	m_FileDownLoadList.SetExtendedStyle (ExStyle);

	CString strSection = GetString(IDS_OFSMESSENGER);
	CString strEntry = GetString(IDS_COLUMN_WIDTH);

	m_FileDownLoadList.InsertColumn (0,GetString(IDS_FILENAME_NAME),LVCFMT_LEFT,AfxGetApp()->GetProfileInt(strSection, strEntry+_T("10"), 120));
	m_FileDownLoadList.InsertColumn (1,GetString(IDS_SENDER_NAME),LVCFMT_CENTER,AfxGetApp()->GetProfileInt(strSection, strEntry+_T("11"), 110),1);	
	m_FileDownLoadList.InsertColumn (2,GetString(IDS_STATUS_NAME),LVCFMT_CENTER,AfxGetApp()->GetProfileInt(strSection, strEntry+_T("12"), 90),2);
	m_FileDownLoadList.InsertColumn (3,GetString(IDS_PROGRESS_NAME),LVCFMT_RIGHT,AfxGetApp()->GetProfileInt(strSection, strEntry+_T("13"), 60),3);
	m_FileDownLoadList.InsertColumn (4,GetString(IDS_DATE_NAME),LVCFMT_LEFT,AfxGetApp()->GetProfileInt(strSection, strEntry+_T("14"), 90),4);
	m_FileDownLoadList.InsertColumn (5,GetString(IDS_FILESIZE_NAME),LVCFMT_RIGHT,AfxGetApp()->GetProfileInt(strSection, strEntry+_T("15"), 65),5);	
	m_FileDownLoadList.InsertColumn (6,GetString(IDS_DESCRIPTION_NAME),LVCFMT_LEFT,AfxGetApp()->GetProfileInt(strSection, strEntry+_T("16"), 90 ),6);	

	m_iSortingMode	= GetOptionInt(IDS_OFSMESSENGER,IDS_INCOMING_FILE,3);

	//m_FileDownLoadList.SetImageList(&m_FileStatusImageList,LVSIL_SMALL);
	m_FileDownLoadList.SetImageList(CImageList::FromHandle(GetSystemImageList(TRUE)),LVSIL_SMALL);

	ShowDialog();

	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

LRESULT CFileDownloadDlg::OnNetEvent(WPARAM w,LPARAM l)
{
	NLT_Container *pItem = (NLT_Container *)w;
	ASSERT(pItem!=NULL);
	
	if(pItem->EventType==NLT_EOffLineFiles)
	{
		theNet2.LockTranslator();
		
		IFiles* pFiles = NULL;
		
		try
		{
			if(SUCCEEDED(AutoUnMarchaling(pItem,(LPUNKNOWN*)&pFiles)))
			{
				long Count  = pFiles->GetCount();
				for(int i=1;i<=Count;i++)
				{
					IFilePtr pFilePtr = pFiles->GetItem(i);
					
					IUserPtr pSender  = pFilePtr->GetSender ();
					CUser	User(pSender);

					if(!pMessenger->GetUserByGlobalId(User.GetGlobalID(),User))
					{
						theNet2.LockTranslator();
						try
						{

							long DetailsHandle = pSession->UserDetails(User.GetGlobalID(),1);
							if(DetailsHandle)
								theNet2.AddToTranslator(DetailsHandle,GetSafeHwnd());
						}
						catch(...)
						{
							ASSERT(FALSE);
						}
						theNet2.UnlockTranslator();
					}
					
					AddToOffline(User,pFilePtr);
					
				}
				pFiles->Release();
			}
		}
		catch(...)
		{
		}
		ASSERT(LoadOfflineFileHandle==pItem->Handel);
		theNet2.RemoveFromTranslator(LoadOfflineFileHandle);
		theNet2.UnlockTranslator();
		
	}
	else if(pItem->EventType==NLT_EDetails)
	{
		IUser *pUser	=	NULL;
		if(SUCCEEDED(AutoUnMarchaling(pItem,(LPUNKNOWN*)&pUser)))
		{
			theNet2.LockTranslator();
			theNet2.RemoveFromTranslator(pItem->Handel);
			try
			{
				CUser AddInfoUser(pUser);
				RefreshSenderDetails(AddInfoUser);
			}
			catch(...)
			{
				ASSERT(FALSE);
			}
			theNet2.UnlockTranslator();
			pUser->Release();
		}
	}
	else
	{
		long Handle = pItem->Handel;
		theNet2.LockTranslator();
		theNet2.RemoveFromTranslator(pItem->Handel);
		theNet2.UnlockTranslator();
		
		int   iIndex  = GetItemFromHandle(Handle);
		CDUFileInfo *pDUFileInfo=(CDUFileInfo *)m_FileDownLoadList.GetItemData (iIndex);	
		
		if(pDUFileInfo)
		{
			switch(pItem->EventType)
			{
			case NLT_ECommandOK:
				if(pMessenger->ConnectEnable() && 
					pDUFileInfo->iProgress != DP_WAIT&&
					pDUFileInfo->iProgress != DP_WAIT_DELETE)
				{
					ChangeStatus(DS_DOWNLOADED,iIndex);
					
					theNet2.LockTranslator();
					try
					{
						BSTR FileID  = pDUFileInfo->strFileID.AllocSysString();
						pDUFileInfo->pFile->PuthWnd(0L);
						if(pDUFileInfo->Handle = pSession->ConfirmFile(FileID,fcDelete))
						{
							ChangeProgress(DP_WAIT,iIndex);
							theNet2.AddToTranslator(pDUFileInfo->Handle,this->m_hWnd);
						}
						::SysFreeString(FileID);
					}
					catch(...)
					{
					}
					theNet2.UnlockTranslator();
				}
				else
				{
					if(pDUFileInfo->iProgress==DP_WAIT_DELETE)
					{
						m_LockList.Lock();
						try
						{
							for(int i = (ListArray.GetSize () - 1); i >= 0;i--)
							{
								CDUFileInfo *pUFileInfoList = ListArray.GetAt(i);
								if(pUFileInfoList == pDUFileInfo)
								{
									delete pUFileInfoList;
									ListArray.RemoveAt (i);
								}
							}
						}
						catch(...)
						{}
						m_LockList.Unlock();
						m_FileDownLoadList.DeleteItem(iIndex);
					}
					else
						ChangeProgress(DP_NONE,iIndex);
				}
				
				if(pDUFileInfo->iStatus==DS_OFFLINE&&!m_bShowOfflineFiles)
				{
					m_LockList.Lock();
					try
					{
						for(int i = (ListArray.GetSize () - 1); i >= 0;i--)
						{
							CDUFileInfo *pUFileInfoList = ListArray.GetAt(i);
							if(pUFileInfoList == pDUFileInfo)
							{
								delete pUFileInfoList;
								ListArray.RemoveAt (i);
							}
						}
					}
					catch(...)
					{}
					m_LockList.Unlock();
					m_FileDownLoadList.DeleteItem(iIndex);
				}

				break;
			case NLT_ECommandError:
				if(pItem->Long1==etSERVER)
				{
					switch(pItem->Long2)
					{
					case ERR_UNABLE_CREATE_CONN:
						_SHOW_IBN_ERROR_DLG_OK(IDS_SERVICENOTAVAILABLE);
						break;
					}
				}
				
				if(LoadOfflineFileHandle)
				{
					LoadOfflineFileHandle = 0L;
					MessageBox(GetString(IDS_IBN_FILE_MANAGER_NAME),GetString(IDS_CANT_LOAD_OFFLINE_FILES),MB_OK|MB_ICONSTOP);
				}
				else
				{
					if(pItem->Long1==etCANCEL)
						ChangeProgress(DP_CANCEL,iIndex); 
					else
						ChangeProgress(DP_ERROR ,iIndex);
				}
				break;
			}
		}
	}
	
	BlockOrUnBlock();

    delete pItem;	
	return 0;
}

LRESULT CFileDownloadDlg::OnDownloadBegin(WPARAM w,LPARAM l)
{
	int   iIndex  = GetItemFromHandle(long(w));
	if(iIndex==-1) ASSERT(FALSE);
	ChangeProgress(DP_PROCENT,iIndex,0);
	CDUFileInfo *pDUFileInfo = (CDUFileInfo *)m_FileDownLoadList.GetItemData (iIndex);
	pDUFileInfo->Size = (long)l;
	//CTime tStartTime = CTime::GetCurrentTime();
	//pDUFileInfo->dwTime = (DWORD)tStartTime.GetTime();
	return 0;
}

LRESULT CFileDownloadDlg::OnDownloadStep(WPARAM w,LPARAM l)
{
	int   iIndex  = GetItemFromHandle(long(w));
	if(iIndex==-1) ASSERT(FALSE);
	ChangeProgress(DP_PROCENT,iIndex,long(l));
	return 0;
}



void CFileDownloadDlg::BlockOrUnBlock()
{
/*	BOOL bFlagSee1,bFlagSee2, bFlagSee3;
	
	if(LoadOfflineFileHandle)
	{
		bFlagSee1 = FALSE;
		bFlagSee2 = TRUE;
		bFlagSee3 = FALSE;
	}
	else
	{
		POSITION pos = m_FileDownLoadList.GetFirstSelectedItemPosition();
	
		if(pos)
		{
			bFlagSee1 = bFlagSee2 = bFlagSee3 = TRUE;

			while(pos)
			{
				int iSel = m_FileDownLoadList.GetNextSelectedItem(pos);
				
				CDUFileInfo *pDUFileInfop=(CDUFileInfo *)m_FileDownLoadList.GetItemData (iSel);
				
				if(pDUFileInfop->iStatus == DS_NEW)
				{
					bFlagSee1&=TRUE;
					bFlagSee2&=FALSE;
					bFlagSee3&=TRUE;
				}
				else				
				if(pDUFileInfop->iStatus == DS_OFFLINE && pDUFileInfop->iProgress == DP_NONE)
				{
					bFlagSee3&=TRUE;
					bFlagSee1&=FALSE;
					bFlagSee2&=FALSE;
				}
				else
				{
					bFlagSee3&=FALSE; 
					bFlagSee1&=FALSE;
					bFlagSee2&=TRUE;
				}

				if(pDUFileInfop->iProgress == DP_COMPLET)
				{
					bFlagSee3&=FALSE;
					bFlagSee1&=FALSE;
					bFlagSee2&=FALSE;
				}
			}
		}
		else
		{
			bFlagSee1=FALSE;
			bFlagSee2=FALSE;
			bFlagSee3 = FALSE;
		}
	}
*/
	GetDlgItem(IDC_SHOWOFFLINEFILES_CHECK)->EnableWindow(!LoadOfflineFileHandle);
	
//	m_btnRememberLater.EnableWindow (bFlagSee1);
//	m_btnOffLine.EnableWindow (bFlagSee1);
//	m_btnDownload.EnableWindow (bFlagSee3);
//	m_btnDelete.EnableWindow (bFlagSee3);
	
//	m_btnCancel.EnableWindow (bFlagSee2);	
}

void CFileDownloadDlg::OnTimer(UINT nIDEvent) 
{
	// TODO: Add your message handler code here and/or call default
	if(nIDEvent==IDC_DOWNLOAD_TIMER)
	{
		/*if(bWasSetComplet)
		{
			m_LockList.Lock();
			try
			{
				for(int i = (ListArray.GetSize () - 1); i >= 0;i--)
				{
					CDUFileInfo *pDUFileInfo = ListArray.GetAt(i);
					if(pDUFileInfo->iProgress == DP_COMPLET)
					{
						if(pDUFileInfo->iStatus==DS_OFFLINE&&m_bShowOfflineFiles)
						{
							pDUFileInfo->iProgress = DP_NONE;
						}
						else
						{
							delete pDUFileInfo;
							ListArray.RemoveAt (i);
						}
					}
				}
			}
			catch(...)
			{}
			m_LockList.Unlock();
			bWasSetComplet = FALSE;
			BuildList();
			BlockOrUnBlock();
		}
		else
		{
			//// Анимация Иконок .... ////
		}*/
	}
	
	CResizableDialog::OnTimer(nIDEvent);
}

void CFileDownloadDlg::OnClose() 
{
	// TODO: Add your message handler code here and/or call default
	//KillTimer(IDC_DOWNLOAD_TIMER);

	CString strSection = GetString(IDS_OFSMESSENGER);
	CString strEntry = GetString(IDS_COLUMN_WIDTH);

	AfxGetApp()->WriteProfileInt(strSection, strEntry+_T("10"), m_FileDownLoadList.GetColumnWidth(0));
	AfxGetApp()->WriteProfileInt(strSection, strEntry+_T("11"), m_FileDownLoadList.GetColumnWidth(1));
	AfxGetApp()->WriteProfileInt(strSection, strEntry+_T("12"), m_FileDownLoadList.GetColumnWidth(2));
	AfxGetApp()->WriteProfileInt(strSection, strEntry+_T("13"), m_FileDownLoadList.GetColumnWidth(3));
	AfxGetApp()->WriteProfileInt(strSection, strEntry+_T("14"), m_FileDownLoadList.GetColumnWidth(4));	
	AfxGetApp()->WriteProfileInt(strSection, strEntry+_T("15"), m_FileDownLoadList.GetColumnWidth(5));	
	AfxGetApp()->WriteProfileInt(strSection, strEntry+_T("16"), m_FileDownLoadList.GetColumnWidth(6));	

	WriteOptionInt(IDS_OFSMESSENGER,IDS_INCOMING_FILE,m_iSortingMode);

	CResizableDialog::OnClose();
}

void CFileDownloadDlg::ShowDialog()
{
	//SetTimer(IDC_DOWNLOAD_TIMER,1000,NULL);
	ShowWindow(SW_SHOWNORMAL);
	SetForegroundWindow();
	SetFocus();
}

void CFileDownloadDlg::OnItemchangedFileDownloadList(NMHDR* pNMHDR, LRESULT* pResult) 
{
	NM_LISTVIEW* pNMListView = (NM_LISTVIEW*)pNMHDR;
	// TODO: Add your control notification handler code here
	BlockOrUnBlock();
	*pResult = 0;
}

int CALLBACK CFileDownloadDlg::CompareListItem(LPARAM lParam1, LPARAM lParam2, LPARAM lParamSort)
{
	CDUFileInfo *pDUFileInfo1=(CDUFileInfo *)(lParam1);
	CDUFileInfo *pDUFileInfo2=(CDUFileInfo *)(lParam2);

	int	bValAddon	=	(lParamSort>=0?1:-1);

	switch(bValAddon*lParamSort)
	{
	case 1:
		// File Name [1/28/2002]
		if(pDUFileInfo1->strFileName<pDUFileInfo2->strFileName) return  -1*bValAddon; 
		if(pDUFileInfo1->strFileName>pDUFileInfo2->strFileName) return  1*bValAddon; 
		break;
	case 2:
		// Sender [1/28/2002]
		if(pDUFileInfo1->Sender.GetShowName()<pDUFileInfo2->Sender.GetShowName()) return  -1*bValAddon; 
		if(pDUFileInfo1->Sender.GetShowName()>pDUFileInfo2->Sender.GetShowName()) return  1*bValAddon; 
		break;
	case 3:
		//// Сортировка по статусу ...
		if(pDUFileInfo1->iStatus<pDUFileInfo2->iStatus) return  -1*bValAddon; 
		if(pDUFileInfo1->iStatus>pDUFileInfo2->iStatus) return  1*bValAddon; 
		break;
	case 4:
		// Progress [1/28/2002]
		if(pDUFileInfo1->iProgress<pDUFileInfo2->iProgress) return  -1*bValAddon; 
		if(pDUFileInfo1->iProgress>pDUFileInfo2->iProgress) return  1*bValAddon; 
		break;
	case 5:
		// Est.Time [1/28/2002]
		if(pDUFileInfo1->dwTime<pDUFileInfo2->dwTime) return  -1*bValAddon; 
		if(pDUFileInfo1->dwTime>pDUFileInfo2->dwTime) return  1*bValAddon; 
		break;
	case 6:
		// FileSize [1/28/2002]
		if(pDUFileInfo1->Size<pDUFileInfo2->Size) return  -1*bValAddon; 
		if(pDUFileInfo1->Size>pDUFileInfo2->Size) return  1*bValAddon; 
		break;
	case 7:
		// Description [1/28/2002]
		if(pDUFileInfo1->strMessage<pDUFileInfo2->strMessage) return  -1*bValAddon; 
		if(pDUFileInfo1->strMessage>pDUFileInfo2->strMessage) return  1*bValAddon; 
		break;
	}

	return 0;
}

void CFileDownloadDlg::SortList(int Mode)
{
	/// Mode in Futeres ...
	m_FileDownLoadList.SortItems (CompareListItem,(LPARAM)Mode);

	m_SortHeader.SetSortArrow((m_iSortingMode>0?m_iSortingMode:-m_iSortingMode)-1,(m_iSortingMode>0?TRUE:FALSE));
}

void CFileDownloadDlg::ChangeStatus(DFileStatus duNewStatus,int iElement)
{
	CDUFileInfo *pDUFileInfo=(CDUFileInfo *)m_FileDownLoadList.GetItemData (iElement);
	pDUFileInfo->iStatus = duNewStatus;
	m_FileDownLoadList.SetItemText (iElement,2,GetString(strDUStatus[duNewStatus]));

	if(duNewStatus== DS_DOWNLOADED) 
	{
		SaveFileToHistory(pDUFileInfo);
	}
}

BOOL CFileDownloadDlg::FindInList(CString FID)
{
	int nSize=m_FileDownLoadList.GetItemCount ();

	for(int i=0; i < nSize; i++)
	{
		CDUFileInfo *pDUFileInfo=(CDUFileInfo *)m_FileDownLoadList.GetItemData (i);
		if(pDUFileInfo->strFileID == FID)
			return TRUE;
	}

	return FALSE;


}

void CFileDownloadDlg::ChangeProgress(DProgressStatus NewProgress, int iElement, long dwGetSize)
{
	CDUFileInfo *pDUFileInfo=(CDUFileInfo *)m_FileDownLoadList.GetItemData (iElement);

	// OZ: Ignore old progress
	if(NewProgress == DP_PROCENT && dwGetSize>0 && pDUFileInfo->iProgress == DP_ERROR)
		return;

	pDUFileInfo->iProgress  = NewProgress;

	if(NewProgress == DP_PROCENT)
	{
		CString strtmp;
		long   dwSize=pDUFileInfo->Size;
		
		// Oleg Zhuk: Fix Problem incorrect transmission percentages  [12/4/2003]
		long   lProgress = long(dwGetSize*1.0/dwSize*100);
		if(lProgress>100)
			lProgress = 100;
		if(lProgress<0)
			lProgress = 0;
		// Oleg Zhuk End:[12/4/2003]
		
		strtmp.Format(_T("%d%%"), lProgress);
		m_FileDownLoadList.SetItemText (iElement,3,strtmp); 
	}
	else
	   m_FileDownLoadList.SetItemText (iElement,3,GetString(strDUProgress[NewProgress]));

}

int CFileDownloadDlg::GetItemFromHandle(long Handle)
{
	int nSize=m_FileDownLoadList.GetItemCount ();

	for(int i=0; i < nSize; i++)
	{
		CDUFileInfo *pDUFileInfo=(CDUFileInfo *)m_FileDownLoadList.GetItemData (i);
		if(pDUFileInfo->Handle == Handle)
			return i;
	}

	return -1;
}

//DEL int CFileDownloadDlg::GetSelectedItem()
//DEL {
//DEL 	POSITION pos = m_FileDownLoadList.GetFirstSelectedItemPosition();
//DEL 	return m_FileDownLoadList.GetNextSelectedItem(pos);
//DEL }

void CFileDownloadDlg::BuildList()
{
    CString tmp;
	
	m_LockList.Lock();
	//Возможно Умное Удалене .... :)
	try
	{
		m_FileDownLoadList.DeleteAllItems ();
		
		for(int i = 0;i < ListArray.GetSize ();i++)
		{
			CDUFileInfo *pDUFileInfo = ListArray.GetAt(i);
			int iSubIndex = m_FileDownLoadList.InsertItem (LVIF_TEXT|LVIF_PARAM|LVIF_IMAGE,i,pDUFileInfo->strFileName,0,0,GetIconIndexInSystemImageList(TRUE,pDUFileInfo->strFileName),(LPARAM)pDUFileInfo);
			m_FileDownLoadList.SetItemText (iSubIndex,1,LPCTSTR(pDUFileInfo->Sender.GetShowName()));
			m_FileDownLoadList.SetItemText (iSubIndex,2,GetString(strDUStatus[pDUFileInfo->iStatus]));
			m_FileDownLoadList.SetItemText (iSubIndex,3,GetString(strDUProgress[pDUFileInfo->iProgress]));
			// Create Date String [6/19/2002]
			SYSTEMTIME	sysFileTime		=	TimeTToSystemTime(pDUFileInfo->dwTime);
			TCHAR	szDate[MAX_PATH]=_T(""), szTime[MAX_PATH]=_T("");
			
			GetDateFormat(LOCALE_USER_DEFAULT,DATE_SHORTDATE,&sysFileTime,NULL,szDate,MAX_PATH);
			GetTimeFormat(LOCALE_USER_DEFAULT,NULL,&sysFileTime,NULL,szTime,MAX_PATH);
			
			CString	strDataFormat;
			strDataFormat.Format(_T("%s %s"),szDate,szTime);
			// End Create Date String [6/19/2002]
			
			m_FileDownLoadList.SetItemText (iSubIndex,4,strDataFormat);// Est.Time ...
			m_FileDownLoadList.SetItemText (iSubIndex,5,ByteSizeToStr(pDUFileInfo->Size));
			m_FileDownLoadList.SetItemText (iSubIndex,6,LPCTSTR(pDUFileInfo->strMessage));
		}
		
		SortList(m_iSortingMode);
	}
	catch(...)
	{}
	m_LockList.Unlock();
}

void CFileDownloadDlg::AddToDownload(CUser &Sender,IFile *pFile)
{
	CString FID = (char*)pFile->GetFID();
	
	if(FindInList(FID))	return;


	CDUFileInfo *pDUFileInfo = new CDUFileInfo;
	
	pDUFileInfo->pFile = pFile;
	pDUFileInfo->bOut = FALSE;
	pDUFileInfo->Size = pFile->GetSize();
    pDUFileInfo->dwTime = pFile->Getdate_time();
	pDUFileInfo->iProgress = DP_NONE;
	pDUFileInfo->iStatus = DS_NEW;
	pDUFileInfo->strFileID = FID;
	_bstr_t Test = pFile->GetRealName();
	pDUFileInfo->strFileName = (char*)Test;
	pDUFileInfo->strFileURL = "Not use";
	pDUFileInfo->strLocalPath = "";
	pDUFileInfo->strMessage = (char*)pFile->GetBody();
	pDUFileInfo->strRecepient = "You";
	pDUFileInfo->Sender = Sender;
	pDUFileInfo->Handle = 0;
	
	m_LockList.Lock();
	try
	{
		ListArray.Add (pDUFileInfo);
	}
	catch(...)
	{}
	m_LockList.Unlock();
	
	
	BuildList();

	BlockOrUnBlock();

	//ShowDialog();
}


void CFileDownloadDlg::OnCancelButton() 
{
	POSITION pos = m_FileDownLoadList.GetFirstSelectedItemPosition();

	if(pos==NULL) return;

	if(pMessenger->ConnectEnable () )
	{
		if(LoadOfflineFileHandle)
		{
			try
			{
				pSession->CancelOperation(LoadOfflineFileHandle);
			}
			catch(...)
			{
			}
		}
		else
		{
			while(pos)
			{
				CDUFileInfo *pDUFileInfo = NULL;

				int iSel = m_FileDownLoadList.GetNextSelectedItem(pos);

				pDUFileInfo = (CDUFileInfo *)m_FileDownLoadList.GetItemData (iSel);	
				try
				{
					pSession->CancelOperation(pDUFileInfo->Handle);
				}
				catch(...)
				{
				}
			}
		}
	}
	BlockOrUnBlock();
}

void CFileDownloadDlg::AddToOffline(CUser &Sender, IFile *pFile)
{
	CString FID = (char*)pFile->GetFID();
	
	if(FindInList(FID))	return;
	
	CDUFileInfo *pDUFileInfo = new CDUFileInfo;
	
	pDUFileInfo->pFile = pFile;
	pDUFileInfo->bOut = FALSE;
	pDUFileInfo->Size = pFile->GetSize();
    pDUFileInfo->dwTime = pFile->Getdate_time();
	pDUFileInfo->iProgress = DP_NONE;
	pDUFileInfo->iStatus = DS_OFFLINE;
	pDUFileInfo->strFileID = FID;
	pDUFileInfo->strFileName = (char*)pFile->GetRealName();
	pDUFileInfo->strFileURL = "Not use";
	pDUFileInfo->strLocalPath = "";
	pDUFileInfo->strMessage = (char*)pFile->GetBody();
	pDUFileInfo->strRecepient = _T("You");
	pDUFileInfo->Sender = Sender;
	pDUFileInfo->Handle = 0;
	
	m_LockList.Lock();
	try
	{
		ListArray.Add (pDUFileInfo);
	}
	catch(...)
	{}
	m_LockList.Unlock();
	
	
	BuildList();

	BlockOrUnBlock();
	
	ShowDialog();
}

void CFileDownloadDlg::OnShowofflinefilesCheck() 
{
	///UpdateData();
	
	if(m_bShowOfflineFiles)
	{
		// Load Offline Files from Server [1/28/2002]
		if(pMessenger->ConnectEnable())
		{
			theNet2.LockTranslator();
			try
			{
				
				LoadOfflineFileHandle = pSession->LoadOffLineFiles();
				if(LoadOfflineFileHandle)
					theNet2.AddToTranslator(LoadOfflineFileHandle,GetSafeHwnd());
			}
			catch (...) 
			{
			}
			theNet2.UnlockTranslator();
		}
	}
	else
	{
		// Delete All Offline Files
		m_LockList.Lock();
		//Возможно Умное Удалене .... :)
		try
		{
			for(int i = (ListArray.GetSize ()-1);i >=0 ;i--)
			{
				CDUFileInfo *pDUFileInfo = ListArray.GetAt(i);
				if(pDUFileInfo->iStatus==DS_OFFLINE)
				{
					ListArray.RemoveAt(i);
					delete pDUFileInfo;
					pDUFileInfo = NULL;
				}
			}
		}
		catch(...)
		{}
		m_LockList.Unlock();
		BuildList();
	}
	BlockOrUnBlock();
}

void CFileDownloadDlg::OnItemclickFileDownloadList(NMHDR* pNMHDR, LRESULT* pResult) 
{
	HD_NOTIFY *phdn = (HD_NOTIFY *) pNMHDR;

	if((phdn->iItem+1)==(m_iSortingMode>=0?m_iSortingMode:-m_iSortingMode))
		m_iSortingMode = -1*m_iSortingMode;
	else
		m_iSortingMode = phdn->iItem+1;
	
	SortList(m_iSortingMode);
	
	*pResult = 0;
}

void CFileDownloadDlg::OnDblclkFileDownloadList(NMHDR* pNMHDR, LRESULT* pResult) 
{
	NM_LISTVIEW* pNMListView = (NM_LISTVIEW*)pNMHDR;
	if(pNMListView->iItem!=-1)
	{
		CDUFileInfo *pDUFileInfop=(CDUFileInfo *)m_FileDownLoadList.GetItemData (pNMListView->iItem);
		
		if(pDUFileInfop!=NULL)
		{
			if(pDUFileInfop->iStatus!=DS_DOWNLOADED)
			{
				if(pDUFileInfop->iProgress!=DP_WAIT&&
					pDUFileInfop->iProgress!=DP_WAIT_DELETE&&
					pDUFileInfop->iProgress!=DP_PROCENT)
				{
					OnDownloadButton();
				}
			}
			else
			{
				OnFiledownloadOpen();
			}
			/*CFileDescriptioDlg	DescrDlg(this);
			DescrDlg.m_strFileName = pDUFileInfop->strFileName;
			DescrDlg.DoModalReadMode(pDUFileInfop->strMessage);*/
		}
	}
		
	*pResult = 0;
}

void CFileDownloadDlg::RefreshSenderDetails(CUser &User)
{
	BOOL bWasChange	=	FALSE;
	m_LockList.Lock();
	try
	{
		for(int i = 0;i < ListArray.GetSize ();i++)
		{
			CDUFileInfo *pDUFileInfo = ListArray.GetAt(i);
			if(pDUFileInfo->Sender==User)
			{
				pDUFileInfo->Sender = User;
				bWasChange = TRUE;
			}
		}
	}
	catch(...)
	{}
	m_LockList.Unlock ();

	if(bWasChange)
	{
		BuildList();
		BlockOrUnBlock();
	}
}

void CFileDownloadDlg::DeleteAllItem()
{
	m_bShowOfflineFiles = FALSE;
	while(ListArray.GetSize () )
	{
		delete ListArray.GetAt (0);
		ListArray.RemoveAt (0);
	}
	m_FileDownLoadList.DeleteAllItems ();
	m_lastUserId = 0;
}

BOOL CFileDownloadDlg::LoadFilesHistory()
{
	if(m_lastUserId==pMessenger->GetUserID())
		return TRUE;

	m_lastUserId=pMessenger->GetUserID();

	CString strSection;
	strSection.Format(_T("%s\\%s\\%d"),GetString(IDS_INFO),pMessenger->GetUserRole(),pMessenger->GetUserID());

	CString strUploadedFilesXML = GetRegFileText(strSection,GetString(IDS_INCOMING_FILE));
	
	/************************************************************************/
	/* 
	<file_history type="download">
		<file fid="">
			<body></body>
			<real_name></real_name>
			<size></size>
			<path></path>
			<sender_id></sender_id>
		</file>
	</file_history>
	*/
	/************************************************************************/
	HRESULT hr = S_OK;
	
	if(!strUploadedFilesXML.IsEmpty())
	{
		// Step 1. Load to XML Document [3/26/2002]
		CComPtr<IXMLDOMDocument>	pDoc	=	NULL;
		hr = pDoc.CoCreateInstance(CLSID_DOMDocument40, NULL, CLSCTX_INPROC_SERVER);
		if(SUCCEEDED(hr))
		{
			VARIANT_BOOL varLoad = VARIANT_FALSE;
			hr = pDoc->loadXML(CComBSTR(strUploadedFilesXML),&varLoad);
			
			if(SUCCEEDED(hr)&&varLoad==VARIANT_TRUE)
			{
				CComPtr<IXMLDOMNodeList>	pFileList	=	NULL;
				hr = pDoc->selectNodes(CComBSTR(L"file_history/file"),&pFileList);
				if(SUCCEEDED(hr)&&pFileList!=NULL)
				{
					long Count = 0;
					pFileList->get_length(&Count);
					for(long i=0;i<Count;i++)
					{
						CComPtr<IXMLDOMNode>	pFile	=	NULL;
						pFileList->get_item(i,&pFile);
						if(pFile!=NULL)
						{
							CComVariant	varFID;
							CComBSTR	bsBody, bsRealName, bsSize, bsSenderId, bsPath, bsDate;
							GetAttribute(pFile,CComBSTR(L"fid"),&varFID);
							GetTextByPath(pFile,CComBSTR(L"body"),&bsBody);
							GetTextByPath(pFile,CComBSTR(L"real_name"),&bsRealName);
							GetTextByPath(pFile,CComBSTR(L"size"),&bsSize);
							GetTextByPath(pFile,CComBSTR(L"sender_id"),&bsSenderId);
							GetTextByPath(pFile,CComBSTR(L"path"),&bsPath);
							GetTextByPath(pFile,CComBSTR(L"date"),&bsDate);
							
							CDUFileInfo *pNewFile = new CDUFileInfo;
							
							pNewFile->strFileID			=	varFID.bstrVal;
							pNewFile->Size				= _wtol(bsSize);
							pNewFile->dwTime			= _wtol(bsDate);
							pNewFile->Handle			= 0;
							pNewFile->iProgress			= DP_COMPLET;
							pNewFile->iStatus			= DS_DOWNLOADED;
							pNewFile->strFileName		= bsRealName;
							pNewFile->strMessage		= bsBody;
							pNewFile->strLocalPath		= bsPath;
							pNewFile->Sender.SetGlobalID(_wtol(bsSenderId));
							
							if(!pMessenger->GetUserByGlobalId(pNewFile->Sender.GetGlobalID(),pNewFile->Sender))
							{
								// TODO: Load User Details [3/27/2002]
								theNet2.LockTranslator();
								try
								{
									
									long DetailsHandle = pSession->UserDetails(pNewFile->Sender.GetGlobalID(),1);
									if(DetailsHandle)
										theNet2.AddToTranslator(DetailsHandle,GetSafeHwnd());
								}
								catch(...)
								{
									ASSERT(FALSE);
								}
								theNet2.UnlockTranslator();
							}
							
							m_LockList.Lock();
							try
							{
								ListArray.Add (pNewFile);
							}
							catch(...)
							{
							}
							m_LockList.Unlock();
							
						}
						
					}
					BuildList();
				}
			}
		}
	}

	return SUCCEEDED(hr);
}

BOOL CFileDownloadDlg::SaveFileToHistory(CDUFileInfo *pDUFileInfo)
{
	CString strSection;
	strSection.Format(_T("%s\\%s\\%d"),GetString(IDS_INFO),pMessenger->GetUserRole(),pMessenger->GetUserID());

	CString strUploadedFilesXML = GetRegFileText(strSection,GetString(IDS_INCOMING_FILE));
	
	/************************************************************************/
	/* 
	<file_history type="download">
		<file fid="">
			<body></body>
			<real_name></real_name>
			<size></size>
			<path></path>
			<sender_id></sender_id>
			<date></date>
		</file>
	</file_history>
	*/
	/************************************************************************/
	HRESULT hr = S_OK;

	// Step 1. Load to XML Document [3/26/2002]
	CComPtr<IXMLDOMDocument>	pDoc	=	NULL;
	hr = pDoc.CoCreateInstance(CLSID_DOMDocument40, NULL, CLSCTX_INPROC_SERVER);
	if(SUCCEEDED(hr))
	{
		VARIANT_BOOL varLoad = VARIANT_FALSE;
		hr = pDoc->loadXML(CComBSTR(strUploadedFilesXML),&varLoad);

		strUploadedFilesXML = _T("<file_history type=\"download\"></file_history>");

		if(varLoad!=VARIANT_TRUE)
		{
			hr = pDoc->loadXML(CComBSTR(strUploadedFilesXML),&varLoad);
		}
			
		if(SUCCEEDED(hr)&&varLoad==VARIANT_TRUE)
		{
			CComPtr<IXMLDOMNode>	pFileHistory	=	NULL;

			hr = pDoc->selectSingleNode(CComBSTR(L"file_history"),&pFileHistory);
			if(pFileHistory==NULL)
			{
				hr = pDoc->loadXML(CComBSTR(strUploadedFilesXML),&varLoad);
				hr = pDoc->selectSingleNode(CComBSTR(L"file_history"),&pFileHistory);
			}

			if(pFileHistory!=NULL)
			{
				CComPtr<IXMLDOMNode>	pFileCurrent	=	NULL;
				
				CString strFindFile;
				strFindFile.Format(_T("file[@fid=\"%d\"]"),pDUFileInfo->strFileID);

				pFileHistory->selectSingleNode(CComBSTR(strFindFile),&pFileCurrent);

				if(pFileCurrent==NULL)
				{
					hr = insertSingleNode(pFileHistory,CComBSTR(L"file"),NULL,NULL,&pFileCurrent);
					
					if(pFileCurrent!=NULL)
					{
						CComBSTR bsSize, bsSenderId, bsDate;
						VarBstrFromI4(pDUFileInfo->Size,NULL,LOCALE_NOUSEROVERRIDE,&bsSize);
						VarBstrFromI4(pDUFileInfo->Sender.GetGlobalID(),NULL,LOCALE_NOUSEROVERRIDE,&bsSenderId);
						VarBstrFromI4(pDUFileInfo->dwTime,NULL,LOCALE_NOUSEROVERRIDE,&bsDate);
						
						
						insertSingleAttribut(pFileCurrent,CComBSTR(L"fid"),CComVariant(pDUFileInfo->strFileID));
						insertSingleNode(pFileCurrent,CComBSTR(L"body"),NULL,CComBSTR((LPCTSTR)pDUFileInfo->strMessage));
						insertSingleNode(pFileCurrent,CComBSTR(L"real_name"),NULL,CComBSTR((LPCTSTR)pDUFileInfo->strFileName));
						insertSingleNode(pFileCurrent,CComBSTR(L"size"),NULL,bsSize);
						insertSingleNode(pFileCurrent,CComBSTR(L"path"),NULL,CComBSTR(pDUFileInfo->strLocalPath));
						insertSingleNode(pFileCurrent,CComBSTR(L"sender_id"),NULL,bsSenderId);
						insertSingleNode(pFileCurrent,CComBSTR(L"date"),NULL,bsDate);
					}
				}
			}
			if(SUCCEEDED(hr))
			{
				CComBSTR	bsOutXML;
				pDoc->get_xml(&bsOutXML);
				SetRegFileText(strSection,GetString(IDS_INCOMING_FILE),CString(bsOutXML));
			}
		}
	}
	
	return SUCCEEDED(hr);
}

void CFileDownloadDlg::OnRclickFileDownloadList(NMHDR* pNMHDR, LRESULT* pResult) 
{
	CPoint point;
	GetCursorPos(&point);
	CMenu menu;
	menu.LoadMenu(IDR_MESSENGER_MENU);
	CMenu* popup = menu.GetSubMenu(6);
	UpdateMenu(this,popup);
	popup->TrackPopupMenu(TPM_LEFTALIGN|TPM_RIGHTBUTTON, point.x, point.y, this);
	
	*pResult = 0;
}

void CFileDownloadDlg::OnFiledownloadCanceltransform() 
{
	OnCancelButton();
	
}

void CFileDownloadDlg::OnUpdateFiledownloadCanceltransform(CCmdUI* pCmdUI) 
{
	BOOL bFlagSee	=	FALSE;
	
	POSITION	pos	=	m_FileDownLoadList.GetFirstSelectedItemPosition();
	
	if(pos)
	{
		bFlagSee	=	TRUE;
		while(pos)
		{
			int iSel = m_FileDownLoadList.GetNextSelectedItem(pos);
			
			CDUFileInfo *pUFileInfop=(CDUFileInfo *)m_FileDownLoadList.GetItemData (iSel);
			
			if(pUFileInfop->iProgress != DP_WAIT &&pUFileInfop->iProgress != DP_PROCENT)
			{
				bFlagSee = FALSE;
				break;
			}
		}
	}
	
	pCmdUI->Enable(bFlagSee);
}

void CFileDownloadDlg::OnFiledownloadClearrecord() 
{
	POSITION	pos	=	m_FileDownLoadList.GetFirstSelectedItemPosition();
	
	if(pos)
	{
		while(pos)
		{
			int iSel = m_FileDownLoadList.GetNextSelectedItem(pos);
			
			CDUFileInfo *pUFileInfop=(CDUFileInfo *)m_FileDownLoadList.GetItemData (iSel);
			
			if(pUFileInfop->iStatus==DS_DOWNLOADED)
			{
				RemoveFileFromHistory(pUFileInfop);
				
				m_LockList.Lock();
				try
				{
					for(int i = (ListArray.GetSize () - 1); i >= 0;i--)
					{
						CDUFileInfo *pUFileInfoList = ListArray.GetAt(i);
						if(pUFileInfoList == pUFileInfop)
						{
							delete pUFileInfoList;
							ListArray.RemoveAt (i);
						}
					}
				}
				catch(...)
				{}
				m_LockList.Unlock();
				
				m_FileDownLoadList.DeleteItem(iSel);
				
				pos	=	m_FileDownLoadList.GetFirstSelectedItemPosition();
			}
			else 
			{
				theNet2.LockTranslator();
				try
				{
					BSTR FileID = pUFileInfop->strFileID.AllocSysString();
					if(pUFileInfop->Handle = pSession->ConfirmFile(FileID,fcDelete))
					{
						ChangeProgress(DP_WAIT_DELETE,iSel);
						theNet2.AddToTranslator(pUFileInfop->Handle,this->m_hWnd);
					}
					::SysFreeString(FileID);
				}
				catch(...)
				{
				}
				theNet2.UnlockTranslator();
			}
		}
	}
}

void CFileDownloadDlg::OnUpdateFiledownloadClearrecord(CCmdUI* pCmdUI) 
{
	BOOL bFlagSee	=	FALSE;
	
	POSITION	pos	=	m_FileDownLoadList.GetFirstSelectedItemPosition();
	
	if(pos)
	{
		bFlagSee	=	TRUE;
		while(pos)
		{
			int iSel = m_FileDownLoadList.GetNextSelectedItem(pos);
			
			CDUFileInfo *pUFileInfop=(CDUFileInfo *)m_FileDownLoadList.GetItemData (iSel);
			
			if(pUFileInfop->iProgress == DP_WAIT || pUFileInfop->iProgress == DP_PROCENT)
			{
				bFlagSee = FALSE;
				break;
			}
		}
	}
	
	pCmdUI->Enable(bFlagSee);
}	

void CFileDownloadDlg::OnFiledownloadDownload() 
{
	OnDownloadButton();
}

void CFileDownloadDlg::OnUpdateFiledownloadDownload(CCmdUI* pCmdUI) 
{
	BOOL bFlagSee	=	FALSE;
	
	POSITION	pos	=	m_FileDownLoadList.GetFirstSelectedItemPosition();
	
	if(pos)
	{
		bFlagSee	=	TRUE;
		while(pos)
		{
			int iSel = m_FileDownLoadList.GetNextSelectedItem(pos);
			
			CDUFileInfo *pUFileInfop=(CDUFileInfo *)m_FileDownLoadList.GetItemData (iSel);
			
			if(pUFileInfop->iProgress == DP_WAIT ||pUFileInfop->iProgress == DP_PROCENT ||pUFileInfop->iStatus==DS_DOWNLOADED)
			{
				bFlagSee = FALSE;
				break;
			}
		}
	}
	
	pCmdUI->Enable(bFlagSee);
}

void CFileDownloadDlg::OnFiledownloadInformation() 
{
	POSITION	pos	=	m_FileDownLoadList.GetFirstSelectedItemPosition();
	
	if(pos)
	{
		int iSel = m_FileDownLoadList.GetNextSelectedItem(pos);

		CDUFileInfo *pDUFileInfop=(CDUFileInfo *)m_FileDownLoadList.GetItemData (iSel);
		
		if(pDUFileInfop!=NULL)
		{
			CFileDescriptioDlg	DescrDlg(this);
			DescrDlg.m_strFileName = pDUFileInfop->strFileName;
			DescrDlg.DoModalReadMode(pDUFileInfop->strMessage);
		}
	}
	
}

void CFileDownloadDlg::OnUpdateFiledownloadInformation(CCmdUI* pCmdUI) 
{
	BOOL bFlagSee	=	FALSE;
	
	POSITION	pos	=	m_FileDownLoadList.GetFirstSelectedItemPosition();
	
	if(pos)
	{
		bFlagSee	=	TRUE;
	}
	
	pCmdUI->Enable(bFlagSee);
	
}

void CFileDownloadDlg::OnFiledownloadMovetooffline() 
{
	OnOfflineButton();
}

void CFileDownloadDlg::OnUpdateFiledownloadMovetooffline(CCmdUI* pCmdUI) 
{
	BOOL bFlagSee	=	FALSE;
	
	POSITION	pos	=	m_FileDownLoadList.GetFirstSelectedItemPosition();
	
	if(pos)
	{
		bFlagSee	=	TRUE;
		while(pos)
		{
			int iSel = m_FileDownLoadList.GetNextSelectedItem(pos);
			
			CDUFileInfo *pUFileInfop=(CDUFileInfo *)m_FileDownLoadList.GetItemData (iSel);
			
			if(pUFileInfop->iProgress == DP_WAIT ||pUFileInfop->iProgress == DP_PROCENT ||pUFileInfop->iStatus!=DS_NEW)
			{
				bFlagSee = FALSE;
				break;
			}
		}
	}
	
	pCmdUI->Enable(bFlagSee);
}

void CFileDownloadDlg::OnFiledownloadOpen() 
{
	POSITION	pos	=	m_FileDownLoadList.GetFirstSelectedItemPosition();
	
	if(pos)
	{
		int iSel = m_FileDownLoadList.GetNextSelectedItem(pos);
		
		CDUFileInfo *pDUFileInfop=(CDUFileInfo *)m_FileDownLoadList.GetItemData (iSel);
		
		if(pDUFileInfop!=NULL)
		{
			long hInstance = (long)ShellExecute(GetSafeHwnd(),_T("open"),pDUFileInfop->strLocalPath,NULL,NULL,SW_SHOWNORMAL);
			
			if(hInstance<=32)
			{
				switch(hInstance) 
				{
				case SE_ERR_NOASSOC:
				case SE_ERR_ASSOCINCOMPLETE:
					{
						CString sOpenWithDlgPath = _T("shell32.dll,OpenAs_RunDLL ") + pDUFileInfop->strLocalPath;
						ShellExecute(NULL,_T("open"),_T("Rundll32.exe"), sOpenWithDlgPath, NULL, SW_SHOWNORMAL);
					}
					break;
				default:
					LPVOID lpMsgBuf;
					FormatMessage( 
						FORMAT_MESSAGE_ALLOCATE_BUFFER | 
						FORMAT_MESSAGE_FROM_SYSTEM | 
						FORMAT_MESSAGE_IGNORE_INSERTS,
						NULL,
						GetLastError(),
						MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), // Default language
						(LPTSTR) &lpMsgBuf,
						0,
						NULL 
						);
					
					MessageBox((LPCTSTR)lpMsgBuf,GetString(IDS_OPEN_FILE_ERROR_NAME),MB_OK | MB_ICONINFORMATION );
					
					// Free the buffer.
					LocalFree( lpMsgBuf );
				}
			}
		}
	}
}

void CFileDownloadDlg::OnUpdateFiledownloadOpen(CCmdUI* pCmdUI) 
{
	BOOL bFlagSee	=	FALSE;
	
	POSITION	pos	=	m_FileDownLoadList.GetFirstSelectedItemPosition();
	
	if(pos)
	{
		bFlagSee	=	TRUE;
		while(pos)
		{
			int iSel = m_FileDownLoadList.GetNextSelectedItem(pos);
			
			CDUFileInfo *pUFileInfop=(CDUFileInfo *)m_FileDownLoadList.GetItemData (iSel);
			
			if(pUFileInfop->iStatus!=DS_DOWNLOADED)
			{
				bFlagSee = FALSE;
				break;
			}
		}
	}
	
	pCmdUI->Enable(bFlagSee);
}

BOOL CFileDownloadDlg::RemoveFileFromHistory(CDUFileInfo *pDUFileInfo)
{
	CString strSection;
	strSection.Format(_T("%s\\%s\\%d"),GetString(IDS_INFO),pMessenger->GetUserRole(),pMessenger->GetUserID());

	CString strUploadedFilesXML = GetRegFileText(strSection,GetString(IDS_INCOMING_FILE));
	
	HRESULT hr = S_OK;

	// Step 1. Load to XML Document [3/26/2002]
	CComPtr<IXMLDOMDocument>	pDoc	=	NULL;
	hr = pDoc.CoCreateInstance(CLSID_DOMDocument40, NULL, CLSCTX_INPROC_SERVER);
	if(SUCCEEDED(hr))
	{
		VARIANT_BOOL varLoad = VARIANT_FALSE;
		hr = pDoc->loadXML(CComBSTR(strUploadedFilesXML),&varLoad);
		
		if(SUCCEEDED(hr)&&varLoad==VARIANT_TRUE)
		{
			CComPtr<IXMLDOMNode>	pFileHistory	=	NULL;
			
			hr = pDoc->selectSingleNode(CComBSTR(L"file_history"),&pFileHistory);
			
			if(pFileHistory!=NULL)
			{
				CComPtr<IXMLDOMNode>	pFileCurrent	=	NULL;
				
				CString strFindFile;
				strFindFile.Format(_T("file[@fid=\"%s\"]"),pDUFileInfo->strFileID);
				
				pFileHistory->selectSingleNode(CComBSTR(strFindFile),&pFileCurrent);
				
				if(pFileCurrent!=NULL)
				{
					hr = pFileHistory->removeChild(pFileCurrent,NULL);
					if(SUCCEEDED(hr))
					{
						CComBSTR	bsOutXML;
						pDoc->get_xml(&bsOutXML);
						SetRegFileText(strSection,GetString(IDS_INCOMING_FILE),CString(bsOutXML));
					}
				}
			}
		}
	}
	
	return SUCCEEDED(hr);
}

void CFileDownloadDlg::OnFiledownloadGotodir() 
{
	POSITION	pos	=	m_FileDownLoadList.GetFirstSelectedItemPosition();
	
	if(pos)
	{
		int iSel = m_FileDownLoadList.GetNextSelectedItem(pos);
		
		CDUFileInfo *pDUFileInfop=(CDUFileInfo *)m_FileDownLoadList.GetItemData (iSel);
		
		if(pDUFileInfop!=NULL)
		{
			CString strDir = pDUFileInfop->strLocalPath.Left(pDUFileInfop->strLocalPath.ReverseFind(_T('\\')));
			long hInstance = (long)ShellExecute(GetSafeHwnd(),_T("explore"),strDir,NULL,NULL,SW_SHOWNORMAL);
			if(hInstance<=32)
			{
				LPVOID lpMsgBuf;
				FormatMessage( 
					FORMAT_MESSAGE_ALLOCATE_BUFFER | 
					FORMAT_MESSAGE_FROM_SYSTEM | 
					FORMAT_MESSAGE_IGNORE_INSERTS,
					NULL,
					GetLastError(),
					MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), // Default language
					(LPTSTR) &lpMsgBuf,
					0,
					NULL 
					);
				
				MessageBox((LPCTSTR)lpMsgBuf,GetString(IDS_OPEN_FILE_ERROR_NAME),MB_OK | MB_ICONINFORMATION );
				
				// Free the buffer.
				LocalFree( lpMsgBuf );
			}
		}
	}
}

void CFileDownloadDlg::OnUpdateFiledownloadGotodir(CCmdUI* pCmdUI) 
{
	BOOL bFlagSee	=	FALSE;
	
	POSITION	pos	=	m_FileDownLoadList.GetFirstSelectedItemPosition();
	
	if(pos)
	{
		bFlagSee	=	TRUE;
		while(pos)
		{
			int iSel = m_FileDownLoadList.GetNextSelectedItem(pos);
			
			CDUFileInfo *pUFileInfop=(CDUFileInfo *)m_FileDownLoadList.GetItemData (iSel);
			
			if(pUFileInfop->iStatus!=DS_DOWNLOADED)
			{
				bFlagSee = FALSE;
				break;
			}
		}
	}
	
	pCmdUI->Enable(bFlagSee);
}


BOOL CFileDownloadDlg::PreTranslateMessage(MSG* pMsg) 
{
	if(pMsg->message==WM_KEYDOWN)
	{
		switch(pMsg->wParam) 
		{
		case VK_ESCAPE:
			if(m_FileDownLoadList.GetEditControl() && pMsg->hwnd==m_FileDownLoadList.GetEditControl()->GetSafeHwnd())
			{
			}
			else
				((CDialog*)GetParent())->EndDialog(IDCANCEL);
			break;
		}
	}

	return CResizableDialog::PreTranslateMessage(pMsg);
}