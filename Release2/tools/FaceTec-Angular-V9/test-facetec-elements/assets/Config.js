export var Config = (function () {
    // -------------------------------------
    // REQUIRED
    // Available at https://dev.facetec.com/#/account
    // NOTE: This field is auto-populated by the FaceTec SDK Configuration Wizard.
    var DeviceKeyIdentifier = "dBwJfnblaqDdRjdzc8TEyxbXlbEjS4IS";

    // -------------------------------------
    // REQUIRED
    // The URL to call to process FaceTec SDK Sessions.
    // In Production, you likely will handle network requests elsewhere and without the use of this variable.
    // See https://dev.facetec.com/#/security-best-practices.
    //"https://api.facetec.com/api/v3/biometrics";
    var BaseURL = "http://localhost:8080";

    // -------------------------------------
    // REQUIRED
    // The FaceMap Encryption Key you define for your application.
    // Please see https://dev.facetec.com/#/licensing-and-encryption-keys for more information.
    //Facetec default public key

    //monni test server generated public key
    // var PublicFaceMapEncryptionKey =
    //     "-----BEGIN PUBLIC KEY-----\n" +
    //     "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAtCu2eCVOnAg9mBn+r0pR\n" +
    //     "0nJUs2h8mUnFTMcH8xOgMrl+oIkRwZrRq7WBdfiCiDAaz6s4BnESmnde+DsxPMKL\n" +
    //     "ljh1uKoC57awsScSVBq3hyIYD3akJnwd4RZS2SRKYNwSmbjVNlFc3PaxEEpyrfpN\n" +
    //     "gKSUJmgfu+zaj0/RaOB4kVbKNnsjf9excRz4uEEXLismfBVcnbTRE/xu/crJXveE\n" +
    //     "mdr4HYuWg1byayiowg80bMw+ymiO3S5AqojHk4AAn6wtSanNSE6ZMPPsr7E+fGCj\n" +
    //     "1MpuENlX/FCH2Cnny6+cq0AUjvmD1JChe/lGn4l4v8me+rOj7yi027yK2jQ/Rj/X\n" +
    //     "4QIDAQAB\n" +
    //     "-----END PUBLIC KEY-----";

    //my own system generated public key
    var PublicFaceMapEncryptionKey =
        "-----BEGIN PUBLIC KEY-----\n" +
        "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAnKQrqYkoz28sbJ68wgLz\n" +
        "O72lJxZdwAqNjojMIGqeTkizjsS6ldmXO5hd5bzESaLD9x2DslmLFu7etnEKlvcX\n" +
        "UQ3M4i7lJFjRbF5ySxeyB3Jdryjp7CarGAJjPnc5YMUeFGdVIFmH14+SaIfspHcO\n" +
        "eAqHk7B+EiKWUJ8mAERVUXatkoDXWQ1dQP3qPMDs2lT48FNVIxpkVof3TCH674Zj\n" +
        "OkEZKK28fVxP6oJKNTV0lNi9BeJ2stbMM7+NB1dU+z5DMnMBzKVNBjPtp3NYLv+Y\n" +
        "R+wAoK/8INKjxPvZvxaZ84D14CAjfIH4euh749xtzt9Fcb+eWkKsGkKmh6KMMtVl\n" +
        "YQIDAQAB\n" +
        "-----END PUBLIC KEY-----";
        
    var currentCustomization;

    // This app can modify the customization to demonstrate different look/feel preferences
    // NOTE: This function is auto-populated by the FaceTec SDK Configuration Wizard based on your UI Customizations you picked in the Configuration Wizard GUI.
    function retrieveConfigurationWizardCustomization(FaceTecSDK) {

        // Set a default customization
        var defaultCustomization = new FaceTecSDK.FaceTecCustomization();

        //defaultCustomization.overlayCustomization.brandingImage = "yourAppLogoImage";
        defaultCustomization.overlayCustomization.showBrandingImage = false;
        defaultCustomization.overlayCustomization.backgroundColor = "transparent";

        this.currentCustomization = defaultCustomization;

        return defaultCustomization;
    };

    // -------------------------------------
    // Boolean to indicate the FaceTec SDK Configuration Wizard was used to generate this file.
    // In this Sample App, if this variable is true, a "Config Wizard Theme" will be added to this App's Design Showcase,
    // and choosing this option will set the FaceTec SDK UI/UX Customizations to the Customizations that you selected in the
    // Configuration Wizard.
    var wasSDKConfiguredWithConfigWizard = true;

    return {
        BaseURL,
        DeviceKeyIdentifier,
        PublicFaceMapEncryptionKey,
        currentCustomization,
        retrieveConfigurationWizardCustomization,
        wasSDKConfiguredWithConfigWizard
    };

})();
