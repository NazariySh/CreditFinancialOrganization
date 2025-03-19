import { Injectable } from '@angular/core';
import { ValidationErrorResponse } from '../../shared/models/validation-error-response';
import { FormGroup } from '@angular/forms';

@Injectable({
  providedIn: 'root'
})
export class ValidationService {

  handleValidationErrors(error: ValidationErrorResponse, form: FormGroup) {
    if (!error?.errors) {
      return;
    }

    Object.keys(error.errors).forEach((key) => {
      const formattedKey = key.charAt(0).toLowerCase() + key.slice(1); 
      const control = form.get(formattedKey);
  
      if (control) {
        control.setErrors({ serverError: error.errors[key] });
      }
    });
  }
}
