import { Injectable } from '@angular/core';
import { User } from '../_models/User';
import { Resolve, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { UserService } from '../_service/user.service';
import { catchError, map } from 'rxjs/operators';
import { AlertifyService } from '../_service/alertify.service';



@Injectable()
export class LikeParamsResolver implements Resolve<User[]>  {
    pageNumber = 1;
    pageSize = 50;
    likeParams = 'likers';

    constructor(private userService: UserService,
        private router: Router, private alertify: AlertifyService) { }

    resolve(): User[] | Observable<User[]> | Promise<User[]> {
        return this.userService.getUsers(this.pageNumber, this.pageSize, null, this.likeParams)
            .pipe(
                catchError((err: any) => {
                    this.alertify.error('Problem retrieving user list');
                    this.router.navigate(['/home']);
                    return of(null);
                })
            );
    }
}
