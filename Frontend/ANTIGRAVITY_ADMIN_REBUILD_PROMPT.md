# 🚀 ANTIGRAVITY: Hướng Dẫn Rebuild Admin System với Backend

## 📋 TỔNG QUAN HỆ THỐNG ADMIN

Hệ thống quản lý dự án theo phong cách Jira với giao diện **light neumorphism** và **màu teal accents**. Admin section cần có backend đầy đủ để quản lý users, audit logs, organization settings, và customization.

---

## 🎯 YÊU CẦU QUAN TRỌNG: MỞ ADMIN TRONG TAB MỚI

**QUAN TRỌNG**: Khi user click vào Settings icon (bánh răng) ở góc phải header và chọn các trang admin, hệ thống phải **MỞ ADMIN TRONG TAB MỚI CỦA TRÌNH DUYỆT** giống như Jira, KHÔNG phải điều hướng trong cùng một tab.

### Cách Thực Hiện:

#### Option 1: Sử dụng `window.open()` với URL đầy đủ
```typescript
// Trong Layout.tsx - Settings Dropdown
<button
  onClick={() => {
    window.open(`${window.location.origin}/admin/audit-log`, "_blank");
    setShowSettings(false);
  }}
  className="w-full px-4 py-2 text-left text-sm text-gray-700 hover:bg-gray-50 transition-colors"
>
  Audit Log
</button>
```

#### Option 2: Sử dụng `<a>` tag với `target="_blank"` (Recommended)
```typescript
// Trong Layout.tsx - Settings Dropdown
<a
  href="/admin/audit-log"
  target="_blank"
  rel="noopener noreferrer"
  className="w-full px-4 py-2 text-left text-sm text-gray-700 hover:bg-gray-50 transition-colors block"
  onClick={() => setShowSettings(false)}
>
  Audit Log
</a>
```

**Áp dụng cho tất cả 6 links admin:**
1. Audit Log → `/admin/audit-log`
2. User Management → `/admin/user-management`
3. Organization Profile → `/admin/organization/profile`
4. Organization Contact → `/admin/organization/contact`
5. Configuration → `/admin/configuration`
6. Customization → `/admin/customization`

---

## 🗄️ DATABASE SCHEMA (Supabase)

### 1. **Bảng `audit_logs`**
```sql
CREATE TABLE audit_logs (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  timestamp TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  user_email TEXT NOT NULL,
  action TEXT NOT NULL,
  resource TEXT NOT NULL,
  status TEXT NOT NULL CHECK (status IN ('Success', 'Warning', 'Error')),
  ip_address INET NOT NULL,
  details JSONB,
  created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX idx_audit_logs_timestamp ON audit_logs(timestamp DESC);
CREATE INDEX idx_audit_logs_user ON audit_logs(user_email);
CREATE INDEX idx_audit_logs_status ON audit_logs(status);
```

### 2. **Bảng `users`**
```sql
CREATE TABLE users (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  name TEXT NOT NULL,
  email TEXT UNIQUE NOT NULL,
  phone TEXT,
  role TEXT NOT NULL CHECK (role IN ('Admin', 'Editor', 'Viewer')),
  status TEXT NOT NULL DEFAULT 'Active' CHECK (status IN ('Active', 'Inactive')),
  avatar_url TEXT,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_role ON users(role);
CREATE INDEX idx_users_status ON users(status);
```

### 3. **Bảng `organization`**
```sql
CREATE TABLE organization (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  name TEXT NOT NULL,
  description TEXT,
  industry TEXT,
  website_url TEXT,

  -- Contact Information
  primary_contact_name TEXT,
  primary_contact_email TEXT,
  primary_contact_phone TEXT,

  -- Address
  address_line1 TEXT,
  address_line2 TEXT,
  city TEXT,
  state TEXT,
  postal_code TEXT,
  country TEXT,

  -- Settings
  timezone TEXT DEFAULT 'UTC',
  date_format TEXT DEFAULT 'MM/DD/YYYY',

  logo_url TEXT,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Insert default organization
INSERT INTO organization (name, description)
VALUES ('Nexus Corporation', 'Default organization');
```

