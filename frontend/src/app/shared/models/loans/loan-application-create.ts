export interface LoanApplicationCreate {
    amount: number;
    startDate: Date;
    loanTypeId: string;
    interestRate: number;
    loanTermInMonths: number;
}