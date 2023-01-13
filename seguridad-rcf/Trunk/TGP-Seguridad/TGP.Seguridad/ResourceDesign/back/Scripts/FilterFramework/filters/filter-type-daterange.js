/* se utiliza solo el input no se snecesita especificar el plugins */
class FilterTypeDateRange extends Filter {
	constructor(jquery_combo, defaultEmpty, dat1 = null, dat2 = null, config = null) {
		super(jquery_combo);
		this._date1 = dat1;
		this._date2 = dat2;
		this._config = config;
		if (defaultEmpty != undefined) { this._fechaDefaultEmpty = true; }
		else {
			this._fechaDefaultEmpty = false;
			this._fechaDefecto = this.combo.parent().parent().find(".fechaPorDefecto");
		}
		this.applyEvent();
	}

	applyEvent() {
		let self = this._self;
		this.combo.unbind("datepicker-change");
		this.combo.dateRangePicker((this._config != null) ? this._config : self.configDateRangePicker()).bind('datepicker-change', function (event, obj) {
			self.changeValueDateRange(obj)
		});
		//    .bind('datepicker-first-date-selected', function (event, obj) {
		//    self.changeValueDateRange(obj)
		//});


		//evento para cuando se escribe la fecha
		this.combo.change(function () {
			let auxdates = self.combo.val().split("al");
			if (auxdates.length != 1) {
				self.date1 = auxdates[0].trim();
				self.date2 = auxdates[1].trim();
				self.value = self.combo.val();
				self.text = self.combo.val();
				self.addToFather();
			} else {
				self.date1 = "";
				self.date2 = "";
				self.value = "";
				self.text = "";
				self.addToFather();
			}
		});

		self.change();
	}


	changeValueDateRange(obj) {
		if (obj != undefined) {
			this.date1 = moment(obj.date1, 'DD-MM-YYYY').format("DD-MM-YYYY");
			this.date2 = moment(obj.date2, 'DD-MM-YYYY').format("DD-MM-YYYY");
			this.value = obj.value;
			this.text = obj.value;
			this.addToFather();
		} else {
			let auxdates = this.combo.val().split("al");
			if (auxdates.length != 1) {
				this.date1 = auxdates[0];
				this.date2 = auxdates[1];
				this.value = this.combo.val();
				this.text = this.combo.val();
				this.addToFather();
			}
		}
	}

	applyDefaultDateEmpty() {
		this.date1 = "";
		this.date2 = "";
		this.value = "";
		this.text = "";
		this.combo.val("");
		this.combo.text = "";
	}
	applyDefaultDate() {
		if (this._fechaDefaultEmpty) {
			this.applyDefaultDateEmpty();
		} else {
			this._fechaDefecto.click();

		}

	}

	change() {
		if (!isNullOrEmpty(this.date1) && !isNullOrEmpty(this.date2)) {
			this.combo.data('dateRangePicker').setDateRange(this.date1, this.date2);
		} else {
			//this._fechaDefecto.click();
			this.applyDefaultDate();
		}
		this.combo.trigger("datepicker-change");
	}

	selectValue(date1, date2) {
		if (!isNullOrEmpty(date1) && !isNullOrEmpty(date2)) {
			this.date1 = date1.trim();
			this.date2 = date2.trim();
			this.change();
		} else {
			this.applyDefaultDate();
			//			console.error("No se pueden setear fechas nulas o vacias")
		}
	}

	configDateRangePicker() {
		return {
			format: 'DD-MM-YYYY',
			language: 'es',
			separator: ' al ',
			monthSelect: true,
			yearSelect: true,
			getValue: function () {
				return $(this).val();
			}
		}
	}

	get date1() {
		return this._date1;
	}
	get date2() {
		return this._date2;
	}
	set date1(date) {
		this._date1 = date;
	}
	set date2(date) {
		this._date2 = date;
	}

	get defaultDate() {
		return this._fechaDefecto;
	}
	set defaultDate(date) {
		this._fechaDefecto = date;
	}

	disable() {
		var element = this.combo;
		localStorage.setItem(this.id + "date1", this.date1);
		localStorage.setItem(this.id + "date2", this.date2);
		element.prop('disabled', true);
		this.date1 = "";
		this.date2 = "";
		this.value = "";
		this.text = "";
		this.combo.val("");
		this.addToFather();
		element.parent().siblings().first().children().first().addClass("uk-text-muted");
		element.parent().siblings(".uk-grid-collapse").children().each(function () {
			$(this).addClass('disabled');
			$(this).removeClass("md-btn-accent")
		});
	}

	enable() {
		var element = this.combo;
		element.prop('disabled', false);
		if (localStorage.getItem(this.id + "date1")) {
			this.date1 = localStorage.getItem(this.id + "date1");
			this.date2 = localStorage.getItem(this.id + "date2");
			this.change();
		} else {
			this.change();
		}
		element.parent().siblings().first().children().first().removeClass("uk-text-muted");
		element.parent().siblings(".uk-grid-collapse").children().each(function () {
			$(this).addClass("md-btn-accent");
			$(this).removeClass('disabled')
		});
	}

	clear() {
		//this._fechaDefecto.click();
		this.applyDefaultDate();
		this.combo.change();
	}
}
