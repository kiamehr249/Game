
var rootObjects = {};

function getInstance(vueObj, name) {
	var instance = new Vue(vueObj);
	rootObjects[name] = instance;
	return instance;
}

var fileIcons = {
	image: '<i class="fa fa-file-image-o" aria-hidden="true"></i>',
	video: '<i class="fa fa-file-video-o" aria-hidden="true"></i>',
	sound: '<i class="fa fa-file-audio-o" aria-hidden="true"></i>',
	other: '<i class="fa fa-file-archive-o" aria-hidden="true"></i>'
};

var fileExtentions = {
	image: ['jpg', 'png', 'jepeg', 'gif'],
	video: ['mp4', 'mwv', '3gp'],
	sound: ['pm3', '3ga'],
	other: ['pdf', 'doc', 'xlsx', 'xls']
}

//mixins: [utilities]
var utilities = {
	methods: {
		postApi: function (apiurl, apiRequest) {
			return fetch(apiurl, {
				method: 'POST',
				headers: {
					'Accept': 'application/json',
					'Content-Type': 'application/json;charset=utf-8'
				},
				body: JSON.stringify(apiRequest)
			}).finally(() => {
				}).then(response => response.json());
		},
		getApi: function (apiurl) {
			return fetch(apiurl, {
				method: 'GET',
				headers: {
					'Accept': 'application/json',
					'Content-Type': 'application/json;charset=utf-8'
				},
			}).finally(() => {
			}).then(response => response.json());
		},
		pdate: function (thed, withhour, formatstring) {
			if (thed === undefined || thed === null) {
				return "";
			}

			if (formatstring != undefined) {
				return new persianDate(thed).format(formatstring);
			}

			if (withhour) {
				return new persianDate(thed).format('YYYY/MM/DD HH:mm');
			}
			return new persianDate(thed).format('YYYY/MM/DD');
		},
		faToEnNumbers: function (str) {
			if (str === undefined || str == null || str === '') {
				return '';
			}
			return str.toString().replace(/[۰-۹]/g, d => '۰۱۲۳۴۵۶۷۸۹'.indexOf(d));
		},
		enToFaNumbers: function (str) {
			if (str === undefined || str == null || str === '') {
				return '';
			}
			return str.toString().replace(/\d/g, d => '۰۱۲۳۴۵۶۷۸۹'[d]);
		},
		getIconByFileUrl: function (url) {
			var lex = url.lastIndexOf('.');
			var extention = url.substring(lex + 1, url.length).toLowerCase();
			console.log(extention);
			var result = '';
			if (fileExtentions.image.includes(extention)) {
				result = fileIcons.image;
			} else if (fileExtentions.video.includes(extention)) {
				result = fileIcons.video;
			} else if (fileExtentions.sound.includes(extention)) {
				result = fileIcons.sound;
			} else if (fileExtentions.other.includes(extention)) {
				result = fileIcons.other;
			}

			return result;
		},
		addSeperator: function (str) {
			if (str === undefined || str == null || str === '') {
				return '';
			}
			return this.completeReplace(str.toString(), ',', '').replace(/\B(?=(\d{3})+(?!\d))/g, ",");
		},
		completeReplace: function (str, find, replace) {
			if (str === undefined || str == null || str === '') {
				return '';
			}
			return str.replace(new RegExp(find, 'g'), replace);
		},
		GetFileName: function (FilePath) {
			if (FilePath === undefined || FilePath === null) {
				return "";
			}
			p = FilePath.lastIndexOf("/");
			if (p == -1) {
				return "";
			}
			return FilePath.substring(p + 1)
		},
		checkDateControls: function (ids) {
			$.each(ids, function (ix, vl) {
				setDateControl('#' + vl);
			});
		},
		stringLimit: function (str, len) {
			if (str.length > len) {
				return { value: str.substring(0, len) + " ...", isMore: true };
			} else {
				return { value: str, isMore: false };
			}
		},
		showMessage: function(input) {
			new Noty({
				text: input.message,
				layout: 'topRight',
				type: input.type,
				theme: 'sunset',
				timeout: 3000
			}).show();
		},
		faDate: function (fullDate, type) {
            var dObj = new persianDate(Date.parse(fullDate)).toLocale('fa');
            if (type == 1) {
                return dObj.format('YYYY/MM/DD');
            } else if (type == 2) {
                return dObj.format('MMMM DD YYYY');
			}
			else if(type == 3) {
				return dObj.format('YYYY/MM/DD, h:mm:ss a');
            } else {
                return dObj.format();
            }
        },
        enDate: function (fullDate, type) {
            var dObj = new persianDate(Date.parse(fullDate)).toLocale('en').toCalendar('gregorian');
            if (type == 1) {
                return dObj.format('MMMM DD YYYY');
            } else if (type == 2) {
                return dObj.format('YYYY-MM-DD h:mm:ss a');
            } else {
                return dObj.format();
            }
        },
		imageControl: function(ev, objSrc, objItem, funcName) {
			var self = this;
			var thisItem = ev.target;
			if (thisItem.files && thisItem.files[0]) {
				this.form[objItem] = thisItem.files[0];
				var reader = new FileReader();
				reader.readAsDataURL(thisItem.files[0]);
				reader.onload = function (e) {
					self.form[objSrc] = e.target.result;
					if(funcName != null && funcName != '') {
						self[funcName]();
					}
				}
				console.log('loaded2');
			}
		},
		setCropper: function(img) {
			var self = this;
			console.log(img);
			var image = $(String(img))[0];
			self.cropperData.cropperObj = cropper = new Cropper(image, {
				preview: '.img-preview',
				ready: function (e) {
				  console.log(e.type);
				},
				cropstart: function (e) {
				  console.log(e.type, e.detail.action);
				},
				cropmove: function (e) {
				  console.log(e.type, e.detail.action);
				},
				cropend: function (e) {
				  console.log(e.type, e.detail.action);
				},
				crop: function (e) {
				  var data = e.detail;
				  console.log(e.type);
				  self.cropperData.dataX = Math.round(data.x);
				  self.cropperData.dataY = Math.round(data.y);
				  self.cropperData.dataHeight = Math.round(data.height);
				  self.cropperData.dataWidth = Math.round(data.width);
				  self.cropperData.dataRotate = typeof data.rotate !== 'undefined' ? data.rotate : '';
				  self.cropperData.dataScaleX = typeof data.scaleX !== 'undefined' ? data.scaleX : '';
				  self.cropperData.dataScaleY = typeof data.scaleY !== 'undefined' ? data.scaleY : '';
				},
				zoom: function (e) {
				  console.log(e.type, e.detail.ratio);
				}
			});
		},
		addSeperator: function (amount) {
			if (amount === null) {
				return null;
			}
			///.match(/\d|\.|\-/g
			amount = amount.toString().match(/\d/g).join("");
			if (
				amount !== "" ||
				amount !== undefined ||
				amount !== 0 ||
				amount !== "0" ||
				amount !== null
			) {
				return amount.substr(0, 18).replace(/\B(?=(\d{3})+(?!\d))/g, ",");
			} else {
				return amount;
			}
		},
		removeSeperator: function (amount) {
			if (amount === null) {
				return null;
			}
			return Number(amount.replace(/\,/g, ""));
		},
		checkNumber: function (inputVal) {
			if (inputVal != null && inputVal != "" && inputVal != undefined) {
				if (isNaN(inputVal)) {
					var strVal = String(inputVal);
					if (strVal.length === 1 || strVal === "NaN") {
						return '';
					} else {
						return parseFloat(
							strVal.substring(0, strVal.length - 1)
						);
					}
				} else {
					return parseFloat(inputVal);
                }
			}
		},
		checkStrNumber: function (entry) {
			if (entry != null && entry != "" && entry != undefined) {
				if (isNaN(entry)) {
					var strVal = String(entry);
					if (strVal.length === 1 || strVal === "NaN") {
						return '';
					} else {
						return strVal.substring(0, strVal.length - 1);
					}
				} else {
					return String(entry);
				}
			}
		},
	}
}
