import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FileModel } from '../models/FileModel';
import { FileService } from '../services/file/file.service';
import { FileExtensionColorPipe } from "../pipes/file-extension-color.pipe";

@Component({
  selector: 'file-view',
  imports: [FileExtensionColorPipe],
  templateUrl: './file-view.component.html',
  styleUrl: './file-view.component.css'
})
export class FileViewComponent {

  @Input() file!: FileModel;
  @Output() click = new EventEmitter<FileModel>();

  constructor(private fileService: FileService) {
  }

  onDelete(event: MouseEvent) {
    event.stopPropagation(); 
    this.fileService.deleteFiles([this.file]);
  }

  openDetails(){
    this.click.emit(this.file);
  }
}
