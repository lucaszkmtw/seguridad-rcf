/**
 * helpers
 */

/**
 * 
 * @param {any} val
 * @param {object} settings
 */
const defaultsettings = {
    conDecimales: false,
    simbolo: "$",
    conSimbolo: false
}

function estaFormateado(val, settings = defaultsettings) {
    return isNaN(val) && val.includes(settings.simbolo)
}

function aplicarFormatoImporte(val, settings = defaultsettings) {
    completarSettings(settings);
    if (!isAnImportNaN(val)) {
        val = sacarFormatoImporte(val, settings);
        let decimales = null;
        if (settings.conDecimales) {
            let array = val.toString().split(".")
            val = array[0];
            decimales = array[1];
        }

        if (/\d/.test(val.toString())) {
            while (/(\d+)(\d{3})/.test(val.toString())) {
                val = val.toString().replace(/(\d+)(\d{3})/, '$1' + '.' + '$2');
            }
            if (/-/.test(val.toString())) {
                val = val.toString().replace(/-/, "");
                val = completarConSimbolo(val, settings, true)
            } else {
                val = completarConSimbolo(val, settings, false)
            }
        }
        return settings.conDecimales && decimales ? val + "," + decimales.toString().substring(0, 2) : val;
    } else {
        return val;
    }
}

function completarConSimbolo(valor, settings, esNegativo) {
    let val = valor;
    if (settings.conSimbolo) {
        val = settings.simbolo + val;
    }
    return esNegativo ? "-" + val : val
}

function completarSettings(settingsImporte) {
    for (let key of Object.keys(defaultsettings)) {
        if (settingsImporte[key] == null || settingsImporte[key] == undefined)
            settingsImporte[key] = defaultsettings[key]
    }
}
/**
 * Deja el string o numero como un tipo numero 
 */
function sacarFormatoImporte(val, settings = defaultsettings) {

    let newValue = val;
    if (typeof newValue !== "number") {
        if (settings.conSimbolo) {
            newValue = val.toString().replace(settings.simbolo, '');
        }
        newValue = newValue.replace(/\./g, "");
        if (settings.conDecimales) {
            newValue = newValue.replace(",", ".");
        }
        newValue = parseFloat(newValue);
    }
    let valorFinal = !settings.conDecimales ? parseInt(newValue) : newValue;
    let isOk = !isAnImportNaN(valorFinal);
    return isOk ? valorFinal : val;
}

function isAnImportNaN(val) {
    let tostring = val.toString();
    if (/\d/.test(tostring)) {
        return false;
    } else {
        return true;
    }
}

Number.prototype.between = function (a, b, inclusive = true) {
    var min = Math.min(a, b),
        max = Math.max(a, b);
    return inclusive ? this >= min && this <= max : this > min && this < max;
}


function returnArrayDate(strDate) {
    let split = strDate.split("-");
    if (split.length > 1) {
        return split;
    }
    return strDate.split("/")
}


$(function () {
    //$(document).ready(function () {
    if (isHighDensity) {
        // enable hires images
        altair_helpers.retina_images();
    }
    if (Modernizr.touch) {
        // fastClick (touch devices)
        FastClick.attach(document.body);
    }


    altair_jquery_val = {
        init: function () {
            if (jQuery.validator != undefined) {
                $.validator.methods.date = function (value, element) {
                    let splitDate = returnArrayDate(value);
                    return parseInt(splitDate[0]).between(1, 32) && parseInt(splitDate[1]).between(1, 12) && parseInt(splitDate[2]).between(1900, 2100)
                }

                if ($("form.form_validation").length > 0) {
                    $('[type="submit"]').click(function (e) {
                        e.preventDefault();
                        //var $form = $(".form_validation").validate();
                        if (!$("form.form_validation").valid()) {
                            for (let input of $("form.form_validation")[0]) {
                                if (!$(input).valid()) {
                                    $(input).addClass("md-input-danger");
                                }
                            }
                        } else {
                            $("form.form_validation").submit();
                        }
                    });
                }
            }
        }
    };
    altair_jquery_val.init();
});

