import { Component, OnInit, Output, EventEmitter, Inject } from '@angular/core';
import { HttpEventType, HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html'
})
export class UploadComponent implements OnInit {
  public progress: number;
  public message: string;
  public baseUrl: String;
  public http: HttpClient;

  @Output() public onUploadFinished = new EventEmitter();

  public loading = true;

  constructor(_http: HttpClient
    , @Inject('BASE_URL') _baseUrl: String) {
        this.http = _http;
        this.baseUrl = _baseUrl + 'api/transaction';
        this.loading = false;
    }

  ngOnInit() {
  }

  public uploadFile = (files) => {
    if (files.length === 0) {
      return;
    }

    this.loading = true;

    let fileToUpload = <File>files[0];
    const formData = new FormData();
    formData.append('file', fileToUpload, fileToUpload.name);

    this.http.post(this.baseUrl + '/upload', formData, {reportProgress: true, observe: 'events'})
      .subscribe(event => {
        if (event.type === HttpEventType.UploadProgress)
          this.progress = Math.round(100 * event.loaded / event.total);
        else if (event.type === HttpEventType.Response) {
          this.message = 'Upload success.';
          this.onUploadFinished.emit(event.body);
        }
        this.loading = false;
      });
  }
}
