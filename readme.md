# Design Prompt: Build a web project that describes a car

Based on this prompt, my first interpretation was to build a web-based application that displays information about cars. <br>
My next thought then was to build a simple, straight-forward MVC Application that allows the user to view and search for cars by make and model. <br>
For the simplicity of the interview, I will simplify the tech stack that I would normally use. <br>
By convention, I would opt for a full featured framework to build the client facing application (Usually Angular), but instead I will rely on traditional MVC views using razor syntax. <br>
The primary goal of this application is to showcase my thought process and design philosophies when working on full-stack applications. <br>

For this reason, instead of functionality I have prioritized the architecture, structure, readability, and maintainability of the application.

## Overview and Setup

This project was created with the latest version of C# and .NET. <br>
Before running the project, this command should be run in the CarViewer folder: dotnet ef database update --project ../CarViewer.Data. <br>
This should guarantee that the database is created.

To seed the database with some initial data, in a web browser while the application is running, navigate to /Home/Seed. Nothing should happen in the browser but upon navigating back, some sample data should be on the home page. 

## Miniature SLDC

### 1. Initial Requirements 

1. Display cars in a table 
	- Show features of specific car (Make, model, mileage, year)
	- filter table by car body (Sedan, Truck, Crossover, Coupe, etc.) 

2. View detailed information about individual cars
	- "Detailed" view that shows in-depth information about a car
	- Body type, transmission, drivetrain, etc

3. View service history of a concrete car
	- Service date
	- Mileage at service date
	- Service description

	 
### 2. Analysis 

1. Displaying cars in a table
	- Car information should be stored and accessed via a an API, which means a data store is required.
	- Representation and structure
		* For organizational and testing purposes, I will abstract the data store into its own project. This way, the data access mechanisms can be reasoned about and tested without any dependency on the actual application. <br>
		  This means that when the data access logic is tested, the test project won't need to depend on the actual application, and it also leaves the logic open to re-use in other projects. <br>
		  For example if we wanted to extend this car viewer application to include an API for integrators, it wouldn't make sense for that logic to depend on the MVC project. 

    - Determining required information to represent a car
		* Car as a physical entity:
			1. VIN
				For simplicity's sake a VIN in this application will simply be exactly 16 ASCII characters which must start with a number and not end in a number. Every car must have a VIN.
			2. Make
				The manufacturer of the car. Every car must have a make and there can only be one model per make (No honda civic and subaru civic)
				This is not guaranteed to be unique.
			3. Model
				The specific product name of the car. This is not guaranteed to be unique. 

			4. Drivetrain
				AWD, FWD, RWD

			5. Transmission
				Manual or Automatic (Will disregard different types of manual transmission)

			6. Body configuration
				Coupe, Sedan, Hatchback, SUV, Truck

				- Number of seats
				- Number of doors

			7. Year
				Year of manufacture

			8. Mileage
			
			9. Service history
				Cars require maintenance, and this information is usually tracked and associated with the VIN in order to maintain an accurate record of the car's condition.

				Service history minimally includes 

				- Mileage to date
				- Service date
				- Description of service

	- Determining what a *Make* is
		The make of a car is really just the brand name of the manufacturer. In theory this could include a lot of information, but for now I will simply assume that it is just a brand name (Subaru, Toyota)
		It is reasonable to represent this as its own entity as well, as for any given make (or manufacturer), there will be many models.

	- Determining what a *model* is
		It is possible to simply represent the model of a car as a string associated with an existing car, but it would be better to concpetualize the model of a car as its own separate entity with its own data. <br> 
		For any given *model* of a car, there are many existing cars. Since a car's model has a lot of associated data (what shape is the car, how many windows, what kind of transmission, etc), it would not make sense to include this information for every instance of a car <br>
		Instead, models should be represented as their own entities and every concrete car should reference an existing model. This reduces the need for storing redundant information and also neatly partitions related concepts. 

	- Determining what a body configuration is
		Cars come in many shapes and sizes with distinct names and features. For example, a sedan is generally a 4 door car with a trunk, whereas a coupe is generally a 2 door car with 2-4 windows. It is also possible for the same model to have different body configurations <br>
		For example, some Mazda cars are available in both sedan and SUV configuration. 


### 3. Implementation

1. Data store representation of business objects

The goal of this project is to create a web project that describes a car. This presents the car as the central business object. From analysis it makes sense to represent a car at the data level using the following entities;

