# Appointment Scheduler API

This is a .NET 7 application that allows patients to book appointments with doctors. Doctors can offer slots, which are periods of time that patients can ask for a visit. The doctor defines a slot duration and determines the work period. Patients can see available slots by week, make a selection, and fill in the required data to book an appointment.

## How to Run the Application

1. Ensure that you have [.NET 7](https://dotnet.microsoft.com/download/dotnet/7.0) installed on your machine.

2. Clone the repository to your local machine using Git:

```bash
git clone https://github.com/Nick9-9/AppointmentSchedulerAPI.git
```

3. Navigate to the project directory:

```bash
cd AppointmentSchedulerAPI
```

4. Run the application:

```bash
dotnet run
```

The application will start and listen on a local port (usually `http://localhost:5197`).

## How to Use the Application

1. Open a web browser and navigate to the application URL.

2. You will see a weekly view of available slots. Each slot represents a period of time that you can book for a visit.

3. To book an appointment, choose an available slot and fill in the required data.

4. Click on the 'Submit' button to finalize your booking.

