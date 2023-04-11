### Viewing your Gateway's Api Orchestration

Your Gateway's Api Orchestration is published by **GET /api/Gateway/orchestration** endpoint. 

You can call the endpoint in **Swagger** to view your Api Orchestration.

**Client developers can use this information to make calls to your Gateway API.**

### GET Orchestration

![API Gateway Swagger](/Docs/Orchestration.PNG)

You can filter by Api Key. This way you can get all the information on an Api.

Eg.
**/api/Gateway/orchestration?api=weatherservice**

You can filter by Api Key and Route Key. This way you can get all information on a Route.

Eg.
**/api/Gateway/orchestration?api=weatherservice&key=forecast**

The filtering supports partial match.

**Note:**

You can secure this endpoint by implementing interface **IGetOrchestrationGatewayAuthorization** in your Gateway API project.

Please see [**Authorization**](README_Authorization.md) section for more information on how to do this.

The endpoint response is like below:

```json
[
  {
    "api": "weatherservice",
    "apiRoutes": [
      {
        "key": "forecast",
        "verb": "GET",
        "downstreamPath": "weatherforecast/forecast",
        "requestJsonSchema": null,
        "responseJsonSchema": {
          "title": "IEnumerableOfWeatherForecast",
          "type": "array",
          "items": {},
          "definitions": {
            "WeatherForecast": {
              "type": "object",
              "additionalProperties": false,
              "properties": {
                "Date": {
                  "type": "string",
                  "format": "date-time"
                },
                "TemperatureC": {
                  "type": "integer",
                  "format": "int32"
                },
                "TemperatureF": {
                  "type": "integer",
                  "format": "int32"
                },
                "Summary": {
                  "type": [
                    "null",
                    "string"
                  ]
                }
              }
            }
          }
        }
      },
      {
        "key": "forecasthead",
        "verb": "HEAD",
        "downstreamPath": "weatherforecast/forecast",
        "requestJsonSchema": null,
        "responseJsonSchema": null
      },
      {
        "key": "typewithparams",
        "verb": "GET",
        "downstreamPath": "weatherforecast/types/{index}",
        "requestJsonSchema": null,
        "responseJsonSchema": null
      },
      {
        "key": "types",
        "verb": "GET",
        "downstreamPath": "weatherforecast/types",
        "requestJsonSchema": null,
        "responseJsonSchema": {
          "title": "String[]",
          "type": "array",
          "items": {
            "type": "string"
          }
        }
      },
      {
        "key": "type",
        "verb": "GET",
        "downstreamPath": "weatherforecast/types/",
        "requestJsonSchema": null,
        "responseJsonSchema": {
          "title": "WeatherTypeResponse",
          "type": "object",
          "additionalProperties": false,
          "properties": {
            "Type": {
              "type": [
                "null",
                "string"
              ]
            }
          }
        }
      },
      {
        "key": "forecast-custom",
        "verb": "GET",
        "downstreamPath": null,
        "requestJsonSchema": null,
        "responseJsonSchema": null
      },
      {
        "key": "add",
        "verb": "POST",
        "downstreamPath": "weatherforecast/types/add",
        "requestJsonSchema": {
          "title": "AddWeatherTypeRequest",
          "type": "object",
          "additionalProperties": false,
          "properties": {
            "weatherType": {
              "type": [
                "null",
                "string"
              ]
            }
          }
        },
        "responseJsonSchema": {
          "title": "String[]",
          "type": "array",
          "items": {
            "type": "string"
          }
        }
      },
      {
        "key": "update",
        "verb": "PUT",
        "downstreamPath": "weatherforecast/types/update",
        "requestJsonSchema": {
          "title": "UpdateWeatherTypeRequest",
          "type": "object",
          "additionalProperties": false,
          "properties": {
            "weatherType": {
              "type": [
                "null",
                "string"
              ]
            },
            "index": {
              "type": "integer",
              "format": "int32"
            }
          }
        },
        "responseJsonSchema": {
          "title": "String[]",
          "type": "array",
          "items": {
            "type": "string"
          }
        }
      },
      {
        "key": "patch",
        "verb": "PATCH",
        "downstreamPath": "weatherforecast/forecast/patch",
        "requestJsonSchema": null,
        "responseJsonSchema": {
          "title": "WeatherForecast",
          "type": "object",
          "additionalProperties": false,
          "properties": {
            "Date": {
              "type": "string",
              "format": "date-time"
            },
            "TemperatureC": {
              "type": "integer",
              "format": "int32"
            },
            "TemperatureF": {
              "type": "integer",
              "format": "int32"
            },
            "Summary": {
              "type": [
                "null",
                "string"
              ]
            }
          }
        }
      },
      {
        "key": "remove",
        "verb": "DELETE",
        "downstreamPath": "weatherforecast/types/remove/",
        "requestJsonSchema": null,
        "responseJsonSchema": {
          "title": "String[]",
          "type": "array",
          "items": {
            "type": "string"
          }
        }
      }
    ],
    "orchestrationType": 0
  },
  {
    "api": "stockservice",
    "apiRoutes": [
      {
        "key": "stocks",
        "verb": "GET",
        "downstreamPath": "stock",
        "requestJsonSchema": null,
        "responseJsonSchema": {
          "title": "IEnumerableOfStockQuote",
          "type": "array",
          "items": {},
          "definitions": {
            "StockQuote": {
              "type": "object",
              "additionalProperties": false,
              "properties": {
                "CompanyName": {
                  "type": [
                    "null",
                    "string"
                  ]
                },
                "CostPerShare": {
                  "type": [
                    "null",
                    "string"
                  ]
                }
              }
            }
          }
        }
      },
      {
        "key": "stock",
        "verb": "GET",
        "downstreamPath": "stock/",
        "requestJsonSchema": null,
        "responseJsonSchema": {
          "title": "StockQuote",
          "type": "object",
          "additionalProperties": false,
          "properties": {
            "CompanyName": {
              "type": [
                "null",
                "string"
              ]
            },
            "CostPerShare": {
              "type": [
                "null",
                "string"
              ]
            }
          }
        }
      }
    ],
    "orchestrationType": 0
  },
  {
    "api": "chatservice",
    "hubRoutes": [
      {
        "key": "room",
        "invokeMethod": "SendMessage",
        "receiveMethod": "ReceiveMessage",
        "receiveGroup": "ChatGroup",
        "broadcastType": "Group",
        "receiveParameterTypes": [
          "String",
          "String"
        ]
      }
    ],
    "orchestrationType": 1
  },
  {
    "api": "eventsourceservice",
    "eventSourceRoutes": [
      {
        "key": "mystream",
        "type": "EventStore",
        "receiveMethod": "ReceiveMyStreamEvent",
        "operationType": "PublishSubscribe",
        "streamName": "my-stream",
        "groupName": "my-group"
      }
    ],
    "orchestrationType": 2
  }
]
```