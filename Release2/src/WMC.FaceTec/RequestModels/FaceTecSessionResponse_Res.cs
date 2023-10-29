using System.Collections.Generic;
using Newtonsoft.Json;
using WMC.Logic;

namespace WMC.FaceTec
{
    public class FaceMap
    {
    }

    public class ZoomFaceBiometrics
    {
        [JsonProperty("auditTrail")]
        public List<string> AuditTrail { get; set; }

        [JsonProperty("timeBasedSessionImages")]
        public List<string> TimeBasedSessionImages { get; set; }

        [JsonProperty("faceMap")]
        public FaceMap FaceMap { get; set; }
    }

    public enum ZoomSessionStatus
    {
        /**
         * The ZoOm Session was performed successfully and a FaceMap was generated.  Pass the FaceMap to ZoOm Server for further processing.
         */
        SessionCompletedSuccessfully = 0,
        /**
         * The ZoOm Session was cancelled because not all guidance images were configured.
         */
        MissingGuidanceImages = 1,
        /**
         * The ZoOm Session was cancelled because your App is not in production and requires a network connection.
         */
        NonProductionModeNetworkRequired = 2,
        /**
         * The ZoOm Session was cancelled because the user was unable to complete a ZoOm Session in the default allotted time or the timeout set by the developer.
         */
        Timeout = 3,
        /**
         * The ZoOm Session was cancelled due to the app being terminated, put to sleep, an OS notification, or the app was placed in the background.
         */
        ContextSwitch = 4,
        /**
         * The ZoOm Session was cancelled because the user was unable to place their face in the UnZoOmed, far away oval in the default allotted time or the timeout set by the developer.
         */
        TooMuchTimeToDetectFirstFace = 5,
        /**
         * The ZoOm Session was cancelled because the user was unable to place their face in the ZoOmed, close oval in the default allotted time or the timeout set by the developer.
         */
        TooMuchTimeToDetectFirstFaceInPhaseTwo = 6,
        /**
         * The developer programmatically called the ZoOm Session cancel API.
         */
        ProgrammaticallyCancelled = 7,
        /**
         * The ZoOm Session was cancelled due to a device orientation change during the ZoOm Session.
         */
        OrientationChangeDuringSession = 8,
        /**
         * The ZoOm Session was cancelled because device is in landscape mode.
         * The user experience of devices in these orientations is poor and thus portrait is required.
         */
        LandscapeModeNotAllowed = 9,
        /**
         * The user pressed the cancel button and did not complete the ZoOm Session.
         */
        UserCancelled = 10,
        /**
         * The user pressed the cancel button during New User Guidance.
         */
        UserCancelledFromNewUserGuidance = 11,
        /**
         * The user pressed the cancel button during Retry Guidance.
         */
        UserCancelledFromRetryGuidance = 12,
        /**
         * The user cancelled out of the ZoOm experience while attempting to get camera permissions.
         */
        UserCancelledWhenAttemptingToGetCameraPermissions = 13,
        /**
         * The ZoOm Session was cancelled because the user was in a locked out state.
         */
        LockedOut = 14,
        /**
         * The ZoOm Session was cancelled because there was no camera available.
         */
        CameraDoesNotExist = 15,
        /**
         * The ZoOm Session was cancelled because camera was not enabled.
         */
        CameraNotEnabled = 16,
        /**
         * This status will never be returned in a properly configured or production app.
         * This status is returned if your license is invalid or network connectivity issues occur during a session when the application is not in production.
         */
        NonProductionModeLicenseInvalid = 17,
        /**
         * The ZoOm Session was cancelled because preload was not completed or an issue was encountered preloading ZoOm.
         */
        PreloadNotCompleted = 18,
        /**
         * The ZoOm Session was cancelled because video initialization encountered an issue.
         * This status is only returned for Unmanaged Sessions.
         */
        UnmanagedSessionVideoInitializationNotCompleted = 19,
        /**
         * The ZoOm Session was cancelled because one of the elements passed to ZoOm does not exist on the DOM.
         * This status is only returned for Unmanaged Sessions.
         */
        ZoomVideoOrInterfaceDOMElementDoesNotExist = 20,
        /**
         * The ZoOm Session was cancelled because ZoOm cannot be rendered when the document is not ready.
         */
        DocumentNotReady = 21,
        /**
         * The ZoOm Session was cancelled because the video height/width was 0. The camera or video may not be initialized.
         * This status is only returned for Unmanaged Sessions.
         */
        VideoHeightOrWidthZeroOrUninitialized = 22,
        /**
         * The ZoOm Session was cancelled because there was another ZoOm Session in progress.
         */
        ZoomSessionInProgress = 23,
        /**
         * The ZoOm Session was cancelled because the video element is not active.
         * This status is only returned for Unmanaged Sessions.
         */
        VideoCaptureStreamNotActive = 24,
        /**
         * The ZoOm Session was cancelled because the selected camera is not active.
         * This status is only returned for Unmanaged Sessions.
         */
        CameraNotRunning = 25,
        /**
         * The ZoOm Session was cancelled because ZoOm initialization has not been completed yet.
         */
        InitializationNotCompleted = 26,
        /**
         * The ZoOm Session was cancelled because of an unknown and unexpected error.  ZoOm leverages a variety of platform APIs including camera, storage, security, networking, and more.
         * This return value is a catch-all for errors experienced during normal usage of these APIs.
         */
        UnknownInternalError = 27,
        /**
         * The ZoOm Session cancelled because user pressed the Get Ready screen subtext message.
         * Note: This functionality is not available by default, and must be requested from FaceTec in order to enable.
         */
        UserCancelledViaClickableReadyScreenSubtext = 28,
        /**
        * The ZoOm Session was cancelled, ZoOm was opened in an Iframe without an Iframe constructor.
        */
        NotAllowedUseIframeConstructor = 29,
        /**
        * The ZoOm Session was cancelled, ZoOm was not opened in an Iframe with an Iframe constructor.
        */
        NotAllowedUseNonIframeConstructor = 30,
        /**
        * The ZoOm Session was cancelled, ZoOm was not opened in an Iframe without permission.
        */
        IFrameNotAllowedWithoutPermission = 31
    }