### 4. **Bảng `configuration`**
```sql
CREATE TABLE configuration (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  key TEXT UNIQUE NOT NULL,
  value JSONB NOT NULL,
  category TEXT NOT NULL, -- 'security', 'notifications', 'integrations', etc.
  description TEXT,
  updated_by TEXT,
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Insert default configurations
INSERT INTO configuration (key, value, category, description) VALUES
  ('session_timeout', '{"value": 30, "unit": "minutes"}', 'security', 'User session timeout duration'),
  ('two_factor_auth', '{"enabled": true}', 'security', 'Two-factor authentication setting'),
  ('email_notifications', '{"enabled": true, "types": ["mentions", "assignments", "due_dates"]}', 'notifications', 'Email notification preferences'),
  ('slack_integration', '{"enabled": false, "webhook_url": ""}', 'integrations', 'Slack integration settings'),
  ('jira_integration', '{"enabled": false, "api_key": "", "domain": ""}', 'integrations', 'Jira integration settings');
```

### 5. **Bảng `theme_customization`**
```sql
CREATE TABLE theme_customization (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  user_id UUID REFERENCES users(id) ON DELETE CASCADE,
  is_global BOOLEAN DEFAULT false, -- true nếu là theme của toàn organization
  theme_name TEXT,
  colors JSONB NOT NULL,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW(),

  CONSTRAINT unique_user_theme UNIQUE (user_id),
  CONSTRAINT unique_global_theme UNIQUE (is_global) WHERE is_global = true
);

-- Insert default global theme
INSERT INTO theme_customization (is_global, theme_name, colors) VALUES
  (true, 'Light Neumorphism (Default)', '{
    "background": "#e8eaed",
    "cardBackground": "#f5f6f8",
    "sidebarBackground": "#ffffff",
    "headerBackground": "#ffffff",
    "text": "#1f2937",
    "textSecondary": "#6b7280",
    "border": "#d1d5db",
    "primary": "#0d9488",
    "primaryHover": "#0f766e"
  }');
```

---

## 🔌 API ENDPOINTS (Supabase Edge Functions hoặc REST API)

### Setup Supabase Client
```typescript
// src/lib/supabase.ts
import { createClient } from '@supabase/supabase-js';

const supabaseUrl = import.meta.env.VITE_SUPABASE_URL;
const supabaseAnonKey = import.meta.env.VITE_SUPABASE_ANON_KEY;

export const supabase = createClient(supabaseUrl, supabaseAnonKey);
```

### Environment Variables (`.env`)
```bash
VITE_SUPABASE_URL=https://your-project.supabase.co
VITE_SUPABASE_ANON_KEY=your-anon-key
```

---

## 📄 CẤU TRÚC CÁC TRANG ADMIN VÀ INTEGRATION

### 1️⃣ **Audit Log Page** (`/admin/audit-log`)

**Chức năng:**
- Hiển thị lịch sử tất cả hành động của users (login, logout, create, update, delete)
- Filter theo thời gian: All Time, 24h, 30d
- Realtime updates khi có action mới
- Export to CSV

**API Integration:**
```typescript
// src/app/components/admin/AuditLog.tsx
import { supabase } from '../../lib/supabase';
import { useEffect, useState } from 'react';

export function AuditLog() {
  const [logs, setLogs] = useState([]);
  const [timeFilter, setTimeFilter] = useState('all');
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchAuditLogs();

    // Subscribe to realtime changes
    const subscription = supabase
      .channel('audit_logs_changes')
      .on('postgres_changes',
        { event: 'INSERT', schema: 'public', table: 'audit_logs' },
        (payload) => {
          setLogs((current) => [payload.new, ...current]);
        }
      )
      .subscribe();

    return () => {
      subscription.unsubscribe();
    };
  }, [timeFilter]);

  const fetchAuditLogs = async () => {
    setLoading(true);
    let query = supabase
      .from('audit_logs')
      .select('*')
      .order('timestamp', { ascending: false });

    if (timeFilter === '24h') {
      const twentyFourHoursAgo = new Date(Date.now() - 24 * 60 * 60 * 1000).toISOString();
      query = query.gte('timestamp', twentyFourHoursAgo);
    } else if (timeFilter === '30d') {
      const thirtyDaysAgo = new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString();
      query = query.gte('timestamp', thirtyDaysAgo);
    }

    const { data, error } = await query.limit(100);

    if (!error && data) {
      setLogs(data);
    }
    setLoading(false);
  };

  const exportToCSV = () => {
    const csvContent = [
      ['Timestamp', 'User', 'Action', 'Resource', 'Status', 'IP Address'].join(','),
      ...logs.map(log => [
        log.timestamp,
        log.user_email,
        log.action,
        log.resource,
        log.status,
        log.ip_address
      ].join(','))
    ].join('\n');

    const blob = new Blob([csvContent], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `audit-log-${new Date().toISOString()}.csv`;
    a.click();
  };

  // ... render UI với loading state và data từ database
}
```

