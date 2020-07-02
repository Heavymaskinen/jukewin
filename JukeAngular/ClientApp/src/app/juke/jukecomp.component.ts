import { Component, OnInit } from '@angular/core';
import { JukeService } from './juke-service';

@Component({
  selector: 'app-juke-component',
  templateUrl: './jukecomp.component.html'
})
export class JukeComponent {
  public titles: string[];

  constructor(private service: JukeService) {
    this.titles = this.GetSongTitles();
  }

  public GetSongTitles() {
    const songs = this.service.getSongs();
    const txt = new Array();
    songs.forEach((song) => {txt.push(song.artist + ' - ' + song.name); });

    return txt;
  }
}
