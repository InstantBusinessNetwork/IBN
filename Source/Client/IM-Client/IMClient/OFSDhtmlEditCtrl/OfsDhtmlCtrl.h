// OfsDhtmlCtrl.h: interface for the COfsDhtmlCtrl class.
//
//////////////////////////////////////////////////////////////////////
#include <docobj.h>
#include <ocidl.h>
#include "site.h"

#if !defined(AFX_OFSDHTMLCTRL_H__46A4D787_1F65_42E1_8FB7_E5D23041C376__INCLUDED_)
#define AFX_OFSDHTMLCTRL_H__46A4D787_1F65_42E1_8FB7_E5D23041C376__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\McButton.h"

/////////////////////////////////////////////////////////////////////////////
/// look triedcid.h for more help info 
/////////////////////////////////////////////////////////////////////////////

class COfsDhtmlEditCtrl : public CWnd, public IOleInPlaceFrame, public IOleCommandTarget  
{
public:
	COfsDhtmlEditCtrl();
	DECLARE_DYNCREATE(COfsDhtmlEditCtrl)
	virtual ~COfsDhtmlEditCtrl();

public:
	BOOL IsFilterIn(){ return m_bFilterEnable;}
	BOOL PreTranslateMessage(MSG* pMsg);
	void SetOptions();
	
private:
	CComBSTR GetAbsoluteFontSizeName(int value);

	void UpdateMenu(CWnd *pWnd, CMenu *pMenu);
	int MenuItem;
	CMenu *pContextMenu;
	CWnd *pNotifyParent;
	HRESULT HrExecDefault(ULONG ucmdID,VARIANT* pVarIn);
	HRESULT HrExecCommand( ULONG ucmdID, VARIANT* pVarIn, VARIANT* pVarOut, BOOL bPromptUser );
	HRESULT HrGetBody(IHTMLBodyElement** ppBody);
	HRESULT HrGetDoc(IHTMLDocument2 **ppDoc);
	HRESULT HrGetElementFromSelection(IHTMLElement **ppElement);
	HRESULT HrGetRangeFromSelection(IHTMLTxtRange **ppRange);
	HRESULT HrGetTags();

	void SetFontState();
	DWORD GetCommandStatus(ULONG ucmdID);

	BOOL  m_bFilterEnable;
	IOleInPlaceActiveObject *m_pIOleIPActiveObject;
	HWND			m_hWndObj;			// The object's window
	BSTR			m_tags[32];
	ULONG			m_tagCount;
	CComBSTR		m_bsDefaultFontName;
	long			m_lDefaultFontSize;
private:
    ULONG m_cRef;
public:
	BOOL SetDefaultFontSize(long FontSize);
	BOOL SetDefaultFontName(BSTR bsFontName);
	long GetDefaultFontSize();
	BSTR GetDefaultFontName();
	long GetDefaultFontSizeIndex();

	void UpdateCmdControl(CMcButton *pBtn, ULONG ucmdID, BOOL bCheck);
	HRESULT GetTXT(BSTR *pStr);
	static HRESULT RegSaveString(LPCTSTR strPath, LPCTSTR strKey, LPCTSTR strData);
	static HRESULT RegSaveBin(LPCTSTR strPath, LPCTSTR strKey, const BYTE* Data, LONG DataSize);
	BOOL InsertTEXT(BSTR strText);
	COLORREF GetBgColor();
	COLORREF GetColor();
	HWND GetWindow();
	CString GetFontName();
	void SetFontName(BSTR strFontName);
	BOOL SetBodyMargin(int top, int right, int down, int left);
	BOOL InsertHTML(BSTR strHTML);
	BOOL IsViewMode();
	BOOL IsEditMode();
	BOOL EnableClipboardCut();
	BOOL EnableClipboardDelete();
	BOOL EnableClipboardPast();
	BOOL EnableClipboardCopy();
	void ClipboardCut();
	void ClipboardDelete();
	void ClipboardPast();
	void ClipboardCopy();
    HRESULT ShowContextMenu(DWORD dwID, POINT * pptPosition, IUnknown* pCommandTarget, IDispatch * pDispatchObjectHit);  	
	BOOL SetContextMenu(UINT resMenuId, int Item, CWnd *pNotifyParent);
	int GetTextSize();
	BOOL GetUnderline();
	BOOL GetItalick();
	BOOL GetBold();
	void UpdateCmdControl(CCmdUI* pCmdUI, ULONG ucmdID, BOOL bCheck = FALSE);
	void InitInfoMessage(DWORD dwInfoMessage = 0L);
	void SetUnLink();
	void ShowDetails(BOOL bShow = TRUE);
	void SetLeftJustify();
	void SetCenterJustify();
	void SetRightJustify();
	void Clear();
	void SetUnderline();
	void SetItalic();
	void SetBold();
	HRESULT SetHTML(BSTR Str);
	HRESULT GetHTML(BSTR* pStr);
	void SetViewMode(BOOL bViewMode = TRUE);
	void SetEditMode(BOOL bEditMode = TRUE);
	void SetTextSize(int FontSize);
	void SetColor(COLORREF color);
	void SetBgColor(COLORREF color);
    class CSite*	m_pSite;            //Site holding object        
	
	//Shared IUnknown implementation
	STDMETHODIMP         QueryInterface(REFIID, void **);
	STDMETHODIMP_(ULONG) AddRef(void);
	STDMETHODIMP_(ULONG) Release(void);
	
	//IOleInPlaceFrame implementation
	STDMETHODIMP         GetWindow(HWND *);
	STDMETHODIMP         ContextSensitiveHelp(BOOL);
	STDMETHODIMP         GetBorder(LPRECT);
	STDMETHODIMP         RequestBorderSpace(LPCBORDERWIDTHS);
	STDMETHODIMP         SetBorderSpace(LPCBORDERWIDTHS);
	STDMETHODIMP         SetActiveObject(LPOLEINPLACEACTIVEOBJECT
		, LPCOLESTR);
	STDMETHODIMP         InsertMenus(HMENU, LPOLEMENUGROUPWIDTHS);
	STDMETHODIMP         SetMenu(HMENU, HOLEMENU, HWND);
	STDMETHODIMP         RemoveMenus(HMENU);
	STDMETHODIMP         SetStatusText(LPCOLESTR);
	STDMETHODIMP         EnableModeless(BOOL);
	STDMETHODIMP         TranslateAccelerator(LPMSG, WORD);
	
	//IOleCommandTarget
	STDMETHODIMP QueryStatus(const GUID *pguidCmdGroup, ULONG cCmds
		, OLECMD prgCmds[], OLECMDTEXT *pCmdText);
	
	STDMETHODIMP Exec(const GUID *pguidCmdGroup, DWORD nCmdID
		, DWORD nCmdexecopt, VARIANTARG *pvaIn, VARIANTARG *pvaOut);
protected:
	
	DWORD dwInfoMessage;
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg void OnSetFocus(CWnd* pOldWnd);
	afx_msg BOOL OnEraseBkgnd(CDC* pDC);
	afx_msg int OnCreate( LPCREATESTRUCT lpCreateStruct );
	
    DECLARE_MESSAGE_MAP()
};

#endif // !defined(AFX_OFSDHTMLCTRL_H__46A4D787_1F65_42E1_8FB7_E5D23041C376__INCLUDED_)
