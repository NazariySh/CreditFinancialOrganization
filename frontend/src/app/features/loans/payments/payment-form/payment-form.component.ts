import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { PaymentService } from '../../../../core/services/payment.service';
import { ValidationService } from '../../../../core/services/validation.service';
import { PaymentMethod } from '../../../../shared/enums/payment-method';
import { PaymentCreate } from '../../../../shared/models/payments/payment-create';
import { ValidationErrorResponse } from '../../../../shared/models/validation-error-response';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { FormFieldComponent } from '../../../../shared/components/form-fields/form-field/form-field.component';
import { SelectFormFieldComponent } from '../../../../shared/components/form-fields/select-form-field/select-form-field.component';

@Component({
  selector: 'app-payment-form',
  imports: [
    FormFieldComponent,
    SelectFormFieldComponent,
    MatCardModule,
    ReactiveFormsModule,
    MatButtonModule
  ],
  templateUrl: './payment-form.component.html',
  styleUrl: './payment-form.component.scss'
})
export class PaymentFormComponent implements OnInit {
  form: FormGroup = new FormGroup({});
  loanId?: string;
  readonly methods = Object.keys(PaymentMethod)
  .filter(key => isNaN(Number(key)))
  .map((key, index) => ({
    value: index,
    label: this.getPaymentMethod(PaymentMethod[key as keyof typeof PaymentMethod])
  }));

  readonly buttonText = 'Create Payment';

  constructor(
    private readonly activatedRoute: ActivatedRoute,
    private readonly formBuilder: FormBuilder,
    private readonly router: Router,
    private readonly paymentService: PaymentService,
    private readonly validationService: ValidationService
  ) {}

  ngOnInit(): void {
    const loanId = this.activatedRoute.snapshot.paramMap.get('loanId');
    if (loanId) {
      this.loanId = loanId;
    }

    this.initForm();
  }

  onSubmit(): void {
    if (!this.loanId) return;

    const { amount, paymentMethod } = this.form.value;
    const paymentCreate: PaymentCreate = {
      loanId: this.loanId, amount, paymentMethod
    };

    this.paymentService.create(paymentCreate).subscribe({
      next: () => {
        this.resetForm();
        this.router.navigate(['/loans', this.loanId]);
      },
      error: (error: ValidationErrorResponse) => {
        this.validationService.handleValidationErrors(error, this.form)
      }
    });
  }

  getPaymentMethod(method: PaymentMethod): string {
    switch (method) {
      case PaymentMethod.CreditCard:
        return 'Credit Card';
      case PaymentMethod.DebitCard:
        return 'Debit Card';
      case PaymentMethod.PayPal:
        return 'Pay Pal';
      case PaymentMethod.BankTransfer:
        return 'Bank Transfer';
      case PaymentMethod.Cash:
        return 'Cash';
      default:
        return 'Unknown';
    }
  }

  private initForm() {
    this.form = this.formBuilder.group({
      amount: [500, Validators.required],
      paymentMethod: ['', Validators.required]
    });
  }

  private resetForm() {
    this.form.reset();
  }
}
