import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styles: [
    'img { max-height: 40px; border: 1px solid #fff; display: inline;}'
  ]
})
export class NavbarComponent implements OnInit {
  model: any = {}   //initilize empty object with dynamic type
  appHasRole: string[] = ["Admin", "Moderator"]

  constructor(public accountService: AccountService,
    private router: Router,
    private toastr: ToastrService) { };

  ngOnInit(): void { } 

  login() {
    this.accountService.login(this.model).subscribe(() => {
      this.toastr.success('Login Successfully');
      this.router.navigateByUrl('/members');
    })
  }

  logout() {
    this.accountService.logout();
    //this.router.navigateByUrl('/');
    this.reloadComponent('/');
  }


  reloadComponent(url: string) {
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
    this.router.onSameUrlNavigation = 'reload';
    this.router.navigateByUrl(url);
  }
}
