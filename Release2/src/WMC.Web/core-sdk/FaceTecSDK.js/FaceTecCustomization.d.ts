export declare enum FaceTecExitAnimationStyle {
    None = 0,
    RippleOut = 1,
    FadeOutMin = 2
}
export declare enum FaceTecCancelButtonLocation {
    Disabled = 0,
    TopLeft = 1,
    TopRight = 2,
    Custom = 3
}
export declare enum FaceTecSecurityWatermarkImage {
    FaceTec_ZoOm = 0,
    FaceTec = 1
}
export declare class FaceTecRect {
    x: number;
    y: number;
    width: number;
    height: number;
    constructor();
    create(x: number, y: number, width: number, height: number): this;
}
export interface FaceTecFeedbackBarCustomization {
    shadow: string;
    backgroundColor: string;
    cornerRadius: string;
    textFont: string;
    textSpacing: string;
    textSize: string;
    textColor: string;
    enablePulsatingText: boolean;
    relativeWidth: string;
}
export interface FaceTecInitialLoadingAnimationCustomization {
    customAnimation: SVGElement | null;
    animationRelativeScale: number;
    backgroundColor: string;
    foregroundColor: string;
    messageTextColor: string;
    messageFont: string;
    messageTextSpacing: string;
    messageTextSize: string;
}
export interface FaceTecFrameCustomization {
    shadow: string;
    backgroundColor: string;
    borderColor: string;
    borderWidth: string;
    borderCornerRadius: string;
}
export interface FaceTecCancelButtonCustomization {
    customImage: string;
    location: FaceTecCancelButtonLocation;
    customLocation: FaceTecRect;
}
export interface FaceTecExitAnimationCustomization {
    exitAnimationSuccess: FaceTecExitAnimationStyle;
    exitAnimationUnsuccess: FaceTecExitAnimationStyle;
}
export interface FaceTecSessionTimerCustomization {
    livenessCheckNoInteractionTimeout: number;
    idScanNoInteractionTimeout: number;
}
export interface FaceTecGuidanceCustomization {
    buttonFont: string;
    buttonTextSpacing: string;
    buttonTextSize: string;
    buttonBorderWidth: string;
    buttonBorderColor: string;
    buttonCornerRadius: string;
    buttonTextNormalColor: string;
    buttonTextHighlightColor: string;
    buttonTextDisabledColor: string;
    buttonBackgroundNormalColor: string;
    buttonBackgroundHighlightColor: string;
    buttonBackgroundDisabledColor: string;
    buttonRelativeWidth: string;
    buttonRelativeWidthOnDesktop: string;
    headerFont: string;
    headerTextSpacing: string;
    headerTextSize: string;
    subtextFont: string;
    subtextTextSpacing: string;
    subtextTextSize: string;
    readyScreenHeaderFont: string;
    readyScreenHeaderTextSpacing: string;
    readyScreenHeaderTextColor: string;
    readyScreenHeaderTextSize: string;
    readyScreenHeaderAttributedString: string;
    readyScreenSubtextFont: string;
    readyScreenSubtextTextSpacing: string;
    readyScreenSubtextTextColor: string;
    readyScreenSubtextTextSize: string;
    readyScreenSubtextAttributedString: string;
    retryScreenHeaderFont: string;
    retryScreenHeaderTextSpacing: string;
    retryScreenHeaderTextColor: string;
    retryScreenHeaderTextSize: string;
    retryScreenHeaderAttributedString: string;
    retryScreenSubtextFont: string;
    retryScreenSubtextTextSpacing: string;
    retryScreenSubtextTextColor: string;
    retryScreenSubtextTextSize: string;
    retryScreenSubtextAttributedString: string;
    backgroundColors: string;
    foregroundColor: string;
    readyScreenOvalFillColor: string;
    readyScreenTextBackgroundColor: string;
    readyScreenTextBackgroundCornerRadius: string;
    retryScreenIdealImage: string;
    retryScreenSlideshowImages: string[];
    retryScreenSlideshowInterval: string;
    enableRetryScreenSlideshowShuffle: boolean;
    retryScreenImageBorderColor: string;
    retryScreenImageBorderWidth: string;
    retryScreenImageCornerRadius: string;
    retryScreenOvalStrokeColor: string;
    enableRetryScreenBulletedInstructions: boolean;
    cameraPermissionsScreenImage: string;
}
export interface FaceTecEnterFullScreenCustomization {
    buttonFont: string;
    buttonTextSpacing: string;
    buttonBorderWidth: string;
    buttonBorderColor: string;
    buttonCornerRadius: string;
    buttonTextNormalColor: string;
    buttonTextHighlightColor: string;
    buttonTextDisabledColor: string;
    buttonBackgroundNormalColor: string;
    buttonBackgroundHighlightColor: string;
    buttonBackgroundDisabledColor: string;
    buttonRelativeWidth: string;
    buttonRelativeWidthOnDesktop: string;
    headerFont: string;
    headerTextSpacing: string;
    subtextFont: string;
    subtextTextSpacing: string;
    backgroundColors: string;
    foregroundColor: string;
    enterFullScreenImage: string;
}
export interface FaceTecOverlayCustomization {
    backgroundColor: string;
    showBrandingImage: boolean;
    brandingImage: string;
}
export interface FaceTecResultScreenCustomization {
    backgroundColors: string;
    foregroundColor: string;
    resultAnimationBackgroundColor: string;
    resultAnimationForegroundColor: string;
    resultAnimationSuccessBackgroundImage: string;
    resultAnimationUnsuccessBackgroundImage: string;
    customResultAnimationSuccess: SVGElement | null;
    customResultAnimationUnsuccess: SVGElement | null;
    messageFont: string;
    messageTextSpacing: string;
    messageTextSize: string;
    activityIndicatorColor: string;
    customActivityIndicatorImage: string;
    customActivityIndicatorRotationInterval: string;
    customActivityIndicatorAnimation: SVGElement | null;
    animationRelativeScale: number;
    showUploadProgressBar: boolean;
    uploadProgressFillColor: string;
    uploadProgressTrackColor: string;
}
export interface FaceTecOvalCustomization {
    /**
    * Color of the the FaceTec Browser SDK Oval outline.
    * Default is white.
    */
    strokeColor: string;
    /**
     * Color of the animated FaceTec Browser SDK Progress Spinner strokes.
     * Default is custom FaceTec Browser SDK color.
     */
    progressColor1: string;
    progressColor2: string;
    /**
     * Thickness of the animated FaceTec Browser SDK Progress Spinner strokes.
     * Default is dynamically configured per device at runtime.
     */
    progressStrokeWidth: number;
    /**
     * Thickness of the FaceTec Browser SDK Oval outline.
     * Default is dynamically configured per device at runtime.
     */
    strokeWidth: number;
}
export interface FaceTecIDScanCustomization {
    /**
    * Image displayed on the ID Scan Select ID Document page.
    * Default is configured to use image empty.
    */
    showSelectionScreenBrandingImage: boolean;
    /**
     * Color of the text displayed on the Identity Document Type Selection Screen (not including the action button text).
     * Default is off-black.
     */
    selectionScreenForegroundColor: string;
    /**
     * Font of the title during the Identity Document Type Selection Screen.
     * Accepts any value assignable to the fontFamily CSS attribute.
     */
    headerFont: string;
    /**
     * Spacing between the characters of the title's text during the Identity Document Type Selection Screen.
     * Accepts any value assignable to the letterSpacing CSS attribute.
     * Default is 'normal'.
     */
    headerTextSpacing: string;
    /**
     * Size of the title text during the Identity Document Type Selection Screen.
     * Accepts any value assignable to the fontSize CSS attribute.
     * Default is dynamically configured per device at runtime.
     */
    headerTextSize: string;
    /**
     * Font of the message text during the Identity Document Capture and Review Screens.
     * Accepts any value assignable to the fontFamily CSS attribute.
     */
    subtextFont: string;
    /**
     * Spacing between the characters of the title's subtext and messages during the Identity Document Capture and Review Screens.
     * Accepts any value assignable to the letterSpacing CSS attribute.
     * Default is 'normal'.
     */
    subtextTextSpacing: string;
    /**
     * Size of the message text during the Identity Document Capture and Review Screens.
     * Accepts any value assignable to the fontSize CSS attribute.
     * Default is dynamically configured per device at runtime.
     */
    subtextTextSize: string;
    /**
     * Font of the action button's text during the Photo ID Match Screens.
     * Accepts any value assignable to the fontFamily CSS attribute.
     */
    buttonFont: string;
    /**
     * Spacing between the characters of the action button's text during the Photo ID Match Screens.
     * Accepts any value assignable to the letterSpacing CSS attribute.
     * Default is 'normal'.
     */
    buttonTextSpacing: string;
    /**
     * Size of the action button's text during the Photo ID Match Screens.
     * Accepts any value assignable to the fontSize CSS attribute.
     * Default is dynamically configured per device at runtime.
     */
    buttonTextSize: string;
    /**
     * Thickness of the action button's border during the Photo ID Match Screens.
     * Default is dynamically configured per device at runtime.
     */
    buttonBorderWidth: string;
    /**
     * Color of the action button's border during the Photo ID Match Screens.
     * Default is transparent.
     */
    buttonBorderColor: string;
    /**
     * Corner radius of the action button's border during the Photo ID Match Screens.
     * Default is dynamically configured per device at runtime.
     */
    buttonCornerRadius: string;
    /**
     * Color of the action button's text during the Photo ID Match Screens.
     * Default is white.
     */
    buttonTextNormalColor: string;
    /**
     * Color of the action button's text when the button is pressed during the Photo ID Match Screens.
     * Default is white.
     */
    buttonTextHighlightColor: string;
    /**
   * Color of the action button's text when the button is disabled during the Photo ID Match Screens.
   * Default is white.
   */
    buttonTextDisabledColor: string;
    /**
     * Color of the action button's background during the Photo ID Match Screens.
     * Default is custom FaceTec Browser SDK color.
     */
    buttonBackgroundNormalColor: string;
    /**
     * Color of the action button's background when the button is pressed during the Photo ID Match Screens.
     * Default is custom FaceTec Browser SDK color.
     */
    buttonBackgroundHighlightColor: string;
    /**
     * Color of the action button's background when the button is disabled during the Photo ID Match Screens.
     * Default is custom FaceTec Browser SDK color.
     */
    buttonBackgroundDisabledColor: string;
    /**
     * Control the percent of the available FaceTec Browser SDK Frame width to use for the action button during the Photo ID Match Screens for mobile browsers.
     * Relative width percent is represented in decimal notation, ranging from 0.0 to 1.0.
     * If the value configured is equal to or greater than 1.0, the action button will be drawn to at max width within the FaceTec Browser SDK Frame.
     * If the value configured results in a width that is less than the action button's height, the action button's width will equal its height.
     * Note: The Identity Document Review Screen action buttons will be drawn at half the configured width.
     * Default is dynamically configured per device at runtime.
     */
    buttonRelativeWidth: string;
    /**
     * Control the percent of the available FaceTec Browser SDK Frame width to use for the action button during the Photo ID Match Screens for desktop browsers.
     * Relative width percent is represented in decimal notation, ranging from 0.0 to 1.0.
     * If the value configured is equal to or greater than 1.0, the action button will be drawn to at max width within the FaceTec Browser SDK Frame.
     * If the value configured results in a width that is less than the action button's height, the action button's width will equal its height.
     * Note: The Identity Document Review Screen action buttons will be drawn at half the configured width.
     * Default is dynamically configured per device at runtime.
     */
    buttonRelativeWidthOnDesktop: string;
    /**
     * Color of the Identity Document Type Selection Screen background.
     * Default is white.
     */
    selectionScreenBackgroundColors: string;
    /**
     * Image displayed on the Identity Document Type Selection Screen.
     * Default is configured to use image named 'FaceTec_branding_logo_id_check' located in '/FaceTec_images/' directory (or custom configured default directory for FaceTec Browser SDK images).
     */
    selectionScreenBrandingImage: string;
    /**
     * Color of the text displayed on the Identity Document Capture Screen (not including the action button text).
     * Default is white.
     */
    captureScreenForegroundColor: string;
    /**
     * Color of the text view background during the Identity Document Capture Screen.
     * Default is custom FaceTec Browser SDK color.
     */
    captureScreenTextBackgroundColor: string;
    /**
     * Color of the text view background border during the Identity Document Capture Screen.
     * Default is transparent.
     */
    captureScreenTextBackgroundBorderColor: string;
    /**
     * Thickness of the text view background border during the Identity Document Capture Screen.
     * Default is 0.
     */
    captureScreenTextBackgroundBorderWidth: string;
    /**
     * Corner radius of the text view background and border during the Identity Document Capture Screen.
     * Default is dynamically configured per device at runtime.
     */
    captureScreenTextBackgroundCornerRadius: string;
    /**
     * Color of the Identity Document Capture Screen's background.
     * Default is white.
     */
    captureScreenBackgroundColor: string;
    /**
     * Color of the Identity Document Capture Frame's stroke.
     * Default is custom FaceTec Browser SDK color.
     */
    captureFrameStrokeColor: string;
    /**
     * Thickness of the Identity Document Capture Frame's stroke.
     * Default is dynamically configured per device at runtime.
     */
    captureFrameStrokeWidth: string;
    /**
     * Corner radius of the Identity Document Capture Frame.
     * Default is dynamically configured per device at runtime.
     */
    captureFrameCornerRadius: string;
    /**
     * Color of the text displayed on the Identity Document Review Screen (not including the action button text).
     * Default is white.
     */
    reviewScreenForegroundColor: string;
    /**
     * Color of the text view background during the Identity Document Review Screen.
     * Default is custom FaceTec Browser SDK color.
     */
    reviewScreenTextBackgroundColor: string;
    /**
     * Color of the text view background border during the Identity Document Review Screen.
     * Default is transparent.
     */
    reviewScreenTextBackgroundBorderColor: string;
    /**
     * Thickness of the text view background border during the Identity Document Review Screen.
     * Default is 0.
     */
    reviewScreenTextBackgroundBorderWidth: string;
    /**
     * Corner radius of the text view background and border during the Identity Document Review Screen.
     * Default is dynamically configured per device at runtime.
     */
    reviewScreenTextBackgroundBorderCornerRadius: string;
    /**
     * Corner radius of the ID Document Preview image displayed on the Identity Document Review Screen.
     * Default is dynamically configured per device at runtime.
     */
    reviewScreenDocumentPreviewCornerRadius: string;
    /**
     * Color of the Identity Document Review Screen background.
     * Default is white.
     */
    reviewScreenBackgroundColors: string;
    /**
     * Image displayed below the FaceTec Browser SDK Frame during Photo ID Match when the Identity Document Type selected is an ID Card.
     * This image acts as a placeholder to show a status of incomplete for capturing the ID Card's front side.
     * This only applies to desktop browsers.
     * Default is configured to use image named 'FaceTec_id_card_placeholder_front' located in '/FaceTec_images/' directory (or custom configured default directory for FaceTec Browser SDK images).
     */
    captureScreenIDFrontPlaceHolderImage: string;
    /**
     * Image displayed below the FaceTec Browser SDK Frame during Photo ID Match when the Identity Document Type selected is an ID Card.
     * This image acts as a placeholder to show a status of incomplete for capturing the ID Card's back side.
     * This only applies to desktop browsers.
     * Default is configured to use image named 'FaceTec_id_card_placeholder_back' located in '/FaceTec_images/' directory (or custom configured default directory for FaceTec Browser SDK images).
     */
    captureScreenIDBackPlaceHolderImage: string;
    /**
     * Image displayed below the FaceTec Browser SDK Frame during Photo ID Match when the Identity Document Type selected is a Passport.
     * This image acts as a placeholder to show a status of incomplete for capturing the Passport.
     * This only applies to desktop browsers.
     * Default is configured to use image named 'FaceTec_passport_placeholder' located in '/FaceTec_images/' directory (or custom configured default directory for FaceTec Browser SDK images).
     */
    captureScreenPassportPlaceholderImage: string;
    /**
     * Image displayed below the FaceTec Browser SDK Frame during Photo ID Match when the Identity Document Type selected is an ID Card.
     * This image acts as a placeholder to show a status of complete for capturing the ID Card's front side.
     * This only applies to desktop browsers.
     * Default is configured to use image named 'FaceTec_id_front_checkmark' located in '/FaceTec_images/' directory (or custom configured default directory for FaceTec Browser SDK images).
     */
    captureScreenIDFrontCheckmarkImage: string;
    /**
     * Image displayed below the FaceTec Browser SDK Frame during Photo ID Match when the Identity Document Type selected is an ID Card.
     * This image acts as a placeholder to show a status of complete for capturing the ID Card's back side.
     * This only applies to desktop browsers.
     * Default is configured to use image named 'FaceTec_id_back_checkmark' located in '/FaceTec_images/' directory (or custom configured default directory for FaceTec Browser SDK images).
     */
    captureScreenIDBackCheckmarkImage: string;
    /**
     * Image displayed below the FaceTec Browser SDK Frame during Photo ID Match when the Identity Document Type selected is a Passport.
     * This image acts as a placeholder to show a status of complete for capturing the Passport.
     * This only applies to desktop browsers.
     * Default is configured to use image named 'FaceTec_passport_checkmark' located in '/FaceTec_images/' directory (or custom configured default directory for FaceTec Browser SDK images).
     */
    captureScreenPassportCheckmarkImage: string;
}
export interface FaceTecSecurityWatermarkCustomization {
    securityWatermarkImage: FaceTecSecurityWatermarkImage;
}
/**
 * Class used to customize the look and feel of the FaceTec Browser SDK Interface.
 * FaceTec Browser SDK ships with a default FaceTec Browser SDK theme but has a variety of variables that you can use to configure the FaceTec Browser SDK to your application's needs.
 * To customize the FaceTec Browser SDK Interface, simply create an instance of FaceTecCustomization and set some, or all, of the variables.
 */
