

class FilterTypeText extends Filter {
    constructor(jquery_combo, texto = null) {
        super(jquery_combo, texto);
        this.applyEvent();
    }

    applyEvent() {
        let self = this._self;
        this.combo.unbind("keyup", "change", "paste");
        this.combo.on("keyup change paste", function () {
            self.value = (isNullOrEmpty($(this).val())) ? null : $(this).val();
            self.text = (isNullOrEmpty($(this).val())) ? null : $(this).val();
            self.addToFather();
            if (self._change != null && self._change != undefined) {
                self._change();
            }
        })
    }

    change(func = null) {
        if (func) {
            this._change = func;
        } else {
            this.combo.keyup();
        }
    }

    selectValue(val) {
        this.combo.val(val);
        this.combo.keyup();      
    }

    clear() {
        this.combo.val("");
        this.value = "";
        this.combo.keyup();
    }
}
