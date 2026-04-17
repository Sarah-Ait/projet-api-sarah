import { bootstrapApplication } from '@angular/platform-browser';
// j importe bootstrapapplication qui es une appli pour demarrer angular
import { App } from './app/app';
//j emporte le composant principale app (app.ts)

bootstrapApplication(App) //demmarer l appli
  .catch(err => console.error(err)); // si err