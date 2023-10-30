import { Component, EventEmitter } from '@angular/core';
import { VehicleService } from '../_services/vehicle.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {

  constructor(private vehicleService: VehicleService, private router: Router,
    private route: ActivatedRoute) { }

  card = false;
  details = false;
  vehicle: any = {};
  selectedUrl: any;
  regNoRegex = new RegExp(/^[A-Z]{2}[ -][0-9]{1,2}(?: [A-Z])?(?: [A-Z]*)? [0-9]{4}$/);
  regNoRegex2 = new RegExp(/^[A-Z]{2}[ -][0-9]{1,2}(?: [A-Z])?(?: [A-Z]*)? [0-9]{4}$/);
  regNoFlag = false;

  changeVehicleCard(value: string) {
    this.card = true;
    this.vehicle.type = value;
    this.vehicle.img = `./assets/img/${this.vehicle.type}.jpg`;

    switch(this.vehicle.type) {
      case "Car": {
        this.selectedUrl = '/car-details';
        break;
      }
      default: {
        this.selectedUrl = '/vehicle-details/details';
        break;
      }
    }
  }

  //FIX OR REMOVE LATER
  regNoValidation(regNo: string) {
    // if (this.regNoRegex.test(regNo) == true){
    //   console.log('TRUE');
    //   this.regNoFlag = true;
    //   return true;
    // }
    // else if (this.regNoRegex2.test(regNo) == true) {
    //   this.regNoFlag = true;
    //   return true;
    // }
    // console.log('FALSE');
    // this.regNoFlag = true;
    return true;
  }

  changeView() {
    if (this.regNoValidation(this.vehicle.regNo)){
      this.vehicleService.setVehicle(this.vehicle);
      console.log(this.selectedUrl);
      this.regNoFlag = true;
      this.router.navigate([this.selectedUrl], {relativeTo: this.route});
    }
    else {
      this.regNoFlag = false;
    }
  }
}
