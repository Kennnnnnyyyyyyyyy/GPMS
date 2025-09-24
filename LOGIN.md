# 🔐 Login Credentials Guide

## Default System Users

The Gate Pass Management System comes with pre-configured user accounts for immediate testing and setup:

### 🛡️ Administrator Account
- **Username**: `admin`
- **Password**: `Admin@123`
- **Email**: `admin@gpms.local`
- **Roles**: Admin
- **Access**: Full system access, user management, configuration, reports

### 👥 Employee Account  
- **Username**: `employee`
- **Password**: `Employee@123`
- **Email**: `employee@gpms.local`
- **Roles**: Employee
- **Access**: Scheduler, visitor management, basic operations

## 🌐 Login URLs

### Standard Login
- **URL**: `/Account/Login`
- **For**: All users (Admin, Employee, Reception, Security, Viewer)

### Admin-Only Login
- **URL**: `/Account/AdminLogin`
- **For**: Admin users only (enhanced security)
- **Note**: Requires 2FA setup after first login

## 🔒 Security Features

### Two-Factor Authentication (2FA)
- **Required for**: Admin accounts
- **Optional for**: Other users
- **Setup**: Prompted on first login
- **Apps**: Google Authenticator, Microsoft Authenticator, Authy

### Account Lockout
- **Failed Attempts**: 5 maximum
- **Lockout Duration**: 10 minutes
- **Auto-unlock**: Yes, after timeout

## 🚀 First-Time Setup

### 1. Initial Login (Admin)
1. Navigate to `/Account/AdminLogin`
2. Enter credentials: `admin` / `Admin@123`
3. Complete 2FA setup (required)
4. Access Admin Dashboard

### 2. Initial Login (Employee)
1. Navigate to `/Account/Login`
2. Enter credentials: `employee` / `Employee@123`
3. Optional: Set up 2FA (recommended)
4. Access Scheduler/Employee Dashboard

## 🛠️ User Management

### Creating New Users
1. Login as Admin
2. Navigate to User Management
3. Create users with appropriate roles:
   - **Admin**: Full system access
   - **Reception**: Visitor approvals, gate scanning
   - **Security**: Gate scanning only
   - **Employee**: Scheduler, basic operations
   - **Viewer**: Read-only access

### Role Descriptions
- **Admin**: System administration, user management, all features
- **Reception**: Visitor intake, approvals, gate operations
- **SecurityGuard**: Gate scanning, visitor check-in/out
- **Employee**: Meeting scheduler, basic visitor operations
- **Viewer**: Read-only dashboard access

## ⚠️ Important Security Notes

### Production Deployment
1. **Change default passwords immediately**
2. **Enable 2FA for all admin accounts**
3. **Use strong, unique passwords**
4. **Regularly review user access**

### Password Requirements
- Minimum 6 characters (configurable in Startup.cs)
- Default settings are relaxed for development
- Consider strengthening for production:
  - Require uppercase letters
  - Require special characters
  - Require numbers

## 🔧 Configuration

### Password Policy (Startup.cs)
```csharp
options.Password.RequireNonAlphanumeric = false;
options.Password.RequireUppercase = false;
options.Password.RequiredLength = 6;
```

### Cookie Settings
- **Login Path**: `/Account/Login`
- **Logout Path**: `/Account/Logout`
- **Session Duration**: 7 days (sliding)
- **Access Denied**: `/Account/AccessDenied`

## 🆘 Troubleshooting

### Common Issues

#### "Invalid login attempt"
- Verify username/password spelling
- Check account hasn't been locked
- Ensure user exists in system

#### "Account locked"
- Wait 10 minutes for auto-unlock
- Or contact admin to unlock manually

#### 2FA Issues
- Ensure time sync on device
- Try recovery codes if available
- Contact admin to reset 2FA

#### Can't Access Admin Features
- Verify user has "Admin" role
- Use `/Account/AdminLogin` endpoint
- Complete 2FA setup if required

## 📱 Mobile Access

The login system is mobile-responsive and works on:
- Smartphones (iOS/Android)
- Tablets
- Desktop browsers

## 🔄 Password Reset

Currently, password reset must be done by administrators:
1. Admin logs in
2. Navigate to User Management
3. Reset user password
4. Provide new temporary password to user

## 📊 Default System Roles

| Role | Users | Permissions |
|------|-------|-------------|
| Admin | admin | Full system access, user management |
| Employee | employee | Scheduler, visitor operations |
| Reception | - | Visitor approvals, gate scanning |
| SecurityGuard | - | Gate scanning only |
| Viewer | - | Read-only dashboard |

---

**Last Updated**: December 2024  
**System Version**: 1.0  
**Security Level**: Development (Update for Production)