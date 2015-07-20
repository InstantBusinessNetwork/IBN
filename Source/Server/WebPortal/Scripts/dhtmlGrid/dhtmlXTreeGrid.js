/*
Copyright Scand LLC http://www.scbr.com
To use this component please contact info@scbr.com to obtain license
*/

/**
*	@desc: switch current row state (collapse/expand) tree grid row
*	@param: obj - row object
*	@type: private
*/
dhtmlXGridObject.prototype.doExpand=function(obj){
    this.editStop();
	var row = obj.parentNode.parentNode.parentNode;
	var tbl = this.obj;
	var disp = "";
	if(obj.src.indexOf("plus")!=-1){
		this.expandKids(row,true)
	}else{
        if (!row._closeable)
    		this.collapseKids(row)
	}
}




/**
*	@desc: close row of treegrid (removes kids from dom)
*	@param: curRow - row to process kids of
*	@param: kidsAr - array where to store child rows. if exists, then rows added to it
*	@param: start - row to start processing from (only if 3 arguments)
*	@type: private
*/
dhtmlXGridObject.prototype.collapseKids=function(curRow,kidsAr,start){
    if (curRow.expand!="") return;
    if ((this._spnFH)&&(!this._spnFH(curRow.idd,1))) return;

	if(!start)
		start = curRow.rowIndex;

	if(!kidsAr){
		kidsAr = new dhtmlxArray(0)
		curRow.expand="none"
		treeCell = curRow.childNodes[this.cellType._dhx_find("tree")]
		treeCell.innerHTML = treeCell.innerHTML.replace(/\/minus\.gif/,"/plus.gif")
	}
	for(var i=start;i<this.obj._rowslength();i++){
		var row = this.obj._rows(i);
		if(row.parent_id==curRow.idd){
			if(row.expand=="")
				var add_fl = true
			else
				var add_fl = false
			if ((this._fake)&&(!this._realfake))
				this._fake._collapseFake(row,i);
			kidsAr[kidsAr.length] = row.parentNode.removeChild(row);
			this.rowsCol._dhx_removeAt(i)
			if(add_fl)
				this.collapseKids(row,null,i)
			i--;
		}
	}

    if (this._ahgr) this.setSizes();



	this.loadedKidsHash.put(curRow.idd,kidsAr)
	if(arguments.length==1){
        if (this._epnFH) this._epnFH(curRow.idd,-1);
        }

	return kidsAr;
}


/**
*	@desc: change parent of row, correct kids collections
*	@param: r2 - moved row
*	@param: r1 - new parent row
*	@type: private
*/
        dhtmlXGridObject.prototype._changeParent=function(r2,r1){
            if (!r1){
               r2.parent_id=0;
                return;
            }
            if (r2.parent_id==r1.idd){
                var a=this.loadedKidsHash.get(r1.idd);
                var z=a._dhx_find(r2);
                a._dhx_removeAt(z);
                if (this._dhkPos){
                    if (this._dhkPos>z) this._dhkPos--;
                    a._dhx_insertAt(this._dhkPos,r2);
                }
                this.loadedKidsHash.put(r1.idd,a);
/*                for (var i=0; i<a.length; i++)
                    alert(a[i].idd);*/
                return;
            }
            var a=this.loadedKidsHash.get(r2.parent_id);
            var b=this.loadedKidsHash.get(r1.idd);
            if (!b) b=new dhtmlxArray();
            if (a){
                a._dhx_removeAt(a._dhx_find(r2));
                this.loadedKidsHash.put(r2.parent_id,a);
                }
            if (this._dhkPos){
                b._dhx_insertAt(this._dhkPos,r2);
                }
            else
                b._dhx_insertAt(0,r2);
            this.loadedKidsHash.put(r1.idd,b);

            r2.parent_id=r1.idd;
        }


