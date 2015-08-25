// McLoginAutoCompleteSource.h: interface for the CMcLoginAutoCompleteSource class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_MCLOGINAUTOCOMPLETESOURCE_H__3B650F79_B5EA_40CD_9BA8_6FDDD29BBA1A__INCLUDED_)
#define AFX_MCLOGINAUTOCOMPLETESOURCE_H__3B650F79_B5EA_40CD_9BA8_6FDDD29BBA1A__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "shlwapi.h"

class CMcLoginAutoCompleteSource: public CCmdTarget
{
public:
	CMcLoginAutoCompleteSource();
	virtual ~CMcLoginAutoCompleteSource();
public:
	BEGIN_INTERFACE_PART(EnumString, IEnumString)
		INIT_INTERFACE_PART(CMcLoginAutoCompleteSource, EnumString)
		STDMETHOD(Next)( /* [in] */ ULONG celt,/* [length_is][size_is][out] */ LPOLESTR  *rgelt,/* [out] */ ULONG __RPC_FAR *pceltFetched);
		STDMETHOD(Skip)(/* [in] */ ULONG celt);
		STDMETHOD(Reset)( void);
		STDMETHOD(Clone)(/* [out] */ IEnumString  **ppenum);
	END_INTERFACE_PART(EnumString)

/*	BEGIN_INTERFACE_PART(ACList, IACList)
		INIT_INTERFACE_PART(CMcLoginAutoCompleteSource, ACList)
		STDMETHOD(Expand)(LPCOLESTR pszExpand);
	END_INTERFACE_PART(ACList)*/
		
public:
	DECLARE_INTERFACE_MAP()
protected:
	void ReloadArray(LPCOLESTR strTemplate);
private:
	CStringArray	m_strArray;
};

#endif // !defined(AFX_MCLOGINAUTOCOMPLETESOURCE_H__3B650F79_B5EA_40CD_9BA8_6FDDD29BBA1A__INCLUDED_)
