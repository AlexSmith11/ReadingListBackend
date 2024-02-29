## Reading List Backend

This project is just meant as a way to keep up to date with my C# skills. I have created a basic book database containing info on a book and its author/genre, with the ability to add these books to lists created by users.


## Structure

This should hopefully conform to basic .NET EF API architecture.
- Controllers that contain the endpoints.
- A DB context class that is responsible for the DB relationships.
- Entities for each table, e.g. a Book, Author, etc.
- Request and Response objects in order to further separate the DB structure and the client side.
- Basic Error and Exception handling.


## To-Do

I still need to:
- Refactor all business logic away from the endpoint controllers into services, including an interface between the two.
- Authentication and Authorization
  - Authentication Tokens
- User system
- [RequireHttps]
- Cross-Site Request Forgery (CSRF) Protection
- Use parameterized queries to prevent SQL injection (Validation)
- 
