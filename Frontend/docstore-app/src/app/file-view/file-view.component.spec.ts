import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FileViewComponent } from './file-view.component';

describe('FileViewComponent', () => {
  let component: FileViewComponent;
  let fixture: ComponentFixture<FileViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FileViewComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FileViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
