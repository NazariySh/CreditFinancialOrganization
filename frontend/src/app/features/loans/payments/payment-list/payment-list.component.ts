import { Component, Input, OnInit } from '@angular/core';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { ActivatedRoute } from '@angular/router';
import { PaymentService } from '../../../../core/services/payment.service';
import { PagedList } from '../../../../shared/models/paged-list';
import { Payment } from '../../../../shared/models/payments/payment';
import { DateFormatPipe } from '../../../../shared/pipes/date-format.pipe';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { PaymentMethod } from '../../../../shared/enums/payment-method';

@Component({
  selector: 'app-payment-list',
  imports: [
    DateFormatPipe,
    CurrencyPipe,
    MatPaginator
  ],
  providers: [DatePipe],
  templateUrl: './payment-list.component.html',
  styleUrl: './payment-list.component.scss'
})
export class PaymentListComponent implements OnInit {
  @Input() loanId!: string;
  payments = new PagedList<Payment>();
  pageNumber = 1;
  pageSize = 20;
  
  constructor(
    private readonly activatedRoute: ActivatedRoute,
    private readonly paymentService: PaymentService
  ) {}

  ngOnInit(): void {
    const loandId = this.activatedRoute.snapshot.paramMap.get('loandId');
    if (loandId) {
      this.loanId = loandId;
    }

    if (this.loanId) {
      this.getPayments();
    }
  }

  getPaymentMethod(method: PaymentMethod): string {
    switch (method) {
      case PaymentMethod.CreditCard:
        return 'Credit Card';
      case PaymentMethod.DebitCard:
        return 'Debit Card';
      case PaymentMethod.PayPal:
        return 'Pay Pal';
      case PaymentMethod.BankTransfer:
        return 'Bank Transfer';
      case PaymentMethod.Cash:
        return 'Cash';
      default:
        return 'Unknown';
    }
  }

  handlePageEvent(event: PageEvent): void {
    this.pageNumber = event.pageIndex + 1;
    this.getPayments();
  }

  private getPayments() {
    this.paymentService.getPaymentsForLoan(this.loanId, this.pageNumber, this.pageSize).subscribe({
      next: data => this.payments = data
    })
  }
}
