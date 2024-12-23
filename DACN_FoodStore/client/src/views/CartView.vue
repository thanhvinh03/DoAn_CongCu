<template>
	
	<div class="cart-container">
		<div class="cart-header">
			<router-link :to="`/order/${idTable}/${key}`">
				<button class="btn-back">
					<i class="fa fa-arrow-circle-left" aria-hidden="true"></i>
				</button>
			</router-link>
			<h4>Giỏ hàng bàn: {{ idTable }}</h4>
		</div>
		<div class="tabs">
      <button :class="{ active: activeTab === 'cart' }" @click="activeTab = 'cart'">Giỏ hàng</button>
      <button :class="{ active: activeTab === 'history' }" @click="loadHistory()" >Lịch sử đặt món</button>
    </div>
		<!-- Cart Content -->
    <div v-if="activeTab === 'cart'" class="cart-content">
      <h5>Món ăn đã thêm</h5>
      <ul v-if="cartItems.length > 0" class="cart-list">
        <li v-for="item in cartItems" :key="item.id" class="cart-item">
          <div class="item-info">
            <div class="item-name">{{ item.name }}</div>
            <div class="item-price">{{ formatPrice(item.price * item.quantity) }}</div>
          </div>
          <div class="quantity-controls">
            <button @click="decreaseQuantity(item)" :disabled="item.quantity === 1" class="btn-quantity">-</button>
            <span>{{ item.quantity }}</span>
            <button @click="increaseQuantity(item)" class="btn-quantity">+</button>
          </div>
          <button class="btn-delete" @click="removeItem(item.id)">
            <i class="fa-solid fa-trash"></i>
          </button>
        </li>
      </ul>
      <p v-else>Giỏ hàng trống</p>
      <div class="total-price">
        <span>Tổng Cộng:</span>
        <span class="mx-2">
          {{ formatPrice(cartItems.reduce((total, item) => total + item.price * item.quantity, 0)) }}
        </span>
      </div>
      <div class="cart-buttons">
        <button class="btn-confirm" @click="submitOrder()">Đặt món</button>
      </div>
    </div>

    <!-- Order History Content -->
    <div v-if="activeTab === 'history'" class="history-content">
      <h5>Lịch sử đặt món</h5>
	  	<ul v-if="orderHistory.length > 0" class="history-list">
  <li v-for="item in orderHistory" :key="item.id" class="history-item">
    <div class="item-details">
      <span class="item-name">{{ item.name }}</span>
      <span class="item-price">{{ formatPrice(item.price) }}</span>
    </div>
    <div class="item-quantity">
      Số lượng: {{ item.quantity }}
    </div>
  </li>
    <div class="total-price">
        <span>Tổng Cộng:</span>
        <span class="mx-2">
          {{ formatPrice(cartItems.reduce((total, item) => total + item.price * item.quantity, 0)) }}
        </span>
      </div>
</ul>
<p v-else class="no-history">Bạn chưa có đơn hàng nào.</p>

    </div>
	</div>
</template>

<style scoped>
.history-list {
  list-style: none;
  padding: 0;
  margin: 0;
  border: 1px solid #ddd;
  border-radius: 8px;
  overflow: hidden;
}

.history-item {
  display: flex;
  flex-direction: column;
  padding: 12px;
  border-bottom: 1px solid #eee;
}

.history-item:last-child {
  border-bottom: none;
}

.item-details {
  display: flex;
  justify-content: space-between;
  font-size: 16px;
  font-weight: bold;
}

.item-name {
  color: #333;
}

.item-price {
  color: #888;
}

.item-quantity {
  margin-top: 5px;
  font-size: 14px;
  color: #666;
}

.no-history {
  color: #999;
  font-style: italic;
  text-align: center;
  margin-top: 20px;
}

.cart-container {
	max-width: 500px;
	margin: auto;
	padding: 20px;
	background-color: #fff;
	border-radius: 8px;
	box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.1);
	font-family: Arial, sans-serif;
}

.cart-header {
	display: flex;
	align-items: center;
	justify-content: space-between;
	margin-bottom: 20px;
}

h4 {
	color: #333;
	font-weight: 600;
	margin: 0;
}

h5 {
	margin-bottom: 10px;
	font-size: 1.1em;
	color: #444;
}

.cart-content {
	display: flex;
	flex-direction: column;
}

.cart-list {
	list-style-type: none;
	padding: 0;
	margin: 0;
}

.cart-item {
	display: flex;
	align-items: center;
	justify-content: space-between;
	padding: 10px;
	border-bottom: 1px solid #ddd;
}

.item-info {
	display: flex;
	flex-direction: column;
	flex-grow: 1;
	margin-right: 10px;
}

.item-name {
	font-weight: bold;
}

.item-price {
	color: #888;
	margin-top: 4px;
}

.quantity-controls {
	display: flex;
	align-items: center;
}

.btn-quantity {
	background-color: #e0e0e0;
	border: none;
	padding: 4px 8px;
	margin: 0 5px;
	border-radius: 4px;
	cursor: pointer;
	font-size: 14px;
}

.btn-quantity:disabled {
	background-color: #ccc;
}

.btn-delete {
	background-color: #e74c3c;
	color: white;
	border: none;
	border-radius: 4px;
	padding: 4px 8px;
	font-size: 12px;
	cursor: pointer;
	transition: background-color 0.3s;
}

.btn-delete:hover {
	background-color: #c0392b;
}

.total-price {
	display: flex;
	justify-content: flex-end;
	font-size: 16px;
	font-weight: bold;
	margin: 15px 0;
}

.cart-buttons {
	display: flex;
	justify-content: space-between;
}

.btn-cancel {
	background-color: #ddd;
	color: #333;
	border: none;
	padding: 8px 16px;
	border-radius: 4px;
	cursor: pointer;
}

