import { Injectable } from '@angular/core';
import { User } from '../_models/User';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { UserService } from '../_service/user.service';
import { catchError} from 'rxjs/operators';
import { AlertifyService } from '../_service/alertify.service';



@Injectable()
export class MemberDetailResolver implements Resolve<User>  {
    constructor(private userService: UserService,
        private router: Router, private alertify: AlertifyService) { }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): User | Observable<User> | Promise<User> {
        return this.userService.getUser(+route.params['id'])
            .pipe(
                catchError((err: any) => {
                    this.alertify.error('Problem retrieving user details');
                    this.router.navigate(['/members']);
                    return of(null);
                })
            );
    }
}
