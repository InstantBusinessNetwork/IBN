//{{AFX_INCLUDES()
#include "mcbutton.h"
#include "ccootree.h"
//}}AFX_INCLUDES
#if !defined(AFX_CHATCREATEDLG_H__42835901_5470_407F_A3A1_47225C3E67D2__INCLUDED_)
#define AFX_CHATCREATEDLG_H__42835901_5470_407F_A3A1_47225C3E67D2__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// ChatCreateDlg.h : header file
//

#include "OFSNcDlg2.h"
#include "UserCollection.h"

class CMainDlg;
/////////////////////////////////////////////////////////////////////////////
// CChatCreateDlg dialog

class CChatCreateDlg : public COFSNcDlg2
{
// Construction
public:
	enum ChatCreateDlgMode
	{
		CCDM_CREATE	=	0, // default
		CCDM_UPDATE	=	1,
		CCDM_INVITE	=	2,
		CCDM_DETAIL	=	3,
	};
public:
	void AutoCreate();
	BOOL Create(CChatCreateDlg::ChatCreateDlgMode DlgMode = CCDM_CREATE, LPCTSTR strName = NULL, LPCTSTR strDescription = NULL,LPCTSTR strInvite = NULL,	CUserCollection *pUsers = NULL, BSTR bsChatId = NULL);
	//int DoModal(BOOL bCreate);
	CChatCreateDlg(CMainDlg *pMsgParent,CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CChatCreateDlg)
	enum { IDD = IDD_CHAT_CREATE };
	CEdit	m_edInvit;
	CEdit	m_edDescr;
	CEdit	m_edName;
	CMcButton	m_btnCreate;
	CMcButton	m_btnX;
	CCCooTree	m_treebox;
	CMcButton	m_btnInvite;
	CMcButton	m_btnUpdate;
	//}}AFX_DATA
	
	
	// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CChatCreateDlg)
protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL
	
	// Implementation
protected:
	BOOL	EnableRecepients();
	CUser* FindUserInVisualContactList(long TID);
	void UpdateID(long UserId);
	void CreateTree();
	void UpdateGroupID(LPCTSTR strName);
	void UpdateGroupCheck();
	void BuildTree();
	void KillWindow();
	void UnBlock();
	void Block();
	void UpdateControls();
	//BOOL m_bCreate;
	void LoadSkin(IXMLDOMNode *pXmlRoot);
	
	// Generated message map functions
	//{{AFX_MSG(CChatCreateDlg)
	afx_msg void OnSelectCcootreectrl(long TID, BOOL bGroupe);
	afx_msg void OnMenuCcootreectrl(long TID, BOOL bGroupe);
	afx_msg void OnDoDropCcootreectrl(long TID, BOOL bGroupe, LPUNKNOWN pDataObject, long DropEffect, long LastKeyState);
	afx_msg void OnActionCcootreectrl(long TID, BOOL bGroupe);
	virtual void OnCancel();
	afx_msg void OnOk();
	afx_msg LRESULT OnNetEvent(WPARAM w,LPARAM l);
	afx_msg void OnClickBtnCreate();
	afx_msg void OnClickBtnX();
	virtual BOOL OnInitDialog();
	afx_msg void OnClickBtnInvite();
	afx_msg void OnClickBtnUpdate();
	DECLARE_EVENTSINK_MAP()
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
private:
	CMapStringToPtr	m_GroupTIDMap;
	CMapStringToPtr	m_UserCheckInGroup;
	
	CMainDlg*			m_pMessenger;
	ISessionPtr			m_pSession;
	ChatCreateDlgMode	m_DlgMode;
	LONG				m_lHandle;
	CUserCollection		m_UserList;
	BOOL				bIsKillWinodow;

	CComBSTR			m_bsChatId;
	
	CString				m_strConferenceName;
	
	
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_CHATCREATEDLG_H__42835901_5470_407F_A3A1_47225C3E67D2__INCLUDED_)
