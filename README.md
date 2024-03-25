# TidyBee Hub

The tidybee-hub is the central component of the TidyBee system. It is responsible for managing the data and the communication between the agents and the frontend.

To run the complete TidyBee system, you need to follow the instructions in the [tidybee-scripts](https://github.com/TidyBee/tidybee-scripts) repository.

## Presentation

The HUB is composed of different microservices:

- **APIGateway**: The main entry point of the system. It is responsible for managing the communication between the frontend and the agents : handling all requests from the frontend and dispatching them to the right microservice. A part of the service is the HiveMiddleware : wich work as a proxy to forward the requests to all the agents into one answer. All the requests send to the hub must go through the APIGateway.

- **Auth**: The authentication microservice. It is responsible for managing the agents and store their data.

- **Dataprocessing**: The microservice responsible for processing the data from the agents. It is used to fill the widgets displayed in the frontend.

- **Uoth**: The microservice responsible for managing the users. It is not used in the current version of the system.


## Developpers team

- [Guillaume Terrière](https://github.com/GuyomT) Team Leader
- [Baptiste Friboulet](https://github.com/Blynqs)
- [Lucas Tesnier](https://github.com/LucasTesnier)
