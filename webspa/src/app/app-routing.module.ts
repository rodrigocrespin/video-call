import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppContainerComponent } from './modules/shell/app-container/app-container.component';

const routes: Routes = [
  {
    path: '',
    component: AppContainerComponent,
    children: [
      {
        path: '',
        loadChildren: () => import('./modules/main/main.module').then(mod => mod.MainModule)
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
