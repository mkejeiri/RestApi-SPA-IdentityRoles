import { Component, OnInit, ViewChild } from '@angular/core';
import { User } from '../../_models/User';
import { UserService } from '../../_service/user.service';
import { AlertifyService } from '../../_service/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryOptions, NgxGalleryImage, NgxGalleryAnimation } from 'ngx-gallery';
import { TabsetComponent } from 'ngx-bootstrap';
import { AuthService } from '../../_service/auth.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  user: User;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  @ViewChild('memberDetailsTab') userDetailsTab: TabsetComponent;
  constructor(private userService: UserService,
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    private authService: AuthService) { }

  ngOnInit() {
    this.route.queryParams.subscribe((data) => {
      this.selectTab(((data['tab'] != null) ? +data['tab'] : 0));
    });

    // this.loadUsers();
    this.route.data.subscribe(
      (data) => {
        this.user = data['user'];
      }
    );

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ];

    this.galleryImages = this.getGalleryImages();
  }

  getGalleryImages() {
    const imagesUrl = [];
    for (let i = 0; i < this.user.photos.length; i++) {
      imagesUrl.push({
        small: this.user.photos[i].url,
        medium: this.user.photos[i].url,
        big: this.user.photos[i].url,
        description: this.user.photos[i].description
      });
    }
    return imagesUrl;
  }

  selectTab(tab: number) {
    this.userDetailsTab.tabs[tab].active = true;
  }

  sendLike() {
    this.userService.sendLike(this.authService.decodedToken.nameid , this.user.id).subscribe(
      (data) => {
        this.alertify.success('You have liked ' + this.user.knownAs);
      },
      error => {
        this.alertify.error(error);
      });
  }

  // members/4
  // loadUsers() {
  //   this.userService.getUser(+this.route.snapshot.params['id']).subscribe(
  //     (user: User) => {
  //       this.user = user;
  //     },
  //     error => this.alertify.error(error));
  // }
}