/*funcion para retornar un una imagen a partir de una url y pasarla a base64*/
function toDataURL(url, callback) {
    var xhr = new XMLHttpRequest();
    xhr.onload = function () {
        var reader = new FileReader();
        reader.onloadend = function () {
            callback(reader.result);
        }
        reader.readAsDataURL(xhr.response);
    };
    xhr.open('GET', url);
    xhr.responseType = 'blob';
    xhr.send();
}


function loadingModal() {
    if ($('#modal_loading') != undefined || $('#modal_loading') != null) {
        return $.UIkit.modal("#modal_loading", { keyboard: false, bgclose: false })
    } else {
        console.error("no existe un modal asignado para el loading")
    }
}

//function showLoadingModal() {
//    if ($('#modal_loading') != undefined || $('#modal_loading') != null) {
//        let modalLoading = $.UIkit.modal("#modal_loading", { keyboard: false, bgclose: false });
//        if (!modalLoading.isActive()) {
//            modalLoading.show();
//        }
//    } else {
//        console.error("no existe un modal asignado para el loading")
//    }
//}

//function hideLoadingModal() {
//    if ($('#modal_loading') != undefined || $('#modal_loading') != null) {
//        let modalLoading = $.UIkit.modal("#modal_loading", { keyboard: false, bgclose: false });
//        if (modalLoading.isActive()) {
//            modalLoading.hide();
//        }
//    } else {
//        console.error("no existe un modal asignado para el loading")
//    }
//}


modal = null;
function showLoadingModal() {
    if (modal && modal.isActive()) {
        return;
    }
    modal = UIkit.modal.blockUI(
        `<div class='uk-text-center'>
                <h3 class="heading_b uk-margin-large-bottom">
                Espere unos segundos...
            </h3>
            <div class="uk-flex uk-flex-center">
                <div class="md-preloader"><svg xmlns="http://www.w3.org/2000/svg" version="1.1" height="96" width="96" viewbox="0 0 75 75"><circle cx="37.5" cy="37.5" r="33.5" stroke-width="6" /></svg></div>
                
            </div>
            </div>`
    );
}

function hideLoadingModal() {
    if (modal && modal.isActive()) {
        modal.hide()
    }
}






function completarConCeros(str, max) {
    str = str.toString();
    return str.length < max ? pad("0" + str, max) : str;
}

function aplicarSelectize(tag) {
    tag.selectize({
        hideSelected: false,
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
                });
        },
        onType: function (value) {
            console.log(value);
        }
    });
};

function aplicarSelectizeMultiple(tag = "") {
    let element;
    if (tag != "") {
        element = tag;
    } else {
        element = $('.selectizeMulti');
    }

    element.selectize({
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
            if (tag != "") {
                tag.selectize({})[0].selectize.addOption({ value: itemKey, text: itemObject.text() });
            }
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


function addCerosToStr(str, max) {
    str = str.toString();
    return str.length < max ? addCerosToStr("0" + str, max) : str;
}

function modalOnHideResetValues() {
    $('.uk-modal').on({
        'hide.uk.modal': function () {
            if ($(this).find("input").length > 0) {
                $(this).find("input").val("");
            }
        }
    });
}


$(document).ready(function () {
    actualizarSigafTooltips()
});

function actualizarSigafTooltips() {
    if ($("[data-sigaf-tooltip]").length > 0) {      
        if ($("[data-sigaf-tooltip]").find(".data-sigaf-tooltip").length === 0) {
            for (let tooltip of $("[data-sigaf-tooltip]")) {
                var parent = tooltip.parentNode;
                var wrapper = document.createElement('div');
                $(wrapper).attr("data-uk-dropdown", "");
                $(wrapper).addClass("uk-button-dropdown");
                parent.replaceChild(wrapper, tooltip);
                wrapper.appendChild(tooltip);

                let data = $(tooltip).attr("content");
                $(wrapper).append(`<div class="uk-dropdown data-sigaf-tooltip">${data ? data : "<span>Poner attr content el html</span>"}</div>`);              

               
            }   
        }      
    }
}

function aplicarSelectizeWithFocus(tag, unaFuncion) {
    tag.selectize({
        hideSelected: false,
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
                });
        },
        onFocus: unaFuncion
    });
};


