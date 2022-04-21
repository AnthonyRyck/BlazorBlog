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
![Paramètre](https://github.com/AnthonyRyck/BlazorBlog/blob/main/ImgRessources/Parametres.png)  

## Roadmap
* ![check](https://github.com/AnthonyRyck/ctrl-alt-suppr/blob/main/ImgBlog/check.png) Création de la base de donnée au démarrage de l'application  
* ![check](https://github.com/AnthonyRyck/ctrl-alt-suppr/blob/main/ImgBlog/check.png) Page pour créer un article  
* ![check](https://github.com/AnthonyRyck/ctrl-alt-suppr/blob/main/ImgBlog/check.png) Ajout d'un composant pour ajouter une galerie d'image  
* ![check](https://github.com/AnthonyRyck/ctrl-alt-suppr/blob/main/ImgBlog/check.png) Ajouter des raccourcis clavier pour ajouter des syntax Markdown  
* ![check](https://github.com/AnthonyRyck/ctrl-alt-suppr/blob/main/ImgBlog/check.png) Publier un article  
* ![check](https://github.com/AnthonyRyck/ctrl-alt-suppr/blob/main/ImgBlog/check.png) Page pour la gestion des articles  
* ![check](https://github.com/AnthonyRyck/ctrl-alt-suppr/blob/main/ImgBlog/check.png) Page d'accueil pour afficher les articles publiés  
* ![check](https://github.com/AnthonyRyck/ctrl-alt-suppr/blob/main/ImgBlog/check.png) Partager l'article via Facebook, Twitter, LinkedIn  
* ![check](https://github.com/AnthonyRyck/ctrl-alt-suppr/blob/main/ImgBlog/check.png) Changement des titres dans la barre du browser de façon dynamique  
* Ajouter le système de catégorie pour les articles  
* Faire une page pour gérer les images de la galerie  
* Intégrer des vidéos dans l'article  
