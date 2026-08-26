-- Migration 0016: Advanced Curriculum Matrix & Direct Measurement Plan Grants
GRANT USAGE ON SCHEMA academic, portfolio, measurement, workflow TO outcomehub_app;

GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE academic.direct_measurement_plan TO outcomehub_app;
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE academic.direct_measurement_source TO outcomehub_app;
GRANT SELECT ON ALL TABLES IN SCHEMA academic TO outcomehub_app;
GRANT SELECT ON ALL TABLES IN SCHEMA measurement TO outcomehub_app;
