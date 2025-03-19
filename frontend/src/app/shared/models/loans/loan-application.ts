import { ApplicationStatus } from "../../enums/application-status";
import { User } from "../users/user";
import { Loan } from "./loan";

export interface LoanApplication {
    id: string;
    date: Date;
    status: ApplicationStatus;
    approvalDate?: Date;
    loan: Loan;
    employee?: User;
}
