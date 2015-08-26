#pragma once
#include "afxwin.h"


// CTextCommentSettings dialog

class CTextCommentSettings : public CDialog
{
	DECLARE_DYNAMIC(CTextCommentSettings)

public:
	CTextCommentSettings(CWnd* pParent = NULL);   // standard constructor
	virtual ~CTextCommentSettings();

// Dialog Data
	enum { IDD = IDD_DIALOG_COMMENTSETTINGS };

private:
	CBitmap* m_bmpIcon;
	CString m_txtComment;

public:
	CFont		m_Font;
	COLORREF	m_fontColor;
	int			m_fontSize;
	CString		m_fontName;

	CString GetCommentText();
	void SetCommentText(CString Text);
	void SetFont(CFont& Font);
	void SetColor(COLORREF fontColor);

protected:
	virtual BOOL OnInitDialog();
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()

	afx_msg void OnDestroy();
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	afx_msg void OnBnClickedOk();
	afx_msg void OnBnClickedCancel();
	CEdit m_editComment;
	afx_msg void OnBnClickedBtnfont();
	afx_msg void OnEnChangeEdit1();
	afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);
};
