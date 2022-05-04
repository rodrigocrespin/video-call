import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { VideoCallService } from './video-call.service';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class NotificationListenerService {
  notificationHub: HubConnection;

  constructor(private videoCallService: VideoCallService) {
    const builder =
      new HubConnectionBuilder()
        .configureLogging(LogLevel.Information)
        .withUrl(`${environment.apiUrl}/notificationHub`);

    this.notificationHub = builder.build();
    this.notificationHub.on('RoomsUpdated', async updated => {
      if (updated) {
        this.videoCallService.nudge();
      }
    });
    this.notificationHub.start();
  }
}