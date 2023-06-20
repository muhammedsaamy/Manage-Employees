
import { Component, ElementRef, Inject, OnInit, AfterViewInit, ViewChild } from '@angular/core';
import { Form, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { EmployeeService } from '../services/employee.service';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { SnackbarService } from '../services/snackbar.service';
import { FileService } from '../services/file.service';

@Component({
  selector: 'app-employee',
  templateUrl: './employee.component.html',
  styleUrls: ['./employee.component.scss']
})
export class EmployeeComponent implements OnInit {

  employeeForm!: FormGroup;
  targetFile!:File
  @ViewChild('fileInput') fileInput: any;
  // selectedFile=this.data.fileData;
  selectedFile?:File | null;
  fileName?:string;
  updatedFile?: File;

  constructor(
    private formBuilder: FormBuilder,
    private employeeService: EmployeeService,
    private dialogRef: MatDialogRef<EmployeeComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private snackbarService: SnackbarService,
    public _elementRef: ElementRef,
    public fileService:FileService
  ) {}

  ngOnInit(): void {
    this.getEmpList();
    this.empForm();
    this.employeeForm.patchValue(this.data);
    if(this.data){
    setTimeout(() => {
      this.fileService.getFile.subscribe((data)=>{

        this.selectedFile=data;
        this.fileName=this.selectedFile?.name
        console.log(this.selectedFile?.name);

        if (this.selectedFile) {
          this.fileName = this.selectedFile.name;

          // Manually set the type property based on the file extension
          let fileType: string;
          if (this.fileName.endsWith('.pdf')) {
            fileType = 'application/pdf';
          } else if (this.fileName.endsWith('.txt')) {
            fileType = 'text/plain';
          } else {
            // Set a default type if the file extension is unknown
            fileType = 'application/octet-stream';
          }

          // Create a new File object with the updated type
          this.updatedFile = new File([this.selectedFile], this.fileName, { type: fileType });
          this.targetFile=this.updatedFile
        }
      }

      )
      console.log(this.updatedFile)

    }, 200);
  }
  }



  getEmpList() {
    this.employeeService.getEmployees().subscribe({
      next: (res) => {
        console.log(res);
      }
    });

  }


  onSubmit() {
    var btns = document.getElementById('buttons');

    btns?.setAttribute('disabled', 'true');

    if (this.employeeForm.valid) {

      const email = this.employeeForm.get('email')?.value;
      const mobile = this.employeeForm.get('mobile')?.value;

      if (this.data) {
        const data = this.employeeForm.value;
        const formData = new FormData();
        formData.append('name', this.employeeForm.get('name')?.value);
        formData.append('email', this.employeeForm.get('email')?.value);
        formData.append('mobile', this.employeeForm.get('mobile')?.value);
        formData.append('address', this.employeeForm.get('address')?.value);
        formData.append('file', this.targetFile);

        console.log(formData);


        this.employeeService.getEmployeeByEmailOrMobile(email, mobile).subscribe({
          next: (res) => {
            if (res.length === 0 || res[0].id === this.data.id) {
              this.updateEmployee();
            } else {
              this.snackbarService.openSnackBar('Email or phone already exists!', 'Error');
              btns?.removeAttribute('disabled');
            }
          },
          error: (err) => {
            console.log(err);
            btns?.removeAttribute('disabled');
          }
        });
      } else {
        const data = this.employeeForm.value;

        this.employeeService.getEmployeeByEmailOrMobile(email, mobile).subscribe({
          next: (res) => {
            if (res.length === 0) {
              this.createEmployee();
            } else {
              this.snackbarService.openSnackBar('Email or phone already exists!', 'Error');
              btns?.removeAttribute('disabled');
            }
          },
          error: (err) => {
            console.log(err);
            btns?.removeAttribute('disabled');
          },
          complete: ()=>{
          }
        });

        const file = this.employeeForm.value.file;
        console.log(file)
        this.fileService.uploadeFile(file)
      }
    }
  }

  updateEmployee() {


    const formData = new FormData();
    formData.append('name', this.employeeForm.get('name')?.value);
    formData.append('email', this.employeeForm.get('email')?.value);
    formData.append('mobile', this.employeeForm.get('mobile')?.value);
    formData.append('address', this.employeeForm.get('address')?.value);
    formData.append('file', this.targetFile);

    this.employeeService.updateEmployee(this.data.id, formData).subscribe({
      next: (res) => {
        // console.log(res);
        this.snackbarService.openSnackBar('Employee updated successfully...', 'Done');
        this.getEmpList();
        this.dialogRef.close(true);
      },
      error: (err) => {
        console.log(err);
      },
      complete: () => {
        var btns = document.getElementById('buttons');
        btns?.removeAttribute('disabled');
      }
    });
  }

  // createEmployee() {
  //   this.employeeService.createEmployee(this.employeeForm.value).subscribe({
  //     next: (res) => {
  //       console.log(res);
  //       this.snackbarService.openSnackBar('Employee added successfully...', 'Done');
  //       this.getEmpList();
  //       this.dialogRef.close(true);
  //     },
  //     error: (err) => {
  //       console.log(err);
  //     },
  //     complete: () => {
  //       var btns = document.getElementById('buttons');
  //       btns?.removeAttribute('disabled');
  //     }
  //   });
  // }

  createEmployee() {
    const formData = new FormData();
    formData.append('name', this.employeeForm.get('name')?.value);
    formData.append('email', this.employeeForm.get('email')?.value);
    formData.append('mobile', this.employeeForm.get('mobile')?.value);
    formData.append('address', this.employeeForm.get('address')?.value);
    formData.append('file', this.targetFile);

    this.employeeService.createEmployee(formData).subscribe({
      next: (res) => {
        console.log(res);
        this.snackbarService.openSnackBar('Employee added successfully...', 'Done');
        this.getEmpList();
        this.dialogRef.close(true);
      },
      error: (err) => {
        console.log(err);
      },
      complete: () => {
        var btns = document.getElementById('buttons');
        btns?.removeAttribute('disabled');
      }
    });
  }



  onFileSelected(event: any) {
    if (event.target.files && event.target.files.length > 0) {
      this.targetFile = event.target.files[0];
      // this.employeeForm.patchValue({ file });
    }
  }


  empForm() {
    if(!this.data){
    this.employeeForm = this.formBuilder.group({
      id: ['0'],
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.pattern(/^[\w.-]+@[a-zA-Z0-9_-]+(?:\.[a-zA-Z0-9_-]+)+$/)]],
      mobile: ['', [Validators.required, Validators.pattern('^(011|012|010)\\d{8}$')]],
      address: ['', Validators.required],
      fileName: ['', Validators.required]
    });
  }else{
    this.employeeForm = this.formBuilder.group({
      id: ['0'],
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.pattern(/^[\w.-]+@[a-zA-Z0-9_-]+(?:\.[a-zA-Z0-9_-]+)+$/)]],
      mobile: ['', [Validators.required, Validators.pattern('^(011|012|010)\\d{8}$')]],
      address: ['', Validators.required],
      fileName: ['']
    });

  }

  }
}

