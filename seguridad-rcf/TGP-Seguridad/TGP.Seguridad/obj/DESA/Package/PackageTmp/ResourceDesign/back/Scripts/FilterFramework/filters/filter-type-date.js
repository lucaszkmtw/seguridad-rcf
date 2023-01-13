/* se utiliza solo el input no se snecesita especificar el plugins */
class FilterTypeDate extends Filter {
    constructor(jquery_combo, texto = null) {
        super(jquery_combo, texto);
        this.applyEvent();
    }

    applyEvent() {
        let self = this._self;
        this.combo.unbind("change");
        this.combo.change(function () {
            let arrayDate = $(this).val().split(/\/|-/);
            let dateString = (arrayDate[2].length > 3) ? new moment(new Date(arrayDate[0], arrayDate[1], arrayDate[2])).format("DD/MM/YYYY") : new moment(new Date(arrayDate[2], arrayDate[1], arrayDate[0])).format("DD/MM/YYYY");
            self.value = (dateString.toLowerCase() === "invalid date") ? null : dateString;
            self.text = dateString;
            self.addToFather();
        });
    }

    clear() {
        this.combo.val(0);
        this.combo.change();
    }
}
