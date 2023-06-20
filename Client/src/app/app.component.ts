import { Component, OnInit, ViewChild, AfterViewInit, ElementRef, ChangeDetectorRef } from '@angular/core';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { EmployeeComponent } from './employee/employee.component';
import { EmployeeService } from './services/employee.service';
import {MatPaginator} from '@angular/material/paginator';
import {MatSort} from '@angular/material/sort';
import {MatTableDataSource} from '@angular/material/table';
import { SnackbarService } from './services/snackbar.service';
import {SelectionModel} from '@angular/cdk/collections';
import { NgxSpinnerService } from "ngx-spinner";
import { DeleteDialogComponent } from './delete-dialog/delete-dialog.component';
import { HttpClient } from '@angular/common/http';
import { FileService } from './services/file.service';



@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit, AfterViewInit{

  displayedColumns: string[] = ['select', 'name', 'email', 'mobile', 'address', 'View','action'];
  dataSource: MatTableDataSource<any> = new MatTableDataSource();
  selection = new SelectionModel<any>(true, []);
  // selectthing?: number

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild('input') filterInput! : ElementRef
  employees:any
  filterEvent!:Event;
  filterString =''
  fileName: any;
  selectedFile!:File;

  constructor(private _matDialog : MatDialog,
      private EmployeeService: EmployeeService,
       private snackbarService: SnackbarService,
       private spinner: NgxSpinnerService,
       private cdr: ChangeDetectorRef,
       private http:HttpClient,
       private fileServices: FileService) { }
  ngAfterViewInit(): void {
    this.sort.disableClear= true;
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;

  }
  ngOnInit(): void {
   this.getEmployeeList();
   this.spinner.show();

  }


    isAllSelected() {
      const numSelected = this.selection.selected.length;
      // this.selectthing = numSelected;
      const numRows = this.dataSource.filteredData.length;
      return numSelected === numRows;
    }


        /** Selects all rows if they are not all selected; otherwise clear selection. */
  toggleAllRows() {
    if (this.isAllSelected()) {
      this.selection.clear();
      return;
    }

    this.selection.select(...this.dataSource.data);
  }


  checkboxLabel(row?: any): string {
    if (!row) {
      return `${this.isAllSelected() ? 'Deselect' : 'Select'} all`;
    }
    return `${this.selection.isSelected(row) ? 'Deselect' : 'Select'} row ${row.position + 1}`;
  }





  openAddEditeEmpForm() {
    const dialogConfig = new MatDialogConfig();

    const dialogRef = this._matDialog.open(EmployeeComponent, {
      width: '500px',
      // position: { top: '100px', left: '420px' },
});

    dialogRef.afterOpened().subscribe({
      next:(val:any) => {
        if (val) {
          this.getEmployeeList();
        }

      }
    });

    dialogRef.afterClosed().subscribe((val) => {
      if (val) {
        this.getEmployeeList();
      }
    });
  }




  getEmployeeList(){
    this.EmployeeService.getEmployees().subscribe({
      next:(res:any) => {
        console.log(res)
        this.employees = res;

        this.employees.sort((a:any,b:any)=>{
          if(a.name){
            if (a.name.toLowerCase().trim() > b.name.toLowerCase().trim()) {
              return 1;
            }
            if (a.name.toLowerCase().trim() < b.name.toLowerCase().trim()) {
              return -1;
            }
          }

          return 0;
        })
        console.log(this.employees)
        this.dataSource = new MatTableDataSource(this.employees)
        this.dataSource.sort = this.sort;
        this.dataSource.paginator = this.paginator;
        if(this.filterEvent){
          this.applyFilter(this.filterEvent);

        }
      },
      error:(err) => {console.log(err)},
      complete:() => {
        this.spinner.hide();
      }
    })

  }

  applyFilter(event: Event) {

    const filterValue = (event.target as HTMLInputElement).value;
    this.filterString = filterValue;
    this.dataSource.filter = filterValue.trim().toLowerCase();
    this.filterEvent=event;

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }


  deleteEmployee(id: number) {
    const dialogRef = this.openDialog();

    dialogRef.afterClosed().subscribe(result => {
      if (result === 'yes') {
        this.EmployeeService.deleteEmployee(id).subscribe({
          next: () => {
            this.snackbarService.openSnackBar('Employee deleted!', 'Done');
            this.getEmployeeList();
          },
          error: (err) => {
            console.log(err);
          },
          complete: () => { this.selection.clear();}
        });
      }
    });
  }





  openDialog(enterAnimationDuration: string = '0', exitAnimationDuration: string = '0'): MatDialogRef<DeleteDialogComponent> {
    return this._matDialog.open(DeleteDialogComponent, {
      width: '300px',
      enterAnimationDuration,
      exitAnimationDuration,
      position: { top: '0px', left: '480px' },

    });
  }





  deleteEmployees() {
    const selectedIds = this.selection.selected.map(employee => employee.id);

    const dialogRef = this.openDialog();

    dialogRef.afterClosed().subscribe(result => {
      console.log(result)
      if (result === 'yes') {
        this.EmployeeService.deleteEmployees(selectedIds).subscribe({
          next: () => {
            alert('Employees deleted!');
            this.snackbarService.openSnackBar('Employees deleted!', 'Done');
            this.selection.clear();
            this.getEmployeeList();
          },
          error: (err) => {
            console.log(err);
            console.log(result)

          }
        });
      }
    });
  }




  openEditeEmpForm(data: any){
    // data.fileData=this.selectedFile;
    const dilaogRef = this._matDialog.open(EmployeeComponent, {
      data,
      width: '500px',

    });


this.fileName = data.file
// console.log(this.fileName)
this.http.get(`https://localhost:7081/employee/download/${this.fileName}`, { responseType: 'blob' })
.subscribe((fileData: Blob) => {
  const file = new File([fileData], this.fileName);
  this.selectedFile = file;

this.fileServices.file.next(this.selectedFile);





  // this.employeeForm.patchValue({ fileName: file }); // Set the file input value
});


    dilaogRef.afterClosed().subscribe({
      next: (val) => {
        if(val){
          this.getEmployeeList();
          // this.filterInput.nativeElement.value = ''
            // this.applyFilter(this.filterEvent);
            this.dataSource.filter = this.filterString.trim().toLowerCase();

            if (this.dataSource.paginator) {
              this.dataSource.paginator.firstPage();
            }



        }
      }
    })


    // this.EmployeeService.updateEmployee(data.id, data).subscribe({
    //   next: (res) => {
    //     console.log(res);
    //     this.getEmployeeList();
    //   },
    //   error: (err) => {
    //     console.log(err);
    //   }
    // })
  }




downloadFile(fileName: string) {
  const url = `https://localhost:7081/employee/download/${fileName}`;

  // Make an HTTP GET request to download the file
  this.http.get(url, { responseType: 'blob' }).subscribe((response: any) => {
      const link = document.createElement('a');
  link.href = url;
  link.target = '_blank'; // Open in a new tab or window

  // Trigger the download by programmatically clicking the link
  link.click();

  });
}








}
