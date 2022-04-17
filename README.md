# BlazorBlog
Application de blog simple en .NET 6 Blazor Server.

## Roadmap
*TODO*

## Installation
---------------------
Exemple de commande Docker :   
```
docker run -d -p 3030:80 \
-e LOGIN_DB=YourLoginDb \
-e PASSWORD_DB=yourPassWord \
-e DB_NAME=NameOfSchema \
-e DB_HOST=Ip_Or_UrlDatabase \
--name nameContainer anthonyryck/blazorblog:latest
```

Exemple de Docker-Compose :  
```
TODO
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
