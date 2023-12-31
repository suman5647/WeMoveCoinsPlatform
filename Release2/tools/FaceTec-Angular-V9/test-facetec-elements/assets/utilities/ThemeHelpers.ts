import { Config } from "../Config";
import { FaceTecSDK} from "../core-sdk/FaceTecSDK.js/FaceTecSDK";
import { FaceTecCustomization } from "../core-sdk/FaceTecSDK.js/FaceTecCustomization";
import { SampleAppUtilities } from "./SampleAppUtilities";


declare var $: any;

export var ThemeHelpers = (function() {
    var currentTheme = "Config Wizard Theme";
    var themeResourceDirectory = "../../../../sample-app-resources/images/themes/";

    function setAppTheme(theme: string) {
        Config.currentCustomization = getCustomizationForTheme(theme);
        var currentLowLightCustomization = getLowLightCustomizationForTheme(theme);

        FaceTecSDK.setCustomization(Config.currentCustomization);
        FaceTecSDK.setLowLightCustomization(currentLowLightCustomization);
    }

    function getCustomizationForTheme(theme: string): FaceTecCustomization {
        var currentCustomization: FaceTecCustomization = new FaceTecSDK.FaceTecCustomization();

        if(SampleAppUtilities.isLikelyMobileDevice()) {
            // using strings with breaks for Ready Screen text on mobile devices
            currentCustomization.guidanceCustomization.readyScreenHeaderAttributedString = "Get Ready For<br/>Your Video Selfie";
            currentCustomization.guidanceCustomization.readyScreenSubtextAttributedString = "Please Frame Your Face In<br/>The Small Oval, Then The Big Oval";
        }

        let retryScreenSlideshowImages: string[] = [themeResourceDirectory + "FaceTec_ideal_1.png", themeResourceDirectory + "FaceTec_ideal_2.png", themeResourceDirectory + "FaceTec_ideal_3.png", themeResourceDirectory + "FaceTec_ideal_4.png", themeResourceDirectory + "FaceTec_ideal_5.png"];

        if(theme === "Config Wizard Theme") {
         // currentCustomization = Config.retrieveConfigurationWizardCustomization(FaceTecSDK);
          // Set the developer element FaceTec loading session token text color
          document.getElementById("loading-session-token-text")!.style.color = "rgb(64, 127, 178)";
        }
        else if(theme === "FaceTec Theme") {
            // using default customizations
            // Set the developer element FaceTec loading session token text color
            document.getElementById("loading-session-token-text")!.style.color = "rgb(64, 127, 178)";
        }
        else if(theme === "Pseudo-Fullscreen") {
            let primaryColor = "rgb(43, 43, 43)"; // black
            let secondaryColor = "rgb(59, 195, 113)"; // green
            let backgroundColor = "rgb(238, 246, 248)"; // white
            let font = "Futura,'Trebuchet MS',Arial,sans-serif";

            var activityIndicatorSVG: SVGElement = document.createElementNS("http://www.w3.org/2000/svg", "svg");
            activityIndicatorSVG.setAttribute("viewBox", "0 0 52 52");
            activityIndicatorSVG.classList.add("pseudo-fullscreen-activity-indicator-svg");
            activityIndicatorSVG.innerHTML = "<circle class='path' cx='26' cy='26' r='22'></circle>";

            var successResultAnimationSVG: SVGElement = document.createElementNS("http://www.w3.org/2000/svg", "svg");
            successResultAnimationSVG.setAttribute("viewBox", "0 0 52 52");
            successResultAnimationSVG.classList.add("pseudo-fullscreen-success-svg");
            successResultAnimationSVG.innerHTML = "<circle class='circlePath' cx='26' cy='26' r='22'></circle><path class='checkmarkPath' d='M14.1 27.2l7.1 7.2 16.7-16.8'></path>";

            var unsuccessResultAnimationSVG: SVGElement = document.createElementNS("http://www.w3.org/2000/svg", "svg");
            unsuccessResultAnimationSVG.setAttribute("viewBox", "0 0 52 52");
            unsuccessResultAnimationSVG.classList.add("pseudo-fullscreen-unsuccess-svg");
            unsuccessResultAnimationSVG.innerHTML = "<circle class='circlePath' cx='26' cy='26' r='22'></circle><line class='crossPath1' x1='18' y1='18' x2='34' y2='34'></line><line class='crossPath2' x1='34' y1='18' x2='18' y2='34'></line>";

            // Initial Loading Animation Customization
            currentCustomization.initialLoadingAnimationCustomization.customAnimation = activityIndicatorSVG;
            currentCustomization.initialLoadingAnimationCustomization.animationRelativeScale = 1.0;
            currentCustomization.initialLoadingAnimationCustomization.backgroundColor = primaryColor;
            currentCustomization.initialLoadingAnimationCustomization.foregroundColor = backgroundColor;
            currentCustomization.initialLoadingAnimationCustomization.messageTextColor = primaryColor;
            currentCustomization.initialLoadingAnimationCustomization.messageFont = font;
            currentCustomization.initialLoadingAnimationCustomization.messageTextSpacing = "normal";
            currentCustomization.initialLoadingAnimationCustomization.messageTextSize = "18px";
            // Overlay Customization
            currentCustomization.overlayCustomization.backgroundColor = backgroundColor;
            currentCustomization.overlayCustomization.showBrandingImage = false;
            currentCustomization.overlayCustomization.brandingImage = "";
            // Guidance Customization
            currentCustomization.guidanceCustomization.backgroundColors = backgroundColor;
            currentCustomization.guidanceCustomization.foregroundColor = primaryColor;
            currentCustomization.guidanceCustomization.headerFont = font;
            currentCustomization.guidanceCustomization.headerTextSpacing = "normal";
            currentCustomization.guidanceCustomization.headerTextSize = "22px";
            currentCustomization.guidanceCustomization.subtextFont = font;
            currentCustomization.guidanceCustomization.subtextTextSpacing = "normal";
            currentCustomization.guidanceCustomization.subtextTextSize = "12px";
            currentCustomization.guidanceCustomization.buttonFont = font;
            currentCustomization.guidanceCustomization.buttonTextSpacing = "normal";
            currentCustomization.guidanceCustomization.buttonTextSize = "16px";
            currentCustomization.guidanceCustomization.buttonTextNormalColor = backgroundColor;
            currentCustomization.guidanceCustomization.buttonBackgroundNormalColor = primaryColor;
            currentCustomization.guidanceCustomization.buttonTextHighlightColor = backgroundColor;
            currentCustomization.guidanceCustomization.buttonBackgroundHighlightColor = "rgb(86, 86, 86)";
            currentCustomization.guidanceCustomization.buttonTextDisabledColor = backgroundColor;
            currentCustomization.guidanceCustomization.buttonBackgroundDisabledColor = primaryColor;
            currentCustomization.guidanceCustomization.buttonBorderColor = "transparent";
            currentCustomization.guidanceCustomization.buttonBorderWidth = "0px";
            currentCustomization.guidanceCustomization.buttonCornerRadius = "20px";
            currentCustomization.guidanceCustomization.buttonRelativeWidth = "1.0";
            currentCustomization.guidanceCustomization.buttonRelativeWidthOnDesktop = "0.4";
            currentCustomization.guidanceCustomization.readyScreenOvalFillColor = "transparent";
            currentCustomization.guidanceCustomization.readyScreenTextBackgroundColor = backgroundColor;
            currentCustomization.guidanceCustomization.readyScreenTextBackgroundCornerRadius = "5px";
            currentCustomization.guidanceCustomization.retryScreenImageBorderColor = primaryColor;
            currentCustomization.guidanceCustomization.retryScreenImageBorderWidth = "2px";
            currentCustomization.guidanceCustomization.retryScreenImageCornerRadius = "10px";
            currentCustomization.guidanceCustomization.retryScreenOvalStrokeColor = backgroundColor;
            currentCustomization.guidanceCustomization.retryScreenSlideshowImages = retryScreenSlideshowImages;
            currentCustomization.guidanceCustomization.retryScreenSlideshowInterval = "2000ms";
            currentCustomization.guidanceCustomization.enableRetryScreenSlideshowShuffle = true;
            currentCustomization.guidanceCustomization.enableRetryScreenBulletedInstructions = true;
            currentCustomization.guidanceCustomization.cameraPermissionsScreenImage = themeResourceDirectory + "pseudo-fullscreen/camera_shutter_black.png";
            // ID Scan Customization
            currentCustomization.idScanCustomization.showSelectionScreenBrandingImage = false;
            currentCustomization.idScanCustomization.selectionScreenBrandingImage = "";
            currentCustomization.idScanCustomization.selectionScreenBackgroundColors = backgroundColor;
            currentCustomization.idScanCustomization.reviewScreenBackgroundColors = backgroundColor;
            currentCustomization.idScanCustomization.captureScreenForegroundColor = primaryColor;
            currentCustomization.idScanCustomization.reviewScreenForegroundColor = primaryColor;
            currentCustomization.idScanCustomization.selectionScreenForegroundColor = primaryColor;
            currentCustomization.idScanCustomization.headerFont = font;
            currentCustomization.idScanCustomization.headerTextSpacing = "normal";
            currentCustomization.idScanCustomization.headerTextSize = "22px";
            currentCustomization.idScanCustomization.subtextFont = font;
            currentCustomization.idScanCustomization.subtextTextSpacing = "normal";
            currentCustomization.idScanCustomization.subtextTextSize = "12px";
            currentCustomization.idScanCustomization.buttonFont = font;
            currentCustomization.idScanCustomization.buttonTextSpacing = "normal";
            currentCustomization.idScanCustomization.buttonTextSize = "16px";
            currentCustomization.idScanCustomization.buttonTextNormalColor = backgroundColor;
            currentCustomization.idScanCustomization.buttonBackgroundNormalColor = primaryColor;
            currentCustomization.idScanCustomization.buttonTextHighlightColor = backgroundColor;
            currentCustomization.idScanCustomization.buttonBackgroundHighlightColor = "rgb(86, 86, 86)";
            currentCustomization.idScanCustomization.buttonTextDisabledColor = backgroundColor;
            currentCustomization.idScanCustomization.buttonBackgroundDisabledColor = primaryColor;
            currentCustomization.idScanCustomization.buttonBorderColor = "transparent";
            currentCustomization.idScanCustomization.buttonBorderWidth = "0px";
            currentCustomization.idScanCustomization.buttonCornerRadius = "20px";
            currentCustomization.idScanCustomization.buttonRelativeWidth = "1.0";
            currentCustomization.idScanCustomization.buttonRelativeWidthOnDesktop = "0.4";
            currentCustomization.idScanCustomization.captureScreenTextBackgroundColor = backgroundColor;
            currentCustomization.idScanCustomization.captureScreenTextBackgroundBorderColor = primaryColor;
            currentCustomization.idScanCustomization.captureScreenTextBackgroundBorderWidth = "2px";
            currentCustomization.idScanCustomization.captureScreenTextBackgroundCornerRadius = "5px";
            currentCustomization.idScanCustomization.reviewScreenTextBackgroundColor = backgroundColor;
            currentCustomization.idScanCustomization.reviewScreenTextBackgroundBorderColor = primaryColor;
            currentCustomization.idScanCustomization.reviewScreenTextBackgroundBorderWidth = "2px";
            currentCustomization.idScanCustomization.reviewScreenTextBackgroundBorderCornerRadius = "5px";
            currentCustomization.idScanCustomization.captureScreenBackgroundColor = backgroundColor;
            currentCustomization.idScanCustomization.captureFrameStrokeColor = primaryColor;
            currentCustomization.idScanCustomization.captureFrameStrokeWidth = "2px";
            currentCustomization.idScanCustomization.captureFrameCornerRadius = "12px";
            // Result Screen Customization
            currentCustomization.resultScreenCustomization.backgroundColors = backgroundColor;
            currentCustomization.resultScreenCustomization.foregroundColor = primaryColor;
            currentCustomization.resultScreenCustomization.messageFont = font;
            currentCustomization.resultScreenCustomization.messageTextSpacing = "normal";
            currentCustomization.resultScreenCustomization.messageTextSize = "18px";
            currentCustomization.resultScreenCustomization.activityIndicatorColor = primaryColor;
            currentCustomization.resultScreenCustomization.customActivityIndicatorImage = themeResourceDirectory + "pseudo-fullscreen/activity_indicator_faded_black.png";
            currentCustomization.resultScreenCustomization.customActivityIndicatorRotationInterval = "0.8s";
            currentCustomization.resultScreenCustomization.customActivityIndicatorAnimation = activityIndicatorSVG;
            currentCustomization.resultScreenCustomization.resultAnimationBackgroundColor = secondaryColor;
            currentCustomization.resultScreenCustomization.resultAnimationForegroundColor = backgroundColor;
            currentCustomization.resultScreenCustomization.resultAnimationSuccessBackgroundImage = "";
            currentCustomization.resultScreenCustomization.resultAnimationUnsuccessBackgroundImage = "";
            currentCustomization.resultScreenCustomization.customResultAnimationSuccess = successResultAnimationSVG;
            currentCustomization.resultScreenCustomization.customResultAnimationUnsuccess = unsuccessResultAnimationSVG;
            currentCustomization.resultScreenCustomization.showUploadProgressBar = true;
            currentCustomization.resultScreenCustomization.uploadProgressTrackColor = "rgba(0, 0, 0, 0.2)";
            currentCustomization.resultScreenCustomization.uploadProgressFillColor = secondaryColor;
            currentCustomization.resultScreenCustomization.animationRelativeScale = 1.0;
            // Feedback Customization
            currentCustomization.feedbackCustomization.backgroundColor = secondaryColor;
            currentCustomization.feedbackCustomization.textColor = backgroundColor;
            currentCustomization.feedbackCustomization.textFont = font;
            currentCustomization.feedbackCustomization.textSpacing = "normal";
            currentCustomization.feedbackCustomization.textSize = "18px";
            currentCustomization.feedbackCustomization.cornerRadius = "5px";
            currentCustomization.feedbackCustomization.shadow = "0px 3px 10px black";
            currentCustomization.feedbackCustomization.relativeWidth = "1.0";
            currentCustomization.feedbackCustomization.relativeWidthOnDesktop = "0.5";
            // Frame Customization
            currentCustomization.frameCustomization.backgroundColor = backgroundColor;
            currentCustomization.frameCustomization.borderColor = primaryColor;
            currentCustomization.frameCustomization.borderWidth = "0px";
            currentCustomization.frameCustomization.borderCornerRadius = "0px";
            currentCustomization.frameCustomization.shadow = "none";
            // Oval Customization
            currentCustomization.ovalCustomization.strokeColor = primaryColor;
            currentCustomization.ovalCustomization.progressColor1 = "rgba(59, 195, 113, 0.7)";
            currentCustomization.ovalCustomization.progressColor2 = "rgba(59, 195, 113, 0.7)";
            // Cancel Button Customization
            currentCustomization.cancelButtonCustomization.customImage = themeResourceDirectory + "pseudo-fullscreen/single_chevron_left_black.png";
            currentCustomization.cancelButtonCustomization.location = FaceTecSDK.FaceTecCancelButtonLocation.Custom;
            currentCustomization.cancelButtonCustomization.setCustomLocation(20, 20, 20, 20);

            // Guidance Customization -- Text Style Overrides
            // Ready Screen Header
            currentCustomization.guidanceCustomization.readyScreenHeaderFont = font;
            currentCustomization.guidanceCustomization.readyScreenHeaderTextSpacing = "normal";
            currentCustomization.guidanceCustomization.readyScreenHeaderTextSize = "22px";
            currentCustomization.guidanceCustomization.readyScreenHeaderTextColor = primaryColor;
            // Ready Screen Subtext
            currentCustomization.guidanceCustomization.readyScreenSubtextFont = font;
            currentCustomization.guidanceCustomization.readyScreenSubtextTextSpacing = "normal";
            currentCustomization.guidanceCustomization.readyScreenSubtextTextSize = "10px";
            currentCustomization.guidanceCustomization.readyScreenSubtextTextColor = "#565656";
            // Retry Screen Header
            currentCustomization.guidanceCustomization.retryScreenHeaderFont = font;
            currentCustomization.guidanceCustomization.retryScreenHeaderTextSpacing = "normal";
            currentCustomization.guidanceCustomization.retryScreenHeaderTextSize = "22px";
            currentCustomization.guidanceCustomization.retryScreenHeaderTextColor = primaryColor;
            // Retry Screen Subtext
            currentCustomization.guidanceCustomization.retryScreenSubtextFont = font;
            currentCustomization.guidanceCustomization.retryScreenSubtextTextSpacing = "normal";
            currentCustomization.guidanceCustomization.retryScreenSubtextTextSize = "12px";
            currentCustomization.guidanceCustomization.retryScreenSubtextTextColor = "#565656";
            // Security Watermark Customization
            currentCustomization.securityWatermarkCustomization.setSecurityWatermarkImage(FaceTecSDK.FaceTecSecurityWatermarkImage.FaceTec);
        }
        else if(theme === "Well-Rounded") {
            let primaryColor = "rgb(9, 181, 163)"; // green
            let backgroundColor = "white";
            let font = "'Source Sans Pro', Helvetica, sans-serif";

            var successResultAnimationSVG: SVGElement = document.createElementNS("http://www.w3.org/2000/svg", "svg");
            successResultAnimationSVG.setAttribute("viewBox", "0 0 52 52");
            successResultAnimationSVG.classList.add("well-rounded-success-svg");
            successResultAnimationSVG.innerHTML = "<circle class='circlePath' cx='26' cy='26' r='19'></circle><path class='checkmarkPath' d='M16.1 27.7l5.1 6.2 13.7-13.3'></path>";

            var unsuccessResultAnimationSVG: SVGElement = document.createElementNS("http://www.w3.org/2000/svg", "svg");
            unsuccessResultAnimationSVG.setAttribute("viewBox", "0 0 52 52");
            unsuccessResultAnimationSVG.classList.add("well-rounded-unsuccess-svg");
            unsuccessResultAnimationSVG.innerHTML = "<circle class='circlePath' cx='26' cy='26' r='19'></circle><line class='crossPath1' x1='18' y1='18' x2='34' y2='34'></line><line class='crossPath2' x1='34' y1='18' x2='18' y2='34'></line>";

            var activityIndicatorSVG: SVGElement = document.createElementNS("http://www.w3.org/2000/svg", "svg");
            activityIndicatorSVG.setAttribute("viewBox", "0 0 52 52");
            activityIndicatorSVG.classList.add("well-rounded-activity-indicator-svg");
            activityIndicatorSVG.innerHTML = "<circle class='circleTrackPath' cx='26' cy='26' r='19'/><circle class='circleFillPath' cx='7' cy='26' r='3'><animateTransform attributeName='transform' dur='2s' type='rotate' from='0 26 26' to='360 26 26' repeatCount='indefinite'/></circle>";

            // Initial Loading Animation Customization
            currentCustomization.initialLoadingAnimationCustomization.customAnimation = activityIndicatorSVG;
            currentCustomization.initialLoadingAnimationCustomization.animationRelativeScale = 2.0;
            currentCustomization.initialLoadingAnimationCustomization.backgroundColor = "transparent";
            currentCustomization.initialLoadingAnimationCustomization.foregroundColor = backgroundColor;
            currentCustomization.initialLoadingAnimationCustomization.messageTextColor = backgroundColor;
            currentCustomization.initialLoadingAnimationCustomization.messageFont = font;
            currentCustomization.initialLoadingAnimationCustomization.messageTextSpacing = "normal";
            currentCustomization.initialLoadingAnimationCustomization.messageTextSize = "16px";
            // Overlay Customization
            currentCustomization.overlayCustomization.backgroundColor = "transparent";
            currentCustomization.overlayCustomization.showBrandingImage = false;
            currentCustomization.overlayCustomization.brandingImage = "";
            // Guidance Customization
            currentCustomization.guidanceCustomization.backgroundColors = backgroundColor;
            currentCustomization.guidanceCustomization.foregroundColor = primaryColor;
            currentCustomization.guidanceCustomization.headerFont = font;
            currentCustomization.guidanceCustomization.headerTextSpacing = "normal";
            currentCustomization.guidanceCustomization.headerTextSize = "20px";
            currentCustomization.guidanceCustomization.subtextFont = font;
            currentCustomization.guidanceCustomization.subtextTextSpacing = "normal";
            currentCustomization.guidanceCustomization.subtextTextSize = "12px";
            currentCustomization.guidanceCustomization.buttonFont = font;
            currentCustomization.guidanceCustomization.buttonTextSpacing = "normal";
            currentCustomization.guidanceCustomization.buttonTextSize = "16px";
            currentCustomization.guidanceCustomization.buttonTextNormalColor = backgroundColor;
            currentCustomization.guidanceCustomization.buttonBackgroundNormalColor = primaryColor;
            currentCustomization.guidanceCustomization.buttonTextHighlightColor = backgroundColor;
            currentCustomization.guidanceCustomization.buttonBackgroundHighlightColor = "rgb(49, 221, 203)";
            currentCustomization.guidanceCustomization.buttonTextDisabledColor = "rgb(215, 215, 215)";
            currentCustomization.guidanceCustomization.buttonBackgroundDisabledColor = "rgb(0, 141, 123)";
            currentCustomization.guidanceCustomization.buttonBorderColor = "transparent";
            currentCustomization.guidanceCustomization.buttonBorderWidth = "0px";
            currentCustomization.guidanceCustomization.buttonCornerRadius = "20px";
            currentCustomization.guidanceCustomization.buttonRelativeWidth = "0.75";
            currentCustomization.guidanceCustomization.buttonRelativeWidthOnDesktop = "0.33";
            currentCustomization.guidanceCustomization.readyScreenOvalFillColor = "rgba(9, 181, 163, 0.2)";
            currentCustomization.guidanceCustomization.readyScreenTextBackgroundColor = backgroundColor;
            currentCustomization.guidanceCustomization.readyScreenTextBackgroundCornerRadius = "5px";
            currentCustomization.guidanceCustomization.retryScreenImageBorderColor = primaryColor;
            currentCustomization.guidanceCustomization.retryScreenImageBorderWidth = "2px";
            currentCustomization.guidanceCustomization.retryScreenImageCornerRadius = "10px";
            currentCustomization.guidanceCustomization.retryScreenOvalStrokeColor = backgroundColor;
            currentCustomization.guidanceCustomization.retryScreenSlideshowImages = [];
            currentCustomization.guidanceCustomization.retryScreenSlideshowInterval = "1500ms";
            currentCustomization.guidanceCustomization.enableRetryScreenSlideshowShuffle = true;
            currentCustomization.guidanceCustomization.enableRetryScreenBulletedInstructions = true;
            currentCustomization.guidanceCustomization.cameraPermissionsScreenImage = themeResourceDirectory + "well-rounded/camera_green.png";
            // ID Scan Customization
            currentCustomization.idScanCustomization.showSelectionScreenBrandingImage = false;
            currentCustomization.idScanCustomization.selectionScreenBrandingImage = "";
            currentCustomization.idScanCustomization.selectionScreenBackgroundColors = backgroundColor;
            currentCustomization.idScanCustomization.reviewScreenBackgroundColors = backgroundColor;
            currentCustomization.idScanCustomization.captureScreenForegroundColor = primaryColor;
            currentCustomization.idScanCustomization.reviewScreenForegroundColor = primaryColor;
            currentCustomization.idScanCustomization.selectionScreenForegroundColor = primaryColor;
            currentCustomization.idScanCustomization.headerFont = font;
            currentCustomization.idScanCustomization.headerTextSpacing = "normal";
            currentCustomization.idScanCustomization.headerTextSize = "20px";
            currentCustomization.idScanCustomization.subtextFont = font;
            currentCustomization.idScanCustomization.subtextTextSpacing = "normal";
            currentCustomization.idScanCustomization.subtextTextSize = "12px";
            currentCustomization.idScanCustomization.buttonFont = font;
            currentCustomization.idScanCustomization.buttonTextSpacing = "normal";
            currentCustomization.idScanCustomization.buttonTextSize = "16px";
            currentCustomization.idScanCustomization.buttonTextNormalColor = backgroundColor;
            currentCustomization.idScanCustomization.buttonBackgroundNormalColor = primaryColor;
            currentCustomization.idScanCustomization.buttonTextHighlightColor = backgroundColor;
            currentCustomization.idScanCustomization.buttonBackgroundHighlightColor = "rgb(49, 221, 203)";
            currentCustomization.idScanCustomization.buttonTextDisabledColor = "rgb(215, 215, 215)";
            currentCustomization.idScanCustomization.buttonBackgroundDisabledColor = "rgb(0, 141, 123)";
            currentCustomization.idScanCustomization.buttonBorderColor = "transparent";
            currentCustomization.idScanCustomization.buttonBorderWidth = "0px";
            currentCustomization.idScanCustomization.buttonCornerRadius = "20px";
            currentCustomization.idScanCustomization.buttonRelativeWidth = "0.75";
            currentCustomization.idScanCustomization.buttonRelativeWidthOnDesktop = "0.33";
            currentCustomization.idScanCustomization.captureScreenTextBackgroundColor = backgroundColor;
            currentCustomization.idScanCustomization.captureScreenTextBackgroundBorderColor = primaryColor;
            currentCustomization.idScanCustomization.captureScreenTextBackgroundBorderWidth = "2px";
            currentCustomization.idScanCustomization.captureScreenTextBackgroundCornerRadius = "5px";
            currentCustomization.idScanCustomization.reviewScreenTextBackgroundColor = backgroundColor;
            currentCustomization.idScanCustomization.reviewScreenTextBackgroundBorderColor = primaryColor;
            currentCustomization.idScanCustomization.reviewScreenTextBackgroundBorderWidth = "2px";
            currentCustomization.idScanCustomization.reviewScreenTextBackgroundBorderCornerRadius = "5px";
            currentCustomization.idScanCustomization.captureScreenBackgroundColor = backgroundColor;
            currentCustomization.idScanCustomization.captureFrameStrokeColor = primaryColor;
            currentCustomization.idScanCustomization.captureFrameStrokeWidth = "2px";
            currentCustomization.idScanCustomization.captureFrameCornerRadius = "12px";
            // Result Screen Customization
            currentCustomization.resultScreenCustomization.backgroundColors = backgroundColor;
            currentCustomization.resultScreenCustomization.foregroundColor = primaryColor;
            currentCustomization.resultScreenCustomization.messageFont = font;
            currentCustomization.resultScreenCustomization.messageTextSpacing = "normal";
            currentCustomization.resultScreenCustomization.messageTextSize = "16px";
            currentCustomization.resultScreenCustomization.activityIndicatorColor = primaryColor;
            currentCustomization.resultScreenCustomization.customActivityIndicatorImage = "";
            currentCustomization.resultScreenCustomization.customActivityIndicatorRotationInterval = "1s";
            currentCustomization.resultScreenCustomization.customActivityIndicatorAnimation = activityIndicatorSVG;
            currentCustomization.resultScreenCustomization.resultAnimationBackgroundColor = "transparent";
            currentCustomization.resultScreenCustomization.resultAnimationForegroundColor = backgroundColor;
            currentCustomization.resultScreenCustomization.resultAnimationSuccessBackgroundImage = "";
            currentCustomization.resultScreenCustomization.resultAnimationUnsuccessBackgroundImage = "";
            currentCustomization.resultScreenCustomization.customResultAnimationSuccess = successResultAnimationSVG;
            currentCustomization.resultScreenCustomization.customResultAnimationUnsuccess = unsuccessResultAnimationSVG;
            currentCustomization.resultScreenCustomization.showUploadProgressBar = false;
            currentCustomization.resultScreenCustomization.uploadProgressTrackColor = "rgba(0, 0, 0, 0.2)";
            currentCustomization.resultScreenCustomization.uploadProgressFillColor = primaryColor;
            currentCustomization.resultScreenCustomization.animationRelativeScale = 2.0;
            // Feedback Customization
            currentCustomization.feedbackCustomization.backgroundColor = primaryColor;
            currentCustomization.feedbackCustomization.textColor = backgroundColor;
            currentCustomization.feedbackCustomization.textFont = font;
            currentCustomization.feedbackCustomization.textSpacing = "normal";
            currentCustomization.feedbackCustomization.textSize = "18px";
            currentCustomization.feedbackCustomization.cornerRadius = "5px";
            currentCustomization.feedbackCustomization.shadow = "0px 2px 8px 2px rgb(0, 0, 0, 0.4)";
            currentCustomization.feedbackCustomization.relativeWidth = "0.75";
            currentCustomization.feedbackCustomization.relativeWidthOnDesktop = "0.33";
            // Frame Customization
            currentCustomization.frameCustomization.backgroundColor = backgroundColor;
            currentCustomization.frameCustomization.borderColor = primaryColor;
            currentCustomization.frameCustomization.borderWidth = "2px";
            currentCustomization.frameCustomization.borderCornerRadius = "20px";
            currentCustomization.frameCustomization.shadow = "0px 2px 8px 2px rgb(0, 0, 0, 0.4)";
            // Oval Customization
            currentCustomization.ovalCustomization.strokeColor = primaryColor;
            currentCustomization.ovalCustomization.progressColor1 = primaryColor;
            currentCustomization.ovalCustomization.progressColor2 = primaryColor;
            // Cancel Button Customization
            currentCustomization.cancelButtonCustomization.customImage = themeResourceDirectory + "well-rounded/cancel_round_green.png";
            currentCustomization.cancelButtonCustomization.location = FaceTecSDK.FaceTecCancelButtonLocation.TopLeft;
        }
        else if(theme === "Bitcoin Exchange") {
            let primaryColor = "rgb(247, 150, 52)"; // orange
            let secondaryColor = "rgb(255, 255, 30)"; // yellow
            let backgroundColor = "rgb(66, 66, 66)"; // dark grey
            let font = "'Source Sans Pro', Helvetica, sans-serif";

            // Initial Loading Animation Customization
            currentCustomization.initialLoadingAnimationCustomization.customAnimation = null;
            currentCustomization.initialLoadingAnimationCustomization.animationRelativeScale = 1.0;
            currentCustomization.initialLoadingAnimationCustomization.backgroundColor = backgroundColor;
            currentCustomization.initialLoadingAnimationCustomization.foregroundColor = primaryColor;
            currentCustomization.initialLoadingAnimationCustomization.messageTextColor = backgroundColor;
            currentCustomization.initialLoadingAnimationCustomization.messageFont = font;
            currentCustomization.initialLoadingAnimationCustomization.messageTextSpacing = "normal";
            currentCustomization.initialLoadingAnimationCustomization.messageTextSize = "16px";
            // Overlay Customization
            currentCustomization.overlayCustomization.backgroundColor = "transparent";
            currentCustomization.overlayCustomization.showBrandingImage = true;
            currentCustomization.overlayCustomization.brandingImage = themeResourceDirectory + "bitcoin-exchange/bitcoin_exchange_logo.png";
            // Guidance Customization
            currentCustomization.guidanceCustomization.backgroundColors = backgroundColor;
            currentCustomization.guidanceCustomization.foregroundColor = primaryColor;
            currentCustomization.guidanceCustomization.headerFont = font;
            currentCustomization.guidanceCustomization.headerTextSpacing = "normal";
            currentCustomization.guidanceCustomization.headerTextSize = "18px";
            currentCustomization.guidanceCustomization.subtextFont = font;
            currentCustomization.guidanceCustomization.subtextTextSpacing = "normal";
            currentCustomization.guidanceCustomization.subtextTextSize = "10px";
            currentCustomization.guidanceCustomization.buttonFont = font;
            currentCustomization.guidanceCustomization.buttonTextSpacing = "normal";
            currentCustomization.guidanceCustomization.buttonTextSize = "16px";
            currentCustomization.guidanceCustomization.buttonTextNormalColor = backgroundColor;
            currentCustomization.guidanceCustomization.buttonBackgroundNormalColor = primaryColor;
            currentCustomization.guidanceCustomization.buttonTextHighlightColor = backgroundColor;
            currentCustomization.guidanceCustomization.buttonBackgroundHighlightColor = primaryColor;
            currentCustomization.guidanceCustomization.buttonTextDisabledColor = backgroundColor;
            currentCustomization.guidanceCustomization.buttonBackgroundDisabledColor = primaryColor;
            currentCustomization.guidanceCustomization.buttonBorderColor = "transparent";
            currentCustomization.guidanceCustomization.buttonBorderWidth = "0px";
            currentCustomization.guidanceCustomization.buttonCornerRadius = "5px";
            currentCustomization.guidanceCustomization.buttonRelativeWidth = "1.0";
            currentCustomization.guidanceCustomization.buttonRelativeWidthOnDesktop = "0.5";
            currentCustomization.guidanceCustomization.readyScreenOvalFillColor = "rgba(247, 150, 52, 0.2)";
            currentCustomization.guidanceCustomization.readyScreenTextBackgroundColor = backgroundColor;
            currentCustomization.guidanceCustomization.readyScreenTextBackgroundCornerRadius = "5px";
            currentCustomization.guidanceCustomization.retryScreenImageBorderColor = primaryColor;
            currentCustomization.guidanceCustomization.retryScreenImageBorderWidth = "2px";
            currentCustomization.guidanceCustomization.retryScreenImageCornerRadius = "5px";
            currentCustomization.guidanceCustomization.retryScreenOvalStrokeColor = primaryColor;
            currentCustomization.guidanceCustomization.retryScreenSlideshowImages = [];
            currentCustomization.guidanceCustomization.retryScreenSlideshowInterval = "1500ms";
            currentCustomization.guidanceCustomization.enableRetryScreenSlideshowShuffle = true;
            currentCustomization.guidanceCustomization.enableRetryScreenBulletedInstructions = true;
            currentCustomization.guidanceCustomization.cameraPermissionsScreenImage = themeResourceDirectory + "bitcoin-exchange/camera_orange.png";
            // ID Scan Customization
            currentCustomization.idScanCustomization.showSelectionScreenBrandingImage = false;
            currentCustomization.idScanCustomization.selectionScreenBrandingImage = "";
            currentCustomization.idScanCustomization.selectionScreenBackgroundColors = backgroundColor;
            currentCustomization.idScanCustomization.reviewScreenBackgroundColors = backgroundColor;
            currentCustomization.idScanCustomization.captureScreenForegroundColor = primaryColor;
            currentCustomization.idScanCustomization.reviewScreenForegroundColor = primaryColor;
            currentCustomization.idScanCustomization.selectionScreenForegroundColor = primaryColor;
            currentCustomization.idScanCustomization.headerFont = font;
            currentCustomization.idScanCustomization.headerTextSpacing = "normal";
            currentCustomization.idScanCustomization.headerTextSize = "18px";
            currentCustomization.idScanCustomization.subtextFont = font;
            currentCustomization.idScanCustomization.subtextTextSpacing = "normal";
            currentCustomization.idScanCustomization.subtextTextSize = "10px";
            currentCustomization.idScanCustomization.buttonFont = font;
            currentCustomization.idScanCustomization.buttonTextSpacing = "normal";
            currentCustomization.idScanCustomization.buttonTextSize = "16px";
            currentCustomization.idScanCustomization.buttonTextNormalColor = backgroundColor;
            currentCustomization.idScanCustomization.buttonBackgroundNormalColor = primaryColor;
            currentCustomization.idScanCustomization.buttonTextHighlightColor = backgroundColor;
            currentCustomization.idScanCustomization.buttonBackgroundHighlightColor = primaryColor;
            currentCustomization.idScanCustomization.buttonTextDisabledColor = backgroundColor;
            currentCustomization.idScanCustomization.buttonBackgroundDisabledColor = primaryColor;
            currentCustomization.idScanCustomization.buttonBorderColor = "transparent";
            currentCustomization.idScanCustomization.buttonBorderWidth = "0px";
            currentCustomization.idScanCustomization.buttonCornerRadius = "5px";
            currentCustomization.idScanCustomization.buttonRelativeWidth = "1.0";
            currentCustomization.idScanCustomization.buttonRelativeWidthOnDesktop = "0.5";
            currentCustomization.idScanCustomization.captureScreenTextBackgroundColor = backgroundColor;
            currentCustomization.idScanCustomization.captureScreenTextBackgroundBorderColor = primaryColor;
            currentCustomization.idScanCustomization.captureScreenTextBackgroundBorderWidth = "0px";
            currentCustomization.idScanCustomization.captureScreenTextBackgroundCornerRadius = "8px";
            currentCustomization.idScanCustomization.reviewScreenTextBackgroundColor = backgroundColor;
            currentCustomization.idScanCustomization.reviewScreenTextBackgroundBorderColor = primaryColor;
            currentCustomization.idScanCustomization.reviewScreenTextBackgroundBorderWidth = "0px";
            currentCustomization.idScanCustomization.reviewScreenTextBackgroundBorderCornerRadius = "8px";
            currentCustomization.idScanCustomization.captureScreenBackgroundColor = backgroundColor;
            currentCustomization.idScanCustomization.captureFrameStrokeColor = primaryColor;
            currentCustomization.idScanCustomization.captureFrameStrokeWidth = "2px";
            currentCustomization.idScanCustomization.captureFrameCornerRadius = "12px";
            // Result Screen Customization
            currentCustomization.resultScreenCustomization.backgroundColors = backgroundColor;
            currentCustomization.resultScreenCustomization.foregroundColor = primaryColor;
            currentCustomization.resultScreenCustomization.messageFont = font;
            currentCustomization.resultScreenCustomization.messageTextSpacing = "normal";
            currentCustomization.resultScreenCustomization.messageTextSize = "16px";
            currentCustomization.resultScreenCustomization.activityIndicatorColor = primaryColor;
            currentCustomization.resultScreenCustomization.customActivityIndicatorImage = themeResourceDirectory + "bitcoin-exchange/activity_indicator_orange.png";
            currentCustomization.resultScreenCustomization.customActivityIndicatorRotationInterval = "1.5s";
            currentCustomization.resultScreenCustomization.customActivityIndicatorAnimation = null;
            currentCustomization.resultScreenCustomization.resultAnimationBackgroundColor = primaryColor;
            currentCustomization.resultScreenCustomization.resultAnimationForegroundColor = backgroundColor;
            currentCustomization.resultScreenCustomization.resultAnimationSuccessBackgroundImage = "";
            currentCustomization.resultScreenCustomization.resultAnimationUnsuccessBackgroundImage = "";
            currentCustomization.resultScreenCustomization.customResultAnimationSuccess = null;
            currentCustomization.resultScreenCustomization.customResultAnimationUnsuccess = null;
            currentCustomization.resultScreenCustomization.showUploadProgressBar = true;
            currentCustomization.resultScreenCustomization.uploadProgressTrackColor = "rgba(0, 0, 0, 0.2)";
            currentCustomization.resultScreenCustomization.uploadProgressFillColor = primaryColor;
            currentCustomization.resultScreenCustomization.animationRelativeScale = 1.0;
            // Feedback Customization
            currentCustomization.feedbackCustomization.backgroundColor = primaryColor;
            currentCustomization.feedbackCustomization.textColor = backgroundColor;
            currentCustomization.feedbackCustomization.textFont = font;
            currentCustomization.feedbackCustomization.textSpacing = "normal";
            currentCustomization.feedbackCustomization.textSize = "16px";
            currentCustomization.feedbackCustomization.cornerRadius = "5px";
            currentCustomization.feedbackCustomization.shadow = "0px 3px 10px rgba(66, 66, 66, 0.6)";
            currentCustomization.feedbackCustomization.relativeWidth = "1.0";
            currentCustomization.feedbackCustomization.relativeWidthOnDesktop = "0.5";
            // Frame Customization
            currentCustomization.frameCustomization.backgroundColor = backgroundColor;
            currentCustomization.frameCustomization.borderColor = secondaryColor;
            currentCustomization.frameCustomization.borderWidth = "0px";
            currentCustomization.frameCustomization.borderCornerRadius = "5px";
            currentCustomization.frameCustomization.shadow = "0px 3px 10px rgba(66, 66, 66, 0.6)";
            // Oval Customization
            currentCustomization.ovalCustomization.strokeColor = primaryColor;
            currentCustomization.ovalCustomization.progressColor1 = secondaryColor;
            currentCustomization.ovalCustomization.progressColor2 = secondaryColor;
            // Cancel Button Customization
            currentCustomization.cancelButtonCustomization.customImage = themeResourceDirectory + "bitcoin-exchange/single_chevron_left_orange.png";
            currentCustomization.cancelButtonCustomization.location = FaceTecSDK.FaceTecCancelButtonLocation.TopLeft;

            // Guidance Customization -- Text Style Overrides
            // Ready Screen Header
            currentCustomization.guidanceCustomization.readyScreenHeaderFont = font;
            currentCustomization.guidanceCustomization.readyScreenHeaderTextSpacing = "normal";
            currentCustomization.guidanceCustomization.readyScreenHeaderTextSize = "18px";
            currentCustomization.guidanceCustomization.readyScreenHeaderTextColor = primaryColor;
            // Ready Screen Subtext
            currentCustomization.guidanceCustomization.readyScreenSubtextFont = font;
            currentCustomization.guidanceCustomization.readyScreenSubtextTextSpacing = "normal";
            currentCustomization.guidanceCustomization.readyScreenSubtextTextSize = "10px";
            currentCustomization.guidanceCustomization.readyScreenSubtextTextColor = secondaryColor;
            // Retry Screen Header
            currentCustomization.guidanceCustomization.retryScreenHeaderFont = font;
            currentCustomization.guidanceCustomization.retryScreenHeaderTextSpacing = "normal";
            currentCustomization.guidanceCustomization.retryScreenHeaderTextSize = "18px";
            currentCustomization.guidanceCustomization.retryScreenHeaderTextColor = primaryColor;
            // Retry Screen Subtext
            currentCustomization.guidanceCustomization.retryScreenSubtextFont = font;
            currentCustomization.guidanceCustomization.retryScreenSubtextTextSpacing = "normal";
            currentCustomization.guidanceCustomization.retryScreenSubtextTextSize = "10px";
            currentCustomization.guidanceCustomization.retryScreenSubtextTextColor = secondaryColor;
        }
        else if(theme === "eKYC") {
            let primaryColor = "rgb(237, 28, 36)"; // red
            let secondaryColor = "black";
            let backgroundColor = "white";
            let font = "'Source Sans Pro', Helvetica, sans-serif";

            var activityIndicatorSVG: SVGElement = document.createElementNS("http://www.w3.org/2000/svg", "svg");
            activityIndicatorSVG.setAttribute("viewBox", "0 0 52 52");
            activityIndicatorSVG.classList.add("ekyc-activity-indicator-svg");
            activityIndicatorSVG.innerHTML = "<defs><filter id='goo'><feGaussianBlur in='SourceGraphic' stdDeviation='2' result='blur' /><feColorMatrix in='blur' mode='matrix' values='1 0 0 0 0  0 1 0 0 0  0 0 1 0 0  0 0 0 19 -9' result='goo' /><feComposite in='SourceGraphic' in2='goo' operator='atop'/></filter></defs><g filter='url(#goo)'><g transform='translate(26 26)'><circle class='circlePath1' cx='7' cy='26' r='3'><animateTransform attributeName='transform' dur='1s' type='translate' values='0,0; -6,-26; 0,0' repeatCount='indefinite' begin='0s'/><animateTransform attributeName='transform' attributeType='XML' dur='1s' type='scale' values='1;2;1' repeatCount='indefinite' additive='sum' begin='0s'/></circle><animateTransform attributeName='transform' dur='1s' type='rotate' from='0 26 26' to='360 26 26' repeatCount='indefinite' begin='0s'/></g>  <g transform='translate(26 26)'><circle class='circlePath2' cx='7' cy='26' r='3'><animateTransform attributeName='transform' dur='1.2s' type='translate' values='0,0; -6,-26; 0,0' repeatCount='indefinite' begin='0s'/><animateTransform attributeName='transform' attributeType='XML' dur='1.2s' type='scale' values='1;2;1' repeatCount='indefinite' additive='sum' begin='0s'/></circle><animateTransform attributeName='transform' dur='1.2s' type='rotate' from='0 26 26' to='360 26 26' repeatCount='indefinite' begin='0s'/></g>  <g transform='translate(26 26)'><circle class='circlePath3' cx='7' cy='26' r='3'><animateTransform attributeName='transform' dur='1.5s' type='translate' values='0,0; -6,-26; 0,0' repeatCount='indefinite' begin='0s'/><animateTransform attributeName='transform' dur='1.5s' type='scale' values='1;2;1' repeatCount='indefinite' additive='sum' begin='0s'/></circle><animateTransform attributeName='transform' dur='1.5s' type='rotate' from='0 26 26' to='360 26 26' repeatCount='indefinite' begin='0s'/></g>  <g transform='translate(26 26)'><circle class='circlePath4' cx='7' cy='26' r='3'><animateTransform attributeName='transform' dur='2s' type='translate' values='0,0; -6,-26; 0,0' repeatCount='indefinite' begin='0s'/><animateTransform attributeName='transform' dur='2s' type='scale' values='1;2;1' repeatCount='indefinite' additive='sum' begin='0s'/></circle><animateTransform attributeName='transform' dur='2s' type='rotate' from='0 26 26' to='360 26 26' repeatCount='indefinite' begin='0s'/></g> </g>";

            var successResultAnimationSVG: SVGElement = document.createElementNS("http://www.w3.org/2000/svg", "svg");
            successResultAnimationSVG.setAttribute("viewBox", "0 0 52 52");
            successResultAnimationSVG.classList.add("ekyc-success-svg");
            successResultAnimationSVG.innerHTML = "<path class='checkmarkPath__back' d='M14.1 27.2l7.1 7.2 16.7-16.8'></path><path class='checkmarkPath__front' d='M14.1 27.2l7.1 7.2 16.7-16.8'></path>";

            var unsuccessResultAnimationSVG: SVGElement = document.createElementNS("http://www.w3.org/2000/svg", "svg");
            unsuccessResultAnimationSVG.setAttribute("viewBox", "0 0 52 52");
            unsuccessResultAnimationSVG.classList.add("ekyc-unsuccess-svg");
            unsuccessResultAnimationSVG.innerHTML = "<line class='crossPath1__back' x1='18' y1='18' x2='34' y2='34'></line><line class='crossPath2__back' x1='34' y1='18' x2='18' y2='34'></line><line class='crossPath1__front' x1='18' y1='18' x2='34' y2='34'></line><line class='crossPath2__front' x1='34' y1='18' x2='18' y2='34'></line>";

            // Initial Loading Animation Customization
            currentCustomization.initialLoadingAnimationCustomization.customAnimation = activityIndicatorSVG;
            currentCustomization.initialLoadingAnimationCustomization.animationRelativeScale = 1.0;
            currentCustomization.initialLoadingAnimationCustomization.backgroundColor = "transparent";
            currentCustomization.initialLoadingAnimationCustomization.foregroundColor = primaryColor;
            currentCustomization.initialLoadingAnimationCustomization.messageTextColor = secondaryColor;
            currentCustomization.initialLoadingAnimationCustomization.messageFont = font;
            currentCustomization.initialLoadingAnimationCustomization.messageTextSpacing = "normal";
            currentCustomization.initialLoadingAnimationCustomization.messageTextSize = "16px";
            // Overlay Customization
            currentCustomization.overlayCustomization.backgroundColor = "transparent";
            currentCustomization.overlayCustomization.showBrandingImage = true;
            currentCustomization.overlayCustomization.brandingImage = themeResourceDirectory + "ekyc/ekyc_logo.png";
            // Guidance Customization
            currentCustomization.guidanceCustomization.backgroundColors = backgroundColor;
            currentCustomization.guidanceCustomization.foregroundColor = secondaryColor;
            currentCustomization.guidanceCustomization.headerFont = font;
            currentCustomization.guidanceCustomization.headerTextSpacing = "normal";
            currentCustomization.guidanceCustomization.headerTextSize = "20px";
            currentCustomization.guidanceCustomization.subtextFont = font;
            currentCustomization.guidanceCustomization.subtextTextSpacing = "normal";
            currentCustomization.guidanceCustomization.subtextTextSize = "12px";
            currentCustomization.guidanceCustomization.buttonFont = font;
            currentCustomization.guidanceCustomization.buttonTextSpacing = "normal";
            currentCustomization.guidanceCustomization.buttonTextSize = "16px";
            currentCustomization.guidanceCustomization.buttonTextNormalColor = primaryColor;
            currentCustomization.guidanceCustomization.buttonBackgroundNormalColor = backgroundColor;
            currentCustomization.guidanceCustomization.buttonTextHighlightColor = backgroundColor;
            currentCustomization.guidanceCustomization.buttonBackgroundHighlightColor = primaryColor;
            currentCustomization.guidanceCustomization.buttonTextDisabledColor = "rgba(237, 28, 36, 0.3)";
            currentCustomization.guidanceCustomization.buttonBackgroundDisabledColor = backgroundColor;
            currentCustomization.guidanceCustomization.buttonBorderColor = primaryColor;
            currentCustomization.guidanceCustomization.buttonBorderWidth = "2px";
            currentCustomization.guidanceCustomization.buttonCornerRadius = "8px";
            currentCustomization.guidanceCustomization.buttonRelativeWidth = "1.0";
            currentCustomization.guidanceCustomization.buttonRelativeWidthOnDesktop = "0.5";
            currentCustomization.guidanceCustomization.readyScreenOvalFillColor = "rgba(237, 28, 36, 0.2)";
            currentCustomization.guidanceCustomization.readyScreenTextBackgroundColor = backgroundColor;
            currentCustomization.guidanceCustomization.readyScreenTextBackgroundCornerRadius = "3px";
            currentCustomization.guidanceCustomization.retryScreenImageBorderColor = primaryColor;
            currentCustomization.guidanceCustomization.retryScreenImageBorderWidth = "2px";
            currentCustomization.guidanceCustomization.retryScreenImageCornerRadius = "3px";
            currentCustomization.guidanceCustomization.retryScreenOvalStrokeColor = primaryColor;
            currentCustomization.guidanceCustomization.retryScreenSlideshowImages = retryScreenSlideshowImages;
            currentCustomization.guidanceCustomization.retryScreenSlideshowInterval = "1500ms";
            currentCustomization.guidanceCustomization.enableRetryScreenSlideshowShuffle = true;
            currentCustomization.guidanceCustomization.enableRetryScreenBulletedInstructions = true;
            currentCustomization.guidanceCustomization.cameraPermissionsScreenImage = themeResourceDirectory + "ekyc/camera_red.png";
            // ID Scan Customization
            currentCustomization.idScanCustomization.showSelectionScreenBrandingImage = false;
            currentCustomization.idScanCustomization.selectionScreenBrandingImage = "";
            currentCustomization.idScanCustomization.selectionScreenBackgroundColors = backgroundColor;
            currentCustomization.idScanCustomization.reviewScreenBackgroundColors = backgroundColor;
            currentCustomization.idScanCustomization.captureScreenForegroundColor = backgroundColor;
            currentCustomization.idScanCustomization.reviewScreenForegroundColor = backgroundColor;
            currentCustomization.idScanCustomization.selectionScreenForegroundColor = primaryColor;
            currentCustomization.idScanCustomization.headerFont = font;
            currentCustomization.idScanCustomization.headerTextSpacing = "normal";
            currentCustomization.idScanCustomization.headerTextSize = "20px";
            currentCustomization.idScanCustomization.subtextFont = font;
            currentCustomization.idScanCustomization.subtextTextSpacing = "normal";
            currentCustomization.idScanCustomization.subtextTextSize = "12px";
            currentCustomization.idScanCustomization.buttonFont = font;
            currentCustomization.idScanCustomization.buttonTextSpacing = "normal";
            currentCustomization.idScanCustomization.buttonTextSize = "16px";
            currentCustomization.idScanCustomization.buttonTextNormalColor = primaryColor;
            currentCustomization.idScanCustomization.buttonBackgroundNormalColor = backgroundColor;
            currentCustomization.idScanCustomization.buttonTextHighlightColor = backgroundColor;
            currentCustomization.idScanCustomization.buttonBackgroundHighlightColor = primaryColor;
            currentCustomization.idScanCustomization.buttonTextDisabledColor = "rgba(237, 28, 36, 0.3)";
            currentCustomization.idScanCustomization.buttonBackgroundDisabledColor = backgroundColor;
            currentCustomization.idScanCustomization.buttonBorderColor = primaryColor;
            currentCustomization.idScanCustomization.buttonBorderWidth = "2px";
            currentCustomization.idScanCustomization.buttonCornerRadius = "8px";
            currentCustomization.idScanCustomization.buttonRelativeWidth = "1.0";
            currentCustomization.idScanCustomization.buttonRelativeWidthOnDesktop = "0.5";
            currentCustomization.idScanCustomization.captureScreenTextBackgroundColor = primaryColor;
            currentCustomization.idScanCustomization.captureScreenTextBackgroundBorderColor = primaryColor;
            currentCustomization.idScanCustomization.captureScreenTextBackgroundBorderWidth = "0px";
            currentCustomization.idScanCustomization.captureScreenTextBackgroundCornerRadius = "2px";
            currentCustomization.idScanCustomization.reviewScreenTextBackgroundColor = primaryColor;
            currentCustomization.idScanCustomization.reviewScreenTextBackgroundBorderColor = primaryColor;
            currentCustomization.idScanCustomization.reviewScreenTextBackgroundBorderWidth = "0px";
            currentCustomization.idScanCustomization.reviewScreenTextBackgroundBorderCornerRadius = "2px";
            currentCustomization.idScanCustomization.captureScreenBackgroundColor = backgroundColor;
            currentCustomization.idScanCustomization.captureFrameStrokeColor = primaryColor;
            currentCustomization.idScanCustomization.captureFrameStrokeWidth = "2px";
            currentCustomization.idScanCustomization.captureFrameCornerRadius = "12px";
            // Result Screen Customization
            currentCustomization.resultScreenCustomization.backgroundColors = backgroundColor;
            currentCustomization.resultScreenCustomization.foregroundColor = secondaryColor;
            currentCustomization.resultScreenCustomization.messageFont = font;
            currentCustomization.resultScreenCustomization.messageTextSpacing = "normal";
            currentCustomization.resultScreenCustomization.messageTextSize = "16px";
            currentCustomization.resultScreenCustomization.activityIndicatorColor = primaryColor;
            currentCustomization.resultScreenCustomization.customActivityIndicatorImage = "";
            currentCustomization.resultScreenCustomization.customActivityIndicatorRotationInterval = "1.5s";
            currentCustomization.resultScreenCustomization.customActivityIndicatorAnimation = activityIndicatorSVG;
            currentCustomization.resultScreenCustomization.resultAnimationBackgroundColor = "transparent";
            currentCustomization.resultScreenCustomization.resultAnimationForegroundColor = "transparent";
            currentCustomization.resultScreenCustomization.resultAnimationSuccessBackgroundImage = "";
            currentCustomization.resultScreenCustomization.resultAnimationUnsuccessBackgroundImage = "";
            currentCustomization.resultScreenCustomization.customResultAnimationSuccess = successResultAnimationSVG;
            currentCustomization.resultScreenCustomization.customResultAnimationUnsuccess = unsuccessResultAnimationSVG;
            currentCustomization.resultScreenCustomization.showUploadProgressBar = false;
            currentCustomization.resultScreenCustomization.uploadProgressTrackColor = "rgba(0, 0, 0, 0.2)";
            currentCustomization.resultScreenCustomization.uploadProgressFillColor = primaryColor;
            currentCustomization.resultScreenCustomization.animationRelativeScale = 1.0;
            // Feedback Customization
            currentCustomization.feedbackCustomization.backgroundColor = secondaryColor;
            currentCustomization.feedbackCustomization.textColor = backgroundColor;
            currentCustomization.feedbackCustomization.textFont = font;
            currentCustomization.feedbackCustomization.textSpacing = "normal";
            currentCustomization.feedbackCustomization.textSize = "18px";
            currentCustomization.feedbackCustomization.cornerRadius = "3px";
            currentCustomization.feedbackCustomization.shadow = "0px 3px 6px 3px rgba(237, 28, 36, 0.7)";
            currentCustomization.feedbackCustomization.relativeWidth = "1.0";
            currentCustomization.feedbackCustomization.relativeWidthOnDesktop = "0.5";
            // Frame Customization
            currentCustomization.frameCustomization.backgroundColor = backgroundColor;
            currentCustomization.frameCustomization.borderColor = primaryColor;
            currentCustomization.frameCustomization.borderWidth = "2px";
            currentCustomization.frameCustomization.borderCornerRadius = "8px";
            currentCustomization.frameCustomization.shadow = "0px 3px 6px 3px rgba(237, 28, 36, 0.7)";
            // Oval Customization
            currentCustomization.ovalCustomization.strokeColor = primaryColor;
            currentCustomization.ovalCustomization.progressColor1 = "rgba(237, 28, 36, 0.7)";
            currentCustomization.ovalCustomization.progressColor2 = "rgba(237, 28, 36, 0.7)";
            // Cancel Button Customization
            currentCustomization.cancelButtonCustomization.customImage = themeResourceDirectory + "ekyc/cancel_box_red.png";
            currentCustomization.cancelButtonCustomization.location = FaceTecSDK.FaceTecCancelButtonLocation.TopRight;
        }
        else if(theme === "Sample Bank") {
            let primaryColor = "white";
            let backgroundColor = "rgb(29, 23, 79)"; // navy
            let font = "'Source Sans Pro', Helvetica, sans-serif";

            // Initial Loading Animation Customization
            currentCustomization.initialLoadingAnimationCustomization.customAnimation = null;
            currentCustomization.initialLoadingAnimationCustomization.animationRelativeScale = 1.0;
            currentCustomization.initialLoadingAnimationCustomization.backgroundColor = backgroundColor;
            currentCustomization.initialLoadingAnimationCustomization.foregroundColor = primaryColor;
            currentCustomization.initialLoadingAnimationCustomization.messageTextColor = backgroundColor;
            currentCustomization.initialLoadingAnimationCustomization.messageFont = font;
            currentCustomization.initialLoadingAnimationCustomization.messageTextSpacing = "normal";
            currentCustomization.initialLoadingAnimationCustomization.messageTextSize = "16px";
            // Overlay Customization
            currentCustomization.overlayCustomization.backgroundColor = "transparent";
            currentCustomization.overlayCustomization.showBrandingImage = true;
            currentCustomization.overlayCustomization.brandingImage = themeResourceDirectory + "sample-bank/sample_bank_logo.png";
            // Guidance Customization
            currentCustomization.guidanceCustomization.backgroundColors = backgroundColor;
            currentCustomization.guidanceCustomization.foregroundColor = primaryColor;
            currentCustomization.guidanceCustomization.headerFont = font;
            currentCustomization.guidanceCustomization.headerTextSpacing = "normal";
            currentCustomization.guidanceCustomization.headerTextSize = "20px";
            currentCustomization.guidanceCustomization.subtextFont = font;
            currentCustomization.guidanceCustomization.subtextTextSpacing = "normal";
            currentCustomization.guidanceCustomization.subtextTextSize = "12px";
            currentCustomization.guidanceCustomization.buttonFont = font;
            currentCustomization.guidanceCustomization.buttonTextSpacing = "normal";
            currentCustomization.guidanceCustomization.buttonTextSize = "16px";
            currentCustomization.guidanceCustomization.buttonTextNormalColor = backgroundColor;
            currentCustomization.guidanceCustomization.buttonBackgroundNormalColor = primaryColor;
            currentCustomization.guidanceCustomization.buttonTextHighlightColor = backgroundColor;
            currentCustomization.guidanceCustomization.buttonBackgroundHighlightColor = "rgba(255, 255, 255, 0.8)";
            currentCustomization.guidanceCustomization.buttonTextDisabledColor = "rgba(29, 23, 79, 0.3)";
            currentCustomization.guidanceCustomization.buttonBackgroundDisabledColor = primaryColor;
            currentCustomization.guidanceCustomization.buttonBorderColor = backgroundColor;
            currentCustomization.guidanceCustomization.buttonBorderWidth = "2px";
            currentCustomization.guidanceCustomization.buttonCornerRadius = "2px";
            currentCustomization.guidanceCustomization.buttonRelativeWidth = "1.0";
            currentCustomization.guidanceCustomization.buttonRelativeWidthOnDesktop = "0.5";
            currentCustomization.guidanceCustomization.readyScreenOvalFillColor = "rgba(255, 255, 255, 0.2)";
            currentCustomization.guidanceCustomization.readyScreenTextBackgroundColor = backgroundColor;
            currentCustomization.guidanceCustomization.readyScreenTextBackgroundCornerRadius = "2px";
            currentCustomization.guidanceCustomization.retryScreenImageBorderColor = primaryColor;
            currentCustomization.guidanceCustomization.retryScreenImageBorderWidth = "2px";
            currentCustomization.guidanceCustomization.retryScreenImageCornerRadius = "2px";
            currentCustomization.guidanceCustomization.retryScreenOvalStrokeColor = primaryColor;
            currentCustomization.guidanceCustomization.retryScreenSlideshowImages = retryScreenSlideshowImages;
            currentCustomization.guidanceCustomization.retryScreenSlideshowInterval = "1500ms";
            currentCustomization.guidanceCustomization.enableRetryScreenSlideshowShuffle = false;
            currentCustomization.guidanceCustomization.enableRetryScreenBulletedInstructions = true;
            currentCustomization.guidanceCustomization.cameraPermissionsScreenImage = themeResourceDirectory + "sample-bank/camera_white_navy.png";
            // ID Scan Customization
            currentCustomization.idScanCustomization.showSelectionScreenBrandingImage = false;
            currentCustomization.idScanCustomization.selectionScreenBrandingImage = "";
            currentCustomization.idScanCustomization.selectionScreenBackgroundColors = backgroundColor;
            currentCustomization.idScanCustomization.reviewScreenBackgroundColors = backgroundColor;
            currentCustomization.idScanCustomization.captureScreenForegroundColor = backgroundColor;
            currentCustomization.idScanCustomization.reviewScreenForegroundColor = backgroundColor;
            currentCustomization.idScanCustomization.selectionScreenForegroundColor = primaryColor;
            currentCustomization.idScanCustomization.headerFont = font;
            currentCustomization.idScanCustomization.headerTextSpacing = "normal";
            currentCustomization.idScanCustomization.headerTextSize = "20px";
            currentCustomization.idScanCustomization.subtextFont = font;
            currentCustomization.idScanCustomization.subtextTextSpacing = "normal";
            currentCustomization.idScanCustomization.subtextTextSize = "12px";
            currentCustomization.idScanCustomization.buttonFont = font;
            currentCustomization.idScanCustomization.buttonTextSpacing = "normal";
            currentCustomization.idScanCustomization.buttonTextSize = "16px";
            currentCustomization.idScanCustomization.buttonTextNormalColor = backgroundColor;
            currentCustomization.idScanCustomization.buttonBackgroundNormalColor = primaryColor;
            currentCustomization.idScanCustomization.buttonTextHighlightColor = backgroundColor;
            currentCustomization.idScanCustomization.buttonBackgroundHighlightColor = "rgba(255, 255, 255, 0.8)";
            currentCustomization.idScanCustomization.buttonTextDisabledColor = "rgba(29, 23, 79, 0.3)";
            currentCustomization.idScanCustomization.buttonBackgroundDisabledColor = primaryColor;
            currentCustomization.idScanCustomization.buttonBorderColor = backgroundColor;
            currentCustomization.idScanCustomization.buttonBorderWidth = "2px";
            currentCustomization.idScanCustomization.buttonCornerRadius = "2px";
            currentCustomization.idScanCustomization.buttonRelativeWidth = "1.0";
            currentCustomization.idScanCustomization.buttonRelativeWidthOnDesktop = "0.5";
            currentCustomization.idScanCustomization.captureScreenTextBackgroundColor = primaryColor;
            currentCustomization.idScanCustomization.captureScreenTextBackgroundBorderColor = backgroundColor;
            currentCustomization.idScanCustomization.captureScreenTextBackgroundBorderWidth = "2px";
            currentCustomization.idScanCustomization.captureScreenTextBackgroundCornerRadius = "2px";
            currentCustomization.idScanCustomization.reviewScreenTextBackgroundColor = primaryColor;
            currentCustomization.idScanCustomization.reviewScreenTextBackgroundBorderColor = backgroundColor;
            currentCustomization.idScanCustomization.reviewScreenTextBackgroundBorderWidth = "2px";
            currentCustomization.idScanCustomization.reviewScreenTextBackgroundBorderCornerRadius = "2px";
            currentCustomization.idScanCustomization.captureScreenBackgroundColor = backgroundColor;
            currentCustomization.idScanCustomization.captureFrameStrokeColor = primaryColor;
            currentCustomization.idScanCustomization.captureFrameStrokeWidth = "2px";
            currentCustomization.idScanCustomization.captureFrameCornerRadius = "12px";
            // Result Screen Customization
            currentCustomization.resultScreenCustomization.backgroundColors = backgroundColor;
            currentCustomization.resultScreenCustomization.foregroundColor = primaryColor;
            currentCustomization.resultScreenCustomization.messageFont = font;
            currentCustomization.resultScreenCustomization.messageTextSpacing = "normal";
            currentCustomization.resultScreenCustomization.messageTextSize = "16px";
            currentCustomization.resultScreenCustomization.activityIndicatorColor = primaryColor;
            currentCustomization.resultScreenCustomization.customActivityIndicatorImage = themeResourceDirectory + "sample-bank/activity_indicator_white.png";
            currentCustomization.resultScreenCustomization.customActivityIndicatorRotationInterval = "1s";
            currentCustomization.resultScreenCustomization.customActivityIndicatorAnimation = null;
            currentCustomization.resultScreenCustomization.resultAnimationBackgroundColor = "transparent";
            currentCustomization.resultScreenCustomization.resultAnimationForegroundColor = primaryColor;
            currentCustomization.resultScreenCustomization.resultAnimationSuccessBackgroundImage = themeResourceDirectory + "sample-bank/reticle_white.png";
            currentCustomization.resultScreenCustomization.resultAnimationUnsuccessBackgroundImage = themeResourceDirectory + "sample-bank/reticle_white.png";
            currentCustomization.resultScreenCustomization.customResultAnimationSuccess = null;
            currentCustomization.resultScreenCustomization.customResultAnimationUnsuccess = null;
            currentCustomization.resultScreenCustomization.showUploadProgressBar = true;
            currentCustomization.resultScreenCustomization.uploadProgressTrackColor = "rgba(255, 255, 255, 0.2)";
            currentCustomization.resultScreenCustomization.uploadProgressFillColor = primaryColor;
            currentCustomization.resultScreenCustomization.animationRelativeScale = 1.0;
            // Feedback Customization
            currentCustomization.feedbackCustomization.backgroundColor = primaryColor;
            currentCustomization.feedbackCustomization.textColor = backgroundColor;
            currentCustomization.feedbackCustomization.textFont = font;
            currentCustomization.feedbackCustomization.textSpacing = "normal";
            currentCustomization.feedbackCustomization.textSize = "18px";
            currentCustomization.feedbackCustomization.cornerRadius = "2px";
            currentCustomization.feedbackCustomization.shadow = "none";
            currentCustomization.feedbackCustomization.relativeWidth = "1.0";
            currentCustomization.feedbackCustomization.relativeWidthOnDesktop = "0.5";
            // Frame Customization
            currentCustomization.frameCustomization.backgroundColor = backgroundColor;
            currentCustomization.frameCustomization.borderColor = backgroundColor;
            currentCustomization.frameCustomization.borderWidth = "2px";
            currentCustomization.frameCustomization.borderCornerRadius = "2px";
            currentCustomization.frameCustomization.shadow = "none";
            // Oval Customization
            currentCustomization.ovalCustomization.strokeColor = primaryColor;
            currentCustomization.ovalCustomization.progressColor1 = "rgba(255, 255, 255, 0.8)";
            currentCustomization.ovalCustomization.progressColor2 = "rgba(255, 255, 255, 0.8)";
            // Cancel Button Customization
            currentCustomization.cancelButtonCustomization.customImage = themeResourceDirectory + "sample-bank/cancel_white.png";
            currentCustomization.cancelButtonCustomization.location = FaceTecSDK.FaceTecCancelButtonLocation.TopLeft;
        }

        return currentCustomization;
    }

    function getLowLightCustomizationForTheme(theme: string): FaceTecCustomization|null {
        var currentLowLightCustomization: FaceTecCustomization|null = getCustomizationForTheme(theme);

        let retryScreenSlideshowImages: string[] = [themeResourceDirectory + "FaceTec_ideal_1.png", themeResourceDirectory + "FaceTec_ideal_2.png", themeResourceDirectory + "FaceTec_ideal_3.png", themeResourceDirectory + "FaceTec_ideal_4.png", themeResourceDirectory + "FaceTec_ideal_5.png"];

        if(theme === "Bitcoin Exchange") {
            let primaryColor = "rgb(247, 150, 52)"; // orange
            let secondaryColor = "rgb(255, 255, 30)"; // yellow
            let backgroundColor = "rgb(66, 66, 66)"; // dark grey

            // Overlay Customization
            currentLowLightCustomization.overlayCustomization.brandingImage = themeResourceDirectory + "bitcoin-exchange/bitcoin_exchange_logo.png";
            // Guidance Customization
            currentLowLightCustomization.guidanceCustomization.foregroundColor = backgroundColor;
            currentLowLightCustomization.guidanceCustomization.buttonTextNormalColor = "white";
            currentLowLightCustomization.guidanceCustomization.buttonBackgroundNormalColor = primaryColor;
            currentLowLightCustomization.guidanceCustomization.buttonTextHighlightColor = "white";
            currentLowLightCustomization.guidanceCustomization.buttonBackgroundHighlightColor = primaryColor;
            currentLowLightCustomization.guidanceCustomization.buttonTextDisabledColor = "white";
            currentLowLightCustomization.guidanceCustomization.buttonBackgroundDisabledColor = primaryColor;
            currentLowLightCustomization.guidanceCustomization.buttonBorderColor = "transparent";
            currentLowLightCustomization.guidanceCustomization.readyScreenOvalFillColor = "rgba(247, 150, 52, 0.2)";
            currentLowLightCustomization.guidanceCustomization.readyScreenTextBackgroundColor = "white";
            currentLowLightCustomization.guidanceCustomization.retryScreenImageBorderColor = primaryColor;
            currentLowLightCustomization.guidanceCustomization.retryScreenOvalStrokeColor = primaryColor;
            currentLowLightCustomization.guidanceCustomization.retryScreenSlideshowImages = [];
            // ID Scan Customization
            currentLowLightCustomization.idScanCustomization.selectionScreenBrandingImage = "";
            currentLowLightCustomization.idScanCustomization.captureScreenForegroundColor = primaryColor;
            currentLowLightCustomization.idScanCustomization.reviewScreenForegroundColor = primaryColor;
            currentLowLightCustomization.idScanCustomization.selectionScreenForegroundColor = primaryColor;
            currentLowLightCustomization.idScanCustomization.buttonTextNormalColor = "white";
            currentLowLightCustomization.idScanCustomization.buttonBackgroundNormalColor = primaryColor;
            currentLowLightCustomization.idScanCustomization.buttonTextHighlightColor = "white";
            currentLowLightCustomization.idScanCustomization.buttonBackgroundHighlightColor = primaryColor;
            currentLowLightCustomization.idScanCustomization.buttonTextDisabledColor = "white";
            currentLowLightCustomization.idScanCustomization.buttonBackgroundDisabledColor = primaryColor;
            currentLowLightCustomization.idScanCustomization.buttonBorderColor = "transparent";
            currentLowLightCustomization.idScanCustomization.captureScreenTextBackgroundColor = backgroundColor;
            currentLowLightCustomization.idScanCustomization.captureScreenTextBackgroundBorderColor = "transparent";
            currentLowLightCustomization.idScanCustomization.reviewScreenTextBackgroundColor = backgroundColor;
            currentLowLightCustomization.idScanCustomization.reviewScreenTextBackgroundBorderColor = "transparent";
            currentLowLightCustomization.idScanCustomization.captureFrameStrokeColor = primaryColor;
            // Result Screen Customization
            currentLowLightCustomization.resultScreenCustomization.foregroundColor = backgroundColor;
            currentLowLightCustomization.resultScreenCustomization.activityIndicatorColor = primaryColor;
            currentLowLightCustomization.resultScreenCustomization.customActivityIndicatorImage = themeResourceDirectory + "bitcoin-exchange/activity_indicator_orange.png";
            currentLowLightCustomization.resultScreenCustomization.customActivityIndicatorAnimation = null;
            currentLowLightCustomization.resultScreenCustomization.resultAnimationBackgroundColor = primaryColor;
            currentLowLightCustomization.resultScreenCustomization.resultAnimationForegroundColor = "white";
            currentLowLightCustomization.resultScreenCustomization.resultAnimationSuccessBackgroundImage = "";
            currentLowLightCustomization.resultScreenCustomization.resultAnimationUnsuccessBackgroundImage = "";
            currentLowLightCustomization.resultScreenCustomization.customResultAnimationSuccess = null;
            currentLowLightCustomization.resultScreenCustomization.customResultAnimationUnsuccess = null;
            currentLowLightCustomization.resultScreenCustomization.uploadProgressTrackColor = "rgba(0, 0, 0, 0.2)";
            currentLowLightCustomization.resultScreenCustomization.uploadProgressFillColor = primaryColor;
            // Feedback Customization
            currentLowLightCustomization.feedbackCustomization.backgroundColor = backgroundColor;
            currentLowLightCustomization.feedbackCustomization.textColor = "white";
            // Frame Customization
            currentLowLightCustomization.frameCustomization.borderColor = backgroundColor;
            // Oval Customization
            currentLowLightCustomization.ovalCustomization.strokeColor = primaryColor;
            currentLowLightCustomization.ovalCustomization.progressColor1 = secondaryColor;
            currentLowLightCustomization.ovalCustomization.progressColor2 = secondaryColor;
            // Cancel Button Customization
            currentLowLightCustomization.cancelButtonCustomization.customImage = themeResourceDirectory + "bitcoin-exchange/single_chevron_left_orange.png";

            // Guidance Customization -- Text Style Overrides
            // Ready Screen Header
            currentLowLightCustomization.guidanceCustomization.readyScreenHeaderTextColor = primaryColor;
            // Ready Screen Subtext
            currentLowLightCustomization.guidanceCustomization.readyScreenSubtextTextColor = backgroundColor;
            // Retry Screen Header
            currentLowLightCustomization.guidanceCustomization.retryScreenHeaderTextColor = primaryColor;
            // Retry Screen Subtext
            currentLowLightCustomization.guidanceCustomization.retryScreenSubtextTextColor = backgroundColor;
        }
        else if(theme === "eKYC") {
            currentLowLightCustomization = null;
        }
        else if(theme === "Sample Bank") {
            let primaryColor = "white";
            let backgroundColor = "rgb(29, 23, 79)"; // navy

            // Overlay Customization
            currentLowLightCustomization.overlayCustomization.brandingImage = themeResourceDirectory + "sample-bank/sample_bank_logo.png";
            // Guidance Customization
            currentLowLightCustomization.guidanceCustomization.foregroundColor = backgroundColor;
            currentLowLightCustomization.guidanceCustomization.buttonTextNormalColor = primaryColor;
            currentLowLightCustomization.guidanceCustomization.buttonBackgroundNormalColor = backgroundColor;
            currentLowLightCustomization.guidanceCustomization.buttonTextHighlightColor = primaryColor;
            currentLowLightCustomization.guidanceCustomization.buttonBackgroundHighlightColor = "rgba(29, 23, 79, 0.8)";
            currentLowLightCustomization.guidanceCustomization.buttonTextDisabledColor = "rgba(255, 255, 255, 0.3)";
            currentLowLightCustomization.guidanceCustomization.buttonBackgroundDisabledColor = backgroundColor;
            currentLowLightCustomization.guidanceCustomization.buttonBorderColor = backgroundColor;
            currentLowLightCustomization.guidanceCustomization.readyScreenOvalFillColor = "rgba(255, 255, 255, 0.2)";
            currentLowLightCustomization.guidanceCustomization.readyScreenTextBackgroundColor = primaryColor;
            currentLowLightCustomization.guidanceCustomization.retryScreenImageBorderColor = backgroundColor;
            currentLowLightCustomization.guidanceCustomization.retryScreenOvalStrokeColor = primaryColor;
            currentLowLightCustomization.guidanceCustomization.retryScreenSlideshowImages = retryScreenSlideshowImages;
            // ID Scan Customization
            currentLowLightCustomization.idScanCustomization.selectionScreenBrandingImage = "";
            currentLowLightCustomization.idScanCustomization.captureScreenForegroundColor = backgroundColor;
            currentLowLightCustomization.idScanCustomization.reviewScreenForegroundColor = backgroundColor;
            currentLowLightCustomization.idScanCustomization.selectionScreenForegroundColor = backgroundColor;
            currentLowLightCustomization.idScanCustomization.buttonTextNormalColor = primaryColor;
            currentLowLightCustomization.idScanCustomization.buttonBackgroundNormalColor = backgroundColor;
            currentLowLightCustomization.idScanCustomization.buttonTextHighlightColor = primaryColor;
            currentLowLightCustomization.idScanCustomization.buttonBackgroundHighlightColor = "rgba(29, 23, 79, 0.8)";
            currentLowLightCustomization.idScanCustomization.buttonTextDisabledColor = "rgba(255, 255, 255, 0.3)";
            currentLowLightCustomization.idScanCustomization.buttonBackgroundDisabledColor = backgroundColor;
            currentLowLightCustomization.idScanCustomization.buttonBorderColor = backgroundColor;
            currentLowLightCustomization.idScanCustomization.captureScreenTextBackgroundColor = primaryColor;
            currentLowLightCustomization.idScanCustomization.captureScreenTextBackgroundBorderColor = backgroundColor;
            currentLowLightCustomization.idScanCustomization.reviewScreenTextBackgroundColor = primaryColor;
            currentLowLightCustomization.idScanCustomization.reviewScreenTextBackgroundBorderColor = backgroundColor;
            currentLowLightCustomization.idScanCustomization.captureFrameStrokeColor = primaryColor;
            // Result Screen Customization
            currentLowLightCustomization.resultScreenCustomization.foregroundColor = backgroundColor;
            currentLowLightCustomization.resultScreenCustomization.activityIndicatorColor = backgroundColor;
            currentLowLightCustomization.resultScreenCustomization.customActivityIndicatorImage = themeResourceDirectory + "sample-bank/activity_indicator_navy.png";
            currentLowLightCustomization.resultScreenCustomization.customActivityIndicatorAnimation = null;
            currentLowLightCustomization.resultScreenCustomization.resultAnimationBackgroundColor = "transparent";
            currentLowLightCustomization.resultScreenCustomization.resultAnimationForegroundColor = backgroundColor;
            currentLowLightCustomization.resultScreenCustomization.resultAnimationSuccessBackgroundImage = themeResourceDirectory + "sample-bank/reticle_navy.png";
            currentLowLightCustomization.resultScreenCustomization.resultAnimationUnsuccessBackgroundImage = themeResourceDirectory + "sample-bank/reticle_navy.png";
            currentLowLightCustomization.resultScreenCustomization.customResultAnimationSuccess = null;
            currentLowLightCustomization.resultScreenCustomization.customResultAnimationUnsuccess = null;
            currentLowLightCustomization.resultScreenCustomization.uploadProgressTrackColor = "rgba(0, 0, 0, 0.2)";
            currentLowLightCustomization.resultScreenCustomization.uploadProgressFillColor = backgroundColor;
            // Feedback Customization
            currentLowLightCustomization.feedbackCustomization.backgroundColor = backgroundColor;
            currentLowLightCustomization.feedbackCustomization.textColor = primaryColor;
            // Frame Customization
            currentLowLightCustomization.frameCustomization.borderColor = backgroundColor;
            // Oval Customization
            currentLowLightCustomization.ovalCustomization.strokeColor = backgroundColor;
            currentLowLightCustomization.ovalCustomization.progressColor1 = "rgba(29, 23, 79, 0.8)";
            currentLowLightCustomization.ovalCustomization.progressColor2 = "rgba(29, 23, 79, 0.8)";
            // Cancel Button Customization
            currentLowLightCustomization.cancelButtonCustomization.customImage = themeResourceDirectory + "sample-bank/cancel_navy.png";
        }
        else {
          currentLowLightCustomization = null;
        }

        return currentLowLightCustomization;
    }

    function showNewTheme(){
      var themes: string[] = [""];
      if(Config.wasSDKConfiguredWithConfigWizard === true) {
        themes = ["Config Wizard Theme", "FaceTec Theme", "Pseudo-Fullscreen", "Well-Rounded", "Bitcoin Exchange", "eKYC", "Sample Bank"];
      }
      else {
        themes = ["FaceTec Theme", "Pseudo-Fullscreen", "Well-Rounded", "Bitcoin Exchange", "eKYC", "Sample Bank"];
      }
      var currentThemeIndex = themes.indexOf(currentTheme);
      currentThemeIndex = currentThemeIndex >= themes.length - 1 ? 0 : currentThemeIndex + 1;
      currentTheme =  themes[currentThemeIndex];
      setAppTheme(currentTheme);
      updateThemeTransitionView();
      SampleAppUtilities.displayStatus("Theme set to: " + currentTheme);
    }

    function updateThemeTransitionView() {
      var transitionViewImage = "";
      var transitionViewTextColor = Config.currentCustomization.guidanceCustomization.foregroundColor;
      var transitionViewClass = "theme-transition-overlay__";
      var deviceType = "desktop";
      if(SampleAppUtilities.isLikelyMobileDevice()) {
        deviceType = "mobile";
      }
      switch (currentTheme) {
        case "FaceTec Theme":
          transitionViewClass = "default";
          break;
        case "Pseudo-Fullscreen":
          transitionViewClass += "default";
          break;
        case "Well-Rounded":
          transitionViewImage = themeResourceDirectory + "well-rounded/well_rounded_" + deviceType + "_bg.svg";
          transitionViewClass += "well-rounded";
          transitionViewTextColor = Config.currentCustomization.frameCustomization.backgroundColor;
          break;
        case "Bitcoin Exchange":
          transitionViewImage = themeResourceDirectory + "bitcoin-exchange/bitcoin_exchange_" + deviceType + "_bg.svg";
          transitionViewClass += "bitcoin-exchange";
          transitionViewTextColor = Config.currentCustomization.frameCustomization.backgroundColor;
          break;
        case "eKYC":
          transitionViewImage = themeResourceDirectory + "ekyc/ekyc_" + deviceType + "_bg.svg";
          transitionViewClass += "ekyc";
          break;
        case "Sample Bank":
          transitionViewImage = themeResourceDirectory + "sample-bank/sample_bank_" + deviceType + "_bg.svg";
          transitionViewClass += "sample-bank";
          transitionViewTextColor = Config.currentCustomization.frameCustomization.backgroundColor;
          break;
        default:
          transitionViewClass = "default";
          break;
      }
      transitionViewClass += "__" + deviceType;

      $("#theme-transition-overlay-img").attr("src", transitionViewImage);
      (document.getElementById("theme-transition-overlay") as HTMLElement).className = transitionViewClass;
      (document.getElementById("loading-session-token-text") as HTMLElement).style.color = transitionViewTextColor;
    }

    return {
        themeResourceDirectory,
        setAppTheme(theme:string) {
            setAppTheme(theme);
        },
        showNewTheme
    };
})();
