
angular.module('CanvasViewer', []).directive('canvasViewer', ['$window', '$http', '$timeout', '$q', '$filter', function ($window, $http, $timeout, $q, $filter) {
	var formatReader = new FormatReader();

	return {
		// name: '',
		// priority: 1,
		// terminal: true,
		scope: {
			imageSource : '=src',
			overlays : '=overlays',
			title : '@title',
			options: '=options',
            content: '=content'
		}, // {} = isolate, true = child, false/undefined = no change
		// controller: ['$scope', '$element', '$attrs', '$transclude' ,function(scope, $element, $attrs, $transclude) {
		// 	console.log('la',scope.options);
		// 	console.log(scope.options);

		// }],
		// require: 'ngModel', // Array = multiple requires, ? = optional, ^ = check parent elements
		restrict: 'E', // E = Element, A = Attribute, C = Class, M = Comment
		template: '<div class="viewer-container"><canvas class="viewer" '+
				'ng-mouseleave="canMove=false"'+
				'ng-mousedown="mousedown($event)"'+
				'ng-mouseup="mouseup($event)"'+
				'ng-init="canMove=false"'+
				'ng-mousemove="mousedrag($event,canMove)">'+
				'</canvas>'+
				'<div class="title" ng-if="title!=null">{{title}}</div>'+
				'<div class="command" ng-if="options.controls.image">'+
				'<div class="btn btn-info" ng-click="options.controls.numPage=options.controls.numPage-1" ng-hide="options.controls.totalPage==1"><i class="fa fa-minus"></i></div>'+
				'<div class="btn btn-info" ng-hide="options.controls.totalPage==1">{{options.controls.numPage}}/{{options.controls.totalPage}}</div>'+
				'<div class="btn btn-info" ng-click="options.controls.numPage=options.controls.numPage+1" ng-hide="options.controls.totalPage==1"><i class="fa fa-plus"></i></div>'+				
				'<div class="btn btn-info" ng-click="resizeTo(\'page\')"><i class="fa fa-file-o"></i></div>' +
                '<div class="btn btn-info" ng-click="print()"><i class="fa fa-print"></i></div>' +
				'<div class="btn btn-info" ng-click="rotate(-1)" ng-hide="options.controls.disableRotate"><i class="fa fa-rotate-left"></i></div>'+
				'<div class="btn btn-info" ng-click="rotate(1)" ng-hide="options.controls.disableRotate"><i class="fa fa-rotate-right"></i></div>'+
				'<div class="btn btn-info" ng-click="zoom(-1)" ng-hide="options.controls.disableZoom"><i class="fa fa-search-minus"></i></div>'+
				'<div class="btn btn-info" ng-click="zoom(1)" ng-hide="options.controls.disableZoom"><i class="fa fa-search-plus"></i></div></div>'+
				'<div class="command" ng-if="options.controls.sound">'+
				'<div class="btn btn-info" ng-click="stop()"><i class="fa fa-stop"></i></div>'+
				'<div class="btn btn-info" ng-click="play()"><i class="fa fa-play"></i></div></div>'+
		'</div>',
		// templateUrl: '',
		// replace: true,
		// transclude: true,
		// compile: function(tElement, tAttrs, function transclude(function(scope, cloneLinkingFn){ return function linking(scope, elm, attrs){}})),
		link: function(scope, iElm, iAttrs, controller) {
			var	canvasEl = iElm.find('canvas')[0];
			var ctx = canvasEl.getContext('2d');
			// look for
			var inNode = angular.element(iElm.find('div')[0])[0];
			directiveParentNode = inNode.parentNode.parentNode;
			// orce correct canvas size
			var canvasSize = canvasEl.parentNode;
			ctx.canvas.width  = canvasSize.clientWidth;
			ctx.canvas.height = canvasSize.clientHeight;
			var resize = { height : canvasSize.clientHeight, width : canvasSize.clientWidth};			
			// initialize variable
			var img = null;
			var curPos = { x : 0, y : 0};
			var picPos = { x : 0, y : 0};
			var mousePos = { x : 0, y : 0};
			var overlays = [];
			var reader = null;
			var owner = "", nameHighway = "";
			fnGetAvenueInfo();
		    //lấy thông tin đơn vị vận hành
			function fnGetAvenueInfo() {
			    var url = serviceBase + 'api/TRC2700_AvenueInfo/GetAvenueInfo';
			    $http.post(url, { headers: { 'Content-Type': 'application/json' } })
                   .success(function (response) {
                       owner = response.Owner;
                       nameHighway = response.Name;
                   })
                   .error(function (errMessage, statusCode) {
                       var result = { isSuccess: false, status: statusCode, message: errMessage };
                       console.log(result.message);
                   });
			}

			// Merge scope with default values
			scope.options = angular.merge({}, {
				ctx : null,
				adsrc : null,
				zoom : {
					value : 1.0,
					step : 0.1,
					min : 0.05,
					max : 6
				},
				rotate : {
					value : 0,
					step : 90
				},
				controls : {
					toolbar : true,
					image : true,
					sound : false,
					fit : 'page',
					disableZoom : false,
					disableMove : false,
					disableRotate : false,
					numPage : 1,
					totalPage : 1,
					filmStrip : false
				},
				info : {}
			}, scope.options );

			scope.options.ctx = ctx;

			function onload() {
				if (reader == null) {
					return;
				}

				if (reader.rendered) {
					applyTransform();
				} else {
				    scope.resizeTo(scope.options.controls.fit);
				}
				scope.resizeTo('page');
			}

			scope.$watch('imageSource', function(value) {
				if (value === undefined || value === null)
					return;
				// initialize values on load
				scope.options.zoom.value = 1.0;
				scope.options.rotate.value = 0;
				curPos = { x : 0, y : 0};
				picPos = { x : 0, y : 0};

				// test if object or string is input of directive
				if (typeof(value) === 'object') {
					// Object type file
					if (formatReader.IsSupported(value.type)) {
						// get object
						var decoder = formatReader.CreateReader(value.type, value);
						// Create image
						reader = decoder.create(value, scope.options, onload, $q, $timeout, ctx);
					} else {
						console.log(value.type,' not supported !');
					}
				} else if(typeof(value) === 'string') {
					reader = formatReader.CreateReader("image/jpeg").create(value, scope.options, onload, $q, $timeout);
				}
			});

			scope.$watch('overlays', function(newarr, oldarr) {
				// initialize new overlay
				if (newarr === null || oldarr === null)
					return;

				// new added
				overlays = [];
				angular.forEach(newarr, function(item) {
					overlays.push(item);
				});

				applyTransform();
			}, true);

			scope.$watch('options.zoom.value', function() {
				if (!scope.options.controls.disableZoom) {
					applyTransform();
				}
			});

			scope.$watch('options.rotate.value', function() {
				if (!scope.options.controls.disableRotate) {
					applyTransform();
				}
			});

			scope.$watch('options.controls.fit', function(value) {
				scope.resizeTo(value);
			});

			scope.$watch('options.controls.filmStrip', function(position) {
				
				if (position) {
					scope.options.controls.disableMove = true;
					scope.options.controls.disableRotate = true;
				} else {
					scope.options.controls.disableMove = false;
					scope.options.controls.disableRotate = false;
				}
				if (reader.refresh != null) {
					reader.refresh();
				}
			});

			scope.$watch('options.controls.numPage', function(value) {
				// Limit page navigation
				if (scope.options.controls.numPage < 1) scope.options.controls.numPage = 1;
				if (scope.options.controls.numPage > scope.options.controls.totalPage) scope.options.controls.numPage = scope.options.controls.totalPage;
				if (reader != null) {
					if (scope.options.controls.filmStrip) {
						// All pages are already rendered so go to correct page
						picPos.y = (scope.options.controls.numPage - 1)  * -(reader.height+15);
						applyTransform();
					} else {
						if (reader.refresh != null) {
							reader.refresh();
						}
					}
				}
			});

			// Bind mousewheel
			angular.element(canvasEl).bind("DOMMouseScroll mousewheel onmousewheel wheel", function ($event) {

                // cross-browser wheel delta
                var event = $window.event || $event; // old IE support
                var delta = Math.max(-1, Math.min(1, (event.wheelDelta || -event.originalEvent.deltaY)));

                if (scope.options.controls.filmStrip) {
					picPos.y += 50 * delta;
					// Limit range
					if (picPos.y > 15) {
						picPos.y = 15;
					}
					if (reader.images) {
						if (picPos.y - reader.height * scope.options.zoom.value < -(reader.height + 15) * reader.images.length  * scope.options.zoom.value ) {
							picPos.y = -(reader.height + 15) * reader.images.length + reader.height;
						}
					} else {
						if (picPos.y - reader.height  * scope.options.zoom.value < -reader.height * scope.options.zoom.value ) {
							picPos.y = -reader.height * scope.options.zoom.value;
						}
					}
	                //
	                scope.$applyAsync( function() {
	                	applyTransform();
	                });
                } else {
	                if(delta > 0) {
						scope.zoom(1);
	                } else {
						scope.zoom(-1);
	                }
            	}
                // for IE
                event.returnValue = false;
                // for Chrome and Firefox
                if(event.preventDefault) {
                    event.preventDefault();
                }

            });

			function applyTransform() {
				if (reader == null) {
					return;
				}
				if (!reader.rendered) {
					return;
				}
				var options = scope.options;
				var canvas = ctx.canvas ;
				var centerX = reader.width * options.zoom.value/2;
				var centerY = reader.height * options.zoom.value/2;
				// Clean before draw
				ctx.clearRect(0,0,canvas.width, canvas.height);
				// Save context
				ctx.save();
				// move to mouse position
				ctx.translate((picPos.x + centerX), (picPos.y + centerY) );
				// Rotate canvas
				ctx.rotate( options.rotate.value * Math.PI/180);
				// Go back
				ctx.translate( - centerX, - centerY);
				// Change scale
				if (reader.isZoom)
					ctx.scale( options.zoom.value , options.zoom.value);
				if ((!options.controls.filmStrip) || (options.controls.totalPage == 1)) {
					if (reader.img != null) {
						ctx.drawImage(reader.img, 0 , 0 , reader.width , reader.height);
						ctx.beginPath();
						ctx.rect(0, 0, reader.width , reader.height );
						ctx.lineWidth = 0.2;
						ctx.strokeStyle = "#000000";
						ctx.stroke();
					}
					// Draw image at correct position with correct scale
					if (reader.data != null) {
	    				ctx.putImageData(reader.data, picPos.x, picPos.y);					
						ctx.beginPath();
						ctx.rect( 0, 0, reader.width , reader.height );
						ctx.lineWidth = 0.2;
						ctx.strokeStyle = "#000000";
						ctx.stroke();
					} 
				} else {
					if (reader.images != null) {
						angular.forEach(reader.images, function(image) { 
							ctx.drawImage(image, 0 , 0 , image.width , image.height);
							ctx.beginPath();
							ctx.rect(0, 0, image.width , image.height );
							ctx.lineWidth = 0.2;
							ctx.strokeStyle = "#000000";
							ctx.stroke();
							ctx.translate(0, image.height + 15);
						});
					}
					// Draw image at correct position with correct scale
					if (reader.data != null) {
						var offsetY = 0;
						angular.forEach(reader.data, function(data) { 
		    				ctx.putImageData(data, picPos.x, picPos.y + offsetY);					
							ctx.beginPath();
							ctx.rect( 0, 0, reader.width , reader.height );
							ctx.lineWidth = 0.2;
							ctx.strokeStyle = "#000000";
							ctx.stroke();
							offsetY += reader.height + 15;
							ctx.translate(0, offsetY);
						});
					} 
				}
				// Restore
				ctx.restore();

				// Draw overlays
				if (overlays.length >0) {
					angular.forEach(overlays, function(item) {
					    ctx.save();
						// move to mouse position
						ctx.translate((picPos.x + centerX) , (picPos.y + centerY));
						// Rotate canvas
						ctx.rotate( options.rotate.value * Math.PI/180);
						// Go back
						ctx.translate(- centerX, - centerY);
						// Change scale
						ctx.scale( options.zoom.value , options.zoom.value);
						// Start rect draw
						ctx.beginPath();
						ctx.rect((item.x ), (item.y ), item.w , item.h );
						ctx.fillStyle = item.color;
						ctx.globalAlpha = 0.4;
						ctx.fill();
						ctx.lineWidth = 1;
						ctx.strokeStyle = item.color;
						ctx.stroke();
					    ctx.restore();
					});
				}
			}

			angular.element(canvasEl).bind('mousedown' , function($event) {
				if (scope.options.controls.disableMove) {
					return;
				}

				scope.canMove = true;
				curPos.x = $event.offsetX;
				curPos.y = $event.offsetY;
			});

			angular.element(canvasEl).bind('mouseup', function($event) {
				if (scope.options.controls.disableMove) {
					return;
				}

				scope.canMove = false;
			});

			angular.element(canvasEl).bind('mousemove', function($event) {
				mousePos.x = $event.offsetX;
				mousePos.y = $event.offsetY;
				if (scope.options.controls.disableMove) {
					return;
				}

				if ((reader !== null) && (scope.canMove)) {
						var coordX = $event.offsetX;
						var coordY = $event.offsetY;
						var translateX = coordX - curPos.x;
						var translateY = coordY - curPos.y;
						picPos.x += translateX;
						picPos.y += translateY;
						applyTransform();
						curPos.x = coordX;
						curPos.y = coordY;
				}
			});

			scope.zoom = function(direction) {
				scope.$applyAsync(function() {
					var oldWidth, newWidth = 0;
					var oldHeight, newHeight = 0;
					// Does reader support zoom ?
					// Compute correct width
					if (!reader.isZoom) {
						oldWidth = reader.oldwidth;
						oldHeight = reader.height;
					} else {
						oldWidth = reader.width * scope.options.zoom.value;
						oldHeight = reader.height * scope.options.zoom.value;
					}

					// Compute new zoom
					scope.options.zoom.value += scope.options.zoom.step * direction;
					// Round
					scope.options.zoom.value = Math.round(scope.options.zoom.value*100)/100;
					if (scope.options.zoom.value >= scope.options.zoom.max) {
						scope.options.zoom.value = scope.options.zoom.max;
					}
					if (scope.options.zoom.value <= scope.options.zoom.min) {
						scope.options.zoom.value = scope.options.zoom.min;
					}
					// Refresh picture
					if (reader.refresh != null) {
						reader.refresh();
					}

					
					// Compute new image size
					if (!reader.isZoom) {
						newWidth = reader.width;
						newHeight = reader.height;
					} else {
						newWidth = reader.width * scope.options.zoom.value;
						newHeight = reader.height * scope.options.zoom.value;
					}
					// new image position after zoom
					picPos.x = picPos.x - (newWidth - oldWidth)/2;
					picPos.y = picPos.y - (newHeight - oldHeight)/2;
				});				
			}

			scope.rotate = function(direction) {
				scope.$applyAsync(function() {
					scope.options.rotate.value += scope.options.rotate.step * direction;
					if ((scope.options.rotate.value <= -360) || (scope.options.rotate.value >= 360)) {
						scope.options.rotate.value = 0;
					}
					applyTransform();
				});
			};

			var centerPics = function() {
				// Position to canvas center
				var centerX = ctx.canvas.width / 2;
				var picPosX = 0;
				picPosX =  centerX - (reader.width * scope.options.zoom.value) / 2;
				curPos = { x : picPosX, y : 0};
				picPos = { x : picPosX, y : 0};
			}

			scope.resizeTo = function(value) {
				if ((ctx.canvas == null) || (reader == null))  {
					return;
				}
				// Compute page ratio
				var options = scope.options;
				var ratioH = ctx.canvas.height / reader.height;
				var ratioW = ctx.canvas.width / reader.width;
				// If reader render zoom itself
				// Precompute from its ratio
				if (!reader.isZoom) {
					ratioH *= scope.options.zoom.value;				
					ratioW *= scope.options.zoom.value;
				}
				// Adjust value
				switch(value) {
					case 'width' : scope.options.zoom.value = ratioW; break;
					case 'height' : scope.options.zoom.value = ratioH; break;
					case 'page' :
					default : scope.options.zoom.value = Math.min(ratioH,ratioW); 
				}
				scope.$applyAsync(function() {
					// Round zoom value
					scope.options.zoom.value = Math.round(scope.options.zoom.value*100)/100;
					// Update options state
					scope.options.controls.fit = value;
					if (!reader.isZoom) {
						if (reader.refresh != null) {
							reader.refresh();
						}

						// Re center image
						centerPics();
					} else {
						// Re center image
						centerPics();
						applyTransform();
					}
				});
			}

			scope.print = function (value) {
			    var printContents = '<div style="margin-top: 50px">' +
                    'Vào hồi <span style="margin-left: 0px">' +
                    scope.content.ViolationTime.substr(0, 2) + ' </span>giờ <span>' + scope.content.ViolationTime.substr(3, 2) + ' phút,' +
                    'ngày <span>' + $filter('date')(scope.content.ViolationDate, "dd") + '</span> tháng <span>' + $filter('date')(scope.content.ViolationDate, "MM") + '</span> năm <span>' + $filter('date')(scope.content.ViolationDate, "yyyy") + '</span> </span> </div>' +
                    '<div> <span style="width: 200px">Địa điểm vi phạm:</span> <span>' + scope.content.LocationName + ' ' + nameHighway + (scope.content.Direction !== null ?' (' + scope.content.Direction + ')' : '') + '</span> </div>' +
                    '<div style="width: 800px; min-height: 25px; margin-bottom: 5px"> <span style="width: 200px; display: block; float: left">Lỗi vi phạm:</span> <span style="font-weight: bold; display: block; width: 580px; float: left">' + (scope.content.ViolationEventDetail != undefined ? scope.content.ViolationEventDetail : scope.content.ViolationName) + '</span> </div><div> <span style="width: 200px">Tốc độ đo được:</span> <span>' + (scope.content.SpeedDetected == 0 ? '' : scope.content.SpeedDetected) + 'km/h </span> </div>' +
                    '<div> <span style="width: 200px">Biển số xe vi phạm:</span> <span style="font-weight: bold">' + scope.content.PlateNumber + '</span> </div>' +
                    '<div> <span style="width: 200px">Thiết bị phát hiện vi phạm:</span> <span>' + scope.content.DeviceName + '</span> </div>' +
                    '<div> <span style="width: 200px">Đơn vị vận hành hệ thống:</span> <span style="font-weight: bold">' + owner + '</span> </div>' +
                    '<div style="margin-bottom: 50px; width: 800px; min-height: 25px"> <div><span style="width: 200px; display: block; float: left">Đường dẫn hình ảnh vi phạm:</span> <span style="display: block; float: left; width: 600px">' + scope.imageSource + '</span> </div></div>' +
                    '<div style="margin-top: 50px"> <img src=' + scope.imageSource + ' style="max-width: 100%; height: auto; display: block"/> </div></div>';
			    var popupWin = window.open("", "", "top=1,left=1,width=" + screen.availWidth + ",height=" + screen.availHeight);
			    popupWin.document.open();
			    popupWin.document.write('<html><head><title></title><style>#violation-detail span{font-weight:normal}#violation-detail div{margin-bottom:15px}span{display:inline-block}</style></head><body><div id="violation-detail"><div style="width: 800px"><div style="float: left; width: 10%;"> <img src="img/logo.png" alt="logo" style="width: 100px"></div><div style="text-align: center; text-transform: uppercase; float: right; width: 85%"><h4>hệ thống giám sát trật tự an toàn giao thông đường bộ bằng hình ảnh</h4><div style="border-top: 1px solid #000; width: 80%; margin: 0 auto"></div><h2>hình ảnh phương tiện vi phạm</h2></div></div><div style="clear: both"></div>' + printContents + '</body></html>');
			    popupWin.document.close();
			    setTimeout(function () {
			        popupWin.print();
			        popupWin.close();
			    }, 200);
			}

			scope.play = function() {
				if (scope.options.adsrc!=null) {
					scope.options.adsrc.start(0);
				}
			}

			scope.stop = function() {
				if (scope.options.adsrc!=null) {
					scope.options.adsrc.stop(0);
				}
			}
			
			function resizeCanvas() {
				scope.$applyAsync(function() {
					var canvasSize = canvasEl.parentNode;
					ctx.canvas.width  = canvasSize.clientWidth;
					ctx.canvas.height = canvasSize.clientHeight;
					applyTransform();
				});
			}
			// Keep object
			function parentChange() {
				if (resize.width != canvasEl.parentNode.clientWidth) {
					resize.width = canvasEl.parentNode.clientWidth;
				}

				if (resize.height != canvasEl.parentNode.clientHeight) {
					resize.height = canvasEl.parentNode.clientHeight;
				}
				return resize;
			}
			//
			scope.$watch(parentChange, function() {
					resizeCanvas();	
			}, true);
   //      	// resize canvas on window resize to keep aspect ratio
			// angular.element($window).bind('resize', function() {
			//  	resizeCanvas();
			// });
      	}
	};
}]);

