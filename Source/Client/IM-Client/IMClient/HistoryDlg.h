//{{AFX_INCLUDES()
#include "ccootree.h"
#include "webbrowser2.h"
//}}AFX_INCLUDES
#if !defined(AFX_HISTORYDLG_H__241D0929_C03B_4F70_ADB1_35D4ACE99150__INCLUDED_)
#define AFX_HISTORYDLG_H__241D0929_C03B_4F70_ADB1_35D4ACE99150__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// HistoryDlg.h : header file
//
#include "OfsNCDlg2.h"
#include "Usercollection.h"
#include "ResizableImage.h"
#include "McButton.h"
#include "McProgress.h"
#include "Label.h"
#include "SaveDataBase.h"

class CMainDlg;
/////////////////////////////////////////////////////////////////////////////
// CHistoryDlg dialog

class CHistoryDlg : public COFSNcDlg2
{
// Construction
public:
	void ShowHistory();
	void CreateTree();
	void BuildContactList();
	void RefreshIfNowThisUser(long Id);
	void SetMessenger(CMainDlg *pMessenger);
	CUser m_FriendUser;
	BOOL Refresh();
	void ShowHistory(CUser &m_FriendUser);
	CHistoryDlg(CWnd* pParent = NULL);   // standard constructor
	
// Dialog Data
	//{{AFX_DATA(CHistoryDlg)
	enum { IDD = IDD_DIALOG_HISORY };
	CLabel	m_ctrlUser;
	BOOL	m_LastFirst;
	long    m_listType; //1,2,3
	CMcButton	m_btnX;
	CMcButton	m_btnMin;
	CMcButton	m_btnMax;
	CMcButton	m_btnRestore;
	CMcButton	m_btnMenu;
	CMcButton	m_btnDialog;
	CMcButton	m_btnIncoming;
	CMcButton	m_btnOutgoing;
	CMcButton	m_btnFind;
	CCCooTree	m_treebox;
	CMcProgress	m_Progress;
	CWebBrowser2	m_History;
	// Time Filter Addon [5/20/2004]
	CComboBox	m_TimeFilterMode;
	CTime		m_FromTime;
	CTime		m_ToTime;
	//}}AFX_DATA
	

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CHistoryDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	int UpdateTimerInterval();
	void InitMpaWebEvent();
	void CloseMpaWebEvent();
	
	BOOL m_bEnableNavigate;
	void LoadSkin(IXMLDOMNode *pXmlRoot);
	ISessionPtr	pSession;

	HRESULT OnSaveToLHComplete(WPARAM w, LPARAM l);
	void SaveMessagesToDataBase(LONG Handle,IMessages *pMessagesList);
	BOOL LoadNextSID();
	void CheckProgress();
	void LocalSIDUpdate(IlocalSIDs *pSid);
	void SinchronizateHistory(long From, long To);
	void OnDoDropTreectrl(long TID, BOOL bGroupe, LPUNKNOWN pDataObject, long DropEffect, long LastKeyState);
	CUser* FindUserInVisualContactList(long TID);

    CMainDlg				*pMessenger;
	CResizableImage			m_ResizeFon;
	CUserCollection			m_ContactList;
	long					m_SynchronizeteHandle;
	CSaveDataBase			*pSaverDB;
	CArray <bstr_t,bstr_t>	m_LoadedSid;
	IMpaWebCustomizerPtr	m_pWebCustomizer;
	DWORD					m_dwSessionCookie;
	
	// Generated message map functions
	//{{AFX_MSG(CHistoryDlg)
	virtual void OnOK();
	virtual void OnCancel();
	afx_msg void OnLastfirst();
	afx_msg void OnDialog();
	afx_msg void OnIncoming();
	virtual BOOL OnInitDialog();
	afx_msg void OnCaptureChanged(CWnd *pWnd);
	afx_msg void OnOutgoing();
	afx_msg BOOL OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message);
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnClickMcclose();
	afx_msg void OnClickMcmini();
	afx_msg void OnClickMcmaxi();
	afx_msg void OnClickMcmaximini();
	afx_msg void OnClickMcmenu();
	afx_msg void OnClickIncoming();
	afx_msg void OnClickOutgoing();
	afx_msg void OnClickDialog();
	afx_msg void OnMenuCcootreectrl(long TID, BOOL bGroupe);
	afx_msg void OnSelectCcootreectrl(long TID, BOOL bGroupe);
	afx_msg void OnActionCcootreectrl(long TID, BOOL bGroupe);
	afx_msg void OnHistorySynhronizehistory();
	afx_msg void OnUpdateHistorySynhronizehistory(CCmdUI* pCmdUI);
	afx_msg void OnHistoryPreferences();
	afx_msg void OnPaint();
	afx_msg void OnBeforeNavigate2(LPDISPATCH pDisp, VARIANT FAR* URL, VARIANT FAR* Flags, VARIANT FAR* TargetFrameName, VARIANT FAR* PostData, VARIANT FAR* Headers, BOOL FAR* Cancel);
	afx_msg void OnDestroy();
	afx_msg void OnHistoryFind();
	afx_msg void OnSelendokTimeFilterMode();
	afx_msg void OnDateTimeChangeFrom (NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnDateTimeChangeTo(NMHDR* pNMHDR, LRESULT* pResult);
	DECLARE_EVENTSINK_MAP()
	//}}AFX_MSG
	afx_msg void OnShowContextMenu(long dwID, long x, long y, IUnknown *pcmdTarget, IDispatch *pdispReserved, long* pShow);
	DECLARE_INTERFACE_MAP()
	DECLARE_DISPATCH_MAP()	
	
	LRESULT OnNetEvent(WPARAM w,LPARAM l);
	
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_HISTORYDLG_H__241D0929_C03B_4F70_ADB1_35D4ACE99150__INCLUDED_)
