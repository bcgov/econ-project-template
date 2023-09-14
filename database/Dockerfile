FROM postgres:latest

# Health check - recommended for local dev
HEALTHCHECK --interval=15s --timeout=15s --retries=3 CMD [ "pg_isready" ]

# Non-privileged user
USER postgres
