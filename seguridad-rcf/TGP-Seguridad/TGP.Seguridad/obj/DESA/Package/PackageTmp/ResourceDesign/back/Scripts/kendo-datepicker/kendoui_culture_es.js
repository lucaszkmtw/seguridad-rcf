 kendo.cultures["es-ES"] = {
            //<language code>-<country/region code>
            name: "es-ES",
            calendars: {
                standard: {
                    days: {
                        // full day names
                        names: ["Domingo", "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado"],
                        // abbreviated day names
                        namesAbbr: ["Dom", "Lun", "Mar", "Mie", "Jue", "Vie", "Sab"],
                        // shortest day names
                        namesShort: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"]
                    },
                    months: {
                        // full month names
                        names: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
                        // abbreviated month names
                        namesAbbr: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic']
                    },
                    // AM and PM designators
                    // [standard,lowercase,uppercase]
                    AM: ["AM", "am", "AM"],
                    PM: ["PM", "pm", "PM"],
                    // set of predefined date and time patterns used by the culture.
                    patterns: {
                        d: "d-MM-yyyy",
                        D: "dddd dd , MMMM , yyyy",
                        F: "dddd dd , MMMM , yyyy h:mm:ss tt",
                        g: "d-MM-yyyy h:mm tt",
                        G: "d-MM-yyyy h:mm:ss tt",
                        m: "MMMM dd",
                        M: "MMMM dd",
                        s: "yyyy'-'MM'-'ddTHH':'mm':'ss",
                        t: "h:mm tt",
                        T: "h:mm:ss tt",
                        u: "yyyy'-'MM'-'dd HH':'mm':'ss'Z'",
                        y: "MMMM, yyyy",
                        Y: "MMMM, yyyy"
                    },
                    // the first day of the week (0 = Sunday, 1 = Monday, etc)
                    firstDay: 0
                }
            }
        };