import { LoanStatus } from "../../enums/loan-status";
import { User } from "../users/user";
import { LoanType } from "./loan-type";

export interface Loan {
    id: string;
    customer: User;
    loanType: LoanType;
    status: LoanStatus;
    amount: number;
    interestRate: number;
    startDate: Date;
    endDate: Date;
}