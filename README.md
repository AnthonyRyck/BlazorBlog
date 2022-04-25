# BlazorBlog
Application de blog simple en .NET 6 Blazor Server. J'utilise [MudBlazor](https://github.com/MudBlazor/MudBlazor/) pour l'interface.

## Installation

Exemple de commande Docker :   
```
docker run -d -p 3030:80 \
-e LOGIN_DB=YourLoginDb \
-e PASSWORD_DB=yourPassWord \
-e DB_NAME=NameOfSchema \
-e DB_HOST=Ip_Or_UrlDatabase \
--name nameContainer anthonyryck/blazorblog:latest
```

Exemple de docker compose avec une utilisation avec Traefik (pour les labels).
```
blazorblog:
     image: anthonyryck/blazorblog:latest
     container_name: blogblazor
     hostname: blazorblog
     expose :
       - 80
     environment:
       - LOGIN_DB=YourLoginDb
       - PASSWORD_DB=yourPassWord 
       - DB_NAME=NameOfSchema
       - DB_HOST=Ip_Or_UrlDatabase
     labels:
       - traefik.enable=true
       - traefik.http.routers.blog.rule=Host(`blog.yourdomain.com`)
       - traefik.http.routers.blog.entrypoints=https
       - traefik.http.routers.blog.tls=true
       - traefik.http.routers.blog.tls.certresolver=letsencrypt
```

## Utilisation
L'utilisateur **root** est créé au lancement de l'application. Il a le rôle d'administrateur. Il permet de configurer le blog.  
URL de connexion : https://YOUR-DOMAIN/Identity/Account/Login  
![Paramètre](https://github.com/AnthonyRyck/BlazorBlog/blob/main/ImgRessources/Parametres.png)  

## Roadmap
* :heavy_check_mark: Création de la base de donnée au démarrage de l'application  
* :heavy_check_mark: Page pour créer un article  
* :heavy_check_mark: Ajout d'un composant pour ajouter une galerie d'image  
* :heavy_check_mark: Ajouter des raccourcis clavier pour ajouter des syntax Markdown  
* :heavy_check_mark: Publier un article  
* :heavy_check_mark: Page pour la gestion des articles  
* :heavy_check_mark: Page d'accueil pour afficher les articles publiés  
* :heavy_check_mark: Partager l'article via Facebook, Twitter, LinkedIn  
* :heavy_check_mark: Changement des titres dans la barre du browser de façon dynamique  
* Un système pour compter le nombre de vue sur chaque article  
* :heavy_check_mark: Ajouter le système de catégorie pour les articles *(sur les posts, page d'édition catégorie, affichage des posts par catégorie,...)*    
* Faire une page pour gérer les images de la galerie  
* Intégrer des vidéos dans l'article  
* Faire une page sur l'auteur  