export declare class FaceTecCustomization {
    /** Customize the FaceTec Browser SDK Oval and the FaceTec Browser SDK Progress Spinner animations. */
    ovalCustomization: FaceTecOvalCustomization;
    /**  Customize the FaceTec Browser SDK Feedback Bar. */
    feedbackCustomization: FaceTecFeedbackBarCustomization;
    /** Customize the FaceTec Browser SDK Frame. */
    frameCustomization: FaceTecFrameCustomization;
    /** Customize the FaceTec Browser SDK Frame exit animation. */
    exitAnimationCustomization: FaceTecExitAnimationCustomization;
    /** Customize the FaceTec Browser SDK Cancel Button. */
    cancelButtonCustomization: FaceTecCancelButtonCustomization;
    /** Customize the time after which the FaceTec Browser SDK Session should timeout. */
    sessionTimerCustomization: FaceTecSessionTimerCustomization;
    /** Customize the loading spinner and the text shown to the user while the camera loads. */
    initialLoadingAnimationCustomization: FaceTecInitialLoadingAnimationCustomization;
    /** Customize the New User Guidance and Retry Screens. */
    guidanceCustomization: FaceTecGuidanceCustomization;
    /** Customize the FaceTec Browser SDK Overlay, separating the FaceTec Browser SDK Interface from the presenting application context. */
    overlayCustomization: FaceTecOverlayCustomization;
    /** Customize the Result Screen. */
    resultScreenCustomization: FaceTecResultScreenCustomization;
    /** Customize the FaceTec Browser SDK Photo ID Match Screens. */
    idScanCustomization: FaceTecIDScanCustomization;
    /** Customize the FaceTec Browser SDK IFrame Enter Full Screen User Interface.
     * This customization is only active when the FaceTec Browser SDK is loaded in an IFrame.
     * Permission from FaceTec is required to run the FaceTec Browser SDK in an iFrame.
     * Please contact FaceTec to request access.
     */
    enterFullScreenCustomization: FaceTecEnterFullScreenCustomization;
    /** Customize the FaceTec Security Watermark Image by selecting from 1 of the 3 available FaceTec watermarks. */
    securityWatermarkCustomization: FaceTecSecurityWatermarkCustomization;
    /** Allow FaceTec Browser SDK to cancel due to context switch when unauthorized key strokes are detected while the FaceTec Browser SDK is active. For accessibility, essential navigation hotkeys will not trigger cancellation. */
    enableHotKeyProtection: boolean;
    /** Show Camera Permissions Denied Screen. */
    enableCameraPermissionsHelpScreen: boolean;
    /** Force the oval stroke to be drawn as opaque. */
    shouldDrawOvalStrokeOpaque: boolean;
    /** Control whether to enable clickable Ready Screen subtext region, cancelling from the session and returning FaceTecSessionStatus.UserCancelledViaClickableReadyScreenSubtext. */
    enableClickableReadyScreenSubtext: boolean;
    /** For non-production instances, display the clickable Development Mode Tag link during the Result Screen. */
    enableDevelopmentModeTag: boolean;
    /**
     * This function allows special runtime control of the success message shown when the success animation occurs.
     * Please note that you can also customize this string via the standard customization/localization methods provided by the FaceTec Browser SDK.
     * Special runtime access is enabled to this text because the developer may wish to change this text depending on the FaceTec Browser SDK's mode of operation.
     * Default is in the customizable localization string "FaceTec_result_success_message".
     */
    static setOverrideResultScreenSuccessMessage: (message: string) => void;
    /**
     * Constructor for FaceTecCustomization object.
     *
     * @param keyValuePairs - The FaceTec Browser SDK Feature Flag key-value pairs for restricted customization access.
     */
    constructor(keyValuePairs?: {
        key: string;
    }[] | any[]);
    [key: string]: {
        key: string;
    }[] | boolean | FaceTecSecurityWatermarkCustomization | FaceTecEnterFullScreenCustomization | FaceTecIDScanCustomization | FaceTecOvalCustomization | FaceTecFeedbackBarCustomization | FaceTecFrameCustomization | FaceTecExitAnimationCustomization | FaceTecCancelButtonCustomization | FaceTecSessionTimerCustomization | FaceTecInitialLoadingAnimationCustomization | FaceTecGuidanceCustomization | FaceTecOverlayCustomization | FaceTecResultScreenCustomization | string;
}
/**
 * Customize the time after which the Session should timeout.
 */