**Logging Function (Helper):**
```typescript
// src/lib/auditLogger.ts
import { supabase } from './supabase';

export async function logAction(
  userEmail: string,
  action: string,
  resource: string,
  status: 'Success' | 'Warning' | 'Error',
  details?: any
) {
  const ipAddress = await fetch('https://api.ipify.org?format=json')
    .then(res => res.json())
    .then(data => data.ip)
    .catch(() => '0.0.0.0');

  await supabase.from('audit_logs').insert({
    user_email: userEmail,
    action,
    resource,
    status,
    ip_address: ipAddress,
    details
  });
}
```

---

### 2️⃣ **User Management Page** (`/admin/user-management`)

**Chức năng:**
- Hiển thị danh sách users với avatar, name, email, phone, role, status
- Add new user (modal popup)
- Edit user (modal popup với form)
- Delete user (confirmation dialog)
- Pagination (10 users per page)
- Search và filter by role/status

**API Integration:**
```typescript
// src/app/components/admin/UserManagement.tsx
import { supabase } from '../../lib/supabase';
import { logAction } from '../../lib/auditLogger';

export function UserManagement() {
  const [users, setUsers] = useState([]);
  const [showAddModal, setShowAddModal] = useState(false);
  const [editingUser, setEditingUser] = useState(null);

  useEffect(() => {
    fetchUsers();
  }, []);

  const fetchUsers = async () => {
    const { data, error } = await supabase
      .from('users')
      .select('*')
      .order('created_at', { ascending: false });

    if (!error && data) {
      setUsers(data);
    }
  };

  const addUser = async (userData) => {
    const { data, error } = await supabase
      .from('users')
      .insert([userData])
      .select()
      .single();

    if (!error && data) {
      setUsers([data, ...users]);
      await logAction('admin@nexus.io', 'User Created', userData.email, 'Success');
      setShowAddModal(false);
    }
  };

  const updateUser = async (userId, updates) => {
    const { data, error } = await supabase
      .from('users')
      .update({ ...updates, updated_at: new Date().toISOString() })
      .eq('id', userId)
      .select()
      .single();

    if (!error && data) {
      setUsers(users.map(u => u.id === userId ? data : u));
      await logAction('admin@nexus.io', 'User Updated', data.email, 'Success');
      setEditingUser(null);
    }
  };

  const deleteUser = async (userId, userEmail) => {
    const { error } = await supabase
      .from('users')
      .delete()
      .eq('id', userId);

    if (!error) {
      setUsers(users.filter(u => u.id !== userId));
      await logAction('admin@nexus.io', 'User Deleted', userEmail, 'Success');
    }
  };

  // ... render UI với Add/Edit modals
}
```

---

### 3️⃣ **Organization Profile Page** (`/admin/organization/profile`)

**Chức năng:**
- Form để edit organization info: name, description, industry, website, logo
- Upload logo (Supabase Storage)
- Save button với loading state

