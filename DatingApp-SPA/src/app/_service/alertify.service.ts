/*
  Service wrapper around alertify:
    0- npm install alertifyjs --save
    1- declare 'alertify.js' in script angular.json
    2- import "alertify.min.css" and 'themes/bootstrap.min.css' in style.css
    3- create a service AlertifyService
    4- declare 'declare let alertify: any;'
    5- redefine methods confirm,success,error,warning,message
    6- Inject the service into the app.module to make available accross app.
*/
import { Injectable } from '@angular/core';
declare let alertify: any;
@Injectable({
  providedIn: 'root'
})
export class AlertifyService {
  constructor() { }
  confirm(message: string, onCallback: () => any) {
    alertify.confirm(message, (e => {
      if (e) {
        onCallback();
      } else { }
    }));
  }

  success(message: string) {
    alertify.success(message);
  }

  error(message: string) {
    alertify.error(message);
  }

  warning(message: string) {
    alertify.warning(message);
  }
  message(message: string) {
    alertify.message(message);
  }
}
