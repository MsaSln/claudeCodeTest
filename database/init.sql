-- SQL Server Database Initialization Script
-- JWT Backend API with Role & Permission Management

-- Create database (run this separately if needed)
-- CREATE DATABASE JwtBackendApi;
-- GO
-- USE JwtBackendApi;
-- GO

-- ===================================
-- Tables
-- ===================================

-- Users table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(100) NOT NULL,
        Email NVARCHAR(255) NOT NULL,
        PasswordHash NVARCHAR(500) NOT NULL,
        Role NVARCHAR(50) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT UQ_Users_Username UNIQUE (Username),
        CONSTRAINT UQ_Users_Email UNIQUE (Email),
        CONSTRAINT CHK_Username_Length CHECK (LEN(Username) >= 3)
    );
END
GO

-- User Groups table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserGroups')
BEGIN
    CREATE TABLE UserGroups (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500),
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2,
        CreatedBy INT,
        UpdatedBy INT,
        CONSTRAINT UQ_UserGroups_Name UNIQUE (Name)
    );
END
GO

-- User Group Memberships table (many-to-many)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserGroupMemberships')
BEGIN
    CREATE TABLE UserGroupMemberships (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        UserGroupId INT NOT NULL,
        AssignedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        AssignedBy INT,
        CONSTRAINT FK_Memberships_User FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
        CONSTRAINT FK_Memberships_UserGroup FOREIGN KEY (UserGroupId) REFERENCES UserGroups(Id) ON DELETE CASCADE,
        CONSTRAINT UQ_User_Group_Membership UNIQUE (UserId, UserGroupId)
    );
END
GO

-- Screens table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Screens')
BEGIN
    CREATE TABLE Screens (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        DisplayName NVARCHAR(200) NOT NULL,
        Description NVARCHAR(500),
        Route NVARCHAR(500),
        Icon NVARCHAR(100),
        ParentScreenId INT,
        [Order] INT NOT NULL DEFAULT 0,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2,
        CONSTRAINT UQ_Screens_Name UNIQUE (Name),
        CONSTRAINT FK_Screens_Parent FOREIGN KEY (ParentScreenId) REFERENCES Screens(Id)
    );
END
GO

-- Screen Actions table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ScreenActions')
BEGIN
    CREATE TABLE ScreenActions (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ScreenId INT NOT NULL,
        ActionName NVARCHAR(100) NOT NULL,
        DisplayName NVARCHAR(200) NOT NULL,
        Description NVARCHAR(500),
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_ScreenActions_Screen FOREIGN KEY (ScreenId) REFERENCES Screens(Id) ON DELETE CASCADE,
        CONSTRAINT UQ_Screen_Action UNIQUE (ScreenId, ActionName)
    );
END
GO

-- Screen Permissions table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ScreenPermissions')
BEGIN
    CREATE TABLE ScreenPermissions (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserGroupId INT NOT NULL,
        ScreenId INT NOT NULL,
        ScreenActionId INT NOT NULL,
        IsGranted BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CreatedBy INT,
        CONSTRAINT FK_Permissions_UserGroup FOREIGN KEY (UserGroupId) REFERENCES UserGroups(Id) ON DELETE CASCADE,
        CONSTRAINT FK_Permissions_Screen FOREIGN KEY (ScreenId) REFERENCES Screens(Id) ON DELETE NO ACTION,
        CONSTRAINT FK_Permissions_Action FOREIGN KEY (ScreenActionId) REFERENCES ScreenActions(Id) ON DELETE NO ACTION,
        CONSTRAINT UQ_Permission UNIQUE (UserGroupId, ScreenId, ScreenActionId)
    );
END
GO

-- Menus table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Menus')
BEGIN
    CREATE TABLE Menus (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        DisplayName NVARCHAR(200) NOT NULL,
        Description NVARCHAR(500),
        Location INT NOT NULL, -- 1: Sidebar, 2: Header
        [Order] INT NOT NULL DEFAULT 0,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2,
        CreatedBy INT,
        UpdatedBy INT,
        CONSTRAINT UQ_Menus_Name UNIQUE (Name),
        CONSTRAINT CHK_Location CHECK (Location IN (1, 2))
    );
END
GO

