# Usage Guide

## Configuring the REST service

After installation, you will find a new **REST** category under the **Settings application**. This allows you to configure the REST service without the need for re-deployment. The following settings are currently available:

- **Enabled**: If checked, the REST endpoints are enabled and data can be queried/modified. When disabled, the base path `/rest` is still available to view the configuration
- **Allowed object types**: A semi-colon separated list of Xperience by Kentico object types that can be retrieved and modified at the REST endpoints. If empty, all object types are allowed and listed at the `/rest` endpoint

## Authenticating REST requests

Requests to the endpoints are authenticated using [Basic authentication](https://developer.mozilla.org/en-US/docs/Web/HTTP/Guides/Authentication#basic), which involves sending an Xperience by Kentico user's credentials as a Base64 encoded string. By nature, this type of authentication is _not_ secure as anyone who intercepts the request can decode the authentication string. A secure connection must be used, and you should create a user with limited [permissions](https://docs.kentico.com/documentation/developers-and-admins/configuration/users/role-management) to authenticate the requests.

To authenticate a REST request, set the **Authorization** header value to "Basic" plus the Base64 encoded credentials in the format "[username]:[password]." The end result will look something like this:

`Authorization: Basic UmVzdENsaWVudDpNeVBhc3N3b3Jk`

If you are testing the REST service with a client such as Postman, you should be able to enter the credentials directly into a form and the client will add the encoded header for you.

## Endpoints

The `/rest` endpoint accepts the following methods:

- GET: Gets [service information](#service-information), [metadata](#getting-metadata), and [single](#getting-a-single-object) or [multiple objects](#getting-multiple-objects)
- POST: [Creates](#creating-an-object) a new object
- PATCH: Partially [updates](#updating-an-object) an object
- DELETE: [Deletes](#deleting-an-object) an object

### Service information

> [!TIP]
>
> - **Method**: GET
> - **Path**: /rest
> - **Request body**: (none)
> - **Response body**: [IndexResponse](/src/Models/Responses/IndexResponse.cs)

Displays information about the REST service's [configuration](#configuring-the-rest-service). This endpoint will always respond to requests, even when the service is disabled.

Sample response:

```json
{
  "enabled": true,
  "enabledObjects": ["cms.user", "om.contact"],
  "enabledForms": ["bizform.dancinggoatcontactus"]
}
```

### Getting metadata

> [!TIP]
>
> - **Method**: GET
> - **Path**: /rest/metadata/[objectType]
> - **Request body**: (none)
> - **Response body**: [ObjectMeta](/src/Models/ObjectMeta.cs)

Gets metadata about the object type and available fields.

Sample request and response:

```json
// http://localhost/rest/metadata/bizform.dancinggoatcontactus
{
  "objectType": "bizform.dancinggoatcontactus",
  "displayName": "Contact Us",
  "codeNameColumn": null,
  "idColumn": "Form_2023_09_12_17_45ID",
  "guidColumn": null,
  "classType": "Form",
  "fields": [
    {
      "name": "Form_2023_09_12_17_45ID",
      "caption": null,
      "isRequired": true,
      "isUnique": false,
      "size": 0,
      "dataType": "integer",
      "defaultValue": null
    },
    {
      "name": "UserMessage",
      "caption": "Message",
      "isRequired": true,
      "isUnique": false,
      "size": 0,
      "dataType": "longtext",
      "defaultValue": null
    }
    // Other fields...
  ]
}
```

### Creating an object

> [!TIP]
>
> - **Method**: POST
> - **Path**: /rest
> - **Request body**: [CreateRequestBody](/src/Models/Requests/CreateRequestBody.cs)
> - **Response body**: (an object containing fields of the created object)

Creates a new object determined by the request body:

- ObjectType: The name of the object type to create, e.g. "om.contact"
- Fields: An array of the object's field names and values. Be sure to provide values for any required fields of the object type, or the request will fail

Sample payload:

```json
{
    "ObjectType": "om.contact",
    "Fields": [
        "ContactEmail": "test@localhost.com",
        "ContactFirstName": "Juraj",
        "ContactLastName": "Ondrus"
    ]
}
```

### Updating an object

> [!TIP]
>
> - **Method**: PATCH
> - **Path**: /rest
> - **Request body**: [UpdateRequestBody](/src/Models/Requests/UpdateRequestBody.cs)
> - **Response body**: (an object containing fields of the updated object)

Partially updates an object determined by the request body:

- ObjectType: The name of the object type to update, e.g. "om.contact"
- Fields: An array of the object's field names and values. Only fields provided here are updated- other fields of the object are untouched
- One of the following properties which identifies the object to update:
  - Id
  - CodeName
  - Guid

Sample payload:

```json
{
    "ObjectType": "om.contact",
    "Id": 4,
    "Fields": [
        "ContactFirstName": "John",
        "ContactLastName": "Lougee"
    ]
}
```

### Deleting an object

> [!TIP]
>
> - **Method**: DELETE
> - **Path**: /rest
> - **Request body**: [DeleteRequestBody](/src/Models/Requests/DeleteRequestBody.cs)
> - **Response body**: (an object containing fields of the deleted object)

Deletes an object determined by the request body:

- ObjectType: The name of the object type to delete, e.g. "om.contact"
- One of the following properties which identifies the object to delete:
  - Id
  - CodeName
  - Guid

Sample payload:

```json
{
  "ObjectType": "cms.user",
  "CodeName": "andy"
}
```

### Getting a single object

> [!TIP]
>
> - **Method**: GET
> - **Path**:
>   - /rest/[objectType]/[id]
>   - /rest/[objectType]/[guid]
>   - /rest/[objectType]/[codeName]
> - **Request body**: (none)
> - **Response body**: (an object containing fields of the retrieved object)

Gets a single object by ID, code name, or GUID.

Sample request and response:

```json
// http://localhost/rest/cms.user/42
{
  "UserID": 42,
  "UserName": "andy"
  // Other fields...
}
```

### Getting multiple objects

> [!TIP]
>
> - **Method**: GET
> - **Path**: /rest/[objectType]/all
> - **Request body**: (none)
> - **Response body**: [GetAllResponse](/src/Models/Responses/GetAllResponse.cs)

Gets a list of objects. The following _optional_ query string parameters can be provided to further configure the data retrieval:

| Parameter | Description                                                                              | Example                               |
| --------- | ---------------------------------------------------------------------------------------- | ------------------------------------- |
| Where     | A SQL WHERE condition used to filter the returned objects                                | where=ContactEmail like '%localhost%' |
| Columns   | The columns of the object type to return, separated by comma                             | columns=UserName,UserID               |
| OrderBy   | A SQL ORDER BY clause used to order the results. May optionally contain "asc" and "desc" | orderby=UserName desc                 |
| TopN      | The number of objects to retrieve                                                        | topn=20                               |
| PageSize  | The number of objects per page                                                           | pagesize=5                            |
| Page      | The page to retrieve, where 0 is the first page                                          | page=2                                |

Sample requests and responses:

```json
// http://localhost/rest/om.contact/all
{
  "totalRecords": 300,
  "nextUrl": null,
  "objects": [
    {
      "ContactID": 1,
      "ContactFirstName": "John",
      "ContactMiddleName": null,
      "ContactLastName": "Smith",
      "ContactGUID": "e4138cff-a05a-42e3-b37e-fef692000cd0",
      "ContactLastModified": "2025-07-23T10:42:50.0047821",
      "ContactCreated": "2025-07-21T14:41:37.9021027"
      // Other fields...
    }
    // Other objects...
  ]
}
```

```json
// http://localhost/rest/om.contact/all?where=contactemail is not null&columns=contactemail&orderby=contactemail
{
  "totalRecords": 300,
  "nextUrl": null,
  "objects": [
    {
      "ContactEmail": "test1@localhost.com"
    },
    {
      "ContactEmail": "test2@localhost.com"
    }
    // Other objects...
  ]
}
```

#### Paging results

To paginate the results of a GET request, add the **Page** and **PageSize** query string parameters described in [Getting multiple objects](#getting-multiple-objects). Pages start at index zero, so to get the first page of the listing with 10 objects per page, the parameters would be: `?page=0&pagesize=10`, The **TotalRecords** property lists the total number of objects for pagination. If another page of objects is available, the **NextUrl** property will be non-null and contain the absolute URL for the next page.

Sample request and response:

```json
// http://localhost/rest/cms.webpageurlpath/all?page=2&pagesize=5&columns=WebPageUrlPath
{
  "totalRecords": 110,
  "nextUrl": "http://localhost/rest/cms.webpageurlpath/all?page=3&pagesize=5&columns=WebPageUrlPath",
  "objects": [
    {
      "WebPageUrlPath": "Articles/Origins-of-Arabica-Bourbon",
      "WebPageUrlPathID": 11
    },
    {
      "WebPageUrlPath": "es/Articles/Origins-of-Arabica-Bourbon",
      "WebPageUrlPathID": 12
    },
    {
      "WebPageUrlPath": "Articles/The-Resilient-Robusta-A-Coffee-with-Character",
      "WebPageUrlPathID": 13
    },
    {
      "WebPageUrlPath": "es/Articles/The-Resilient-Robusta-A-Coffee-with-Character",
      "WebPageUrlPathID": 14
    },
    {
      "WebPageUrlPath": "Articles/Which-brewing-fits-you",
      "WebPageUrlPathID": 15
    }
  ]
}
```
