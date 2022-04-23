import { Photo } from "./photo";

export interface Member {
    id:                number;
    firstName:         string;
    lastName:          null;
    mainPhotoUrl:      string;
    age:               number;
    knownAs:           string;
    profileCreatedAt:  Date;
    lastActive:        Date;
    gender:            string;
    introduction:      string;
    lookingFor:        string;
    interests:         string;
    country:           string;
    city:              string;
    applicationUserId: number;
    photos:            Photo[];
}