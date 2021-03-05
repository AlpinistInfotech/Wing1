$(function () {
    let webRootPath = localStorage.getItem('_rootpath');
    if (webRootPath == null) { webRootPath = ""; }
    let sideMenutemp = JSON.parse(sessionStorage.getItem('_SideMenu'));
    let currentApplication = sessionStorage.getItem('_CurrentApplication');
    if (currentApplication == null) {
        currentApplication = 1;
    }
    if (sideMenutemp == null) {
        let requestUrl = webRootPath + "/Home/GetMenu";// + currentApplication ;
        $.ajax({ url: requestUrl }).done(function (data) {
            sessionStorage.setItem('_SideMenu', JSON.stringify(data));
            fncAddSideMenu(data)
        }).fail(function (jqXHR, exception) {
            // Our error logic here
            var msg = '';
            if (jqXHR.status === 0) {
                msg = 'Not connect.\n Verify Network.';
            } else if (jqXHR.status == 404) {
                msg = 'Requested page not found. [404]';
            } else if (jqXHR.status == 500) {
                msg = 'Internal Server Error [500].';
            } else if (exception === 'parsererror') {
                msg = 'Requested JSON parse failed.';
            } else if (exception === 'timeout') {
                msg = 'Time out error.';
            } else if (exception === 'abort') {
                msg = 'Ajax request aborted.';
            } else {
                msg = 'Uncaught Error.\n' + jqXHR.responseText;
            }
            alert(msg);
        })
    }
    else {
        fncAddSideMenu(sideMenutemp)
    }


    function fncCreateMenuIcon(iconname) {
        var elemet = document.createElement("i");
        elemet.setAttribute("class", iconname);
        return elemet;
    }
    function fncCreateMenuItem(menuname, iconname, linkname, hastree, isactive) {
        var item = document.createElement("li");

        item.classList.add("nav-item");
        if (hastree) {
            item.classList.add("has-treeview");
        }

        var itemlink = document.createElement("a");
        itemlink.href = linkname;
        itemlink.classList.add("nav-link");
        if (isactive) { itemlink.classList.add("active"); }
        itemlink.appendChild(fncCreateMenuIcon(iconname));

        var itemTxt = document.createElement("p");
        itemTxt.textContent = menuname;
        if (hastree) {
            itemTxt.appendChild(fncCreateMenuIcon("fas fa-angle-left right"));
        }
        itemlink.appendChild(itemTxt);
        item.appendChild(itemlink);
        return item;
    }
    function fncAddSideMenu(datas) {
        panelSideMenu = document.getElementById("panelSideMenu");
        var AllModules = datas._module;
        var AllSubModules = datas._subModule;
        var AllDocuments = datas._document;
        //Remove Unnecessary SubModules
        for (let i = AllSubModules.length; i >= 0; i--) {
            let isFound = false;
            for (let j = 0; j < AllDocuments.length; j++) {
                if (AllDocuments[j].subModuleId == AllSubModules[i].id) {
                    isFound = true;
                    j = AllDocuments.length;
                }
            }
            if (!isFound) {
                AllSubModules.splice(i, 1);
            }
        }
        
        //Remove Unnecessary Module
        for (let i = AllModules.length; i >= 0; i--) {
            let isFound = false;
            for (let j = 0; j < AllDocuments.length; j++) {
                if (AllDocuments[j].moduleId == AllModules[i].id) {
                    isFound = true;
                    j = AllDocuments.length;
                }
            }
            if (!isFound) {
                for (let j = 0; j < AllSubModules.length; j++) {
                    if (AllSubModules[j].moduleId == AllModules[i].id) {
                        isFound = true;
                        j = AllSubModules.length;
                    }
                }
            }
            if (!isFound) {
                AllModules.splice(i, 1);
            }
        }

        



        for (var i = 0; i < datas.length; i++) {
            var AppHeader = document.createElement("li"); AppHeader.classList.add("nav-header"); AppHeader.textContent = datas[i].name;
            panelSideMenu.appendChild(AppHeader);
            for (var j = 0; j < datas[i].modules.length; j++) {
                var ModuleItem = fncCreateMenuItem(datas[i].modules[j].name, datas[i].modules[j].icon, "#", true, datas[i].modules[j].id == document.getElementById("_currentModule").value);

                var SubModuleUL = document.createElement("ul"); SubModuleUL.setAttribute("class", "nav nav-treeview");
                for (var k = 0; k < datas[i].modules[j].subModules.length; k++) {

                    var SubModuleItem = fncCreateMenuItem(datas[i].modules[j].subModules[k].name, datas[i].modules[j].subModules[k].icon, datas[i].modules[j].subModules[k].cntrlName, true, datas[i].modules[j].subModules[k].id == document.getElementById("_currentSubModule").value);
                    var DocumentUL = document.createElement("ul"); DocumentUL.setAttribute("class", "nav nav-treeview");
                    for (var l = 0; l < datas[i].modules[j].subModules[k].documents.length; l++) {

                        var documentLink = webRootPath + "/";
                        if (datas[i].isArea) { documentLink = documentLink + datas[i].areaName + "/"; }
                        if (datas[i].modules[j].isArea) { documentLink = documentLink + datas[i].modules[j].areaName + "/"; }
                        //add controller
                        documentLink = documentLink + datas[i].modules[j].subModules[k].cntrlName + "/";
                        documentLink = documentLink + datas[i].modules[j].subModules[k].documents[l].actionName;
                        var DocumentItem = fncCreateMenuItem(datas[i].modules[j].subModules[k].documents[l].name, datas[i].modules[j].subModules[k].documents[l].icon, documentLink, false, datas[i].modules[j].subModules[k].documents[l].id == document.getElementById("_currentDocument").value);
                        DocumentUL.appendChild(DocumentItem);
                    }
                    SubModuleItem.appendChild(DocumentUL);
                    if (datas[i].modules[j].name == "") {
                        panelSideMenu.appendChild(SubModuleItem);
                    }
                    else {
                        SubModuleUL.appendChild(SubModuleItem);
                    }

                }
                if (datas[i].modules[j].name != "") {
                    ModuleItem.appendChild(SubModuleUL);
                    panelSideMenu.appendChild(ModuleItem);
                }

            }
        }
    }


});