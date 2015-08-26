#if !defined(AFX_FILEEXPLORE_H__EB58A900_C47E_4699_AB77_6AF4E0F34DEF__INCLUDED_)
#define AFX_FILEEXPLORE_H__EB58A900_C47E_4699_AB77_6AF4E0F34DEF__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// FileExplore.h : header file
//
#include "FolderChangeNotfication.h"

/////////////////////////////////////////////////////////////////////////////
// CFileExplore dialog
#include "ResizableDialog.h"
#include "SortHeaderCtrl.h"

class CFileExplore : public CResizableDialog
{
// Construction
public:
	void ShowDetails(BOOL bUseFile	=	FALSE);
	void Delete(BOOL bUseFile	=	FALSE);
	void Refresh();
	COLORREF GetBkColor();
	void SetBkColor(COLORREF RgbColor);
	
	LPCTSTR GetStartFolder();
	void SetStartFolder(LPCTSTR strPath);
	LPCTSTR GetDefaultPath();
	void SetDefaultPath(LPCTSTR strPath);
	CFileExplore(CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CFileExplore)
	enum { IDD = IDD_FILES_EXPLORER };
	CStatic	m_VSplitter;
	CListCtrl	m_FileList;
	CTreeCtrl	m_DirTree;
	//}}AFX_DATA
public:

	class CFileItem
	{
	public:
		CString			strFileName;	
		DWORD			dwFileSize;	
		CString			strFileType;
		ULARGE_INTEGER 	ftData;
	};

	class COleFileListDropSource : public COleDropSource
	{
	public:
		COleFileListDropSource(CFileExplore* pFE = NULL):m_pFE(pFE)
		{
		};
		// Overrides
		SCODE GiveFeedback(DROPEFFECT dropEffect);
	private:
		CFileExplore* m_pFE;
	};

	class COleFEDropTarget : public COleDropTarget
	{
	public:
		COleFEDropTarget(CFileExplore* pFE = NULL):m_pFE(pFE)
		{
			m_dropEffectCurrent = DROPEFFECT_NONE;
		};
		// Overrides
		DROPEFFECT OnDragEnter( CWnd* pWnd, COleDataObject* pDataObject, DWORD dwKeyState, CPoint point );
		DROPEFFECT OnDragOver(CWnd* pWnd, COleDataObject* pDataObject, DWORD dwKeyState, CPoint point );
		void OnDragLeave(CWnd* pWnd);               
		BOOL OnDrop( CWnd* pWnd, COleDataObject* pDataObject, DROPEFFECT dropEffect, CPoint point );
	private:
		DROPEFFECT m_dropEffectCurrent;
		CFileExplore* m_pFE;
	};
	
	friend COleFileListDropSource;
	friend COleFEDropTarget;
// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CFileExplore)
	public:
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	void FileListDeleteAllItem();
	static int CALLBACK CompareListItem(LPARAM lParam1, LPARAM lParam2, LPARAM lParamSort);
	void SortList(int isorMode);
	BOOL VSplitterHitTest(CPoint &point);
	BOOL EndVSplitterMove(BOOL Move, CPoint &point);
	BOOL VSplitterMove(CPoint &newPoint);
	BOOL IsVSplitterMove();
	BOOL BeginVSplitterMove(CPoint& point);
	
	HTREEITEM CreateTreeItem(HTREEITEM hParentItem, LPCTSTR Name);
	BOOL LoadFilesByPath(LPCTSTR Path);
	int LoadTreeByPath(HTREEITEM hItem, LPCTSTR Path, BOOL bSubRoot = FALSE);

	// Generated message map functions
	//{{AFX_MSG(CFileExplore)
	virtual void OnOK();
	virtual void OnCancel();
	virtual BOOL OnInitDialog();
	virtual LRESULT WindowProc(UINT message,WPARAM wParam,LPARAM lParam);
	afx_msg void OnItemexpandedDirectoryTree(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnSelchangedDirectoryTree(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
	afx_msg BOOL OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message);
	afx_msg void OnCancelMode();
	afx_msg void OnMouseMove(UINT nFlags, CPoint point);
	afx_msg BOOL OnEraseBkgnd( CDC * pDc);
	afx_msg void OnDestroy();
	afx_msg void OnClose();
	afx_msg void OnRclickDirectoryTree(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnRclickFileList(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnDblclkFileList(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnEndlabeleditDirectoryTree(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnEndlabeleditFileList(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnBegindragFileList(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnItemclickFileList(NMHDR* pNMHDR, LRESULT* pResult);
	//}}AFX_MSG
	afx_msg LONG OnHandleDrag( UINT, LONG );
	afx_msg LRESULT OnRefreshPath(WPARAM, LPARAM);
	DECLARE_MESSAGE_MAP()
		
private:
	CSortHeaderCtrl	m_SortHeader;
	//CImageList	m_CheckHeader;
	int			m_iSortingMode;
	CString		m_strDefaultPath;
	CString		m_strStartFolder;
	COLORREF	m_dwBkColor;

	// VSlider Addon [3/20/2002]
	HTREEITEM	m_hRoot;
	BOOL		m_bSplitterMove;
	long		m_iMaxPosition;
	long		m_iMinPosition;
	CRect		m_rectLast;
	HCURSOR		m_hSplitter;

	// Drag&Drop Addon [3/22/2002]
	COleFileListDropSource	m_OleFileListDropSource;
	COleFEDropTarget  m_OleFileListDropTarget;
	COleFEDropTarget  m_OleDirTreeDropTarget;

	CImageList	*m_pDragImage;
	BOOL		m_bImageHidden;

	CComPtr<IContextMenu2>	m_pContextMenu2;
	
	CComPtr<IContextMenu3>	m_pContextMenu3;

	CFolderChangeNotification	m_FolderNotify;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_FILEEXPLORE_H__EB58A900_C47E_4699_AB77_6AF4E0F34DEF__INCLUDED_)
