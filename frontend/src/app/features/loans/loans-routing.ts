import { Routes } from "@angular/router";
import { ApplicationFormComponent } from "./application-form/application-form.component";
import { LoanDetailsComponent } from "./loan-details/loan-details.component";
import { LoanListComponent } from "./loan-list/loan-list.component";

export const loansRoutes: Routes = [
    { path: '', component: LoanListComponent },
    { path: 'new-application', component: ApplicationFormComponent },
    { path: ':id', component: LoanDetailsComponent },
    {
        path: ':loanId/payments',
        loadChildren: () => import('./payments/payments-routing').then(r => r.paymentsRoutes),
    }
];