import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { WeatherForecast, Transaction } from '../../models/Transaction';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
  public object: Observable<any[]>;

  constructor(http: HttpClient
    , @Inject('BASE_URL') baseUrl: String) {
    http.get<Observable<WeatherForecast[]>>(baseUrl + 'weatherforecast')
      .subscribe(result => {
        this.object = result;
      }, error => console.error(error));
  }
}


