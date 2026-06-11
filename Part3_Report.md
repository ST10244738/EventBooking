# CLDV7111 — Part 3 Reflective Technical Report
## EventEase Venue Booking System

---

## A. System Feature List

The following is a comprehensive list of all features implemented in the EventEase Venue Booking System across all three development parts.

---

### 1. Venue Management (CRUD)
**How it works:** Booking specialists can create, view, update, and delete venue records. Each venue stores its name, location, seating capacity, an optional image URL, and an availability flag. The availability toggle allows admins to temporarily mark a venue as unavailable without removing it from the system.

### 2. Event Management (CRUD)
**How it works:** Events are created with a name, date, optional description, a linked venue, and an optional event type classification. The system supports full editing and safe deletion. An event cannot be deleted if it has active bookings attached to it — an error message is shown to the user instead.

### 3. Booking Management (CRUD)
**How it works:** Bookings link an event to a venue on a specific date. The system validates each new booking to prevent double-booking (the same venue cannot be booked on the same date for two different events). Bookings can be deleted freely, but the underlying event and venue are protected from deletion while they have bookings.

### 4. Double-Booking Prevention
**How it works:** Before any booking is saved, the system queries the database to check whether the chosen venue already has a booking on the selected date. If a conflict is found, a validation error is displayed and the booking is rejected.

### 5. Deletion Protection for Venues and Events
**How it works:** In both the VenuesController and EventsController, the delete confirmation action first checks whether any booking references that venue or event. If it does, deletion is blocked and an alert is displayed to the booking specialist.

### 6. Search Functionality (Bookings)
**How it works:** The Bookings index page accepts a search term. If the term is a number, it filters by Booking ID. If it is text, it filters by event name using a partial match (`Contains`).

### 7. Advanced Filtering — Event Type
**How it works:** Events can be assigned to a predefined category from the `EventType` lookup table (Conference, Wedding, Concert, Birthday, Corporate, Exhibition, Other). The Bookings index provides a dropdown to filter bookings to show only those of a specific event type.

### 8. Advanced Filtering — Date Range
**How it works:** The Bookings index accepts an optional start date and end date. When applied, only bookings whose linked event date falls within that range are displayed.

### 9. Advanced Filtering — Venue Availability
**How it works:** A checkbox on the Bookings index allows the specialist to show only bookings at venues currently marked as available (`IsAvailable = true`). This helps quickly identify which active bookings are at venues that can still accept new events.

### 10. Consolidated Booking View
**How it works:** Rather than showing only raw booking IDs, the Bookings index table joins and displays data from the Event and Venue tables in a single rich view — including event name, event date, event type badge, description, venue name, location, capacity, availability status, and venue image.

### 11. Image Support via URL
**How it works:** Venues support an optional image URL. The Bookings index displays a thumbnail of the venue image inline in the table. Images are stored as URLs referencing external or Azure Blob-hosted images.

### 12. Responsive UI with Bootstrap 5
**How it works:** The application uses Bootstrap 5 and Bootstrap Icons for a modern, responsive layout. Navigation, tables, cards, form cards, and alert banners are all styled consistently to provide a professional booking specialist interface.

### 13. Dashboard with Statistics
**How it works:** The home page displays live counts of total venues, total events, and total bookings as colour-coded stat cards, along with quick-action buttons to create new records.

---

## B. Component Discussion

### Azure Services Used

#### Part 1 — Azure App Service (Web App Service)
**Service:** Azure App Service (PaaS)
**Why used:** Azure App Service was chosen to host the ASP.NET Core MVC application because it provides a fully managed platform that handles the underlying infrastructure (OS patching, load balancing, scaling) without requiring server management. This meant the application could be deployed rapidly from Visual Studio with a single publish step.

**Alternative considered:** Azure Virtual Machines (IaaS) could host the application but would require managing the OS, IIS configuration, security patches, and scaling manually — far more operational overhead for a team that wants to focus on application features.

#### Part 1 — Azure SQL Database
**Service:** Azure SQL Database (PaaS)
**Why used:** Azure SQL Database provides a fully managed relational database that integrates natively with Entity Framework Core. It supports automatic backups, geo-redundancy, and scaling without DBA involvement. The connection string was updated from the local SQL Express instance to the Azure SQL endpoint after migration.

**Alternative considered:** Azure Cosmos DB could be used as a NoSQL alternative, but the EventEase data model is inherently relational (venues → events → bookings with foreign key constraints), making Azure SQL a far more natural fit. Cosmos DB would have required denormalised schemas and more complex query handling.

#### Part 2 — Azure Blob Storage
**Service:** Azure Blob Storage
**Why used:** In Part 1, venue images were stored as plain URL strings. This is not scalable or secure for a production system. Azure Blob Storage was integrated to provide a centralised, durable, and cost-effective store for binary image files. The application uploads image files to a Blob container and stores the resulting public URL in the database.

**Alternative considered:** Azure Files could also store images, but it is better suited to file-share workloads rather than web-accessible content. Blob Storage is the standard solution for serving unstructured binary data over HTTP.

