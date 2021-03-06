# BlazorBlog
Application de blog simple en .NET 6 Blazor Server. J'utilise [MudBlazor](https://github.com/MudBlazor/MudBlazor/) pour l'interface. Pour l'écriture des articles, je suis partie sur une écriture "fluide" avec du MarkDown.  

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

Exemple de docker compose avec une utilisation avec [Traefik](https://github.com/traefik/traefik) (pour les labels).
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

URL de connexion : `https://YOUR-DOMAIN/Identity/Account/Login`  

L'utilisateur **root** est créé au lancement de l'application. Il a le rôle d'administrateur. Il permet de configurer le blog.  
  
![Paramètre](https://github.com/AnthonyRyck/BlazorBlog/blob/main/ImgRessources/Parametres.png)  
  
* Choix du nom du blog  
* Une description du blog
* Choix du logo du blog  
* Choix de l'icône du blog (le favicon)
C'est l'utilisateur root qui peut créer des comptes **Auteur**.  

### Drag & Drop d'une image dans un post

![](https://raw.githubusercontent.com/AnthonyRyck/BlazorBlog/main/ImgRessources/BlazorBlog-DragAndDropImg.gif)  
  
## Roadmap
* :white_check_mark: Création de la base de donnée au démarrage de l'application  
* :white_check_mark: Page pour créer un article  
* :white_check_mark: Ajout d'un composant pour ajouter une galerie d'image  
* :white_check_mark: Ajouter des raccourcis clavier pour ajouter des syntaxes Markdown  
* :white_check_mark: Publier un article  
* :white_check_mark: Page pour la gestion des articles  
* :white_check_mark: Page d'accueil pour afficher les articles publiés  
* :white_check_mark: Partager l'article via Facebook, Twitter, LinkedIn  
* :white_check_mark: Pouvoir sauvegarder un article
* :white_check_mark: Changement des titres dans la barre du browser de façon dynamique  
* :black_square_button: Un système pour compter le nombre de vue sur chaque article  
* :white_check_mark: Ajouter le système de catégorie pour les articles *(sur les posts, page d'édition catégorie, affichage des posts par catégorie,...)*    
* :white_check_mark: Faire une page pour gérer les images de la galerie  
* :white_check_mark: Intégrer des vidéos dans l'article (*ne sera pas lu dans l'article, mais ouvre un nouvel onglet*)  
* :black_square_button: Faire une page sur l'auteur  
* :white_check_mark: Possible de Drag&Drop une image pour l'ajouter à l'article.  
* :white_check_mark: `root` peut faire un export/import complet de la base et des images.
