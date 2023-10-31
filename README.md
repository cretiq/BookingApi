# Booking API

This API allows you to manage bookings for various purposes. You can view existing bookings, create new bookings, and delete bookings as needed.

## Endpoints

- **Get All Bookings**
  - **URL:** `/api/booking/all`
  - **Method:** `GET`
  - **Description:** Retrieve a list of all bookings.
  - **Authentication:** Requires authentication.
  - **Response:** A list of all booking data.

- **Get My Bookings**
  - **URL:** `/api/booking`
  - **Method:** `GET`
  - **Description:** Retrieve a list of your bookings.
  - **Authentication:** Requires authentication.
  - **Response:** A list of your booking data.

- **Create Booking**
  - **URL:** `/api/booking`
  - **Method:** `POST`
  - **Description:** Create a new booking.
  - **Authentication:** Requires authentication.
  - **Request Body:** A booking date and time.
  - **Response:** Returns the newly created booking data or a BadRequest response if the booking couldn't be created.

- **Delete Booking**
  - **URL:** `/api/booking`
  - **Method:** `DELETE`
  - **Description:** Delete a booking by its ID.
  - **Authentication:** Requires authentication.
  - **Request Parameters:** `bookingId` - The ID of the booking to be deleted.
  - **Response:** Returns an OK response if the booking was successfully deleted, or a BadRequest response if deletion fails.

## Authentication

This API requires authentication to protect booking data. You must provide valid authentication credentials to access the endpoints. Configure your preferred authentication method (e.g., JWT, OAuth2) as needed.

## Usage

To use this API, make HTTP requests to the specified endpoints using tools like cURL, Postman, or HTTP libraries in your preferred programming language. Ensure you include the required authentication headers and use the correct request format (e.g., JSON) based on the endpoint's documentation.

Example cURL request to create a booking:

```bash
curl -X POST "https://your-api-url/api/booking" -H "Content-Type: application/json" -H "Authorization: Bearer your-access-token" -d "2023-10-31T14:20:20"
