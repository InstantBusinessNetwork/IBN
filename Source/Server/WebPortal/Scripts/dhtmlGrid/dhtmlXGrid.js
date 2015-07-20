/*
Copyright Scand LLC http://www.scbr.com
To use this component please contact info@scbr.com to obtain license
$CVSHeader$
*/

/*_TOPICS_
@0:initialization
@1:selection control
@2:rows control
@3:colums control
@4:cells controll
@5:data manipulation
@6:appearence control
@7:overal control
@8:tools
@9:treegrid
@10: event handlers
@11: paginal output
*/

var globalActiveDHTMLGridObject;
String.prototype._dhx_trim = function(){
                     return this.replace(/&nbsp;/g," ").replace(/(^[ \t]*)|([ \t]*$)/g,"");
                  }


function dhtmlxArray(ar){ return dhtmlXHeir((ar||new Array()),new _dhtmlxArray()); };
function _dhtmlxArray(){ return this; };
_dhtmlxArray.prototype._dhx_find = function(pattern){
   for(var i=0;i<this.length;i++){
      if(pattern==this[i])
            return i;
   }
   return -1;
}
_dhtmlxArray.prototype._dhx_delAt = function(ind){
   if(Number(ind)<0 || this.length==0)
      return false;
   for(var i=ind;i<this.length;i++){
      this[i]=this[i+1];
   }
   this.length--;
}
_dhtmlxArray.prototype._dhx_insertAt = function(ind,value){
   this[this.length] = null;
   for(var i=this.length-1;i>=ind;i--){
      this[i] = this[i-1]
   }
   this[ind] = value
}
_dhtmlxArray.prototype._dhx_removeAt = function(ind){
   for(var i=ind;i<this.length;i++){
      this[i] = this[i+1]
   }
   this.length--;
}
_dhtmlxArray.prototype._dhx_swapItems = function(ind1,ind2){
   var tmp = this[ind1];
   this[ind1] = this[ind2]
   this[ind2] = tmp;
}

