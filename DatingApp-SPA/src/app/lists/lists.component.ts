import { Component, OnInit } from '@angular/core';
import { User } from '../_models/User';
import { Pagination, PaginationResult } from '../_models/PaginationResult';
import { UserService } from '../_service/user.service';
import { AlertifyService } from '../_service/alertify.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  users: User[];
  pagination: Pagination;
  likesParam = 'likers';
  constructor(private userService: UserService,
    private alertify: AlertifyService,
    private route: ActivatedRoute) {}

  ngOnInit() {
    this.route.data.subscribe(
      (data) => {
        this.users = data['users'].result;
        this.pagination = data['users'].pagination;
      });
  }

  pageChanged(e) {
    this.pagination.currentPage = e.page;
    this.loadUsers();
  }

  loadUsers() {
    this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, null, this.likesParam).subscribe(
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
