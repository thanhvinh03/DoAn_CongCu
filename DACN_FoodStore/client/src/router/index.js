import { createRouter, createWebHistory } from "vue-router";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/order/:idTable/:key",
      name: "order",
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import("../views/OrderView.vue")
    },
    {
      path: "/cart/:idTable",
      name: "cart",
      component: () => import("../views/CartView.vue")
    },
    {
      path: "/access-denied",
      name: "access-denied",
      component: () => import("../views/AccessDenied.vue")
    }
  ]
});

export default router;
