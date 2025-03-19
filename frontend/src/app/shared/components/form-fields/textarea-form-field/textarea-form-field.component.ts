import { Component, Input, Self } from '@angular/core';
import { BaseFormFieldValueAccessor } from '../base-form-field-value-accessor';
import { ReactiveFormsModule, NgControl } from '@angular/forms';
import { MatFormField } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { ValidationErrorDirective } from '../../../directives/validation-error.directive';

@Component({
  selector: 'app-textarea-form-field',
  imports: [
    ValidationErrorDirective,
    ReactiveFormsModule,
    MatFormField,
    MatInputModule
  ],
  templateUrl: './textarea-form-field.component.html',
  styleUrl: './textarea-form-field.component.scss'
})
export class TextareaFormFieldComponent extends BaseFormFieldValueAccessor {
  @Input() label = '';
  @Input() type = 'text';
  @Input() placeholder = '';
  @Input() class = 'w-full mb-3';
  @Input() inputClass = '';

  constructor(@Self() controlDir: NgControl) {
    super(controlDir);
  }
}
