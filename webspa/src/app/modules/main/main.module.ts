import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MainRoutingModule } from './main-routing.module';
import { ReactiveFormsModule } from '@angular/forms';
import { HomeComponent } from './home/home.component';
import { CameraComponent } from './camera/camera.component';
import { RoomsComponent } from './rooms/rooms.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { RoomComponent } from './rooms/room.component';
import { RoomGridComponent } from './rooms/room-grid/room-grid.component';

@NgModule({
  declarations: [
    HomeComponent,
    CameraComponent,
    RoomsComponent,
    RoomComponent,
    RoomGridComponent
  ],
  imports: [
    CommonModule,
    MainRoutingModule,
    ReactiveFormsModule,
    NgbModule
  ]
})
export class MainModule { }