/*scripts del anterior GEP*/
jQuery.fn.setFormTimeout = function () {
    var $form = $(this);
    setTimeout(function () {
        $('input[type="submit"]', $form).button('reset');
        alert('Form failed to submit within 30 seconds');
    }, 30000);
};

function cleanSelectizeValues(tag, valPorDefecto = 0) {
    /**
        * El valor por defecto del tag que utiliza selectize debe tener el valor -1
        */
    var $select = tag.selectize();
    var selectize = $select[valPorDefecto].selectize;
    selectize.setValue(valPorDefecto);
}

function restrictKeyboard(tag) {
    tag.keypress(function (event) {
        if (event.which < 48 || event.which > 57) {
            if (!(event.which == 8 || event.which == 0 || event.which == 13)) {
                event.preventDefault();
            }
        }
    });
}

function modalToggle(id) {
    var modal = UIkit.modal(id);
    if (modal.isActive()) {
        modal.hide();
    } else {
        modal.show();
    }
}

/*daterange disable y enable   se pasa como parametro el id en forma de texto
EJ: "#fecha_cobro"
No pasar objeto de jquery
*/
var dateRange = {
    disable: function (tag) {
        localStorage.setItem(tag, $(tag).val());
        auxDate = $(tag).val();
        $(tag).prop('disabled', true);
        $(tag).val("");
        $(tag).parent().siblings().first().children().first().addClass("uk-text-muted");
        $(tag).parent().siblings(".uk-grid-collapse").children().each(function () {
            $(this).addClass('disabled');
            $(this).removeClass("md-btn-accent")
        });
    },
    enable: function (tag) {
        $(tag).prop('disabled', false);
        if (localStorage.getItem(tag)) {
            $(tag).val(localStorage.getItem(tag))
        };
        $(tag).val(auxDate);
        $(tag).parent().siblings().first().children().first().removeClass("uk-text-muted");
        $(tag).parent().siblings(".uk-grid-collapse").children().each(function () {
            $(this).addClass("md-btn-accent");
            $(this).removeClass('disabled')
        });
    }
}


altair_form_file_upload = {
    init: function (allow, notAllowedText) {
        var progressbar = $("#file_upload-progressbar"),
            bar = progressbar.find('.uk-progress-bar'),
            settings = {
                allow: allow, // File filter
                loadstart: function () {
                    bar.css("width", "0%").text("0%");
                    progressbar.removeClass("uk-hidden");
                },
                error: function (error) {
                    alert(error);
                },
                notallowed: function (error) {
                    swal({
                        title: "Archivo no válido",
                        text: notAllowedText,
                        icon: "warning",
                        type: "warning"
                    });
                },
                progress: function (percent) {
                    percent = Math.ceil(percent);
                    bar.css("width", percent + "%").text(percent + "%");
                },
                allcomplete: function (response, xhr) {
                    bar.css("width", "100%").text("100%");
                    bar.addClass('uk-progress-success');
                    setTimeout(function () {
                        progressbar.addClass("uk-hidden");
                    }, 250);
                    setTimeout(function () {
                        UIkit.notify({
                            message: "Carga Completa",
                            pos: 'top-right'
                        });
                    }, 280);
                    $(".archivoText strong").text($("#files")[0].files[0].name);
                    $("#submit_file").removeClass("uk-hidden");
                    $(".uk-form-file").removeClass("md-btn-accent");
                    $(".uk-form-file").get(0).firstChild.nodeValue = "Cambiar archivo";
                }
            };

        var select = UIkit.uploadSelect($("#files"), settings);
        //drop = UIkit.uploadDrop($("#file_upload-drop"), settings);
    }
};

function agregarDomDatatableWithoutButtons(idsearch = null) {
    return `<'uk-grid uk-margin-small-bottom'<'uk-width-large-1-2'l><'uk-width-large-1-2'f>>tirp`
}
/*datatable functions globals*/
function agregarDomDatatable() {
    return `<'uk-grid uk-margin-small-bottom'<'uk-width-large-3-10'l><'uk-width-large-3-10'f><'uk-width-large-4-10 uk-flex uk-flex-right'B>>tirp`
}
function agregarDomDatatableOnlyTable() {
    return "t"
}


/**
 * @param {string} columnsWidth example: "1-3" 
 */
