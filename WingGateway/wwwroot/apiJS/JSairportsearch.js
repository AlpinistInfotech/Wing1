
$(document).ready(function ()
{    
    callairportAPI('ddlairportname',0);
}
)

function callairportAPI(ControlId, EmployeeId) {

    $('#loader').show();
    ControlId = '#' + ControlId;

    localStorage.removeItem("airport_lst")
    var airport_lst;
    var lst = localStorage.getItem("airport_lst");
    if (lst != undefined && lst != null && lst != "" && lst.length > 0) {
        airport_lst = JSON.parse(lst);
        
    }

    else {

        alert('calling');
        var apiurl = 'https://localhost:44341/api/Home/sEARCHaIRPORT';
        $.ajax({
            type: "POST",
            url: apiurl,
            data: {},
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            headers: {
                "Authorization": "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJfX2N1c3RvbWVySWQiOiIxIiwiX19Vc2VySWQiOiIxMSIsImV4cCI6MTYyNDU1NTc0NywiaXNzIjoid2luZy5jb20iLCJhdWQiOiJiYWJhcmFtLmNvbSJ9.rx1v1ycsLk0X1L_HUJ-j_ynPROeKdf3plWYHN7J17Ns"
            },


            success: function (response) {
                var res = response;


                if (res.statusCode == undefined) {
                    messageBox("info", res.message);
                    $('#loader').hide();
                    return false;
                }
                if (lst == "" || lst == null || lst == undefined || lst.length == 0) {
                    localStorage.setItem("airport_lst", JSON.stringify(res));
                }

                airport_lst = res;
            },
            error: function (err) {
                alert(err.responseText);
                $('#loader').hide();
                return false;
            }
        });

    }

    alert(JSON.stringify(airport_lst).length);

    if (airport_lst != null && airport_lst != "" && JSON.stringify(airport_lst).length > 0) {
        
        alert('sadasafdsadf');
            //if (SelectedVal == 0) {
            //    $(ControlId).empty().append('<option selected="selected" value="0">All</option>');
            //    // $(ControlId).append('<option value="0">All</option>');
            //}
            //else { $(ControlId).empty().append('<option selected="selected" value="0">All</option>'); }
         alert('asd')
        alert(airport_lst)
        airport_lst = JSON.stringify(airport_lst);
            $.each(airport_lst, function (data, value) {
                alert('asd')
                alert(value);
                
                $(ControlId).append($("<option></option>").val(value.airportCode).html(value.airportName));
            });

            //get and set selected value
            if (SelectedVal != '' && SelectedVal != '0' && SelectedVal != 'undefined') {
                $(ControlId).val(SelectedVal);
                $(ControlId).trigger("select2:updated");
                $(ControlId).select2();
            }

            $(ControlId).trigger("select2:updated");
            $(ControlId).select2();
        }

        $('#loader').hide();
    }









