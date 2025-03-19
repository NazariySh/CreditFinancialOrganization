import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { LoanTypeService } from '../../../core/services/loan-type.service';
import { LoanService } from '../../../core/services/loan.service';
import { ValidationService } from '../../../core/services/validation.service';
import { LoanApplicationCreate } from '../../../shared/models/loans/loan-application-create';
import { LoanType } from '../../../shared/models/loans/loan-type';
import { SelectItem } from '../../../shared/models/select-item';
import { ValidationErrorResponse } from '../../../shared/models/validation-error-response';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { CurrencyPipe } from '@angular/common';
import { FormFieldComponent } from '../../../shared/components/form-fields/form-field/form-field.component';
import { SelectFormFieldComponent } from '../../../shared/components/form-fields/select-form-field/select-form-field.component';
import { DateFormFieldComponent } from '../../../shared/components/form-fields/date-form-field/date-form-field.component';

@Component({
  selector: 'app-application-form',
  imports: [
    DateFormFieldComponent,
    FormFieldComponent,
    SelectFormFieldComponent,
    MatCardModule,
    MatButtonModule,
    CurrencyPipe,
    ReactiveFormsModule
  ],
  templateUrl: './application-form.component.html',
  styleUrl: './application-form.component.scss'
})
export class ApplicationFormComponent implements OnInit {
  form: FormGroup = new FormGroup({});
  loanTypes: SelectItem<LoanType>[] = [];
  monthlyPayment = 0;

  get amount() {
    return this.form.get('amount')?.value ?? 0;
  }

  get loanTermInMonths() {
    return this.form.get('loanTermInMonths')?.value ?? 0;
  }

  get loanType() {
    return this.form.get('loanType')?.value;
  }

  constructor(
    private readonly router: Router,
    private readonly formBuilder: FormBuilder,
    private readonly loanTypeService: LoanTypeService,
    private readonly loanService: LoanService,
    private readonly validationService: ValidationService
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.getTypes();

    this.form.valueChanges.subscribe(() => {
      this.calculateMonthlyPayment();
    });
  }

  onSubmit(): void {
    const { loanType, amount, startDate, loanTermInMonths } = this.form.value;
    const application: LoanApplicationCreate = {
      amount: amount,
      startDate: startDate,
      loanTypeId: loanType?.id,
      interestRate: loanType?.interestRate,
      loanTermInMonths: loanTermInMonths
    };

    this.loanService.applyForLoan(application).subscribe({
      next: () => this.router.navigateByUrl('/loans'),
      error: (error: ValidationErrorResponse) => {
        this.validationService.handleValidationErrors(error, this.form)
      }
    })
  }

  calculateMonthlyPayment(): void {
    const amount = this.amount;
    const loanLength = this.loanTermInMonths;
    const loanType = this.loanType;
  
    if (amount > 0 && loanLength > 0 && loanType) {
      const annualRate = loanType.interestRate / 100;
      const monthlyRate = annualRate / 12;
  
      if (monthlyRate === 0) {
        this.monthlyPayment = amount / loanLength;
        return;
      }
  
      const denominator = Math.pow(1 + monthlyRate, -loanLength);
      this.monthlyPayment = (amount * monthlyRate) / (1 - denominator);
    } else {
      this.monthlyPayment = 0;
    }
  }

  private getTypes() {
    this.loanTypeService.getAll().subscribe({
      next: (data) => {
        this.loanTypes = data.map(loanType => ({
          label: loanType.name,
          value: loanType
        }));
      }
    })
  }

  private initForm() {
    this.form = this.formBuilder.group({
      loanType: [null, Validators.required],
      amount: [5000, [Validators.required]],
      startDate: [null, Validators.required],
      loanTermInMonths: [12, [Validators.required]]
    });
  }
}
