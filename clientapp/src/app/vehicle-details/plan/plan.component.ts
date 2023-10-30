import { Component } from '@angular/core';
import { VehicleDetailsComponent } from '../vehicle-details.component';
import { VehicleService } from 'src/app/_services/vehicle.service';
import { Vehicle } from 'src/app/_models/vehicle';
import { lastValueFrom, map } from 'rxjs';

@Component({
  selector: 'app-plan',
  templateUrl: './plan.component.html',
  styleUrls: ['./plan.component.css']
})
export class PlanComponent {
  vehicle: any;
  compPremium: number = 0;
  thirdPremium: number = 0;
  idvEditor: boolean = true;
  selectedYear: number = 0;
  selectedInsType: string = "";

  constructor(private vehicleDetails: VehicleDetailsComponent,
    private vehicleService: VehicleService) {
    this.getDetails();
  }

  async getDetails() {
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

    this.vehicle.regYear = this.vehicle.year.year;

    try {
      const response = await lastValueFrom(this.vehicleService.getPremiumDetails(this.vehicle));
      this.mapVehicleDetails(response);
      this.compPremium = this.vehicle.cprem1Year;
      this.thirdPremium = this.vehicle.tprem1Year;

    } catch (error) {
      console.log(error);
    }

    this.vehicle.finalPremium = this.compPremium;
  }

  mapVehicleDetails(response: any) {
    this.vehicle.cprem1Year = response.cPrem1Year;
    this.vehicle.cprem2Year = response.cPrem2Year;
    this.vehicle.cprem3Year = response.cPrem3Year;
    this.vehicle.tprem1Year = response.tPrem1Year;
    this.vehicle.tprem2Year = response.tPrem2Year;
    this.vehicle.tprem3Year = response.tPrem3Year;
  }

  onYearChange(event: any, year: number) {
    switch (year) {
      case 1:
        this.compPremium = this.vehicle.cprem1Year;
        this.thirdPremium = this.vehicle.tprem1Year;
        if (this.idvEditor) {
          this.vehicle.finalPremium = this.compPremium;
        }
        else {
          this.vehicle.finalPremium = this.thirdPremium;
        }
        break;
      case 2:
        this.compPremium = this.vehicle.cprem2Year;
        this.thirdPremium = this.vehicle.tprem2Year;
        if (this.idvEditor) {
          this.vehicle.finalPremium = this.compPremium;
        }
        else {
          this.vehicle.finalPremium = this.thirdPremium;
        }
        break;
      case 3:
        this.compPremium = this.vehicle.cprem3Year;
        this.thirdPremium = this.vehicle.tprem3Year;
        if (this.idvEditor) {
          this.vehicle.finalPremium = this.compPremium;
        }
        else {
          this.vehicle.finalPremium = this.thirdPremium;
        }
        break;
    }
    this.selectedYear = year;
  }

  onInsTypeChange(event: any, type: string) {
    switch (type) {
      case 'CP':
        this.vehicle.finalPremium = this.compPremium;
        this.idvEditor = true;
        break;
      default:
        this.vehicle.finalPremium = this.thirdPremium;
        this.idvEditor = false;
        break;
    }
    this.selectedInsType = type;
  }

  changeView() {
    this.vehicle.selectedPlan = this.selectedYear + "-" + this.selectedInsType;
    this.vehicleService.setVehicle(this.vehicle);
    this.vehicleDetails.changeView('addon');
  }
}
