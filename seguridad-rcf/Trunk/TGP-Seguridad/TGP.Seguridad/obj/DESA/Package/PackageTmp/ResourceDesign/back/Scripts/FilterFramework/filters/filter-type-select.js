
class FilterTypeSelect extends Filter {
    constructor(jquery_combo, defaultValue = null, texto = null) {
        super(jquery_combo, texto);
        this.init();
        this.text = this.combo.text();
        this.applyEvent();
        this._defaultValue = defaultValue;
        if (this._defaultValue)
            this.selectValue(this._defaultValue);
    }

    get defaultValue() {
        return this._defaultValue;
    }

    set defaultValue(newVal) {
        this._defaultValue = newVal;
    }

    init() {
        this._selectizeControl = this.aplicarSelectize(this.combo)[0].selectize;
    }

    selectValue(newVal) {
        this._selectizeControl.setValue(newVal);
    }

    enable() {
        this._selectizeControl.enable()
    }

    disable() {
        this._selectizeControl.disable()
    }

    clear() {
        if (this.defaultValue == null) {
            if (!isNullOrEmpty(Object.keys(this.selectizeControl.options).find((e) => { return e == "TODOS" }))) {
                return this.selectValue("TODOS");
            } else if (!isNullOrEmpty(Object.keys(this.selectizeControl.options).find((e) => { return e == "-1" }))) {
                return this.selectValue("-1");
            }
        } else {
            return this.selectValue(this.defaultValue);
        }
        return this._selectizeControl.clear()
    }

    change(func = null) {
        if (func) {
            this._change = func;
        } else {
            this.combo.change();
        }
    }

    applyEvent() {
        let self = this._self;
        this.combo.unbind("change");
        this.combo.change(function () {
            self.value = $(this).val();
            if (self.value != undefined && self.value != null) {
                self.text = $(this).text();
            }
            self.addToFather();
            if (self._change != null && self._change != undefined) {
                self._change();
            }
        })
    }

    get selectizeControl() {
        return this._selectizeControl;
    }

    aplicarSelectize(tag) {
        var self = this;
        return tag.selectize({
            hideSelected: false,
            create: false,
            maxOptions:2000,
            onDropdownOpen: function ($dropdown) {
                $dropdown
                    .hide()
                    .velocity('slideDown', {
                        begin: function () {
                            $dropdown.css({ 'margin-top': '0' })
                        },
                        duration: 200,
                        easing: easing_swiftOut
                    });
            },
            onDropdownClose: function ($dropdown) {
                if (isNullOrEmpty(this.$control_input.siblings("div").text())) {
                    if (!isNullOrEmpty(Object.keys(this.options).find((e) => { return e == "TODOS" }))) {
                        self.selectValue("TODOS");
                    } else if (!isNullOrEmpty(Object.keys(this.options).find((e) => { return e == "-1" }))) {
                        self.selectValue("-1");
                    }
                }
                $dropdown
                    .show()
                    .velocity('slideUp', {
                        complete: function () {
                            $dropdown.css({ 'margin-top': '' })
                        },
                        duration: 200,
                        easing: easing_swiftOut
                    });

            }
        });
    }

}