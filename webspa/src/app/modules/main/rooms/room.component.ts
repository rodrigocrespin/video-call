import { AfterViewInit, ChangeDetectionStrategy, Component, ViewChild } from '@angular/core';
import { createLocalAudioTrack, Room, LocalTrack, LocalVideoTrack, LocalAudioTrack, RemoteParticipant } from 'twilio-video';
import { VideoCallService } from '../../../services/video-call.service';
import { RoomGridComponent } from './room-grid/room-grid.component';
import { ActivatedRoute, Router } from '@angular/router';
import { filter, first, map, switchMap } from 'rxjs/operators';
import { CameraComponent } from '../camera/camera.component';

@Component({
  templateUrl: './room.component.html',
  styleUrls: ['./room.component.scss'],
  //changeDetection: ChangeDetectionStrategy.OnPush
})
export class RoomComponent implements AfterViewInit {
  activeRoom: Room;

  @ViewChild(CameraComponent) camera: CameraComponent;
  @ViewChild(RoomGridComponent) roomGrid: RoomGridComponent;

  constructor(private route: ActivatedRoute, private router: Router, private videoCallService: VideoCallService) {
  }

  ngAfterViewInit(): void {
    const id$ = this.route.params.pipe(
      map((params) => params.id),
      filter((id) => id)
    );

    const identity$ = this.route.queryParams.pipe(
      map(params => params.name)
    );

    id$.pipe(
      first(),
      switchMap(id => identity$.pipe(map(identity => ({ id, identity }))))
    ).subscribe(m => this.onRoomChanged(m.id, m.identity));
  }

  async onLeaveRoom(_: boolean) {
    if (this.activeRoom) {
      this.activeRoom.disconnect();
      this.activeRoom = null;
    }

    this.roomGrid.clear();
    await this.router.navigate(['/']);
  }

  async onRoomChanged(roomName: string, identity: string) {
    if (roomName) {
      if (this.activeRoom) {
        this.activeRoom.disconnect();
      }

      const tracks = await Promise.all([
        createLocalAudioTrack(),
        this.camera.showPreviewCamera()
      ]);

      this.activeRoom =
        await this.videoCallService
          .joinOrCreateRoom(roomName, identity, tracks);

      this.roomGrid.initialize(this.activeRoom.participants);
      this.registerRoomEvents();
    }
  }

  onParticipantsChanged(_: boolean) {
    this.videoCallService.nudge();
  }

  private registerRoomEvents() {
    this.activeRoom
      .on('disconnected',
        (room: Room) => room.localParticipant.tracks.forEach(publication => this.detachLocalTrack(publication.track)))
      .on('participantConnected',
        (participant: RemoteParticipant) => this.roomGrid.add(participant))
      .on('participantDisconnected',
        (participant: RemoteParticipant) => this.roomGrid.remove(participant))
      .on('dominantSpeakerChanged',
        (dominantSpeaker: RemoteParticipant) => this.roomGrid.loudest(dominantSpeaker));
  }

  private detachLocalTrack(track: LocalTrack) {
    if (this.isDetachable(track)) {
      track.detach().forEach(el => el.remove());
    }
  }

  private isDetachable(track: LocalTrack): track is LocalAudioTrack | LocalVideoTrack {
    return !!track
      && ((track as LocalAudioTrack).detach !== undefined
        || (track as LocalVideoTrack).detach !== undefined);
  }
}