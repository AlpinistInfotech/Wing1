function checkfilter2() {
    var test = new Array()
    $("input[name='airlines']:checked").each(function () {
        test.push($(this).val());
    });
    var stoparr = new Array()
    $("input[name='stop']:checked").each(function () {
        stoparr.push($(this).val());
    });
    var deptimearr = new Array()
    $("input[name='departureTime']:checked").each(function () {
        deptimearr.push($(this).val());
    });

    $('.flight-item').each(function () {
        debugger;
        let val = $(this).find(".spnairlines").text();
        let valstop = $(this).find(".spnstop").text();
        let valdeptime = $(this).find(".deptimefil").val();
        if (valstop != "Non Stop" && valstop != "1 Stop") {
            valstop = "Many Stop";
        }
        if (test.length > 0 && stoparr.length > 0 && deptimearr.length > 0) {
            if (jQuery.inArray(val, test) !== -1 && jQuery.inArray(valstop, stoparr) !== -1 && jQuery.inArray(valdeptime, deptimearr) !== -1) {

                $(this).show();
            }
            else {
                $(this).hide();
            }
        }
        else if (test.length > 0 && stoparr.length == 0 && deptimearr.length == 0) {
            if (jQuery.inArray(val, test) !== -1) {
                $(this).show();
            }
            else {
                $(this).hide();
            }
        }
        else if (test.length == 0 && stoparr.length > 0 && deptimearr.length == 0) {
            if (jQuery.inArray(valstop, stoparr) !== -1) {
                $(this).show();
            }
            else {
                $(this).hide();
            }
        }
        else if (test.length == 0 && stoparr.length == 0 && deptimearr.length > 0) {
            if (jQuery.inArray(valdeptime, deptimearr) !== -1) {
                $(this).show();
            }
            else {
                $(this).hide();
            }
        }
        else if (test.length > 0 && stoparr.length > 0 && deptimearr.length == 0) {
            if (jQuery.inArray(val, test) !== -1 && jQuery.inArray(valstop, stoparr) !== -1) {

                $(this).show();
            }
            else {
                $(this).hide();
            }
        }
        else if (test.length > 0 && stoparr.length == 0 && deptimearr.length > 0) {
            if (jQuery.inArray(val, test) !== -1 && jQuery.inArray(valdeptime, deptimearr) !== -1) {

                $(this).show();
            }
            else {
                $(this).hide();
            }
        }
        else if (test.length == 0 && stoparr.length > 0 && deptimearr.length > 0) {
            if (jQuery.inArray(valstop, stoparr) !== -1 && jQuery.inArray(valdeptime, deptimearr) !== -1) {

                $(this).show();
            }
            else {
                $(this).hide();
            }
        }

        else if (test.length == 0 && stoparr.length == 0 && deptimearr.length == 0) {

            $(this).show();

        }
    });

}