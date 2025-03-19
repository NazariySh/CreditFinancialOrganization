import { PaymentMethod } from "../../enums/payment-method";

export interface PaymentCreate {
    loanId: string;
    amount: number;
    paymentMethod: PaymentMethod;
}