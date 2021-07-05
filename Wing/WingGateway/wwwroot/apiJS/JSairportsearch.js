
$(document).ready(function ()
{    
    callairportAPI('ddlairportname',0);
}
)
var airport_lst;
function callairportAPI(ControlId, EmployeeId) {

    $('#loader').show();
    ControlId = '#' + ControlId;

   // localStorage.removeItem("airport_lst")
    
    var lst = localStorage.getItem("airport_lst");
    if (lst != undefined && lst != null && lst != "" && lst.length > 0) {
        airport_lst = JSON.parse(lst);    
    }

    else {

        var apiurl = localStorage.getItem("AirlinesAPIList_APIURL") + '' + localStorage.getItem("AirlinesAPIList_AirportSearch");
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
    
    if (airport_lst != null && airport_lst != "" && airport_lst.mdlSearches.length > 0) {
        
            //if (SelectedVal == 0) {
            //    $(ControlId).empty().append('<option selected="selected" value="0">All</option>');
            $(ControlId).append('<option value=""></option>');
            //}
            //else { $(ControlId).empty().append('<option selected="selected" value="0">All</option>'); }
        $.each(airport_lst.mdlSearches, function (data, value) {

            $(ControlId).append($("<option></option>").val(value.airportCode).html(value.airportName + '-' + value.airportCode + ' ' + value.cityName + ' ' + value.countryName));
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









