export default [
  {
    path: '/space/:id',
    name: 'SpaceSummary',
    component: () => import('../views/SpaceSummary.vue')
  }
]