-- Menu Items table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MenuItems')
BEGIN
    CREATE TABLE MenuItems (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        MenuId INT NOT NULL,
        ParentMenuItemId INT,
        ItemType INT NOT NULL, -- 1: Group, 2: Screen
        ScreenId INT,
        DisplayName NVARCHAR(200) NOT NULL,
        Icon NVARCHAR(100),
        Route NVARCHAR(500),
        [Order] INT NOT NULL DEFAULT 0,
        IsActive BIT NOT NULL DEFAULT 1,
        BadgeText NVARCHAR(50),
        BadgeColor NVARCHAR(50),
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2,
        CreatedBy INT,
        UpdatedBy INT,
        CONSTRAINT FK_MenuItems_Menu FOREIGN KEY (MenuId) REFERENCES Menus(Id) ON DELETE CASCADE,
        CONSTRAINT FK_MenuItems_Parent FOREIGN KEY (ParentMenuItemId) REFERENCES MenuItems(Id),
        CONSTRAINT FK_MenuItems_Screen FOREIGN KEY (ScreenId) REFERENCES Screens(Id),
        CONSTRAINT CHK_ItemType CHECK (ItemType IN (1, 2))
    );
END
GO

-- ===================================
-- Indexes
-- ===================================

-- User indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Username')
    CREATE INDEX IX_Users_Username ON Users(Username);
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email')
    CREATE INDEX IX_Users_Email ON Users(Email);
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_CreatedAt')
    CREATE INDEX IX_Users_CreatedAt ON Users(CreatedAt);
GO

-- User Group Membership indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Memberships_UserId')
    CREATE INDEX IX_Memberships_UserId ON UserGroupMemberships(UserId);
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Memberships_GroupId')
    CREATE INDEX IX_Memberships_GroupId ON UserGroupMemberships(UserGroupId);
GO

-- Screen indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Screens_Name')
    CREATE INDEX IX_Screens_Name ON Screens(Name);
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Screens_Order')
    CREATE INDEX IX_Screens_Order ON Screens([Order]);
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Screens_Parent')
    CREATE INDEX IX_Screens_Parent ON Screens(ParentScreenId);
GO

-- Screen Action indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Actions_ScreenId')
    CREATE INDEX IX_Actions_ScreenId ON ScreenActions(ScreenId);
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Actions_Name')
    CREATE INDEX IX_Actions_Name ON ScreenActions(ActionName);
GO

-- Permission indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Permissions_UserGroup')
    CREATE INDEX IX_Permissions_UserGroup ON ScreenPermissions(UserGroupId);
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Permissions_Screen')
    CREATE INDEX IX_Permissions_Screen ON ScreenPermissions(ScreenId);
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Permissions_Action')
    CREATE INDEX IX_Permissions_Action ON ScreenPermissions(ScreenActionId);
GO

-- Menu indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Menus_Location')
    CREATE INDEX IX_Menus_Location ON Menus(Location);
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Menus_Order')
    CREATE INDEX IX_Menus_Order ON Menus([Order]);
GO

-- Menu Item indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MenuItems_Menu')
    CREATE INDEX IX_MenuItems_Menu ON MenuItems(MenuId);
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MenuItems_Parent')
    CREATE INDEX IX_MenuItems_Parent ON MenuItems(ParentMenuItemId);
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MenuItems_Screen')
    CREATE INDEX IX_MenuItems_Screen ON MenuItems(ScreenId);
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MenuItems_Order')
    CREATE INDEX IX_MenuItems_Order ON MenuItems([Order]);
GO

-- ===================================
-- Seed Data
-- ===================================

-- Seed User Groups
IF NOT EXISTS (SELECT 1 FROM UserGroups WHERE Name = 'Administrators')
    INSERT INTO UserGroups (Name, Description, IsActive, CreatedAt) VALUES ('Administrators', 'Full system access', 1, GETUTCDATE());
IF NOT EXISTS (SELECT 1 FROM UserGroups WHERE Name = 'Managers')
    INSERT INTO UserGroups (Name, Description, IsActive, CreatedAt) VALUES ('Managers', 'Management level access', 1, GETUTCDATE());
IF NOT EXISTS (SELECT 1 FROM UserGroups WHERE Name = 'Users')
    INSERT INTO UserGroups (Name, Description, IsActive, CreatedAt) VALUES ('Users', 'Standard user access', 1, GETUTCDATE());
GO

