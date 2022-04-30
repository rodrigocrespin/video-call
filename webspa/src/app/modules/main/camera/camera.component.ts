import {
  AfterViewInit,
  ChangeDetectionStrategy,
  Component,
  ElementRef, EventEmitter,
  Output,
  Renderer2,
  ViewChild
} from '@angular/core';
import { createLocalTracks, LocalTrack, LocalVideoTrack } from 'twilio-video';
import { from, Observable, Subject } from 'rxjs';
import { map, mapTo, startWith, switchMap, tap } from 'rxjs/operators';

interface CameraModel {
  initializing: boolean;
}

@Component({
  selector: 'app-camera',
  templateUrl: './camera.component.html',
  styleUrls: ['./camera.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CameraComponent implements AfterViewInit {
  @ViewChild('preview', { static: false }) previewElement?: ElementRef;

  get tracks(): LocalTrack[] {
    return this.localTracks;
  }

  model$: Observable<CameraModel>;

  private videoTrack?: LocalVideoTrack;
  private localTracks: LocalTrack[] = [];

  private initializeSubject = new Subject<MediaDeviceInfo>();

  constructor(private readonly renderer: Renderer2) {
    const initializing$ = this.initializeSubject.pipe(
      switchMap(deviceInfo => from(this.initializeDevice(deviceInfo?.kind, deviceInfo?.deviceId)).pipe(
        mapTo(false),
        startWith(true),
      )),
      startWith(true)
    );
    this.model$ = initializing$.pipe(map(initializing => ({ initializing })));
  }

  async ngAfterViewInit() {
    if (this.previewElement && this.previewElement.nativeElement) {
      this.initializeSubject.next();
    }
  }

  initializePreview(deviceInfo?: MediaDeviceInfo) {
    this.initializeSubject.next(deviceInfo);
  }

  async showPreviewCamera() {
    await this.initializePreview();
    return this.tracks;
  }

  finalizePreview() {
    try {
      if (this.videoTrack) {
        this.videoTrack.detach().forEach(element => element.remove());
      }
    } catch (e) {
      console.error(e);
    }
  }

  private async initializeDevice(kind?: MediaDeviceKind, deviceId?: string) {
    this.finalizePreview();

    this.localTracks = kind && deviceId
      ? await this.initializeTracks(kind, deviceId)
      : await this.initializeTracks();

    this.videoTrack = this.localTracks.find(t => t.kind === 'video') as LocalVideoTrack;
    const videoElement = this.videoTrack.attach();
    this.renderer.setStyle(videoElement, 'height', '100%');
    //this.renderer.setStyle(videoElement, 'width', '100%');
    this.renderer.appendChild(this.previewElement!.nativeElement, videoElement);
  }

  private initializeTracks(kind?: MediaDeviceKind, deviceId?: string) {
    if (kind) {
      switch (kind) {
        case 'audioinput':
          return createLocalTracks({ audio: { deviceId }, video: true });
        case 'videoinput':
          return createLocalTracks({ audio: true, video: { deviceId } });
      }
    }

    return createLocalTracks({ audio: true, video: true });
  }
}