import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { AlertifyService } from './alertify.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from '../../environments/environment';
import { User } from '../_models/User';
import { BehaviorSubject } from 'rxjs/';
// import { JwtModule } from '@auth0/angular-jwt';
// import { HttpClientModule } from '@angular/common/http';
// import { TypeaheadOptions } from 'ngx-bootstrap';


@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = environment.apiUrl + 'auth/';
  constructor(private http: HttpClient, private alertify: AlertifyService) { }
  jwtHelper = new JwtHelperService();
  currentUser: User;
  decodedToken: any;
  currentPhotoSubject = new BehaviorSubject<string>('../../assets/user.png');


  // changeMemberPhoto(photoUrl: string) {
  //   this.photoUrlSubject.next(photoUrl);
  // }

  // const expirationDate = jwtHelper.getTokenExpirationDate(myRawToken);
  // const isExpired = jwtHelper.isTokenExpired(myRawToken);

  login(user: User) {
    // in angular 6 to use rxjs we need to go through a pipe!
    return this.http.post(this.baseUrl + 'login', user)
      .pipe(
        map(
          (response: any) => {
            const constUser = response;
            if (constUser) {
              // console.log(response);
              localStorage.setItem('token', constUser.token);
              this.decodedToken = this.jwtHelper.decodeToken(constUser.token);
              localStorage.setItem('user', JSON.stringify(constUser.user));
              this.currentUser = constUser.user;
              this.currentPhotoSubject.next(this.currentUser.photoUrl);
            }
          }));
  }
  register(user: User) {
    return this.http.post(this.baseUrl + 'register', user);
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    this.currentUser = JSON.parse(localStorage.getItem('user'));
    return !this.jwtHelper.isTokenExpired(token);
  }
  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.decodedToken = null;
    this.currentUser = null;
    // console.log('logged out');
    this.alertify.message('logged out');
  }
}