#### Part 3 — No new Azure service required
The Part 3 enhancements (EventType lookup table, IsAvailable field, advanced filters) are handled through the existing Azure SQL Database and Azure App Service deployment. The database schema was extended via an EF Core migration applied to the Azure SQL instance.

---

### Technologies Used to Build the Project

#### ASP.NET Core 9 MVC
**Why used:** ASP.NET Core MVC is a mature, high-performance web framework with built-in support for the Model-View-Controller pattern, routing, model binding, validation, and anti-forgery tokens. It was the required technology for this module and is well-suited to data-driven admin portals like EventEase.

#### Entity Framework Core 9
**Why used:** EF Core is Microsoft's ORM for .NET. It allowed the database schema to be defined in C# model classes, with migrations auto-generating the SQL DDL. This eliminated the need to write raw SQL for standard CRUD operations and made the codebase far easier to maintain as the schema evolved across three parts.

#### Bootstrap 5 and Bootstrap Icons
**Why used:** Bootstrap 5 provides a responsive CSS grid and component library (cards, tables, alerts, badges, form controls) that allowed a professional UI to be built rapidly without custom CSS. Bootstrap Icons complemented this with vector icons for navigation and actions.

#### C# with Nullable Reference Types
**Why used:** Nullable reference types in C# 8+ allowed the codebase to express intent clearly — for example, `string? ImageUrl` versus `string VenueName` immediately communicates which fields are optional. This reduced null reference exceptions and improved code quality.

---

## C. Reflection on the Project

### How the Project Went

Building the EventEase Venue Booking System across three parts was an effective way to learn cloud development progressively. Part 1 established the architectural foundations — the MVC application, the relational data model, and the Azure deployment pipeline. These early decisions cascaded through every subsequent part, so getting the entity relationships right from the start proved important.

Part 2 introduced the most real-world complexity: Azure Blob Storage integration, double-booking validation, and deletion protection. The validation logic required careful thought about the order of checks — the double-booking check needed to run before `ModelState.IsValid` was used to gate the save, since the conflict check depended on data outside the model itself.

Part 3's filtering requirements revealed how easily a controller action can become overloaded with query logic. Keeping the filters composable — building the EF Core query incrementally with nullable filter parameters — kept the `Index` action readable while supporting all three filter axes (event type, date range, venue availability) independently or in combination.

### Challenges Faced

**Connection string management** was an early challenge. The local development connection string (SQL Express) and the Azure SQL connection string needed to be kept separate to avoid accidentally exposing credentials in source control. This was resolved by using `appsettings.Development.json` for local overrides.

**Cascade delete behaviour** in Entity Framework required explicit configuration. When a booking is deleted, the event should not be deleted with it — but EF's default cascade rules would have caused conflicts across the multiple foreign key paths. The `OnDelete(DeleteBehavior.NoAction)` configuration on the Venue→Booking relationship was necessary to avoid SQL Server referential integrity errors.

**Seeding lookup data** for the `EventType` table required understanding that EF Core's `HasData` seeding uses fixed primary key values and applies through migrations, not at runtime. This meant the seed data is versioned with the schema, which is the correct pattern for reference data.

### Lessons Learned

The most significant lesson was that cloud PaaS services dramatically reduce operational complexity compared to on-premises or IaaS deployments. Deploying to Azure App Service and Azure SQL Database meant that concerns like server provisioning, OS updates, SSL certificate management, and database backups were handled by the platform. The development team could focus entirely on application features.

A second lesson was the value of incremental migration-based schema evolution. Rather than dropping and recreating the database as requirements changed, EF Core migrations applied targeted, auditable changes. This mirrors how real production databases are managed and prevented data loss between parts.

Finally, designing the data model with extensibility in mind from Part 1 paid dividends in Part 3. Adding the `EventType` lookup table only required creating a new model, a new `DbSet`, a migration, and a FK column on `Events` — the rest of the MVC plumbing followed naturally. A poorly normalised initial schema would have made these changes significantly more disruptive.

### Current Understanding of Cloud-Based Application Architecture

After completing this project, the key insight is that cloud-based applications are not simply traditional applications moved to a remote server. The cloud fundamentally changes the design decisions available to a developer:

- **Elasticity:** Azure App Service can scale horizontally under load. This means stateless design (not storing session data in server memory) becomes important from day one.
- **Managed services over self-managed:** Every hour spent managing infrastructure is an hour not spent on features. PaaS services like Azure SQL and Azure Blob Storage are not just convenient — they are architecturally preferable for most application workloads.
- **Security at the boundary:** Azure handles network-level security, DDoS protection, and TLS termination. The application developer's responsibility is securing the application layer — input validation, anti-forgery tokens, parameterised queries (via ORM), and appropriate access control.
- **Observability:** Azure Application Insights and Azure Monitor provide logging, tracing, and alerting that would require significant custom infrastructure on-premises. These should be integrated from the start, not added as an afterthought.

This project has built a solid foundation for more complex cloud-native architectures, including microservices, event-driven systems using Azure Event Grid, and serverless workflows using Azure Functions and Logic Apps.

---

*Report compiled as part of CLDV7111 Portfolio of Evidence — Part 3 submission.*
*Student: AronM@interfile.co.za*
*Date: June 2026*
