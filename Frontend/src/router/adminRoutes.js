export default [
  {
    path: '/admin/audit-log',
    name: 'AdminAuditLog',
    component: () => import('../views/admin/AuditLog.vue')
  },
  {
    path: '/admin/users',
    name: 'AdminUserManagement',
    component: () => import('../views/admin/UserManagement.vue')
  },
  {
    path: '/admin/organization/profile',
    name: 'AdminOrgProfile',
    component: () => import('../views/admin/OrganizationProfile.vue')
  },
  {
    path: '/admin/organization/contact',
    name: 'AdminOrgContact',
    component: () => import('../views/admin/OrganizationContact.vue')
  },
  {
    path: '/admin/configuration',
    name: 'AdminConfiguration',
    component: () => import('../views/admin/Configuration.vue')
  },
  {
    path: '/admin/security/2fa',
    name: 'AdminSecurity2FA',
    component: () => import('../views/admin/security/TwoFactorAuth.vue')
  },
  {
    path: '/admin/security/password',
    name: 'AdminSecurityPassword',
    component: () => import('../views/admin/security/ChangePassword.vue')
  },
  {
    path: '/admin/security/ip-whitelist',
    name: 'AdminSecurityIpWhitelist',
    component: () => import('../views/admin/security/IpWhitelist.vue')
  }
]
