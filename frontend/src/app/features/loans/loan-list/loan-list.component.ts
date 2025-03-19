import { Component, OnInit } from '@angular/core';
import { PagedList } from '../../../shared/models/paged-list';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { LoanService } from '../../../core/services/loan.service';
import { LoanStatus } from '../../../shared/enums/loan-status';
import { Loan } from '../../../shared/models/loans/loan';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DateFormatPipe } from '../../../shared/pipes/date-format.pipe';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { RouterLink } from '@angular/router';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-loan-list',
  imports: [
    MatInputModule,
    FormsModule,
    DateFormatPipe,
    CurrencyPipe,
    RouterLink,
    MatFormFieldModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatMenuModule,
    MatIcon,
    MatPaginator
  ],
  providers: [DatePipe],
  templateUrl: './loan-list.component.html',
  styleUrl: './loan-list.component.scss'
})
export class LoanListComponent implements OnInit {
  loans = new PagedList<Loan>();
  pageNumber = 1;
  pageSize = 10;
  searchTerm = '';

  constructor(
    private readonly loanService: LoanService
  ) {}

  ngOnInit(): void {
    this.getLoans();
  }

  onSearchChange(): void {
    this.pageNumber = 1;
    this.getLoans();
  }

  handlePageEvent(event: PageEvent): void {
    this.pageNumber = event.pageIndex + 1;
    this.getLoans();
  }

  statusText(status: LoanStatus): string {
    return LoanStatus[status];
  }

  private getLoans() {
    this.loanService.getAll(this.pageNumber, this.pageSize, this.searchTerm).subscribe({
      next: data => this.loans = data
    });
  }
}
