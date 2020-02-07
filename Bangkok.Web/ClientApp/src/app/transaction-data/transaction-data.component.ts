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
  public baseUrl: string;
  public options: RequestOption;
  public http: HttpClient;
  public loading = true;

  constructor(_http: HttpClient
    , @Inject('BASE_URL') _baseUrl: string) {
      this.http = _http;
      this.baseUrl = _baseUrl + 'api/transaction';

      this.getAllTransactions();
  }

  public getAllTransactions(){
    let options = {
      OptionType: 4
    };

    return this.http.post<Observable<Transaction[]>>(this.baseUrl, options)
    .subscribe(result => {
      this.transactions = result;
      this.loading = false;
    }, error => console.error(error));
  }

  public getByCurrency(currency: string){
    let options = {
      OptionType: 1,
      CurrencyCode: currency
    };

    return this.http.post<Observable<Transaction[]>>(this.baseUrl, options)
    .subscribe(result => {
      this.transactions = result;
      console.log(this.transactions);
    }, error => console.error(error));
  }

  public uploadFinished(event: any){
    this.loading = true;
    this.getAllTransactions();
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


