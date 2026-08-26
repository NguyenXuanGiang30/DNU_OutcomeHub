-- Migration 0019: AI Assistant, RAG Diagnostics & Prompt Governance Grants
GRANT USAGE ON SCHEMA academic, portfolio, measurement, integration, iam, workflow TO outcomehub_app;

GRANT SELECT ON ALL TABLES IN SCHEMA academic TO outcomehub_app;
GRANT SELECT ON ALL TABLES IN SCHEMA portfolio TO outcomehub_app;
GRANT SELECT ON ALL TABLES IN SCHEMA measurement TO outcomehub_app;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA integration TO outcomehub_app;
