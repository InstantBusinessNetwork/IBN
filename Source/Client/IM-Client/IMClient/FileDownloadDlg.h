#if !defined(AFX_FILEDOWNLOADDLG_H__C7DF052D_6E04_4947_8AE0_0BB1F961A79E__INCLUDED_)
#define AFX_FILEDOWNLOADDLG_H__C7DF052D_6E04_4947_8AE0_0BB1F961A79E__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// FileDownloadDlg.h : header file
//
#include "ResizableDialog.h"
#include "User.h"

/////////////////////////////////////////////////////////////////////////////
// CFileDownloadDlg dialog
class CMainDlg;

#include "SortHeaderCtrl.h"

class CFileDownloadDlg : public CResizableDialog
{
private:
	
enum DFileStatus
{
	DS_NEW = 0,
	DS_OFFLINE ,
	DS_DOWNLOADED ,
};

enum DProgressStatus
{
	DP_NONE = 0,
	DP_WAIT,
	DP_WAIT_DELETE,
	DP_PROCENT ,
	DP_COMPLET,
	DP_CANCEL,
	DP_ERROR
};

struct CDUFileInfo
{
	CString strFileID;
	CString strFileName;
	CString strFileURL;
	CString strLocalPath;
	CUser	Sender;
	CString strRecepient;
	CString strMessage;
	long    Size;
	time_t  dwTime;
	BOOL    bOut; // TRUE UpLoad File ; FALSE Download File;
	DFileStatus iStatus;
	DProgressStatus iProgress;
	long   Handle;
	IFilePtr pFile;
};
	
// Construction
public:
	CFileDownloadDlg(CWnd* pParent = NULL);   // standard constructor
    virtual ~CFileDownloadDlg();
// Dialog Data
	//{{AFX_DATA(CFileDownloadDlg)
	enum { IDD = IDD_DOWNLOAD_DIALOG };
	CButton	m_btnRememberLater;
	CButton	m_btnOffLine;
	CButton	m_btnCancel;
	CButton	m_btnDownload;
	CButton	m_btnDelete;
	CListCtrl	m_FileDownLoadList;
	BOOL	m_bShowOfflineFiles;

	CImageList	m_FileStatusImageList;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CFileDownloadDlg)
	public:
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
public:
	BOOL LoadFilesHistory();
	void DeleteAllItem();
	void RefreshSenderDetails(CUser& User);
	void AddToOffline(CUser &Sender,IFile *pFile);
	void SetMessanger(CMainDlg *pMessenger);
	void AddToDownload(CUser &Sender,IFile *pFile);
	void ShowDialog();
	afx_msg void OnOfflineButton();
	afx_msg void OnDeleteButton();
	afx_msg void OnRememberlaterButton();
	afx_msg void OnDownloadButton();
	afx_msg void OnCancelButton();
	afx_msg void OnShowofflinefilesCheck();
	
protected:
	CSortHeaderCtrl	m_SortHeader;
	BOOL RemoveFileFromHistory(CDUFileInfo *pDUFileInfo);
	long LoadOfflineFileHandle;
	CMainDlg *pMessenger;
	ISessionPtr pSession;
	void BlockOrUnBlock();
	BOOL bWasSetComplet;
	CCriticalSection m_LockList;
	CArray <CDUFileInfo *,CDUFileInfo *> ListArray;
	long m_lastUserId;

	BOOL SaveFileToHistory(CDUFileInfo *pDUFileInfo);
	void BuildList();
	int GetItemFromHandle(long Handle);
	void ChangeProgress(DProgressStatus NewProgress, int iElement, long dwGetSize=0L);
	BOOL FindInList(CString FID);
	void ChangeStatus(DFileStatus duNewStatus,int iElement);
	void SortList(int Mode=2);
	static int CALLBACK CompareListItem(LPARAM lParam1, LPARAM lParam2, LPARAM lParamSort);
	//DWORD Handle;
	CArray <CDUFileInfo *,CDUFileInfo *> Array;
	LRESULT OnNetEvent(WPARAM w,LPARAM l);
	LRESULT OnDownloadBegin(WPARAM w,LPARAM l);
	LRESULT OnDownloadStep(WPARAM w,LPARAM l);

	int	m_iSortingMode;
	// Generated message map functions
	//{{AFX_MSG(CFileDownloadDlg)
	afx_msg void OnOk();
	virtual void OnCancel();
	virtual BOOL OnInitDialog();
	afx_msg void OnTimer(UINT nIDEvent);
	afx_msg void OnClose();
	afx_msg void OnItemchangedFileDownloadList(NMHDR* pNMHDR, LRESULT* pResult);	
	afx_msg void OnItemclickFileDownloadList(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnDblclkFileDownloadList(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnRclickFileDownloadList(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnFiledownloadCanceltransform();
	afx_msg void OnUpdateFiledownloadCanceltransform(CCmdUI* pCmdUI);
	afx_msg void OnFiledownloadClearrecord();
	afx_msg void OnUpdateFiledownloadClearrecord(CCmdUI* pCmdUI);
	afx_msg void OnFiledownloadDownload();
	afx_msg void OnUpdateFiledownloadDownload(CCmdUI* pCmdUI);
	afx_msg void OnFiledownloadInformation();
	afx_msg void OnUpdateFiledownloadInformation(CCmdUI* pCmdUI);
	afx_msg void OnFiledownloadMovetooffline();
	afx_msg void OnUpdateFiledownloadMovetooffline(CCmdUI* pCmdUI);
	afx_msg void OnFiledownloadOpen();
	afx_msg void OnUpdateFiledownloadOpen(CCmdUI* pCmdUI);
	afx_msg void OnFiledownloadGotodir();
	afx_msg void OnUpdateFiledownloadGotodir(CCmdUI* pCmdUI);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_FILEDOWNLOADDLG_H__C7DF052D_6E04_4947_8AE0_0BB1F961A79E__INCLUDED_)
