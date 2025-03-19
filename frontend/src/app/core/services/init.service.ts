import { Injectable } from '@angular/core';
import { AccountService } from './account.service';
import { catchError, forkJoin, Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class InitService {

  constructor(
    private readonly accountService: AccountService
  ) {}

  initialize(): Observable<unknown> {
    return forkJoin({
      user: this.accountService.getProfile().pipe(
        catchError((error) => {
          console.error('Error loading user profile:', error);
          return of(null);
        })
      )
    });
  }
}
