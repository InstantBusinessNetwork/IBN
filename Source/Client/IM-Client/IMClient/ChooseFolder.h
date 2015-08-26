// ChooseFolder.h: interface for the CChooseFolder class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_CHOOSEFOLDER_H__A500610B_4C54_4A20_A071_FCF03D9833C3__INCLUDED_)
#define AFX_CHOOSEFOLDER_H__A500610B_4C54_4A20_A071_FCF03D9833C3__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <shlobj.h>
#pragma comment(lib,"shell32")

class CChooseFolder  
{
public:
    BOOL DoModal(LPCTSTR title, CString &path, HWND hOwner = NULL);
    CChooseFolder(HWND hOwner = NULL);
    virtual ~CChooseFolder();
	
protected:
    LPMALLOC m_pMalloc;
    BROWSEINFO m_asFolder;
    LPSHELLFOLDER m_pShf;
};

#endif // !defined(AFX_CHOOSEFOLDER_H__A500610B_4C54_4A20_A071_FCF03D9833C3__INCLUDED_)
