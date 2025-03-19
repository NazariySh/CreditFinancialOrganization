import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TextareaFormFieldComponent } from './textarea-form-field.component';

describe('TextareaFormFieldComponent', () => {
  let component: TextareaFormFieldComponent;
  let fixture: ComponentFixture<TextareaFormFieldComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TextareaFormFieldComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TextareaFormFieldComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
