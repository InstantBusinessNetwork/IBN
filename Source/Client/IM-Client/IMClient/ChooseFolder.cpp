// ChooseFolder.cpp: implementation of the CChooseFolder class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "ChooseFolder.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////
static int CALLBACK
BrowseCallbackProc (HWND hWnd, UINT uMsg, LPARAM lParam, LPARAM lpData)
{
    TCHAR szPath[_MAX_PATH];
    switch (uMsg) {
    case BFFM_INITIALIZED:
        if (lpData)
            SendMessage(hWnd,BFFM_SETSELECTION,TRUE,lpData);
        break;
    case BFFM_SELCHANGED:
        SHGetPathFromIDList(LPITEMIDLIST(lParam),szPath);
        SendMessage(hWnd, BFFM_SETSTATUSTEXT, NULL, LPARAM(szPath));
        break;
    }
    return 0;
}

CChooseFolder::CChooseFolder(HWND hOwner/* = NULL*/)
:    m_pShf(NULL), m_pMalloc(NULL)
{
    if (SHGetDesktopFolder(&m_pShf) != NOERROR) m_pShf    = NULL;
    if (SHGetMalloc(&m_pMalloc) != NOERROR) m_pMalloc = NULL;
    memset(&m_asFolder, 0, sizeof(m_asFolder));
    m_asFolder.ulFlags =    BIF_RETURNONLYFSDIRS | BIF_STATUSTEXT;
    m_asFolder.pszDisplayName = NULL;
    m_asFolder.hwndOwner = hOwner;
    m_asFolder.lpfn = BrowseCallbackProc;        
}

CChooseFolder::~CChooseFolder()
{
    if (m_pMalloc)    m_pMalloc->Release();
    if (m_pShf)    m_pShf->Release();
}

BOOL CChooseFolder::DoModal(LPCTSTR title, CString &path, HWND hOwner)
{
    int pathLen  = path.GetLength();
    LPTSTR asPath = path.GetBuffer(_MAX_PATH + 1);
    /*LPITEMIDLIST pIdl = NULL;
    if (pathLen && m_pShf) { // IShellFolder
        ULONG                    chEaten = 0L, dwAttributes = 0L;
        LPOLESTR            pOleStr = path.AllocSysString();  
		m_pShf->ParseDisplayName(hOwner, NULL, pOleStr, &chEaten, &pIdl, &dwAttributes);
        ::SysFreeString(pOleStr);    
    } */   
    m_asFolder.lpszTitle    = title;
    m_asFolder.lParam            = LPARAM(asPath);
    if (hOwner) m_asFolder.hwndOwner = hOwner;
    LPITEMIDLIST item = SHBrowseForFolder(&m_asFolder);
    BOOL ok = FALSE;
    if (item) ok =  SHGetPathFromIDList(item,    asPath); 
    if (m_pMalloc) {
        if (item) m_pMalloc->Free(item);
        //if (pIdl) m_pMalloc->Free(pIdl); 
    }
    path.ReleaseBuffer();
    return ok;
}
