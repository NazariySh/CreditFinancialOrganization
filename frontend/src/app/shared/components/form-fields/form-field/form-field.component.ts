import { Component, Input, Self } from '@angular/core';
import { ValidationErrorDirective } from '../../../directives/validation-error.directive';
import { NgControl, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { BaseFormFieldValueAccessor } from '../base-form-field-value-accessor';

@Component({
  selector: 'app-form-field',
  imports: [
    ValidationErrorDirective,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule
  ],
  templateUrl: './form-field.component.html',
  styleUrl: './form-field.component.scss'
})
export class FormFieldComponent extends BaseFormFieldValueAccessor {
  @Input() label = '';
  @Input() type = 'text';
  @Input() placeholder = '';
  @Input() class = 'w-full mb-3';
  @Input() inputClass = '';
  @Input() min?: number;
  @Input() max?: number;
  @Input() step?: number;

  constructor(@Self() controlDir: NgControl) {
    super(controlDir);
  }
}
