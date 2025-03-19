import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { LoanType } from '../../shared/models/loans/loan-type';
import { LoanTypeUpdate } from '../../shared/models/loans/loan-type-update';
import { LoanTypeCreate } from '../../shared/models/loans/loan-type-create';

@Injectable({
  providedIn: 'root'
})
export class LoanTypeService {
  private readonly baseUrl = `${environment.apiUrl}/loanTypes`;

  constructor(
    private readonly http: HttpClient
  ) {}

  getAll(): Observable<LoanType[]> {
    return this.http.get<LoanType[]>(this.baseUrl);
  }

  getById(id: string): Observable<LoanType> {
    return this.http.get<LoanType>(`${this.baseUrl}/${id}`);
  }

  create(loanType: LoanTypeCreate): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/admin`, loanType);
  }

  update(id: string, loanType: LoanTypeUpdate): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/admin/${id}`, loanType);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/admin/${id}`);
  }
}