/**
*   @desc: dhtmlxGrid constructor
*   @param: id - (optional) id of div element to base grid on
*   @returns: dhtmlxGrid object
*   @type: public
*/
function dhtmlXGridObject(id){
	if (_isIE) try { document.execCommand("BackgroundImageCache", false, true); } catch (e){}
   if(id){
      if(typeof(id)=='object'){
         this.entBox = id
         this.entBox.id = "cgrid2_"+(new Date()).getTime();
      }else
         this.entBox = document.getElementById(id);
   }else{
      this.entBox = document.createElement("DIV");
      this.entBox.id = "cgrid2_"+(new Date()).getTime();
   }



    this._tttag=this._tttag||"rows";
    this._cttag=this._cttag||"cell";
    this._rttag=this._rttag||"row";

   var self = this;

    this._wcorr=0;
   this.nm = this.entBox.nm || "grid";
   this.cell = null;
   this.row = null;
   this.editor=null;
    this._f2kE=true; this._dclE=true;
   this.combos=new Array(0);
    this.defVal=new Array(0);
   this.rowsAr = new Array(0);//array of rows by idd
   this.rowsCol = new dhtmlxArray(0);//array of rows by index
    //this.hiddenRowsAr = new Array(0);//nb added for paging
   this._maskArr=new Array(0);
   this.selectedRows = new dhtmlxArray(0);//selected rows array
   this.rowsBuffer = new Array(new dhtmlxArray(0),new dhtmlxArray(0));//buffer of rows loaded, but not rendered (array of ids, array of cell values arrays)
   this.loadedKidsHash = null;//not null if there is tree cell in grid
   this.UserData = new Array(0)//array of rows (and for grid - "gridglobaluserdata") user data elements

/*MAIN OBJECTS*/

   this.styleSheet = document.styleSheets;
      this.entBox.className = "gridbox";
       this.entBox.style.width = this.entBox.getAttribute("width") ||   (window.getComputedStyle?window.getComputedStyle(this.entBox,null)["width"]:(this.entBox.currentStyle?this.entBox.currentStyle["width"]:0)) || "100%";
       this.entBox.style.height = this.entBox.getAttribute("height") || (window.getComputedStyle?window.getComputedStyle(this.entBox,null)["height"]:(this.entBox.currentStyle?this.entBox.currentStyle["height"]:0)) || "100%";
      //cursor and text selection
      this.entBox.style.cursor = 'default';
        this.entBox.onselectstart = function(){return false};//avoid text select
   this.obj = document.createElement("TABLE");
      this.obj.cellSpacing = 0;
      this.obj.cellPadding = 0;
      this.obj.style.width = "100%";//nb:
      this.obj.style.tableLayout = "fixed";
      this.obj.className = "obj";

        this.obj._rows=function(i){ return this.rows[i+1]; }
        this.obj._rowslength=function(){ return this.rows.length-1; }

   this.hdr = document.createElement("TABLE");
        this.hdr.style.border="1px solid gray";  //FF 1.0 fix
      this.hdr.cellSpacing = 0;
      this.hdr.cellPadding = 0;
        if ((!_isOpera)||(_OperaRv>=9))
             this.hdr.style.tableLayout = "fixed";
      this.hdr.className = "hdr";
      this.hdr.width = "100%";

   this.xHdr = document.createElement("TABLE");
      this.xHdr.cellPadding = 0;
      this.xHdr.cellSpacing = 0;
      var r = this.xHdr.insertRow(0)
      var c = r.insertCell(0);
         r.insertCell(1).innerHTML = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
         c.appendChild(this.hdr)
   this.objBuf = document.createElement("DIV");
      this.objBuf.appendChild(this.obj);
   this.entCnt = document.createElement("TABLE");
      this.entCnt.insertRow(0).insertCell(0)
      this.entCnt.insertRow(1).insertCell(0);



      this.entCnt.cellPadding = 0;
      this.entCnt.cellSpacing = 0;
      this.entCnt.width = "100%";
      this.entCnt.height = "100%";

          this.entCnt.style.tableLayout = "fixed";

   this.objBox = document.createElement("DIV");
      this.objBox.style.width = "100%";
      this.objBox.style.height = this.entBox.style.height;
      this.objBox.style.overflow = "auto";
      this.objBox.style.position = "relative";
      this.objBox.appendChild(this.objBuf);
      this.objBox.className = "objbox";
      this.objBox.id = "finance-main-div";


   this.hdrBox = document.createElement("DIV");
      this.hdrBox.style.width = "100%"
        if (((_isOpera)&&(_OperaRv<9)) )
            this.hdrSizeA=25; else this.hdrSizeA=100;



           this.hdrBox.style.height=this.hdrSizeA+"px";
       if (_isIE)
            this.hdrBox.style.overflowX="hidden";
      else
          this.hdrBox.style.overflow = "hidden";
      

      this.hdrBox.style.position = "relative";
      this.hdrBox.appendChild(this.xHdr);

      this.xHdr.style.width = parseInt(this.obj.style.width) + 'px';



   this.preloadImagesAr = new Array(0)

   this.sortImg = document.createElement("IMG")
   this.sortImg.style.display = "none";
   this.hdrBox.insertBefore(this.sortImg,this.xHdr)
    this.entCnt.rows[0].cells[0].vAlign="top";
   this.entCnt.rows[0].cells[0].appendChild(this.hdrBox);
   this.entCnt.rows[1].cells[0].appendChild(this.objBox);


   this.entBox.appendChild(this.entCnt);
   //add links to current object
   this.entBox.grid = this;
   this.objBox.grid = this;
   this.hdrBox.grid = this;
   this.obj.grid = this;
   this.hdr.grid = this;
/*PROPERTIES*/
   this.cellWidthPX = new Array(0);//current width in pixels
   this.cellWidthPC = new Array(0);//width in % if cellWidthType set in pc
   this.cellWidthType = this.entBox.cellwidthtype || "px";//px or %

   this.delim = this.entBox.delimiter || ",";
   this._csvDelim = ",";

   this.hdrLabels = (this.entBox.hdrlabels || "").split(",");
   this.columnIds = (this.entBox.columnids || "").split(",");
   this.columnColor = (this.entBox.columncolor || "").split(",");
   this.cellType =  dhtmlxArray((this.entBox.cellstype || "").split(","));
   this.cellAlign =  (this.entBox.cellsalign || "").split(",");
   this.initCellWidth = (this.entBox.cellswidth || "").split(",");
   this.fldSort = (this.entBox.fieldstosort || "").split(",")
   this.imgURL = this.entBox.imagesurl || "gridCfx/";
   this.isActive = false;//fl to indicate if grid is in work now
   this.isEditable = true;
   this.raNoState = this.entBox.ranostate || null;
   this.chNoState = this.entBox.chnostate || null;
   this.selBasedOn = (this.entBox.selbasedon || "cell").toLowerCase()
   this.selMultiRows = this.entBox.selmultirows || false;
   this.multiLine = this.entBox.multiline || false;
   this.noHeader = this.entBox.noheader || false;
   this.dynScroll = this.entBox.dynscroll || false;//add/remove rows from dom
   this.dynScrollPageSize = 0;//will be autodetected
   this.dynScrollPos = 0;//position of dynamic scroll
   this.xmlFileUrl = this.entBox.xmlfileurl || "";
   this.recordsNoMore = this.entBox.infinitloading || true;;//if true, then it will not attempt to fill the buffer from server
   this.useImagesInHeader = false;//use images in header or not
   this.pagingOn = false;//paging on/off
   //this.dynScrollBuffer = 100;//number of rows in buffer
   this.rowsBufferOutSize = 0;//number of rows rendered at a moment
/*EVENTS*/
   this.onCheckbox=this.onEnter=this.onRowSelect=this.onEditCell = function(){return true;};
   dhtmlxEvent(window,"unload",function(){try{ self.destructor(); } catch(e){}});

/*XML LOADER(S)*/
   /**
   *   @desc: loads xml content from specified url
   *   @param: url - XML file url
   *   @param: afterCall - function which will be called after xml loading
   *   @type: public
   *   @topic: 0,5
   */
   this.loadXML = function(url,afterCall){
        if (this._dload) { this._dload=url; this._askRealRows(); return true; };
        if (this._xmlaR) this.setXMLAutoLoading(url);


      if(url.indexOf("?")!=-1)
         var s = "&";
      else
         var s = "?";
      var obj = this;
       if (this.onXLS) this.onXLS(this);

      if (afterCall) this.xmlLoader.waitCall=afterCall;
      this.xmlLoader.loadXML(url+""+s+"rowsLoaded="+this.getRowsNum()+"&lastid="+this.getRowId(this.getRowsNum()-1)+"&sn="+Date.parse(new Date()));
       //nb:
      //if (this.onXLS) setTimeout(function(){obj.onXLS(obj)},0);
      //setTimeout(function(){obj.xmlLoader.loadXML(url+""+s+"rowsLoaded="+obj.getRowsNum()+"&lastid="+obj.getRowId(obj.getRowsNum()-1)+"&sn="+Date.parse(new Date()));},1)
   }

   /**
   *   @desc: set one of predefined css styles (currently : xp)
   *   @param: name - style name
   *   @type: public
   *   @topic: 0,6
   */
   this.setSkin = function(name){
      this.entBox.className = "gridbox gridbox_"+name;
      switch(name){
         case "xp": this._srdh=22; break;
		 case "gray": this._borderFix=(_isIE?1:0); break;
      }
   }

//#__pro_feature:21092006{
//#loadfrom_string:21092006{
   /**
   *   @desc: loads xml content from specified string
   *   @param: str - XML string
   *   @param: afterCall - function which will be called after xml loading
   *   @type: public
   *   @topic: 0,5
   *   @edition: Professional
   */
   this.loadXMLString = function(str,afterCall){
        if (this.onXLS) this.onXLS(this);

      if (afterCall) this.xmlLoader.waitCall=afterCall;
      this.xmlLoader.loadXMLString(str);
   }
//#}
//#}
   /**
   *   @desc: puts xml to parser
   *   @type: private
   *   @topic: 0,5
   */
   this.doLoadDetails = function(obj){
      var root = self.xmlLoader.getXMLTopNode(self._tttag)
        if (root.tagName!="DIV")
      if(!self.xmlLoader.xmlDoc.nodeName){
         self.parseXML(self.xmlLoader.xmlDoc.responseXML)
      }else{
         self.parseXML(self.xmlLoader.xmlDoc)
        }
      //nb:paging
      if(self.pagingOn)
         self.createPagingBlock()
   }
   this.xmlLoader = new dtmlXMLLoaderObject(this.doLoadDetails,window,true,this.no_cashe);
   if (_isIE) this.preventIECashing(true);
   this.dragger=new dhtmlDragAndDropObject();

/*METHODS. SERVICE*/
      /**
      *   @desc: on scroll grid inner actions
      *   @type: private
      *   @topic: 7
      */
      this._doOnScroll = function(e,mode){
                        if (this._onSCRL) this._onSCRL(this.objBox.scrollLeft,this.objBox.scrollTop);
                        this.doOnScroll(e,mode);
               }
      /**
      *   @desc: on scroll grid more inner action
      *   @type: private
      *   @topic: 7
      */
      this.doOnScroll = function(e,mode){
                  this.hdrBox.scrollLeft = this.objBox.scrollLeft;
				  if (this.ftr)
				  	this.ftr.parentNode.scrollLeft = this.objBox.scrollLeft;
                  this.setSortImgPos(null,true);
                        if (mode) return;
                  
                  //load more rows on scroll
                  if(!this.pagingOn && this.objBox.scrollTop+this.hdrSizeA+this.objBox.offsetHeight>this.objBox.scrollHeight){
                     if(this._xml_ready && (this.objBox._oldScrollTop!=this.objBox.scrollTop) && this.addRowsFromBuffer()){
                        this.objBox.scrollTop = this.objBox.scrollHeight - (this.hdrSizeA+1+this.objBox.offsetHeight)
						this.objBox._oldScrollTop=this.objBox.scrollTop;
						}
                  }

                        if (this._dload){
                        if (this._dLoadTimer)  window.clearTimeout(this._dLoadTimer);
                        this._dLoadTimer=window.setTimeout(function(){ self._askRealRows(); },500);
                        }

               }
      /**
      *    @desc: attach this grid to some object in DOM
      *    @param: obj - object to attach to
      *   @type: public
      *   @topic: 0,7
      */
      this.attachToObject = function(obj){
                        obj.appendChild(this.entBox)

                     }
      /**
      *   @desc: initialize grid
      *   @param: fl - if to parse on page xml dataisland
      *   @type: public
      *   @topic: 0,7
      */
      this.init =    function(fl){

                     this.editStop()
                     /*TEMPORARY STATES*/
                     this.lastClicked = null;//row clicked without shift key. used in multiselect only
                     this.resized = null;//hdr cell that is resized now
                     this.fldSorted = null;//hdr cell last sorted
                     this.gridWidth = 0;
                     this.gridHeight = 0;
                     //empty grid if it already was initialized
                     this.cellWidthPX = new Array(0);
                     this.cellWidthPC = new Array(0);
                     if(this.hdr.rows.length>0){
                        this.clearAll(true);
                     }
                     if(this.cellType._dhx_find("tree")!=-1){//create hashtable for treegrid
                        this.loadedKidsHash = new Hashtable();
                        this.loadedKidsHash.put("hashOfParents",new Hashtable())
                     }

                     var hdrRow = this.hdr.insertRow(0);
                            for(var i=0;i<this.hdrLabels.length;i++){
                                hdrRow.appendChild(document.createElement("TH"));
                                hdrRow.childNodes[i]._cellIndex=i;
                                }
                            if (_isIE) hdrRow.style.position="absolute";
                            else hdrRow.style.height='auto';
                     var hdrRow = this.hdr.insertRow(_isKHTML?2:1);

                            hdrRow._childIndexes=new Array();
                            var col_ex=0;
                     for(var i=0;i<this.hdrLabels.length;i++){
                                hdrRow._childIndexes[i]=i-col_ex;

                            if ((this.hdrLabels[i]==this.splitSign)&&(i!=0)){
								if (_isKHTML)
									hdrRow.insertCell(i-col_ex);
                                hdrRow.cells[i-col_ex-1].colSpan=(hdrRow.cells[i-col_ex-1].colSpan||1)+1;
                                hdrRow.childNodes[i-col_ex-1]._cellIndex++;
                                col_ex++;
                                hdrRow._childIndexes[i]=i-col_ex;
                                continue;
                                }

							hdrRow.insertCell(i-col_ex);

                            hdrRow.childNodes[i-col_ex]._cellIndex=i;
                            hdrRow.childNodes[i-col_ex]._cellIndexS=i;
                        	this.setHeaderCol(i,this.hdrLabels[i]);
                        }
						if (col_ex==0) hdrRow._childIndexes=null;
						this._cCount=this.hdrLabels.length;

						if (_isIE) window.setTimeout(function(){ self.setSizes(); },1);

//create virtual top row
                                if (!this.obj.firstChild)
                                    this.obj.appendChild(document.createElement("TBODY"));

                                var tar=this.obj.firstChild;
								if (!tar.firstChild){
                                    tar.appendChild(document.createElement("TR"));
                                    tar=tar.firstChild;
                                    if (_isIE) tar.style.position="absolute";
                                    else tar.style.height='auto';

	                                for(var i=0;i<this.hdrLabels.length;i++)
    	                                tar.appendChild(document.createElement("TH"));
								}


                     this.setColumnIds()
                     if(this.multiLine==-1)
                        this.multiLine = true;
                     if(this.multiLine != true)
                        this.obj.className+=" row20px";
                        
                     //DV
                     this.obj.id = "finance-main-table"; 

                     //
                     //this.combos = new Array(this.hdrLabels.length);
                     //set sort image to initial state
                     this.sortImg.style.position = "absolute";
                     this.sortImg.style.display = "none";
                     this.sortImg.src = this.imgURL+"sort_desc.gif";
                     this.sortImg.defLeft = 0;
                     //create and kill a row to set initial size
                     //this.addRow("deletethisrtowafterresize",new Array("",""))
                     this.entCnt.rows[0].style.display = ''//display header
                     if(this.noHeader){
                        this.entCnt.rows[0].style.display = 'none';
                     }else{
                        this.noHeader = false
                     }

//#__pro_feature:21092006{
//#column_hidden:21092006{
                 if (this._ivizcol)   this.setColHidden();
//#}
//#}


                     this.setSizes();

					 this.attachHeader();
					 this.attachHeader(0,0,"_aFoot");

                     if(fl)
                        this.parseXML()
                     this.obj.scrollTop = 0

                            if (this.dragAndDropOff)  this.dragger.addDragLanding(this.entBox,this);
                            if (this._initDrF) this._initD();

                  };
      /**
      *    @desc: sets sizes of grid elements
      *   @type: private
      *   @topic: 0,7
      */
      this.setSizes  =    function(fl){
                     if ((!this.noHeader)&&((!this.hdr.rows[0])||(!this.hdrBox.offsetWidth))) return;
                     if(fl && this.gridWidth==this.entBox.offsetWidth && this.gridHeight==this.entBox.offsetHeight){
                        return false
                     }else if(fl){
                        this.gridWidth = this.entBox.offsetWidth
                        this.gridHeight = this.entBox.offsetHeight
                     }


                            if ((!this.hdrBox.offsetHeight)&&(this.hdrBox.offsetHeight>0))
                         this.entCnt.rows[0].cells[0].height = this.hdrBox.offsetHeight+"px";

                     var gridWidth = parseInt(this.entBox.offsetWidth);
                     var gridHeight = parseInt(this.entBox.offsetHeight);



                     var _isVSroll=(this.objBox.scrollHeight>this.objBox.offsetHeight);
                     if (((!this._ahgr)&&(_isVSroll))||((this._ahgrM)&&(this._ahgrM<this.objBox.scrollHeight)))
                                gridWidth-=(this._scrFix||(_isFF?19:16));



                     var len = this.hdr.rows[0].cells.length
                     
                     for(var i=0;i<this._cCount;i++){
                        if(this.cellWidthType=='px' && this.cellWidthPX.length < len){
                           this.cellWidthPX[i] = this.initCellWidth[i] - this._wcorr;
                           
                        }else if(this.cellWidthType=='%' && this.cellWidthPC.length < len){
                           this.cellWidthPC[i] = this.initCellWidth[i];
                        }
//                        if(this.cellWidthPC.length!=0){
//                           this.cellWidthPX[i] = parseInt(gridWidth*this.cellWidthPC[i]/100);
//                        }
                     }

                    var wcor=this.entBox.offsetWidth-this.entBox.clientWidth;

                     var summ = 0;
					 var fcols=new Array();

                     for(var i=0;i<this._cCount;i++)
					    if (this.initCellWidth[i]=="*")
                        	fcols[fcols.length]=i;
						else
	                        summ += parseInt(this.cellWidthPX[i]);
					 if (fcols.length){
					 	var ms=Math.floor((gridWidth-summ-1-wcor)/fcols.length);
						if (ms<0) ms=1;
					 	for(var i=0;i<fcols.length; i++){
                        	this.cellWidthPX[fcols[i]]=ms-this._wcorr;
							summ+=ms;
							}
					 }


                this.chngCellWidth();



                     var summ = 0;
                     for(var i=0;i<this._cCount;i++)
                        summ += parseInt(this.cellWidthPX[i])
                   if (_isOpera) summ-=1;

                       this.objBuf.style.width = summ + "px";
                       this.objBuf.childNodes[0].style.width = summ + "px";
                            //if (_isOpera) this.hdr.style.width = summ + this.cellWidthPX.length*2 + "px";
                     //set auto page size of dyn scroll
                     this.doOnScroll(0,1);

                     //set header part of container visible height to header's height
                     //this.entCnt.rows[0].cells[0].style.height = this.hdr.offsetHeight;

                                 this.hdr.style.border="0px solid gray";  //FF 1.0 fix
/*                         if ((_isMacOS)&&(_isFF))
                                    var zheight=20;
                                 else*/
                                    var zheight=this.hdr.offsetHeight+(this._borderFix?this._borderFix:0);

                                if (this._ahgr)
                                    if (this.objBox.scrollHeight){
                                        if (_isIE)
                                            var z2=this.objBox.scrollHeight;
                                        else
                                            var z2=this.objBox.childNodes[0].scrollHeight;
                                    	var scrfix=((this.objBox.offsetWidth<this.objBox.scrollWidth)?(_isFF?20:18):1);
										if (this._ahgrMA)
											z2=this.entBox.parentNode.offsetHeight-zheight-scrfix;

                                        if (this._ahgrM)
                                            z2=(z2>this._ahgrM?this._ahgrM:z2)*1;

                                        gridHeight=z2+zheight+scrfix;
                                        this.entBox.style.height=gridHeight+"px";
                                  }

					 var aRow=this.entCnt.rows[1].cells[0].childNodes[0];
                     if(!this.noHeader)
	  				 	aRow.style.top = (zheight-this.hdrBox.offsetHeight+(_isFF?0:(1-wcor)))+"px";

                     //nb 072006:
                     aRow.style.height = (((gridHeight - zheight-1)<0 && _isIE)?20:(gridHeight - zheight-1))-(this.ftr?this.ftr.offsetHeight:0)+"px";
					 if (this.ftr) this.entCnt.style.height=this.entBox.offsetHeight-this.ftr.offsetHeight+"px";

                            if (this._dload)
                                this._dloadSize=Math.floor(parseInt(this.entBox.style.height)/20)+2; //rough, but will work

                  };

      /**
      *   @desc: changes cell width
      *   @param: [ind] - index of row in grid
      *   @type: private
      *   @topic: 4,7
      */
      this.chngCellWidth = function(){
	  					   if ((_isOpera)&&(this.ftr))
						   		this.ftr.width=this.objBox.scrollWidth+"px";
						   var l=this._cCount;
                           for(var i=0;i<l;i++){
                              this.hdr.rows[0].cells[i].style.width = this.cellWidthPX[i]+"px";
                              this.obj.rows[0].childNodes[i].style.width = this.cellWidthPX[i]+"px";
							  if (this.ftr)
	                              this.ftr.rows[0].cells[i].style.width = this.cellWidthPX[i]+"px";
                           }
                        }
      /**
      *   @desc: set default delimiter
      *   @param: delim - delimiter as string
      *   @before_init: 1
      *   @type: public
      *   @topic: 0
      */
      this.setDelimiter = function(delim){
         this.delim = delim;
      }
      /**
      *   @desc: set width of columns in percents
      *   @type: public
      *   @before_init: 1
      *   @param: wp - width in percents
      *   @topic: 0,7
      */
      this.setInitWidthsP = function(wp){
         this.cellWidthType = "%";
         this.initCellWidth = wp.split(this.delim.replace(/px/gi,""));
            var el=window;
            var self=this;
            if(el.addEventListener){
                if ((_isFF)&&(_FFrv<1.8))
                    el.addEventListener("resize",function (){
                        if (!self.entBox) return;
                        var z=self.entBox.style.width;
                        self.entBox.style.width="1px";

                        window.setTimeout(function(){  self.entBox.style.width=z; self.setSizes(); },10);
                        },false);
                else
                    el.addEventListener("resize",function (){ if (self.setSizes) self.setSizes(); },false);
                }
            else if (el.attachEvent)
                el.attachEvent("onresize",function(){
                    if (self._resize_timer) window.clearTimeout(self._resize_timer);
                    if (self.setSizes)
                        self._resize_timer=window.setTimeout(function(){ self.setSizes(); },500);
                });

      }
      /**
      *   @desc: set width of columns in pixels
      *   @type: public
      *   @before_init: 1
      *   @param: wp - width in pixels
      *   @topic: 0,7
      */
      this.setInitWidths = function(wp){
         this.cellWidthType = "px";
         this.initCellWidth = wp.split(this.delim);
            if (_isFF){
                for (var i=0; i<this.initCellWidth.length; i++)
					if (this.initCellWidth[i]!="*")
                    this.initCellWidth[i]=parseInt(this.initCellWidth[i])-2;
            }

      }

      /**
      *   @desc: set multiline rows support to enabled or disabled state
      *   @type: public
      *   @before_init: 1
      *   @param: state - true or false
      *   @topic: 0,7
      */
      this.enableMultiline = function(state){
         this.multiLine = convertStringToBoolean(state);
      }

      /**
      *   @desc: set multiselect mode to enabled or disabled state
      *   @type: public
      *   @param: state - true or false
      *   @topic: 0,7
      */
      this.enableMultiselect = function(state){
         this.selMultiRows = convertStringToBoolean(state);
      }

      /**
      *   @desc: set path to grid internal images (sort direction, any images used in editors, checkbox, radiobutton)
      *   @type: public
      *   @param: path - path to images folder with closing "/"
      *   @topic: 0,7
      */
      this.setImagePath = function(path){
		if (path.lastIndexOf("/") == path.length - 1)
			this.imgURL = path;
		else
			this.imgURL = path + "/";
      }

      /**
      *   @desc: part of column resize routine
      *   @type: private
      *   @param: ev - event
      *   @topic: 3
      */
      this.changeCursorState = function (ev){
                           var el = ev.target||ev.srcElement;
                     if(el.tagName!="TD")
                           el = this.getFirstParentOfType(el,"TD")
                           if ((el.tagName=="TD")&&(this._drsclmn)&&(!this._drsclmn[el._cellIndex])) return;
                           if((el.offsetWidth - (ev.offsetX||(parseInt(this.getPosition(el,this.hdrBox))-ev.layerX)*-1))<10){
                              el.style.cursor = "E-resize";
                           }else
                              el.style.cursor = "default";
	  					   if (_isOpera) this.hdrBox.scrollLeft = this.objBox.scrollLeft;
                        }
      /**
      *   @desc: part of column resize routine
      *   @type: private
      *   @param: ev - event
      *   @topic: 3
      */
      this.startColResize = function(ev){
                           this.resized = null;
                           var el = ev.target||ev.srcElement;
                     if(el.tagName!="TD")
                        el = this.getFirstParentOfType(el,"TD")
                           var x = ev.clientX;
                           var tabW = this.hdr.offsetWidth;
                           var startW = parseInt(el.offsetWidth);
                           if(el.tagName=="TD" && el.style.cursor!="default"){
                                        if ((this._drsclmn)&&(!this._drsclmn[el._cellIndex])) return;
                              this.entBox.onmousemove = function(e){this.grid.doColResize(e||window.event,el,startW,x,tabW)}
                              document.body.onmouseup = new Function("","document.getElementById('"+this.entBox.id+"').grid.stopColResize()");
                           }
                        }
      /**
      *   @desc: part of column resize routine
      *   @type: private
      *   @param: ev - event
      *   @topic: 3
      */
      this.stopColResize = function(){
                           this.entBox.onmousemove = "";//removeEventListener("mousemove")//
                           document.body.onmouseup = "";
                           this.setSizes();
                           this.doOnScroll(0,1)
                                    if (this.onRSE) this.onRSE(this);
                        }
      /**
      *   @desc: part of column resize routine
      *   @param: el - element (column resizing)
      *   @param: startW - started width
      *   @param: x - x coordinate to resize from
      *   @param: tabW - started width of header table
      *   @type: private
      *   @topic: 3
      */
      this.doColResize = function(ev,el,startW,x,tabW){
                        el.style.cursor = "E-resize";
                        this.resized = el;
                        var fcolW = startW + (ev.clientX-x);
                        var wtabW = tabW + (ev.clientX-x);
                                if ((this.onRSI)&&(!this.onRSI(el._cellIndex,fcolW,this))) return;
                                if (el.colSpan>1){
                                    var a_sizes=new Array();
                                    for (var i=0; i<el.colSpan; i++)
                                        a_sizes[i]=Math.round(fcolW*this.hdr.rows[0].childNodes[el._cellIndexS+i].offsetWidth/el.offsetWidth);
                                    for (var i=0; i<el.colSpan; i++)
                                        this._setColumnSizeR(el._cellIndexS+i*1,a_sizes[i]);
                                }
                        else
                        {
                                this._setColumnSizeR(el._cellIndex,fcolW);
                         }
                                this.doOnScroll(0,1);
                              if (_isOpera) this.setSizes();
                              //ff3+ fix
                              //this.objBuf.childNodes[0].style.width = "";
                              
                     }

      /**
      *   @desc: set width of grid columns ( zero row of header and body )
      *   @type: private
      *   @topic: 7
      */
       this._setColumnSizeR=function(ind, fcolW){
                        if(fcolW>(this._drsclmW?(this._drsclmW[ind]||10):10)){
                        	this.obj.firstChild.firstChild.childNodes[ind].style.width = fcolW+"px";
                        	
	                        this.hdr.rows[0].childNodes[ind].style.width = fcolW+"px";
							if (this.ftr)
								this.ftr.rows[0].childNodes[ind].style.width = fcolW+"px";
                           if(this.cellWidthType=='px'){
                              this.cellWidthPX[ind]=fcolW;
                           }else{
	                          var gridWidth = parseInt(this.entBox.offsetWidth);
                              if (this.objBox.scrollHeight>this.objBox.offsetHeight)
							  	gridWidth-=(this._scrFix||(_isFF?19:16));
                              var pcWidth = Math.round(fcolW/gridWidth*100)
                              this.cellWidthPC[ind]=pcWidth;
                           }
                        }
         }
      /**
      *    @desc: sets position and visibility of sort image
      *    @param: [state] - true/false - show/hide image
      *    @param: [ind] - index of field
      *    @param: [direction] - ASC/DESC - type of image
      *   @type: public
      *   @topic: 7
      */
         this.setSortImgState=function(state,ind,direction){
            if (!convertStringToBoolean(state)){
             this.sortImg.style.display = "none";
                return;
                }

            if  (direction=="ASC")
             this.sortImg.src = this.imgURL+"sort_asc.gif";
            else
             this.sortImg.src = this.imgURL+"sort_desc.gif";
            this.sortImg.style.display="";
            this.fldSorted=this.hdr.rows[0].cells[ind];
            this.setSortImgPos(ind);
        }

      /**
      *    @desc: sets position and visibility of sort image
      *    @param: [ind] - index of field
      *   @type: private
      *   @topic: 7
      */
      this.setSortImgPos = function(ind,mode){
                           if(!ind)
                              var el = this.fldSorted;
                           else
                              var el = this.hdr.rows[0].cells[ind];
                           if(el!=null){
                              var pos = this.getPosition(el,this.hdrBox)
                              var wdth = el.offsetWidth;
                              this.sortImg.style.left = Number(pos[0]+wdth-13)+"px";//Number(pos[0]+5)+"px";
                              this.sortImg.defLeft = parseInt(this.sortImg.style.left)
                              this.sortImg.style.top = Number(pos[1]+5)+"px";
                              if ((!this.useImagesInHeader)&&(!mode))
                                 this.sortImg.style.display = "inline";
                              this.sortImg.style.left = this.sortImg.defLeft+"px";//-parseInt(this.hdrBox.scrollLeft)
                           }
                        }

      /**
      *   @desc: manage activity of the grid.
      *   @param: fl - true to activate,false to deactivate
      *   @type: private
      *   @topic: 1,7
      */
      this.setActive = function(fl){
                     if(arguments.length==0)
                        var fl = true;
                     if(fl==true){
                           //document.body.onkeydown = new Function("","document.getElementById('"+this.entBox.id+"').grid.doKey()")//
                        globalActiveDHTMLGridObject = this;
                        this.isActive = true;
                     }else{
                        this.isActive = false;
                     }
                  };
      /**
      *     @desc: called on click occured
      *     @type: private
      */
      this._doClick = function(ev){
                     var selMethod = 0;
                     var el = this.getFirstParentOfType(_isIE?ev.srcElement:ev.target,"TD");
                     var fl = true;
                     if(this.selMultiRows!=false){
                        if(ev.shiftKey && this.row!=null){
                           selMethod = 1;
                        }
                        if(ev.ctrlKey){
                           selMethod = 2;
                        }

                     }
                     this.doClick(el,fl,selMethod)
                  };


      /**
      *   @desc: called onmousedown inside grid area
      *   @type: private
      */
        this._doContClick=function(ev){
                     var el = this.getFirstParentOfType(_isIE?ev.srcElement:ev.target,"TD");
                            if ((!el)||(el.parentNode.idd===undefined)) return true;

                            if (ev.button==2){
								if ((this.onRCL)&&(!this.onRCL(el.parentNode.idd,el._cellIndex,ev))) return;
								if (this._ctmndx){
	                                if ((this.onBCM)&&(!this.onBCM(el.parentNode.idd,el._cellIndex,this))) return true;
	                                el.contextMenuId=el.parentNode.idd+"_"+el._cellIndex;
	                                el.contextMenu=this._ctmndx;
	                                el.a=this._ctmndx._contextStart;
	                                if (_isIE)
	                                    ev.srcElement.oncontextmenu = function(){ event.cancelBubble=true; return false; };
	                                el.a(el,ev);
	                                el.a=null;
							   	}
                            }
            return true;
        }

      /**
      *    @desc: occures on cell click (supports treegrid)
      *   @param: [el] - cell to click on
      *   @param:   [fl] - true if to call onRowSelect function
      *   @param: [selMethod] - 0 - simple click, 1 - shift, 2 - ctrl
      *   @type: private
      *   @topic: 1,2,4,9
      */
      this.doClick = function(el,fl,selMethod){

	  				 var psid=this.row?this.row.idd:0;

                     this.setActive(true);
                     if(!selMethod)
                        selMethod = 0;
                     if(this.cell!=null)
                        this.cell.className = this.cell.className.replace(/cellselected/g,"");
                     if(el.tagName=="TD" && (this.rowsCol._dhx_find(this.rowsAr[el.parentNode.idd])!=-1 || this.rowsBuffer[0]._dhx_find(el.parentNode.idd)!=-1)){
                        if (this.onSSC) var initial=this.getSelectedId();
                  var prow=this.row;
                        if(selMethod==0){
                           this.clearSelection();
                        }else if(selMethod==1){
                           var elRowIndex = this.rowsCol._dhx_find(el.parentNode)
                           var lcRowIndex = this.rowsCol._dhx_find(this.lastClicked)
                           if(elRowIndex>lcRowIndex){
                              var strt = lcRowIndex;
                              var end = elRowIndex;
                           }else{
                              var strt = elRowIndex;
                              var end = lcRowIndex;
                           }
                           this.clearSelection();
                           for(var i=0;i<this.rowsCol.length;i++){
                              if((i>=strt && i<=end)&&(this.rowsCol[i])&&(!this.rowsCol[i]._sRow)){
							  	 if ((!this.onBFS)||(this.onBFS(this.rowsCol[i].idd,psid))){
	                                 this.rowsCol[i].className+=" rowselected";
    	                             this.selectedRows[this.selectedRows.length] = this.rowsCol[i]
								 }
                              }/*else{
                                 this.rowsCol[i].className = "";
                              }*/
                           }

                        }else if(selMethod==2){
                           if(el.parentNode.className.indexOf("rowselected") != -1){
                              el.parentNode.className=el.parentNode.className.replace("rowselected","");
                              this.selectedRows._dhx_removeAt(this.selectedRows._dhx_find(el.parentNode))
                              var skipRowSelection = true;
                           }
                        }
                        this.editStop()
                        this.cell = el;

                        if ((prow == el.parentNode)&&(this._chRRS))
                     fl=false;

                        this.row = el.parentNode;

                        if((!skipRowSelection)&&(!this.row._sRow)){
						   if ((!this.onBFS)||(this.onBFS(this.row.idd,psid))){
	                           this.row.className+= " rowselected"
    	                       if(this.selectedRows._dhx_find(this.row)==-1)
        	                      this.selectedRows[this.selectedRows.length] = this.row;
						   }
						   else this.row=true;

                        }
                        if(this.selBasedOn=="cell"){
                           if (this.cell.parentNode.className.indexOf("rowselected")!=-1)
                               this.cell.className = this.cell.className.replace(/cellselected/g,"")+" cellselected";
                        }

                  if(selMethod!=1)
                           this.lastClicked = el.parentNode;

                        var rid = this.row.idd;
                        var cid = this.cell.cellIndex;
                        if (fl) setTimeout(function(){ self.onRowSelect(rid,cid); },100)
                        if (this.onSSC) {
                            var afinal=this.getSelectedId();
                            if (initial!=afinal)  this.onSSC(afinal);
                        }
                     }
                     this.isActive = true;
                     this.moveToVisible(this.cell)
                  }
      /**
      *   @desc: set selection to specified row-cell
      *   @param: r - row object or row index
      *   @param: cInd - cell index
      *   @param: [fl] - true if to call onRowSelect function
        *   @param: preserve - preserve previously selected rows true/false (false by default)
        *   @param: edit - switch selected cell to edit mode
      *   @type: public
      *   @topic: 1,4
      */
      this.selectCell = function(r,cInd,fl,preserve,edit){
                     if(!fl)
                        fl = false;
                     if(typeof(r)!="object")
                        r = this.rowsCol[r]
//#__pro_feature:21092006{
//#colspan:20092006{
                            if (r._childIndexes)
                         var c = r.childNodes[r._childIndexes[cInd]];
                            else
//#}
//#}
                         var c = r.childNodes[cInd];
                            if (preserve)
                         this.doClick(c,fl,3)
                            else
                         this.doClick(c,fl)
                            if (edit) this.editCell();
                  }
      /**
      *   @desc: moves specified cell to visible area (scrolls)
      *   @param: cell_obj - object of the cell to work with
      *   @param: onlyVScroll - allow only vertical positioning

      *   @type: private
      *   @topic: 2,4,7
      */
      this.moveToVisible = function(cell_obj,onlyVScroll){
                     try{
                        var distance = cell_obj.offsetLeft+cell_obj.offsetWidth+20;

                        if(distance>(this.objBox.offsetWidth+this.objBox.scrollLeft)){
                           var scrollLeft = distance - this.objBox.offsetWidth;
                        }else if(cell_obj.offsetLeft<this.objBox.scrollLeft){
                           var scrollLeft =  cell_obj.offsetLeft-5
                        }
                        if ((scrollLeft)&&(!onlyVScroll))
                           this.objBox.scrollLeft = scrollLeft;

                        var distance = cell_obj.offsetTop+cell_obj.offsetHeight + 20;
                        if(distance>(this.objBox.offsetHeight+this.objBox.scrollTop)){
                           var scrollTop = distance - this.objBox.offsetHeight;
                        }else if(cell_obj.offsetTop<this.objBox.scrollTop){
                           var scrollTop =  cell_obj.offsetTop-5
                        }
                        if(scrollTop)
                           this.objBox.scrollTop = scrollTop;
                                          }catch(er){
                     }
                  }
      /**
      *   @desc: creates Editor object and switch cell to edit mode if allowed
      *   @type: public
      *   @topic: 4
      */
      this.editCell = function(){
                     this.editStop();
                     if ((this.isEditable!=true)||(!this.cell))
                        return false;
                     var c = this.cell;
                            //#locked_row:11052006{
                            if (c.parentNode._locked) return false;
                            //#}

					 this.editor = this.cells4(c);

                     //initialize editor
                     if(this.editor!=null){
                           if (this.editor.isDisabled()) { this.editor=null; return false; }
                           c.className+=" editable";

                           if(this.onEditCell(0,this.row.idd,this.cell._cellIndex)!=false){
                              this._Opera_stop=(new Date).valueOf();
                              this.editor.edit()
                              this.onEditCell(1,this.row.idd,this.cell._cellIndex)
                           }else{//preserve editing
                              this.editor=null;
                           }
                     }
                  }
      /**
      *   @desc: gets value from editor(if presents) to cell and closes editor
      *   @type: public
      *   @topic: 4
      */
      this.editStop = function(){
                            if (_isOpera)
                                if (this._Opera_stop){
                                    if ((this._Opera_stop*1+50)>(new Date).valueOf()) return;
                                    this._Opera_stop=null;
                                }

                    if(this.editor && this.editor!=null){
                    	this.cell.className=this.cell.className.replace("editable","");
                    if (this.editor.detach()) this.cell.wasChanged = true;

					var g=this.editor;
                    this.editor=null;
                    var z=this.onEditCell(2,this.row.idd,this.cell._cellIndex,g.getValue(),g.val);
					if ((typeof(z)=="string")||(typeof(z)=="number"))
						g.setValue(z);
					else
						if (!z) g.setValue(g.val);
                     }
                  }
      /**
      *   @desc: manages keybord activity in grid
      *   @type: private
      *   @topic: 7
      */
      this.doKey =   function(ev){
                            if (!ev) return true;
                            if ((ev.target||ev.srcElement).value!==window.undefined){
                                 var zx= (ev.target||ev.srcElement);
                                 if ((!zx.parentNode)||(zx.parentNode.className.indexOf("editable")==-1))
                                     return true;
                                 }
                            if ((globalActiveDHTMLGridObject)&&(this!=globalActiveDHTMLGridObject))
                                return globalActiveDHTMLGridObject.doKey(ev);
                     if(this.isActive==false){
                        //document.body.onkeydown = "";
                        return true;
                     }

                            if (this._htkebl) return true;
							if ((this.onKPR)&&(!this.onKPR(ev.keyCode,ev.ctrlKey,ev.shiftKey))) return false;
                           try{
                        var type = this.cellType[this.cell._cellIndex]
                        //ENTER
                        if(ev.keyCode==13 && (ev.ctrlKey || ev.shiftKey)){
                           var rowInd = this.rowsCol._dhx_find(this.row)
                           if(window.event.ctrlKey && rowInd!=this.rowsCol.length-1){
                              if(this.row.rowIndex==this.obj._rowslength()-1 && this.dynScroll && this.dynScroll!='false')
                                 this.doDynScroll("dn")
                              this.selectCell(this.rowsCol[rowInd+1],this.cell._cellIndex,true);
                           }else if(ev.shiftKey && rowInd!=0){
                              if(this.row.rowIndex==0 && this.dynScroll && this.dynScroll!='false')
                                 this.doDynScroll("up")
                              this.selectCell(this.rowsCol[rowInd-1],this.cell._cellIndex,true);
                           }
                           _isIE?ev.returnValue=false:ev.preventDefault();
                        }
                        if(ev.keyCode==13 && !ev.ctrlKey && !ev.shiftKey){
                           this.editStop();
                           this.onEnter(this.row.idd,this.cell._cellIndex);
                           _isIE?ev.returnValue=false:ev.preventDefault();
                        }
                        //TAB
                        if(ev.keyCode==9 && !ev.shiftKey ){
                                    this.editStop();
                                    var aind=this.cell._cellIndex;
                                    var arow=this.row;
//#__pro_feature:21092006{
//#colspan:20092006{
                                    if (arow._childIndexes)
                                        while (arow._childIndexes[aind+1]==arow._childIndexes[aind]) aind++;
//#}
//#}
                                    aind++;

                                    if (aind>=this.obj.rows[0].childNodes.length){
                                        aind=0;
                                        arow=this.rowsCol[this.rowsCol._dhx_find(this.row)+1];
                                        if (!arow){
                                            aind=this.row.childNodes.length-1;
                                            return true; }
                                    }
                           this.selectCell(arow||this.row,aind,((arow)&&(this.row!=arow)));
                           this.editCell()
                           _isIE?ev.returnValue=false:ev.preventDefault();
                        }else if(ev.keyCode==9 && ev.shiftKey){
                                    this.editStop();
                                    var aind=this.cell._cellIndex-1;
                                    var arow=this.row;
//#__pro_feature:21092006{
//#colspan:20092006{
                                    if (arow._childIndexes)
                                        while ((aind>=0)&&(arow._childIndexes[aind]==arow._childIndexes[aind+1])) aind--;
//#}
//#}
                                    if (aind<0)
                                    {
                                        aind=this.obj.rows[0].childNodes.length-1;
//#__pro_feature:21092006{
//#colspan:20092006{
                                        if (arow._childIndexes)
                                            while (((arow._childIndexes[aind]!="0")&&(!arow._childIndexes[aind]))&&(aind>=0)) aind--;
//#}
//#}
                                        arow=this.rowsCol[this.rowsCol._dhx_find(this.row)-1];
                                        if (!arow) {    aind=0;
                                        return true; }
                                    }
                           this.selectCell(arow||this.row,aind,((arow)&&(this.row!=arow)));
                           this.editCell()
                           _isIE?ev.returnValue=false:ev.preventDefault();
                        }
                        //UP & DOWN
                        if(ev.keyCode==40 || ev.keyCode==38){//&& ev.ctrlKey

                                    if (this.editor && this.editor.combo){
                                        if (ev.keyCode==40) this.editor.shiftNext();
                                        if (ev.keyCode==38) this.editor.shiftPrev();
                                        return false;
                                    }
                                    else{
                              var rowInd = this.row.rowIndex;//rowsCol._dhx_find(this.row)
                              if(ev.keyCode==38 && rowInd!=1){
                                 //if(this.row.rowIndex==0 && this.dynScroll && this.dynScroll!='false')
                                 //   this.doDynScroll("up")
                                 this.selectCell(this.obj._rows(rowInd-2),this.cell._cellIndex,true);
                              }else if(this.pagingOn && ev.keyCode==38 && rowInd==1 && this.currentPage!=1){
                                 this.changePage(this.currentPage-1)
                                 this.selectCell(this.obj.rows[this.obj.rows.length-1],this.cell._cellIndex,true);
                              }else if(ev.keyCode==40 && rowInd!=this.rowsCol.length && rowInd!=this.obj.rows.length-1){
                                 //if(this.row.rowIndex==this.obj._rowslength-1 && this.dynScroll && this.dynScroll!='false')
                                 //     this.doDynScroll("dn")
                                 this.selectCell(this.obj._rows(rowInd),this.cell._cellIndex,true);
                              }else if(this.pagingOn && ev.keyCode==40 && (this.row!=this.rowsCol[this.rowsCol.length-1] || this.rowsBuffer[0].length>0 || !this.recordsNoMore)){
                                 this.changePage(this.currentPage+1)
                                 this.selectCell(this.obj._rows(0),this.cell._cellIndex,true);
                              }
                                      }
                           _isIE?ev.returnValue=false:ev.preventDefault();

                        }
                        //F2
                        if((ev.keyCode==113)&&(this._f2kE)){
                           this.editCell();
                           return false;
                        }
                        //SPACE
                        if(ev.keyCode==32){// && (type=='ch'|| type.indexOf('ra')==0)){
                           var c = this.cell
						   var ed = cells4(c);
                           //this.cell.children(0).click()
                           if(ed.changeState()!=false)
                              _isIE?ev.returnValue=false:ev.preventDefault();
                        }
                        //Esc
                        if(ev.keyCode==27 && this.oe!=false){
                           this.editStop();
                           _isIE?ev.returnValue=false:ev.preventDefault();
                        }
                        //PAGEUP / PAGEDOWN
                        if(ev.keyCode==33 || ev.keyCode==34){
                           if(this.pagingOn){
                              if(ev.keyCode==33){
                                 this.changePage(this.currentPage-1)
                              }else{
                                 this.changePage(this.currentPage+1)
                              }
                           }
                           this.selectCell(this.getRowIndex(this.row.idd)+this.rowsBufferOutSize*(ev.keyCode!=33?1:-1),this.cell._cellIndex,true);

                           /*if(ev.keyCode==33)
                              this.doDynScroll("up")
                           else
                              this.doDynScroll("dn")*/
                           _isIE?ev.returnValue=false:ev.preventDefault();
                        }
                        //RIGHT LEFT
                                if (!this.editor)
                                {
                        if(ev.keyCode==37 && this.cellType._dhx_find("tree")!=-1){
                           this.collapseKids(this.row)
                           _isIE?ev.returnValue=false:ev.preventDefault();
                        }
                        if(ev.keyCode==39 && this.cellType._dhx_find("tree")!=-1){
                           this.expandKids(this.row)
                           _isIE?ev.returnValue=false:ev.preventDefault();
                        }
                                }
                        return true;
                       } catch(er){ return true; }


                  }
   /**
   *   @desc: selects row (?)for comtatibility with previous version
   *   @param: cell - cell object(or cell's child)
   *   @invoke: click on cell(or cell content)
   *   @type: private
   *   @topic: 1,2
   */
   this.getRow = function(cell){
                  if(!cell)
                     cell = window.event.srcElement;
                  if(cell.tagName!='TD')
                     cell = cell.parentElement;
                  r = cell.parentElement;
                  if(this.cellType[cell._cellIndex]=='lk')
                     eval(this.onLink+"('"+this.getRowId(r.rowIndex)+"',"+cell._cellIndex+")");
                  this.selectCell(r,cell._cellIndex,true)
               }
   /**
   *   @desc: selects row (and first cell of it)
   *   @param: r - row index or row object
   *   @param: fl - if true, then call function on select
    *   @param: preserve - preserve previously selected rows true/false (false by default)
   *   @type: public
   *   @topic: 1,2
   */
   this.selectRow = function(r,fl,preserve){
                  if(typeof(r)!='object')
                     r = this.rowsCol[r]
                  this.selectCell(r,0,fl,preserve)
               };
   /**
   *   @desc: sorts specified column
   *   @param: col - column index
   *   @param:   type - str.int.date
   *   @param: order - asc.desc
   *   @type: public
   *   @topic: 2,3,5,9
   */
   this.sortRows = function(col,type,order){
                  while(this.addRowsFromBuffer(true));//nb:paging - before sorting put all rows from buffer to rows collection.
                  //if tree cell exists
                  if(this.cellType._dhx_find("tree")!=-1){
                     return this.sortTreeRows(col,type,order)
                  }
                        var self=this;
                        var arrTS=new Array();
                        var atype = this.cellType[col];
                        for (var i=0; i<this.rowsCol.length; i++)
                                 arrTS[this.rowsCol[i].idd]=this.cells3(this.rowsCol[i],col).getValue();

                        this._sortRows(col,type,order,arrTS);
               }

      /**
      *   @desc: inner sorting routine
      *   @type: private
      *   @topic: 7
      */
    this._sortRows = function(col,type,order,arrTS){
						var sort="sort";
						if (this._sst)  sort="stablesort";


//#__pro_feature:21092006{
//#custom_sort:21092006{
                        if(type=='cus'){
                     this.rowsCol[sort](function(a,b){
                                                    return self._customSorts[col](arrTS[a.idd],arrTS[b.idd],order,a.idd,b.idd);
                                    });
                        }else
//#}
//#}
                        if(type=='str'){
                     this.rowsCol[sort](function(a,b){
                                       if(order=="asc")
                                          return arrTS[a.idd]>arrTS[b.idd]?1:-1
                                       else
                                          return arrTS[a.idd]<arrTS[b.idd]?1:-1
                                    });
                  }else if(type=='int'){
                     this.rowsCol[sort](function(a,b){
                                       var aVal = parseFloat(arrTS[a.idd])||-99999999999999
                                       var bVal = parseFloat(arrTS[b.idd])||-99999999999999
                                       if(order=="asc")
                                          return aVal-bVal
                                       else
                                          return bVal-aVal

                                    });
                  }else if(type=='date'){
                     this.rowsCol[sort](function(a,b){
                                       var aVal = Date.parse(new Date(arrTS[a.idd])||new Date("01/01/1900"))
                                       var bVal = Date.parse(new Date(arrTS[b.idd])||new Date("01/01/1900"))
                                       if(order=="asc")
                                          return aVal-bVal
                                       else
                                          return bVal-aVal

                                    });
                  }
                  if(this.dynScroll && this.dynScroll!='false'){
                     alert("not implemented yet")
                  }else if(this.pagingOn){//nb:paging
                       this.changePage(this.currentPage);
                       if (this.onGridReconstructed) this.onGridReconstructed();
                  }else{
                     var tb = this.obj.firstChild;
                            if (tb.tagName == 'TR') tb = this.obj;


                     for(var i=0;i<this.rowsCol.length;i++){
                                    if  (this.rowsCol[i]!=this.obj._rows(i))
                               tb.insertBefore(this.rowsCol[i],this.obj._rows(i))
                               //tb.moveRow(this.rowsCol[i].rowIndex,i)
                     }
                  }
                       //this.setSizes()
                        if (this.onGridReconstructed) this.onGridReconstructed();
}


   /**
   *   @desc: enables the possibility to load content from server when already loaded content was rendered. Using this you decrease the grid loading time for extremely big amounts of data.
   *   @type: public
   *   @topic: 0,7
   */
   this.setXMLAutoLoading = function(filePath,bufferSize){
        if (arguments.length==0) return (this._xmlaR=true);
      this.recordsNoMore = false;
      this.xmlFileUrl = filePath;
      this.rowsBufferOutSize = bufferSize||this.rowsBufferOutSize==0?40:this.rowsBufferOutSize;
   }

   /**
   *   @desc: enables buffering in content rendering. Using this you decrease the grid loading time.
   *   @type: public
   *   @topic: 0,7
   */
   this.enableBuffering = function(bufferSize){
      this.rowsBufferOutSize = bufferSize||this.rowsBufferOutSize==0?40:this.rowsBufferOutSize;;
   }




   /**
   *   @desc: create rows from another part of buffer
   *   @type: private
   *   @topic: 0,2,7
   */
   this.addRowsFromBuffer = function(stopBeforeServerCall){
      if(this.rowsBuffer[0].length==0){
         if(!this.recordsNoMore && !stopBeforeServerCall){
            if ((this.xmlFileUrl!="")&&(!this._startXMLLoading)){
                    this._startXMLLoading=true;
               this.loadXML(this.xmlFileUrl)
            }
         }else
            return false;
      }
      var cnt = Math.min(this.rowsBufferOutSize,this.rowsBuffer[0].length)


      //this.rowsBuffer.length
      for(var i=0;i<cnt;i++){
         //nb:newbuffer

         if(this.rowsBuffer[1][0].tagName == "TR"){//insert already created row
            this._insertRowAt(this.rowsBuffer[1][0],-1,this.pagingOn);
         }else{//create row from xml tag and insert it
            var rowNode = this.rowsBuffer[1][0]
            this._insertRowAt(this.createRowFromXMLTag(rowNode),-1,this.pagingOn);
         }
         this.rowsBuffer[0]._dhx_removeAt(0);
         this.rowsBuffer[1]._dhx_removeAt(0);
      }

      return this.rowsBuffer[0].length!=0;
   }
   /**
   *   @desc: creates row object based on xml tag
   *   @param: rowNode - object of xml tag "row"
   *   @returns: TR object
   */
   this.createRowFromXMLTag = function(rowNode){
      if(rowNode.tagName=="TR")//not xml tag, but already created TR
         return rowNode;

      var tree=this.cellType._dhx_find("tree");
      var rId = rowNode.getAttribute("id")

      var r= this._fillRowFromXML(this._prepareRow(rId),rowNode,tree,null);

      this.rowsAr[rId] = r;
      return r;
   }

   /**
   *   @desc: allow multiselection
   *   @param: fl - false/true
   *   @type: public
   *   @before_init: 1
   *   @topic: 0,2,7
   */
   this.setMultiselect = function(fl){
      this.selMultiRows = convertStringToBoolean(fl);
   }

   /**
   *   @desc: called when row was double clicked
   *   @type: private
   *   @topic: 1,2
   */
   this.wasDblClicked = function(ev){
      var el = this.getFirstParentOfType(_isIE?ev.srcElement:ev.target,"TD");
      if(el){
         var rowId = el.parentNode.idd;
         return ((this.onRowDblClicked)?this.onRowDblClicked(rowId,el._cellIndex):true);
      }
   }

   /**
   *   @desc: called when header was clicked
   *   @type: private
   *   @topic: 1,2
   */
   this._onHeaderClick = function(e){
         var that=this.grid;
        var el = that.getFirstParentOfType(_isIE?event.srcElement:e.target,"TD");

      if ((this.grid.onHeaderClick)&&(!this.grid.onHeaderClick(el._cellIndexS))) return false;
      if (this.grid.resized==null)
           that.sortField(el._cellIndexS)
   }

   /**
   *   @desc: deletes selected row(s)
   *   @type: public
   *   @topic: 2
   */
   this.deleteSelectedItem = function(){
                     var num = this.selectedRows.length//this.obj.rows.length
                     if(num==0)
                        return;
                     var tmpAr = this.selectedRows;
                     this.selectedRows = new dhtmlxArray(0)
                     for(var i=num-1;i>=0;i--){
                        var node = tmpAr[i]

                                if(!this.deleteRow(node.idd,node)){
                           this.selectedRows[this.selectedRows.length] = node;
                        }else{
                           if(node==this.row){
                              var ind = i;
                           }
                        }
/*
                           this.rowsAr[node.idd] = null;
                           var posInCol = this.rowsCol._dhx_find(node)
                           this.rowsCol[posInCol].parentNode.removeChild(this.rowsCol[posInCol]);//nb:this.rowsCol[posInCol].removeNode(true);
                           this.rowsCol._dhx_removeAt(posInCol)*/
                     }
                     if(ind){
                        try{
                           if(ind+1>this.rowsCol.length)//this.obj.rows.length)
                              ind--;
                           this.selectCell(ind,0,true)
                        }catch(er){
                           this.row = null
                           this.cell = null
                        }
                     }
                  }

   /**
   *   @desc: gets selected row id
   *   @returns: id of selected row (list of ids with default delimiter) or null if non row selected
   *   @type: public
   *   @topic: 1,2,9
   */
   this.getSelectedId = function(){
                     var selAr = new Array(0);
                     for(var i=0;i<this.selectedRows.length;i++){
                        selAr[selAr.length]=this.selectedRows[i].idd
                     }

                     //..
                     if(selAr.length==0)
                        return null;
                     else
                        return selAr.join(this.delim);
                  }
   /**
   *   @desc: gets index of selected cell
   *   @returns: index of selected cell or -1 if there is no selected sell
   *   @type: public
   *   @topic: 1,4
   */
   this.getSelectedCellIndex = function(){
                           if(this.cell!=null)
                              return this.cell._cellIndex;
                           else
                              return -1;
                        }
   /**
   *   @desc: gets width of specified column in pixels
   *   @param: ind - column index
   *   @returns: column width in pixels
   *   @type: public
   *   @topic: 3,7
   */
   this.getColWidth = function(ind){
                           return parseInt(this.cellWidthPX[ind])+((_isFF)?2:0);
                        }

   /**
   *   @desc: sets width of specified column in pixels (soen't works with procent based grid)
   *   @param: ind - column index
   *   @param: value - new width value
   *   @type: public
   *   @topic: 3,7
   */
   this.setColWidth = function(ind,value){
                        if (this.cellWidthType=='px')
                               this.cellWidthPX[ind]=parseInt(value);
                     else
                        this.cellWidthPC[ind]=parseInt(value);
                            this.setSizes();
                        }


   /**
   *   @desc: gets row object by id
   *   @param: id - row id
   *   @returns: row object or null if there is no row with specified id
   *   @type: private
   *   @topic: 2,7,9
   */
   this.getRowById = function(id){
                  var row = this.rowsAr[id]
                  if(row)
                     return row;
                  else
                     if (this._dload){
                         var ind = this.rowsBuffer[0]._dhx_find(id);
                         if (ind>=0) {
                               this._askRealRows(ind);
                                return this.getRowById(id);
                         }
                     }
                     else if(this.pagingOn){//use new buffer
                        var ind = this.rowsBuffer[0]._dhx_find(id);
                                if (ind>=0) {
                           var r = this.createRowFromXMLTag(this.rowsBuffer[1][ind]);
                           this.rowsBuffer[1][ind] = r;
                           return r;
                        }else{
                           return null;
                        }
                     }
					 else if (this._slowParse) //smart parsing mode in treegrid
						return this._seekAndDeploy(id);
                  return null;
               }
   /**
   *   @desc: gets row by index from rowsCola and rowsBuffer
   *   @param: ind - row index
   *   @returns: row object
   *   @type: private
   */
   this.getRowByIndex = function(ind){
      if(this.rowsCol.length<=ind){
         if((this.rowsCol.length+this.rowsBuffer[0].length)<=ind)
            return null;
         else{
            var indInBuf = ind-this.rowsCol.length-1;
            var r = this.createRowFromXMLTag(this.rowsBuffer[1][indInBuf]);
            return r;
         }
      }else{
         return this.rowsCol[ind]
      }
   }

   /**
   *   @desc: gets row index by id (grid only)
   *   @param: row_id - row id
   *   @returns: row index or -1 if there is no row with specified id
   *   @type: public
   *   @topic: 2
   */
   this.getRowIndex = function(row_id){
                        var ind = this.rowsCol._dhx_find(this.getRowById(row_id));
                        if(ind!=-1)
                           return ind;
                        else{
                           ind = this.rowsBuffer[0]._dhx_find(row_id)
                           if(ind!=-1)
                              return ind+this.rowsCol.length;
                           return -1;
                        }
                  }
   /**
   *   @desc: gets row id by index
   *   @param: ind - row index
   *   @returns: row id or null if there is no row with specified index
   *   @type: public
   *   @topic: 2
   */
   this.getRowId = function(ind){
                            var z=this.rowsCol[parseInt(ind)];
                            if (z) return z.idd;
                            return (this.rowsBuffer[0][this._dload?ind:(ind-this.rowsCol.length-1)]||null);
                  }
   /**
   *   @desc: sets new id for row by its index
   *   @param: ind - row index
   *   @param: row_id - new row id
   *   @type: public
   *   @topic: 2
   */
   this.setRowId = function(ind,row_id){
                     var r = this.rowsCol[ind]
                     this.changeRowId(r.idd,row_id)
                  }
   /**
   *   @desc: changes id of the row to the new one
   *   @param: oldRowId - row id to change
   *   @param: newRowId - row id to set
   *   @type:public
   *   @topic: 2
   */
   this.changeRowId = function(oldRowId,newRowId){
                  var row = this.rowsAr[oldRowId]
                  row.idd = newRowId;
                  if(this.UserData[oldRowId]){
                     this.UserData[newRowId] = this.UserData[oldRowId]
                     this.UserData[oldRowId] = null;
                  }
                        if (this.loadedKidsHash){
                            var oldHash=this.loadedKidsHash.get(oldRowId);
                      if (oldHash!=null){
                                for (var z=0; z<oldHash.length; z++)
                                    oldHash[z].parent_id=newRowId;
                         this.loadedKidsHash.put(newRowId,oldHash);
                         this.loadedKidsHash.remove(oldRowId);
                      }
                      var parentsHash = this.loadedKidsHash.get("hashOfParents")
                      if(parentsHash!=null){
                         if(parentsHash.get(oldRowId)!=null){
                            parentsHash.put(newRowId,row);
                            parentsHash.remove(oldRowId);
                            this.loadedKidsHash.put("hashOfParents",parentsHash)
                         }
                      }
                        }

                  this.rowsAr[oldRowId] = null;
                  this.rowsAr[newRowId] = row;
               }
   /**
   *   @desc: sets ids to every column. Can be used then to retreive the index of the desired colum
   *   @param: [ids] - "," delimitered list of ids, or empty if to use values set earlier
   *   @type: public
   *   @topic: 3
   */
   this.setColumnIds = function(ids){
                     if(ids)
                        this.columnIds = ids.split(",")
                if (this.hdr.rows.length>0){
                        if(this.hdr.rows[0].cells.length>=this.columnIds.length){
                           for(var i=0;i<this.columnIds.length;i++){
                              this.hdr.rows[0].cells[i].column_id = this.columnIds[i];
                           }
                        }
                }
                  }
   /**
   *   @desc: gets column index by column id
   *   @param: id - column id
   *   @returns: index of the column
   *   @type: public
   *   @topic: 3
   */
   this.getColIndexById = function(id){
                     for(var i=0;i<this.hdr.rows[0].cells.length;i++){
                        if(this.hdr.rows[0].cells[i].column_id==id)
                           return i;
                     }
                  }
   /**
   *   @desc: gets column id of column specified by index
   *   @param: cin - column index
   *   @returns: column id
   *   @type: public
   *   @topic: 3
   */
   this.getColumnId = function(cin){
                     return this.hdr.rows[0].cells[cin].column_id
                  }

   /**
   *   @desc: gets label of column specified by index
   *   @param: cin - column index
   *   @returns: column label
   *   @type: public
   *   @topic: 3
   */
   this.getHeaderCol = function(cin){
                            var z=this.hdr.rows[1]
                     return z.cells[z._childIndexes?z._childIndexes[parseInt(cin)]:cin].innerHTML;
                  }

   /**
   *   @desc: sets row text BOLD
   *   @param: row_id - row id
   *   @type: public
   *   @topic: 2,6
   */
   this.setRowTextBold = function(row_id){
                     this.getRowById(row_id).style.fontWeight = "bold";
                  }
   /**
   *   @desc: sets style to row
   *   @param: row_id - row id
   *   @param: styleString - style string in common format (exmpl: "color:red;border:1px solid gray;")
   *   @type: public
   *   @topic: 2,6
   */
   this.setRowTextStyle = function(row_id,styleString){
                     var r = this.getRowById(row_id)
                     for(var i=0;i<r.childNodes.length;i++){
                                 var pfix="";

//#__pro_feature:21092006{
//#column_hidden:21092006{
                                 if ((this._hrrar)&&(this._hrrar[i]))  pfix="display:none;";
//#}
//#}
                                 if (_isIE)
                                    r.childNodes[i].style.cssText = pfix+"width:"+r.childNodes[i].style.width+";"+styleString;
                                 else
                            r.childNodes[i].style.cssText = pfix+"width:"+r.childNodes[i].style.width+";"+styleString;
                     }

                  }
   /**
   *   @desc: sets color of row
   *   @param: row_id - row id
   *   @param: color - color value
   *   @type: public
   *   @topic: 2,6
   */
   this.setRowColor = function(row_id,color){
                     var r = this.getRowById(row_id)
                     for(var i=0;i<r.childNodes.length;i++)
                   r.childNodes[i].bgColor=color;
                  }
   /**
   *   @desc: sets style to cell
   *   @param: row_id - row id
   *   @param: ind - cell index
   *   @param: styleString - style string in common format (exmpl: "color:red;border:1px solid gray;")
   *   @type: public
   *   @topic: 2,6
   */
   this.setCellTextStyle = function(row_id,ind,styleString){
                     var r = this.getRowById(row_id)
                            if (!r) return;
                            if (ind<r.childNodes.length)
                            {
                                 var pfix="";
//#__pro_feature:21092006{
//#column_hidden:21092006{
                                 if ((this._hrrar)&&(this._hrrar[i]))  pfix="display:none;";
//#}
//#}
                                 if (_isIE)
                                    r.childNodes[ind].style.cssText = pfix+"width:"+r.childNodes[ind].style.width+";"+styleString;
                                 else
                            r.childNodes[ind].style.cssText = pfix+"width:"+r.childNodes[ind].style.width+";"+styleString;
                     }

                  }

   /**
   *   @desc: sets row text NORMAL
   *   @param: row_id - row id
   *   @type: public
   *   @topic: 2,6
   */
   this.setRowTextNormal = function(row_id){
                     this.getRowById(row_id).style.fontWeight = "normal";
                  }
   /**
   *   @desc: determines if row with specified id exists
   *   @param: row_id - row id
   *   @returns: true if exists, false otherwise
   *   @type: public
   *   @topic: 2,7
   */
   this.isItemExists = function(row_id){
                     if(this.getRowById(row_id)!=null)
                        return true
                     else
                        return false
                  }

   /**
   *   @desc: gets number of rows in grid
   *   @returns: number of rows in grid
   *   @type: public
   *   @topic: 2,7
   */
   this.getRowsNum = function(){
                     if (this._dload)
                     	return  this.limit;
                     return this.rowsCol.length+this.rowsBuffer[0].length;
                  }
   /**
   *   @desc: gets number of columns in grid
   *   @returns: number of columns in grid
   *   @type: public
   *   @topic: 3,7
   */
   this.getColumnCount = function(){
                     return this.hdr.rows[0].cells.length;
                  }

   /**
   *   @desc: moves row one position up if possible
   *   @param: row_id -  row id
   *   @type: public
   *   @topic: 2
   */
   this.moveRowUp = function(row_id){
                     var r = this.getRowById(row_id)
                     var rInd = this.rowsCol._dhx_find(r)
                            if (this.isTreeGrid()){
                                if (this.rowsCol[rInd].parent_id!=this.rowsCol[rInd-1].parent_id) return;
                                this.collapseKids(r);
                                }

                            this.rowsCol._dhx_swapItems(rInd,rInd-1)

                                if (r.previousSibling){
                                  r.parentNode.insertBefore(r,r.previousSibling)
                                    this.setSizes();
                                    }
                  }
   /**
   *   @desc: moves row one position down if possible
   *   @param: row_id -  row id
   *   @type: public
   *   @topic: 2
   */
   this.moveRowDown = function(row_id){
                     var r = this.getRowById(row_id)
                     var rInd = this.rowsCol._dhx_find(r)
                            if (this.isTreeGrid())
                                if (this.rowsCol[rInd].parent_id!=this.rowsCol[rInd+1].parent_id) return;

                            this.rowsCol._dhx_swapItems(rInd,rInd+1)
                            if (r.nextSibling){
                                if (r.nextSibling.nextSibling)
                                  r.parentNode.insertBefore(r,r.nextSibling.nextSibling)
                                else
                                    r.parentNode.appendChild(r)
                                this.setSizes();
                                }
                  }
   /**
   *   @desc: gets dhtmlXGridCellObject object (if no arguments then gets dhtmlXGridCellObject object of currently selected cell)
   *   @param: row_id -  row id
   *   @param: col -  column index
   *   @returns: dhtmlXGridCellObject object (see its methods below)
   *   @type: public
   *   @topic: 4
   */
   this.cells = function(row_id,col){
                     if(arguments.length==0)
	                        return this.cells4(this.cell);
                     else
                        var c = this.getRowById(row_id);
                        var cell=(c._childIndexes?c.childNodes[c._childIndexes[col]]:c.childNodes[col]);
						return this.cells4(cell);
                  }
   /**
   *   @desc: gets dhtmlXGridCellObject object
   *   @param: row_index -  row index
   *   @param: col -  column index
   *   @returns: dhtmlXGridCellObject object (see its methods below)
   *   @type: public
   *   @topic: 4
   */
   this.cells2 = function(row_index,col){
		var c = this.rowsCol[parseInt(row_index)];
		var cell=(c._childIndexes?c.childNodes[c._childIndexes[col]]:c.childNodes[col]);
		return this.cells4(cell);
                  }

   /**
   *   @desc: gets exCell editor for row  object and column id
   *   @type: private
   *   @topic: 4
   */
   this.cells3 = function(row,col){
        var cell=(row._childIndexes?row.childNodes[row._childIndexes[col]]:row.childNodes[col]);
		return this.cells4(cell);
                 }
   /**
   *   @desc: gets exCell editor for cell  object
   *   @type: private
   *   @topic: 4
   */
   this.cells4 = function(cell){
		if (!cell._cellType)
	 		return eval("new eXcell_"+this.cellType[cell._cellIndex]+"(cell)");
		else
			return eval("new eXcell_"+cell._cellType+"(cell)");
                  }
   /**
   * @desc: gets Combo object of specified column
   *   @type: public
   *   @topic: 3,4
   *   @param: col_ind - index of the column to get combo object for
   */
   this.getCombo = function(col_ind){
      if(this.cellType[col_ind].indexOf('co')==0){
         if(!this.combos[col_ind]){
            this.combos[col_ind] = new dhtmlXGridComboObject();
         }
         return this.combos[col_ind];
      }else{

         return null;
      }
   }
   /**
   *   @desc: sets user Data
   *   @param: row_id -  row id. if empty then user data is for grid (not row)
   *   @param: name -  name of user data
   *   @param: value -  value of user data
   *   @type: public
   *   @topic: 2,5
   */
   this.setUserData = function(row_id,name,value){
                     try{
                        if(row_id=="")
                           row_id = "gridglobaluserdata";
                        if(!this.UserData[row_id])
                           this.UserData[row_id] = new Hashtable()
                        this.UserData[row_id].put(name,value)
                     }catch(er){
                        alert("UserData Error:"+er.description)
                     }
                  }
   /**
   *   @desc: gets user Data
   *   @param: row_id -  row id. if empty then user data is for grid (not row)
   *   @param: name -  name of user data
   *   @returns: value of user data
   *   @type: public
   *   @topic: 2,5
   */
   this.getUserData = function(row_id,name){
            if(row_id=="")
               row_id = "gridglobaluserdata";
                var z=this.UserData[row_id];
               return (z?z.get(name):"");
      }

   /**
   *   @desc: manage editibility of the grid
   *   @param: [fl] - set not editable if FALSE, set editable otherwise
   *   @type: public
   *   @topic: 7
   */
   this.setEditable = function(fl){
                     if(fl!='true' && fl!=1 && fl!=true)
                        ifl = true;
                     else
                        ifl = false;
                     for(var j=0;j<this.cellType.length;j++){
                        if(this.cellType[j].indexOf('ra')==0 || this.cellType[j]=='ch'){
                           for(var i=0;i<this.rowsCol.length;i++){
                                        var z=this.rowsCol[i].cells[j];
                              if ((z.childNodes.length>0)&&(z.firstChild.nodeType==1)){
                                 this.rowsCol[i].cells[j].firstChild.disabled = ifl;
                              }
                           }
                        }
                     }
                     this.isEditable = !ifl;
                  }
   /**
   *   @desc: selects row
   *   @param: row_id - row id
   *   @param: multiFL - VOID. select multiple rows
   *   @param: show - VOID. scroll row to view
   *   @param: call - true to call function on select
   *   @type: public
   *   @topic: 1,2
   */
   this.setSelectedRow = function(row_id, multiFL,show,call){
                     if(!call)
                        call = false;
                     this.selectCell(this.getRowById(row_id),0,call,multiFL);//selectRow(this.getRowById(row_id),false)
                     if(arguments.length>2 && show==true){
                        this.moveToVisible(this.getRowById(row_id).cells[0],true)
                     }
                  }
   /**
   *   @desc: removes selection from the grid
   *   @type: public
   *   @topic: 1,9
   */
   this.clearSelection = function(){
                     this.editStop()
                     for(var i=0;i<this.selectedRows.length;i++){
                        this.selectedRows[i].className=this.selectedRows[i].className.replace(/rowselected/g,"");
                     }

                     //..
                     this.selectedRows = new dhtmlxArray(0)
                     this.row = null;
                     if(this.cell!=null){
                        this.cell.className = this.cell.className.replace(/cellselected/g,"");
                        this.cell = null;
                     }
                  }
   /**
   *   @desc: copies row content to another existing row
   *   @param: from_row_id - id of the row to copy content from
   *   @param: to_row_id - id of the row to copy content to
   *   @type: public
   *   @topic: 2,5
   */
   this.copyRowContent = function(from_row_id, to_row_id){
                     var frRow = this.getRowById(from_row_id)

                            if (!this.isTreeGrid())
                         for(i=0;i<frRow.cells.length;i++){
                            this.cells(to_row_id,i).setValue(this.cells(from_row_id,i).getValue())
                         }
                            else
                                this._copyTreeGridRowContent(frRow,from_row_id,to_row_id);

                     //for Mozilla (to avaoid visual glitches)
                     if(!isIE())
                        this.getRowById(from_row_id).cells[0].height = frRow.cells[0].offsetHeight
                  }




   /**
   *   @desc: sets new column header label
   *   @param: col - header column index
   *   @param: label - new label for the cpecified header's column. Can contai img:[imageUrl]Text Label
   *   @type: public
   *   @topic: 3,6
   */
   this.setHeaderCol = function(col,label){
                     var z=this.hdr.rows[1];
					 var col=(z._childIndexes?z._childIndexes[col]:col);
                     if(!this.useImagesInHeader){
                        var hdrHTML = "<div class='hdrcell'>"
						if(label.indexOf('img:[')!=-1){
							var imUrl = label.replace(/.*\[([^>]+)\].*/,"$1");
							label = label.substr(label.indexOf("]")+1,label.length)
							hdrHTML+="<img width='18px' height='18px' align='absmiddle' src='"+imUrl+"' hspace='2'>"
						}
						hdrHTML+=label;
						hdrHTML+="</div>";
						z.cells[col].innerHTML = hdrHTML;

					 }else{//if images in header header
                        z.cells[col].style.textAlign = "left";
                        z.cells[col].innerHTML = "<img src='"+this.imgURL+""+label+"' onerror='this.src = \""+this.imgURL+"imageloaderror.gif\"'>";
                        //preload sorting headers (asc/desc)
                        var a = new Image();
                        a.src = this.imgURL+""+label.replace(/(\.[a-z]+)/,".desc$1");
                        this.preloadImagesAr[this.preloadImagesAr.length] = a;
                        var b = new Image();
                        b.src = this.imgURL+""+label.replace(/(\.[a-z]+)/,".asc$1");
                        this.preloadImagesAr[this.preloadImagesAr.length] = b;
                     }
                  }
   /**
   *   @desc: deletes all rows in grid
   *   @param: header - (boolean) enable/disable cleaning header
   *   @type: public
   *   @topic: 5,7,9
   */
   this.clearAll = function(header){
   					this.limit=this._limitC=0;
                            this.editStop();

                            if (this._dload){
                               this.objBox.scrollTop=0;
                               this.limit=this._limitC||0;
                               this._initDrF=true;
                               }

                     var len = this.rowsCol.length;
                     //treegrid
                     if(this.loadedKidsHash!=null){
                        this.loadedKidsHash.clear();
                                this.loadedKidsHash.put("hashOfParents",new Hashtable());
                     }
                     //for some case
                     len = this.obj._rowslength();

                     for(var i=len-1;i>=0;i--){
	                    var t_r=this.obj._rows(i);
                        t_r.parentNode.removeChild(t_r);
                     }
					 if (header){
					 	 this.obj.rows[0].parentNode.removeChild(this.obj.rows[0]);
    	                 for(var i=this.hdr.rows.length-1;i>=0;i--){
		                    var t_r=this.hdr.rows[i];
                        	t_r.parentNode.removeChild(t_r);
                     	}
					 }

                     //..
                     this.row = null;
                     this.cell = null;
                     this._hrrar=null;

                     this.rowsCol = new dhtmlxArray(0)
                     this.rowsAr = new Array(0);//array of rows by idd
                     this.rowsBuffer = new Array(new dhtmlxArray(0),new dhtmlxArray(0));//buffer of rows loaded, but not rendered (array of ids, array of cell values arrays)
                     this.UserData = new Array(0)

                     if(this.pagingOn){
                        this.changePage(1);
                        //this.createPagingBlock();
                     }

                if ((this._hideShowColumn)&&(this.hdr.rows[0]))
                	for (var i=0; i<this.hdr.rows[0].length; i++)
                    	this._hideShowColumn(i,"");
				this._hrrar=new Array();

				if (this._sst)
					this.enableStableSorting(true);

                    this.setSizes();
                    //this.obj.scrollTop = 0;
   }


   /**
   *   @desc: sorts grid by specified field
   *    @invoke: header click
   *   @param: [ind] - index of the field
   *   @param: [repeatFl] - if to repeat last sorting
   *   @type: private
   *   @topic: 3
   */
   this.sortField = function(ind,repeatFl){
                  if(this.getRowsNum()==0)
                     return false;
                  var el = this.hdr.rows[0].cells[ind];
                        if (!el) return; //somehow
                  if(el.tagName == "TH" && (this.fldSort.length-1)>=el._cellIndex && this.fldSort[el._cellIndex]!='na'){//this.entBox.fieldstosort!="" &&
                     if((((this.sortImg.src.indexOf("_desc.gif")==-1) && (!repeatFl)) || ((this.sortImg.style.filter!="") && (repeatFl))) && (this.fldSorted==el)){//desc
                        var sortType = "desc";
                        this.sortImg.src = this.imgURL+"sort_desc.gif";
                     }else{//asc
                        var sortType = "asc";
                        this.sortImg.src = this.imgURL+"sort_asc.gif";
                     }
                  if ((this.onCLMS)&&(!this.onCLMS(ind,this,sortType))) return;

                     //for header images
                     if(this.useImagesInHeader){
                   var cel=this.hdr.rows[1].cells[el._cellIndex].firstChild;
                        if(this.fldSorted!=null){
                     var celT=this.hdr.rows[1].cells[this.fldSorted._cellIndex].firstChild;
                           celT.src = celT.src.replace(/\.[ascde]+\./,".");
                        }
                        cel.src = cel.src.replace(/(\.[a-z]+)/,"."+sortType+"$1")
                     }
                     //.
                     this.sortRows(el._cellIndex,this.fldSort[el._cellIndex],sortType)
                     this.fldSorted = el;
					 var c=this.hdr.rows[1];
                     var real_el=c._childIndexes?c._childIndexes[el._cellIndex]:el._cellIndex;
                     this.setSortImgPos(this.hdr.rows[1].childNodes[real_el]._cellIndex);
                  }
               }

//#__pro_feature:21092006{
//#custom_sort:21092006{
    /**
    *   @desc: set custom sorting (custom sort has three params - valueA,valueB,order; where order can be asc or des)
    *   @param: func - column index
    *   @param:   col - str.int.date
    *   @type: public
    *   @edition: Professional
    *   @topic: 3
    */
    this.setCustomSorting = function(func,col){
       if (!this._customSorts) this._customSorts=new Array();
       this._customSorts[col]=func;
       this.fldSort[col]="cus";
    }
//#}
//#}

   /**
   *   @desc: specify if values passed to Header are images names
   *   @param: fl - true to treat column header values as image names
   *   @type: public
   *   @before_init: 1
   *   @topic: 0,3
   */
   this.enableHeaderImages = function(fl){
      this.useImagesInHeader = fl;
   }

   /**
   *   @desc: set header label and default params for new headers
   *   @param: hdrStr - header string with delimiters
   *   @param: splitMarker - string used as a split marker, optional
   *   @type: public
   *   @before_init: 1
   *   @topic: 0,3
   */
   this.setHeader = function(hdrStr,splitSign){
      var arLab = hdrStr.split(this.delim);
      var arWdth = new Array(0);
      var arTyp = new dhtmlxArray(0);
      var arAlg = new Array(0);
      var arVAlg = new Array(0);
      var arSrt = new Array(0);
      for(var i=0;i<arLab.length;i++){
         arWdth[arWdth.length] = Math.round(100/arLab.length);
         arTyp[arTyp.length] = "ed";
         arAlg[arAlg.length] = "left";
         arVAlg[arVAlg.length] = "";//top
         arSrt[arSrt.length] = "na";
      }
	  
      this.splitSign = splitSign||"#cspan";
      this.hdrLabels = arLab;
      this.cellWidth = arWdth;
      this.cellType =  arTyp;
      this.cellAlign =  arAlg;
      this.cellVAlign =  arVAlg;
      this.fldSort = arSrt;
   }


   /**
   *   @desc: get column types
   *   @param: cell_index - column index
   *   @returns:  type code
   *   @type: public
   *   @topic: 0,3,4
   */
    this.getColType = function(cell_index) {
       return this.cellType[cell_index];
    }

   /**
   *   @desc: get column types
   *   @param: col_id - column id
   *   @returns:  type code
   *   @type: public
   *   @topic: 0,3,4
   */
    this.getColTypeById = function(col_id) {
       return this.cellType[this.getColIndexById(col_id)];
    }

   /**
   *   @desc: set column types
   *   @param: typeStr - type codes list with default delimiter
   *   @before_init: 2
   *   @type: public
   *   @topic: 0,3,4
   */
   this.setColTypes = function(typeStr){
      this.cellType = dhtmlxArray(typeStr.split(this.delim));
          this._strangeParams=new Array();
        for (var i=0; i<this.cellType.length; i++)
        if ((this.cellType[i].indexOf("[")!=-1))
            {
                var z=this.cellType[i].split(/[\[\]]+/g);
                this.cellType[i]=z[0];
                this.defVal[i]=z[1];
                if (z[1].indexOf("=")==0){
                    this.cellType[i]="math";
                    this._strangeParams[i]=z[0];
                    }
            }
   }
   /**
   *   @desc: set column sort types (avaialble: str, int, date, na)
   *   @param: sortStr - sort codes list with default delimiter
   *   @before_init: 1
   *   @type: public
   *   @topic: 0,3,4
   */
   this.setColSorting = function(sortStr){
      this.fldSort = sortStr.split(this.delim)
//#__pro_feature:21092006{
//#custom_sort:21092006{
        for (var i=0; i<this.fldSort.length; i++)
            if (((this.fldSort[i]).length>4)&&(typeof(window[this.fldSort[i]])=="function"))
                {
                   if (!this._customSorts) this._customSorts=new Array();
                   this._customSorts[i]=window[this.fldSort[i]];
                   this.fldSort[i]="cus";
                }
//#}
//#}
   }
   /**
   *   @desc: set align of columns
   *   @param: alStr - align string with delimiters
   *   @before_init: 1
   *   @type: public
   *   @topic: 0,3
   */
   this.setColAlign = function(alStr){
      this.cellAlign = alStr.split(this.delim)
   }
   /**
   *   @desc: set vertical align of columns
   *   @param: alStr - align string with delimiters
   *   @before_init: 1
   *   @type: public
   *   @topic: 0,3
   */
   this.setColVAlign = function(alStr){
      this.cellVAlign = alStr.split(this.delim)
   }

   /**
   *   @desc: sets grid to multiline row support (call before init)
   *   @param:   fl - true to set multiline support
   *   @type: deprecated
   *   @before_init: 1
   *   @topic: 0,2
   */
   this.setMultiLine = function(fl){
      if(fl==true)
         this.multiLine = -1;
   }
   /**
   *   @desc: use to create grid with no header
   *   @param: fl - true to use no header in the grid
   *   @type: public
   *   @before_init: 1
   *   @topic: 0,7
   */
   this.setNoHeader = function(fl){
      if(fl==true)
         this.noHeader = true;
   }
   /**
   *   @desc: scrolls row to the visible area
   *   @param: rowID - row id
   *   @type: public
   *   @topic: 2,7
   */
   this.showRow = function(rowID){
      this.moveToVisible(this.getRowById(rowID).cells[0],true)
   }

   /**
   *   @desc: modify default style of grid and its elements. Call before or after Init
   *   @param: ss_header - style def. expression for header
   *   @param: ss_grid - style def. expression for grid cells
   *   @param: ss_selCell - style def. expression for selected cell
   *   @param: ss_selRow - style def. expression for selected Row
   *   @type: public
   *   @before_init: 2
   *   @topic: 0,6
   */
   this.setStyle = function(ss_header,ss_grid,ss_selCell,ss_selRow){
      this.ssModifier = [ss_header, ss_grid , ss_selCell,ss_selCell, ss_selRow];
     var prefs=["#"+this.entBox.id+" table.hdr td","#"+this.entBox.id+" table.obj td","#"+this.entBox.id+" table.obj tr.rowselected td.cellselected","#"+this.entBox.id+" table.obj td.cellselected","#"+this.entBox.id+" table.obj tr.rowselected td"];

     for (var i=0; i<prefs.length; i++)
      if (_isIE)
         this.styleSheet[0].addRule(prefs[i],this.ssModifier[i]);
          else
         this.styleSheet[0].insertRule(prefs[i]+" { "+this.ssModifier[i]+" } ",0);
   }
   /**
   *   @desc: colorize columns.
   *   @param: clr - colors list
   *   @type: public
   *   @before_init: 1
   *   @topic: 3,6
   */
   this.setColumnColor = function(clr){
      this.columnColor = clr.split(this.delim)
   }

   /**
   *   @desc: set even/odd css styles
   *   @param: cssE - name of css class for even rows
   *   @param: cssU - name of css class for odd rows
   *   @type: public
   *   @before_init: 1
   *   @topic: 3,6
   */
   this.enableAlterCss = function(cssE,cssU){
        if (cssE||cssU)
            this.setOnGridReconstructedHandler(function(){
                this._fixAlterCss();
            });


      this._cssEven = cssE;
      this._cssUnEven = cssU;
   }

   /**
   *   @desc: recolor grid from defined point
   *   @type: private
   *   @before_init: 1
   *   @topic: 3,6
   */
   this._fixAlterCss = function(ind){
        ind=ind||0;
        var j=ind;
        for (var i=ind; i<this.rowsCol.length; i++){
            if (!this.rowsCol[i]) continue;
            if (this.rowsCol[i].style.display!="none"){
            if (this.rowsCol[i].className.indexOf("rowselected")!=-1){
                if (j%2==1)
                    this.rowsCol[i].className=this._cssUnEven+" rowselected";
                else
                    this.rowsCol[i].className=this._cssEven+" rowselected";
            }
            else{
                if (j%2==1)
                    this.rowsCol[i].className=this._cssUnEven;
                else
                    this.rowsCol[i].className=this._cssEven;
            }
                j++;
            }
        }
   }


   /**
   *   @desc: dynamicaly scrolls grid content.
   *   @param: fl - if no fl  initializes dynamic scroll, if up  scroll page up, if dn  scrolls page down
   *   @type: public
   *   @topic: 0,6,7,9
   */
   this.doDynScroll = function(fl){
      if(!this.dynScroll || this.dynScroll=='false')
         return false;
      //this.objBox.style.overflowY = "hidden";
      //alert(this.dynScrollPageSize)
      this.setDynScrollPageSize();
      //alert(this.dynScrollPageSize)

      var tmpAr = new Array(0)
      if(fl && fl=='up'){
         this.dynScrollPos = Math.max(this.dynScrollPos-this.dynScrollPageSize,0);
      }else if(fl && fl=='dn' && this.dynScrollPos+this.dynScrollPageSize<this.rowsCol.length){
         if(this.dynScrollPos+this.dynScrollPageSize+this.rowsBufferOutSize>this.rowsCol.length){
            this.addRowsFromBuffer()
         }
         this.dynScrollPos+=this.dynScrollPageSize
      }
      var start = Math.max(this.dynScrollPos-this.dynScrollPageSize,0);
      for(var i = start;i<this.rowsCol.length;i++){
         if(i>=this.dynScrollPos && i<this.dynScrollPos+this.dynScrollPageSize){
            tmpAr[tmpAr.length] = this.rowsCol[i];
         }
         this.rowsCol[i].removeNode(true);
      }
      for(var i=0;i<tmpAr.length;i++){
         this.obj.childNodes[0].appendChild(tmpAr[i]);
         if(this.obj.offsetHeight>this.objBox.offsetHeight)
            this.dynScrollPos-=(this.dynScrollPageSize-i)
      }
      this.setSizes()


   }
   /**
   *   @desc: counts the number of rows to fit the grid for dyn scroll
   *   @type: private
   *   @topic: 0
   */
   this.setDynScrollPageSize = function(){
            if(this.dynScroll && this.dynScroll!='false'){
               var rowsH = 0;
               try{
                  var rowH = this.obj._rows(0).scrollHeight;
               }catch(er){
                  var rowH = 20
               }
               for(var i=0;i<1000;i++){
                  rowsH = i*rowH;
                  if(this.objBox.offsetHeight<rowsH)
                     break
               }
               this.dynScrollPageSize = i+ 2;//parseInt(i/3.5);
               this.rowsBufferOutSize = this.dynScrollPageSize * 4
            }
   }

//#__pro_feature:21092006{
/**
*     @desc: clear wasChanged state for all cells in grid
*     @type: public
*     @edition: Professional
*     @topic: 7
*/
this.clearChangedState = function(){
   for (var i=0; i<this.rowsCol.length; i++){
      var row=this.rowsCol[i];
      var cols=row.childNodes.length;
      for (var j=0; j<cols; j++)
         row.childNodes[i].wasChanged=false;
   }
};

/**
*     @desc: return list of changed rows
*     @type: public
*     @edition: Professional
*     @return: list of ID of changed rows
*     @topic: 7
*/
this.getChangedRows = function(){
   var res=new Array();
   for (var i=0; i<this.rowsCol.length; i++){
      var row=this.rowsCol[i];
      var cols=row.childNodes.length;
      for (var j=0; cols; j++)
        if (row.childNodes[i].wasChanged) {
         res[res.length]=row.idd;
         break;
         }
   }
   return res.join(this.delim);
};



//#serialization:21092006{

this._sUDa = false;
this._sAll = false;

/**
*     @desc: configure XML serialization
*     @type: public
*     @edition: Professional
*     @param: userData - enable/disable user data serialization
*     @param: fullXML - enable/disable full XML serialization (selection state)
*     @param: config - serialize grid configuration
*     @param: changedAttr - include changed attribute
*     @param: onlyChanged - include only Changed  rows in result XML
*     @topic: 0,5,7
*/
this.setSerializationLevel = function(userData,fullXML,config,changedAttr,onlyChanged){
   this._sUDa = userData;
   this._sAll = fullXML;
   this._sConfig = config;
   this._chAttr = changedAttr;
   this._onlChAttr = onlyChanged;
}



/**
*     @desc: configure which column must be serialized
*     @type: public
*     @edition: Professional
*     @param: list - list of true/false values separated by comma, if list empty then all fields will be serialized
*     @topic: 0,5,7
*/
this.setSerializableColumns=function(list){
    if (!list) {
        this._srClmn=null;
        return;
        }
    this._srClmn=(list||"").split(",");
    for (var i=0; i<this._srClmn.length; i++)
        this._srClmn[i]=convertStringToBoolean(this._srClmn[i]);
}

/**
*     @desc: serialize a collection of rows
*     @type: private
*     @topic: 0,5,7
*/
this._serialise = function(rCol,inner,closed){
     this.editStop()
    var out="";
    //rows collection
    var i=0;
    var j=0;
    var leni=(this._dload)?this.rowsBuffer[0].length:rCol.length;
   for(i; i<leni; i++){

       var r = rCol[i];
           var temp=this._serializeRow(r)
          out += temp;

            if (this.loadedKidsHash){

              var z=this.loadedKidsHash.get(r.idd);
              if (z){
                  temp=this._serialise(z,1,closed||r.expand!=="");
                  out+=temp[0];
                  if ((!closed)&&(r.expand===""))
                  if (!inner)
                      i+=temp[1];
                  else j+=temp[1];
              }

            }
      if (temp!="")
         out += "</row>";
   }

    return [out,j+i];
}

/**
*   @desc: serialize xml node to XML string
*   @param: r - TR or xml node (row)
*   @retruns: string - xml representation of passed row
*   @type: private
*/
this._manualXMLSerialize = function(r){
	var out = "<row id='"+r.getAttribute("id")+"'>";
	var i=0;
    for(var jj=0;jj<r.childNodes.length;jj++){
		var z=r.childNodes[jj];
		if (z.tagName!="cell") continue;
        if ((!this._srClmn)||(this._srClmn[i]))
			out += "<cell>"+(z.firstChild?z.firstChild.data:"")+"</cell>";
		i++;
      }
	out+="</row>";
    return out;
}


/**
*   @desc: serialize TR or xml node to grid formated xml (row tag)
*   @param: r - TR or xml node (row)
*   @retruns: string - xml representation of passed row
*   @type: private
*/
this._serializeRow = function(r){
    var out = "";
    if ((!r)||(r._sRow)||(r._rLoad)) {
            if (this.xmlSerializer)
                out+=this.xmlSerializer.serializeToString(this.rowsBuffer[1][i]);
            else
                out+=this.rowsBuffer[1][i].xml;
            return out;
     }


      var selStr = "";

      //serialize selection
      if(this._sAll && this.selectedRows._dhx_find(r)!=-1)
         selStr = " selected='1'";
      out += "<row id='"+r.idd+"'"+selStr+" "+((r.expand=="")?"open='1'":"")+">";
      //userdata
      if(this._sUDa && this.UserData[r.idd]){
         keysAr = this.UserData[r.idd].getKeys()
           for(var ii=0;ii<keysAr.length;ii++){
            out += "<userdata name='"+keysAr[ii]+"'>"+this.UserData[r.idd].get(keysAr[ii])+"</userdata>";
         }
      }


      //cells
     var changeFl=false;
      for(var jj=0;jj<r.childNodes.length;jj++){
            if ((!this._srClmn)||(this._srClmn[jj]))
                {
                var cvx=r.childNodes[jj];
                out += "<cell"

                var zx=this.cells(r.idd,cvx._cellIndex);
                if (zx.cell)
                    zxVal=zx[this._agetm]();
                else zxVal="";

//#colspan:20092006{
                if ((this._ecspn)&&(cvx.colSpan)&&cvx.colSpan>1)
                    out+=" colspan=\""+cvx.colSpan+"\" ";
//#}

            if (this._chAttr){
               if (zx.wasChanged()){
                  out+=" changed=\"1\"";
                  changeFl=true;
                  }
               }
            else
               if ((this._onlChAttr)&&(zx.wasChanged())) changeFl=true;

                if    (this._sAll)
                  out+=(cvx._aimage?(" image='"+cvx._aimage+"'"):"")+">"+((zxVal===null)?"":zxVal)+"</cell>";
                else
                 out+=">"+((zxVal===null)?"":zxVal)+"</cell>";
//#colspan:20092006{
                if ((this._ecspn)&&(cvx.colSpan)){
                    cvx=cvx.colSpan-1;
                    for (var u=0; u<cvx; u++)
                        out += "<cell/>";
                        }
//#}
                }
      }
     if ((this._onlChAttr)&&(!changeFl)) return "";
      return out;
}

/**
*     @desc: serialize grid configuration
*     @type: private
*     @topic: 0,5,7
*/
this._serialiseConfig=function(){
    var out="<head>";
        for (var i=0; i<this.hdr.rows[0].cells.length; i++){
            out+="<column width='"+this.cellWidthPX[i]+"' align='"+this.cellAlign[i]+"' type='"+this.cellType[i]+"' sort='"+this.fldSort[i]+"' color='"+this.columnColor[i]+"'>";
            out+=this.hdr.rows[1].cells[i].childNodes[0].innerHTML;
            var z=this.getCombo(i);
            if (z)
                for (var j=0; j<z.keys.length; j++)
                    out+="<option value='"+z.keys[j]+"'>"+z.values[j]+"</option>";
            out+="</column>"
            }
    return out+="</head>";
}
/**
*     @desc: return actual xml of grid
*     @type: public
*     @edition: Professional
*     @topic: 5,7
*/
this.serialize = function(){
    if(_isFF)
      this.xmlSerializer = new XMLSerializer();

   var out='<?xml version="1.0"?><rows>';
        if (this._mathSerialization)
             this._agetm="getMathValue";
        else this._agetm="getValue";

   if(this._sUDa && this.UserData["gridglobaluserdata"]){
      var keysAr = this.UserData["gridglobaluserdata"].getKeys()
      for(var i=0;i<keysAr.length;i++){
         out += "<userdata name='"+keysAr[i]+"'>"+this.UserData["gridglobaluserdata"].get(keysAr[i])+"</userdata>";
      }

   }

    if (this._sConfig)
        out+=this._serialiseConfig();
    out+=this._serialise(this.rowsCol)[0];


    if (!this._dload){
       //rows buffer
       for(var i=0;i<this.rowsBuffer[1].length;i++){
          if(this.rowsBuffer[1][i].tagName=="TR"){

         }else{
			if (!this._onlChAttr){
				if (this._srClmn)
					out += this._manualXMLSerialize(this.rowsBuffer[1][i]);
				else
		            if(!this.xmlSerializer)//ie
		                out += this.rowsBuffer[1][i].xml;
		             else{//mozilla
		                out += this.xmlSerializer.serializeToString(this.rowsBuffer[1][i]);
		             }
			}
         }
       }
    }
   out+='</rows>';
   return out;
}
//#}
//#}

/**
*     @desc: attach event to event collection
*     @type: private
*     @topic: 0
*/
this.dhx_attachEvent=function(original,catcher){
    if ((!this[original])||(!this[original].dhx_addEvent)){
        var z=new this.dhx_eventCatcher(this);
        z.dhx_addEvent(this[original]);
        this[original]=z;
    }
    this[original].dhx_addEvent(catcher);
}
/**
*     @desc: event collection object
*     @type: private
*     @topic: 0
*/
this.dhx_eventCatcher=function(obj){
    var dhx_catch=new Array();
    var m_obj=obj;
    var z=function(){
          if (dhx_catch)
             var res=true;

       for (var i=0; i<dhx_catch.length; i++)
          if (!dhx_catch[i].apply(m_obj,arguments)) res=false;
       return res;
       }
    z.dhx_addEvent=function(ev){
            if (typeof(ev)!="function")
            ev=eval(ev);
            if (ev)
            dhx_catch[dhx_catch.length]=ev;
       }
    return z;
}

/*SET EVENT HANDLERS*/
//#events_basic:21092006{
   /**
   *     @desc: set function called when row selected
   *     @param: func - event handling function (or its name)
   *     @param: anyClick - call handler on any click event, react only on changed row by default
   *     @type: public
   *     @topic: 10
   *     @event: onRowSelect
   *     @eventdesc: Event raised immideatly after row was clicked.
   *     @eventparam:  ID of clicked row
   *     @eventparam:  index of clicked cell
   */
   this.setOnRowSelectHandler = function(func,anyClick){
        this.dhx_attachEvent("onRowSelect",func);
      this._chRRS=(!convertStringToBoolean(anyClick));
   }


   /**
   *     @desc: set function called on grid scrolling
   *     @param: func - event handling function (or its name)
   *     @type: public
   *     @topic: 10
   *     @event: onScroll
   *     @eventdesc: Event raised immideatly after scrolling occured
   *     @eventparam:  scroll left
   *     @eventparam:  scroll top
   */
   this.setOnScrollHandler = function(func){
        this.dhx_attachEvent("_onSCRL",func);
   }

   /**
   *     @desc: set function called when cell editted
   *     @param: func - event handling function (or its name)
   *     @type: public
   *     @topic: 10
   *   @event: onEditCell
   *     @eventdesc: Event raises 1-3 times depending on cell editibality.
   *     @eventparam:  stage of editting (0-before start[can be canceled if returns false],1-editor opened,2-editor closed)
   *     @eventparam:  ID or row
   *     @eventparam:  index of cell
   *     @eventparam:  new value ( only for stage 2 )
   *     @eventparam:  old value ( only for stage 2 )
   *     @returns:   for stage (0) - false - deny editing; for stag (2) - false - revert to old value, (string) - set (string) instead of new value
   */
   this.setOnEditCellHandler = function(func){
        this.dhx_attachEvent("onEditCell",func);
   }
   /**
   *     @desc: set function called when checkbox or radiobutton was clicked
   *     @param: func - event handling function (or its name)
   *     @type: public
   *     @topic: 10
   *   @event: onCheck
   *     @eventdesc: Event raises after state was changed.
   *     @eventparam:  ID or row
   *     @eventparam:  index of cell
   *     @eventparam:  state of checkbox/radiobutton
   */
   this.setOnCheckHandler = function(func){
        this.dhx_attachEvent("onCheckbox",func);
   }

   /**
   *     @desc: set function called when user press Enter
   *     @param: func - event handling function (or its name)
   *     @type: public
   *     @topic: 10
   *   @event: onEnterPressed
   *     @eventdesc: Event raised immideatly after Enter pressed.
   *     @eventparam:  ID or row
   *     @eventparam:  index of cell
   */
   this.setOnEnterPressedHandler = function(func){
        this.dhx_attachEvent("onEnter",func);
   }

   /**
   *     @desc: set function called before row removed from grid
   *     @param: func - event handling function (or its name)
   *     @type: public
   *     @topic: 10
   *      @event: onBeforeRowDeleted
   *     @eventdesc: Event raised right before row deleted (if returns false, deletion canceled)
   *     @eventparam:  ID or row
   */
   this.setOnBeforeRowDeletedHandler = function(func){
        this.dhx_attachEvent("onBeforeRowDeleted",func);
   }
   /**
   *     @desc: set function called after row added to grid
   *     @param: func - event handling function (or its name)
   *     @type: public
   *     @topic: 10
   *      @event: onRowAdded
   *     @eventdesc: Event raised right after row was added to grid
   *     @eventparam:  ID or row
   */
   this.setOnRowAddedHandler = function(func){
        this.dhx_attachEvent("onRowAdded",func);
   }

   /**
   *     @desc: set function called when row added/deleted or grid reordered
   *     @param: func - event handling function (or its name)
   *     @type: public
   *     @topic: 10
   *     @event: OnGridReconstructed
   *     @eventdesc: Event raised immideatly after row was clicked.
   *     @eventparam:  grid object
   */
   this.setOnGridReconstructedHandler = function(func){
        this.dhx_attachEvent("onGridReconstructed",func);
   }
//#}

//#__pro_feature:21092006{
//#events_adv:21092006{
/**
*     @desc: set function called moment before row selected in grid
*     @param: func - event handling function
*     @type: public
*     @edition: Professional
*     @topic: 10
*     @event:  onBeforeSelect
*     @eventdesc: event fired moment before row in grid get selection
*     @eventparam: new selected row
*     @eventparam: old selected row
*     @eventreturns: false - block selection
*/
   dhtmlXGridObject.prototype.setOnBeforeSelect=function(func){
                this.dhx_attachEvent("onBFS",func);
    };
/**
*     @desc: set function called after key pressed in grid
*     @param: func - event handling function
*     @type: public
*     @edition: Professional
*     @topic: 10
*     @event:  onKeyPress
*     @eventdesc: event fired after key pressed but before default key processing started
*     @eventparam: key code
*     @eventparam: control key flag
*     @eventparam: shift key flag
*     @eventreturns: false - block defaul key processing
*/
   dhtmlXGridObject.prototype.setOnKeyPressed=function(func){
                this.dhx_attachEvent("onKPR",func);
    };
/**
*     @desc: set function called after row created
*     @param: func - event handling function
*     @type: public
*     @edition: Professional
*     @topic: 10
*     @event:  onRowCreated
*     @eventdesc: event fired after row created in grid, and filled with data
*     @eventparam: row id
*     @eventparam: row object
*     @eventparam: related xml ( if available )
*/
   dhtmlXGridObject.prototype.setOnRowCreated=function(func){
                this.dhx_attachEvent("onRowCr",func);
    };

/**
*     @desc: set function called after xml loading/parsing ended
*     @param: func - event handling function
*     @type: public
*     @edition: Professional
*     @topic: 10
*     @event:  onXMLLoadingEnd
*     @eventdesc: event fired simultaneously with ending XML parsing, new items already available in tree
*     @eventparam: grid object
*     @eventparam: count of nodes added
*/
   dhtmlXGridObject.prototype.setOnLoadingEnd=function(func){
                this.dhx_attachEvent("onXLE",func);
    };

/**
*     @desc: set function called after value of cell changed by user actions
*     @param: func - event handling function
*     @type: public
*     @edition: Professional
*     @topic: 10
*     @event:  onCellChanged
*     @eventdesc: event fired after value was changed
*     @eventparam: row ID
*     @eventparam: cell index
*     @eventparam: old value
*     @eventparam: new value
*/
   dhtmlXGridObject.prototype.setOnCellChanged=function(func){
                this.dhx_attachEvent("_onCCH",func);
    };
/**
*     @desc: set function called before xml loading started
*     @param: func - event handling function
*     @type: public
*     @edition: Professional
*     @topic: 10
*     @event:  onXMLLoadingStart
*     @eventdesc: event fired before request for new XML sent to server
*     @eventparam: grid object
*/
   dhtmlXGridObject.prototype.setOnLoadingStart=function(func){
                this.dhx_attachEvent("onXLS",func);
    };
/**
*     @desc: set function called after resizing finished
*     @param: func - event handling function
*     @type: public
*     @edition: Professional
*     @topic: 10
*     @event:  OnResizeEnd
*     @eventdesc: event fired after resizing of column finished
*     @eventparam: grid object
*/
   dhtmlXGridObject.prototype.setOnResizeEnd=function(func){
                this.dhx_attachEvent("onRSE",func);
    };
/**
*     @desc: set function called on each resizing itteration
*     @param: func - event handling function
*     @type: public
*     @edition: Professional
*     @topic: 10
*     @event:  OnResize
*     @eventdesc: event fired on each resize itteration
*     @eventparam: cell index
*     @eventparam: cell width
*     @eventparam: grid object
*     @eventreturns: if event returns false - the resizig denied
*/
   dhtmlXGridObject.prototype.setOnResize=function(func){
                this.dhx_attachEvent("onRSI",func);
    };


/**
*     @desc: set function called before sorting of data started, didn't occur while calling grid.sortRows
*     @param: func - event handling function
*     @type: public
*     @edition: Professional
*     @topic: 10
*     @event:  onBeforeSorting
*     @eventdesc: event called before sorting of data started
*     @eventparam: index of sorted column
*     @eventparam: grid object
*     @eventparam: direction of sorting asc/desc
*     @eventreturns: if event returns false - the sorting denied
*/
   dhtmlXGridObject.prototype.setOnColumnSort=function(func){
            this.dhx_attachEvent("onCLMS",func);
        };

   /**
   *     @desc: set function called when row selection changed
   *     @param: func - event handling function (or its name)
   *     @type: public
   *     @topic: 10
   *      @edition: Professional
   *     @event: onSelectStateChanged
   *     @eventdesc: Event raised immideatly after selection state was changed
   *     @eventparam:  ID or list of IDs of selected row(s)
   */
   this.setOnSelectStateChanged = function(func){
        this.dhx_attachEvent("onSSC",func);
   }

   /**
   *     @desc: set function called when row was dbl clicked
   *     @param: func - event handling function (or its name)
   *     @type: public
   *     @topic: 10
   *      @edition: Professional
   *      @event: onRowDblClicked
   *     @eventdesc: Event raised right after row was double clicked, before cell editor opened by dbl click. If retuns false, event canceled;
   *     @eventparam:  ID or row
   *     @eventparam:  index of column
   */
   this.setOnRowDblClickedHandler = function(func){
        this.dhx_attachEvent("onRowDblClicked",func);
   }

   /**
   *     @desc: set function called when header was clicked
   *     @param: func - event handling function (or its name)
   *     @type: public
   *     @topic: 10
   *     @edition: Professional
   *     @event: onHeaderClick
   *     @eventdesc: Event raised right after header was clicked, before sorting or any other actions;
   *     @eventparam:  index of column
   *     @eventreturns: if event returns false - defaul action denied
   */
   this.setOnHeaderClickHandler = function(func){
        this.dhx_attachEvent("onHeaderClick",func);
   }



//#}
//#}


   /**
   *    @desc: returns absolute left and top position of specified element
   *    @returns: array of two values: absolute Left and absolute Top positions
   *    @param: oNode - element to get position of
   *   @type: private
   *   @topic: 8
   */
   this.getPosition = function(oNode,pNode){

                  if(!pNode)
                        var pNode = document.body

                  var oCurrentNode=oNode;
                  var iLeft=0;
                  var iTop=0;
                  while ((oCurrentNode)&&(oCurrentNode!=pNode)){//.tagName!="BODY"){
					iLeft+=oCurrentNode.offsetLeft-oCurrentNode.scrollLeft;
					iTop+=oCurrentNode.offsetTop-oCurrentNode.scrollTop;
					oCurrentNode=oCurrentNode.offsetParent;//isIE()?:oCurrentNode.parentNode;
                  }
				  if (pNode == document.body ){
				  	if (_isIE){
				  	if (document.documentElement.scrollTop)
						iTop+=document.documentElement.scrollTop;
				  	if (document.documentElement.scrollLeft)
						iLeft+=document.documentElement.scrollLeft;
						}
						else
                    	if (!_isFF){
    	                   	iLeft+=document.body.offsetLeft;
	                        iTop+=document.body.offsetTop;
						}
                 }
                     return new Array(iLeft,iTop);
               }
   /**
   *   @desc: gets nearest parent of specified type
   *   @param: obj - input object
   *   @param: tag - string. tag to find as parent
   *   @returns: object. nearest paraent object (including spec. obj) of specified type.
   *   @type: private
   *   @topic: 8
   */
   this.getFirstParentOfType = function(obj,tag){
      while(obj.tagName!=tag && obj.tagName!="BODY"){
         obj = obj.parentNode;
      }
      return obj;
   }

/*METHODS deprecated*/
   /**
   *   @desc: deprecated. sets number of columns
   *   @param: cnt - number of columns
   *   @type: void
   *   @topic: 3,7
   */
   this.setColumnCount = function(cnt){alert('setColumnCount method deprecated')}
   /**
   *   @desc: deprecated. repaint of the grid
   *   @topic: 7
   *   @type: void
   */
   this.showContent = function(){alert('showContent method deprecated')}

/*INTERNAL EVENT HANDLERS*/
      this.objBox.onscroll = new Function("","this.grid._doOnScroll()")
    if ((!_isOpera)||(_OperaRv>8.5))
    {
   this.hdr.onmousemove = new Function("e","this.grid.changeCursorState(e||window.event)");
      this.hdr.onmousedown = new Function("e","this.grid.startColResize(e||window.event)");
    }
   this.obj.onmousemove = this._drawTooltip;
   this.obj.onclick = new Function("e","this.grid._doClick(e||window.event); if (this.grid._sclE) this.grid.editCell(e||window.event);  (e||event).cancelBubble=true; ProcessMouseClick(); ");
   this.entBox.onmousedown = new Function("e","return this.grid._doContClick(e||window.event);");
   this.obj.ondblclick = new Function("e","if(!this.grid.wasDblClicked(e||window.event)){return false}; if (this.grid._dclE) this.grid.editCell(e||window.event);  (e||event).cancelBubble=true; ProcessMouseDblClick();");
   this.hdr.onclick = this._onHeaderClick;
   this.hdr.ondblclick = this._onHeaderDblClick;

   //VOID this.grid.ondblclick = this.onDoubleClick;
    if (!document.body._dhtmlxgrid_onkeydown){
		dhtmlxEvent(document,"keydown",new Function("e","if (globalActiveDHTMLGridObject) return globalActiveDHTMLGridObject.doKey(e||window.event); return true;"));
    	document.body._dhtmlxgrid_onkeydown=true;
    }

	dhtmlxEvent(document.body,"click",function(){ if (self.editStop) self.editStop(); return true;});

//nb:document.body.attachEvent("onclick",new Function("","if(this.document.getElementById('"+this.entBox.id+"').grid.isActive==-1)this.document.getElementById('"+this.entBox.id+"').grid.setActive(false)"))
   //activity management
    this.entBox.onbeforeactivate = new Function("","this.grid.setActive(); event.cancelBubble=true;");
   this.entBox.onbeforedeactivate = new Function("","this.grid.isActive=-1; event.cancelBubble=true;");
   //postprocessing events (method can be redeclared to react on some events during processing)
   this.doOnRowAdded = function(row){};
  return this;
}


   /**
   *   @desc: detect is current grid is a treeGrid
   *   @type: private
   *   @topic: 2
   */
   dhtmlXGridObject.prototype.isTreeGrid=    function(){
        return (this.cellType._dhx_find("tree")!=-1);
    }

   /**
   *   @desc: adds row to the specified position
   *   @param: new_id - id for new row
   *   @param: text - Array of values or String(with delimiter as in delimiter parameter)
   *   @param: [ind] - index of row (0 by default)
   *   @returns: new row dom object
   *   @type: public
   *   @topic: 2
   */
   dhtmlXGridObject.prototype.addRow   =    function(new_id,text,ind){
		var r =  this._addRow(new_id,text,ind);
	  	if(this.onRowAdded)
			this.onRowAdded(new_id);
		if (this.onRowCr)
			this.onRowCr(r.idd,r,null);
		if(this.pagingOn)
			this.changePage(this.currentPage)

		this.setSizes();
		return r;
   }


      /**
      *   @desc: first step of add row,  created a TR
      *   @type: private
      */
    dhtmlXGridObject.prototype._prepareRow=function(new_id){
                                var r=document.createElement("TR");
                        r.idd = new_id;
                        r.grid = this;

                                for(var i=0;i<this.hdr.rows[0].cells.length;i++){
                            var c = document.createElement("TD");
                                    //#cell_id:11052006{
                                    if (this._enbCid) c.id="c_"+r.idd+"_"+i;
                                    //#}
                                    c._cellIndex = i;
                                    if (this.dragAndDropOff) this.dragger.addDraggableItem(c,this);
                           c.align = this.cellAlign[i];
                           c.style.verticalAlign = this.cellVAlign[i];
                           //add color to column
                           c.bgColor = this.columnColor[i] || "";

//#__pro_feature:21092006{
//#column_hidden:21092006{
                                    if ((this._hrrar)&&(this._hrrar[i]))
                                c.style.display="none";
//#}
//#}


                                    r.appendChild(c);
                                }
                                return r;
    }
      /**
      *   @desc: second step of add row,  fill tr with data
      *   @type: private
      */
    dhtmlXGridObject.prototype._fillRow=function(r,text){
        if (!this._parsing_) this.editStop();

        this.math_off=true;
        this.math_req=false;

        if(typeof(text)!='object')
        	text = (text||"").split(this.delim);
        for(var i=0; i<r.childNodes.length; i++){
			if((i<text.length)||(this.defVal[i])){
			var val = text[i]
				if ((this.defVal[i])&&((val=="")||(val===window.undefined)))
            		val = this.defVal[i];

			if (this._dload)
                  this.editor = this.cells3(r,r.childNodes[i]._cellIndex);
			else
                  this.editor = this.cells4(r.childNodes[i]);

                       //this.editor=this.cells4(r.childNodes[i]);
              this.editor.setValue(val)
              this.editor = this.editor.destructor();
           }else{
              var val = "&nbsp;";
              r.childNodes[i].innerHTML = val;
                       r.childNodes[i]._clearCell=true;
           }
        }
        this.math_off=false;
        if ((this.math_req)&&(!this._parsing_)){
               for(var i=0;i<this.hdr.rows[0].cells.length;i++)
                    this._checkSCL(r.childNodes[i]);
            this.math_req=false;
        }
        return r;
    }

      /**
      *   @desc: third step of add row,  attach TR to DOM
      *   @type: private
      */
    dhtmlXGridObject.prototype._insertRowAt=function(r,ind,skip){
                            if (ind<0) ind=this.rowsCol.length;

                     if ((arguments.length<2)||(ind===window.undefined))
                        ind = this.rowsCol.length//getRowsNum();
                     else{
                        if(ind>this.rowsCol.length)
                           ind = this.rowsCol.length;
                     }

                            if (!skip)
                            if ((ind==(this.obj.rows.length-1))||(!this.rowsCol[ind]))
                                if (_isKHTML)
                                    this.obj.appendChild(r);
                                else{
                                    this.obj.firstChild.appendChild(r);
                                    }
                            else
                                {
                                this.rowsCol[ind].parentNode.insertBefore(r,this.rowsCol[ind]);
                                }


                     this.rowsAr[r.idd] = r;
                     this.rowsCol._dhx_insertAt(ind,r);

                            if (this._cssEven){
                                if (ind%2==1) r.className+=" "+this._cssUnEven;
                                else r.className+=" "+this._cssEven;

                                if (ind!=(this.rowsCol.length-1))
                                    this._fixAlterCss(ind+1);
                            }

                     //this.chngCellWidth(ind)
                        this.doOnRowAdded(r);

                            //bad code, need to be rethinked
                            if ((this.math_req)&&(!this._parsing_)){
                                for(var i=0;i<this.hdr.rows[0].cells.length;i++)
                                   this._checkSCL(r.childNodes[i]);
                                this.math_req=false;
                            }

                            return r;
    }

   /**
   *   @desc: adds row to the specified position
   *   @param: new_id - id for new row
   *   @param: text - Array of values or String(with delimiter as in delimiter parameter)
   *   @param: [ind] - index of row (0 by default)
   *   @returns: new row dom object
   *   @type: private
   *   @topic: 2
   */
   dhtmlXGridObject.prototype._addRow   =    function(new_id,text,ind){
                       var row = this._fillRow(this._prepareRow(new_id),text);
                  if(ind>this.rowsCol.length && ind<(this.rowsCol.length+this.rowsBuffer[0].length)){
                     var inBufInd = ind - this.rowsCol.length;
                     this.rowsBuffer[0]._dhx_insertAt(inBufInd,new_id);
                     this.rowsBuffer[1]._dhx_insertAt(inBufInd,row);
                     return row;
                  }
                  return this._insertRowAt(row,ind);
               }