export declare class FaceTecSessionTimerCustomization implements FaceTecSessionTimerCustomization {
    maxTimeBeforeCameraPermissionsError: number;
    /**
     * The amount of seconds until the Liveness Check portion of the Session will be cancelled if there is no user interaction.
     * If a user presses a button or performs a Liveness Check, the timer that tracks this timeout is reset.
     * This value has to be between 40 and 60 (seconds). If it’s lower than 40 or higher than 60, it will be defaulted to 40 or 60 respectively.
     */
    livenessCheckNoInteractionTimeout: number;
    /**
     * The amount of seconds until the ID Scan portion of the Session will be cancelled if there is no user interaction.
     * If a user presses a button or performs a scan, the timer that tracks this timeout is reset.
     * This value has to be between 60 and 180 (seconds). If it’s lower than 60 or higher than 180, it will be defaulted to 60 or 180 respectively.
     * The default value is 120 (seconds).
     */
    idScanNoInteractionTimeout: number;
    /** Constructor for FaceTecSessionTimerCustomization object. */
    constructor();
}
/**
 * Customize the FaceTec Session Frame exit animation.
 */
export declare class FaceTecExitAnimationCustomization implements FaceTecExitAnimationCustomization {
    /** Customize the transition out animation for a successful FaceTec Browser SDK Session. */
    exitAnimationSuccess: FaceTecExitAnimationStyle;
    /** Customize the transition out animation for an unsuccessful FaceTec Browser SDK Session. */
    exitAnimationUnsuccess: FaceTecExitAnimationStyle;
    /** Constructor for FaceTecExitAnimationCustomization object. */
    constructor();
}
/**
 * Customize the FaceTec Session Oval and the FaceTec Session Progress Spinner animations.
 */
export declare class FaceTecOvalCustomization implements FaceTecOvalCustomization {
    /**
    * Color of the the FaceTec Browser SDK Oval outline.
    * Default is white.
    */
    strokeColor: string;
    /**
     * Color of the animated FaceTec Browser SDK Progress Spinner strokes.
     * Default is custom FaceTec Browser SDK color.
     */
    progressColor1: string;
    progressColor2: string;
    /**
     * Thickness of the animated FaceTec Browser SDK Progress Spinner strokes.
     * Default is dynamically configured per device at runtime.
     */
    progressStrokeWidth: number;
    /**
     * Thickness of the FaceTec Browser SDK Oval outline.
     * Default is dynamically configured per device at runtime.
     */
    strokeWidth: number;
    /** Constructor for FaceTecOvalCustomization object. */
    constructor();
}
/**
 * Customize the FaceTec Session Frame.
 */
export declare class FaceTecFrameCustomization implements FaceTecFrameCustomization {
    /**
     * Shadow displayed behind the FaceTec Browser SDK Frame.
     * This accepts box-shadow css attribute string values.
     * Default is none.
     */
    shadow: string;
    /**
     * Color of the FaceTec Browser SDK Frame's border.
     * Default is white.
     */
    borderColor: string;
    /**
     * Corner radius of the FaceTec Browser SDK Frame.
     * Default is dynamically configured per device at runtime.
     */
    borderCornerRadius: string;
    /**
     * Thickness of the FaceTec Browser SDK Frame's border.
     * Default is dynamically configured per device at runtime.
     */
    borderWidth: string;
    /**
     * Color of the background surrounding the oval outline during FaceTec Browser SDK session.
     * Default is custom FaceTec Browser SDK color.
     */
    backgroundColor: string;
    /** Constructor for FaceTecFrameCustomization object. */
    constructor();
}
/**
 * Customize the FaceTec Browser SDK Cancel Button.
 * Shown during FaceTec Browser SDK session, New User Guidance, Retry, and Photo ID Match Screens.
 */
export declare class FaceTecCancelButtonCustomization implements FaceTecCancelButtonCustomization {
    /**
     * Location, or use, of the FaceTec Browser SDK Cancel Button.
     * Default is FaceTecCancelButtonLocation.TopLeft.
     */
    location: FaceTecCancelButtonLocation;
    /**
     * The size and location of the cancel button within the current screen's bounds.
     * Configure using the convenience method .setCustomLocation(x:number, y:number, width:number, height:number).
     * Note: In order to use a custom-located cancel button, you MUST set .location to the enum value FaceTecCancelButtonLocation.Custom.
     * Default is set at origin 0,0 with a size of 0 by 0.
     */
    customLocation: FaceTecRect;
    /**
     * Image displayed on the FaceTec Browser SDK Cancel Button.
     * Default is configured to use image named 'FaceTec_cancel' located in '/FaceTec_images/' directory (or custom configured default directory for FaceTec Browser SDK images).
     */
    customImage: string;
    /** Constructor for FaceTecCancelButtonCustomization object. */
    constructor();
    /**
     * Set the size and location of the cancel button within the current screen's bounds.
     * Note: In order to use a custom-located cancel button, you MUST set .location to the enum value FaceTecCancelButtonLocation.Custom.
     */
    setCustomLocation(x: number, y: number, width: number, height: number): void;
}
/**
 * Customize the FaceTec Session Feedback Bar.
 */
export declare class FaceTecFeedbackBarCustomization implements FaceTecFeedbackBarCustomization {
    /**
     * Color of the FaceTec Browser SDK Feedback Bar's background. Recommend making this have some transparency.
     * Default is custom FaceTec Browser SDK color.
     */
    backgroundColor: string;
    /**
     * Color of the text displayed within the FaceTec Browser SDK Feedback Bar.
     * Default is white.
     */
    textColor: string;
    /**
     * Font of the text displayed within the FaceTec Browser SDK Feedback Bar.
     * Accepts any value assignable to the fontFamily CSS attribute.
     */
    textFont: string;
    /**
     * Spacing between the characters of the text displayed within the FaceTec Browser SDK Feedback Bar.
     * Accepts any value assignable to the letterSpacing CSS attribute.
     * Default is 'normal'.
     */
    textSpacing: string;
    /**
     * Size of the text displayed within the FaceTec Browser SDK Feedback Bar.
     * Accepts any value assignable to the fontSize CSS attribute.
     * Default is dynamically configured per device at runtime.
     */
    textSize: string;
    /**
     * Corner radius of the FaceTec Browser SDK Feedback Bar.
     * Default is dynamically configured per device at runtime.
     */
    cornerRadius: string;
    /**
     * Shadow displayed behind the FaceTec Browser SDK Feedback Bar.
     * This accepts box-shadow css attribute string values.
     * Default is a custom sized black shadow.
     */
    shadow: string;
    /**
     * Control whether to enable the pulsating-text animation within the FaceTec Browser SDK Feedback Bar.
     * Default is true (enabled).
     */
    enablePulsatingText: boolean;
    /**
     * Control the percent of the available FaceTec Browser SDK Frame width to use for the FaceTec Browser SDK Feedback Bar's width on desktop browsers.
     * Relative width percent is represented in decimal notation, ranging from 0.0 to 1.0.
     * If the value configured is equal to or greater than 1.0, the FaceTec Browser SDK Feedback Bar will be drawn to at max width within the FaceTec Browser SDK Frame.
     * If the value configured results in a width that is less than the minimum width, which is 2x the FaceTec Browser SDK Feedback Bar's height, then the FaceTec Browser SDK Feedback Bar's width will be set at the minimum.
     * Default is dynamically configured per device at runtime.
     */
    relativeWidthOnDesktop: string;
    /**
     * Control the percent of the available FaceTec Browser SDK Frame width to use for the FaceTec Browser SDK Feedback Bar's width on mobile browsers.
     * Relative width percent is represented in decimal notation, ranging from 0.0 to 1.0.
     * If the value configured is equal to or greater than 1.0, the FaceTec Browser SDK Feedback Bar will be drawn to at max width within the FaceTec Browser SDK Frame.
     * If the value configured results in a width that is less than the minimum width, which is 2x the FaceTec Browser SDK Feedback Bar's height, then the FaceTec Browser SDK Feedback Bar's width will be set at the minimum.
     * Default is dynamically configured per device at runtime.
     */
    relativeWidth: string;
    /** Constructor for FaceTecFeedbackBarCustomization object. */
    constructor();
}
/**
 * Customize the loading spinner and the text shown to the user while the camera loads.
 */