-- Seed Screens
IF NOT EXISTS (SELECT 1 FROM Screens WHERE Name = 'Dashboard')
    INSERT INTO Screens (Name, DisplayName, Description, Route, Icon, [Order], IsActive, CreatedAt) VALUES ('Dashboard', N'Ana Sayfa', N'Dashboard ekranı', '/dashboard', 'dashboard', 1, 1, GETUTCDATE());
IF NOT EXISTS (SELECT 1 FROM Screens WHERE Name = 'Users')
    INSERT INTO Screens (Name, DisplayName, Description, Route, Icon, [Order], IsActive, CreatedAt) VALUES ('Users', N'Kullanıcılar', N'Kullanıcı yönetimi ekranı', '/users', 'people', 2, 1, GETUTCDATE());
IF NOT EXISTS (SELECT 1 FROM Screens WHERE Name = 'Settings')
    INSERT INTO Screens (Name, DisplayName, Description, Route, Icon, [Order], IsActive, CreatedAt) VALUES ('Settings', N'Ayarlar', N'Sistem ayarları', '/settings', 'settings', 3, 1, GETUTCDATE());
GO

-- Seed Screen Actions
DECLARE @DashboardId INT = (SELECT Id FROM Screens WHERE Name = 'Dashboard');
DECLARE @UsersId INT = (SELECT Id FROM Screens WHERE Name = 'Users');
DECLARE @SettingsId INT = (SELECT Id FROM Screens WHERE Name = 'Settings');

-- Dashboard actions
IF NOT EXISTS (SELECT 1 FROM ScreenActions WHERE ScreenId = @DashboardId AND ActionName = 'view')
    INSERT INTO ScreenActions (ScreenId, ActionName, DisplayName, IsActive, CreatedAt) VALUES (@DashboardId, 'view', N'Görüntüle', 1, GETUTCDATE());
IF NOT EXISTS (SELECT 1 FROM ScreenActions WHERE ScreenId = @DashboardId AND ActionName = 'export')
    INSERT INTO ScreenActions (ScreenId, ActionName, DisplayName, IsActive, CreatedAt) VALUES (@DashboardId, 'export', N'Dışa Aktar', 1, GETUTCDATE());

-- Users actions
IF NOT EXISTS (SELECT 1 FROM ScreenActions WHERE ScreenId = @UsersId AND ActionName = 'view')
    INSERT INTO ScreenActions (ScreenId, ActionName, DisplayName, IsActive, CreatedAt) VALUES (@UsersId, 'view', N'Görüntüle', 1, GETUTCDATE());
IF NOT EXISTS (SELECT 1 FROM ScreenActions WHERE ScreenId = @UsersId AND ActionName = 'create')
    INSERT INTO ScreenActions (ScreenId, ActionName, DisplayName, IsActive, CreatedAt) VALUES (@UsersId, 'create', N'Oluştur', 1, GETUTCDATE());
IF NOT EXISTS (SELECT 1 FROM ScreenActions WHERE ScreenId = @UsersId AND ActionName = 'edit')
    INSERT INTO ScreenActions (ScreenId, ActionName, DisplayName, IsActive, CreatedAt) VALUES (@UsersId, 'edit', N'Düzenle', 1, GETUTCDATE());
IF NOT EXISTS (SELECT 1 FROM ScreenActions WHERE ScreenId = @UsersId AND ActionName = 'delete')
    INSERT INTO ScreenActions (ScreenId, ActionName, DisplayName, IsActive, CreatedAt) VALUES (@UsersId, 'delete', 'Sil', 1, GETUTCDATE());
IF NOT EXISTS (SELECT 1 FROM ScreenActions WHERE ScreenId = @UsersId AND ActionName = 'export')
    INSERT INTO ScreenActions (ScreenId, ActionName, DisplayName, IsActive, CreatedAt) VALUES (@UsersId, 'export', N'Dışa Aktar', 1, GETUTCDATE());

-- Settings actions
IF NOT EXISTS (SELECT 1 FROM ScreenActions WHERE ScreenId = @SettingsId AND ActionName = 'view')
    INSERT INTO ScreenActions (ScreenId, ActionName, DisplayName, IsActive, CreatedAt) VALUES (@SettingsId, 'view', N'Görüntüle', 1, GETUTCDATE());
IF NOT EXISTS (SELECT 1 FROM ScreenActions WHERE ScreenId = @SettingsId AND ActionName = 'edit')
    INSERT INTO ScreenActions (ScreenId, ActionName, DisplayName, IsActive, CreatedAt) VALUES (@SettingsId, 'edit', N'Düzenle', 1, GETUTCDATE());