/**
*	@desc: change parent of row, correct kids collections
*	@param: curRow - row to process
*	@type: private
*/
dhtmlXGridObject.prototype.expandKids=function(curRow){
    if ((curRow.has_kids==0)&&(!curRow._xml_await)) return;
    if (curRow.expand=="") return;
    if (!curRow._loading)
        if ((this._spnFH)&&(!this._spnFH(curRow.idd,-1))) return;


	var start = this.getRowIndex(curRow.idd)+1;
	var kidsAr = this.loadedKidsHash.get(curRow.idd);

	if(kidsAr!=null){
        curRow._loading=false;
    	curRow.expand = "";
		if (this._fake)
			this._fake._expandCorrect(curRow);
    	treeCell = curRow.childNodes[this.cellType._dhx_find("tree")];
	    treeCell.innerHTML = treeCell.innerHTML.replace(/\/(plus|blank)\.gif/,"/minus.gif");

		for(var i=0;i<kidsAr.length;i++){
			//alert(kidsAr[i].outerHTML)
			this._insertRowAt(kidsAr[i],start);
			/*
			var row = (!_isKHTML)?this.obj.childNodes[0].appendChild(kidsAr[i]):this.obj.appendChild(kidsAr[i]);
			if (this.obj._rows(start) && this.obj._rows(start)!=row )
                this.obj._rows(start).parentNode.insertBefore(row,this.obj._rows(start));
            //next line commented, because record already exists in  rowsCol array
			this.rowsCol._dhx_insertAt(start,row)
			*/
			start++;
		}
		if (this._cssEven)
			this._fixAlterCss(start-i<kidsAr.length);

	    if(this.fldSorted!=null){
    		this.sortTreeRows(this._ls_col, this._ls_type, this._ls_order);
        if (this._epnFH) this._epnFH(curRow.idd,1);
    	}
	}else{
        if (curRow._xml_await){
			if ((this._slowParse)&&(curRow._xml)){
				this._reParse(curRow);
				return this.expandKids(curRow);
			}
			if(this.kidsXmlFile.indexOf("?")!=-1)
				var s = "&";
			else
				var s = "?";
	        curRow._loading=true;
			this.loadXML(this.kidsXmlFile+""+s+"id="+curRow.idd);
        }
	}
    if ((this._ahgr)&&(!this._startXMLLoading)) this.setSizes();
    if (!curRow._loading)
    if (this._epnFH) this._epnFH(curRow.idd,1);
}

dhtmlXGridObject.prototype.kidsXmlFile = "";



/**
*	@desc: sorts treegrid by specified column
*	@param: col - column index
*	@param:	type - str.int.date
*	@param: order - asc.desc
*	@type: public
*   @edition: Professional
*	@topic: 2,3,5,9
*/
dhtmlXGridObject.prototype.sortTreeRows = function(col,type,order){
                    this._ls_col=col;
                    this._ls_type=type;
                    this._ls_order=order;

					var byParentHash = new Hashtable();
					for(var i=0;i<this.obj._rowslength();i++){
						if(byParentHash.get(this.obj._rows(i).parent_id)==null)
							byParentHash.put(this.obj._rows(i).parent_id,new dhtmlxArray());
					}
					for(var i=0;i<this.obj._rowslength();i++){
						var tmpAr = byParentHash.get(this.obj._rows(i).parent_id);
						tmpAr[tmpAr.length] = this.obj._rows(i).idd//i
						byParentHash.put(this.obj._rows(i).parent_id,tmpAr);
					}


					var keysAr = byParentHash.keys;
					for(var j=0;j<keysAr.length;j++){
						var tmpAr = byParentHash.get(keysAr[j]);
						var ar = new Array();
						for(var i=0;i<tmpAr.length;i++){
							ar[i] = new Array();

                            var a_val=this.cells(tmpAr[i],col).getValue().toString();
							if(type=='int'){
								if(a_val._dhx_trim()=="")
									ar[i][0] = -999999999
								else
									ar[i][0] = parseInt(a_val);
							}else
								ar[i][0] = a_val;

							ar[i][1] = this.getRowById(tmpAr[i])//obj.rows[tmpAr[i]];
                            if ((i!=0)&&(ar[i][0]!=ar[i-1][0])) ar._sort_me=true;
						}

                        // debugger;
                        if (ar._sort_me)
                        {
    					if(type=='str')
							ar.sort(function(a,b){return (a[0]<b[0]?-1:(a[0]==b[0]?0:1))})

						if(type=='int')
							ar.sort(function(a,b){return (a[0]<b[0]?-1:(a[0]==b[0]?0:1))})

						if(type=='date'){
							try{
								ar.sort(function(a,b){return Date.parse(new Date(a[0]))-Date.parse(new Date(b[0]))})
							}catch(er){
								alert('Incompatible date format.Sorted as string')
							    ar.sort(function(a,b){return (a[0]<b[0]?-1:(a[0]==b[0]?0:1))})
							}
						}
                        }


            //  debugger;

                if (!_isKHTML){
                    var z=this.getRowById(keysAr[j]);
                    if (z) z=z.rowIndex+1;
                    }
                else{
                    var z=-1;
                    for (var zkk=0; zkk<this.obj._rowslength(); zkk++)
                        if (this.obj._rows(zkk).idd==keysAr[j])
                            { z=zkk+1; break; }
                    }



                if ((z<1)||(!z)) 	var parentInd=0;
                else       	var parentInd=z-1;


				this._sortTreeRows(col,type,order,ar,parentInd);
					}
}

