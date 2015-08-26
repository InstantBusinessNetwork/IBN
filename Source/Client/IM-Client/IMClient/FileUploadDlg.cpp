// FileUploadDlg.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "FileUploadDlg.h"
#include "resource.h"
#include "MainDlg.h"
#include "FileDescriptioDlg.h"
#include "SupportXMLFunction.h"
#include "MessageDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CFileUploadDlg dialog
// TCHAR strUFileStatus[3][20]   = {"Incomplete", "Complete", "Incomplete"};
DWORD strUFileStatus[4]	=	{IDS_FILE_INCOMPLETE_NAME, IDS_FILE_COMPLETE_NAME,IDS_FILE_INCOMPLETE_NAME, IDS_FILE_PUBLISHED_NAME};
// TCHAR strUFileProgress[6][20] = {"", "Wait", "0%", "", "Cancel", "Error" };
DWORD strUFileProgress[6]	=	{IDS_FILE_EMPTY,IDS_FILE_WAIT_NAME ,IDS_FILE_START_NAME,IDS_FILE_EMPTY,IDS_FILE_CANCEL_NAME,IDS_FILE_ERROR_NAME};

CFileUploadDlg::CFileUploadDlg(CWnd* pParent /*=NULL*/)
	: CResizableDialog(CFileUploadDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CFileUploadDlg)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
	//CoInitialize(NULL);
	bWasSetComplet = FALSE;
	m_lastUserId = 0;

	//CMcLoaderImpl	test;
}

CFileUploadDlg::~CFileUploadDlg()
{
	while(ListArray.GetSize () )
	{
		CUFileInfo *pItem = ListArray.GetAt (0);
		delete ListArray.GetAt (0);
		ListArray.RemoveAt (0);
	}
}


void CFileUploadDlg::DoDataExchange(CDataExchange* pDX)
{
	CResizableDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CFileUploadDlg)
	DDX_Control(pDX, IDC_CANCEL_BUTTON, m_btnCancel);
	DDX_Control(pDX, IDC_DELETEFILE_BUTTON, m_btnDeleteFile);
	DDX_Control(pDX, IDC_UPLOAD_BUTTON, m_btnUpLoadFile);
	DDX_Control(pDX, IDC_FILE_UPLOAD_LIST, m_FileUpLoadList);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CFileUploadDlg, CResizableDialog)
	//{{AFX_MSG_MAP(CFileUploadDlg)
	ON_BN_CLICKED(IDOK, OnOk)
	ON_WM_CLOSE()
	ON_WM_TIMER()
	ON_NOTIFY(LVN_ITEMCHANGED, IDC_FILE_UPLOAD_LIST, OnItemchangedFileUploadList)
	ON_NOTIFY(HDN_ITEMCLICK, 0, OnItemclickFileUploadList)
	ON_NOTIFY(NM_DBLCLK, IDC_FILE_UPLOAD_LIST, OnDblclkFileUploadList)
	ON_NOTIFY(NM_RCLICK, IDC_FILE_UPLOAD_LIST, OnRclickFileUploadList)
	ON_COMMAND(ID_FILESUPLOAD_CANCELTRANSFER, OnFilesuploadCanceltransfer)
	ON_UPDATE_COMMAND_UI(ID_FILESUPLOAD_CANCELTRANSFER, OnUpdateFilesuploadCanceltransfer)
	ON_COMMAND(ID_FILESUPLOAD_CLEARRECORD, OnFilesuploadClearrecord)
	ON_UPDATE_COMMAND_UI(ID_FILESUPLOAD_CLEARRECORD, OnUpdateFilesuploadClearrecord)
	ON_COMMAND(ID_FILESUPLOAD_UPLOAD, OnFilesuploadUpload)
	ON_UPDATE_COMMAND_UI(ID_FILESUPLOAD_UPLOAD, OnUpdateFilesuploadUpload)
	ON_BN_CLICKED(IDC_DELETEFILE_BUTTON, OnDeletefileButton)
	ON_BN_CLICKED(IDC_UPLOAD_BUTTON, OnUploadButton)
	ON_BN_CLICKED(IDC_CANCEL_BUTTON, OnCancelButton)
	ON_MESSAGE(WM_NLT_CONTAINER_EVENT,OnNetEvent)
	ON_MESSAGE(WM_UPLOAD_BEGIN,OnUploadBegin)
	ON_MESSAGE(WM_UPLOAD_STEP,OnUploadStep)
	ON_MESSAGE(WM_UPLOAD_APP_PROGRESS,OnUploadAppProgress)
	ON_MESSAGE(WM_UPLOAD_APP_COMPLETED,OnUploadAppCompleted)
	ON_COMMAND(ID_FILESUPLOAD_INFORMATION, OnFilesuploadInformation)
	ON_UPDATE_COMMAND_UI(ID_FILESUPLOAD_INFORMATION, OnUpdateFilesuploadInformation)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CFileUploadDlg message handlers

void CFileUploadDlg::OnOk() 
{
	// TODO: Add your control notification handler code here
	
}

void CFileUploadDlg::OnCancel() 
{
	// TODO: Add extra cleanup here
	
	///CResizableDialog::OnCancel();
}

void CFileUploadDlg::OnDeletefileButton() 
{
	POSITION	pos		=	m_FileUpLoadList.GetFirstSelectedItemPosition();
	int CorrectIndex	=	0;
	while(pos)
	{
		int iSel	=	m_FileUpLoadList.GetNextSelectedItem(pos);
		m_LockList.Lock();
		try
		{
			delete ListArray.GetAt (iSel - CorrectIndex);
            ListArray.RemoveAt (iSel - CorrectIndex);
			CorrectIndex++;
		}
		catch(...)
		{
		}
		m_LockList.Unlock();
	}
	BuildList();
	BlockOrUnBlock();
}

