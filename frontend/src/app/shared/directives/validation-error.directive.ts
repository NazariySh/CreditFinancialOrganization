import { Directive, ElementRef, inject, Input, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, ValidationErrors } from '@angular/forms';
import { distinctUntilChanged, merge, Subscription, tap } from 'rxjs';

@Directive({
  selector: '[appValidationError]'
})
export class ValidationErrorDirective implements OnInit, OnDestroy {
  private readonly elementRef = inject(ElementRef);
  private subscription: Subscription = Subscription.EMPTY;

  @Input('appValidationError')
  control: AbstractControl | null = null;

  ngOnInit(): void {
    this.subscribeToControlChanges();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  private subscribeToControlChanges(): void {
    if (!this.control) return;

    this.subscription = merge(
      this.control.statusChanges.pipe(
        distinctUntilChanged(), 
        tap(() => this.updateErrorMessage())
      ),
      this.control.valueChanges.pipe(
        distinctUntilChanged(),
        tap(() => this.updateErrorMessage())
      )
    ).subscribe();

    this.updateErrorMessage();
  }

  private updateErrorMessage(): void {
    this.elementRef.nativeElement.textContent = this.generateErrorMessage(this.control?.errors);
  }

  private generateErrorMessage(errors?: ValidationErrors | null): string {
    if (!errors) return '';

    if (errors['required']) return 'This field is required.';
    if (errors['email']) return 'Invalid email format.';
    if (errors['minlength']) return `Minimum length is ${errors['minlength'].requiredLength} characters.`;
    if (errors['maxlength']) return `Maximum length is ${errors['maxlength'].requiredLength} characters.`;
    if (errors['pattern']) return 'Invalid format.';
    if (errors['serverError']) return errors['serverError'];

    return 'Invalid input.';
  }
}
