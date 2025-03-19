import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountService } from '../../../core/services/account.service';
import { AuthService } from '../../../core/services/auth.service';
import { ValidationService } from '../../../core/services/validation.service';
import { Login } from '../../../shared/models/auth/login';
import { ValidationErrorResponse } from '../../../shared/models/validation-error-response';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { FormFieldComponent } from '../../../shared/components/form-fields/form-field/form-field.component';
import { PasswordFormFieldComponent } from '../../../shared/components/form-fields/password-form-field/password-form-field.component';

@Component({
  selector: 'app-login',
  imports: [
    FormFieldComponent,
    PasswordFormFieldComponent,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {
  form: FormGroup = new FormGroup({});
  returnUrl = '/loans';

  constructor(
    private readonly activatedRoute: ActivatedRoute,
    private readonly router: Router,
    private readonly formBuilder: FormBuilder,
    private readonly authService: AuthService,
    private readonly accountService: AccountService,
    private readonly validationService: ValidationService
  ) {}

  ngOnInit(): void {
    const returnUrl = this.activatedRoute.snapshot.queryParams['returnUrl'];
    if (returnUrl) this.returnUrl = returnUrl;

    this.initForm();
  }

  onSubmit(): void {
    const loginData = this.form.value as Login;
    this.authService.login(loginData).subscribe({
      next: () => {
        this.accountService.getProfile().subscribe();
        this.router.navigateByUrl(this.returnUrl);
      },
      error: (error: ValidationErrorResponse) => {
        this.validationService.handleValidationErrors(error, this.form)
      }
    })
  }

  private initForm() {
    this.form = this.formBuilder.group({
      email: ['', [Validators.required]],
      password: ['', [Validators.required]]
    });
  }
}
