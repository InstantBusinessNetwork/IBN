// FileExplore.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "FileExplore.h"
#include "GlobalFunction.h"


#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif


#define WM_HANDLE_DRAG	(WM_USER + 100)
#define WM_REFRESHPATH	(WM_USER + 101)

/////////////////////////////////////////////////////////////////////////////
// CFileExplore dialog


CFileExplore::CFileExplore(CWnd* pParent /*=NULL*/)
	: CResizableDialog(CFileExplore::IDD, pParent),m_OleFileListDropSource(this),m_OleDirTreeDropTarget(this),m_OleFileListDropTarget(this)
{
	//{{AFX_DATA_INIT(CFileExplore)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
	m_strDefaultPath	=	_T("");
	m_strStartFolder	=	_T("");
	m_hRoot				=	NULL;
	m_bSplitterMove		=	FALSE;
	m_dwBkColor			=	GetSysColor(COLOR_MENU);

	m_pDragImage		=	NULL;
	m_bImageHidden		=	TRUE;
}


void CFileExplore::DoDataExchange(CDataExchange* pDX)
{
	CResizableDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CFileExplore)
	DDX_Control(pDX, IDC_VSPLITTER, m_VSplitter);
	DDX_Control(pDX, IDC_FILE_LIST, m_FileList);
	DDX_Control(pDX, IDC_DIRECTORY_TREE, m_DirTree);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CFileExplore, CResizableDialog)
//{{AFX_MSG_MAP(CFileExplore)
	ON_NOTIFY(TVN_ITEMEXPANDED, IDC_DIRECTORY_TREE, OnItemexpandedDirectoryTree)
	ON_NOTIFY(TVN_SELCHANGED, IDC_DIRECTORY_TREE, OnSelchangedDirectoryTree)
	ON_WM_LBUTTONDOWN()
	ON_WM_LBUTTONUP()
	ON_WM_SETCURSOR()
	ON_WM_CANCELMODE()
	ON_WM_MOUSEMOVE()
	ON_WM_ERASEBKGND()
	ON_WM_DESTROY()
	ON_WM_CLOSE()
	ON_NOTIFY(NM_RCLICK, IDC_DIRECTORY_TREE, OnRclickDirectoryTree)
	ON_NOTIFY(NM_RCLICK, IDC_FILE_LIST, OnRclickFileList)
	ON_NOTIFY(NM_DBLCLK, IDC_FILE_LIST, OnDblclkFileList)
	ON_NOTIFY(TVN_ENDLABELEDIT, IDC_DIRECTORY_TREE, OnEndlabeleditDirectoryTree)
	ON_NOTIFY(LVN_ENDLABELEDIT, IDC_FILE_LIST, OnEndlabeleditFileList)
	ON_NOTIFY(LVN_BEGINDRAG, IDC_FILE_LIST, OnBegindragFileList)
	ON_NOTIFY(HDN_ITEMCLICK, 0, OnItemclickFileList)
	//}}AFX_MSG_MAP
	ON_MESSAGE(WM_HANDLE_DRAG, OnHandleDrag)
	ON_MESSAGE(WM_REFRESHPATH,OnRefreshPath)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CFileExplore message handlers

void CFileExplore::OnOK() 
{
}

void CFileExplore::OnCancel() 
{
}

