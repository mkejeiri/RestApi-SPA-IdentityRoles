import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_service/auth.service';
import { AlertifyService } from '../_service/alertify.service';
import { Router } from '@angular/router';
import { User } from '../_models/User';

/*  I N F O : Why Auth0 JWT
  using localStorage to store the token is a BAD idea, get store in client ùachine and
  we don't want end users to access the token key. We need to store the token in the server instead
  Auth0 JWT offer support for that
  0- npm install @auth0/angular-jwt@2.0.0 , see more on 'https://github.com/auth0/angular2-jwt'
*/
@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  photoUrl: string;
  constructor(
    public authService: AuthService,
    private alertify: AlertifyService,
    private router: Router
  ) { }

  ngOnInit() {
    this.authService.currentPhotoSubject
      .subscribe(picUrl => {
        this.photoUrl = picUrl;
      });
  }
  login() {
    this.authService.login(this.model).subscribe(
      next => {
        this.alertify.success('Logged in successfully!');
      },
      err => {
        this.alertify.error(err);
      },
      () => {
        // when completed!
        this.router.navigate(['/members']);
      }
    );
  }

  loggedIn(): boolean {
    return this.authService.loggedIn();
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/']);
  }

  // onHidden(event: any): void {
  //   console.log('Dropdown is hidden');
  // }
  // onShown(event: any): void {
  //   console.log('Dropdown is shown');
  // }
  // isOpenChange(event: any): void {
  //   console.log('Dropdown state is changed');
  // }

  /*
    Should go to authservice, because if it's use elsewhere we won't inject nav component into other component
    it's not a good pratice!!.
  */

  // loggedIn() {
  //   const token = localStorage.getItem('token');
  //   // shorthand for : if empty ->false else true
  //   return !!token;
  // }
  // logout() {
  //   localStorage.removeItem('token');
  //   // console.log('logged out');
  //   this.alertify.message('logged out');
  // }
}
