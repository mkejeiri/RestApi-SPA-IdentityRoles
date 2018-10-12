import { Component, OnInit } from '@angular/core';
import { Photo } from '../../_models/Photo';
import { AdminService } from '../../_service/admin.service';
import { AlertifyService } from '../../_service/alertify.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {
  photos: Photo[] = [];
  constructor(private adminService: AdminService,
    private alertify: AlertifyService) { }

  ngOnInit() {
    this.adminService.getPhotosForApproval()
    .subscribe((data: Photo[])  => {
      this.photos = data;
      }, (error) => {
        this.alertify.error(error);
      });

  }
  approvePhoto(id: number) {
    this.adminService.approvePhoto(id)
      .subscribe(() => {
        this.photos.splice(this.photos.indexOf(this.photos.find(p => p.id === id)), 1);
        this.alertify.success('Photo has been successfully approved');
      }, (error) => {
        this.alertify.error('Failed to approve photos');
      });
  }

  rejectPhoto(id: number) {
    this.adminService.rejectPhoto(id)
    .subscribe(() => {
      this.photos.splice(this.photos.indexOf(this.photos.find(p => p.id === id)), 1);
      this.alertify.success('Photo has been successfully rejected');
    }, (error) => {
      this.alertify.error('Failed to reject photos');
    });
  }
}
