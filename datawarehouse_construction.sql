CREATE TABLE DimensionTemps(
    tempsID INT PRIMARY KEY,
    jour INT,
    mois INT,
    annee INT
);

CREATE TABLE DimensionClient(
    clientID INT PRIMARY KEY,
    telephone VARCHAR(10)
);

CREATE TABLE DimensionTypeMaison(
    typeMaisonID INT PRIMARY KEY,
    nom VARCHAR(100)
);

CREATE TABLE DimensionLieu(
    lieuID INT PRIMARY KEY,
    nom VARCHAR(255)
);

CREATE TABLE FaitConstruction(
    faitID INT PRIMARY KEY,
    dateDevis TIMESTAMP,
    dateDebut TIMESTAMP,
    dateFin TIMESTAMP,
    montantDuDevis DOUBLE PRECISION,
    paiementEffectue DOUBLE PRECISION,
    tempsID INT,
    clientID INT,
    typeMaisonID INT,
    lieuID INT,
    FOREIGN KEY(tempsID) REFERENCES DimensionTemps(tempsID),
    FOREIGN KEY(clientID) REFERENCES DimensionClient(clientID),
    FOREIGN KEY(typeMaisonID) REFERENCES DimensionTypeMaison(typeMaisonID),
    FOREIGN KEY(lieuID) REFERENCES DimensionLieu(lieuID)
);