BOOL CFileExplore::OnInitDialog() 
{
	CResizableDialog::OnInitDialog();

	//m_CheckHeader.Create(IDB_TRAYSTATUS_BITMAP,16,0,0xFF00FF);
	//m_FileList.GetHeaderCtrl()->SetImageList(&m_CheckHeader);
	m_SortHeader.SubclassWindow(m_FileList.GetHeaderCtrl()->GetSafeHwnd());

	m_iSortingMode	= GetOptionInt(IDS_OFSMESSENGER,IDS_FILEL,1);

	m_hSplitter = AfxGetApp()->LoadCursor(IDC_SPLITH);

	ShowSizeGrip(FALSE);
	
	//m_FileList.SetExtendedStyle (m_FileList.GetExtendedStyle ()|LVS_EX_FULLROWSELECT);

	m_FileList.SetImageList(CImageList::FromHandle(GetSystemImageList(TRUE)),LVSIL_SMALL);

	CString strSection = GetString(IDS_OFSMESSENGER);
	CString strEntry = GetString(IDS_COLUMN_WIDTH);

	m_FileList.InsertColumn(0,GetString(IDS_FILENAME_NAME),LVCFMT_LEFT,AfxGetApp()->GetProfileInt(strSection, strEntry+_T("30"), 150));
	m_FileList.InsertColumn(1,GetString(IDS_FILESIZE_NAME),LVCFMT_RIGHT,AfxGetApp()->GetProfileInt(strSection, strEntry+_T("33"), 80),3);
	m_FileList.InsertColumn(2,GetString(IDS_FILETYPE_NAME),LVCFMT_LEFT,AfxGetApp()->GetProfileInt(strSection, strEntry+_T("32"), 130),2);
	m_FileList.InsertColumn(3,GetString(IDS_FILEMODIFIED_NAME),LVCFMT_LEFT,AfxGetApp()->GetProfileInt(strSection, strEntry+_T("31"), 125),1);
	

	m_DirTree.SetImageList(CImageList::FromHandle(GetSystemImageList(TRUE)),TVSIL_NORMAL);

	//m_hRoot	=	CreateTreeItem(TVI_ROOT,GetString(IDS_MYDOCUMENTS));
	//LoadTreeByPath(m_hRoot,m_strStartFolder);
	//m_DirTree.Expand(m_hRoot,TVE_EXPAND);

/*	CString	strDefaultPath = GetDefaultPath();
	CString strPathAddon	=	GetStartFolder();
	while(!strDefaultPath.IsEmpty())
	{
		int Pos = strDefaultPath.Find(_T('\\'));
		if(Pos==-1)
		{
			strPathAddon += _T('\\')+strDefaultPath;
		}
		else
		{
			strPathAddon += _T('\\')+strDefaultPath.Left(Pos);
			strDefaultPath = strDefaultPath.Mid(Pos+1);
		}
		LoadTreeByPath(m_hRoot,m_strStartFolder);
	}*/

	AddAnchor(&m_DirTree,CSize(0,0),CSize(0,100));
	AddAnchor(&m_VSplitter,CSize(0,0),CSize(0,100));
	AddAnchor(&m_FileList,CSize(0,0),CSize(100,100));

	HANDLE hHanle1 = GetProp(m_FileList.GetSafeHwnd(),(LPCTSTR)(DWORD)0xC01C);
	
	m_OleFileListDropTarget.Register(&m_FileList);
	m_OleDirTreeDropTarget.Register(&m_DirTree);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CFileExplore::SetDefaultPath(LPCTSTR strPath)
{
	m_strDefaultPath	=	strPath;
}

LPCTSTR CFileExplore::GetDefaultPath()
{
	return m_strDefaultPath;
}

void CFileExplore::SetStartFolder(LPCTSTR strPath)
{
	if(m_strStartFolder.CompareNoCase(strPath))
	{
		m_strStartFolder = strPath;

		if(IsWindow(GetSafeHwnd()))
		{
			m_DirTree.DeleteAllItems();
			//m_FileList.DeleteAllItems();
			FileListDeleteAllItem();
			m_hRoot	=	CreateTreeItem(TVI_ROOT,GetString(IDS_MYDOCUMENTS));
			LoadTreeByPath(m_hRoot,m_strStartFolder);
			m_DirTree.Expand(m_hRoot,TVE_EXPAND);
			m_DirTree.SelectItem(m_hRoot);
			
			m_FolderNotify.Stop();
			m_FolderNotify.DeleteAllPath();
			m_FolderNotify.AddPath(strPath,TRUE,FILE_NOTIFY_CHANGE_FILE_NAME|FILE_NOTIFY_CHANGE_DIR_NAME|FILE_NOTIFY_CHANGE_SIZE);
			m_FolderNotify.Start(GetSafeHwnd(),WM_REFRESHPATH);
		}
	}
}

LPCTSTR CFileExplore::GetStartFolder()
{
	return m_strStartFolder;
}

BOOL CFileExplore::LoadFilesByPath(LPCTSTR Path)
{
	//m_FileList.DeleteAllItems();
	FileListDeleteAllItem();

	CString	strFilePath;
	strFilePath.Format(_T("%s\\*.*"),Path);
	WIN32_FIND_DATA FindData	=	{0};
	HANDLE hFile = FindFirstFile(strFilePath,&FindData);
	if(hFile==INVALID_HANDLE_VALUE)
		return FALSE;
	
	do 
	{
		if(!(FindData.dwFileAttributes&FILE_ATTRIBUTE_DIRECTORY))
		{
			int iIconIndex 	=	GetIconIndexInSystemImageList(TRUE,FindData.cFileName);

			SHFILEINFO	hFileInfo	=	{0};
			SHGetFileInfo(FindData.cFileName,FindData.dwFileAttributes,&hFileInfo,sizeof(hFileInfo),SHGFI_TYPENAME|SHGFI_USEFILEATTRIBUTES);

			SYSTEMTIME	sysFileTime		=	{0};
			TCHAR	szDate[MAX_PATH]=_T(""), szTime[MAX_PATH]=_T("");
			FileTimeToSystemTime(&FindData.ftLastWriteTime,&sysFileTime);
			GetDateFormat(LOCALE_USER_DEFAULT,DATE_SHORTDATE,&sysFileTime,NULL,szDate,MAX_PATH);
			GetTimeFormat(LOCALE_USER_DEFAULT,NULL,&sysFileTime,NULL,szTime,MAX_PATH);
			
			CString	strDataFormat;
			strDataFormat.Format(_T("%s %s"),szDate,szTime);

			CFileItem	*pNewFileItem	=	new CFileItem;

			pNewFileItem->strFileName	=	FindData.cFileName;
			pNewFileItem->dwFileSize	=	FindData.nFileSizeLow;
			memcpy(&pNewFileItem->ftData,&FindData.ftLastWriteTime,sizeof(ULARGE_INTEGER));
			pNewFileItem->strFileType	=	hFileInfo.szTypeName;

			int iSubIndex = m_FileList.InsertItem(0,pNewFileItem->strFileName,iIconIndex);
			
			m_FileList.SetItemText(iSubIndex ,1,ByteSizeToStr(pNewFileItem->dwFileSize));
			m_FileList.SetItemText(iSubIndex ,2,pNewFileItem->strFileType);
			m_FileList.SetItemText(iSubIndex ,3,strDataFormat);

			m_FileList.SetItemData(iSubIndex,DWORD(pNewFileItem));
		}
	} 
	while(FindNextFile(hFile,&FindData));
	FindClose(hFile);

	SortList(m_iSortingMode);

	return TRUE;
}

int CFileExplore::LoadTreeByPath(HTREEITEM hItem, LPCTSTR Path, BOOL bSubRoot)
{
	int RetValue = 0;

	CString	strFilePath;
	strFilePath.Format(_T("%s\\*.*"),Path);
	WIN32_FIND_DATA FindData	=	{0};
	HANDLE hFile = FindFirstFile(strFilePath,&FindData);
	if(hFile==INVALID_HANDLE_VALUE)
		return RetValue;
	
	if(m_DirTree.ItemHasChildren(hItem))
	{
		HTREEITEM hNextItem;
		HTREEITEM hChildItem = m_DirTree.GetChildItem(hItem);
		
		while (hChildItem != NULL)
		{
			hNextItem = m_DirTree.GetNextItem(hChildItem, TVGN_NEXT);
			m_DirTree.DeleteItem(hChildItem);
			hChildItem = hNextItem;
		}
	}
	
	do 
	{
		if(FindData.dwFileAttributes&FILE_ATTRIBUTE_DIRECTORY&&
			_tcscmp(FindData.cFileName,_T("."))&&
			_tcscmp(FindData.cFileName,_T("..")))
		{
			RetValue++;
			HTREEITEM hNewItem = CreateTreeItem(hItem,FindData.cFileName);
			if(!bSubRoot)
			{
				CString strTmpPath;
				strTmpPath.Format(_T("%s\\%s"),Path,FindData.cFileName);
				LoadTreeByPath(hNewItem,strTmpPath,TRUE);
			}
			//CreateTreeItem(hNewItem,_T(""));
		}
	} 
	while(FindNextFile(hFile,&FindData));
	FindClose(hFile);

	return RetValue;
}

void CFileExplore::OnItemexpandedDirectoryTree(NMHDR* pNMHDR, LRESULT* pResult) 
{
	NM_TREEVIEW* pNMTreeView = (NM_TREEVIEW*)pNMHDR;
	
	if(pNMTreeView)
	{
		HTREEITEM hItem = pNMTreeView->itemNew.hItem, hTmpItem;
		if(hItem)
		{
			CString strItemPath;

			hTmpItem = hItem;
			
			while(hTmpItem!=m_hRoot)
			{
				strItemPath	= _T('\\') + m_DirTree.GetItemText(hTmpItem)+ strItemPath;
				hTmpItem = m_DirTree.GetParentItem(hTmpItem);
			}
			LoadTreeByPath(hItem,m_strStartFolder+strItemPath);
			//LoadFilesByPath(m_strStartFolder+strItemPath);
		}
	}
	
	*pResult = 0;
}

//DEL void CFileExplore::OnItemexpandingDirectoryTree(NMHDR* pNMHDR, LRESULT* pResult) 
//DEL {
//DEL 	TRACE("\r\n --- OnItemexpandingDirectoryTree ---");
//DEL 	NM_TREEVIEW* pNMTreeView = (NM_TREEVIEW*)pNMHDR;
//DEL 	// TODO: Add your control notification handler code here
//DEL 	
//DEL 	*pResult = 0;
//DEL }

//DEL void CFileExplore::OnGetdispinfoDirectoryTree(NMHDR* pNMHDR, LRESULT* pResult) 
//DEL {
//DEL 	TV_DISPINFO* pTVDispInfo = (TV_DISPINFO*)pNMHDR;
//DEL 	// TODO: Add your control notification handler code here
//DEL 	
//DEL 	*pResult = 0;
//DEL }

HTREEITEM CFileExplore::CreateTreeItem(HTREEITEM hParentItem, LPCTSTR Name)
{
	SHFILEINFO shfi	=	{0};
	SHGetFileInfo(Name, FILE_ATTRIBUTE_DIRECTORY,&shfi, sizeof(shfi),SHGFI_SYSICONINDEX|SHGFI_USEFILEATTRIBUTES|SHGFI_SMALLICON);
	int IconIndex  = shfi.iIcon;
	
	TVINSERTSTRUCT tvInsert			=	{0};
	tvInsert.hParent				=	hParentItem;
	tvInsert.hInsertAfter			=	TVI_SORT;
	tvInsert.item.mask				=	TVIF_TEXT|TVIF_IMAGE|TVIF_SELECTEDIMAGE;
	tvInsert.item.pszText			=	(LPTSTR)Name;
	tvInsert.item.iImage			=	IconIndex;
	tvInsert.item.iSelectedImage	=	IconIndex;
	
	return m_DirTree.InsertItem(&tvInsert);
}

void CFileExplore::OnSelchangedDirectoryTree(NMHDR* pNMHDR, LRESULT* pResult) 
{
	NM_TREEVIEW* pNMTreeView = (NM_TREEVIEW*)pNMHDR;
	
	if(pNMTreeView)
	{
		HTREEITEM hItem = pNMTreeView->itemNew.hItem, hTmpItem;
		if(hItem)
		{
			CString strItemPath;
			
			hTmpItem = hItem;
			
			while(hTmpItem!=m_hRoot)
			{
				strItemPath	= _T('\\') + m_DirTree.GetItemText(hTmpItem)+ strItemPath;
				hTmpItem = m_DirTree.GetParentItem(hTmpItem);
			}
			LoadFilesByPath(m_strStartFolder+strItemPath);
		}
	}
	
	*pResult = 0;
}

void CFileExplore::OnLButtonDown(UINT nFlags, CPoint point) 
{
	BeginVSplitterMove(point);
	
	CResizableDialog::OnLButtonDown(nFlags, point);
}

void CFileExplore::OnLButtonUp(UINT nFlags, CPoint point) 
{
	EndVSplitterMove(TRUE,point);

	CResizableDialog::OnLButtonUp(nFlags, point);
}

BOOL CFileExplore::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message) 
{
	CPoint	point, tmpPoint;
	GetCursorPos(&point);
	tmpPoint = point;
	ScreenToClient(&point);
	if(VSplitterHitTest(point)||IsVSplitterMove())
	{
		SetCursor(m_hSplitter);
		return TRUE;
	}
	
	return CResizableDialog::OnSetCursor(pWnd, nHitTest, message);
}

