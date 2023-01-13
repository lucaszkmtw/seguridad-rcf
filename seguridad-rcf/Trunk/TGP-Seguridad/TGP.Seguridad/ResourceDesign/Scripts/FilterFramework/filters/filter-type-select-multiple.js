

class FilterTypeMultiSelect extends FilterTypeSelect {    
    constructor(jquery_combo, defaultValue = null, texto = null) {
        super(jquery_combo, defaultValue, texto);
        this._numberOfElementsShowing = 1;
        this._isShowing = false;        
    }

    get numberOfElementsShowing() {
        return this._numberOfElementsShowing;
    }
    set numberOfElementsShowing(number) {
        this._numberOfElementsShowing = number;
    }

    get isShowing() {
        return this._isShowing;
    }
    set isShowing(bool) {
        this._isShowing = bool;
    }

    applyEvent() {
        let self = this._self;
        this._combo.unbind("change");
        this._combo.change(function () {
            self.value = $(this).val();
            if (self.value !== undefined && self.value !== null) {
                self.text = self.completeMultipleText($(this).val());
            }
            self.addToFather();           
            if (self._change !== null && self._change !== undefined) {
                self._change();
            }
        })
    }

    onFinishUpdate() {
        let self = this;
        $(`#expandInfo-${this.id}`).unbind("click");
        $(`#expandInfo-${this.id}`).click(function () {
            self.toggleShowingComponents();
        });
    }

    completeMultipleText(arrayValues) {
        let string = "";
        let self = this._self;
        for (let [index, value] of arrayValues.entries()) {
            if (!self.isShowing) {
                if (!(index > this._numberOfElementsShowing)) {
                    string += `<span class=""> ${$(self.selectizeControl.getItem(value)[0]).text()}`;
                    if (arrayValues.length - 1 != index) {
                        string += "&nbsp • &nbsp";
                    }
                    string += "</span>";
                } else {
                    string += `<a class="uk-margin-small-left" title="Ampliar seleccionadas" data-uk-tooltip id="expandInfo-${this.id}"><i class="material-icons">add_circle</i></a>`;
                    break;
                }
            } else {               
                string += `<span>${$(self.selectizeControl.getItem(value)[0]).text()}`;
                if (arrayValues.length - 1 != index) {
                    string += "&nbsp • &nbsp";
                    string += "</span>";    
                } else {
                    string += "</span>";    
                    string += `<a class="uk-margin-small-left" title="Reducir seleccionadas"  data-uk-tooltip id="expandInfo-${this.id}"><i class="material-icons">remove_circle</i></a>`;    

                }
            }
        }
        return string
    }

    toggleShowingComponents() {
        this.isShowing = !this.isShowing;
        this.text = this.completeMultipleText(this.value);
        this.addToFather();      
    }


    selectAll() {
        let self = this;
        var optKeys = Object.keys(self._selectizeControl.options);
        optKeys.forEach(function (key, index) {
            self._selectizeControl.addItem(key);
        });
    }

    deselectAll() {
        this._selectizeControl.clear();
    }

    aplicarSelectize(tag) {
        return tag.selectize({
            plugins: {
                'remove_button': {
                    label: ''
                }
            },
            persist: false,
            maxItems: null,
            onItemAdd: function () {
                this.blur();
            },
            sortField: [{ field: '$score' }],
            onItemRemove: function (itemKey, itemObject) {
                tag.selectize({})[0].selectize.addOption({ value: itemKey, text: itemObject.text() });
            },
            onDropdownOpen: function ($dropdown) {
                $dropdown
                    .hide()
                    .velocity('slideDown', {
                        begin: function () {
                            $dropdown.css({ 'margin-top': '0' })
                        },
                        duration: 200,
                        easing: easing_swiftOut
                    })
            },
            onDropdownClose: function ($dropdown) {
                $dropdown
                    .show()
                    .velocity('slideUp', {
                        complete: function () {
                            $dropdown.css({ 'margin-top': '' })
                        },
                        duration: 200,
                        easing: easing_swiftOut
                    })
            }
        });
    }
}



