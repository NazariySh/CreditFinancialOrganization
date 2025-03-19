import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { PagedList } from '../../shared/models/paged-list';
import { Payment } from '../../shared/models/payments/payment';
import { Observable } from 'rxjs';
import { PaymentCreate } from '../../shared/models/payments/payment-create';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  private readonly baseUrl = `${environment.apiUrl}/payments`;

  constructor(
    private readonly http: HttpClient
  ) {}

  getPaymentsForLoan(
    loanId: string,
    pageNumber: number,
    pageSize: number): Observable<PagedList<Payment>> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    return this.http.get<PagedList<Payment>>(`${this.baseUrl}/loans/${loanId}`, { params });
  }

  getById(id: string): Observable<Payment> {
    return this.http.get<Payment>(`${this.baseUrl}/${id}`);
  }

  create(paymentCreate: PaymentCreate): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}`, paymentCreate);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/employee/${id}`);
  }
}