**API Integration:**
```typescript
// src/app/components/admin/OrganizationProfile.tsx
import { supabase } from '../../lib/supabase';

export function OrganizationProfile() {
  const [orgData, setOrgData] = useState(null);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    fetchOrganization();
  }, []);

  const fetchOrganization = async () => {
    const { data, error } = await supabase
      .from('organization')
      .select('*')
      .single();

    if (!error && data) {
      setOrgData(data);
    }
  };

  const uploadLogo = async (file) => {
    const fileExt = file.name.split('.').pop();
    const fileName = `logo-${Date.now()}.${fileExt}`;

    const { data, error } = await supabase.storage
      .from('organization-assets')
      .upload(fileName, file);

    if (!error) {
      const { data: { publicUrl } } = supabase.storage
        .from('organization-assets')
        .getPublicUrl(fileName);

      return publicUrl;
    }
    return null;
  };

  const saveOrganization = async (updates) => {
    setSaving(true);
    const { error } = await supabase
      .from('organization')
      .update({ ...updates, updated_at: new Date().toISOString() })
      .eq('id', orgData.id);

    if (!error) {
      await logAction('admin@nexus.io', 'Organization Updated', 'Profile', 'Success');
      await fetchOrganization();
    }
    setSaving(false);
  };

  // ... render form với các fields
}
```

---

### 4️⃣ **Organization Contact Page** (`/admin/organization/contact`)

**Chức năng:**
- Form để edit contact info: primary contact name/email/phone
- Address fields: line1, line2, city, state, postal code, country
- Timezone selector
- Save button

**API Integration:** (Tương tự OrganizationProfile, cùng bảng `organization`)

---

### 5️⃣ **Configuration Page** (`/admin/configuration`)

**Chức năng:**
- Các sections: Security, Notifications, Integrations
- Toggle switches cho các settings
- Input fields cho API keys, webhook URLs
- Save changes button

**API Integration:**
```typescript
// src/app/components/admin/Configuration.tsx
import { supabase } from '../../lib/supabase';

export function Configuration() {
  const [configs, setConfigs] = useState({});

  useEffect(() => {
    fetchConfigurations();
  }, []);

  const fetchConfigurations = async () => {
    const { data, error } = await supabase
      .from('configuration')
      .select('*');

    if (!error && data) {
      const configMap = {};
      data.forEach(config => {
        configMap[config.key] = config.value;
      });
      setConfigs(configMap);
    }
  };

  const updateConfiguration = async (key, value) => {
    const { error } = await supabase
      .from('configuration')
      .update({
        value,
        updated_by: 'admin@nexus.io',
        updated_at: new Date().toISOString()
      })
      .eq('key', key);

    if (!error) {
      await logAction('admin@nexus.io', 'Configuration Changed', key, 'Success');
      await fetchConfigurations();
    }
  };

  // ... render sections với toggles và inputs
}
```

---

### 6️⃣ **Customization Page** (`/admin/customization`)

**Chức năng:**
- Tab 1: Custom Colors (color pickers cho từng element)
- Tab 2: Templates (8 pre-built themes: 4 light + 4 dark)
- Apply button để save theme
- Reset to default button
- Preview mode

**API Integration:**
```typescript
// src/app/components/admin/Customization.tsx
import { supabase } from '../../lib/supabase';
import { useTheme } from '../../contexts/ThemeContext';

export function Customization() {
  const { colors, setColors } = useTheme();
  const [customColors, setCustomColors] = useState(colors);
  const [activeTab, setActiveTab] = useState('custom');

  const saveCustomTheme = async () => {
    const { error } = await supabase
      .from('theme_customization')
      .upsert({
        is_global: true,
        theme_name: 'Custom Theme',
        colors: customColors,
        updated_at: new Date().toISOString()
      });

    if (!error) {
      setColors(customColors);
      await logAction('admin@nexus.io', 'Theme Changed', 'Custom Colors', 'Success');
    }
  };

  const applyTemplate = async (template) => {
    const { error } = await supabase
      .from('theme_customization')
      .upsert({
        is_global: true,
        theme_name: template.name,
        colors: template.colors,
        updated_at: new Date().toISOString()
      });

    if (!error) {
      setColors(template.colors);
      await logAction('admin@nexus.io', 'Theme Changed', template.name, 'Success');
    }
  };

  // ... render tabs với color pickers và template cards
}
```

---

## 🔐 ROW LEVEL SECURITY (RLS) POLICIES

