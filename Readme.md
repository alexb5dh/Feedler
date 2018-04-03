# Feedler

This is a ~~web debugging tool from Telerik~~ simple news aggregator API developed for learning purposes only.

## Implementation status

||Function|Comment
|---|---|---
|✅|Managing feed collections|User can create and manage groups of feeds.
|✅|Fetching collection news|User can load news grouped for all feeds in collection.
|✅|Caching|For each feed source data is cached for a short time after loading via Redis.
|✅|GUI for demonstration|Swagger UI is available on http://\<project-url\>/swagger.
|❌|.NET SDK||
|❌|Authentication|Not yet implemented.|
|✅|Persistence|Supported feeds and created collections are stored in SQL Server database and managed via Entity Framework.|
|✅|Logging|All write/update/delete actions are logged via default ASP.NET Core logging framework.
|✅|Tests|Full-pipeline tests are implemented for most API endpoints.

## Prerequisites

1. **Redis** instance is needed for app and tests as it's used for caching in both. Endpoint can be configured in *appsettings.\*.json*.
2. **SQL Server** is needed for app only as it's used for storing feeds and collections. Connection string can be configured in *appsettings.\*.json*.