.btn-confirm {
	background-color: #3498db;
	color: white;
	border: none;
	padding: 8px 16px;
	border-radius: 4px;
	cursor: pointer;
}

.btn-back {
	background: none;
	border: none;
	color: #3498db;
	cursor: pointer;
	font-size: 20px;
}

.btn-confirm:hover {
	background-color: #2980b9;
}

.btn-cancel:hover {
	background-color: #bbb;
}

.tabs {
  display: flex;
  justify-content: center;
  margin-bottom: 20px;
}

.tabs button {
  padding: 10px 20px;
  border: none;
  background-color: #f1f1f1;
  cursor: pointer;
  font-size: 16px;
  margin: 0 5px;
}

.tabs button.active {
  background-color: #007bff;
  color: white;
}

.cart-content,
.history-content {
  padding: 20px;
  border: 1px solid #ddd;
  border-radius: 8px;
  background-color: #fff;
}

.history-item {
  margin-bottom: 15px;
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 8px;
  background-color: #f9f9f9;
}

</style>
<script setup>
import { ref, onMounted } from "vue";
import { useRoute } from "vue-router";
import axiosInstance from "../config/axios.js";
import { useToast } from "vue-toastification";

const route = useRoute();
const idTable = ref(route.params.idTable); // Lấy idTable từ route params
const cartItems = ref([]); // Danh sách món ăn trong giỏ hàng

const toast = useToast();
const orderHistory = ref([]);
const activeTab = ref("cart");
const key = sessionStorage.getItem('key');

// Hàm lấy dữ liệu món ăn từ API theo ID
async function getFoodById(id) {
	try {
		const response = await axiosInstance.get("/food/" + id);
		return response.data;
	} catch (error) {
		console.error("Error fetching food data:", error);
	}
}

// Hàm lấy giỏ hàng và dữ liệu chi tiết của từng món ăn
async function getCartItems() {
	const cart = JSON.parse(sessionStorage.getItem("cart")) || {};

	const items = await Promise.all(
		Object.entries(cart).map(async ([id, quantity]) => {
			console.log(id);
			const food = await getFoodById(id);
			return {
				id: parseInt(id),
				name: food.name,
				price: food.price,
				quantity,
			};
		})
	);

	cartItems.value = items;
}

function loadHistory() {
	const history = JSON.parse(sessionStorage.getItem("history"));
	console.log(history);
	orderHistory.value = history;

	activeTab.value = 'history';
}

// Hàm tăng số lượng
function increaseQuantity(item) {
	item.quantity += 1;
	updateCartItem(item);
}

// Hàm giảm số lượng
function decreaseQuantity(item) {
	if (item.quantity > 1) {
		item.quantity -= 1;
		updateCartItem(item);
	}
}

// Hàm cập nhật giỏ hàng trong sessionStorage
function updateCartItem(item) {
	const cart = JSON.parse(sessionStorage.getItem("cart")) || {};
	cart[item.id] = item.quantity;
	sessionStorage.setItem("cart", JSON.stringify(cart));
}

// Hàm xóa món khỏi giỏ hàng
function removeItem(id) {
	cartItems.value = cartItems.value.filter((item) => item.id !== id);
	const cart = JSON.parse(sessionStorage.getItem("cart")) || {};
	delete cart[id];
	sessionStorage.setItem("cart", JSON.stringify(cart));
}

async function submitOrder() {
	// Lấy dữ liệu giỏ hàng từ sessionStorage
	const cart = JSON.parse(sessionStorage.getItem("cart")) || {};

	// Chuẩn bị dữ liệu OrderDTO
	const orderDTO = {
		tableId: route.params.idTable, // ID của bàn (có thể lấy từ giao diện)
		status: false, // Đặt mặc định là chưa xử lý
		totalPrice: 0, // Sẽ tính lại từ giỏ hàng
		statusPay: false, // Đặt mặc định là chưa thanh toán
		listOrderDetail: [], // Danh sách chi tiết món ăn
	};

	// Xử lý chi tiết giỏ hàng
	const items = await Promise.all(
		Object.entries(cart).map(async ([id, quantity]) => {
			const food = await getFoodById(id);
			const totalItemPrice = food.price * quantity;

			// Tính tổng giá của đơn hàng
			orderDTO.totalPrice += totalItemPrice;

			// Thêm chi tiết món ăn vào danh sách
			return {
				FoodId: parseInt(id),
				name: food.name,
				price: food.price,
				quantity,
			};
		})
	);

	// Thêm listOrderDetail vào orderDTO
	orderDTO.listOrderDetail = items;
	// Gửi đơn hàng lên server
	try {
		const response = await axiosInstance.post("/order", orderDTO);
		if (response.status === 200) {
			orderHistory.value = cartItems.value;

			console.log("Đơn hàng đã được gửi thành công!");

			sessionStorage.setItem("history", JSON.stringify(orderDTO.listOrderDetail));

			sessionStorage.removeItem("cart");

			// Cập nhật giao diện hoặc điều hướng nếu cần
			this.toast.success("Đặt món thành công !", {
				position: "bottom-left",
				timeout: 1500,
				closeOnClick: true,
				pauseOnFocusLoss: true,
				pauseOnHover: true,
				draggable: true,
				draggablePercent: 0.42,
				showCloseButtonOnHover: false,
				hideProgressBar: true,
				closeButton: "button",
				icon: true,
				rtl: false,
			});
		}
	} catch (error) {
		console.error("Lỗi khi gửi đơn hàng:", error);
	}
};

function formatPrice(price) {
      // Định dạng giá tiền theo VND
      return price.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
};


// Khởi tạo dữ liệu khi component được mounted
onMounted(async () => {
	await getCartItems();
});
</script>
