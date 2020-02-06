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
  public transaction: Observable<any[]>;
  public options: RequestOption;
  public forecasts: any[];

  constructor(http: HttpClient
    , @Inject('BASE_URL') baseUrl: String) {
      // this.options.OptionType = 1;
      // this.options.CurrencyCode = 'USD';
      // this.options.Status = 1;
      http.post<Observable<WeatherForecast[]>>(baseUrl + 'bangkok/transaction', this.options)
        .subscribe(result => {
          this.transaction = result;
          console.log(result);
        }, error => console.error(error));
      // http.get<Observable<WeatherForecast[]>>(baseUrl + 'weatherforecast')
      //   .subscribe(result => {
      //     this.transaction = result;
      //     console.log(result);
      //   }, error => console.error(error));
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


