# Leyla.Bot

Privacy-focused Administration Bot for Discord

[![Codacy Badge](https://app.codacy.com/project/badge/Grade/de4216d87f1240d98b5383a7d4e58df8)](https://www.codacy.com/gh/Leyla-Labs/Leyla.Bot/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Leyla-Labs/Leyla.Bot&amp;utm_campaign=Badge_Grade)
[![CodeQL](https://github.com/Leyla-Labs/Leyla.Bot/actions/workflows/codeql.yml/badge.svg)](https://github.com/Leyla-Labs/Leyla.Bot/actions/workflows/codeql.yml)

## Docker Environment Variables

| Environment Variable | Description                                             | Required                           | Default Value                                                                              |
|----------------------|---------------------------------------------------------|------------------------------------|--------------------------------------------------------------------------------------------|
| MODULES              | Enables or disables modules.                            | Must contain 'main' for operation. | main;logs;spam                                                                             |
| CONNECTION_STRING    | EFCore connection string for Postgres database.         | Yes                                | Host=host.docker.internal;<br/>Database=leyla_dev;<br/>Username=tawmy;<br/>Password=abc123 |
| MAIN_CHANNEL         | Default bot output will be posted in this channel (ID). | Yes                                |                                                                                            |
| TOKEN_MAIN           | Discord token for main bot.                             | Yes                                |                                                                                            |
| TOKEN_LOGS           | Discord token for logs bot.                             | If MODULES contains 'logs'         |                                                                                            |
| TOKEN_SPAM           | Discord token for anti-spam bot.                        | If MODULES contains 'spam'         |                                                                                            |
| ID_MAIN              | Discord ID for main bot.                                | Yes                                |                                                                                            |
| ID_LOGS              | Discord ID for logs bot.                                | If MODULES contains 'logs'         |                                                                                            |
| ID_SPAM              | Discord ID for anti-spam bot.                           | If MODULES contains 'spam'         |                                                                                            |
