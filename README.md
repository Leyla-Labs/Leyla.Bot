# Leyla.Bot

Privacy-focused Administration Bot for Discord

### Docker Environment Variables

| Environment Variable | Description                                             | Default Value                                               |
|----------------------|---------------------------------------------------------|-------------------------------------------------------------|
| MODULES              | Enables or disables modules.                            | main;logs;spam                                              |
| CONNECTION_STRING    | EFCore connection string for Postgres database.         | Host=host.docker.internal;Database=leyla_dev;Username=tawmy |
| MAIN_CHANNEL         | Default bot output will be posted in this channel (ID). |                                                             |
| TOKEN_MAIN           | Discord token for main bot. Required for operation.     |                                                             |
| TOKEN_LOGS           | Discord token for logs bot. Optional.                   |                                                             |
| TOKEN_SPAM           | Discord token for anti-spam bot. Optional.              |                                                             |