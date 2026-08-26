-- Migration 0015: Dashboard and Accreditation Analytics Grants for outcomehub_app
GRANT USAGE ON SCHEMA academic, portfolio, measurement, result, quality, iam, audit, governance, reporting TO outcomehub_app;

GRANT SELECT ON ALL TABLES IN SCHEMA academic TO outcomehub_app;
GRANT SELECT ON ALL TABLES IN SCHEMA portfolio TO outcomehub_app;
GRANT SELECT ON ALL TABLES IN SCHEMA measurement TO outcomehub_app;
GRANT SELECT ON ALL TABLES IN SCHEMA result TO outcomehub_app;
GRANT SELECT ON ALL TABLES IN SCHEMA quality TO outcomehub_app;
GRANT SELECT ON ALL TABLES IN SCHEMA governance TO outcomehub_app;
GRANT SELECT ON ALL TABLES IN SCHEMA reporting TO outcomehub_app;
