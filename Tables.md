# Tables

This document provides detailed information about each table in the Railway Reservation System database, including columns, data types, primary keys, and foreign keys.

## Station Table
The `Station` table stores information about railway stations.

| Column        | Data Type         | Constraints |
|---------------|-------------------|-------------|
| station_id    | INT               | PRIMARY KEY |
| station_name  | VARCHAR(255)      | NOT NULL    |
| station_type  | VARCHAR(255)      | NOT NULL    |

## Train Table
The `Train` table stores information about trains.

| Column             | Data Type    | Constraints                    |
|--------------------|--------------|--------------------------------|
| train_no           | INT          | PRIMARY KEY                    |
| train_name         | VARCHAR(255) | NOT NULL                       |
| start_time         | TIMESTAMP    | NOT NULL                       |
| end_time           | TIMESTAMP    | NOT NULL                       |
| start_station_code | INT          | NOT NULL, FOREIGN KEY          |
| end_station_code   | INT          | NOT NULL, FOREIGN KEY          |

- **Foreign Keys**:
  - `start_station_code` references `Station(station_id)`
  - `end_station_code` references `Station(station_id)`

## Route Table
The `Route` table stores information about train routes.

| Column              | Data Type    | Constraints                            |
|---------------------|--------------|----------------------------------------|
| route_id            | INT          | PRIMARY KEY (composite)                |
| sequence_no         | INT          | PRIMARY KEY (composite)                |
| train_no            | INT          | FOREIGN KEY                            |
| station_id          | INT          | FOREIGN KEY                            |
| arrival_time        | TIMESTAMP    |                                        |
| departure_time      | TIMESTAMP    |                                        |
| distance_from_origin| INT          |                                        |

- **Primary Key**: `(route_id, sequence_no)`
- **Foreign Keys**:
  - `train_no` references `Train(train_no)`
  - `station_id` references `Station(station_id)`

## Class Table
The `Class` table stores information about the classes available on each train.

| Column        | Data Type    | Constraints                    |
|---------------|--------------|--------------------------------|
| class_id      | INT          | PRIMARY KEY (composite)        |
| train_no      | INT          | PRIMARY KEY (composite), FOREIGN KEY |
| seats_per_coach| INT         | NOT NULL                       |

- **Primary Key**: `(class_id, train_no)`
- **Foreign Key**: `train_no` references `Train(train_no)`

## Train Fare Table
The `Train_fare` table stores fare details for different classes on each train.

| Column          | Data Type     | Constraints                        |
|-----------------|---------------|------------------------------------|
| train_no        | INT           | PRIMARY KEY (composite), FOREIGN KEY |
| class_id        | INT           | PRIMARY KEY (composite), FOREIGN KEY |
| fixed_charge    | DECIMAL(10, 2)| NOT NULL                           |
| distance_charge | DECIMAL(10, 2)| NOT NULL                           |

- **Primary Key**: `(train_no, class_id)`
- **Foreign Keys**:
  - `train_no` references `Train(train_no)`
  - `class_id` references `Class(class_id)`

## Coach Table
The `Coach` table stores information about the coaches on each train.

| Column    | Data Type | Constraints                    |
|-----------|-----------|--------------------------------|
| coach_no  | INT       | PRIMARY KEY (composite)        |
| train_no  | INT       | PRIMARY KEY (composite), FOREIGN KEY |
| class_id  | INT       | FOREIGN KEY                    |

- **Primary Key**: `(coach_no, train_no)`
- **Foreign Keys**:
  - `train_no` references `Train(train_no)`
  - `class_id` references `Class(class_id)`

## Login Details Table
The `Login_details` table stores user login information.

| Column        | Data Type     | Constraints |
|---------------|---------------|-------------|
| username      | VARCHAR(255)  | PRIMARY KEY |
| name          | VARCHAR(255)  | NOT NULL    |
| mobile_number | VARCHAR(20)   | NOT NULL    |

## Ticket Table
The `Ticket` table stores information about tickets booked by passengers.

| Column             | Data Type     | Constraints                            |
|--------------------|---------------|----------------------------------------|
| pnr                | INT           | PRIMARY KEY                            |
| date_of_travel     | DATE          | NOT NULL                               |
| boarding_station   | INT           | NOT NULL, FOREIGN KEY                  |
| destination_station| INT           | NOT NULL, FOREIGN KEY                  |
| train_no           | INT           | NOT NULL, FOREIGN KEY                  |
| fare               | DECIMAL(10, 2)| NOT NULL                               |
| username           | VARCHAR(255)  | NOT NULL, FOREIGN KEY                  |
| class_id           | INT           | NOT NULL, FOREIGN KEY                  |

- **Foreign Keys**:
  - `boarding_station` references `Station(station_id)`
  - `destination_station` references `Station(station_id)`
  - `train_no` references `Train(train_no)`
  - `username` references `Login_details(username)`
  - `class_id` references `Class(class_id)`

## Payment Table
The `Payment` table stores information about payments made for tickets.

| Column         | Data Type     | Constraints                            |
|----------------|---------------|----------------------------------------|
| payment_id     | INT           | PRIMARY KEY                            |
| pnr            | INT           | NOT NULL, FOREIGN KEY                  |
| payment_date   | TIMESTAMP     | NOT NULL                               |
| payment_amount | DECIMAL(10, 2)| NOT NULL                               |
| payment_mode   | VARCHAR(50)   | NOT NULL                               |

- **Foreign Key**: `pnr` references `Ticket(pnr)`

## Passenger Table
The `Passenger` table stores information about passengers associated with a PNR.

| Column    | Data Type     | Constraints                            |
|-----------|---------------|----------------------------------------|
| pnr       | INT           | PRIMARY KEY (composite), FOREIGN KEY   |
| name      | VARCHAR(255)  | PRIMARY KEY (composite)                |
| age       | INT           |                                        |
| gender    | VARCHAR(10)   |                                        |
| mobile_no | VARCHAR(20)   |                                        |
| status    | VARCHAR(50)   |                                        |

- **Primary Key**: `(pnr, name)`
- **Foreign Key**: `pnr` references `Ticket(pnr)`

## Waiting List Table
The `Inwaiting` table stores information about passengers on the waiting list.

| Column       | Data Type    | Constraints                          |
|--------------|--------------|--------------------------------------|
| pnr          | INT          | PRIMARY KEY (composite), FOREIGN KEY |
| name         | VARCHAR(255) | PRIMARY KEY (composite), FOREIGN KEY |
| waiting_no   | INT          |                                      |

- **Primary Key**: `(pnr, waiting_no)`
- **Foreign Keys**:
  - `pnr` references `Passenger(pnr)`
  - `name` references `Passenger(name)`

## Booked Seat Table
The `Booked_seat` table stores information about booked seats on trains.

| Column   | Data Type    | Constraints                            |
|----------|--------------|----------------------------------------|
| train_no | INT          | PRIMARY KEY (composite), FOREIGN KEY   |
| seat_no  | INT          | PRIMARY KEY (composite)                |
| name     | VARCHAR(255) | FOREIGN KEY                            |
| pnr      | INT          | FOREIGN KEY                            |
| coach_no | INT          | PRIMARY KEY (composite), FOREIGN KEY   |

- **Primary Key**: `(train_no, seat_no, coach_no)`
- **Foreign Keys**:
  - `train_no` references `Train(train_no)`
  - `coach_no` references `Coach(coach_no)`
  - `pnr` references `Ticket(pnr)`
  - `name` references `Passenger(name)`
