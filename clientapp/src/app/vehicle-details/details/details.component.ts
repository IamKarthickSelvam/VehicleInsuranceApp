import { Component } from '@angular/core';
import { VehicleDetailsComponent } from '../vehicle-details.component';
import { VehicleService } from 'src/app/_services/vehicle.service';
import { HttpClient, HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-details',
  templateUrl: './details.component.html',
  styleUrls: ['./details.component.css']
})
export class DetailsComponent {
  vehicle: any;
  //hardcoded vehicle models for now
  // vehicleModels: string[] = ['Aprilia RSV4', 'BMW M100R', 'Ducati Panigale V4R', 'Honda CBR-1000RR-R', 'Kawasaki ZX-10R', 'Suzuki GSX-R1000', 'Yamaha R1M'];
  vehicleStatus: string[] = ['Not Expired', 'Expired in the last 90 days', 'Expired for more than 90 days'];
  vehicleModels: string[] = [];

  constructor(
    private vehicleService: VehicleService,
    private vehicleDetails: VehicleDetailsComponent,
    private http: HttpClient) {
    this.getDetails();
    this.getVehicleList();
  }

  getDetails() {
    if (sessionStorage.getItem('vehicle') == null) {
      if (this.vehicle == null) {
        this.vehicleService.vehicleDetails$.subscribe({
          next: response => this.vehicle = response
        });
      }
    }
    else {
      this.vehicle = JSON.parse(sessionStorage['vehicle']);
    }
  }

  getVehicleList() {
    this.vehicleService.getVehicleList().subscribe({
        next: response => this.vehicleModels = response,
        error: error => console.log(error),
        complete: () => console.log('List retrieved')
      }
    )
  }

  async updateDetails() {
    console.log(this.vehicle);
    // await this.vehicleService.getPremiumDetails(this.vehicle);
    this.vehicleService.setVehicle(this.vehicle);
    this.changeView();
  }

  changeView() {
    this.vehicleDetails.changeView('plan');
  }

  // ngOnDestroy() {
  //   this.vehicleService.vehicleDetails$.unsubscribe();
  // }

}
