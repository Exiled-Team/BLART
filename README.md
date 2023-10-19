# BLART

![Docker Pulls](https://img.shields.io/docker/pulls/xroier/blart?link=https%3A%2F%2Fhub.docker.com%2Fr%2Fxroier%2Fblart)

BLART is the official moderating and utils bot for EXILED.

# Running on Docker

Those are the instructions on how to run the `xroier/blart` Docker image with the specified environment variables. The image requires the following environment variables to be set:

- `DB_SERVER`: The database server address.
- `DB_DATABASE`: The name of the database to connect to.
- `DB_USER`: The username for the database connection.
- `DB_PASSWORD`: The password for the database connection.
- `BOT_TOKEN`: The token for your bot.
- `BOT_PREFIX`: The prefix for bot commands (default is "~").
- `DISC_STAFF_ID`: The Discord staff ID.
- `SPAM_LIMIT`: The spam message limit.
- `SPAM_TIMEOUT`: The spam timeout in minutes.
- `CHANNEL_RENT_ID`: The ID of the voice rent channel.
- `CHANNEL_RENT_CATEGORY_ID`: The ID of the voice rent category.
- `LOGS_CHANNEL_ID`: The ID of the logs channel.
- `DEBUG`: Set to "true" for debug mode, or "false" to disable debug.
- `RED_ROLE_ID`: The ID of the red role.
- `BUG_REPORT_CHANNEL_ID`: The ID of the bug report channel.
- `LENGHT_LIMIT`: The character length limit for ping triggers.
- `CONTRIBUTOR_ROLE_ID`: The ID of the contributor role.
- `NW_API_KEY`: The API key for northwood api requests.
- `STAFF_CHANNEL_ID`: The ID of the staff channel.
- `CREDIT_ROLE_IDS`: Comma-separated IDs for credit roles.

Make sure you have Docker installed on your system before proceeding.

## Running the Docker Container

To run the `xroier/blart` Docker container with the specified environment variables, you can use the following `docker run` command. Replace the placeholders with the actual values for your environment:

```bash
docker run -d \
  -e DB_SERVER=<YourDBServer> \
  -e DB_DATABASE=<YourDBDatabase> \
  -e DB_USER=<YourDBUser> \
  -e DB_PASSWORD=<YourDBPassword> \
  -e BOT_TOKEN=<YourBotToken> \
  -e BOT_PREFIX=<BotPrefix> \
  -e DISC_STAFF_ID=<YourDiscStaffID> \
  -e SPAM_LIMIT=<YourSpamLimit> \
  -e SPAM_TIMEOUT=<YourSpamTimeout> \
  -e CHANNEL_RENT_ID=<YourRentChannelID> \
  -e CHANNEL_RENT_CATEGORY_ID=<YourRentCategoryID> \
  -e LOGS_CHANNEL_ID=<YourLogsChannelID> \
  -e DEBUG=<true_or_false> \
  -e RED_ROLE_ID=<YourRedRoleID> \
  -e BUG_REPORT_CHANNEL_ID=<YourBugReportChannelID> \
  -e LENGHT_LIMIT=<YourLengthLimit> \
  -e CONTRIBUTOR_ROLE_ID=<YourContributorRoleID> \
  -e NW_API_KEY=<YourApiKey> \
  -e STAFF_CHANNEL_ID=<YourStaffChannelID> \
  -e CREDIT_ROLE_IDS=<YourCreditRoleIDs> \
  docker.io/xroier/blart
```

Replace `<YourDBServer>`, `<YourDBDatabase>`, `<YourDBUser>`, and so on with your specific values for each environment variable.

After running the command, the Docker container will start with the specified environment variables, and the `xroier/blart` application will use these settings.