import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { PagedList } from '../../shared/models/paged-list';
import { Loan } from '../../shared/models/loans/loan';
import { Observable } from 'rxjs';
import { LoanApplicationCreate } from '../../shared/models/loans/loan-application-create';
import { ApplicationStatus } from '../../shared/enums/application-status';
import { LoanApplication } from '../../shared/models/loans/loan-application';

@Injectable({
  providedIn: 'root'
})
export class LoanService {
  private readonly baseUrl = `${environment.apiUrl}/loans`;

  constructor(
    private readonly http: HttpClient
  ) {}

  getAll(
    pageNumber: number,
    pageSize: number,
    searchTerm?: string): Observable<PagedList<Loan>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }

    return this.http.get<PagedList<Loan>>(`${this.baseUrl}`, { params });
  }

  getById(id: string): Observable<Loan> {
    return this.http.get<Loan>(`${this.baseUrl}/${id}`);
  }

  applyForLoan(application: LoanApplicationCreate): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}`, application);
  }

  updateApplicationStatus(
    id: string,
    status: ApplicationStatus): Observable<void> {
    const params = new HttpParams().set('status', status)
    return this.http.patch<void>(`${this.baseUrl}/employee/${id}`, null, {params});
  }

  delete(id: string, userId: string): Observable<void> {
    const params = new HttpParams().set('userId', userId)
    return this.http.delete<void>(`${this.baseUrl}/employee/${id}`, {params});
  }

  getCustomerLoans(
    userId: string,
    pageNumber: number,
    pageSize: number,
    searchTerm?: string): Observable<PagedList<Loan>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }

    return this.http.get<PagedList<Loan>>(`${this.baseUrl}/employee/${userId}`, {params});
  }

  getByIdCustomerLoan(id: string, userId: string): Observable<Loan> {
    const params = new HttpParams().set('userId', userId);
    return this.http.get<Loan>(`${this.baseUrl}/employee/${id}`, {params});
  }

  getAllApplications(
    pageNumber: number,
    pageSize: number): Observable<PagedList<LoanApplication>> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    return this.http.get<PagedList<LoanApplication>>(`${this.baseUrl}/employee/applications`, {params});
  }
}
