CREATE TABLE IF NOT EXISTS posts
(idpost int NOT NULL AUTO_INCREMENT,
title VARCHAR(100) NOT NULL,
content LONGTEXT,
image VARCHAR(300),
posted DATETIME,
updateat DATETIME NOT NULL,
userid VARCHAR(256) NOT NULL,
ispublished BIT(1) NOT NULL DEFAULT 0,
PRIMARY KEY(idpost));

CREATE TABLE IF NOT EXISTS settings
(settingname VARCHAR(100) NOT NULL,
settingvalue VARCHAR(250) NOT NULL,
PRIMARY KEY(settingname));

CREATE TABLE IF NOT EXISTS categories
(idcategorie INT UNSIGNED NOT NULL AUTO_INCREMENT,
nom VARCHAR(100) NOT NULL,
PRIMARY KEY(idcategorie));

CREATE TABLE IF NOT EXISTS categorietopost
(postid INT NOT NULL,
categorieid INT UNSIGNED NOT NULL,
PRIMARY KEY(postid, categorieid),
FOREIGN KEY (postid) REFERENCES posts (idpost) ON DELETE CASCADE,
FOREIGN KEY (categorieid) REFERENCES categories (idcategorie) ON DELETE CASCADE);

CREATE TABLE IF NOT EXISTS profils
(userid VARCHAR(256) NOT NULL,
imgprofil VARCHAR(300),
profilcontent LONGTEXT,
PRIMARY KEY(userid));