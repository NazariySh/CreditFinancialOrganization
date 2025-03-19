import { Routes } from "@angular/router";
import { ApplicationListComponent } from "./applications/application-list/application-list.component";

export const employeeRoutes: Routes = [
    {
        path: 'applications',
        children: [
            { path: '', component: ApplicationListComponent },
        ],
    },
];