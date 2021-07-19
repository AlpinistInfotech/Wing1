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

    $scope.funcFlightSearch = function () {
        EnableLoader();
        var config = {
            headers: {
                'Authorization': 'Bearer ' + tokkenData.tokken,
                "Content-Type": "application/json; charset=utf-8"
            }
        }
        var segments = [];
        segments.push({
            "Destination": $scope.Destination, "Origin": $scope.Origin, "FlightCabinClass": 1, "TravelDt": $scope.departureDt,
            "PreferredArrival": 1, "PreferredDeparture": 1
        });
        if ($scope.journeyType == 2) {
            segments.push({
                "Destination": $scope.Origin, "Origin": $scope.Destination, "FlightCabinClass": 1, "TravelDt": $scope.returnDt,
                "PreferredArrival": 1, "PreferredDeparture": 1
            }); 
        }
        var modelData = {
            AdultCount: $scope.adultCount,
            ChildCount: $scope.childCount,
            InfantCount: $scope.infantCount,
            DirectFlight: true,
            JourneyType: $scope.journeyType,
            Segments:segments
        };
        fetch(apiRootPath + "api/Home/SearchFlight", {
            method: "POST",
            credentials: 'same-origin',
            headers: {
                "Content-Type": "application/json; charset=utf-8"
            },
            body: modelData 
        }).then(response => response.json())
            .then(data => {
                if (data.statusCode == 1){
                    $scope.mdlSearches = data.mdlSearches;
                    
                    DisableLoader();
                }
            }).catch(function (err) {
                alert("Something went wrong!", err);
            });
        
    };


});