import { Injectable } from '@angular/core';
import {Song} from './song';
// import * as SongData from '../juke-library.json';
import * as _ from 'lodash';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class JukeService {
  private library: Song[];

  constructor(http: HttpClient ) {
    this.library = new Array();
    http.get<Song[]>('http://localhost:5001/api/library?id=1626777457').subscribe(result => {
      this.library = result;
    }, error => console.error(error));
  }

  public getSongs(): Song[] {
    return this.library;
  }

  public getSongByName(name: string) {
    return _.find(this.library, (song: Song) => song.name === name);
  }

  public enqueueSong(song:Song) {
    //TODO
  }
}