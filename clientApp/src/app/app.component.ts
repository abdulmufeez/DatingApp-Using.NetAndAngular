import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';
import { User } from './_models/User';
import { AccountService } from './_services/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styles: []
})
export class AppComponent implements OnInit {
  title = 'Dating App';
  users: any;

  constructor(private http: HttpClient, private accountService: AccountService) {}

  ngOnInit(): void {
    //when angular start everything here is also started
    //this.getUsers();
    this.setCurrentUser();
  }

  setCurrentUser(){
    const user: User = JSON.parse(localStorage.getItem('user.info'));
    this.accountService.setCurrentUser(user);
  }
  
  // getUsers(){
  //   this.http.get(environment.dotnetUrl.toString() + '/api/users').subscribe(response => {    //environment.dotnetUrl.toString() this will get text from the enviroment file
  //     this.users = response;
  //     console.log(response);
  //   }, error => {
  //     console.log(error);
  //   })
  // }
}