dhtmlXGridObject.prototype._sortTreeRows = function(col,type,order,ar,parentInd){
					/*	var s=new Array();
						for (var j=0; j<ar.length; j++)
							s[j]=ar[j][1].idd;

						alert(s.join(","));    */

		                if (!_isKHTML)
		                    var tb = this.obj.childNodes[0];
		                else
		                    var tb = this.obj._rows(0).parentNode;

						if ((order!='asc')&&(ar._sort_me))
							for(var i=0;i<ar.length;i++)                        {
                                if (tb.moveRow)
    								tb.moveRow(this.rowsAr[ar[i][1].idd].rowIndex,parentInd+1)
                                else
                                   if (this.obj._rows(parentInd))
                                       {
                                       if (this.rowsAr[ar[i][1].idd]!=this.obj._rows(parentInd))
                                       tb.insertBefore(this.rowsAr[ar[i][1].idd],this.obj._rows(parentInd));
                                       }
                                   else
                                       tb.appendChild(this.rowsAr[ar[i][1].idd]);                }

						else
						   	for(var i=ar.length-1;i>=0;i--)                      {
                                if (tb.moveRow)
        						  tb.moveRow(this.rowsAr[ar[i][1].idd].rowIndex,parentInd+1) ;
                                else
                                   if (this.obj._rows(parentInd))
                                       {
                                       if (this.rowsAr[ar[i][1].idd]!=this.obj._rows(parentInd))
                                       tb.insertBefore(this.rowsAr[ar[i][1].idd],this.obj._rows(parentInd));
                                       }
                                   else
                                       tb.appendChild(this.rowsAr[ar[i][1].idd]);                  }



                    this.rowsCol=new dhtmlxArray();
                    for (var i=0; i<this.obj._rowslength(); i++)
                        this.rowsCol[i]=this.obj._rows(i);
};

/**
*	@desc: correct kids collection after adding new row
*	@param: pRow - parent row
*	@type: private
*/
dhtmlXGridObject.prototype.has_kids_dec=function(pRow){
            if (!pRow) return;
            pRow.has_kids--;
            if (pRow.has_kids==0){
                    pRow.expand=null;
                	var treeCell = pRow.childNodes[this.cellType._dhx_find("tree")];
  		        	treeCell.innerHTML = treeCell.innerHTML.replace(/\/plus|minus\.gif/,"/blank.gif")
               }
			if ((this._fake)&&(this._fake._realfake))
				this._fake.has_kids_dec(this._fake.rowsAr[pRow.idd],treeCell);
      }

/**
*	@desc: correct kids collection after removing new row
*	@param: pRow - parent row
*	@param: treeCell - index of tree column
*	@type: private
*/
dhtmlXGridObject.prototype.has_kids_inc=function(pRow,treeCell){
                if (!pRow) return;
                if ((!pRow.has_kids)||(pRow.has_kids==0)){
                    pRow.has_kids=1;
                    pRow.expand="no";
  			        pRow.childNodes[treeCell].innerHTML = pRow.childNodes[treeCell].innerHTML.replace(/\/blank\.gif/,"/plus.gif")
                    }
                else{
                    pRow.has_kids++;
                    }
				if ((this._fake)&&(this._fake._realfake))
					this._fake.has_kids_inc(this._fake.rowsAr[pRow.idd],treeCell);
        }



