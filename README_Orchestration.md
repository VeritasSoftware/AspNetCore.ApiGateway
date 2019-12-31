### Viewing your Gateway's Api Orchestration

Your Gateway's Api Orchestration is published by **GET api/Gateway/orchestration** endpoint. 

You can call the endpoint in **Swagger** to view your Api Orchestration.

This way, client developers can get information about your Gateway's Api Orchestration.

### GET Orchestration

![API Gateway Swagger](https://github.com/VeritasSoftware/AspNetCore.ApiGateway/blob/master/Orchestration.PNG)

The Api Orchestration returned has:

*	All Api Keys and their Route Keys,
*	the Request and Response Json Schema of each Route (if specified).

The response is like below:

```C#
[
  {
    "api": "weatherservice",
    "routes": [
      {
        "key": "forecast",
        "verb": "GET",
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
        "key": "types",
        "verb": "GET",
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
        "key": "typescustom",
        "verb": "GET",
        "requestJsonSchema": null,
        "responseJsonSchema": null
      },
      {
        "key": "add",
        "verb": "POST",
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
        "key": "remove",
        "verb": "DELETE",
        "requestJsonSchema": null,
        "responseJsonSchema": {
          "title": "String[]",
          "type": "array",
          "items": {
            "type": "string"
          }
        }
      }
    ]
  },
  {
    "api": "stockservice",
    "routes": [
      {
        "key": "stocks",
        "verb": "GET",
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
    ]
  }
]
```