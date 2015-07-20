// RS2XML1.h: interface for the CRS2XML class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_RS2XML1_H__B046384D_B6FD_43F4_B531_A0B09A56783D__INCLUDED_)
#define AFX_RS2XML1_H__B046384D_B6FD_43F4_B531_A0B09A56783D__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

// че-то говорит что имена обрезаны до 255 символов [18:12 - 13.11.2000]
#pragma warning(disable:4786)

#define R2XPOS_BODY_BEGIN		1
#define R2XPOS_TAG				2
#define R2XPOS_ATTR				3
#define R2XPOS_CHILD			4
#define R2XPOS_BODY_END			5

#define R2X_BODY_BEGIN(bodyName) bodyName,NULL,R2XPOS_BODY_BEGIN,
#define R2X_TAG(tagName,fieldName) tagName,fieldName,R2XPOS_TAG,
#define R2X_ATTR(attrName,fieldName) attrName,fieldName,R2XPOS_ATTR,
#define R2X_CHILD(arrName) (LPTSTR)arrName,NULL,R2XPOS_CHILD,
#define R2X_BODY_END NULL,NULL,R2XPOS_BODY_END

struct CR2XItem
{
	LPTSTR m_szItemName;
	LPTSTR m_szFieldName;
	BYTE m_dwItemPos;
};

class CRS2XML
{
public:
	CRS2XML();
	virtual ~CRS2XML();
	static BOOL Parse(const _RecordsetPtr &pRS, const CComPtr<IXMLDOMNode> pNode, const CR2XItem *pItems);
	static BOOL ParseList(const _RecordsetPtr &pRS, const CComPtr<IXMLDOMNode> pNode, const CR2XItem *pItems, CComBSTR bsListName);
	static BOOL ParseMulti(const _RecordsetPtr &pRS, const CComPtr<IXMLDOMNode> pNode, const CR2XItem *pItems, CComBSTR bsListName);
};

#endif // !defined(AFX_RS2XML1_H__B046384D_B6FD_43F4_B531_A0B09A56783D__INCLUDED_)
