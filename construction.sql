CREATE TABLE profil(
    id SERIAL PRIMARY KEY,
    nom VARCHAR(255)
);

CREATE TABLE administrateurs(
    id SERIAL PRIMARY KEY,
    email VARCHAR(255),
    password VARCHAR(100),
    idProfil INT DEFAULT 1,
    FOREIGN KEY(idProfil) REFERENCES profil(id)
);

CREATE TABLE clients(
    id SERIAL PRIMARY KEY,
    numero VARCHAR(10),
    idProfil INT DEFAULT 2,
    FOREIGN KEY(idProfil) REFERENCES profil(id)
);

-- AJOUTER colonne prix pour utiliser TRIGGER
CREATE TABLE typeMaison(
    id SERIAL PRIMARY KEY,
    nom VARCHAR(100),
    description TEXT,
    dureeDeConstruction INT,
    surface DOUBLE PRECISION
);

CREATE TABLE typeFinition(
    id SERIAL PRIMARY KEY,
    nom VARCHAR(100),
    pourcentage DOUBLE PRECISION
);

CREATE SEQUENCE dev_seq;

CREATE TABLE devis(
    id SERIAL PRIMARY KEY,
    numero VARCHAR(10) DEFAULT 'D' || LPAD(NEXTVAL('dev_seq')::text, 6, '0'),
    idClient INT,
    idTypeMaison INT,
    dureeDeConstruction INT,
    surface DOUBLE PRECISION,
    idTypeFinition INT,
    pourcentageFinition DOUBLE PRECISION,
    dateDevis TIMESTAMP DEFAULT NOW(),
    dateDebut TIMESTAMP,
    montantTravaux DOUBLE PRECISION,
    lieu VARCHAR(255),
    FOREIGN KEY(idClient) REFERENCES clients(id),
    FOREIGN KEY(idTypeMaison) REFERENCES typeMaison(id),
    FOREIGN KEY(idTypeFinition) REFERENCES typeFinition(id)
);

SELECT SUM(montantTravaux * (1 + (pourcentageFinition / 100))) AS montantTotalDevis
FROM devis;

SELECT EXTRACT(month FROM dateDevis) AS mois, 
SUM(montantTravaux * (1 + (pourcentageFinition / 100))) AS montantTotalDevis
FROM devis
WHERE EXTRACT(year FROM dateDevis) = '2024'
GROUP BY EXTRACT(month FROM dateDevis);

CREATE VIEW vw_devis AS
SELECT devis.*, typeMaison.nom AS nomMaison, typeFinition.nom AS nomFinition
FROM devis
JOIN typeMaison ON typeMaison.id = devis.idTypeMaison
JOIN typeFinition ON typeFinition.id = devis.idTypeFinition;

CREATE SEQUENCE paiement_seq;

CREATE TABLE paiement(
    id SERIAL PRIMARY KEY,
    idDevis INT,
    ref_paiement VARCHAR(10) DEFAULT 'P' || LPAD(NEXTVAL('paiement_seq')::text, 6, '0'),
    date TIMESTAMP,
    montant DOUBLE PRECISION,
    FOREIGN KEY(idDevis) REFERENCES devis(id)
);

SELECT SUM(montant) AS montantTotalPaiementEffectue
FROM paiement;

CREATE VIEW vw_paiementTotalDevis AS
SELECT paiement.iddevis, SUM(montant) AS paiementEffectue
FROM paiement
GROUP BY paiement.iddevis;

CREATE VIEW vw_devis_paiementEffectue AS
SELECT vw_devis.*, COALESCE(vw_paiementTotalDevis.paiementEffectue, 0) AS paiementEffectue
FROM vw_devis
LEFT JOIN vw_paiementTotalDevis ON vw_devis.id = vw_paiementTotalDevis.iddevis;

CREATE TABLE unite(
    id SERIAL PRIMARY KEY,
    nom VARCHAR(100)
);

CREATE TABLE typeTravaux(
    id SERIAL PRIMARY KEY,
    numero VARCHAR(5),
    designation TEXT,
    idUnite INT,
    prixUnitaire DOUBLE PRECISION,
    FOREIGN KEY(idUnite) REFERENCES unite(id)
);

CREATE VIEW vw_typeTravaux AS
SELECT typeTravaux.*, unite.nom AS nomUnite
FROM typeTravaux
JOIN unite ON unite.id = typeTravaux.idUnite;

CREATE TABLE travauxTypeDeMaison(
    id SERIAL PRIMARY KEY,
    idTypeMaison INT,
    idTypeTravaux INT,
    quantite DOUBLE PRECISION,
    FOREIGN KEY(idTypeMaison) REFERENCES typeMaison(id),
    FOREIGN KEY(idTypeTravaux) REFERENCES typeTravaux(id)
);

CREATE TABLE travauxDevis(
    id SERIAL PRIMARY KEY,
    idDevis INT,
    idTypeTravaux INT,
    quantite DOUBLE PRECISION,
    prixUnitaire DOUBLE PRECISION,
    FOREIGN KEY(idDevis) REFERENCES devis(id),
    FOREIGN KEY(idTypeTravaux) REFERENCES typeTravaux(id)
);

