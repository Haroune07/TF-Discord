# Discord-TF

Discord-TF est une plateforme de communication "full-stack" inspirée de Discord, comprenant un backend en C# développé avec .NET 8 et un client frontend en WPF. Le système utilise MongoDB pour la persistance des données et implémente une architecture asynchrone moderne pour la gestion de l'authentification et des fonctionnalités en temps réel via SignalR.

---

## Architecture et Design

Le projet repose sur des standards modernes de développement .NET :
- **Pattern MVVM** : Séparation stricte de la logique et de la vue dans le client WPF.
- **Injection de Dépendances (IoC)** : Utilisation de `Microsoft.Extensions.DependencyInjection` dans `App.xaml.cs` pour gérer le cycle de vie des services et ViewModels.
- **SignalR Hubs** : Communication bidirectionnelle en temps réel avec gestion automatique de la reconnexion.
- **Services d'Infrastructure** : Couche dédiée pour les intégrations externes, comme **Twilio** pour les notifications.

---

## Structure du Projet

- **Backend** : API ASP.NET Core 8 avec MongoDB Driver et SignalR Hubs.
- **Frontend** : Application WPF utilisant le `CommunityToolkit.Mvvm`.
- **Infrastructure** : Services transversaux (Notification, Stockage).
- **Shared** : Modèles de données (DTOs), Requêtes, Enums et Constantes partagés.
- **Discord-TF.Tests** : [À venir] Suite de tests unitaires avec xUnit et Moq.

---

## Fonctionnalités Clés

### 🔐 Sécurité & Profil
- Authentification complète (Inscription/Connexion).
- Hachage sécurisé des mots de passe.
- Persistance de la session utilisateur.

### 💬 Messagerie & Social
- **Serveurs & Salons** : Création dynamique de serveurs et de canaux textuels.
- **Temps Réel** : Indicateurs de saisie ("User is typing"), notifications de message et mise à jour instantanée.
- **Amis & DMs** : Système de gestion de liste d'amis (Friendships) et messagerie privée directe.
- **Gestion des membres** : Rôles hiérarchiques (Owner, Admin, Member).

---

## Schéma de Base de Données (MongoDB)

| Collection | Description |
|---|---|
| `Users` | Profils et identifiants utilisateurs. |
| `Servers` | Groupes de discussion et métadonnées du serveur. |
| `ServerMembers` | Association Utilisateur-Serveur avec gestion des rôles. |
| `Channels` | Canaux (Textuels) rattachés à un serveur ou de type Direct. |
| `Messages` | Contenu, horodatage et référence à l'auteur/canal. |
| `Friendships` | État des relations sociales (Amis, En attente, Bloqués). |

---

## Technologies Utilisées

| Couche | Technologie |
|---|---|
| **Backend** | .NET 8 (Web API), MongoDB.Driver |
| **Temps Réel** | ASP.NET Core SignalR |
| **Frontend** | WPF, MVVM, HttpClient |
| **Infrastructure** | Twilio SDK |
| **Tests** | xUnit, Moq |

---

## Configuration & Installation

### Prérequis
- SDK .NET 8.0
- Instance MongoDB (Atlas ou Local)
- Visual Studio 2022

### Installation
1. Cloner le repository.
2. Modifier `Backend/appsettings.json` pour y inclure votre `ConnectionString` MongoDB.
3. Exécuter le projet `Backend` (Swagger disponible sur le port 8080).
4. Lancer le client `Frontend`.

