import { Component, OnInit, Input } from '@angular/core';
import { Message } from '../../_models/Message';
import { AuthService } from '../../_service/auth.service';
import { UserService } from '../../_service/user.service';
import { AlertifyService } from '../../_service/alertify.service';
import { tap } from 'rxjs/operators';


@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @Input() recipientId: number;
  messages: Message[];
  messageContent: '';
  newMessage: any = {
                      content: '' ,
                      recipientId: this.recipientId,
                      senderId: this.authService.decodedToken.nameid
                    };
  constructor(private authService: AuthService,
              private userService: UserService,
              private alertify: AlertifyService) { }

  ngOnInit() {
    this.loadMessages();
    this.newMessage.content = '';
  }

  loadMessages() {
    const currentUserId = + this.authService.decodedToken.nameid;
    this.userService.getMessageThread(currentUserId, this.recipientId)
        .pipe(
          tap((msg: Message[] ) => {
          for (let i = 0; i < msg.length; i++) {
            if (!msg[i].isRead && msg[i].recipientId === currentUserId) {
              this.userService.markMessageAsRead(msg[i].id, currentUserId);
            }
          }
        }))
      .subscribe(
        (response: Message[]) => {
          // tslint:disable-next-line:no-debugger
          // debugger;
          this.messages = response;
        },
        error => {
          this.alertify.error(error);
        }
      );
  }

  sendMessage() {
    this.newMessage.recipientId = this.recipientId;
    this.newMessage.senderId = this.authService.decodedToken.nameid;
    this.userService.sendMessage(this.authService.decodedToken.nameid, this.newMessage)
      .subscribe(
        (message: Message) => {
          this.messages.unshift(message);
          this.newMessage.content = '';
        },
        error => {
          this.alertify.error('Unable to send the message!');
        }
      );
  }
}
