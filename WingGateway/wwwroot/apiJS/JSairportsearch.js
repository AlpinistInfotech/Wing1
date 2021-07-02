
$(document).ready(function ()
{    
    callairportAPI();
}
)

function callairportAPI() {


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
            , "Content-Type": "application/json"
            ,"Access-Control-Allow-Origin":"*"

        },

       
        success: function (response) {
            var res = response;

            alert(res);
            if (res.statusCode != undefined) {
                messageBox("info", res.message);
                $('#loader').hide();
                return false;
            }

            if (lst == "" || lst == null || lst.length == 0) {
                localStorage.setItem("emp_companies_lst", JSON.stringify(res));
            }

            emp_company_lst = res;


        },
        error: function (err) {
            alert(err.responseText);
            $('#loader').hide();
            return false;
        }
    });

}



function BindAllEmp_Company(ControlId, EmployeeId, SelectedVal) {

    $('#loader').show();
    ControlId = '#' + ControlId;

    var emp_company_lst;
    var lst = localStorage.getItem("emp_companies_lst");

    if (lst != undefined && lst != null && lst != "" && lst.length > 0) {

        emp_company_lst = JSON.parse(lst);
    }
    else {

        var listapi = localStorage.getItem("ApiUrl");

        $.ajax({
            type: "GET",
            url: listapi + "apiEmployee/Get_Emp_all_Company/" + EmployeeId,
            data: {},
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            headers: { 'Authorization': 'Bearer ' + localStorage.getItem('Token') },
            success: function (response) {
                var res = response;

                if (res.statusCode != undefined) {
                    messageBox("info", res.message);
                    $('#loader').hide();
                    return false;
                }

                if (lst == "" || lst == null || lst.length == 0) {
                    localStorage.setItem("emp_companies_lst", JSON.stringify(res));
                }

                emp_company_lst = res;


            },
            error: function (err) {
                alert(err.responseText);
                $('#loader').hide();
                return false;
            }
        });

    }

    if (emp_company_lst != null && emp_company_lst != "" && emp_company_lst.length > 0) {


        if (SelectedVal == 0) {
            $(ControlId).empty().append('<option selected="selected" value="0">All</option>');
            // $(ControlId).append('<option value="0">All</option>');
        }
        else { $(ControlId).empty().append('<option selected="selected" value="0">All</option>'); }

        $.each(emp_company_lst, function (data, value) {

            $(ControlId).append($("<option></option>").val(value.company_id).html(value.company_name));
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



