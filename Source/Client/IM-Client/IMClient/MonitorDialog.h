// MonitorDialog.h: interface for the CMonitorDialog class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_MONITORDIALOG_H__8083FDCC_ADFE_44A0_9E57_34D7C7094D78__INCLUDED_)
#define AFX_MONITORDIALOG_H__8083FDCC_ADFE_44A0_9E57_34D7C7094D78__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "Resource.h"
#include "ResizableImage.h"
#include "ResizableDialog.h"
#include "OFSNcDlg2.h"

#include "MonitorGraph.h"

#define WEB_WINDOW_PARENT COFSNcDlg2

class CMainDlg;

class CMonitorDialog  : public WEB_WINDOW_PARENT
{
public:
	CMonitorDialog(CMainDlg *pMessenger,CWnd* pParent = NULL);
	virtual ~CMonitorDialog();

// Dialog Data
	//{{AFX_DATA(CDlgAbout)
	enum { IDD = IDD_MONITOR_DIALOG };
	CMcButton	m_btnX;
	CMcButton	m_btnMenu;
	CMcButton	m_btnMini;
	CMcButton	m_btnUpdate;
	CMcButton	m_btnLineTotal;
	CMcButton	m_btnLineSent;
	CMcButton	m_btnLineReceived;
	CMonitorGraph	m_graph;

	CLabel		m_lbTotalSent;
	CLabel		m_lbTotalReceived;
	CLabel		m_lbMessageSent;
	CLabel		m_lbMessageReceived;
	CLabel		m_lbFileSent;
	CLabel		m_lbFileReceived;
	CLabel		m_lbTotalCost;
	
	CComboBox	m_comboCountType;
	CEdit		m_editCost;
	float		m_Cost;
	//}}AFX_DATA
	
	// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CMonitorDialog)
protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL
private:
	CComPtr<IMonitor>	m_pMonitor;

	long		m_lTotalSent;
	long		m_lTotalReceived;
	long		m_lMessageSent;
	long		m_lMessageReceived;
	long		m_lFileSent;
	long		m_lFileReceived;

	BOOL		bIsKillWinodow;

	CMainDlg	*pMessenger;
	

protected:
	void	LoadSkin(IXMLDOMNode *pXmlRoot);
	void	KillWindow();
	float	RecaluclateTotalCost();
	
	// Generated message map functions
	//{{AFX_MSG(CMonitorDialog)
	virtual BOOL OnInitDialog();
	afx_msg void OnClickBtnX();
	afx_msg void OnClickBtnMini();
	afx_msg void OnClickBtnMenu();
	afx_msg void OnClickBtnUpdate();
	afx_msg void OnPaint();
	afx_msg void OnTimer(UINT nIDEvent);
	afx_msg void OnCountTypeComboSelChange();
	afx_msg void OnClickBtnLineTotal();
	afx_msg void OnClickBtnLineSent();
	afx_msg void OnClickBtnLineReceived();
	virtual void OnOK();
	virtual void OnCancel();
	DECLARE_EVENTSINK_MAP()
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

#endif // !defined(AFX_MONITORDIALOG_H__8083FDCC_ADFE_44A0_9E57_34D7C7094D78__INCLUDED_)
