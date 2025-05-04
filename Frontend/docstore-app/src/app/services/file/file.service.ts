import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { HttpClient, HttpEvent, HttpRequest } from '@angular/common/http';
import { FileModel } from '../../models/FileModel';
import { AuthService } from '../auth/auth.service';

@Injectable({
  providedIn: 'root',
})

export class FileService {
  
  uploadUrl: string = "http://localhost:5231/api/files"; //Obicno bi islo u enviroment variablu
  selectedBucket: string = "";

  fileSubject = new BehaviorSubject<FileModel[]>([]);
  files$ = this.fileSubject.asObservable();
  private files: FileModel[] = [];

  constructor(private http: HttpClient, private auth: AuthService) {}
  
  filterFiles(filter: string){
    filter = filter.toLowerCase();
    this.files = this.files.filter(x => {var lowercase = x.name.toLowerCase(); return lowercase.includes(filter)});
    this.fileSubject.next(this.files);
  }

  removeFilter(){
    this.getFiles();
  }

  uploadFile(file: File): Observable<HttpEvent<any>> {
    const formData = new FormData();
    formData.append('file', file);
    const req = new HttpRequest('POST',`${this.uploadUrl}/${this.selectedBucket}/${this.auth.getUsername()}/${file.name}`, formData);
    return this.http.request(req);
  }

  clearFiles(){
    this.files = [];
    this.fileSubject.next(this.files);
  }

  removeFile(name: string){
    var lower = name.toLowerCase();
    this.files = this.files.filter(x => x.name.toLowerCase() != lower);
    this.fileSubject.next(this.files);
  }

  addFiles(files: FileModel[]){

    //Nije efikasno kad je puno fileova, ali ovo radim s pretpostavkom
    //da se može po potrebi ubaciti paging još.
    for(var file of files){
        var index = this.files.findIndex(x => x.name.toLowerCase() == file.name.toLowerCase());

        if(index != -1){
            this.files[index] = file;
        }
        else{
            this.files.push(file);
        }
    }

    this.fileSubject.next(this.files);
  }

  getFiles(): void {

    this.clearFiles();

    if(this.selectedBucket != ""){
        this.http.get<FileModel[]>(`${this.uploadUrl}/${this.selectedBucket}?prefix=${this.auth.getUsername()}/`).subscribe({
            next: (data: FileModel[]) => {
                this.addFiles(data);
            },
            error: (err) => {
                console.error(err);
                this.files = [];
            }
        })
    }
  }

  deleteFiles(files: FileModel[]): void {
    for(var file of files){
        this.http.delete(`${this.uploadUrl}/${this.selectedBucket}/${this.auth.getUsername()}/${file.name}`, { responseType: 'blob' }).subscribe({
            next: () => {
                this.removeFile(file.name);
            },
            error: (err) => {
                alert("Failed deleting file");
                console.error(err);
            }
        })
    }
  }

  fetchFileBlob(file: FileModel): Observable<Blob>{
    return this.http.get(`${this.uploadUrl}/${this.selectedBucket}/${this.auth.getUsername()}/${file.name}`, { responseType: 'blob' });
  }
}
