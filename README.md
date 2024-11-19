# EDI315 API

This project provides an API for managing user authentication, watchlist items, and container details. It is built using ASP.NET Core and integrates with Cosmos DB for data storage.

## API Endpoints

### AuthController
Handles user registration and authentication.

- **POST /api/auth/register**
  - **Function**: Registers a new user.
  - **Request Body**: 
    ```json
    {
      "email": "user@example.com",
      "password": "yourpassword"
    }
    ```
  - **Response**: 
    ```json
    {
      "Message": "User registered successfully!"
    }
    ```

- **POST /api/auth/authenticate**
  - **Function**: Authenticates a user and returns a JWT token along with Cosmos DB data if successful.
  - **Request Body**: 
    ```json
    {
      "email": "user@example.com",
      "password": "yourpassword"
    }
    ```
  - **Response**:
    ```json
    {
      "Token": "your_jwt_token",
      "CosmosData": [
        {
          "id": "some_id",
          "name": "some_name",
          "data": "some_data"
        }
      ]
    }
    ```
  - **Error Response**:
    ```json
    {
      "Message": "Invalid email or password!"
    }
    ```

### WatchlistController
Manages a user's watchlist and container details.

- **POST /api/watchlist/{userId}/add**
  - **Function**: Adds a container to a user's watchlist.
  - **Path Parameter**: `userId` (string) - The ID of the user.
  - **Request Body**: 
    ```json
    "containerNumber"
    ```
  - **Response**: 
    ```json
    {
      "Message": "Container added to watchlist."
    }
    ```

- **GET /api/watchlist/{userId}**
  - **Function**: Retrieves all items in a user's watchlist.
  - **Path Parameter**: `userId` (string) - The ID of the user.
  - **Response**: 
    ```json
    [
      {
        "UserId": "userId",
        "ContainerNumber": "container1"
      },
      {
        "UserId": "userId",
        "ContainerNumber": "container2"
      }
    ]
    ```

- **GET /api/watchlist/details/{containerNumber}**
  - **Function**: Retrieves detailed information about a specific container.
  - **Path Parameter**: `containerNumber` (string) - The ID of the container.
  - **Response**: 
    ```json
    {
      "ContainerNumber": "container1",
      "Details": {
        "Size": "Large",
        "Weight": "500kg",
        "Status": "In Transit"
      }
    }
    ```
  - **Error Response**:
    ```json
    {
      "Message": "Container details not found."
    }
    ```

