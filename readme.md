<b>Auto Admin</b> is a tiny accelerator library built on top of Asp.net core.

Requirements:<br>
[.net core 5.0](https://dotnet.microsoft.com/download) <br>
[MySql 8](https://dev.mysql.com/downloads/mysql/8.html) <br>

The library consists of 2 layers:
    1. Infrastructure
    2. 

How to use<br>
To be written
You can check test cases meanwhile

How to test<br>
- run this command from terminal:<br>
  - sudo docker compose up<br>
  - dotnet test<br>

Libraries used:<br>
    As a start, this project uses <b>Dapper</b>.


The general steps to use the BaseRepository class:
    1. Define the data class
    2. Define 

1. How to define data classes ?
Any data class must inherit from BaseEntity<T> where T is the type of the primary key.

    example1:
          defining a simple user class with unsingned integer key
2. 
   public class User : BaseEntity\<uint > {<br>
        .<br>
        .<br>
        .<br>
   }

    This means the primary key of the User is of type: uint

    example2:
        defining a simple photo class with Guid key

    public class Photo : BaseEntity\<Guid> {<br>
   .<br>
   .<br>
   .<br>
    }
            

2. BaseRepository:<br>
    This is a generic base repository class that expose CRUD functionality with deep level of control and reusability.
Example:

    In order to explain the usage, we will start with an example:

    Let's suppose we have a data class called "User".

    It is a simple POCO class, but it should inherit from BaseEntity. 




Crud Controller
    getOne
    getMany
    count
    insertOne
    insertMany
    updateOne
    updateMany
    deleteOne
    deleteMany


Crud Repository


Minimize traffic using projection