```sql
-- Enable RLS on all tables
ALTER TABLE audit_logs ENABLE ROW LEVEL SECURITY;
ALTER TABLE users ENABLE ROW LEVEL SECURITY;
ALTER TABLE organization ENABLE ROW LEVEL SECURITY;
ALTER TABLE configuration ENABLE ROW LEVEL SECURITY;
ALTER TABLE theme_customization ENABLE ROW LEVEL SECURITY;

-- Audit Logs: Admin can view all, users can view their own
CREATE POLICY "Admin can view all audit logs" ON audit_logs
  FOR SELECT USING (
    EXISTS (
      SELECT 1 FROM users
      WHERE users.email = auth.jwt() ->> 'email'
      AND users.role = 'Admin'
    )
  );

CREATE POLICY "Users can view their own audit logs" ON audit_logs
  FOR SELECT USING (user_email = auth.jwt() ->> 'email');

-- Users table: Admin full access, users can view all
CREATE POLICY "Admin can manage users" ON users
  FOR ALL USING (
    EXISTS (
      SELECT 1 FROM users
      WHERE users.email = auth.jwt() ->> 'email'
      AND users.role = 'Admin'
    )
  );

CREATE POLICY "Users can view all users" ON users
  FOR SELECT USING (true);

-- Organization: Admin can edit, everyone can view
CREATE POLICY "Everyone can view organization" ON organization
  FOR SELECT USING (true);

CREATE POLICY "Admin can update organization" ON organization
  FOR UPDATE USING (
    EXISTS (
      SELECT 1 FROM users
      WHERE users.email = auth.jwt() ->> 'email'
      AND users.role = 'Admin'
    )
  );

-- Configuration: Admin full access, others can view
CREATE POLICY "Everyone can view configuration" ON configuration
  FOR SELECT USING (true);

CREATE POLICY "Admin can manage configuration" ON configuration
  FOR ALL USING (
    EXISTS (
      SELECT 1 FROM users
      WHERE users.email = auth.jwt() ->> 'email'
      AND users.role = 'Admin'
    )
  );

-- Theme: Everyone can view global theme, admin can update
CREATE POLICY "Everyone can view theme" ON theme_customization
  FOR SELECT USING (true);

CREATE POLICY "Admin can update theme" ON theme_customization
  FOR ALL USING (
    EXISTS (
      SELECT 1 FROM users
      WHERE users.email = auth.jwt() ->> 'email'
      AND users.role = 'Admin'
    )
  );
```

---

## 📦 DEPENDENCIES CẦN CÀI ĐẶT

```bash
pnpm add @supabase/supabase-js
pnpm add date-fns  # For date formatting
pnpm add react-hot-toast  # For notifications
```

---

## 🎨 GIAO DIỆN DESIGN SYSTEM

### Colors
- **Background**: `#e8eaed` (light grey)
- **Card Background**: `#f5f6f8` (soft white)
- **Primary (Teal)**: `#0d9488`
- **Primary Hover**: `#0f766e`
- **Text**: `#1f2937` (dark grey)
- **Text Secondary**: `#6b7280` (medium grey)

### Shadows (Neumorphism)
```css
/* Elevated card */
box-shadow: 8px 8px 16px rgba(0,0,0,0.1), -8px -8px 16px rgba(255,255,255,0.7);

/* Pressed/Inset */
box-shadow: inset 2px 2px 5px rgba(0,0,0,0.1), inset -2px -2px 5px rgba(255,255,255,0.7);

/* Button hover */
box-shadow: 4px 4px 10px rgba(20,184,166,0.3), -2px -2px 6px rgba(255,255,255,0.7);
```

---

## 🧪 TESTING CHECKLIST

- [ ] Admin pages mở trong tab mới khi click từ Settings dropdown
- [ ] Audit logs hiển thị realtime khi có action mới
- [ ] User Management: CRUD operations hoạt động
- [ ] Upload logo cho Organization hoạt động
- [ ] Configuration changes được lưu vào database
- [ ] Theme customization apply và persist
- [ ] RLS policies chặn non-admin users edit
- [ ] Mobile responsive cho tất cả admin pages
- [ ] Export CSV từ Audit Log hoạt động
- [ ] Search và pagination trong User Management

