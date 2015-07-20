
var CurCol=2;
var CurRow=0;
var PrevRowId = null;
var PrevCol = -1;
var StartCol = 2;
var StartRow = 0;
var CurRowId = null;

//≈сли true то отключаетьс€ подсветка по клавиатуре
var DisableSelection = false;

//  оличество колонок/р€дов в гриде
var RowsCount = 0;
var ColsCount = 0;

var OldText = null;

var EditingFinished = true;

//breakEdit, если true то при редактировании был нажат ESC
var breakEdit = false;
//«начение на которое надо откатить при нажатие esc
var breakValue = null;
var GridFocused = true;

var _selColIndex = 0;
var _selRowId = null;

function HighlightCell(PrevRowId, PrevCol, NewRowId, NewCol)
{
	//alert('HighlightCell');
	if(PrevRowId!=null && PrevCol>=StartCol && mygrid!=null)
	{
		mygrid.cells(PrevRowId, PrevCol).setBgColor('white');
	}
	if(NewRowId!=null && NewCol>=StartCol)
	{
		mygrid.cells(NewRowId, NewCol).setBgColor('#FED758'); //E1FCFC
		CurRowId = NewRowId; CurCol = NewCol;
	}
}

function ProcessPressedKey(evnt)
{
	if (DisableSelection) return;
	
	//var tmp = document.getElementById('finance-main-div').scrollLeft;
	//document.getElementById('finance-main-table').focus();
	//document.getElementById('finance-main-div').scrollLeft = tmp;
	
	var SelRowId = null;
	var SelColInd = -1;
	if(mygrid!=null && GridFocused)
		{
				SelRowId = mygrid.getSelectedId();
				SelColInd = mygrid.getSelectedCellIndex();
			
			if(CurRow>=StartRow&&CurCol>=StartCol&&mygrid!=null && SelColInd<0)
			{
				mygrid.clearSelection();
				mygrid.enableKeyboardSupport(false);
				
				if(CurRowId==null) CurRowId = mygrid.getRowId(StartRow)
				RowsCount = mygrid.getRowsNum();
				ColsCount = mygrid.getColumnCount();
				if(evnt.keyCode==9||evnt.altKey||evnt.ctrlKey||evnt.keyCode==116||evnt.keyCode==27)//Tab, Alt, Cntrl, F5, Esc
				{
					return;
				}

				switch(evnt.keyCode)
				{
					case 39://right
							PrevCol = CurCol;
							PrevRowId = CurRowId;
							//DV:  do {CurCol++; } while (mygrid.getColWidth(CurCol) == 0);
							//document.getElementById("finance-main-div").scrollLeft += 25;
							CurCol++;
							if(CurCol>=ColsCount)
								{
									CurRowId = Get_NextRowId(PrevRowId);
									CurCol=StartCol;
									//DV: do {CurCol++; } while (mygrid.getColWidth(CurCol) == 0);
								}
							if(CurRowId!=null)
								{
									HighlightCell(PrevRowId, PrevCol, CurRowId, CurCol);
								}
							else
							{
								CurRowId = PrevRowId;
								CurCol = PrevCol;
							}		
						break;	
					case 38: //top
							PrevRowId = CurRowId;
							CurRowId = Get_PrevRowId(PrevRowId);
							//document.getElementById('finance-main-table').focus();
							//document.getElementById('finance-main-table').scrollTop -= 25;
							if(CurRowId!=null)
							{
								HighlightCell(PrevRowId, CurCol, CurRowId, CurCol);
							}
							else
							{
								CurRowId = PrevRowId;
							}
						break;	
					case 37: //left	
							PrevCol = CurCol;
							PrevRowId = CurRowId;
							//document.getElementById("finance-main-div").scrollLeft -= 25;							
							//DV: do {CurCol--; } while (mygrid.getColWidth(CurCol) == 0);
							CurCol--;
							if(CurCol<StartCol)
							{
								CurCol=ColsCount-1; 
								CurRowId = Get_PrevRowId(PrevRowId);
								//DV: do {CurCol--; } while (mygrid.getColWidth(CurCol) == 0);
							}
							if(CurRowId!=null)
							{
								HighlightCell(PrevRowId, PrevCol, CurRowId, CurCol);
							}
							else
							{
								CurRowId = PrevRowId;
								CurCol = PrevCol;
							}
						break;
					case 40: //down
							PrevRowId = CurRowId;
							CurRowId = Get_NextRowId(PrevRowId);
							//document.getElementById('finance-main-table').focus();
							if(CurRowId!=null)
							{
								HighlightCell(PrevRowId, CurCol, CurRowId, CurCol);
							}
							else
							{
								CurRowId = PrevRowId;
							}
						break;
					/*case 46: //delete
						var oldValue = mygrid.cells(CurRowId, CurCol).getValue();
						if (oldValue.length > 5 && oldValue.substr(0,5).toUpperCase() == "<DIV>") { break; }
						if(EditingFinished)
						{
							mygrid.cells(CurRowId, CurCol).setValue("0");
							//GridCellEdit(2, CurRowId, CurCol);
						}
						break;	*/
					case 13: //enter
							PrevRowId = CurRowId;
							CurRowId = Get_NextRowId(PrevRowId);
							if(CurRowId!=null)
							{
								HighlightCell(PrevRowId, CurCol, CurRowId, CurCol);
							}
							else
							{
								CurRowId = PrevRowId;
							}
						break;
					default:
						OldText = mygrid.cells(CurRowId, CurCol).getValue();
						//alert(OldText);
						if (OldText.length > 5 && OldText.substr(0, 5).toUpperCase()=="<DIV>") break;
						EditingFinished = false;
						mygrid.cells(CurRowId,CurCol).setValue("");
						mygrid.selectCell(mygrid.getRowIndex(CurRowId), CurCol,false,false,true);
						break;	
				}
			}
			if(CurRow>=StartRow&&CurCol>=StartCol&&mygrid!=null && SelColInd>=StartCol)
			{
				switch(evnt.keyCode)
				{
					case 27://Escape
							StopEditing(false);
							break;
					case 13://Enter
							PrevRowId = CurRowId;
							CurRowId = Get_NextRowId(PrevRowId);
							if(CurRowId!=null)
							{
								HighlightCell(PrevRowId, CurCol, CurRowId, CurCol);
							}
							else
							{
								CurRowId = PrevRowId;
							}
							StopEditing(true);
							break;
				}
			}
	}
}

