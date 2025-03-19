import { Component, Input, Self } from '@angular/core';
import { NgControl, ReactiveFormsModule } from '@angular/forms';
import { MatFormField } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { ValidationErrorDirective } from '../../../directives/validation-error.directive';
import { BaseFormFieldValueAccessor } from '../base-form-field-value-accessor';
import { CommonModule } from '@angular/common';
import { DateAdapter, MAT_DATE_FORMATS, NativeDateAdapter } from '@angular/material/core';

export const MY_DATE_FORMATS = {
  parse: {
    dateInput: 'yyyy/MM/dd',
  },
  display: {
    dateInput: 'yyyy/MM/dd',
    monthYearLabel: 'MMM yyyy',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM yyyy',
  }
};

@Component({
  selector: 'app-date-form-field',
  imports: [
    CommonModule,
    ValidationErrorDirective,
    ReactiveFormsModule,
    MatFormField,
    MatInputModule,
    MatDatepickerModule
  ],
  templateUrl: './date-form-field.component.html',
  styleUrl: './date-form-field.component.scss',
  providers: [
    { provide: DateAdapter, useClass: NativeDateAdapter },
    { provide: MAT_DATE_FORMATS, useValue: MY_DATE_FORMATS },
  ]
})
export class DateFormFieldComponent extends BaseFormFieldValueAccessor {
  @Input() label = '';
  @Input() type = 'text';
  @Input() placeholder = '';
  @Input() class = 'w-full mb-3';
  @Input() inputClass = '';

  constructor(@Self() controlDir: NgControl) {
    super(controlDir);
  }
}