---

## 🚀 DEPLOYMENT STEPS

1. **Setup Supabase Project:**
   - Tạo project mới tại https://supabase.com
   - Copy URL và Anon Key vào `.env`

2. **Run Database Migrations:**
   - Copy tất cả SQL queries vào SQL Editor trong Supabase
   - Execute từng bảng một

3. **Setup Storage Bucket:**
   - Tạo bucket `organization-assets` (public)
   - Configure CORS nếu cần

4. **Deploy Frontend:**
   - Build app: `pnpm run build`
   - Deploy to Vercel/Netlify
   - Set environment variables

---

## 📝 PROMPT ĐỂ ANTIGRAVITY ĐỌC LẠI DATABASE

Sau khi Antigravity đã dựng xong hệ thống admin với backend, sử dụng prompt sau để yêu cầu kiểm tra và điều chỉnh data:

```
Hãy kết nối với Supabase database và thực hiện các bước sau:

1. **ĐỌC DỮ LIỆU HIỆN TẠI:**
   - Đọc tất cả records từ bảng `audit_logs`, `users`, `organization`, `configuration`, `theme_customization`
   - Kiểm tra xem có data nào bị thiếu, duplicate, hoặc không hợp lý không

2. **KIỂM TRA TÍNH NHẤT QUÁN:**
   - Audit logs có user_email tương ứng với users trong bảng `users` không?
   - Organization info đã được điền đầy đủ chưa?
   - Configuration có đủ các keys cần thiết: session_timeout, two_factor_auth, email_notifications, integrations không?
   - Theme customization có global theme mặc định chưa?

3. **CHỈNH SỬA NỘI DUNG CHO HỢP LÝ:**
   - Nếu audit_logs thiếu data mẫu, thêm 20-30 logs đa dạng với các actions: User Created, Role Updated, Login Success, Login Failed, Document Edited, Settings Changed, Access Denied
   - Nếu users table thiếu, thêm 5-10 users mẫu với roles khác nhau (Admin, Editor, Viewer) và avatars từ Unsplash
   - Organization profile phải có đầy đủ: name, description, industry, website, contact info, address đầy đủ
   - Configuration keys phải có values hợp lý và realistic
   - Theme customization phải có ít nhất 1 global theme

4. **VALIDATE DATA INTEGRITY:**
   - Check foreign key constraints
   - Verify email formats hợp lệ
   - Ensure timestamps đúng format
   - Test RLS policies bằng cách query với different user roles

5. **GENERATE SEED DATA:**
   - Tạo SQL script để seed initial data cho production
   - Include INSERT statements cho tất cả tables
   - Export script thành file `seed.sql`

6. **REPORT:**
   - List ra tất cả changes đã thực hiện
   - Highlight bất kỳ issues nào cần manual fix
   - Provide statistics: số lượng records trong mỗi table
```

---

## ✅ FINAL CHECKLIST

- [ ] Database schema created
- [ ] RLS policies configured
- [ ] Supabase client setup
- [ ] All 6 admin pages integrated with backend
- [ ] Settings dropdown links mở admin trong tab mới
- [ ] Audit logging hoạt động cho mọi action
- [ ] Upload functionality tested
- [ ] Realtime subscriptions working
- [ ] Error handling implemented
- [ ] Loading states added
- [ ] Toast notifications working
- [ ] Mobile responsive
- [ ] Dark mode support (nếu cần)
- [ ] Documentation updated

---

**LƯU Ý QUAN TRỌNG:**
- Admin section phải MỞ TRONG TAB MỚI như Jira (sử dụng `target="_blank"`)
- Tất cả admin actions phải log vào `audit_logs` table
- Sử dụng ThemeContext để apply theme từ database
- Validate user permissions trước khi cho phép edit
- Implement loading states và error handling cho mọi API call
- Test RLS policies kỹ để đảm bảo security

---

🎉 **Với prompt này, Antigravity sẽ có đầy đủ thông tin để rebuild lại toàn bộ admin system với backend Supabase hoàn chỉnh!**
