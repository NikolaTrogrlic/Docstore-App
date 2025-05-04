import { Component } from '@angular/core';
import { ActionBarComponent } from "../action-bar/action-bar.component";
import { FileModel } from '../models/FileModel';
import { FileService } from '../services/file/file.service';
import { FileViewComponent } from "../file-view/file-view.component";
import { FileModalComponent } from "../file-modal/file-modal.component";

@Component({
  selector: 'app-dashboard',
  imports: [ActionBarComponent, FileViewComponent, FileModalComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent {

  files: FileModel[] = [];
  selectedFile: FileModel | null = null;
  isModalVisible = false;

  constructor(private fileService: FileService) {
  }

  ngOnInit() {
    this.fileService.files$.subscribe(files => {
      this.files = files;
    });

    this.fileService.getFiles();
  }

  onViewDetails(file: FileModel) {
    this.selectedFile = file;
    this.isModalVisible = true;
  }

  onCloseModal() {
    this.isModalVisible = false;
    this.selectedFile = null;
  }
}
