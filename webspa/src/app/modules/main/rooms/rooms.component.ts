import { AfterViewInit, ChangeDetectionStrategy, Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RoomsService } from '../../../services/rooms.service';
import { Room } from '../../../models/room';
import { combineLatest, Observable, of, Subject } from 'rxjs';
import { map, mapTo, startWith, switchMap, switchMapTo, tap } from 'rxjs/operators';
import { Router } from '@angular/router';

interface RoomsModel {
  creatingRoom: boolean;
  rooms: Room[];
}

@Component({
  selector: 'app-rooms',
  templateUrl: './rooms.component.html',
  styleUrls: ['./rooms.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RoomsComponent implements AfterViewInit {
  form: FormGroup;
  model$: Observable<RoomsModel>;

  private createRoomSubject = new Subject<string>();
  private refreshRoomsSubject = new Subject();

  constructor(fb: FormBuilder, private roomsService: RoomsService, private router: Router) {
    this.form = fb.group({
      roomName: [null, Validators.required]
    });

    const rooms$ = this.refreshRoomsSubject.pipe(switchMapTo(roomsService.getAll()));
    const creatingRoom$ = this.createRoomSubject.pipe(
      switchMap(name => this.roomsService.create(name).pipe(
        tap(r => this.router.navigate(['/room', r.name])),
        mapTo(false),
        startWith(true),
      )),
      startWith(false)
    );

    this.model$ = combineLatest([rooms$, creatingRoom$]).pipe(
      map(([rooms, creatingRoom]) => ({ rooms, creatingRoom }))
    );
  }

  createRoom() {
    if (!this.form.controls.roomName.value) {
      return;
    }
    this.createRoomSubject.next(this.form.controls.roomName.value);
  }

  ngAfterViewInit(): void {
    this.refreshRoomsSubject.next();
  }
}