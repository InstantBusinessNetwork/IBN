// SENDTO_MESSAGE.h: interface for the SENDTO_MESSAGE class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_SENDTO_MESSAGE_H__5C02EB0C_E9EA_4057_B395_0861D4959F20__INCLUDED_)
#define AFX_SENDTO_MESSAGE_H__5C02EB0C_E9EA_4057_B395_0861D4959F20__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

class SENDTO_MESSAGE  
{
public:
	SENDTO_MESSAGE();
	virtual ~SENDTO_MESSAGE();

	BOOL Parse(LPCTSTR szCmd);
	CArray< CString, CString& > m_files;
};

#endif // !defined(AFX_SENDTO_MESSAGE_H__5C02EB0C_E9EA_4057_B395_0861D4959F20__INCLUDED_)
