# ForecastAlert
A [TimeTrigger](https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-timer?tabs=python-v2%2Cisolated-process%2Cnodejs-v4&pivots=programming-language-csharp) Azure function which lets the user monitor different weather conditions and notify if these exceed a specified threshold value.

## Setup
### Azure
#### Blob Storage
This function needs an `AlarmConfig.json` file stored in a [blob storage](https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blobs-introduction) in the Azure portal.
An example of the `AlarmConfig.json` can be found below.

#### Environment Variables
This function requires some environment variables to work which will differ from user to user:
- SLACK_KEY - URL to Slack workflow. Should be setup as a secret
- MET_USER_AGENT - See [MET API Terms of Service](https://api.met.no/doc/TermsOfService)
- BLOB_URI - See Blog Storage section
- BLOB_CONTAINER - See Blog Storage section
- BLOB_NAME - See Blog Storage section
- 
#### App Setting
- `ForecastSchedule` - Timetrigger Cron tab



## Alert notification channels
The function currently only support **ONE** notification channel, which is Slack. 

## Alarm Config
The `AlarmConfig.json` is where you setup the alarms for the different locations you want to monitor. 

### Alarm types
The function currently supports two alarm types: `Wind` and `Tidal`.

#### Wind
Notify when the forecast exceeds the `MaxWindSpeed` threshold value. It can be further specified with one or more wind directions.
#### Tidal
Notify when the forecast exceeds the `MaxWaterLevels` threshold value

### Example
```
{
  "Locations": [
    {
      "Latitude": "59.9139",
      "Longitude": "10.7522",
      "LocationName": "Oslo",
      "Alarms": [
        {
          "AlarmClass": "TidalClass",
          "AlarmType": "Tidal",
          "Name": "Oslo - High sea level",
          "MaxWaterLevels": 300
        },
        {
          "AlarmClass": "WindClass",
          "AlarmType": "Wind",
          "Name": "Oslo - Strong wind",
          "MaxWindSpeed": 25
        }
      ]
    },
    {
      "Latitude": "60.3913",
      "Longitude": "5.3221",
      "LocationName": "Bergen",
      "Alarms": [
        {
          "AlarmClass": "WindClass",
          "AlarmType": "Wind",
          "Name": "Bergen - String wind (S, W, SW)",
          "MaxWindSpeed": 25,
          "Directions": [
            "South",
            "West",
            "SouthWest"
          ]
        }
      ]
    }
  ]
}
```

## Attribution
- [Kartverket Vannstand API](https://kartkatalog.geonorge.no/metadata/vannstand-api/6f2b2773-d128-4472-a463-d15b1d4aa02f) - [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/deed.no)
- [MET LocationForecast 2.0 API](https://api.met.no/weatherapi/locationforecast/2.0/documentation) - [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/)

## License
## Copyright