export declare class FaceTecInitialLoadingAnimationCustomization implements FaceTecInitialLoadingAnimationCustomization {
    /**
     * Control the size of the animation displayed while the camera is loading.
     * This value represents the scaling factor that will be applied to the default animation bounds.
     * This value has to be between 0.5 and 2.0. If it’s lower than 0.5 or higher than 2.0, it will be defaulted to 0.5 or 2.0 respectively.
     * Default value is 1.
     */
    animationRelativeScale: number;
    /**
     * Configure an SVG element to display while the camera is loading.
     * Note: All CSS required for the SVG animation needs to be included in your app's CSS file before launching FaceTec Browser SDK.
     * Note: The activity indicator animation is displayed indefinitely while the camera loads, so the custom animation should be set to loop/repeat infinitely.
     * If this is set to an SVGElement object, the SVG supplied will be used instead of the default loading spinner animation.
     * If this is null, default loading spinner will be used.
     * Default is null.
     */
    customAnimation: SVGElement | null;
    /**
     * Color of the loading spinner background track shown to the user while the camera loads.
     * Default is off-white.
     */
    backgroundColor: string;
    /**
     * Color of the loading spinner foreground fill and message text shown to the user while the camera loads.
     * If this value is an empty string, FaceTecOverlayCustomization.foregroundColor will be used for the color of the loading spinner foreground fill and message text.
     * Default is custom FaceTec Browser SDK color.
     */
    foregroundColor: string;
    /**
     * Color of the message text shown to the user while the camera loads.
     * Note: This will override the text color configured with FaceTecOverlayCustomization.foregroundColor or FaceTecInitialLoadingAnimationCustomization.foregroundColor.
     * If this value is an empty string, FaceTecInitialLoadingAnimationCustomization.foregroundColor will be used for the color of the message text.
     * Default is an empty string.
     */
    messageTextColor: string;
    /**
     * Font of the message text shown to the user while the camera loads.
     * Accepts any value assignable to the fontFamily CSS attribute.
     */
    messageFont: string;
    /**
     * Spacing between the characters of the message text shown to the user while the camera loads.
     * Accepts any value assignable to the letterSpacing CSS attribute.
     * Default is 'normal'.
     */
    messageTextSpacing: string;
    /**
     * Size of the message text shown to the user while the camera loads.
     * Accepts any value assignable to the fontSize CSS attribute.
     * Default is dynamically configured per device at runtime.
     */
    messageTextSize: string;
    /** Constructor for FaceTecInitialLoadingAnimationCustomization object. */
    constructor();
}
/**
 * Customize the New User Guidance and Retry Screens.
 * New User Guidance Screens are shown before the FaceTec Browser SDK Session and Retry Screens are shown after an unsuccessful FaceTec Browser SDK Session.
 */
