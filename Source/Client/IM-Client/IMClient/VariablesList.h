// VariablesList.h: interface for the CVariablesList class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_VARIABLESLIST_H__EF06F1B2_D17E_4EC1_A43F_40506D024679__INCLUDED_)
#define AFX_VARIABLESLIST_H__EF06F1B2_D17E_4EC1_A43F_40506D024679__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

class CVariablesList  
{
	struct tagListElement
	{
		tagListElement *pNextElement;
//		CComBSTR bstrName;
//		CComBSTR bstrValue;
		_bstr_t bstrName;
		_bstr_t bstrValue;

		tagListElement()
		{
			pNextElement = NULL;
			bstrName = _T("");
			bstrValue = _T("");
		}
	};

protected:
	tagListElement *m_pFirst;

public:
	void DeleteVariable(LPCTSTR bstrName);
	BOOL GetVariable(LPCTSTR bstrName, BSTR* bstrValue);
	BOOL SetVariable(LPCTSTR bstrName, LPCTSTR bstrValue);
	CVariablesList();
	virtual ~CVariablesList();

private:
	HANDLE m_hCanModify;
};

#endif // !defined(AFX_VARIABLESLIST_H__EF06F1B2_D17E_4EC1_A43F_40506D024679__INCLUDED_)
