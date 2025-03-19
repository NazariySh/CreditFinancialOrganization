import { DatePipe } from '@angular/common';
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'dateFormat'
})
export class DateFormatPipe implements PipeTransform {
  private readonly defaultFormat = 'dd.MM.yyyy';

  constructor(
    private datePipe: DatePipe
  ) {}

  transform(value?: Date, format: string = this.defaultFormat): string | null  {
    if (!value) {
      return null;
    }

    const localDate = new Date(value);
    return this.datePipe.transform(localDate, format);
  }
}
