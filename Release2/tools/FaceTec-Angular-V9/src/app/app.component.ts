import { Component, OnInit, Input } from "@angular/core";
import { AngularSampleApp } from "../assets/angular-sample-controller";
import { FaceTecSDK } from "../assets/core-sdk/FaceTecSDK.js/FaceTecSDK";
import { Config } from "../assets/Config";
import { SampleAppUtilities } from "../assets/utilities/SampleAppUtilities";

@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.css"]
})


export class AppComponent implements OnInit {
  constructor() { }

  @Input()
  buttonHeading: string;

  @Input()
  prodKey: any;

  @Input()
  deviceKey: string;

  ngOnInit(): void {
    // Set a the directory path for other FaceTec Browser SDK Resources.
    FaceTecSDK.setResourceDirectory("/assets/core-sdk/FaceTecSDK.js/resources");

    // Set the directory path for required FaceTec Browser SDK images.
    FaceTecSDK.setImagesDirectory("/assets/core-sdk/FaceTec_images");

    //Initialize FaceTec in Production Mode and configure the UI features.
    FaceTecSDK.initializeInProductionMode(this.prodKey, this.deviceKey, Config.PublicFaceMapEncryptionKey, function (initializedSuccessfully: boolean) {
      if (initializedSuccessfully) {
        SampleAppUtilities.enableAllButtons();
      }
      SampleAppUtilities.displayStatus(FaceTecSDK.getFriendlyDescriptionForFaceTecSDKStatus(FaceTecSDK.getStatus()));
    });

    SampleAppUtilities.formatUIForDevice();
  }

  // Perform Liveness Check.
  onLivenessCheckPressed() {
    AngularSampleApp.onLivenessCheckPressed();
  }

  // Perform Photo ID Match
  onPhotoIdMatchPressed() {
    document.getElementById("tabContent").style.filter = "blur(2px)";
    AngularSampleApp.onPhotoIdMatchPressed();
  }
}