* Car <br>
  This entity will represent a concrete car with a VIN, Mileage and year of production, as well as a foreign key to a Model. There will be many cars for any given model, and there will consequently be many cars for any given manufacturer.
* Model <br>
  This entity will represent a concrete configuration that a car may be available in. It has two foreign keys, one to a manufacturer and one to a body configuration. This means for any body configuration or manufacturer, there are many models.
  The model will also contain information about the actual car's functionality, such as its transmission and drivetrain.
* Manufacturer <br>
  This entity will represent a concrete manufacturer otherwise referred to as the "Make". For any given make there will be many models and consequently many cars.
* Body Configuration <br>
  This entity represents the concrete physical features of a car. Will contain information such as door count, window count, seat count. Could be extended to also represent if a car has a convertible roof, as well as any other distinguishing features. <br>
  For any given body configuration, there will be many models and consequently many cars.
* Service History <br>
  This entity will represent a record of all service down to a concrete car. This will include mileage to date, a description of service, and a service date. It will also include the VIN as a foreign key, as for every one car there will be many service history entities.

Entity Framework will be used as an abstraction over a relational database. SQLite will be used for simplicity but could be substituted with a more featured implementation of SQL with minimal changes. <br>

The underlying CarContext will be accessed through a CarDataService, which will provide all functionality for performing operations against the data store. By doing this, the backing data store can be stubbed or mocked in order to perform integration / unit tests. 


#### DbContext scaffolding

A specialized derived class, CarContext, extends the DbContext which represents an active, managed connection to a relational database.

The CarContext shall utilize the defined models found in CarViewer.Data.Domain to translate operations against the underlying database provider,
which is an abstraction that allows for the substition of various kinds of relational database. For simplicity, SQLite will be used. 

The CarContext exposes a set of DbSet<T>, which are abstractions over defined relations or tables. <br>
When operations are performed against these DbSets, Entity Framework will generate an IQueryable object that is lazily evaluated. <br>
Upon evaluation, the IQueryable will be translated into a concrete operation in the dialect of the underlying provider.

#### Database Migration

Changes to the entity relationships and overall business logic model are tracked and managed through a system of migrations. <br>
When the model is changed, a migration is manually generated by the developer. <br>
These migrations may be applied manually via commandline or automatically through code. <br>
I generally prefer to apply migrations manually when possible as issues in migrations can be subtle and hard to diagnose at runtime.


Because of the structure of the project, some extra configuration is required ot generate migrations via the commandline. <br>
EF will do its best to generate database migration information, but this process requires a static hint based on the usage of the DbContext <br>

In order to generate the initial migration, this command was executed in the CarViewer project folder, using the CarViewer.Data project as the default.

```sh
dotnet ef migrations add Initial --project ../CarViewer.Data
```

2. UI Application and presentation

The frontend for this will be built with convential MVC and razor pages. This demo application will be readonly in nature, only presenting existing information about cars in the underlying data store. 

* There will be two controllers
	- Home <br>
		This controller will serve as the homepage of the application. This page will be a simple table containing the most relevant information about cars in the database. <br>
	- Details <br>
		This controller will serve a simple page that displays tabular, detailed information about a particular car. If the car has a service record, that shall be displayed in a tabular format as well.


3. Business logic / Domain

* VIN Validation
For the sake of demonstration, a VIN will need to be validated to be 16 characters exactly, starting with a digit and ending with a letter. If a car with an invalid VIN is requested, an error shall occur redirecting the user to the error page. <br>
If this application were to be real, it should gracefully redirect at this point and indicate to the user why the VIN is invalid. <br> 
This is a sad-path scenario as only valid VIN should ever be sent to the DetailsController through normal use, but it is important to be defensive.


4. Logging

This application includes bare minimum logging for demonstration purposes, as proper logging is integral to sustainable design. For simplicity, I will use the standard Microsoft logger, and its sink shall be the console. <br>
Again for simplicity, the minimum log level of any event shall be information, as that will all logs to be demonstrated easily in the console without requiring a lower default level that would be difficult to parse. <br>

5. Testing

Testing is a vital part of the implementation of features. Generally I implement tests before I implement functionality as I find that it dramatically improves code quality and speeds up iterations. <br>
In this case I wrote some basic tests to ensure that the underlying data access services and validation logic behave as expected. <br>
I also wrote some controller tests to ensure that the actual MVC application works.
