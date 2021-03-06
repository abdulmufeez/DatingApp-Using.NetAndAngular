import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ToastrModule } from 'ngx-toastr';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { NgxGalleryModule } from '@kolkov/ngx-gallery';
import { NgxSpinnerModule } from 'ngx-spinner';
import { FileUploadModule } from 'ng2-file-upload';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { TimeagoModule } from 'ngx-timeago';
import { ModalModule } from 'ngx-bootstrap/modal'

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    BrowserAnimationsModule,
    FormsModule, //for forms in angular 
    BsDropdownModule.forRoot(),  //for dropdown
    ToastrModule.forRoot({      //toastr notifications
      positionClass:"toast-bottom-right"
    }),
    TabsModule.forRoot(), //for tabs
    NgxGalleryModule,    //for photos gallery
    NgxSpinnerModule,
    FileUploadModule,
    ReactiveFormsModule,
    BsDatepickerModule.forRoot(),
    PaginationModule.forRoot(),
    ButtonsModule.forRoot(),
    TimeagoModule.forRoot(),
    ModalModule.forRoot()
  ],
  exports: [
    BrowserAnimationsModule,
    FormsModule,
    BsDropdownModule,
    ToastrModule,
    TabsModule,
    NgxGalleryModule,
    NgxSpinnerModule,
    FileUploadModule,
    ReactiveFormsModule,
    BsDatepickerModule,
    PaginationModule,
    ButtonsModule,
    TimeagoModule,
    ModalModule
  ]
})
export class SharedModule { }
