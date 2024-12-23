<template>
  <div class="scroll-container">
    <h5>Best-selling</h5>
    <div class="scroll-wrapper">
      <ul class="food-list">
        <li v-for="food in bestSellingFoods" :key="food.id" class="food-item">
          <img :src="`https://localhost:7093/${food.image}`" :alt="food.name" class="food-image" />
          <div class="food-info">
            <h2>{{ food.name }}</h2>
            <p>Giá: {{ formatPrice(food.price) }}</p>
            <p>Đã bán: {{ food.soldCount }} suất</p>
          </div>
        </li>
      </ul>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import axiosInstance from "../config/axios.js";

const bestSellingFoods = ref([]);

const fetchBestSellingFoods = async () => {
  try {
    const response = await axiosInstance.get("/food/best-selling");
    bestSellingFoods.value = response.data.$values;
  } catch (error) {
    console.error("Lỗi khi tải danh sách món ăn bán chạy:", error);
  }
};

const formatPrice = (price) => {
  return price.toLocaleString("vi-VN", { style: "currency", currency: "VND" });
};

// Tạo danh sách lặp lại để hiệu ứng scroll hoạt động liên tục
const loopedFoods = computed(() => {
  return [...bestSellingFoods.value, ...bestSellingFoods.value];
});

onMounted(fetchBestSellingFoods);
</script>

<style scoped>
.scroll-container {
  position: relative;
  width: 100%;
  overflow: hidden;
  padding: 20px 0;
  background-color: #f9f9f9;
}

.scroll-container h5 {
  font-size: 24px;
  font-weight: bold;
  color: #333;
  text-align: center;
  margin-bottom: 20px;
  position: relative;
  padding-bottom: 10px;
}

.scroll-container h5::after {
  content: "";
  display: block;
  width: 50px;
  height: 3px;
  background-color: #ff5722;
  margin: 8px auto 0;
  border-radius: 2px;
}

.scroll-wrapper {
  display: flex;
  align-items: center;
  overflow-x: scroll; /* Kích hoạt cuộn ngang */
  scroll-behavior: smooth; /* Cuộn mượt */
  gap: 16px;
}

.scroll-wrapper::-webkit-scrollbar {
  display: none; /* Ẩn thanh cuộn */
}

.food-list {
  display: flex;
  list-style: none;
  padding: 0;
  margin: 0;
}

.food-item {
  flex: 0 0 auto;
  width: 200px;
  margin: 0 10px;
  border: 1px solid #ddd;
  border-radius: 8px;
  background: #fff;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  text-align: center;
  padding: 16px;
  transition: transform 0.3s ease, box-shadow 0.3s ease;
}

.food-item:hover {
  transform: scale(1.1);
  box-shadow: 0 6px 12px rgba(0, 0, 0, 0.2);
}

.food-image {
  width: 100px;
  height: 100px;
  object-fit: cover;
  border-radius: 6px;
  margin-bottom: 8px;
}

.food-info h2 {
  font-size: 1.1em;
  margin-bottom: 8px;
  height: 40px;
}

.food-info p {
  font-size: 1em;
  color: #555;
  padding: 4px 0;
  height: 25px;
}
</style>
