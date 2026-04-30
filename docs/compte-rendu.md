# Compte rendu technique — Kanbe

## 1. Objectif du projet

Kanbe est une application Kanban full-stack réalisée dans le cadre d’un test technique.

Le sujet imposait déjà les grandes lignes : backend **.NET Core**, frontend **Angular**, **Entity Framework Core**, authentification **JWT avec refresh token**, rôles **Admin / Standard**, et séparation **Controllers / Services / Repositories**.

Je ne vais donc pas m’attarder sur ces éléments imposés.  
Ce compte rendu se concentre surtout sur les choix que j’ai dû faire pendant le développement, les difficultés rencontrées et la manière dont je les ai résolues.

---

## 2. Choix techniques réalisés

### Utiliser SQLite plutôt que PostgreSQL

Le sujet permettait d’utiliser SQLite ou PostgreSQL.

J’ai choisi **SQLite** car le projet devait rester simple à lancer en local.  
Pour un test technique, cela évite d’imposer l’installation d’un serveur de base de données externe.

La base est contenue dans un fichier `app.db`, ce qui rend le projet plus facile à tester, à corriger et à partager.

PostgreSQL aurait été plus adapté pour une vraie mise en production, mais SQLite était plus cohérent pour livrer rapidement une application fonctionnelle et facilement exécutable.

---

### Utiliser des DTOs plutôt que renvoyer directement les entités

Avec les relations entre `User`, `KanbanColumn` et `Ticket`, renvoyer directement les entités Entity Framework pouvait provoquer des réponses trop lourdes, des données inutiles, voire des cycles JSON.

J’avais une solution rapide : utiliser `IgnoreCycles`.

Mais j’ai préféré utiliser des **DTOs**.

Ce choix permet de contrôler précisément ce que le frontend peut envoyer et ce que l’API renvoie.  
Il évite aussi d’exposer directement la structure interne de la base de données.

C’est une solution un peu plus longue à mettre en place, mais plus propre, plus claire pour Swagger, et plus stable pour le frontend.

---

### Centraliser les erreurs avec un middleware

Une autre décision importante a été la gestion des erreurs.

J’aurais pu gérer les erreurs directement dans chaque controller avec des `if`, des `return NotFound()`, `return BadRequest()`, etc.

Mais cette approche devient vite répétitive.

J’ai donc choisi de créer des exceptions métier comme :

- `NotFoundException` ;
- `ValidationException` ;
- `UnauthorizedException` ;
- `ForbiddenException`.

Puis un middleware transforme ces exceptions en réponses HTTP propres.

Ce choix permet de garder les controllers plus courts, d’éviter la répétition et d’avoir une gestion des erreurs plus cohérente dans toute l’API.

---

### Distinguer clairement 401 et 403

J’ai aussi fait attention à ne pas mélanger les erreurs d’authentification et les erreurs de permission.

- **401** : l’utilisateur n’est pas connecté ou son token est invalide.
- **403** : l’utilisateur est connecté, mais il n’a pas le droit de faire l’action.

Cette distinction est importante, car côté frontend, ce ne sont pas les mêmes réactions :  
un 401 peut demander une reconnexion, alors qu’un 403 signifie simplement que l’action est interdite.

---

### Ne pas faire confiance au frontend

Un choix important a été de ne pas laisser le frontend décider des données sensibles.

Par exemple, lors de la création d’un ticket, je n’ai pas simplement accepté un `AssignedUserId` envoyé par le client.

Le backend déduit l’utilisateur assigné à partir de la colonne choisie, puis vérifie les droits.

L’idée est simple :

> Le frontend aide l’utilisateur, mais le backend reste l’autorité.

Cela évite qu’un utilisateur standard puisse modifier une requête manuellement pour agir sur les données d’un autre utilisateur.

---

### Stocker le refresh token en cookie HTTP-only

Pour l’option “Se souvenir de moi”, il fallait garder une session persistante.

Une solution simple aurait été de stocker un token dans le `localStorage`.

Mais j’ai préféré utiliser un refresh token stocké dans un cookie **HTTP-only**.

Ce choix est plus sécurisé, car le cookie n’est pas lisible directement par JavaScript.  
Cela réduit le risque en cas de faille XSS.

