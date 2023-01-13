

class FilterTypeCheck extends Filter {
    constructor(jquery_combo, texto = null) {
        super(jquery_combo, texto);
        this.init();
        this.applyEvent();
        this.value = false;
    }

    init() {
        altair_md.checkbox_radio(this.combo);
    }

    selectValue(bool) {
        if (bool) {
            this.check()
        } else {
            this.uncheck()
        }
    }

    applyEvent() {
        let self = this._self;
        this.combo.unbind("ifChanged");
        this.combo.on('ifChanged', function (event) {
            self.value = event.target.checked;
            self.text = self.value.toString();
            self.addToFather();
            if (self._change != null && self._change != undefined) {
                self._change();
            }
        });
    }

    change(func = "null") {
        if (func) {
            this._change = func;
        }
    }

    uncheck() {
        this.combo.iCheck("uncheck")
    }

    check() {
        this.combo.iCheck("check")
    }

    disable() {
        this.combo.iCheck("disable")
    }

    enable() {
        this.combo.iCheck("enable")
    }

}