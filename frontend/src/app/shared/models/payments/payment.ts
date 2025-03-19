import { PaymentMethod } from "../../enums/payment-method";
import { PaymentStatus } from "../../enums/payment-status";
import { Loan } from "../loans/loan";

export interface Payment {
    id: string;
    loan: Loan;
    date: Date;
    amount: number;
    status: PaymentStatus;
    paymentMethod: PaymentMethod;
}