void CFileUploadDlg::OnUploadButton() 
{
	POSITION	pos		=	m_FileUpLoadList.GetFirstSelectedItemPosition();
	if(pMessenger->ConnectEnable()&&pos)
	{
		while(pos)
		{
			int iSel = m_FileUpLoadList.GetNextSelectedItem(pos);

			CUFileInfo *pUFileInfop=(CUFileInfo *)m_FileUpLoadList.GetItemData (iSel);

			if(pUFileInfop->pMcFileUp==NULL)
			{
				try
				{
					pUFileInfop->pFile = pSession->CreateFile();
					BSTR FileName = pUFileInfop->strFileName.AllocSysString();
					pUFileInfop->pFile->PutRealName(FileName);
					::SysFreeString(FileName);
					//pUFileInfop->pFile->PutFID(GUIDGen());
					IUsersPtr pUsers = pUFileInfop->pFile->GetRecipients();
					
					if(pUFileInfop->RecepientID==-1)
					{
						CString strTmp = pUFileInfop->strRecepientID;
						while(!strTmp.IsEmpty())
						{
							long RecepientID = _ttol((LPCTSTR)strTmp);
							
							if(strTmp.Find(',')!=-1)
								strTmp = strTmp.Mid(strTmp.Find(',')+1);
							else
								strTmp.Empty();
							
							if(RecepientID)
							{
								IUserPtr  pUser  = pUsers->AddUser();
								pUser->PutValue("@id",RecepientID);
							}
						}
					}
					else
					{
						IUserPtr  pUser  = pUsers->AddUser();
						pUser->PutValue("@id",pUFileInfop->RecepientID);
					}
					
					pUFileInfop->pFile->PuthWnd(long(this->m_hWnd));
					pUFileInfop->pFile->PutBody((LPCTSTR)pUFileInfop->strDescription);
					theNet2.LockTranslator();
					try
					{
						HRESULT hr = pUFileInfop->pFile->Send(&(pUFileInfop->Handle));
						if(SUCCEEDED(hr)&&pUFileInfop->Handle)
						{
							theNet2.AddToTranslator(pUFileInfop->Handle,this->m_hWnd);
							//CTime tStartTtime = CTime::GetCurrentTime();
							//pUFileInfop->dwTime = (DWORD)tStartTtime.GetTime () ;
							ChangeStatus(US_NEW,iSel);
							ChangeProgress(UP_WAIT, iSel);
						}
					}
					catch(...)
					{
					}
					theNet2.UnlockTranslator();
				}
				catch(...)
				{
				}
			}
			else
			{
				if(SUCCEEDED(pUFileInfop->pMcFileUp->Send(GetSafeHwnd(),WM_UPLOAD_APP_PROGRESS,WM_UPLOAD_APP_COMPLETED,pMessenger->GetServerPath(),pMessenger->GetSID())))
				{
					pUFileInfop->Handle			= (long)pUFileInfop->pMcFileUp;
					ChangeStatus(US_NEW,iSel);
					ChangeProgress(UP_WAIT, iSel);
				}
			}
		}
		
		BlockOrUnBlock();
	}
}

void CFileUploadDlg::OnCancelButton() 
{
	CUFileInfo *pUFileInfo = NULL;

	POSITION	pos		=	m_FileUpLoadList.GetFirstSelectedItemPosition();

	if(pos==NULL) return;

	if(pMessenger->ConnectEnable ())
	{
		while(pos)
		{
			int iSel = m_FileUpLoadList.GetNextSelectedItem(pos);
			pUFileInfo = (CUFileInfo *)m_FileUpLoadList.GetItemData (iSel);
			if(pUFileInfo->pMcFileUp==NULL)
			{
				if(pUFileInfo->Handle)
					pSession->CancelOperation(pUFileInfo->Handle);
			}
			else
			{
				try
				{
					(*(pUFileInfo->pMcFileUp))->Stop();
				}
				catch(_com_error&)
				{
					ASSERT(FALSE);
				}
			}
		}
	}
	
	BlockOrUnBlock();
}

void CFileUploadDlg::OnClose() 
{
	// TODO: Add your message handler code here and/or call default
    //KillTimer(IDC_UPLOAD_TIMER);
	
	CString strSection = GetString(IDS_OFSMESSENGER);
	CString strEntry = GetString(IDS_COLUMN_WIDTH);
	
	AfxGetApp()->WriteProfileInt(strSection, strEntry+_T("20"), m_FileUpLoadList.GetColumnWidth(0));
	AfxGetApp()->WriteProfileInt(strSection, strEntry+_T("21"), m_FileUpLoadList.GetColumnWidth(1));
	AfxGetApp()->WriteProfileInt(strSection, strEntry+_T("22"), m_FileUpLoadList.GetColumnWidth(2));
	AfxGetApp()->WriteProfileInt(strSection, strEntry+_T("23"), m_FileUpLoadList.GetColumnWidth(3));
	AfxGetApp()->WriteProfileInt(strSection, strEntry+_T("24"), m_FileUpLoadList.GetColumnWidth(4));	
	AfxGetApp()->WriteProfileInt(strSection, strEntry+_T("25"), m_FileUpLoadList.GetColumnWidth(5));	
	AfxGetApp()->WriteProfileInt(strSection, strEntry+_T("26"), m_FileUpLoadList.GetColumnWidth(6));	

	WriteOptionInt(IDS_OFSMESSENGER,IDS_OUTGOING_FILE,m_iSortingMode);
	
	CResizableDialog::OnClose();
}

BOOL CFileUploadDlg::OnInitDialog() 
{
	CResizableDialog::OnInitDialog();

	//m_FileStatusImageList.Create(IDB_DOWNLOAD_FILESTATUS,16,1,0xff00ff);
	m_SortHeader.SubclassWindow(m_FileUpLoadList.GetHeaderCtrl()->GetSafeHwnd());
	
	pSession = theNet2.GetSession();

	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),FALSE);
	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),TRUE);

	AddAnchor(IDC_DELETEFILE_BUTTON,CSize(100,100),CSize(100,100));
	AddAnchor(IDC_UPLOAD_BUTTON,CSize(100,100),CSize(100,100));
	AddAnchor(IDC_CANCEL_BUTTON,CSize(100,100),CSize(100,100));
	AddAnchor(IDC_FILE_UPLOAD_LIST,CSize(0,0),CSize(100,100));
	
	// TODO: Add extra initialization here
	DWORD ExStyle=m_FileUpLoadList.GetExtendedStyle ();
	ExStyle|=LVS_EX_FULLROWSELECT;
	m_FileUpLoadList.SetExtendedStyle (ExStyle);

	CString strSection = GetString(IDS_OFSMESSENGER);
	CString strEntry = GetString(IDS_COLUMN_WIDTH);

	m_FileUpLoadList.InsertColumn (0,GetString(IDS_FILENAME_NAME),LVCFMT_LEFT,AfxGetApp()->GetProfileInt(strSection, strEntry+_T("20"), 120));
	m_FileUpLoadList.InsertColumn (1,GetString(IDS_RECIPIENT_NAME),LVCFMT_CENTER,AfxGetApp()->GetProfileInt(strSection, strEntry+_T("21"), 110),1);	
	m_FileUpLoadList.InsertColumn (2,GetString(IDS_STATUS_NAME),LVCFMT_CENTER,AfxGetApp()->GetProfileInt(strSection, strEntry+_T("22"), 90),2);
	m_FileUpLoadList.InsertColumn (3,GetString(IDS_PROGRESS_NAME),LVCFMT_RIGHT,AfxGetApp()->GetProfileInt(strSection, strEntry+_T("23"), 60),3);
	m_FileUpLoadList.InsertColumn (4,GetString(IDS_DATE_NAME),LVCFMT_LEFT,AfxGetApp()->GetProfileInt(strSection, strEntry+_T("24"), 90),4);
	m_FileUpLoadList.InsertColumn (5,GetString(IDS_FILESIZE_NAME),LVCFMT_RIGHT,AfxGetApp()->GetProfileInt(strSection, strEntry+_T("25"), 65),5);	
	m_FileUpLoadList.InsertColumn (6,GetString(IDS_DESCRIPTION_NAME),LVCFMT_RIGHT,AfxGetApp()->GetProfileInt(strSection, strEntry+_T("26"), 90),6);	

	//m_FileStatusImageList.Attach(GetSystemImageList(TRUE));
	m_iSortingMode	= GetOptionInt(IDS_OFSMESSENGER,IDS_OUTGOING_FILE,3);
	
	m_FileUpLoadList.SetImageList(CImageList::FromHandle(GetSystemImageList(TRUE)),LVSIL_SMALL);

	ShowDialog();
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CFileUploadDlg::OnTimer(UINT nIDEvent) 
{
	if(bWasSetComplet)
	{
/*
				m_LockList.Lock();
				try
				{
					for(int i = (ListArray.GetSize () - 1); i >= 0;i--)
					{
						CUFileInfo *pUFileInfo = ListArray.GetAt(i);
						if(pUFileInfo->iProgress == UP_COMPLET)
						{
							delete pUFileInfo;
							ListArray.RemoveAt (i);
						}
					}
				}
				catch(...)
				{}
				m_LockList.Unlock();
				
				bWasSetComplet = FALSE;
				BuildList();
				BlockOrUnBlock();*/
		
	}
	else
	{
		//// Анимация Иконок .... ////
	}	
	
	CResizableDialog::OnTimer(nIDEvent);
}