/**
*   @desc: hide/show row (warning! - this command doesn't affect row indexes, only visual appearance)
*   @param: ind - column index
*   @param: state - true/false - hide/show column
*   @type:  public
*/
dhtmlXGridObject.prototype.setRowHidden=function(id,state){
    var f=convertStringToBoolean(state);
    //var ind=this.getRowIndex(id);
    //if (id<0)
   //   return;
    var row= this.getRowById(id)//this.rowsCol[ind];
   if(!row)
      return;

    if (row.expand==="")
        this.collapseKids(row);

    if ((state)&&(row.style.display!="none")){
        row.style.display="none";
        var z=this.selectedRows._dhx_find(row);
        if (z!=-1){
          row.className=row.className.replace("rowselected","");
            for (var i=0; i<row.childNodes.length; i++)
                row.childNodes[i].className=row.childNodes[i].className.replace(/cellselected/g,"");
             this.selectedRows._dhx_removeAt(z);
            }
           if (this.onGridReconstructed)
            this.onGridReconstructed();
        }

    if ((!state)&&(row.style.display=="none")){
        row.style.display="";
        if (this.onGridReconstructed) this.onGridReconstructed();
        }

}

//#__pro_feature:21092006{
//#column_hidden:21092006{
/**
*   @desc: hide/show column
*   @param: ind - column index
*   @param: state - true/false - hide/show column
*   @type:  public
*   @edition: Professional
*/
	dhtmlXGridObject.prototype.setColumnHidden=function(ind,state){
    if ((this.fldSorted)&&(this.fldSorted.cellIndex==ind)&&(state))
            this.sortImg.style.display = "none";

    var f=convertStringToBoolean(state);
    if (f){
        if (!this._hrrar) this._hrrar=new Array();
        else if (this._hrrar[ind]) return;
            this._hrrar[ind]="display:none;";
            this._hideShowColumn(ind,"none");
    }
    else
    {
        if ((!this._hrrar)||(!this._hrrar[ind])) return;
        this._hrrar[ind]="";
        this._hideShowColumn(ind,"");
    }

    if ((this.fldSorted)&&(this.fldSorted.cellIndex==ind)&&(!state))
            this.sortImg.style.display = "inline";
}



