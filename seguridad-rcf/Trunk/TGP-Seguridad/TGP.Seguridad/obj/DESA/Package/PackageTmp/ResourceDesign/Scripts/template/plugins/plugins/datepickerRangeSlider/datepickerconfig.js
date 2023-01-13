$(function () {
        altair_form_adv.date_range()
}), altair_form_adv = {
    date_range: function () {
        var t = $("#FechaDesde"),
            e = $("#FechaHasta"),
            i = UIkit.datepicker(t, {
                format: "DD/MM/YYYY"
            }),
            n = UIkit.datepicker(e, {
                format: "DD/MM/YYYY"
            });
        t.on("change", function () {
            n.options.minDate = t.val(), setTimeout(function () {
                e.focus()
            }, 300)
        }), e.on("change", function () {
            i.options.maxDate = e.val()
        })
    }
};