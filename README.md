# Kanbe — Organiser, suivre, avancer.

**Kanbe** est une application Kanban full-stack qui permet de gérer des tâches de manière simple, visuelle et structurée.

L’idée du projet : créer une application qui ne se contente pas de fonctionner, mais qui repose sur une base propre, maintenable et défendable techniquement.

Le nom **Kanbe** vient de la contraction entre **Kanban** et **can be** : une application qui peut évoluer, s’améliorer et devenir plus complète avec le temps.

---

## Présentation

Kanbe permet à chaque utilisateur de gérer ses tickets dans un tableau Kanban personnel.

Le projet contient :

- un **backend ASP.NET Core / .NET 8** ;
- un **frontend Angular** ;
- une **API REST** ;
- une **authentification JWT** ;
- une **gestion des rôles** ;
- une base de données **SQLite** avec **Entity Framework Core**.

L’objectif était de construire une application complète, avec une séparation claire entre la logique métier, l’accès aux données et l’interface utilisateur.

---

## Fonctionnalités principales

### Utilisateur standard

Un utilisateur standard possède son propre tableau Kanban avec trois colonnes par défaut :

- **À faire**
- **En cours**
- **Terminé**

Il peut :

- créer ses tickets ;
- consulter ses tickets ;
- modifier ses tickets ;
- supprimer ses tickets ;
- déplacer ses tickets entre les colonnes avec le drag and drop.

Il ne peut pas créer, modifier ou supprimer les colonnes.  
La structure de base du Kanban reste donc simple et cohérente.

---

### Administrateur

L’administrateur dispose de droits étendus sur l’application.

Il peut :

- consulter les Kanban des autres utilisateurs ;
- créer des tickets pour d’autres utilisateurs ;
- modifier les tickets des autres utilisateurs ;
- supprimer les tickets des autres utilisateurs ;
- déplacer les tickets entre les colonnes ;
- créer de nouvelles colonnes ;
- modifier les colonnes existantes ;
- supprimer des colonnes ;
- superviser l’organisation générale des tableaux.

L’administrateur a donc un rôle de supervision et peut intervenir sur l’ensemble de l’application.

---

## Stack technique

### Backend

- **.NET 8**
- **ASP.NET Core**
- **Entity Framework Core**
- **SQLite**
- **JWT Authentication**
- **Swagger**

### Frontend

- **Angular**
- **TypeScript**
- **HTML**
- **CSS**

---

## Architecture du backend

Le backend suit une architecture en couches :

```txt
Controller → Service → Repository → Database