    public enum FaceTecSessionStatus
    {
        /**
         * The Session was performed successfully and a FaceScan was generated.  Pass the FaceScan to the Server for further processing.
         */
        SessionCompletedSuccessfully = 0,
        /**
         * The Session was cancelled because not all guidance images were configured.
         */
        MissingGuidanceImages = 1,
        /**
         * The Session was cancelled because the user was unable to complete a Session in the default allotted time or the timeout set by the developer.
         */
        Timeout = 2,
        /**
         * The Session was cancelled due to the app being terminated, put to sleep, an OS notification, or the app was placed in the background.
         */
        ContextSwitch = 3,
        /**
         * The developer programmatically called the Session cancel API.
         */
        ProgrammaticallyCancelled = 4,
        /**
         * The Session was cancelled due to a device orientation change during the Session.
         */
        OrientationChangeDuringSession = 5,
        /**
         * The Session was cancelled because device is in landscape mode.
         * The user experience of devices in these orientations is poor and thus portrait is required.
         */
        LandscapeModeNotAllowed = 6,
        /**
         * The user pressed the cancel button and did not complete the Session.
         */
        UserCancelled = 7,
        /**
         * The user pressed the cancel button during New User Guidance.
         */
        UserCancelledFromNewUserGuidance = 8,
        /**
         * The user pressed the cancel button during Retry Guidance.
         */
        UserCancelledFromRetryGuidance = 9,
        /**
         * The user cancelled out of the the FaceTec Browser SDK experience while attempting to get camera permissions.
         */
        UserCancelledWhenAttemptingToGetCameraPermissions = 10,
        /**
         * The Session was cancelled because the user was in a locked out state.
         */
        LockedOut = 11,
        /**
         * The Session was cancelled because camera was not enabled.
         */
        CameraNotEnabled = 12,
        /**
         * This status will never be returned in a properly configured or production app.
         * This status is returned if your Key is invalid or network connectivity issues occur during a session when the application is not in production.
         */
        NonProductionModeDeviceKeyIdentifierInvalid = 13,
        /**
         * The Session was cancelled because the FaceTec Browser SDK cannot be rendered when the document is not ready.
         */
        DocumentNotReady = 14,
        /**
         * The Session was cancelled because there was another Session in progress.
         */
        SessionInProgress = 15,
        /**
         * The Session was cancelled because the selected camera is not active.
         * This status is only returned for Unmanaged Sessions.
         */
        CameraNotRunning = 16,
        /**
         * The Session was cancelled because initialization has not been completed yet.
         */
        InitializationNotCompleted = 17,
        /**
         * The Session was cancelled because of an unknown and unexpected error.  The FaceTec Browser SDK leverages a variety of platform APIs including camera, storage, security, networking, and more.
         * This return value is a catch-all for errors experienced during normal usage of these APIs.
         */
        UnknownInternalError = 18,
        /**
         * The Session cancelled because user pressed the Get Ready screen subtext message.
         * Note: This functionality is not available by default, and must be requested from FaceTec in order to enable.
         */
        UserCancelledViaClickableReadyScreenSubtext = 19,
        /**
        * The Session was cancelled, the FaceTec Browser SDK was opened in an Iframe without an Iframe constructor.
        */
        NotAllowedUseIframeConstructor = 20,
        /**
        * The Session was cancelled, the FaceTec Browser SDK was not opened in an Iframe with an Iframe constructor.
        */
        NotAllowedUseNonIframeConstructor = 21,
        /**
        * The Session was cancelled, the FaceTec Browser SDK was not opened in an Iframe without permission.
        */
        IFrameNotAllowedWithoutPermission = 22,
        /**
        * FaceTec SDK is still loading resources.
        */
        StillLoadingResources = 23,
        /**
        * FaceTec SDK could not load resources.
        */
        ResourcesCouldNotBeLoadedOnLastInit = 24
    }
    public class FaceTecSessionResponse : ICloneCacheObject<FaceTecSessionResponse>
    {
        [JsonProperty("faceMetrics")]
        public ZoomFaceBiometrics FaceMetrics { get; set; }

        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        [JsonProperty("status")]
        public ZoomSessionStatus Status { get; set; }

        [JsonProperty("countOfZoomSessionsPerformed")]
        public int CountOfZoomSessionsPerformed { get; set; }

        public FaceTecSessionResponse Clone()
        {
            return new FaceTecSessionResponse { Status = Status, SessionId = SessionId, CountOfZoomSessionsPerformed = CountOfZoomSessionsPerformed };
        }
    }

    public class FaceTecSessionResult : ICloneCacheObject<FaceTecSessionResult>
    {

        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        [JsonProperty("status")]
        public FaceTecSessionStatus Status { get; set; }

        public FaceTecSessionResult Clone()
        {
            return new FaceTecSessionResult { Status = Status, SessionId = SessionId };
        }
    }
    public class FaceTecSession : ICloneCacheObject<FaceTecSession>
    {
        public FaceTecSessionResult zoomSessionResult { get; set; }
        public FaceTecScanIDSessionResult zoomIDScanResult { get; set; }
        public bool isSuccess { get; set; }

        public FaceTecSession Clone()
        {
            return new FaceTecSession { zoomIDScanResult = zoomIDScanResult?.Clone(), zoomSessionResult = zoomSessionResult?.Clone() };
        }
    }
}