export declare class FaceTecGuidanceCustomization implements FaceTecGuidanceCustomization {
    private defaultLocationForImages;
    /**
     * Thickness of the action button's border during the New User Guidance and Retry Screens.
     * Default is dynamically configured per device at runtime.
     */
    buttonBorderWidth: string;
    /**
     * Color of the action button's border during the New User Guidance and Retry Screens.
     * Default is transparent.
     */
    buttonBorderColor: string;
    /**
     * Corner radius of the action button's border during the New User Guidance and Retry Screens.
     * Default is dynamically configured per device at runtime.
     */
    buttonCornerRadius: string;
    /**
     * Color of the action button's text during the New User Guidance and Retry Screens.
     * Default is white.
     */
    buttonTextNormalColor: string;
    /**
     * Color of the action button's text when the button is pressed during the New User Guidance and Retry Screens.
     * Default is white.
     */
    buttonTextHighlightColor: string;
    /**
     * Color of the action button's text when the button is disabled during the New User Guidance and Retry Screens.
     * Default is white.
     */
    buttonTextDisabledColor: string;
    /**
     * Color of the action button's background during the New User Guidance and Retry Screens.
     * Default is custom FaceTec Browser SDK color.
     */
    buttonBackgroundNormalColor: string;
    /**
     * Color of the action button's background when the button is pressed during the New User Guidance and Retry Screens.
     * Default is custom FaceTec Browser SDK color.
     */
    buttonBackgroundHighlightColor: string;
    /**
     * Color of the action button's background when the button is disabled during the New User Guidance and Retry Screens.
     * Default is custom FaceTec Browser SDK color.
     */
    buttonBackgroundDisabledColor: string;
    /**
     * Font of the title's text during the New User Guidance and Retry Screens.
     * Note: This customization can be overridden for specific text using FaceTecGuidanceCustomization.readyScreenHeaderFont and/or .retryScreenHeaderFont.
     * Accepts any value assignable to the fontFamily CSS attribute.
     */
    headerFont: string;
    /**
     * Spacing between the characters of the title's text during the New User Guidance and Retry Screens.
     * Note: This customization can be overridden for specific text using FaceTecGuidanceCustomization.readyScreenHeaderTextSpacing and/or .retryScreenHeaderTextSpacing.
     * Accepts any value assignable to the letterSpacing CSS attribute.
     * Default is 'normal'.
     */
    headerTextSpacing: string;
    /**
     * Size of the title's text during the New User Guidance and Retry Screens.
     * Note: This customization can be overridden for specific text using FaceTecGuidanceCustomization.readyScreenHeaderTextSize and/or .retryScreenHeaderTextSize.
     * Accepts any value assignable to the fontSize CSS attribute.
     * Default is dynamically configured per device at runtime.
     */
    headerTextSize: string;
    /**
     * Font of the title's subtext and messages during the New User Guidance and Retry Screens.
     * Note: This customization can be overridden for specific text using FaceTecGuidanceCustomization.readyScreenSubtextFont and/or .retryScreenSubtextFont.
     * Accepts any value assignable to the fontFamily CSS attribute.
     */
    subtextFont: string;
    /**
     * Spacing between the characters of the title's subtext and messages during the New User Guidance and Retry Screens.
     * Note: This customization can be overridden for specific text using FaceTecGuidanceCustomization.readyScreenSubtextTextSpacing and/or .retryScreenSubtextTextSpacing.
     * Accepts any value assignable to the letterSpacing CSS attribute.
     * Default is 'normal'.
     */
    subtextTextSpacing: string;
    /**
     * Size of the title's subtext and messages during the New User Guidance and Retry Screens.
     * Note: This customization can be overridden for specific text using FaceTecGuidanceCustomization.readyScreenSubtextTextSize and/or .retryScreenSubtextTextSize.
     * Accepts any value assignable to the fontSize CSS attribute.
     * Default is dynamically configured per device at runtime.
     */
    subtextTextSize: string;
    /**
     * Font of the title's text displayed on the Get Ready To FaceTec Browser SDK Screen during the New User Guidance and Retry Screens.
     * Note: This will override the header font configured with FaceTecGuidanceCustomization.headerFont for the Get Ready To FaceTec Browser SDK Screen.
     * Accepts any value assignable to the fontFamily CSS attribute.
     * If this value is an empty string, FaceTecGuidanceCustomization.headerFont will be used for the font of the title's text displayed on the Get Ready To FaceTec Browser SDK Screen.
     * Default value is an empty string.
     */
    readyScreenHeaderFont: string;
    /**
     * Spacing between the characters of the title's text displayed on the Get Ready To FaceTec Browser SDK Screen during the New User Guidance and Retry Screens.
     * Accepts any value assignable to the letterSpacing CSS attribute.
     * Note: This will override the header text spacing configured with FaceTecGuidanceCustomization.headerTextSpacing for the Get Ready To FaceTec Browser SDK Screen.
     * If this value is an empty string, FaceTecGuidanceCustomization.headerTextSpacing will be used for the character spacing of the title's text displayed on the Get Ready To FaceTec Browser SDK Screen.
     * Default value is an empty string.
     */
    readyScreenHeaderTextSpacing: string;
    /**
     * Color of the header text displayed on the Get Ready To FaceTec Browser SDK Screen during the New User Guidance and Retry Screens.
     * Note: This will override the header text color configured with FaceTecGuidanceCustomization.foregroundColor for the Get Ready To FaceTec Browser SDK Screen.
     * If this value is an empty string, FaceTecGuidanceCustomization.foregroundColor will be used for the color of the title's text displayed on the Get Ready To FaceTec Browser SDK Screen.
     * Default value is an empty string.
     */
    readyScreenHeaderTextColor: string;
    /**
     * Size the title's text displayed on the Get Ready To FaceTec Browser SDK Screen during the New User Guidance and Retry Screens.
     * Accepts any value assignable to the fontSize CSS attribute.
     * Note: This will override the header text size configured with FaceTecGuidanceCustomization.headerTextSize for the Get Ready for Video Sel Screen.
     * If this value is an empty string, FaceTecGuidanceCustomization.headerTextSize will be used for the size of the title's text displayed on the Get Ready To FaceTec Browser SDK Screen.
     * Default value is an empty string.
     */
    readyScreenHeaderTextSize: string;
    /**
     * Specify an html-attributed string to use instead of the localized string for the text of the title displayed on the Get Ready To FaceTec Browser SDK Screen during the New User Guidance and Retry Screens.
     * If this value is an empty string, the localized string, FaceTec_instructions_header_ready, will be used for the text of the title displayed on the Get Ready To FaceTec Browser SDK Screen during the New User Guidance and Retry Screens.
     * Default is an empty string.
     */
    readyScreenHeaderAttributedString: string;
    /**
     * Font of the title's subtext displayed on the Get Ready for Video Selfie Screen during the New User Guidance and Retry Screens.
     * Note: This will override the title's subtext font configured with FaceTecGuidanceCustomization.subtextFont for the Get Ready for Video Selfie Screen.
     * Accepts any value assignable to the fontFamily CSS attribute.
     * If this value is an empty string, FaceTecGuidanceCustomization.subtextFont will be used for the font of the title's subtext displayed on the Get Ready for Video Selfie Screen.
     * Default value is an empty string.
     */
    readyScreenSubtextFont: string;
    /**
     * Spacing between the characters of the title's subtext displayed on the Get Ready for Video Selfie Screen during the New User Guidance and Retry Screens.
     * Accepts any value assignable to the letterSpacing CSS attribute.
     * Note: This will override the subtext text spacing configured with FaceTecGuidanceCustomization.subtextTextSpacing for the Get Ready for Video Selfie Screen.
     * If this value is an empty string, FaceTecGuidanceCustomization.subtextTextSpacing will be used for the character spacing of the title's subtext displayed on the Get Ready for Video Selfie Screen.
     * Default value is an empty string.
     */
    readyScreenSubtextTextSpacing: string;
    /**
     * Color of the title's subtext displayed on the Get Ready for Video Selfie Screen during the New User Guidance and Retry Screens.
     * Note: This will override the title's subtext color configured with FaceTecGuidanceCustomization.foregroundColor for the Get Ready for Video Selfie Screen.
     * If this value is an empty string, FaceTecGuidanceCustomization.foregroundColor will be used for the color of the title's subtext displayed on the Get Ready for Video Selfie Screen.
     * Default value is an empty string.
     */
    readyScreenSubtextTextColor: string;
    /**
     * Size of the title's subtext displayed on the Get Ready for Video Selfie Screen during the New User Guidance and Retry Screens.
     * Accepts any value assignable to the fontSize CSS attribute.
     * Note: This will override the subtext text size configured with FaceTecGuidanceCustomization.subtextTextSize for the Get Ready for Video Selfie Screen.
     * If this value is an empty string, FaceTecGuidanceCustomization.subtextTextSize will be used for the size of the title's subtext displayed on the Get Ready for Video Selfie Screen.
     * Default value is an empty string.
     */
    readyScreenSubtextTextSize: string;
    /**
     * Specify an html-attributed string to use instead of the localized string for the text of the title's subtext displayed on the Get Ready for Video Selfie Screen during the New User Guidance and Retry Screens.
     * If this value is an empty string, the localized string, FaceTec_instructions_message_ready, will be used for the text of the title's subtext displayed on the Get Ready for Video Selfie Screen during the New User Guidance and Retry Screens.
     * Default is an empty string.
     */
    readyScreenSubtextAttributedString: string;
    /**
     * Font of the title's text displayed on the first Retry Screen.
     * Note: This will override the header font configured with FaceTecGuidanceCustomization.headerFont for the first Retry Screen.
     * Accepts any value assignable to the fontFamily CSS attribute.
     * If this value is an empty string, FaceTecGuidanceCustomization.headerFont will be used for the font of the title's text displayed on the first Retry Screen.
     * Default value is an empty string.
     */
    retryScreenHeaderFont: string;
    /**
     * Spacing between the characters of the title's text displayed on the first Retry Screen.
     * Accepts any value assignable to the letterSpacing CSS attribute.
     * Note: This will override the header text spacing configured with FaceTecGuidanceCustomization.headerTextSpacing for the first Retry Screen.
     * If this value is an empty string, FaceTecGuidanceCustomization.headerTextSpacing will be used for the character spacing of the title's text displayed on the first Retry Screen.
     * Default value is an empty string.
     */
    retryScreenHeaderTextSpacing: string;
    /**
     * Color of the header text displayed on the first Retry Screen.
     * Note: This will override the header text color configured with FaceTecGuidanceCustomization.foregroundColor for the first Retry Screen.
     * If this value is an empty string, FaceTecGuidanceCustomization.foregroundColor will be used for the color of the title's text displayed on the first Retry Screen.
     * Default value is an empty string.
     */
    retryScreenHeaderTextColor: string;
    /**
     * Size of the title's text displayed on the first Retry Screen.
     * Accepts any value assignable to the fontSize CSS attribute.
     * Note: This will override the header text size configured with FaceTecGuidanceCustomization.headerTextSize for the first Retry Screen.
     * If this value is an empty string, FaceTecGuidanceCustomization.headerTextSize will be used for the size of the title's text displayed on the first Retry Screen.
     * Default value is an empty string.
     */
    retryScreenHeaderTextSize: string;
    /**
     * Specify an html-attributed string to use instead of the localized string for the text of the title displayed on the first Retry Screen.
     * If this value is an empty string, the localized string, FaceTec_retry_header, will be used for the text of the title displayed on the first Retry Screen.
     * Default is an empty string.
     */
    retryScreenHeaderAttributedString: string;
    /**
     * Font of the title's subtext and messages displayed on the first Retry Screen.
     * Note: This will override the font of the title's subtext and messages configured with FaceTecGuidanceCustomization.subtextFont for the first Retry Screen.
     * Accepts any value assignable to the fontFamily CSS attribute.
     * If this value is an empty string, FaceTecGuidanceCustomization.subtextFont will be used for the font of the title's subtext and messages displayed on the first Retry Screen.
     * Default value is an empty string.
     */
    retryScreenSubtextFont: string;
    /**
     * Spacing between the characters of the title's subtext and messages displayed on the first Retry Screen.
     * Accepts any value assignable to the letterSpacing CSS attribute.
     * Note: This will override the subtext and message text spacing configured with FaceTecGuidanceCustomization.subtextTextSpacing for the first Retry Screen.
     * If this value is an empty string, FaceTecGuidanceCustomization.subtextTextSpacing will be used for the character spacing of the title's subtext and messages displayed on the first Retry Screen.
     * Default value is an empty string.
     */
    retryScreenSubtextTextSpacing: string;
    /**
     * Color of the title's subtext and messages displayed on the first Retry Screen.
     * Note: This will override the title's subtext and message color configured with FaceTecGuidanceCustomization.foregroundColor for the first Retry Screen.
     * If this value is an empty string, FaceTecGuidanceCustomization.foregroundColor will be used for the color of the title's subtext displayed on the first Retry Screen.
     * Default value is an empty string.
     */
    retryScreenSubtextTextColor: string;
    /**
     * Size of the title's subtext and messages displayed on the first Retry Screen.
     * Accepts any value assignable to the fontSize CSS attribute.
     * Note: This will override the subtext and message text size configured with FaceTecGuidanceCustomization.subtextTextSize for the first Retry Screen.
     * If this value is an empty string, FaceTecGuidanceCustomization.subtextTextSpacing will be used for the size of the title's subtext and messages displayed on the first Retry Screen.
     * Default value is an empty string.
     */
    retryScreenSubtextTextSize: string;
    /**
     * Specify an html-attributed string to use instead of the localized string for the text of the title's subtext displayed on the first Retry Screen.
     * If this value is an empty string, the localized strings, FaceTec_retry_subheader_message, will be used for the text of the title's subtext displayed on the first Retry Screen.
     * Default is an empty string.
     */
    retryScreenSubtextAttributedString: string;
    /**
     * Font of the action button's text during the New User Guidance and Retry Screens.
     * Accepts any value assignable to the fontFamily CSS attribute.
     * Default is a bold system font.
     */
    buttonFont: string;
    /**
     * Spacing between the characters of the action button's text during the New User Guidance and Retry Screens.
     * Accepts any value assignable to the letterSpacing CSS attribute.
     * Default is 'normal'.
     */
    buttonTextSpacing: string;
    /**
     * Size of the action button's text during the New User Guidance and Retry Screens.
     * Accepts any value assignable to the fontSize CSS attribute.
     * Default is dynamically configured per device at runtime.
     */
    buttonTextSize: string;
    /**
     * Control the percent of the available FaceTec Browser SDK Frame width to use for the action button during the New User Guidance and Retry Screens for mobile browsers.
     * Relative width percent is represented in decimal notation, ranging from 0.0 to 1.0.
     * If the value configured is equal to or greater than 1.0, the action button will be drawn to at max width within the FaceTec Browser SDK Frame.
     * If the value configured results in a width that is less than the action button's height, the action button's width will equal its height.
     * Default is dynamically configured per device at runtime.
     */
    buttonRelativeWidth: string;
    /**
     * Control the percent of the available FaceTec Browser SDK Frame width to use for the action button during the New User Guidance and Retry Screens for desktop browsers.
     * Relative width percent is represented in decimal notation, ranging from 0.0 to 1.0.
     * If the value configured is equal to or greater than 1.0, the action button will be drawn to at max width within the FaceTec Browser SDK Frame.
     * If the value configured results in a width that is less than the action button's height, the action button's width will equal its height.
     * Default is dynamically configured per device at runtime.
     */
    buttonRelativeWidthOnDesktop: string;
    /**
     * Color of the background for the New User Guidance and Retry Screens.
     * Default is white.
     */
    backgroundColors: string;
    /**
     * Color of the text displayed on the New User Guidance and Retry Screens (not including the action button text).
     * Note: This customization can be overridden for specific text using FaceTecGuidanceCustomization.readyScreenHeaderTextColor, .readyScreenSubtextTextColor, .retryScreenHeaderTextColor, and/or .retryScreenSubtextTextColor.
     * Default is custom FaceTec Browser SDK color.
     */
    foregroundColor: string;
    /**
     * Color of the Get Ready for Video Selfie Screen's oval fill.
     * Default is transparent.
     */
    readyScreenOvalFillColor: string;
    /**
     * Background color of the Get Ready for Video Selfie Screen text views during the New User Guidance and Retry Screens.
     * This will only be visible when text is detected as overlapping or too close with the Ready screen oval.
     * Default is a semi-opaque shade of black.
     */
    readyScreenTextBackgroundColor: string;
    /**
     * Background corner radius of the Get Ready for Video Selfie Screen text views during the New User Guidance and Retry Screens.
     * This will only be visible when text is detected as overlapping or too close with the Get Ready for Video Selfie Screen's oval.
     * Default is dynamically configured per device at runtime.
     */
    readyScreenTextBackgroundCornerRadius: string;
    /**
     * Image displayed as Ideal FaceTec Browser SDK example (right image) during the first Retry Screen.
     * Default is configured to use image named 'FaceTec_ideal' located in '/FaceTec_images/' directory (or custom configured default directory for FaceTec Browser SDK images).
     */
    retryScreenIdealImage: string;
    /**
     * Images displayed as Ideal FaceTec Browser SDK examples (right image) during the first Retry Screen.
     * When configured to a non-empty array, these images will override the single image configured for FaceTecGuidanceCustomization.retryScreenIdealImage.
     * Default is an empty array.
     */
    retryScreenSlideshowImages: string[];
    /**
     * Control the time that each image is shown for before transitioning to the next image.
     * Default is 1500ms.
     */
    retryScreenSlideshowInterval: string;
    /**
     * Control whether to allow the slideshow images to appear in a randomized order during each Retry Screen.
     * Default is true (enabled).
     */
    enableRetryScreenSlideshowShuffle: boolean;
    /**
     * Color of the image borders during the first Retry Screen.
     * Default is custom FaceTec Browser SDK color.
     */
    retryScreenImageBorderColor: string;
    /**
     * Thickness of the image borders during the first Retry Screen.
     * Default is dynamically configured per device at runtime.
     */
    retryScreenImageBorderWidth: string;
    /**
     * Corner radius of the image borders during the first Retry Screen.
     * Default is dynamically configured per device at runtime.
     */
    retryScreenImageCornerRadius: string;
    /**
     * Color of the oval's stroke that overlay's the Ideal image example during the first Retry Screen.
     * Default is white.
     */
    retryScreenOvalStrokeColor: string;
    /**
     * Control whether to layout the Retry Screen's instruction messages using bullet-points.
     * Applicable localized instruction message strings include: FaceTec_retry_instruction_message_1, FaceTec_retry_instruction_message_2, FaceTec_retry_instruction_message_3.
     * If enabled, each instruction message will be placed on a new line, proceeded with a bullet-point, and will not extend to multiple lines.
     * If disabled, all instruction messages will be concatenated into a multi-line string.
     * Default is true (enabled).
     */
    enableRetryScreenBulletedInstructions: boolean;
    /**
     * Image displayed on the Camera Permissions Screen.
     * Default is configured to use image named 'FaceTec_camera' located in '/FaceTec_images/' directory (or custom configured default directory for FaceTec Browser SDK images).
     */
    cameraPermissionsScreenImage: string;
    /**
     * Constructor for FaceTecGuidanceCustomization object.
     *
     * @param directoryForImageFiles - (optional) specify a custom directory to search for default image path names.
     */
    constructor(directoryForImageFiles?: string);
}
/**
 * Customize the New User Guidance and Retry Screens.
 * New User Guidance Screens are shown before the FaceTec Browser SDK Session and Retry Screens are shown after an unsuccessful FaceTec Browser SDK Session.
 */
