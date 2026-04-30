# Installation et lancement du projet

## Prérequis

Avant de lancer le projet, il faut avoir installé :

- **.NET 8 SDK**
- **Node.js**
- **npm**
- **Angular CLI**
- **Entity Framework Core CLI**

Si l’outil Entity Framework Core n’est pas installé :

```bash
dotnet tool install --global dotnet-ef
```

---

## 1. Lancer le backend

Depuis la racine du projet :

```bash
cd backend
dotnet restore
dotnet ef database update
dotnet run
```

Le backend démarre sur :

```txt
http://localhost:5065
```

Swagger est disponible ici :

```txt
http://localhost:5065/swagger
```

Swagger permet de tester les routes de l’API directement depuis le navigateur.

---

## 2. Lancer le frontend

Ouvre un **deuxième terminal**, puis depuis la racine du projet :

```bash
cd frontend
npm install
ng serve
```

Ou bien, si le script `start` est configuré dans `package.json` :

```bash
npm start
```

Le frontend démarre sur :

```txt
http://localhost:4200
```

---

## 3. Tester l’application

1. Lancer le backend avec `dotnet run`.
2. Lancer le frontend avec `ng serve` ou `npm start`.
3. Ouvrir le navigateur sur :

```txt
http://localhost:4200
```

4. Créer un compte ou se connecter.
5. Tester le tableau Kanban.

---

## 4. Compte administrateur de test

Un compte administrateur est créé par défaut pour tester l’application :

```txt
Email : sarah@kanban.com
Mot de passe : Sarah0552
```

Ce compte permet d’accéder aux fonctionnalités administrateur, notamment la consultation des Kanban des utilisateurs et la gestion des colonnes.

---

## 5. Base de données

Le projet utilise une base SQLite locale nommée :

```txt
app.db
```

Elle se trouve dans le dossier `backend/`.

Si la base n’existe pas encore, elle est créée après la commande :

```bash
dotnet ef database update
```

---

## 6. Remarques

Le backend et le frontend doivent être lancés en même temps.

Le backend tourne sur :

```txt
http://localhost:5065
```

Le frontend tourne sur :

```txt
http://localhost:4200
```

La communication entre les deux est autorisée grâce à la configuration CORS du backend.