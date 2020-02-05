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

  constructor(http: HttpClient
    , @Inject('BASE_URL') baseUrl: String) {
    http.get<Observable<WeatherForecast[]>>(baseUrl + 'weatherforecast')
      .subscribe(result => {
        this.transaction = result;
      }, error => console.error(error));
  }
}


