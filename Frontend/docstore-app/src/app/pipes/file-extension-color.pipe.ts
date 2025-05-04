import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'fileExtensionColor'
})
export class FileExtensionColorPipe implements PipeTransform {

  transform(fileName: string): string {
    const ext = this.getExtension(fileName);
    return this.colorFromExtension(ext);
  }

  private getExtension(fileName: string): string {
    const match = fileName.match(/\.([0-9a-z]+)(?:[\?#]|$)/i);
    return match ? match[1].toLowerCase() : 'unknown';
  }

  private colorFromExtension(extension: string): string {
    let hash = 0;
    for (let i = 0; i < extension.length; i++) {
      hash = extension.charCodeAt(i) + ((hash << 5) - hash);
    }
    const hue = Math.abs(hash) % 360;
    return `hsl(${hue}, 60%, 65%)`;
  }

}
