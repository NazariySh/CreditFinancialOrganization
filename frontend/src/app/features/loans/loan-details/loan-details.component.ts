import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { LoanService } from '../../../core/services/loan.service';
import { LoanStatus } from '../../../shared/enums/loan-status';
import { Loan } from '../../../shared/models/loans/loan';
import { DateFormatPipe } from '../../../shared/pipes/date-format.pipe';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { PaymentListComponent } from '../payments/payment-list/payment-list.component';

@Component({
  selector: 'app-loan-details',
  imports: [
    PaymentListComponent,
    DateFormatPipe,
    CurrencyPipe,
    MatButtonModule,
    RouterLink
  ],
  providers: [DatePipe],
  templateUrl: './loan-details.component.html',
  styleUrl: './loan-details.component.scss'
})
export class LoanDetailsComponent implements OnInit {
  id!: string;
  loan!: Loan;

  get statusText(): string {
    return LoanStatus[this.loan.status];
  }

  constructor(
    private readonly activatedRoute: ActivatedRoute,
    private readonly loanService: LoanService
  ) {}

  ngOnInit(): void {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if (id) {
      this.id = id;
      this.getLoan();
    }
  }

  private getLoan() {
    this.loanService.getById(this.id).subscribe({
      next: loan => this.loan = loan
    })
  }
}
