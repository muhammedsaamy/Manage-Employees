import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FileService {

  constructor(private http: HttpClient) { }
  file =new BehaviorSubject<File|null>(null);
  getFile=this.file.asObservable();

  uploadeFile(file: any)
  {
    // this.file.next();
    // this.file.subscribe(()=)
   return this.http.post('https://localhost:7081/file/upload', file);
 }

 downloadeFile(fileName: any)
 {
  return this.http.get('https://localhost:7081/file/upload/'+fileName);
}

}