GO

-- Seed Screen Permissions (Administrators get all permissions)
DECLARE @AdminGroupId INT = (SELECT Id FROM UserGroups WHERE Name = 'Administrators');
DECLARE @UsersGroupId INT = (SELECT Id FROM UserGroups WHERE Name = 'Users');

INSERT INTO ScreenPermissions (UserGroupId, ScreenId, ScreenActionId, IsGranted, CreatedAt)
SELECT @AdminGroupId, sa.ScreenId, sa.Id, 1, GETUTCDATE()
FROM ScreenActions sa
WHERE NOT EXISTS (
    SELECT 1 FROM ScreenPermissions sp
    WHERE sp.UserGroupId = @AdminGroupId AND sp.ScreenId = sa.ScreenId AND sp.ScreenActionId = sa.Id
);

-- Seed Screen Permissions (Users get view permissions)
INSERT INTO ScreenPermissions (UserGroupId, ScreenId, ScreenActionId, IsGranted, CreatedAt)
SELECT @UsersGroupId, sa.ScreenId, sa.Id, 1, GETUTCDATE()
FROM ScreenActions sa
WHERE sa.ActionName = 'view'
AND NOT EXISTS (
    SELECT 1 FROM ScreenPermissions sp
    WHERE sp.UserGroupId = @UsersGroupId AND sp.ScreenId = sa.ScreenId AND sp.ScreenActionId = sa.Id
);
GO

-- Seed Menus
IF NOT EXISTS (SELECT 1 FROM Menus WHERE Name = 'MainSidebar')
    INSERT INTO Menus (Name, DisplayName, Description, Location, [Order], IsActive, CreatedAt) VALUES ('MainSidebar', N'Ana Menü', N'Sol taraf ana menü', 1, 1, 1, GETUTCDATE());
IF NOT EXISTS (SELECT 1 FROM Menus WHERE Name = 'TopHeader')
    INSERT INTO Menus (Name, DisplayName, Description, Location, [Order], IsActive, CreatedAt) VALUES ('TopHeader', N'Üst Menü', N'Header menüsü', 2, 1, 1, GETUTCDATE());
GO

-- Seed Menu Items
DECLARE @SidebarMenuId INT = (SELECT Id FROM Menus WHERE Name = 'MainSidebar');
DECLARE @HeaderMenuId INT = (SELECT Id FROM Menus WHERE Name = 'TopHeader');
DECLARE @DashboardScreenId INT = (SELECT Id FROM Screens WHERE Name = 'Dashboard');
DECLARE @UsersScreenId INT = (SELECT Id FROM Screens WHERE Name = 'Users');
DECLARE @SettingsScreenId INT = (SELECT Id FROM Screens WHERE Name = 'Settings');

-- Dashboard menu item (sidebar)
IF NOT EXISTS (SELECT 1 FROM MenuItems WHERE MenuId = @SidebarMenuId AND DisplayName = N'Ana Sayfa')
    INSERT INTO MenuItems (MenuId, ItemType, ScreenId, DisplayName, Icon, Route, [Order], IsActive, CreatedAt)
    VALUES (@SidebarMenuId, 2, @DashboardScreenId, N'Ana Sayfa', 'dashboard', '/dashboard', 1, 1, GETUTCDATE());

-- Management group
DECLARE @ManagementGroupId INT;
IF NOT EXISTS (SELECT 1 FROM MenuItems WHERE MenuId = @SidebarMenuId AND DisplayName = N'Yönetim' AND ItemType = 1)
BEGIN
    INSERT INTO MenuItems (MenuId, ItemType, DisplayName, Icon, [Order], IsActive, CreatedAt)
    VALUES (@SidebarMenuId, 1, N'Yönetim', 'settings', 2, 1, GETUTCDATE());
    SET @ManagementGroupId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    SET @ManagementGroupId = (SELECT Id FROM MenuItems WHERE MenuId = @SidebarMenuId AND DisplayName = N'Yönetim' AND ItemType = 1);
END

-- Users under Management
IF NOT EXISTS (SELECT 1 FROM MenuItems WHERE MenuId = @SidebarMenuId AND ParentMenuItemId = @ManagementGroupId AND ScreenId = @UsersScreenId)
    INSERT INTO MenuItems (MenuId, ParentMenuItemId, ItemType, ScreenId, DisplayName, Icon, Route, [Order], IsActive, CreatedAt)
    VALUES (@SidebarMenuId, @ManagementGroupId, 2, @UsersScreenId, N'Kullanıcılar', 'people', '/users', 1, 1, GETUTCDATE());

