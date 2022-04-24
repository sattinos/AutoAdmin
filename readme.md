<b>Auto Admin</b> is a tiny accelerator library built on top of Asp.net core. It offers reusable classes that boost productivity.

<b>Requirements</b>:<br>
[.net core 5.0](https://dotnet.microsoft.com/download) <br>
[MySql 8](https://dev.mysql.com/downloads/mysql/8.html) <br>

Although currently the library only supports MySql but supporting others is on the roadmap.

<b>Solution Structure and Organization</b>:<br>
The solution is organized into 3 main projects:<br>
   1. <b>AutoAdminLib</b>: This will contain the <b>Auto Admin</b> library.
   2. <b>AutoAdminLib.TestWebApp</b>: This is a web app that consumes the library and shows how it can be utilized inside Asp.net web app.
   3. <b>AutoAdminLib.IntegrationTest</b>: This is an xUnit project that test the library and shows most of the possible use cases.

Make sure MySql8 is up and running before starting the project. 

The main features are:<br>
1. Generic CRUD repository<br>
2. Generic CRUD controller<br>
3. Implicit dependency injection.

<b>Generic CRUD repository features:</b><br>
1. The support of projection:<br>
You can select group of columns that are needed instead of returning the whole entity. This will minimize DB traffic and memory consumption.<br>
2. Powerful where condition:<br>
            Whether it is simple condition or complex one, the syntax will allow lot of control.<br>

Basic syntax:<br />
1. Defining the data class:<br />
Most of data entities have key as either number or GUID. The BaseEntity will allow you to define the type of the desired key.<br /><br />
- Example1:<br />Let's assume your data class is User and it has uint key. So the syntax will be:<br />


      User: BaseEntity<uint>

- Example2:<br />
Let's assume your data class is Photo and it has GUID key. So the syntax will be:<br />

      Photo: BaseEntity<Guid>

Once the data class is defined as above, you are ready to define your repository classes.<br /><br /><br />

2. Defining the repository:<br />The repository class <b>CrudRepository</b> accepts 2 generic parameters:<br />
   - <b>TKeyType</b>: The type of the key found in the data class itself.<br />
   - <b>T</b>: The type of data class.<br /><br />

- Example1: To define UserReposiroty for the User class above the syntax will be:<br />


      UserRepository : CrudRepository<uint, User>

- Example2: To define PhotoReposiroty for the Photo class above the syntax will be:


    PhotoRepository : CrudRepository<Guid, Photo>

3. After doing steps 1 and 2, the following functionality is ready in repository class:<br />
      - <b>InsertOneAsync</b>
      - <b>InsertManyAsync</b>
      - <b>GetOneAsync</b>
      - <b>GetOneCompactAsync</b>
      - <b>GetByIdAsync</b>
      - <b>GetManyAsync</b>
      - <b>GetManyCompactAsync</b>
      - <b>UpdateOneAsync</b>
      - <b>UpdateAsync</b>
      - <b>DeleteAsync</b>
      - <b>CountAsync</b>

      Note: all the IO operations with DB is asyncronous in AutoAdmin.
