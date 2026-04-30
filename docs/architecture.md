# Architecture du projet

## Vue générale

Le projet est séparé en deux parties :

```txt
projet-api-sarah/
├── backend/
├── frontend/
└── docs/
```

Le backend gère l’API, la base de données, l’authentification et les règles métier.

Le frontend gère l’interface utilisateur et communique avec le backend via des requêtes HTTP.

---

## Backend

Le backend suit une architecture en couches :

```txt
Controller → Service → Repository → Database
```

## Controllers

Les controllers reçoivent les requêtes HTTP.

Ils servent principalement à :

- exposer les routes API ;
- recevoir les DTOs ;
- appeler les services ;
- renvoyer les réponses HTTP.

Ils ne contiennent pas la logique métier principale.

---

## Services

Les services contiennent les règles métier.

Ils vérifient par exemple :

- si une ressource existe ;
- si l’utilisateur a le droit d’effectuer une action ;
- si les données sont cohérentes ;
- comment transformer une entité en DTO de réponse.

C’est dans cette couche que se trouvent les décisions importantes du backend.

---

## Repositories

Les repositories s’occupent de l’accès aux données.

Ils utilisent Entity Framework Core pour :

- lire les données ;
- créer des éléments ;
- modifier des éléments ;
- supprimer des éléments.

Ils ne doivent pas contenir de logique métier.

---

## Base de données

La base utilisée est SQLite.

Les principales entités sont :

- `User`
- `KanbanColumn`
- `Ticket`
- `RefreshToken`

Chaque utilisateur possède son propre Kanban.

Un utilisateur standard agit uniquement sur ses données.  
Un administrateur peut consulter et gérer les Kanban des autres utilisateurs.

---

## Frontend

Le frontend Angular est organisé autour de plusieurs parties :

```txt
src/app/
├── core/
├── models/
├── services/
└── pages/
```

## core

Contient les éléments globaux :

- guards ;
- interceptor HTTP ;
- configuration API.

## models

Contient les interfaces TypeScript utilisées côté frontend.

## services

Contient les services Angular qui appellent l’API backend.

## pages

Contient les pages principales :

- login ;
- register ;
- board ;
- admin.

---

## Sécurité

L’application utilise :

- un access token JWT ;
- un refresh token ;
- un cookie HTTP-only ;
- des routes protégées ;
- des vérifications de rôle côté backend.

Le frontend améliore l’expérience utilisateur, mais les vraies vérifications de sécurité sont faites côté backend.