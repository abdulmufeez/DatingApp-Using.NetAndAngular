import { Component, OnInit } from '@angular/core';
import { LikeParams } from '../_models/likeParams';
import { Member } from '../_models/member';
import { Pagination } from '../_models/pagination';
import { MembersService } from '../_services/members.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styles: [
  ]
})
export class ListsComponent implements OnInit {
  members: Partial<Member[]>;  
  pagination: Pagination;
  likeParams: LikeParams;

  constructor(private memberService: MembersService) { 
    this.likeParams = this.memberService.getLikeParams();    
  }

  ngOnInit(): void {    
    this.loadLikes();
  }

  loadLikes(){    
    this.memberService.setLikeParams(this.likeParams);      
    this.memberService.getLikes(this.likeParams).subscribe(response => {
      this.members = response.results;
      this.pagination = response.pagination;
    });
  }

  pageChanged(event: any) {
    this.likeParams.pageNumber = event.page;
    this.memberService.setLikeParams(this.likeParams); 
    this.loadLikes();
  }

}
