window.setCookie = function (cname, cvalue, exdays, path = '/', samesite = 'strict') {
    let d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    document.cookie = cname + '=' + cvalue + ';expires=' + d.toUTCString() +
        ';path=' + path + ';  samesite=' + samesite + ';';
}

window.getCookie = function (cname) {
    let name = cname + '=';
    let ca = document.cookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) === ' ') c = c.substring(1);
        if (c.indexOf(name) === 0) return c.substring(name.length, c.length);
    }
    return '';
}

document.addEventListener('DOMContentLoaded', function () {
    if (!mustAcceptCookies) {
        // client timezone
        if (getCookie('HomegrownAnalytics.TimeOffset') === '') {
            setCookie('HomegrownAnalytics.TimeOffset', new Date().getTimezoneOffset(), 1, '/', 'strict');
        }
        // client screen width and height
        if (getCookie('HomegrownAnalytics.ScreenSize') === '') {
            setCookie('HomegrownAnalytics.ScreenSize', window.screen.availWidth.toString() +
                'X' + window.screen.availHeight.toString(), 365, '/', 'strict');
        }
    }
});

function EnableLoader() {
    document.getElementById("loader").style.display = "block"
}
function DisableLoader() {
    document.getElementById("loader").style.display = "none"
}

