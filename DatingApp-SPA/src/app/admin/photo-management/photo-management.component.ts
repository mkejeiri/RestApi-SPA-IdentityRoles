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
    console.log('"getPhotosForApproval"') ;
    this.adminService.getPhotosForApproval()
    .subscribe((data: Photo[])  => {
      this.photos = data;
        console.log(data);
      }, (error) => {
        console.log(error);
      });

  }

//   rejectPhoto(photoId: number) {
//     return this.http.post(this.baseUrl + 'admin/rejectPhoto/' + photoId, {});
//   }


//   approvePhoto(photoId: number) {
//     return this.http.post(this.baseUrl + 'admin/approvePhoto/'  + photoId, {});
//   }
// }



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
