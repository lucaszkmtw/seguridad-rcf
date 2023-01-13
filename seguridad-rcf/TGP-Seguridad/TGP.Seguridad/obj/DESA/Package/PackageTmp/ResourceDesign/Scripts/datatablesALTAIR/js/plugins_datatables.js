$(function() {
    altair_datatables.dt_ellipsis_plugin(),altair_datatables.dt_default(), altair_datatables.dt_scroll(), altair_datatables.dt_individual_search(), altair_datatables.dt_colVis(), altair_datatables.dt_tableExport()
}), altair_datatables = {
    dt_ellipsis_plugin:function(){
        
$.fn.DataTable.ext.pager.full_numbers_no_ellipses = function (page, pages) {
    var numbers = [];
    var buttons = $.fn.DataTable.ext.pager.numbers_length;
    var half = Math.floor(buttons / 2);
    var _range = function (len, start) {
        var end;
        if (typeof start === "undefined") {
            start = 0;
            end = len;
        } else {
            end = start;
            start = len;
        }
        var out = [];
        for (var i = start; i < end; i++) { out.push(i); }
        return out;
    };
    if (pages <= buttons) {
        numbers = _range(0, pages);
    } else if (page <= half) {
        numbers = _range(0, buttons);
    } else if (page >= pages - 1 - half) {
        numbers = _range(pages - buttons, pages);
    } else {
        numbers = _range(page - half, page + half + 1);
    }
    numbers.DT_el = 'span';
    return ['first', 'previous', numbers, 'next', 'last'];
};
    },
    dt_default: function() {        
        $.extend(true, $.fn.dataTable.defaults, {
            drawCallback: function (settings) {
                if (this.api().rows({ search: 'applied' })[0].length == 0) {
                    this.api().table().buttons().disable()
                } else {
                    this.api().table().buttons().enable()
                }
            },
            "lengthMenu": [[25, 50, 100, 300], [25, 50, 100, 300]],
            pagingType: "full_numbers_no_ellipses",
            language:{
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
            },
            dom:"<'uk-grid uk-margin-small-bottom'<'uk-width-large-3-10'l><'uk-width-large-3-10'f><'uk-width-large-4-10 uk-flex uk-flex-right'B>>tirp",
            responsive: {
                details: {
                    renderer: function (api, rowIdx, columns) {
                        var data = $.map(columns, function (col, i) {
                            return col.hidden ?
                                `<div class="uk-width-small-1-3">
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
            },
            fixedHeader:true,
        });
    },
    dt_scroll: function() {
        var t = $("#dt_scroll");
        t.length && t.DataTable({
            scrollY: "200px",
            scrollCollapse: !1,
            paging: !1
        })
    },
    dt_individual_search: function() {
        var t = $("#dt_individual_search");
        t.length && (t.find("tfoot th").each(function() {
            var a = t.find("tfoot th").eq($(this).index()).text();
            $(this).html('<input type="text" class="md-input searchTablaBtn" placeholder="' + a + '" />')
        }), altair_md.inputs(), t.DataTable().columns().every(function() {
            var t = this;
            $("input", this.footer()).on("keyup change", function() {
                t.search(this.value).draw()
            })
        }))
    },
    dt_colVis: function() {
        var t = $("#dt_colVis"),
            a = t.prev(".dt_colVis_buttons");
        t.length && t.DataTable({
            buttons: [{
                extend: "colvis",
                fade: 0
            }]
        }).buttons().container().appendTo(a)
    },
    dt_tableExport: function() {
        var t = $("#dt_tableExport"),
            a = t.prev(".dt_colVis_buttons");
        t.length && t.DataTable({
            buttons: [{
                extend: "copyHtml5",
                text: '<i class="uk-icon-files-o"></i> Copy',
                titleAttr: "Copy"
            }, {
                extend: "print",
                text: '<i class="uk-icon-print"></i> Print',
                titleAttr: "Print"
            }, {
                extend: "excelHtml5",
                text: '<i class="uk-icon-file-excel-o"></i> XLSX',
                titleAttr: ""
            }, {
                extend: "csvHtml5",
                text: '<i class="uk-icon-file-text-o"></i> CSV',
                titleAttr: "CSV"
            }, {
                extend: "pdfHtml5",
                text: '<i class="uk-icon-file-pdf-o"></i> PDF',
                titleAttr: "PDF"
            }]
        }).buttons().container().appendTo(a)
    }
};