function agregarResponsiveDatatable(columnsWidth) {
    return {
        details: {
            renderer: function (api, rowIdx, columns) {
                var data = $.map(columns, function (col, i) {
                    return col.hidden ?
                        `<div class="uk-width-small-${columnsWidth}">
                            <strong>${col.title}</strong>  <br>  ${col.data}
                        </div>`
                        :
                        '';
                }).join('');

                return data ?
                    $('<div class="uk-grid" data-uk-grid-margin/>').append(data) :
                    false;
            }
        }
    }
}

function agregarLenguajeDatatable() {
    return {
        "sProcessing": "Procesando...",
        "sLengthMenu": "Mostrar _MENU_ registros",
        "sZeroRecords": "No se encontraron resultados",
        "sEmptyTable": "Ningún dato disponible en esta tabla",
        "sInfo": "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
        "sInfoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
        "sInfoFiltered": "(filtrado de un total de _MAX_ registros)",
        "sInfoPostFix": "",
        "sSearch": "Buscar:",
        "sUrl": "",
        "sInfoThousands": ",",
        "sLoadingRecords": "Cargando...",
        "oPaginate": {
            "sFirst": "Primero",
            "sLast": "Último",
            "sNext": "Siguiente",
            "sPrevious": "Anterior"
        },
        "oAria": {
            "sSortAscending": ": Activar para ordenar la columna de manera ascendente",
            "sSortDescending": ": Activar para ordenar la columna de manera descendente"
        }
    }
}


function activarMenu(liActiveId) {
    $(".menu_section").find("a").click(function () {
        if ($(this).siblings("ul").length == 0) showLoadingModal()
    });

    if (!$("body").hasClass("sidebar_mini")) {
        if ($(liActiveId).parents('li').hasClass("submenu_trigger")) {
            $(liActiveId).addClass('act_item');
            $(liActiveId).addClass('md-bg-grey-300');
            $li = $(liActiveId).parents('li');
            if (!$li.hasClass('act_section')) {
                $li.addClass('act_section');
                $li.children("a").children(".menu_title").addClass('md-color-dark');
                $li.children("a").children(".menu_icon").children().addClass('md-color-dark');
            }
            $(liActiveId).parents('ul').css('display', 'block')
                .css('overflow', 'hidden')
        } else {
            $(liActiveId).addClass('md-bg-grey-300');
            $(liActiveId).children("a").children(".menu_title").addClass('md-color-dark');
            $(liActiveId).children("a").children(".menu_icon").children().addClass('md-color-dark');
        }
    } else {
        /*with mini sidebar */
        if ($(liActiveId).parents('li').hasClass("submenu_trigger")) {
            $(liActiveId).addClass('act_item');
            $li = $(liActiveId).parents('li');
            if (!$li.hasClass('act_section')) {
                $li.addClass('act_section');
                $li.children("a").children(".menu_title").addClass('md-color-dark');
                $li.children("a").children(".menu_icon").children().addClass('md-color-dark');
            }
        } else {
            $(liActiveId).children("a").children(".menu_title").addClass('md-color-dark');
            $(liActiveId).children("a").children(".menu_icon").children().addClass('md-color-dark');
        }
    }
}

function configDateRangePicker() {
    return {
        format: 'DD-MM-YYYY',
        language: 'es',
        separator: ' al ',
        monthSelect: true,
        yearSelect: true,
        getValue: function () {
            return $(this).val();
        },
    }
};
/**
 * Fixerrrr DATERANGE
toma el valor de la vista y retorna un array con las fechas 
 */
function catchValuesDateRange(tag) {
    var arrayDates;
    if (tag.val()) {
        arrayDates = tag.val().split("al");
        arrayDates[0] = arrayDates[0].trim();
        arrayDates[1] = arrayDates[1].trim();
    } else {
        arrayDates[0] = null;
        arrayDates[1] = null;
    }
    return arrayDates
}

function isNullOrEmpty(tag) {
    if (typeof (tag) === "object") {
        if (tag instanceof jQuery) {
            return $(tag).length > 0
        } else {
            if (tag === null) {
                return true
            }
            return false
        }
    }
    return tag === null || tag === undefined || tag === ""

}

function disabledBtn(tag) {
    if (!$(tag).hasClass("disabled")) {
        $(tag).addClass("disabled");
    }
}

