import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RepositorioMusicasService {
  http: any;
  baseUrl: string;


  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) { 
    this.http = http;
    this.baseUrl = baseUrl;
  }

  public getMusicasRepertorio() {
    return this.http.get(this.baseUrl + 'api/Repertorio');
  }
}


