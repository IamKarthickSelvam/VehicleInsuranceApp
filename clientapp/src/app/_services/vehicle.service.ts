import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, lastValueFrom } from 'rxjs';
import { Vehicle } from '../_models/vehicle';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class VehicleService {
  private vehicleDetails = new BehaviorSubject<Vehicle | null>(null);
  vehicleDetails$ = this.vehicleDetails.asObservable();
  vehicle: any;
  vehicleList: string[] = [];
  baseUrl: string = 'https://localhost:7224/api/Vehicle';
  headers = { headers: new Headers({ 'Content-Type': 'application/json' })}

  constructor(private http: HttpClient) { }

  getVehicleList(): Observable<any> {
    return this.http.get<any>(this.baseUrl);
  }

  getPremiumDetails(model: Vehicle): Observable<any> {
    console.log('getPremiumDetails');
    return this.http.post<Vehicle>(this.baseUrl + '/calculate', model);
  }

  generatePolicy(model: Vehicle): Observable<any> {
    return this.http.post(this.baseUrl + '/pdf', model, {
      observe: 'response',
      responseType: 'blob'
    });
  }

  setVehicle(vehicle: Vehicle) {
    this.vehicleDetails.next(vehicle);
    sessionStorage.setItem('vehicle', JSON.stringify(vehicle));
  }

  getVehicleDetails() {
    return this.vehicleDetails$;
  }

}
