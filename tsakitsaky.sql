CREATE TABLE profil(
    id SERIAL PRIMARY KEY,
    nom VARCHAR(255)
);

CREATE TABLE utilisateurs(
    id SERIAL PRIMARY KEY,
    nom VARCHAR(255),
    prenom VARCHAR(255),
    dateNaissance TIMESTAMP,
    genre INT,
    email VARCHAR(255),
    password VARCHAR(100),
    idProfil INT,
    FOREIGN KEY(idProfil) REFERENCES profil(id)
);

CREATE TABLE categorie(
    id SERIAL PRIMARY KEY,
    nom VARCHAR(100)
);

CREATE TABLE film(
    id SERIAL PRIMARY KEY,
    nom VARCHAR(255),
    dateDeSortie TIMESTAMP,
    idCategorie INT,
    FOREIGN KEY(idCategorie) REFERENCES categorie(id)
);

INSERT INTO categorie (nom) VALUES
('Action'),
('Adventure'),
('Animation'),
('Drama'),
('Thriller'),
('Science Fiction'),
('Biography'),
('Comedy'),
('Fantasy'),
('Family'),
('Romance');

INSERT INTO categorie (nom) VALUES
('Horror'),
('Mystery'),
('Crime'),
('War'),
('Western'),
('Musical'),
('Historical'),
('Documentary');


INSERT INTO film (nom, dateDeSortie, idCategorie) VALUES
    ('The Hunger Games', '2012-03-23', 9),
    ('Gravity', '2013-10-04', 9),
    ('The Martian', '2015-10-02', 9),
    ('Frozen', '2013-11-27', 10),
    ('The Incredibles', '2004-11-05', 10),
    ('The Dark Knight Rises', '2012-07-20', 1),
    ('The Wolf of Wall Street', '2013-12-25', 7),
    ('Django Unchained', '2012-12-25', 5),
    ('Inglourious Basterds', '2009-08-21', 5),
    ('The Revenant', '2015-12-25', 1),
    ('Deadpool', '2016-02-12', 6),
    ('The Pursuit of Happyness', '2006-12-15', 11),
    ('The Notebook', '2004-06-25', 11),
    ('Gone with the Wind', '1939-12-15', 11),
    ('Casablanca', '1942-01-23', 11);

INSERT INTO profil VALUES(default, 'User');
INSERT INTO profil VALUES(default, 'Admin');

INSERT INTO utilisateurs VALUES(default, 'Rakoto', 'Nirina', '2000-04-09', 1, 'rn@gmail.com', 'rnmdp', 1);
INSERT INTO utilisateurs VALUES(default, 'Master', 'Admin', '2000-04-09', 1, 'admin@gmail.com', 'adminmdp', 2);

