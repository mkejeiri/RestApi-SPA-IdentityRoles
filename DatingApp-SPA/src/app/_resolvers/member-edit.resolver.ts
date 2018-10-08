import { Injectable } from '@angular/core';
import { User } from '../_models/User';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { UserService } from '../_service/user.service';
import { catchError, map } from 'rxjs/operators';
import { AlertifyService } from '../_service/alertify.service';
import { AuthService } from '../_service/auth.service';



@Injectable()
export class MemberEditResolver implements Resolve<User>  {
    constructor(private userService: UserService,
        private router: Router,
        private alertify: AlertifyService,
        private authService: AuthService) { }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): User | Observable<User> | Promise<User> {
       return this.userService.getUser(this.authService.decodedToken.nameid)
            .pipe(
                catchError((err: any) => {
                    console.log(err);
                    this.alertify.error('Problem retrieving user to edit profile');
                    this.router.navigate(['/members']);
                    return of(null);
                })
            );
    }
}
