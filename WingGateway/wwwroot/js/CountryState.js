(function () {
    const DB_NAME = localStorage.getItem("databaseName");
    const DB_VERSION = localStorage.getItem("databaseName");;
    var db;
    var req = indexedDB.open(DB_NAME, DB_VERSION);
    console.log("opendb");
    req.onsuccess = function (evt) {
        // Equal to: db = req.result;
        db = this.result;        
        var countrydropdown = document.getElementById("ddl_country_id");
        countrydropdown.onchange = LoadState.bind(countrydropdown.value);
        if (countrydropdown != null) {
            console.log("Remove Drop down");
            removeOptions(countrydropdown);
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


    function LoadState(countryId) {
        var req = indexedDB.open(DB_NAME, DB_VERSION);
        console.log("opendb");
        req.onsuccess = function (evt) {
            // Equal to: db = req.result;
            db = this.result;
            var statedropdown = document.getElementById("ddl_state_id")
            if (statedropdown != null) {
                console.log("Remove Drop down");
                removeOptions(statedropdown);
                var obstore = db.transaction('tblStateMaster', 'readwrite').objectStore('tblStateMaster');
                var index = objectStore.index("countryId");

                index.get(countryId).onsuccess = function (event) {
                    var scursor = event.target.result;
                    if (scursor) {
                        var option = document.createElement("option");
                        option.text = cursor.value.stateCode + " - " + cursor.value.stateName;
                        option.value = cursor.value.stateId;
                        statedropdown.add(option);
                        cursor.continue();
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



