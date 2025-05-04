import { Component, inject } from '@angular/core';
import { AuthService } from '../services/auth/auth.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FileService } from '../services/file/file.service';
import { HttpEventType } from '@angular/common/http';

@Component({
  selector: 'app-action-bar',
  exportAs: "app-action-bar",
  imports: [CommonModule, FormsModule],
  templateUrl: './action-bar.component.html',
  styleUrl: './action-bar.component.css'
})
export class ActionBarComponent {

  constructor(private authService: AuthService, private fileService: FileService) {}

  searchQuery = '';
  selectedOption: string = "";
  options: string[] = [];

  onOptionChange() {
    this.fileService.selectedBucket = this.selectedOption;
    this.searchQuery = "";
    this.fileService.getFiles();
  }

  ngOnInit(){
    var options = this.authService.getBuckets();
    if(options != null){
       this.options = options.split(",");
       this.selectedOption = this.options[0];
       this.onOptionChange();
    }
  }

  onSearch() {
    //Ovo se može unaprijediti da pričeka malo prije slanja requesta
    //tako da ne šalje za svako poslano slovo nego samo kad smo gotovi s pisanjem
    if(this.searchQuery != ""){
      this.fileService.filterFiles(this.searchQuery);    
    }
    else{
      this.fileService.removeFilter();
    }
  }
  

  onFileSelected(event: Event) {

    const input = event.target as HTMLInputElement;
  
    if (input.files && input.files.length > 0) {
      const selectedFiles: File[] = Array.from(input.files); // Convert FileList to array
      selectedFiles.forEach(file => {
        if (file) {
          this.fileService.uploadFile(file).subscribe({
            next: () => {
              this.fileService.addFiles([{name: file.name, fileSize: `${(file.size/1000)} kB`, modifiedOn: new Date()}])
            },
            error: err => {
              console.log(err);
            }
          });
        }
      });
    }

  }

  logout() {
    this.authService.logout();
  }
}
