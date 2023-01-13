

class FilterTypeAutocomplete extends Filter {
    constructor(jquery_combo, url, successResponseFunc, multiple = false) {
        super(jquery_combo);
        this._url = url;
        this.label = this.combo.find("label").html();
        //this._vItem = vItem;
        this._multiple = multiple;
        this._idsSeleccionados = "";
        this._successResponseFunc = successResponseFunc;
        this.aplicarAutocomplete();
        this.applyEvent();
    }

    _operarAutocompleteMultiple(data) {
        let self = this;
        if (isNullOrEmpty(self._idsSeleccionados)) {
            self._idsSeleccionados = "";
        }

        // Verificar que el elemento no haya sido previamente seleccionado.
        let arrayIds = self._idsSeleccionados.split(/,\s*/);
        let elemDuplicate = arrayIds.includes(data.id.toString());
        if (!elemDuplicate) {
            self._idsSeleccionados = isNullOrEmpty(self._idsSeleccionados)
                ? ""
                : self._idsSeleccionados + ",";
            self._idsSeleccionados = self._idsSeleccionados + data.id;
        }
        data.id = self._idsSeleccionados;

        let valoresSeleccionados = self.combo.find("input").val().split(/,\s*/);
        valoresSeleccionados.pop();
        // Si el elemento no fue previamente seleccionado, entonces carga tambien su valor.
        if (!elemDuplicate) {
            valoresSeleccionados.push(data.value);
        }
        valoresSeleccionados.push("");
        data.value = valoresSeleccionados.join(", ");

        return data;
    }

    applyEvent() {
        let self = this;

        // Fixed bug autocomplete en moviles TGPCDP-241
        this._combo.unbind("show.uk.autocomplete");
        this._combo.on("show.uk.autocomplete", function () {
            $(".uk-nav-autocomplete li").on("touchstart", function (e) {
                let data = {
                    id: parseInt($(e.target).attr("data-id")),
                    value: $(e.target).attr("data-value")
                }

                if (self._multiple) {
                    data = self._operarAutocompleteMultiple(data);
                }

                /*change dom values*/
                self._setValue(data.id, data.value);
            });
        });

        this._combo.unbind("selectitem.uk.autocomplete");
        this._combo.on('selectitem.uk.autocomplete', function (event, data) {
            if (self._multiple) {
                data = self._operarAutocompleteMultiple(data);
            }

            /*change dom values*/
            self._setValue(data.id, data.value);
        });

        this.combo.keyup(function (e) {
            if (e.which == 8 || e.which == 0 || e.which == 13) {
                self._idsSeleccionados = "";
                self._setValue("", "")
            }
        })
    }

    change(func = "null") {
        if (func) {
            this._change = func;
        }
    }

    /**
     * /
     * @param {any} id   -> lo que viaja por el request
     * @param {any} descript -> lo que se muestra en el combo
     */
    _setValue(id, descript) {
        this.combo.find("input").val(descript); 
        this.value = id;
        this.text = descript;
        this.addToFather();
        if (this._change != null && this._change != undefined) {
            this._change();
        }
    }

    selectValue(id,descript) {
        $(this).find("input").val(descript); 
        this._setValue(id, descript);
    }

    disable() {
        $(this).find("input").attr("disabled", "disabled");
    }

    enable() {
        $(this).find("input").removeAttr("disabled");
    }

    clear() {
        this.combo.find("input").val("");
        this._idsSeleccionados = "";
        this._setValue("", "");
    }

    aplicarAutocomplete() {
        var _self = this;
        $.UIkit.autocomplete(_self.combo, {
            source: function (response) {
                let searchString = "";
                if (_self._multiple) {
                    searchString = this.value.split(/,\s*/).pop();
                } else {
                    searchString = this.value;
                }
                if (searchString.length >= 5) {
                    $.ajax({
                        url: _self._url,
                        data: { query: searchString },
                        dataType: 'json',
                        type: 'POST',
                        success: function (data) {
                            response($.map(data, _self._successResponseFunc));
                        },
                        error: function (e) {
                            console.log(e);
                        }
                    })
                }
            },
            minLength: 5,
        });
    }
}
