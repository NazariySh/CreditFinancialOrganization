import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DateFormFieldComponent } from './date-form-field.component';

describe('DateFormFieldComponent', () => {
  let component: DateFormFieldComponent;
  let fixture: ComponentFixture<DateFormFieldComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DateFormFieldComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DateFormFieldComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
