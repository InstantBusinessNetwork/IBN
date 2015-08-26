// IBNTO_MESSAGE.h: interface for the IBNTO_MESSAGE class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_IBNTO_MESSAGE_H__04997910_7F65_436A_9EE7_EE2128025222__INCLUDED_)
#define AFX_IBNTO_MESSAGE_H__04997910_7F65_436A_9EE7_EE2128025222__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

struct LoginItem
{
	LoginItem()
	{
		Login	=	_T("");
		Domain	=	_T("");
	}

	LoginItem(LPCTSTR Login, LPCTSTR Domain)
	{
		this->Login = Login;
		this->Domain = Domain;
	}

	CString Login;
	CString Domain;
};

class IBNTO_MESSAGE  
{
public:
	CArray<LoginItem,LoginItem> Logins;

	CString Message;

	IBNTO_MESSAGE();
	virtual ~IBNTO_MESSAGE();

	BOOL Parse(LPCTSTR szURL);
	void Unescape(CString* pstr);
};

#endif // !defined(AFX_IBNTO_MESSAGE_H__04997910_7F65_436A_9EE7_EE2128025222__INCLUDED_)
