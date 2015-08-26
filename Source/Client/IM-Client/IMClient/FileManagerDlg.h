//{{AFX_INCLUDES()
#include "mcbutton.h"
//}}AFX_INCLUDES
#if !defined(AFX_FILEMANAGERDLG_H__6665B388_2B9E_4BC6_87BE_0EC2727C64F8__INCLUDED_)
#define AFX_FILEMANAGERDLG_H__6665B388_2B9E_4BC6_87BE_0EC2727C64F8__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// FileManagerDlg.h : header file
//

#include "OfsNCDlg2.h"
#include "Label.h"
#include "ResizableImage.h"
#include "McButton.h"

#include "FileDownloadDlg.h"
#include "FileUploadDlg.h"
#include "FileExplore.h"
#include "Label.h"

class CMainDlg;
/////////////////////////////////////////////////////////////////////////////
// CFileManagerDlg dialog

class CFileManagerDlg : public COFSNcDlg2
{
// Construction
public:
	void AddToUpload3(LPCTSTR XML);
	void SetUserDocumetFolder(LPCTSTR strPath);
	void DeleteAllItem();
	void AddToUpload2(CString FileName, CString Login, CString RecepientID, LPCTSTR strDescription);
	void ShowDialog(BOOL bAfterDownload = FALSE);
	void RefreshSenderDetails(CUser &User);
	void AddToUpLoad(CString FileName, CString Login,long RecepientID, LPCTSTR strDescription);
	void AddToDownload(CUser &Sender,IFile *pFile);
	CFileManagerDlg(CMainDlg *pMessenger,CWnd* pParent = NULL);   // standard constructor
	~CFileManagerDlg();
// Dialog Data
	//{{AFX_DATA(CFileManagerDlg)
	enum { IDD = IDD_FILEMANAGER_DIALOG };
	CMcButton	m_btnX;
	CMcButton	m_btnMin;
	CMcButton	m_btnMax;
	CMcButton	m_btnRestore;
	CMcButton	m_btnMenu;
	CMcButton	m_btnLibrary;
	CMcButton	m_btnReceived;
	CMcButton	m_btnSent;
	CMcButton	m_DownShowOffline;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CFileManagerDlg)
	protected:
		virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
		//}}AFX_VIRTUAL
		
		// Implementation
	protected:
		COLORREF m_crSplitter;
		void LoadSkin(IXMLDOMNode *pXmlRoot);
		CMainDlg*		m_pMessenger;
//		CResizableImage		m_ResizeFon;
		CFileDownloadDlg	m_FileDownlaodDlg;
		CFileUploadDlg		m_FileUploadDlg;
		CFileExplore		m_FileExplore;
		CWnd				*pFrgWindow;
		
		// Generated message map functions
		//{{AFX_MSG(CFileManagerDlg)
		virtual void OnOK();
		virtual void OnCancel();
		virtual BOOL OnInitDialog();
		afx_msg void OnTimer(UINT nIDEvent);
		afx_msg void OnClose();
	afx_msg void OnCaptureChanged(CWnd *pWnd);
	afx_msg void OnDestroy();
	afx_msg void OnClickBtnLibrary();
	afx_msg void OnClickBtnReceived();
	afx_msg void OnClickBtnSent();
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	DECLARE_EVENTSINK_MAP()
	//}}AFX_MSG
	void OnClickMcclose();
	void OnClickMcmini();
	void OnClickMcmenu();
	void OnClickMcSendDelete();
	void OnClickMcSendUpload();
	void OnClickMcSendCancel();
	void OnClickMcDownOffline();
	void OnClickMcDownDelete();
	void OnClickMcDownRemlater();
	void OnClickMcDownDownload();
	void OnClickMcDownCancel();
	void OnClickMcDownShowOffline();
	void OnClickMcmaxi();
	void OnClickMcmaximini();
		
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_FILEMANAGERDLG_H__6665B388_2B9E_4BC6_87BE_0EC2727C64F8__INCLUDED_)

