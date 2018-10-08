import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/Message';
import { Pagination, PaginationResult } from '../_models/PaginationResult';
import { UserService } from '../_service/user.service';
import { AlertifyService } from '../_service/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '../_service/auth.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages: Message[];
  pagination: Pagination;
  // user: User = JSON.parse(localStorage.getItem('user'));
  messageContainer: 'Unread';
  // genderList = [{ value: 'male', dispaly: 'Males' }, { value: 'female', dispaly: 'Females' }];
  // userParams = {gender: '', minAge: 18, maxAge: 99, orderBy: 'lastActive'};
  constructor( private userService: UserService,
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    private authService: AuthService) { }

  ngOnInit() {
    this.route.data.subscribe(
      (data) => {
        this.messages = data['messages'].result;
        this.pagination = data['messages'].pagination;
      }
    );
    // this.userParams.gender = this.user.gender === 'female' ? 'male' : 'female';
  }

  loadMessages() {
    // userId: number, page?, itemsPerPage?, messageContainer?
      this.userService.getMessages(
      this.authService.decodedToken.nameid,
      this.pagination.currentPage,
      this.pagination.itemsPerPage,
      this.messageContainer).subscribe(
        (paginatedResult: PaginationResult<Message[]>) => {
          this.messages = paginatedResult.result;
          this.pagination = paginatedResult.pagination;
        },
        error => {
          this.alertify.error(error);
        }
      );
  }

  pageChanged(event) {
    this.pagination.currentPage = event.page;
    this.loadMessages();
  }

  deleteMessage(messageId: number) {
    this.alertify.confirm('Are sure that you want to delete this message', () => {
      this.userService.deleteMessage(messageId, this.authService.decodedToken.nameid)
      .subscribe(() => {
        this.messages = this.messages.slice(this.messages.findIndex(m => m.id === messageId), 1);
        this.alertify.success('Message has been deleted');
      },
        (error) => {
          this.alertify.error('Unable to delete the message');
        }
    );
    });
  }
}
