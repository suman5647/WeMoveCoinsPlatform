import { FaceTecIDScanProcessor, FaceTecIDScanResult, FaceTecSessionResult, FaceTecFaceScanProcessor } from "./FaceTecPublicApi";
/**
 * FaceTecIDScan callback function
 */
export interface FaceTecIDScanCompleteFunction {
    (iDScanResult?: FaceTecIDScanResult): void;
}
/**
 * Session Complete Function
 */
export interface FaceTecSessionCompleteFunction {
    (sessionResult?: FaceTecSessionResult): void;
}
/**
 * FaceTecSession class
 */
export declare class FaceTecSession {
    onSessionCaptureComplete: FaceTecSessionCompleteFunction;
    start: () => void;
    /**
     * FaceTecSession constructor with overrides.
     */
    /**
     * IDScan constructor
     * Use this for Identity Scan process
     */
    constructor(idScanProcessor: FaceTecIDScanProcessor, sessionToken: string);
    /**
     * FaceScan constructor
     * Use this for Enrollment, Authentication, and Liveness Check
     */
    constructor(faceScanProcessor: FaceTecFaceScanProcessor, sessionToken: string);
}
export declare class FaceTecSessionFromIFrame extends FaceTecSession {
    /**
     * Session in Iframe constructor with overrides. This is the same as Session
     */
    /**
     * IDScan constructor
     * Use this for Identity Scan process
     */
    constructor(idScanProcessorFunction: FaceTecIDScanProcessor, sessionToken: string);
    /**
     * FaceScan constructor
     * Use this for Enrollment, Authentication, and Liveness Check
     */
    constructor(faceScanProcessorFunction: FaceTecFaceScanProcessor, sessionToken: string);
}