void CFileUploadDlg::BlockOrUnBlock()
{
	/*BOOL bFlagSee1	=	TRUE,	bFlagSee2	=	TRUE;
	
	POSITION	pos	=	m_FileUpLoadList.GetFirstSelectedItemPosition();

	if(pos)
	{
		while(pos)
		{
			int iSel = m_FileUpLoadList.GetNextSelectedItem(pos);

			CUFileInfo *pUFileInfop=(CUFileInfo *)m_FileUpLoadList.GetItemData (iSel);
			
			if(pUFileInfop->iStatus == US_NEW || pUFileInfop->iStatus == US_STOP)
			{
				bFlagSee1&=TRUE;
				bFlagSee2&=FALSE;
			}
			else
			{
				bFlagSee1&=FALSE;
				bFlagSee2&=TRUE;
			}
			
			if(pUFileInfop->iProgress == UP_COMPLET)
			{
				bFlagSee1&=FALSE;
				bFlagSee2&=FALSE;
			}
		}
	}
	else
	{
		bFlagSee1=FALSE;
		bFlagSee2=FALSE;
	}

	m_btnUpLoadFile.EnableWindow (bFlagSee1);
	m_btnDeleteFile.EnableWindow (bFlagSee1);
	m_btnCancel.EnableWindow (bFlagSee2);*/
}

LRESULT CFileUploadDlg::OnNetEvent(WPARAM w,LPARAM l)
{
	NLT_Container *pItem = (NLT_Container *)w;
	ASSERT(pItem!=NULL);

	long Handle = pItem->Handel;
	theNet2.LockTranslator();
	theNet2.RemoveFromTranslator(pItem->Handel);
	theNet2.UnlockTranslator();

	int   iIndex  = GetItemFromHandle(Handle);
	if(iIndex!=-1)
	{
		switch(pItem->EventType)
		{
		case NLT_ECommandOK: 
			ChangeStatus(US_UPLOAD,iIndex);
			ChangeProgress(UP_COMPLET,iIndex);
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
			
			ChangeStatus(US_STOP,iIndex);
			if(pItem->Long1==etCANCEL)
				ChangeProgress(UP_CANCEL,iIndex); 
			else
				ChangeProgress(UP_ERROR ,iIndex);
			break;
		}
	}

	BlockOrUnBlock();
	
    delete pItem;	
	return 0;
}

LRESULT CFileUploadDlg::OnUploadBegin(WPARAM w,LPARAM l)
{
	TRACE(_T("\r\n	CFileUploadDlg::OnUploadBegin	LP	=	%d"), l);
    int   iIndex  = GetItemFromHandle(long(w));	
	if(iIndex!=-1)
	{
		ChangeProgress(UP_PROCENT,iIndex,0);
		CUFileInfo *pUFileInfo = (CUFileInfo *)m_FileUpLoadList.GetItemData (iIndex);
		pUFileInfo->BufferSize = (long)l;
    	//CTime tStartTime = CTime::GetCurrentTime();
		//pUFileInfo->dwTime = (DWORD)tStartTime.GetTime();
	}
	return 0;
}

LRESULT CFileUploadDlg::OnUploadStep(WPARAM w,LPARAM l)
{
	TRACE(_T("\r\n	CFileUploadDlg::OnUploadStep	LP	=	%d"), l);
    int   iIndex  = GetItemFromHandle(long(w));	
	if(iIndex!=-1) 
		ChangeProgress(UP_PROCENT,iIndex,(long)l);
	return 0;
}

void CFileUploadDlg::ShowDialog()
{
	ShowWindow(SW_SHOWNORMAL);
	SetForegroundWindow();
	SetFocus();
	//SetTimer(IDC_UPLOAD_TIMER,1000,NULL);
}

void CFileUploadDlg::AddToUpLoad(CString FileName, CString Login,long RecepientID, LPCTSTR strDescription)
{
	if(!TestFile(FileName))
		return;

	CUFileInfo *pNewFile = new CUFileInfo;
	
	CFile	UploadFile(FileName,CFile::modeRead|CFile::shareDenyNone);

    pNewFile->Size         = UploadFile.GetLength();
	UploadFile.Close();

	CTime tStartTtime = CTime::GetCurrentTime();
	pNewFile->dwTime       = (DWORD)tStartTtime.GetTime () ;

	pNewFile->Handle			= 0;
	pNewFile->iProgress			= UP_WAIT;
	pNewFile->iStatus			= US_NEW;
	pNewFile->strFileName		= FileName;
	pNewFile->RecepientID		= RecepientID;
	pNewFile->RecepientStr		= Login;
	pNewFile->strDescription	= strDescription;
	pNewFile->pMcFileUp			= NULL;

	m_LockList.Lock();
	try
	{
		ListArray.Add (pNewFile);
	}
	catch(...)
	{
	}
	m_LockList.Unlock();

	if(pMessenger->ConnectEnable())
	{
		try
		{
			CUFileInfo *pUFileInfop=(CUFileInfo *)pNewFile;

			pUFileInfop->pFile = pSession->CreateFile();
			BSTR FileName = pUFileInfop->strFileName.AllocSysString();
			pUFileInfop->pFile->PutRealName(FileName);
			::SysFreeString(FileName);

			IUsersPtr pUsers = pUFileInfop->pFile->GetRecipients();
			IUserPtr  pUser  = pUsers->AddUser();
			pUser->PutValue("@id",pUFileInfop->RecepientID);
			pUFileInfop->pFile->PuthWnd(long(this->m_hWnd));
			pUFileInfop->pFile->PutBody((LPCTSTR)pUFileInfop->strDescription);
			theNet2.LockTranslator();
			try
			{
				HRESULT hr = pUFileInfop->pFile->Send(&(pUFileInfop->Handle));
				if(SUCCEEDED(hr)&&pUFileInfop->Handle)
				{
					theNet2.AddToTranslator(pUFileInfop->Handle,this->m_hWnd);
					//CTime tStartTtime = CTime::GetCurrentTime();
					//pUFileInfop->dwTime = (DWORD)tStartTtime.GetTime () ;
				}
			}
			catch(...)
			{
				pNewFile->iProgress    = UP_ERROR;
				pNewFile->iStatus      = US_STOP;
			}
			theNet2.UnlockTranslator();
		}
		catch(...)
		{
			pNewFile->iProgress    = UP_ERROR;
			pNewFile->iStatus      = US_STOP;
		}
		
		BlockOrUnBlock();
	}
	else
	{
		pNewFile->iProgress    = UP_ERROR;
		pNewFile->iStatus      = US_STOP;
	}

	BuildList();
	//ShowDialog();
}

