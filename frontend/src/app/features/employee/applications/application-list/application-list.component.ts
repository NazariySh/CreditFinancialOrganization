import { Component, OnInit } from '@angular/core';
import { LoanService } from '../../../../core/services/loan.service';
import { ApplicationStatus } from '../../../../shared/enums/application-status';
import { LoanApplication } from '../../../../shared/models/loans/loan-application';
import { PagedList } from '../../../../shared/models/paged-list';
import { DateFormatPipe } from '../../../../shared/pipes/date-format.pipe';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatIcon } from '@angular/material/icon';
import { CurrencyPipe, DatePipe } from '@angular/common';
import {MatPaginator, PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-application-list',
  imports: [
    DateFormatPipe,
    CurrencyPipe,
    MatButtonModule,
    MatMenuModule,
    MatIcon,
    MatPaginator
  ],
  providers: [DatePipe],
  templateUrl: './application-list.component.html',
  styleUrl: './application-list.component.scss'
})
export class ApplicationListComponent implements OnInit {
  applications = new PagedList<LoanApplication>();
  pageNumber = 1;
  pageSize = 10;
  
  constructor(
    private readonly loanService: LoanService
  ) {}

  ngOnInit(): void {
    this.getApplications();
  }

  approveLoan(id: string): void {
    this.updateApplicationStatus(id, ApplicationStatus.Approved)
  }

  rejectLoan(id: string): void {
    this.updateApplicationStatus(id, ApplicationStatus.Rejected);
  }

  deleteLoan(id: string, userId: string): void {
    this.loanService.delete(id, userId).subscribe({
      next: () => {
        this.applications.items = this.applications.items.filter(loan => loan.id !== id)
      }
    })
  }

  onSearchChange(): void {
    this.pageNumber = 1;
    this.getApplications();
  }

  handlePageEvent(event: PageEvent): void {
    this.pageNumber = event.pageIndex + 1;
    this.getApplications();
  }

  updateApplicationStatus(id: string, status: ApplicationStatus): void {
    this.loanService.updateApplicationStatus(id, status).subscribe({
      next: () => {
        this.applications.items = this.applications.items.filter(loan => loan.id !== id)
      }
    });
  }

  private getApplications() {
    this.loanService.getAllApplications(this.pageNumber, this.pageSize).subscribe({
      next: applications => this.applications = applications
    });
  }
}
