class FilterList {
    /**
     * constructor
     * @param {$("#multipleFilters")} jquery_combo
     * @param withoutDblDispatch boolean que habilita o 
     */
    constructor(jquery_combo = null, withoutDblDispatch = false) {
        if (jquery_combo) {
            if (jquery_combo instanceof jQuery) {
                this._combo = jquery_combo;
                this._WithDblDispatch = true;
            } else {
                console.error(`Error en la instanciacion del filterlist...¿Existe el combo en el DOM del html? ¿Se corresponde con un objeto de JQUERY?`);
            }
        } else {
            if (!withoutDblDispatch) {
                this._WithDblDispatch = true;
            } else {
                this._WithDblDispatch = false;
            }
        }
        this._tables = [];
        this.filters = [];
    }

    /*borra todo*/
    delete() {
        this._combo.selectize({})[0].selectize.clear();
        this.filters = new Array();
    }

    /*limpia los campos*/
    clear() {
        var i = this.filters.length;
        while (i >= 0) {
            i--;
            if (i >= 0)
                this.filters[i].clear();
        }
    }

    //get tables() {
    //    return this._value;
    //}

    //set tables(newVal) {
    //    this._value = newVal;
    //}

    addFixedHeader(...tables) {
        this._tables = (tables) ? tables:null;
        var self = this;
        var sticky = self._combo.offset().top;
        $(window).scroll(function () { self._fixedHeader(self._tables, sticky,false) });
        $(window).resize(function () { self._fixedHeader(self._tables, sticky,true) });
    }
     


_fixedHeader(tables, sticky,needToReloadTable) {
        var header = this._combo;
        if (this._combo)
            if ($(window).width() > 600) {
                if (window.pageYOffset > sticky) {
                    if (needToReloadTable || !header.hasClass("floating")) {
                        let realWidthHeader = header.closest(".md-card-content").width();
                        header.addClass("floating");
                        header.css("width", `${realWidthHeader - parseInt($(header).css("padding-left")) - parseInt($(header).css("padding-right")) - 2}px`);
                        header.css("margin-left", "1px")
                        if (tables)
                            for (let tab of tables)
                                if (!isNullOrEmpty(tab) && $.fn.DataTable.isDataTable(tab)) {
                                    let offsettop = header.height() + parseInt(header.css('padding-top')) + parseInt(header.css('padding-bottom'));
                                    tab.fixedHeader.headerOffset(offsettop);
                                    tab.fixedHeader.adjust();
                                    if (needToReloadTable)
                                        tab.responsive.recalc();
                                }
                    }
                } else {
                    header.removeClass("floating");
                    header.removeAttr("style");
                }
            } else {
                header.removeClass("floating");
                header.removeAttr("style");
            }
    }

    deleteFilter(filterclass) {
        let index = this.filters.indexOf(filterclass);
        if (index > -1) {
            this.filters.splice(index, 1);
        }
        filterclass = null;
        this._updateHtml();
    }

    _updateHtml() {
        if (this._combo) {
            this._combo.html(this.compileHtml());
            if (this.filters.length > 0) {
                for (let [index, value] of this.filters.entries()) {
                    value.onFinishUpdate();
                }
            }
        }
    }

    _updateFilter(filterclass) {
        try {           
            if (!this.filters.includes(filterclass)) {
                this.filters.push(filterclass);
            }
            this._updateHtml();
        } catch (e) {
            console.log(e.message);
        }
    }

    addFilter(filter) {
        try {
            if (this.isAFilter(filter)) {
                if (!this.contain(filter)) {
                    if (!this._WithDblDispatch) {
                        this.addFilter(filter)
                    } else {
                        filter.addFilterList(this);
                        filter.addToFather();
                    }
                } else {
                    this.deleteFilter(this.find(filter));
                    if (!this._WithDblDispatch) {
                        this.addFilter(filter)
                    } else {
                        filter.addFilterList(this);
                        filter.addToFather();
                    }
                }
            } else {
                console.error("Hay un error en la carga de filtros.. \n Se estan pasando objetos que no heredan de filter");
            }
        } catch (e) {
            console.log(e.message);
        }
    }

    addMultipleFilters(...filters) {
        for (let filter of filters) {               
            this.addFilter(filter);              
        }       
    }

    contain(filter) {
        let elem = this.find(filter);
        return (elem != null && elem != undefined)
    }

    find(filter) {
        if (this.isAFilter(filter)) {
            return this.filters.find((elemt) => {
                return elemt._id == filter.id
            })   
        }
        return this.filters.find((elemt) => {
            return elemt._id == filter
        })          
    }

    isAFilter(filter) {
        return filter instanceof Filter
    }

    printConsole() {
        console.log(this.filters)
    }

    compileHtml() {
        if ((!this._combo.parent().hasClass("uk-margin-bottom")) && this.filters.length > 0)
            this._combo.parent().addClass("uk-margin-bottom uk-width-1-1")
        let html = "";
        if (this.filters.length > 0) {
            for (let [index, value] of this.filters.entries()) {
                html += value.html;
            }
        }
        return html;
    }

    toJson() {
        //let newjsonObj = new Object();
        let newjsonValues = new Object();
        for (let filter of this.filters) {
            //newjsonObj[filter.id] = filter;
            newjsonValues[filter.id] = filter.value;
        }
        //let obj = new Object();
        //obj["filters"] = newjsonObj;
        //obj["values"] = newjsonValues;
        //return obj;
        return newjsonValues
    }
}