function StopEditing(updatedata)
{
	if (mygrid == null) return;
	if(mygrid.editor)
	{
		var val = mygrid.editor.val;
		if(OldText!=null && !updatedata)
			{
				breakEdit = true;
				breakValue = OldText;
			}
		mygrid.editStop();
		OldText = null;
		mygrid.clearSelection();
		EditingFinished = true;
	}
	else
	{
		//mouse click
		EditingFinished = true;
		//GridCellEdit(2, CurRowId, CurCol);
	}
}



function ProcessMouseClick()
{
	if (DisableSelection) return;
	if(mygrid!=null)
	{
		GridFocused = true;
		
		var SelRowId = mygrid.getSelectedId();
		var SelCol = mygrid.getSelectedCellIndex();
		
		_selColIndex = mygrid.getSelectedCellIndex(); 
		_selRowId = mygrid.getSelectedId();
		if(SelRowId!=null && SelCol>=StartCol && SelCol<mygrid.getColumnCount())
		{
			StopEditing(true);	
			HighlightCell(CurRowId, CurCol, SelRowId, SelCol);
		}
		mygrid.clearSelection();
	}
}


function ProcessMouseDblClick()
{
	if (DisableSelection) return;
	if(mygrid!=null)
	{
		
		//alert('ProcessMouseDblClick '+_selColIndex+' '+_selRowId);
		if (_selColIndex == 0 && mygrid.getParentId(_selRowId) != 'null')
		{
			OpenWindow('../Projects/FinanceSpreadSheetPopUp.aspx?RowId='+_selRowId+'&Index='+_selColIndex+'&Value='+mygrid.cells(_selRowId, _selColIndex).getValue()+qstring2+'&ProjectId='+get_querystring("ProjectId"),350,60,false);
		}
		else
		{
			GridFocused = true;
			StopEditing(true);
			if(CurRowId!=null && CurCol>=StartCol && CurCol<mygrid.getColumnCount())
			{
				OldText = mygrid.cells(CurRowId, CurCol).getValue();
				if (OldText.length > 5 && OldText.substr(0, 5).toUpperCase()=="<DIV>") return;
				EditingFinished = false;
				mygrid.cells(CurRowId,CurCol).setValue("");
				mygrid.selectCell(mygrid.getRowIndex(CurRowId), CurCol, false, false, true);
			}
		}
	}
}

//===========================//
function Get_NextRowId(rowid)
{
	if(mygrid!=null)
	{
		index = mygrid.getRowIndex(rowid);
		
		if(index>=0)
		{
			if(mygrid.rowsCol[index].nextSibling)
				return mygrid.rowsCol[index].nextSibling.idd;
			else
				return null;	
		}		
		else
		if (mygrid.getParentId(CurRowId) != null || mygrid.getParentId(CurRowId) != 'null')
			return mygrid.rowsCol[mygrid.getRowIndex(mygrid.getParentId(CurRowId))].nextSibling.idd;		
	}
	else
		return null;
}

function Get_PrevRowId(rowid)
{
	if(mygrid!=null)
	{
		index = mygrid.getRowIndex(rowid);
		
		if(index>=0)
		{
			if(mygrid.rowsCol[index].previousSibling)
				return mygrid.rowsCol[index].previousSibling.idd;
			else
				return null;	
		}		
		else
		if (mygrid.getParentId(CurRowId) != null || mygrid.getParentId(CurRowId) != 'null')
			return mygrid.rowsCol[mygrid.getRowIndex(mygrid.getParentId(CurRowId))].idd;
	}
	else
		return null;
}