export declare class FaceTecEnterFullScreenCustomization implements FaceTecEnterFullScreenCustomization {
    private defaultLocationForImages;
    /**
     * Thickness of the action button's border during the Enter Full Screen Page.
     * Default is dynamically configured per device at runtime.
     */
    buttonBorderWidth: string;
    /**
     * Color of the action button's border during the Enter Full Screen Page.
     * Default is transparent.
     */
    buttonBorderColor: string;
    /**
     * Corner radius of the action button's border during the Enter Full Screen Page.
     * Default is dynamically configured per device at runtime.
     */
    buttonCornerRadius: string;
    /**
     * Color of the action button's text during the Enter Full Screen Page.
     * Default is white.
     */
    buttonTextNormalColor: string;
    /**
     * Color of the action button's text when the button is pressed during the Enter Full Screen Page.
     * Default is white.
     */
    buttonTextHighlightColor: string;
    /**
     * Color of the action button's text when the button is disabled during the Enter Full Screen Page.
     * Default is white.
     */
    buttonTextDisabledColor: string;
    /**
     * Color of the action button's background during the Enter Full Screen Page.
     * Default is custom FaceTec Browser SDK color.
     */
    buttonBackgroundNormalColor: string;
    /**
     * Color of the action button's background when the button is pressed during the Enter Full Screen Page.
     * Default is custom FaceTec Browser SDK color.
     */
    buttonBackgroundHighlightColor: string;
    /**
     * Color of the action button's background when the button is disabled during the Enter Full Screen Page.
     * Default is custom FaceTec Browser SDK color.
     */
    buttonBackgroundDisabledColor: string;
    /**
     * Font of the title's text during the Enter Full Screen Page.
     */
    headerFont: string;
    /**
     * Spacing between the characters of the title's text during the Enter Full Screen Page.
     * Accepts any value assignable to the letterSpacing CSS attribute.
     * Default is 'normal'.
     */
    headerTextSpacing: string;
    /**
     * Font of the title's subtext and messages during the Enter Full Screen Page.
     */
    subtextFont: string;
    /**
     * Spacing between the characters of the title's subtext and messages during the Enter Full Screen Page.
     * Accepts any value assignable to the letterSpacing CSS attribute.
     * Default is 'normal'.
     */
    subtextTextSpacing: string;
    /**
   * Font of the title's subtext during the Enter Full Screen Page.
   * Default is a bold system font.
   */
    buttonFont: string;
    /**
     * Spacing between the characters of the action button's text during the Enter Full Screen Page.
     * Accepts any value assignable to the letterSpacing CSS attribute.
     * Default is 'normal'.
     */
    buttonTextSpacing: string;
    /**
     * Control the percent of the available FaceTec Browser SDK Frame width to use for the action button during the Enter Full Screen Page for mobile browsers.
     * Relative width percent is represented in decimal notation, ranging from 0.0 to 1.0.
     * If the value configured is equal to or greater than 1.0, the action button will be drawn to at max width within the FaceTec Browser SDK Frame.
     * If the value configured results in a width that is less than the action button's height, the action button's width will equal its height.
     * Default is dynamically configured per device at runtime.
     */
    buttonRelativeWidth: string;
    /**
     * Control the percent of the available FaceTec Browser SDK Frame width to use for the action button during the Enter Full Screen Page for desktop browsers.
     * Relative width percent is represented in decimal notation, ranging from 0.0 to 1.0.
     * If the value configured is equal to or greater than 1.0, the action button will be drawn to at max width within the FaceTec Browser SDK Frame.
     * If the value configured results in a width that is less than the action button's height, the action button's width will equal its height.
     * Default is dynamically configured per device at runtime.
     */
    buttonRelativeWidthOnDesktop: string;
    /**
     * Color of the background for the Enter Full Screen Page.
     * Default is white.
     */
    backgroundColors: string;
    /**
     * Color of the text displayed on the Enter Full Screen Page (not including the action button text).
     * Default is custom FaceTec Browser SDK color.
     */
    foregroundColor: string;
    /**
     * Image displayed on the Enter Full Screen Page.
     * Default is configured to use image named 'FaceTec_enter_fullscreen' located in '/FaceTec_images/' directory (or custom configured default directory for FaceTec Browser SDK images).
     */
    enterFullScreenImage: string;
    /**
     * Shadow displayed behind the Enter Full Screen Page.
     * This accepts box-shadow css attribute string values.
     * Default is none.
     */
    shadow: string;
    /**
     * Color of the Enter Full Screen Page border.
     * Default is white.
     */
    borderColor: string;
    /**
     * Corner radius of the Enter Full Screen Page.
     * Default is dynamically configured per device at runtime.
     */
    borderCornerRadius: string;
    /**
     * Thickness of the Enter Full Screen Page.
     * Default is dynamically configured per device at runtime.
     */
    borderWidth: string;
    /**
     * Constructor for FaceTecEnterFullScreenCustomization object.
     *
     * @param directoryForImageFiles - (optional) specify a custom directory to search for default image path names.
     */
    constructor(directoryForImageFiles?: string);
}
/**
 * Customize the FaceTec Session Overlay.
 * The FaceTec Session Overlay separates the FaceTec Interface from the presenting application, covering the device's full screen.
 */
export declare class FaceTecOverlayCustomization implements FaceTecOverlayCustomization {
    private defaultLocationForImages;
    /**
     * Color of the FaceTec Browser SDK Overlay background.
     * Default is white.
     */
    backgroundColor: string;
    /**
     * Control whether to show the branding image the FaceTec Browser SDK Frame on top of the FaceTec Browser SDK Overlay.
     * Default is true (shown).
     */
    showBrandingImage: boolean;
    /**
     * Image displayed below the FaceTec Browser SDK Frame on top of the FaceTec Browser SDK Overlay.
     * Default is configured to use image named 'FaceTec_your_app_logo' located in '/FaceTec_images/' directory (or custom configured default directory for FaceTec Browser SDK images).
     */
    brandingImage: string;
    /** Constructor for FaceTecOverlayCustomization object. */
    constructor();
}
/**
 * Customize the Result Screen.
 * Shown for server-side work and response handling.
 */