/**
*   @desc: get show/hidden status of column
*   @param: ind - column index
*   @type:  public
*   @edition: Professional
*   @returns:  if column hidden then true else false
*/
dhtmlXGridObject.prototype.isColumnHidden=function(ind){
     if ((this._hrrar)&&(this._hrrar[ind])) return true;
     return false;
}


/**
*   @desc: set list of visible columns
*   @param: list - list of true/false separated by comma
*   @type:  public
*   @edition: Professional
*   @topic:0
*/
dhtmlXGridObject.prototype.setColHidden=function(list){
    if (list) this._ivizcol=list.split(",");
   if (this.hdr.rows.length)
   for (var i=0; i<this._ivizcol.length; i++)
       this.setColumnHidden(i,this._ivizcol[i]);
}

      /**
      *   @desc: fix hidden state for column in all rows
      *   @type: private
      */
dhtmlXGridObject.prototype._fixHiddenRowsAll=function(ind,state){
        var z=this.obj.rows.length;
        for (var i=0; i<z; i++)
            this.obj.rows[i].cells[ind].style.display=state;
        }
/**
*   @desc: hide column
*   @param: ind - column index
*   @param: state - hide/show
*   @edition: Professional
*   @type:  private
*/
dhtmlXGridObject.prototype._hideShowColumn=function(ind,state){
	var hind=ind;
	if ((this.hdr.rows[1]._childIndexes)&&(this.hdr.rows[1]._childIndexes[ind]!=ind))
		hind=this.hdr.rows[1]._childIndexes[ind];

    if (state=="none"){
        this.hdr.rows[0].cells[ind]._oldWidth = this.hdr.rows[0].cells[ind].style.width;
        this.hdr.rows[0].cells[ind]._oldWidthP = this.cellWidthPC[ind];
        this.obj.rows[0].cells[ind].style.width = "0px";
        this._fixHiddenRowsAll(ind,"none");

        if (_isOpera||_isKHTML) {
			this.hdr.rows[0].cells[ind].style.display="none";
			this.hdr.rows[1].cells[ind].style.display="none";
			}

	        this.hdr.rows[1].cells[hind].style.whiteSpace="nowrap";

            if (this.cellWidthPX[ind]) this.cellWidthPX[ind]=0;
            if (this.cellWidthPC[ind]) this.cellWidthPC[ind]=0;
        }
    else {
        if (this.hdr.rows[0].cells[ind]._oldWidth){
        var zrow=this.hdr.rows[0].cells[ind];
             if (_isOpera||_isKHTML){
            	 this.hdr.rows[0].cells[ind].style.display="";
            	 this.hdr.rows[1].cells[ind].style.display="";
				 }


             this.obj.rows[0].cells[ind].style.width = this.hdr.rows[0].cells[ind]._oldWidth;
             this._fixHiddenRowsAll(ind,"");

             zrow.style.width = zrow._oldWidth;

             this.hdr.rows[1].cells[hind].style.whiteSpace = "normal";
             if (zrow._oldWidthP) this.cellWidthPC[ind]=zrow._oldWidthP;
             if (zrow._oldWidth) this.cellWidthPX[ind]=parseInt(zrow._oldWidth);
        }
    }
    this.setSizes();

    if ((!_isIE)&&(!_isFF))
    {
    //dummy Opera/Safari fix
    this.obj.border=1;
    this.obj.border=0;
    }

}

