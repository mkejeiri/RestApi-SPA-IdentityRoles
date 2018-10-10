import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/User';
import { AdminService } from '../../_service/admin.service';
import { AlertifyService } from '../../_service/alertify.service';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { RolesModalComponent } from '../roles-modal/roles-modal.component';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  users: User[];
  bsModalRef: BsModalRef;

  constructor(private adminService: AdminService,
              private alertify: AlertifyService,
              private modalService: BsModalService) { }

  ngOnInit() {
    this.getUsersWithRoles();
  }

  private getUsersWithRoles() {
    this.adminService.getUserWithRoles()
      .subscribe((users: User[]) => {
        this.users = users;
      }, (error) => {
        this.alertify.error(error);
      });
  }

  // Edit role for a particular User
  editRolesModal(user: User) {
      const initialState = {
        user : user,
        roles : this.getRolesArray(user), // UI available roles checked/Unchecked
      };
      this.bsModalRef = this.modalService.show(RolesModalComponent, {initialState});
      this.bsModalRef.content.closeBtnName = 'Close';
      this.bsModalRef.content.submitBtnName = 'Submit';
      this.bsModalRef.content.updatedSelectedRoles.subscribe((rolesAsOutputProp) => {
        // get only the selected role name to pass to our API
        const roleNames = [...rolesAsOutputProp.filter(r => r.checked === true).map(el => el.name)];
        if (roleNames) {
          const rolesToUpdate = {
            RoleNames: roleNames
          };
          this.adminService.updateUserWithRoles(user, rolesToUpdate).subscribe(() => {
            user.roles = [...rolesToUpdate.RoleNames];
            this.alertify.success(user.userName + ' successfully updated');
          }, (error) => {
            this.alertify.error(error);
          });
        }
      });
  }

  // checked/uncheked user roles
  private getRolesArray(user: User) {
    const roles = [];
    const availableUIUserRoles: any[] = [
      { name: 'Member', value: 'Member' },
      { name: 'VIP', value: 'VIP' },
      { name: 'Admin', value: 'Admin' },
      { name: 'Moderator', value: 'Moderator' }
    ];
    for (let i = 0; i < availableUIUserRoles.length; i++) {
      let isMatch = false;
      for (let j = 0; j < user.roles.length; j++) {
        if ( availableUIUserRoles[i].value === user.roles[j]) {
          availableUIUserRoles[i].checked = true;
          roles.push(availableUIUserRoles[i]);
          isMatch = true;
          break;
        }
      }
      if (!isMatch) {
        availableUIUserRoles[i].checked = false;
        roles.push(availableUIUserRoles[i]);
      }
    }
    return roles;
  }

}
