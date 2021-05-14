function windowMinWidthMatchesQuery(dotNetHelper) {
    var pendingCall;
    var match = window.matchMedia("(min-width: 1200px)")
    handleMinWidthQuery(match).then(function () { match.addListener(handleMinWidthQuery) });
    function handleMinWidthQuery(queryMatch) {
        return (pendingCall || Promise.resolve(true))
            .then(function () {
                return pendingCall = new Promise(function (resolve, reject) {
                    dotNetHelper.invokeMethodAsync('OnWindowMinWidthQueryChanged', queryMatch.matches).then(resolve).catch(reject);
                });
            });
    }
}

function loginLanguage() {
    var supportedCuls = ["en", "de", "vi"];

    var currentLangLocalStorage = localStorage.getItem("BlazorCulture");

    var browserLang = navigator.language || navigator.userLanguage;
    browserLang = browserLang.substring(0, 2);

    if (supportedCuls.includes(browserLang)) {
        localStorage.setItem("BlazorCulture", browserLang);
    }
    else {
        localStorage.setItem("BlazorCulture", "en");
    }

    if (!currentLangLocalStorage.includes(browserLang)) {
        if (currentLangLocalStorage.includes("en")) {
            if (!supportedCuls.includes(browserLang)) {

            }
            else {
                window.location.replace("");
            }
        }
        else {
            window.location.replace("");
        }
    }
}

function changePasswordVisibility(cssClass, showPassword) {
    var passInput = document.querySelector("." + cssClass + " input");
    if (passInput) {
        passInput.type = showPassword ? "text" : "password";
    }
}

function disableScreen(isDisable) {
    var modal = document.querySelector('.modal-backdrop.show.dxbs-modal-back');
    var loadingIcon = document.getElementById("LoadingIcon");

    if (modal && loadingIcon) {
        if (isDisable) {
            modal.classList.add("enableScreen");
            modal.classList.remove("disableScreen");
            loadingIcon.classList.remove("hideElement");
            loadingIcon.classList.add("showElement");
        }
        else {
            modal.classList.add("disableScreen");
            modal.classList.remove("enableScreen");
            loadingIcon.classList.remove("showElement");
            loadingIcon.classList.add("hideElement");
        }
    }
}

function enableNavMenu(enable, time) {
    var navbar = document.querySelector(".sidebar nav");

    if (!enable) {
        navbar.classList.remove("showElement");
        navbar.classList.add("hideElement");

        setTimeout(function () {
            navbar.classList.remove("hideElement");
            navbar.classList.add("showElement");
        }, time);
    }
    else {
        navbar.classList.remove("hideElement");
        navbar.classList.add("showElement");
    }
}

window.addEventListener("load", function () {
    var preventDefaultOnEnterCallback = function (e) {
        if (e.keyCode === 13 || e.key === "Enter") {
            e.preventDefault();
            return false;
        }
    };
    function setupPreventDefaultOnEnterOnNode(node, add) {
        if (node instanceof HTMLElement) {
            var el = node;
            if (el.classList.contains("prevent-default-on-enter") && add) {
                el.addEventListener('keydown', preventDefaultOnEnterCallback, false);
            }
            else {
                el.removeEventListener('keydown', preventDefaultOnEnterCallback, false);
            }
        }
    }
    function setupPreventDefaultOnEnterOnElements(nodelist, add) {
        for (var i = 0; i < nodelist.length; i++) {
            var node = nodelist[i];
            if (node instanceof HTMLElement) {
                var el = node;
                setupPreventDefaultOnEnterOnNode(node, add);
                var elements = el.getElementsByClassName("prevent-default-on-enter");
                for (var i_1 = 0; i_1 < elements.length; i_1++) {
                    setupPreventDefaultOnEnterOnNode(elements[i_1], add);
                }
            }
        }
    }
    var preventDefaultOnEnterObserver = new MutationObserver(function (mutations) {
        for (var _i = 0, mutations_1 = mutations; _i < mutations_1.length; _i++) {
            var mutation = mutations_1[_i];
            if (mutation.type === 'childList') {
                setupPreventDefaultOnEnterOnElements(mutation.addedNodes, true);
            }
            else if (mutation.type === 'attributes') {
                if (mutation.attributeName === "class") {
                    setupPreventDefaultOnEnterOnNode(mutation.target, false);
                    setupPreventDefaultOnEnterOnNode(mutation.target, true);
                }
            }
        }
    });
    var preventDefaultOnEnterObserverConfig = { subtree: true, childList: true, attributes: true };
    preventDefaultOnEnterObserver.observe(document, preventDefaultOnEnterObserverConfig);
    setupPreventDefaultOnEnterOnElements(document.getElementsByClassName("prevent-default-on-enter"), true);
});

