-- PostgreSQL Database Initialization Script
-- JWT Backend API with Role & Permission Management

-- Create database (run this separately if needed)
-- CREATE DATABASE jwtbackendapi;

-- Connect to the database
-- \c jwtbackendapi;

-- Enable UUID extension (optional, for future use)
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- ===================================
-- Tables
-- ===================================

-- Users table
CREATE TABLE IF NOT EXISTS users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(100) NOT NULL UNIQUE,
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(500) NOT NULL,
    role VARCHAR(50) NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT chk_username_length CHECK (LENGTH(username) >= 3),
    CONSTRAINT chk_email_format CHECK (email ~* '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}$')
);

-- User Groups table
CREATE TABLE IF NOT EXISTS user_groups (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE,
    description VARCHAR(500),
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    created_by INTEGER,
    updated_by INTEGER
);

-- User Group Memberships table (many-to-many)
CREATE TABLE IF NOT EXISTS user_group_memberships (
    id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL,
    user_group_id INTEGER NOT NULL,
    assigned_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    assigned_by INTEGER,
    CONSTRAINT fk_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT fk_user_group FOREIGN KEY (user_group_id) REFERENCES user_groups(id) ON DELETE CASCADE,
    CONSTRAINT uq_user_group_membership UNIQUE (user_id, user_group_id)
);

-- Screens table
CREATE TABLE IF NOT EXISTS screens (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE,
    display_name VARCHAR(200) NOT NULL,
    description VARCHAR(500),
    route VARCHAR(500),
    icon VARCHAR(100),
    parent_screen_id INTEGER,
    "order" INTEGER NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    CONSTRAINT fk_parent_screen FOREIGN KEY (parent_screen_id) REFERENCES screens(id) ON DELETE SET NULL
);

-- Screen Actions table
CREATE TABLE IF NOT EXISTS screen_actions (
    id SERIAL PRIMARY KEY,
    screen_id INTEGER NOT NULL,
    action_name VARCHAR(100) NOT NULL,
    display_name VARCHAR(200) NOT NULL,
    description VARCHAR(500),
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_screen FOREIGN KEY (screen_id) REFERENCES screens(id) ON DELETE CASCADE,
    CONSTRAINT uq_screen_action UNIQUE (screen_id, action_name)
);

-- Screen Permissions table
CREATE TABLE IF NOT EXISTS screen_permissions (
    id SERIAL PRIMARY KEY,
    user_group_id INTEGER NOT NULL,
    screen_id INTEGER NOT NULL,
    screen_action_id INTEGER NOT NULL,
    is_granted BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by INTEGER,
    CONSTRAINT fk_permission_user_group FOREIGN KEY (user_group_id) REFERENCES user_groups(id) ON DELETE CASCADE,
    CONSTRAINT fk_permission_screen FOREIGN KEY (screen_id) REFERENCES screens(id) ON DELETE CASCADE,
    CONSTRAINT fk_permission_action FOREIGN KEY (screen_action_id) REFERENCES screen_actions(id) ON DELETE CASCADE,
    CONSTRAINT uq_permission UNIQUE (user_group_id, screen_id, screen_action_id)
);

-- Menus table
CREATE TABLE IF NOT EXISTS menus (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE,
    display_name VARCHAR(200) NOT NULL,
    description VARCHAR(500),
    location INTEGER NOT NULL, -- 1: Sidebar, 2: Header
    "order" INTEGER NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    created_by INTEGER,
    updated_by INTEGER,
    CONSTRAINT chk_location CHECK (location IN (1, 2))
);

-- Menu Items table
CREATE TABLE IF NOT EXISTS menu_items (
    id SERIAL PRIMARY KEY,
    menu_id INTEGER NOT NULL,
    parent_menu_item_id INTEGER,
    item_type INTEGER NOT NULL, -- 1: Group, 2: Screen
    screen_id INTEGER,
    display_name VARCHAR(200) NOT NULL,
    icon VARCHAR(100),
    route VARCHAR(500),
    "order" INTEGER NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT true,
    badge_text VARCHAR(50),
    badge_color VARCHAR(50),
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    created_by INTEGER,
    updated_by INTEGER,
    CONSTRAINT fk_menu FOREIGN KEY (menu_id) REFERENCES menus(id) ON DELETE CASCADE,
    CONSTRAINT fk_parent_menu_item FOREIGN KEY (parent_menu_item_id) REFERENCES menu_items(id) ON DELETE CASCADE,
    CONSTRAINT fk_menu_screen FOREIGN KEY (screen_id) REFERENCES screens(id) ON DELETE SET NULL,
    CONSTRAINT chk_item_type CHECK (item_type IN (1, 2))
);