L’access token, lui, reste court et sert uniquement aux appels API protégés.

---

### Hasher le refresh token en base de données

J’ai aussi choisi de ne pas stocker le refresh token brut en base.

Il est stocké sous forme hashée.

Comme pour un mot de passe, cela évite qu’un token puisse être réutilisé directement si la base de données était consultée.

C’est un détail de sécurité important, même si ce n’était pas le chemin le plus rapide à implémenter.

---

### Utiliser une rotation du refresh token

À chaque refresh, l’ancien token est révoqué et remplacé par un nouveau.

C’est plus complexe qu’un refresh token fixe, mais plus sécurisé.

Si un ancien token est réutilisé, il n’est plus valide.  
Cela permet de limiter les risques en cas de vol ou de réutilisation abusive du token.

---

### Garder l’access token en mémoire côté Angular

Côté frontend, j’ai choisi de garder l’access token en mémoire plutôt que dans le `localStorage`.

Le `localStorage` est pratique, mais il est plus exposé en cas de script malveillant.

Avec un access token en mémoire, le token disparaît au rechargement de la page.  
Pour compenser, l’application utilise le refresh token en cookie HTTP-only pour récupérer une session valide au démarrage.

C’est un compromis : un peu plus de logique côté frontend, mais une meilleure approche côté sécurité.

---

### Éviter plusieurs refresh en parallèle

Un problème possible est que plusieurs requêtes expirent en même temps.

Sans protection, le frontend pourrait appeler plusieurs fois `/refresh` en parallèle.  
Avec la rotation du refresh token, cela peut provoquer des conflits, car le premier appel révoque déjà l’ancien token.

J’ai donc choisi de partager un seul appel de refresh en cours.

Si un refresh est déjà lancé, les autres requêtes attendent le même résultat.  
Cela rend l’authentification plus stable et évite des déconnexions imprévues.

---

### Faire un drag and drop optimiste, mais contrôlé

Pour le drag and drop, j’ai choisi une mise à jour optimiste.

Quand l’utilisateur déplace un ticket, le ticket bouge immédiatement dans l’interface.  
Cela rend l’application plus agréable à utiliser.

Mais la validation finale reste côté backend.

Si le backend refuse l’action, par exemple à cause des droits, le frontend remet le ticket à sa place.

Ce choix permet d’avoir une interface fluide sans sacrifier la sécurité.

---

### Utiliser des mises à jour immutables côté frontend

Pendant le drag and drop, j’ai rencontré des comportements visuels instables.

La cause venait du fait que modifier directement un objet ou une liste ne déclenche pas toujours un rendu propre côté Angular.

J’ai donc utilisé des mises à jour immutables : au lieu de modifier directement un ticket, je recrée une nouvelle version de la liste.

Cela rend le comportement plus stable et plus prévisible.

---

### Créer un endpoint dédié pour réordonner les colonnes

Pour l’ordre des colonnes, j’aurais pu modifier chaque colonne une par une.

Mais cela aurait demandé plusieurs requêtes et aurait pu créer un état temporairement incohérent.

J’ai donc choisi un endpoint dédié au réordonnancement.

Le frontend envoie le nouvel ordre complet, et le backend applique le changement de manière globale.

Ce choix est plus propre, car réordonner des colonnes est une action d’ensemble, pas une suite de petites modifications séparées.

---

## 3. Difficultés rencontrées et solutions

### Découvrir C# et .NET dans un vrai projet

C’était mon premier vrai projet en **C#** et en **.NET**.

La difficulté n’était pas seulement de faire fonctionner le code, mais de comprendre les bonnes pratiques : où placer la logique, comment gérer les erreurs, comment sécuriser l’API et comment garder un projet lisible.

Pour avancer, j’ai travaillé étape par étape : communication front/back, base de données, DTOs, authentification, rôles, frontend, puis documentation.

Ce découpage m’a permis de progresser sans tout mélanger.

---

### Choisir entre solution rapide et solution propre

La difficulté principale du projet a été de faire les bons choix.

Plusieurs fois, j’ai eu une solution rapide possible, mais pas forcément propre sur le long terme.

Par exemple :