//#}

//#colspan:20092006{
/**
*   @desc: enable/disable colspan support
*   @param: mode - true/false
*   @type:  public
*   @edition: Professional
*/
 dhtmlXGridObject.prototype.enableCollSpan=function(mode){
    this._ecspn=convertStringToBoolean(mode);
 }
 //#}

 //#}
/**
*   @desc: enable/disable hovering row under mouse
*   @param: mode - true/false
*   @param: cssClass - css class for hovered row
*   @type:  public
*/
dhtmlXGridObject.prototype.enableRowsHover = function(mode,cssClass){
    this._hvrCss=cssClass;
    if (convertStringToBoolean(mode)){
        if (!this._elmnh){
             this.obj._honmousemove=this.obj.onmousemove;
             this.obj.onmousemove=this._setRowHover;
             if (_isIE)
                this.obj.onmouseleave=this._unsetRowHover;
             else
                 this.obj.onmouseout=this._unsetRowHover;

             this._elmnh=true;
        }
    } else {
        if (this._elmnh){
             this.obj.onmousemove=this.obj._honmousemove;
             if (_isIE)
                this.obj.onmouseleave=null;
             else
                 this.obj.onmouseout=null;

             this._elmnh=false;
        }
    }
};

/**
*   @desc: enable/disable events which fire excell editing, mutual exclusive with enableLightMouseNavigation
*   @param: click - true/false - enable/disable editing by single click
*   @param: dblclick - true/false - enable/disable editing by double click
*   @param: f2Key - enable/disable editing by pressing F2 key
*   @type:  public
*/
dhtmlXGridObject.prototype.enableEditEvents = function(click, dblclick, f2Key){
         this._sclE = convertStringToBoolean(click);
         this._dclE = convertStringToBoolean(dblclick);
         this._f2kE = convertStringToBoolean(f2Key);
}