void CFileExplore::OnCancelMode() 
{
	CResizableDialog::OnCancelMode();
	
	EndVSplitterMove(FALSE,CPoint(0,0));
}

void CFileExplore::OnMouseMove(UINT nFlags, CPoint point) 
{
	VSplitterMove(point);

	CResizableDialog::OnMouseMove(nFlags, point);
}

BOOL CFileExplore::BeginVSplitterMove(CPoint &point)
{
	CRect	rectVSplitter, rectWindow;
	m_VSplitter.GetWindowRect(&rectVSplitter);
	ScreenToClient(&rectVSplitter);
	
	if(rectVSplitter.PtInRect(point))
	{
		SetCapture();
		m_bSplitterMove = TRUE;
		GetClientRect(&rectWindow);
		m_iMinPosition	= 10;
		m_iMaxPosition	= rectWindow.Width()-20;

		m_VSplitter.GetWindowRect(&rectVSplitter);
		m_rectLast = rectVSplitter;

		CWindowDC dc(GetDesktopWindow());
		dc.DrawDragRect(rectVSplitter,CSize(3,3),NULL,CSize(3,3));
		return TRUE;
	}
	return FALSE;
}

BOOL CFileExplore::IsVSplitterMove()
{
	return m_bSplitterMove;
}

BOOL CFileExplore::VSplitterMove(CPoint &newPoint)
{
	if(!IsVSplitterMove())
		return FALSE;
	
	CRect	rectVSplitter;
	m_VSplitter.GetWindowRect(&rectVSplitter);

	int XValue = newPoint.x;

	if(XValue<m_iMinPosition)
		XValue = m_iMinPosition;
	if(XValue>m_iMaxPosition)
		XValue = m_iMaxPosition;
	
	CWindowDC dc(GetDesktopWindow());
	dc.DrawDragRect(&m_rectLast,CSize(3,3),NULL,CSize(3,3));
	
	CPoint	ptScreen	=	newPoint;
	ptScreen.x = XValue;
	
	ClientToScreen(&ptScreen);
	rectVSplitter.OffsetRect(ptScreen.x - rectVSplitter.left,0);
	m_rectLast = rectVSplitter;
	
	dc.DrawDragRect(&m_rectLast,CSize(3,3),NULL,CSize(3,3));
	
	return TRUE;
}

BOOL CFileExplore::EndVSplitterMove(BOOL Move, CPoint &point)
{
	if(!IsVSplitterMove())
		return FALSE;
	
	m_bSplitterMove	=	FALSE;
	ReleaseCapture();

	CWindowDC dc(GetDesktopWindow());
	dc.DrawDragRect(&m_rectLast,CSize(3,3),NULL,CSize(3,3));
	
	if(Move)
	{
		CRect	rectVSplitter,rectDirTree, rectFileList;
		m_VSplitter.GetWindowRect(&rectVSplitter);

		int _CX = m_rectLast.left - rectVSplitter.left;

		ScreenToClient(&rectVSplitter);
		rectVSplitter.OffsetRect(_CX,0);

		m_DirTree.GetWindowRect(&rectDirTree);
		ScreenToClient(&rectDirTree);
		rectDirTree.right += _CX;

		m_FileList.GetWindowRect(&rectFileList);
		ScreenToClient(&rectFileList);
		rectFileList.left += _CX;

		m_VSplitter.MoveWindow(rectVSplitter);
		m_DirTree.MoveWindow(rectDirTree);
		m_FileList.MoveWindow(rectFileList);

		Invalidate(TRUE);
		UpdateWindow();

		m_DirTree.Invalidate();
		m_FileList.Invalidate();
		m_VSplitter.Invalidate();

		AddAnchor(&m_DirTree,CSize(0,0),CSize(0,100));
		AddAnchor(&m_VSplitter,CSize(0,0),CSize(0,100));
		AddAnchor(&m_FileList,CSize(0,0),CSize(100,100));
	}
	
	return TRUE;
}

