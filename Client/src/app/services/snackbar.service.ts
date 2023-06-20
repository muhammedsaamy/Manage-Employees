import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class SnackbarService {

  constructor(private snackbar: MatSnackBar) { }

  openSnackBar(message : string, action:string){
    this.snackbar.open(message, action = 'Ok', {
      duration:2000,
      verticalPosition: 'top',
      panelClass: "snackbar"
    })
  }
}
