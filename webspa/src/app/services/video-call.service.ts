import { connect, ConnectOptions, LocalTrack, Room } from 'twilio-video';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ReplaySubject , Observable } from 'rxjs';
import { environment } from '../../environments/environment';

interface AuthToken {
  token: string;
}

@Injectable({ providedIn: 'root' })
export class VideoCallService {
  roomsUpdated$: Observable<boolean>;

  private roomBroadcast = new ReplaySubject<boolean>();

  constructor(private readonly http: HttpClient) {
    this.roomsUpdated$ = this.roomBroadcast.asObservable();
  }

  private async getAuthToken() {
    const auth =
      await this.http
        .get<AuthToken>(`${environment.apiUrl}/api/auth/token`)
        .toPromise();

    return auth.token;
  }

  async joinOrCreateRoom(name: string, tracks: LocalTrack[]) {
    let room: Room = null;
    try {
      const token = await this.getAuthToken();
      room =
        await connect(
          token, {
            name,
            tracks,
            dominantSpeaker: true
          } as ConnectOptions);
    } catch (error) {
      console.error(`Unable to connect to Room: ${error.message}`);
    } finally {
      if (room) {
        this.roomBroadcast.next(true);
      }
    }

    return room;
  }

  nudge() {
    this.roomBroadcast.next(true);
  }
}