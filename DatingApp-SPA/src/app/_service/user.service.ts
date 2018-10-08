import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/User';
import { PaginationResult } from '../_models/PaginationResult';
import { map } from 'rxjs/operators';
import { Message } from '../_models/Message';

// const httpOptions = {
//   headers: new HttpHeaders({
//     'Authorization': 'Bearer ' + localStorage.getItem('token')
//   })
// };

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }

  getUsers(page?, itemsPerPage?, userParams?, likeParams?: string): Observable<PaginationResult<User[]>> {
    // return this.http.get<User[]>(this.baseUrl + 'users', httpOptions);
    const paginatedResult: PaginationResult<User[]> = new PaginationResult<User[]>();
    // tslint:disable-next-line:prefer-const
    let params = new HttpParams();
    if (page != null && itemsPerPage != null) {
      params = params.append('PageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    if (userParams != null) {
      params = params.append('gender', userParams.gender);
      params = params.append('minAge', userParams.minAge);
      params = params.append('maxAge', userParams.maxAge);
      params = params.append('orderBy', userParams.orderBy);

    }

    if (likeParams != null) {
      if (likeParams.toLowerCase() === 'likers') {
        params = params.append('likers', 'true');
      }

      if (likeParams.toLowerCase() === 'likees') {
        params = params.append('likees', 'true');
      }
    }

    return this.http.get<User[]>(this.baseUrl + 'users', { observe: 'response', params })
      .pipe(
        map((response) => {
          paginatedResult.result = response.body;
          if (response.headers.get('Pagination') != null) {
            paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
          }
          return paginatedResult;
        })
    );
  }

  getUser(id: number): Observable<User> {
    // return this.http.get<User>(this.baseUrl + 'users/' + id, httpOptions);
    return this.http.get<User>(this.baseUrl + 'users/' + id);
  }

  updateUser(id: number, user: User) {
    return this.http.put(this.baseUrl + 'users/' + id, user);
  }

  // This a post request so we need to send something we will send an empty object {}
  setMainPhoto(userId: number, id: number) {
    return this.http.post(this.baseUrl + 'users/' + userId + '/photos/' + id + '/setMain', {});
  }

  deletePhoto(userId: number, id: number) {
    return this.http.delete(this.baseUrl + 'users/' + userId + '/photos/' + id );
  }

  sendLike(userId: number, recipientId: number) {
    return this.http.post(this.baseUrl + 'users/' + userId + '/like/' + recipientId, {});
  }

  getMessages(userId: number, page?, itemsPerPage?, messageContainer?) {
    const paginatedResult: PaginationResult<Message[]> = new PaginationResult<Message[]>();
    let params = new HttpParams();
    if (messageContainer != null) {
      params = params.append('messageContainer', messageContainer);
    }
    if (page != null && itemsPerPage != null) {
      params = params.append('PageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }
    return this.http.get<Message[]>(this.baseUrl + 'users/' + userId + '/messages', { observe: 'response', params })
      .pipe(
        map((response) => {
          paginatedResult.result = response.body;
          if (response.headers.get('Pagination') != null) {
            paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
          }
          return paginatedResult;
        })
      );
  }

  // http://localhost:5000/api/users/1/messages/thread/7
  getMessageThread(senderId: number, recipientId: number) {
    return this.http.get<Message[]>(this.baseUrl + 'users/' + senderId + '/messages/thread/' + recipientId);
  }

  // http://localhost:5000/api/users/userId/controller]
    // CreateMessage(int userId,
    // MessageForCreationDto messageForCreationDto)

  sendMessage(senderId: number, message: Message) {
    return this.http
      .post<Message>(this.baseUrl + 'users/' + senderId + '/messages',
        message);
  }

  deleteMessage(messageId: number, userId: number) {
    return this.http
      .post<Message>(this.baseUrl + 'users/' + userId + '/messages/' + messageId, {});
  }

  markMessageAsRead(messageId: number, userId: number) {
    return this.http
      .post<Message>(this.baseUrl + 'users/' + userId + '/messages/' + messageId + '/read', {})
      .subscribe();
  }
}