export declare class FaceTecResultScreenCustomization implements FaceTecResultScreenCustomization {
    /**
     * Color of the Result Screen's background.
     * Default is white.
     */
    backgroundColors: string;
    /**
     * Color of the text displayed on the Result Screen.
     * Default is custom FaceTec Browser SDK color.
     */
    foregroundColor: string;
    /**
     * Color of the result animation's background.
     * Default is custom FaceTec Browser SDK color.
     */
    resultAnimationBackgroundColor: string;
    /**
     * Color of the result animation's accent color.
     * Default is white.
     */
    resultAnimationForegroundColor: string;
    /**
     * Image displayed behind the result foreground animation for success scenarios.
     * If image is configured, default result background animation will be hidden.
     * Default is set to an empty string and will fallback to using the default result background animation, which respects the color assigned to .resultAnimationBackgroundColor.
     */
    resultAnimationSuccessBackgroundImage: string;
    /**
     * Image displayed behind the result foreground animation for unsuccess scenarios. Unsuccess result animations are only shown for unsuccessful Photo ID Match attempts.
     * If image is configured, default result background animation will be hidden.
     * Default is set to an empty string and will fallback to using the default result background animation, which respects the color assigned to .resultAnimationBackgroundColor.
     */
    resultAnimationUnsuccessBackgroundImage: string;
    /**
     * Configure an SVG element to display on the Result Screen for success scenarios.
     * Note: All CSS required for the SVG animation needs to be included in your app's CSS file before launching FaceTec Browser SDK.
     * Note: The result animation is displayed for 2 seconds, so custom animation timing should be configured accordingly.
     * If this is set to an SVGElement object, the SVG supplied will be used instead of the default success animation or any success image configured with FaceTecResultScreenCustomization.resultAnimationSuccessBackgroundImage.
     * If this is null, the default success animation will be used.
     * Default is null.
     */
    customResultAnimationSuccess: SVGElement | null;
    /**
     * Configure an SVG element to display on the Result Screen for unsuccess scenarios. Unsuccess result animations are only shown for unsuccessful Photo ID Match attempts.
     * Note: All CSS required for the SVG animation needs to be included in your app's CSS file before launching FaceTec Browser SDK.
     * Note: The result animation is displayed for 2 seconds, so custom animation timing should be configured accordingly.
     * If this is set to an SVGElement object, the SVG supplied will be used instead of the default unsuccess animation or any unsuccess image configured with FaceTecResultScreenCustomization.resultAnimationUnsuccessBackgroundImage.
     * If this is null, the default unsuccess animation will be used.
     * Default is null.
     */
    customResultAnimationUnsuccess: SVGElement | null;
    /**
     * Font of the message text displayed on the Result Screen.
     * Accepts any value assignable to the fontFamily CSS attribute.
     */
    messageFont: string;
    /**
     * Spacing between the characters of the message text displayed on the Result Screen.
     * Accepts any value assignable to the letterSpacing CSS attribute.
     * Default is 'normal'.
     */
    messageTextSpacing: string;
    /**
     * Size of the message text displayed on the Result Screen.
     * Accepts any value assignable to the fontSize CSS attribute.
     * Default is dynamically configured per device at runtime.
     */
    messageTextSize: string;
    /**
     * Color of the activity indicator animation shown during server-side work.
     * Default is custom FaceTec Browser SDK color.
     */
    activityIndicatorColor: string;
    /**
     * Image displayed and rotated during server-side work.
     * If image is configured, default activity indicator will be hidden.
     * Default is set to an empty string and will fallback to using default activity indicator animation.
     */
    customActivityIndicatorImage: string;
    /**
     * Control the speed of the rotation for your custom activity indicator image.
     * Only applicable when image is configured for .customActivityIndicatorImage.
     * This value indicates the duration of each full rotation.
     * Default is 1s.
     */
    customActivityIndicatorRotationInterval: string;
    /**
     * Configure an SVG element to display on the Result Screen for the activity indicator animation during server-side work.
     * Note: All CSS required for the SVG animation needs to be included in your app's CSS file before launching FaceTec Browser SDK.
     * Note: The activity indicator animation is displayed indefinitely during the server-side work, so the custom animation should be set to loop/repeat infinitely.
     * If this is set to an SVGElement object, the SVG supplied will be used instead of the default activity indicator animation or any rotating-image animation configured with FaceTecResultScreenCustomization.customActivityIndicatorImage.
     * If this is null, FaceTecResultScreenCustomization.customActivityIndicatorImage will be used. If FaceTecResultScreenCustomization.customActivityIndicatorImage is null, the default activity indicator animation will be used.
     * Default is null.
     */
    customActivityIndicatorAnimation: SVGElement | null;
    /**
     * Control the size of the activity indicator and result animations displayed on the Result Screen.
     * This value represents the scaling factor that will be applied to the default animation bounds.
     * This value has to be between 0.5 and 2.0. If it’s lower than 0.5 or higher than 2.0, it will be defaulted to 0.5 or 2.0 respectively.
     * Default value is 1.
     */
    animationRelativeScale: number;
    /**
     * Control whether to show or hide the upload progress bar during server-side work.
     * Default is true (shown).
     */
    showUploadProgressBar: boolean;
    /**
     * Color of the upload progress bar's fill.
     * Default is custom FaceTec Browser SDK color.
     */
    uploadProgressFillColor: string;
    /**
     * Color of upload progress bar's track.
     * Default is a semi-opaque shade of black.
     */
    uploadProgressTrackColor: string;
    /** Constructor for FaceTecResultScreenCustomization object. */
    constructor();
}
/**
 * Customize the FaceTec Photo ID Match Screens.
 * .selectionScreenBackgroundColors will be implemented in an upcoming release of the FaceTec Browser SDK.
 * .reviewScreenForegroundColor will be implemented in an upcoming release of the FaceTec Browser SDK.
 * .reviewScreenTextBackgroundColor will be implemented in an upcoming release of the FaceTec Browser SDK.
 * .reviewScreenTextBackgroundBorderColor will be implemented in an upcoming release of the FaceTec Browser SDK.
 * .reviewScreenTextBackgroundBorderWidth will be implemented in an upcoming release of the FaceTec Browser SDK.
 * .reviewScreenTextBackgroundBorderCornerRadius will be implemented in an upcoming release of the FaceTec Browser SDK.
 * .reviewScreenBackgroundColors will be implemented in an upcoming release of the FaceTec Browser SDK.
 *
 */
