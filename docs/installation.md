# Installation et lancement du projet

## Prérequis

Avant de lancer le projet, il faut avoir installé :

- .NET 8 SDK
- Node.js
- npm
- Angular CLI
- Entity Framework Core CLI

Installation de l’outil EF Core si besoin :

```bash
dotnet tool install --global dotnet-ef
```

---

## Lancer le backend

Depuis la racine du projet :

```bash
cd backend
dotnet restore
dotnet ef database update
dotnet run
```

Le backend se lance sur :

```txt
http://localhost:5065
```

Swagger est disponible ici :

```txt
http://localhost:5065/swagger
```

---

## Lancer le frontend

Depuis la racine du projet :

```bash
cd frontend
npm install
npm start
```

Le frontend se lance sur :

```txt
http://localhost:4200
```

---

## Tester l’application

1. Lancer le backend.
2. Lancer le frontend.
3. Ouvrir `http://localhost:4200`.
4. Se connecter ou créer un utilisateur.
5. Tester le Kanban.

---

## Remarque

Le projet utilise une base SQLite locale nommée `app.db`.

Si la base n’existe pas encore, elle sera créée après la commande :

```bash
dotnet ef database update
```