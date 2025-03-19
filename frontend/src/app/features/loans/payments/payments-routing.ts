import { Routes } from "@angular/router";
import { PaymentListComponent } from "./payment-list/payment-list.component";
import { PaymentFormComponent } from "./payment-form/payment-form.component";

export const paymentsRoutes: Routes = [
    { path: '', component: PaymentListComponent },
    { path: 'new', component: PaymentFormComponent }
];