-- Settings under Management
IF NOT EXISTS (SELECT 1 FROM MenuItems WHERE MenuId = @SidebarMenuId AND ParentMenuItemId = @ManagementGroupId AND ScreenId = @SettingsScreenId)
    INSERT INTO MenuItems (MenuId, ParentMenuItemId, ItemType, ScreenId, DisplayName, Icon, Route, [Order], IsActive, CreatedAt)
    VALUES (@SidebarMenuId, @ManagementGroupId, 2, @SettingsScreenId, N'Ayarlar', 'settings', '/settings', 2, 1, GETUTCDATE());

-- Dashboard menu item (header)
IF NOT EXISTS (SELECT 1 FROM MenuItems WHERE MenuId = @HeaderMenuId AND DisplayName = 'Dashboard')
    INSERT INTO MenuItems (MenuId, ItemType, ScreenId, DisplayName, Icon, Route, [Order], IsActive, CreatedAt)
    VALUES (@HeaderMenuId, 2, @DashboardScreenId, 'Dashboard', 'dashboard', '/dashboard', 1, 1, GETUTCDATE());
GO

-- Seed default admin user (password: Admin123!)
-- Password hash is SHA256 of "Admin123!"
IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'admin')
    INSERT INTO Users (Username, Email, PasswordHash, Role, CreatedAt)
    VALUES ('admin', 'admin@example.com', 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=', 'Admin', GETUTCDATE());
GO

-- Assign admin to Administrators group
DECLARE @AdminUserId INT = (SELECT Id FROM Users WHERE Username = 'admin');
DECLARE @AdminsGroupId INT = (SELECT Id FROM UserGroups WHERE Name = 'Administrators');

IF NOT EXISTS (SELECT 1 FROM UserGroupMemberships WHERE UserId = @AdminUserId AND UserGroupId = @AdminsGroupId)
    INSERT INTO UserGroupMemberships (UserId, UserGroupId, AssignedAt) VALUES (@AdminUserId, @AdminsGroupId, GETUTCDATE());
GO

-- ===================================
-- Views (Optional - for easier queries)
-- ===================================

-- View: User permissions summary
IF EXISTS (SELECT * FROM sys.views WHERE name = 'v_UserPermissions')
    DROP VIEW v_UserPermissions;
GO

CREATE VIEW v_UserPermissions AS
SELECT
    u.Id AS UserId,
    u.Username,
    ug.Name AS UserGroupName,
    s.Name AS ScreenName,
    s.DisplayName AS ScreenDisplayName,
    sa.ActionName,
    sa.DisplayName AS ActionDisplayName,
    sp.IsGranted
FROM Users u
INNER JOIN UserGroupMemberships ugm ON u.Id = ugm.UserId
INNER JOIN UserGroups ug ON ugm.UserGroupId = ug.Id
INNER JOIN ScreenPermissions sp ON ug.Id = sp.UserGroupId
INNER JOIN Screens s ON sp.ScreenId = s.Id
INNER JOIN ScreenActions sa ON sp.ScreenActionId = sa.Id
WHERE ug.IsActive = 1 AND s.IsActive = 1 AND sa.IsActive = 1;
GO

-- View: Menu structure
IF EXISTS (SELECT * FROM sys.views WHERE name = 'v_MenuStructure')
    DROP VIEW v_MenuStructure;
GO

CREATE VIEW v_MenuStructure AS
SELECT
    m.Id AS MenuId,
    m.Name AS MenuName,
    m.DisplayName AS MenuDisplayName,
    m.Location,
    mi.Id AS MenuItemId,
    mi.ParentMenuItemId,
    mi.ItemType,
    mi.DisplayName AS ItemDisplayName,
    mi.Icon,
    mi.Route,
    mi.[Order],
    s.Name AS ScreenName,
    s.Id AS ScreenId
FROM Menus m
LEFT JOIN MenuItems mi ON m.Id = mi.MenuId
LEFT JOIN Screens s ON mi.ScreenId = s.Id
WHERE m.IsActive = 1 AND (mi.Id IS NULL OR mi.IsActive = 1);
GO

PRINT 'Database initialization completed successfully.';
GO
