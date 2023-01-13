$(function() {
    altair_form_validation.init();
    validators_sigaf_parsley.init();
    Parsley.addMessages('es', {
        defaultMessage: "Este valor parece ser inválido.",
        type: {
            email: "Este valor debe ser un correo válido.",
            url: "Este valor debe ser una URL válida.",
            number: "Este valor debe ser un número válido.",
            integer: "Este valor debe ser un número válido.",
            digits: "Este valor debe ser un dígito válido.",
            alphanum: "Este valor debe ser alfanumérico."
        },

        notblank: "Este valor no debe estar en blanco.",
        required: "Este valor es requerido.",
        pattern: "Este valor es incorrecto.",
        min: "Este valor no debe ser menor que %s.",
        max: "Este valor no debe ser mayor que %s.",
        range: "Este valor debe estar entre %s y %s.",
        minlength: "Este valor es muy corto. La longitud mínima es de %s caracteres.",
        maxlength: "Este valor es muy largo. La longitud máxima es de %s caracteres.",
        length: "La longitud de este valor debe estar entre %s y %s caracteres.",
        mincheck: "Debe seleccionar al menos %s opciones.",
        maxcheck: "Debe seleccionar %s opciones o menos.",
        check: "Debe seleccionar entre %s y %s opciones.",
        equalto: "Este valor debe ser idéntico."
    });
    Parsley.setLocale('es');
}), 

altair_form_validation = {
    init: function() {
        var i = $("#form_validation");
        i.parsley({
            excluded: "input[type=button], input[type=submit], input[type=reset], input[type=hidden], .selectize-input > input"
        }).on("form:validated", function() {
            altair_md.update_input(i.find(".md-input-danger"))
        }).on("field:validated", function(i) {
            $(i.$element).hasClass("md-input") && altair_md.update_input($(i.$element))
        }), window.Parsley.on("field:validate", function() {
            var i = $(this.$element).closest(".md-input-wrapper").siblings(".error_server_side");
            i && i.hide()
        }), $("#val_birth").on("hide.uk.datepicker", function() {
            $(this).parsley().validate()
        })
    }
};

validators_sigaf_parsley = {
    init: function() {
        Parsley.addMessages('es', {           
            dateSigaf: "Nesesita ser una fecha valida" 
        });        
        Parsley.setLocale('es');

        function corroborarFecha(value){
            let arr = value.split("-");
            if (arr.length == 0) return false;
            if (arr[0].length != 2) return false;
            if (arr[1].length != 2) return false;
            if (arr[2].length != 4) return false;
            return true;
        }      

        window.Parsley.addValidator('dateSigaf', (value, requirement) => {           
            const def = new $.Deferred();
            // check if the momentjs library has been loaded
            if (typeof moment === 'function') {
                const timestamp = Date.parse(moment(value, requirement, true));
                // only start the ajax call if the value has the correct date format
                if (!isNaN(timestamp)) {
                    let isValid = moment(value, "DD-MM-YYYY").isValid();                   
                    if (isValid) {                        
                        if (corroborarFecha(value)){ def.resolve() } else { def.reject() };
                    } else {                       
                        def.reject();
                    }                   
                } else {                   
                    def.reject();
                }
                return def.promise();
            }            
            def.reject();
        });        
    }
};







