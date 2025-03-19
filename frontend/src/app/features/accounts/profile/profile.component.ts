import { Component, OnInit } from '@angular/core';
import { User } from '../../../shared/models/users/user';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { AccountService } from '../../../core/services/account.service';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { CommonModule, DatePipe } from '@angular/common';
import { DateFormatPipe } from '../../../shared/pipes/date-format.pipe';

@Component({
  selector: 'app-profile',
  imports: [
    CommonModule,
    DateFormatPipe,
    MatCardModule,
    MatButtonModule,
    MatIcon
  ],
  providers: [DatePipe],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit {
  user!: User | null;

  constructor(
    private readonly router: Router,
    private readonly authService: AuthService,
    private readonly accountService: AccountService
  ) {}

  ngOnInit(): void {
    this.user = this.accountService.currentUser();
  }

  deleteProfile(): void {
    this.accountService.deleteProfile().subscribe({
      next: () => this.authService.logout().subscribe({
        next: () => this.router.navigateByUrl('/auth/login')
      })
    })
  }
}
