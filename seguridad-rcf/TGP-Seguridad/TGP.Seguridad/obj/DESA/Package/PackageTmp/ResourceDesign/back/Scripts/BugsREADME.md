/*
Errores que aparecen en sigaf:

  1***Error en daterange por seleccion automatica de fecha actual (con evento click)
  sumado a una fecha maxima permitida que es la misma que la fecha por defecto

  2***Error de fixedheader de datatable. Cuando hay mas de un scroll en la pantalla el fixedheader de datatable se 
  vuelve loco porque no reconoce su techo superior en la vista del dom y sigue de largo...

*/

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/*1. daterange bug */

//se debe incorporar estos scripts y hacer uso de "corroborarDateRangeUndefinedElmts" en dos eventos importates:
//'datepicker-open' y en los eventos click de los botones $("span.next") y $("span.prev")


function corroborarDateRangeUndefinedElmts(date = moment(new Date)) {
    var wrappers = $(".date-picker-wrapper .month-wrapper .select-wrapper:contains('undefined')");
    for (let wrap of wrappers) {
        let type = $(wrap).find("select").attr("class");
        if (type == "month") {
            wrap.childNodes[1].nodeValue = returnMonth(date.format("MM"));
        } else {
            if (returnMonth(date.format("MM")) == "Diciembre") {
                wrap.childNodes[1].nodeValue = date.add(1, "year").format("YYYY");
            } else {
                wrap.childNodes[1].nodeValue = date.format("YYYY");
            }
        }
    }
}

function returnMonth(number) {
    let name = "";
    switch (number) {
        case "01": name= "Enero"; break;
        case "02": name= "Febrero"; break;
        case "03": name= "Marzo"; break;
        case "04": name= "Abril"; break;
        case "05": name= "Mayo"; break;
        case "06": name= "Junio"; break;
        case "07": name = "Julio"; break;
        case "08": name = "Agosto"; break;
        case "09": name = "Septiembre"; break;
        case "10": name = "Octubre"; break;
        case "11": name = "Noviembre"; break;
        case "12": name = "Diciembre"; break;
    }
    return name;
}


//eventos que usan
$("#reportrange-busqueda").dateRangePicker(configDateRangePickerWithMaxMin("fechapordefecto.enformato.dd/mm/yyyy"))
        .bind('datepicker-open', function (event, obj) {
            corroborarDateRangeUndefinedElmts(moment("fechapordefecto.enformato.dd/mm/yyyy", 'DD/MM/YYYY'));
        })
        .bind('datepicker-change', function (event, obj) {
            date1 = moment(obj.date1, 'DD-MM-YYYY').format("DD-MM-YYYY");
            date2= moment(obj.date2, 'DD-MM-YYYY').format("DD-MM-YYYY");
        });
$("span.next").click(() => { setTimeout(corroborarDateRangeUndefinedElmts(moment("fechapordefecto.enformato.dd/mm/yyyy", 'DD/MM/YYYY')),500) })
$("span.prev").click(() => { setTimeout(corroborarDateRangeUndefinedElmts(moment("fechapordefecto.enformato.dd/mm/yyyy", 'DD/MM/YYYY')),500) })



/*1. fixedHeader DATATABLE bug */

    /*FIXER para Datatable
        *problema de fixedheader con scroll en tabla (overflow en y)
        TABLA1 Y TABLA2 son Objetos instacioados con DATATABLE
        */
    $(document).scroll(function (e) {
        fixFixerHeader(TABLA1);
        fixFixerHeader(TABLA2);
        }
    )

    function fixFixerHeader(tabla) {
        if (!isNullOrEmpty(tabla)) {
            let idtabla = tabla.table().node().id;
            let topTable = $("#" + idtabla).closest("div[id*='div-resultados']").offset().top;
            let documentScroll = $(document).scrollTop();
            if (documentScroll < topTable) {
                if ($(`[aria-describedby='${idtabla}_info'].fixedHeader-floating`).length === 1)
                    tabla.fixedHeader.disable();
            }
            else {
                tabla.fixedHeader.enable();
            }
        }
    }
    /****************************/