/**
*	@desc: TreeGrid cell constructor
*	@param: cell - cell object
*	@type: private
*/
function eXcell_tree(cell){
	try{
		this.cell = cell;
		this.grid = this.cell.parentNode.grid;
	}catch(er){}
	this.edit = function(){
        if ((this.er)||(this.grid._edtc)) return;
        this.er=this.cell.childNodes[0];
        this.er=this.er.childNodes[this.er.childNodes.length-1];
        this.val=this.er.innerHTML;
        this.er.innerHTML="<textarea class='dhx_combo_edit' type='text' style='height:"+(this.cell.offsetHeight-6)+"px; width:100%; border:0px; margin:0px; padding:0px; padding-top:"+(_isFF?1:2)+"px; overflow:hidden; font-size:12px; font-family:Arial;'></textarea>";
        if (_isFF)         this.er.style.top="1px";
        this.er.className+=" editable";
        this.er.firstChild.onclick = function(e){(e||event).cancelBubble = true};
        this.er.firstChild.value=this.val;
        this.er.firstChild.focus();
    }
	this.detach = function(){
        if (!this.er) return;
            this.er.innerHTML=this.er.firstChild.value;
            this.er.className=this.er.className.replace("editable","");
            var z=(this.val==this.er.innerHMTL);
            if (_isFF) this.er.style.top="2px";
            this.er=null;
        return (z);
    }
	this.getValue = function(){
		//alert(this.cell)
		var kidsColl = this.cell.childNodes[0].childNodes;
		for(var i=0;i<kidsColl.length;i++){
			//alert("value: "+kidsColl[i].tagName)
			if(kidsColl[i].id=='nodeval')
				return kidsColl[i].innerHTML;
		}
	}
    /**
    *	@desc: set label of treegrid item
    *	@param: content - new text of label
    *	@type: private
    */
	this.setValueA = function(content){
		//alert(this.cell)
		var kidsColl = this.cell.childNodes[0].childNodes;
		for(var i=0;i<kidsColl.length;i++){
			//alert("value: "+kidsColl[i].tagName)
			if(kidsColl[i].id=='nodeval')
				kidsColl[i].innerHTML=content;
		}
    }
    /**
    *	@desc: get image of treegrid item
    *	@param: content - new text of label
    *	@type: private
    */
	this.setImage = function(url){
		var z=this.cell.childNodes[0];
        var img=z.childNodes[z.childNodes.length-2];
		this.cell._aimage=url;
		ul=this.grid.imgURL+"/"+url;
        img.src=url;

	}

    /**
    *	@desc: set image of treegrid item
    *	@param: content - new text of label
    *	@type: private
    */
	this.getImage = function(){
        return this.cell._aimage;
	}

	/**
	*	@desc: sets text representation of cell ( setLabel doesn't triger math calculations as setValue do)
	*	@param: val - new value
	*	@type: public
	*/
	this.setLabel = function(val){
						this.setValueA(val);
				}

    /**
    *	@desc: set value of grid item
    *	@param: val  - new value (for treegrid this method only used while adding new rows)
    *	@type: private
    */
	this.setValue = function(val){
					var valAr = val.split("^");//parent_id^Label^children^im0^im1^im2

					this.cell.parentNode.parent_id = valAr[0];

                    if ((!this.grid.kidsXmlFile)&&(!this.grid._slowParse)) valAr[2]=0;
                    else
                       this.cell.parentNode._xml_await=(valAr[2]!=0);

  					this.cell.parentNode.has_kids = valAr[2];

					//this.cell.parentNode._xml_wait = true;
					var pRow = this.grid.getRowById(valAr[0]);
					if(pRow==null){//check detached rows
						pRow = this.grid.loadedKidsHash.get("hashOfParents").get(valAr[0])
						//if(pRow!=null)
						//	alert(pRow+"::"+pRow.childNodes.length)
					}

					var preStr = "";//prenode html
					var node = "";//node html (two images and label)
					if(pRow!=null){//not zero level
						//alert(pRow.cells.length)

						var level = (pRow.childNodes[cell._cellIndex].firstChild.childNodes.length-1)-1
						for(var i=0;i<level;i++)
							preStr += "<span class='space'><img src='"+this.grid.imgURL+"/blanc.gif' height='1px' class='space'></span>"

                        this.grid.has_kids_inc(pRow,this.cell._cellIndex);

						//alert("aga")
						if(pRow.expand!=""){
							//this.cell.parentNode.style.display = "none"
							this.grid.doOnRowAdded = function(row){
										if(row.has_kids>0){
											var parentsHash = this.loadedKidsHash.get("hashOfParents")
											parentsHash.put(row.idd,row)
											this.loadedKidsHash.put("hashOfParents",parentsHash)
										}
										var kidsAr = this.loadedKidsHash.get(row.parent_id)
										if(kidsAr==null){
											var kidsAr = new dhtmlxArray(0)
										}

										kidsAr[kidsAr.length] = row.parentNode.removeChild(row);
                                        this.rowsCol._dhx_removeAt(this.rowsCol._dhx_find(row));
										this.loadedKidsHash.put(row.parent_id,kidsAr)
										row._fhd=true; //split
                               			this.doOnRowAdded=function(){};
							}
						}else{
							this.grid.doOnRowAdded = function(row){}
							pRow.childNodes[this.cell._cellIndex].innerHTML = pRow.childNodes[this.cell._cellIndex].innerHTML.replace(/\/plus\.gif/,"/minus.gif")
                            var kidsAr = this.grid.loadedKidsHash.get(pRow.idd)

                            if (this._dhkPos)
                                kidsAr_dhx_insertAt(this._dhkPos,this.cell.parentNode);
                            else
                                kidsAr[kidsAr.length] = this.cell.parentNode;

                            this.grid.loadedKidsHash.put(pRow.idd,kidsAr)
							this.doOnRowAdded=function(){};
						}

					}else{//zero level
						this.grid.doOnRowAdded = function(row){}
						preStr = "";
					}

					//if has children
				 	if(valAr[2]!="" && valAr[2]!=0)
						node+="<img src='"+this.grid.imgURL+"/plus.gif";
                    else
                        node+="<img src='"+this.grid.imgURL+"/blank.gif";
						node+="' align='absmiddle' onclick='this."+(_isKHTML?"":"parentNode.")+"parentNode.parentNode.parentNode.parentNode.grid.doExpand(this);event.cancelBubble=true;'>";
						//this.grid.collapseKids(this.cell.parentNode)

				   node+="<img src='"+this.grid.imgURL+"/"+valAr[3]+"' align='absmiddle' "+(this.grid._img_height?(" height=\""+this.grid._img_height+"\""):"")+(this.grid._img_width?(" width=\""+this.grid._img_width+"\""):"")+" >";
				   node+="<span "+(_isFF?"style='position:relative; top:2px;'":"")+"id='nodeval'>"+valAr[1]+"</span>"

                 /*   if (this.grid.multiLine)
    				    this.cell.innerHTML = "<div style=' overflow:hidden; '>"+preStr+""+node+"</div>";
                    else*/
                        this.cell.innerHTML = "<div style=' overflow:hidden; white-space : nowrap; height:"+(_isKHTML?18:20)+"px;'>"+preStr+""+node+"</div>";

                    this.cell._aimage=valAr[3];
                    if (_isKHTML) this.cell.vAlign="top";
                    this.cell.parentNode.has_kids=0;
				}
	
}
eXcell_tree.prototype = new eXcell;

    /**
    *	@desc: correct visual representation of tree grid item  - initialisation part
    *	@param: r2  - row object
    *	@type: private
    */
