import { Injectable } from '@angular/core';
import { Resolve, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { UserService } from '../_service/user.service';
import { catchError } from 'rxjs/operators';
import { AlertifyService } from '../_service/alertify.service';
import { Message } from '../_models/Message';
import { AuthService } from '../_service/auth.service';

@Injectable()
export class MessagesResolver implements Resolve<Message[]>  {
    pageNumber = 1;
    pageSize = 10;
    messageContainer = 'Unread';

    constructor(private userService: UserService,
        private router: Router,
        private alertify: AlertifyService,
        private authService: AuthService) { }

    resolve(): Message[] | Observable<Message[]> | Promise<Message[]> {
        return this.userService.getMessages(this.authService.decodedToken.nameid,
            this.pageNumber,
            this.pageSize,
            this.messageContainer)
            .pipe(
                catchError((err: any) => {
                    this.alertify.error('Problem retrieving messages');
                    this.router.navigate(['/home']);
                    return of(null);
                })
            );
    }
}
