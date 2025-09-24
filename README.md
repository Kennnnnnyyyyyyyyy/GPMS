
# 🚪 Gate Pass Management System

## 📦 Overview

The **Gate Pass Management System** is a web-based application developed to efficiently manage the entries and exits of visitors and employees within an organization. This system automates the gate pass issuance process, tracks visitor and employee movements, and generates comprehensive reports, making it an essential tool for organizational security and visitor management.

## ✨ Features

- **Visitor Entry Management**: Record and manage the details of visitors, including their name, company, purpose of visit, entry and exit times, and vehicle information.
- **Local OD (Official Duty) Management**: Allows employees to apply for out-duty (OD) passes, specifying the visit location, purpose, and time of departure and return.
- **Data Export**: Export visitor and OD records to Excel for further analysis and reporting.
- **Real-time Monitoring**: Track the current status of visitors and employees within the organization.
- **User-friendly Interface**: Intuitive design with easy navigation, ensuring a seamless user experience.

## 🛠️ Technologies Used

- **Backend**: ASP.NET MVC Framework, Entity Framework Core
- **Frontend**: HTML, CSS, JavaScript, Bootstrap
- **Database**: Microsoft SQL Server
- **Other Libraries**: EPPlus for Excel export functionality

## 🚀 Installation

### Prerequisites

- .NET Core SDK 5.0 or higher
- Microsoft SQL Server 2017 or higher
- Visual Studio 2019 or higher (or Visual Studio Code)
- Basic knowledge of C#, ASP.NET Core, HTML, CSS, JavaScript, and SQL

### Steps

1. **Clone the Repository**
   ```bash
   git clone https://github.com/rohan-takmoge/Gate-Pass-management-System.git
   cd Gate-Pass-management-System
   ```

2. **Set Up the Database**
   - Create a new database in SQL Server.
   - Update the connection string in `appsettings.json` with your SQL Server details.

3. **Build and Run the Application**
   - Open the solution in Visual Studio.
   - Restore the NuGet packages.
   - Build the solution.
   - Run the application.

## 🔐 Login Credentials

### Default System Users
The system comes with pre-configured accounts for immediate testing:

| Role | Username | Password | Access Level |
|------|----------|----------|--------------|
| **Administrator** | `admin` | `Admin@123` | Full system access |
| **Employee** | `employee` | `Employee@123` | Scheduler & basic operations |

### Login URLs
- **Standard Login**: `/Account/Login` (all users)
- **Admin Login**: `/Account/AdminLogin` (admin only)

⚠️ **Security Note**: Change default passwords before production use!

📖 **Detailed Guide**: See [LOGIN.md](LOGIN.md) for complete authentication documentation.

## 💻 Usage

1. **System Login**
   - Access the application at the root URL
   - Use credentials from the table above
   - Complete 2FA setup if prompted (required for admin)

2. **Visitor Management**
   - **Registration**: Public form at `/Intake` (QR code accessible)
   - **Approval**: Admin dashboard at `/VisitorApproval`
   - **Gate Scanning**: Security interface at `/GateScan`

3. **Visitor Entry** (Legacy)
   - Navigate to the Visitor Entry section.
   - Fill in the visitor's details, including name, mobile number, company, and purpose of visit.
   - Record entry and exit times, as well as vehicle details.

4. **Local OD Management**
   - Access the Local OD section.
   - Employees can apply for out-duty by providing details such as the visit location, purpose, and time.
   - Track the time of departure and return.

5. **Exporting Data**
   - Export visitor and Local OD data to Excel for reporting purposes using the Export to Excel functionality.

## 📸 Screenshots
![Screenshot (39)](https://github.com/user-attachments/assets/bc93cabf-88f5-4a8a-852a-b9eacaa20d94)

### Visitor Entry

![Screenshot (43)](https://github.com/user-attachments/assets/df93a96a-36ed-4f9d-a6f6-fe44644191c0)
![Screenshot (44)](https://github.com/user-attachments/assets/dcb02b8b-1a67-40aa-9281-3560628cda90)
![Screenshot (45)](https://github.com/user-attachments/assets/0926dce8-9893-426d-9453-8017bc05271d)


### Local OD Management

![Screenshot (40)](https://github.com/user-attachments/assets/a68fde0d-4816-4e28-9c76-f55da27c08b4)
![Screenshot (41)](https://github.com/user-attachments/assets/7688c5dc-5a76-4e07-87d3-a887707bc8d9)
![Screenshot (42)](https://github.com/user-attachments/assets/2942917a-de9c-403a-a8ab-df761de7a30e)

## 🤝 Contributing

Contributions are welcome! Please fork the repository and submit a pull request. For major changes, please open an issue first to discuss what you would like to change.

## 🙌 Acknowledgments

- **Guide**: Mr. Kiran Chougule, Asst. General Manager-IT, Ashoka Buildcon Limited
- **Project by**: Rohan Rahul Takmoge
- 
## 📬 Contact  
Feel free to reach out via [LinkedIn](https://www.linkedin.com/in/rohan-takmoge-141b52251/) or [Email](mailto:rohantakmoge19@gmail.com).

# 🙏🏻 Thanks For Visiting !!!
