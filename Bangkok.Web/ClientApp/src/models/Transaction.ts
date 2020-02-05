export interface Transaction {
  ID: string;
  Amount: number;
  temperatureF: number;
  summary: string;
}

export interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}