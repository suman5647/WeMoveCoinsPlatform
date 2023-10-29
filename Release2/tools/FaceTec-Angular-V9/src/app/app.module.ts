import { BrowserModule } from '@angular/platform-browser';
import { Injector, NgModule } from '@angular/core';
import { createCustomElement } from '@angular/elements';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [],
  entryComponents: [AppComponent]
})
export class AppModule {
  constructor(private injector: Injector) {

    if (!customElements.get('facetec-element')) {
      const el = createCustomElement(AppComponent, { injector });
      customElements.define('facetec-element', el);
      var button = (document.getElementById("photo-id-match-button") as HTMLButtonElement);
      if (button != undefined) {
        button.disabled = true;
      }
    } else {
      var button = (document.getElementById("photo-id-match-button") as HTMLButtonElement);
      if (button != undefined)
        button.disabled = true;
    }
  }
  ngDoBootstrap() { }
}