dhtmlXGridObject.prototype._fixLevel=function(r2){
     var pRow=this.getRowById(r2.parent_id);

     var trcol=this.cellType._dhx_find("tree");

     this.has_kids_inc(pRow,trcol);

     if (pRow){
     pRow.childNodes[trcol].innerHTML = pRow.childNodes[trcol].innerHTML.replace(/\/plus\.gif/,"/minus.gif")
     pRow.expand = "";
     }

     var preStr="";
     if (!pRow) var level=0;
     else
  	 var level = (pRow.childNodes[trcol].firstChild.childNodes.length-1)-1;
	 for(var i=0;i<level;i++)
		 preStr += "<span class='space'><img src='' height='1' class='space'></span>"


    this._fixLevel2(r2,preStr,trcol);
        };

    /**
    *	@desc: correct visual representation of tree grid item - recursive part
    *	@param: r2  - row object
    *	@param: preStr  - string with corrected html
    *	@param: trcol  - index of treeGrid column
    *	@type: private
    */
dhtmlXGridObject.prototype._fixLevel2=function(r2,preStr,trcol){

     var z=r2.childNodes[trcol].firstChild.innerHTML;
     z=preStr+z.replace(/<(SPAN)[^>]*(><IMG)[^>]*(><\/SPAN>)/gi,"");
     r2.childNodes[trcol].firstChild.innerHTML=z;

     var a=this.loadedKidsHash.get(r2.idd);
     if (a){
     for (var i=0; i<a.length; i++)
         this._fixLevel2(a[i],preStr+"<span class='space'><img src='' height='1' class='space'></span>",trcol);
    this.loadedKidsHash.put(r2.idd,a);
    }

        };

    /**
    *	@desc: remove row from treegrid
    *	@param: node  - row object
    *	@type: private
    */
dhtmlXGridObject.prototype._removeTrGrRow=function(node){
			var parent_id = node.parent_id
			this.collapseKids(node)
            //not sure about purpose of logic below,
            //but seems that is works for only one level of inheritance
			var tmpAr = this.loadedKidsHash.get(parent_id)
			if(tmpAr!=null)
				tmpAr._dhx_removeAt(tmpAr._dhx_find(node))

			this.loadedKidsHash.remove(node.idd)
            //still not enough - must delete all child nodes as well
            var noda=node.nextSibling;
            this._removeTrGrRowRec(node.idd);

            pRow=this.getRowById(parent_id);
            if (!pRow) return;
            this.has_kids_dec(pRow);

		}

    /**
    *	@desc: remove child rows from treegrid
    *	@param: id  - parent row id
    *	@type: private
    */
dhtmlXGridObject.prototype._removeTrGrRowRec=function(id,inner){
            //I think it relative slow
            //more than that, it based on
            var newa=new Array();
            for (var i=0; i<this.rowsCol.length; i++)
                if (id==this.rowsCol[i].parent_id)
                {
                    newa[newa.length]=this.rowsCol[i].idd;
                    this.rowsAr[this.rowsCol[i].idd] = null;
                    this.rowsCol._dhx_removeAt(i);
                    i--;
                }
            if (newa.length)
                for (var i=0; i<newa.length; newa++)
                    this._removeTrGrRowRec(newa[i],true);
}


/**
*   @desc:  count rows inside branch
*   @type:  private
*/
dhtmlXGridObject.prototype._countBranchLength=function(ind){
    if (!this.rowsCol[ind+1]) return 1;
    if (this.rowsCol[ind+1].parent_id!=this.rowsCol[ind].idd) return 1;
    var count=1; var i=1;
    while   ((this.rowsCol[ind+count])&&(this.rowsCol[ind+count].parent_id==this.rowsCol[ind].idd)){
        count+=this._countBranchLength(ind+count);
        }
    return count;
}

/**
*	@desc: expand row
*	@param: rowId - id of row
*	@type:  public
*   @edition: Professional
*   @topic: 7
*/
dhtmlXGridObject.prototype.openItem=function(rowId){
        var x=this.getRowById(rowId);
        if (!x) return;
        this._openItem(x);
}


dhtmlXGridObject.prototype._openItem=function(x){
        var y=this.getRowById(x.parent_id);
        if (y)
            if (y.expand!="") this._openItem(y);
        this.expandKids(x);
}