-- ===================================
-- Indexes
-- ===================================

-- User indexes
CREATE INDEX idx_users_username ON users(username);
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_created_at ON users(created_at);

-- User Group Membership indexes
CREATE INDEX idx_memberships_user_id ON user_group_memberships(user_id);
CREATE INDEX idx_memberships_group_id ON user_group_memberships(user_group_id);

-- Screen indexes
CREATE INDEX idx_screens_name ON screens(name);
CREATE INDEX idx_screens_order ON screens("order");
CREATE INDEX idx_screens_parent ON screens(parent_screen_id);

-- Screen Action indexes
CREATE INDEX idx_actions_screen_id ON screen_actions(screen_id);
CREATE INDEX idx_actions_name ON screen_actions(action_name);

-- Permission indexes
CREATE INDEX idx_permissions_user_group ON screen_permissions(user_group_id);
CREATE INDEX idx_permissions_screen ON screen_permissions(screen_id);
CREATE INDEX idx_permissions_action ON screen_permissions(screen_action_id);

-- Menu indexes
CREATE INDEX idx_menus_location ON menus(location);
CREATE INDEX idx_menus_order ON menus("order");

-- Menu Item indexes
CREATE INDEX idx_menu_items_menu ON menu_items(menu_id);
CREATE INDEX idx_menu_items_parent ON menu_items(parent_menu_item_id);
CREATE INDEX idx_menu_items_screen ON menu_items(screen_id);
CREATE INDEX idx_menu_items_order ON menu_items("order");

-- ===================================
-- Seed Data
-- ===================================

-- Seed User Groups
INSERT INTO user_groups (name, description, is_active, created_at) VALUES
('Administrators', 'Full system access', true, CURRENT_TIMESTAMP),
('Managers', 'Management level access', true, CURRENT_TIMESTAMP),
('Users', 'Standard user access', true, CURRENT_TIMESTAMP)
ON CONFLICT (name) DO NOTHING;

-- Seed Screens
INSERT INTO screens (name, display_name, description, route, icon, "order", is_active, created_at) VALUES
('Dashboard', 'Ana Sayfa', 'Dashboard ekranı', '/dashboard', 'dashboard', 1, true, CURRENT_TIMESTAMP),
('Users', 'Kullanıcılar', 'Kullanıcı yönetimi ekranı', '/users', 'people', 2, true, CURRENT_TIMESTAMP),
('Settings', 'Ayarlar', 'Sistem ayarları', '/settings', 'settings', 3, true, CURRENT_TIMESTAMP)
ON CONFLICT (name) DO NOTHING;

-- Seed Screen Actions
INSERT INTO screen_actions (screen_id, action_name, display_name, is_active, created_at)
SELECT s.id, 'view', 'Görüntüle', true, CURRENT_TIMESTAMP FROM screens s WHERE s.name = 'Dashboard'
ON CONFLICT (screen_id, action_name) DO NOTHING;

INSERT INTO screen_actions (screen_id, action_name, display_name, is_active, created_at)
SELECT s.id, 'export', 'Dışa Aktar', true, CURRENT_TIMESTAMP FROM screens s WHERE s.name = 'Dashboard'
ON CONFLICT (screen_id, action_name) DO NOTHING;

INSERT INTO screen_actions (screen_id, action_name, display_name, is_active, created_at)
SELECT s.id, 'view', 'Görüntüle', true, CURRENT_TIMESTAMP FROM screens s WHERE s.name = 'Users'
ON CONFLICT (screen_id, action_name) DO NOTHING;

INSERT INTO screen_actions (screen_id, action_name, display_name, is_active, created_at)
SELECT s.id, 'create', 'Oluştur', true, CURRENT_TIMESTAMP FROM screens s WHERE s.name = 'Users'
ON CONFLICT (screen_id, action_name) DO NOTHING;

