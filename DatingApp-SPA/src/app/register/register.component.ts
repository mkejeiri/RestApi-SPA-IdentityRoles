import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_service/auth.service';
import { AlertifyService } from '../_service/alertify.service';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { User } from '../_models/User';
import { BsDatepickerConfig } from 'ngx-bootstrap';
import { Router } from '@angular/router';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  // @Input() valuesFromHome: any;
  @Output() cancelRegister = new EventEmitter();
  user: User;
  registerForm: FormGroup;
  bsConfig: Partial<BsDatepickerConfig> = Object.assign({}, { dateInputFormat: 'DD/MM/YYYY', containerClass: 'theme-red' });
  minDate = new Date(1950, 1, 1);
  maxDate = new Date(2002, 1, 1);

  constructor(private authService: AuthService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private router: Router) { }

  ngOnInit() {
    // this.bsConfig = { dateInputFormat: 'DD/MM/YYYY', containerClass: 'theme-red' };
    this.createRegisterForm();
  }

  private createRegisterForm() {
    // this.registerForm = this.fb.group({
    //   gender: ['male', [Validators.required]],
    //   username: ['', [Validators.required]],
    //   knownAs: ['', [Validators.required]],
    //   dateOfBirth: [null, [Validators.required]],
    //   city: ['', [Validators.required]],
    //   country: ['', [Validators.required]],
    //   password: ['', [Validators.required, Validators.maxLength(8), Validators.minLength(4)]],
    //   confirmPassword: ['', [Validators.required]]
    // }, { validator: this.passwordMatchValidator });
    this.registerForm = this.fb.group({
      gender: ['male', [Validators.required]],
      username: ['Bob', [Validators.required]],
      knownAs: ['Bob', [Validators.required]],
      dateOfBirth: ['1985-10-20T23:00:00.000Z', [Validators.required]],
      city: ['London', [Validators.required]],
      country: ['UK', [Validators.required]],
      password: ['password', [Validators.required, Validators.maxLength(8), Validators.minLength(4)]],
      confirmPassword: ['password', [Validators.required]]
    }, { validator: this.passwordMatchValidator });
    // this.registerForm = new FormGroup({
    //   username: new FormControl('', Validators.required),
    //   password: new FormControl('', [Validators.required, Validators.maxLength(8), Validators.minLength(4)]),
    //   confirmPassword: new FormControl('', Validators.required)
    // }, this.passwordMatchValidator);
  }
  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmPassword').value ? null : { mismatch: true };
  }
  register() {
    if (this.registerForm.valid) {
      this.user = Object.assign({}, this.registerForm.value);
      this.authService.register(this.user).subscribe(
        (next) => {
          this.alertify.success('Registration successful!');
        },
        (err) => {
          this.alertify.error(err);
        }, () => {
          this.authService.login(this.user).subscribe(() => {
            this.router.navigate(['/members']);
          });
        });
    }
    // this.authService.register(this.model).subscribe(
    //   (next) => {
    //     // console.log('Registration successful!');
    //     this.alertify.success('Registration successful!');
    //   },
    //   (err) => {
    //     // console.log(err);
    //     this.alertify.error(err);
    //   });

    // console.log(this.registerForm.value);

  }
  cancel() {
    this.cancelRegister.emit(false);
    // console.log('cancelled');
    this.alertify.warning('cancelled');
  }
}
