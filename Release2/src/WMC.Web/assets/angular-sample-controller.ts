import { Config } from "./Config";
import { FaceTecSDK } from "./core-sdk/FaceTecSDK.js/FaceTecSDK";
import { LivenessCheckProcessor } from "./processors/LivenessCheckProcessor";
import { SampleAppUtilities } from "./utilities/SampleAppUtilities";
import { FaceTecSessionResult, FaceTecIDScanResult } from "./core-sdk/FaceTecSDK.js/FaceTecPublicApi";
import { PhotoIDMatchProcessor } from "./processors/PhotoIDMatchProcessor";

export const AngularSampleApp = (function () {
  let latestEnrollmentIdentifier = "";
  let latestSessionResult: FaceTecSessionResult | null = null;
  let latestIDScanResult: FaceTecIDScanResult | null = null;
  let latestProcessor: LivenessCheckProcessor | PhotoIDMatchProcessor;;


  // Initiate a 3D Liveness Check.
  function onLivenessCheckPressed() {
    SampleAppUtilities.fadeOutMainUIAndPrepareForSession();

    // Get a Session Token from the FaceTec SDK, then start the 3D Liveness Check.
    getSessionToken(function (sessionToken) {
      latestProcessor = new LivenessCheckProcessor(sessionToken as string, AngularSampleApp as any);
    });
  }

  // Initiate a Photo ID Match.
  function onPhotoIdMatchPressed() {
    SampleAppUtilities.fadeOutMainUIAndPrepareForSession();

    // Get a Session Token from the FaceTec SDK, then start the 3D Liveness Check.
    getSessionToken(function (sessionToken) {
      latestEnrollmentIdentifier = SampleAppUtilities.generateUUId();
      latestProcessor = new PhotoIDMatchProcessor(sessionToken as string, AngularSampleApp as any);
    });

  }

  // Show the final result and transition back into the main interface.
  function onComplete(isSuccess: boolean, zoomSessionResultL?: FaceTecSessionResult, ZoomIDScanResultL?: FaceTecIDScanResult) {
    isSuccess = latestProcessor.isSuccess();
    zoomSessionResultL = latestSessionResult;
    ZoomIDScanResultL = latestIDScanResult;

    var ZoomIdscan = {
      status: ZoomIDScanResultL?.status,
      idType: ZoomIDScanResultL?.idType,
      //idScan: ZoomIDScanResultL.idScan,
      // frontImages: [ZoomIDScanResult.frontImages[0]],
      // backImages: [ZoomIDScanResult.backImages[0]],
      sessionId: ZoomIDScanResultL?.sessionId
    }

    var zoomSession = {
      //faceScan: zoomSessionResultL.faceScan,
      // auditTrail: zoomSessionResult.auditTrail,
      //lowQualityAuditTrail: [zoomSessionResult.lowQualityAuditTrail[0]],
      sessionId: zoomSessionResultL.sessionId,
      status: zoomSessionResultL.status
    }

    //TODO:Using image[0] to reduce the size of maxJSON lengtht
    var faceResults = {
      isSuccess: isSuccess,
      zoomSessionResult: zoomSession,
      zoomIDScanResult: ZoomIdscan
    }
    
    SampleAppUtilities.showMainUI();

    //clear the blur in app
    document.getElementById("tabContent").style.filter = "";

    //Dispatch an event and call in Monni Application
    var evt = new CustomEvent("OnSucccess", { detail: faceResults });
    window.dispatchEvent(evt);

    if (!latestProcessor.isSuccess()) {
      // Reset the enrollment identifier.
      latestEnrollmentIdentifier = "";

      // Show early exit message to screen.  If this occurs, please check logs.
      SampleAppUtilities.displayStatus("Session exited early, see logs for more details.");

      return;
    }

    // Show successful message to screen
    SampleAppUtilities.displayStatus("Success");

  }


  // Get the Session Token from the server
  function getSessionToken(sessionTokenCallback: (sessionToken?: string) => void) {
    const XHR = new XMLHttpRequest();
    XHR.open("GET", Config.BaseURL + "/session-token");
    XHR.setRequestHeader("X-Device-Key", Config.DeviceKeyIdentifier);
    XHR.setRequestHeader("X-User-Agent", FaceTecSDK.createFaceTecAPIUserAgentString(""));
    XHR.onreadystatechange = function () {
      if (this.readyState === XMLHttpRequest.DONE) {
        let sessionToken = "";
        try {
          // Attempt to get the sessionToken from the response object.
          sessionToken = JSON.parse(this.responseText).sessionToken;
          // Something went wrong in parsing the response. Return an error.
          if (typeof sessionToken !== "string") {
            onServerSessionTokenError();
            return;
          }
        }
        catch {
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
    SampleAppUtilities.handleErrorGettingServerSessionToken();
  }

  //
  // DEVELOPER NOTE:  This is a convenience function for demonstration purposes only so the Sample App can have access to the latest session results.
  // In your code, you may not even want or need to do this.
  //
  function setLatestSessionResult(sessionResult: FaceTecSessionResult) {
    latestSessionResult = sessionResult;
  }

  function setLatestIDScanResult(idScanResult: FaceTecIDScanResult) {
    latestIDScanResult = idScanResult;
  }

  function getLatestEnrollmentIdentifier() {
    return latestEnrollmentIdentifier;
  }

  function setLatestServerResult(responseJSON: any) {
  }

  function clearLatestEnrollmentIdentifier(responseJSON: any) {
    latestEnrollmentIdentifier = "";
  }

  return {
    onLivenessCheckPressed,
    onPhotoIdMatchPressed,
    onComplete,
    setLatestSessionResult,
    setLatestIDScanResult,
    getLatestEnrollmentIdentifier,
    setLatestServerResult,
    clearLatestEnrollmentIdentifier
  };

})();
