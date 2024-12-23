import "./assets/main.css";

import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import axios from "axios";
import Toast from "vue-toastification";
import "vue-toastification/dist/index.css";
// Cấu hình axios mặc định
axios.defaults.baseURL = "https://your-api-url.com";
axios.defaults.headers.common["Authorization"] = "Bearer token";
axios.defaults.headers.post["Content-Type"] = "application/json";

const app = createApp(App);

app.config.globalProperties.$axios = axios;

const options = {
	transition: "Vue-Toastification__bounce",
	maxToasts: 20,
	newestOnTop: true,
};

app.use(Toast, options);
app.use(router);

app.mount("#app");
