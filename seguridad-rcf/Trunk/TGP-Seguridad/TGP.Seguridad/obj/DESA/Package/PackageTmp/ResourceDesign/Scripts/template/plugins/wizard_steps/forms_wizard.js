$(function() {
    // wizard
    altair_wizard.advanced_wizard();
    altair_wizard.vertical_wizard();
});

// wizard
altair_wizard = {
    content_height: function(this_wizard,step) {
        var this_height = $(this_wizard).find('.step-'+ step).actual('outerHeight');
        $(this_wizard).children('.content').animate({ height: this_height }, 140, bez_easing_swiftOut);
    },
    advanced_wizard: function() {
        var $wizard_advanced = $('#wizard_advanced'),
            $wizard_advanced_form = $('#wizard_advanced_form');

        if ($wizard_advanced.length) {
            $wizard_advanced.steps({
                labels: {
                    cancel: "Cancelar ",
                    current: "paso actual:",
                    pagination: "Paginacion",
                    finish: "Terminar",
                    next: "Siguiente",
                    previous: "Anterior",
                    loading: "Cargando..."
                },
                headerTag: "h3",
                bodyTag: "section",
                transitionEffect: "slideLeft",
                trigger: 'change',
                onInit: function(event, currentIndex) {
                    altair_wizard.content_height($wizard_advanced,currentIndex);
                    // reinitialize textareas autosize
                    altair_forms.textarea_autosize();
                    // reinitialize checkboxes
                    altair_md.checkbox_radio($(".wizard-icheck"));
                    $(".wizard-icheck").on('ifChecked', function(event){
                        console.log(event.currentTarget.value);
                    });
                    // reinitialize uikit margin
                    altair_uikit.reinitialize_grid_margin();
                    // reinitialize selects
                    altair_forms.select_elements($wizard_advanced);
                    // reinitialize switches
                    $wizard_advanced.find('span.switchery').remove();
                    altair_forms.switches();
                    // reinitialize dynamic grid
                    altair_forms.dynamic_fields($wizard_advanced,true,true);
                    // resize content when accordion is toggled
                    $('.uk-accordion').on('toggle.uk.accordion',function() {
                        $window.resize();
                    });

                    setTimeout(function() {
                        $window.resize();
                    },100);
                },
                onStepChanged: function (event, currentIndex) {
                    altair_wizard.content_height($wizard_advanced,currentIndex);
                    setTimeout(function() {
                        $window.resize();
                    },100)
                },
                onStepChanging: function (event, currentIndex, newIndex) {
                    var step = $wizard_advanced.find('.body.current').attr('data-step'),
                        $current_step = $('.body[data-step=\"'+ step +'\"]');

                    // check input fields for errors
                    $current_step.find('.parsley-row').each(function() {
                        $(this).find('input,textarea,select').each(function() {
                            $(this).parsley().validate();
                        });
                    });

                    // adjust content height
                    $window.resize();
                    if (!(currentIndex >= newIndex)) {
                        if ($current_step.find('.md-input-danger').length) {
                            return false;
                        }
                        else {
                            //if (!validarPasos(currentIndex, newIndex) == false) {
                            //   return false;
                            //}
                            //var resultado = validarPasos(currentIndex, newIndex);
                            //return true                       
                            return validarPasos(currentIndex, newIndex)
                        }
                    } else {
                        return true;
                    }
                },
                onFinished: function () {
                    /*aca se termina el paso... hayq ue ver que hacer*/

                    $wizard_advanced_form.parsley().validate();
                    if ($wizard_advanced_form.parsley().isValid()){
                        var form_serialized = $wizard_advanced_form.serializeObject();
                        recuperarDatos(form_serialized);
                    }
                }
            })/*.steps("setStep", 2)*/;

            $window.on('debouncedresize',function() {
                var current_step = $wizard_advanced.find('.body.current').attr('data-step');
                altair_wizard.content_height($wizard_advanced,current_step);
            });

            // wizard
            $wizard_advanced_form
                .parsley()
                .on('form:validated',function() {
                    setTimeout(function() {
                        altair_md.update_input($wizard_advanced_form.find('.md-input'));
                        // adjust content height
                        $window.resize();
                    },100)
                })
                .on('field:validated',function(parsleyField) {

                    var $this = $(parsleyField.$element);
                    setTimeout(function() {
                        altair_md.update_input($this);
                        // adjust content height
                        var currentIndex = $wizard_advanced.find('.body.current').attr('data-step');
                        altair_wizard.content_height($wizard_advanced,currentIndex);
                    },100);

                });

        }
    },
    vertical_wizard: function() {
        var $wizard_vertical = $('#wizard_vertical');
        if ($wizard_vertical.length) {
            $wizard_vertical.steps({
                headerTag: "h3",
                bodyTag: "section",
                enableAllSteps: true,
                enableFinishButton: false,
                transitionEffect: "slideLeft",
                stepsOrientation: "vertical",
                onInit: function(event, currentIndex) {
                    altair_wizard.content_height($wizard_vertical,currentIndex);
                },
                onStepChanged: function (event, currentIndex) {
                    altair_wizard.content_height($wizard_vertical,currentIndex);
                }
            });
        }
    }

};