/**
*   @desc: enable/disable light mouse navigation mode (row selection with mouse over, editing with single click), mutual exclusive with enableEditEvents
*   @param: mode - true/false
*   @type:  public
*/
dhtmlXGridObject.prototype.enableLightMouseNavigation = function(mode){
    if (convertStringToBoolean(mode)){
        if (!this._elmn){
             this.entBox._onclick=this.entBox.onclick;
               this.entBox.onclick = function () {return true; };

             this.obj.onclick=function (e){
                 var c = this.grid.getFirstParentOfType(e?e.target:event.srcElement,'TD');
                 this.grid.editStop();
                 this.grid.doClick(c);
	             this.grid.editCell();
				 (e||event).cancelBubble=true;
                 }

             this.obj._onmousemove=this.obj.onmousemove;
             this.obj.onmousemove=this._autoMoveSelect;
             this._elmn=true;
        }
    } else {
        if (this._elmn){
               this.entBox.onclick = this.entBox._onclick;
             this.obj.onclick=function(){ return true };
             this.obj.onmousemove=this.obj._onmousemove;
             this._elmn=false;
        }
    }
}


/**
*   @desc: remove hower state on row
*   @type:  private
*/
dhtmlXGridObject.prototype._unsetRowHover = function(e,c){
        if (c) that=this; else that=this.grid;

        if ((that._lahRw)&&(that._lahRw!=c)){
         for(var i=0;i<that._lahRw.childNodes.length;i++)
                that._lahRw.childNodes[i].className=that._lahRw.childNodes[i].className.replace(that._hvrCss,"");
            that._lahRw=null;
        }
}