CREATE VIEW vw_travauxDevis AS
SELECT travauxDevis.*, devis.idTypeMaison, typeTravaux.numero, typeTravaux.designation, unite.nom AS unite, 
(travauxDevis.prixUnitaire * quantite) AS total
FROM travauxDevis
JOIN devis ON devis.id = travauxDevis.idDevis
JOIN typeTravaux ON typeTravaux.id = travauxDevis.idTypeTravaux
JOIN unite ON unite.id = typeTravaux.idUnite; 

SELECT
    1 AS iddevis, 
    idTypeTravaux,
    quantite,
    prixUnitaire
FROM
    travauxTypeDeMaison
    JOIN typeTravaux ON travauxTypeDeMaison.idTypeTravaux = typeTravaux.id
WHERE
    -- Ajoutez des conditions si nécessaire pour sélectionner les lignes à insérer
    -- Par exemple, pour insérer uniquement les lignes avec un certain idTypeMaison :
    idTypeMaison = 2;

INSERT INTO profil VALUES(default, 'BTP');
INSERT INTO profil VALUES(default, 'Client');

INSERT INTO administrateurs(email, password) VALUES('admin@gmail.com', 
'AQAAAAIAAYagAAAAEN/1Lq6GTSUJIsuSt2s1mVReK7/458u1TqJEXtpSfKQ6i4gFc/NjeSuaXgQjfw9euA==');

INSERT INTO typeFinition(nom, pourcentage) VALUES('Standard', 0);
INSERT INTO typeFinition(nom, pourcentage) VALUES('Gold', 10);
INSERT INTO typeFinition(nom, pourcentage) VALUES('Premium', 20);
INSERT INTO typeFinition(nom, pourcentage) VALUES('VIP', 30);

INSERT INTO unite(nom) VALUES('m3');
INSERT INTO unite(nom) VALUES('m2');
INSERT INTO unite(nom) VALUES('fft');

INSERT INTO typeMaison (nom, description, dureeDeConstruction, surface) VALUES 
('Maison Traditionnelle', E'Une maison traditionnelle spacieuse 
avec\n_5 chambres\n_2 salles de bains\n_1 grand salon\n_1 cuisine equipee', 180, 128),
('Maison Moderne', E'Une maison moderne elegante comprenant\n_4 chambres\n_3 salles 
de bains\n_1 salon ouvert avec des finitions haut de gamme\n_1 cuisine design.', 200, 148),
('Maison en Bois', E'Une charmante maison en bois offrant\n_3 chambres\n_2 salles 
de bains\n_1 salon avec une cheminee\n_1 grande terrasse en bois.', 220, 168),
('Maison Conteneur', E'Une maison originale fabriquee a partir de conteneurs maritimes reamenages, 
comprenant\n_2 chambres\n_1 salle de bain\n_1 espace de vie ouvert\n_1 terrasse sur le toit.', 160, 188);

INSERT INTO typeTravaux VALUES(default, '001', 'mur de soutenement et demi Cloture ht 1m', 1, 190000);
INSERT INTO typeTravaux VALUES(default, '101', 'Decapage des terrains meubles', 2, 3072.87);
INSERT INTO typeTravaux VALUES(default, '102', 'Dressage du plateforme', 2, 3736.26);
INSERT INTO typeTravaux VALUES(default, '103', 'Fouille d'' ouvrage terrain ferme', 1, 9390.93);
INSERT INTO typeTravaux VALUES(default, '104', 'Remblai d'' ouvrage', 1, 37563.26);
INSERT INTO typeTravaux VALUES(default, '105', 'Travaux d'' implantation', 3, 152656);
INSERT INTO typeTravaux VALUES(default, '201', 'maconnerie de moellons, ep= 35cm', 1, 172114.4);
INSERT INTO typeTravaux VALUES(default, '202', 'beton armee dosee a 350kg/m3: - semelles isolee - armorces poteaux - chainage bas de 20x20', 1, 573215.80);
INSERT INTO typeTravaux VALUES(default, '203', 'remblai technique', 1, 37563.26);
INSERT INTO typeTravaux VALUES(default, '204', 'herrissonage ep=10', 1, 73245.40);
INSERT INTO typeTravaux VALUES(default, '205', 'beton ordinaire dosee a 300kg/m3', 1, 487815.80);
INSERT INTO typeTravaux VALUES(default, '206', 'chape de 2cm', 1, 33566.54);

INSERT INTO travauxTypeDeMaison (idTypeMaison, idTypeTravaux, quantite) VALUES
(2, 1, 26.98),
(2, 2, 101.36),
(2, 3, 101.36);

-- INSERT INTO travauxDevis VALUES(default, );

SELECT COALESCE(SUM(montant), 0) AS montant FROM paiement 
RIGHT JOIN devis ON devis.id = paiement.iddevis
WHERE iddevis = 1; 





