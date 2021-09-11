# Rate Simulator

This project is the back-end of the [Rate Simulator](https://rate-simulator.herokuapp.com/) , it's a REST API to analyze light consumption. To do so, you need to provide the bill extraction in **CSV** format, for now only supports **Endesa** format (as it's the main provider in Spain). You can download the bill export file from the [client area](https://www.endesaclientes.com/oficina/mis-facturas.html).

## Example file

The format of the CSV file that is provided doesn't follow the common pattern of having a header and then the rows. For example, at the beginning when parsing the csv we have to skip 5 lines of bill info and also stop reading before the last line of the file.

[Example file](consumptionExample.csv)

## Purpose

The purpose of this project is to be aware of which electricity rate plan fits better with your consumption habits. The two main rates are compared, [One Luz](https://www.endesa.com/es/luz-y-gas/luz/one/tarifa-one-luz)  provides always the same price however [One Luz 3 Periodos](https://www.endesa.com/es/luz-y-gas/luz/one/tarifa-one-luz-3periodos) varies according to the time or if it's weekend.

## Motivation

After reading  [Head First Design Patterns](https://www.oreilly.com/library/view/head-first-design/9781492077992/) I was looking for some project ideas where I could apply the knowledge acquired. Then I realised that I could take profit of what I was watching everyday in the news, the light rise. 

So in this project I have been able to be in touch with some patterns like **Strategy** as each rate can be seen as a way of performing some operations over the same input. Moreover, rate instances are created through a **Factory**. It also uses **Dependency Injection** for the configuration and the logger. 

This project also has been an opportunity to learn a bit more of the deployment stage,  the back-end it's using a Dockerfile and hosted in Azure. The front-end (React) it's on heroku with a pipeline that builds the master branch.

## Get default price configuration

### Request

`GET /api/Rate`

### Response

```json
{
    "pricePerKwHPunta": 0.262682,
    "pricePerKwHLlano": 0.164195,
    "pricePerKwHValle": 0.122177,
    "pricePerKwH": 0.173941
}
```

## Analyze files

In this endpoint is where you can upload the files to analyze. Also allows to change the price configuration for this specific request.

### Request

`POST /api/Rate`

`"Content-Type: multipart/form-data" -F "files=@consumption.csv;type=application/vnd.ms-excel" -F "PricePerKwHLlano=0,2315"`

 ### Response

```json
{
    "oneLuz": {
        "cost": 64.897908923,
        "consumptionBreakdown": {
            "unique-rate": 373103
        }
    },
    "oneLuz3Periodos": {
        "cost": 63.513783024999995,
        "consumptionBreakdown": {
            "llano": 96443,
            "punta": 98764,
            "valle": 177896
        }
    }
}
```



