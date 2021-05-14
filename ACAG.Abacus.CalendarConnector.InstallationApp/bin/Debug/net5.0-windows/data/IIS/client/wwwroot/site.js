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

    //var currentLangCookie = getLanguageCookie(".AspNetCore.Culture");
    var currentLangLocalStorage = localStorage.getItem("BlazorCulture");

    var browserLang = navigator.language || navigator.userLanguage;
    browserLang = browserLang.substring(0, 2);

    if (supportedCuls.includes(browserLang)) {
        //setLanguageCookie(".AspNetCore.Culture", browserLang, 1);
        localStorage.setItem("BlazorCulture", browserLang);
    }
    else {
        //setLanguageCookie(".AspNetCore.Culture", "en", 1);
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

//function setLanguageCookie(cname, code, exdays) {
//    var d = new Date();
//    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
//    var expires = "expires=" + d.toUTCString();
//    var valueCookie = "c%3D" + code + "%7Cuic%3D" + code;
//    document.cookie = cname + "=" + valueCookie + ";" + expires + ";path=/";
//}

//function getLanguageCookie(cname) {
//    var name = cname + "=";
//    var ca = document.cookie.split(';');
//    for (var i = 0; i < ca.length; i++) {
//        var c = ca[i];
//        while (c.charAt(0) == ' ') {
//            c = c.substring(1);
//        }
//        if (c.indexOf(name) == 0) {
//            return c.substring(name.length, c.length);
//        }
//    }
//    return "";
//}

function changePasswordVisibility(cssClass, showPassword) {
    var passInput = document.querySelector("." + cssClass + " input");
    if (passInput) {
        passInput.type = showPassword ? "text" : "password";
    }
}

function changeNoDataText(gridCss, customText) {
    setTimeout(function () {
        var grid = document.querySelector('.' + gridCss);

        if (grid) {
            var emptyRow = grid.querySelector(".dxbs-empty-data-row");

            if (emptyRow) {
                var td = emptyRow.querySelector("td");

                if (td) {
                    td.innerText = customText;
                }
            }
        }
    }, 50)
}

function disableScreen(isDisable) {
    var modal = document.querySelector('.modal-backdrop.show.dxbs-modal-back');
    var loadingIcon = document.getElementById("LoadingIcon");

    if (modal && loadingIcon) {
        if (isDisable) {
            modal.classList.add("enableScreen");
            modal.classList.remove("disableScreen");
            loadingIcon.classList.remove("hideLoadingIcon");
            loadingIcon.classList.add("showLoadingIcon");
        }
        else {
            modal.classList.add("disableScreen");
            modal.classList.remove("enableScreen");
            loadingIcon.classList.remove("showLoadingIcon");
            loadingIcon.classList.add("hideLoadingIcon");
        }
    }
}

window.addEventListener("load", function () {
    //This will be called when a key is pressed
    var preventDefaultOnEnterCallback = function (e) {
        if (e.keyCode === 13 || e.key === "Enter") {
            // console.log("Prevented default.")
            e.preventDefault();
            return false;
        }
    };
    //This will add key event listener on all nodes with the class preventEnter.
    function setupPreventDefaultOnEnterOnNode(node, add) {
        if (node instanceof HTMLElement) {
            var el = node;
            //Check if main element contains class
            if (el.classList.contains("prevent-default-on-enter") && add) {
                // console.log("Adding preventer: " + el.id);
                el.addEventListener('keydown', preventDefaultOnEnterCallback, false);
            }
            else {
                // console.log("Removing preventer: " + el.id);
                el.removeEventListener('keydown', preventDefaultOnEnterCallback, false);
            }
        }
    }
    //This will add key event listener on all nodes with the class preventEnter.
    function setupPreventDefaultOnEnterOnElements(nodelist, add) {
        for (var i = 0; i < nodelist.length; i++) {
            var node = nodelist[i];
            if (node instanceof HTMLElement) {
                var el = node;
                //Check if main element contains class
                setupPreventDefaultOnEnterOnNode(node, add);
                //Check if any child nodes contains class
                var elements = el.getElementsByClassName("prevent-default-on-enter");
                for (var i_1 = 0; i_1 < elements.length; i_1++) {
                    setupPreventDefaultOnEnterOnNode(elements[i_1], add);
                }
            }
        }
    }
    // Create an observer instance linked to the callback function
    // Read more: https://developer.mozilla.org/en-US/docs/Web/API/MutationObserver
    var preventDefaultOnEnterObserver = new MutationObserver(function (mutations) {
        for (var _i = 0, mutations_1 = mutations; _i < mutations_1.length; _i++) {
            var mutation = mutations_1[_i];
            if (mutation.type === 'childList') {
                // A child node has been added or removed.
                setupPreventDefaultOnEnterOnElements(mutation.addedNodes, true);
            }
            else if (mutation.type === 'attributes') {
                if (mutation.attributeName === "class") {
                    //class was modified on this node. Remove previous event handler (if any).
                    setupPreventDefaultOnEnterOnNode(mutation.target, false);
                    //And add event handler if class i specified.
                    setupPreventDefaultOnEnterOnNode(mutation.target, true);
                }
            }
        }
    });
    // Only observe changes in nodes in the whole tree, but do not observe attributes.
    var preventDefaultOnEnterObserverConfig = { subtree: true, childList: true, attributes: true };
    // Start observing the target node for configured mutations
    preventDefaultOnEnterObserver.observe(document, preventDefaultOnEnterObserverConfig);
    //Also check all elements when loaded.
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
