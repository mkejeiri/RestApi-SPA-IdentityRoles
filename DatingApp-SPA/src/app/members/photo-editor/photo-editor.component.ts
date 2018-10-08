import { Component, OnInit, Input } from '@angular/core';
import { Photo } from '../../_models/Photo';
import { FileUploader } from 'ng2-file-upload';
import { AuthService } from '../../_service/auth.service';
import { environment } from '../../../environments/environment';
import { UserService } from '../../_service/user.service';
import { AlertifyService } from '../../_service/alertify.service';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() photos: Photo[];
  // @Output() getMembePhotoChanged = new EventEmitter<string>();
  public uploader: FileUploader;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  currentMainPhoto: Photo;


  constructor(private authService: AuthService
    , private userService: UserService
    , private alertify: AlertifyService) { }

  ngOnInit() {
    this.intializeUploader();
  }

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  intializeUploader() {
    // autoUpload: false : photo will be auto uploaded no press button's required
    this.uploader = new FileUploader(
      {
        url: this.baseUrl + 'users/' + this.authService.decodedToken.nameid + '/photos',
        authToken: 'Bearer ' + localStorage.getItem('token'),
        isHTML5: true,
        allowedFileType: ['image'],
        removeAfterUpload: true,
        autoUpload: false,
        maxFileSize: 10 * 1024 * 1024
      });

    /* To avoid this we need :
    Failed to load http://localhost:5000/api/user/1/photos:
    Response to preflight request doesn't pass access control check:
    The value of the 'Access-Control-Allow-Origin' header in the response
    must not be the wildcard '*' when the request's credentials mode is 'include'.
    Origin 'http://localhost:4200' is therefore not allowed access.
    The credentials mode of requests initiated by the XMLHttpRequest
    is controlled by the withCredentials attribute.
    */
    this.uploader.onAfterAddingFile = (file) => { file.withCredentials = false; };

    // onSuccessItem(item: FileItem, response: string, status: number, headers: ParsedResponseHeaders): any;
    // after finishing the upload
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const res: Photo = JSON.parse(response);
        const photo = {
          id: res.id,
          url: res.url,
          description: res.description,
          dateAdded: res.dateAdded,
          isMain: res.isMain
        };
        if (photo.isMain) {
          this.authService.currentPhotoSubject.next(photo.url);
          this.authService.currentUser.photoUrl = photo.url;
          localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
        }
        this.photos.push(photo);
      }
    };

    // this.uploader.onSuccessItem =  function (item, response, status, headers) {
    //   if (response) {
    //     const res: Photo = JSON.parse(response);
    //     const photo = {
    //       id: res.id,
    //       url: res.url,
    //       description: res.description,
    //       dateAdded: res.dateAdded,
    //       isMain: res.isMain
    //     };
    //   }
    // };
  }
  setMainPhoto(photo: Photo): void {
    this.userService.setMainPhoto(this.authService.decodedToken.nameid, photo.id)
      .subscribe(next => {
        this.currentMainPhoto = this.photos.filter(p => p.isMain === true)[0];
        this.currentMainPhoto.isMain = false;
        photo.isMain = true;
        // this.getMembePhotoChanged.emit(photo.url);
        this.alertify.success('Successfully set to main');
        this.authService.currentPhotoSubject.next(photo.url);
        this.authService.currentUser.photoUrl = photo.url;
        localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
      },
        error => {
          this.alertify.error(error);
        }
      );

  }
  deletePhoto(id: number) {
    this.alertify.confirm('Are you sure you want to delete this photo?', () => {
      this.userService.deletePhoto(this.authService.decodedToken.nameid, id).subscribe(next => {
        this.alertify.success('Photo is successfully deleted');
        this.photos.splice(this.photos.findIndex(p => p.id === id), 1);
      },
        error => {
          // this.alertify.error('failed to delete the photo');
          this.alertify.error(error);
        }
      );
    });
  }
}