void CFileUploadDlg::BuildList()
{
   CString tmp;
	
   m_LockList.Lock();
   //Возможно Умное Удалене .... :)
   try
   {
	   m_FileUpLoadList.DeleteAllItems ();
	   
	   
	   for(int i = 0;i < ListArray.GetSize ();i++)
	   {
			CUFileInfo *pUFileInfo = ListArray.GetAt(i);
			CString	strTmp	=	pUFileInfo->strFileName;
			strTmp = strTmp.Mid(strTmp.ReverseFind('\\')+1);
		   
			int iSubIndex = m_FileUpLoadList.InsertItem (LVIF_TEXT|LVIF_PARAM|LVIF_IMAGE,i,strTmp,0,0,GetIconIndexInSystemImageList(TRUE,strTmp),(LPARAM)pUFileInfo);
			m_FileUpLoadList.SetItemText (iSubIndex,1,LPCTSTR(pUFileInfo->RecepientStr));
			m_FileUpLoadList.SetItemText (iSubIndex,2,GetString(strUFileStatus[pUFileInfo->iStatus]));
			m_FileUpLoadList.SetItemText (iSubIndex,3,GetString(strUFileProgress[pUFileInfo->iProgress]));

			// Create Date String [6/19/2002]
			SYSTEMTIME	sysFileTime		=	TimeTToSystemTime(pUFileInfo->dwTime);
			TCHAR	szDate[MAX_PATH]=_T(""), szTime[MAX_PATH]=_T("");

			GetDateFormat(LOCALE_USER_DEFAULT,DATE_SHORTDATE,&sysFileTime,NULL,szDate,MAX_PATH);
			GetTimeFormat(LOCALE_USER_DEFAULT,NULL,&sysFileTime,NULL,szTime,MAX_PATH);
		   
			CString	strDataFormat;
			strDataFormat.Format(_T("%s %s"),szDate,szTime);
			// End Create Date String [6/19/2002]

			m_FileUpLoadList.SetItemText (iSubIndex,4,strDataFormat);// Date ...

			m_FileUpLoadList.SetItemText (iSubIndex,5,ByteSizeToStr(pUFileInfo->Size));
			m_FileUpLoadList.SetItemText (iSubIndex,6,LPCTSTR(pUFileInfo->strDescription));
		}
		
		SortList(m_iSortingMode);
	}
	catch(...)
	{}
	m_LockList.Unlock();	
}

void CFileUploadDlg::SortList(int Mode)
{
	m_FileUpLoadList.SortItems (CompareListItem,(LPARAM)Mode);
	m_SortHeader.SetSortArrow((m_iSortingMode>0?m_iSortingMode:-m_iSortingMode)-1,(m_iSortingMode>0?TRUE:FALSE));
}

int CALLBACK CFileUploadDlg::CompareListItem(LPARAM lParam1, LPARAM lParam2, LPARAM lParamSort)
{
	CUFileInfo *pDUFileInfo1=(CUFileInfo *)(lParam1);
	CUFileInfo *pDUFileInfo2=(CUFileInfo *)(lParam2);
	
	int	bValAddon	=	(lParamSort>=0?1:-1);

	CString strFileName1, strFileName2;

	switch(bValAddon*lParamSort)
	{
	case 1:
		// File Name [1/28/2002]
		strFileName1 = pDUFileInfo1->strFileName.Right(pDUFileInfo1->strFileName.GetLength() - pDUFileInfo1->strFileName.ReverseFind(_T('\\')));
		strFileName2 = pDUFileInfo2->strFileName.Right(pDUFileInfo2->strFileName.GetLength() - pDUFileInfo2->strFileName.ReverseFind(_T('\\')));

		if(strFileName1<strFileName2) return  -1*bValAddon; 
		if(strFileName1>strFileName2) return  1*bValAddon; 
		break;
	case 2:
		// Sender [1/28/2002]
		if(pDUFileInfo1->RecepientStr <pDUFileInfo2->RecepientStr) return  -1*bValAddon; 
		if(pDUFileInfo1->RecepientStr>pDUFileInfo2->RecepientStr) return  1*bValAddon; 
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
		//if(pDUFileInfo1->strMessage<pDUFileInfo2->strMessage) return  -1; 
		//if(pDUFileInfo1->strMessage>pDUFileInfo2->strMessage) return  1; 
		break;
	}
	
	return 0;
}

void CFileUploadDlg::ChangeStatus(UFileStatus duNewStatus,int iElement)
{
	CUFileInfo *pUFileInfo=(CUFileInfo *)m_FileUpLoadList.GetItemData (iElement);
	pUFileInfo->iStatus = duNewStatus;
	m_FileUpLoadList.SetItemText (iElement,2,GetString(strUFileStatus[duNewStatus]));
	
}


void CFileUploadDlg::ChangeProgress(UFileProgress NewProgress, int iElement, long dwGetSize)
{
	CUFileInfo *pDUFileInfo=(CUFileInfo *)m_FileUpLoadList.GetItemData (iElement);
	pDUFileInfo->iProgress  = NewProgress;

	if(NewProgress == UP_COMPLET) 
	{
		//	Save item to Upolad DB
		bWasSetComplet = TRUE;
		pDUFileInfo->pFile = NULL;
		if(pDUFileInfo->strFID.IsEmpty())
			pDUFileInfo->strFID = (LPCTSTR)GUIDGen();
		SaveFileToHistory(pDUFileInfo);
	}

	if(NewProgress == UP_PROCENT)
	{
		CString strtmp;
		long   Size=pDUFileInfo->BufferSize;
		
		// Oleg Zhuk: Fix Problem incorrect transmission percentages  [12/4/2003]
		long   lProgress = long(dwGetSize*1.0/Size*100);
		if(lProgress>100)
			lProgress = 100;
		if(lProgress<0)
			lProgress = 0;
		// Oleg Zhuk End:[12/4/2003]

		strtmp.Format (_T("%d%%"), lProgress);
		m_FileUpLoadList.SetItemText (iElement,3,strtmp); 
	}
	else
	   m_FileUpLoadList.SetItemText (iElement,3,GetString(strUFileProgress[NewProgress]));

}

