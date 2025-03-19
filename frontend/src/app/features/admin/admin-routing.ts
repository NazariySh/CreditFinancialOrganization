import { Routes } from "@angular/router";
import { LoanTypeListComponent } from "./loan-types/loan-type-list/loan-type-list.component";
import { LoanTypeFormComponent } from "./loan-types/loan-type-form/loan-type-form.component";

export const adminRoutes: Routes = [
    {
        path: 'loan-types',
        children: [
            { path: '', component: LoanTypeListComponent },
            { path: 'new', component: LoanTypeFormComponent },
            { path: 'edit/:id', component: LoanTypeFormComponent }
        ],
    },
];