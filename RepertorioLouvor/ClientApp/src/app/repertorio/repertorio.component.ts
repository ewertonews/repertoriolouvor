import { Component, OnInit } from '@angular/core';
import { RepositorioMusicasService } from '../repositorio-musicas.service';

@Component({
  selector: 'app-repertorio',
  templateUrl: './repertorio.component.html',
  styleUrls: ['./repertorio.component.css'],
  providers:[RepositorioMusicasService]
})
export class RepertorioComponent implements OnInit {

  repertorioSvc: RepositorioMusicasService;
  constructor(repertorioSvc: RepositorioMusicasService) { 
    this.repertorioSvc = repertorioSvc;
  }

  ngOnInit() {
    const musicas = this.repertorioSvc.getMusicasRepertorio();
    console.log(musicas);
  }

}
