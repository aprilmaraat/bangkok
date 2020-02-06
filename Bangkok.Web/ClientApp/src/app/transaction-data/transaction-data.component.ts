import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { WeatherForecast, Transaction } from '../../models/Transaction';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-transaction-data',
  templateUrl: './transaction-data.component.html'
})
export class TransactionDataComponent {
  public transactions: Observable<Transaction[]>;
  public forecasts: any[];
  public baseUrl: String;
  public options: RequestOption;
  public http: HttpClient;

  constructor(_http: HttpClient
    , @Inject('BASE_URL') _baseUrl: String) {
      this.http = _http;
      this.baseUrl = _baseUrl;

      this.getAllTransactions();
  }

  public getAllTransactions(){
    return this.http.post<Observable<Transaction[]>>(this.baseUrl + 'bangkok/transaction', this.options)
    .subscribe(result => {
      this.transactions = result;
      console.log(this.transactions);
    }, error => console.error(error));
  }
}

export interface RequestOption{
  OptionType: OptionType,
  CurrencyCode: string,
  FromDT: Date,
  ToDT: Date,
  Status: Status
}

export enum Status{
  A = 1,
  R = 2,
  D = 3,
}

export enum OptionType{
  ByCurrency = 1,
  ByDateRange = 2,
  ByStatus = 3,
}