function ableBtn(tag) {
    if ($(tag).hasClass("disabled")) {
        $(tag).removeClass("disabled");
    }
}

function addfileUpload(tag) {
    var progressbar = tag.find(".file_upload-progressbar"),
        bar = progressbar.find('.uk-progress-bar'),
        settings = {
            allow: '*.(pdf)', // File filter
            loadstart: function () {
                bar.css("width", "0%").text("0%");
                progressbar.removeClass("uk-hidden");
            },
            error: function (error) {
                alert(error);
            },
            notallowed: function (error) {
                swal({
                    title: "Archivo no válido",
                    text: "Solo se permiten archivos *.pdf",
                    icon: "warning",
                    type: "warning"
                });
            },
            progress: function (percent) {
                percent = Math.ceil(percent);
                bar.css("width", percent + "%").text(percent + "%");
            },
            allcomplete: function (response, xhr) {
                bar.css("width", "100%").text("100%");
                bar.addClass('uk-progress-success');
                setTimeout(function () {
                    progressbar.addClass("uk-hidden");
                }, 250);
                setTimeout(function () {
                    UIkit.notify({
                        message: "Carga Completa",
                        pos: 'top-right'
                    });
                }, 280);
                tag.find(".archivoText strong").text(tag.find(".files").val());
                tag.find(".submit_file").removeClass("uk-hidden");
            }
        };
    select = UIkit.uploadSelect(tag.find(".files"), settings);
}



/**
 * datatable para arreglar problmas del fixerheader
    SOLO PASA CUADNO ESTA DENTRO DE TABS COMO en Consulta De Pagos 
 */
function updateFixedHeaderDatatable(tabla) {
    console.error("updateFixedHeaderDatatable is a deprecated function");
    //$("#tabs_1").unbind('change.uk.tab');
    //$("#tabs_1").on('change.uk.tab', function (e, active, previous) {
    //    if (previous.text().toLowerCase().split(" ")[0] !== "detalle" && active.text().toLowerCase().split(" ")[0] === "detalle") {
    //        console.log("destruyo")
    //        tabla.fixedHeader.disable();
    //    }

    //    if (previous.text().toLowerCase().split(" ")[0] === "detalle" && active.text().toLowerCase().split(" ")[0] !== "detalle") {
    //        let miPrimeraPromise = new Promise((resolve, reject) => {
    //            setTimeout(function () {
    //                resolve();
    //            }, 50);
    //        });

    //        miPrimeraPromise.then((successMessage) => {
    //            tabla.fixedHeader.enable();
    //            tabla.fixedHeader.adjust();
    //            tabla.responsive.recalc()

    //        });
    //    }
    //});
}


/**
* ***Modo de uso****
* Param Obj se tiene que pasar un array con clave:"tabId" y valor la tabla con fixedheader que esta en la tab
 * 
 * ejemplo: 

    let tabs = [];
    tabs["tab-busqueda"] = tablaEnBusqueda;
    tabs["tab-busqueda"] = tabla;
    updateTabsHeader(tabs);
*/
function updateTabsHeader(obj = null) {
    $(".uk-tab").on('change.uk.tab', function (e, active, previous) {
        if (obj) {
            for (let key in obj) {
                if (key == active[0].id) {
                    let animationTab = new Promise((resolve, reject) => {
                        setTimeout(function () {
                            resolve();
                        }, 50);
                    });
                    animationTab.then(() => {
                        if (obj[key])
                            habilitarTablasHeader(obj[key])
                    })
                } else {
                    desabilitarTablasHeader(obj[key])
                }
            }
        }
    });
}

function desabilitarTablasHeader(tablas) {
    if (tablas.length > 0) {
        for (let tabla of tablas) {
            tabla.fixedHeader.disable();
        }
    } else {
        tablas.fixedHeader.disable();
    }
}

function habilitarTablasHeader(tablas) {
    if (tablas.length > 0) {
        for (let tabla of tablas) {
            tabla.fixedHeader.enable();
            tabla.fixedHeader.adjust();
            tabla.responsive.recalc();
        }
    } else {
        tablas.fixedHeader.enable();
        tablas.fixedHeader.adjust();
        tablas.responsive.recalc();
    }
}



