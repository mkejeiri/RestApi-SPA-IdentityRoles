import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { User } from '../_models/User';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }

  getUserWithRoles() {
    return this.http.get(this.baseUrl + 'admin/usersWithRoles');
  }

  updateUserWithRoles(user: User, role) {
    return this.http.post(this.baseUrl + 'admin/editRoles/' + user.userName, role);
  }
}