BOOL CFileExplore::VSplitterHitTest(CPoint &point)
{
	CRect	rectVSplitter;
	m_VSplitter.GetWindowRect(&rectVSplitter);
	ScreenToClient(&rectVSplitter);
	if(rectVSplitter.PtInRect(point))
		return TRUE;
	return FALSE;
}

void CFileExplore::SetBkColor(COLORREF RgbColor)
{
	m_dwBkColor =	RgbColor;
	if(IsWindow(GetSafeHwnd()))
	{
		CPaintDC	dc(this);
		dc.SetBkColor(m_dwBkColor);
		Invalidate();
	}
}

COLORREF CFileExplore::GetBkColor()
{
	return m_dwBkColor;
}

BOOL CFileExplore::OnEraseBkgnd( CDC * pDc)
{
	CRect	rectWin;
	GetClientRect(&rectWin);
	pDc->FillSolidRect(&rectWin,GetBkColor());
	return FALSE;
}

void CFileExplore::OnDestroy() 
{
	CResizableDialog::OnDestroy();
}

void CFileExplore::OnClose() 
{
	FileListDeleteAllItem();

	CString strSection = GetString(IDS_OFSMESSENGER);
	CString strEntry = GetString(IDS_COLUMN_WIDTH);
	
	AfxGetApp()->WriteProfileInt(strSection, strEntry+_T("30"), m_FileList.GetColumnWidth(0));
	AfxGetApp()->WriteProfileInt(strSection, strEntry+_T("31"), m_FileList.GetColumnWidth(1));
	AfxGetApp()->WriteProfileInt(strSection, strEntry+_T("32"), m_FileList.GetColumnWidth(2));
	AfxGetApp()->WriteProfileInt(strSection, strEntry+_T("33"), m_FileList.GetColumnWidth(3));

	WriteOptionInt(IDS_OFSMESSENGER,IDS_FILEL,m_iSortingMode);
	
	CResizableDialog::OnClose();
}

BOOL CFileExplore::PreTranslateMessage(MSG* pMsg) 
{
	if(pMsg->message==WM_KEYDOWN)
	{
		switch(pMsg->wParam) 
		{
		case VK_F5:
			Refresh();
			break;
		case VK_DELETE:
			if(pMsg->hwnd==m_DirTree.GetSafeHwnd())
				Delete();
			else if(pMsg->hwnd==m_FileList.GetSafeHwnd())
					Delete(TRUE);
			break;
		case VK_F2:
			if(pMsg->hwnd==m_DirTree.GetSafeHwnd())
			{
				HTREEITEM hItem = m_DirTree.GetSelectedItem();
				if(hItem)
					m_DirTree.EditLabel(hItem);
			} 
			else if(pMsg->hwnd==m_FileList.GetSafeHwnd())
			{
				POSITION Pos = m_FileList.GetFirstSelectedItemPosition();
				if(Pos)
				{
					int iIndex = m_FileList.GetNextSelectedItem(Pos);
					m_FileList.EditLabel(iIndex);
				}
			}
			break;	
		case VK_ESCAPE:
			if(m_DirTree.GetEditControl()&&pMsg->hwnd==m_DirTree.GetEditControl()->GetSafeHwnd())
			{
				m_DirTree.SendMessage(TVM_ENDEDITLABELNOW,TRUE);
			}
			else if(m_FileList.GetEditControl() && pMsg->hwnd==m_FileList.GetEditControl()->GetSafeHwnd())
			{
				//m_FileList.SendMessage(LVM_CANCELEDITLABEL,0,0);
			}
			else
				((CDialog*)GetParent())->EndDialog(IDCANCEL);
			break;
		case VK_RETURN:
			if(m_DirTree.GetEditControl()&&pMsg->hwnd==m_DirTree.GetEditControl()->GetSafeHwnd())
			{
				m_DirTree.SendMessage(TVM_ENDEDITLABELNOW,FALSE);
			} 
			break;
		}
	}
	else if(pMsg->message==WM_SYSKEYDOWN)
	{
		switch(pMsg->wParam) 
		{
		case VK_RETURN:
			if((GetKeyState(VK_MENU)>>1))
			{
				if(pMsg->hwnd==m_FileList.GetSafeHwnd())
					ShowDetails(TRUE);
				else if(pMsg->hwnd==m_DirTree.GetSafeHwnd())
					ShowDetails();
			} 
			break;
		}
	}
	
	return CResizableDialog::PreTranslateMessage(pMsg);
}

void CFileExplore::Refresh()
{
	HTREEITEM hItem = m_DirTree.GetSelectedItem(), hTmpItem, hFileDirItem;
	if(hItem==NULL)
		hItem = m_hRoot;
	if(hItem)
	{
		CString strItemPath, strFileDir;

		hFileDirItem = hTmpItem = hItem;

		if(hItem != m_hRoot)
		{
			strFileDir = m_DirTree.GetItemText(hItem);
			hFileDirItem = hTmpItem = m_DirTree.GetParentItem(hItem);
		}
		
		while(hTmpItem!=m_hRoot)
		{
			strItemPath	= _T('\\') + m_DirTree.GetItemText(hTmpItem)+ strItemPath;
			hTmpItem = m_DirTree.GetParentItem(hTmpItem);
		}

		LoadTreeByPath(hFileDirItem,m_strStartFolder+strItemPath);
		if(!strFileDir.IsEmpty()&&m_DirTree.ItemHasChildren(hFileDirItem))
		{
			hTmpItem = m_DirTree.GetChildItem(hFileDirItem);
			while (hTmpItem != NULL)
			{
				if(strFileDir.CompareNoCase(m_DirTree.GetItemText(hTmpItem))==0)
				{
					hFileDirItem = hTmpItem;
					strItemPath += _T('\\') + strFileDir;
					break;
				}
				hTmpItem = m_DirTree.GetNextItem(hTmpItem, TVGN_NEXT);
			}
			LoadTreeByPath(hFileDirItem,m_strStartFolder+strItemPath);
		}
		
		m_DirTree.SelectItem(hFileDirItem);
		
		if(hFileDirItem==m_hRoot)
			LoadFilesByPath(m_strStartFolder+strItemPath);
	}
}

