#ifndef _OFS_GLOBAL_FUNCITON_H
#define _OFS_GLOBAL_FUNCITON_H

CString GetString(UINT nID);
void UpdateMenu(CWnd *pWnd, CMenu *pMenu);
CString GetOptionString(UINT nSectionID, UINT nEntryID, LPCTSTR lpszDefault = NULL);
UINT GetOptionInt(UINT nSectionID, UINT nEntryID, int nDefault = 0);
BOOL WriteOptionString(UINT nSectionID, UINT nEntryID, LPCTSTR lpszValue);
BOOL WriteOptionInt(UINT nSectionID, UINT nEntryID, int nValue);
_bstr_t DelSpace(_bstr_t &wstr);
INT CALLBACK NEnumFontNameProc(LOGFONT *plf, TEXTMETRIC *ptm, INT nFontType, LPARAM lParam);
CString RectToString(const CRect &m_Rect);
CRect   StringToRect(const CString &m_StrRect);
void    Pack(CString &strPack,const CString &strPath);
void    UnPack(CString &strPack,const CString &strPath);
int Replace(BSTR &lpszData, LPCWSTR lpszOld, LPCWSTR lpszNew);
void    RemoveParagraf(BSTR &string);
void    RoundExitAddon(CWnd *m_hWnd,int Step = 12,DWORD dwTime = 20);
void	FitRectToWindow(CRect &ModRect);

HRESULT LoadDATAFromStream(IN IStream* pDataStream,OUT BYTE **pBuffer, OUT DWORD *pSize);

void	TestDirAndCreateDir(LPCTSTR Path);

HIMAGELIST GetSystemImageList(BOOL fSmall);

int GetIconIndexInSystemImageList(BOOL fSmall,LPCTSTR Path);

BOOL IsTVFileType(BSTR URL);
HRESULT GetTargetRect(LPDISPATCH pDisp, long &cx, long &cy, long &tearoff, long &fullscreen);

BOOL McPlaySound(UINT SoundId);

void AddWindowToClose(CWnd *pWnd);
void RemoveWindowToClose(CWnd *pWnd);
void CloseAllWindows();

HRESULT	Array2Stream(VARIANT varData, IStream** ppDataStream);

void HiMetricToPixel(const SIZEL *lpSizeInHiMetric, LPSIZEL lpSizeInPix);

CString	GetRegFileText(LPCTSTR lpszSection, LPCTSTR lpszEntry);
BOOL	SetRegFileText(LPCTSTR lpszSection, LPCTSTR lpszEntry, LPCTSTR pData);
CString	GetDataDir();

CString	ByteSizeToStr(DWORD dwSize);

CString	GetAppDataDir();

CString	GetMyDocumetPath(LPCTSTR UserRoleName, int UserId);

BOOL	SetMyDocumetPath(LPCTSTR UserRoleName, int UserId, LPCTSTR Path);

int ShowContextMenu(long TreeAddon, HWND hWnd, LPCTSTR pszPath, int x, int y, LPCTSTR stVer = NULL	, IContextMenu2 **pContextMenu2 = NULL, IContextMenu3 **pContextMenu3 = NULL);

HRESULT NavigateNewWindow(LPUNKNOWN pBrowserDispatch, LPCTSTR strUrl);

SYSTEMTIME TimeTToSystemTime(time_t &TimeT);

BOOL SetItemToColorStorage(LPCTSTR UserRole, int UserId, DWORD dwId, LPCTSTR strText, DWORD dwColor);

BOOL GetItemFromColorStorage(LPCTSTR UserRole, int UserId, DWORD dwId, CString& strText, DWORD &dwColor);

BOOL GetColorFromColorStorage(LPCTSTR UserRole, int UserId, DWORD dwId, DWORD &dwColor);

BOOL CheckEmailString(LPCTSTR strEmail);

BOOL IsSendToLinkCreated();
void CreateSendToLink();
BOOL DeleteSendToLink();

CComPtr<IXMLDOMNode> AppendWithSort(IXMLDOMNode* parentNode, IXMLDOMNode* newChildNode, BSTR bstCmpAttributeName);

#define PACKVERSION(major,minor) MAKELONG(minor,major)
DWORD GetDllVersion(LPCTSTR lpszDllName);

CComBSTR SplitHTML(unsigned int MaxLen, CComBSTR &bsText);

CString GetProductLanguage();

CString GetProductPath();


#endif // _OFS_GLOBAL_FUNCITON_H