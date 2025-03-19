import { Self } from "@angular/core";
import { ControlValueAccessor, FormControl, NgControl } from "@angular/forms";

export abstract class BaseFormFieldValueAccessor implements ControlValueAccessor {

  constructor(@Self() public controlDir: NgControl) {
    this.controlDir.valueAccessor = this;
  }

  private _value: any = '';
  private onChange: (value: any) => void = () => {};
  private onTouched: () => void = () => {};
  private disabled = false;

  writeValue(obj: any): void {
    if (obj !== undefined && obj !== null) {
      this._value = obj;
    }
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  handleChange(value: any): void {
    this._value = value;
    this.onChange(value);
  }

  handleBlur(): void {
    this.onTouched();
  }

  get control(): FormControl {
    return this.controlDir.control as FormControl;
  }
}