dhtmlXGridObject.prototype._addRowClassic=dhtmlXGridObject.prototype.addRow;

    /**
    *	@desc: add new row to treeGrid
    *	@param: new_id  - new row id
    *	@param: text  - array of row label
    *	@param: ind  - position of row (set to null, for using parentId)
    *	@param: parent_id  - id of parent row
    *	@param: img  - img url for new row
    *	@param: child - child flag [optional]
    *	@type: public
    *   @edition: Professional
    */
dhtmlXGridObject.prototype.addRow=function(new_id,text,ind,parent_id,img,child){
     var trcol=this.cellType._dhx_find("tree");
	 if (typeof(text)=="string") text=text.split(this.delim);
     var last_row=null;
     if ((trcol!=-1)&&((text[trcol]||"").toString().search(/\^/gi)==-1)){
        var def=text[trcol];
        var d=(parent_id||"0").toString().split(",");
        for (var i=0; i<d.length; i++){
            text[trcol]=d[i]+"^"+def+"^"+(child?1:0)+"^"+(img||"leaf.gif");
                if (d[i]!=0)
                if ((!ind)||(ind==0)){
                    ind=this.getRowIndex(d[i]);
					if (this.rowsCol[ind].expand=="") ind+=this._countBranchLength(ind);
                    }
              /*  var z=0;
                for (var i=0; i<this.rowsCol.length; i++)
                    z+="<div>"+this.rowsCol[i].idd+"</div>";
                document.getElementById("output_a").innerHTML=z;*/
            last_row=this._addRowClassic(new_id,text,((!parent_id)&&(!ind)&&(ind!="0"))?window.undefined:ind);
            }
        return last_row;
     }

     return this._addRowClassic(new_id,text,ind);

}


//#__pro_feature:21092006{

//#smart_parsing:21092006{
/**
*     @desc: enable/disable smart XML parsing mode (usefull for big, well structured XML)
*     @beforeInit: 1
*     @param: mode - 1 - on, 0 - off;
*     @type: public
*     @edition: Professional
*     @topic: 0
*/
dhtmlXGridObject.prototype.enableSmartXMLParsing=function(mode) { this._slowParse=convertStringToBoolean(mode); };


/**
*     @desc: search id in unparsed chunks and render its node
*     @param: id - id in question
*     @type: prlvate
*     @edition: Professional
*     @topic: 0
*/
dhtmlXGridObject.prototype._seekAndDeploy=function(id) {
	if ((id=="null")||(!id)) return;
//	debugger;
	var a;

	for (a in this.rowsAr)
		if (this.rowsAr[a]._xml){
			var res=this.xmlLoader.doXPath('//row[@id="'+id+'"]',this.rowsAr[a]._xml[0].parentNode);
			if (res.length){
				//detect back line of ids
				res=res[0];
				var line=new Array();
				while (!this.rowsAr[res.getAttribute("id")]){
					line[line.length]=res.getAttribute("id");
					res=res.parentNode;
				}
				line[line.length]=res.getAttribute("id");

				for (var i=line.length-1; i>0; i--){
					this._reParse(this.rowsAr[line[i]]);
					this._openItem(this.rowsAr[line[i]]);
					}
				for (var i=1; i<line.length; i++)
					this.collapseKids(this.rowsAr[line[i]]);

			   return this.getRowById(id);
			}
		}
};

/**
*     @desc: reparse branch
*     @param: row - parent row
*     @type: prlvate
*     @edition: Professional
*     @topic: 0
*/
dhtmlXGridObject.prototype._reParse=function(row){
				var row=this.rowsAr[row.idd];
			    var ind=this.rowsCol._dhx_find(row);
				ind+=this._countBranchLength(ind);
				this._innerParse(row._xml,ind,this.cellType._dhx_find("tree"),row.idd);
				row.has_kids=row._xml_await=row._xml=null;
}

//#}


    /**
    *	@desc: copy content between different rows
    *	@param: frRow  - source row object
    *	@param: from_row_id  - source row id
    *	@param: to_row_id  - target row id
    *	@type: private
    */
dhtmlXGridObject.prototype._copyTreeGridRowContent=function(frRow,from_row_id,to_row_id){
    var z=this.cellType._dhx_find("tree");
    for(i=0;i<frRow.cells.length;i++){
        if (i!=z)
    	    this.cells(to_row_id,i).setValue(this.cells(from_row_id,i).getValue())
        else
            this.cells(to_row_id,i).setValueA(this.cells(from_row_id,i).getValue())

    }
}

