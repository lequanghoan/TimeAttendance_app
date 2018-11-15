//Format tiền khi nhập
Number.prototype.format = function (n, x, s, c) {
    var re = '\\d(?=(\\d{' + (x || 3) + '})+' + (n > 0 ? '\\D' : '$') + ')',
        num = this.toFixed(Math.max(0, ~~n));

    return (c ? num.replace('.', c) : num).replace(new RegExp(re, 'g'), '$&' + (s || ','));
};
$(document).on('keypress', '.inputcurrency', function (e) {
    // Allow only backspace and chấm
    if (e.keyCode == 8 || e.keyCode == 46) {
        // let it happen, don't do anything
    }
    else {
        // Ensure that it is a number and stop the keypress
        if (e.keyCode < 48 || e.keyCode > 57) {
            e.preventDefault();
        }
    }
});

$(document).on('keyup', '.inputcurrency', function (e) {
    var strNumber = $(this).val();
    if (strNumber != '') {
        strNumber = strNumber.replace(/[._]/g, '');
        if (strNumber.length > 0) {
            var number = parseInt(strNumber);
            $(this).val(number.format(0, 3, '.'));
        }
    }
});

//Check số điện thoại
$(document).on('keypress', '.inputphone', function (e) {
    // Allow only backspace
    if (e.keyCode == 8) {
        // let it happen, don't do anything
    }
    else {
        // Ensure that it is a number and stop the keypress
        if (e.keyCode < 48 || e.keyCode > 57) {
            e.preventDefault();
        }
    }
});

//Check số
$(document).on('keypress', '.inputnumber', function (e) {
    // Allow only backspace
    if (e.keyCode == 8) {
        // let it happen, don't do anything
    }
    else {
        // Ensure that it is a number and stop the keypress
        if (e.keyCode < 48 || e.keyCode > 57) {
            e.preventDefault();
        }
    }
});

//Check nhập IP
$(document).on('keypress', '.inputip', function (e) {
    // Allow only backspace and chấm
    if (e.keyCode == 8 || e.keyCode == 46) {
        // let it happen, don't do anything
    }
    else {
        // Ensure that it is a number and stop the keypress
        if (e.keyCode < 48 || e.keyCode > 57) {
            e.preventDefault();
        }
    }
});

//Số có chấm ngăn cách đơn vị và dấu ,  bắt đầu vị trí thứ 2
$(document).on('keypress', '.inputlocationpoint', function (e) {
    // Allow only backspace or chấm
    if (e.keyCode == 8 || e.keyCode == 46 || e.keyCode == 44) {
        // let it happen, don't do anything
    }
    else {
        // Ensure that it is a number and stop the keypress
        if (e.keyCode < 48 || e.keyCode > 57) {
            e.preventDefault();
        }
    }
});

//Số có chấm ngăn cách đơn vị
$(document).on('keypress', '.inputnumberpoint', function (e) {
    // Allow only backspace or chấm
    if (e.keyCode == 8 || e.keyCode == 46) {
        // let it happen, don't do anything
    }
    else {
        // Ensure that it is a number and stop the keypress
        if (e.keyCode < 48 || e.keyCode > 57) {
            e.preventDefault();
        }
    }
});
$(document).on('change', '.inputnumberpoint', function (e) {
    var strNumber = $(this).val();
    if (isNaN(strNumber)) {
        e.preventDefault();
    } 
 
});

//Check số có dấu chấm phẩy
$(document).on('keypress', '.inputnumbersemicolon', function (e) {
    // Allow only backspace and delete
    if (e.keyCode == 8 || e.keyCode == 59) {
        // let it happen, don't do anything
    }
    else {
        // Ensure that it is a number and stop the keypress
        if (e.keyCode < 48 || e.keyCode > 57) {
            e.preventDefault();
        }
    }
});

//Số có chấm ngăn cách đơn vị và dấu ,  và ;
$(document).on('keypress', '.inputpointmultiple', function (e) {
    // Allow only backspace or chấm
    if (e.keyCode == 8 || e.keyCode == 46 || e.keyCode == 44 || e.keyCode == 59) {
        // let it happen, don't do anything
    }
    else {
        // Ensure that it is a number and stop the keypress
        if (e.keyCode < 48 || e.keyCode > 57) {
            e.preventDefault();
        }
    }
});