int CFileUploadDlg::GetItemFromHandle(long Handle)
{
	int nSize = m_FileUpLoadList.GetItemCount ();

	for(int i=0; i < nSize; i++)
	{
		CUFileInfo *pDUFileInfo=(CUFileInfo *)m_FileUpLoadList.GetItemData (i);
		if(pDUFileInfo!=NULL && pDUFileInfo->Handle == Handle)
			return i;
	}

	return -1;
}

//DEL int CFileUploadDlg::GetSelectedItem()
//DEL {
//DEL 	POSITION pos = m_FileUpLoadList.GetFirstSelectedItemPosition();
//DEL 	return m_FileUpLoadList.GetNextSelectedItem(pos);
//DEL }

void CFileUploadDlg::OnItemchangedFileUploadList(NMHDR* pNMHDR, LRESULT* pResult) 
{
	NM_LISTVIEW* pNMListView = (NM_LISTVIEW*)pNMHDR;
	// TODO: Add your control notification handler code here
	BlockOrUnBlock();
	
	*pResult = 0;
}

void CFileUploadDlg::SetMessenger(CMainDlg *pMessenger)
{
	this->pMessenger = pMessenger;
}

void CFileUploadDlg::OnItemclickFileUploadList(NMHDR* pNMHDR, LRESULT* pResult) 
{
	HD_NOTIFY *phdn = (HD_NOTIFY *) pNMHDR;
	
	if((phdn->iItem+1)==(m_iSortingMode>=0?m_iSortingMode:-m_iSortingMode))
		m_iSortingMode = -1*m_iSortingMode;
	else
		m_iSortingMode = phdn->iItem+1;
	
	SortList(m_iSortingMode);
	
	*pResult = 0;
}

void CFileUploadDlg::OnDblclkFileUploadList(NMHDR* pNMHDR, LRESULT* pResult) 
{
	NM_LISTVIEW* pNMListView = (NM_LISTVIEW*)pNMHDR;

	if(pNMListView->iItem!=-1)
	{
		CUFileInfo *pUFileInfop=(CUFileInfo *)m_FileUpLoadList.GetItemData (pNMListView->iItem);
		
		if(pUFileInfop!=NULL&&
			pUFileInfop->iProgress!=UP_WAIT&&
			pUFileInfop->iProgress!=UP_PROCENT)
		{
			CFileDescriptioDlg	DescrDlg(this);
			DescrDlg.m_strFileName = pUFileInfop->strFileName;

			if(pUFileInfop->iStatus==US_UPLOAD||pUFileInfop->iStatus==US_PUBLISHED)
			{
				DescrDlg.DoModalReadMode(pUFileInfop->strDescription);
			}
			else
			if(DescrDlg.DoModalEditMode(pUFileInfop->strDescription)==IDOK)
			{
				pUFileInfop->strDescription = DescrDlg.GetDescription();
				m_FileUpLoadList.SetItemText (pNMListView->iItem,6,pUFileInfop->strDescription);
			}
		}
	}
	
	*pResult = 0;
}

void CFileUploadDlg::AddToUpload2(CString FileName, CString Login, CString RecepientID, LPCTSTR strDescription)
{
	if(!TestFile(FileName))
		return;
	
	CUFileInfo *pNewFile = new CUFileInfo;
	
	CFile	UploadFile(FileName,CFile::modeRead|CFile::shareDenyNone);

    pNewFile->Size         = UploadFile.GetLength();
	UploadFile.Close();

	CTime tStartTtime = CTime::GetCurrentTime();
	pNewFile->dwTime       = (DWORD)tStartTtime.GetTime () ;
	pNewFile->Handle       = 0;
	pNewFile->iProgress    = UP_WAIT;
	pNewFile->iStatus      = US_NEW;
	pNewFile->strFileName  = FileName;
	pNewFile->RecepientID  = -1;
	pNewFile->strRecepientID = RecepientID;
	pNewFile->RecepientStr = Login;
	pNewFile->strDescription = strDescription;
	pNewFile->pMcFileUp = NULL;

	m_LockList.Lock();
	try
	{
		ListArray.Add (pNewFile);
	}
	catch(...)
	{
	}
	m_LockList.Unlock();

	//ShowDialog();
	
	if(pMessenger->ConnectEnable())
	{
		try
		{
			CUFileInfo *pUFileInfop=(CUFileInfo *)pNewFile;
			
			pUFileInfop->pFile = pSession->CreateFile();
			BSTR FileName = pUFileInfop->strFileName.AllocSysString();
			pUFileInfop->pFile->PutRealName(FileName);
			::SysFreeString(FileName);
			
			IUsersPtr pUsers = pUFileInfop->pFile->GetRecipients();
			CString strTmp = pUFileInfop->strRecepientID;
			while(!strTmp.IsEmpty())
			{
				long RecepientID = _ttol((LPCTSTR)strTmp);
				
				if(strTmp.Find(',')!=-1)
					strTmp = strTmp.Mid(strTmp.Find(',')+1);
				else
					strTmp.Empty();

				if(RecepientID)
				{
					IUserPtr  pUser  = pUsers->AddUser();
					pUser->PutValue("@id",RecepientID);
				}
			}
			pUFileInfop->pFile->PuthWnd(long(this->m_hWnd));
			pUFileInfop->pFile->PutBody((LPCTSTR)pUFileInfop->strDescription);
			theNet2.LockTranslator();
			try
			{
				HRESULT hr = pUFileInfop->pFile->Send(&(pUFileInfop->Handle));
				if(SUCCEEDED(hr)&&pUFileInfop->Handle)
				{
					theNet2.AddToTranslator(pUFileInfop->Handle,this->m_hWnd);
					//CTime tStartTtime = CTime::GetCurrentTime();
					//pUFileInfop->dwTime = (DWORD)tStartTtime.GetTime () ;
				}
			}
			catch(...)
			{
				pNewFile->iProgress    = UP_ERROR;
				pNewFile->iStatus      = US_STOP;
			}
			theNet2.UnlockTranslator();
		}
		catch(...)
		{
			pNewFile->iProgress    = UP_ERROR;
			pNewFile->iStatus      = US_STOP;
		}
		
		BlockOrUnBlock();
	}
	else
	{
		pNewFile->iProgress    = UP_ERROR;
		pNewFile->iStatus      = US_STOP;
	}

	BuildList();
}