/**
*	@desc: collapse row
*	@param: rowId - id of row
*	@type:  public
*   @edition: Professional
*   @topic: 7
*/
dhtmlXGridObject.prototype.closeItem=function(rowId){
        var x=this.getRowById(rowId);
        if (!x) return;
        this.collapseKids(x);
}
/**
*	@desc: delete all childs of selected row
*	@param: rowId - id of row
*	@param: ind - child node index
*	@type:  public
*   @edition: Professional
*   @topic: 7
*/
dhtmlXGridObject.prototype.deleteChildItems=function(rowId){
        var w=this.getRowById(rowId);
        if (!w) return;
        this.expandKids(w);

        var x=this.loadedKidsHash.get(rowId);
        if (x)
        {
        var z=x.length;
        for (var i=0; i<z; i++){
            this.deleteRow("",x[0]);
            }
        }
}
/**
*	@desc: get list of id of all nested rows
*	@param: rowId - id of row
*	@type:  public
*	@returns: list of id of all nested rows
*   @edition: Professional
*   @topic: 7
*/
dhtmlXGridObject.prototype.getAllSubItems=function(rowId){
        var str="";
        var x=this.loadedKidsHash.get(rowId);
        if (x)
        for (var i=0; i<x.length; i++){
            if (str) str+=",";
            str+=x[i].idd;
            var z=this.getAllSubItems(x[i].idd);
            if ((z)&&(str)) str+=",";
            str+=z;
            }

        return str;
}

/**
*	@desc: get id of child item at specified position
*	@param: rowId - id of row
*	@param: ind - child node index
*	@type:  public
*	@returns: id of child item at specified position
*   @edition: Professional
*   @topic: 7
*/
dhtmlXGridObject.prototype.getChildItemIdByIndex=function(rowId,ind){
        var x=this.loadedKidsHash.get(rowId);
        if (!x) return null;
        return (x[ind]?x[ind].idd:null);
}

/**
*	@desc: get real caption of tree col
*	@param: rowId - id of row
*	@type:  public
*   @edition: Professional
*	@returns: real caption of tree col
*   @topic: 7
*/
dhtmlXGridObject.prototype.getItemText=function(rowId){
        var x=this.getRowById(rowId);
        if (!x) return;
        var col=this.cellType._dhx_find("tree");
		var c = x.childNodes[col];
        var z=eval("new eXcell_"+this.cellType[col]+"(c)");
        return z.getValue();
}
/**
*	@desc: return level of treeGrid row
*	@param: rowId - id of row
*	@type:  public
*   @edition: Professional
*	@returns: level of treeGrid row
*   @topic: 7
*/
dhtmlXGridObject.prototype.getLevel=function(rowId){
        var x=this.getRowById(rowId);
        if (!x) return;
        var level=0;
        while ((x)&&(x.parent_id)){
            x=this.getRowById(x.parent_id);
            level++;
        }
        return level;
}
/**
*	@desc: return open/close state of row
*	@param: rowId - id of row
*	@type:  public
*	@returns: open/close state of row
*   @edition: Professional
*   @topic: 7
*/
dhtmlXGridObject.prototype.getOpenState=function(rowId){
        var x=this.getRowById(rowId);
        if (!x) return;
        if (x.expand=="") return true;
        return false;
}
/**
*	@desc: return id of parent row
*	@param: rowId - id of row
*	@type:  public
*   @edition: Professional
*	@returns: id of parent row
*   @topic: 7
*/
dhtmlXGridObject.prototype.getParentId=function(rowId){
        var x=this.getRowById(rowId);
        if (!x) return;
        return x.parent_id;
}
/**
*	@desc: return list of child row id, sparated by comma
*	@param: rowId - id of row
*	@type:  public
*   @edition: Professional
*	@returns: list of child rows
*   @topic: 7
*/
dhtmlXGridObject.prototype.getSubItems=function(rowId){
        var str="";
        var x=this.loadedKidsHash.get(rowId);
        if (x)
        for (var i=0; i<x.length; i++){
            if (str) str+=",";
            str+=x[i].idd;
            }
        return str;
}
/**
*	@desc: return children count
*	@param: rowId - id of row
*	@type:  public
*   @edition: Professional
*	@returns: children count
*   @topic: 7
*/
dhtmlXGridObject.prototype.hasChildren=function(rowId){
        var x=this.loadedKidsHash.get(rowId);
        if (x)
            return x.length;
        if (this.getRowById(rowId)._xml_await) return -1;
        return 0;
}


/**
*	@desc: enable/disable closing of row
*	@param: rowId - id of row
*	@param: status - true/false
*	@type:  public
*   @edition: Professional
*   @topic: 7
*/

dhtmlXGridObject.prototype.setItemCloseable=function(rowId,status){
        var x=this.getRowById(rowId);
        if (!x) return;
        x._closeable=(!convertStringToBoolean(status));
}
/**
*	@desc: set real caption of tree col
*	@param: rowId - id of row
*	@param: newtext - new text
*	@type:  public
*   @edition: Professional
*   @topic: 7
*/
dhtmlXGridObject.prototype.setItemText=function(rowId,newtext){
        var x=this.getRowById(rowId);
        if (!x) return;
        var col=this.cellType._dhx_find("tree");
		var c = x.childNodes[col];
        var z=eval("new eXcell_"+this.cellType[col]+"(c)");
        z.setValueA(newtext);
}

