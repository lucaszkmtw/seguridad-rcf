
class FilterMultipleText extends Filter {
    constructor(jquery_combo) {
        super(jquery_combo);
        this._textObjs = [];
        this.init();
    }

    get textObjs() {
        return this._textObjs;
    }

    get value() {
        let text = "";
        for (let [index, value] of this._textObjs.entries()) {
            text += value.value;
            if (index != this._textObjs.length - 1)
                text += ",";
        }
        return text;
    }

    set value(text) {
        this.clear();
        let words = text.split(",");
        for (let word of words) {
            this.addItem(word);
        }
    }

    get text() {
        return this.value
    }

    clear() {
        for (let [index, value] of this._textObjs.entries()) {
            value = null;
        }
        this._textObjs = [];
        this.updateHtml();
    }

    addItem(value) {
        this._textObjs.push(new ItemMultipleText(value, this));
        this.updateHtml();
    }

    updateHtml() {
        let string = ""
        for (let [index, value] of this._textObjs.entries()) {
            string += value.returnHtml();
        }

        (this._combo.prop("tagName") === "INPUT") ? this._combo.val(string) : this._combo.html(string);
        let self = this;
        $(".multi-capsule").unbind("click");
        $(".multi-capsule").click(function () {
            self.deleteItem($(this).text());
        });
        this.addToFather();
        if (this._change != null && this._change != undefined) {
            this._change();
        }
    }

    deleteItem(text) {
        let obj = this.searchInObjs(text);
        let index = this._textObjs.indexOf(obj);
        if (index > -1) {
            this._textObjs.splice(index, 1);
        }
        obj = null;
        this.updateHtml();
    }

    change(func = "null") {
        if (func) {
            this._change = func;
            //let self= this
            //this.combo.bind("DOMSubtreeModified", function () {
            //    self.updateHtml();
            //    func();
            //});
        } else {
            this.updateHtml();
        }
    }

    searchInObjs(value) {
        let txt = value.replace(' close', '').trim();
        for (let [index, value] of this._textObjs.entries()) {
            if (value._value === txt) {
                return value;
            }
        }
    }

    init() {
        $("body").append(this.modal());
        let self = this;
        this._multiple_input_modal = $(`#multiple-input-modal-${this._combo[0].id}`);
        this._multiple_text_modal = $(`#multiple-text-modal-${this._combo[0].id}`);
        this.realoc();
        $(window).resize(function () {
            self.realoc();
        });
        this._combo.siblings("label")[0].remove();
        this._combo.wrap("<div class='md-input-wrapper md-input-filled'></div>");
        this._combo.parent().prepend(`<label>${this.label}</label>`);
        this._combo.addClass("multiple-text");
        this._combo.unbind("click");
        this._combo.click(function (e) {
            /**
             * esto es porque hay eventos hijos adentro del nroOP
             */
            var senderElementName = e.target.id;
            if (senderElementName === self.id) {
                self._multiple_input_modal.val("")
                $.UIkit.modal(self._multiple_text_modal).show();
            }
        });

        $(`#multiple-text-accept-${this._combo[0].id}`).unbind("click");
        $(`#multiple-text-accept-${this._combo[0].id}`).click(function () {

            /**
             * esto es solo por si se quiere tomar digitos numericos hay que ver a forma de generalizar la validacion del combo
             */
            //let regex = /^[0-9 _ ,]*$/;
            if (!isNullOrEmpty(self._multiple_input_modal.val())) {
                $.UIkit.modal(self._multiple_text_modal).hide();
                //if (regex.test(_multiple_input_modal.val())) {
                self.addItem(self._multiple_input_modal.val());
                /*} else {
                    alert("Solo admite digitos y \",\"");
                }*/
            } else {
                $.UIkit.modal(self._multiple_text_modal).hide();
            }
        });
    }

    realoc() {
        let top = this._combo.offset().top - (this._multiple_text_modal.height() / 2);
        let left = this._combo.offset().left - $(window).width() / 2 + 150; //150 es la mitad del width del modal
        this._multiple_text_modal.find(".uk-modal-dialog").attr("style", `top: ${top}px ; left: ${left}px ; width: 300px`);
    }

    modal() {
        return `<div class="uk-modal" id="multiple-text-modal-${this._combo[0].id}" aria-hidden="true" style="display: none;">
                    <div class="uk-modal-dialog">
                        <div class="uk-modal-header modal-header-gestion">
                            <h3 class="uk-modal-title">Ingrese uno o mas valores</h3>
                            <!-- <span class="uk-form-help-block">alguna acotacion</span> -->
                        </div>
                        <div class="uk-modal-body">
                            <div class="uk-grid" data-uk-grid-margin>
                                <div class="uk-width-1-1">
                                    <input class="md-input" id="multiple-input-modal-${this._combo[0].id}" type="text" name="input-op-modal" value="" />
                                </div>
                            </div>
                        </div>
                        <div class="uk-modal-footer uk-text-center">
                            <button id="multiple-text-accept-${this._combo[0].id}" type="button" class="md-btn md-btn-flat md-btn-accent">Aceptar</button>
                            <button type="button" class="md-btn md-btn-flat uk-modal-close">Cancelar</button>
                        </div>
                    </div>
                </div>
                `
    }
}

class ItemMultipleText {
    constructor(value, container) {
        this._value = value;
        this._container = container;
    }

    get value() {
        return this._value;
    }

    returnHtml() {
        return `<span class="multi-capsule">${this._value} <a class="remove-capsule"> <i class="material-icons">close</i></a></span>`
    }

    clear() {
        this._container.deleteItem(this);
    }
}

