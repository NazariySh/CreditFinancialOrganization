import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AccountService } from '../../../core/services/account.service';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { ValidationService } from '../../../core/services/validation.service';
import { Register } from '../../../shared/models/auth/register';
import { ValidationErrorResponse } from '../../../shared/models/validation-error-response';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { FormFieldComponent } from '../../../shared/components/form-fields/form-field/form-field.component';
import { PasswordFormFieldComponent } from '../../../shared/components/form-fields/password-form-field/password-form-field.component';

@Component({
  selector: 'app-register',
  imports: [
    FormFieldComponent,
    PasswordFormFieldComponent,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent implements OnInit {
  form: FormGroup = new FormGroup({});

  constructor(
    private readonly router: Router,
    private readonly formBuilder: FormBuilder,
    private readonly accountService: AccountService,
    private readonly snack: SnackbarService,
    private readonly validationService: ValidationService
  ) {}

  ngOnInit(): void {
    this.initForm();
  }

  onSubmit(): void {
    const registerData = this.form.value as Register;
    this.accountService.register(registerData).subscribe({
      next: () => {
        this.snack.success('Registration successful - you can now login');
        this.router.navigateByUrl('/auth/login');
      },
      error: (error: ValidationErrorResponse) => {
        this.validationService.handleValidationErrors(error, this.form)
      }
    })
  }

  private initForm() {
    this.form = this.formBuilder.group({
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    })
  }
}