void CFileExplore::OnRclickDirectoryTree(NMHDR* pNMHDR, LRESULT* pResult) 
{
	CPoint	point, DirPoint;
	GetCursorPos(&point);
	
	DirPoint	=	point;
	m_DirTree.ScreenToClient(&DirPoint);
	
	HTREEITEM hItemHitTest =	m_DirTree.HitTest(DirPoint);
	if(hItemHitTest)
	{
		m_DirTree.SelectItem(hItemHitTest);
		
		HTREEITEM hItem =	m_DirTree.GetSelectedItem(), hTmpItem;
		
		if(hItem)
		{
			long TreeAddon = 1;
			if(m_DirTree.ItemHasChildren(hItem))
			{
				TreeAddon |= 2;
				if(m_DirTree.GetItemState(hItem,TVIS_EXPANDED)&TVIS_EXPANDED)
					TreeAddon |= 4;
			}
			
			CString	strDirPath;
			hTmpItem = hItem;
			while(hTmpItem!=m_hRoot)
			{
				strDirPath = _T("\\") + m_DirTree.GetItemText(hTmpItem)+strDirPath;
				hTmpItem =  m_DirTree.GetParentItem(hTmpItem);
			}
			
			int RetCom = ShowContextMenu(TreeAddon,GetSafeHwnd(),m_strStartFolder+strDirPath, point.x, point.y, NULL,&m_pContextMenu2, &m_pContextMenu3);
			
			switch(RetCom) 
			{
			case 19:
				m_DirTree.EditLabel(hItem);
				break;
			case 0x8001:
				m_DirTree.Expand(hItem,TreeAddon&4?TVE_COLLAPSE:TVE_EXPAND);
				break;
			case 0x8002:
				{
					CString strNewDirPath;
					long Index = 2;
					strNewDirPath.Format(GetString(IDS_NEWFOLDER_FORMAT),m_strStartFolder+strDirPath);
					do 
					{
						if(!CreateDirectory(strNewDirPath,NULL))
						{
							if(GetLastError()==0xb7)
							{
								strNewDirPath.Format(GetString(IDS_NEWFOLDERN_FORMAT),m_strStartFolder+strDirPath,Index);
								Index++;
								continue;
							}
						}
						
						break;
					} 
					while(TRUE);
				}
				
				break;
			}
		}
	}
	
	*pResult = 0;
}

void CFileExplore::OnRclickFileList(NMHDR* pNMHDR, LRESULT* pResult) 
{
	CPoint	point;
	GetCursorPos(&point);
	
	HTREEITEM hItem =	m_DirTree.GetSelectedItem();
	POSITION pos = m_FileList.GetFirstSelectedItemPosition();
	if(hItem!=NULL&&pos!=NULL)
	{
		CString	strDirPath;
		while(hItem!=m_hRoot)
		{
			strDirPath = _T("\\") + m_DirTree.GetItemText(hItem)+strDirPath;
			hItem =  m_DirTree.GetParentItem(hItem);
		}

		int nItem = m_FileList.GetNextSelectedItem(pos);
		strDirPath = m_strStartFolder + strDirPath + _T('\\') + m_FileList.GetItemText(nItem,0);
		
		int RetCmd = ShowContextMenu(0,GetSafeHwnd(), strDirPath, point.x, point.y, NULL,&m_pContextMenu2, &m_pContextMenu3);
		if(RetCmd==19)
		{
			m_FileList.EditLabel(nItem);			
		}
	}
	
	*pResult = 0;
}

void CFileExplore::OnDblclkFileList(NMHDR* pNMHDR, LRESULT* pResult) 
{
	HTREEITEM hItem =	m_DirTree.GetSelectedItem();
	POSITION pos = m_FileList.GetFirstSelectedItemPosition();
	if(hItem!=NULL&&pos!=NULL)
	{
		CString	strDirPath;
		while(hItem!=m_hRoot)
		{
			strDirPath = _T("\\") + m_DirTree.GetItemText(hItem)+strDirPath;
			hItem =  m_DirTree.GetParentItem(hItem);
		}
		
		int nItem = m_FileList.GetNextSelectedItem(pos);
		strDirPath = m_strStartFolder + strDirPath + _T('\\') + m_FileList.GetItemText(nItem,0);
		
		long hInstance = (long)ShellExecute(GetSafeHwnd(),_T("open"),strDirPath,NULL,NULL,SW_SHOWNORMAL);

		if(hInstance<=32)
		{
			switch(hInstance) 
			{
			case SE_ERR_NOASSOC:
			case SE_ERR_ASSOCINCOMPLETE:
				{
					CString sOpenWithDlgPath = _T("shell32.dll,OpenAs_RunDLL ") + strDirPath;
					ShellExecute(NULL,_T("open"),_T("Rundll32.exe"), sOpenWithDlgPath, NULL, SW_SHOWNORMAL);
				}
				break;
			default:
				LPVOID lpMsgBuf;
				FormatMessage( 
					FORMAT_MESSAGE_ALLOCATE_BUFFER | 
					FORMAT_MESSAGE_FROM_SYSTEM | 
					FORMAT_MESSAGE_IGNORE_INSERTS,
					NULL,
					GetLastError(),
					MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), // Default language
					(LPTSTR) &lpMsgBuf,
					0,
					NULL 
					);
				
				MessageBox((LPCTSTR)lpMsgBuf,GetString(IDS_OPEN_FILE_ERROR_NAME),MB_OK | MB_ICONINFORMATION );
				
				// Free the buffer.
				LocalFree( lpMsgBuf );
			}
		}
		
	}
	
	*pResult = 0;
}

void CFileExplore::Delete(BOOL bUseFile)
{
	HTREEITEM hItem = m_DirTree.GetSelectedItem(), hTmpItem;
	
	if(hItem&&(hItem!=m_hRoot||bUseFile))
	{
		CString strItemPath;
		
		hTmpItem = hItem;

		LPTSTR	buf	=	NULL;
		
		while(hTmpItem!=m_hRoot)
		{
			strItemPath	= _T('\\') + m_DirTree.GetItemText(hTmpItem)+ strItemPath;
			hTmpItem = m_DirTree.GetParentItem(hTmpItem);
		}

		CString strDir	=	m_strStartFolder+strItemPath;
		
		if(bUseFile)
		{
			POSITION pos = m_FileList.GetFirstSelectedItemPosition();
			if(pos==NULL)
				return;

			CStringArray	FileList;
			DWORD			dwAllBufferSize	=	0;
			DWORD			dwBufferOffset	=	0;

			while(pos)
			{
				int nItem = m_FileList.GetNextSelectedItem(pos);

				CString strTmp	=	strDir + _T('\\') + m_FileList.GetItemText(nItem,0);
				dwAllBufferSize	+=	(strTmp.GetLength()+1);

				FileList.Add(strDir+_T('\\') + m_FileList.GetItemText(nItem,0));
			}

			buf	=	new	TCHAR[dwAllBufferSize+2];
			ZeroMemory(buf,(dwAllBufferSize+2)*sizeof(TCHAR));

			for(int i=0;i<FileList.GetSize();i++)
			{
				DWORD dwStrLen	=	FileList[i].GetLength()+1;
				memcpy(buf+dwBufferOffset, FileList[i],dwStrLen*sizeof(TCHAR));   
				dwBufferOffset	+=	dwStrLen;
			}
		}
		else
		{
			buf	=	new	TCHAR[strDir.GetLength()+1];
			memcpy(buf, strDir,(strDir.GetLength()+1)*sizeof(TCHAR));   
		}
		
		int res;
		SHFILEOPSTRUCT fo	=	{0};
		
		//TCHAR buf[_MAX_PATH + 1]; // allow one more character
		//_tcscpy(buf, m_strStartFolder+strItemPath);    // copy caller's path name
		//buf[_tcslen(buf)+1]=0;    // need two NULLs at end
				
		fo.hwnd   = GetSafeHwnd();  // хэндл окна-владельца прогресс-диалога
		fo.pFrom  = buf;
		fo.wFunc  = FO_DELETE;
		fo.fFlags = ((GetKeyState(VK_SHIFT)>>1)?0:FOF_ALLOWUNDO);
		
		res = SHFileOperation(&fo);

		if(buf)
			delete []buf;

		if(!bUseFile)
		{
			hItem = m_DirTree.GetParentItem(hItem);
			m_DirTree.SelectItem(hItem);
		}
		
		//Refresh();
	}
}

