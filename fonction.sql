CREATE OR REPLACE FUNCTION total_devis_par_mois(p_annee INT)
RETURNS TABLE (mois INT, montantTotalDevis DOUBLE PRECISION) AS $$
BEGIN
    RETURN QUERY 
    WITH mois AS (
        SELECT generate_series(1,12) AS mois
    ),
    total_devis AS (
        SELECT 
            EXTRACT(month FROM dateDevis) AS mois, 
            SUM(montantTravaux * (1 + (pourcentageFinition / 100))) AS montantTotalDevis
        FROM devis
        WHERE EXTRACT(year FROM dateDevis) = p_annee
        GROUP BY EXTRACT(month FROM dateDevis)
    )
    SELECT 
        m.mois,
        COALESCE(td.montantTotalDevis, 0) AS montantTotalDevis
    FROM mois m
    LEFT JOIN total_devis td ON m.mois = td.mois
    ORDER BY m.mois;
END; $$ 
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION resetDatabase() RETURNS void
AS $$
DECLARE 
    table_name text; 
BEGIN 
    FOR table_name IN (SELECT tablename FROM pg_tables WHERE schemaname = 'public') 
    LOOP 
        EXECUTE 'TRUNCATE TABLE ' || table_name || ' RESTART IDENTITY CASCADE;'; 
    END LOOP;
    INSERT INTO profil(nom) VALUES('BTP'), ('Client');
    ALTER SEQUENCE dev_seq RESTART WITH 1;
    ALTER SEQUENCE paiement_seq RESTART WITH 1;
END; $$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION getMontantPayer(idDevis INT) RETURNS DOUBLE PRECISION
LANGUAGE plpgsql
AS $$
DECLARE
    montantPayer DOUBLE PRECISION;
BEGIN
    SELECT COALESCE(SUM(montant), 0) INTO montantPayer
    FROM paiement
    RIGHT JOIN devis ON devis.id = paiement.iddevis
    WHERE paiement.iddevis = $1;

    RETURN montantPayer;
END;
$$;

CREATE OR REPLACE FUNCTION getMontantRestant(idDevis INT) RETURNS DOUBLE PRECISION
AS $$
DECLARE
    montantPayer DOUBLE PRECISION;
    montantTotalDevis DOUBLE PRECISION;
BEGIN
    SELECT getMontantPayer(idDevis) INTO montantPayer;
    SELECT montantTravaux * (1 + (pourcentageFinition / 100)) INTO montantTotalDevis
    FROM devis
    WHERE devis.id = $1;

    RETURN montantTotalDevis - montantPayer;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION payer(idDevis INT, montant DOUBLE PRECISION, date TIMESTAMP) RETURNS void
AS $$
DECLARE 
    montantRestant DOUBLE PRECISION;
BEGIN
    IF montant < 0 THEN
        RAISE EXCEPTION 'Montant negatif non autorise';
    END IF;
    SELECT getMontantRestant(idDevis) INTO montantRestant;
    IF montant > montantRestant THEN
        RAISE EXCEPTION 'Le montant total des paiements depasse le montant total du devis';
    END IF;
    INSERT INTO paiement VALUES(default, idDevis, default, date, montant);
END;
$$
LANGUAGE plpgsql;
