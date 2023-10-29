"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var FaceTecSDK_1 = require("../core-sdk/FaceTecSDK.js/FaceTecSDK");
var Config_js_1 = require("../Config.js");
exports.SampleAppUtilities = (function () {
    function displayStatus(message) {
        console.log('i am called here', message);
        document.getElementById("status").innerHTML = message;
    }
    function fadeInMainUI() {
        $("#theme-transition-overlay").fadeOut(800);
        $(".wrapping-box-container").fadeIn(800);
    }
    // Disable buttons to prevent hammering, fade out main interface elements, and shuffle the guidance images.
    function fadeOutMainUIAndPrepareForSession() {
        disableAllButtons();
        $(".wrapping-box-container").fadeOut(800);
        $("#theme-transition-overlay").fadeIn(800);
    }
    function disableAllButtons() {
        $("button").prop("disabled", true);
    }
    function enableAllButtons() {
        $("button").prop("disabled", false);
    }
    function fadeInBlurOverlay() {
        document.getElementById("controls").classList.add("blur-content");
    }
    function fadeOutBlurOverlay() {
        if (document.getElementById("controls").classList.contains("blur-content")) {
            document.getElementById("controls").classList.remove("blur-content");
        }
    }
    function showMainUI() {
        fadeInMainUI();
        enableAllButtons();
    }
    function handleErrorGettingServerSessionToken() {
        fadeInMainUI();
        enableAllButtons();
        displayStatus("Session could not be started due to an unexpected issue during the network request.");
    }
    function generateUUId() {
        // @ts-ignore
        return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, function (c) {
            return (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16);
        });
    }
    function formatUIForDevice() {
        configureGuidanceScreenTextForDevice();
        if (isLikelyMobileDevice()) {
            // Adjust button sizing
            document.querySelectorAll("#controls > button").forEach(function (element) {
                if (element.className === "big-button") {
                    element.style.height = "40px";
                    element.style.fontSize = "18px";
                }
                else if (element.className === "medium-button") {
                    element.style.height = "30px";
                    element.style.fontSize = "14px";
                }
                element.style.width = "220px";
            });
            // Hide border around control panel
            document.getElementById("controls").style.borderColor = "transparent";
            // Hide status label text background and decrease label font size
            document.getElementById("status").style.backgroundColor = "transparent";
            document.getElementById("status").style.fontSize = "12px";
            // Move logo above buttons
            var logoContainer = document.getElementById("custom-logo-container");
            logoContainer.parentNode.insertBefore(logoContainer, logoContainer.parentNode.firstChild);
            document.getElementById("custom-logo-container").style.margin = "0 auto";
            document.querySelector("#custom-logo-container img").style.height = "40px";
            // Center control interface on screen
            document.getElementsByClassName("wrapping-box-container")[0].style.top = "50%";
            document.getElementsByClassName("wrapping-box-container")[0].style.left = "50%";
            document.getElementsByClassName("wrapping-box-container")[0].style.transform = "translate(-50%, -50%)";
        }
    }
    function configureGuidanceScreenTextForDevice() {
        // FaceTecSDK.setCustomization(Config.currentCustomization);
        FaceTecSDK_1.FaceTecSDK.setCustomization(Config_js_1.Config.retrieveConfigurationWizardCustomization(FaceTecSDK_1.FaceTecSDK));
    }
    function isLikelyMobileDevice() {
        var isMobileDeviceUA = !!(/Android|iPhone|iPad|iPod|IEMobile|Mobile|mobile/i.test(navigator.userAgent || ""));
        // ChromeOS/Chromebook detection.
        if (isMobileDeviceUA && ((navigator.userAgent.indexOf("CrOS") !== -1) || (navigator.userAgent.indexOf("Chromebook") !== -1))) {
            isMobileDeviceUA = false;
        }
        // Mobile device determination based on portrait / landscape and user agent.
        if (screen.width < screen.height || isMobileDeviceUA) {
            // Assume mobile device when in portrait mode or when determined by the user agent.
            return true;
        }
        else {
            return false;
        }
    }
    return {
        displayStatus: displayStatus,
        fadeInMainUI: fadeInMainUI,
        fadeOutMainUIAndPrepareForSession: fadeOutMainUIAndPrepareForSession,
        disableAllButtons: disableAllButtons,
        enableAllButtons: enableAllButtons,
        generateUUId: generateUUId,
        formatUIForDevice: formatUIForDevice,
        handleErrorGettingServerSessionToken: handleErrorGettingServerSessionToken,
        configureGuidanceScreenTextForDevice: configureGuidanceScreenTextForDevice,
        showMainUI: showMainUI,
        isLikelyMobileDevice: isLikelyMobileDevice
    };
})();
//# sourceMappingURL=SampleAppUtilities.js.map