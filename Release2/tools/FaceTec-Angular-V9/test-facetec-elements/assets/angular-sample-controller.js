"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var Config_1 = require("./Config");
var FaceTecSDK_1 = require("./core-sdk/FaceTecSDK.js/FaceTecSDK");
var LivenessCheckProcessor_1 = require("./processors/LivenessCheckProcessor");
var SampleAppUtilities_1 = require("./utilities/SampleAppUtilities");
var PhotoIDMatchProcessor_1 = require("./processors/PhotoIDMatchProcessor");
exports.AngularSampleApp = (function () {
    var latestEnrollmentIdentifier = "";
    var latestSessionResult = null;
    var latestIDScanResult = null;
    var latestProcessor;
    ;
    // Wait for onload to be complete before attempting to access the Browser SDK.
    window.onload = function () {
        // Set a the directory path for other FaceTec Browser SDK Resources.
        FaceTecSDK_1.FaceTecSDK.setResourceDirectory("/assets/core-sdk/FaceTecSDK.js/resources");
        // Set the directory path for required FaceTec Browser SDK images.
        FaceTecSDK_1.FaceTecSDK.setImagesDirectory("/assets/core-sdk/FaceTec_images");
        // Initialize FaceTec Browser SDK and configure the UI features.
        FaceTecSDK_1.FaceTecSDK.initializeInDevelopmentMode(Config_1.Config.DeviceKeyIdentifier, Config_1.Config.PublicFaceMapEncryptionKey, function (initializedSuccessfully) {
            if (initializedSuccessfully) {
                SampleAppUtilities_1.SampleAppUtilities.enableAllButtons();
            }
            SampleAppUtilities_1.SampleAppUtilities.displayStatus(FaceTecSDK_1.FaceTecSDK.getFriendlyDescriptionForFaceTecSDKStatus(FaceTecSDK_1.FaceTecSDK.getStatus()));
        });
        SampleAppUtilities_1.SampleAppUtilities.formatUIForDevice();
    };
    // Initiate a 3D Liveness Check.
    function onLivenessCheckPressed() {
        SampleAppUtilities_1.SampleAppUtilities.fadeOutMainUIAndPrepareForSession();
        // Get a Session Token from the FaceTec SDK, then start the 3D Liveness Check.
        getSessionToken(function (sessionToken) {
            latestProcessor = new LivenessCheckProcessor_1.LivenessCheckProcessor(sessionToken, exports.AngularSampleApp);
        });
    }
    // Initiate a Photo ID Match.
    function onPhotoIdMatchPressed() {
        SampleAppUtilities_1.SampleAppUtilities.fadeOutMainUIAndPrepareForSession();
        // Get a Session Token from the FaceTec SDK, then start the 3D Liveness Check.
        getSessionToken(function (sessionToken) {
            latestEnrollmentIdentifier = SampleAppUtilities_1.SampleAppUtilities.generateUUId();
            latestProcessor = new PhotoIDMatchProcessor_1.PhotoIDMatchProcessor(sessionToken, exports.AngularSampleApp);
        });
    }
    // Show the final result and transition back into the main interface.
    function onComplete() {
        SampleAppUtilities_1.SampleAppUtilities.showMainUI();
        if (!latestProcessor.isSuccess()) {
            // Reset the enrollment identifier.
            latestEnrollmentIdentifier = "";
            // Show early exit message to screen.  If this occurs, please check logs.
            SampleAppUtilities_1.SampleAppUtilities.displayStatus("Session exited early, see logs for more details.");
            return;
        }
        // Show successful message to screen
        SampleAppUtilities_1.SampleAppUtilities.displayStatus("Success");
    }
    // Get the Session Token from the server
    function getSessionToken(sessionTokenCallback) {
        var XHR = new XMLHttpRequest();
        XHR.open("GET", Config_1.Config.BaseURL + "/session-token");
        XHR.setRequestHeader("X-Device-Key", Config_1.Config.DeviceKeyIdentifier);
        XHR.setRequestHeader("X-User-Agent", FaceTecSDK_1.FaceTecSDK.createFaceTecAPIUserAgentString(""));
        XHR.onreadystatechange = function () {
            if (this.readyState === XMLHttpRequest.DONE) {
                var sessionToken = "";
                try {
                    // Attempt to get the sessionToken from the response object.
                    sessionToken = JSON.parse(this.responseText).sessionToken;
                    // Something went wrong in parsing the response. Return an error.
                    if (typeof sessionToken !== "string") {
                        onServerSessionTokenError();
                        return;
                    }
                }
                catch (_a) {
                    // Something went wrong in parsing the response. Return an error.
                    onServerSessionTokenError();
                    return;
                }
                sessionTokenCallback(sessionToken);
            }
        };
        XHR.onerror = function () {
            onServerSessionTokenError();
        };
        XHR.send();
    }
    function onServerSessionTokenError() {
        SampleAppUtilities_1.SampleAppUtilities.handleErrorGettingServerSessionToken();
    }
    //
    // DEVELOPER NOTE:  This is a convenience function for demonstration purposes only so the Sample App can have access to the latest session results.
    // In your code, you may not even want or need to do this.
    //
    function setLatestSessionResult(sessionResult) {
        latestSessionResult = sessionResult;
    }
    function setLatestIDScanResult(idScanResult) {
        latestIDScanResult = idScanResult;
    }
    function getLatestEnrollmentIdentifier() {
        return latestEnrollmentIdentifier;
    }
    function setLatestServerResult(responseJSON) {
    }
    function clearLatestEnrollmentIdentifier(responseJSON) {
        latestEnrollmentIdentifier = "";
    }
    return {
        onLivenessCheckPressed: onLivenessCheckPressed,
        onPhotoIdMatchPressed: onPhotoIdMatchPressed,
        onComplete: onComplete,
        setLatestSessionResult: setLatestSessionResult,
        setLatestIDScanResult: setLatestIDScanResult,
        getLatestEnrollmentIdentifier: getLatestEnrollmentIdentifier,
        setLatestServerResult: setLatestServerResult,
        clearLatestEnrollmentIdentifier: clearLatestEnrollmentIdentifier
    };
})();
//# sourceMappingURL=angular-sample-controller.js.map