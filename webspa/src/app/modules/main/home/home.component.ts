import { ChangeDetectionStrategy, Component } from '@angular/core';
import { NotificationListenerService } from '../../../services/notification-listener.service';

@Component({
  templateUrl: './home.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HomeComponent {
  constructor(private notificationService: NotificationListenerService) {
  }
}