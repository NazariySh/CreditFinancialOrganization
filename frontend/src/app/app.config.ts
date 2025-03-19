import { APP_INITIALIZER, ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { refreshTokenInterceptor } from './core/interceptors/refresh-token.interceptor';
import { errorInterceptor } from './core/interceptors/error.interceptor';
import { InitService } from './core/services/init.service';
import { firstValueFrom } from 'rxjs';

function initializeApp(initService: InitService) {
  return () => firstValueFrom(initService.initialize()).finally(() => {
    const splash = document.getElementById('initial-splash');
    if (splash) {
      splash.remove();
    }
  })
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([
        authInterceptor,
        refreshTokenInterceptor,
        errorInterceptor
      ])
    ),
    {
      provide: APP_INITIALIZER,
      useFactory: initializeApp,
      multi: true,
      deps: [InitService]
    }
  ]
};
