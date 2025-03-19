import { Directive, effect, inject, TemplateRef, ViewContainerRef } from '@angular/core';
import { AccountService } from '../../core/services/account.service';

@Directive({
  selector: '[appIsEmployee]'
})
export class IsEmployeeDirective {
  private readonly accountService = inject(AccountService);
  private readonly viewContainerRef = inject(ViewContainerRef);
  private readonly templateRef = inject(TemplateRef);

  constructor() {
    effect(() => {
      if (this.accountService.isEmployee()) {
        this.viewContainerRef.createEmbeddedView(this.templateRef);
      } else {
        this.viewContainerRef.clear();
      }
    })
  }
}
