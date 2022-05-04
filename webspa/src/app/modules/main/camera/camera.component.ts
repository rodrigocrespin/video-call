import {
  AfterViewInit,
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  Renderer2,
  ViewChild
} from '@angular/core';
import { createLocalVideoTrack, LocalTrack, LocalVideoTrack } from 'twilio-video';

@Component({
  selector: 'app-camera',
  templateUrl: './camera.component.html',
  styleUrls: ['./camera.component.scss'],
  changeDetection: ChangeDetectionStrategy.Default
})
export class CameraComponent implements AfterViewInit {
  @ViewChild('preview', { static: false }) previewElement?: ElementRef;

  get tracks(): LocalTrack[] {
    return this.localTracks;
  }

  isInitializing: boolean;

  private videoTrack?: LocalVideoTrack;
  private localTracks: LocalTrack[] = [];

  constructor(private readonly renderer: Renderer2) {
  }

  async ngAfterViewInit() {
    if (this.previewElement && this.previewElement.nativeElement) {
      await this.initializeDevice();
    }
  }

  async initializePreview(deviceId: string) {
    await this.initializeDevice(deviceId);
  }

  async showPreviewCamera() {
    await this.initializeDevice();
    return this.videoTrack;
  }

  finalizePreview() {
    try {
      if (this.videoTrack) {
        this.videoTrack.detach().forEach(element => element.remove());
      }
      this.videoTrack = null;
    } catch (e) {
      console.error(e);
    }
  }

  private async initializeDevice(deviceId?: string) {
    try {
      this.isInitializing = true;

      this.finalizePreview();

      this.videoTrack = deviceId
        ? await createLocalVideoTrack({ deviceId })
        : await createLocalVideoTrack();

      const videoElement = this.videoTrack.attach();
      this.renderer.setStyle(videoElement, 'height', '100%');
      //this.renderer.setStyle(videoElement, 'width', '100%');
      this.removeNodeChildren(this.previewElement);
      this.renderer.appendChild(this.previewElement.nativeElement, videoElement);
    } finally {
      this.isInitializing = false;
    }
  }

  private removeNodeChildren(el: ElementRef): void {
    const childElements = el.nativeElement.children;
    for (let child of childElements) {
      this.renderer.removeChild(el.nativeElement, child);
    }
  }
}
