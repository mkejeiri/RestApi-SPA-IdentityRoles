import { Directive, Input, ViewContainerRef, TemplateRef, OnInit } from '@angular/core';
import { AuthService } from '../_service/auth.service';

@Directive({
  selector: '[appHasRole]' // * will create an <ng-template> tag
})
export class HasRoleDirective implements OnInit {
  @Input() appHasRole: Array<string>;
  isVisible = false;

  constructor(private viewContainerRef: ViewContainerRef,
    private templateRef: TemplateRef<any>,
    private authService: AuthService) { }

  ngOnInit() {
    const userRole = this.authService.decodedToken.role;
    // if no roles clear the container
    if (!userRole) {
      this.viewContainerRef.clear();
      this.isVisible = false;
      return;
    }

    // if user has role than we might have to render the element
    if (this.authService.roleMatch(this.appHasRole)) {
      this.viewContainerRef.createEmbeddedView(this.templateRef);
      this.isVisible = true;
    } else {
      this.viewContainerRef.clear();
      this.isVisible = false;
    }
  }
}
