// src/axios.js
import axios from "axios";

// Tạo một instance axios với các cấu hình mặc định
const axiosInstance = axios.create({
  baseURL: "https://localhost:7093/api", // URL API mặc định
  headers: {
    "Content-Type": "application/json",
    Authorization: "Bearer token" // Thêm token nếu cần
  },
  timeout: 10000 // Thời gian chờ cho mỗi request
});

// Bạn có thể thêm interceptor để quản lý response và errors
axiosInstance.interceptors.response.use(
  (response) => response,
  (error) => {
    // Xử lý lỗi
    console.error("API Error:", error);
    return Promise.reject(error);
  }
);

export default axiosInstance;
