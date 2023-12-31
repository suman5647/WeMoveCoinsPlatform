import { FaceTecCustomization, FaceTecOvalCustomization, FaceTecCancelButtonCustomization, FaceTecFeedbackBarCustomization, FaceTecFrameCustomization, FaceTecExitAnimationCustomization, FaceTecSessionTimerCustomization, FaceTecExitAnimationStyle, FaceTecCancelButtonLocation, FaceTecOverlayCustomization, FaceTecGuidanceCustomization, FaceTecResultScreenCustomization, FaceTecEnterFullScreenCustomization, FaceTecSecurityWatermarkImage, FaceTecSecurityWatermarkCustomization } from "./FaceTecCustomization";
import { FaceTecLoggingMode } from "./FaceTecLogging";
import { FaceTecSession, FaceTecSessionFromIFrame } from "./FaceTecSession";
import { FaceTecAuditTrailType, FaceTecSDKStatus, FaceTecSessionStatus, FaceTecIDScanStatus, FaceTecFaceScanProcessor, FaceTecIDScanProcessor, FaceTecIDScanRetryMode, FaceTecIDScanNextStep, FaceTecFaceScanResultCallback, FaceTecIDScanResultCallback } from "./FaceTecPublicApi";
export declare var FaceTecSDK: {
    /**
      * Initialize FaceTecSDK in development mode using a Device Identifier Key - HTTPS Log mode.
    **/
    initializeInDevelopmentMode: (deviceKeyIdentifier: string, publicEncryptionKey: string, onInitializationComplete: (result: boolean) => void) => void;
    /**
      * Initialize FaceTecSDK in production mode using a Production Key - SFTP Log mode.
    **/
    initializeInProductionMode: (productionKey: string, deviceKeyIdentifier: string, publicEncryptionKey: string, onInitializationComplete: (result: boolean) => void) => void;
    /**
      * Ensure that the FaceTecSDK is initialized and ready before attempting to start a Session.
    **/
    getStatus: () => FaceTecSDKStatus;
    /**
      * Return Enums associated with FaceTecSDK.getStatus.
    **/
    FaceTecSDKStatus: typeof FaceTecSDKStatus;
    /**
    * Return friendly names for the enums in FaceTecSDKStatus.
    **/
    getFriendlyDescriptionForFaceTecSDKStatus: (enumValue: FaceTecSDKStatus) => string;
    /**
      * Core function calls that create and launch the FaceTec SDK Interface.
    **/
    FaceTecSession: typeof FaceTecSession;
    /**
      * Core function calls that create and launch the FaceTec SDK Interface in an IFrame.
    **/
    FaceTecSessionFromIFrame: typeof FaceTecSessionFromIFrame;
    /**
      * Developer created class for handling processing of the FaceTec SDK FaceScan.
    **/
    FaceTecFaceScanProcessor: typeof FaceTecFaceScanProcessor;
    /**
      * Developer created class for handling processing of the FaceTec SDK ID Scan.
    **/
    FaceTecIDScanProcessor: typeof FaceTecIDScanProcessor;
    /**
      * FaceTec SDK Logging Mode API.
    **/
    FaceTecLoggingMode: typeof FaceTecLoggingMode;
    /**
      * The Session status from the session that was just performed.
    **/
    FaceTecSessionStatus: typeof FaceTecSessionStatus;
    /**
      * The ID Scan status from the ID Scan that was just performed.
    **/
    FaceTecIDScanStatus: typeof FaceTecIDScanStatus;
    /**
      * Return Friendly names for the enums FaceTecIDScanStatus.
    **/
    getFriendlyDescriptionForFaceTecIDScanStatus: (enumValue: FaceTecIDScanStatus) => string;
    /**
      * FaceTecIDSCan retry front, back or both.
    **/
    FaceTecIDScanRetryMode: typeof FaceTecIDScanRetryMode;
    /**
      * FaceTec SDK ID Scan process behavior on starting.
      * Configure whether to show the ID Type Selection Screen, go directly to the ID Capture Screen for a specified ID type, or skip the entire ID Scan process and exit.
    **/
    FaceTecIDScanNextStep: typeof FaceTecIDScanNextStep;
    /**
      * An object of this type is passed back to the developer inside of FaceTecFaceScanProcessor.processSessionResultWhileFaceTecSDKWaits.
      * in order to control the FaceTec SDK UX flow based on the result of the processing of the FaceTec SDK 3D FaceScan.
    **/
    FaceTecFaceScanResultCallback: typeof FaceTecFaceScanResultCallback;
    /**
      * An object of this type is passed back to the developer inside of FaceTecIDScanProcessor.processSessionResultWhileFaceTecSDKWaits.
      * in order to control the FaceTec SDK UX flow based on the result of the processing of the FaceTec SDK 3D FaceScan.
    **/
    FaceTecIDScanResultCallback: typeof FaceTecIDScanResultCallback;
    /**
     * Return friendly names for the enums in FaceTecSessionStatus.
    **/
    getFriendlyDescriptionForFaceTecSessionStatus: (enumValue: FaceTecSessionStatus) => string;
    /**
     * Return the FaceTecSDK customization object.
     **/
    FaceTecCustomization: typeof FaceTecCustomization;
    /**
      * Return the FaceTecSDK oval customization object.
    **/
    FaceTecOvalCustomization: typeof FaceTecOvalCustomization;
    /**
      * Return the FaceTecSDK cancel button customization object.
    **/
    FaceTecCancelButtonCustomization: typeof FaceTecCancelButtonCustomization;
    /**
      * Return the FaceTecSDK exit animation customization object.
    **/
    FaceTecExitAnimationCustomization: typeof FaceTecExitAnimationCustomization;
    /**
      * Return the FaceTecSDK feedback customization object.
    **/
    FaceTecFeedbackBarCustomization: typeof FaceTecFeedbackBarCustomization;
    /**
      * Return the FaceTecSDK frame customization object.
    **/
    FaceTecFrameCustomization: typeof FaceTecFrameCustomization;
    /**
     * Return the FaceTecSDK session timer customization object.
   **/
    FaceTecSessionTimerCustomization: typeof FaceTecSessionTimerCustomization;
    /**
      * Return the FaceTecSDK Interface customization object.
    **/
    FaceTecOverlayCustomization: typeof FaceTecOverlayCustomization;
    /**
      * Return the FaceTecSDK Guidance customization object.
    **/
    FaceTecGuidanceCustomization: typeof FaceTecGuidanceCustomization;
    /**
      * Return the FaceTecSDK Result customization object.
    **/
    FaceTecResultScreenCustomization: typeof FaceTecResultScreenCustomization;
    /**
      * Return the FaceTecSDK Result customization object.
    **/
    FaceTecEnterFullScreenCustomization: typeof FaceTecEnterFullScreenCustomization;
    /**
      * Apply the specified customization parameters for FaceTec SDK.
    **/
    setCustomization: (customizationOrSecurityChangeOperation: FaceTecCustomization) => void;
    /**
      * Apply the specified customization parameters for FaceTec SDK to use when low light mode is active.
      * If not configured or set to nil, we will fallback to using the FaceTecCustomization instance configured with setCustomization(), or our default customizations if setCustomization() has not been called.
    **/
    setLowLightCustomization: (lowLightCustomization: FaceTecCustomization | null) => void;
    /**
      * Return the available FaceTecSDK exit animation styles.
    **/
    FaceTecExitAnimationStyle: typeof FaceTecExitAnimationStyle;
    /**
      * Return the available FaceTecSDK cancel button locations.
    **/
    FaceTecCancelButtonLocation: typeof FaceTecCancelButtonLocation;
    /**
      * Return the FaceTecSDK Security Watermark customization object.
     */
    FaceTecSecurityWatermarkCustomization: typeof FaceTecSecurityWatermarkCustomization;
    /**
     * Return the available FaceTecSDK Watermark image names.
     */
    FaceTecSecurityWatermarkImage: typeof FaceTecSecurityWatermarkImage;
    /**
     * Convenience method to get the time when a lockout will end.
     * This will be null if the user is not locked out.
    */
    getLockoutEndTime: () => number | null;
    /**
    * True if the user is locked out of FaceTec SDK.
    **/
    isLockedOut: () => boolean;
    /**
      * Get the available FaceTecSDK audit trail types.
    **/
    FaceTecAuditTrailType: typeof FaceTecAuditTrailType;
    /**
     * To be deprecated in a future release - replaced by configureLocalization.
    */
    configureLocalizationWithJSON: (localizationJSON: {
        [key: string]: string;
    }) => void;
    /**
     * API to set FaceTec Localization Strings with the strings passed in as arguments.
    */
    configureLocalization: (localizationJSON: {
        [key: string]: string;
    }) => void;
    /**
      * Set the desired FaceTecSDK audit trail behavior.
    **/
    auditTrailType: FaceTecAuditTrailType;
    /**
      * Unload FaceTecSDK and all its resources.
    **/
    unload: (callback: () => void) => void;
    /**
    *   Developer API to set logging mode to enumerated FaceTecLoggingMode.
    *   Default       - Log all important messages to the console.
    *   LocalhostOnly - Remove all logging except for when developing on Localhost.
    *   Usage Example - FaceTecSDK.setFaceTecLoggingMode(FaceTecSDK.FaceTecLoggingMode.LocalhostOnly).
    */
    setFaceTecLoggingMode: (enumValue: FaceTecLoggingMode) => void;
    /**
      * Return the current FaceTecSDK version.
    **/
    version: () => string;
    /**
      * Change the default location of the FaceTecSDK resources to be loaded. Default is "../FaceTecSDK.js/resources".
    **/
    setResourceDirectory: (directory: string) => void;
    /**
      * Change the default location of the FaceTec SDK images to be loaded. Default is "./FaceTec_images".
      * Images must all exist in one flat directory in the directory specified.
      * Please see the sample-custom-images-location for a working example of this API in action.
    **/
    setImagesDirectory: (directory: string) => void;
    /**
      * Create a FaceTec Rest API compatible User Agent string to be used in header element X-User-Agent when using FaceTec's Rest Api Services.
    **/
    createFaceTecAPIUserAgentString: (sessionID: string) => string;
};
