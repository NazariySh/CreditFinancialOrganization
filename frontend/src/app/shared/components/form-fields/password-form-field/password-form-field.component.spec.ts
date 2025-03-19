import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PasswordFormFieldComponent } from './password-form-field.component';

describe('PasswordFormFieldComponent', () => {
  let component: PasswordFormFieldComponent;
  let fixture: ComponentFixture<PasswordFormFieldComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PasswordFormFieldComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PasswordFormFieldComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
