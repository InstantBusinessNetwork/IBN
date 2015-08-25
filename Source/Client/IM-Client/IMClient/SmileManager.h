// SmileManager.h: interface for the CSmileManager class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_SMILEMANAGER_H__19D08281_7950_4468_8E37_CB8AA61AF43E__INCLUDED_)
#define AFX_SMILEMANAGER_H__19D08281_7950_4468_8E37_CB8AA61AF43E__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <list>


class CSmileInfo
{
private:
	long m_Index;
	CString m_Id;
	CString m_Smile;
	CString m_Text;

	// Auto-calculated
	CString m_HtmlSmile;

	long m_Hit;
	long m_LastAccess;

public:
	CSmileInfo()
	{
		m_Index = -1;

		m_Id = _T("");
		m_Smile = _T("");
		m_Text = _T("");
		m_HtmlSmile = _T("");

		m_Hit = 0;
		m_LastAccess = 0;
	}

	CSmileInfo( const CSmileInfo& Src)
	{
		m_Index = Src.GetIndex();
		m_Id = Src.GetId();
		m_Smile = Src.GetSmile();
		m_Text = Src.GetText();
		m_HtmlSmile = Src.GetHtmlSmile();

		m_Hit = Src.m_Hit;
		m_LastAccess = Src.m_LastAccess;
	}

	CSmileInfo(long Index, LPCTSTR Id, LPCTSTR Smile, LPCTSTR Text)
	{
		m_Index = Index;
		m_Id = Id;
		m_Smile = Smile;
		m_Text = Text;

		m_HtmlSmile = m_Smile;

		m_HtmlSmile.Replace(_T("&"),_T("&amp;"));
		m_HtmlSmile.Replace(_T(">"),_T("&gt;"));
		m_HtmlSmile.Replace(_T("<"),_T("&lt;"));
		m_HtmlSmile.Replace(_T("\""),_T("&quot;"));
		
		m_Hit = 0;
		m_LastAccess = 0;
	}

	virtual ~CSmileInfo()
	{
	}

	long GetIndex() const {return m_Index;};
	LPCTSTR GetId() const {return m_Id;};
	LPCTSTR GetSmile() const {return m_Smile;};
	LPCTSTR GetText() const {return m_Text;};

	LPCTSTR GetHtmlSmile() const
	{
		return m_HtmlSmile;
	};

	long GetHit() const {return m_Hit;};
	long GetLastAccess() const {return m_LastAccess;};

	const CSmileInfo& operator=(const CSmileInfo &Src) 
	{
		m_Index = Src.GetIndex();

		m_Id = Src.GetId();
		m_Smile = Src.GetSmile();
		m_Text = Src.GetText();
		m_HtmlSmile = Src.GetHtmlSmile();

		m_Hit = Src.m_Hit;
		m_LastAccess = Src.m_LastAccess;
		
		return *this;
	};

	BOOL operator==(const CSmileInfo &Src) const
	{
		return (m_Id==Src.m_Id) &&
			(m_Smile==Src.m_Smile) &&
			(m_Text==Src.m_Text);
	};

	BOOL operator!=(const CSmileInfo &Src) const
	{
		return !(operator==(Src));
	};

	BOOL operator>(const CSmileInfo &Src) const
	{
		return m_Hit>Src.m_Hit || (m_Hit==Src.m_Hit && m_LastAccess>Src.m_LastAccess);
	};

	void SetSort(long Hit, long LastAccess)
	{
		m_Hit = Hit;
		m_LastAccess = LastAccess;
	}

	void IncSort()
	{
		m_Hit++;
		m_LastAccess = (long)time(NULL);
	}

public:
	static CSmileInfo Empty;
};

extern CSmileInfo FindSmileImage(LPCWSTR HtmlText, int& StartPos, int& Length);

typedef std::list<CSmileInfo> CSmileInfoList;
typedef CSmileInfoList::iterator	CSmileInfoListEnum;

class CSmileManager  
{
public:
	CSmileManager();
	virtual ~CSmileManager();

	BOOL Init();

	CSmileInfoList& GetSmiles();

	CSmileInfo& GetSmile(LPCTSTR SmileId);
	CSmileInfo& GetSmile(int SmileIndex);

	CBitmap* GetSmilePreview(LPCTSTR SmileId);

	void IncHitCount(LPCTSTR SmileId);

protected:
	void LoadHitCount();
	void SaveHitCount();

private:
	CSmileInfoList m_SmileList;
};

#endif // !defined(AFX_SMILEMANAGER_H__19D08281_7950_4468_8E37_CB8AA61AF43E__INCLUDED_)
