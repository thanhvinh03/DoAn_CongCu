<script setup lang="js">
import { ref, onMounted, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import axiosInstance from '../config/axios.js';
import { useToast } from "vue-toastification";
import BestSellingFoods from "./BestSellingFoods.vue";
const route = useRoute();
const idTable = ref(route.params['idTable']);
const key = ref(route.params['key']);

const listFood = ref([]);
const listCategory = ref([]);
const selectedCategory = ref(null);
const searchTerm = ref('');
const toast = useToast();

// Hàm lấy danh sách món ăn và danh mục từ API
onMounted(async () => {
  try {
	const checkKey = await axiosInstance.get(`/qrcode/check/${key.value}`);
	console.log(checkKey);
	if (checkKey.data.accept) {
		sessionStorage.setItem('keyOrder', key.value)
		const response = await axiosInstance.get('/food');
    	const categoryRes = await axiosInstance.get('/FoodCategory');
    	listFood.value = response.data.$values;
    	listCategory.value = categoryRes.data.$values;
	}
  } catch (error) {
    console.error("Error fetching food data:", error);
	window.location = "/access-denied"
	toast.error("Không thể truy cập vào bàn! ", {
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
			rtl: false
		});
  }
});

// Hàm xử lý khi thêm vào giỏ hàng
function addToCart(idFood) {
  let cart = JSON.parse(sessionStorage.getItem('cart')) || {};

  if (cart[idFood]) {
    cart[idFood] += 1;
  } else {
    cart[idFood] = 1;
  }

  sessionStorage.setItem('cart', JSON.stringify(cart));
  this.toast.success("Thêm vào giỏ hàng thành công! ", {
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
    rtl: false
  });
}

// Hàm chọn danh mục
async function selectCategory(categoryId) {
  let response;
  selectedCategory.value = categoryId;
    if(selectedCategory.value == null) {
       response = await axiosInstance.get('/food');
    }
    else {
      response = await axiosInstance.get('/food/category/'+selectedCategory.value);
    }

    listFood.value = response.data.$values;
}

let timer;
// Hàm tìm kiếm món ăn
async function searchFood() {
  clearTimeout(timer);
  return timer = setTimeout(async () => {
  let response;
    if(selectedCategory.value == null) {
       response = await axiosInstance.get('/food');
    }
    else {
      response = await axiosInstance.get('/food/category/'+selectedCategory.value);
    }

    listFood.value = response.data.$values;
    if (searchTerm.value != '') {
      listFood.value = listFood.value.filter((e) => e.name.includes(searchTerm.value));
    }
  }, 2000);

}

function formatPrice(price) {
      // Định dạng giá tiền theo VND
      return price.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
};

</script>

<template>
	<div class="container">
		<h4 class="store-title">Cửa hàng Food Store</h4>
		<h5 class="table">Bàn: {{ idTable }}</h5>
		<div class="order--wrapper">
			<div class="order--header">
				<div class="search">
					<input
						type="text"
						class="form-control"
						placeholder="Tìm món ăn"
						@input="searchFood()"
						v-model="searchTerm" />
					<button class="btn btn-danger" v-on:click="searchFood()">
						<i class="fa fa-search" aria-hidden="true"></i>
					</button>
				</div>
				<router-link :to="`/cart/${idTable}`">
					<button class="btn btn-sm btn-primary">
						<i class="fa-solid fa-cart-shopping"></i>
					</button>
				</router-link>
			</div>
			<div class="order--main">
				<BestSellingFoods />
				<h5>Loại món ăn</h5>
				<div class="list--category">
					<span
						@click="selectCategory(null)"
						class="category badge-sm"
						:class="{
							'active-category': selectedCategory === null,
						}">
						Tất cả
					</span>
					<span
						v-for="category in listCategory"
						:key="category.id"
						@click="selectCategory(category.id)"
						class="category badge-sm"
						:class="{
							'active-category': category.id === selectedCategory,
						}">
						{{ category.name }}
					</span>
				</div>
				<div class="list--food">
					<div v-for="food in listFood" :key="food.id" class="food">
						<img
							:src="`https://localhost:7093/${food.image}`"
							alt="Food Image"
							class="food-image" />
						<h5>{{ food.name }}</h5>
						<p>{{ formatPrice(food.price) }}</p>
						<button
							class="btn btn-sm btn-primary w-50"
							@click="addToCart(food.id)">
							<i
								class="fa fa-cart-arrow-down"
								aria-hidden="true"></i>
						</button>
					</div>
				</div>
			</div>
		</div>
	</div>
</template>

<style scoped>


.order--wrapper {
	padding: 8px;
}

.order--header {
	display: flex;
	align-items: center;
	justify-content: space-between;
	gap: 24px;
	margin-bottom: 16px;
}

.order--header .search {
	display: flex;
	align-items: center;
	width: 300px;
}

.order--main {
	padding: 8px 4px;
}

.order--main .list--food {
	display: grid;
	grid-template-columns: repeat(2, 1fr);
	gap: 12px;
}

.food {
	border: 1px solid #cecece;
	padding: 12px;
	text-align: center;
	border-radius: 8px;
	box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.food h5 {
	font-size: 1em;
	color: #555;
	padding: 4px 0;
	height: 25px;
}

.food p {
	font-size: 1em;
	color: #555;
	padding-top: 10%;
}

.food button {
	margin-top: 8px;
}

.food-image {
	width: 100px;
	height: 100px;
	object-fit: cover;
	border-radius: 6px;
	margin-bottom: 8px;
}

.list--category {
	display: flex;
	gap: 8px;
	flex-wrap: wrap;
	margin-bottom: 16px;
}

.category {
	cursor: pointer;
	padding: 8px 12px;
	font-size: 12px;
	background-color: #cecece;
	color: black;
	font-size: 100%;
	border-radius: 10px;
	transition: background-color 0.3s;
}

.category:hover {
	background-color: #1cdf4a;
}

.active-category {
	background-color: #1cdf4a;
	color: white;
}
.table {
	text-align: center;
	font-family: "Courier New", Courier, monospace;
	background-color: #fd5b7c;
	position: relative;
	border-radius: 8px; /* Bo tròn góc */
	width: 50%; /* Adjust the width as needed */
	margin: auto; /* Center the table */
	padding: 10px; /* Adjust padding for a smaller look */
}

.store-title {
	text-align: center;
	padding: 16px;
	font-size: 150%;
	font-family: "Courier New", Courier, monospace; /* Phông chữ nổi bật hơn */
	color: #fff;
	background-color: #28a745; /* Màu nền nổi bật */
	text-transform: uppercase; /* Chuyển sang chữ hoa */
	border-radius: 8px; /* Bo tròn góc */
	box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.3); /* Đổ bóng để tạo hiệu ứng nổi */
	margin-bottom: 20px; /* Khoảng cách phía dưới */
	letter-spacing: 2px; /* Khoảng cách giữa các ký tự */
	position: relative;
}

.store-title::before {
	content: "";
	position: absolute;
	top: 100%;
	left: 50%;
	transform: translateX(-50%);
	width: 80%;
	height: 4px;
	background-color: #d4081c;
	border-radius: 2px;
	margin-top: 10px;
	box-shadow: 0px 2px 6px rgba(0, 0, 0, 0.2); /* Đổ bóng cho đường kẻ */
}

/* Responsive adjustments */
@media (min-width: 768px) {
	.order--main .list--food {
		grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
	}
}
</style>
