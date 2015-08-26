#if !defined(AFX_FILEUPLOADDLG_H__098B08BA_D249_4D9F_8074_7E7C7BE520F7__INCLUDED_)
#define AFX_FILEUPLOADDLG_H__098B08BA_D249_4D9F_8074_7E7C7BE520F7__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// FileUploadDlg.h : header file
//
#include "ResizableDialog.h"
#include "McLoaderImpl.h"
#include "SortHeaderCtrl.h"
/////////////////////////////////////////////////////////////////////////////
// CFileUploadDlg dialog
class CMainDlg;

class CFileUploadDlg : public CResizableDialog
{
private:
enum UFileStatus
{
	US_NEW = 0,
	US_UPLOAD,
	US_STOP,
	US_PUBLISHED
};

enum UFileProgress
{
	UP_NONE = 0,
	UP_WAIT,
	UP_PROCENT ,
	UP_COMPLET,
	UP_CANCEL,
	UP_ERROR,
};

struct CUFileInfo
{
	CUFileInfo()
	{
		pMcFileUp = NULL;
	};

	~CUFileInfo()
	{
		if(pMcFileUp)
		{
			try
			{
				delete pMcFileUp;
			}
			catch (...) 
			{
				ASSERT(FALSE);
			}
		}
	};

	CString			strFileName;
	CString			RecepientStr;
	long			RecepientID;
	CString			strRecepientID;
	long			Size;
	long			BufferSize;
	long			Handle;
	time_t			dwTime;
	UFileProgress	iProgress;
	UFileStatus		iStatus;
	IFilePtr		pFile;
	CString			strDescription;
	CString			strFID;
	CMcLoaderImpl	*pMcFileUp;
};

// Construction
public:
	void AddToUpload3(LPCTSTR XML);
	BOOL LoadFilesHistory();
	void DeleteAllItem();
	void AddToUpload2(CString FileName, CString Login, CString RecepientID, LPCTSTR strDescription);
	void AddToUpLoad(CString FileName, CString Login,long RecepientId, LPCTSTR strDescription);
	void ShowDialog();
	void SetMessenger(CMainDlg *pMessenger);
	CFileUploadDlg(CWnd* pParent = NULL);   // standard constructor
    virtual ~CFileUploadDlg();
	afx_msg void OnDeletefileButton();
	afx_msg void OnUploadButton();
	afx_msg void OnCancelButton();
	
// Dialog Data
	//{{AFX_DATA(CFileUploadDlg)
	enum { IDD = IDD_DIALOG_FILE_UPLOAD };
	CButton	m_btnCancel;
	CButton	m_btnDeleteFile;
	CButton	m_btnUpLoadFile;
	CListCtrl	m_FileUpLoadList;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CFileUploadDlg)
	public:
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	CSortHeaderCtrl	m_SortHeader;

	BOOL TestFile(LPCTSTR FilePath);
	BOOL RemoveFileFromHistory(CUFileInfo *pUFileInfo);
	BOOL SaveFileToHistory(CUFileInfo *pDUFileInfo);

	CImageList			m_FileStatusImageList;
	CMainDlg			*pMessenger;
    ISessionPtr			pSession;
	BOOL				bWasSetComplet;
	long				m_lastUserId;
	IFormSenderPtr		m_pFromSender;

	int	m_iSortingMode;

	static int CALLBACK CompareListItem(LPARAM lParam1, LPARAM lParam2, LPARAM lParamSort);
	int GetItemFromHandle(long Handle);
	void ChangeProgress(UFileProgress NewProgress, int iElement, long dwGetSize=0L);
	void ChangeStatus(UFileStatus duNewStatus,int iElement);
	void SortList(int Mode=2);
	CCriticalSection m_LockList;
	void BuildList();
	CArray <CUFileInfo *,CUFileInfo *> ListArray;
	LRESULT OnNetEvent(WPARAM w,LPARAM l);
	LRESULT OnUploadBegin(WPARAM w,LPARAM l);
	LRESULT OnUploadStep(WPARAM w,LPARAM l);
	void BlockOrUnBlock();
	// Generated message map functions
	//{{AFX_MSG(CFileUploadDlg)
	afx_msg void OnOk();
	virtual void OnCancel();
	afx_msg void OnClose();
	virtual BOOL OnInitDialog();
	afx_msg void OnTimer(UINT nIDEvent);
	afx_msg void OnItemchangedFileUploadList(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnItemclickFileUploadList(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnDblclkFileUploadList(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnRclickFileUploadList(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnFilesuploadCanceltransfer();
	afx_msg void OnUpdateFilesuploadCanceltransfer(CCmdUI* pCmdUI);
	afx_msg void OnFilesuploadClearrecord();
	afx_msg void OnUpdateFilesuploadClearrecord(CCmdUI* pCmdUI);
	afx_msg void OnFilesuploadUpload();
	afx_msg void OnUpdateFilesuploadUpload(CCmdUI* pCmdUI);
	afx_msg void OnFilesuploadInformation();
	afx_msg void OnUpdateFilesuploadInformation(CCmdUI* pCmdUI);
	afx_msg LRESULT OnUploadAppProgress(WPARAM w,LPARAM l);
	afx_msg LRESULT OnUploadAppCompleted(WPARAM w,LPARAM l);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_FILEUPLOADDLG_H__098B08BA_D249_4D9F_8074_7E7C7BE520F7__INCLUDED_)
