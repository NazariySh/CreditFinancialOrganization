import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Login } from '../../shared/models/auth/login';
import { Observable, tap, throwError } from 'rxjs';
import { Token } from '../../shared/models/auth/token';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly baseUrl = `${environment.apiUrl}/auth`;
  private readonly accessTokenKey = 'accessToken';

  constructor(
    private readonly http: HttpClient
  ) {}

  login(login: Login): Observable<Token> {
    return this.http.post<Token>(`${this.baseUrl}/login`, login).pipe(
      tap(token => this.saveToken(token))
    );
  }

  refresh(): Observable<Token> {
    const accessToken = this.getAccessToken();

    if (accessToken) {
      return this.http.post<Token>(`${this.baseUrl}/refresh`, accessToken).pipe(
        tap(token => this.saveToken(token))
      );
    }

    return throwError(() => 'No tokens found for refresh');
  }

  logout(): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/logout`, null).pipe(
      tap(() => this.clearTokens())
    );
  }

  getAuthState(): Observable<{isAuthenticated: boolean}> {
    return this.http.get<{isAuthenticated: boolean}>(`${this.baseUrl}/auth-state`);
  }

  getAccessToken(): string | null {
    return localStorage.getItem(this.accessTokenKey);
  }

  private saveToken(token: Token): void {
    localStorage.setItem(this.accessTokenKey, token.accessToken);
  }

  private clearTokens(): void {
    localStorage.removeItem(this.accessTokenKey);
  }
}
