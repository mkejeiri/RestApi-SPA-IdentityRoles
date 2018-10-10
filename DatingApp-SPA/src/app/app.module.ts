import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule} from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { BsDropdownModule, TabsModule } from 'ngx-bootstrap';
import { appRoutes } from './routes.routing';
import { JwtModule } from '@auth0/angular-jwt';
import { NgxGalleryModule } from 'ngx-gallery';
import { FileUploadModule } from 'ng2-file-upload';
import { BsDatepickerModule } from 'ngx-bootstrap';
import { TimeAgoPipe } from 'time-ago-pipe';
import { ButtonsModule } from 'ngx-bootstrap';
import { PaginationModule } from 'ngx-bootstrap';
import { ModalModule } from 'ngx-bootstrap';



import { AlertifyService } from './_service/alertify.service';
import { AuthService } from './_service/auth.service';
import { ErrorInterceptorProvider } from './_service/error.interceptor';
import { AuthGuard } from './_guards/auth.guard';
import { MembersListResolver } from './_resolvers/members-list.resolver';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';
import { MemberEditResolver } from './_resolvers/member-edit.resolver';
import { LikeParamsResolver } from './_resolvers/like-params.resolver';
import { MessagesResolver } from './_resolvers/messages.resolver';
import { PreventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';
import { UserService } from './_service/user.service';


import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { NavComponent } from './nav/nav.component';
import { ListsComponent } from './lists/lists.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';

import { MemberCardComponent } from './members/member-card/member-card.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { PhotoEditorComponent } from './members/photo-editor/photo-editor.component';
import { MemberMessagesComponent } from './members/member-messages/member-messages.component';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { HasRoleDirective } from './_directives/hasRole.directive';
import { PhotoManagementComponent } from './admin/photo-management/photo-management.component';
import { UserManagementComponent } from './admin/user-management/user-management.component';
import { AdminService } from './_service/admin.service';
import { RolesModalComponent } from './admin/roles-modal/roles-modal.component';

export function tokenGetter() {
    return localStorage.getItem('token');
}

@NgModule({
    declarations: [
        AppComponent,
        HomeComponent,
        RegisterComponent,
        NavComponent,
        ListsComponent,
        MemberListComponent,
        MessagesComponent,
        MemberCardComponent,
        MemberDetailComponent,
        MemberEditComponent,
        PhotoEditorComponent,
        TimeAgoPipe,
        MemberMessagesComponent,
        AdminPanelComponent,
        HasRoleDirective,
        PhotoManagementComponent,
        UserManagementComponent,
        RolesModalComponent
    ],
    imports: [
        BrowserModule,
        HttpClientModule,
        FormsModule,
        ReactiveFormsModule,
        NgxGalleryModule,
        BsDropdownModule.forRoot(),
        RouterModule.forRoot(appRoutes),
        BsDatepickerModule.forRoot(),
        JwtModule.forRoot({
            config: {
                tokenGetter: tokenGetter,
                whitelistedDomains: ['localhost:5000'],
                blacklistedRoutes: ['localhost:5000/api/auth/']
            }
        }),
        ButtonsModule.forRoot(),
        TabsModule.forRoot(),
        FileUploadModule,
        PaginationModule.forRoot(),
        ModalModule.forRoot()
    ],
    providers: [
        AuthService,
        ErrorInterceptorProvider,
        AlertifyService,
        AuthGuard,
        UserService,
        MembersListResolver,
        MemberDetailResolver,
        MemberEditResolver,
        LikeParamsResolver,
        PreventUnsavedChangesGuard,
        MessagesResolver,
        AdminService

    ],
    entryComponents: [
        RolesModalComponent
    ],
    bootstrap: [
        AppComponent
    ]
})
export class AppModule { }