- utiliser `IgnoreCycles` ou créer des DTOs ;
- gérer les erreurs dans chaque controller ou les centraliser ;
- stocker un token dans `localStorage` ou utiliser un cookie HTTP-only ;
- accepter les données envoyées par le frontend ou les recalculer côté serveur ;
- déplacer visuellement un ticket tout de suite ou attendre la réponse du backend.

J’ai essayé de privilégier les solutions propres, même quand elles demandaient plus de temps.

---

### Gérer les droits correctement

Une autre difficulté a été la gestion des droits.

Il ne suffisait pas de cacher certains boutons côté frontend.  
Un utilisateur peut toujours appeler directement une API.

J’ai donc vérifié les permissions côté backend, notamment dans les services.

Un utilisateur standard peut gérer ses propres tickets, mais ne peut pas agir sur les données des autres ni gérer les colonnes.  
L’administrateur, lui, peut superviser les Kanban des utilisateurs.

Cette partie m’a appris que la sécurité ne doit jamais dépendre uniquement de l’interface.

---

### Le frontend et le drag and drop

Le frontend a demandé beaucoup d’ajustements.

Je voulais que l’application soit simple et agréable à utiliser, surtout pour le déplacement des tickets.

Le drag and drop a demandé de gérer à la fois l’expérience utilisateur et la validation backend.

La solution finale combine une mise à jour immédiate côté interface, une confirmation côté serveur, et un retour arrière si l’action est refusée.

---

### Le déploiement

J’ai essayé de préparer un déploiement en ligne pour rendre l’application accessible directement depuis un navigateur.

Cependant, je m’y suis prise trop tard, au moment du rendu.  
Avec le temps restant, j’ai préféré ne pas prendre le risque de casser une version fonctionnelle ou de livrer un déploiement instable.

J’ai donc choisi de fournir un dépôt Git avec des instructions claires pour lancer le projet en local.

Le déploiement reste une amélioration prévue pour la suite, notamment avec Docker ou une plateforme comme Render, Railway ou Azure.

---

### Gestion du temps

Le projet est arrivé pendant une période chargée, avec des examens universitaires et du travail le week-end.

La difficulté était donc aussi organisationnelle.

J’ai priorisé les éléments essentiels : backend fonctionnel, sécurité, rôles, Kanban utilisable, frontend clair, puis documentation.

Certaines fonctionnalités bonus ont été gardées comme améliorations futures.

---

## 4. Utilisation de l’IA

L’IA a été utilisée comme un outil d’aide, pas comme un remplacement de compréhension.

Je l’ai utilisée principalement pour comparer plusieurs approches techniques, vérifier certaines bonnes pratiques, comprendre des notions .NET / Angular et gagner du temps sur du code répétitif.

Je l’ai aussi utilisée pour m’aider à générer des documentations propres et bien structurées.  
Je donnais mes directives, les éléments à expliquer et le style souhaité, puis l’IA m’aidait à reformuler et organiser le contenu de manière plus claire.

Enfin, je l’ai beaucoup utilisée pour le frontend, notamment pour améliorer le style, l’ergonomie et la présentation visuelle de l’application.  
Dans ce cas, l’objectif était surtout d’embellir l’interface et de rendre l’application plus agréable à utiliser.

Chaque partie importante a ensuite été relue, testée et adaptée au projet.  
J’ai également complété ce travail avec de la documentation, des recherches personnelles et des vidéos pour bien comprendre les choix techniques réalisés.
---

## 5. Améliorations possibles

Avec plus de temps, j’aimerais ajouter :

- des tests unitaires et d’intégration ;
- un historique complet des déplacements de tickets ;
- une recherche textuelle dans les tickets ;
- une interface frontend plus avancée ;
- un déploiement complet en ligne.

---

## 6. Conclusion

Ce projet m’a permis de découvrir concrètement une stack que je ne maîtrisais pas encore : **C#**, **ASP.NET Core**, **Angular**, **Entity Framework Core** et l’authentification JWT.

La partie la plus importante n’a pas été uniquement de faire fonctionner l’application, mais d’apprendre à faire des choix techniques propres et défendables.

Kanbe reste améliorable, mais le projet possède une base claire, fonctionnelle et évolutive.