/**
*	@desc: set real caption of tree col
*	@param: rowId - id of row
*	@param: newtext - new text
*	@type:  public
*   @edition: Professional
*   @topic: 7
*/
dhtmlXGridObject.prototype.setItemText=function(rowId,newtext){
        var x=this.getRowById(rowId);
        if (!x) return;
        var col=this.cellType._dhx_find("tree");
		var c = x.childNodes[col];
        var z=eval("new eXcell_"+this.cellType[col]+"(c)");
        z.setValueA(newtext);
}

/**
*	@desc: set real caption of tree col
*	@param: rowId - id of row
*	@param: newtext - new text
*	@type:  public
*   @edition: Professional
*   @topic: 7
*/
dhtmlXGridObject.prototype.setItemText=function(rowId,newtext){
        var x=this.getRowById(rowId);
        if (!x) return;
        var col=this.cellType._dhx_find("tree");
		var c = x.childNodes[col];
        var z=eval("new eXcell_"+this.cellType[col]+"(c)");
        z.setValueA(newtext);
}

/**
*	@desc: set image of tree col
*	@param: rowId - id of row
*	@param: url - image url
*	@type:  public
*   @edition: Professional
*   @topic: 7
*/
dhtmlXGridObject.prototype.setItemImage=function(rowId,url){
        var x=this.getRowById(rowId);
        if (!x) return;
        var col=this.cellType._dhx_find("tree");
		var z = this.cells3(x,col);
        z.setImage(url);
}

/**
*	@desc: get image of tree col
*	@param: rowId - id of row
*	@type:  public
*   @edition: Professional
*   @topic: 7
*/
dhtmlXGridObject.prototype.getItemImage=function(rowId){
        var x=this.getRowById(rowId);
        if (!x) return;
        var col=this.cellType._dhx_find("tree");
		if (col==-1) return "";
		var z = this.cells3(x,col);
        return z.getImage();
}


/**
*	@desc: set size of treegrid images
*	@param: width -  width of image
*	@param: height - height of image
*	@type:  public
*   @edition: Professional
*   @topic: 7
*/
dhtmlXGridObject.prototype.setImageSize=function(width,height){
        this._img_width=width;
        this._img_height=height;
}


dhtmlXGridObject.prototype._getRowImage=function(row){
    return row.childNodes[this.cellType._dhx_find("tree")]._aimage;
        }


/**
*     @desc: set function called before tree node opened/closed
*     @param: func - event handling function
*     @type: public
*     @topic: 0,10
*     @event:  onOpenStart
*     @eventdesc: Event raised immideatly after item in tree got command to open/close , and before item was opened//closed. Event also raised for unclosable nodes and nodes without open/close functionality - in that case result of function will be ignored.
				Event not raised if node opened by dhtmlXtree API.
*     @eventparam: ID of node which will be opened/closed
*     @eventparam: Current open state of tree item. -1 - item closed, 1 - item opened.
*     @eventreturn: true - confirm opening/closing; false - deny opening/closing;
*/
	dhtmlXGridObject.prototype.setOnOpenStartHandler=function(func){  if (typeof(func)=="function") this._spnFH=func; else this._spnFH=eval(func);  };

/**
*     @desc: set function called after tree node opened/closed
*     @param: func - event handling function
*     @type: public
*     @topic: 0,10
*     @event:  onOpenEnd
*     @eventdesc: Event raised immideatly after item in tree got command to open/close , and before item was opened//closed. Event also raised for unclosable nodes and nodes without open/close functionality - in that case result of function will be ignored.
				Event not raised if node opened by dhtmlXtree API.
*     @eventparam: ID of node which will be opened/closed
*     @eventparam: Current open state of tree item. -1 - item closed, 1 - item opened.
*/
	dhtmlXGridObject.prototype.setOnOpenEndHandler=function(func){  if (typeof(func)=="function") this._epnFH=func; else this._epnFH=eval(func);  };


/**
*     @desc: set function called after tree node opened/closed
*     @param: func - event handling function
*     @type: public
*     @topic: 0,10
*     @event:  onOpenEnd
*     @eventdesc: Event raised immideatly after item in tree got command to open/close , and before item was opened//closed. Event also raised for unclosable nodes and nodes without open/close functionality - in that case result of function will be ignored.
				Event not raised if node opened by dhtmlXtree API.
*     @eventparam: ID of node which will be opened/closed
*     @eventparam: Current open state of tree item. -1 - item closed, 1 - item opened.
*/
	dhtmlXGridObject.prototype.setOnOpenEndHandler=function(func){  if (typeof(func)=="function") this._epnFH=func; else this._epnFH=eval(func);  };

    /**
*     @desc: enable/disable editor of tree cell ; enabled by default
*     @param: mode -  (boolean) true/false
*     @type: public
*     @topic: 0
*/
	dhtmlXGridObject.prototype.enableTreeCellEdit=function(mode){
        this._edtc=!convertStringToBoolean(mode);
    };


//#}