export declare class FaceTecIDScanCustomization implements FaceTecIDScanCustomization {
    private defaultLocationForImages;
    /**
    * Image displayed on the ID Scan Select ID Document page.
    * Default is configured to use image empty.
    */
    showSelectionScreenBrandingImage: boolean;
    /**
     * Color of the text displayed on the Identity Document Type Selection Screen (not including the action button text).
     * Default is off-black.
     */
    selectionScreenForegroundColor: string;
    /**
     * Font of the title during the Identity Document Type Selection Screen.
     * Accepts any value assignable to the fontFamily CSS attribute.
     */
    headerFont: string;
    /**
     * Spacing between the characters of the title's text during the Identity Document Type Selection Screen.
     * Accepts any value assignable to the letterSpacing CSS attribute.
     * Default is 'normal'.
     */
    headerTextSpacing: string;
    /**
     * Size of the title text during the Identity Document Type Selection Screen.
     * Accepts any value assignable to the fontSize CSS attribute.
     * Default is dynamically configured per device at runtime.
     */
    headerTextSize: string;
    /**
     * Font of the message text during the Identity Document Capture and Review Screens.
     * Accepts any value assignable to the fontFamily CSS attribute.
     */
    subtextFont: string;
    /**
     * Spacing between the characters of the title's subtext and messages during the Identity Document Capture and Review Screens.
     * Accepts any value assignable to the letterSpacing CSS attribute.
     * Default is 'normal'.
     */
    subtextTextSpacing: string;
    /**
     * Size of the message text during the Identity Document Capture and Review Screens.
     * Accepts any value assignable to the fontSize CSS attribute.
     * Default is dynamically configured per device at runtime.
     */
    subtextTextSize: string;
    /**
     * Font of the action button's text during the Photo ID Match Screens.
     * Accepts any value assignable to the fontFamily CSS attribute.
     */
    buttonFont: string;
    /**
     * Spacing between the characters of the action button's text during the Photo ID Match Screens.
     * Accepts any value assignable to the letterSpacing CSS attribute.
     * Default is 'normal'.
     */
    buttonTextSpacing: string;
    /**
     * Size of the action button's text during the Photo ID Match Screens.
     * Accepts any value assignable to the fontSize CSS attribute.
     * Default is dynamically configured per device at runtime.
     */
    buttonTextSize: string;
    /**
     * Thickness of the action button's border during the Photo ID Match Screens.
     * Default is dynamically configured per device at runtime.
     */
    buttonBorderWidth: string;
    /**
     * Color of the action button's border during the Photo ID Match Screens.
     * Default is transparent.
     */
    buttonBorderColor: string;
    /**
     * Corner radius of the action button's border during the Photo ID Match Screens.
     * Default is dynamically configured per device at runtime.
     */
    buttonCornerRadius: string;
    /**
     * Color of the action button's text during the Photo ID Match Screens.
     * Default is white.
     */
    buttonTextNormalColor: string;
    /**
     * Color of the action button's text when the button is pressed during the Photo ID Match Screens.
     * Default is white.
     */
    buttonTextHighlightColor: string;
    /**
   * Color of the action button's text when the button is disabled during the Photo ID Match Screens.
   * Default is white.
   */
    buttonTextDisabledColor: string;
    /**
     * Color of the action button's background during the Photo ID Match Screens.
     * Default is custom FaceTec Browser SDK color.
     */
    buttonBackgroundNormalColor: string;
    /**
     * Color of the action button's background when the button is pressed during the Photo ID Match Screens.
     * Default is custom FaceTec Browser SDK color.
     */
    buttonBackgroundHighlightColor: string;
    /**
     * Color of the action button's background when the button is disabled during the Photo ID Match Screens.
     * Default is custom FaceTec Browser SDK color.
     */
    buttonBackgroundDisabledColor: string;
    /**
     * Control the percent of the available FaceTec Browser SDK Frame width to use for the action button during the Photo ID Match Screens for mobile browsers.
     * Relative width percent is represented in decimal notation, ranging from 0.0 to 1.0.
     * If the value configured is equal to or greater than 1.0, the action button will be drawn to at max width within the FaceTec Browser SDK Frame.
     * If the value configured results in a width that is less than the action button's height, the action button's width will equal its height.
     * Note: The Identity Document Review Screen action buttons will be drawn at half the configured width.
     * Default is dynamically configured per device at runtime.
     */
    buttonRelativeWidth: string;
    /**
     * Control the percent of the available FaceTec Browser SDK Frame width to use for the action button during the Photo ID Match Screens for desktop browsers.
     * Relative width percent is represented in decimal notation, ranging from 0.0 to 1.0.
     * If the value configured is equal to or greater than 1.0, the action button will be drawn to at max width within the FaceTec Browser SDK Frame.
     * If the value configured results in a width that is less than the action button's height, the action button's width will equal its height.
     * Note: The Identity Document Review Screen action buttons will be drawn at half the configured width.
     * Default is dynamically configured per device at runtime.
     */
    buttonRelativeWidthOnDesktop: string;
    /**
     * Color of the Identity Document Type Selection Screen background.
     * Default is white.
     */
    selectionScreenBackgroundColors: string;
    /**
     * Image displayed on the Identity Document Type Selection Screen.
     * Default is configured to use image named 'FaceTec_branding_logo_id_check' located in '/FaceTec_images/' directory (or custom configured default directory for FaceTec Browser SDK images).
     */
    selectionScreenBrandingImage: string;
    /**
     * Color of the text displayed on the Identity Document Capture Screen (not including the action button text).
     * Default is white.
     */
    captureScreenForegroundColor: string;
    /**
     * Color of the text view background during the Identity Document Capture Screen.
     * Default is custom FaceTec Browser SDK color.
     */
    captureScreenTextBackgroundColor: string;
    /**
     * Color of the text view background border during the Identity Document Capture Screen.
     * Default is transparent.
     */
    captureScreenTextBackgroundBorderColor: string;
    /**
     * Thickness of the text view background border during the Identity Document Capture Screen.
     * Default is 0.
     */
    captureScreenTextBackgroundBorderWidth: string;
    /**
     * Corner radius of the text view background and border during the Identity Document Capture Screen.
     * Default is dynamically configured per device at runtime.
     */
    captureScreenTextBackgroundCornerRadius: string;
    /**
     * Color of the Identity Document Capture Screen's background.
     * Default is white.
     */
    captureScreenBackgroundColor: string;
    /**
     * Color of the Identity Document Capture Frame's stroke.
     * Default is custom FaceTec Browser SDK color.
     */
    captureFrameStrokeColor: string;
    /**
     * Thickness of the Identity Document Capture Frame's stroke.
     * Default is dynamically configured per device at runtime.
     */
    captureFrameStrokeWidth: string;
    /**
     * Corner radius of the Identity Document Capture Frame.
     * Default is dynamically configured per device at runtime.
     */
    captureFrameCornerRadius: string;
    /**
     * Color of the text displayed on the Identity Document Review Screen (not including the action button text).
     * Default is white.
     */
    reviewScreenForegroundColor: string;
    /**
     * Color of the text view background during the Identity Document Review Screen.
     * Default is custom FaceTec Browser SDK color.
     */
    reviewScreenTextBackgroundColor: string;
    /**
     * Color of the text view background border during the Identity Document Review Screen.
     * Default is transparent.
     */
    reviewScreenTextBackgroundBorderColor: string;
    /**
     * Thickness of the text view background border during the Identity Document Review Screen.
     * Default is 0.
     */
    reviewScreenTextBackgroundBorderWidth: string;
    /**
     * Corner radius of the text view background and border during the Identity Document Review Screen.
     * Default is dynamically configured per device at runtime.
     */
    reviewScreenTextBackgroundBorderCornerRadius: string;
    /**
     * Corner radius of the ID Document Preview image displayed on the Identity Document Review Screen.
     * Default is dynamically configured per device at runtime.
     */
    reviewScreenDocumentPreviewCornerRadius: string;
    /**
     * Color of the Identity Document Review Screen background.
     * Default is white.
     */
    reviewScreenBackgroundColors: string;
    /**
     * Image displayed below the FaceTec Browser SDK Frame during Photo ID Match when the Identity Document Type selected is an ID Card.
     * This image acts as a placeholder to show a status of incomplete for capturing the ID Card's front side.
     * This only applies to desktop browsers.
     * Default is configured to use image named 'FaceTec_id_card_placeholder_front' located in '/FaceTec_images/' directory (or custom configured default directory for FaceTec Browser SDK images).
     */
    captureScreenIDFrontPlaceHolderImage: string;
    /**
     * Image displayed below the FaceTec Browser SDK Frame during Photo ID Match when the Identity Document Type selected is an ID Card.
     * This image acts as a placeholder to show a status of incomplete for capturing the ID Card's back side.
     * This only applies to desktop browsers.
     * Default is configured to use image named 'FaceTec_id_card_placeholder_back' located in '/FaceTec_images/' directory (or custom configured default directory for FaceTec Browser SDK images).
     */
    captureScreenIDBackPlaceHolderImage: string;
    /**
     * Image displayed below the FaceTec Browser SDK Frame during Photo ID Match when the Identity Document Type selected is a Passport.
     * This image acts as a placeholder to show a status of incomplete for capturing the Passport.
     * This only applies to desktop browsers.
     * Default is configured to use image named 'FaceTec_passport_placeholder' located in '/FaceTec_images/' directory (or custom configured default directory for FaceTec Browser SDK images).
     */
    captureScreenPassportPlaceholderImage: string;
    /**
     * Image displayed below the FaceTec Browser SDK Frame during Photo ID Match when the Identity Document Type selected is an ID Card.
     * This image acts as a placeholder to show a status of complete for capturing the ID Card's front side.
     * This only applies to desktop browsers.
     * Default is configured to use image named 'FaceTec_id_front_checkmark' located in '/FaceTec_images/' directory (or custom configured default directory for FaceTec Browser SDK images).
     */
    captureScreenIDFrontCheckmarkImage: string;
    /**
     * Image displayed below the FaceTec Browser SDK Frame during Photo ID Match when the Identity Document Type selected is an ID Card.
     * This image acts as a placeholder to show a status of complete for capturing the ID Card's back side.
     * This only applies to desktop browsers.
     * Default is configured to use image named 'FaceTec_id_back_checkmark' located in '/FaceTec_images/' directory (or custom configured default directory for FaceTec Browser SDK images).
     */
    captureScreenIDBackCheckmarkImage: string;
    /**
     * Image displayed below the FaceTec Browser SDK Frame during Photo ID Match when the Identity Document Type selected is a Passport.
     * This image acts as a placeholder to show a status of complete for capturing the Passport.
     * This only applies to desktop browsers.
     * Default is configured to use image named 'FaceTec_passport_checkmark' located in '/FaceTec_images/' directory (or custom configured default directory for FaceTec Browser SDK images).
     */
    captureScreenPassportCheckmarkImage: string;
    /** Constructor for FaceTecIDScanCustomization object. */
    constructor();
    [key: string]: string | boolean;
}
export declare class FaceTecSecurityWatermarkCustomization implements FaceTecSecurityWatermarkCustomization {
    securityWatermarkImage: FaceTecSecurityWatermarkImage;
    constructor();
    setSecurityWatermarkImage(selectedSecurityWatermarkImage: FaceTecSecurityWatermarkImage): void;
    [key: string]: FaceTecSecurityWatermarkImage | ((selectedSecurityWatermarkImage: FaceTecSecurityWatermarkImage) => void);
}
export declare var FaceTecCustomizations: {
    overrideResultScreenSuccessMessageObject: {
        message: string;
    };
    getSuccessResultMessageOrOverrideResultScreenSuccessMessage: () => string;
    setCustomization: (updatedCustomization: FaceTecCustomization) => void;
    setLowLightCustomization: (llCustomization: FaceTecCustomization | null) => void;
    FaceTecCustomization: typeof FaceTecCustomization;
    currentCustomization: FaceTecCustomization;
    currentLowLightCustomization: () => FaceTecCustomization | null;
    isLowLightModeActive: () => boolean;
    setIsLowLightModeActive: (isActive: boolean) => void;
    shouldUseLowLightCustomization: () => boolean;
    setImagesDirectory: (directory: string) => void;
    verifyColorCustomizations: (latestCustomization: FaceTecCustomization) => void;
    FaceTecOvalCustomization: typeof FaceTecOvalCustomization;
    FaceTecCancelButtonCustomization: typeof FaceTecCancelButtonCustomization;
    FaceTecExitAnimationCustomization: typeof FaceTecExitAnimationCustomization;
    FaceTecFeedbackBarCustomization: typeof FaceTecFeedbackBarCustomization;
    FaceTecFrameCustomization: typeof FaceTecFrameCustomization;
    FaceTecSessionTimerCustomization: typeof FaceTecSessionTimerCustomization;
    FaceTecInitialLoadingAnimationCustomization: typeof FaceTecInitialLoadingAnimationCustomization;
    FaceTecGuidanceCustomization: typeof FaceTecGuidanceCustomization;
    FaceTecOverlayCustomization: typeof FaceTecOverlayCustomization;
    FaceTecSecurityWatermarkCustomization: typeof FaceTecSecurityWatermarkCustomization;
    FaceTecExitAnimationStyle: typeof FaceTecExitAnimationStyle;
    FaceTecCancelButtonLocation: typeof FaceTecCancelButtonLocation;
};