function focusEditor(className) {
    setTimeout(function () {
        document.getElementsByClassName(className)[0].querySelector("input").focus();
    }, 500);
}

function focusThemeSwitcher() {
    setTimeout(function () {
        document.getElementById("settingsbar").focus();
    }, 500);
}

function deleteAnotationErrorMessage() {
    let txtBox = document.querySelector('.mytext');
    if (txtBox == null) {
        return;
    }
    let errorMessages = txtBox.children;
    if (errorMessages.length < 2 || errorMessages == null || errorMessages == undefined) {
        return;
    }

    var count = errorMessages.length;
    while (count != 1) {
        let tempChild = errorMessages[count - 1];
        console.log(tempChild);
        //tempChild.remove();
        tempChild.style.display = "none";
        count--;
    }
}

function timeoutLoadingIcon() {
    setTimeout(function () {
        var modalLoading = document.querySelector('.modal.dxbs-modal.customModal');
        var loadingIcon = document.querySelector('#LoadingIcon');

        if (modalLoading && loadingIcon) {
            modalLoading.classList.add('hideElement');
            loadingIcon.classList.remove('showElement');
            loadingIcon.classList.add('hideElement');
        }
    }, 10000);
}

function showModalUnauthorized() {
    var modalLoading = document.querySelector('.modal.dxbs-modal.customModal');
    var loadingIcon = document.querySelector('#LoadingIcon');

    if (modalLoading && loadingIcon) {
        modalLoading.classList.add('hideElement');
        loadingIcon.classList.add('hideElement');
    }

    $('#exampleModal').modal('show');

    var value = 10;
    setInterval(function () {
        document.getElementById('counter').innerText = value;
        value -= 1;
    }, 1000);

    setTimeout(function () {
        location.reload();
    }, 10000);
}

var defaultTimeout = 150;
function changeNoDataText(gridCss, customText, timeout) {
    setTimeout(function () {
        var grid = document.querySelector('.' + gridCss);

        if (grid) {
            var emptyRow = grid.querySelector(".dxbs-empty-data-row");

            if (emptyRow) {
                var td = emptyRow.querySelector("td");

                if (td) {
                    td.innerText = customText;
                }
                else {
                    emptyRow.innerText = customText;
                }
            }
        }
    }, timeout)
}

function onClickGridHeader(anchorClass, customText, timeout) {
    var columns = document.querySelectorAll('.' + anchorClass + ' th .dxbs-fixed-header-content');

    columns.forEach(function (column) {
        column.onclick = function () {
            changeNoDataText(anchorClass, customText, timeout);
            console.log(anchorClass + ', ' + customText + ', ' + timeout);
        }
    });
}

function afterPayTypeComponentRender(anchorClass, customText){
    changeNoDataText(anchorClass, customText, defaultTimeout);
    onClickGridHeader(anchorClass, customText, defaultTimeout);
}

function afterExchangeSettingComponentRender(anchorClass, customText) {
    changeNoDataText(anchorClass, customText, defaultTimeout);
    onClickGridHeader(anchorClass, customText, defaultTimeout);
}

function afterMenuLogComponentRender(anchorClass, customText) {
    changeNoDataText(anchorClass, customText, defaultTimeout);
    onClickGridHeader(anchorClass, customText, defaultTimeout);
}

function afterMenuUserComponentRender(anchorClass, customText) {
    changeNoDataText(anchorClass, customText, defaultTimeout);
    onClickGridHeader(anchorClass, customText, defaultTimeout);
}

function afterUserRender(anchorClass, customText) {
    changeNoDataText(anchorClass, customText, defaultTimeout);
    onClickGridHeader(anchorClass, customText, defaultTimeout);
}

function afterTenantRender(anchorClass, customText) {
    changeNoDataText(anchorClass, customText, defaultTimeout);
    onClickGridHeader(anchorClass, customText, defaultTimeout);
}