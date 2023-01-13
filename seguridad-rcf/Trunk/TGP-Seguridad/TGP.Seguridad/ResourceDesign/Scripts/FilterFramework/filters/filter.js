
class Filter {
    constructor(jquery_combo, texto = null) {
        /*
         *params: 
         * jquery_combo -> combo de jquery en donde aplicar los filtros
         * texto -> si se quiere algun texto en particular para mostrar en el dom
         */
        this._combo = jquery_combo;
        try {
            this._id = this._combo[0].id;
        } catch (e) {
            console.error(`Error en la instanciacion del filtro...¿Existe el combo en el DOM del html?¿Se corresponde con un objeto de JQUERY ? `);
        }

        this._label = (texto === null) ? this.generarLabel() : texto;
        this._self = this;
        this._value = this._combo.val();
        this._text = this._combo.val();//this._label;
        this._html = "";
    }

    generarLabel() {
        let label = this._combo.closest('[class^="uk-width"]').find('label').html();
        if (label) {
            return label
        } else {
            return ""
        }
    }

    disable() {
        this.combo.attr("disabled", "disabled");
    }
    enable() {
        this.combo.removeAttr("disabled");
    }

    onFinishUpdate() {
        //Se debe heredar en los hijos que necesiten esta funcionalidad
    }

    get combo() {
        return this._combo;
    }

    get id() {
        return this._id;
    }

    get label() {
        return this._label;
    }

    get text() {
        return this._text;
    }

    get value() {
        return this._value;
    }

    set value(newVal) {
        this._value = newVal;
    }

    set label(newLabel) {
        this._label = newLabel;
    }

    set text(newText) {
        this._text = newText;
    }

    get html() {
        return this._html;
    }

    set html(htmlText) {
        this._html = htmlText;
    }

    addFilterList(aList) {
        this._totalList = aList;
    }

    removeHtml() {
        this._html = "";
    }

    printConsole() {
        console.log(this._combo)
    }

    selectValue(value) {
        this.combo.val(value);
        this.value = value;
    }


    addToFather() {
        if (this._self._totalList != undefined && this._self._totalList != null) {
            if (this.value) {
                this._html = `<span class="md-color-primary filter filter-${this.id}"><b>${this.label.toUpperCase()} :</b>  </span><span class="md-color-grey-500 uk-margin-right"> ${this.text}</span>`;
            } else {
                this.removeHtml();
                //this._totalList.deleteFilter(this);
            }
            this._totalList._updateFilter(this);
        }
    }
}

//uk-badge  uk-badge-notification 