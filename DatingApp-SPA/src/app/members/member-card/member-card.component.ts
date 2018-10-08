import { Component, OnInit, Input } from '@angular/core';
import { User } from '../../_models/User';
import { AuthService } from '../../_service/auth.service';
import { UserService } from '../../_service/user.service';
import { AlertifyService } from '../../_service/alertify.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() user: User;

  constructor(private authService: AuthService,
    private usersService: UserService,
    private alertify: AlertifyService) { }

  ngOnInit() {
  }

  sendLike(recipientId: number) {
    this.usersService.sendLike(this.authService.decodedToken.nameid, recipientId).subscribe(
      (data) => {
        this.alertify.success('You have liked ' + this.user.knownAs);
      },
      error => {
        this.alertify.error(error);
      });
  }
}
