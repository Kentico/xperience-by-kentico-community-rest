# Xperience Community: REST

[![Nuget](https://img.shields.io/nuget/v/Xperience.Community.Rest)](https://www.nuget.org/packages/Xperience.Community.Rest#versions-body-tab)
[![CI: Build and Test](https://github.com/kentico-ericd/xperience-community-rest/actions/workflows/ci.yml/badge.svg)](https://github.com/kentico-ericd/xperience-community-rest/actions/workflows/ci.yml)

## Description

This integration adds endpoints to your Xperience by Kentico website allowing for data retrieval and manipulation via REST requests. Using these endpoints, you can create, update, and delete objects, as well as retrieve single or multiple objects with paging support.

## Library Version Matrix

| Xperience Version | Library Version |
| ----------------- | --------------- |
| >= 30.0.0         | >= 1.0.0        |

## :gear: Package Installation

Add the package to your application using the .NET CLI

```powershell
dotnet add package Xperience.Community.Rest
```

## 🚀 Quick Start

After installation, no further steps are necessary and the REST service is available at the `/rest` endpoint. By default, the service is disabled and only a GET request to the base endpoint is allowed. Log into the administration to further [configure the service](/docs/Usage-Guide.md#configuring-the-rest-service).

## Full Instructions

View the [Usage Guide](docs/Usage-Guide.md) for more detailed instructions.
