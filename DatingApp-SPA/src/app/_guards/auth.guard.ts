import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from '../_service/auth.service';
import { AlertifyService } from '../_service/alertify.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService,
    private alertify: AlertifyService,
    private router: Router) {
  }
  canActivate(next: ActivatedRouteSnapshot): boolean {
    const roles = next.firstChild.data['roles'] as Array<string>;
    if (roles) {
      const match = this.authService.roleMatch(roles);
      if (!match) {
        this.router.navigate(['/members']);
        this.alertify.error('You are not allowed to access this area');
        return false;
      } else {
        return true;
      }
    }
    if (this.authService.loggedIn()) {
      return true;
    }
    this.alertify.error('Unauthorized access, you need to log in');
    this.router.navigate(['/']);
    return false;
  }
}
