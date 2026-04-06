# Discord-TF

Discord-TF est une plateforme de communication "full-stack" inspirée de Discord, comprenant un backend en C# développé avec .NET 8 et un client frontend en WPF. Le système utilise MongoDB pour la persistance des données et implémente une architecture asynchrone moderne pour la gestion de l'authentification et des fonctionnalités en temps réel.

---

## Structure du Projet

La solution est composée de trois projets principaux :

**Backend** : Une API Web ASP.NET Core qui gère la logique d'affaires, les interactions avec la base de données MongoDB et fournit les points de terminaison (endpoints) REST pour l'authentification, les serveurs, les canaux et les messages. La communication en temps réel est gérée via SignalR.

**Frontend** : Une application de bureau Windows conçue avec WPF (Windows Presentation Foundation) suivant le patron de conception MVVM (Modèle-Vue-VueModèle).

**Shared** : Une bibliothèque de classes contenant les objets de transfert de données (DTOs), les requêtes, les énumérations, les constantes et les modèles communs utilisés à la fois par le client et le serveur pour garantir la cohérence des données.

---

## Fonctionnalités Implémentées

### Authentification
- Création de compte
- Connexion / Déconnexion
- Hachage de mot de passe (SHA-256)
- Gestion de session côté client

---

## Schéma de Base de Données (MongoDB)

La base de données `DiscordTFDB` contient les collections suivantes :

| Collection | Description |
|---|---|
| `Users` | Comptes utilisateurs |
| `Servers` | Serveurs de communication |
| `ServerMembers` | Relation utilisateur ↔ serveur avec rôle |
| `Channels` | Canaux textuels (serveur ou DM) |
| `Messages` | Messages envoyés dans un canal |

### Relations
- Un `Server` possède plusieurs `Channels`
- Un `User` appartient à plusieurs `Servers` via `ServerMember`
- Un `Channel` de type `Server` appartient à un `Server`
- Un `Channel` de type `Direct` relie deux `Users` via `Participants`
- Un `Channel` contient plusieurs `Messages`

---

## Structure des DTOs (Shared)

```
Shared/
├── DTOs/
│   ├── UserDTO.cs
│   ├── ServerDTO.cs
│   ├── ServerMemberDTO.cs
│   ├── ChannelDTO.cs
│   ├── MessageDTO.cs
│   └── Requests/
│       ├── CreateServerRequest.cs
│       ├── JoinServerRequest.cs
│       ├── CreateChannelRequest.cs
│       ├── CreateMessageRequest.cs
│       └── CreateDMRequest.cs
├── Constants/
│   ├── Routes.cs
│   ├── Messages.cs
│   ├── Ports.cs
│   └── Auth.cs
└── Enums/
    ├── ChannelType.cs
    └── MemberRole.cs
```

---

## Configuration

### Prérequis
- .NET 8 SDK
- MongoDB Atlas (ou instance locale)
- Visual Studio 2022+

### Base de données
La chaîne de connexion MongoDB est configurée dans `Backend/appsettings.json` :

```json
"MongoDB": {
  "ConnectionString": "<votre_connection_string>",
  "DatabaseName": "DiscordTFDB"
}
```

### Lancer le projet
1. Cloner le dépôt
2. Configurer la connexion MongoDB dans `appsettings.json`
3. Lancer le projet `Backend` (écoute sur `http://localhost:8080`)
4. Lancer le projet `Frontend`

La documentation Swagger est disponible à `http://localhost:8080/swagger` en mode développement.

---

## Technologies Utilisées

| Couche | Technologie |
|---|---|
| Backend | ASP.NET Core 8, MongoDB.Driver, SignalR |
| Frontend | WPF, MVVM, HttpClient |
| Base de données | MongoDB Atlas |
| Shared | .NET 8 Class Library |
| Temps réel | ASP.NET Core SignalR |
