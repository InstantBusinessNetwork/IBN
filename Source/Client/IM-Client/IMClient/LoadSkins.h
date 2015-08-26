// LoadSkins.h: interface for the LoadSkins class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_LOADSKINS_H__13F4491B_C199_489A_8E60_A52A9978DBD5__INCLUDED_)
#define AFX_LOADSKINS_H__13F4491B_C199_489A_8E60_A52A9978DBD5__INCLUDED_

#include <comdef.h>
#include <list>

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

class LoadSkins  
{
	CRITICAL_SECTION m_lock;
public:
	HRESULT Load(BSTR URL, IStream **pStream, long* ErrorCode);
	LoadSkins();
	virtual ~LoadSkins();
};

#endif // !defined(AFX_LOADSKINS_H__13F4491B_C199_489A_8E60_A52A9978DBD5__INCLUDED_)
