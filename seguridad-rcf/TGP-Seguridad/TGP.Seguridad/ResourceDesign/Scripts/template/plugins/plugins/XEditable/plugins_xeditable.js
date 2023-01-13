$(function(){
    editable.x_options();
});

editable = {   
    x_options: function () {
        //defaults
        //$.fn.editable.defaults.mode = 'inline';
        $.fn.editable.defaults.url = '/post';
        $.fn.editabletypes.combodate.defaults.inputclass = 'md-input';
        $.fn.editabletypes.email.defaults.inputclass = 'md-input';
        $.fn.editabletypes.number.defaults.inputclass = 'md-input';
        $.fn.editabletypes.password.defaults.inputclass = 'md-input';
        $.fn.editabletypes.select.defaults.inputclass = 'md-input';
        $.fn.editabletypes.tel.defaults.inputclass = 'md-input';
        $.fn.editabletypes.text.defaults.inputclass = 'md-input';
        $.fn.editabletypes.textarea.defaults.inputclass = 'md-input';
        $.fn.editabletypes.time.defaults.inputclass = 'md-input';
        $.fn.editabletypes.url.defaults.inputclass = 'md-input';


        $.fn.editableform.buttons = '<div class="editable-footer">' +
            '<button type="button" class="editable-cancel md-btn md-btn-small md-btn-flat">cancel</button>' +
            '<button type="submit" class="editable-submit md-btn md-btn-small md-btn-accent">ok</button>'+
            '</div>';
    } 
};