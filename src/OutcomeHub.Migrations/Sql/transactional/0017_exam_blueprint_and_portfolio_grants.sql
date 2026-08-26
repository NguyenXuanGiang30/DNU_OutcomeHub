-- Migration 0017: Exam Blueprint, Assessment Matrices & Portfolio Vault Grants
GRANT USAGE ON SCHEMA academic, portfolio, document, measurement, workflow, ai TO outcomehub_app;

GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA portfolio TO outcomehub_app;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA document TO outcomehub_app;
GRANT SELECT ON ALL TABLES IN SCHEMA ai TO outcomehub_app;
