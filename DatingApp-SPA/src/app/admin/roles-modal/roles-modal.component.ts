import { Component, OnInit, Output , EventEmitter } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap';
import { User } from '../../_models/User';

@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.css']
})
export class RolesModalComponent implements OnInit {
  closeBtnName: string;
  submitBtnName: string;
  enabledSubmit = false;
  user: User;
  roles: any[] = [];
  initialRole: any[] = [];
  @Output() updatedSelectedRoles = new EventEmitter();

  constructor(public bsModalRef: BsModalRef) { }

  ngOnInit() {
    this.initialRole = [...this.roles];
   }

  updateRoles() {
    this.updatedSelectedRoles.emit(this.roles);
    this.bsModalRef.hide();
  }

  changed(role) {
    role.checked = !role.checked;
    this.enabledSubmit = true;

  }
}