/**
*   @desc: set hower state on row
*   @type:  private
*/
dhtmlXGridObject.prototype._setRowHover = function(e){
        var c = this.grid.getFirstParentOfType(e?e.target:event.srcElement,'TD');
        if (c) {
            this.grid._unsetRowHover(0,c);
            c=c.parentNode;
         for(var i=0;i<c.childNodes.length;i++)
                c.childNodes[i].className+=" "+this.grid._hvrCss;
            this.grid._lahRw=c;
        }
        this._honmousemove(e);
}

/**
*   @desc: onmousemove, used in light mouse navigaion mode
*   @type:  private
*/
dhtmlXGridObject.prototype._autoMoveSelect = function(e){
    //this - grid.obj
    if(!this.grid.editor)
    {
        var c = this.grid.getFirstParentOfType(e?e.target:event.srcElement,'TD');
		if (c.parentNode.idd)
	        this.grid.doClick(c,true,0);
    }
    this._onmousemove(e);
}

//#__pro_feature:21092006{
//#distrb_parsing:21092006{
/**
*   @desc: enable/disable light mouse navigation mode
*   @param: mode - true/false
*   @param: count - count of nodes parsed by one step (the 10 by default)
*   @param: time - time between parsing counts in milli seconds (the 250 by default)
*   @type:  public
*   @edition: Professional
*/
dhtmlXGridObject.prototype.enableDistributedParsing = function(mode,count,time){
    count=count||10;
    time=time||250;
    if (convertStringToBoolean(mode)){
        this._ads_count=count;
        this._ads_time=time;
    }
    else  this._ads_count=0;
}


/**
*   @desc:  call function from saved context, used in distributed parsing
*   @type:  private
*/
function _contextCall(obj,name,rowsCol,startIndex,tree,pId,i,n){
    window.setTimeout(function(){
        var res=obj[name](rowsCol,startIndex,tree,pId,i);
        if ((obj.onXLE)&&(res!=-1))
            obj.onXLE(obj,obj.rowsCol.length);
    },n);
    return this;
}
//#}
//#}
      /**
          *     @desc: destructor, cleans used memory
          *     @type: public
        *     @topic: 0
          */
dhtmlXGridObject.prototype.destructor=function(){
    var a;
    this.xmlLoader=this.xmlLoader.destructor();
    for (var i=0; i<this.rowsCol.length; i++)
        if (this.rowsCol[i]) this.rowsCol[i].grid=null;
    for (i in this.rowsAr)
        if (this.rowsAr[i]) this.rowsAr[i]=null;

    this.rowsCol=new dhtmlxArray();
    this.rowsAr=new Array();
    this.entBox.innerHTML="";
   this.entBox.onclick = function(){};
   this.entBox.onmousedown = function(){};
   this.entBox.onbeforeactivate = function(){};
   this.entBox.onbeforedeactivate = function(){};
   this.entBox.onbeforedeactivate = function(){};

    for (a in this){
        if ((this[a])&&(this[a].m_obj))
            this[a].m_obj=null;
        this[a]=null;
        }


    if (this==globalActiveDHTMLGridObject)
        globalActiveDHTMLGridObject=null;
//	self=null;
    return null;
}





/**
*     @desc: get sorting state of grid
*     @type: public
*     @returns: array, first element is index of sortef column, second - direction of sorting.
*     @topic: 0
*/
   dhtmlXGridObject.prototype.getSortingState=function(){
                var z=new Array();
                if (this.fldSorted){
                    z[0]=this.fldSorted._cellIndex;
                    z[1]=(this.sortImg.src.indexOf("sort_desc.gif")!=-1)?"DES":"ASC";
                }
                return z;
    };

/**
*     @desc: enable autoheight of grid
*     @param: mode - true/false
*     @param: maxHeight - maximum height before scrolling appears (infinite by default)
*     @type: public
*     @topic: 0
*/
   dhtmlXGridObject.prototype.enableAutoHeigth=function(mode,maxHeight){
        this._ahgr=convertStringToBoolean(mode);
        this._ahgrM=maxHeight||null;
      if (maxHeight=="auto")
         {
         this._ahgrM=null;
         this._ahgrMA=true;
         this._activeResize();
         }
    };

/**
*     @desc: enable stable sorting (slow)
*     @param: mode - true/false
*     @type: public
*     @topic: 0
*/
   dhtmlXGridObject.prototype.enableStableSorting=function(mode){
        this._sst=convertStringToBoolean(mode);
		this.rowsCol.stablesort=function(cmp){
			for (var i=0; i<this.length-1; i++)
				for (var j=i; j<this.length; j++)
					if (cmp(this[i],this[j])){
						var temp=this[j];
						this[j]=this[i];
						this[i]=temp;
					}
		}
    };

/**
*     @desc: enable/disable hot keys in grid
*     @param: mode - true/false
*     @type: public
*     @topic: 0
*/
   dhtmlXGridObject.prototype.enableKeyboardSupport=function(mode){
        this._htkebl=!convertStringToBoolean(mode);
    };


/**
*     @desc: enable/disable context menu
*     @param: menu object, if null - context menu will be disabled
*     @type: public
*     @topic: 0
*/
   dhtmlXGridObject.prototype.enableContextMenu=function(menu){
        this._ctmndx=menu;
    };
/**
*     @desc: set event handler, which will be called immideatly before showing context menu
*     @param: func - user defined function
*     @type: public
*     @event: OnBeforeContextMenu
*     @eventdesc: Event raised immideatly before showing context menu
*     @eventparam:  ID of clicked row
*     @eventparam:  index of cell column
*     @eventparam:  grid object
*     @eventreturns: if event returns false, then context menu is not shown
*     @topic: 0,10
*/
   dhtmlXGridObject.prototype.setOnBeforeContextMenu=function(func){
            this.dhx_attachEvent("onBCM",func);
    };

/**
*     @desc: set event handler, which will be called immideatly after right mouse button click on grid row
*     @param: func - user defined function
*     @type: public
*     @event: OnRightClick
*     @eventdesc: Event raised immideatly after right mouse button clicked on grid row
*     @eventparam:  ID of clicked row
*     @eventparam:  index of cell column
*     @eventparam:  event object
*     @eventreturns: if event returns false, then dhtmlxMenu integration disabled
*     @topic: 0,10
*/
dhtmlXGridObject.prototype.setOnRightClick=function(func){
	this.dhx_attachEvent("onRCL",func);
};



/**
*     @desc: set width of browser scrollbars, will be used for correct autoWidth calculations (by default grid will use 16 for IE and 19 for FF)
*     @param: width - scrollbar width
*     @type: public
*     @topic: 0
*/
   dhtmlXGridObject.prototype.setScrollbarWidthCorrection=function(width){
        this._scrFix=parseInt(width);
    };

/**
*     @desc: enable/disable tooltips for specified colums
*     @param: list - list of true/false values, tooltips enabled for all columns by default
*     @type: public
*     @topic: 0
*/
   dhtmlXGridObject.prototype.enableTooltips=function(list){
        this._enbTts=list.split(",");
        for (var i=0; i<this._enbTts.length; i++)
            this._enbTts[i]=convertStringToBoolean(this._enbTts[i]);
    };


/**
*     @desc: enable/disable resizing for specified colums
*     @param: list - list of true/false values, resizing enabled for all columns by default
*     @type: public
*     @topic: 0
*/
   dhtmlXGridObject.prototype.enableResizing=function(list){
        this._drsclmn=list.split(",");
        for (var i=0; i<this._drsclmn.length; i++)
            this._drsclmn[i]=convertStringToBoolean(this._drsclmn[i]);
    };

/**
*     @desc: set minimum column width ( works only for resizing )
*     @param: width - minimum column width, can be set for specified column, or as comma separated list for all columns
*     @param: ind - column index
*     @type: public
*     @topic: 0
*/
   dhtmlXGridObject.prototype.setColumnMinWidth=function(width,ind){
        if (arguments.length==2){
            if (!this._drsclmW) this._drsclmW=new Array();
            this._drsclmW[ind]=width;
            }
        else
            this._drsclmW=width.split(",");
    };


//#cell_id:11052006{
/**
*     @desc: enable/disable unique id for cells (c_RowId_colIndex)
*     @param: mode - true/false - enable/disable
*     @type: public
*     @topic: 0
*/
   dhtmlXGridObject.prototype.enableCellIds=function(mode){
        this._enbCid=convertStringToBoolean(mode);
    };
//#}



//#locked_row:11052006{
/**
*     @desc: lock/unlock row for editing
*     @param: rowId - id of row
*     @param: mode - true/false - lock/unlock
*     @type: public
*     @topic: 0
*/
   dhtmlXGridObject.prototype.lockRow=function(rowId,mode){
        var z=this.getRowById(rowId);
        if (z) {
            z._locked=convertStringToBoolean(mode);
            if ((this.cell)&&(this.cell.parentNode.idd==rowId))
                this.editStop();
            }
    };
//#}

/**
*   @desc:  get values of all cells in row
*   @type:  private
*/
   dhtmlXGridObject.prototype._getRowArray=function(row){
        var text=new Array();
        for (var ii=0; ii<row.childNodes.length; ii++)
            text[ii]=this.cells3(row,ii).getValue();
        return text;
        }

//#__pro_feature:21092006{
//#data_format:12052006{
/**
*     @desc: set mask for formatting date
*     @param: mask - date mask, d,m,y will mean day,month,year; for example d/m/y - 22/05/1985
*     @type: public
*     @edition: Professional
*     @topic: 0
*/
   dhtmlXGridObject.prototype.setDateFormat=function(mask){
        this._dtmask=mask;
    }

/**
*     @desc: set mask for formatting numeric data
*     @param: mask - numeric mask; for example 0,000.00 - 1,234.56
*     @param: cInd - column index
*     @type: public
*     @edition: Professional
*     @topic: 0
*/
   dhtmlXGridObject.prototype.setNumberFormat=function(mask,cInd,p_sep,d_sep){
            var nmask=mask.replace(/[^0\,\.]*/g,"");
            var pfix=nmask.indexOf(".");
            if (pfix>-1) pfix=nmask.length-pfix-1;
            var dfix=nmask.indexOf(",");
            if (dfix>-1) dfix=nmask.length-pfix-2-dfix;

            p_sep=p_sep||".";
            d_sep=d_sep||",";
            var pref=mask.split(nmask)[0];
            var postf=mask.split(nmask)[1];
            this._maskArr[cInd]=[pfix,dfix,pref,postf,p_sep,d_sep];
    }
/**
*   @desc:  convert formated value to original
*   @type:  private
*/
    dhtmlXGridObject.prototype._aplNFb=function(data,ind){
            var a=this._maskArr[ind];
            if (!a) return data;

            var ndata=parseFloat(data.toString().replace(/[^0-9]*/g,""));
            if (data.toString().substr(0,1)=="-") ndata=ndata*-1;
            if (a[0]>0) ndata=ndata/Math.pow(10,a[0]);
            return ndata;
    }

/**
*   @desc:  format data with mask
*   @type:  private
*/
   dhtmlXGridObject.prototype._aplNF=function(data,ind){
            var a=this._maskArr[ind];
            if (!a) return data;

            var c=(parseFloat(data)<0?"-":"")+a[2];
            data = Math.abs(Math.round(parseFloat(data)*Math.pow(10,a[0]>0?a[0]:0))).toString();
            data=(data.length<a[0]?Math.pow(10,a[0]+1-data.length).toString().substr(1,a[0]+1)+data.toString():data).split("").reverse();
            data[a[0]]=(data[a[0]]||"0")+a[4];
            if (a[1]>0)  for (var j=(a[0]>0?0:1)+a[0]+a[1]; j<data.length; j+=a[1]) data[j]+=a[5];
            return c+data.reverse().join("")+a[3];
    }
//#}


//#config_from_xml:20092006{

/**
*   @desc:  configure grid structure from XML
*   @type:  private
*/
      dhtmlXGridObject.prototype._launchCommands = function(arr){
			for (var i=0; i<arr.length; i++){
				var args=new Array();
				for (var j=0; j<arr[i].childNodes.length; j++)
					if (arr[i].childNodes[i].nodeType==1)
						args[args.length]=arr[i].childNodes[j].firstChild.data;
				this[arr[i].getAttribute("command")].apply(this,args);
			}
	  }


/**
*   @desc:  configure grid structure from XML
*   @type:  private
*/
      dhtmlXGridObject.prototype._parseHead = function(hheadCol){
                          var headCol = this.xmlLoader.doXPath("//rows/head/column",hheadCol[0]);
                                var asettings = this.xmlLoader.doXPath("//rows/head/settings",hheadCol[0]);
                                var awidthmet="setInitWidths";
                                var split=false;

                                if (asettings[0]){
                                    for (var s=0; s<asettings[0].childNodes.length; s++)
                                        switch (asettings[0].childNodes[s].tagName){
                                            case "colwidth":
                                                if (asettings[0].childNodes[s].firstChild && asettings[0].childNodes[s].firstChild.data=="%")
                                                    awidthmet="setInitWidthsP";
                                                break;
                                            case "splitat":
                                                split=(asettings[0].childNodes[s].firstChild?asettings[0].childNodes[s].firstChild.data:false);
                                                break;
                                        }
                                }
						  this._launchCommands(this.xmlLoader.doXPath("//rows/head/beforeInit/call",hheadCol[0]));
                          if(headCol.length>0){
                                      var a_list="";var b_list="";var c_list="";
                                      var d_list="";var e_list="";var f_list="";
									  var f_arr=[];
                                      for (var i=0; i<headCol.length; i++){
                                          a_list+=headCol[i].getAttribute("width")+",";
                                          b_list+=headCol[i].getAttribute("type")+",";
                                          c_list+=headCol[i].getAttribute("align")+",";
                                          d_list+=headCol[i].getAttribute("sort")+",";
                                          e_list+=(headCol[i].getAttribute("color")!=null?headCol[i].getAttribute("color"):"")+",";
                                          f_list+=(headCol[i].firstChild?headCol[i].firstChild.data:"").replace(/^\s*((.|\n)*.+)\s*$/gi,"$1")+",";
										  f_arr[i]=headCol[i].getAttribute("format");
                                      }
                                      this.setHeader(f_list.substr(0,f_list.length-1));
                                      this[awidthmet](a_list.substr(0,a_list.length-1));
                                      this.setColAlign(c_list.substr(0,c_list.length-1));
                                      this.setColTypes(b_list.substr(0,b_list.length-1));
                                      this.setColSorting(d_list.substr(0,d_list.length-1));
                                      this.setColumnColor(e_list.substr(0,e_list.length-1));

                                       for (var i=0; i<headCol.length; i++){
                                         if ((this.cellType[i].indexOf('co')==0)||(this.cellType[i]=="clist")){
                                           var optCol = this.xmlLoader.doXPath("./option",headCol[i]);
			                                 if (optCol.length){
			                                    var resAr=new Array();
			                                    if (this.cellType[i]=="clist"){
			                                                  for (var j=0;j<optCol.length; j++)
			                                          resAr[resAr.length]=optCol[j].firstChild?optCol[j].firstChild.data:"";
			                                       this.registerCList(i,resAr);
			                                       }
			                                    else{
			                                       var combo=this.getCombo(i);
			                                                    for (var j=0;j<optCol.length; j++)
			                                          combo.put(optCol[j].getAttribute("value"),optCol[j].firstChild?optCol[j].firstChild.data:"");
			                                                  }
			                                 }
                                       }
									   else
                                          if (f_arr[i])
                                             if ((this.cellType[i]=="calendar")||(this.fldSort[i]=="date"))
											 	this.setDateFormat(f_arr[i],i);
											 else
											 	this.setNumberFormat(f_arr[i],i);
                                   }


                                       this.init();
                                       if ((split)&&(this.splitAt)) this.splitAt(split);
				}
				this._launchCommands(this.xmlLoader.doXPath("//rows/head/afterInit/call",hheadCol[0]));
        }
//#}

