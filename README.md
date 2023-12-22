# SnackAdmin project based on snacks-bb-g1-heseltime-g1-ilk
snacks-bb-g1-heseltime-g1-ilk created by GitHub Classroom, SWK5 project with documentation (this Readme) - Hagenberg Software Engineering project. Documentation in English for the most part, with some evaluation in German.

## Part 1 (Ausbaustufe 1)

**Development Process**: We use git locally and host on GitHub: Both project group members agree to review each other's code in pull requests to the main branch, from feature branches. We provide feedback in GitHub's Code Review functionality.

We work from the ADO.NET project **directory structure** presented in UE5, as a first step. Basic structure:

```
|-   snacks-bb-g1-heseltime-g1-ilk
|    |-- .github
|        |-- workflows (see notes on pipeline below)
|    |-- SnackBackend incl. tests
|    |-- Database incl. data scripts
|    REAMDE.md
```

We also use GitHub for **Issue Tracking**. We make new branches per issue as well.

**Technical Notes on Pipeline**: Build-pipeline only for the moment, GitHub runner runs in Docker, database as well. From this we aim to build a test pipeline that can access the individual components over the Docker network.

**Testing Part 1 (DB-Integration Tests:** The testing so far establishes the pipeline/network setup for db access and is implemented following this DAO-Test for the Restaurant entity. Here without Docker started (no db-connection, the test fails):

![Alt text](_Documents\Bilder\1.png)
<!-- <img width="894" alt="image" src="https://github.com/swk5-2023ws/snacks-bb-g1-heseltime-g1-ilk/assets/66922223/a1d211dc-1563-42aa-85bd-cfbc5b37c3ed"> -->

And here after starting Docker, where all the tests run through:

![Alt text](_Documents\Bilder\2.png)
<!-- <img width="898" alt="Screenshot 2023-11-11 192556" src="https://github.com/swk5-2023ws/snacks-bb-g1-heseltime-g1-ilk/assets/66922223/bd4a358d-67b5-4ec0-9047-36d8e7284f96"> -->

These integration tests are located in SnackAdmin.Dal.IntegrationTest (end to end connectivity tests), and the project for the future unit tests for our code is SnackAdmin.BusinessLogic.Test, where we aim to test Controllers.

Our answers to the question re: Part 1 of the project follow.

### Data Model

The data is **modeled in an ER Model** according to the use cases 1-12 and summarized in this diagram:

![Alt text](_Documents\Bilder\Datenbank-Schema.png)

## Snack Project Database Entities

### 1. Address
- `id` (Primary Key)
- `street`
- `postal_code`
- `city`
- `state`
- `country`

### 2. Restaurant
- `id` (Primary Key)
- `name`
- `address_id` (Foreign Key referencing Address)
- `gps_lat`
- `gps_long`
- `webhook_url`
- `title_image`
- `api_key`

### 3. Menu
- `id` (Primary Key)
- `restaurant_id` (Foreign Key referencing Restaurant)
- `category`
- `item_name`
- `description`
- `price`

### 4. Snack Order
- `id` (Primary Key)
- `restaurant_id` (Foreign Key referencing Restaurant)
- `address_id` (Foreign Key referencing Address)
- `gps_lat`
- `gps_long`
- `free_text`
- `status`

### 5. Delivery Condition
- `id` (Primary Key)
- `restaurant_id` (Foreign Key referencing Restaurant)
- `distance`
- `min_order_value`
- `delivery_cost`

### 6. Order Item
- `order_id` (Composite Primary Key, part of Foreign Key referencing Snack Order)
- `menu_id` (Composite Primary Key, part of Foreign Key referencing Menu)
- `quantity`

### 7. Opening Hours
- `id` (Primary Key)
- `restaurant_id` (Foreign Key referencing Restaurant)
- `day`
- `open_time`
- `close_time`

## Relationships in Snack Project Database

### Address
- Referenced by `Restaurant` (via `address_id`)
- Referenced by `Snack Order` (via `address_id`)

### Restaurant
- Has many `Menus` (via `restaurant_id` in `Menu`)
- Has many `Snack Orders` (via `restaurant_id` in `Snack Order`)
- Has many `Delivery Conditions` (via `restaurant_id` in `Delivery Condition`)
- Has many `Opening Hours` entries (via `restaurant_id` in `Opening Hours`)
- References `Address` (via `address_id`)

### Menu
- Belongs to `Restaurant` (via `restaurant_id`)
- Referenced by `Order Item` (via `menu_id`)

### Snack Order
- Belongs to `Restaurant` (via `restaurant_id`)
- References `Address` (via `address_id`)
- Has many `Order Items` (via `order_id` in `Order Item`)

### Delivery Condition
- Belongs to `Restaurant` (via `restaurant_id`)

### Order Item
- Belongs to `Snack Order` (via `order_id`)
- Belongs to `Menu` (via `menu_id`)

### Opening Hours
- Belongs to `Restaurant` (via `restaurant_id`)

The SQL to **implement** this model is located in the Database folder, in the sub-directory init, 01_setup_snack_db.sql, along with a script to generate dummy data (reset with the docker-compose up command, so the database is fresh every time, for testing mainly).


## Part 2 (Ausbaustufe 2)

### Requirements for API

#### Business management for restaurant owners
- `business/`
  - POST - e.g. registrate as a new restaurant
- ~~`business/{apikey}/`~~
  - ~~GET - login by apikey~~ *
- `business/{apikey}/orders/`
  - GET
- `business/{apikey}/orders/{orderId}`
  - GET
  - PUT (esp. for Requirement No. 10)
- `business/{apikey}/orders/{orderToken, newOrderStatus}`
  - PUT (Requirement No. 11!)
- `business/{apikey}/menus/` (Requirement No. 4)
  - GET
  - PUT
  - DELETE
- `business/{apikey}/deliverycosts/` (Requirement No. 3)
  - GET
  - PUT
  - DELETE

 *Anmerkung: Login via API Key an 
 - Auth/login

#### Restaurant managment
- `restaurants/{radius}/`
  - GET - lists all restaurants within radius

#### Order management
- `orders/`
  - POST - new order by getting the menu items and the delivery address
- `orders/({restaurantId}:{orderId})/`
  - url for composite keys
  - GET - customer should be able to check his order

#### Snack management
- `menus/{restaurantId}/`
  - GET - lists all menu items



### Possible improvements

#### Common
1. instead of a restaurant id within the url, it could be possible to use a combination between restaurant name and the partial address e.g. `https://www.lieferando.at/speisekarte/hellmon-pizzeria-4202-hellmonsoedt`
2. instead of int as ID's, it would be a much better solution to use Guid's.
    + e.g. at creating an order automatically add a guid to find the order via url
    + increases also safety issues, especially orders/{orderid} would be easily accessable and the delivery address public.
3. the database design was chosen so that there is no redundant data. However, this means that price changes affect orders that have already been placed.

#### Orders
1. to check the delivery conditions it's necessary to calculate the distance between the gps coordinate. Assuming the gps coordinates matches to the delivery address it would work, but otherwise e.g. a different delivery address would cause problems. In this case it would be better to use an additional api e.g goolge maps to calculate distance between two addresses. 


### Frontend tasks
- calculation of total price before ordering
- validation of the minmal price to order


# Evaluierung (DE)
## Beantwortung der Fragen
1. Für welches Datenmodell haben Sie sich entschieden? ER-Diagramm, etwaige Besonder-heiten erklären.
    - Postgres in einem Docker Container
    - Es wurde bewusst darauf geachtet möglichst keine redundanten Daten zu speichern. Im Nachhinein gesehen war das ein Fehler, im Bezug auf Preisänderungen nachdem eine Bestellung aufgegeben wurde.

2. Dokumentieren Sie auf Request-Ebene den gesamten Workflow anhand eines durchgängigen Beispiels (von der Registrierung eines Restaurants bis zur Abfrage des Bestellstatus). HTTP-Requests inkl. HTTP-Verb, URL, Parametern, Body und Headern
    1. Business-Seitig
  
   <img width="778" alt="image" src="https://github.com/swk5-2023ws/snacks-bb-g1-heseltime-g1-ilk/assets/66922223/3cd586fb-a92c-4e74-bacf-be8ca4076392">

  
   - Beispiel Übermittlung (s. Anforderung #1)
  
    ```
    {
      "id": 0,
      "name": "Postman Post Test for Readme",
      "newAddress": {
        "id": 0,
        "street": "string",
        "postalCode": "string",
        "city": "string",
        "state": "string",
        "country": "string"
      },
      "gpsLat": 0,
      "gpsLong": 0,
      "webHookUrl": "http://api.example.com/webhook",
      "titleImage": "U29tZUV4YW1wbGVCYXNlNjRTdHJpbmc=",
      "openingHours": [
        {
          "id": 0,
          "restaurantId": 0,
          "day": "1",
          "openTime": "09:00:00",
          "closeTime": "23:33:00"
        },
        {
          "id": 0,
          "restaurantId": 0,
          "day": "2",
          "openTime": "09:00:00",
          "closeTime": "18:00:00"
        }
      ]
    }
    ```

    Ergebnis:

   <img width="544" alt="image" src="https://github.com/swk5-2023ws/snacks-bb-g1-heseltime-g1-ilk/assets/66922223/38afb075-65e8-4482-b789-fc0178b7f2b8">

   In der DB wurde der API-Key hinterlegt:

   <img width="995" alt="image" src="https://github.com/swk5-2023ws/snacks-bb-g1-heseltime-g1-ilk/assets/66922223/6936f34e-1ffe-4188-9855-c25d4ccdd00e">

   ... Validierungen dabei, z.B.:

   -- doppelte Übermittlung:

   <img width="537" alt="image" src="https://github.com/swk5-2023ws/snacks-bb-g1-heseltime-g1-ilk/assets/66922223/5a508b44-fb77-4203-a052-b4cd4ef6cd49">

   -- implausible Öffnungszeiten:

   <img width="550" alt="image" src="https://github.com/swk5-2023ws/snacks-bb-g1-heseltime-g1-ilk/assets/66922223/77e298a3-1cdf-4b73-a73e-6b599430b583">

   ... Ganz ähnlich Anforderungen #3 und #4, nur das nun der API-Key erwartet wird (übermittlung and das Business als getrennt/sicher angenommen), s. Punkt 3 zum API-Key für diese Geschäftsfälle.
       
    2. User-seitig 
    
    ![Alt text](_Documents\Bilder\SwaggerUserAccess.png)

    - /Restaurant/{Radius} - GET - verlangt sowohl den Radius für die Route als auch die Longitude und Latitude als Parameter.
      - Returniert eine Liste von gefunden Restaurants.
      - Die Restaurants beinhalten diese Informationen:

      ```
      [
        {
          "id": 0,
          "name": "string",
          "address": {
            "id": 0,
            "street": "string",
            "postalCode": "string",
            "city": "string",
            "state": "string",
            "country": "string"
          },
          "deliveryCondition": {
            "id": 0,
            "distance": 0,
            "minOrderValue": 0,
            "deliveryCost": 0
          },
          "openingHour": {
            "openTime": "string",
            "closeTime": "string"
          },
          "gpsLat": 0,
          "gpsLong": 0,
          "webHookUrl": "string",
          "titleImage": "string"
        }
      ]
      ```
      
      - DeliveryCondition und openingHour sind bereits gefiltert --> jeweils ein Objekt

    - /Menu/{restaurantId} - GET - übermittelt alle Speisen des Restaurants mit der abgefragten ID.
      ```
      [
        {
          "id": 0,
          "category": "string",
          "itemName": "string",
          "description": "string",
          "price": 0
        }
      ]
      ```

    - Order/ - POST - 
      
      ```
      {
        "id": 0,
        "timestamp": "2023-12-18T10:16:33.636Z",
        "gpsLat": 0,
        "gpsLong": 0,
        "freeText": "string",
        "status": "Unkown",
        "address": {
          "id": 0,
          "street": "string",
          "postalCode": "string",
          "city": "string",
          "state": "string",
          "country": "string"
        },
        "restaurant": {
          "id": 0,
          "name": "string",
          "address": {
            "id": 0,
            "street": "string",
            "postalCode": "string",
            "city": "string",
            "state": "string",
            "country": "string"
          },
          "deliveryCondition": {
            "id": 0,
            "distance": 0,
            "minOrderValue": 0,
            "deliveryCost": 0
          },
          "openingHour": {
            "openTime": "string",
            "closeTime": "string"
          },
          "gpsLat": 0,
          "gpsLong": 0,
          "webHookUrl": "string",
          "titleImage": "string"
        },
        "items": [
          {
            "menu": {
              "id": 0,
              "category": "string",
              "itemName": "string",
              "description": "string",
              "price": 0
            },
            "quantity": 0
          }
        ]
      }
      ```
      Die Elemente des Restaurant können leer sein, es wird nur die RestaurantID benötigt.

    - Order/{orderId} - GET - Student kann die Order abfragen
      - listet die gleiche Datenstruktur auf wie oben dargestellt.
      - 



4. Wie stellen Sie sicher, dass manche Requests nur mit einem gültigen API-Key aufgerufen werden können?

Ein Endpunkt für Login stellt einen Token bereit, der ab Erstellung eine bestimmte Zeit lang (aktuell fünf Tage) gültig ist.

<img width="535" alt="image" src="https://github.com/swk5-2023ws/snacks-bb-g1-heseltime-g1-ilk/assets/66922223/dda1be93-c2d8-4c4e-9538-da7e53f5c64b">

Für die Erstellung muss der in einem Post Request übermittelte Api Key mit dem für den ebenfalls anzugebenen Restaurant in der DB hinterlegten Key übereinstimmen. Stimmen sie nicht überein, wird kein Token ausgestellt:

<img width="533" alt="image" src="https://github.com/swk5-2023ws/snacks-bb-g1-heseltime-g1-ilk/assets/66922223/30572336-83b4-4b1b-8b01-ce968be96862">

In weiterer Folge: der Token durch das Frontend enstprechen als Bearer Token bei kritischen Endpunkten mitgegeben werden. Als Beispiel: Es gibt einen Endpunkt Business/orders der sogar ausgehend von den im Token enthaltenen Informationen die relevanten Bestellungen für das Restaurant, das den Aufruf macht, zurückgibt.

<img width="535" alt="image" src="https://github.com/swk5-2023ws/snacks-bb-g1-heseltime-g1-ilk/assets/66922223/3dc72231-91b5-41e0-bc50-e3430f826e36">

Die eingesetzte Technologie ist JWT, die in SnackAdmin/Services/JwtTokenServices ausimplementiert ist und Informationen entsprechend verpackt.

4. Welche Teile Ihres Systems sind aus Ihrer Sicht am kritischsten? Welche Maßnahmen haben Sie getroffen, um eine korrekte Funktionsweise sicherzustellen?
    - serverseitig Validierungen
      - z.B. beim Absetzen der Bestellung, werden alle Kritierien für eine valide Bestllung im Server geprüft. 
        - (Existiert Restaurant, Öffnungszeit, Zustellbereich, Minimale Bestellkosten abgedeckt, usw...)
      
5. Wie stellen Sie sicher, dass ihre API bei der Berechnung des Gesamtpreises für eine Bestellung inkl. Lieferkosten in allen Fällen ein korrektes Ergebnis liefert?
    - Serverseitige Validierung beim erstellen der Bestellung

6. Welche Maßnahmen haben Sie getroffen, um sicherzustellen, dass alle Bestellungen tatsächlich beim Restaurant ankommen, auch wenn dessen API kurzfristig nicht erreichbar ist?
    - sollte die Restaurant API nicht verfügbar sein, gibt es eine entsprechende Response an den Client. Es wird bewusst kein Retry oder dergleichen durchgeführt. Grund: Anwender soll sofort erfahren dass seine Bestellung nicht funktioniert.

7. Ein Restaurant behauptet, keine Bestellungen übermittelt zu bekommen. Wie analysieren Sie das Problem?
    - die Kommunkation kann mittels ILogger geloggt werden. Zu Testzwecken lediglich in der Konsole:
    ![Alt text](_Documents\Bilder\WebhookLogger.png)

8. Wie haben Sie sichergestellt, dass die Lieferbedingungen (Mindestpreis und Versandbe-dingungen) möglichst einfach erweiterbar sind? Beispielsweise könnten die Versandkosten von der Postleitzahl abhängig werden. Beschreiben Sie die relevanten Stellen des Designs (Klassen-Diagramm).
    - Die Lieferbedingungen sind im Moment nur abhängig von der angegebenen Maximaldistanz. Eine Anpassung an die Postleitzahl ist ohne viel Aufwand möglich:
      - Erweiterung des Entität in der Datenbank um einen String __zipCode__
      - Erweiterung der Methode zum Prüfen und Ermitteln der passenden __deliverycondition__ in der Klasse __OrderManagementLogic__

9. Welche Teile Ihres Systems sind aus Ihrer Sicht am kritischsten? Welche Maßnahmen ha-ben Sie getroffen, um eine korrekte Funktionsweise sicherzustellen?
    - siehe Punkt 4.

10. Denken Sie an die Skalierbarkeit Ihres Projekts: Plötzlich verwenden tausende Restaurants und zehntausende Studierende das Produkt. Was macht Ihnen am meisten Kopfzerbrechen?
    - das Lifecyclemanagement der einzelnen Services. Im Moment (Testzwecke) sind alle Singleton. Welche Controller als Scoped instanziert werden müssen, würde dann neu zu bewerten sein.

11. Ein Restaurant behauptet, keine Bestellungen u bermittelt zu bekommen. Wie analysieren Sie das Problem?
    - siehe Punkt 7.

12. Wenn Sie das Projekt neu anfangen würden – was würden Sie anders machen?
    - den Erhalt von bestimmten redundanten Daten im Datenbankschema. Wie bereits in Punkt 1 angedeutet, wird es passieren, dass nach Preisänderungen, die Gesamtsumme einer bereits getätigten Order nicht mehr korrekt ermittelt werden kann. Somit müssten die Informationen über das Menu selbst + Lieferkosten in der __Order__ Entität gespeichert werden
    - bessere codes nach oben aus der DB Schicht: zB wurde sehr spät festgestellt, dass ein sinnvoller foreign key constraint nicht kommuniziert werden kann (ohne viel späten Aufwand) und daher in einen Internal Server Error mündet, obwohl dieser besser ausgegeben werden könnte. Hier der Constraint-Verletz:
<img width="769" alt="image" src="https://github.com/swk5-2023ws/snacks-bb-g1-heseltime-g1-ilk/assets/66922223/9fa6f30b-b060-4286-8034-b157717929f7">
Hier ohne Constraint-Verletzung und wie erwartet, mit ersichtlichen Status Codes auf HTTP Ebene.
<img width="566" alt="image" src="https://github.com/swk5-2023ws/snacks-bb-g1-heseltime-g1-ilk/assets/66922223/a0b86096-3511-4ef9-b0e8-7d491c83156d">




## Automatisierte Tests
#### dotnet Test
Folgende Testszenarien wurden abgedeckt:
1. EndToEnd Data Access Layer
2. UnitTests für ManagementLogic
3. UnitTests für Controller
<img width="1217" alt="image" src="https://github.com/swk5-2023ws/snacks-bb-g1-heseltime-g1-ilk/assets/66922223/228f37d4-fcce-4fd0-9afd-032adbbc3068">



## Manuelle Tests
#### Postman
S. Dokumentation/Evaluierung zur Testung mit Postman.

#### Test Webhook Anbindung
Der Test zeigt einen Post mittels Postman sowie die Antwort des Test-webhooks.
![Alt text](_Documents\Bilder\TestWebHook.png)