void CFileExplore::OnEndlabeleditDirectoryTree(NMHDR* pNMHDR, LRESULT* pResult) 
{
	TV_DISPINFO* pTVDispInfo = (TV_DISPINFO*)pNMHDR;

	*pResult = 0;

	if(pTVDispInfo->item.pszText)
	{
		HTREEITEM hItem = m_DirTree.GetSelectedItem(), hTmpItem;

		if(hItem&&hItem!=m_hRoot)
		{
			CString strSrcItemPath, strDestItemPath;

			strDestItemPath.Format(_T("\\%s"),pTVDispInfo->item.pszText);
			
			hTmpItem = hItem;
			
			while(hTmpItem!=m_hRoot)
			{
				if(!strSrcItemPath.IsEmpty())
					strDestItemPath	= _T('\\') + m_DirTree.GetItemText(hTmpItem)+ strDestItemPath;
				strSrcItemPath	= _T('\\') + m_DirTree.GetItemText(hTmpItem)+ strSrcItemPath;

				hTmpItem = m_DirTree.GetParentItem(hTmpItem);
			}
			
			int res;
			SHFILEOPSTRUCT fo	=	{0};
			
			TCHAR bufSrc[_MAX_PATH + 1], bufDest[_MAX_PATH + 1]; // allow one more character
			_tcscpy(bufSrc, m_strStartFolder+strSrcItemPath);    // copy caller's path name
			_tcscpy(bufDest, m_strStartFolder+strDestItemPath);    // copy caller's path name
			bufSrc[_tcslen(bufSrc)+1]=0;    // need two NULLs at end
			bufDest[_tcslen(bufDest)+1]=0;    // need two NULLs at end
			
			fo.hwnd   = GetSafeHwnd();  // хэндл окна-владельца прогресс-диалога
			fo.pFrom  = bufSrc;
			fo.pTo	  = bufDest;
			fo.wFunc  = FO_RENAME;
			
			*pResult = !(res = SHFileOperation(&fo));
		}

	}
}

void CFileExplore::OnEndlabeleditFileList(NMHDR* pNMHDR, LRESULT* pResult) 
{
	LV_DISPINFO* pDispInfo = (LV_DISPINFO*)pNMHDR;
	
	*pResult = 0;

	if(pDispInfo->item.pszText)
	{

		HTREEITEM hItem = m_DirTree.GetSelectedItem(), hTmpItem;
		
		if(hItem)
		{
			CString strSrcItemPath = _T("\\"), strDestItemPath;
			
			hTmpItem = hItem;
			
			while(hTmpItem!=m_hRoot)
			{
				strSrcItemPath	= _T('\\') + m_DirTree.GetItemText(hTmpItem)+ strSrcItemPath;
				
				hTmpItem = m_DirTree.GetParentItem(hTmpItem);
			}

			strDestItemPath = strSrcItemPath;

			strSrcItemPath += m_FileList.GetItemText(pDispInfo->item.iItem,pDispInfo->item.iSubItem);
			strDestItemPath += pDispInfo->item.pszText;
			
			int res;
			SHFILEOPSTRUCT fo	=	{0};
			
			TCHAR bufSrc[_MAX_PATH + 1], bufDest[_MAX_PATH + 1]; // allow one more character
			_tcscpy(bufSrc, m_strStartFolder+strSrcItemPath);    // copy caller's path name
			_tcscpy(bufDest, m_strStartFolder+strDestItemPath);    // copy caller's path name
			bufSrc[_tcslen(bufSrc)+1]=0;    // need two NULLs at end
			bufDest[_tcslen(bufDest)+1]=0;    // need two NULLs at end
			
			fo.hwnd   = GetSafeHwnd();  // хэндл окна-владельца прогресс-диалога
			fo.pFrom  = bufSrc;
			fo.pTo	  = bufDest;
			fo.wFunc  = FO_RENAME;
			
			*pResult = !(res = SHFileOperation(&fo));
		}
		
	}
}

void CFileExplore::ShowDetails(BOOL bUseFile)
{
	HTREEITEM hItem = m_DirTree.GetSelectedItem(), hTmpItem;
	
	if(hItem&&hItem!=m_hRoot)
	{
		CString strItemPath;
		
		hTmpItem = hItem;
		
		while(hTmpItem!=m_hRoot)
		{
			strItemPath	= _T('\\') + m_DirTree.GetItemText(hTmpItem)+ strItemPath;
			hTmpItem = m_DirTree.GetParentItem(hTmpItem);
		}
		
		if(bUseFile)
		{
			POSITION pos = m_FileList.GetFirstSelectedItemPosition();
			if(pos==NULL)
				return;
			int nItem = m_FileList.GetNextSelectedItem(pos);
			strItemPath += _T('\\') + m_FileList.GetItemText(nItem,0);
		}
		
		ShowContextMenu(0,GetSafeHwnd(),m_strStartFolder+strItemPath,0,0,(LPCTSTR)20,&m_pContextMenu2, &m_pContextMenu3);
	}
}

