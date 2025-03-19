import { Component } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AccountService } from '../../../../core/services/account.service';
import { AuthService } from '../../../../core/services/auth.service';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatMenuTrigger, MatMenu, MatMenuItem } from '@angular/material/menu';
import { MatDivider } from '@angular/material/divider';
import { IsAdminDirective } from '../../../directives/is-admin.directive';
import { IsEmployeeDirective } from '../../../directives/is-employee.directive';

@Component({
  selector: 'app-header',
  imports: [
    MatIcon,
    MatButton,
    RouterLink,
    RouterLinkActive,
    MatMenuTrigger,
    MatMenu,
    MatDivider,
    MatMenuItem,
    IsAdminDirective,
    IsEmployeeDirective
  ],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {

  get isLoggedIn() {
    return this.authService.getAccessToken() != null;
  }

  get currentUser() {
    return this.accountService.currentUser();
  }

  constructor(
    private readonly router: Router,
    private readonly authService: AuthService,
    private readonly accountService: AccountService
  ) {}

  logout() {
    this.authService.logout().subscribe({
      next: () => {
        this.accountService.currentUser.set(null);
        this.router.navigateByUrl('/auth/login');
      }
    })
  }
}
