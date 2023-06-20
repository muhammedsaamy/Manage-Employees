import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, OnInit } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService implements OnInit{

  constructor(private http: HttpClient) { }
  ngOnInit(): void {
  }

  getEmployees()
  {
    return this.http.get('https://localhost:7081/employee');
  }

  getEmployee(id :number)
  {
    return this.http.get('https://localhost:7081/employee/' + id);
  }

  getEmployeeByEmailOrMobile(email: string, mobile: string): Observable<any[]> {
    return this.http.get<any[]>(`https://localhost:7081/employee/search?email=${email}&mobile=${mobile}`);
  }



  createEmployee(employee: FormData)
   {
    return this.http.post('https://localhost:7081/employee', employee);
  }

  updateEmployee(id: number, employee: FormData)
   {
    return this.http.put<void>('https://localhost:7081/employee/' + id, employee);
  }

  deleteEmployee(id: number)
   {
    return this.http.delete<void>('https://localhost:7081/employee/' + id);
  }

  deleteEmployees(ids: number[]) {
    // const params = new HttpParams().set('ids', ids.join(','));
    return this.http.delete<void>('https://localhost:7081/employee/deleteSelected', { body:ids });
  }

}
