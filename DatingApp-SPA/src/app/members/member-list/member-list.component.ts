import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/User';
import { UserService } from '../../_service/user.service';
import { AlertifyService } from '../../_service/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { Pagination, PaginationResult } from '../../_models/PaginationResult';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  users: User[];
  pagination: Pagination;
  user: User = JSON.parse(localStorage.getItem('user'));
  genderList = [{ value: 'male', dispaly: 'Males' }, { value: 'female', dispaly: 'Females' }];
  userParams = {gender: '', minAge: 18, maxAge: 99, orderBy: 'lastActive'};

  constructor(
    private userService: UserService,
    private alertify: AlertifyService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    // this.loadUsers();
    this.route.data.subscribe(
      (data) => {
        this.users = data['users'].result;
        this.pagination = data['users'].pagination;
      }
    );
    this.userParams.gender = this.user.gender === 'female' ? 'male' : 'female';
  }
  resetFilters() {
    this.userParams.gender = this.user.gender === 'female' ? 'male' : 'female';
    this.loadUsers();
    }


  pageChanged(e) {
    this.pagination.currentPage = e.page;
    this.loadUsers();
  }
  loadUsers() {
    this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, this.userParams).subscribe(
      (paginatedResult: PaginationResult<User[]>) => {
        this.users = paginatedResult.result;
        this.pagination = paginatedResult.pagination;
      },
      error => {
        this.alertify.error(error);
      }
    );
  }
}
