import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { LoanType } from '../../../../shared/models/loans/loan-type';
import { ValidationErrorResponse } from '../../../../shared/models/validation-error-response';
import { ActivatedRoute, Router } from '@angular/router';
import { LoanTypeService } from '../../../../core/services/loan-type.service';
import { ValidationService } from '../../../../core/services/validation.service';
import { FormFieldComponent } from '../../../../shared/components/form-fields/form-field/form-field.component';
import { MatButtonModule } from '@angular/material/button';
import { TextareaFormFieldComponent } from '../../../../shared/components/form-fields/textarea-form-field/textarea-form-field.component';
import { MatInputModule } from '@angular/material/input';
import { LoanTypeUpdate } from '../../../../shared/models/loans/loan-type-update';
import { LoanTypeCreate } from '../../../../shared/models/loans/loan-type-create';

@Component({
  selector: 'app-loan-type-form',
  imports: [
    ReactiveFormsModule,
    FormFieldComponent,
    MatInputModule,
    TextareaFormFieldComponent,
    MatButtonModule
  ],
  templateUrl: './loan-type-form.component.html',
  styleUrl: './loan-type-form.component.scss'
})
export class LoanTypeFormComponent implements OnInit {
  form: FormGroup = new FormGroup({});
  isEditMode = false;
  id?: string;

  get buttonText(): string {
    return this.isEditMode ? 'Update Loan Type' : 'Create Loan Type';
  }

  constructor(
    private readonly cd: ChangeDetectorRef,
    private readonly formBuilder: FormBuilder,
    private readonly router: Router,
    private readonly activatedRoute: ActivatedRoute,
    private readonly loanTypeService: LoanTypeService,
    private readonly validationService: ValidationService
  ) {}

  ngOnInit(): void {
    const id = this.activatedRoute.snapshot.paramMap.get('id');

    this.initForm();
    if (id) {
      this.loanTypeService.getById(id).subscribe({
        next: type => this.setEditMode(type)
      });
    }
  }

  onSubmit(): void {
    if (this.isEditMode && this.id) {
      this.updateType(this.id)
    }
    else {
      this.createType()
    }
  }

  private createType() {
    const { name, interestRate, description } = this.form.value;
    const loanType: LoanTypeCreate = { name, interestRate, description };

    this.loanTypeService.create(loanType).subscribe({
      next: () => {
        this.resetForm();
        this.router.navigate(['admin/loan-types']);
      },
      error: (error: ValidationErrorResponse) => {
        this.validationService.handleValidationErrors(error, this.form)
      }
    });
  }

  private updateType(typeId: string) {
    const { id, name, interestRate, description } = this.form.value;
    const loanType: LoanTypeUpdate = { id, name, interestRate, description };

    this.loanTypeService.update(typeId, loanType).subscribe({
      next: () => {
        this.resetForm();
        this.router.navigate(['admin/loan-types']);
      },
      error: (error: ValidationErrorResponse) => {
        this.validationService.handleValidationErrors(error, this.form)
      }
    });
  }

  private setEditMode(loanType: LoanType) {
    this.isEditMode = true;
    this.form.patchValue(loanType);
    this.id = loanType.id;
  }

  private initForm() {
    this.form = this.formBuilder.group({
      id: [undefined],
      name: ['', Validators.required],
      interestRate: [1, [Validators.required]],
      description: ['']
    });
  }

  private resetForm() {
    this.form.reset();
    this.isEditMode = false;
    this.id = undefined;
  }
}
