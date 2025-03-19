import { Component, OnInit } from '@angular/core';
import { LoanType } from '../../../../shared/models/loans/loan-type';
import { LoanTypeService } from '../../../../core/services/loan-type.service';
import { MatButtonModule } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-loan-type-list',
  imports: [
    RouterLink,
    MatButtonModule,
    MatIcon,
    MatMenuModule
  ],
  templateUrl: './loan-type-list.component.html',
  styleUrl: './loan-type-list.component.scss'
})
export class LoanTypeListComponent implements OnInit {
  loanTypes: LoanType[] = [];

  constructor(
    private readonly loanTypeService: LoanTypeService
  ) {}

  ngOnInit(): void {
    this.loanTypeService.getAll().subscribe({
      next: loanTypes => this.loanTypes = loanTypes
    });
  }

  deleteType(id: string) {
    this.loanTypeService.delete(id).subscribe({
      next: () => this.loanTypes = this.loanTypes.filter(type => type.id !== id)
    });
  }
}