void CFileExplore::OnBegindragFileList(NMHDR* pNMHDR, LRESULT* pResult) 
{
	NM_LISTVIEW* pNMListView = (NM_LISTVIEW*)pNMHDR;
	
	HTREEITEM hItem = m_DirTree.GetSelectedItem(), hTmpItem;
	
	if(hItem)
	{
		CString strFolderPath;
		
		hTmpItem = hItem;
		
		while(hTmpItem!=m_hRoot)
		{
			strFolderPath	= _T('\\') + m_DirTree.GetItemText(hTmpItem)+ strFolderPath;
			hTmpItem = m_DirTree.GetParentItem(hTmpItem);
		}

		CStringArray	m_FileArray;

		DWORD	dwAllStringSize	=	0;
		int		iFirstItem		=	-1;
		
		POSITION pos = m_FileList.GetFirstSelectedItemPosition();
		while(pos!=NULL)
		{
			CString strItemPath;

			int nItem = m_FileList.GetNextSelectedItem(pos);

			if(iFirstItem==-1)
				iFirstItem = nItem;
			
			//////////////////////////////////////////////////////////////////////////
			strItemPath = m_strStartFolder + strFolderPath + _T('\\') + m_FileList.GetItemText(nItem,0);
			//////////////////////////////////////////////////////////////////////////

			m_FileArray.Add(strItemPath);

			dwAllStringSize	+= (strItemPath.GetLength()+1)*sizeof(TCHAR);
		}

		if(m_FileArray.GetSize())
		{
			COleDataSource	*pDataSource	=	new COleDataSource();
			
			DROPFILES pFile = {0};
			pFile.pFiles = sizeof(DROPFILES);

			//long	lStringLen = (strItemPath.GetLength())*sizeof(TCHAR);
			
			HGLOBAL hgCF_HDROPData = GlobalAlloc(GPTR, sizeof(DROPFILES)+dwAllStringSize+2*sizeof(TCHAR)/*End Char*/);   

			LPVOID   lpCF_HDROPData = (LPSTR)GlobalLock(hgCF_HDROPData);

			memcpy(lpCF_HDROPData, (LPCVOID)&pFile,sizeof(DROPFILES));

			DWORD dwWriteOffset = sizeof(DROPFILES);
			for(int i =0;i<m_FileArray.GetSize();i++)
			{
				CString strItem	=	m_FileArray[i];
				DWORD dwStringLen	=	(strItem.GetLength()+1)*sizeof(TCHAR);

				memcpy(PBYTE(lpCF_HDROPData)+dwWriteOffset, (LPCVOID)(LPCTSTR)strItem,dwStringLen);

				dwWriteOffset	+=	dwStringLen;
			}

			GlobalUnlock(hgCF_HDROPData);                                                 
			
			pDataSource->CacheGlobalData( CF_HDROP, hgCF_HDROPData);

			PostMessage(WM_HANDLE_DRAG, (UINT)pDataSource, iFirstItem);
		}
	}
	
	*pResult = 0;
}

LONG CFileExplore::OnHandleDrag( UINT pDataSource, LONG nSelectedItem)
{
	//////////////////////////////////////////////////////////////////////////
	CPoint point, pointClient;
	GetCursorPos(&point);
	
	pointClient = point;
	ScreenToClient(&pointClient);
	
	m_pDragImage = m_FileList.CreateDragImage(nSelectedItem, &pointClient);
	m_pDragImage->Add(m_FileList.CreateDragImage(nSelectedItem, &pointClient)->ExtractIcon(0));
	
	// changes the cursor to the drag image (DragMove() is still required 
	m_bImageHidden = FALSE;
	m_pDragImage->BeginDrag(0, CPoint(8, 8));
	m_pDragImage->BeginDrag(1, CPoint(8, 16));
	m_pDragImage->DragEnter(GetDesktopWindow(), point);

	DROPEFFECT retVal =  ((COleDataSource*)pDataSource)->DoDragDrop(DROPEFFECT_COPY|DROPEFFECT_MOVE,NULL, &m_OleFileListDropSource);
	
	//Refresh();

	delete (COleDataSource*)pDataSource;
	
	m_pDragImage->DragLeave(GetDesktopWindow());
	m_pDragImage->EndDrag();
	
	delete m_pDragImage;
	m_pDragImage = NULL; 
	
	return 0;
}

//////////////////////////////////////////////////////////////////////////
// Overrides
SCODE CFileExplore::COleFileListDropSource::GiveFeedback(DROPEFFECT dropEffect)
{
	CPoint point;
	GetCursorPos(&point);

	if(m_pFE)
	{
		CRect	rctWindow;
		m_pFE->GetWindowRect(&rctWindow);

		if(rctWindow.PtInRect(point)&&WindowFromPoint(point)&&
			(WindowFromPoint(point)->GetSafeHwnd()==m_pFE->m_FileList.GetSafeHwnd()||
			WindowFromPoint(point)->GetSafeHwnd()==m_pFE->GetSafeHwnd()))
		{
			if(m_pFE->m_bImageHidden)
			{
				m_pFE->m_bImageHidden = FALSE;
				m_pFE->m_pDragImage->DragEnter(m_pFE->GetDesktopWindow(), point);
			}
				
			m_pFE->m_pDragImage->DragMove(point);
		}
		else
		{
			if(!m_pFE->m_bImageHidden)
			{
				m_pFE->m_bImageHidden = TRUE;
				m_pFE->m_pDragImage->DragLeave(m_pFE->GetDesktopWindow());
			}
		}
	}
		
	return m_bDragStarted ? DRAGDROP_S_USEDEFAULTCURSORS : S_OK;
}

DROPEFFECT CFileExplore::COleFEDropTarget::OnDragEnter( CWnd* pWnd, COleDataObject* pDataObject, DWORD dwKeyState, CPoint point )
{
	if(!pDataObject->IsDataAvailable(CF_HDROP))
	{
		m_dropEffectCurrent = DROPEFFECT_NONE;
		return m_dropEffectCurrent;
	}
	
    // Check if the control key was pressed          
    if((dwKeyState & MK_CONTROL) == MK_CONTROL)
	{
		m_dropEffectCurrent = DROPEFFECT_COPY;
		return m_dropEffectCurrent;
	}
    else
	{
		// We don't do move yet
		m_dropEffectCurrent = DROPEFFECT_MOVE;
		return DROPEFFECT_MOVE;
	}
}

BOOL CFileExplore::COleFEDropTarget::OnDrop( CWnd* pWnd, COleDataObject* pDataObject, DROPEFFECT dropEffect, CPoint point )
{
	m_pFE->m_DirTree.SelectDropTarget(NULL);
	
	BOOL bDirMode = pWnd->IsKindOf(RUNTIME_CLASS(CTreeCtrl));
	
	CString strDestPath;
	HTREEITEM hRootItem, hTmpItem;
	
	if(bDirMode)
	{
		UINT uFlags;
		hRootItem = m_pFE->m_DirTree.HitTest(point,&uFlags);
	}
	else
		hRootItem = m_pFE->m_DirTree.GetSelectedItem();

	if(hRootItem)
	{
		hTmpItem = hRootItem;
		while(hTmpItem!=m_pFE->m_hRoot)
		{
			strDestPath	= _T('\\') + m_pFE->m_DirTree.GetItemText(hTmpItem)+ strDestPath;
			hTmpItem = m_pFE->m_DirTree.GetParentItem(hTmpItem);
		}
		strDestPath = m_pFE->m_strStartFolder + strDestPath;
		
		HGLOBAL	hMem	=	pDataObject->GetGlobalData(CF_HDROP);
		HDROP   hDrop	=	(HDROP)GlobalLock(hMem);
		
		UINT FileCount = DragQueryFile(hDrop,0xFFFFFFFF,NULL,0);
		
		for(UINT i=0;i<FileCount;i++)
		{
			CString strSrcFilePath, strFileName, strDestFilePath;
			
			LPTSTR  FileBuffer = strSrcFilePath.GetBuffer(MAX_PATH);
			DragQueryFile(hDrop,i,FileBuffer,MAX_PATH);
			strSrcFilePath.ReleaseBuffer();
			
			strFileName = strSrcFilePath.Mid(strSrcFilePath.ReverseFind(_T('\\'))+1);
			
			strDestFilePath.Format(_T("%s\\%s"),strDestPath,strFileName);

			if(strDestFilePath.CompareNoCase(strSrcFilePath))
			{
				int res;
				SHFILEOPSTRUCT fo	=	{0};
				
				TCHAR bufSrc[_MAX_PATH + 1], bufDest[_MAX_PATH + 1]; // allow one more character
				_tcscpy(bufSrc, strSrcFilePath);    // copy caller's path name
				_tcscpy(bufDest, strDestFilePath);    // copy caller's path name
				bufSrc[_tcslen(bufSrc)+1]=0;    // need two NULLs at end
				bufDest[_tcslen(bufDest)+1]=0;    // need two NULLs at end
				
				fo.hwnd   = m_pFE->GetSafeHwnd();  // хэндл окна-владельца прогресс-диалога
				fo.pFrom  = bufSrc;
				fo.pTo	  = bufDest;
				fo.wFunc  = ((dropEffect==DROPEFFECT_MOVE)?FO_MOVE:FO_COPY);
				
				res = SHFileOperation(&fo);
			}
		}

		m_pFE->m_DirTree.SelectItem(hRootItem);
		//m_pFE->Refresh();

		GlobalUnlock(hMem);

	}
	
	return TRUE;
}

