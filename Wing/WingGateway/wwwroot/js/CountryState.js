$(document).ready(function () {
    const DB_NAME = localStorage.getItem("databaseName");
    const DB_VERSION = localStorage.getItem("databaseVersion");;
    var db;
    var req = indexedDB.open(DB_NAME, DB_VERSION);
    console.log("opendb");
    req.onsuccess = function (evt) {
        // Equal to: db = req.result;
        db = this.result;        
        var countrydropdown = document.getElementById("ddl_country_id");
        countrydropdown.onchange = LoadState.bind();
        if (countrydropdown != null) {
            console.log("Remove Drop down");
            removeOptions(countrydropdown);

            var defaultoption = document.createElement("option");
            defaultoption.text = "Select Country";            
            countrydropdown.add(defaultoption);

            var obstore = db.transaction('tblCountryMaster', 'readwrite').objectStore('tblCountryMaster');
            obstore.openCursor().onsuccess = function (event) {
                var cursor = event.target.result;
                if (cursor) {
                    var option = document.createElement("option");
                    option.text = cursor.value.countryCode + " - " + cursor.value.countryName;
                    option.value = cursor.value.countryId;
                    countrydropdown.add(option);
                    cursor.continue();
                }
            };
        }
    };
    req.onerror = function (evt) {
        console.error("openDb:", evt.target.errorCode);
    };

    function removeOptions(selectElement) {
        var i, L = selectElement.options.length - 1;
        for (i = L; i >= 0; i--) {
            selectElement.remove(i);
        }
    }


    function LoadState() {

        let countryId = parseInt(document.getElementById("ddl_country_id").value);
        var req = indexedDB.open(DB_NAME, DB_VERSION);
        console.log("opendb");
        req.onsuccess = function (evt) {
            // Equal to: db = req.result;
            db = this.result;
            var statedropdown = document.getElementById("ddl_state_id")
            if (statedropdown != null) {
                console.log("Remove Drop down");
                removeOptions(statedropdown);
                var defaultstoption = document.createElement("option");
                defaultstoption.text = "Select State";
                statedropdown.add(defaultstoption );

                var obstore = db.transaction('tblStateMaster', 'readwrite').objectStore('tblStateMaster');
                var index = obstore.index("countryId");

                index.openCursor(countryId).onsuccess = function (event) {
                    var scursor = event.target.result;
                    if (scursor) {
                        var option = document.createElement("option");
                        option.text = scursor.value.stateCode + " - " + scursor.value.stateName;
                        option.value = scursor.value.stateId;
                        statedropdown.add(option);
                        scursor.continue();
                    }
                };

                obstore.openCursor().onsuccess = function (event) {
                    var cursor = event.target.result;
                    
                };
            }
        };
        req.onerror = function (evt) {
            console.error("openDb:", evt.target.errorCode);
        };
    }

})



