
const DB_NAME = "b2bDatabase";
const DB_VERSION = 1;

var db = null;
var openRequest = indexedDB.open(DB_NAME, DB_VERSION);
openRequest.onupgradeneeded = function (e) {
    var thisDB = e.target.result;
    var tableList = [{ _tableName: "tblCountryMaster", _keyPath: "countryId", _index: [] },
    { _tableName: "tblStateMaster", _keyPath: "stateId", _index: [{ name: "countryId", isUnique: false }] },
    { _tableName: "tblAirportMaster", _keyPath: "id", _index: [] }];
    for (let indI = 0; indI < tableList.length; indI++) {
        if (!thisDB.objectStoreNames.contains(tableList[indI]._tableName)) {
            var objectStore = thisDB.createObjectStore(tableList[indI]._tableName, { keyPath: tableList[indI]._keyPath });
            let indexCount = tableList[indI]._index == null ? 0 : tableList[indI]._index.length;
            for (let indJ = 0; indJ < indexCount; indJ++) {
                objectStore.createIndex(tableList[indI]._index[indJ].name, tableList[indI]._index[indJ].name, { unique: tableList[indI]._index[indJ].isUnique });
            }
        }
    }
}

openRequest.onsuccess = function (e) {
    db = e.target.result;
}
openRequest.onerror = function (e) {
    console.log("Error");
    console.dir(e);
}
function LoadDataInDB(urls, postingdata, tableName) {

    urls = urls + localStorage.getItem('_apiRootPath')
    let tokkenData = JSON.parse(sessionStorage.getItem('_tokkenData'));
    fetch(urls, {
        method: "POST",
        credentials: 'same-origin',
        headers: {
            'Authorization': 'Bearer ' + tokkenData.tokken,
            "Content-Type": "application/json; charset=utf-8"
        },
        body: postingdata
    }).then(response => response.json())
        .then(data => {
            let ObjectStore = db.transaction(tableName, "readwrite")
                .objectStore(tableName);
            let ReqSuccess = ObjectStore.clear();
            ReqSuccess.onsuccess = function (event) {
                for (var i in data) {
                    ObjectStore.add(data[i]);
                }
                LoadLastDownload(tableName, true);
                return data;
            };
        });
}




function LoadLastDownload(tableName, toBeUpdate) {
    var expiryTime = new Date();
    expiryTime = expiryTime.setMinutes(expiryTime.getMinutes() - 1);
    var req = indexedDB.open(DB_NAME, DB_VERSION);
    req.onsuccess = function (evt) {
        var db = this.result;
        var obstore = db.transaction('tblLastDownload', 'readwrite').objectStore('tblLastDownload');
        if (toBeUpdate) {
            expiryTime = expiryTime.setHours(expiryTime.getHour() + 24);
            obstore.delete(tableName);
            obstore.add({ "tableName": tableName, "expiryTime": expiryTime });
        }
        else {
            var request = obstore.get(tableName);
            request.onsuccess = function (event) {
                var scursor = event.result;
                if (scursor) {
                    expiryTime = scursor.value.expiryTime;
                    scursor.continue();
                }
            };
        }


    };
    req.onerror = function (evt) {
        console.error("openDb:", evt.target.errorCode);
    };
    return expiryTime;
}
function LoadAirport() {

    var AirportData = []
    var expiryTime = LoadLastDownload("tblAirportMaster", false);
    var currentTime = new Date();
    if (currentTime > expiryTime) {
        AirportData = LoadDataInDB("/api/Home/SearchAirPort")
    }
    else {
    }
    
    var req = indexedDB.open(DB_NAME, DB_VERSION);
    req.onsuccess = function (evt) {
        // Equal to: db = req.result;
        var db = this.result;
        var statedropdown = document.getElementById("ddl_state_id")
        if (statedropdown != null) {
            console.log("Remove Drop down");
            removeOptions(statedropdown);
            var defaultstoption = document.createElement("option");
            defaultstoption.text = "Select State";
            statedropdown.add(defaultstoption);

            var obstore = db.transaction('tblAirportMaster', 'readwrite').objectStore('tblAirportMaster');
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

