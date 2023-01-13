
class FilterTextAjax extends Filter {
    constructor(jquery_combo) {
        super(jquery_combo);
        this._title = "Agregue un texto";
        this._description = "";
        this.init();
    }

    get text() {
        return this.value
    }
    set url(url) {
        this._url = url;
    }
    get url() {
        return this._url;
    }

    set title(title) {
        this._title = title;
    }
    get title() {
        return this._title;
    }

    set description(description) {
        this._description = description;
    }
    get description() {
        return this._description;
    }

    successAjax(aFunc) {
        this._success = aFunc;
    }

    errorAjax(aFunc) {
        this._error = aFunc;
    }

    errorMsj(aText) {
        this._multiple_text_modal_error = $(`#multiple-text-modal-error-${this._combo[0].id}`);
        this._multiple_text_modal_error.text(aText);
    }

    disabled() {
        this.combo.attr("disabled", true)
    }

    enabled() {
        this.combo.attr("disabled", false)
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
        this.combo.unbind("click");
        this.combo.click(function (e) {
            /**
             * esto es porque hay eventos hijos adentro del nroOP
             */
            var senderElementName = e.target.tagName.toLowerCase();
            if (senderElementName === 'input') {
                self._multiple_input_modal.val("");
                $.UIkit.modal(self._multiple_text_modal).show();
            }
        });

        $(`#multiple-text-accept-${this._combo[0].id}`).click(function () {

            /**
             * esto es solo por si se quiere tomar digitos numericos hay que ver a forma de generalizar la validacion del combo
             */
            //let regex = /^[0-9 _ ,]*$/;
            if (!isNullOrEmpty(self._multiple_input_modal.val())) {
                if (self.url != null || self.url != undefined) {
                    $.ajax({
                        type: "POST",
                        url: self.url,
                        data: {
                            data: self._multiple_input_modal.val()
                        },
                        //beforeSend: function () {},
                        success: (self._success) ? self._success() : console.log("se realizo el request con exito"),
                        error: function (xhr, ajaxOptions, thrownError) {
                            (self._error) ? self._error() : console.log(thrownError);
                        }
                    });
                } else {
                    self.combo.val(self._multiple_input_modal.val());
                    $.UIkit.modal(self._multiple_text_modal).hide();
                }
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
                            <h3 class="uk-modal-title">${this.title}</h3>
                            <span class="uk-form-help-block">${this.description}</span
                        </div>
                        <div class="uk-modal-body">
                            <div class="uk-grid" data-uk-grid-margin>
                                <div class="uk-width-1-1">
                                    <input class="md-input" id="multiple-input-modal-${this._combo[0].id}" type="text" name="multiple-input-modal-${this._combo[0].id}" value="" />
                                    <p id="multiple-input-modal-error-${this._combo[0].id}" class="uk-text-danger" hidden></p>
                                </div>
                            </div>
                        </div>
                        <div class="uk-modal-footer uk-text-right">
                            <button type="button" class="md-btn md-btn-flat uk-modal-close">Cancelar</button>
                            <button id="multiple-text-accept-${this._combo[0].id}" type="button" class="md-btn md-btn-flat md-btn-accent">Aceptar</button>
                        </div>
                    </div>
                </div>
                `
    }
}