DROPEFFECT CFileExplore::COleFEDropTarget::OnDragOver(CWnd* pWnd, COleDataObject* pDataObject, DWORD dwKeyState, CPoint point )
{
	BOOL bDirMode = pWnd->IsKindOf(RUNTIME_CLASS(CTreeCtrl));

	if(bDirMode)
	{
		UINT uFlags;
		HTREEITEM hItem = m_pFE->m_DirTree.HitTest(point,&uFlags);

		TRACE(_T("\r\n --- OnDragOver --- %d"), uFlags);

		if(hItem!=NULL&&(TVHT_ONITEM&uFlags))
		{
			m_pFE->m_DirTree.SelectDropTarget(hItem);
		}
		else if(uFlags==TVHT_ONITEMBUTTON)
		{
			m_pFE->m_DirTree.Expand(hItem,TVE_EXPAND);
		}
	}

    if((dwKeyState & MK_CONTROL) == MK_CONTROL)
	{
		m_dropEffectCurrent = DROPEFFECT_COPY;
		return m_dropEffectCurrent;
	}
    else
	{
		// We don't do move yet
		m_dropEffectCurrent = DROPEFFECT_MOVE;
		return DROPEFFECT_MOVE;
	}
}
void CFileExplore::COleFEDropTarget::OnDragLeave(CWnd* pWnd)
{
	m_pFE->m_DirTree.SelectDropTarget(NULL);
	
	COleDropTarget::OnDragLeave(pWnd);
}

LRESULT CFileExplore::OnRefreshPath(WPARAM, LPARAM)
{
	//TRACE("\r\n --- CFileExplore::OnRefreshPath (%d) \"%s\" ---",Index, strPath);
	Refresh();
	return 0;
}

void CFileExplore::OnItemclickFileList(NMHDR* pNMHDR, LRESULT* pResult) 
{
	HD_NOTIFY *phdn = (HD_NOTIFY *) pNMHDR;

	if((phdn->iItem+1)==(m_iSortingMode>=0?m_iSortingMode:-m_iSortingMode))
		m_iSortingMode = -1*m_iSortingMode;
	else
		m_iSortingMode = phdn->iItem+1;
	
	SortList(m_iSortingMode);
	
	
	*pResult = 0;
}

void CFileExplore::SortList(int Mode)
{
	HDITEM	hItem	=	{0};

//	m_FileList.GetHeaderCtrl()->GetItem(Mode-1,&hItem);
//
//	hItem.mask		=	HDI_IMAGE| HDI_FORMAT;
//	hItem.iImage	=	1;
//	hItem.fmt		|=  HDF_IMAGE;
//
//	m_FileList.GetHeaderCtrl()->SetItem(Mode-1,&hItem);
	
	m_FileList.SortItems (CompareListItem,(LPARAM)Mode);

	m_SortHeader.SetSortArrow((m_iSortingMode>0?m_iSortingMode:-m_iSortingMode)-1,(m_iSortingMode>0?TRUE:FALSE));
}

int CALLBACK CFileExplore::CompareListItem(LPARAM lParam1, LPARAM lParam2, LPARAM lParamSort)
{
	CFileItem	*pItem1	=	(CFileItem	*)lParam1;
	CFileItem	*pItem2	=	(CFileItem	*)lParam2;

	if(IsBadReadPtr(pItem1,sizeof(CFileItem))||	IsBadReadPtr(pItem2,sizeof(CFileItem)))
		return 0;

	int	bValAddon	=	(lParamSort>=0?1:-1);

	switch(bValAddon*lParamSort) 
	{
	case 1:
		if(pItem1->strFileName<pItem2->strFileName) return -1*bValAddon;
		if(pItem1->strFileName>pItem2->strFileName) return 1*bValAddon;
		break;
	case 2:
		if(pItem1->dwFileSize<pItem2->dwFileSize) return -1*bValAddon;
		if(pItem1->dwFileSize>pItem2->dwFileSize) return 1*bValAddon;
		break;
	case 3:
		if(pItem1->strFileType<pItem2->strFileType) return -1*bValAddon;
		if(pItem1->strFileType>pItem2->strFileType) return 1*bValAddon;
		break;
	case 4:
		if(pItem1->ftData.QuadPart<pItem2->ftData.QuadPart) return -1*bValAddon;
		if(pItem1->ftData.QuadPart>pItem2->ftData.QuadPart) return 1*bValAddon;
		break;
	}
	
	return 0;
}

void CFileExplore::FileListDeleteAllItem()
{
	for(int iItemIndex	=	0; iItemIndex<m_FileList.GetItemCount();iItemIndex++)
	{
		CFileItem	*pItem = (CFileItem	*)m_FileList.GetItemData(iItemIndex);
		
		m_FileList.SetItemData(iItemIndex,NULL);
		
		if(!IsBadReadPtr(pItem,sizeof(CFileItem)))
			delete pItem;
	}
	
	m_FileList.DeleteAllItems();
}


LRESULT CFileExplore::WindowProc(UINT message,WPARAM wParam,LPARAM lParam)
{
	if(m_pContextMenu2)
		if(message==WM_INITMENUPOPUP||message==WM_DRAWITEM||message==WM_MENUCHAR||message==WM_MEASUREITEM)
		{
			return m_pContextMenu2->HandleMenuMsg(message, wParam, lParam);
		}

	if(m_pContextMenu3)
		if(message==WM_MENUCHAR)
		{
			LRESULT lr = 0;
			m_pContextMenu3->HandleMenuMsg2(message, wParam, lParam,&lr);
			return lr; 
		}
		
	return CResizableDialog::WindowProc(message,wParam,lParam);
}
