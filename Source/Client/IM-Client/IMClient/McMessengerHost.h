// MyHost.h: interface for the CMcMessengerHost class.
//
//////////////////////////////////////////////////////////////////////
#include "mshtml.h"
#include "mshtmhst.h"


#if !defined(AFX_MYHOST_H__B70FAD01_96E6_11D5_8B75_966A41699779__INCLUDED_)
#define AFX_MYHOST_H__B70FAD01_96E6_11D5_8B75_966A41699779__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

class CMainDlg;
class CWebBrowser2;

// IDropTarget [3/30/2002]
class CMcMessengerDropTarget: public COleDropTarget
{
public:
	CMcMessengerDropTarget(CMainDlg	*pMessenger, CWebBrowser2* pAttachBrowser):m_pMessenger(pMessenger),m_pAttachBrowser(pAttachBrowser)
	{};
	virtual ~CMcMessengerDropTarget(){};
	virtual DROPEFFECT OnDragEnter(	CWnd* pWnd,COleDataObject* pDataObject,DWORD dwKeyState,CPoint point );
	virtual DROPEFFECT OnDragOver(CWnd* pWnd,COleDataObject* pDataObject,DWORD dwKeyState,CPoint point );
	virtual BOOL OnDrop(CWnd* pWnd,COleDataObject* pDataObject,DROPEFFECT dropEffect,CPoint point);
protected:
	CMainDlg		*m_pMessenger;	
	CWebBrowser2	*m_pAttachBrowser;
	DROPEFFECT		m_dropEffectCurrent;
};

//class CMcImpIDocHostUIHandler : public IDocHostUIHandler
//{
//protected:
//	long							m_cRef;
//	CMcMessengerDropTarget			*m_pTarget;
//	
//    public:
//        CMcImpIDocHostUIHandler(CMcMessengerDropTarget*pTarget);
//        ~CMcImpIDocHostUIHandler(void);
//		
//        STDMETHODIMP QueryInterface(REFIID, void **);
//        STDMETHODIMP_(ULONG) AddRef(void);
//        STDMETHODIMP_(ULONG) Release(void);
//		
//		STDMETHODIMP GetHostInfo(DOCHOSTUIINFO * pInfo);
//		STDMETHODIMP ShowUI(
//			DWORD dwID, 
//			IOleInPlaceActiveObject * pActiveObject,
//			IOleCommandTarget * pCommandTarget,
//			IOleInPlaceFrame * pFrame,
//			IOleInPlaceUIWindow * pDoc);
//		STDMETHODIMP HideUI(void);
//		STDMETHODIMP UpdateUI(void);
//		STDMETHODIMP EnableModeless(BOOL fEnable);
//		STDMETHODIMP OnDocWindowActivate(BOOL fActivate);
//		STDMETHODIMP OnFrameWindowActivate(BOOL fActivate);
//		STDMETHODIMP ResizeBorder(
//			LPCRECT prcBorder, 
//			IOleInPlaceUIWindow * pUIWindow, 
//			BOOL fRameWindow);
//		STDMETHODIMP ShowContextMenu(
//			DWORD dwID, 
//			POINT * pptPosition,
//			IUnknown* pCommandTarget,
//			IDispatch * pDispatchObjectHit);
//		STDMETHODIMP TranslateAccelerator(/* [in] */ LPMSG lpMsg,/* [in] */ const GUID __RPC_FAR *pguidCmdGroup,/* [in] */ DWORD nCmdID);
//		STDMETHODIMP GetOptionKeyPath(BSTR* pbstrKey, DWORD dw);
//		STDMETHODIMP GetDropTarget( 
//            /* [in] */ IDropTarget __RPC_FAR *pDropTarget,
//            /* [out] */ IDropTarget __RPC_FAR *__RPC_FAR *ppDropTarget);
//			
//			STDMETHODIMP GetExternal( 
//            /* [out] */ IDispatch __RPC_FAR *__RPC_FAR *ppDispatch);
//			
//			STDMETHODIMP TranslateUrl( 
//            /* [in] */ DWORD dwTranslate,
//            /* [in] */ OLECHAR __RPC_FAR *pchURLIn,
//            /* [out] */ OLECHAR __RPC_FAR *__RPC_FAR *ppchURLOut);
//			
//			STDMETHODIMP FilterDataObject( 
//            /* [in] */ IDataObject __RPC_FAR *pDO,
//            /* [out] */ IDataObject __RPC_FAR *__RPC_FAR *ppDORet);
//};


#endif // !defined(AFX_MYHOST_H__B70FAD01_96E6_11D5_8B75_966A41699779__INCLUDED_)
