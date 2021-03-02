//Load all data in index DB
var w;

if (localStorage.getItem("loadData") == null) {
    loadDatainIndexDb();
}

function loadDatainIndexDb() {
    localStorage.setItem("loadData", 1);
    location.reload();
}
function startWorker() {
    if (typeof (Worker) !== "undefined") {
        if (localStorage.getItem("loadData") == 1) {
            localStorage.setItem("loadData", 0);
            if (typeof (w) == "undefined") {
                w = new Worker("/js/downloadData.js");
            }
            w.onmessage = function (event) {
                document.getElementById("lblDataStatus").innerHTML = event.data;
            };
        }
    } else {
        document.getElementById("lblDataStatus").innerHTML = "Sorry, your browser does not support Web Workers...";
    }
}

function stopWorker() {
    w.terminate();
    w = undefined;
}
//document.onreadystatechange = function () {
//    startWorker();
//    if (document.readyState !== "complete") {
//        document.querySelector(
//            "body").style.visibility = "hidden";
//        document.querySelector(
//            "#loader").style.visibility = "visible";
//    } else {
//        document.querySelector(
//            "#loader").style.display = "none";
//        document.querySelector(
//            "body").style.visibility = "visible";
//    }
//};