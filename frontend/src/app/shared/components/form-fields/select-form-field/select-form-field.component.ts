import { Component, Input, Self } from '@angular/core';
import { BaseFormFieldValueAccessor } from '../base-form-field-value-accessor';
import { NgControl, ReactiveFormsModule } from '@angular/forms';
import { SelectItem } from '../../../models/select-item';
import { MatFormField } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { ValidationErrorDirective } from '../../../directives/validation-error.directive';
import { NgFor } from '@angular/common';

@Component({
  selector: 'app-select-form-field',
  imports: [
    NgFor,
    ValidationErrorDirective,
    ReactiveFormsModule,
    MatFormField,
    MatInputModule,
    MatSelectModule
  ],
  templateUrl: './select-form-field.component.html',
  styleUrl: './select-form-field.component.scss'
})
export class SelectFormFieldComponent<T> extends BaseFormFieldValueAccessor {
  @Input() label = '';
  @Input() placeholder = '';
  @Input() class = 'w-full mb-3';
  @Input() inputClass = '';
  @Input() items: SelectItem<T>[] = [];

  constructor(@Self() controlDir: NgControl) {
    super(controlDir);
  }
}
