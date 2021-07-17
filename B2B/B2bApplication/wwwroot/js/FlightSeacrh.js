var app = angular.module('flightSearchApp', []);
app.controller('flightCtrl', function ($scope, $http) {
    EnableLoader();
    var apiRootPath = localStorage.getItem("_apiRootPath");
    let tokkenData = {};
    let temptokkenData = sessionStorage.getItem('_tokkenData')    
    if (tokkenData == null) {
        location.replace("/");
    } else {
        tokkenData = JSON.parse(sessionStorage.getItem('_tokkenData'));
    }
    let tempairport = localStorage.getItem("Airport");    
    if (tempairport == null) {
        var config = {
            headers: {
                'Authorization': 'Bearer ' + tokkenData.tokken,
                "Content-Type": "application/json; charset=utf-8"
            }
        }
        $http.get(apiRootPath +"api/Home/SearchAirPort", config)
            .then(function (response) {
                if (response.data.statusCode == 1) {
                    $scope.airportData = response.data.tblAirports;
                    localStorage.setItem("Airport", JSON.stringify($scope.airportData));
                    DisableLoader();
                }
            });
    }
    else {
        $scope.airportData = JSON.parse(tempairport);
        DisableLoader();
    }

    


    
});