INSERT INTO screen_actions (screen_id, action_name, display_name, is_active, created_at)
SELECT s.id, 'edit', 'Düzenle', true, CURRENT_TIMESTAMP FROM screens s WHERE s.name = 'Users'
ON CONFLICT (screen_id, action_name) DO NOTHING;

INSERT INTO screen_actions (screen_id, action_name, display_name, is_active, created_at)
SELECT s.id, 'delete', 'Sil', true, CURRENT_TIMESTAMP FROM screens s WHERE s.name = 'Users'
ON CONFLICT (screen_id, action_name) DO NOTHING;

INSERT INTO screen_actions (screen_id, action_name, display_name, is_active, created_at)
SELECT s.id, 'export', 'Dışa Aktar', true, CURRENT_TIMESTAMP FROM screens s WHERE s.name = 'Users'
ON CONFLICT (screen_id, action_name) DO NOTHING;

INSERT INTO screen_actions (screen_id, action_name, display_name, is_active, created_at)
SELECT s.id, 'view', 'Görüntüle', true, CURRENT_TIMESTAMP FROM screens s WHERE s.name = 'Settings'
ON CONFLICT (screen_id, action_name) DO NOTHING;

INSERT INTO screen_actions (screen_id, action_name, display_name, is_active, created_at)
SELECT s.id, 'edit', 'Düzenle', true, CURRENT_TIMESTAMP FROM screens s WHERE s.name = 'Settings'
ON CONFLICT (screen_id, action_name) DO NOTHING;

-- Seed Screen Permissions (Administrators get all permissions)
INSERT INTO screen_permissions (user_group_id, screen_id, screen_action_id, is_granted, created_at)
SELECT ug.id, sa.screen_id, sa.id, true, CURRENT_TIMESTAMP
FROM user_groups ug
CROSS JOIN screen_actions sa
WHERE ug.name = 'Administrators'
ON CONFLICT (user_group_id, screen_id, screen_action_id) DO NOTHING;

-- Seed Screen Permissions (Users get view permissions)
INSERT INTO screen_permissions (user_group_id, screen_id, screen_action_id, is_granted, created_at)
SELECT ug.id, sa.screen_id, sa.id, true, CURRENT_TIMESTAMP
FROM user_groups ug
CROSS JOIN screen_actions sa
WHERE ug.name = 'Users' AND sa.action_name = 'view'
ON CONFLICT (user_group_id, screen_id, screen_action_id) DO NOTHING;

-- Seed Menus
INSERT INTO menus (name, display_name, description, location, "order", is_active, created_at) VALUES
('MainSidebar', 'Ana Menü', 'Sol taraf ana menü', 1, 1, true, CURRENT_TIMESTAMP),
('TopHeader', 'Üst Menü', 'Header menüsü', 2, 1, true, CURRENT_TIMESTAMP)
ON CONFLICT (name) DO NOTHING;

-- Seed Menu Items
DO $$
DECLARE
    sidebar_menu_id INTEGER;
    header_menu_id INTEGER;
    dashboard_screen_id INTEGER;
    users_screen_id INTEGER;
    settings_screen_id INTEGER;
    management_group_id INTEGER;
