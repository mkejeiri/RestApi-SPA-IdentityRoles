import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AlertifyService } from '../../_service/alertify.service';
import { User } from '../../_models/User';
import { NgForm } from '@angular/forms';
import { UserService } from '../../_service/user.service';
import { AuthService } from '../../_service/auth.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  constructor(private route: ActivatedRoute,
    private alertify: AlertifyService,
    private userService: UserService,
    private authService: AuthService,
    private router: Router) { }
  @ViewChild('editForm') editForm: NgForm;
  user: User;
  photoUrl: string;
  /*
    to prevent user from closing the browser if changes aren't saved!
    Angular has no authority over the window, it can only plays with the DOM!!!
  */
  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: any) {
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }

  ngOnInit() {
    this.route.data.subscribe(
      data => {
        this.user = data['user'];
      }
    );

    this.authService.currentPhotoSubject
    .subscribe(picUrl => {
      this.photoUrl = picUrl;
    });
  }

  udpateUser() {
    // console.log(this.user);
    /*
      we should send token nameid (userId) to check whether is the
      same user trying to update his profile
    */
    this.userService.updateUser(this.authService.decodedToken.nameid, this.user).subscribe(
      next => {
        this.alertify.success('profile succesfully updated');
        this.editForm.reset(this.user);
        this.router.navigate(['/members/' + this.user.id]);

      },
      error => {
        this.alertify.error(error);
      }
    );
  }
  // updateMainPhoto(photoUrl) {

  //   this.user.photoUrl = photoUrl;
  // }

}
