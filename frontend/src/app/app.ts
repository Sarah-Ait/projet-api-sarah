import { Component, OnInit, ChangeDetectorRef, inject } from '@angular/core';
//importer les outils angular 
@Component({
  selector: 'app-root',
  standalone: true,// ce composant est autonome 
  imports: [],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit { //creer le composant principale
  message = 'Chargement...';
  private cdr = inject(ChangeDetectorRef); // rafraichir l ecrans

  ngOnInit(): void {
    fetch('http://localhost:5065/api/test')
      .then(response => response.text())//transformer la rep en text
      .then(data => { //.then attends une fct il met le message dans data puis dans this.message
        this.message = data;
        this.cdr.detectChanges();
      })
      .catch(error => {
        console.error(error);
        this.message = 'Erreur de connexion au backend';
        this.cdr.detectChanges();
      });
  }
}