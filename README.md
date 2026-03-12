# Discord-TF
Discord-TF est un projet de plateforme de communication "full-stack" comprenant un backend en C# développé avec .NET 8 et un client frontend en WPF. Le système utilise MongoDB pour la persistance des données et implémente une architecture asynchrone moderne pour la gestion de l'authentification et des fonctionnalités en temps réel.

## Structure du Projet
La solution est composée de trois projets principaux :

**Backend** : Une API Web ASP.NET Core qui gère la logique d'affaires, les interactions avec la base de données MongoDB et fournit les points de terminaison (endpoints) REST pour l'authentification.

**Frontend** : Une application de bureau Windows conçue avec WPF (Windows Presentation Foundation) suivant le patron de conception MVVM (Modèle-Vue-VueModèle).

**Shared** : Une bibliothèque de classes contenant les objets de transfert de données (DTOs), les constantes et les modèles communs utilisés à la fois par le client et le serveur pour garantir la cohérence des données.# Discord-TF
