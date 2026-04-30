# Documentation API

## Base URL

En local, l’API est disponible sur :

```txt
http://localhost:5065
```

Swagger est disponible sur :

```txt
http://localhost:5065/swagger
```

---

## Authentification

### Connexion

```http
POST /api/auth/login
```

Permet à un utilisateur de se connecter.

Exemple de body :

```json
{
  "email": "user@example.com",
  "password": "Password123",
  "rememberMe": true
}
```

Réponse attendue :

```json
{
  "accessToken": "...",
  "email": "user@example.com",
  "role": "Standard"
}
```

---

### Refresh token

```http
POST /api/auth/refresh
```

Permet de récupérer un nouvel access token grâce au refresh token stocké en cookie HTTP-only.

---

### Déconnexion

```http
POST /api/auth/logout
```

Révoque le refresh token et déconnecte l’utilisateur.

---

## Utilisateurs

### Créer un utilisateur

```http
POST /api/users
```

Crée un utilisateur standard.

Exemple :

```json
{
  "name": "Sarah",
  "email": "sarah@example.com",
  "password": "Password123"
}
```

---

### Récupérer les utilisateurs

```http
GET /api/users
```

Route réservée à l’administrateur.

---

### Supprimer un utilisateur

```http
DELETE /api/users/{id}
```

Route réservée à l’administrateur.

---

## Colonnes Kanban

### Récupérer les colonnes

```http
GET /api/kanbancolumns
```

Retourne les colonnes accessibles à l’utilisateur connecté.

Un utilisateur standard voit ses propres colonnes.  
Un administrateur peut consulter les colonnes d’autres utilisateurs.

---

### Créer une colonne

```http
POST /api/kanbancolumns
```

Route réservée à l’administrateur.

Exemple :

```json
{
  "name": "À vérifier",
  "userId": 2
}
```

---

### Modifier une colonne

```http
PUT /api/kanbancolumns/{id}
```

Route réservée à l’administrateur.

---

### Réordonner les colonnes

```http
PUT /api/kanbancolumns/reorder
```

Permet de modifier l’ordre des colonnes.

Exemple :

```json
{
  "columnIds": [3, 1, 2]
}
```

---

### Supprimer une colonne

```http
DELETE /api/kanbancolumns/{id}
```

Route réservée à l’administrateur.

---

## Tickets

### Récupérer les tickets

```http
GET /api/tickets
```

Retourne les tickets accessibles à l’utilisateur connecté.

---

### Créer un ticket

```http
POST /api/tickets
```

Exemple :

```json
{
  "title": "Créer la page login",
  "description": "Mettre en place le formulaire de connexion",
  "timeSpentHours": 2,
  "kanbanColumnId": 1
}
```

L’utilisateur assigné est déduit côté backend à partir de la colonne.

---

### Modifier un ticket

```http
PUT /api/tickets/{id}
```

Exemple :

```json
{
  "title": "Modifier la page login",
  "description": "Améliorer le design du formulaire",
  "timeSpentHours": 3
}
```

---

### Déplacer un ticket

```http
PATCH /api/tickets/{id}/move
```

Exemple :

```json
{
  "targetColumnId": 2
}
```

Permet de déplacer un ticket vers une autre colonne.

---

### Supprimer un ticket

```http
DELETE /api/tickets/{id}
```

Supprime un ticket si l’utilisateur a les droits nécessaires.

---

## Codes HTTP utilisés

```txt
200 OK              Requête réussie
201 Created         Ressource créée
204 No Content      Suppression réussie
400 Bad Request     Données invalides
401 Unauthorized    Utilisateur non connecté
403 Forbidden       Action interdite
404 Not Found       Ressource introuvable
500 Internal Error  Erreur serveur
```