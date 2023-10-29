import { FaceTecSDK } from "../core-sdk/FaceTecSDK.js/FaceTecSDK";
import { LivenessCheckProcessor } from "../processors/LivenessCheckProcessor";
import { Config } from "../Config.js";

declare var $: any;

export const SampleAppUtilities = (function () {

  function displayStatus(message: string) {
    var button = (document.getElementById("status") as HTMLElement);
    if (button != undefined) {
      button.innerHTML = message;
    }
    else {
    }

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
    //enable the button from Monni app
    //  $("button").prop("disabled", false);
  }

  function fadeInBlurOverlay() {
    (document.getElementById("controls") as HTMLElement).classList.add("blur-content");
  }

  function fadeOutBlurOverlay() {
    if ((document.getElementById("controls") as HTMLElement).classList.contains("blur-content")) {
      (document.getElementById("controls") as HTMLElement).classList.remove("blur-content");
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
    return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
      (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
    );
  }

  function formatUIForDevice() {
    configureGuidanceScreenTextForDevice();
    if (isLikelyMobileDevice()) {
      // Adjust button sizing
      document.querySelectorAll("#controls > button").forEach(function (element) {
        if (element.className === "big-button") {
          (element as HTMLElement).style.height = "40px";
          (element as HTMLElement).style.fontSize = "18px";
        }
        else if (element.className === "medium-button") {
          (element as HTMLElement).style.height = "30px";
          (element as HTMLElement).style.fontSize = "14px";
        }

        (element as HTMLElement).style.width = "220px";
      });
      // Hide border around control panel
      (document.getElementById("controls") as HTMLElement).style.borderColor = "transparent";
      // Hide status label text background and decrease label font size
      (document.getElementById("status") as HTMLElement).style.backgroundColor = "transparent";
      (document.getElementById("status") as HTMLElement).style.fontSize = "12px";
      // Move logo above buttons
      const logoContainer = (document.getElementById("custom-logo-container") as HTMLElement) as HTMLElement;
      (logoContainer.parentNode as HTMLElement).insertBefore(logoContainer, logoContainer.parentNode.firstChild);
      (document.getElementById("custom-logo-container") as HTMLElement).style.margin = "0 auto";
      (document.querySelector("#custom-logo-container img") as HTMLElement).style.height = "40px";
      // Center control interface on screen
      (document.getElementsByClassName("wrapping-box-container")[0] as HTMLElement).style.top = "50%";
      (document.getElementsByClassName("wrapping-box-container")[0] as HTMLElement).style.left = "50%";
      (document.getElementsByClassName("wrapping-box-container")[0] as HTMLElement).style.transform = "translate(-50%, -50%)";
    }
  }

  function configureGuidanceScreenTextForDevice() {
    // FaceTecSDK.setCustomization(Config.currentCustomization);
    FaceTecSDK.setCustomization(Config.retrieveConfigurationWizardCustomization(FaceTecSDK));
  }

  function isLikelyMobileDevice() {
    let isMobileDeviceUA = !!(/Android|iPhone|iPad|iPod|IEMobile|Mobile|mobile/i.test(navigator.userAgent || ""));
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
    displayStatus,
    fadeInMainUI,
    fadeOutMainUIAndPrepareForSession,
    disableAllButtons,
    enableAllButtons,
    generateUUId,
    formatUIForDevice,
    handleErrorGettingServerSessionToken,
    configureGuidanceScreenTextForDevice,
    showMainUI,
    isLikelyMobileDevice
  };
})();