BEGIN
    -- Get menu IDs
    SELECT id INTO sidebar_menu_id FROM menus WHERE name = 'MainSidebar';
    SELECT id INTO header_menu_id FROM menus WHERE name = 'TopHeader';

    -- Get screen IDs
    SELECT id INTO dashboard_screen_id FROM screens WHERE name = 'Dashboard';
    SELECT id INTO users_screen_id FROM screens WHERE name = 'Users';
    SELECT id INTO settings_screen_id FROM screens WHERE name = 'Settings';

    -- Insert Dashboard menu item (sidebar)
    INSERT INTO menu_items (menu_id, item_type, screen_id, display_name, icon, route, "order", is_active, created_at)
    VALUES (sidebar_menu_id, 2, dashboard_screen_id, 'Ana Sayfa', 'dashboard', '/dashboard', 1, true, CURRENT_TIMESTAMP)
    ON CONFLICT DO NOTHING;

    -- Insert Management group
    INSERT INTO menu_items (menu_id, item_type, display_name, icon, "order", is_active, created_at)
    VALUES (sidebar_menu_id, 1, 'Yönetim', 'settings', 2, true, CURRENT_TIMESTAMP)
    ON CONFLICT DO NOTHING
    RETURNING id INTO management_group_id;

    -- Get management group ID if not returned
    IF management_group_id IS NULL THEN
        SELECT id INTO management_group_id FROM menu_items
        WHERE menu_id = sidebar_menu_id AND item_type = 1 AND display_name = 'Yönetim';
    END IF;

    -- Insert Users under Management
    INSERT INTO menu_items (menu_id, parent_menu_item_id, item_type, screen_id, display_name, icon, route, "order", is_active, created_at)
    VALUES (sidebar_menu_id, management_group_id, 2, users_screen_id, 'Kullanıcılar', 'people', '/users', 1, true, CURRENT_TIMESTAMP)
    ON CONFLICT DO NOTHING;

    -- Insert Settings under Management
    INSERT INTO menu_items (menu_id, parent_menu_item_id, item_type, screen_id, display_name, icon, route, "order", is_active, created_at)
    VALUES (sidebar_menu_id, management_group_id, 2, settings_screen_id, 'Ayarlar', 'settings', '/settings', 2, true, CURRENT_TIMESTAMP)
    ON CONFLICT DO NOTHING;

    -- Insert Dashboard menu item (header)
    INSERT INTO menu_items (menu_id, item_type, screen_id, display_name, icon, route, "order", is_active, created_at)
    VALUES (header_menu_id, 2, dashboard_screen_id, 'Dashboard', 'dashboard', '/dashboard', 1, true, CURRENT_TIMESTAMP)
    ON CONFLICT DO NOTHING;
END $$;

-- Seed default admin user (password: Admin123!)
-- Password hash is SHA256 of "Admin123!"
INSERT INTO users (username, email, password_hash, role, created_at) VALUES
('admin', 'admin@example.com', 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=', 'Admin', CURRENT_TIMESTAMP)
ON CONFLICT (username) DO NOTHING;

-- Assign admin to Administrators group
INSERT INTO user_group_memberships (user_id, user_group_id, assigned_at)
SELECT u.id, ug.id, CURRENT_TIMESTAMP
FROM users u
CROSS JOIN user_groups ug
WHERE u.username = 'admin' AND ug.name = 'Administrators'
ON CONFLICT (user_id, user_group_id) DO NOTHING;

-- ===================================
-- Views (Optional - for easier queries)
-- ===================================

-- View: User permissions summary
CREATE OR REPLACE VIEW v_user_permissions AS
SELECT
    u.id as user_id,
    u.username,
    ug.name as user_group_name,
    s.name as screen_name,
    s.display_name as screen_display_name,
    sa.action_name,
    sa.display_name as action_display_name,
    sp.is_granted
FROM users u
JOIN user_group_memberships ugm ON u.id = ugm.user_id
JOIN user_groups ug ON ugm.user_group_id = ug.id
JOIN screen_permissions sp ON ug.id = sp.user_group_id
JOIN screens s ON sp.screen_id = s.id
JOIN screen_actions sa ON sp.screen_action_id = sa.id
WHERE ug.is_active = true AND s.is_active = true AND sa.is_active = true;

-- View: Menu structure
CREATE OR REPLACE VIEW v_menu_structure AS
SELECT
    m.id as menu_id,
    m.name as menu_name,
    m.display_name as menu_display_name,
    m.location,
    mi.id as menu_item_id,
    mi.parent_menu_item_id,
    mi.item_type,
    mi.display_name as item_display_name,
    mi.icon,
    mi.route,
    mi."order",
    s.name as screen_name,
    s.id as screen_id
FROM menus m
LEFT JOIN menu_items mi ON m.id = mi.menu_id
LEFT JOIN screens s ON mi.screen_id = s.id
WHERE m.is_active = true AND (mi.id IS NULL OR mi.is_active = true)
ORDER BY m."order", mi."order";

COMMENT ON TABLE users IS 'Application users';
COMMENT ON TABLE user_groups IS 'User groups/roles';
COMMENT ON TABLE user_group_memberships IS 'User to group assignments (many-to-many)';
COMMENT ON TABLE screens IS 'Application screens/pages';
COMMENT ON TABLE screen_actions IS 'Actions available on each screen';
COMMENT ON TABLE screen_permissions IS 'Permissions assigned to user groups for screen actions';
COMMENT ON TABLE menus IS 'Menu definitions';
COMMENT ON TABLE menu_items IS 'Menu items with hierarchical support';
