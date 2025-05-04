import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FileModel } from '../models/FileModel';
import { FileService } from '../services/file/file.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Component({
  selector: 'file-modal',
  imports: [],
  templateUrl: './file-modal.component.html',
  styleUrl: './file-modal.component.css'
})
export class FileModalComponent {

  constructor(private fileService: FileService, private sanitizer: DomSanitizer) { }

  @Input() file!: FileModel;
  @Input() visible: boolean = false;
  @Output() close = new EventEmitter<void>();

  downloadedFile: Blob | undefined;
  previewUrl: SafeResourceUrl | null = null;
  loading = false;

  ngOnChanges() {
    if (this.visible && this.file) {
      this.loadFilePreview();
    }
  }

  onDownload() {
    if (!this.downloadedFile) return;

    const url = URL.createObjectURL(this.downloadedFile);
    const a = document.createElement('a');
    a.href = url;
    a.download = this.file.name || 'download';
    document.body.appendChild(a);
    a.click();

    setTimeout(() => {
      document.body.removeChild(a);
      URL.revokeObjectURL(url);
    }, 0);
  }

  loadFilePreview() {
    this.loading = true;
    this.fileService.fetchFileBlob(this.file).subscribe({
      next: (blob) => {
        console.log(blob);
        this.previewUrl = this.sanitizer.bypassSecurityTrustResourceUrl(
          URL.createObjectURL(blob)
        );
        this.downloadedFile = blob;
        this.loading = false;
      },
      error: () => {
        this.previewUrl = null;
        this.loading = false;
      }
    });
  }

  onClose() {
    this.previewUrl = null;
    this.close.emit();
  }
}