void CFileUploadDlg::DeleteAllItem()
{
	while(ListArray.GetSize () )
	{
		delete ListArray.GetAt (0);
		ListArray.RemoveAt (0);
	}
	m_FileUpLoadList.DeleteAllItems();

	m_lastUserId = 0;

	m_pFromSender	=	NULL;
}

BOOL CFileUploadDlg::LoadFilesHistory()
{
	if(m_lastUserId==pMessenger->GetUserID())
		return TRUE;

	m_pFromSender	=	NULL;

	m_pFromSender.CreateInstance(CLSID_FormSender);

	try
	{
		// Init Net Settings [4/1/2002]
		m_pFromSender->PutUseProxyAuth(VARIANT_FALSE);

		CComBSTR	bsProxy;

		switch(GetOptionInt(IDS_NETOPTIONS,IDS_ACCESSTYPE,INTERNET_OPEN_TYPE_PRECONFIG))
		{
		case INTERNET_OPEN_TYPE_PRECONFIG:
			m_pFromSender->put_ProxyType(ptDefault);
			if(GetOptionInt(IDS_NETOPTIONS,IDS_USEFIREWALL,FALSE))
			{
				m_pFromSender->PutUseProxyAuth(VARIANT_TRUE);
				m_pFromSender->PutProxyUserName((LPCTSTR)GetOptionString(IDS_NETOPTIONS, IDS_FIREWALLUSER, _T("")));
				m_pFromSender->PutProxyPassword((LPCTSTR)(GetOptionString(IDS_NETOPTIONS, IDS_FIREWALLPASS, _T(""))));
			}
			break;
		case INTERNET_OPEN_TYPE_DIRECT:
			m_pFromSender->PutProxyType(ptDirect);
			break;
		case INTERNET_OPEN_TYPE_PROXY:
			m_pFromSender->PutProxyType(ptCustom);
			m_pFromSender->PutProxyServer((LPCTSTR)GetOptionString(IDS_NETOPTIONS, IDS_PROXYNAME, _T("")));

			if(!GetOptionString(IDS_NETOPTIONS, IDS_PROXYPORT, _T("")).IsEmpty())
			{
				m_pFromSender->PutProxyPort(_ttol(GetOptionString(IDS_NETOPTIONS, IDS_PROXYPORT, _T(""))));
			}
			
			if(GetOptionInt(IDS_NETOPTIONS,IDS_USEFIREWALL,FALSE))
			{
				m_pFromSender->PutUseProxyAuth(VARIANT_TRUE);
				m_pFromSender->PutProxyUserName((LPCTSTR)(GetOptionString(IDS_NETOPTIONS, IDS_FIREWALLUSER, _T(""))));
				m_pFromSender->PutProxyPassword((LPCTSTR)(GetOptionString(IDS_NETOPTIONS, IDS_FIREWALLPASS, _T(""))));
			}

			break;
		}
	}
	catch (_com_error&) 
	{
		ASSERT(FALSE);
	}



	m_lastUserId=pMessenger->GetUserID();

	CString strSection;
	strSection.Format(_T("%s\\%s\\%d"),GetString(IDS_INFO),pMessenger->GetUserRole(),pMessenger->GetUserID());

	CString strUploadedFilesXML = GetRegFileText(strSection,GetString(IDS_OUTGOING_FILE));
	
	/************************************************************************/
	/* 
	<file_history type="upload">
		<file fid="">
			<body></body>
			<real_name></real_name>
			<size></size>
			<recipients></recipients>
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
							CComBSTR	bsBody, bsRealName, bsSize, bsRecipients, bsTime;
							GetAttribute(pFile,CComBSTR(L"fid"),&varFID);
							GetTextByPath(pFile,CComBSTR(L"body"),&bsBody);
							GetTextByPath(pFile,CComBSTR(L"real_name"),&bsRealName);
							GetTextByPath(pFile,CComBSTR(L"size"),&bsSize);
							GetTextByPath(pFile,CComBSTR(L"recipients"),&bsRecipients);
							GetTextByPath(pFile,CComBSTR(L"date"),&bsTime);
							
							CUFileInfo *pNewFile = new CUFileInfo;
							 
							pNewFile->strFID			=	varFID.bstrVal;
							pNewFile->Size				= _wtol(bsSize);
							pNewFile->dwTime			= _wtol(bsTime);
							pNewFile->Handle			= 0;
							pNewFile->RecepientStr		= bsRecipients;
							

							pNewFile->iStatus			= pNewFile->RecepientStr==GetString(IDS_APP_NAME)?US_PUBLISHED:US_UPLOAD;
							pNewFile->iProgress			= UP_COMPLET;
														
							pNewFile->strFileName		= bsRealName;
							pNewFile->strDescription	= bsBody;
							
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

BOOL CFileUploadDlg::SaveFileToHistory(CUFileInfo *pUFileInfo)
{
	CString strSection;
	strSection.Format(_T("%s\\%s\\%d"),GetString(IDS_INFO),pMessenger->GetUserRole(),pMessenger->GetUserID());

	CString strUploadedFilesXML = GetRegFileText(strSection,GetString(IDS_OUTGOING_FILE));
	
	/************************************************************************/
	/* 
	<file_history type="upload">
		<file fid="">
			<body></body>
			<real_name></real_name>
			<size></size>
			<recipients></recipients>
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

		strUploadedFilesXML = _T("<file_history type=\"upload\"></file_history>");

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
				strFindFile.Format(_T("file[@fid=\"%d\"]"),pUFileInfo->strFID);

				pFileHistory->selectSingleNode(CComBSTR(strFindFile),&pFileCurrent);

				if(pFileCurrent==NULL)
				{
					hr = insertSingleNode(pFileHistory,CComBSTR(L"file"),NULL,NULL,&pFileCurrent);
					
					if(pFileCurrent!=NULL)
					{
						CComBSTR bsSize, bsDate;
						VarBstrFromI4(pUFileInfo->Size,NULL,LOCALE_NOUSEROVERRIDE,&bsSize);
						VarBstrFromI4(pUFileInfo->dwTime,NULL,LOCALE_NOUSEROVERRIDE,&bsDate);
						
						insertSingleAttribut(pFileCurrent,CComBSTR(L"fid"),CComVariant(pUFileInfo->strFID));
						insertSingleNode(pFileCurrent,CComBSTR(L"body"),NULL,CComBSTR((LPCTSTR)pUFileInfo->strDescription));
						insertSingleNode(pFileCurrent,CComBSTR(L"real_name"),NULL,CComBSTR((LPCTSTR)pUFileInfo->strFileName));
						insertSingleNode(pFileCurrent,CComBSTR(L"size"),NULL,bsSize);
						insertSingleNode(pFileCurrent,CComBSTR(L"recipients"),NULL,CComBSTR((LPCTSTR)pUFileInfo->RecepientStr));
						insertSingleNode(pFileCurrent,CComBSTR(L"date"),NULL,bsDate);
					}
				}
			}
			if(SUCCEEDED(hr))
			{
				CComBSTR	bsOutXML;
				pDoc->get_xml(&bsOutXML);
				SetRegFileText(strSection,GetString(IDS_OUTGOING_FILE),CString(bsOutXML));
			}
		}
	}
	
	return SUCCEEDED(hr);
}

void CFileUploadDlg::OnRclickFileUploadList(NMHDR* pNMHDR, LRESULT* pResult) 
{
	CPoint point;
	GetCursorPos(&point);
	CMenu menu;
	menu.LoadMenu(IDR_MESSENGER_MENU);
	CMenu* popup = menu.GetSubMenu(5);
	UpdateMenu(this,popup);
	popup->TrackPopupMenu(TPM_LEFTALIGN|TPM_RIGHTBUTTON, point.x, point.y, this);
	
	*pResult = 0;
}

void CFileUploadDlg::OnFilesuploadCanceltransfer() 
{
	OnCancelButton();
}

void CFileUploadDlg::OnUpdateFilesuploadCanceltransfer(CCmdUI* pCmdUI) 
{
	BOOL bFlagSee	=	FALSE;
	
	POSITION	pos	=	m_FileUpLoadList.GetFirstSelectedItemPosition();
	
	if(pos)
	{
		bFlagSee	=	TRUE;
		while(pos)
		{
			int iSel = m_FileUpLoadList.GetNextSelectedItem(pos);
			
			CUFileInfo *pUFileInfop=(CUFileInfo *)m_FileUpLoadList.GetItemData (iSel);
			
			if(pUFileInfop->iStatus != US_NEW)
			{
				bFlagSee = FALSE;
				break;
			}
		}
	}
	
	pCmdUI->Enable(bFlagSee);
}

void CFileUploadDlg::OnFilesuploadClearrecord() 
{
	POSITION	pos	=	m_FileUpLoadList.GetFirstSelectedItemPosition();
	
	if(pos)
	{
		while(pos)
		{
			int iSel = m_FileUpLoadList.GetNextSelectedItem(pos);
			
			CUFileInfo *pUFileInfop=(CUFileInfo *)m_FileUpLoadList.GetItemData (iSel);
			if(!pUFileInfop->strFID.IsEmpty())
			{
				RemoveFileFromHistory(pUFileInfop);
			}
			
			m_LockList.Lock();
			try
			{
				for(int i = (ListArray.GetSize () - 1); i >= 0;i--)
				{
					CUFileInfo *pUFileInfoList = ListArray.GetAt(i);
					if(pUFileInfoList == pUFileInfop)
					{
						delete pUFileInfoList;
						ListArray.RemoveAt (i);
						break;
					}
				}
			}
			catch(...)
			{}
			m_LockList.Unlock();
			
			m_FileUpLoadList.DeleteItem(iSel);

			pos	=	m_FileUpLoadList.GetFirstSelectedItemPosition();
		}
	}
}

void CFileUploadDlg::OnUpdateFilesuploadClearrecord(CCmdUI* pCmdUI) 
{
	BOOL bFlagSee	=	FALSE;
	
	POSITION	pos	=	m_FileUpLoadList.GetFirstSelectedItemPosition();
	
	if(pos)
	{
		bFlagSee	=	TRUE;
		while(pos)
		{
			int iSel = m_FileUpLoadList.GetNextSelectedItem(pos);
			
			CUFileInfo *pUFileInfop=(CUFileInfo *)m_FileUpLoadList.GetItemData (iSel);
			
			if(pUFileInfop->iStatus == US_NEW)
			{
				bFlagSee = FALSE;
				break;
			}
		}
	}
	
	pCmdUI->Enable(bFlagSee);
}

void CFileUploadDlg::OnFilesuploadUpload() 
{
	OnUploadButton();
}

void CFileUploadDlg::OnUpdateFilesuploadUpload(CCmdUI* pCmdUI) 
{
	BOOL bFlagSee	=	FALSE;
	
	POSITION	pos	=	m_FileUpLoadList.GetFirstSelectedItemPosition();
	
	if(pos)
	{
		bFlagSee	=	TRUE;
		while(pos)
		{
			int iSel = m_FileUpLoadList.GetNextSelectedItem(pos);
			
			CUFileInfo *pUFileInfop=(CUFileInfo *)m_FileUpLoadList.GetItemData (iSel);
			
			if(pUFileInfop->iStatus != US_STOP)
			{
				bFlagSee = FALSE;
				break;
			}
		}
	}
	
	pCmdUI->Enable(bFlagSee);
}

BOOL CFileUploadDlg::RemoveFileFromHistory(CUFileInfo *pUFileInfo)
{
	CString strSection;
	strSection.Format(_T("%s\\%s\\%d"),GetString(IDS_INFO),pMessenger->GetUserRole(),pMessenger->GetUserID());

	CString strUploadedFilesXML = GetRegFileText(strSection,GetString(IDS_OUTGOING_FILE));
	
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
				strFindFile.Format(_T("file[@fid=\"%s\"]"),pUFileInfo->strFID);

				pFileHistory->selectSingleNode(CComBSTR(strFindFile),&pFileCurrent);
				
				if(pFileCurrent!=NULL)
				{
					hr = pFileHistory->removeChild(pFileCurrent,NULL);
					if(SUCCEEDED(hr))
					{
						CComBSTR	bsOutXML;
						pDoc->get_xml(&bsOutXML);
						SetRegFileText(strSection,GetString(IDS_OUTGOING_FILE),CString(bsOutXML));
					}
				}
			}
		}
	}
	
	return SUCCEEDED(hr);
}

void CFileUploadDlg::OnFilesuploadInformation() 
{
	POSITION	pos	=	m_FileUpLoadList.GetFirstSelectedItemPosition();
	
	if(pos)
	{
		int iSel = m_FileUpLoadList.GetNextSelectedItem(pos);
		CUFileInfo *pUFileInfop=(CUFileInfo *)m_FileUpLoadList.GetItemData (iSel);

		CFileDescriptioDlg	DescrDlg(this);
		DescrDlg.m_strFileName = pUFileInfop->strFileName;
		
		if(pUFileInfop->iStatus==US_UPLOAD||pUFileInfop->iStatus==US_PUBLISHED)
		{
			DescrDlg.DoModalReadMode(pUFileInfop->strDescription);
		}
		else
			if(DescrDlg.DoModalEditMode(pUFileInfop->strDescription)==IDOK)
			{
				pUFileInfop->strDescription = DescrDlg.GetDescription();
				m_FileUpLoadList.SetItemText (iSel,6,pUFileInfop->strDescription);
			}
	}
}

void CFileUploadDlg::OnUpdateFilesuploadInformation(CCmdUI* pCmdUI) 
{
	BOOL bFlagSee	=	FALSE;
	
	POSITION	pos	=	m_FileUpLoadList.GetFirstSelectedItemPosition();
	
	if(pos)
	{
		bFlagSee	=	TRUE;
		while(pos)
		{
			int iSel = m_FileUpLoadList.GetNextSelectedItem(pos);
			
			CUFileInfo *pUFileInfop=(CUFileInfo *)m_FileUpLoadList.GetItemData (iSel);
			
			if(pUFileInfop->iStatus==US_NEW)
			{
				bFlagSee = FALSE;
				break;
			}
		}
	}
	
	pCmdUI->Enable(bFlagSee);
}

void CFileUploadDlg::AddToUpload3(LPCTSTR XML)
{
	//AfxMessageBox("Not Implemented.");
	//return;

	USES_CONVERSION;

	CComBSTR	bsXML	=	XML;

	CUFileInfo *pNewFile = new CUFileInfo;
	pNewFile->pMcFileUp = new CMcLoaderImpl(m_pFromSender);

	HRESULT hr = pNewFile->pMcFileUp->SetXML(bsXML);

	if(SUCCEEDED(hr))
	{
		//CString	FileName = pNewFile->pMcFileUp->GetFilePath();

		//if(!TestFile(FileName))
		//{
		//	delete pNewFile->pMcFileUp;
		//	delete pNewFile;
		//	return;
		//}

		//CFile	UploadFile(FileName,CFile::modeRead|CFile::shareDenyNone);
		
		pNewFile->Size         = (*pNewFile->pMcFileUp)->GetFormSize();
		pNewFile->BufferSize   = pNewFile->Size;
		
		//UploadFile.Close();
		CTime tStartTtime			= CTime::GetCurrentTime();
		pNewFile->dwTime			= (DWORD)tStartTtime.GetTime () ;

		pNewFile->Handle			= (long)pNewFile->pMcFileUp;
		pNewFile->iProgress			= UP_WAIT;
		pNewFile->iStatus			= US_NEW;
		pNewFile->strFileName		= W2CT(pNewFile->pMcFileUp->GetTitle());
		pNewFile->RecepientID		= 0;
		pNewFile->RecepientStr		= GetString(IDS_APP_NAME);
		pNewFile->strDescription	= _T(""); 
		
		m_LockList.Lock();
		try
		{
			ListArray.Add (pNewFile);
		}
		catch(...)
		{
		}
		m_LockList.Unlock();
		
		if(pMessenger->ConnectEnable())
		{
			HRESULT hr = pNewFile->pMcFileUp->Send(GetSafeHwnd(),WM_UPLOAD_APP_PROGRESS,WM_UPLOAD_APP_COMPLETED,pMessenger->GetServerPath(),pMessenger->GetSID());
			if(FAILED(hr))
			{
				pNewFile->iProgress    = UP_ERROR;
				pNewFile->iStatus      = US_STOP;
			}
		}
		else
		{
			pNewFile->iProgress    = UP_ERROR;
			pNewFile->iStatus      = US_STOP;
		}
		
		BuildList();
	}
	else
	{
		delete pNewFile->pMcFileUp;
		delete pNewFile;
	}
}

LRESULT CFileUploadDlg::OnUploadAppProgress(WPARAM w,LPARAM l)
{
    int   iIndex  = GetItemFromHandle(long(w));	
	if(iIndex!=-1)
	{
		CUFileInfo *pUFileInfo = (CUFileInfo *)m_FileUpLoadList.GetItemData (iIndex);
		ChangeProgress(UP_PROCENT,iIndex,l);
	}
	return 0;
}

LRESULT CFileUploadDlg::OnUploadAppCompleted(WPARAM w,LPARAM l)
{
    int   iIndex  = GetItemFromHandle(long(w));	
	if(iIndex!=-1)
	{
		if(l==0)
		{
			//All Ok
			CUFileInfo *pUFileInfo = (CUFileInfo *)m_FileUpLoadList.GetItemData (iIndex);
			if(pUFileInfo->pMcFileUp)
			{
				ChangeStatus(US_PUBLISHED,iIndex);
				ChangeProgress(UP_COMPLET,iIndex);

				pUFileInfo->Handle = 0;
				delete pUFileInfo->pMcFileUp;
				pUFileInfo->pMcFileUp = NULL;
			}
		}
		else
		{
			// Error
			ChangeStatus(US_STOP,iIndex);
			ChangeProgress(UP_ERROR ,iIndex);
		}
	}
	return 0;
}

BOOL CFileUploadDlg::TestFile(LPCTSTR FilePath)
{
	// New Action [7/22/2002]
	/************************************************************************/
	/*	When dragging a file that is already open by an application, 
		the IBN should not allow the user to attempt the send and log 
		it in the file manager.  It should open up a dialog that says 
		"The file you are trying to send is currently open.  Please close the 
		file and then send it again."
	*/
	/************************************************************************/

	BOOL	bRetFlag = FALSE;
	
	HANDLE hFile = CreateFile(FilePath,GENERIC_READ,FILE_SHARE_READ|FILE_SHARE_WRITE,NULL,OPEN_EXISTING,0,0);
	
	if(hFile==INVALID_HANDLE_VALUE)
	{
		if(GetLastError()==32)
		{
			CMessageDlg	BadFile(IDS_BAD_FILE,this);

			CString	strMessage;
			strMessage.Format(GetString(IDS_CANT_UPLOAD_FILE_ERROR_2_FORMAT),FilePath);

			BadFile.Show(strMessage,MB_OK);
		}
		else
		{
			LPVOID lpMsgBuf;
			FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
				NULL,GetLastError(),MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), // Default language
				(LPTSTR) &lpMsgBuf,	0,NULL 
				);
			// Process any inserts in lpMsgBuf.
			CString	strMessage;
			strMessage.Format(GetString(IDS_CANT_UPLOAD_FILE_ERROR_FORMAT),FilePath,(LPCTSTR)lpMsgBuf);
			
			CMessageDlg	BadFile(IDS_BAD_FILE,this);
			if(BadFile.Show(strMessage,MB_YESNO)==IDYES)
			{
				bRetFlag = TRUE;
			}
			
			//MessageBox( NULL, (LPCTSTR)lpMsgBuf, "Error", MB_OK | MB_ICONINFORMATION );
			// Free the buffer.
			LocalFree( lpMsgBuf );
		}
	}
	else
	{
		bRetFlag = TRUE;
		CloseHandle(hFile);
	}

	return bRetFlag;
}

BOOL CFileUploadDlg::PreTranslateMessage(MSG* pMsg) 
{
	if(pMsg->message==WM_KEYDOWN)
	{
		switch(pMsg->wParam) 
		{
		case VK_ESCAPE:
			if(m_FileUpLoadList.GetEditControl() && pMsg->hwnd==m_FileUpLoadList.GetEditControl()->GetSafeHwnd())
			{
			}
			else
				((CDialog*)GetParent())->EndDialog(IDCANCEL);
			break;
		}
	}

	return CResizableDialog::PreTranslateMessage(pMsg);
}