//#}
   /**
      *    @desc: populate grid with data from xml file (if passed parameter contains '.') or island
      *    @param: [xml] - xml island will be found by grid id (id_xml) if no file or island id is specified. This also can be ready-to-use XML object
      *   @param: [startIndex] - index of row in grid to start insertion from
      *   @type: public
      *   @topic: 2,5
      */
      dhtmlXGridObject.prototype.parseXML = function(xml,startIndex){
                   this._xml_ready=true;
                   var pid=null;
                   var zpid=null;
                        if(!xml)
                           try{
                              var xmlDoc = eval(this.entBox.id+"_xml").XMLDocument;
                           }catch(er){
                              var xmlDoc = this.loadXML(this.xmlFileUrl)
                           }
                        else{
                           if(typeof(xml)=="object"){
                              var xmlDoc = xml;
                           }else{
                              if(xml.indexOf(".")!=-1){
                                 if(this.xmlFileUrl=="")
                                    this.xmlFileUrl = xml
                                 var xmlDoc = this.loadXML(xml)
                                            return;
                              }else
                                 var xmlDoc = eval(xml).XMLDocument;
                           }
                        }





                        var ar = new Array();
                        var idAr = new Array();
//#__pro_feature:21092006{
//#config_from_xml:20092006
                                //head
                        var hheadCol = this.xmlLoader.doXPath("//rows/head",xmlDoc);
                                if (hheadCol.length)
                                    this._parseHead(hheadCol);
//#)
//#}


                        var tree=this.cellType._dhx_find("tree");
                        var rowsCol = this.xmlLoader.doXPath("//rows/row",xmlDoc);
                        if(rowsCol.length==0){
                        	this.recordsNoMore = true;
							var top=this.xmlLoader.doXPath("//rows",xmlDoc);
                            var pid=(top[0].getAttribute("parent")||0);
		                    if ((tree!=-1)&&(this.rowsAr[pid])){
								var tree_r=this.rowsAr[pid].childNodes[tree];
						    	tree_r.innerHTML=tree_r.innerHTML.replace(/\/(plus)\.gif/,"/blank.gif");
							}
                        }
                                else{
                                    pid=(rowsCol[0].parentNode.getAttribute("parent")||null);
                                    zpid=this.getRowById(pid);
                                    if (zpid) zpid._xml_await=false;
                                    else pid=null;
                                    startIndex=this.getRowIndex(pid)+1;
                                }

                        //global(grid) user data
                        var gudCol = this.xmlLoader.doXPath("//rows/userdata",xmlDoc);
                        if(gudCol.length>0){
                           this.UserData["gridglobaluserdata"] = new Hashtable();
                           for(var j=0;j<gudCol.length;j++){
                              this.UserData["gridglobaluserdata"].put(gudCol[j].getAttribute("name"),gudCol[j].firstChild?gudCol[j].firstChild.data:"");
                           }
                        }

                        //rows
                                if (tree==-1) tree=this.cellType._dhx_find("3d");
                                if (this._innerParse(rowsCol,startIndex,tree,pid)==-1) return;
                                if (zpid) this.expandKids(zpid);

                        if(this.dynScroll && this.dynScroll!='false'){
                             this.doDynScroll()
                        }

                                if (tree!=-1){
                                  var oCol = this.xmlLoader.doXPath("//row[@open]",xmlDoc);
                                for (var i=0; i<oCol.length; i++)
                                    this.openItem(oCol[i].getAttribute("id"));
                                    }

                              this.setSizes();
                            if (_isOpera){
                                this.obj.style.border=1;
                                this.obj.style.border=0;
                                }
                            this._startXMLLoading=false;



                            if (this.onXLE)
                              this.onXLE(this,rowsCol.length);
                  }
/**
*   @desc:  add additional attributes to row, based on XML attributes
*   @type:  private
*/
         dhtmlXGridObject.prototype._postRowProcessing=function(r,xml){
               var rId = xml.getAttribute("id")
               var xstyle = xml.getAttribute("style");

                //user data
              var udCol = this.xmlLoader.doXPath("./userdata",xml);
              if(udCol.length>0){
                 this.UserData[rId] = new Hashtable();
                 for(var j=0;j<udCol.length;j++){
                    this.UserData[rId].put(udCol[j].getAttribute("name"),udCol[j].firstChild?udCol[j].firstChild.data:"");
                 }
              }

                //#td_tr_classes:06062006{
                var css1=xml.getAttribute("class");
                if (css1) r.className+=" "+css1;
                //#}

                //#locked_row:11052006{
                if (xml.getAttribute("locked"))
                {
                    r._locked=true;
                }
                //#}


                //select row
                if(xml.getAttribute("selected")==true){
                    this.setSelectedRow(rId,this.selMultiRows,false,xml.getAttribute("call")==true)
                }
                //expand row
                if(xml.getAttribute("expand")=="1"){
                    r.expand = "";
                }

                if (xstyle) this.setRowTextStyle(rId,xstyle);

				if (this.onRowCr) this.onRowCr(r.idd,r,xml);
         }
/**
*   @desc:  fill row with data from XML
*   @type:  private
*/
         dhtmlXGridObject.prototype._fillRowFromXML=function(r,xml,tree,pId){
            var cellsCol = this.xmlLoader.doXPath("./cell",xml);
            var strAr = new Array(0);

            for(var j=0;j<cellsCol.length;j++){
				var cellVal=cellsCol[j];
				var exc=cellVal.getAttribute("type");
//#__pro_feature:21092006{
//#xml_content:23102006{
					if (cellVal.getAttribute("xmlcontent"))
						cellVal=cellsCol[j];
					else
//#}
//#}
					if (cellVal.firstChild)
						cellVal=cellVal.firstChild.data;
				else cellVal="";
                if (j!=tree)
                    strAr[strAr.length] = cellVal;
                else
                    strAr[strAr.length] = pId+"^"+cellVal+"^"+((xml.getAttribute("xmlkids")||r._xml)?"1":"0")+"^"+(cellsCol[j].getAttribute("image")||"leaf.gif");

				if (exc)
					r.childNodes[j]._cellType=exc;

	        }
			if (this._c_order) strAr=this._swapColumns(strAr);
//			__c.log(strAr);
            for(var j=0;j<cellsCol.length;j++){
               css1=cellsCol[j].getAttribute("class");
                 if (css1) r.childNodes[j].className+=" "+css1;
              }
            this._fillRow(r,strAr);
//#__pro_feature:21092006{
//#colspan:20092006{
            if (this._ecspn)
            {
                r._childIndexes=new Array();
                var col_ex=0;
                var l=this.obj.rows[0].childNodes.length
                for(var j=0;j<l;j++){
                    r._childIndexes[j]=j-col_ex;
                    if (!cellsCol[j]) continue;
                    var col=cellsCol[j].getAttribute("colspan");
                    if (col){
                    r.childNodes[j-col_ex].colSpan=col;
                    for (var z=1; z<col; z++){
                        r.removeChild(r.childNodes[j-col_ex+1]);
                        r._childIndexes[j+z]=j-col_ex;
                        }
                    col_ex+=(col-1);
                    j+=(col-1);
                  }
                }
                if (!col_ex)
                   r._childIndexes=null;

            }
//#}
//#}
         if ((r.parentNode)&&(r.parentNode.tagName))
            this._postRowProcessing(r,xml);

            return r;
         }


/**
*   @desc:  inner recursive part of XML parsing routine, parses xml for one branch of treegrid or for whole grid
*   @type:  private
*/
         dhtmlXGridObject.prototype._innerParse=function(rowsCol,startIndex,tree,pId,i){
                            i=i||0;    var imax=i+this._ads_count;
                            var r=null;
							var rowsCol2;
                            for(var i;i<rowsCol.length;i++){
//#__pro_feature:21092006{
//#distrb_parsing:21092006{
                                    if (this._ads_count && i==imax) {
                                        new _contextCall(this,"_innerParse",rowsCol,startIndex,tree,pId,i,this._ads_time);
                                        return -1;
                                        }
//#}
//#}
							if ((pId)||(i<this.rowsBufferOutSize || this.rowsBufferOutSize==0)){

                            	this._parsing_=true;
                                var rId = rowsCol[i].getAttribute("id");
								r=this._prepareRow(rId);

								if (tree!=-1){
									rowsCol2 = this.xmlLoader.doXPath("./row",rowsCol[i]);
									if ((rowsCol2.length!=0)&&(this._slowParse))
				                        r._xml=rowsCol2;
								}

                                r=this._fillRowFromXML(r,rowsCol[i],tree,pId);

                            	if(startIndex){
                    		    	r = this._insertRowAt(r,startIndex);
                	                startIndex++;
            	                }else{
        	                    	r = this._insertRowAt(r);
    	                        }

						        this._postRowProcessing(r,rowsCol[i]);
                              	this._parsing_=false;
                            }
							else{
                            	var len = this.rowsBuffer[0].length
                              	this.rowsBuffer[1][len] = rowsCol[i];
                                this.rowsBuffer[0][len] = rowsCol[i].getAttribute("id")
                            }

                            if ((tree!=-1)&&(rowsCol2.length!=0)&&(!this._slowParse))
                                startIndex=this._innerParse(rowsCol2,startIndex,tree,rId);

					}

                //nb:paging
                if(this.pagingOn && this.rowsBuffer[0].length>0){
                    this.changePage(this.currentPage)
                }

                if ((r)&&(this._checkSCL))
                    for(var i=0;i<this.hdr.rows[0].cells.length;i++)
                        this._checkSCL(r.childNodes[i]);
                return startIndex;
            }


      /**
      *   @desc: get list of all roes with checked exCell in specified column
      *   @type: public
      *   @param: col_ind - column in question
      *   @topic: 5
      */
 dhtmlXGridObject.prototype.getCheckedRows=function(col_ind){
    var d=new Array();
    for (var i=0; i<this.rowsCol.length; i++){
        if (this.cells3(this.rowsCol[i],col_ind).getValue()!="0")
            d[d.length]=this.rowsCol[i].idd;
        }
    return d.join(",");
 }
/**
*   @desc:  grid body onmouseover function
*   @type:  private
*/
 dhtmlXGridObject.prototype._drawTooltip=function(e){
    var c = this.grid.getFirstParentOfType(e?e.target:event.srcElement,'TD');
    if((this.grid.editor)&&(this.grid.editor.cell==c)) return true;

    var r = c.parentNode;
    if (r.idd==window.unknown) return true;
    if ((this.grid._enbTts)&&(!this.grid._enbTts[c._cellIndex])) {
         (e?e.target:event.srcElement).title='';
         return true; }

    var ced = this.grid.cells(r.idd,c._cellIndex);

    if (ced)
        (e?e.target:event.srcElement).title=ced.getTitle?ced.getTitle():(ced.getValue()||"").toString().replace(/<[^>]*>/gi,"");

    return true;
    };

/**
*   @desc:  can be used for setting correction for cell padding, while calculation setSizes
*   @type:  private
*/
 dhtmlXGridObject.prototype.enableCellWidthCorrection=function(size){
    if (_isFF) this._wcorr=parseInt(size);
 }


    /**
   *   @desc: gets a list of all row ids in grid
   *   @param: separator - delimiter to use in list
   *   @returns: list of all row ids in grid
   *   @type: public
   *   @topic: 2,7
   */
dhtmlXGridObject.prototype.getAllItemIds = function(separator){
                     var ar = new Array(0)
                     for(i=0;i<this.rowsCol.length;i++){
                        ar[ar.length]=this.rowsCol[i].idd
                     }
                     for(i=0;i<this.rowsBuffer[0].length;i++){
                        ar[ar.length]=this.rowsBuffer[0][i]
                     }
                     return ar.join(separator||",")
                  }

   /**
   *   @desc: deletes row
   *   @param: row_id - id of row to delete
   *   @type: public
   *   @topic: 2,9
   */
dhtmlXGridObject.prototype.deleteRow = function(row_id,node){
                                //debugger;
                        if (!node)
                           node = this.getRowById(row_id)
                        if (!this.rowsAr[row_id])
                           return;
                                this.editStop();
                        if(typeof(this.onBeforeRowDeleted)=="function" && this.onBeforeRowDeleted(row_id)==false)
                           return false;

                        if(node!=null){
                           if (this.cellType._dhx_find("tree")!=-1)
                              this._removeTrGrRow(node);
                                   if (node.parentNode){
                                 node.parentNode.removeChild(node);
                                   }
                           var ind=this.rowsCol._dhx_find(node);
                              if (ind!=-1)
                              this.rowsCol._dhx_removeAt(ind);
                                   else{
                                 ind = this.rowsBuffer[0]._dhx_find(row_id)
                                 if(ind>=0){
                                    this.rowsBuffer[0]._dhx_removeAt(ind)
                                    this.rowsBuffer[1]._dhx_removeAt(ind)
                                 }

                           }
                           node = null;
                        }

                            for (var i=0; i<this.selectedRows.length; i++)
                                if (this.selectedRows[i].idd==row_id)
                                    this.selectedRows._dhx_removeAt(i);

                     this.rowsAr[row_id] = null;
                            if (this.onGridReconstructed)
                        this.onGridReconstructed();
                            if(this.pagingOn){
                        this.changePage();
                     }
                     this.setSizes();
                     return true;
                  }

//#__pro_feature:21092006{
//#colspan:20092006{

/**
*   @desc: deletes row
*   @param: row_id - row id
*   @param: col_id - index of column
*   @param: colspan - size of colspan
*   @type: public
*   @topic: 2,9
*/
dhtmlXGridObject.prototype.setColspan = function(row_id,col_ind,colspan){
    if (!this._ecspn) return;

    var r=this.getRowById(row_id);
    if ((r._childIndexes)&&(r.childNodes[r._childIndexes[col_ind]])){
        var j=r._childIndexes[col_ind];
        var n=r.childNodes[j];
        var m=n.colSpan;        n.colSpan=1;
        if ((m)&&(m!=1))
            for (var i=1; i<m; i++){
                var c=document.createElement("TD");
                if (n.nextSibling) r.insertBefore(c,n.nextSibling);
                else r.appendChild(c);
                r._childIndexes[col_ind+i]=j+i;
                c._cellIndex=col_ind+i;
                c.align = this.cellAlign[i];
            c.style.verticalAlign = this.cellVAlign[i];
                n=c;
                this.cells3(r,j+i).setValue("");
            }

        for (var z=col_ind*1+1*m; z<r._childIndexes.length; z++){
            r._childIndexes[z]+=(m-1)*1;                 }

    }

    if ((colspan)&&(colspan>1)){
        if (r._childIndexes)
            var j=r._childIndexes[col_ind];
        else{
            var j=col_ind;
            r._childIndexes=new Array();
            for (var z=0; z<r.childNodes.length; z++)
                r._childIndexes[z]=z;
            }

        r.childNodes[j].colSpan=colspan;
        for (var z=1; z<colspan; z++){
            r._childIndexes[r.childNodes[j+1]._cellIndex]=j;
            r.removeChild(r.childNodes[j+1]);
        }

        var c1=r.childNodes[r._childIndexes[col_ind]]._cellIndex;
        for (var z=c1*1+1*colspan; z<r._childIndexes.length; z++)
            r._childIndexes[z]-=(colspan-1);

    }
}

//#}
//#}

/**
*   @desc: prevent caching in IE  by adding random seed to URL string
*   @param: mode - enable/disable random seed ( disabled by default )
*   @type: public
*   @topic: 2,9
*/
dhtmlXGridObject.prototype.preventIECashing=function(mode){
   this.no_cashe = convertStringToBoolean(mode);
   this.xmlLoader.rSeed=this.no_cashe;
}


/**
*   @desc: enable/disable autosize of column on doubleclick
*   @param: mode - true/false
*   @type:  public
*/
dhtmlXGridObject.prototype.enableColumnAutoSize = function(mode){
	this._eCAS=convertStringToBoolean(mode);
}
/**
*   @desc: called when header was dbllicked
*   @type: private
*   @topic: 1,2
*/
dhtmlXGridObject.prototype._onHeaderDblClick = function(e){
	 var that=this.grid;
     var el = that.getFirstParentOfType(_isIE?event.srcElement:e.target,"TD");

   if (!that._eCAS) return false;
   that.adjustColumnSize(el._cellIndexS)
}

/**
*   @desc: enable/disable autosize of column on doubleclick
*   @param: cInd - index of column
*   @type:  public
*/
dhtmlXGridObject.prototype.adjustColumnSize = function(cInd){
	var a=this.hdr.rows[1].childNodes[cInd].childNodes[0];
	this._setColumnSizeR(cInd,20);
	var m=a.scrollWidth;

	var l=this.obj._rowslength();
	for (var i=0; i<l; i++){
		if (_isFF||_isOpera)
			var z=this.obj._rows(i).childNodes[cInd].innerHTML.replace(/<[^>]*>/g,"").length*7;
		else
			var z=this.obj._rows(i).childNodes[cInd].scrollWidth;
		if (z>m) m=z;
	}
	m+=2;
	this._setColumnSizeR(cInd,m);
	this.setSizes();
}





/**
*   @desc: attach additional line to header
*   @param: values - array of header titles
*   @param: style - array of styles, optional
*   @type:  public
*/
dhtmlXGridObject.prototype.attachHeader = function(values,style,_type){
	if (typeof(values)=="string") values=values.split(this.delim);
	_type=_type||"_aHead";
	if (this.hdr.rows.length){
		if (values)
			this._createHRow([values,style],this[(_type=="_aHead")?"hdr":"ftr"]);
    	else if (this[_type])
			for (var i=0; i<this[_type].length; i++)
				this.attachHeader.apply(this,this[_type][i]);
	}
	else{
		if (!this[_type]) this[_type]=new Array();
		this[_type][this[_type].length]=[values,style,_type];
	}
}

dhtmlXGridObject.prototype._createHRow = function(data,parent){
	if (!parent){
		//create footer zone
		this.entBox.style.position = "relative";
		var z=document.createElement("DIV");
   		z.className="ftr";
		this.entBox.appendChild(z);
		var t=document.createElement("TABLE");
		t.cellPadding=t.cellSpacing=0;
		if (!_isIE){
			t.width="100%";
			t.style.paddingRight="20px";
		}
        t.style.tableLayout = "fixed";

		z.appendChild(t);
		t.appendChild(document.createElement("TBODY"));
		this.ftr=parent=t;

        var hdrRow =t.insertRow(0);
        for(var i=0;i<this.hdrLabels.length;i++){
           hdrRow.appendChild(document.createElement("TH"));
           hdrRow.childNodes[i]._cellIndex=i;
        }
        if (_isIE) hdrRow.style.position="absolute";
        else hdrRow.style.height='auto';
	}
	var st1=data[1];
	var z=document.createElement("TR");
    parent.rows[0].parentNode.appendChild(z);
	for (var i=0; i<data[0].length; i++){
		if (data[0][i]=="#cspan"){
			var pz=z.cells[z.cells.length-1];
			pz.colSpan=(pz.colSpan||1)+1;
			continue;
		}
		if ((data[0][i]=="#rspan")&&(parent.rows.length>1)){
			var pind=parent.rows.length-2;
			var found=false;
			var pz=null;
			while(!found){
				var pz=parent.rows[pind];
				for (var j=0; j<pz.cells.length; j++)
					if (pz.cells[j]._cellIndex==i) {
						found=j+1;
						break;
					}
				pind--;
			}

			pz=pz.cells[found-1];
			pz.rowSpan=(pz.rowSpan||1)+1;
		   	if (!_isKHTML) continue;
            data[0][i]="";
		}

		var w=document.createElement("TD");
		w._cellIndex=w._cellIndexS=i;
		w.innerHTML=data[0][i];
		if (st1) w.style.cssText = st1[i];

		z.appendChild(w);
	}
}

//#__pro_feature:21092006{
/**
*   @desc: attach additional line to footer
*   @param: values - array of header titles
*   @param: style - array of styles, optional
*   @type:  public
*/
dhtmlXGridObject.prototype.attachFooter = function(values,style){
	this.attachHeader(values,style,"_aFoot");
}


/**
*   @desc: set excell type for cell in question
*   @param: rowId - row ID
*   @param: cellIndex - cell index
*   @param: type - type of excell
*   @type:  public
*/
dhtmlXGridObject.prototype.setCellExcellType = function(rowId,cellIndex,type){
	this.changeCellType(this.rowsAr[rowId],cellIndex,type);
}
dhtmlXGridObject.prototype.changeCellType=function(r,ind,type){
	type=type||this.cellType[ind];
	var z=this.cells3(r,ind);
	var v=z.getValue();
	z.cell._cellType=type;
	var z=this.cells3(r,ind);
	z.setValue(v);
}
/**
*   @desc: set excell type for all cells in specified row
*   @param: rowId - row ID
*   @param: type - type of excell
*   @type:  public
*/
dhtmlXGridObject.prototype.setRowExcellType = function(rowId,type){
	var z=this.rowsAr[rowId];
	for (var i=0; i<z.childNodes.length; i++)
		this.changeCellType(z,i,type);
}
/**
*   @desc: set excell type for all cells in specified column
*   @param: cellIndex - cell index
*   @param: type - type of excell
*   @type:  public
*/
dhtmlXGridObject.prototype.setColumnExcellType = function(cellIndex,type){
	for (var i=0; i<this.rowsCol.length; i++)
		this.changeCellType(this.rowsCol[i],cellIndex,type);
}


//#}
