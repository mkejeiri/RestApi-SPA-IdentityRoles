import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
/*
    STEP TO CREATE AN HTTP ERR INTERCEPTOR
    1- Create a Injectable export class ErrorInterceptor that  implements HttpInterceptor
    2- Go down
*/
@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(req)
            .pipe(
            catchError(error => {
                    if (error instanceof HttpErrorResponse) {
                        if (error.status === 401) {
                            return throwError(error.statusText);
                        }
                        const applicationError = error.headers.get('Application-Error');
                        if (applicationError) {
                            return throwError(applicationError);
                        }
                        const serverError = error.error;
                        let modalStateErrors = '';
                        if (serverError && typeof serverError === 'object') {
                            if (serverError instanceof Array) {
                                for (const e in serverError) {
                                    if (serverError[e]) {
                                        modalStateErrors += serverError[e].description + '\n';
                                }                                }
                                return throwError(modalStateErrors || serverError || 'Server Error');
                            }


                            for (const key in serverError) {
                                if (serverError[key]) {
                                    modalStateErrors += serverError[key] + '\n';
                                }
                            }
                        }
                        return throwError(modalStateErrors || serverError || 'Server Error');
                    }
                })
            )
            ;
    }
}
/*
    Step 2 -
    We need to register our class as a custom HTTP_INTERCEPTOR as follows:
    'provide: HTTP_INTERCEPTORS' -> A multi-provider token which represents the
                                    array of 'HttpInterceptor's that are registered.
    'multi: true': we want to preserve our array of interceptors (built-in or customs)
                    and add 'ErrorInterceptor' to them!
*/
export const ErrorInterceptorProvider = {
    provide: HTTP_INTERCEPTORS,
    useClass: ErrorInterceptor,
    multi: true
};
