import { computed, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { User } from '../../shared/models/users/user';
import { RoleType } from '../../shared/enums/role-type';
import { HttpClient } from '@angular/common/http';
import { Register } from '../../shared/models/auth/register';
import { Observable, tap } from 'rxjs';
import { Address } from '../../shared/models/users/address';
import { ChangePassword } from '../../shared/models/users/change-password';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private readonly baseUrl = `${environment.apiUrl}/accounts`;

  currentUser = signal<User | null>(null);
  isAdmin = computed(() => {
    const roles = this.currentUser()?.roles;
    return roles?.includes(RoleType[RoleType.Admin]) ?? false;
  });
  isEmployee = computed(() => {
    const roles = this.currentUser()?.roles;
    return roles?.includes(RoleType[RoleType.Employee]) ?? false;
  });

  constructor(
    private readonly http: HttpClient
  ) {}

  register(register: Register): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/register`, register);
  }

  deleteProfile(): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/profile`);
  }

  getProfile(): Observable<User> {
    return this.http.get<User>(`${this.baseUrl}/profile`).pipe(
      tap(user => this.currentUser.set(user))
    );
  }

  updateAddress(address: Address): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/address`, address).pipe(
      tap(() => {
        const user = this.currentUser();

        if (user) {
          const updatedUser = { ...user, address };
          this.currentUser.set(updatedUser);
        }
      })
    );
  }

  changePassword(changePassword: ChangePassword): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/reset-password`, changePassword);
  }

  registerWithRole(register: Register, roleName: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/admin/register/${roleName}`, register);
  }
}
