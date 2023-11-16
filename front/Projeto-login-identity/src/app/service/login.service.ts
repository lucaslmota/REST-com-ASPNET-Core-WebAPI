import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LoginModel } from '../models/LoginModel';
import { environment } from 'src/environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class LoginService {

  baseURL = `${environment.endPoint}logar`;

  constructor(private httpClient: HttpClient) { }

  Logar(login: LoginModel) {
    return this.httpClient.post(`${this.baseURL}/login`, login);
  }
}
